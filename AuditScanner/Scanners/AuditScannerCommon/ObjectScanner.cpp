#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "ObjectScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Objects"
#define V_OBJECT_NAME				"Name"
#define V_OBJECT_VERSION			"Version"

CObjectScanner::CObjectScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CObjectScanner::~CObjectScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CObjectScanner::ScanWMI(CWMIScanner *pScanner)
{
	// Cannot recover objects using WMI
	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CObjectScanner::ScanRegistryXP()
{
	CLogFile log;
	log.Write("CObjectScanner::ScanRegistryXP Start" ,true);

	HKEY hKey;	
	CString strKey = "SOFTWARE\\Microsoft\\DataAccess";
	if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_LOCAL_MACHINE, strKey, 0, KEY_READ, &hKey)) 
	{
		CString strVersion = CReg::GetItemString(HKEY_LOCAL_MACHINE, strKey.GetBuffer(0), "FullInstallVer");	
		RegCloseKey (hKey);
		CInstalledObject installedObject("MDAC" ,strVersion);
		_listObjects.Add(installedObject);
	}

	log.Write("CObjectScanner::ScanRegistryXP End" ,true);
	return true;
}


//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CObjectScanner::ScanRegistryNT()
{
	return ScanRegistryXP();
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CObjectScanner::ScanRegistry9X()
{
	return ScanRegistryXP();
}


//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CObjectScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CObjectScanner::SaveData Start" ,true);
	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	for (int isub=0; isub<(int)_listObjects.GetCount(); isub++)
	{
		CInstalledObject* pObject = &_listObjects[isub];
		CAuditDataFileItem item(pObject->Name() ,pObject->Version());
		category.AddItem(item);
	}

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);

	log.Write("CObjectScanner::SaveData End" ,true);
	return true;
}