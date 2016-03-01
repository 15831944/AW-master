///////////////////////////////////////////////////////////////////////////////
//
//    CHardwareScanner
//    ================
//
//    The Main HARDWARE Scanning Class
//
///////////////////////////////////////////////////////////////////////////////
//
//    History
//		11-OCT-2011		Chris Drew			8.3.3
//		Corrections to the hardware scanner to only scan items requested
//
///////////////////////////////////////////////////////////////////////////////

#include "Stdafx.h"
#include "AuditDataFile.h"
#include "HardwareScanner.h"

// Scanner include files
#include "GraphicsAdaptersScanner.h"
#include "NetworkAdaptersScanner.h"
#include "NetworkInformationScanner.h"
#include "SystemBiosScanner.h"
#include "SystemProcessorScanner.h"
#include "MemoryScanner.h"
#include "MemorySlotScanner.h"
#include "PrinterScanner.h"
#include "PhysicalDiskScanner.h"
#include "LogicalDriveScanner.h"
#include "ActiveProcessScanner.h"
#include "ServicesScanner.h"
#include "EnvironmentVariablesScanner.h"
#include "LocaleScanner.h"
#include "ObjectScanner.h"
#include "UsersScanner.h"
#include "WindowsSecurityScanner.h"



//
//    Constructor
//
//    Build the internal list of hardware items to scan
//
CHardwareScanner::CHardwareScanner()
{
}


CHardwareScanner::~CHardwareScanner()
{
	// Empty the list of scanners to ensure that the list is cleaned up
	m_listHardware.Empty();
}


//
//    Scan
//    ====
//
//    Main Hardware scanning function
//
bool CHardwareScanner::Scan(CWMIScanner* pWMIScanner)
{
	CLogFile log;

	// First lets build the list of individual hardware scanners that we will use
	// First those which cannot be disabled
	m_listHardware.Add(new CGraphicsAdaptersScanner());
	m_listHardware.Add(new CNetworkAdaptersScanner());
	m_listHardware.Add(new CNetworkInformationScanner());
	m_listHardware.Add(new CSystemBiosScanner());
	m_listHardware.Add(new CSystemProcessorScanner());
	m_listHardware.Add(new CMemorySlotScanner());
	m_listHardware.Add(new CMemoryScanner());
	m_listHardware.Add(new CPrinterScanner());

	// Now optional hardware scanners
	if (_scanPhysicalDisks)										// Physical Drives + Logical Drives
	{
		m_listHardware.Add(new CPhysicalDiskScanner());
		m_listHardware.Add(new CLogicalDriveScanner());
	}

	if (_scanActiveSystem)										// Active System = Services / users
	{
		m_listHardware.Add(new CServicesScanner());
		m_listHardware.Add(new CUsersScanner());
	}

	if (_scanSecurity)											// Hardware Security = Firewall / Updates
		m_listHardware.Add(new CWindowsSecurityScanner());	

	if (_scanSettings)											// Hardware Settings = Environment variables / locale / objects
	{
		m_listHardware.Add(new CEnvironmentVariablesScanner());
		m_listHardware.Add(new CLocaleScanner());
		m_listHardware.Add(new CObjectScanner());
	}

	// Loop through the scanners and call their scan function
	for (int isub=0; isub<(int)m_listHardware.GetCount(); isub++)
	{
		CAuditDataScanner* pThisScanner = m_listHardware[isub];
		log.Write("", true);

		log.Format("start scanning [%s]\n" ,pThisScanner->ItemName());

		/*if (!pThisScanner->Scan(pWMIScanner))
			return false;*/

		pThisScanner->Scan(pWMIScanner);

		log.Format("finished scanning [%s]\n" ,pThisScanner->ItemName());
	}

	// All done return true
	return true;
}


//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//    We do this by adding the hardware scanners to the AuditDataFile which will call the Save
//    function of each in turn at a later stage
//
bool CHardwareScanner::Save	(CAuditDataFile& auditDataFile)
{
	CLogFile log;
	log.Format("Hardware Scanner Saving %d items\n" ,m_listHardware.GetCount());
	
	for (int isub=0; isub<(int)m_listHardware.GetCount(); isub++)
	{
		CAuditDataScanner* pThisScanner = m_listHardware[isub];
		log.Write("", TRUE);
		log.Format("start saving data for [%s]\n" ,pThisScanner->ItemName());
		pThisScanner->SaveData(&auditDataFile);
		log.Format("finish saving data for [%s]\n" ,pThisScanner->ItemName());
	}

	return true;
}



