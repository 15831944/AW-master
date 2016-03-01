#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "ActiveProcessScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Active Processes"
#define V_PROCESS_NAME				"Name"
#define V_PROCESS_EXECUTABLE		"Executable"
#define V_PROCESS_PID				"PID"

CActiveProcessScanner::CActiveProcessScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
	//
	m_pFnProcess32First = NULL;
	m_pFnProcess32Next = NULL;
	m_pFnCreateToolHelpSnapshot = NULL;
	//
	m_hKernel32 = LoadLibrary("KERNEL32.DLL");
	if (m_hKernel32)
	{
		m_pFnModule32First = (pModule32First) GetProcAddress(m_hKernel32, "Module32First");
		m_pFnProcess32First = (pProcess32First) GetProcAddress(m_hKernel32, "Process32First");
		m_pFnProcess32Next  = (pProcess32Next)  GetProcAddress(m_hKernel32, "Process32Next");
		m_pFnCreateToolHelpSnapshot  = (pCreateToolhelp32Snapshot)  GetProcAddress(m_hKernel32, "CreateToolhelp32Snapshot");
	}
}

CActiveProcessScanner::~CActiveProcessScanner(void)
{
	if (m_hKernel32)
		FreeLibrary(m_hKernel32);
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CActiveProcessScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	CString strBuffer;

	// Ensure that the list is empty
	_listActiveProcesses.Empty();
		
	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Process"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				CActiveProcess activeProcess;
				activeProcess.Executable(wmiConnection.GetClassObjectStringValue("ExecutablePath"));
				activeProcess.Name(wmiConnection.GetClassObjectStringValue("Name"));
				activeProcess.Pid(wmiConnection.GetClassObjectDwordValue("ProcessId"));

				// Add this process to our list
				_listActiveProcesses.Add(activeProcess);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CActiveProcessScanner::ScanWMI");
	}

	return true;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CActiveProcessScanner::ScanRegistryXP()
{
	ScanActiveProcesses();
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CActiveProcessScanner::ScanRegistryNT()
{
	ScanActiveProcesses();
	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CActiveProcessScanner::ScanRegistry9X()
{
	ScanActiveProcesses();
	return true;
}



//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CActiveProcessScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CString processName;

	if (_listActiveProcesses.GetCount() != 0)
	{
		// As we are going to flag the Active Processes as being grouped we must ensure that the category name
		// is unique as things get very screwy otherwise as we cannot identify connected once in the database
		// without a unique reference
		for (int isub=0; isub < (int)_listActiveProcesses.GetCount(); isub++)
		{
			// Format the hardware class name for this drive
			CActiveProcess* pProcess = &_listActiveProcesses[isub];
			processName.Format("%s|%d" ,HARDWARE_CLASS ,pProcess->Pid());

			// Each process has its own category (THIS ITEM DOES NOT GENERATE HISTORY) and GROUPED
			CAuditDataFileCategory category(processName ,FALSE ,TRUE);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem p1(V_PROCESS_NAME ,pProcess->Name());
			CAuditDataFileItem p2(V_PROCESS_EXECUTABLE ,pProcess->Executable());

			// Add the items to the category
			category.AddItem(p1);
			category.AddItem(p2);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	return true;
}



//
//    ScanActiveProcesses
//    ===================
//
//    This function does the buulk of the work if WMI has not been successful.  It is used for all OS
//    platforms
//
BOOL CActiveProcessScanner::ScanActiveProcesses()
{
	CLogFile log;

	// Ensure that the list is empty
	_listActiveProcesses.Empty();

	// If we failed to get ANY of the function addresses then just forget it
	if ((m_pFnProcess32First == NULL)
	||  (m_pFnProcess32Next == NULL)
	||  (m_pFnCreateToolHelpSnapshot == NULL)
	||  (m_pFnModule32First == NULL))
		return false;

	// PSAPI Functions not found - try KERNAL32 functions instead - these are available on all platforms
	HANDLE hSnapShot = m_pFnCreateToolHelpSnapshot(TH32CS_SNAPALL,NULL);

	PROCESSENTRY32 processEntry;
	processEntry.dwSize = sizeof(processEntry);
	
	//Get first process
	m_pFnProcess32First (hSnapShot ,&processEntry);
	
	//Iterate thru all processes
	while(1)
	{
		BOOL hRes = m_pFnProcess32Next(hSnapShot ,&processEntry);
		if (hRes == FALSE)
			break;

		// Try and determine the process executable
		CString strExecutable = GetProcessExecutable(processEntry.th32ProcessID);
		if (strExecutable == "")
			strExecutable = processEntry.szExeFile;

		// Add the new process
		CActiveProcess activeProcess(strExecutable ,"" ,processEntry.th32ProcessID);
		_listActiveProcesses.Add(activeProcess);
	}

	if (hSnapShot != INVALID_HANDLE_VALUE)
		::CloseHandle(hSnapShot);

	return TRUE;
}


//
//    GetProcessExecutable
//    ====================
//
//    Return the full name of the executable being run by the specified process.  This is done
//    by returning the executable for the first module (thread) of the process.
//
CString CActiveProcessScanner::GetProcessExecutable (DWORD dwPID)
{
	CString strModule = "";
	HANDLE hModuleSnap = INVALID_HANDLE_VALUE;
	MODULEENTRY32 me32;

	// Take a snapshot of all modules in the specified process.
	hModuleSnap = m_pFnCreateToolHelpSnapshot(TH32CS_SNAPMODULE, dwPID);
	if (hModuleSnap != INVALID_HANDLE_VALUE)
	{

		// Set the size of the structure before using it.
		me32.dwSize = sizeof(MODULEENTRY32);

		// Retrieve information about the first module, and exit if unsuccessful
		if (m_pFnModule32First(hModuleSnap, &me32))
			strModule = me32.szExePath;

		CloseHandle( hModuleSnap );
	}

	return strModule;
}
