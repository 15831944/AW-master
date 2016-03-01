#pragma once

// Include our base class header
#include "AuditDataScanner.h"

//
class CSystemBiosScanner : public CAuditDataScanner
{
public:
	CSystemBiosScanner();
	~CSystemBiosScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

// Data Accessor Functions
public:
	CString&	UniqueID	(void)
	{ return m_strSystemUniqueID; }
	void		UniqueID	(CString& value)
	{ m_strSystemUniqueID = value.Trim(); }

	CString&	AssetTag			(void)
	{ return m_strAssetTag; }
	void		AssetTag	(CString& value)
	{ m_strAssetTag = value.Trim(); }

	CString&	SystemName			(void)
	{ return m_strSystemName; }
	void		SystemName	(CString& value)
	{ m_strSystemName = value.Trim(); }

	CString&	SystemManufacturer	(void)
	{ return m_strSystemManufacturer; }
	void		SystemManufacturer	(CString& value)
	{ m_strSystemManufacturer = value.Trim(); }

	CString&	SystemModel			(void)
	{ return m_strSystemModel; }
	void		SystemModel	(CString& value)
	{ m_strSystemModel = value.Trim(); }

	CString&	SystemSerialNumber	(void)
	{ return m_strSystemSerialNumber; }
	void		SystemSerialNumber	(CString& value)
	{ m_strSystemSerialNumber = value.Trim(); }

	CString&	ComputerType		(void)
	{ return m_strMachineType; }
	void		ComputerType	(CString& value)
	{ m_strMachineType = value.Trim(); }

	CString&	BiosManufacturer	(void)
	{ return m_strBiosManufacturer; }
	void		BiosManufacturer	(CString& value)
	{ m_strBiosManufacturer = value.Trim(); }

	CString&	BiosVersion			(void)
	{ return m_strBiosVersion; }
	void		BiosVersion	(CString& value)
	{ m_strBiosVersion = value.Trim(); }

	CString&	BiosDate			(void)
	{ return m_strBiosDate; }
	void		BiosDate	(CString& value)
	{ m_strBiosDate = value.Trim(); }

	CString&	UUID			(void)
	{ return m_strUUID; }
	void		UUID	(LPCSTR value)
	{ m_strUUID = value; m_strUUID = m_strUUID.Trim(); }

	CString&	ChassisType			(void)
	{ return m_strChassisType; }
	void		ChassisType	(CString& value)
	{ m_strChassisType = value.Trim(); }


protected:
	void	CreateUniqueID	();
	
	// Parse multi string registry value
	LPCTSTR ParseMultiSZ(LPCTSTR lpstrCurrent = NULL)
	{
		static LPCTSTR szRemainder = NULL;
		LPCTSTR szReturn = NULL;
		if (lpstrCurrent)
			szRemainder = lpstrCurrent;
		else
			if (szRemainder == NULL)
				return NULL;
		if (*szRemainder)	
		{
			szReturn = szRemainder;
			while (*++szRemainder);
			szRemainder++;     
		}
		return szReturn;
	}

protected:
	// Data
	CString		m_strSystemUniqueID;			// A unique ID hashed for this system
	CString		m_strUUID;						// System unique ID from WMI
	CString		m_strSystemName;				// Device netbios or DNS name
	CString		m_strSystemManufacturer;		// System manufacturer
	CString		m_strSystemModel;				// System model
	CString		m_strSystemSerialNumber;		// System serial number
	CString		m_strMachineType;				// System type (tower, mini-tower...)
	CString		m_strBiosManufacturer;			// BIOS manufacturer
	CString		m_strBiosVersion;				// BIOS version
	CString		m_strBiosDate;					// BIOS date
	CString		m_strAssetTag;					// BIOS date
	CString		m_strChassisType;				//Chassis type for determining PC or Laptop
};
