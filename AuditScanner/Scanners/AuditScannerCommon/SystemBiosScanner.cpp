#include "stdafx.h"
#include "AuditDataFile.h"
#include "SystemBiosScanner.h"


//
// Registry definitions for recovering BIOS data
// =============================================
//
// Windows WIN
// ----------
#define WIN_BIOS_KEY							"ENUM\\Root\\*PNP0C01\\0000"
#define WIN_SYSTEM_MANUFACTURER_VALUE			"Mfg"
#define WIN_SYSTEM_MODEL_VALUE					"Model"
#define WIN_SYSTEM_SERIAL_VALUE					"Serial"
#define WIN_BIOS_MACHINE_TYPE_VALUE				"MachineType"
#define WIN_BIOS_MANUFACTURER_VALUE				"BIOSName"
#define WIN_BIOS_VERSION_VALUE					"BIOSVersion"
#define WIN_BIOS_DATE_VALUE						"BIOSDate"

// Windows NT
// ----------
#define NT_BIOS_KEY								"Hardware\\Description\\System"
#define NT_SYSTEM_MANUFACTURER_VALUE			"Manufacturer"
#define NT_SYSTEM_MODEL_VALUE					"Model"
#define NT_SYSTEM_SERIAL_VALUE					"Serial"
#define NT_BIOS_MACHINE_TYPE_VALUE				"Identifier"
#define NT_BIOS_MANUFACTURER_VALUE				"BIOSName"
#define NT_BIOS_VERSION_VALUE					"SystemBiosVersion"
#define NT_BIOS_DATE_VALUE						"SystemBiosDate"

// Storage Strings
#define HARDWARE_CLASS					"Hardware|BIOS"
#define V_BIOS_INTERNAL_SYSTEMID		"Internal System ID"
#define V_BIOS_SYSTEM_MANUFACTURER		"System Manufacturer"
#define V_BIOS_SYSTEM_MODEL				"System Model"
#define V_BIOS_SYSTEM_SERIAL			"System Serial Number"
#define V_BIOS_MANUFACTURER				"Bios Manufacturer"
#define V_BIOS_VERSION					"Bios Version"
#define V_BIOS_DATE						"Bios Date"
#define V_BIOS_ASSETTAG					"Asset Tag"

CSystemBiosScanner::CSystemBiosScanner() : CAuditDataScanner()
{
	m_strItemPath = HARDWARE_CLASS;
	//
	// Initialize all values
	m_strSystemUniqueID		= UNKNOWN;		
	m_strSystemName			= UNKNOWN;			
	m_strSystemManufacturer	= UNKNOWN;			
	m_strSystemModel		= UNKNOWN;			
	m_strSystemSerialNumber	= UNKNOWN;	
	m_strMachineType		= UNKNOWN;			
	m_strBiosManufacturer	= UNKNOWN;		
	m_strBiosVersion		= UNKNOWN;			
	m_strBiosDate			= UNKNOWN;
	m_strUUID				= UNKNOWN;
	m_strAssetTag			= UNKNOWN;
}

CSystemBiosScanner::~CSystemBiosScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CSystemBiosScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CSystemBiosScanner::ScanWMI Start" ,true);

	// Temporary work data
	CString strManufacturer = "";
	CString	strModel = "";
	CString	strSerial = "";
	//
	CString strBiosManufacturer = "";
	CString strBiosModel = "";
	CString strBiosVersion = "";
	CString strBiosDate = "";
	CString strUUID = "";

	CString strAssetTag = "";

	CString strChassisType = "";

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_ComputerSystem"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				strManufacturer = wmiConnection.GetClassObjectStringValue("Manufacturer");
				strModel		= wmiConnection.GetClassObjectStringValue("Model");
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CSystemBiosScanner::ScanWMI (Enumerating Win32_ComputerSystem)");
	}

	// Now we shall recover details of the BIOS itself...
	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Bios"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				strBiosManufacturer = wmiConnection.GetClassObjectStringValue("Manufacturer");
				strBiosVersion		= wmiConnection.GetClassObjectStringValue("BIOSVersion");

				// TODO - temp fix for BIOS date, find it via Registry
				HKEY	hKey = NULL;
				TCHAR	szValue[256];
				LONG	lResult;
				DWORD dwType = REG_SZ;
				DWORD dwSize = 255;

				lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, NT_BIOS_KEY, 0, KEY_READ, &hKey);
				if (lResult == ERROR_SUCCESS)
				{
					if (RegQueryValueEx( hKey, NT_BIOS_DATE_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
						strBiosDate = szValue;
				}

				/*if (m_bIsXP)
				{
					lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, NT_BIOS_KEY, 0, KEY_READ, &hKey);
					if (lResult == ERROR_SUCCESS)
					{
						if (RegQueryValueEx( hKey, NT_BIOS_DATE_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
							strBiosDate = szValue;
					}
				}
				else
				{
					lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, WIN_BIOS_KEY, 0, KEY_READ, &hKey);
					if (lResult == ERROR_SUCCESS)
					{
						if (RegQueryValueEx( hKey, WIN_BIOS_DATE_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
							strBiosDate = szValue;
					}
				}*/

				// If BIOSVersion is not set then just read Version
				if (strBiosVersion.IsEmpty())
					strBiosVersion = wmiConnection.GetClassObjectStringValue("Version");

				// Recover the BIOS serial number also - this is probably the same as that above
				//if (strSerial.IsEmpty() || strSerial == "None")

				strSerial = wmiConnection.GetClassObjectStringValue("SerialNumber");
			}
		}
	}
	catch (CException *pEx)
	{
		return false;
	}

	// If we still have missing information then our last chance is to recover from the Win32_Baseboard
	if (strManufacturer.IsEmpty() || strModel.IsEmpty() || strSerial.IsEmpty())
	{
		try
		{
			if (wmiConnection.BeginEnumClassObject("Win32_BaseBoard"))
			{
				while (wmiConnection.MoveNextEnumClassObject())
				{
					if (strManufacturer.IsEmpty())
						strManufacturer = wmiConnection.GetClassObjectStringValue("Manufacturer");

					if (strModel.IsEmpty())
						strModel = wmiConnection.GetClassObjectStringValue("Product");

					if (strSerial.IsEmpty() || strSerial == "None")
						strSerial = wmiConnection.GetClassObjectStringValue("SerialNumber");
				}
				wmiConnection.CloseEnumClassObject();
			}
		}
		catch (CException *pEx)
		{
			LogException(pEx, "CSystemBiosScanner::ScanWMI (Enumerating Win32_BaseBoard)");
		}
	}

	// Try to use system enclosure object to get System Manufacturer, Model, S/N and chassis type
	// noting that we will not over-write any values previously recovered above
	try
	{
		if (wmiConnection.BeginEnumClassObject( _T( "Win32_SystemEnclosure")))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// If we do not already have the manufacturer then recover it here
				if (strManufacturer.IsEmpty())
					strManufacturer = wmiConnection.GetClassObjectStringValue("Manufacturer");

				// ...and recove the model if we don't already have it
				if (strModel.IsEmpty())
					strModel = wmiConnection.GetClassObjectStringValue("Model");

				// Recover serial number or SMBIOS Asset Tag
				if (strSerial.IsEmpty() || strSerial == "None")
					strSerial = wmiConnection.GetClassObjectStringValue("SerialNumber");

				strAssetTag = wmiConnection.GetClassObjectStringValue("SMBIOSAssetTag");
				strChassisType= wmiConnection.GetClassObjectStringValue("ChassisTypes");
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CSystemBiosScanner::ScanWMI (Enumerating Win32_SystemEnclosure)");
	}	

	// Now try asnd get the system unique ID value from Win32_ComputerSystemProduct
	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_ComputerSystemProduct"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				strUUID = wmiConnection.GetClassObjectStringValue("UUID");
			}
		}
	}

	catch (CException *pEx)
	{
		return false;
	}

	// Now store the recovered information back into the supplied BIOS object
	SystemManufacturer(strManufacturer);
	SystemModel(strModel);
	SystemSerialNumber(strSerial);
	BiosManufacturer(strBiosManufacturer);
	BiosDate(strBiosDate);
	BiosVersion(strBiosVersion);
	UUID(strUUID);
	AssetTag(strAssetTag);
	ChassisType(strChassisType);

	// Create a unique ID for this system from the bios data
	//CreateUniqueID();
	m_strSystemUniqueID = strUUID;

	// ...and return the result
	log.Write("CSystemBiosScanner::ScanWMI End" ,true);
	return true;
}


//
//    CreateUniqueID
//    ===============
//
//    Create a unique identification string for this computer based on BIOS information read
//
void CSystemBiosScanner::CreateUniqueID()
{
	//CCheckSum checkSum;
	//checkSum.clear();
	//checkSum.add(m_strSystemSerialNumber);
	//checkSum.add(m_strSystemManufacturer);
	//checkSum.add(m_strSystemModel);
	//checkSum.add(m_strBiosManufacturer);
	//checkSum.add(m_strBiosDate);
	//DWORD checkSumValue = checkSum.get();

	////
	//m_strSystemUniqueID.Format("%s-%08.8d" ,m_strUUID ,checkSumValue);

	//HKEY hk;
	//DWORD dwDisp;
	//m_strSystemUniqueID = "";

	//if (RegOpenKeyEx( HKEY_LOCAL_MACHINE, "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8", 0, KEY_ALL_ACCESS, &hk ) == ERROR_SUCCESS)
	//{
	//	char szUniqueID[100];
	//	DWORD ulSize = sizeof(szUniqueID);
	//	if (RegQueryValueEx( hk, "UniqueID", NULL, NULL, (LPBYTE)szUniqueID, &ulSize) == ERROR_SUCCESS)
	//	{
	//		m_strSystemUniqueID = szUniqueID;
	//	}

	//	RegCloseKey(hk);
	//}
}



//
//    ScanRegistry9X
//    ==============
//
//    Recover BIOS information on a Windows WIN platform
//
bool CSystemBiosScanner::ScanRegistry9X()
{
	return ScanRegistryXP();

	//HKEY	hKey = NULL;
	//TCHAR	szValue[256];
	//LONG	lResult;
	//DWORD	dwType;
	//DWORD	dwSize;

	//// Temporary work data
	//CString strManufacturer(UNKNOWN);
	//CString	strModel(UNKNOWN);
	//CString	strSerial(UNKNOWN);
	//CString strBiosManufacturer(UNKNOWN);
	//CString strBiosModel(UNKNOWN);
	//CString strBiosVersion(UNKNOWN);
	//CString strBiosDate(UNKNOWN);

	//try
	//{
	//	// Open the main BIOS key
	//	lResult = RegOpenKeyEx(m_hKey, WIN_BIOS_KEY, 0, KEY_READ, &hKey);
	//	if (lResult == ERROR_SUCCESS)
	//	{
	//		// Get System manufacturer
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx(hKey, WIN_SYSTEM_MANUFACTURER_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strManufacturer = szValue;

	//		// Get System model
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx(hKey, WIN_SYSTEM_MODEL_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strModel = szValue;

	//		// Get System serial number
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx( hKey, WIN_SYSTEM_SERIAL_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strSerial = szValue;

	//		// Get BIOS manufacturer
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx( hKey, WIN_BIOS_MANUFACTURER_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strBiosManufacturer = szValue;
	//			
	//		// Get BIOS version
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx( hKey, WIN_BIOS_VERSION_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strBiosVersion = szValue;

	//		// Get BIOS date
	//		dwType = REG_SZ;
	//		dwSize = 255;
	//		if (RegQueryValueEx( hKey, WIN_BIOS_DATE_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
	//			strBiosDate = szValue;

	//		// Close the main key
	//		RegCloseKey(hKey);

	//		// Now store the recovered information back into the supplied BIOS object
	//		SystemManufacturer(strManufacturer);
	//		SystemModel(strModel);
	//		SystemSerialNumber(strSerial);
	//		BiosManufacturer(strBiosManufacturer);
	//		BiosDate(strBiosDate);
	//		BiosVersion(strBiosVersion);
	//		UUID("NONWMI");
	//	}
	//}
	//catch (CException *pEx)
	//{
	//	throw pEx;
	//}

	//return true;
}


//
//    ScanRegistryNT
//    ==============
//
//    Recover BIOS information on a Windows NT Upwards platform
//
bool CSystemBiosScanner::ScanRegistryNT()
{
	return ScanRegistryXP();
}



//
//    ScanRegistryXP
//    ==============
//
//    Recover BIOS information on a Windows XP Upwards platform
//
bool CSystemBiosScanner::ScanRegistryXP()
{
	HKEY	hKey = NULL;
	TCHAR	szValue[256];
	LONG	lResult;
	DWORD	dwType;
	DWORD	dwSize;
	CString	csResult;

	CLogFile log;
	log.Write("CSystemBiosScanner::ScanRegistryXP Start" ,true);

	try
	{
		// Temporary work data
		CString strManufacturer = "";
		CString	strModel = "";
		CString	strSerial = "";
		CString strBiosManufacturer = "";
		CString strBiosModel = "";
		CString strBiosVersion = "";
		CString strBiosDate = "";

		// Open the main BIOS key
		lResult = RegOpenKeyEx(m_hKey, NT_BIOS_KEY, 0, KEY_READ, &hKey);
		if (lResult == ERROR_SUCCESS)
		{
			// Get System manufacturer
			dwType = REG_SZ;
			dwSize = 255;
			if (RegQueryValueEx(hKey, NT_SYSTEM_MANUFACTURER_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
				strManufacturer = szValue;

			// Get System model
			dwType = REG_SZ;
			dwSize = 255;
			if (RegQueryValueEx(hKey, NT_SYSTEM_MODEL_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
				strModel = szValue;

			// Get System serial number
			dwType = REG_SZ;
			dwSize = 255;
			if (RegQueryValueEx( hKey, NT_SYSTEM_SERIAL_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
				strSerial = szValue;

			// Get BIOS manufacturer
			dwType = REG_SZ;
			dwSize = 255;
			if (RegQueryValueEx( hKey, NT_BIOS_MANUFACTURER_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
				strBiosManufacturer = szValue;
				
			// Get BIOS version
			dwType = REG_MULTI_SZ;
			dwSize = 255;
			if (RegQueryValueEx( hKey, NT_BIOS_VERSION_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
			{
				// Parse multistring registry value
				CString strResult;
				LPCTSTR	pSZ = ParseMultiSZ(szValue);
				while (pSZ != NULL)
				{
					strResult += pSZ;
					strResult += " ";
					pSZ = ParseMultiSZ();
				}
				strBiosVersion = strResult;
			}

			// Get BIOS date
			dwType = REG_SZ;
			dwSize = 255;
			if (RegQueryValueEx( hKey, NT_BIOS_DATE_VALUE, NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
				strBiosDate = szValue;

			// Close the main key
			RegCloseKey(hKey);

			// Now store the recovered information back into the supplied BIOS object
			SystemManufacturer(strManufacturer);
			SystemModel(strModel);
			SystemSerialNumber(strSerial);
			BiosManufacturer(strBiosManufacturer);
			BiosDate(strBiosDate);
			BiosVersion(strBiosVersion);
			UUID("NONWMI");
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CSystemBiosScanner::ScanRegistryXP End" ,true);
	return true;
}



//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CSystemBiosScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CSystemBiosScanner::SaveData Start"  ,true);

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	// Each audited item gets added an a CAuditDataFileItem to the category
	CAuditDataFileItem b1(V_BIOS_INTERNAL_SYSTEMID ,m_strSystemUniqueID);
	CAuditDataFileItem b2(V_BIOS_SYSTEM_MANUFACTURER ,m_strSystemManufacturer);
	CAuditDataFileItem b3(V_BIOS_SYSTEM_MODEL ,m_strSystemModel);
	CAuditDataFileItem b4(V_BIOS_SYSTEM_SERIAL ,m_strSystemSerialNumber);
	CAuditDataFileItem b5(V_BIOS_MANUFACTURER ,m_strBiosManufacturer);
	CAuditDataFileItem b6(V_BIOS_VERSION ,m_strBiosVersion);
	CAuditDataFileItem b7(V_BIOS_DATE ,m_strBiosDate ,"" ,CAuditDataFileItem::date);
	CAuditDataFileItem b8(V_BIOS_ASSETTAG, m_strAssetTag);

	// Add the items to the category
	category.AddItem(b1);
	category.AddItem(b2);
	category.AddItem(b3);
	category.AddItem(b4);
	category.AddItem(b5);
	category.AddItem(b6);
	category.AddItem(b7);
	category.AddItem(b8);

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);

	log.Write("CSystemBiosScanner::SaveData End" ,true);
	return true;
}

