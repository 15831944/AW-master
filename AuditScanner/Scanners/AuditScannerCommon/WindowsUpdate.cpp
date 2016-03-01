//------------------------------------------------------------------------------
// WindowsUpdate.cpp
//    
//   This file contains CWindowsUpdate, which grabs information about the status of Windows Update
// 
#include "stdafx.h"

#include "WindowsUpdate.h"

const char gszUpdateKey[]	= "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update";
#define REG_BUFFER_SIZE	1024




//
//    Constructor
//
CWindowsUpdate::CWindowsUpdate()
{
	m_strUpdateState = "";
	m_strNextDetectionTime = "";
	m_strScheduledInstallDate = "";
}


CWindowsUpdate::~CWindowsUpdate()
{
}


//
//    Detect
//    ======
//
//    Detect Windows Update Settings
//
void CWindowsUpdate::Detect(void)
{
	try
	{
		// Does the Windows Update registry key exist?
		HKEY hKey;
		m_bUpdateValid = (RegOpenKeyEx (HKEY_LOCAL_MACHINE, gszUpdateKey, 0, KEY_READ, &hKey) == ERROR_SUCCESS);
		RegCloseKey (hKey);
		if (!m_bUpdateValid)
			return;

		// OK Windows update supported - Get the options value which will provide most of the required information
		// Construct the name of the registry key for this firewall profile
		DWORD dwOptions = CReg::GetItemInt(HKEY_LOCAL_MACHINE, gszUpdateKey, "AUOptions");
		switch (dwOptions)
		{
		case 1:
			m_strUpdateState = "Keep my computer up to date has been disabled in Automatic Updates";
			break;
		case 2: 
			m_strUpdateState = "Notify of download and installation";
			break;
		case 3: 
			m_strUpdateState = "Automatically download and notify of installation";
			break;
		case 4:
			m_strUpdateState = "Automatically download and scheduled installation";
			break;
		default:
			m_strUpdateState = "Unknown state";
		}

		// 
		if (dwOptions > 1)
			m_strNextDetectionTime = CReg::GetItemString(HKEY_LOCAL_MACHINE, gszUpdateKey, "NextDetectionTime"); 
		if (dwOptions == 4)
			m_strScheduledInstallDate = CReg::GetItemString(HKEY_LOCAL_MACHINE, gszUpdateKey, "ScheduledInstallDate"); 
	}
	catch (CException *pEx)
	{
		throw pEx;;
	}
}

