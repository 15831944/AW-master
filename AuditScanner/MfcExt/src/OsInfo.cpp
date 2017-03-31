/
// FILE:	OsInfo.cpp
// PURPOSE:	Class for detection of Operating System details
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)
//			CMD	 - 03.12.2001 - support for Windows XP
//			JRFT - 17.03.2003 - Support for Windows 2000/XP/Server 2003, including detection of home/pro build etc
//
// NOTES:	The copy of winnt.h that ships with DevStudio 6.0 does not include the latest definition of the version
//			info structures & flags, which are needed to detect the latest OS's. Hence these have been added into 
//			this source file. When this is later ported to DevStudio 7 these definitions should be removed.

#include "stdafx.h"
#include <windows.h>
#include <tchar.h>
#include <stdio.h>
#include <strsafe.h>

#pragma comment(lib, "User32.lib")

#define BUFSIZE 256

typedef void (WINAPI *PGNSI)(LPSYSTEM_INFO);
typedef BOOL (WINAPI *PGPI)(DWORD, DWORD, DWORD, DWORD, PDWORD);

// Registry Key for NT system type, contains either "Winnt", "Servernt" or "Lanmannt"
#define RK_NT_TYPE	"System\\CurrentControlSet\\Control\\ProductOptions"
#define RI_NT_TYPE	"Product Type"

// Registry Key for obtaining OS serial number under win95 / WinNT platforms and derivatives
#define RK_95_VERSION	"SOFTWARE\\Microsoft\\Windows\\CurrentVersion"
#define RK_NT_VERSION	"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"
#define RI_OS_SERIAL	"ProductId"
#define RI_OS_NAME      "ProductName"

/*
** initialise static variables
*/
CString	COsInfo::m_strName;
CString	COsInfo::m_strVersion;
CString	COsInfo::m_strProductID;
CString	COsInfo::m_strCDKey;
int		COsInfo::m_nClass = COsInfo::unknown;
BOOL	COsInfo::m_bIs64BitWindows = FALSE;
const char gszDigitalProductID[]	= "DigitalProductID";

// in case these aren't defined in the system header files
#ifndef SM_TABLETPC
    #define SM_TABLETPC                     86
#endif

#ifndef SM_MEDIACENTER
    #define SM_MEDIACENTER                  87
#endif

#ifndef SM_STARTER
	#define SM_STARTER						88
#endif

#ifndef SM_SERVERR2
	#define SM_SERVERR2						89
#endif

//Flags returned from GetProductInfo - mainly for Windows Vista (aka Longhorn)
#ifndef PRODUCT_BUSINESS
#define PRODUCT_BUSINESS 0x00000006
#endif

#ifndef PRODUCT_BUSINESS_N
#define PRODUCT_BUSINESS_N 0x00000010
#endif

#ifndef PRODUCT_HOME_BASIC
#define PRODUCT_HOME_BASIC 0x00000002
#endif

#ifndef PRODUCT_HOME_BASIC_N
#define PRODUCT_HOME_BASIC_N 0x00000005
#endif

#ifndef PRODUCT_HOME_PREMIUM
#define PRODUCT_HOME_PREMIUM 0x00000003
#endif 

#ifndef PRODUCT_STARTER
#define PRODUCT_STARTER 0x0000000B
#endif

#ifndef PRODUCT_ENTERPRISE
#define PRODUCT_ENTERPRISE 0x00000004
#endif 

#ifndef PRODUCT_ULTIMATE
#define PRODUCT_ULTIMATE 0x00000001
#endif


// Windows 64 function
typedef BOOL (WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);

/*
** Constructor
*/
COsInfo::COsInfo()
{
	// only collect all the data once per application instance
	if (unknown == m_nClass)
		Detect();
}

/*
** Destructor
*/
COsInfo::~COsInfo()
{
}


#define PRODUCT_UNDEFINED                       0x00000000
#define PRODUCT_ULTIMATE                        0x00000001
#define PRODUCT_HOME_BASIC                      0x00000002
#define PRODUCT_HOME_PREMIUM                    0x00000003
#define PRODUCT_ENTERPRISE                      0x00000004
#define PRODUCT_HOME_BASIC_N                    0x00000005
#define PRODUCT_BUSINESS                        0x00000006
#define PRODUCT_STANDARD_SERVER                 0x00000007
#define PRODUCT_DATACENTER_SERVER               0x00000008
#define PRODUCT_SMALLBUSINESS_SERVER            0x00000009
#define PRODUCT_ENTERPRISE_SERVER               0x0000000A
#define PRODUCT_STARTER                         0x0000000B
#define PRODUCT_DATACENTER_SERVER_CORE          0x0000000C
#define PRODUCT_STANDARD_SERVER_CORE            0x0000000D
#define PRODUCT_ENTERPRISE_SERVER_CORE          0x0000000E
#define PRODUCT_ENTERPRISE_SERVER_IA64          0x0000000F
#define PRODUCT_BUSINESS_N                      0x00000010
#define PRODUCT_WEB_SERVER                      0x00000011
#define PRODUCT_CLUSTER_SERVER                  0x00000012
#define PRODUCT_HOME_SERVER                     0x00000013
#define PRODUCT_STORAGE_EXPRESS_SERVER          0x00000014
#define PRODUCT_STORAGE_STANDARD_SERVER         0x00000015
#define PRODUCT_STORAGE_WORKGROUP_SERVER        0x00000016
#define PRODUCT_STORAGE_ENTERPRISE_SERVER       0x00000017
#define PRODUCT_SERVER_FOR_SMALLBUSINESS        0x00000018
#define PRODUCT_SMALLBUSINESS_SERVER_PREMIUM    0x00000019

#define PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT	0x0000001E
#define PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY		0x0000001F
#define PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING		0x00000020
#define PRODUCT_SERVER_FOUNDATION					0x00000021
#define PRODUCT_SERVER_FOR_SMALLBUSINESS_V			0x00000023
#define PRODUCT_STANDARD_SERVER_V					0x00000024
#define PRODUCT_STORAGE_EXPRESS_SERVER_CORE			0x00000028
#define PRODUCT_STANDARD_SERVER_CORE_V				0x0000002B
#define PRODUCT_STORAGE_STANDARD_SERVER_CORE		0x0000002C
#define PRODUCT_STORAGE_WORKGROUP_SERVER_CORE		0x0000002D
#define PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE		0x0000002E
#define PRODUCT_STARTER_N							0x0000002F
#define PRODUCT_PROFESSIONAL						0x00000030
#define PRODUCT_PROFESSIONAL_N						0x00000031
#define PRODUCT_SB_SOLUTION_SERVER					0x00000032
#define PRODUCT_SERVER_FOR_SB_SOLUTIONS				0x00000033
#define PRODUCT_STANDARD_SERVER_SOLUTIONS			0x00000034
#define PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE		0x00000035
#define PRODUCT_SB_SOLUTION_SERVER_EM				0x00000036
#define PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM			0x00000037
#define PRODUCT_SOLUTION_EMBEDDEDSERVER				0x00000038
#define PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE	0x0000003F
#define PRODUCT_PROFESSIONAL_WMC					0x00000067
#define PRODUCT_MULTIPOINT_STANDARD_SERVER			0x0000004C
#define PRODUCT_MULTIPOINT_PREMIUM_SERVER			0x0000004D
#define PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER	0x0000005F
#define PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER	0x00000060



#define PRODUCT_UNLICENSED                      0xABCDABCD
#define PROCESSOR_ARCHITECTURE_AMD64 9
#define SM_SERVERR2 89
#define VER_SUITE_STORAGE_SERVER				0x00002000
#define VER_SUITE_COMPUTE_SERVER				0x00004000
#define VER_SUITE_WH_SERVER						0x00008000
#define VER_SUITE_BLADE 0x00000400
#define VER_SUITE_PERSONAL 0x00000200


/*
** This hack is 'cos I haven't got a new enough edition of the header files.
** When a later DevStudio version is installed this can be deleted and replaced
** by a reference to the standard OSVERSIONINFO structure
*/
typedef struct _OSVERINFOEX
{
    DWORD dwOSVersionInfoSize;
    DWORD dwMajorVersion;
    DWORD dwMinorVersion;
    DWORD dwBuildNumber;
    DWORD dwPlatformId;
    CHAR   szCSDVersion[ 128 ];     // Maintenance string for PSS usage
    WORD wServicePackMajor;
    WORD wServicePackMinor;
	WORD wSuiteMask;
	BYTE bProductType;
	BYTE bReserved;
} OSVERINFOEX;

// possible flags for wSuiteMask
#ifndef VER_SUITE_SMALLBUSINESS
#define VER_SUITE_SMALLBUSINESS				0x00000001
#define VER_SUITE_ENTERPRISE				0x00000002
#define VER_SUITE_BACKOFFICE				0x00000004
#define VER_SUITE_COMMUNICATIONS			0x00000008
#define VER_SUITE_TERMINAL					0x00000010
#define VER_SUITE_SMALLBUSINESS_RESTRICTED	0x00000020
#define VER_SUITE_EMBEDDEDNT				0x00000040
#define VER_SUITE_DATACENTER				0x00000080
#define VER_SUITE_SINGLEUSERTS				0x00000100
#define VER_SUITE_PERSONAL					0x00000200
#define VER_SUITE_BLADE						0x00000400
#endif

// possible values for bProductType
#ifndef VER_NT_WORKSTATION
#define VER_NT_WORKSTATION					0x00000001
#define VER_NT_DOMAIN_CONTROLLER			0x00000002
#define VER_NT_SERVER						0x00000003
#endif 

/*
** Detect current operating system and store in member variables
*/
BOOL COsInfo::Detect()
{
	OSVERSIONINFOEX osvi;
	SYSTEM_INFO si;
	PGNSI pGNSI;
	PGPI pGPI;
	BOOL bOsVersionInfoEx;
	DWORD dwType;

	// Initialize Buffers
	ZeroMemory(&si, sizeof(SYSTEM_INFO));
	ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
	m_strName = "";
	CString version ="12312";
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);

	if (!(bOsVersionInfoEx = GetVersionEx ((OSVERSIONINFO *) &osvi)))
		return 1;

	// Call GetNativeSystemInfo if supported or GetSystemInfo otherwise.
	pGNSI = (PGNSI) GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetNativeSystemInfo");
	if (pGNSI != NULL)
		pGNSI(&si);
	else 
		GetSystemInfo(&si);

	
	// Check if he windows versio is windows 10 or not
	CString versiondetails = CReg::GetItemString (HKEY_LOCAL_MACHINE, RK_NT_VERSION,RI_OS_NAME);	
	if (versiondetails.Find("Windows 10") >= 0 || versiondetails.Find("2016") >= 0)
	{
		osvi.dwMajorVersion = 10;
	}
	else if(versiondetails.Find("Windows 8.1") >= 0 || versiondetails.Find("2012 R2") >= 0)
	{
		osvi.dwMinorVersion = 3;
	}
	// If the platform type is WINDOWS then we are dealing with very early versions indeed
	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
	{
		// it's a win95 derivative, but which one ?
		switch (osvi.dwMinorVersion)
		{
			case 10:
				m_nClass = win98;
				m_strName = "Microsoft Windows 98";
				break;

			case 90:
				m_nClass = winME;
				m_strName = "Microsoft Windows ME";
				break;

			default:
				m_nClass = win95;
				m_strName = "Microsoft Windows 95";
				break;
		}
	}

	else if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT && osvi.dwMajorVersion > 4 )
	{
		m_strName = "Microsoft ";

		if ( osvi.dwMajorVersion == 6 )
		{
			m_iMajorVersion = 6;
			m_nClass = winXP;

			if (osvi.dwMinorVersion == 0)
			{
				if ( osvi.wProductType == VER_NT_WORKSTATION )
					m_strName += "Windows Vista ";
				else 
					m_strName += "Windows Server 2008 ";
			}
			else if (osvi.dwMinorVersion == 1)
			{
				if (osvi.wProductType == VER_NT_WORKSTATION )
					m_strName += "Windows 7 ";
				else
					m_strName += "Windows Server 2008 R2 ";
			}
			else if (osvi.dwMinorVersion == 2)
			{
				if (osvi.wProductType == VER_NT_WORKSTATION )
					m_strName += "Windows 8 ";
				else
					m_strName += "Windows Server 2012 ";
			}
			else if (osvi.dwMinorVersion == 3)
			{
				if (osvi.wProductType == VER_NT_WORKSTATION )
					m_strName += "Windows 8.1 ";
				else
					m_strName += "Windows Server 2012 R2 ";
			}

			// Now get the varient
			pGPI = (PGPI) GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetProductInfo");
			pGPI( osvi.dwMajorVersion, osvi.dwMinorVersion, 0, 0, &dwType);
			switch (dwType)
			{
				case PRODUCT_ULTIMATE:
					m_strName += "Ultimate Edition";
					break;
				case PRODUCT_HOME_PREMIUM:
					m_strName += "Home Premium Edition";
					break;
				case PRODUCT_HOME_BASIC:
					m_strName += "Home Basic Edition";
					break;
				case PRODUCT_ENTERPRISE:
					m_strName += "Enterprise Edition";
					break;
				case PRODUCT_BUSINESS:
					m_strName += "Business Edition";
					break;
				case PRODUCT_STARTER:
					m_strName += "Starter Edition";
					break;
				case PRODUCT_CLUSTER_SERVER:
					m_strName += "Cluster Server Edition";
					break;
				case PRODUCT_DATACENTER_SERVER:
					m_strName += "Datacenter Edition";
					break;
				case PRODUCT_DATACENTER_SERVER_CORE:
					m_strName += "Datacenter Edition (core installation)";
					break;
				case PRODUCT_ENTERPRISE_SERVER:
					m_strName += "Enterprise Edition";
					break;
				case PRODUCT_ENTERPRISE_SERVER_CORE:
					m_strName += "Enterprise Edition (core installation)";
					break;
				case PRODUCT_ENTERPRISE_SERVER_IA64:
					m_strName += "Enterprise Edition for Itanium-based Systems";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER:
					m_strName += "Small Business Server";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
					m_strName += "Small Business Server Premium Edition";
					break;
				case PRODUCT_STANDARD_SERVER:
					m_strName += "Standard Edition";
					break;
				case PRODUCT_STANDARD_SERVER_CORE:
					m_strName += "Standard Edition (core installation)";
					break;
				case PRODUCT_WEB_SERVER:
					m_strName += "Web Server Edition";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
					m_strName += "Windows Essential Business Server Management Server";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
					m_strName += "Windows Essential Business Server Security Server";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
					m_strName += "Windows Essential Business Server Messaging Server";
					break;
				case PRODUCT_SERVER_FOUNDATION:
					m_strName += "Server Foundation";
					break;
				case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
					m_strName += "Windows Server 2008 without Hyper-V for Windows Essential Server Solutions";
					break;
				case PRODUCT_STANDARD_SERVER_V:
					m_strName += "Server Standard without Hyper-V";
					break;
				case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
					m_strName += "Server Standard without Hyper-V (core installation)";
					break;
				case PRODUCT_STANDARD_SERVER_CORE_V:
					m_strName += "Storage Server Express (core installation)";
					break;
				case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
					m_strName += "Storage Server Standard (core installation)";
					break;
				case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
					m_strName += "Storage Server Workgroup (core installation)";
					break;
				case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
					m_strName += "Storage Server Enterprise (core installation)";
					break;
				case PRODUCT_STARTER_N:
					m_strName += "Starter N";
					break;
				case PRODUCT_PROFESSIONAL:
					m_strName += "Professional";
					break;
				case PRODUCT_PROFESSIONAL_N:
					m_strName += "Professional N";
					break;
				case PRODUCT_SB_SOLUTION_SERVER:
					m_strName += "Windows Small Business Server 2011 Essentials";
					break;
				case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
					m_strName += "Server For SB Solutions";
					break;
				case PRODUCT_STANDARD_SERVER_SOLUTIONS:
					m_strName += "Server Solutions Premium";
					break;
				case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
					m_strName += "Server Solutions Premium (core installation)";
					break;
				case PRODUCT_SB_SOLUTION_SERVER_EM:
					m_strName += "Server For SB Solutions EM";
					break;
				case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
					m_strName += "Server For SB Solutions EM";
					break;
				case PRODUCT_SOLUTION_EMBEDDEDSERVER:
					m_strName += "Windows MultiPoint Server";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
					m_strName += "Small Business Server Premium (core installation)";
					break;
				case PRODUCT_PROFESSIONAL_WMC:
					m_strName += "Professional with Media Center";
					break;
				case PRODUCT_MULTIPOINT_STANDARD_SERVER:
					m_strName += "Web Server Edition";
					break;
				case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
					m_strName += "Windows MultiPoint Server Standard (full installation)";
					break;
				case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
					m_strName += "Storage Server Workgroup (evaluation installation)";
					break;
				case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
					m_strName += "Storage Server Standard (evaluation installation)";
					break;
			}

			if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64 )
				m_strName +=  ", 64-bit";
			else if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_INTEL )
				m_strName += ", 32-bit";
		}
		else if(osvi.dwMajorVersion == 10)
		{							
			if (osvi.wProductType == VER_NT_WORKSTATION )
				m_strName += "Windows 10 ";
			else
				m_strName += "Windows Server 2016 ";

			// Now get the varient
			pGPI = (PGPI) GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), "GetProductInfo");
			pGPI( osvi.dwMajorVersion, osvi.dwMinorVersion, 0, 0, &dwType);

			switch (dwType)
			{
				case PRODUCT_ULTIMATE:
					m_strName += "Ultimate Edition";
					break;
				case PRODUCT_HOME_PREMIUM:
					m_strName += "Home Premium Edition";
					break;
				case PRODUCT_HOME_BASIC:
					m_strName += "Home Basic Edition";
					break;
				case PRODUCT_ENTERPRISE:
					m_strName += "Enterprise Edition";
					break;
				case PRODUCT_BUSINESS:
					m_strName += "Business Edition";
					break;
				case PRODUCT_STARTER:
					m_strName += "Starter Edition";
					break;
				case PRODUCT_CLUSTER_SERVER:
					m_strName += "Cluster Server Edition";
					break;
				case PRODUCT_DATACENTER_SERVER:
					m_strName += "Datacenter Edition";
					break;
				case PRODUCT_DATACENTER_SERVER_CORE:
					m_strName += "Datacenter Edition (core installation)";
					break;
				case PRODUCT_ENTERPRISE_SERVER:
					m_strName += "Enterprise Edition";
					break;
				case PRODUCT_ENTERPRISE_SERVER_CORE:
					m_strName += "Enterprise Edition (core installation)";
					break;
				case PRODUCT_ENTERPRISE_SERVER_IA64:
					m_strName += "Enterprise Edition for Itanium-based Systems";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER:
					m_strName += "Small Business Server";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
					m_strName += "Small Business Server Premium Edition";
					break;
				case PRODUCT_STANDARD_SERVER:
					m_strName += "Standard Edition";
					break;
				case PRODUCT_STANDARD_SERVER_CORE:
					m_strName += "Standard Edition (core installation)";
					break;
				case PRODUCT_WEB_SERVER:
					m_strName += "Web Server Edition";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
					m_strName += "Windows Essential Business Server Management Server";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
					m_strName += "Windows Essential Business Server Security Server";
					break;
				case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
					m_strName += "Windows Essential Business Server Messaging Server";
					break;
				case PRODUCT_SERVER_FOUNDATION:
					m_strName += "Server Foundation";
					break;
				case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
					m_strName += "Windows Server 2008 without Hyper-V for Windows Essential Server Solutions";
					break;
				case PRODUCT_STANDARD_SERVER_V:
					m_strName += "Server Standard without Hyper-V";
					break;
				case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
					m_strName += "Server Standard without Hyper-V (core installation)";
					break;
				case PRODUCT_STANDARD_SERVER_CORE_V:
					m_strName += "Storage Server Express (core installation)";
					break;
				case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
					m_strName += "Storage Server Standard (core installation)";
					break;
				case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
					m_strName += "Storage Server Workgroup (core installation)";
					break;
				case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
					m_strName += "Storage Server Enterprise (core installation)";
					break;
				case PRODUCT_STARTER_N:
					m_strName += "Starter N";
					break;
				case PRODUCT_PROFESSIONAL:
					m_strName += "Professional";
					break;
				case PRODUCT_PROFESSIONAL_N:
					m_strName += "Professional N";
					break;
				case PRODUCT_SB_SOLUTION_SERVER:
					m_strName += "Windows Small Business Server 2011 Essentials";
					break;
				case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
					m_strName += "Server For SB Solutions";
					break;
				case PRODUCT_STANDARD_SERVER_SOLUTIONS:
					m_strName += "Server Solutions Premium";
					break;
				case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
					m_strName += "Server Solutions Premium (core installation)";
					break;
				case PRODUCT_SB_SOLUTION_SERVER_EM:
					m_strName += "Server For SB Solutions EM";
					break;
				case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
					m_strName += "Server For SB Solutions EM";
					break;
				case PRODUCT_SOLUTION_EMBEDDEDSERVER:
					m_strName += "Windows MultiPoint Server";
					break;
				case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
					m_strName += "Small Business Server Premium (core installation)";
					break;
				case PRODUCT_PROFESSIONAL_WMC:
					m_strName += "Professional with Media Center";
					break;
				case PRODUCT_MULTIPOINT_STANDARD_SERVER:
					m_strName += "Web Server Edition";
					break;
				case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
					m_strName += "Windows MultiPoint Server Standard (full installation)";
					break;
				case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
					m_strName += "Storage Server Workgroup (evaluation installation)";
					break;
				case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
					m_strName += "Storage Server Standard (evaluation installation)";
					break;
			}

			if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64 )
				m_strName +=  ", 64-bit";
			else if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_INTEL )
				m_strName += ", 32-bit";

		}

		// Versiopn 5.3 is Windows Server 2003 variants (including XP Pro x64)
		else if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 2 )
		{
			m_nClass = winXP;
			if (GetSystemMetrics(SM_SERVERR2))
				m_strName +=  "Windows Server 2003 R2, ";
			else if (osvi.wSuiteMask == VER_SUITE_STORAGE_SERVER )
				m_strName +=  "Windows Storage Server 2003";
			else if (osvi.wSuiteMask == VER_SUITE_WH_SERVER )
				m_strName +=  "Windows Home Server";
			else if (osvi.wProductType == VER_NT_WORKSTATION && si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64)
				m_strName +=  "Windows XP Professional x64 Edition";
			else m_strName += "Windows Server 2003, ";

			// Test for the server type.
			if ( osvi.wProductType != VER_NT_WORKSTATION )
			{
				if ( si.wProcessorArchitecture==PROCESSOR_ARCHITECTURE_IA64 )
				{
					if( osvi.wSuiteMask & VER_SUITE_DATACENTER )
						m_strName +=  "Datacenter Edition for Itanium-based Systems";
					else if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
						m_strName +=  "Enterprise Edition for Itanium-based Systems";
				}

				else if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64)
				{
					if ( osvi.wSuiteMask & VER_SUITE_DATACENTER )
						m_strName +=  "Datacenter x64 Edition";
					else if ( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
						m_strName +=  "Enterprise x64 Edition";
					else m_strName +=  "Standard x64 Edition";
				}

				else
				{
					if ( osvi.wSuiteMask & VER_SUITE_COMPUTE_SERVER )
						m_strName +=  "Compute Cluster Edition";
					else if( osvi.wSuiteMask & VER_SUITE_DATACENTER )
						m_strName +=  "Datacenter Edition";
	                else if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
						m_strName +=  "Enterprise Edition";
					else if ( osvi.wSuiteMask & VER_SUITE_BLADE )
						m_strName +=  "Web Edition";
					else m_strName +=  "Standard Edition";
				}
			}
		}

		// Version 5.1 is Windows XP 
		else if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 1 )
		{
			m_nClass = winXP;
			m_strName += "Windows XP ";
			if( osvi.wSuiteMask & VER_SUITE_PERSONAL )
				m_strName +=  "Home Edition";
			else 
				m_strName +=  "Professional";
		}

		// Version 5.0 is Windows 2000
		else if ( osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 0 )
		{
			m_nClass = winXP;
			m_strName += "Windows 2000 ";

			if ( osvi.wProductType == VER_NT_WORKSTATION )
			{
				m_strName +=  "Professional";
			}
			else 
			{
				if( osvi.wSuiteMask & VER_SUITE_DATACENTER )
					m_strName +=  "Datacenter Server";
				else if( osvi.wSuiteMask & VER_SUITE_ENTERPRISE )
					m_strName +=  "Advanced Server";
				else 
					m_strName +=  "Server";
			}
		}

		else if (osvi.dwMajorVersion == 4)
		{
			m_nClass = winNT4;
			m_strName += "Windows NT ";

			if (osvi.wProductType == VER_NT_WORKSTATION)
				m_strName += "Workstation";
		}

		else if (osvi.dwMajorVersion == 3)
		{
			m_nClass = winNT3;
			m_strName += "Windows NT ";
		}
		
		// exact version
		if(osvi.dwMajorVersion == 6 || osvi.dwMajorVersion == 10)
		{
			CString Buildno = CReg::GetItemString (HKEY_LOCAL_MACHINE, RK_NT_VERSION,"CurrentBuild");
			if(!osvi.dwMajorVersion == 10)
				m_strVersion.Format ("%d.%d %s (build %s)", osvi.dwMajorVersion, osvi.dwMinorVersion, osvi.szCSDVersion, Buildno);
			else
				m_strVersion.Format ("%d.%d %s (build %s)", osvi.dwMajorVersion, 0, osvi.szCSDVersion , Buildno);
		}
		else
		{
			m_strVersion.Format ("%d.%d %s (build %d)", osvi.dwMajorVersion, osvi.dwMinorVersion, osvi.szCSDVersion, osvi.dwBuildNumber);
		}

		
	}

	else
	{
		// cannot use GetVersionEx so assume it's Windows NT 3.1
		m_nClass = winNT3;
		m_strName = "MS Windows NT";
		m_strVersion = "3.1";
	}

	// OS serial number
	switch (osvi.dwPlatformId)
	{
		case VER_PLATFORM_WIN32_WINDOWS:
			m_strProductID = CReg::GetItemString (HKEY_LOCAL_MACHINE, RK_95_VERSION, RI_OS_SERIAL);
			break;

		case VER_PLATFORM_WIN32_NT:
			// See if we have a DigitalProductID as this is the actual key that we are interested in
			// as the serial number recovered below is actually an encrypted string
			CByteArray arrayDigitalProductID;
			if (CReg::GetItemBinary (HKEY_LOCAL_MACHINE, RK_NT_VERSION, gszDigitalProductID ,arrayDigitalProductID))
				m_strCDKey = DecodeDigitalProductKey(arrayDigitalProductID);
			m_strProductID = CReg::GetItemString (HKEY_LOCAL_MACHINE, RK_NT_VERSION, RI_OS_SERIAL);
			break;
	}

	// Lastly check to see if this is a 64 bit version of the OS or not												// 8.3.4 - CMD
	m_bIs64BitWindows = FALSE;
	LPFN_ISWOW64PROCESS fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(GetModuleHandle("kernel32"),"IsWow64Process");
    if (NULL != fnIsWow64Process)
        fnIsWow64Process(GetCurrentProcess(),&m_bIs64BitWindows);


	//CLogFile log;

	//// connect to Windows Kernel
	//HMODULE hKernel32 = GetModuleHandle("KERNEL32.DLL");
	//if (NULL == hKernel32)
	//	return FALSE;

	//// attempt to map to the Extended version function
	//typedef int (FAR WINAPI * VERPROC)(LPOSVERSIONINFO);
	//VERPROC pfnGetVersionEx = (VERPROC)GetProcAddress(hKernel32, "GetVersionExA");
	//if (pfnGetVersionEx)
	//{
	//	// we've successfully mapped the function - try running it using the extended structure
	//	OSVERINFOEX osvi;
	//	memset (&osvi, 0, sizeof(OSVERSIONINFOEX));
	//	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);
	//	BOOL bExtended = pfnGetVersionEx((OSVERSIONINFO*)&osvi);
	//	
	//	// if it failed try again using the ordinary version
	//	if (!bExtended)
	//	{
	//		memset (&osvi, 0, sizeof(OSVERSIONINFO));
	//		osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	//		if (!pfnGetVersionEx((OSVERSIONINFO*)&osvi))
	//			return FALSE;
	//	}

	//	log.Format("Major Version is %d  Minor Version is %d  Product Type is %d\n" ,osvi.dwMajorVersion ,osvi.dwMinorVersion ,osvi.bProductType);
	//	
	//	// there are two basic platform types...
	//	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
	//	{
	//		// it's a win95 derivative, but which one ?
	//		switch (osvi.dwMinorVersion)
	//		{
	//			case 10:
	//				m_nClass = win98;
	//				m_strName = "MS Windows 98";
	//				break;

	//			case 90:
	//				m_nClass = winME;
	//				m_strName = "MS Windows ME";
	//				break;

	//			default:
	//				m_nClass = win95;
	//				m_strName = "MS Windows 95";
	//				break;
	//		}
	//	}

	//	
	//	// Specific tests for Windows Vista (V6)
	//	else if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 0)
	//	{
	//		m_nClass = winXP;
	//		if (osvi.bProductType == VER_NT_WORKSTATION)
	//		{
	//			m_strName = "MS Windows Vista";
	//			CString strAdvanced = GetAdvancedProductInfo();
	//			if (!strAdvanced.IsEmpty())
	//				m_strName = m_strName + " " + strAdvanced;
	//		}
	//		else
	//		{
	//			m_strName = "MS Windows Server \"Longhorn\"";
	//		}
	//	}

	//	// Otherwise some other brand of Windows NT 
	//	// Windows NT and derivatives - first try the "easy" checks for Windows XP / 2003...
	//	else
	//	{
	//		if (::GetSystemMetrics (SM_MEDIACENTER))
	//		{
	//			m_nClass = winXP;
	//			m_strName = "Windows XP Media Center Edition";
	//		}
	//	
	//		else if (::GetSystemMetrics (SM_STARTER))
	//		{
	//			m_nClass = winXP;
	//			m_strName = "Windows XP Starter Edition";
	//		}
	//	
	//		else if (::GetSystemMetrics (SM_TABLETPC))
	//		{
	//			m_nClass = winXP;
	//			m_strName = "Windows XP Tablet PC Edition";
	//		}
	//	
	//		else if (::GetSystemMetrics (SM_SERVERR2))
	//		{
	//			m_nClass = winXP;
	//			m_strName = "Windows Server 2003 R2";
	//		}

	//		else
	//		{
	//			if (osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 2)
	//			{
	//				m_nClass = winXP;
	//				m_strName = "MS Windows Server 2003";
	//			}
	//			
	//			if (osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 1)
	//			{
	//				m_nClass = winXP;
	//				m_strName = "MS Windows XP";
	//			}

	//			if (osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 0)
	//			{
	//				m_nClass = win2K;
	//				m_strName = "MS Windows 2000";
	//			}

	//			if (osvi.dwMajorVersion == 4)
	//			{
	//				m_nClass = winNT4;
	//				m_strName = "MS Windows NT";
	//			}

	//			if (osvi.dwMajorVersion == 3)
	//			{
	//				m_nClass = winNT3;
	//				m_strName = "MS Windows NT";
	//			}

	//			// do we have extended info available ?
	//			if (bExtended)
	//			{
	//				// yes - sort out various workstation products
	//				if (osvi.bProductType == VER_NT_WORKSTATION)
	//				{
	//					if (osvi.dwMajorVersion == 4)
	//						m_strName += " Workstation";
	//					else if (osvi.wSuiteMask & VER_SUITE_PERSONAL)
	//						m_strName += " Home Edition";
	//					else
	//						m_strName += " Professional";
	//				}

	//				// Server products
	//				else if (osvi.bProductType == VER_NT_SERVER)
	//				{
	//					// Windows Server 2003
	//					if (osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 2)
	//					{
	//						if (osvi.wSuiteMask & VER_SUITE_DATACENTER)
	//							m_strName += " Datacenter Edition";
	//						else if (osvi.wSuiteMask & VER_SUITE_ENTERPRISE)
	//							m_strName += " Enterprise Edition";
	//						else if (osvi.wSuiteMask & VER_SUITE_BLADE)
	//							m_strName += " Web Edition";
	//						else
	//							m_strName += " Standard Edition";
	//					}

	//					// Windows 2000
	//					else if (osvi.dwMajorVersion == 5 && osvi.dwMinorVersion == 0)
	//					{
	//						if (osvi.wSuiteMask & VER_SUITE_DATACENTER)
	//							m_strName += " Datacenter Server";
	//						else if (osvi.wSuiteMask & VER_SUITE_ENTERPRISE)
	//							m_strName += " Advanced Server";
	//						else
	//							m_strName += " Server";
	//					}

	//					// Windows NT4
	//					else
	//					{
	//						if (osvi.wSuiteMask & VER_SUITE_ENTERPRISE)
	//							m_strName += " Server, Enterprise Edition";
	//						else
	//							m_strName += " Server";
	//					}
	//				}
	//			}

	//			// tests for NT versions that don't support extended info (NT4SP5 or earlier)
	//			else
	//			{
	//				// Detect whether server or workstation ?
	//				typedef DWORD (WINAPI *PRtlGetNtProductType) (PDWORD pVersion);
	//				PRtlGetNtProductType pfn;
	//				HMODULE hNTdll = GetModuleHandle ("ntdll.dll");
	//			
	//				// try mapping and calling the API function
	//				if (!hNTdll || NULL == (pfn = (PRtlGetNtProductType)GetProcAddress(hNTdll, "RtlGetNtProductType")))
	//				{
	//					// can't get hold of the API function - read from registry instead (probably means NT3.xx)
	//					CString strBuffer = CReg::GetItemString(HKEY_LOCAL_MACHINE, RK_NT_TYPE, RI_NT_TYPE);
	//					if (strBuffer.GetLength())
	//					{
	//						if (!strBuffer.CompareNoCase("winnt"))
	//							m_strName += " Workstation";
	//						else
	//							m_strName += " Server";
	//					}
	//				}
	//				else
	//				{
	//					DWORD dwVersion;
	//					pfn(&dwVersion);
	//					if (dwVersion != 1)
	//						m_strName += " Server";
	//					else
	//						m_strName += " Workstation";
	//				}
	//			}
	//		}
	//	}

	//	// exact version
	//	m_strVersion.Format ("%d.%d %s", osvi.dwMajorVersion, osvi.dwMinorVersion, osvi.szCSDVersion);
	//	


	//	}
	//}

	//else
	//{
	//	// cannot use GetVersionEx so assume it's Windows NT 3.1
	//	m_nClass = winNT3;
	//	m_strName = "MS Windows NT";
	//	m_strVersion = "3.1";
	//}

	//// Lastly check to see if this is a 64 bit version of the OS or not
	//m_bIs64BitWindows = FALSE;
	//LPFN_ISWOW64PROCESS fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(GetModuleHandle("kernel32"),"IsWow64Process");
 //   if (NULL != fnIsWow64Process)
 //       fnIsWow64Process(GetCurrentProcess(),&m_bIs64BitWindows);
 
	return TRUE;
}



//
//   GetAdvancedProductInfo
//   ======================
//
//   Currently only called for Windows Vista to recover advanced product information to identify the
//   specific type of Windows installed.
//
CString COsInfo::GetAdvancedProductInfo()
{
	CString strAdvancedVersion = "";
  typedef BOOL  (FAR PASCAL  *lpfnGetProductInfo)   (DWORD, DWORD, DWORD, DWORD, PDWORD);

	DWORD dwProductType = 0;
	HMODULE hKernel32 = GetModuleHandle(_T("KERNEL32.DLL"));
	if (hKernel32)
	{
		lpfnGetProductInfo pGetProductInfo = (lpfnGetProductInfo) GetProcAddress(hKernel32, "GetProductInfo"); 
		if (pGetProductInfo)
			pGetProductInfo(6, 0, 0, 0, &dwProductType);
			//pGPI( osvi.dwMajorVersion, osvi.dwMinorVersion, 0, 0, &dwProductType);
	}  
  
	switch (dwProductType)
	{
		case PRODUCT_STARTER:
		{
			strAdvancedVersion = "Starter Edition";
			break;
		}

		case PRODUCT_HOME_BASIC_N:
		{
			strAdvancedVersion = "Home Basic N Edition";
			break;
		}
	
		case PRODUCT_HOME_BASIC:
		{
			strAdvancedVersion = "Home Basic Edition";
			break;
		}
		
		case PRODUCT_HOME_PREMIUM:
		{
			strAdvancedVersion = "Home Premium Edition";
			break;
		}

		case PRODUCT_BUSINESS_N:
		{
			strAdvancedVersion = "Business N Edition";
			break;
		}
	
		case PRODUCT_BUSINESS:
		{
			strAdvancedVersion = "Business N Edition";
			break;
		}

		case PRODUCT_ENTERPRISE:
		{
			strAdvancedVersion = "Enterprise Edition";
			break;
		}

		case PRODUCT_ULTIMATE:
		{
			strAdvancedVersion = "Ultimate Edition";
			break;
		}

		default:
		{
			break;
		}
	}

	return strAdvancedVersion;
}