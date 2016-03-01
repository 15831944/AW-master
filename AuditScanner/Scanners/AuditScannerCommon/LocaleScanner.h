#pragma once

class CLocaleScanner :	public CAuditDataScanner
{
public:
	CLocaleScanner(void);
public:
	virtual ~CLocaleScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);
	virtual			CString			GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem);

protected:
	void ScanLocale ();

private:
	int         m_iCodePage;
	CString		m_strCalendarType;
    int         m_iOEMCodePage;
	CString		m_strLanguage;
	CString		m_strDateFormat;
	CString		m_strCountry;
    int         m_iCountryCode;
	CString		m_strTimeFormat;
	CString		m_strCurrency;
	CString		m_strTimeFormatSpecifier;
	CString		m_strLocaleLocalLanguage;
	CString		m_strLocaleTimeZone;
	BOOL		m_bDetected;
};
