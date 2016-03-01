#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "PhysicalDiskScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"Hardware|Physical Disk"
#define V_DISK_MANUFACTURER			"Manufacturer"
#define V_DISK_MODEL				"Model"
#define V_DISK_PARTITIONS			"Partitions"
#define V_DISK_SIZE					"Size"
#define V_DISK_CYLINDERS			"Cylinders"
#define V_DISK_TRACKS				"Tracks"
#define V_DISK_SECTORS				"Sectors"
#define V_DISK_HEADS				"Heads"
#define GIGABYTE					"GB"


/////////////////////////////////////////////////////////////////////////////////////
//
//    CPhysicalDiskScanner
//    ====================
//
CPhysicalDiskScanner::CPhysicalDiskScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CPhysicalDiskScanner::~CPhysicalDiskScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CPhysicalDiskScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CPhysicalDiskScanner::ScanWMI Start" ,true);

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Ensure that the list is empty
	_listPhysicalDisks.Empty();

	try
	{
		CString		strBuffer;
		CString		strMacAddress;
		LPCTSTR		pDataValue = NULL;

		// We need to enumerate through the physical disk drives
		if (wmiConnection.BeginEnumClassObject("Win32_DiskDrive"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty disk drive object to populate
				CPhysicalDisk physicalDisk;

				// Recover information about this disk
				strBuffer = wmiConnection.GetClassObjectStringValue("Manufacturer");
				physicalDisk.Manufacturer(strBuffer);
				//
				strBuffer = wmiConnection.GetClassObjectStringValue("Model");
				physicalDisk.Model(strBuffer);
				
				// Get Size in bytes - we need to divide by 1024 to get KB,1024 to get MB and then 1024 to get GB
				log.Write("Recovering physical disk size");
				unsigned __int64 u64Value = wmiConnection.GetClassObjectU64Value("Size");
				DWORD gb = (DWORD)(u64Value / (1024*1024*1000));
				physicalDisk.Size(gb);
				log.Format("Physical disk size in GB is %d\n",gb);
				physicalDisk.Partitions(wmiConnection.GetClassObjectDwordValue("Partitions"));
				physicalDisk.Cylinders((DWORD)wmiConnection.GetClassObjectU64Value("TotalCylinders"));
				physicalDisk.TracksPerCylinder(wmiConnection.GetClassObjectDwordValue("TracksPerCylinder"));
				physicalDisk.SectorsPerTrack(wmiConnection.GetClassObjectDwordValue("SectorsPerTrack"));
				physicalDisk.Heads(wmiConnection.GetClassObjectDwordValue("TotalHeads"));

				// Add this disk to our list
				_listPhysicalDisks.Add(physicalDisk);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		return false;
	}

	log.Write("CPhysicalDiskScanner::ScanWMI End" ,true);
	return true;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
//		*** NO REGISTRY SOLUTION FOR RECOVERING PHYSICAL DISK INFORMATION ***
//
bool	CPhysicalDiskScanner::ScanRegistryXP()
{
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//		*** NO REGISTRY SOLUTION FOR RECOVERING PHYSICAL DISK INFORMATION ***
//
bool	CPhysicalDiskScanner::ScanRegistryNT()
{
	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//		*** NO REGISTRY SOLUTION FOR RECOVERING PHYSICAL DISK INFORMATION ***
//
bool	CPhysicalDiskScanner::ScanRegistry9X()
{
	return true;
}


//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CPhysicalDiskScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CPhysicalDiskScanner::SaveData Start" ,true);

	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	CString diskName;

	// OK on entry we will have started the Hardware Category so we do not need to worry about
	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listPhysicalDisks.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// and iterate through the audited physical disks - we create a CAuditDataFileCategory for each disl
		// The items within the category are the values recovered for each disk
		for (int isub=0; isub<(int)_listPhysicalDisks.GetCount(); isub++)
		{
			// Format the hardware class name for this graphics adapter
			CPhysicalDisk* pDisk = &_listPhysicalDisks[isub];
			diskName.Format("%s|%s" ,HARDWARE_CLASS ,pDisk->Model());

			// Each Adapter has its own category
			CAuditDataFileCategory category(diskName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			//CAuditDataFileItem d1(V_DISK_MANUFACTURER ,pDisk->Manufacturer());
			CAuditDataFileItem d2(V_DISK_MODEL ,pDisk->Model());
			//
			CAuditDataFileItem d3(V_DISK_PARTITIONS ,pDisk->Partitions());
			CAuditDataFileItem d4(V_DISK_SIZE ,pDisk->Size() ," GB");
			CAuditDataFileItem d5(V_DISK_CYLINDERS ,pDisk->Cylinders());
			CAuditDataFileItem d6(V_DISK_TRACKS ,pDisk->TracksPerCylinder());
			CAuditDataFileItem d7(V_DISK_SECTORS ,pDisk->SectorsPerTrack());
			CAuditDataFileItem d8(V_DISK_HEADS ,pDisk->Heads());

			// Add the items to the category
			//category.AddItem(d1);
			category.AddItem(d2);
			category.AddItem(d3);
			category.AddItem(d4);
			category.AddItem(d5);
			category.AddItem(d6);
			category.AddItem(d7);
			category.AddItem(d8);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CPhysicalDiskScanner::SaveData End" ,true);
	return true;
}