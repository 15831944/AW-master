#include "stdafx.h"
#include "scan32.h"
#include "PageFinish.h"

#include "../AuditScannerCommon/CkSFtp.h"
#include "../AuditScannerCommon/CkSFtpFile.h"
#include "../AuditScannerCommon/CkSFtpDir.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define TIMER_ID_START	1
#define TIMER_ID_FINISH	2

/////////////////////////////////////////////////////////////////////////////
// CPageFinish property page

IMPLEMENT_DYNCREATE(CPageFinish, CScannerPage)

CPageFinish::CPageFinish() : CScannerPage(CPageFinish::IDD)
{
	//{{AFX_DATA_INIT(CPageFinish)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

CPageFinish::~CPageFinish()
{
}

void CPageFinish::DoDataExchange(CDataExchange* pDX)
{
	CScannerPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageFinish)
	DDX_Control(pDX, IDC_LB_RESULTS, m_lbResults);
	//}}AFX_DATA_MAP
}



//
//    SaveData
//    ========
//
//    Save the resultant data from the audit.
//
void CPageFinish::SaveData()
{
	CWaitCursor wc;
	CString tempAuditDataFile = "";

	// When saving results to FTP, TCP or EMAIL we need to save them temporarily first
	if ((GetCFG().UploadSetting() == CAuditScannerConfiguration::ftp)
	||  (GetCFG().UploadSetting() == CAuditScannerConfiguration::tcp)
	||  (GetCFG().UploadSetting() == CAuditScannerConfiguration::email))
	{
		// First save locally...
		SaveResultsToTemp(tempAuditDataFile);
	}

	// Now save the results as required
	if (GetCFG().UploadSetting() == CAuditScannerConfiguration::ftp)
	{
		// ...then save to the FTP site - is it FTP or SFTP
		if (GetCFG().FTPType() == "FTP")
			SaveResultsToFtp(tempAuditDataFile);
		else
			SaveResultsToSftp(tempAuditDataFile);
	}

	// Otherwise are we saving to the network share?
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::network)
	{
		SaveResultsToNetwork();
	}

	// Otherwise are we saving to Email ?
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::email)
	{
		SaveResultsToEmail(tempAuditDataFile);
	}

	// Otherwise are we saving to TCP/IP?
	else if (GetCFG().UploadSetting() == CAuditScannerConfiguration::tcp)
	{
		SaveResultsToTcp(tempAuditDataFile);
	}

	// are we saving adf to central FTP site?
	if (GetCFG().FTPCopyToNetwork())
	{
		SaveResultsToTemp(tempAuditDataFile);

		if (GetCFG().FTPTypeBackup() == "FTP")
			SaveResultsToFtpBackup(tempAuditDataFile);
		else
			SaveResultsToSftpBackup(tempAuditDataFile);
	}
	
	// If we created any temporary files then delete them now
	if (!tempAuditDataFile.IsEmpty())
	{
		CFile::Remove(tempAuditDataFile);		
	}
}


//
//    SaveResultsToTemp
//    =================
//
//    Save the resultant audit data file to the TEMP folder and return the resultant name
//
void CPageFinish::SaveResultsToTemp	(CString& outputFileName)
{
	CLogFile log;
	CShtScan * pSht = (CShtScan*)GetParent();

	// Format a unique name for this audit file in the Windows Temp Folder
	char szTempPath[_MAX_PATH];
	GetTempPath(sizeof(szTempPath), szTempPath);

	// Construct a unique filename in the temp folder for the audit data file
	outputFileName = MakeUniqueFileName(szTempPath ,GetAPP()->GetAssetName() ,"ADF");
		
	// Save the results to the CAuditDataFile object
	CAuditDataFile outputAuditFile;
	pSht->SaveAuditData(outputAuditFile);

	// ...then force the output file to be written
	int status = outputAuditFile.Write(outputFileName);
	if (status != 0)
			AfxMessageBox("Error writing Audit Data");
}


//
//    SaveResultsToNetwork
//    ====================
//
//    Save the resultant AUD/SWB files to the specified network folder
//
void CPageFinish::SaveResultsToNetwork	(void)
{
	CLogFile log;
	CShtScan * pSht = (CShtScan*)GetParent();

	// Determine the name of the output file
	CString outputFileName;
	outputFileName = MakeUniqueFileName(GetCFG().DeployPathData() ,GetAPP()->GetAssetName() ,"ADF");
		
	// save the audit data
	m_lbResults.AddString("Saving Entered Data");
		
	// Save the results to the CAuditDataFile object
	CAuditDataFile outputAuditFile;
	pSht->SaveAuditData(outputAuditFile);

	// ...then force the output file to be written
	log.Format("writing data to file %s \n", outputFileName);
	int status = outputAuditFile.Write(outputFileName);
	if (status != 0)
			AfxMessageBox("Error writing Audit Data");

	log.Write("audit results saved to network");
}

//
//    SaveResultsToFtp
//    ================
//
//    Return the results of an audit via FTP
//
void CPageFinish::SaveResultsToFtp (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to an FTP Location\n");

	CScan32App* pApp = GetAPP();
	CFtpConnection* pFTPConnection = pApp->ValidateFTP();

	if (!pFTPConnection)
	{
		log.Format("The FTP details specified failed validation, data cannot be written\n");
		return;
	}

	// Build the base file name
	CString strBaseName = GetAPP()->GetAssetName();
	CString strOutputAud = strBaseName + ".ADF";

	// We need to ensure that this file does not conflict with an existing file
	try
	{
		CFtpFileFind finder(pFTPConnection);
		BOOL bStatus = finder.FindFile(strOutputAud);
		int nFileNumber = 1;

		while (bStatus) 
		{
			// yes there is - build a unique name
			strOutputAud.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus = finder.FindFile(strOutputAud);
		}

		// It seems that we have to close and re-open the FTP connection after the CFtpFileFind
		pFTPConnection->Close();
		pFTPConnection = pApp->ValidateFTP();
	
		// ...and transfer the files
		log.Format("Copying Audit Data File %s to the FTP Site as %s\n" ,auditDataFileName ,strOutputAud);
		if (!pFTPConnection->PutFile(auditDataFileName ,strOutputAud ,FTP_TRANSFER_TYPE_ASCII))
		{
			CString strMessage;
			strMessage.Format("Error code %d whilst uploading audit data file %s to the FTP Site %s", GetLastError() ,strOutputAud ,GetCFG().FTPSite());
			m_lbResults.AddString(strMessage);
			strMessage += "\n";
			log.Format(strMessage);
		}
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", GetLastError() ,GetCFG().FTPSite());
		m_lbResults.AddString(strMessage);	
		strMessage += "\n";
		log.Format(strMessage);
	}	

	pFTPConnection->Close();
	delete pFTPConnection;
}


//
//    SaveResultsToFtpBackup
//    ================
//
//    Return the results of an audit via FTP
//
void CPageFinish::SaveResultsToFtpBackup (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to FTP site\n");

	CScan32App* pApp = GetAPP();
	CFtpConnection* pFTPConnection = pApp->ValidateFTPBackup();

	if (!pFTPConnection)
	{
		log.Format("The FTP details specified failed validation, data cannot be written\n");
		return;
	}

	// Build the base file name
	CString strBaseName = GetAPP()->GetAssetName();
	CString strOutputAud = strBaseName + ".ADF";

	// We need to ensure that this file does not conflict with an existing file
	try
	{
		CFtpFileFind finder(pFTPConnection);
		BOOL bStatus = finder.FindFile(strOutputAud);
		int nFileNumber = 1;

		while (bStatus) 
		{
			// yes there is - build a unique name
			strOutputAud.Format("%s%d.ADF" ,strBaseName ,nFileNumber);
			nFileNumber++;
			bStatus = finder.FindFile(strOutputAud);
		}

		// It seems that we have to close and re-open the FTP connection after the CFtpFileFind
		pFTPConnection->Close();
		pFTPConnection = pApp->ValidateFTPBackup();
	
		// ...and transfer the files
		log.Format("Copying Audit Data File %s to the FTP Site as %s\n" ,auditDataFileName ,strOutputAud);
		if (!pFTPConnection->PutFile(auditDataFileName ,strOutputAud ,FTP_TRANSFER_TYPE_ASCII))
		{
			CString strMessage;
			strMessage.Format("Error code %d whilst uploading audit data file %s to the FTP Site %s", GetLastError() ,strOutputAud ,GetCFG().FTPSiteBackup());
			m_lbResults.AddString(strMessage);
			strMessage += "\n";
			log.Format(strMessage);
		}

		log.Format("Data saved to FTP site\n");
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", GetLastError() ,GetCFG().FTPSiteBackup());
		m_lbResults.AddString(strMessage);	
		strMessage += "\n";
		log.Format(strMessage);
	}	

	pFTPConnection->Close();
	delete pFTPConnection;
}

//
//    SaveResultsToSftp
//    ================
//
//    Return the results of an audit via SFTP
//
void CPageFinish::SaveResultsToSftp (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to an SFTP Location\n");

	CkSFtp sftp;
	CScan32App* pApp = GetAPP();

	try
	{
		bool success;
		success = sftp.UnlockComponent("LAYTONSSH_pErKX4Vp3InH");
		if (success != true)
		{
			log.Format("Unable to unlock component: %s\n", sftp.lastErrorText());
			return;
		}

		//  Set some timeouts, in milliseconds:
		sftp.put_ConnectTimeoutMs(10000);
		sftp.put_IdleTimeoutMs(10000);

		//  Connect to the SSH server.
		//  The standard SSH port = 22
		//  The hostname may be a hostname or IP address.
		long port;
		const char * hostname;
		hostname = GetCFG().FTPSite();
		port = GetCFG().FTPPort();

		success = sftp.Connect(hostname,port);
		if (success != true) 
		{
			log.Format("Unable to connect to SFTP: %s\n",sftp.lastErrorText());
			return;
		}

		//  Authenticate with the SSH server
		success = sftp.AuthenticatePw(GetCFG().FTPUsername(), pApp->DecryptFTPPassword(GetCFG().FTPPassword()));
		if (success != true) 
		{
			log.Format("%s\n",sftp.lastErrorText());
			return;
		}

		//  After authenticating, the SFTP subsystem must be initialized:
		success = sftp.InitializeSftp();
		if (success != true) 
		{
			log.Format("Unable to initialise SFTP: %s\n",sftp.lastErrorText());
			return;
		}

		//  Upload from the local file to the SSH server.
		//  Important -- the remote filepath is the 1st argument,
		//  the local filepath is the 2nd argument;

		// Build the base file name
		CString strBaseName = GetAPP()->GetAssetName();
		CString strOutputAud = strBaseName + ".ADF";

		bool bStatus = CheckFileExists(strOutputAud, sftp);
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

		success = sftp.UploadFileByName(remoteFilePath,localFilePath);
		if (success != true) 
		{
			log.Format("Unable to upload file: %s\n",sftp.lastErrorText());
			return;
		}
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", GetLastError() ,GetCFG().FTPSite());
		m_lbResults.AddString(strMessage);	
		strMessage += "\n";
		log.Format(strMessage);
	}
}




//
//    SaveResultsToSftp
//    ================
//
//    Return the results of an audit via FTP
//
void CPageFinish::SaveResultsToSftpBackup (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to SFTP site\n");

	CkSFtp sftp;
	CScan32App* pApp = GetAPP();

	try
	{
		bool success;
		success = sftp.UnlockComponent("LAYTONSSH_pErKX4Vp3InH");
		if (success != true)
		{
			log.Format("Unable to unlock sftp component: %s\n", sftp.lastErrorText());
			return;
		}

		//  Set some timeouts, in milliseconds:
		sftp.put_ConnectTimeoutMs(10000);
		sftp.put_IdleTimeoutMs(10000);

		//  Connect to the SSH server.
		//  The standard SSH port = 22
		//  The hostname may be a hostname or IP address.
		long port;
		const char * hostname;
		hostname = GetCFG().FTPSiteBackup();
		port = GetCFG().FTPPortBackup();

		success = sftp.Connect(hostname,port);
		if (success != true) 
		{
			log.Format("Unable to connect to SFTP: %s\n",sftp.lastErrorText());
			return;
		}

		//  Authenticate with the SSH server.  Chilkat SFTP supports
		//  both password-based authenication as well as public-key
		//  authentication.  This example uses password authenication.
		success = sftp.AuthenticatePw(GetCFG().FTPUsernameBackup(), pApp->DecryptFTPPassword(GetCFG().FTPPasswordBackup()));
		if (success != true) 
		{
			log.Format("Unable to authenticate to SFTP: %s\n",sftp.lastErrorText());
			return;
		}

		//  After authenticating, the SFTP subsystem must be initialized:
		success = sftp.InitializeSftp();
		if (success != true) 
		{
			log.Format("Unable to initialise SFTP: %s\n",sftp.lastErrorText());
			return;
		}

		//  Upload from the local file to the SSH server.
		//  Important -- the remote filepath is the 1st argument,
		//  the local filepath is the 2nd argument;

		// Build the base file name
		CString strBaseName = GetAPP()->GetAssetName();
		CString strOutputAud = strBaseName + ".ADF";

		bool bStatus = CheckFileExists(strOutputAud, sftp);
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
			return;
		}

		log.Format("Data saved to SFTP site\n");
	}

	catch (CInternetException* pEx)
	{
		pEx->Delete();
		CString strMessage;
		strMessage.Format("Error code %d whilst uploading Audit files to the FTP Site %s", sftp.lastErrorText(), GetCFG().FTPSite());
		m_lbResults.AddString(strMessage);	
		strMessage += "\n";
		log.Format(strMessage);
	}
}


bool CPageFinish::CheckFileExists(CString& auditDataFileName, CkSFtp& sftp)
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
void CPageFinish::SaveResultsToEmail (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to an Email\n");

	// compose and send an email...
	CString strBuffer;
	CMapiMsg msg;
		
	// recipient is the email address
	msg.m_To.Add(GetCFG().EmailAddress());
	
	// first attachment is the AUD file
	CString strAssetName = GetAPP()->GetAssetName();
	strBuffer.Format ("%s.ADF", strAssetName);
	msg.m_Attachments.Add(auditDataFileName);
	msg.m_AttachmentTitles.Add(strBuffer);
		
	// set the subject and text
	strBuffer.Format("AuditWizard Scan:%s", strAssetName);
	msg.m_strSubject = strBuffer;
	msg.m_strBody = "Message automatically generated by the AuditWizard Scanner";
			
	// send the message
	if (GetAPP()->m_mapi.Logon(""))
	{
		// logging off immediately after the send causes huge problems with Outlook Express
		// We now leave the session connected until the app shuts down, logging out via the CMapiSession d'tor
		if (GetAPP()->m_mapi.Send(msg))
		{
			m_lbResults.AddString("Scan results emailed to AuditWizard manager");
		}
		else
		{
			CString strMessage;
			strMessage.Format("Error code %d whilst sending mail", GetAPP()->m_mapi.GetLastError());
			m_lbResults.AddString(strMessage);
		}
	}
	else
	{
		CString strMessage;
		strMessage.Format("Error code %d whilst logging on to MAPI mail client", GetAPP()->m_mapi.GetLastError());
		log.Write(strMessage);
		m_lbResults.AddString(strMessage);
	}
}




//
//    SaveResultsToTcp
//    ================
//
//    Return the results of an audit via TCP/IP
//
void CPageFinish::SaveResultsToTcp (CString& auditDataFileName)
{
	CLogFile log;
	log.Format("Saving data to the AuditWizard TCP/IP Server on %s\n" ,GetCFG().AuditWizardServer());	
	
	// Connect to the AuditWizard TCP Server uif we have not already done so
	if (!m_tcp.IsConnecting())
	{
		CString strServer = GetCFG().AuditWizardServer();
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
			return;
		}
	}	
	
	// ...and send the audit file to the AuditWizard server		
	if (!m_tcp.SendFile(auditDataFileName))
		log.Format("Failed to save data to the AuditWizard TCP/IP Server on %s\n" ,GetCFG().AuditWizardServer());
}


// TCP Message Callback - we don't expect messages from the server so ignore any we might get
void CPageFinish::MsgCallback (CString strMessage, LPARAM lParam)
{
}

BEGIN_MESSAGE_MAP(CPageFinish, CScannerPage)
	//{{AFX_MSG_MAP(CPageFinish)
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageFinish message handlers

BOOL CPageFinish::OnInitDialog() 
{
	CScannerPage::OnInitDialog();

	CShtScan * pSht = (CShtScan*)GetParent();
	pSht->SetWizardButtons(PSWIZB_FINISH);
	
	// Fire a timer to start the saving process
	SetTimer (TIMER_ID_START, 100, NULL);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageFinish::OnTimer(UINT nIDEvent) 
{
	switch (nIDEvent)
	{
		case TIMER_ID_START:
			KillTimer(nIDEvent);
			SaveData();
			SetTimer (TIMER_ID_FINISH, (GetAPP()->ScannerMode() == CAuditScannerConfiguration::modeNonInteractive) ? 100 : 5000, NULL);
			break;

		case TIMER_ID_FINISH:
			KillTimer(nIDEvent);
			// All done - close down
			((CPropertySheet*)GetParent())->PressButton(PSBTN_FINISH);
			break;
	}

	CScannerPage::OnTimer(nIDEvent);
}
