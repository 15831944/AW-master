#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "ServicesScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Services"
#define V_SERVICE_NAME				"Name"
#define V_SERVICE_STATE				"State"
#define V_SERVICE_STARTUP			"Startup"


typedef BOOL (WINAPI *pEnumServicesStatusExA)
(
	SC_HANDLE,
	SC_ENUM_TYPE,
	DWORD,
	DWORD,
	LPBYTE,
	DWORD,
	LPDWORD,
	LPDWORD,
	LPDWORD,
	LPCSTR
);

CServicesScanner::CServicesScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CServicesScanner::~CServicesScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CServicesScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CServicesScanner::ScanWMI Start" ,true);
	CString strBuffer;

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Ensure that the list is empty
	_listServices.Empty();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Services"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				CSystemService service;
				service.Name(wmiConnection.GetClassObjectStringValue("DisplayName"));
				service.State(wmiConnection.GetClassObjectStringValue("State"));
				service.StartMode(wmiConnection.GetClassObjectStringValue("StartMode"));

				// Add this to our list
				_listServices.Add(service);
			}
			wmiConnection.CloseEnumClassObject();
		}
		else
		{
			return false;
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CServicesScanner::ScanWMI (Enumerating Win32_Services)");
		return false;
	}

	log.Write("CServicesScanner::ScanWMI End" ,true);
	return true;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool CServicesScanner::ScanRegistryXP()
{
	ScanServices();
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool CServicesScanner::ScanRegistryNT()
{
	ScanServices();
	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool CServicesScanner::ScanRegistry9X()
{
	ScanServices();
	return true;
}



//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CServicesScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CServicesScanner::SaveData Start" ,true);
	CString serviceName;

	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listServices.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		for (int isub=0; isub<(int)_listServices.GetCount(); isub++)
		{
			// Format the hardware class name for this drive
			CSystemService* pService = &_listServices[isub];
			serviceName.Format("%s|%s" ,HARDWARE_CLASS ,pService->Name());

			// Each process has its own category
			CAuditDataFileCategory category(serviceName ,TRUE ,TRUE);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem p1(V_SERVICE_NAME ,pService->Name());
			CAuditDataFileItem p2(V_SERVICE_STATE ,pService->State());
			CAuditDataFileItem p3(V_SERVICE_STARTUP ,pService->StartMode());

			// Add the items to the category
			category.AddItem(p1);
			category.AddItem(p2);
			category.AddItem(p3);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CServicesScanner::SaveData End" ,true);
	return true;
}



//
//    ScanServices
//    ============
//
//    This function does the buulk of the work if WMI has not been successful.  It is used for all OS
//    platforms
//
void CServicesScanner::ScanServices()
{
	CLogFile log;
	log.Write("CServicesScanner::ScanServices Start" ,true);

	try
	{
		// Ensure that the list is empty
		_listServices.Empty();

		// Pick up all installed services
		DetectInstalledServices();

		// ...and then update the list with the active services
		DetectActiveServices();
		log.Write("CServicesScanner::ScanServices End" ,true);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
}



//
//    DetectActiveServices
//    ====================
//
//    Pick up a list of all of the currently active system services on this PC
//
void CServicesScanner::DetectActiveServices	(void) 
{
	pEnumServicesStatusExA fnEnumServicesStatusExA;

	HMODULE		hAdvapi32;
	ENUM_SERVICE_STATUS_PROCESSA *lpServiceStatus;
	DWORD		dwSize=0, dwNumServices=0, dwResumeHandle=0;
	int			iRet;
	void		*lpBuffer=NULL;
	SC_HANDLE	scHandle;

	try 
	{
		//load advapi32.dll
		hAdvapi32 = LoadLibrary("advapi32.dll");
		if (hAdvapi32 == NULL) 
			return;

		//load EnumServicesStatusExA from advapi32.dll
		fnEnumServicesStatusExA=(pEnumServicesStatusExA)GetProcAddress(hAdvapi32,"EnumServicesStatusExA");
		if (fnEnumServicesStatusExA == NULL) 
			return;

		// establishes a connection to the service control manager 
		scHandle = OpenSCManager(NULL ,NULL ,SC_MANAGER_ALL_ACCESS);
		if (scHandle == NULL) 
			return;		
		
		do 
		{
			// First time dwResumeHandle needs to be 0 and dwSize = 0  after return it is 
			// set to the size needed normaly it will only loop twice first time to get size of 
			// the buffer second time to fill the allocated buffer
			iRet = fnEnumServicesStatusExA(scHandle
										  ,SC_ENUM_PROCESS_INFO
										  ,SERVICE_WIN32
										  ,SERVICE_ACTIVE
										  ,(LPBYTE)lpBuffer
										  ,dwSize
										  ,&dwSize
										  ,&dwNumServices
										  ,&dwResumeHandle
										  ,0);

			// iRet is always 0 the first time because dwSize was 0
			if (iRet == 0 && GetLastError() != ERROR_MORE_DATA) 
				return;

			lpServiceStatus = (ENUM_SERVICE_STATUS_PROCESSA*)lpBuffer;

			//first time dwNumServices == 0 so we dont loop the first time
			if (dwNumServices > 0) 
			{	
				for (DWORD dwCount=0; dwCount<dwNumServices; dwCount++)
				{
					CString strDescription;
					strDescription = lpServiceStatus[dwCount].lpDisplayName;

					// Can we find this service in the installed list?
					for (DWORD dw=0; dw<_listServices.GetCount(); dw++)
					{
						CSystemService* pService = &_listServices[dw];
						if (pService->Name() == strDescription)
							pService->State("Running");
					}
				}		
			}
			
			if (lpBuffer) 
			{
				free(lpBuffer);
				lpBuffer=NULL;
			}

			if (iRet == 0) 
			{ 
				//not all entry's returned normaly this only happens the first time we loop
				//allocate memory 
				lpBuffer = malloc(dwSize);
			}

		} 
		while (iRet == 0); 

		throw("End EnumServices"); 
	}

	catch(char * /*szError*/)
	{
		if (scHandle)	CloseServiceHandle(scHandle);
		if (lpBuffer)	free(lpBuffer);
	}	
}


static char WinNTServices[] = "System\\CurrentControlSet\\Services";
static char MuiCache[] = "S-1-5-8\\Software\\Microsoft\\Windows\\Shell\\MuiCache";
#define REG_BUFFER_SIZE 1024


//
//    DetectInstalledServices
//    =======================
//
//    Build a list of all of the installed system services on this PC.
//
void CServicesScanner::DetectInstalledServices	(void) 
{
	HKEY hKey;

	// Try and open the services key - if this fails then that's it
	if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_LOCAL_MACHINE, WinNTServices, 0, KEY_READ, &hKey)) 
	{
		DWORD dwIndex = 0;
		char szBuffer[REG_BUFFER_SIZE];
		DWORD dwLength = REG_BUFFER_SIZE;

		// Enumerate all of the sub-keys as these are the services.
		while (ERROR_NO_MORE_ITEMS != RegEnumKeyEx (hKey,  dwIndex, szBuffer, &dwLength, NULL, NULL, NULL, NULL)) 
		{
			// prepare to get next key
			dwIndex++;
			dwLength = REG_BUFFER_SIZE;

			// Skip if there is no Display name
			CString strDisplayName, strListName ,strStartup;
			strDisplayName = CReg::GetItemString(hKey, szBuffer, "DisplayName");

			// If the name is empty then simply ignore this entry
			if (strDisplayName.IsEmpty())
				continue;

			// If the display name begins with an '@' character then we need to go elsewhere to 
			// find the actual service description/name
			if (strDisplayName[0] == '@')
				strDisplayName = CheckMuiCache(strDisplayName);

			// If the name is (still) empty then simply ignore this entry
			if (strDisplayName.IsEmpty())
				continue;

			// Check the Service Type - we ignore all type 1 services
			int nType = CReg::GetItemInt(hKey ,szBuffer, "Type");
			if (nType <= 2)
				continue;

			int nStartup = CReg::GetItemInt(hKey ,szBuffer, "Start");
			if (nStartup == 0)
				continue;
	
			if (nStartup == 1)
				strStartup = "Unknown";
			else if (nStartup == 2)
				strStartup = "Automatic";
			else if (nStartup == 3)
				strStartup = "Manual";
			else if (nStartup == 4)
				strStartup = "Disabled";
			else
				strStartup = "Out of Range";

			// Add a new service with these details
			CSystemService service(strDisplayName ,UNKNOWN ,strStartup);
			_listServices.Add(service);
		}
		RegCloseKey (hKey);
	}
}



//
//    CheckMuiCache
//	  ==============
//
//    Called when the service name begins with an '@' character indicating that it is in fact a path
//    and not the actual service name.  We look in the MuiCache to see if we can actually determine
//    the name from a matching entry.
//
CString CServicesScanner::CheckMuiCache (CString& strDisplayName)
{
	HKEY hKey;
	CString strReturn = "";

	// Try and open the MuiCache key - if this fails then that's it
	if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_USERS, MuiCache, 0, KEY_READ, &hKey)) 
	{
		DWORD dwIndex = 0;
		char szItem[1024];
		unsigned char szValue[1024];
		DWORD dwItemLen = sizeof(szItem);
		DWORD dwValueLen = sizeof(szValue);
			
		while (ERROR_NO_MORE_ITEMS != ::RegEnumValue (hKey, dwIndex++, szItem, &dwItemLen, NULL, NULL, szValue, &dwValueLen))
		{
			if (strDisplayName == szItem)
			{
				strReturn = szValue;
				break;
			}
			dwItemLen = sizeof(szItem);
			dwValueLen = sizeof(szValue);
		}
		RegCloseKey (hKey);
	}
		
	return strReturn;
}
