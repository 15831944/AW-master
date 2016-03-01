#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "LogicalDriveScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"Hardware|Disk Drives"
#define V_LOGICAL_FILESYSTEM		"File System"
#define V_LOGICAL_VOLUMENAME		"Volume Name"
#define V_LOGICAL_PROVIDERNAME		"Provider Name"
#define V_LOGICAL_DRIVELETTER		"Drive Letter"
#define V_LOGICAL_SIZE				"Total Size"
#define V_LOGICAL_FREESPACE			"Free Space"
#define V_LOGICAL_DEVICETYPE		"Device Type"

#define MEGABYTE					"MB"
#define GIGABYTE					"GB"

CLogicalDriveScanner::CLogicalDriveScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CLogicalDriveScanner::~CLogicalDriveScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CLogicalDriveScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CLogicalDriveScanner::ScanWMI Start" ,true);
	//CString strBuffer;

	//// Ensure that the list is empty
	//_listLogicalDrives.Empty();

	//// Get the WMI object itself
	//CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	//try
	//{
	//	if (wmiConnection.BeginEnumClassObject("Win32_LogicalDisk"))
	//	{
	//		while (wmiConnection.MoveNextEnumClassObject())
	//		{
	//			CLogicalDrive drive;

	//			drive.FileSystem(wmiConnection.GetClassObjectStringValue("FileSystem"));
	//			drive.VolumeName(wmiConnection.GetClassObjectStringValue("VolumeName"));
	//			drive.DriveLetter(wmiConnection.GetClassObjectStringValue("Name"));
	//			drive.DeviceName(wmiConnection.GetClassObjectStringValue("Description"));
	//			drive.TotalSize((int)(wmiConnection.GetClassObjectU64Value("Size") / ONE_MEGABYTE));
	//			drive.FreeSpace((int)(wmiConnection.GetClassObjectU64Value("FreeSpace") / ONE_MEGABYTE));
	//			drive.DeviceType((CLogicalDrive::DEVICETYPE)wmiConnection.GetClassObjectDwordValue("DriveType"));

	//			// If the drive is a network share then recover the share point
	//			if (drive.DeviceType() == CLogicalDrive::eDeviceNetwork)
	//				drive.ProviderName(wmiConnection.GetClassObjectStringValue("Providername"));

	//			// Add this disk to our list
	//			_listLogicalDrives.Add(drive);
	//		}
	//		wmiConnection.CloseEnumClassObject();
	//	}
	//}
	//catch (CException *pEx)
	//{
	//	return false;
	//}

	//log.Write("CLogicalDriveScanner::ScanWMI End" ,true);
	//return true;

	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CLogicalDriveScanner::ScanRegistryXP()
{
	ScanDrives();
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CLogicalDriveScanner::ScanRegistryNT()
{
	ScanDrives();
	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CLogicalDriveScanner::ScanRegistry9X()
{
	ScanDrives();
	return true;
}


//
//    ScanDrives
//    ==========
//
//    This function does the buulk of the work if WMI has not been successful.  It is used for all OS
//    platforms
//
BOOL CLogicalDriveScanner::ScanDrives()
{
	CLogFile log;
	log.Write("CLogicalDriveScanner::ScanDrives Start" ,true);

	try
	{
		CString	strDrive, strName, strType;

		// Ensure that the list is empty
		_listLogicalDrives.Empty();

		// get a list of all available drives...
		char szDriveLetters[1024];
		GetLogicalDriveStrings (sizeof(szDriveLetters), szDriveLetters);

		// and loop through them
		for (char * p = szDriveLetters ; *p ; p = p + strlen(p) + 1)
		{
			CLogicalDrive drive;
			char chDrv = *p;
			strDrive.Format("%c:\\" ,chDrv);
			drive.DriveLetter(strDrive);

			// Get drive type
			UINT nType = GetDriveType(strDrive);
			CString strDeviceType;

			enum DEVICETYPE { eDeviceUnknown=0, eDeviceNoRoot ,eDeviceRemovable ,eDeviceLocal ,eDeviceNetwork ,eDeviceCompact ,eDeviceRAM };

			switch (nType)
			{
				case 0:
					strDeviceType = "Unknown Drive";
					break;

				case 1:
					strDeviceType = "Invalid Drive";
					break;

				case 2:
					strDeviceType = "Removable Drive";
					break;

				case 3:
					strDeviceType = "Fixed Drive";
					break;

				case 4:
					strDeviceType = "Remote/Network Drive";
					break;

				case 5:
					strDeviceType = "CD/DVD Drive";
					break;

				case 6:
					strDeviceType = "RAM Disk Drive";
					break;
				
				default:
					strDeviceType = "Unknown Drive";
					break;
			}

			drive.DeviceType(strDeviceType);

			// only get remaining details for hard drives
			if ((DRIVE_FIXED == nType) || (DRIVE_REMOTE == nType))
			{

				TCHAR volumeName[MAX_PATH + 1] = { 0 };
				TCHAR fileSystemName[MAX_PATH + 1] = { 0 };
				DWORD serialNumber = 0;
				DWORD maxComponentLen = 0;
				DWORD fileSystemFlags = 0;
				GetVolumeInformation(
						_T(strDrive),
						volumeName,
						ARRAYSIZE(volumeName),
						&serialNumber,
						&maxComponentLen,
						&fileSystemFlags,
						fileSystemName,
						ARRAYSIZE(fileSystemName));

				drive.VolumeName(volumeName);
				drive.FileSystem(fileSystemName);
				
				// Get Drive space with fix for Win 95 platforms
				DWORD dwFree, dwTotal;
				if (GetDiskSpace(strDrive, &dwFree, &dwTotal)) 
				{
					CString strBuffer;
					strBuffer.Format ("%d", dwTotal);
					drive.TotalSize(dwTotal);
					drive.FreeSpace(dwFree);
				}
			}

			if (DRIVE_REMOTE == nType)
			{
				char szVolumeName[_MAX_FNAME];
				GetUniversalName (szVolumeName, strDrive);
				drive.ProviderName(szVolumeName);
			}

			_listLogicalDrives.Add(drive);
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CLogicalDriveScanner::ScanDrives End" ,true);
	return TRUE;
}



//
//    GetDiskSpace
//    ============
//
//    Get the free / total space for a disk
//
BOOL CLogicalDriveScanner::GetDiskSpace (LPCSTR pszDirectory, LPDWORD pFree, LPDWORD pTotal)
{
	try
	{
		// First see if the Kernel contains a GetDiskFreeSpaceEx function
		HMODULE hKernel = LoadLibrary("KERNEL32.DLL");
		if (hKernel)
		{
			typedef BOOL (FAR WINAPI * DISKPROC)(LPCTSTR, PULARGE_INTEGER, PULARGE_INTEGER, PULARGE_INTEGER);
			DISKPROC pfn = (DISKPROC)GetProcAddress(hKernel, "GetDiskFreeSpaceExA");
			if (pfn)
			{
				// yes - call it
				ULARGE_INTEGER ulFreeToCaller, ulTotal, ulTotalFree;
				BOOL bResult = pfn(pszDirectory, &ulFreeToCaller, &ulTotal, &ulTotalFree);
				if (bResult)
				{
					*pFree = (DWORD)(ulTotalFree.QuadPart >> 20);
					*pTotal = (DWORD)(ulTotal.QuadPart >> 20);
					return TRUE;
				}
			}
		}

		// above failed for whatever reason - use the "old" GetDiskFreeSpace call instead
		DWORD dwSectorsPerCluster, dwBytesPerSector, dwFreeClusters, dwTotalClusters;
		if (!GetDiskFreeSpace (pszDirectory, &dwSectorsPerCluster, &dwBytesPerSector, &dwFreeClusters, &dwTotalClusters))
			return FALSE;
		// convert and store
		DWORD dwBytesPerCluster = dwBytesPerSector * dwSectorsPerCluster;
		*pFree = (dwBytesPerCluster * dwFreeClusters) >> 20;
		*pTotal = (dwBytesPerCluster * dwTotalClusters) >> 20;
		return TRUE;
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
}


//
//    GetUniversalName
//    ================
//
//	  Convert input path pszDrive into UNC equivalent written to pszBuffer
//
BOOL CLogicalDriveScanner::GetUniversalName (char * pszBuffer, LPCSTR pszDrive)
{
	try
	{
		strcpy(pszBuffer, "n/a");

		// Get the local drive letter.
		char chLocal = toupper(*pszDrive);

		// Cursory validation.
		if ( chLocal < 'A' || chLocal > 'Z' )
			return FALSE;

		if (pszDrive[1] != ':' || pszDrive[2] != '\\' )
			return FALSE;

		HANDLE hEnum;
		DWORD dwResult = WNetOpenEnum( RESOURCE_CONNECTED, RESOURCETYPE_DISK, 0, NULL, &hEnum );

		if ( dwResult != NO_ERROR )
			return FALSE;

		// Request all available entries.
		const int    c_cEntries   = 0xFFFFFFFF;

		// Start with a reasonable buffer size.
		DWORD        cbBuffer     = 100 * sizeof( NETRESOURCE );
		NETRESOURCE *pNetResource = (NETRESOURCE*) malloc( cbBuffer );

		BOOL bAllDone = FALSE;

		while (!bAllDone)
		{
			DWORD dwSize = cbBuffer,
			cEntries = c_cEntries;

			dwResult = WNetEnumResource( hEnum, &cEntries, pNetResource, &dwSize );

			// If error is 'No More Data' this is expected and requires us to allocate 
			// a larger buffer for the data
			if (dwResult == ERROR_MORE_DATA)
			{
				// The buffer was too small, enlarge.
				cbBuffer = dwSize;
				pNetResource = (NETRESOURCE*) realloc(pNetResource, cbBuffer);
			}

			// NO_ERROR means we have the required data
			else if (dwResult == NO_ERROR)
			{
				// Search for the specified drive letter.
				for ( int i = 0; i < (int) cEntries; i++ )
				{
					if ((pNetResource[i].lpLocalName) 
					&&  (chLocal == toupper(pNetResource[i].lpLocalName[0])))
					{
						// Build a UNC name.
						strcpy (pszBuffer, pNetResource[i].lpRemoteName );
						strcat (pszBuffer, pszDrive + 2 );
						_strupr (pszBuffer);
						break;
					}
				}
				bAllDone = TRUE;
			}

			// Some other error - we cannot continue.
			else
			{
				bAllDone = TRUE;
			}
		}

		// Clean up.
		WNetCloseEnum( hEnum );
		free( pNetResource );
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	return TRUE;
} 









//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CLogicalDriveScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CLogicalDriveScanner::SavaData Start" ,true);

	CString driveName;

	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listLogicalDrives.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		for (int isub=0; isub<(int)_listLogicalDrives.GetCount(); isub++)
		{
			// Format the hardware class name for this drive
			CLogicalDrive* pDrive = &_listLogicalDrives[isub];
			driveName.Format("%s|Drive %s" ,HARDWARE_CLASS ,pDrive->DriveLetter());

			// Each Adapter has its own category
			CAuditDataFileCategory category(driveName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem l1(V_LOGICAL_FILESYSTEM ,pDrive->FileSystem());
			/*CAuditDataFileItem l2(V_LOGICAL_DEVICENAME ,pDrive->DeviceName());
			CAuditDataFileItem l3(V_LOGICAL_DESCRIPTION ,pDrive->Description());*/
			CAuditDataFileItem l4(V_LOGICAL_VOLUMENAME ,pDrive->VolumeName());
			CAuditDataFileItem l5(V_LOGICAL_PROVIDERNAME ,pDrive->ProviderName());
			CAuditDataFileItem l6(V_LOGICAL_DRIVELETTER ,pDrive->DriveLetter());
			CAuditDataFileItem l7(V_LOGICAL_SIZE ,pDrive->TotalSize()/1024 ," GB");
			CAuditDataFileItem l8(V_LOGICAL_FREESPACE ,pDrive->FreeSpace()/1024 ," GB" ,CAuditDataFileItem::numeric ,FALSE);
			CAuditDataFileItem l9(V_LOGICAL_DEVICETYPE ,pDrive->DeviceType());

			// Add the items to the category
			category.AddItem(l1);
			//category.AddItem(l2);
			//category.AddItem(l3);
			category.AddItem(l4);
			category.AddItem(l5);
			category.AddItem(l6);
			category.AddItem(l7);
			category.AddItem(l8);
			category.AddItem(l9);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CLogicalDriveScanner::SavaData End" ,true);
	return true;
}