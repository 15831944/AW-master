//////////////////////////////////////////////////////////////////////////////
//																			//
//    ScanApp.cpp															//
//    ===========															//
//																			//
//	Changes:																// 
//		Problem 1392	Chris Drew		21/05/2015	AuditWizard 8.4.3		// 
//		Correct problem with FTP transfer of files failing with a 12005		//
//		error code.  Fix is to putthe FTP connection in passive mode		//
//		but not really sure why this works other than it maybe an issue		//
//		with a firewall blocking the transfer port							//
//																			//
//==========================================================================//
#include "stdafx.h"

#include <iostream>
#include <fstream>
#include <cstring>

#include "ScanApp.h"
#include "../AuditScannerCommon/SystemBiosScanner.h"
#include "../AuditScannerCommon/NetworkInformationScanner.h"
#include "../AuditScannerCommon/NetworkAdaptersScanner.h"
#include "../AuditScannerCommon/AssetType.h"

#include "../AuditScannerCommon/CkSFtp.h"
#include "../AuditScannerCommon/CkSFtpFile.h"
#include "../AuditScannerCommon/CkSFtpDir.h"
#include "../AuditScannerCommon/CkCrypt2.h"

const char* gszErrorMsgTable[] = 
{
	"No Error",											// 0 = no error
	"Cannot open Configuration File 'AuditAgent.xml'",	// 1 = Failed to read settings
	"Cannot access the specified audit data folder",	// 2 = Cannot access audit data path
};

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define SCANNER_VERSION	"8.4.4.0"

#define DEPLOY_MANUAL	0
#define DEPLOY_NETWORK	1
#define DEPLOY_EMAIL	2
#define DEPLOY_WEB		3
// characters not permissible in a file name
static unsigned char szIllegalChars[] = "\"*/:<>?\\|üìÄÅÉØ×";

//#define AUDITWIZARD_REGKEY "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8"
#define SCANNER_INI	"AuditAgent.xml"
#define FTP_TIMEOUT 10000				// Time to wait for an FTP Connection in milliseconds

#define AUDITWIZARD_REGKEY "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8"

IMPLEMENT_DYNAMIC(CScanApp,CWinApp)

CScanApp::CScanApp ()
{
	m_pInternet = NULL;

	// Create the scanners 
	CreateScanners();

	// connect the WMI scanner if possible
	//m_pWmiScanner->Connect();

	// Initialize any strings
	m_strDomain = "";
	m_strUniqueID = "";
	m_strCat = "";	
	m_strMake = "";	
	m_strModel = "";	
	m_strSerial = "";
	m_strMacAddr = "";
	m_strIPAddr = "";
	m_iChassisType=-1;
	
	// We create the FileSystem scanner if we need it
	_pFileSystemScanner = NULL;

}

CScanApp::~CScanApp ()
{
	if (_pSoftwareScanner != NULL)
		delete _pSoftwareScanner;
	//
	if (_pOperatingSystem != NULL)
		delete _pOperatingSystem;
	//
	if (_pHardwareScanner != NULL)
		delete _pHardwareScanner;
	//
	if (_pFileSystemScanner != NULL)
		delete _pFileSystemScanner;
	//
	if (_pRegistryScanner != NULL)
		delete _pRegistryScanner;

	// Free the WMI and registry scanner objects
	if (m_pWmiScanner != NULL)
	{
		if (m_pWmiScanner->IsConnected())
			m_pWmiScanner->Disconnect();
		delete m_pWmiScanner;
		m_pWmiScanner = NULL;
	}
}

//
//    Initialise
//    ==========
//
//    This is the main initialization routine called from the ServiceApp class to perform once-only
//    initialization of the scanner service.
//
int CScanApp::Initialise ()
{
	CLogFile log;
	int nReturn = 0;

	log.Format("Version 8.4.4.0 \n");
	log.Format ("Performing initialisation of AuditAgent...\n");

	m_pWmiScanner->Connect();

	// get the current module path
	char szBuffer[_MAX_PATH], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
	GetModuleFileName (m_hInstance, szBuffer, sizeof(szBuffer));
	_splitpath (szBuffer, szDrive, szDir, NULL, NULL);
	m_strExePath.Format ("%s%s", szDrive, szDir);

	// load scanner configuration
	nReturn = RefreshAgentConfiguration();
	if (nReturn != 0)
		return nReturn;

	// get the asset name
	char szBuffer1[MAX_COMPUTERNAME_LENGTH + 1];
	CString		strBuffer;
	CString	netBiosComputerName("");

	DWORD dwLen = sizeof(szBuffer1);
	::GetComputerName(szBuffer1, &dwLen);
	netBiosComputerName = szBuffer1;
	m_strAssetName = netBiosComputerName;

	if (m_strAssetName == "")
	{
		// need an asset to continue
		log.Write("unable to find an asset name, exiting scanner.", true);
		return false;
	}
	else
	{
		log.Format("found asset name : %s \n", m_strAssetName);
	}

	// Read the asset cache (formerly the station file)
	//ReadAssetCacheFile();

	// If we have not found an asset type then see if we can deduce one
	if (m_strCat == "" || m_strCat == "PC")
	{
		CAssetType assetType;
		m_strCat = assetType.GetAssetType();
	}
	
	// return status
	return nReturn;
}


void CScanApp::CreateScanners()
{
	// Create a scanner which will audit the software installed on this PC
	if (_pSoftwareScanner != NULL)
		delete _pSoftwareScanner;
	_pSoftwareScanner = new CSoftwareScanner();

	// Create a scanner which will audit the OS installed on this PC
	if (_pOperatingSystem != NULL)
		delete _pOperatingSystem;
	_pOperatingSystem = new COperatingSystemScanner();

	// Create a scanner which will audit the hardware on this PC
	if (_pHardwareScanner != NULL)
		delete _pHardwareScanner;
	_pHardwareScanner = new CHardwareScanner();

	// Create a scanner which will audit the registry keys on this PC
	if (_pRegistryScanner != NULL)
		delete _pRegistryScanner;
	_pRegistryScanner = new CRegistryScanner();
		
	// Create the WMI and Registry scanner objects
	if (m_pWmiScanner != NULL)
		delete m_pWmiScanner;
	m_pWmiScanner = new CWMIScanner;
}


//
//    RefreshAgentConfiguration
//    =========================
//
//    Called periodically to refresh the agent configuration file if that read is out-of-date
//
int CScanApp::RefreshAgentConfiguration()
{
	CLogFile log;

	if (CheckAgentConfiguration())
	{
		if (!ReadAgentConfiguration())
		{
			log.Format("Fatal Error - could not read AuditWizard Agent configuration file\n");
			return 1;
		}
		else
		{
			// validate data path
			if (!ValidateDataPath())
			{
				log.Format("Fatal Error - Invalid audit data path [%s]", _scannerConfiguration.DeployPathData());
				return 2;
			}
		}
	}

	return 0;
}

//
//    CheckAgentConfiguration
//    =======================
//
//    Check whether the audit scanner configuration settings need loading 
//
BOOL CScanApp::CheckAgentConfiguration()
{
	CLogFile log;

	// build the file name from current path
	char szBuffer[_MAX_FNAME], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
	GetModuleFileName (m_hInstance, szBuffer, sizeof(szBuffer));
	_splitpath (szBuffer, szDrive, szDir, NULL, NULL);
	wsprintf (szBuffer, "%s%s%s", szDrive, szDir ,SCANNER_INI);

	// read file datestamp
	try
	{
		CFileStatus fs;
		CFile::GetStatus (szBuffer, fs);
		if (fs.m_mtime > _dateConfigurationFile)
		{
			log.Format("The AuditAgent Configuration file needs to be re-loaded as it has been updated\n");
			_dateConfigurationFile = fs.m_mtime;
			return TRUE;
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::CheckAgentConfiguration");
	}

	return FALSE;
}


//
//    ReadAgentConfiguration
//    ======================
//
//    Read the Audit Agent Configuration File
//
BOOL CScanApp::ReadAgentConfiguration ()
{
	CLogFile log;
	log.Write("Reading the Audit Scanner Configuration File as it has either not been read or is out-of-date\n");

	// build the file name from current path
	try
	{
		char szBuffer[_MAX_FNAME], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
		GetModuleFileName (m_hInstance, szBuffer, sizeof(szBuffer));
		_splitpath (szBuffer, szDrive, szDir, NULL, NULL);
		wsprintf (szBuffer, "%s%s%s", szDrive, szDir ,SCANNER_INI);

		// Determine the name of the scanner initialization file and load it
		CString strFileName = szBuffer;
		if (_scannerConfiguration.Load(strFileName) != 0)
		{
			log.Format("Fatal Error - Cannot open the Audit Agent Configuration File [%s] or it is invalid\n", strFileName);
			return FALSE;
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::ReadAgentConfiguration");
		return FALSE;
	}

	return TRUE;
}



//
//    Ensure that the output data path is valid
//
BOOL CScanApp::ValidateDataPath(void)
{
	CLogFile log;
	CFile fileTest;
	CString strTestFileName;

	//HANDLE hToken = NULL;
	//if (LogonUser("NETWORK SERVICE", "NT AUTHORITY", "", LOGON32_LOGON_SERVICE, LOGON32_PROVIDER_DEFAULT, &hToken))
	//{
	//	if (ImpersonateLoggedOnUser(hToken))
	//	{
	//		log.Format("nw success \n");
	//	}
	//}
	//else
	//{
	//	log.Format("nw failure \n");
	//}	

	// File path - try and create a file
	try
	{
		strTestFileName = MakePathName(_scannerConfiguration.DeployPathData(), "123XYZ.ZYX");

		log.Format("Attempting to write to share folder: %s \n", _scannerConfiguration.DeployPathData());

		if (!fileTest.Open(strTestFileName, CFile::modeCreate|CFile::shareDenyNone)) 
		{
			DWORD dwErrorCode = GetLastError();
			CString strMessage;
			strMessage.Format ("Error Initialising Audit Data Path\n\n\'%s\' ", _scannerConfiguration.DeployPathData());
			switch (dwErrorCode)
			{
				case 3:
					strMessage += "is not a valid pathname";
					break;
				case 19:
					strMessage += "is write-protected";
					break;
				case 21:
					strMessage += "- Device is not ready";
					break;
				default:
					{
						LPVOID lpMsgBuffer;
						FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
							NULL,
							dwErrorCode,
							MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
							(LPTSTR)&lpMsgBuffer,
							0,
							NULL);
						strMessage += "\n\n";
						strMessage += (LPCSTR)lpMsgBuffer;
						LocalFree(lpMsgBuffer);
					}
					break;
			}

			// Log the error and return
			log.Write(strMessage);
			return FALSE;
		}

		log.Write("Successfully wrote to share folder. \n");
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::ValidateDataPath");
		return FALSE;
	}

	// file got created ok, so delete it again
	fileTest.Close();	

	try
	{
		CFile::Remove(strTestFileName);
	}
	catch (CFileException * e)
	{
		TRACE("Failed to delete %s", strTestFileName);
		e->Delete();
	}

	// If we are uploading to an FTP location then we need to ensure now that the FTP details are 
	// valid and we can connect OK.
	if (_scannerConfiguration.UploadSetting() == CAuditScannerConfiguration::ftp)
	{
		m_pFTPConnection = ValidateFTP();
		if (m_pFTPConnection == NULL)
			return FALSE;
		m_pFTPConnection->Close();
		delete m_pFTPConnection;
	}

	//RevertToSelf();

	return TRUE;
}

CString CScanApp::DecryptFTPPassword(LPCSTR pszCipherText) 
{
	CkCrypt2 crypt;
	CLogFile log;

	bool success;
	success = crypt.UnlockComponent("LAYTONCrypt_57s4BuUMVIHi");
	if (success != true) {
		printf("Crypt component unlock failed\n");
		return "";
	}

	crypt.put_CryptAlgorithm("aes");
	crypt.put_CipherMode("cbc");
	crypt.put_KeyLength(256);
	crypt.put_PaddingScheme(0);
	crypt.put_EncodingMode("base64");

	CkString ivHex;
	ivHex = "EDD4667D21FFD546FB5E28D520D66637";

	CkString keyHex;
	keyHex = "6981BAEC0A56A19F2573E334F371881A7FBF2CF180C3AB4BE405CAD2784D5606";

	crypt.SetEncodedIV(ivHex,"hex");
	crypt.SetEncodedKey(keyHex,"hex");

    return crypt.decryptStringENC(pszCipherText);
}


//
//    ValidateFTP
//    ===========
//
//    Validate whether or not a connection can be established to the specified FTP site
//
CFtpConnection* CScanApp::ValidateFTP(void)
{
	CLogFile log;
	CFtpConnection* pFTPConnection = NULL;
	INTERNET_PORT nPort;

	// create an internet session
	try
	{
		m_pInternet = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PRECONFIG);

		// Handle the case where the FTP address contains a port number
		nPort = INTERNET_INVALID_PORT_NUMBER;
		CString strFTPSite = _scannerConfiguration.FTPSite();

		int nPos = strFTPSite.Find(':');
		if (nPos != -1)
		{
			nPort = atoi(strFTPSite.Mid(nPos+1));
			strFTPSite = strFTPSite.Left(nPos);
		}
		else
		{
			nPort = _scannerConfiguration.FTPPort();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::ValidateFTP [CreateInternetSession]");
		return FALSE;
	}


	try
	{
		// Set the timeout for the internet session.  If we do not connect in 15 
		// seconds it is likely that we will never do so.
		m_pInternet->SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, FTP_TIMEOUT);


		// Request a connection to the specified FTP Site
		pFTPConnection = m_pInternet->GetFtpConnection(_scannerConfiguration.FTPSite() 
														 ,_scannerConfiguration.FTPUsername()
														 ,DecryptFTPPassword(_scannerConfiguration.FTPPassword())
														 ,nPort);
		if (pFTPConnection == NULL)
		{
			log.Format("Error code %d whilst connecting to FTP Site %s using username [%s] and password [%s]"
					 , GetLastError() 
					 , _scannerConfiguration.FTPSite() 
					 , _scannerConfiguration.FTPUsername() 
					 , _scannerConfiguration.FTPPassword());
		}
		else
		{
			if (!_scannerConfiguration.FTPDefDir().IsEmpty())
			{
				if (!pFTPConnection->SetCurrentDirectory(_scannerConfiguration.FTPDefDir()))
					log.Format("Error code %d whilst setting default directory to %s"
							 , GetLastError() 
							 , _scannerConfiguration.FTPDefDir());
			}
		}
	}	

	catch (CInternetException* pEx)
	{
		log.Format("Error code %d whilst connecting to FTP Site %s using username [%s] and password [%s]"
				 , GetLastError() 
				 , _scannerConfiguration.FTPSite() 
				 , _scannerConfiguration.FTPUsername() 
				 , _scannerConfiguration.FTPPassword());
		pEx->Delete();
	}

	return pFTPConnection;
}

//
//    ValidateFTPBackup
//    ===========
//
//    Validate whether or not a connection can be established to the specified FTP site
//
CFtpConnection* CScanApp::ValidateFTPBackup(void)
{
	CLogFile log;
	CFtpConnection* pFTPConnection = NULL;
	INTERNET_PORT nPort;

	// create an internet session
	try
	{
		m_pInternet = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PRECONFIG);

		// Handle the case where the FTP address contains a port number
		nPort = INTERNET_INVALID_PORT_NUMBER;
		CString strFTPSite = _scannerConfiguration.FTPSiteBackup();

		int nPos = strFTPSite.Find(':');
		if (nPos != -1)
		{
			nPort = atoi(strFTPSite.Mid(nPos+1));
			strFTPSite = strFTPSite.Left(nPos);
		}
		else
		{
			nPort = _scannerConfiguration.FTPPortBackup();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::ValidateFTP [CreateInternetSession]");
		return FALSE;
	}


	try
	{
		// Set the timeout for the internet session.  If we do not connect in 15 
		// seconds it is likely that we will never do so.
		m_pInternet->SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, FTP_TIMEOUT);

		// Request a connection to the specified FTP Site
		//
		// Problem 1392 - the last parameter puts the connection in Passive mode which seems to fix an issue with the later PutFile 
		// failing - presumably this is down to a firewall / security setting but if false this does not work.
		pFTPConnection = m_pInternet->GetFtpConnection(_scannerConfiguration.FTPSiteBackup() 
														 ,_scannerConfiguration.FTPUsernameBackup()
														 ,DecryptFTPPassword(_scannerConfiguration.FTPPasswordBackup())
														 ,nPort
														 ,true);						
		if (pFTPConnection == NULL)
		{
			log.Format("Error code %d whilst connecting to FTP Site %s using username [%s] and password [%s]"
					 , GetLastError() 
					 , _scannerConfiguration.FTPSiteBackup() 
					 , _scannerConfiguration.FTPUsernameBackup() 
					 , _scannerConfiguration.FTPPasswordBackup());
		}
		else
		{
			if (!_scannerConfiguration.FTPDefDirBackup().IsEmpty())
			{
				if (!pFTPConnection->SetCurrentDirectory(_scannerConfiguration.FTPDefDirBackup()))
					log.Format("Error code %d whilst setting default directory to %s"
							 , GetLastError() 
							 , _scannerConfiguration.FTPDefDirBackup());
			}
		}
	}	

	catch (CInternetException* pEx)
	{
		log.Format("Error code %d whilst connecting to FTP Site %s using username [%s] and password [%s]"
				 , GetLastError() 
				 , _scannerConfiguration.FTPSiteBackup() 
				 , _scannerConfiguration.FTPUsernameBackup() 
				 , _scannerConfiguration.FTPPasswordBackup());
		pEx->Delete();
	}

	return pFTPConnection;
}





//
//    ReadAssetCacheFile
//    ==================
//
//    Read the asset cache file as this maintains details relating to previous audits such as the date of 
//    any previous audit and any user defined data field values.
//
BOOL CScanApp::ReadAssetCacheFile ()
{
	//CLogFile log;
	//// set the file option, then try and load it
	//_pAssetCacheFile = new CAssetCacheFile();
	//_pAssetCacheFile->SetStatus(CAssetCacheFile::cacheFileBoth);
	//BOOL bStatus = _pAssetCacheFile->Load(_scannerConfiguration.DeployPathData(), TRUE);
	//log.Format("The asset cache file was read with a status of [%d]\n" ,bStatus);

	//// Save the date of previous audit (if any)
	//DateOfLastAudit(_pAssetCacheFile->GetLastAuditDate());
	//log.Format("Date of last audit read from the asset cache file was [%s]\n" ,DateTimeToString(_pAssetCacheFile->GetLastAuditDate() ,TRUE));
	//
	//// Read category, make and model, serial, unique id, MAC and IP address from any cache file
	//if (_pAssetCacheFile->GetString("Category") != "")
	//	m_strCat	= _pAssetCacheFile->GetString("Category");
	//if (_pAssetCacheFile->GetString("Domain") != "")
	//	m_strDomain	= _pAssetCacheFile->GetString("Domain");
	//if (_pAssetCacheFile->GetString("Make") != "")
	//	m_strMake	= _pAssetCacheFile->GetString("Make");
	//if (_pAssetCacheFile->GetString("Model") != "")
	//	m_strModel	= _pAssetCacheFile->GetString("Model");
	//if (_pAssetCacheFile->GetString("Serial") != "")
	//	m_strSerial	= _pAssetCacheFile->GetString("Serial");
	//if (_pAssetCacheFile->GetString("UniqueID") != "")
	//	m_strUniqueID = _pAssetCacheFile->GetString("UniqueID");
	//if (_pAssetCacheFile->GetString("MACAddress") != "")
	//	m_strMacAddr = _pAssetCacheFile->GetString("MACAddress");
	//if (_pAssetCacheFile->GetString("IPAddress") != "")
	//	m_strIPAddr = _pAssetCacheFile->GetString("IPAddress");

	//// Rewrite all of the remaining code
	//return bStatus;

	return false;
}


//
//    ScanNonInteractive
//    ==================
//
//    Perform a full non-interactive scan if due - return TRUE if any data was collected
//
int CScanApp::ScanNonInteractive ()
{
	CLogFile log;

	// 8.3.4 - CMD
	// Ensure all old auditing data is cleared before proceeding by re-creating the scanners
	CreateScanners();

	// Connect the WMI scanner at this point
	m_pWmiScanner->Connect();

	// we are scanning so get the latest registry values (if they exist)
	if (!ProcessConfigRegKeys())
	{
		return FALSE;
	}

	// Get PC Identification
	AuditAssetData();

	switch(m_iChassisType)
		{
		case 10:
			//Notebook
			m_strCat="Laptop";
			break;
		case 9:
			//Laptop
			m_strCat="Laptop";
			break;
		default:
			//any other value will be treated as PC 
			if(m_strCat=="Laptop")
			m_strCat="PC";
			break;
		}


	AuditUserDefinedData();

	// Audit the hardware on this system
	ScanHardware();

	// Audit the Operating System and Installed Software
	ScanSoftware();
	
	// Scan the File System
	ScanFileSystem();

	// Scan the registry keys
	ScanRegistryKeys();

	// We need to write the results of this audit to local files in the Windows Temp folder
	// as we can never be sure exactly when we will be able to upload them to the network folder
	// and/or the FTP site
	log.Write("Saving Audit Results to the Temp folder");
	SaveResultsToTemp();
	log.Write(" [COMPLETED]\n");

	// ...and return
	return 0;
}

BOOL CScanApp::ScanHardware()
{
	CLogFile log;

	log.Write("", true);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanHardware() in");

	if (_scannerConfiguration.HardwareScan() == false)
	{
		log.Write("ScanHardware set to false so not scanning.");
		return true;
	}
	
	// Configure the hardware scanner										// 8.3.3.2
	_pHardwareScanner->ScanPhysicalDisks(_scannerConfiguration.HardwarePhysicalDisks());
	_pHardwareScanner->ScanActiveSystem(_scannerConfiguration.HardwareActiveSystem());
	_pHardwareScanner->ScanSecurity(_scannerConfiguration.HardwareSecurity());
	_pHardwareScanner->ScanSettings(_scannerConfiguration.HardwareSettings());

	// ...and call it
	if(!_pHardwareScanner->Scan(m_pWmiScanner))
		return false;

	log.Write("CPageScanning::ScanHardware() out");
	log.Write("", true);

	return true;
}

//
//    ScanSoftware
//    ============
//
//    This function is called to audit the operating System and Installed software
//
BOOL CScanApp::ScanSoftware()
{
	try
	{
		CLogFile log;

		log.Write("", true);
		log.Write("===================================================================================");
		log.Write("CPageScanning::ScanSoftware() in");

		if (!_scannerConfiguration.SoftwareScan())
		{
			log.Write("ScanSoftware set to false so not scanning.");
			return true;
		}

		// JML TODO - change this to allow user choice
		// there is no way for the user to control these settings
		// if we have got here, just set them both to true - to be improved
		_scannerConfiguration.SoftwareScanOs(true);
		_scannerConfiguration.SoftwareScanApplications(true);

		// Operating System
		if (_scannerConfiguration.SoftwareScanOs())
		{
			log.Write("scanning operating system");
			_pOperatingSystem->Scan();
			_installedPatches.Scan(m_pWmiScanner);
		}
		
		// Registered Apps
		if (_scannerConfiguration.SoftwareScanApplications())
		{
			log.Write("scanning software applications");
			_pSoftwareScanner->Scan(&_scannerConfiguration);
		}

		log.Write("CPageScanning::ScanSoftware() out");
		log.Write("", true);
	}
	catch (CException *pEx)
	{
		CLogFile log;
   		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		log.Format("An exception has occured during CPageScanning::ScanSoftware - the exception text was '%s' \n" ,szCause);
   		pEx->Delete();
		return false;
	}

	return true;
}

//
//    ScanFileSystem
//    ===============
//
//    This function is called to audit the File System
//
BOOL	CScanApp::ScanFileSystem()
{	
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanFileSystem() in");

	CString message;

	if (_scannerConfiguration.ScanFileSystem() == false)
	{
		log.Write("ScanFileSystem set to false so not scanning.");
		return true;
	}
	
	// Create a scanner which will audit the File System on this PC
	_pFileSystemScanner = new CFileSystemScanner();

	// Initiate the audit - recover a list of the drives which we need to scan
	_pFileSystemScanner->SetOptions((int)_scannerConfiguration.ScanFolders()
								  , (int)_scannerConfiguration.ScanFiles()
								  , _scannerConfiguration.ListFolders()	
								  , _scannerConfiguration.ListFiles());	


	// ...and scan the file system
	if (!_pFileSystemScanner->Scan(NULL))
		return false;

	log.Write("CPageScanning::ScanFileSystem() out");
	log.Write("", true);

	// ...finally return
	return true;
}

BOOL CScanApp::ScanRegistryKeys()
{
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanRegistryKeys() in");

	CString message;
	
	if (_scannerConfiguration.RegistryScan() == false)
	{
		log.Write("RegistryScan set to false so not scanning.");
		return true;
	}
	
	// Initiate the audit - recover a list of the drives which we need to scan
	_pRegistryScanner = new CRegistryScanner();

	_pRegistryScanner->SetOptions(_scannerConfiguration.ListRegistryKeys());

	// ...and scan the file system
	if (!_pRegistryScanner->Scan())
		return false;

	log.Write("CPageScanning::ScanFileSystem() out");
	log.Write("", true);

	return true;
}

BOOL CScanApp::ProcessConfigRegKeys(void)
{
	CLogFile log;
	HKEY hk;
	DWORD dwDisp;

	if (RegOpenKeyEx( HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, 0, KEY_ALL_ACCESS, &hk ) != ERROR_SUCCESS)
	{
		// key didn't exist so this must be the first time we have audited this PC
		log.Write("No value found in HKEY_LOCAL_MACHINE so assuming this is first time audit for AuditAgent.", true);

		if (RegCreateKeyEx(	HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hk, &dwDisp) != ERROR_SUCCESS)
		{
			log.Format("Error creating registry key '%s'", AUDITWIZARD_REGKEY);
			return false;
		}
		else
		{
			// created the key so now write the values for LastAuditDate
			LPCTSTR szLastAuditDate;

			// get the current date/time to be used in LastAuditDate
			CTime currentTime;
			CString strCurrentTime;
			currentTime = CTime::GetCurrentTime();		

			strCurrentTime += currentTime.Format( "%Y-%m-%d %H:%M:%S" );
			szLastAuditDate = strCurrentTime;

			// write the registry key values
			log.Format("setting LastAuditDate to current date/time - %s \n", szLastAuditDate);
			RegSetValueEx (hk, "LastAuditDate", 0, REG_SZ, (LPBYTE) szLastAuditDate, strlen(szLastAuditDate) + 1); 

			RegCloseKey(hk);

			// no last audit so set this to null
			_dateOfLastAudit = NULL;			 
		}
	}
	else
	{
		// key exists so get the LastAuditDate and UniqueID which were populated during previous audit
		log.Write("Registry settings located, assuming this asset has been audited before.", true);

		LONG lResult;

		char szLastAuditDate[100];
		DWORD ulSize = sizeof(szLastAuditDate);
		lResult = RegQueryValueEx( hk, "LastAuditDate", NULL, NULL, (LPBYTE)szLastAuditDate, &ulSize);

		CString lastAuditDate;
		if (lResult != ERROR_SUCCESS)
		{
			lastAuditDate = "";
		}
		else
		{	
			lastAuditDate = szLastAuditDate;
		}

		log.Format("located LastAuditDate with value - %s \n", lastAuditDate);

		// convert string to date and store this value
		if (!lastAuditDate.IsEmpty())
		{
			LPCSTR p = (LPCSTR)lastAuditDate;
			int nYear	= atoi(p);
			int nMonth	= atoi(p + 5);
			int nDay	= atoi(p + 8);
			int nHour	= atoi(p + 11);
			int nMinute = atoi(p + 14);
			int nSecond = atoi(p + 17);
			_dateOfLastAudit = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
		}

		// now update the LastAuditDate registry value with current time
		// this value will be referenced by future audits
		LPCTSTR szUpdatedLastAuditDate;
		CTime currentTime;
		CString strCurrentTime;

		// get current date/time
		currentTime = CTime::GetCurrentTime();				

		strCurrentTime += currentTime.Format( "%Y-%m-%d %H:%M:%S" );
		szUpdatedLastAuditDate = strCurrentTime;

		// write the registry key values
		log.Format("setting LastAuditDate to current date/time - %s \n", szUpdatedLastAuditDate);
		RegSetValueEx (hk, "LastAuditDate", 0, REG_SZ, (LPBYTE) szUpdatedLastAuditDate, strlen(szUpdatedLastAuditDate) + 1); 

		RegCloseKey(hk);
	}

	return true;
}

//
//    UpdateAssetCacheFile
//    ====================
//
//    Write any changes made during the audit process to the asset cache file and save it
//	  Note that as all audits run from the AuditAgent are run non-interactively the only
//    thing that could possibly change is the name of the asset which has already been 
//    updated in the cache file when it was loaded.  We just need to force it to be written
// 
//    We also update the date of last audit to be now.
//
void CScanApp::UpdateAssetCacheFile ()
{
	// First update basic information fields held in the 'Asset' section
	//_pAssetCacheFile->SetSection ("Asset");
	//_pAssetCacheFile->WriteString ("Asset Name" ,_pAssetCacheFile->GetAssetName());
	//_pAssetCacheFile->WriteString ("NetBIOS Name", _pAssetCacheFile->GetNetBiosName());
	//_pAssetCacheFile->WriteString ("New Name", _pAssetCacheFile->GetNewAssetName());

	//// Save the Date of Audit into the cache file
	//CString strBuffer;
	//CTime now = CTime::GetCurrentTime();
	//strBuffer.Format ("%4.4d-%2.2d-%2.2d %2.2d:%2.2d:%2.2d", now.GetYear(), now.GetMonth(), now.GetDay(),
	//				now.GetHour(), now.GetMinute(), now.GetSecond());
	//_pAssetCacheFile->WriteString("Date", strBuffer);

	//// General asset information
	//_pAssetCacheFile->WriteString ("Category" ,m_strCat);
	//_pAssetCacheFile->WriteString ("Domain" ,m_strDomain);
	//_pAssetCacheFile->WriteString ("Make" ,m_strMake);
	//_pAssetCacheFile->WriteString ("Model" ,m_strModel);
	//_pAssetCacheFile->WriteString ("Serial" ,m_strSerial);
	//_pAssetCacheFile->WriteString ("UniqueID" ,m_strUniqueID);
	//_pAssetCacheFile->WriteString ("MACAddress" ,m_strMacAddr);
	//_pAssetCacheFile->WriteString ("IPAddress" ,m_strIPAddr);

	//// ...then save it
	//_pAssetCacheFile->Save();
}


//
//    SaveResultsToTemp
//    =================
//
//    Save the resultant audit data file to the TEMP folder and return the resultant name
//
void CScanApp::SaveResultsToTemp	(void)
{
	CLogFile log;
	CString baseFileName;
	CString outputFileName;

	try
	{
		// Format a unique name for this audit file in the Windows Temp Folder
		char szTempPath[_MAX_PATH];
		GetTempPath(sizeof(szTempPath), szTempPath);

		// start with temporary folder
		baseFileName.Format("%s\\%s" ,szTempPath, m_strAssetName);
		outputFileName = baseFileName + ".ADF";

		// Now try and open a file of this name
		CFile file;
		int nFileNumber = 0;
		BOOL bStatus = file.Open(outputFileName, CFile::modeRead);
		while (bStatus) 
		{
			file.Close();

			// That file existing so add a numeric suffix (incremented) until we get a unique name
			outputFileName.Format("%s%d.ADF" ,baseFileName ,++nFileNumber);
			bStatus = file.Open(outputFileName, CFile::modeRead);
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::SaveResultsToTemp");
		return;
	}
		
	log.Format("Writing auditing results to the Windows Temp Folder initially as [%s]\n" ,outputFileName);
		
	// Save the results to the CAuditDataFile object
	CAuditDataFile outputAuditFile;
	CString strBuffer;

	// write the scanner and version used
	char szAppName[_MAX_PATH + 1];
	GetModuleFileName(NULL, szAppName, _MAX_PATH);
	CString strVersion = SCANNER_VERSION;

	strBuffer.Format("AuditAgent v%s", strVersion); 
	outputAuditFile.Version(strBuffer);

	// Save data into the 'General' section
	outputAuditFile.Computername(m_strAssetName);	// AuditWizard asset name
	//outputAuditFile.Netbios_name((CString)_pAssetCacheFile->GetNetBiosName());	// NetBIOS asset name
	//outputAuditFile.Newname((CString)_pAssetCacheFile->GetNewAssetName());		// Indicate a change to the name

	// date stamp
	outputAuditFile.AuditDate(CTime::GetCurrentTime());
	
	// Asset data - category / domain / make / model / serial / IP / MAC / Unique ID
	outputAuditFile.Category(m_strCat);
	outputAuditFile.Domain(m_strDomain);
	outputAuditFile.Make(m_strMake);
	outputAuditFile.Model(m_strModel);
	outputAuditFile.Serial_number(m_strSerial);
	outputAuditFile.Uniqueid(m_strUniqueID);
	outputAuditFile.Macaddress(m_strMacAddr);
	outputAuditFile.Ipaddress(m_strIPAddr);
	outputAuditFile.AssetTag(m_strAssetTag);
	log.Format("Creating audit file for Computer [%s] with unique id [%s]\n" ,outputAuditFile.Computername() ,m_strUniqueID);

	// location
	outputAuditFile.Location((CString)"");

	// OS Scanner...
	if (_scannerConfiguration.SoftwareScanOs())
	{
		_pOperatingSystem->Save(outputAuditFile);

		// Installed Patches
		for (DWORD dw = 0; dw<_installedPatches.GetCount(); dw++)
		{
			outputAuditFile.AddInstalledPatch(_installedPatches[dw]);
		}

	}

	outputAuditFile.SetUserDataList(&_scannerConfiguration.UserDataCategories());

	// Software Scanner
	if (_scannerConfiguration.SoftwareScanApplications())
		_pSoftwareScanner->Save(outputAuditFile);

	// Hardware Scanner
	if (_scannerConfiguration.HardwareScan())
		_pHardwareScanner->Save(outputAuditFile);

	// FileSystem Scanner
	if (_pFileSystemScanner != NULL)
		_pFileSystemScanner->Save(&outputAuditFile);

	// Registry Keys Scanner
	if (_pRegistryScanner != NULL)
		_pRegistryScanner->Save(outputAuditFile);

	// Force the file to be written
	outputAuditFile.Write(outputFileName);
}


//
//    UploadResults
//    =============
//
//    Search the Windows Temp folder looking for any audits which may need uploading 
//
int CScanApp::UploadResults (void)
{
	CLogFile log;

	// First things first we need to get a lost of the Audit Data Files which are available to upload
	char szTempPath[_MAX_PATH];
	GetTempPath(sizeof(szTempPath), szTempPath);
	CString baseFileName;
	baseFileName.Format("%s\\%s*.ADF" ,szTempPath, m_strAssetName);

	// Iterate through (any) matching files
	try
	{
		CDynaList<CString> listFilesToDelete;
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile (baseFileName, &fd);
		if (INVALID_HANDLE_VALUE != hFind)
		{
			do
			{
				// Get the matching file name
				CString outputFileName;
				outputFileName.Format("%s\\%s" ,szTempPath ,fd.cFileName);
				log.Format("Upload file with name [%s] has been detected and will be uploaded\n" ,outputFileName);

				// ...and process it
				if (UploadFile(outputFileName))
					listFilesToDelete.Add(outputFileName);
			}
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);

			// Delete any files which we have uploaded
			for (DWORD dw=0; dw<listFilesToDelete.GetCount(); dw++)
			{
				CFile::Remove(listFilesToDelete[dw]);
			}
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::UploadResults");
		return 0;
	}

	return 0;
}



//
//    UploadFile
//    ==========
//
//    Upload the specified audit data file to either or both of the network data folder
//    and the FTP site specified.
//
BOOL CScanApp::UploadFile (CString& tempAuditDataFile)
{
	BOOL bStatus = TRUE;
	CLogFile log;
	
	// Now save the results as required
	if (_scannerConfiguration.UploadSetting() == CAuditScannerConfiguration::ftp)
	{
		// ...then save to the FTP site
		if (_scannerConfiguration.FTPType() == "FTP")
			bStatus = SaveResultsToFtp(tempAuditDataFile);
		else
			bStatus = SaveResultsToSftp(tempAuditDataFile);
	}

	// Otherwise are we saving to the network share?
	else if (_scannerConfiguration.UploadSetting() == CAuditScannerConfiguration::network)
	{
		bStatus = SaveResultsToNetwork(tempAuditDataFile);
	}

	// Otherwise are we saving to Email ?
	else if (_scannerConfiguration.UploadSetting() == CAuditScannerConfiguration::email)
	{
		bStatus = SaveResultsToEmail(tempAuditDataFile);
	}

	// Otherwise are we saving to TCP/IP?
	else if (_scannerConfiguration.UploadSetting() == CAuditScannerConfiguration::tcp)
	{
		bStatus = SaveResultsToTcp(tempAuditDataFile);
	}

	// are we saving adf to central FTP site?
	if (_scannerConfiguration.FTPCopyToNetwork())
	{
		if (_scannerConfiguration.FTPTypeBackup() == "FTP")
			bStatus = SaveResultsToFtpBackup(tempAuditDataFile);
		else
			bStatus = SaveResultsToSftpBackup(tempAuditDataFile);
	}
	
	return bStatus;
}


//
//    SaveResultsToNetwork
//    =============
//
//    Save the resultant Audit files to the specified network folder
//
BOOL CScanApp::SaveResultsToNetwork	(CString& auditDataFileName)
{
	CLogFile log;
	//CString assetName = _pAssetCacheFile->GetAssetName();
	CString assetName = m_strAssetName;
	log.Format("Saving audit data file [%s] to network location [%s]\n" ,auditDataFileName ,_scannerConfiguration.DeployPathData());

	// is there an audit of this name already in the network data folder?
	CString baseFileName;
	baseFileName.Format("%s\\%s" ,_scannerConfiguration.DeployPathData(), assetName);
	CString outputFileName = baseFileName + ".ADF";

	// Now try and open a file of this name
	try
	{
		CFile file;
		int nFileNumber = 0;
		BOOL bStatus = file.Open(outputFileName, CFile::modeRead);
		while (bStatus) 
		{
			file.Close();

			// That file existing so add a numeric suffix (incremented) until we get a unique name
			outputFileName.Format("%s%d.ADF" ,baseFileName ,++nFileNumber);
			bStatus = file.Open(outputFileName, CFile::modeRead);
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CScanApp::SaveResultsToNetwork");
		return false;
	}

	// OK now we have a unique file name - we simply copy the file specified from the temp folder
	// to the network folder and give it the name determined previously
	return CopyFile(auditDataFileName ,outputFileName ,FALSE);
}


//
//    SaveResultsToFtp
//    ================
//
//    Return the results of an audit via FTP
//
BOOL CScanApp::SaveResultsToFtp	(CString& auditDataFileName)
{
	BOOL bStatus = TRUE;
	CLogFile log;
	log.Format("Saving data to an FTP Location\n");
	//CString assetName = _pAssetCacheFile->GetAssetName();
	CString assetName = m_strAssetName;

	// Confirm that we can connect to the FTP site still
	CFtpConnection* pFTPConnection = ValidateFTP();
	if (!pFTPConnection)
	{
		log.Format("The FTP details specified failed validation, data cannot be written\n");
		return FALSE;
	}

	// Build the base file name
	CString strBaseName = assetName;
	CString outputFileName = strBaseName + ".ADF";

	// We need to ensure that this file does not conflict with an existing file
	try
	{
		CFtpFileFind finder(pFTPConnection);
		BOOL bStatus = finder.FindFile(outputFileName);
		int nFileNumber = 1;

		while (bStatus) 
		{
			// yes there is - build a unique name
			outputFileName.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus = finder.FindFile(outputFileName);
		}

		// It seems that we have to close and re-open the FTP connection after the CFtpFileFind
		pFTPConnection->Close();
		pFTPConnection = ValidateFTP();
	
		// ...and transfer the files
		log.Format("Copying Audit Data File %s to the FTP Site as %s\n" ,auditDataFileName ,outputFileName);
		if (!pFTPConnection->PutFile(auditDataFileName ,outputFileName ,FTP_TRANSFER_TYPE_ASCII))
		{
			CString strMessage;
			strMessage.Format("Error code %d whilst uploading audit data file %s to the FTP Site %s\n"
							 , GetLastError() 
							 , outputFileName 
							 , _scannerConfiguration.FTPSite());
			log.Format(strMessage);
			bStatus = FALSE;
		}
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s\n"
						 , GetLastError() 
						 , _scannerConfiguration.FTPSite());
		log.Format(strMessage);
		bStatus = FALSE;
	}	

	pFTPConnection->Close();
	delete pFTPConnection;
	return bStatus;
}

//
//    SaveResultsToFtpBackup
//    ================
//
//    Return the results of an audit via FTP
//
BOOL CScanApp::SaveResultsToFtpBackup (CString& auditDataFileName)
{
	BOOL bStatus = TRUE;
	CLogFile log;
	log.Format("Saving data to an FTP Location\n");
	//CString assetName = _pAssetCacheFile->GetAssetName();
	CString assetName = m_strAssetName;

	// Confirm that we can connect to the FTP site still
	CFtpConnection* pFTPConnection = ValidateFTPBackup();
	if (!pFTPConnection)
	{
		log.Format("The FTP details specified failed validation, data cannot be written\n");
		return FALSE;
	}

	// Build the base file name
	CString strBaseName = assetName;
	CString outputFileName = strBaseName + ".ADF";

	// We need to ensure that this file does not conflict with an existing file
	try
	{
		CFtpFileFind finder(pFTPConnection);
		BOOL bStatus = finder.FindFile(outputFileName);
		int nFileNumber = 1;

		while (bStatus) 
		{
			// yes there is - build a unique name
			outputFileName.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus = finder.FindFile(outputFileName);
		}

		// It seems that we have to close and re-open the FTP connection after the CFtpFileFind
		pFTPConnection->Close();
		pFTPConnection = ValidateFTPBackup();
	
		// ...and transfer the files
		log.Format("Copying Audit Data File %s BACKUP to the FTP Site as %s\n" ,auditDataFileName ,outputFileName);
		if (!pFTPConnection->PutFile(auditDataFileName ,outputFileName ,FTP_TRANSFER_TYPE_ASCII))
		{
			CString strMessage;
			strMessage.Format("Error code %d whilst uploading audit data file %s to the FTP Site %s\n"
							 , GetLastError() 
							 , outputFileName 
							 , _scannerConfiguration.FTPSiteBackup());
			log.Format(strMessage);
			bStatus = FALSE;
		}
		log.Write("SUCCESS\n");
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s\n"
						 , GetLastError() 
						 , _scannerConfiguration.FTPSiteBackup());
		log.Format(strMessage);
		bStatus = FALSE;
	}	

	log.Write("Closing FTP Connection\n");
	pFTPConnection->Close();
	delete pFTPConnection;
	return bStatus;
}



//
//    SaveResultsToSftp
//    ================
//
//    Return the results of an audit via SFTP
//
BOOL CScanApp::SaveResultsToSftp (CString& auditDataFileName)
{
	BOOL bStatus = TRUE;

	CLogFile log;
	log.Format("Saving data to an SFTP Location\n");
	CString assetName = m_strAssetName;
	CkSFtp sftp;

	try
	{
		bool success;
		success = sftp.UnlockComponent("LAYTONSSH_pErKX4Vp3InH");
		if (success != true)
		{
			log.Format("%s\n", sftp.lastErrorText());
			bStatus = FALSE;;
		}

		//  Set some timeouts, in milliseconds:
		sftp.put_ConnectTimeoutMs(10000);
		sftp.put_IdleTimeoutMs(10000);

		//  Connect to the SSH server.
		//  The standard SSH port = 22
		//  The hostname may be a hostname or IP address.
		long port;
		const char * hostname;
		hostname = _scannerConfiguration.FTPSite();
		port = _scannerConfiguration.FTPPort();

		success = sftp.Connect(hostname,port);
		if (success != true) 
		{
			log.Format("%s\n",sftp.lastErrorText());
			bStatus = FALSE;;
		}

		//  Authenticate with the SSH server.  Chilkat SFTP supports
		//  both password-based authenication as well as public-key
		//  authentication.  This example uses password authenication.
		success = sftp.AuthenticatePw(_scannerConfiguration.FTPUsername(), DecryptFTPPassword(_scannerConfiguration.FTPPassword()));
		if (success != true) 
		{
			log.Format("%s\n",sftp.lastErrorText());
			bStatus = FALSE;;
		}

		//  After authenticating, the SFTP subsystem must be initialized:
		success = sftp.InitializeSftp();
		if (success != true) 
		{
			log.Format("%s\n",sftp.lastErrorText());
			bStatus = FALSE;;
		}

		//  Upload from the local file to the SSH server.
		//  Important -- the remote filepath is the 1st argument,
		//  the local filepath is the 2nd argument;

		// Build the base file name
		CString strBaseName = assetName;
		CString strOutputAud = strBaseName + ".ADF";

		BOOL bStatus1 = CheckFileExists(strOutputAud, sftp);
		int nFileNumber = 1;

		while (bStatus1) 
		{
			// yes there is - build a unique name
			strOutputAud.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus1 = CheckFileExists(strOutputAud, sftp);
		}

		const char * remoteFilePath;
		remoteFilePath = strOutputAud;
		const char * localFilePath;
		localFilePath = auditDataFileName;

		success = sftp.UploadFileByName(remoteFilePath,localFilePath);
		
		if (success != true) 
		{
			log.Format("%s\n",sftp.lastErrorText());
			bStatus = FALSE;
		}
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", GetLastError() ,_scannerConfiguration.FTPSite());
		strMessage += "\n";
		log.Format(strMessage);
	}

	return bStatus;
}

//
//    SaveResultsToSftp
//    ================
//
//    Return the results of an audit via FTP
//
BOOL CScanApp::SaveResultsToSftpBackup (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to SFTP site\n");
	CString assetName = m_strAssetName;
	CkSFtp sftp;

	try
	{
		bool success;
		success = sftp.UnlockComponent("LAYTONSSH_pErKX4Vp3InH");
		if (success != true)
		{
			log.Format("Ubale to unlock component: %s\n", sftp.lastErrorText());
			return FALSE;
		}

		//  Set some timeouts, in milliseconds:
		sftp.put_ConnectTimeoutMs(10000);
		sftp.put_IdleTimeoutMs(10000);

		//  Connect to the SSH server.
		//  The standard SSH port = 22
		//  The hostname may be a hostname or IP address.
		long port;
		const char * hostname;
		hostname = _scannerConfiguration.FTPSiteBackup();
		port = _scannerConfiguration.FTPPortBackup();

		success = sftp.Connect(hostname,port);
		if (success != true) 
		{
			log.Format("Unable to connect to SFTP: %s\n",sftp.lastErrorText());
			return FALSE;
		}

		//  Authenticate with the SSH server.  Chilkat SFTP supports
		//  both password-based authenication as well as public-key
		//  authentication.  This example uses password authenication.
		success = sftp.AuthenticatePw(_scannerConfiguration.FTPUsernameBackup(), DecryptFTPPassword(_scannerConfiguration.FTPPasswordBackup()));
		if (success != true) 
		{
			log.Format("Unable to authenticate to SFTP: %s\n",sftp.lastErrorText());
			return FALSE;
		}

		//  After authenticating, the SFTP subsystem must be initialized:
		success = sftp.InitializeSftp();
		if (success != true) 
		{
			log.Format("Unable to initialise SFTP: %s\n",sftp.lastErrorText());
			return FALSE;
		}

		//  Upload from the local file to the SSH server.
		//  Important -- the remote filepath is the 1st argument,
		//  the local filepath is the 2nd argument;

		// Build the base file name
		CString strBaseName = assetName;
		CString strOutputAud = strBaseName + ".ADF";

		BOOL bStatus = CheckFileExists(strOutputAud, sftp);
		int nFileNumber = 1;

		while (bStatus) 
		{
			// yes there is - build a unique name
			strOutputAud.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus = CheckFileExists(strOutputAud, sftp);
		}

		const char * remoteFilePath;
		remoteFilePath = strOutputAud;

		const char * localFilePath;
		localFilePath = auditDataFileName;

		success = sftp.UploadFileByName(remoteFilePath, localFilePath);
		if (success != true) 
		{
			log.Format("Unable to upload file: %s\n", sftp.lastErrorText());
			return FALSE;
		}

		log.Format("Data saved to SFTP site\n");
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", sftp.lastErrorText(), _scannerConfiguration.FTPSiteBackup());
		strMessage += "\n";
		log.Format(strMessage);
	}

	return TRUE;
}




BOOL CScanApp::CheckFileExists(CString& auditDataFileName, CkSFtp& sftp)
{
	CLogFile log;
	bool fileExists = false;

	const char * handle;
    handle = sftp.openDir(".");
    if (handle == 0 ) {
        log.Format("%s\n",sftp.lastErrorText());
        return false;
    }


	CkSFtpDir *dirListing = 0;
    dirListing = sftp.ReadDir(handle);
    if (dirListing == 0 ) {
        log.Format("%s\n",sftp.lastErrorText());
        return false;
    }

    //  Iterate over the files.
    long i;
    long n;
    n = dirListing->get_NumFilesAndDirs();
    if (n == 0) {
        log.Format("No entries found in this directory.\n");
    }
    else {
        for (i = 0; i <= n - 1; i++) {
            CkSFtpFile *fileObj = 0;
            fileObj = dirListing->GetFileObject(i);

            if (fileObj->filename() == auditDataFileName)
			{
				fileExists = true;
				break;
			}

            delete fileObj;
        }

    }

	delete dirListing;

    //  Close the directory
    sftp.CloseHandle(handle);

	return fileExists;
}



//
//    SaveResultsToEmail
//    ==================
//
//    Return the results of an audit via Email
//
BOOL CScanApp::SaveResultsToEmail (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to an Email\n");

	// compose and send an email...
	CString strBuffer;
	CMapiMsg msg;
		
	// recipient is the email address
	msg.m_To.Add(_scannerConfiguration.EmailAddress());
	
	// first attachment is the AUD file
	//CString strAssetName = _pAssetCacheFile->GetAssetName();
	CString strAssetName = m_strAssetName;
	strBuffer.Format ("%s.ADF", strAssetName);
	msg.m_Attachments.Add(auditDataFileName);
	msg.m_AttachmentTitles.Add(strBuffer);
		
	// set the subject and text
	strBuffer.Format("AuditWizard Scan:%s", strAssetName);
	msg.m_strSubject = strBuffer;
	msg.m_strBody = "Message automatically generated by the AuditWizard Scanner";
			
	// send the message
	if (m_mapi.Logon(""))
	{
		// logging off immediately after the send causes huge problems with Outlook Express
		// We now leave the session connected until the app shuts down, logging out via the CMapiSession d'tor
		if (m_mapi.Send(msg))
			log.Write("Scan results emailed to AuditWizard manager\n");
		else
			log.Format("Error code %d whilst sending mail", m_mapi.GetLastError());
	}
	else
	{
		log.Format("Error code %d whilst logging on to MAPI mail client", m_mapi.GetLastError());
	}
		
	return TRUE;
}




//
//    SaveResultsToTcp
//    ================
//
//    Return the results of an audit via TCP/IP
//
BOOL CScanApp::SaveResultsToTcp (CString& auditDataFileName)
{
	CLogFile log;
	CString strServer = _scannerConfiguration.AuditWizardServer();
	log.Format("Saving data to the AuditWizard TCP/IP Server on %s\n" ,strServer);	
	
	// Connect to the AuditWizard TCP Server uif we have not already done so
	if (!m_tcp.IsConnecting())
	{
		if (!m_tcp.Connect(strServer, SOCKET_PORT, &MsgCallback, (LPARAM)this))
		{
			// failed to initialize the named pipes communication
			LPVOID lpMsgBuf;
			FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM
						, NULL
						, GetLastError()
						, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT)
						, (LPTSTR) &lpMsgBuf
						, 0, NULL );
			log.Format("Error connecting to the AuditWizard TCP Server on %s: %s\n", strServer, lpMsgBuf);
			return FALSE;
		}
	}	
	
	// ...and send the audit file to the AuditWizard server		
	if (!m_tcp.SendFile(auditDataFileName))
		log.Format("Failed to save data to the AuditWizard TCP/IP Server on %s\n" ,strServer);

	return TRUE;
}


// TCP Message Callback - we don't expect messages from the server so ignore any we might get
void CScanApp::MsgCallback (CString strMessage, LPARAM lParam)
{
}

//
//    AuditUserDefinedData
//    ====================
//
//    Perform an audit of User Defined Data Fields whose values can be automatically recovered.
//    Currently this applies to fields which are:
//
//		Environment Variables
//		Registry Keys
//
void CScanApp::AuditUserDefinedData	(void)
{
	CLogFile log;

	log.Format("CScan32App::AuditUserDefinedData in \n");	

	// Loop around the User Defined Data Categories
	CUserDataCategoryList& listCategories = _scannerConfiguration.UserDataCategories();

	for (int index = 0; index < listCategories.GetCount(); index++)
	{
		CUserDataCategory* pCategory = &(listCategories.ListCategories())[index];
		
		// Loop through the fields for each category
		for (int fieldIndex = 0; fieldIndex < pCategory->GetCount(); fieldIndex++)
		{
			CUserDataField* pField= &(pCategory->ListDataFields())[fieldIndex];
			
			// Is the field an environment variable, if so recover the value for this variable
			if (pField->DataType() == CUserDataField::typeEnvVar)
			{
				char szBuffer[1024];
				memset(szBuffer, 0, 1024);
				::GetEnvironmentVariable(pField->EnvironmentVariable(), szBuffer, sizeof(szBuffer));
				
				// Store the value (up to the max length)
				if ((pField->Length() != 0) && (pField->Length() < (int)strlen(szBuffer)))
					szBuffer[pField->Length()] = '\0';
				pField->CurrentValue(szBuffer);
			}

			// If the field is set from a registry key then recover the key value now			
			else if (pField->DataType() == CUserDataField::typeRegKey)
			{
				CString strValue = GetRegValue (pField->RegistryKey(), pField->RegistryValue());
				pField->CurrentValue(strValue);
			}
		}
	}

	log.Format("CScan32App::AuditUserDefinedData out \n");
}



//
//    AuditAssetData
//    ==============
//
//    Perform an audit of asset data fields
//
void CScanApp::AuditAssetData	(void)
{
	CLogFile log;
	log.Write("[Auditing System Identification]\n");

	// Recover basic information about this computer and store it locally
	CSystemBiosScanner bios;
	bios.Scan(m_pWmiScanner);
	if (m_strMake == "")
		m_strMake = bios.SystemManufacturer();
	if (m_strModel == "")
		m_strModel = bios.SystemModel();
	if (m_strSerial == "")
		m_strSerial = bios.SystemSerialNumber();
	if (m_strAssetTag == "")
		m_strAssetTag = bios.AssetTag();
	if (m_strUniqueID == "")
		m_strUniqueID = bios.UniqueID();
	if (m_iChassisType == -1)
		m_iChassisType = atoi(bios.ChassisType());

	log.Format("Make: '%s', Model : '%s'\n" ,m_strMake ,m_strModel);

	// Domain/Workgroup
	CNetworkInformationScanner networkInformation;
	networkInformation.Scan(m_pWmiScanner);

	m_strDomain = networkInformation.DomainName();
	log.Format("found domain as %s \n", networkInformation.DomainName());

	m_strIPAddr = networkInformation.IPAddress();
	log.Format("found ip address as %s \n", networkInformation.IPAddress());

	// Get the network adapters as from here we can determine the 'active' MAC address as it will
	// be the NIC with the above IP address
	log.Write("scanning network adapters", true);
	CNetworkAdaptersScanner networkAdapters;
	networkAdapters.Scan(m_pWmiScanner);
	CNetworkAdapter* pPrimaryAdapter = networkAdapters.FindAdapter(m_strIPAddr);

	if (pPrimaryAdapter != NULL)
		m_strMacAddr = pPrimaryAdapter->MacAddress();
	else
		m_strMacAddr = UNKNOWN;

	// Finished
	log.Write("[Auditing System Identification] out\n");
	log.Write("", true);
}


//
//    GetRegValue
//    ===========
//
//    Recover a value from the system registry
//
CString CScanApp::GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem)
{
	CString strResult;

	// work out which "hive" to access
	CString strRegKey(pszRegKey);
	CString strHive = BreakString(strRegKey, '\\');
	HKEY hkHive = NULL, hkSubKey;
	if (strHive == "HKEY_CLASSES_ROOT")
		hkHive = HKEY_CLASSES_ROOT;
	if (strHive == "HKEY_CURRENT_CONFIG")
		hkHive = HKEY_CURRENT_CONFIG;
	if (strHive == "HKEY_CURRENT_USER")
		hkHive = HKEY_CURRENT_USER;
	if (strHive == "HKEY_LOCAL_MACHINE")
		hkHive = HKEY_LOCAL_MACHINE;
	if (strHive == "HKEY_USERS")
		hkHive = HKEY_USERS;

	if (hkHive)
	{
		int nStatus = RegOpenKeyEx(hkHive, strRegKey, 0, KEY_QUERY_VALUE, &hkSubKey);
		if (nStatus == ERROR_SUCCESS)
		{
			// key opened ok - look for matching item
			DWORD dwIndex = 0;
			unsigned char szThisRegValue[1024];
			DWORD dwType;
			DWORD dwRegValueLen = sizeof(szThisRegValue);

			int nStatus = RegQueryValueEx(hkSubKey ,pszRegItem ,NULL ,&dwType ,szThisRegValue ,&dwRegValueLen);
			if (nStatus == ERROR_SUCCESS)
			{
				// FOUND IT - sort out the type conversion
				switch (dwType)
				{
					case REG_BINARY:
						{
							// write as a sequence of hex values
							for (DWORD dw = 0 ; dw < dwRegValueLen ; dw++)
							{
								BYTE b = szThisRegValue[dw];
								CString strThisBit;
								strThisBit.Format("%2.2X ", b);
								strResult += strThisBit;
							}
							strResult.TrimRight();
						}
						break;

					case REG_DWORD:
//					case REG_DWORD_LITTLE_ENDIAN:
						strResult.Format("%d", *((LPDWORD)szThisRegValue));
						break;
						
					case REG_SZ:
						strResult = szThisRegValue;
						break;

					case REG_EXPAND_SZ:
						{
							char szBuffer[1024];
							ExpandEnvironmentStrings ((LPCSTR)szThisRegValue, szBuffer, sizeof(szBuffer));
							strResult = szBuffer;
						}
						break;

					case REG_MULTI_SZ:
						{
							for (char * p = (LPSTR)szThisRegValue ; *p != NULL ; p += strlen(p) + 1)
							{
								if (strResult.GetLength())
									strResult += ';';
								strResult += p;
							}
						}
						break;

					case REG_DWORD_BIG_ENDIAN:
					case REG_LINK:
					case REG_NONE:
//					case REG_QWORD:
//					case REG_QWORD_LITTLE_ENDIAN:
					case REG_RESOURCE_LIST:
					default:
						strResult.Format("Unsupported Registry Data Type %d", dwType);
						break;
				}
			}
		}
		RegCloseKey(hkSubKey);
	
	
	}
	return strResult;
}


LPSTR CScanApp::GetLastErrorText( LPSTR lpszBuf, DWORD dwSize ) 
{
    LPTSTR lpszTemp = 0;

    DWORD dwRet =	::FormatMessage(
						FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM |FORMAT_MESSAGE_ARGUMENT_ARRAY,
						0,
						GetLastError(),
						LANG_NEUTRAL,
						(LPTSTR)&lpszTemp,
						0,
						0
					);

    if( !dwRet || (dwSize < dwRet+14) )
        lpszBuf[0] = TEXT('\0');
    else {
        lpszTemp[_tcsclen(lpszTemp)-2] = TEXT('\0');  //remove cr/nl characters
        _tcscpy(lpszBuf, lpszTemp);
    }

    if( lpszTemp )
        LocalFree(HLOCAL(lpszTemp));

    return lpszBuf;
}


void CScanApp::LogException (CException* pEx, LPCSTR function)
{
	CLogFile log;
   	TCHAR   szCause[255];
   	pEx->GetErrorMessage(szCause, 255);
	log.Format("An exception occured during '%s' - the message text was %s\n" ,function ,szCause);
   	pEx->Delete();
}