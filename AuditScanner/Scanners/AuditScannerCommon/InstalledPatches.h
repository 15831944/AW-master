#pragma once
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "AuditDataFile.h"

/*
**	Helper class to hold details of an installed patch
*/
class CInstalledPatch
{
public:
	// Constructor
	CInstalledPatch ()
		{}
	// destructor
	virtual ~CInstalledPatch()
		{}

	CString List (char chSep = '|') const;

	CString	Name () const
		{ return m_strName; }
	void Name (LPCSTR strName)
		{ m_strName = strName; }

	CString	Product	() const
		{ return m_strProduct; }
	void Product (LPCSTR strProduct)
		{ m_strProduct = strProduct; }

	CString	ServicePack	() const
		{ return m_strServicePack; }
	void ServicePack (LPCSTR strServicePack)
		{ m_strServicePack = strServicePack; }

	CString	Description	() const
		{ return m_strDescription; }
	void Description (LPCSTR strDescription )
		{ m_strDescription = strDescription; }

	CString InstallDate	() const
		{ return m_strInstallDate; }
	void InstallDate (LPCSTR strInstallDate)
		{ m_strInstallDate = strInstallDate; }

	CString Installer () const
		{ return m_strInstaller; }
	void Installer (LPCSTR strInstaller)
		{ m_strInstaller = strInstaller; }

protected:
	CString		m_strProduct;
	CString		m_strServicePack;
	CString		m_strName;
	CString		m_strDescription;
	CString		m_strInstallDate;
	CString		m_strInstaller;
};


/*
**	Class to obtain details of installed patches
*/
class CInstalledPatchList : public CDynaList<CInstalledPatch>
{
public:
		
	// Scan for installed patches
	int	 Scan (CWMIScanner* pScanner);
	// Save results to the audit data file
	bool	Save	(CAuditDataFile& auditDataFile)
	{
		return true;
	}
protected:
	void ScanNT4 ();
	void Scan2000 ();
	void ScanProduct (LPCSTR strProduct);
	void ScanServicePack (LPCSTR strProduct, LPCSTR strServicePack);
	void GetPatchDetails (LPCSTR strProduct, LPCSTR strServicePack, LPCSTR strPatch);
	void AddPatch (CInstalledPatch& newPatch);
};