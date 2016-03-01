
#pragma once

class CFirewallSettings
{
public:
	CFirewallSettings (LPCSTR pszProfileName);
	virtual ~CFirewallSettings ();

public:        
	CString		ProfileName	() const						{ return m_strProfileName; } 
	BOOL		Enabled () const							{ return m_bEnabled; }
	BOOL		NoExceptions () const						{ return m_bNoExceptions; }
	CDynaList<CString>&	AuthorizedApplications ()			{ return m_listApplications; }
	CDynaList<CString>&	GlobalPorts()						{ return m_listPorts; }

protected:
	void Detect(void);

protected:
	CString	m_strProfileName;
	BOOL	m_bEnabled;
	BOOL	m_bNoExceptions;

	CDynaList<CString>	m_listApplications;
	CDynaList<CString>	m_listPorts;
};


//
//    CFirewallInfo
//    ============
//
//    Class to hold details of the Windows Firewall Settings 
//
class CFirewallInfo 
{
public:        // object creation/destruction
   CFirewallInfo ();
   virtual ~CFirewallInfo ();

public:        
	// operations
	void	Detect		();
	BOOL	FirewallValid	(void) const			{ return m_bFirewallValid; }
	CFirewallSettings* DomainSettings (void)		{ return m_pDomainSettings; }
	CFirewallSettings* StandardSettings (void)		{ return m_pStandardSettings; }

protected:
	BOOL				m_bDetected;
	BOOL				m_bFirewallValid;
	CFirewallSettings*	m_pDomainSettings;
	CFirewallSettings*	m_pStandardSettings;
};
