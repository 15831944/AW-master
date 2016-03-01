//////////////////////////////////////////////////////////////////////////////////////
//																					//
//    CAuditDataScanner																//
//    =================																//
//																					//
//////////////////////////////////////////////////////////////////////////////////////
//																					//
//	This class is the base class from which all other audit scanners should be		//
//	derived.  It handles the detection of the specific OS and from there audits		//
//	the specific item first trying a WMI approach and if that fails then trying		//
//	to recover the data from the system registry									//
//																					//
//	Note that this is a pure virtual class - all derived classes must implement the //
//	specific scanning functions which do the actual work							//
//																					//
//	Derived classes will recover specific type of data for example					//
//																					//
//	CGraphicsAdaptersScanner	- Returns information on graphics adapters			//
//	CNetworkAdaptersScanner		- Scans network adapters							//
//	CNetworkInformationScanner	- Returns information about the network settings	//
//	CSystemBiosScanner			- Returns information about the system bios			//
//	CSystemProcessorScanner		- Returns information on the processor installed	//
//																					//
//	Each derived class must implement the following pure virtual functions			//
//																					//
//	ScanWMI					- Recover data using WMI								//
//	ScanRegistry9X			- Recover data using a registry scan under Windows 9x	//
//	ScanRegistryNT			- Recover data using a registry scan under Windows NT	//
//	ScanRegistryXP			- Recover data using a registry scan under Windows XP	//
//	Save					- Save the resultant data to the audit data file		//
//																					//
//////////////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "WMIScanner.h"
#include "AuditDataFile.h"
#include "AuditDataScanner.h"



//////////////////////////////////////////////////////////////////////////////////////
//
//    CAuditDataScanner Constructor
//
CAuditDataScanner::CAuditDataScanner()
{
	CLogFile log;

	// We need to have an idea of the operating system which we are running on - not interested in
	// specifics just the platform
	OSVERSIONINFO	osVersion;
	osVersion.dwOSVersionInfoSize = sizeof( OSVERSIONINFO);
	if (GetVersionEx( &osVersion))
	{
		m_dwPlatformId = osVersion.dwPlatformId;

		// XP (or higher) is denoted by being on an NT platform with
		// a version greater or equal to 5.1
		m_bIsXP = (osVersion.dwPlatformId == VER_PLATFORM_WIN32_NT) 
			    && (((osVersion.dwMajorVersion == 5) && (osVersion.dwMinorVersion >= 1)) 
				||  (osVersion.dwMajorVersion > 5));
	}
	else
	{
		// Failed so assume NT based, but not XP or later
		log.Write("failed to determine OS, assuming NT based.");
		m_dwPlatformId = VER_PLATFORM_WIN32_NT;
		m_bIsXP = FALSE;
	}

	// Initialize the registry hive 
	m_hKey = HKEY_LOCAL_MACHINE;
}

CAuditDataScanner::~CAuditDataScanner(void)
{
}


//
//    Scan
//    ====
//
//    This is the main scan function - it simply calls the appropriate scan function.
//
bool CAuditDataScanner::Scan(CWMIScanner* pScanner)
{
	CLogFile log;
	bool bScanStatus = false;

	try
	{
		//pScanner->Connect();

		if (pScanner != NULL && pScanner->IsConnected())
			bScanStatus = ScanWMI(pScanner);

		if (!bScanStatus)
		{
			if (m_bIsXP)
			{
				log.Write("could not use local WMI, scanning registry instead. (OS is XP or higher)");
				bScanStatus = ScanRegistryXP();		
			}
			else
			{
				log.Write("could not use local WMI, scanning registry instead. (OS is NT)");
				bScanStatus = ScanRegistryNT();		
			}
		}

		else
		{
			log.Write("scanned using local WMI");
		}

		// Call the exception scanner to pick up any additional information
		ScanExceptions();
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CAuditDataScanner::Scan\n");
		return false;
	}

	return bScanStatus;
}


//
// Log an exception error
//
void CAuditDataScanner::LogException (CException* pEx, LPCSTR function)
{
	CLogFile log;
   	TCHAR   szCause[255];
   	pEx->GetErrorMessage(szCause, 255);
	log.Format("An exception occured during %s - the message text was %s\n" ,function ,szCause);
   	pEx->Delete();
}


