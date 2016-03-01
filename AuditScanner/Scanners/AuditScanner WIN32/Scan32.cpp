//////////////////////////////////////////////////////////////////////////////////////////
//																						//
//    Scan32.cpp																		//
//    ==========																		//
//																						//
//--------------------------------------------------------------------------------------//
//																						//
//	History																				//
//		11-OCT-2011		Chris Drew				AW 8.3.3			CMD#001				//
//		Changes to get the project to build under VS 2010								//
//		Change to use HKLM not HKCU for storage of last audit date						//
//																						//
//////////////////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MemLeakDetect.h"
#include "Scan32.h"

#include <fstream>

#include "../AuditScannerCommon/AlertDefinitionsFile.h"
#include "../AuditScannerCommon/AlertTriggerScanner.h"
//
#include "../AuditScannerCommon/SystemBiosScanner.h"
#include "../AuditScannerCommon/NetworkInformationScanner.h"
#include "../AuditScannerCommon/NetworkAdaptersScanner.h"
#include "../AuditScannerCommon/AssetType.h"
#include "../AuditScannerCommon/AlertNotificationFile.h"

#include "../../MfcExt/Include/UpdateFile.h"

#include "../AuditScannerCommon/CkCrypt2.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

// Command line for installing the Mobile Device Scanner(s)
#define MDA_INSTALLER	"lyncreg.exe"
#define MDA_CMD_INSTALL	"-i \"%s\" -yr %s \"%s\""
#define MDA_CMD_REMOVE	"-u"

// Command line for installing and starting the Lync USB Scanner
#define USB_INSTALLER	"lyncusb.exe"
#define USB_CMD_INSTALL	"-i \"%s\" -%s %s \"%s\""
#define USB_CMD_REMOVE	"-u"
#define USB_START_EXE	"net"
#define USB_START_CMD	"start lyncusbserv"

#define SCANNER_INI	"AuditScanner.xml"
#define FTP_TIMEOUT 10000				// Time to wait for an FTP Connection in milliseconds
#define SOCKET_PORT      31730			// TCP Port to connect back to the AuditWizard Server

#define AUDITWIZARD_REGKEY "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8"

/////////////////////////////////////////////////////////////////////////////
// The one and only CScan32App object
CScan32App theApp;

//
// Constructor
// ===========
//
// This is the main constructor for the CScan32App class
//
CScan32App::CScan32App()
{
	m_pLogFile = new CLogFile(LOG_START|LOG_FINISH|LOG_LOC_TEMP);
	m_pWizard = NULL;
	//
	_testMode = FALSE;
	_traceMode = FALSE;
	_scannerConfigurationName = SCANNER_INI;
	//_pAssetCacheFile = NULL;

	// Create the WMI and Registry scanner objects
	m_pWmiScanner = new CWMIScanner;

	// connect the WMI scanner if possible
	//m_pWmiScanner->Connect();

	// Flag to show whether or not we have already logged the fact that no audit is yet required
	_reportAuditNotRequired = false;
	
	// Time to check Alert Monitor triggers
	_timeNextTriggerCheck = 0;	

	_firstTimeAudit = false;
}

CScan32App::~CScan32App()
{
	delete m_pLogFile;

	if (m_pInetSession != NULL)
	{
		m_pInetSession->Close();
		delete m_pInetSession;
	}

	/*if (_pAssetCacheFile != NULL)
		delete _pAssetCacheFile;*/

	// Free the WMI and registry scanner objects
	if (m_pWmiScanner != NULL)
	{
		if (m_pWmiScanner->IsConnected())
			m_pWmiScanner->Disconnect();
		delete m_pWmiScanner;
		m_pWmiScanner = NULL;
	}
}

BEGIN_MESSAGE_MAP(CScan32App, CWinApp)
	//{{AFX_MSG_MAP(CScan32App)
		// NOTE - the ClassWizard will add and remove mapping macros here.
		//    DO NOT EDIT what you see in these blocks of generated code!
	//}}AFX_MSG
	ON_COMMAND(ID_HELP, CWinApp::OnHelp)
END_MESSAGE_MAP()

//
// App Initialisation
//			   
BOOL CScan32App::InitInstance()
{
	try
	{
		CLogFile log;

		// check for multiple instances... 
		CheckActive();

		// save application executable path
		char szBuffer[_MAX_FNAME], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
		GetModuleFileName (m_hInstance, szBuffer, sizeof(szBuffer));
		_splitpath (szBuffer, szDrive, szDir, NULL, NULL);
		_scannerPath.Format("%s%s", szDrive, szDir);

		// interpret any command line parameters...
		ParseCmdLine ();

		// open logfile if necessary
		OpenLogFile(log);

		// Recover and store as a long file name the Windows Temp folder as we use this
		// for tempoary file storage
		SECFileSystem fs;
		char szTmpPath[_MAX_PATH + 1];
		char szLongTmpPath[_MAX_PATH + 1];
		GetTempPath(_MAX_PATH, szTmpPath);

		// Ensure that this is a long path
		GetLongPathName(szTmpPath ,szLongTmpPath ,sizeof(szLongTmpPath));

		// Ensure that the folder exists 
		_tempPath = szLongTmpPath;
		if (!fs.DirectoryExists(_tempPath))
		{
			log.Format("Windows Tmp path did not exist, creating...\n");
			fs.MakePath(_tempPath);
		}

		//
		// This is the main processing loop for the AuditWizard Scanner.
		//
		// In normal (non-AlertMonitor) mode we only execute this loop once.
		//
		// In AlertMonitor mode we continuously execute this loop keeping the
		// scanner active waiting a pre-determined time between successive iterations
		bool firstTime = true;
		do
		{
			// Check to see if we have been requested to closedown and exit if so
			if (CheckCloseDownRequested())
				break;
			
			// Now call the processing function which will actually run the audit if required
			if (!Process(firstTime))
				break;

			// Not first time anymore
			firstTime = false;
			
			// We do not want to loop back to exit from this process too quickly so delay between successive loops (inner)
			if (_scannerConfiguration.AlertMonitorEnabled(GetAssetName()))
			{
				for (int isub = 0; isub < _scannerConfiguration.AlertInterval(); isub++)
				{
					Sleep(1000);
					YieldProcess();
				}
				continue;
			}
			
		} 
		while (_scannerConfiguration.AlertMonitorEnabled(GetAssetName()));

		// Since the dialog has been closed, return FALSE so that we exit the
		// application, rather than start the application's message pump.
		log.Format("shutting down\n");
	}
	catch (CException *pEx)
	{
		CLogFile log;
   		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		log.Format("An exception has occured during CApplicationSerials::InitInstance - the message text was '%s'\n" ,szCause);
   		pEx->Delete();
	}
	return FALSE;
}


//
// 8.3.4 - CMD
//
// Recover the date and time of the last audit carried out
// Note that we may have to look in either HKLM or HKCU for this value as the scanner may not have
// permission to write to the former and will therefore have to use HKCU to store this
//
CTime CScan32App::GetLastAuditDate(void)
{
	CLogFile log;
	CTime lTimeLastAudit_HKCU;
	CTime lTimeLastAudit_HKLM;

	CString lastAuditDate_HKLM = "";
	CString lastAuditDate_HKCU = "";

	// Read the last audit date from both the HKLM and HKCU hives and take the latest.  This should allow us to be certain
	// that we are always getting the actual last date of audit
	lastAuditDate_HKLM = CReg::GetItemString(HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, "LastAuditDate");			// 8.3.4
	lastAuditDate_HKCU = CReg::GetItemString(HKEY_CURRENT_USER, AUDITWIZARD_REGKEY, "LastAuditDate");			// 8.3.4
	
	// Log these values
	log.Format("located LastAuditDate in HKLM with value - %s \n", lastAuditDate_HKLM);
	log.Format("located LastAuditDate in HKCU with value - %s \n", lastAuditDate_HKCU);

	// Convert the values to CTime objects
	// convert string to date and store this value
	if (!lastAuditDate_HKLM.IsEmpty())
	{
		LPCSTR p = (LPCSTR)lastAuditDate_HKLM;
		int nYear	= atoi(p);
		int nMonth	= atoi(p + 5);
		int nDay	= atoi(p + 8);
		int nHour	= atoi(p + 11);
		int nMinute = atoi(p + 14);
		int nSecond = atoi(p + 17);
		lTimeLastAudit_HKLM = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
	}
	//
	if (!lastAuditDate_HKCU.IsEmpty())
	{
		LPCSTR p = (LPCSTR)lastAuditDate_HKCU;
		int nYear	= atoi(p);
		int nMonth	= atoi(p + 5);
		int nDay	= atoi(p + 8);
		int nHour	= atoi(p + 11);
		int nMinute = atoi(p + 14);
		int nSecond = atoi(p + 17);
		lTimeLastAudit_HKCU = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
	}

	// Return which-ever time is the latest
	return (lTimeLastAudit_HKLM > lTimeLastAudit_HKCU) ? lTimeLastAudit_HKLM : lTimeLastAudit_HKCU;
}


//
//    Try and write the date of last audit to the registry - we first try HKEY_LOCAL_MACHINE as this is better than HKEY_LOCAL_USER
//    as auditing the PC is irrespective of the user logged on.  We may however not have sufficient privilege to write here so we may
//    have to resort to the inferior HKEY_LOCAL_USER hive instead.
//
BOOL CScan32App::SetLastAuditDate(void)
{	
	CLogFile log;
	HKEY hk;
	DWORD dwDisp;
	LPCTSTR szLastAuditDate;

	// Get the current time as this is what we use as the date of last audit
	m_strLastAuditDate = CTime::GetCurrentTime().Format( "%Y-%m-%d %H:%M:%S" );

	// We use RegCreateKeyEx so that if the key does not already exist then it is created - we can therefore assume that 
	// if we get an error then the cause is likely to be insufficient privilege.
	if (RegCreateKeyEx(HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hk, &dwDisp) == ERROR_SUCCESS)		// 8.3.3
	{
		szLastAuditDate = m_strLastAuditDate;
		log.Format("Setting HKLM LastAuditDate to current date/time - %s \n", szLastAuditDate);
		RegSetValueEx (hk, "LastAuditDate", 0, REG_SZ, (LPBYTE) szLastAuditDate, strlen(szLastAuditDate) + 1);
		RegCloseKey(hk);
		return true;
	}
	else if (RegCreateKeyEx(HKEY_CURRENT_USER, AUDITWIZARD_REGKEY, 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hk, &dwDisp) == ERROR_SUCCESS)	// 8.3.3
	{
		szLastAuditDate=m_strLastAuditDate;
		log.Format("Setting HKCU LastAuditDate to current date/time - %s \n", szLastAuditDate);
		RegSetValueEx (hk, "LastAuditDate", 0, REG_SZ, (LPBYTE) szLastAuditDate, strlen(szLastAuditDate) + 1);
		RegCloseKey(hk);
		return true;
	}
	return false;
}



//
//    Process
//    =======
//
//    This is the main processing loop of the audit scanner.
//
BOOL CScan32App::Process(bool firstTime)
{
	CLogFile log;

	try
	{
		log.Write("", TRUE);
		log.Write("===================================================================================");
		log.Write("performing initialisation of audit file");
		log.Write("===================================================================================");
		log.Write("starting processing of audit file");

		m_pWmiScanner->Connect();
				
		// load scanner configuration each time around the main processing loop as this ensures that
		// we are always up-to-date with any configuration file changes
		if (CheckConfigFile())
		{
			log.Write("checking the scanner configuration file");
			if (!ReadAuditConfiguration())
			{
				log.Format("Fatal Error - could not read INI file\n");
				AfxMessageBox ("Cannot read Configuration File...Aborting", MB_ICONSTOP);
				return FALSE;
			}
			else
			{
				log.Write("validating data path");
				
				// validate data path
				if (!ValidateDataPath())
				{
					log.Format("Invalid audit data path [%s]", _scannerConfiguration.DeployPathData());
					return FALSE;
				}
				
				log.Write("checking if shutdown requested");
				// is a general shutdown requested?
				if (_scannerConfiguration.Shutdown())
				{
					log.Format("shutdown signalled\n");
					return FALSE;
				}
			}
		}

		// Are we running in AlertMonitor mode - if yes then log this
		if (_scannerConfiguration.IsAlertMonitorEnabled())
		{
			log.Format("This scanner enables AlertMonitor\n");
			CDynaList<CString> listMonitoredAssets = _scannerConfiguration.AlertMonitoredAssets();
			string monitoredAssets = ListToString(listMonitoredAssets ,',');
			log.Format("...Alert Monitored Assets : %s\n",monitoredAssets);
		}

		// get the asset name
		char szBuffer[MAX_COMPUTERNAME_LENGTH + 1];
		CString		strBuffer;
		CString	netBiosComputerName("");

		DWORD dwLen = sizeof(szBuffer);
		::GetComputerName(szBuffer, &dwLen);
		netBiosComputerName = szBuffer;
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

		// Determine if we need the system tray window and if so create it
		CheckSystemTray();

		// If this is the first time round then we should install or uninstall the PDA/USB scanners as required
		if (firstTime)
		{
			if (_scannerConfiguration.ScanMDA())
				log.Write("installing ScanMDA");
				InstallMDA ();
				log.Write("ScanMDA installed");

			if (_scannerConfiguration.ScanUSB())
				log.Write("installing ScanUSB");
				InstallUSB ();
				log.Write("ScanUSB installed");
		}

		// get any specified re-audit interval
		int nReAuditDays = _scannerConfiguration.ReAuditInterval();

		// get last audit date - if any - if we don't have one that assume that this // is the first time around (or a flag for re-audit)
		CTime tmNow = CTime::GetCurrentTime();
		CTime tmLastAudit = GetLastAuditDate();
		log.Format("time of last audit was : %s\n", tmLastAudit.Format("%A, %B %d, %Y at %X"));

		// If we have not audited this PC previously then signal an audit is required
		bool	bAuditNow = false;
		if (tmLastAudit.GetTime() == 0)
		{
			bAuditNow = true;

			// CMD 8.3.6
			// As we have not recovered a date of previous audit, indicate that we have not had an audit before and should run
			// an interactive audit if First Time Interactive is specified.
			_firstTimeAudit = true;
		}

		else if (nReAuditDays == 0)
		{
			// If we have a zero reaudit period then we should do a re-audit IF this is the first time
			// around the processing loop OR it has been 1 day since the last audit	
			if (firstTime)
			{
				bAuditNow = true;
			}
			else
			{
				CTimeSpan elapsedDays = tmNow - tmLastAudit;
				if (elapsedDays.GetDays() >= 1)
					bAuditNow = true;
			}
		}
		
		// We need to check a non-zero re-audit interval to see whether or not it is time to perform a re-audit
		else
		{
			// or has re-audit interval elapsed ?
			CTimeSpan tsElapsed = tmNow - tmLastAudit;

			// get (rounded) number of days
			int nDays = (int)((tsElapsed.GetTotalHours()) / 24);
			bAuditNow = (nDays >= nReAuditDays);
		}
				
		// Audit required or not?
		if (bAuditNow)
		{
			log.Format("An audit is required...\n");

			// If we have a system tray icon then set it to show an active state
			if (_pTrayIconWnd != NULL)
				_pTrayIconWnd->SetActive(FALSE);

			// Audit requested so run it now.
			BOOL bAuditResult=AuditNow(log);

			// If we have a system tray icon then update it to show a not active status
			if (_pTrayIconWnd != NULL)
				_pTrayIconWnd->SetActive(FALSE);

			// Update the audit cache file with any changes made during the audit process
			UpdateAssetCacheFile();	

			// Set the last audit date in registry here
			SetLastAuditDate();
		}
		
		// If AlertMonitor is enabled then we should now check for any alerts being triggered
		if (_scannerConfiguration.AlertMonitorEnabled(GetAssetName()))
			CheckAlertMonitorTriggers();
	}
	catch (CException *pEx)
	{
		throw pEx;;
	}

	// Return success to keep the scanner going
	return TRUE;
}





//
//    CheckSystemTray
//    ===============
//
//    Check to see if we require a system tray icon and create/delete it as appropriate
//
void	CScan32App::CheckSystemTray	(void)
{
	// A system tray icon is only ever required when AlertMonitor functionality has been enabled
	if (!_scannerConfiguration.AlertMonitorEnabled(GetAssetName()))
		return;

	// ...otherwise check the ini file to see if the icon is required
	if (!_scannerConfiguration.AlertTrayIcon())
	{
		// If we have a system tray icon already then we need to remove it as required
		if (_pTrayIconWnd != NULL)
		{
			_pTrayIconWnd->CloseWindow();
			delete _pTrayIconWnd;
			_pTrayIconWnd = NULL;
		}
	}

	// ...otherwise we need the tray icon so should create one if we have not already
	else if (_pTrayIconWnd == NULL)
	{
		CRect Rect(5,5,50,50);
		_pTrayIconWnd = new CScannerTrayWnd;
		_pTrayIconWnd->CreateEx(0, // Make a client edge label.
								_T("STATIC"), 
								"AuditWizard Scanner",
							    0,
								5, 5, 30, 30, NULL, (HMENU)0);
		_pTrayIconWnd->SetActive(FALSE);
	}
	
}


//
//    CheckAlertMonitorTriggers
//    =========================
//
//    Called periodically to check and see if any of the Alert Monitor triggers have been satisfied
//
BOOL	CScan32App::CheckAlertMonitorTriggers	(void)
{
	CString alertMonitorCacheName = _tempPath + "\\AlertMonitor.xml";
	
	// are we due for a check yet?
	if (CTime::GetCurrentTime() < _timeNextTriggerCheck)
		return FALSE;

	// If we have a system tray icon then make it appear active
	if (_pTrayIconWnd != NULL)
		_pTrayIconWnd->SetActive(TRUE);
		
	// Read the Alert Monitor Cache file also as this holds previous values recovered
	// It is actually an Audit Data File or at least part of one
	CAuditDataFile alertCacheFile;
	alertCacheFile.Load(alertMonitorCacheName);

	CString fileName = MakePathName(_scannerConfiguration.DeployPathScanner() , "AuditScanner.xml");
	CAlertDefinitionsFile alertDefinitionsFile;
	alertDefinitionsFile.Load(fileName);
	
	// Now we need to build a special list of audit scanners which we can use to recover the current
	// values for the fields defined within these triggers
	CAlertTriggerScannerList listAlertTriggers;
	listAlertTriggers.Setup(&_scannerConfiguration, m_pWmiScanner, alertDefinitionsFile ,GetAssetName(), GetLastAuditDate());
	
	// Create an internal Alert Notifications File just in case and load in any previous alerts that we may
	// have generated but not yet uploaded.
	CAlertNotificationFile notificationFile;
	notificationFile.Load(_scannerConfiguration.DeployPathData());
	notificationFile.AssetName(GetAssetName());
	
	// Run a scan of the triggers - this will populate them all with their latest values
	if (listAlertTriggers.Scan())
	{
		TRACE("New AlertMonitor trigger values loaded\n");
		
		// We now need to temporarily save the data from ALL of these scanners into a work AuditDataFile
		CAuditDataFile newAlertDataFile;
		newAlertDataFile.Computername(GetAssetName());
		listAlertTriggers.Save(alertCacheFile, newAlertDataFile);
	
		// Now we can call the 'Test' function for each scanner passing (any) cached audit data file and the latest
		// results file so that we can compare values and generate alerts as required.
		int iTriggerCount = listAlertTriggers.Test(notificationFile ,alertCacheFile ,newAlertDataFile);
		if(iTriggerCount>0)
		{
			TRACE("AlertMonitor has fired some alert notifications, saving the notification file\n");
			notificationFile.Write(_scannerConfiguration.DeployPathData());
		}
		
		// Save the latest values back to the Alert Monitor cache file
		TRACE("Saving AlertMonitor cache file to disk\n");
		newAlertDataFile.Write(alertMonitorCacheName);
	}

	// set time up for next trigger check
	CTimeSpan tsInterval(0, 0, 0, _scannerConfiguration.AlertInterval());
	_timeNextTriggerCheck = CTime::GetCurrentTime() + tsInterval;

	// If we are displaying a system tray icon appear inactive now
	if (_pTrayIconWnd != NULL)
		_pTrayIconWnd->SetActive(FALSE);

	// Return success
	return TRUE;
}




//
// Read and store any command line parameters
//
void CScan32App::ParseCmdLine ()
{
	char cToken[256];
	int nTokenLen;
	const char * p = m_lpCmdLine;

	CLogFile log;
	
	log.Format("Command line is %s\n",m_lpCmdLine);

	while (*p)
	{
		char ch = toupper(*p++);
		// ignore spaces and slashes
		if (ch == ' ' || ch == '/')
			continue;
		
		// is it a "T" for trace?
		if (ch == 'T')
			_traceMode = TRUE;
		
		// is it a "N" for "No Alert Monitor"
		if (ch == 'N')
		{
			log.Write("Test Mode has been enabled on the command line\n");
			_testMode = TRUE;
		}

		// is it a 's' for "Settings File"?
		if (ch == 'S')
		{
			// The following text is the ini file name then
			p++;						// Skip following space
			const char *pToken = strchr(p ,' ');
			if (pToken == NULL)			// If no space found continue to end of line
			{
				nTokenLen = strlen(m_lpCmdLine);
				nTokenLen -= (p - m_lpCmdLine);
			}
			else
			{
				nTokenLen = pToken - p;
			}
			
			memcpy(cToken ,p ,nTokenLen);
			cToken[nTokenLen] = '\0';
			_scannerConfigurationName = cToken;

			p += nTokenLen;
		}
	}
}


//
// Check whether the audit scanner configuration settings need loading 
//
BOOL CScan32App::CheckConfigFile()
{
	// build the file name from current path
	char szBuffer[_MAX_FNAME], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
	GetModuleFileName (m_hInstance, szBuffer, sizeof(szBuffer));
	_splitpath (szBuffer, szDrive, szDir, NULL, NULL);
	wsprintf (szBuffer, "%s%sAuditScanner.xml", szDrive, szDir);

	// read file datestamp
	CFileStatus fs;
	CFile::GetStatus (szBuffer, fs);
	if (fs.m_mtime > m_timCfgFile)
	{
		m_timCfgFile = fs.m_mtime;
		return TRUE;
	}
	return FALSE;
}


//
//    ReadAuditConfiguration
//    ======================
//
//    Read the Audit Scanner Configuration File
//
BOOL CScan32App::ReadAuditConfiguration ()
{
	// Determine the name of the scanner initialization file and load it
	CString strFileName = _scannerPath + _scannerConfigurationName;
	try
	{
		_scannerConfiguration.Load(strFileName);
	}
	catch (CException *pEx)
	{
		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		CString strMessage;
		strMessage.Format("Cannot open the Audit Scanner Configuration File\n%s\nError was %s", strFileName, szCause);
		AfxMessageBox (strMessage, MB_ICONEXCLAMATION);
		return FALSE;
	}

	// Assume both cache files
	_assetCacheFileScope = CAssetCacheFile::cacheFileBoth;

	return TRUE;
}

/*
** Ensure that the output data path is valid
*/
BOOL CScan32App::ValidateDataPath()
{
	CLogFile log;

	// an empty path is a special case - use Windows Temporary Folder
	if (_scannerConfiguration.DeployPathData().IsEmpty())
		_scannerConfiguration.DeployPathData(_tempPath);

	// Check network path if we are saving to the network
	if (GetCFG().UploadSetting() == CAuditScannerConfiguration::network)
	{	
		// File path - try and create a file
		CString strTestFileName = MakePathName(_scannerConfiguration.DeployPathData(), "123XYZ.ZYX");
		CFile fileTest;
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
			log.Write(strMessage);
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
	}

	// If we are uploading to an FTP location then we need to ensure now that the FTP details are 
	// valid and we can connect OK.
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::ftp)
	{
		m_pFTPConnection = ValidateFTP();
		if (m_pFTPConnection == NULL)
			return FALSE;
		m_pFTPConnection->Close();
		delete m_pFTPConnection;
	}

	// If we are uploading via Email then ensure that we can initialize the email system
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::email)
	{
		if (!m_mapi.Installed())
		{
			log.Write("AuditWizard scanner is configured to transmit results by email,\nbut no MAPI email connection can be found");
			return FALSE;
		}	
	}
	
	// If we are uploading via the TCP service then ensure that we can connect to the server
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::tcp)
	{
		// Leave this until we write the results
	}	
	
	return TRUE;
}

CString CScan32App::DecryptFTPPassword(LPCSTR pszCipherText) 
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
CFtpConnection* CScan32App::ValidateFTP(void)
{
	CLogFile log;
	CFtpConnection* pFTPConnection = NULL;
	

	// create an internet session
	m_pInetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PRECONFIG);

	// Handle the case where the FTP address contains a port number
	INTERNET_PORT nPort = INTERNET_INVALID_PORT_NUMBER;
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
	
	try
	{
		// Set the timeout for the internet session.  If we do not connect in 15 
		// seconds it is likely that we will never do so.
		m_pInetSession->SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, FTP_TIMEOUT);

		// nPort = 21;
		// Request a connection to the specified FTP Site
		pFTPConnection = m_pInetSession->GetFtpConnection(_scannerConfiguration.FTPSite() 
														 ,_scannerConfiguration.FTPUsername()
														 ,DecryptFTPPassword(_scannerConfiguration.FTPPassword())
														 ,nPort);
		if (pFTPConnection == NULL)
		{
			log.Format("Error code %d whilst connecting to FTP Site %s using username [%s]"
					 , GetLastError() 
					 , _scannerConfiguration.FTPSite() 
					 , _scannerConfiguration.FTPUsername());
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
CFtpConnection* CScan32App::ValidateFTPBackup(void)
{
	CLogFile log;
	CFtpConnection* pFTPConnection = NULL;

	// create an internet session
	m_pInetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PRECONFIG);

	// Handle the case where the FTP address contains a port number
	INTERNET_PORT nPort = INTERNET_INVALID_PORT_NUMBER;
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
	
	try
	{
		// Set the timeout for the internet session.  If we do not connect in 15 
		// seconds it is likely that we will never do so.
		m_pInetSession->SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, FTP_TIMEOUT);

		//nPort = 21;
		// Request a connection to the specified FTP Site
		pFTPConnection = m_pInetSession->GetFtpConnection(_scannerConfiguration.FTPSiteBackup() 
														 ,_scannerConfiguration.FTPUsernameBackup()
														 ,DecryptFTPPassword(_scannerConfiguration.FTPPasswordBackup())
														 ,_scannerConfiguration.FTPPortBackup());
		if (pFTPConnection == NULL)
		{
			log.Format("Error code %d whilst connecting to FTP Site %s using username [%s]"
					 , GetLastError() 
					 , _scannerConfiguration.FTPSiteBackup() 
					 , _scannerConfiguration.FTPUsernameBackup());
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
BOOL CScan32App::ReadAssetCacheFile ()
{
	// set the file option, then try and load it
	//_pAssetCacheFile = new CAssetCacheFile();
	//_pAssetCacheFile->SetStatus ((CAssetCacheFile::eCacheFileScope)_assetCacheFileScope);
	//BOOL bAssetCacheFileRead = _pAssetCacheFile->Load(_scannerConfiguration.DeployPathData(), _scannerConfiguration.AutoName());

	//// Save the date of previous audit (if any)
	//DateOfLastAudit(_pAssetCacheFile->GetLastAuditDate());
	//
	//// Read category, make and model, serial, unique id, MAC and IP address from any cache file
	//m_strCat	= _pAssetCacheFile->GetString("Category");
	//m_strDomain	= _pAssetCacheFile->GetString("Domain");
	//m_strMake	= _pAssetCacheFile->GetString("Make");
	//m_strModel	= _pAssetCacheFile->GetString("Model");
	//m_strSerial	= _pAssetCacheFile->GetString("Serial");
	////m_strUniqueID = _pAssetCacheFile->GetString("UniqueID");
	//m_strMacAddr = _pAssetCacheFile->GetString("MACAddress");
	//m_strIPAddr = _pAssetCacheFile->GetString("IPAddress");

	//// We need to read any 'old' user defined data field values from the ini file into our local storage
	//// First get the list of currently defined user data categories and fields
	//CUserDataCategoryList& listCategories = GetCFG().UserDataCategories();

	//// Now enumerate the values which have been stored in the asset cache file
	//CDynaList<CString> keys;
	//_pAssetCacheFile->SetSection("User Data");
	//_pAssetCacheFile->EnumKeys("User Data", keys);
	//
	//// Loop through these saved items and initialize the value for the current list for any matches found
	//for (DWORD dw = 0 ; dw < keys.GetCount() ; dw++)
	//{
	//	CString strKey = keys[dw];
	//	CString strValue = _pAssetCacheFile->GetString(strKey);
	//	
	//	// The key should be in the form category|field so we need to split it
	//	int delimiter = strKey.Find("|");
	//	if (delimiter == -1) 
	//		continue;
	//	CString categoryName = strKey.Mid(0, delimiter);
	//	CString fieldName = strKey.Mid(delimiter + 1);
	//	
	//	// Now find this category/field in our internal list
	//	CUserDataField* pUserDataField = listCategories.FindField(categoryName ,fieldName);
	//	if (pUserDataField != NULL)
	//	{
	//		pUserDataField->CurrentValue(strValue);
	//	}
	//}	
	//
	//// Rewrite all of the remaining code
	//return bAssetCacheFileRead;

	return false;
}



//
//    BuildWizardPages
//    ================
//
//    Build the Wizard Form, depending on configuration
//
BOOL CScan32App::BuildWizardPages()
{
	CLogFile log;
	log.Write("Creating Wizard Pages...");

	// Asset, Location & User Data only collected for Interactive Audit
	if (GetAPP()->ScannerMode() == CAuditScannerConfiguration::modeInteractive)
	{
		// add the first page if flagged.
		//if ((GetCFG().ScreensMask() & CAuditScannerConfiguration::BasicInfo) != 0)
		//	m_pWizard->AddPage (&(m_pWizard->m_pageAsset));

		if (GetCFG().DisplayBasicInformationScreen())
			m_pWizard->AddPage (&(m_pWizard->m_pageAsset));
		
		// add the LOCATIONS page if flagged.
		//if ((GetCFG().ScreensMask() & CAuditScannerConfiguration::Location) != 0)
		//	m_pWizard->AddPage (&(m_pWizard->m_pageLocations));

		if (GetCFG().DisplayLocationScreen())
			m_pWizard->AddPage (&(m_pWizard->m_pageLocations));
			
		// OK now the user data pages - we add one for each category defined and as such our first task is
		// to recover the list of user data categories
		/*if (_scannerMode != CAuditScannerConfiguration::modeNonInteractive)
			AddUserDataPages();*/

		if (GetCFG().DisplayUserDataScreen())
			AddUserDataPages();
	}

	// Add the scanning page
	m_pWizard->AddPage(&(m_pWizard->m_pageScanning));

	// Ensure that the scanner knows about the WMI scanner object!
	m_pWizard->m_pageScanning.WmiScanner(m_pWmiScanner);

	// always add the data saving page
	m_pWizard->AddPage (&(m_pWizard->m_pageSave));

	// set it to run as a wizard
	m_pWizard->SetWizardMode();
	log.Format("OK\n");

	return TRUE;
}


//
//    AddUserDataPages
//    ================
//
//    Add pages to the wizard for the entry of User Defined Data
//
BOOL CScan32App::AddUserDataPages()
{
	CLogFile log;
	log.Format("Creating User Data Wizard Pages...");

	// Loop around and create a page for each Category
	CUserDataCategoryList& listCategories = GetCFG().UserDataCategories();
	for (int index = 0; index < listCategories.GetCount(); index++)
	{
		CUserDataCategory* pCategory = &(listCategories.ListCategories())[index];
		CPageUserData* pUserData = new CPageUserData(pCategory);
		m_pWizard->AddPage(pUserData);
	}
	
	return TRUE;
}




//
//    ExitInstance
//    ============
//
//    Override to ensure correct application return code
//
int CScan32App::ExitInstance() 
{
	return CWinApp::ExitInstance();
}


void	CScan32App::OpenLogFile	(CLogFile& log)
{
	if (_traceMode)
	{
		log.Open();
		// and log the scanner version
		char szAppName[_MAX_PATH + 1];
		GetModuleFileName(m_hInstance, szAppName, _MAX_PATH);
		CString strVersion = FindVersionString(szAppName, "ProductVersion");
		log.Format("%s - version %s\n", szAppName, strVersion);
		// tell user we are logging, and where to...
		CString strMessage;
		strMessage.Format("Scan32 is running in diagnostic trace mode\nMessages will be written to\n\n%s", log.GetName());
		AfxMessageBox(strMessage);
	}
}



//
//    CheckActive
//    ===========
//
//    Check for a previous instance of the scanner and close it down
//
void	CScan32App::CheckActive	(void)
{
	// Try and open the  
	HANDLE hMutex ,hCloseMutex;

	// Create the scan32 mutex - if it already exists there is another scanner running
    hMutex = CreateMutex(NULL, TRUE, "AuditScannerv8");
	
	if (GetLastError() == ERROR_ALREADY_EXISTS)
	{
		// Create a Close mutex.  Other scanners check this periodically
		hCloseMutex = CreateMutex(NULL, TRUE, "ShutdownScanv8");

		// ...and wait for the mutex to be released.
		do
		{
			Sleep(500);
		} while (WAIT_OBJECT_0 != WaitForSingleObject(hMutex, 200));

		// Close the shutdown mutex so that we do not pick it up later
		CloseHandle(hCloseMutex);
	}
}

	


//
//    CheckCloseDownRequested
//    ========================
//
//    This is the opposite of the above - we check to see if another instance has requested that we close down 
//    and return true if we have been requested as such
//
BOOL	CScan32App::CheckCloseDownRequested	(void)
{
	// Check the closedown mutex
	HANDLE hShutdownMutex = OpenMutex(MUTEX_ALL_ACCESS, FALSE, "ShutdownScanv8");
	if (hShutdownMutex != NULL)
	{
		TRACE("Shutting down as requested by Mutex...");

		// Uninstall the LyncUSB and PDA scanners
		if (_scannerConfiguration.ScanMDA())
			InstallMDA (TRUE);

		//
		if (_scannerConfiguration.ScanUSB())
		{
			// We must not exit until the scanner has been uninstalled as otherwise
			// it is possible that another instance of the scanner will fail to install
			InstallUSB (TRUE);
			Sleep(5000);				
		}
		return TRUE;
	}
	
	return FALSE;
}




//
//    AuditNow
//    ========
//
//    Perform a full audit of this PC 
//
BOOL CScan32App::AuditNow (CLogFile & log)
{
	BOOL bRetCode = TRUE;

	try
	{
		// if no category then default to "PC"
		if (m_strCat.IsEmpty())
		{
			CAssetType assetType;
			m_strCat = assetType.GetAssetType();
		}
		
		// Some of the user defined data fields may require their values to be automatically recovered rather
		// than entered by the user - this relates specifically to those fields flagged as either environment
		// variables or registry keys.
		log.Write("", TRUE);
		log.Write("===================================================================================");
		log.Write("performing AuditUserDefinedData()");
		log.Write("===================================================================================");		
		AuditUserDefinedData();
		
		// derive the basic asset data fields, if not already stored
		log.Write("", TRUE);
		log.Write("===================================================================================");
		log.Write("performing AuditAssetData()");
		log.Write("===================================================================================");
		AuditAssetData();

		// CMD 16/06/14 - 8.4.3
		// Chassis Types of 8-10 all indicate Laptops or Portable Computers
		if (m_iChassisType == 8 || m_iChassisType == 9 || m_iChassisType == 10 || m_iChassisType == 12 || m_iChassisType == 14)
		{
			m_strCat = "Laptop";
		}
		else if (m_strCat == "Laptop")
		{
			m_strCat = "PC";
		}

		// sort out the interactive mode - first recover from the configuration file
		_scannerMode = _scannerConfiguration.ScannerMode();

		// if scanner is running for the first time we convert to full interactive, otherwise it
		// is not the first time and we convert to noninteractive
		if (_scannerMode == CAuditScannerConfiguration::modeFirstTimeInteractive)
		{
			//if (!_pAssetCacheFileRead)
			if (_firstTimeAudit)
			{
				_scannerMode = CAuditScannerConfiguration::modeInteractive;
			}
			
			// Cache file read ok - convert to non-interactibe and insist on auto-name for assets
			else
			{
				// non-interactive mode - is there an asset name stored ?
				_scannerMode = CAuditScannerConfiguration::modeNonInteractive;
				if (m_strAssetName == "")
				{
					// no - log a fault and get out
					log.Format("Aborting non-interactive scan - No Asset Name was obtained\n");
					return FALSE;
				}
			}
		}

		// create the main sheet object
		m_pWizard = new CShtScan("AuditWizard Scanner");

		BuildWizardPages();

		// if running non-interactive then allow the hidden mode, which minimises the app
		if (_scannerMode == CAuditScannerConfiguration::modeNonInteractive)
		{
			log.Write("Enabling Non-Interactive Mode");
			m_pWizard->m_bHidden = _scannerConfiguration.Hidden();
		}

		// datestamp the audit
		_dateOfThisAudit = CTime::GetCurrentTime();

		// run the wizard
		log.Format("Initialisation completed OK\n");
		int nResponse = m_pWizard->DoModal();

		if (IDCANCEL == nResponse)
		{
			log.Format("Scan was cancelled by User\n");
			bRetCode = FALSE;
		}

		else
		{
			log.Write("", true);
			log.Format("Scan completed\n");
			bRetCode = TRUE;
		}
		delete m_pWizard;
	}

	catch (CException *pEx)
	{
		bRetCode = FALSE;
		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		log.Format("Exception occurred in CScan32App::AuditNow, error was %s\n", szCause);
	}

	return bRetCode;
}



//
//    UpdateAssetCacheFile
//    ====================
//
//    Write any changes made during the audit process to the asset cache file and save it
//    These are changes which we will need to know about in a subsequent run such as the 
//    name of the asset and any fields that the user is able to change such as location, make
//    model and category as well as any specific user defined fields
//
void CScan32App::UpdateAssetCacheFile ()
{
	// First update basic information fields held in the 'Asset' section
	// Note that the asset name will have already been updated at the end of the 'basic information' page
	//_pAssetCacheFile->SetSection ("Asset");
	//_pAssetCacheFile->WriteString ("Asset Name" ,_pAssetCacheFile->GetAssetName());
	//_pAssetCacheFile->WriteString ("NetBIOS Name", _pAssetCacheFile->GetNetBiosName());
	//_pAssetCacheFile->WriteString ("New Name", _pAssetCacheFile->GetNewAssetName());
	//_pAssetCacheFile->WriteString ("Domain" ,m_strDomain);
	//_pAssetCacheFile->WriteString ("Category" ,m_strCat);
	//_pAssetCacheFile->WriteString ("Make" ,m_strMake);
	//_pAssetCacheFile->WriteString ("Model" ,m_strModel);
	//_pAssetCacheFile->WriteString ("Serial" ,m_strSerial);
	//_pAssetCacheFile->WriteString ("UniqueID" ,m_strUniqueID);
	//_pAssetCacheFile->WriteString ("MACAddress" ,m_strMacAddr);
	//_pAssetCacheFile->WriteString ("IPAddress" ,m_strIPAddr);

	//// Save the Date of Audit into the cache file
	//CString strBuffer;
	//strBuffer.Format ("%4.4d-%2.2d-%2.2d %2.2d:%2.2d:%2.2d", _dateOfThisAudit.GetYear(), _dateOfThisAudit.GetMonth(), _dateOfThisAudit.GetDay(),
	//				_dateOfThisAudit.GetHour(), _dateOfThisAudit.GetMinute(), _dateOfThisAudit.GetSecond());
	//_pAssetCacheFile->WriteString("Date", strBuffer);

	//// location
	//_pAssetCacheFile->WriteString ("Location", m_strLoc);

	//// Save the current value of ALL user defined data fields 
	//// We need to loop through the user data categories and fields
	//_pAssetCacheFile->SetSection ("User Data");
	//
	//CString fullName;
	//CUserDataCategoryList& listCategories = GetCFG().UserDataCategories();
	//for (int index = 0; index < listCategories.GetCount(); index++)
	//{
	//	CUserDataCategory* pCategory = &(listCategories.ListCategories())[index];
	//	
	//	// Loop through the fields for each category
	//	for (int fieldIndex = 0; fieldIndex < pCategory->GetCount(); fieldIndex++)
	//	{
	//		CUserDataField* pField= &(pCategory->ListDataFields())[fieldIndex];
	//		fullName.Format("%s|%s" ,pCategory->Name() ,pField->Label());
	//		_pAssetCacheFile->WriteString (fullName ,pField->CurrentValue());
	//	}
	//}


	//
	//// ...then save it
	//_pAssetCacheFile->Save();
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
void CScan32App::AuditUserDefinedData	(void)
{
	CLogFile log;

	log.Format("CScan32App::AuditUserDefinedData in \n");	

	// Loop around the User Defined Data Categories
	CUserDataCategoryList& listCategories = GetCFG().UserDataCategories();

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
void CScan32App::AuditAssetData	(void)
{
	CLogFile log;
	log.Format("CScan32App::AuditAssetData in... \n");

	// Recover basic information about this computer and store it locally
	CSystemBiosScanner bios;
	bios.Scan(m_pWmiScanner);
	log.Write("out of bios scanner");
	if (m_strMake == "")
	{
		log.Write("setting make");
		m_strMake = bios.SystemManufacturer();
	}

	if (m_strModel == "")
	{
		log.Write("setting model");
		m_strModel = bios.SystemModel();
	}

	if (m_strSerial == "")
	{
		log.Write("setting serial no");
		m_strSerial = bios.SystemSerialNumber();
	}

	if (m_strAssetTag == "")
	{
		log.Write("setting asset tag");
		m_strAssetTag = bios.AssetTag();
	}

	if (m_strUniqueID == "")
	{
		log.Write("setting unique id");
		m_strUniqueID = bios.UniqueID();
	}
	
	m_iChassisType= atoi(bios.ChassisType());
	log.Format("Setting Chassis Type :  %d\n", m_iChassisType);

	log.Format("Make: %s\n" ,m_strMake);
	log.Format("Model : %s\n" ,m_strModel);

	// Domain/Workgroup
	CNetworkInformationScanner networkInformation;
	networkInformation.Scan(m_pWmiScanner);
	m_strDomain = networkInformation.DomainName();
	m_strIPAddr = networkInformation.IPAddress();

	// Get the network adapters as from here we can determine the 'active' MAC address as it will
	// be the NIC with the above IP address
	CNetworkAdaptersScanner networkAdapters;
	networkAdapters.Scan(m_pWmiScanner);
	CNetworkAdapter* pPrimaryAdapter = networkAdapters.FindAdapter(m_strIPAddr);
	if (pPrimaryAdapter != NULL)
		m_strMacAddr = pPrimaryAdapter->MacAddress();
	else
		m_strMacAddr = UNKNOWN;

	log.Format("CScan32App::AuditAssetData out \n");
}




//
//    GetRegValue
//    ===========
//
//    Recover a value from the system registry
//
CString CScan32App::GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem)
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


//
//  GetLongPathName
//
void	CScan32App::GetLongPathName (LPCSTR lpPath ,LPSTR lpLongPath ,DWORD dwBufSize)
{
	// Assume that this will all fail and we will simply return what was passed in
	strcpy(lpLongPath ,lpPath);

	// See if the Kernel contains a GetLongPathName function
	HMODULE hKernel = LoadLibrary("KERNEL32.DLL");
	if (hKernel)
	{
		typedef DWORD (FAR WINAPI * LONGPATHPROC)(LPCTSTR, LPTSTR, DWORD);
		LONGPATHPROC pfn = (LONGPATHPROC)GetProcAddress(hKernel, "GetLongPathNameA");
		if (pfn)
			pfn(lpPath ,lpLongPath ,dwBufSize);
	}
	
	return;
}



//
//	InstallMDA
//  ==========
//
//  Install / Remove Mobile device scanners
//
//   Pass TRUE to force an uninstall of the scanner
//
void CScan32App::InstallMDA (BOOL bUninstall/*=FALSE*/)
{
	CString strCmdLine;
	CFileStatus fs;
	CLogFile log;

	// first see if the installation executable exists
	CString fileName = MakePathName(_scannerConfiguration.DeployPathScanner() , MDA_INSTALLER);
	if (!CFile::GetStatus (fileName, fs))
		return;

	// install or uninstall, as required...
	if (!bUninstall && _scannerConfiguration.ScanMDA())
	{
		int nScanFiles	= _scannerConfiguration.ScanMobileFiles();

		// read in the folder specs
		CString strFileSpec;
		if (nScanFiles == 1)
		{
			strFileSpec = "*.*";
		}
		
		else
		{
			// Get the list of file specifications to scan on mobile devices
			CDynaList<CString>& listMobileFiles = _scannerConfiguration.ListMobileFiles();
			
			// Convert it to a comma delimited string
			strFileSpec = ListToString(listMobileFiles ,',');

			// Lync objects if we don't pass anything as a file spec so pass a dummy value
			if (strFileSpec.IsEmpty())
				strFileSpec = "*.xyz";
		}

		// We need to ensure that the data path is in short format as Lync do not like
		// a path with quotes nor does it like a long path
		CString strShortDataPath = GetAlternatePathName(_scannerConfiguration.DeployPathData());
		
		// Ensure that the path does not end in a '\' as Lync does not like that
		if (strShortDataPath.Right(1) == '\\')
			strShortDataPath = strShortDataPath.Left(strShortDataPath.GetLength() - 1);

		// install - format up the command line...
		strCmdLine.Format (MDA_CMD_INSTALL, strShortDataPath, strFileSpec ,GetAssetName());
	}
	else
	{
		// remove - command line is simpler
		strCmdLine = MDA_CMD_REMOVE;
	}
	
	// Execute the command to install or uninstall the Mobile Device Scanner as required
	ShellExecute (CWnd::GetSafeOwner()->GetSafeHwnd(), "open", fileName, strCmdLine, NULL, SW_HIDE);	
}




//
//    InstallUSB
//    ==========
//
//    Install/uninstall the USB scanner - Pass TRUE to force an uninstall of the scanner
//
void CScan32App::InstallUSB (BOOL bUninstall/*=FALSE*/)
{
	CFileStatus	fs;
	CLogFile log;

	// first see if the installer exists
	CString fileName = MakePathName(_scannerConfiguration.DeployPathScanner() , USB_INSTALLER);
	if (!CFile::GetStatus (fileName, fs))
		return;

	// Uninstall the USB device every time as we may need to change settings
	log.Format("Lync UnInstall USB Command is : %s %s\n" ,fileName ,USB_CMD_REMOVE);
	ShellExecute (CWnd::GetSafeOwner()->GetSafeHwnd(), "open", fileName, USB_CMD_REMOVE, NULL, SW_HIDE);

	// install or uninstall, as required...
	if (!bUninstall && _scannerConfiguration.ScanUSB())
	{
		// We need to wait a few seconds for the uninstall to complete
		Sleep(5000);

		// Find file specification to scan
		int nScanFiles	= _scannerConfiguration.ScanUSBFiles();

		// read in the folder specs
		CString strFileSpec;
		if (nScanFiles == 1)
		{
			strFileSpec = "*.*";
		}
		
		else
		{
			// Get the list of file specifications to scan on mobile devices
			CDynaList<CString>& listFiles = _scannerConfiguration.ListUSBFiles();
			
			// Convert it to a comma delimited string
			strFileSpec = ListToString(listFiles ,',');

			// Lync objects if we don't pass anything as a file spec so pass a dummy value
			if (strFileSpec.IsEmpty())
				strFileSpec = "*.xyz";
		}

		// We need to ensure that the data path is in short format as Lync do not like
		// a path with quotes nor does it like a long path
		CString strShortDataPath = GetAlternatePathName(_scannerConfiguration.DeployPathData());
		
		// Ensure that the path does not end in a '\' as Lync does not like that
		if (strShortDataPath.Right(1) == '\\')
			strShortDataPath = strShortDataPath.Left(strShortDataPath.GetLength() - 1);

		// install the USB scanner
		CString strCmdLine;
		strCmdLine.Format (USB_CMD_INSTALL, strShortDataPath, (nScanFiles ? "y" : "n"), strFileSpec ,GetAssetName());
		log.Format("Lync Install USB Command is : %s %s\n",fileName ,strCmdLine);
		HINSTANCE hResult = ShellExecute (CWnd::GetSafeOwner()->GetSafeHwnd(), "open", fileName, strCmdLine, NULL, SW_HIDE);
		TRACE ("ShellExecute(%s,%s) returned %d\n", fileName, strCmdLine, hResult);

		// then start it
		log.Format("Lync Start USB Command is : %s %s\n",USB_START_EXE ,USB_START_CMD);	
		hResult = ShellExecute (CWnd::GetSafeOwner()->GetSafeHwnd(), "open", USB_START_EXE, USB_START_CMD, NULL, SW_HIDE);
		TRACE ("ShellExecute(%s,%s) returned %d\n", USB_START_EXE, USB_START_CMD, hResult);
		
	}
}
