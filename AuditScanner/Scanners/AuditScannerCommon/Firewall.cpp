//------------------------------------------------------------------------------
// CFirewallInfo.cpp
//    
//   This file contains CFirewallInfo, which grabs information about the Windows Firewall if
//	 installed.
// 
#include "stdafx.h"

#include "Firewall.h"

const char gszFirewallKey[]	= "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy";
#define REG_BUFFER_SIZE	1024



//======================================================================================
//
//    CFirewallSettings
//
//    Helper class to store settings for a firewall profile
//
//    Constructor
//
CFirewallSettings::CFirewallSettings(LPCSTR pszProfileName) : m_strProfileName(pszProfileName)
{
	Detect();
}


CFirewallSettings::~CFirewallSettings()
{
}

void CFirewallSettings::Detect(void)
{
	HKEY hKey;
	CString strProfileKey;
	char szBuffer[REG_BUFFER_SIZE];
	DWORD dwLength = REG_BUFFER_SIZE;
	DWORD dwIndex = 0;

	// Construct the name of the registry key for this firewall profile
	strProfileKey.Format("%s\\%s" ,gszFirewallKey ,m_strProfileName);

	// Check to see if the firewall is enabled in this profile
	m_bEnabled = (CReg::GetItemInt(HKEY_LOCAL_MACHINE, strProfileKey.GetBuffer(0), "EnableFirewall") != 0);
	if (!m_bEnabled)
		return;

	// OK - so it's enabled - what about exceptions
	m_bNoExceptions = (CReg::GetItemInt(HKEY_LOCAL_MACHINE, strProfileKey.GetBuffer(0), "DoNotAllowExceptions") != 0);
	if (m_bNoExceptions)
		return;

	// OK exceptions are permitted so recover application exceptions
	CString strApplicationsKey = strProfileKey + "\\AuthorizedApplications\\List";
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, strApplicationsKey, 0, KEY_READ, &hKey) == ERROR_SUCCESS)
	{
		while (RegEnumValue(hKey, dwIndex++, szBuffer, &dwLength, NULL, NULL, NULL, NULL) != ERROR_NO_MORE_ITEMS)
		{
			// Get the application name and add to our internal list
			CReg::GetItemString(hKey, szBuffer, "Name");
			CString strApplicationName = szBuffer;
			if (!strApplicationName.IsEmpty())
				m_listApplications.Add(strApplicationName);
			dwLength = REG_BUFFER_SIZE;
		}
		RegCloseKey(hKey);
	}
	
	// ...then do the same for Globally Open Ports
	CString strPortsKey = strProfileKey + "\\GloballyOpenPorts\\List";
	dwIndex = 0;
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, strPortsKey, 0, KEY_READ, &hKey) == ERROR_SUCCESS)
	{
		while (RegEnumValue(hKey, dwIndex++, szBuffer, &dwLength, NULL, NULL, NULL, NULL) != ERROR_NO_MORE_ITEMS)
		{
			// Get the application name and add to our internal list
			CReg::GetItemString(hKey, szBuffer, "Name");
			CString strPortName = szBuffer;
			if (!strPortName.IsEmpty())
				m_listPorts.Add(strPortName);
			dwLength = REG_BUFFER_SIZE;
		}
		RegCloseKey(hKey);
	}
}


//
//    Constructor
//
CFirewallInfo::CFirewallInfo()
{
	m_bFirewallValid = FALSE;
	m_pDomainSettings = NULL;
	m_pStandardSettings = NULL;
}


CFirewallInfo::~CFirewallInfo()
{
	if (m_pDomainSettings != NULL)
		delete m_pDomainSettings;
	if (m_pStandardSettings != NULL)
		delete m_pStandardSettings;
}


//
//    Detect
//    ======
//
//    Detect Firewall Settings
//
void CFirewallInfo::Detect(void)
{
	try
	{
		// Firewall only on Windows 2000 and above and even then not always
		COsInfo os;
		if (os.GetClass() < COsInfo::win2K)
			return;

		// See if we can determine if the firewall registry hive is present
		HKEY hKey;
		m_bFirewallValid = (RegOpenKeyEx (HKEY_LOCAL_MACHINE, gszFirewallKey, 0, KEY_READ, &hKey) == ERROR_SUCCESS);
		RegCloseKey (hKey);
		if (!m_bFirewallValid)
			return;
		
		// Create the two settings objects for domain and standard profiles
		m_pDomainSettings = new CFirewallSettings("DomainProfile");
		m_pStandardSettings = new CFirewallSettings("StandardProfile");
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
}

