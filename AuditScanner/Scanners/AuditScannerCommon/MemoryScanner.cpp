#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "MemoryScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"Hardware|Memory"
#define V_MEMORY_TOTAL_RAM			"Total RAM"
#define V_MEMORY_AVAILABLE_RAM		"Available RAM"
#define V_MEMORY_TOTAL_PAGEFILE		"Total Pagefile"
#define V_MEMORY_AVAILABLE_PAGEFILE	"Available Pagefile"
#define V_MEMORY_TOTAL_VIRTUAL		"Total Virtual"
#define V_MEMORY_AVAILABLE_VIRTUAL	"Available Virtual"
#define MEGABYTE					"MB"

// Under Windows 2K or higher, use MEMORYSTATUSEX
BOOL (__stdcall *lpfnGlobalMemoryStatusEx) ( LPMEMORYSTATUSEX lpBuffer);

CMemoryScanner::CMemoryScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
	//
	_totalRam = 0;
	_availableRam = 0;
	_totalPageFile = 0;
	_availablePageFile = 0;
	_totalVirtualMemory = 0;
	_availableVirtualMemory = 0;
}

CMemoryScanner::~CMemoryScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CMemoryScanner::ScanWMI(CWMIScanner *pScanner)
{
	// No WMI Solution for memory
	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CMemoryScanner::ScanRegistryXP()
{
	CLogFile log;
	log.Write("CMemoryScanner::ScanRegistryXP Start" ,true);

	try
	{
		// Windows 2K or higher => use MEMORYSTATUSEX
		MEMORYSTATUSEX pMemStatus;

		if ((*(FARPROC*)&lpfnGlobalMemoryStatusEx = GetProcAddress( GetModuleHandle(_T( "KERNEL32.DLL")), _T( "GlobalMemoryStatusEx"))) == NULL)
			return FALSE;

		pMemStatus.dwLength = sizeof( MEMORYSTATUSEX);
		if (!lpfnGlobalMemoryStatusEx (&pMemStatus))
			return FALSE;

		_totalRam = (int)((pMemStatus.ullTotalPhys + 655360) / ONE_MEGABYTE);
		_availableRam = (int)((pMemStatus.ullAvailPhys + 655360) / ONE_MEGABYTE);
		_totalPageFile = (int)((pMemStatus.ullTotalPageFile + 655360) / ONE_MEGABYTE);
		_availablePageFile = (int)((pMemStatus.ullAvailPageFile + 655360) / ONE_MEGABYTE);
		_totalVirtualMemory = (int)((pMemStatus.ullTotalVirtual + 655360) / ONE_MEGABYTE);
		_availableVirtualMemory = (int)((pMemStatus.ullAvailVirtual + 655360) / ONE_MEGABYTE);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CMemoryScanner::ScanRegistryXP End" ,true);
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CMemoryScanner::ScanRegistryNT()
{
	CLogFile log;
	log.Write("CMemoryScanner::ScanRegistryNT Start" ,true);

	try
	{
		// Windows 2K or higher => use MEMORYSTATUSEX
		MEMORYSTATUSEX pMemStatus;

		if ((*(FARPROC*)&lpfnGlobalMemoryStatusEx = GetProcAddress( GetModuleHandle(_T( "KERNEL32.DLL")), _T( "GlobalMemoryStatusEx"))) == NULL)
			return FALSE;

		pMemStatus.dwLength = sizeof( MEMORYSTATUSEX);
		if (!lpfnGlobalMemoryStatusEx (&pMemStatus))
			return FALSE;

		_totalRam = (int)((pMemStatus.ullTotalPhys + 655360) / ONE_MEGABYTE);
		_availableRam = (int)((pMemStatus.ullAvailPhys + 655360) / ONE_MEGABYTE);
		_totalPageFile = (int)((pMemStatus.ullTotalPageFile + 655360) / ONE_MEGABYTE);
		_availablePageFile = (int)((pMemStatus.ullAvailPageFile + 655360) / ONE_MEGABYTE);
		_totalVirtualMemory = (int)((pMemStatus.ullTotalVirtual + 655360) / ONE_MEGABYTE);
		_availableVirtualMemory = (int)((pMemStatus.ullAvailVirtual + 655360) / ONE_MEGABYTE);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}


	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CMemoryScanner::ScanRegistry9X()
{
	// Use the GlobalMemoryStatus API call as in Windows NT/2000
	return ScanRegistryNT();
}


//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CMemoryScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CMemoryScanner::SaveData Start" ,true);

	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	// Each audited item gets added an a CAuditDataFileItem to the category
	CAuditDataFileItem m1(V_MEMORY_TOTAL_RAM ,_totalRam ," MB");
	CAuditDataFileItem m2(V_MEMORY_AVAILABLE_RAM ,_availableRam ," MB" ,CAuditDataFileItem::numeric ,false);
	CAuditDataFileItem m3(V_MEMORY_TOTAL_PAGEFILE ,_totalPageFile ," MB" ,CAuditDataFileItem::numeric ,false);
	CAuditDataFileItem m4(V_MEMORY_AVAILABLE_PAGEFILE ,_availablePageFile ," MB" ,CAuditDataFileItem::numeric ,false);
	CAuditDataFileItem m5(V_MEMORY_TOTAL_VIRTUAL ,_totalVirtualMemory ," MB" ,CAuditDataFileItem::numeric ,false);
	CAuditDataFileItem m6(V_MEMORY_AVAILABLE_VIRTUAL ,_availableVirtualMemory ," MB" ,CAuditDataFileItem::numeric ,false);

	// Add the items to the category
	category.AddItem(m1);
	category.AddItem(m2);
	category.AddItem(m3);
	category.AddItem(m4);
	category.AddItem(m5);
	category.AddItem(m6);

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);

	log.Write("CMemoryScanner::SaveData End" ,true);
	return true;
}