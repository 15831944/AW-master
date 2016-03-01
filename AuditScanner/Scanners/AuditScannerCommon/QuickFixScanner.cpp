
#include "stdafx.h"
#include "QuickFixScanner.h"
#include "SystemBiosScanner.h"
#include "AuditDataFile.h"


CQuickFix::CQuickFix()
{
	m_strCaption=UNKNOWN;
	m_strCSName=UNKNOWN;	
	m_strDescription=UNKNOWN;
	m_strFixComments=UNKNOWN;
	m_strHotFixID=UNKNOWN;
	m_strInstallDate=UNKNOWN;
	m_strInstalledBy=UNKNOWN;
	m_InstalledON=UNKNOWN;
	m_strName=UNKNOWN;	
	m_strServicePack=UNKNOWN;	
	m_strStatus=UNKNOWN;

}
CQuickFixScanner::CQuickFixScanner(void)
{
}

CQuickFixScanner::~CQuickFixScanner(void)
{
}

bool	CQuickFixScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CQuickFixScanner::ScanWMI Start" ,true);

	// Ensure that the list is empty
	m_listQuickFix.Empty();

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();
	CString		strBuffer;
	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_QuickFixEngineering"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty Quick Fix object
				CQuickFix objQuickFix;
				
				//
				strBuffer = wmiConnection.GetClassObjectStringValue("Caption");
				objQuickFix.Caption(strBuffer);
				//
				strBuffer = wmiConnection.GetClassObjectStringValue("CSName");
				objQuickFix.CSName(strBuffer);
				//
				strBuffer=wmiConnection.GetClassObjectStringValue("Description");
				objQuickFix.Description(strBuffer);
				//
				strBuffer=wmiConnection.GetClassObjectStringValue("FixComments");
				objQuickFix.FixComments(strBuffer);
				//
				strBuffer=wmiConnection.GetClassObjectStringValue("HotFixID");
				objQuickFix.HotFixID(strBuffer);
				//
				strBuffer=wmiConnection.GetClassObjectStringValue("InstallDate");
				objQuickFix.InstallDate(strBuffer);

				//
				strBuffer=wmiConnection.GetClassObjectStringValue("InstalledBy");
				objQuickFix.InstalledBy(strBuffer);

				//
				strBuffer=wmiConnection.GetClassObjectStringValue("InstalledOn");
				objQuickFix.InstalledON(strBuffer);

				//
				strBuffer = wmiConnection.GetClassObjectStringValue("Name");
				objQuickFix.Name(strBuffer);
				//
				strBuffer= wmiConnection.GetClassObjectStringValue("ServicePackInEffect");
				objQuickFix.ServicePack(strBuffer);

				//
				//
				strBuffer= wmiConnection.GetClassObjectStringValue("Status");
				objQuickFix.Status(strBuffer);
				
				m_listQuickFix.Add(objQuickFix);
				
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CQuickFixScanner::ScanWMI (Enumerating Win32_QuickFixEngineering)");
	}

	return true;
	
}

//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT/2000 or higher
//
bool	CQuickFixScanner::ScanRegistryNT()
{
	// Network adapter information is not recovered from the registry so use a common routine
	 return true;
}

//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CQuickFixScanner::ScanRegistryXP()
{
	// Network adapter information is not recovered from the registry so use a common routine
	return true;
}



//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
bool	CQuickFixScanner::ScanRegistry9X()
{
	// Network adapter information is not recovered from the registry so use a common routine
	return true;
}

bool	CQuickFixScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	return true;
}

