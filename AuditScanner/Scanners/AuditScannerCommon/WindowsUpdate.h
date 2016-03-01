
#pragma once


//
//    CWindowsUpdate
//    ===============
//
//    Class to hold details of the Windows Firewall Settings 
//
class CWindowsUpdate 
{
public:        // object creation/destruction
   CWindowsUpdate ();
   virtual ~CWindowsUpdate ();

public:        
	// operations
	void	Detect		();
	BOOL	WindowsUpdateValid	(void) const			{ return m_bUpdateValid; }
	CString	GetUpdateState	(void) const				{ return m_strUpdateState; }
	CString	NextDetectionTime	(void) const			{ return m_strNextDetectionTime; }
	CString	ScheduledInstallDate	(void) const		{ return m_strScheduledInstallDate; }

protected:
	CString		m_strUpdateState;
	CString		m_strNextDetectionTime;
	CString		m_strScheduledInstallDate;
	BOOL		m_bDetected;
	BOOL		m_bUpdateValid;
};
