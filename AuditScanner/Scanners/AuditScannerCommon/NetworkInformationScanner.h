#pragma once
#include "auditdatascanner.h"

class CNetworkInformationScanner : public CAuditDataScanner
{
public:
	CNetworkInformationScanner	();
	~CNetworkInformationScanner(void);


// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistry9X	(void);
	bool	ScanExceptions	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

// Data Accessor Functions
public:
	CString& ComputerName()
	{ return m_strComputerName; }
	void	 ComputerName(LPCSTR value)
	{ m_strComputerName = value; m_strComputerName.Trim(); }
	//
	CString&	DomainName()
	{ return m_strDomainName; }
	void		DomainName(LPCSTR value)
	{ 		
		m_strDomainName = value; 
		m_strDomainName.Trim();
		m_strDomainName.MakeUpper();
	}
	//
	CString&	LogonClient()
	{ return m_strLogonClient; }
	void		LogonClient(LPCSTR value)
	{ m_strLogonClient = value; m_strLogonClient.Trim(); }
	//
	CString&	UserName()
	{ return m_strUserName; }
	void		UserName(LPCSTR value)
	{ m_strUserName = value; m_strUserName.Trim(); }
	//
	CString&	IPAddress()
	{ return m_strIPAddress; }
	void		IPAddress(CString& value)
	{ m_strIPAddress = value; }
	//
	CString&	MACAddress()
	{ return m_strMACAddress; }
	void		MACAddress(CString& value)
	{ m_strMACAddress = value; }

private:
	// Internal class data
	CString		m_strComputerName;
	CString		m_strDomainName;
	CString		m_strLogonClient;
	CString		m_strUserName;
	CString		m_strIPAddress;
	CString		m_strMACAddress;
};
