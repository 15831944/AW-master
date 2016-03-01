#pragma once



////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a user account
//
class CUserAccount
{
public:
	CUserAccount ()
	{};
	CUserAccount (LPCSTR pszUser ,LPCSTR pszFullName ,LPCSTR pszDescription ,BOOL bDisabled ,CTime& ctLastLogon ,int nLogonCount) 
		: m_strUser(pszUser) ,m_strFullName(pszFullName) ,m_strDescription(pszDescription) ,m_bDisabled(bDisabled) ,m_ctLastLogon(ctLastLogon) ,m_nLogonCount(nLogonCount)
	{};

	virtual ~CUserAccount()
	{};

	CString		User		(void) const
	{ return m_strUser; }

	CString		FullName	(void) const
	{ return m_strFullName; }

	CString		Description	(void) const
	{ return m_strDescription; }

	BOOL		IsDisabled	(void) const
	{ return m_bDisabled; }

	CTime		LastLogon	(void) const
	{ return m_ctLastLogon; }

	int			LogonCount	(void) const
	{ return m_nLogonCount; }

protected:
	CString		m_strUser;
	CString		m_strFullName;
	CString		m_strDescription;
	BOOL		m_bDisabled;
	CTime		m_ctLastLogon;
	int			m_nLogonCount;
};



class CUsersScanner : public CAuditDataScanner
{
public:
	CUsersScanner(void);
public:
	virtual ~CUsersScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	void	ScanUsers();

private:
	CDynaList<CUserAccount>	_listUsers;

private:
};