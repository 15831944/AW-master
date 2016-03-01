#include "stdafx.h"
#include <iphlpapi.h>
#include "AuditDataFile.h"
#include "PrinterScanner.h"


// Storage Strings
#define HARDWARE_CLASS					"Hardware|Printers"
#define V_PRINTER_NAME					"Name"
#define V_PRINTER_DRIVER				"Driver"
#define V_PRINTER_PORT					"Port"
#define V_PRINTER_DEFAULT				"Default Printer"
#define V_PRINTER_LASTERROR				"Last Detected Error State"
#define V_PRINTER_STATUS				"Status when Audited"

///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CPrinter Class
//
CPrinter::CPrinter()
{
	_name = UNKNOWN;
	_driver = UNKNOWN;
	_port = UNKNOWN;
}


///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CPrinterScanner Class
//
CPrinterScanner::CPrinterScanner()
{
	m_strItemPath = HARDWARE_CLASS;
}

CPrinterScanner::~CPrinterScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool CPrinterScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CPrinterScanner::ScanWMI Start" ,true);

	bool returnStatus = true;

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Ensure that the list is empty
	_listPrinters.Empty();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Printer"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty network adapter object
				CPrinter printer;

				printer.Name(wmiConnection.GetClassObjectStringValue("Name"));
				printer.Driver(wmiConnection.GetClassObjectStringValue("DriverName"));
				printer.Port(wmiConnection.GetClassObjectStringValue("PortName"));

				int printerStatus = wmiConnection.GetClassObjectU64Value("PrinterStatus");
				CString strPrinterStatus;

				switch ( printerStatus )
				{
				case 1:
					strPrinterStatus = "Other";
					break;
				case 2:
					strPrinterStatus = "Unknown";
					break;
				case 3:
					strPrinterStatus = "Idle";
					break;
				case 4:
					strPrinterStatus = "Printing";
					break;
				case 5:
					strPrinterStatus = "Warming Up";
					break;
				case 6:
					strPrinterStatus = "Stopped printing";
					break;
				case 7:
					strPrinterStatus = "Offline";
					break;
				default:
					strPrinterStatus = "Unknown";
				}

				printer.PrinterStatus(strPrinterStatus);

				int detectedErrorState = wmiConnection.GetClassObjectU64Value("DetectedErrorState");
				CString strDetectedErrorState;

				switch ( detectedErrorState )
				{
				case 0:
					strDetectedErrorState = "Unknown";
					break;
				case 1:
					strDetectedErrorState = "Other";
					break;
				case 2:
					strDetectedErrorState = "No Error";
					break;
				case 3:
					strDetectedErrorState = "Low Paper";
					break;
				case 4:
					strDetectedErrorState = "No Paper";
					break;
				case 5:
					strDetectedErrorState = "Low Toner";
					break;
				case 6:
					strDetectedErrorState = "No Toner";
					break;
				case 7:
					strDetectedErrorState = "Door Open";
					break;
				case 8:
					strDetectedErrorState = "Jammed";
					break;
				case 9:
					strDetectedErrorState = "Offline";
					break;
				case 10:
					strDetectedErrorState = "Service Requested";
					break;
				case 11:
					strDetectedErrorState = "Output Bin Full";
					break;
				default:
					strDetectedErrorState = "Other";
				}

				printer.DetectedErrorState(strDetectedErrorState);

				bool lastErrorState = wmiConnection.GetClassObjectBoolValue("Default");
				printer.DefaultPrinter((lastErrorState ? _T("True") : _T("False")));


				// add this to our list
				_listPrinters.Add(printer);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		return false;
	}

	log.Write("CPrinterScanner::ScanWMI End" ,true);
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
bool	CPrinterScanner::ScanRegistryNT()
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
bool	CPrinterScanner::ScanRegistryXP()
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
bool	CPrinterScanner::ScanRegistry9X()
{
	return false;
}



//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CPrinterScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CPrinterScanner::SaveData Start" ,true);
	CString categoryName;

	// OK on entry we will have started the Hardware Category so we do not need to worry about
	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listPrinters.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// and iterate through the audited applications
		for (int isub=0; isub<(int)_listPrinters.GetCount(); isub++)
		{
			// Format the hardware class name for this graphics adapter
			CPrinter* pPrinter = &_listPrinters[isub];
			//categoryName.Format("%s|Printer (%d)" ,HARDWARE_CLASS ,isub);
			categoryName.Format("%s|%s" ,HARDWARE_CLASS ,pPrinter->Name());

			// Each Adapter has its own category
			CAuditDataFileCategory category(categoryName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem p1(V_PRINTER_NAME, pPrinter->Name());
			CAuditDataFileItem p2(V_PRINTER_DRIVER, pPrinter->Driver());
			CAuditDataFileItem p3(V_PRINTER_PORT, pPrinter->Port());
			CAuditDataFileItem p4(V_PRINTER_DEFAULT, pPrinter->DefaultPrinter());
			CAuditDataFileItem p5(V_PRINTER_LASTERROR, pPrinter->DetectedErrorState());
			CAuditDataFileItem p6(V_PRINTER_STATUS, pPrinter->PrinterStatus());

			// Add the items to the category
			category.AddItem(p1);
			category.AddItem(p2);
			category.AddItem(p3);
			category.AddItem(p4);
			category.AddItem(p5);
			category.AddItem(p6);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CPrinterScanner::ScanWMI End" ,true);
	return false;
}