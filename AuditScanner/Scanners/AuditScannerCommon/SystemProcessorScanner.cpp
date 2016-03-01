#include "stdafx.h"
#include "AuditDataFile.h"
#include "SystemProcessorScanner.h"

// Registry key mappings for System Processor
#define NT_PROCESSOR_KEY	"Hardware\\Description\\System\\CentralProcessor"
#define WIN_PROCESSOR_KEY	"Hardware\\Description\\System\\CentralProcessor"

// Storage Strings
#define HARDWARE_CLASS				"Hardware|CPU"
#define V_PROCESSOR_NAME			"Name"
#define V_PROCESSOR_SPEED			"Speed"
#define V_PROCESSOR_SOCKET			"Socket"
#define V_PROCESSOR_VOLTAGE			"Voltage"
#define V_PROCESSOR_NUMBERCORES		"Core Count"
#define V_PROCESSOR_NUMBERPROCS		"Processor Count"
#define V_PROCESSOR_ARCHITECTURE	"Architecture"
#define V_PROCESSOR_SYSTEMTYPE		"System Type"
#define V_PROCESSOR_BUSTYPE			"Bus Type"

// 
// Constructor - simply calls base class constructor to perform the scan
//
CSystemProcessorScanner::CSystemProcessorScanner() : CAuditDataScanner()
{
	m_strItemPath = HARDWARE_CLASS;
}

CSystemProcessorScanner::~CSystemProcessorScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CSystemProcessorScanner::ScanWMI(CWMIScanner *pScanner)
{
	//return false;
	CLogFile log;
	log.Write("CSystemProcessorScanner::ScanWMI Start" ,true);

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Processor information is held within the Win32_Processor 
	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Processor"))
		{
			// Loop through - we only list details for the first processor but we do need to get
			// the count of processors in this system
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Get Details from the Win32_Processor
				CString socket = wmiConnection.GetClassObjectStringValue("SocketDesignation");
				Socket(socket);
				Voltage(wmiConnection.GetClassObjectDwordValue("CurrentVoltage"));

				// Processor Architecture																	// 8.3.4 - CMD
				int architecture = wmiConnection.GetClassObjectDwordValue("Architecture");
				CString architectureName;
				switch (architecture)
				{
				case 0:
					architectureName = "x86";
					break;
				case 1:
					architectureName = "MIPS";
					break;
				case 2:
					architectureName = "Alpha";
					break;
				case 3:
					architectureName = "PowerPC";
					break;
				case 6:
					architectureName = "Itanium-based system";
					break;
				case 9:
					architectureName = "x64";
					break;
				default:
					architectureName = "unknown";
				}
				Architecture(architectureName);

				// Address width gives us the OS system type - 32 or 64 bit windows							// 8.3.4 - CMD
				int addressWidth = wmiConnection.GetClassObjectDwordValue("AddressWidth");
				CString systemType = (addressWidth == 32) ? "32-Bit Operating System" : "64-Bit Operating System";
				SystemType(systemType);
			}
			wmiConnection.CloseEnumClassObject();
		}

		if (wmiConnection.BeginEnumClassObject("Win32_ComputerSystem"))
		{
			// Loop through - we only list details for the first processor but we do need to get
			// the count of processors in this system
			while (wmiConnection.MoveNextEnumClassObject())
			{				
				Processors(wmiConnection.GetClassObjectDwordValue("NumberOfProcessors"));
				Cores(wmiConnection.GetClassObjectDwordValue("NumberOfLogicalProcessors"));
			}
			wmiConnection.CloseEnumClassObject();
		}

		if (wmiConnection.BeginEnumClassObject("Win32_MotherboardDevice"))
		{
			// Loop through - we only list details for the first processor but we do need to get
			// the count of processors in this system
			while (wmiConnection.MoveNextEnumClassObject())
			{
				CString primaryBusType = wmiConnection.GetClassObjectStringValue("PrimaryBusType");
				CString secondaryBusType = wmiConnection.GetClassObjectStringValue("SecondaryBusType");

				if (secondaryBusType != "")
					primaryBusType += ", " + secondaryBusType;

				BusType(primaryBusType);
			}

			wmiConnection.CloseEnumClassObject();
		}

		ScanRegistryForNameSpeed();
	}

	catch (CException *pEx)
	{
		return false;
	}

	log.Write("CSystemProcessorScanner::ScanWMI End" ,true);
	return true;
}


//
//    ScanRegistryForNameSpeed
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CSystemProcessorScanner::ScanRegistryForNameSpeed()
{
	CLogFile log;
	log.Write("CSystemProcessorScanner::ScanRegistryXP Start" ,true);

	// Detect processor...
	CCpuType cpu;

	try
	{	
		// Manufacturer & model - this is now rationalized to be more generic

		// first try to get the CPU name from the registry
		HKEY	hKey = NULL;
		TCHAR	szValue[256];
		LONG	lResult;
		DWORD dwType = REG_SZ;
		DWORD dwSize = 255;
		CString processorName = "";

		lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", 0, KEY_READ, &hKey);
		if (lResult == ERROR_SUCCESS)
		{
			if (RegQueryValueEx( hKey, "ProcessorNameString", NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
			{
				processorName = szValue;
				RegCloseKey(hKey);
				log.Format("found CPU name from registry, value before rationalise: %s \n", processorName);
				processorName = cpu.RationalizeName(processorName);
				log.Format("value after rationalise: %s \n", processorName);
			}
		}

		if (processorName == "")
		{
			// - if we didn't find a process name from the registry then use the old method
			// - TODO - this needs to be improved as it can sometimes report the wrong CPU name
			log.Write("unable to find CPU from registry, using GetRationalizedName()", true);
			processorName = cpu.GetRationalizedName();
			log.Format("value from GetRationalizedName(): %s \n", processorName);
		}

		Name(processorName);

		// Get Processor speed
		CCpuSpeed speed;
		Speed(speed.GetMHz());
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CSystemProcessorScanner::ScanRegistryXP End" ,true);
	return true;
}



//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT / 2000
//
bool	CSystemProcessorScanner::ScanRegistryNT()
{
	return ScanRegistryXP();
}


//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CSystemProcessorScanner::ScanRegistryXP()
{
	CLogFile log;
	log.Write("CSystemProcessorScanner::ScanRegistryXP Start" ,true);

	// Detect processor...
	CCpuType cpu;

	try
	{	
		// Manufacturer & model - this is now rationalized to be more generic

		// first try to get the CPU name from the registry
		HKEY	hKey = NULL;
		TCHAR	szValue[256];
		LONG	lResult;
		DWORD dwType = REG_SZ;
		DWORD dwSize = 255;
		CString processorName = "";

		lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", 0, KEY_READ, &hKey);
		if (lResult == ERROR_SUCCESS)
		{
			if (RegQueryValueEx( hKey, "ProcessorNameString", NULL, &dwType, (LPBYTE) szValue, &dwSize) == ERROR_SUCCESS)
			{
				processorName = szValue;
				RegCloseKey(hKey);
				log.Format("found CPU name from registry, value before rationalise: %s \n", processorName);
				processorName = cpu.RationalizeName(processorName);
				log.Format("value after rationalise: %s \n", processorName);
			}
		}

		if (processorName == "")
		{
			// - if we didn't find a process name from the registry then use the old method
			// - TODO - this needs to be improved as it can sometimes report the wrong CPU name
			log.Write("unable to find CPU from registry, using GetRationalizedName()", true);
			processorName = cpu.GetRationalizedName();
			log.Format("value from GetRationalizedName(): %s \n", processorName);
		}

		Name(processorName);

		// Get Processor speed
		CCpuSpeed speed;
		Speed(speed.GetMHz());

		// CPU count is recovered from the system registry by checking to see how many entries are beneath
		// the main processor key
		int nCpuCount = 1;				// Assume a single processor
		CReg processorKey;
		if (processorKey.Open(HKEY_LOCAL_MACHINE, NT_PROCESSOR_KEY))
		{
			// enumerate all subkeys then examine them
			CDynaList<CString> subKeys;
			processorKey.EnumSubKeys(subKeys);
			nCpuCount = subKeys.GetCount();
			processorKey.Close();
		}

		// Store the count
		//Count(nCpuCount);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CSystemProcessorScanner::ScanRegistryXP End" ,true);
	return true;
}


//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
//    Note we actually use the NT function as the registry key is the same
//
bool	CSystemProcessorScanner::ScanRegistry9X()
{
	return ScanRegistryNT();
}



//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CSystemProcessorScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CSystemProcessorScanner::SaveData Start" ,true);

	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	// Each audited item gets added an a CAuditDataFileItem to the category
	CAuditDataFileItem p1(V_PROCESSOR_NAME ,m_strName);
	CAuditDataFileItem p2(V_PROCESSOR_SPEED ,m_nSpeed ," Mhz");
	CAuditDataFileItem p4(V_PROCESSOR_SOCKET ,m_strSocket);
	CAuditDataFileItem p5(V_PROCESSOR_VOLTAGE ,m_nVoltage, "V");
	CAuditDataFileItem p6(V_PROCESSOR_NUMBERPROCS ,m_nProcessors);
	CAuditDataFileItem p7(V_PROCESSOR_NUMBERCORES ,m_nCores);
	CAuditDataFileItem p8(V_PROCESSOR_BUSTYPE ,m_strBusType);
	CAuditDataFileItem p9(V_PROCESSOR_ARCHITECTURE ,m_strArchitecture);							// 8.3.4 - CMD
	CAuditDataFileItem p10(V_PROCESSOR_SYSTEMTYPE ,m_strSystemType);							// 8.3.4 - CMD

	// Add the items to the category
	category.AddItem(p1);
	category.AddItem(p2);
	category.AddItem(p4);
	category.AddItem(p5);
	category.AddItem(p6);
	category.AddItem(p7);
	category.AddItem(p8);
	category.AddItem(p9);																		// 8.3.4 - CMD
	category.AddItem(p10);																		// 8.3.4 - CMD

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);
	log.Write("CSystemProcessorScanner::SaveData End" ,true);
	return true;
}