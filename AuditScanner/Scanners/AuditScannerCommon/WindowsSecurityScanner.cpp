#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "WindowsSecurityScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Windows Security"
#define HARDWARE_CLASS_FIREWALL1	"System|Windows Security|Windows Firewall|Standard Profile"
#define HARDWARE_CLASS_FIREWALL2	"System|Windows Security|Windows Firewall|Domain Profile"
#define HARDWARE_CLASS_UPDATE		"System|Windows Security|Windows Update"

#define V_FIREWALL_ENABLED			"Enabled"
#define V_FIREWALL_EXCEPTIONS		"Exceptions Permitted"
#define V_UPDATE_NEXTDETECT			"Next Detection Time"
#define V_UPDATE_INSTALLDATE		"Scheduled Install Date"
#define V_UPDATE_STATUS				"Status"

CWindowsSecurityScanner::CWindowsSecurityScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CWindowsSecurityScanner::~CWindowsSecurityScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CWindowsSecurityScanner::ScanWMI(CWMIScanner *pScanner)
{
	// No WMI Solution for Windows Security!
	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CWindowsSecurityScanner::ScanRegistryXP()
{
	try
	{
		ScanWindowsSecurity();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CWindowsSecurityScanner::ScanRegistryNT()
{
	try
	{
		ScanWindowsSecurity();
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
bool	CWindowsSecurityScanner::ScanRegistry9X()
{
	try
	{
		ScanWindowsSecurity();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CWindowsSecurityScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CWindowsSecurityScanner::SaveData Start" ,true);

	// Do we have any Windows Update Information?
	if (_updateInformation.WindowsUpdateValid())
	{
		// Add the Category for Windows Update
		CAuditDataFileCategory category(HARDWARE_CLASS_UPDATE);

		// add the items to this category
		CAuditDataFileItem itemStatus(V_UPDATE_STATUS ,_updateInformation.GetUpdateState());
		category.AddItem(itemStatus);

		if (!_updateInformation.ScheduledInstallDate().IsEmpty())
		{
			CAuditDataFileItem item(V_UPDATE_INSTALLDATE ,_updateInformation.ScheduledInstallDate());
			category.AddItem(item);
		}

		if (!_updateInformation.NextDetectionTime().IsEmpty())
		{
			CAuditDataFileItem item(V_UPDATE_NEXTDETECT ,_updateInformation.NextDetectionTime());
			category.AddItem(item);
		}

		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddAuditDataFileItem(category);
	}
	
	// Now handle the Windows Firewall Settings
	if (_firewallInformation.FirewallValid())
	{
		// Add the Category for DOMAIN FIREWALL
		CAuditDataFileCategory category1(HARDWARE_CLASS_FIREWALL2);

		// Two groups of settings so add each separately
		CFirewallSettings* pDomainSettings = _firewallInformation.DomainSettings();
		CFirewallSettings* pStandardSettings = _firewallInformation.StandardSettings();

		// First the domain profile
		CAuditDataFileItem f1(V_FIREWALL_ENABLED ,(pDomainSettings->Enabled()) ? "Yes" : "No");
		CAuditDataFileItem f2(V_FIREWALL_EXCEPTIONS ,(pDomainSettings->NoExceptions()) ? "No" : "Yes");

		// Add these items to the category
		category1.AddItem(f1);
		category1.AddItem(f2);

		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddAuditDataFileItem(category1);

		// ...now the standalone profile
		CAuditDataFileCategory category2(HARDWARE_CLASS_FIREWALL1);
		CAuditDataFileItem sf1(V_FIREWALL_ENABLED ,(pStandardSettings->Enabled()) ? "Yes" : "No");
		CAuditDataFileItem sf2(V_FIREWALL_EXCEPTIONS ,(pStandardSettings->NoExceptions()) ? "No" : "Yes");

		// Add these items to the category
		category1.AddItem(sf1);
		category1.AddItem(sf2);

		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddAuditDataFileItem(category2);
	}
	
	log.Write("CWindowsSecurityScanner::SaveData End" ,true);
	return true;
}


//
//    ScanWindowsSecurity
//    ===================
//
//    Scan Windows Security settings
//
void CWindowsSecurityScanner::ScanWindowsSecurity()
{
	CLogFile log;
	log.Write("CWindowsSecurityScanner::ScanWindowsSecurity Start" ,true);
	try
	{
		_updateInformation.Detect();
		_firewallInformation.Detect();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	log.Write("CWindowsSecurityScanner::ScanWindowsSecurity End" ,true);
}