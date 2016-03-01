#include "stdafx.h"
#include <iphlpapi.h>
#include "AuditDataFile.h"
#include "MemorySlotScanner.h"


// Storage Strings
#define HARDWARE_CLASS					"Hardware|Memory Slots"
#define V_MEMORYSLOT_NAME				"Name"
#define V_MEMORYSLOT_CAPACTITY			"Capacity"
#define V_MEMORYSLOT_SPEED				"Speed"
#define V_MEMORYSLOT_FORM				"Form Factor"
#define V_MEMORYSLOT_TYPE				"Type"

///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CMemorySlot Class
//
CMemorySlot::CMemorySlot()
{
	_capacity = 0;
	_speed = 0;
	_name = UNKNOWN;
	_formFactor = 0;
	_type = 0;
}


///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CMemorySlotScanner Class
//
CMemorySlotScanner::CMemorySlotScanner()
{
	m_strItemPath = HARDWARE_CLASS;
}

CMemorySlotScanner::~CMemorySlotScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool CMemorySlotScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CMemorySlotScanner::ScanWMI Start" ,true);

	bool returnStatus = true;
	CString	strBuffer;

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Ensure that the list is empty
	_listMemorySlots.Empty();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_PhysicalMemory"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty network adapter object
				CMemorySlot slot;

				// We'll set the name of this memory slot to either the DeviceLocator if this is not available to
				// the bank label.
				strBuffer = wmiConnection.GetClassObjectStringValue("DeviceLocator");
				if (strBuffer.IsEmpty())
					strBuffer = wmiConnection.GetClassObjectStringValue("BankLabel");
				slot.Name(strBuffer);

				// Get the capacity
				slot.Capacity(DWORD(wmiConnection.GetClassObjectU64Value("Capacity") / ONE_MEGABYTE));
				slot.Speed(wmiConnection.GetClassObjectDwordValue("Speed"));
				slot.Type(wmiConnection.GetClassObjectI64Value("MemoryType"));
				slot.FormFactor(wmiConnection.GetClassObjectI64Value("FormFactor"));

				//CString strFormFactor = slot.FormFactorAsString();
				
				// add this to our list
				_listMemorySlots.Add(slot);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		return false;
	}

	log.Write("CMemorySlotScanner::ScanWMI End" ,true);
	return returnStatus;
}



//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT/2000 or higher
//
//    MEMORY SLOT IDENTIFICATION NOT AVAILABLE WITHOUT WMI
//
bool	CMemorySlotScanner::ScanRegistryNT()
{
	return false;
}


//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
//    MEMORY SLOT IDENTIFICATION NOT AVAILABLE WITHOUT WMI
//
bool	CMemorySlotScanner::ScanRegistryXP()
{
	return false;
}



//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
//    MEMORY SLOT IDENTIFICATION NOT AVAILABLE WITHOUT WMI
//
bool	CMemorySlotScanner::ScanRegistry9X()
{
	return false;
}



//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CMemorySlotScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CMemorySlotScanner::SaveData Start" ,true);

	CString categoryName;
	CString speed;

	// OK on entry we will have started the Hardware Category so we do not need to worry about
	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listMemorySlots.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// and iterate through the audited applications
		for (int isub=0; isub<(int)_listMemorySlots.GetCount(); isub++)
		{
			// Format the hardware class name for this memory slot
			CMemorySlot* pSlot = &_listMemorySlots[isub];
			categoryName.Format("%s|%s" ,HARDWARE_CLASS ,pSlot->Name());
			CAuditDataFileCategory category(categoryName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem s1(V_MEMORYSLOT_CAPACTITY ,pSlot->Capacity(), " MB");
			CAuditDataFileItem s2(V_MEMORYSLOT_SPEED ,pSlot->Speed(), " MHz");
			CAuditDataFileItem s3(V_MEMORYSLOT_FORM ,pSlot->FormFactorAsString());
			//CAuditDataFileItem s4(V_MEMORYSLOT_TYPE ,pSlot->TypeAsString());

			// Add the items to the category
			category.AddItem(s1);
			category.AddItem(s2);
			category.AddItem(s3);
			//category.AddItem(s4);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CMemorySlotScanner::SaveData End" ,true);
	return true;
}