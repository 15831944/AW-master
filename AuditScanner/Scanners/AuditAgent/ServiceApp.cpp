// ServiceApp.cpp: implementation of the CServiceApp class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ServiceApp.h"

CServiceApp theApp;

#define RESAVE_INTERVAL_MINUTES	30
#define RESAVE_MAX_TRIES 10
#define AGENT_REQUEST_REAUDIT 128

#define AUDITWIZARD_REGKEY "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CServiceApp::CServiceApp() : CNTService(TEXT("AuditAgent"), TEXT("AuditWizard AuditAgent")), m_hStop(0)
{
	m_doAudit = TRUE;
	m_reauditRequested = FALSE;
	_reportAuditNotRequired = FALSE;
}

CServiceApp::~CServiceApp()
{
}

BOOL CServiceApp :: InitInstance()
{
	RegisterService (__argc, __argv);
	return FALSE;
}


//
//    Run
//    ===
//
//    This is the main processing function for the service.
//	  It is called as the service is started from the ServiceMain function
//
void CServiceApp::Run (DWORD, LPTSTR *)
{
	// report to the SCM that we're about to start
	ReportStatus (SERVICE_START_PENDING);

	// Try and delete any old log files - we need to do it this way as we cannot
	// call the Empty function as this gives a linker error
	CString strLogFile = "./AuditAgent.log";
	try
	{
		CFile::Remove(strLogFile);
	}
	catch (CFileException*) 
	{
	}

	// ...then open the log file
	m_log.Open();
	
	// Create the stop event which we check to determine if the user has cancelled the scan
	m_hStop = ::CreateEvent (0, TRUE, FALSE, 0);

	// report SERVICE_RUNNING immediately before you enter the main-loop
	ReportStatus (SERVICE_RUNNING);

	// Initialize the service
	int nErrorCode = Initialise();
	if (nErrorCode == 0)
	{
		// ...and report that it is active
		AddToMessageLog ("The AuditWizard Agent Service is active", EVENTLOG_INFORMATION_TYPE);
		m_log.Write ("The AuditWizard Agent Service is active");
	
		//
		// MAIN SERVICE LOOP
		// =================
		//
		//    We loop around here waiting for something to do.  The first time through the loop we will
		//    audit this Computer however subsequently we will wait for the audit flag to be (re)set
		//
		while (::WaitForSingleObject(m_hStop, 10) != WAIT_OBJECT_0 && (0 == nErrorCode))
		{
			// Check to see if we need to run an audit at this time
			if (IsAuditRequired())
			{
				m_log.Write ("Performing non-interactive audit");
				ScanNonInteractive ();
				m_bComputerAuditedThisRun = TRUE;
			}

			// Try and save any results which are present.  These may be from a recent audit or possibly
			// from one which occured some time ago.
			UploadResults();

			// We do not want to loop too frequently so let's sleep here for a while
			Sleep(10000);
			YieldProcess();
		}
	}

	// OK All done - either windows is closing or we have been requested to close - add an appropriate message to the log
	if (nErrorCode != 0)
	{
		Message (gszErrorMsgTable[nErrorCode], MB_ICONSTOP);
		CString strMsg;
		strMsg.Format("The AuditWizard Agent service is exiting due to an error.  The error is : %s" ,gszErrorMsgTable[nErrorCode]);
	}

	// If we have the stop event registered close it now
	m_log.Write ("The AuditWizard Agent is exiting");

	if (m_hStop)
		::CloseHandle (m_hStop);

	// and exit after reporting this fact
	m_log.Write ("---- EXITING -----");
	return;
}



//
//    Stop
//    ====
//
//    Called when we request the service to stop or it otherwise closes down of its own volition
//
void CServiceApp::Stop()
{
	// report to the SCM that we're about to stop
	// Note that the service might Sleep(), so we have to tell
	// the SCM "The next operation may take me up to 11 seconds. Please be patient."
	ReportStatus (SERVICE_STOP_PENDING, 11000);

	if (m_hStop)
		::SetEvent(m_hStop);
}

BOOL CServiceApp::CustomCommand(DWORD dwCtrlCode)
{
	CLogFile log;
	log.Format("Custom Service Command Received [%d]\n" ,dwCtrlCode);

	if (dwCtrlCode == AGENT_REQUEST_REAUDIT)
	{
		CLogFile log;
		log.Write("Explicit Re-audit request received...\n");
		m_reauditRequested = TRUE;
		return TRUE;
	}
	return FALSE;

}

CTime CServiceApp::GetLastAuditDateFromReg(void)
{
	char nLastAuditDate[2048];
	CLogFile log;
	CTime lTimeLastAudit;

	CString lastAuditDate = CReg::GetItemString(HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, "LastAuditDate");
	//log.Format("located LastAuditDate with value - %s \n", lastAuditDate);

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
		lTimeLastAudit = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
	}

	return lTimeLastAudit;
}

//CTime CServiceApp::GetLastAuditDateFromRegistry(void)
//{
//	CLogFile log;
//	HKEY hk;
//	CTime lTimeLastAudit;
//	LONG lResult;
//
//	if (RegOpenKeyEx( HKEY_LOCAL_MACHINE, AUDITWIZARD_REGKEY, 0, KEY_ALL_ACCESS, &hk ) == ERROR_SUCCESS)
//	{
//		char szLastAuditDate[100];
//		DWORD ulSize = sizeof(szLastAuditDate);
//		lResult = RegQueryValueEx( hk, "LastAuditDate", NULL, NULL, (LPBYTE)szLastAuditDate, &ulSize);
//
//		CString lastAuditDate;
//		if (lResult != ERROR_SUCCESS)
//		{
//			lastAuditDate = "";
//		}
//		else
//		{	
//			lastAuditDate = szLastAuditDate;
//		}
//
//		//log.Format("located LastAuditDate with value - %s \n", lastAuditDate);
//
//		// convert string to date and store this value
//		if (!lastAuditDate.IsEmpty())
//		{
//			LPCSTR p = (LPCSTR)lastAuditDate;
//			int nYear	= atoi(p);
//			int nMonth	= atoi(p + 5);
//			int nDay	= atoi(p + 8);
//			int nHour	= atoi(p + 11);
//			int nMinute = atoi(p + 14);
//			int nSecond = atoi(p + 17);
//			lTimeLastAudit = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
//		}
//
//		RegCloseKey(hk);
//	}
//
//	return lTimeLastAudit;
//}
//
//

//
//    Message
//    =======
//
//    Output a message to the message log optionally closing down the service if the error is severe
// 
BOOL CServiceApp::Message (LPCSTR pszMessage, UINT nIcon)
{
	if (nIcon == MB_ICONINFORMATION)
	{
		// just an "info" message ?
		AddToMessageLog (pszMessage, EVENTLOG_INFORMATION_TYPE);
	}
	else
	{
		// assume all other messages are fatal
		AddToMessageLog (pszMessage);
		// kill the service
		if (m_hStop)
			::SetEvent (m_hStop);
	}
	return TRUE;
}


//
//    IsAuditRequired
//    ===============
//
//    Check to see if an audit is required.  This is signalled using the re-audit interval and date
//    of last audit to see if the time between audits has been reached or an explicit request from
//    the main user interface
//
BOOL CServiceApp::IsAuditRequired(void)
{
	CLogFile log;
	BOOL bAuditNow = FALSE;

	// Ensure that we don't need to re-read the scanner configuration file
	RefreshAgentConfiguration();

	// get any specified re-audit interval
	int nReAuditDays = _scannerConfiguration.ReAuditInterval();

	// get last audit date - if any - if we don't have one that assume that this
	// is the first time around (or a flag for re-audit)
	CTime tmLastAudit = GetLastAuditDateFromReg();

	// If we have a re-audit interval of 0 days and have already audited this computer in this run of the
	// Agent then we will re-set the interval to b 1 day to prevent excessive re-audits of the PC each time
	// around the main processing loop
	if (nReAuditDays == 0 && m_bComputerAuditedThisRun)
		nReAuditDays = 1;

	// if no previous audit or we have been explicitely asked to audit now then set the flag to 
	// force an immediate audit
	if ((tmLastAudit.GetTime() == 0) || (m_reauditRequested))
	{
		m_reauditRequested = FALSE;
		bAuditNow = TRUE;
	}

	else
	{
		// or has re-audit interval elapsed ?
		CTimeSpan tsElapsed = CTime::GetCurrentTime() - tmLastAudit;

		// get (rounded) number of days
		int nDays = (int)((tsElapsed.GetTotalHours()) / 24);		
		if (nDays >= nReAuditDays)
		{
			log.Format("The specified re-audit interval of %d day(s) has elapsed\n" ,nReAuditDays);
			bAuditNow = TRUE;
			_reportAuditNotRequired = FALSE;
		}
		else if (!_reportAuditNotRequired)
		{
			log.Format("The specified re-audit interval of %d day(s) has not yet elapsed - an audit is not required at this point\n", nReAuditDays);
			_reportAuditNotRequired = TRUE;
		}
	}

	return bAuditNow;
}
