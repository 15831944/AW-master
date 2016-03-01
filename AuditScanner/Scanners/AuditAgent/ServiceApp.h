// ServiceApp.h: interface for the CServiceApp class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "ScanApp.h"

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

class CServiceApp : public CScanApp, public CNTService  
{
	HANDLE	m_hStop;

public:
	CServiceApp();
	virtual ~CServiceApp();

public:
	virtual BOOL InitInstance ();
	virtual void Run (DWORD, LPTSTR *);
	virtual void Stop ();
	virtual BOOL CustomCommand(DWORD dwCtrlCode);

	virtual BOOL Message (LPCSTR pszMessage, UINT nIcon);

protected:
	// Check to see if an audit is required at this time
	BOOL	IsAuditRequired	(void);

	// Save the results of the audit
	void	SaveResults		(void);
	CTime GetLastAuditDateFromReg(void);

// Settings
protected:
	CLogFile	m_log;

	// This flag indicates that we need to run an audit
	BOOL	m_doAudit;

	// This flag indicates whether or not we have audited this computer in this run of the agent
	// It is used to handle 0 reaudit days where we want to audit the computer first time but not 
	// every subsequent iteration of the main processing loop
	BOOL	m_bComputerAuditedThisRun;

	// This is the time at which we last tried to upload results
	CTime	m_LastSaveDate;					// Date/time of last save attempt

	// Flag to indictate that a re-audit has been requested by the UI
	BOOL	m_reauditRequested;

	// Flag to show that we have or haven't reported that the re-audit interval has expired
	BOOL	_reportAuditNotRequired;
};