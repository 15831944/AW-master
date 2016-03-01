
#include "auditdatascanner.h"

class CQuickFix
{
public:
	CQuickFix(void);
	~CQuickFix(void)
	{
	}

// Data accessors
public: 
	CString& Caption (void)
	{ 
		return m_strCaption;
	}
	void Caption	(LPCSTR value)
	{ m_strCaption = value; }

	//
	CString& CSName(void)
	{ 
		return m_strCSName;
	}
	void CSName(LPCSTR value)
	{
		m_strCSName=value;
	}
	
	//
	CString& Description(void)
	{
		return m_strDescription;
	}
	void Description(LPCSTR value)
	{
		m_strDescription=value;
	}

	//
	CString& FixComments(void)
	{ 
		return m_strFixComments;
	}
	void FixComments(LPCSTR value)
	{ 
		m_strFixComments=value; 
	}
	//
	CString& HotFixID(void)
	{
		return m_strHotFixID;
	}
	void HotFixID (LPCSTR value)
	{
		m_strHotFixID=value;
	}
	//
	CString& InstallDate(void)
	{
		return m_strInstallDate;
	}
	void InstallDate(LPCSTR value)
	{
		m_strInstallDate=value;
	}

	//
	CString& InstalledBy(void)
	{
		return m_strInstalledBy;
	}
	void InstalledBy(LPCSTR value)
	{
		m_strInstalledBy=value;
	}
	//
	CString& InstalledON(void)
	{
		return m_InstalledON;
	}
	void InstalledON(LPCSTR value)
	{
		m_InstalledON=value;
	}
	//
	CString& Name(void)
	{
		return m_strName;
	}
	void Name(LPCSTR value)
	{
		m_strName=value;
	}
	//
	CString& ServicePack(void)
	{
		return m_strServicePack;
	}
	void ServicePack(LPCSTR value)
	{
		m_strServicePack=value;
	}
	//
	CString& Status(void)
	{
		return m_strStatus;
	}
	void Status(LPCSTR value)
	{
		m_strStatus=value;
	}

// Internal Data
private:
	CString		m_strCaption;
	CString		m_strCSName;	
	CString		m_strDescription;
	CString		m_strFixComments;
	CString		m_strHotFixID;
	CString		m_strInstallDate;
	CString		m_strInstalledBy;
	CString		m_InstalledON;
	CString		m_strName;	
	CString		m_strServicePack;	
	CString		m_strStatus;

};

class CQuickFixScanner : public CAuditDataScanner	
{
public:
	CQuickFixScanner(void);
	~CQuickFixScanner(void);
	CDynaList<CQuickFix>		QuickFixList(void)
	{
		return m_listQuickFix;
	}
// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistry9X	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

private:
	CDynaList<CQuickFix>		m_listQuickFix;
	
};
