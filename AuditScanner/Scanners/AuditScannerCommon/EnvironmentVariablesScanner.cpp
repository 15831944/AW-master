#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "EnvironmentVariablesScanner.h"

// Storage Strings
#define CLASS						"System|Environment Variables"
#define V_ENVIRONMENT_NAME			"Name"
#define V_ENVIRONMENT_VALUE			"Value"

CEnvironmentVariablesScanner::CEnvironmentVariablesScanner(void)
{
	m_strItemPath = CLASS;
}

CEnvironmentVariablesScanner::~CEnvironmentVariablesScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CEnvironmentVariablesScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CEnvironmentVariablesScanner::ScanWMI Start" ,true);
	CString strBuffer;

	// Ensure that the list is empty
	_listVariables.Empty();

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_Environment"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				CEnvironmentVariable envVar;
				envVar.Name(wmiConnection.GetClassObjectStringValue("Name"));
				envVar.Value(wmiConnection.GetClassObjectStringValue("VariableValue"));

				// Add this process to our list (if it doesn't already exist!)
				if (!VariableExists(envVar.Name()))
					_listVariables.Add(envVar);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CEnvironmentVariablesScanner::ScanWMI");
		return false;
	}

	log.Write("CEnvironmentVariablesScanner::ScanWMI End" ,true);
	return true;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CEnvironmentVariablesScanner::ScanRegistryXP()
{
	try
	{
		ScanVariables();
	}
	catch (CException *pEx)
	{
		throw pEx;;
	}
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CEnvironmentVariablesScanner::ScanRegistryNT()
{
	try
	{
		ScanVariables();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}

//
//    ScanVariables
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CEnvironmentVariablesScanner::ScanRegistry9X()
{
	try
	{
		ScanVariables();
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
bool CEnvironmentVariablesScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CEnvironmentVariablesScanner::SaveData Start" ,true);

	CString itemName;

	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listVariables.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// All environment variables go in the same class - we do not audit changes for this category
		CAuditDataFileCategory category(CLASS ,FALSE);

		// Now add the variables
		for (int isub=0; isub<(int)_listVariables.GetCount(); isub++)
		{
			// Format the hardware class name for this drive
			CEnvironmentVariable* pItem = &_listVariables[isub];
			CAuditDataFileItem e1(pItem->Name() ,pItem->Value());

			// Add the variable if it does not already exist
			//if (!VariableExists(pItem->Name()))
				category.AddItem(e1);
		}

		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddAuditDataFileItem(category);
	}

	log.Write("CEnvironmentVariablesScanner::SaveData End" ,true);
	return true;
}



//
//    ScanVariables
//    =============
//
//    This function does the bulk of the work if WMI has not been successful.  It is used for all OS
//    platforms
//
BOOL CEnvironmentVariablesScanner::ScanVariables()
{
	CLogFile log;
	log.Write("CEnvironmentVariablesScanner::ScanVariables Start" ,true);

	try
	{
		// Ensure that the list is empty
		_listVariables.Empty();

		//
		LPSTR pszEnv = (LPSTR)GetEnvironmentStrings();
		if (pszEnv)
		{
			for (LPSTR p = pszEnv ; *p ; p += strlen(p) + 1)
			{
				// decipher returned string into key + value
				char * pszBlock = new char[strlen(p) + 1];
				strcpy (pszBlock, p);
				LPCSTR pszKey = strtok(pszBlock, "=");
				LPCSTR pszValue = strtok(NULL, "=");

				// write to storage unless we have already found it
				CEnvironmentVariable variable(pszKey ,pszValue);
				_listVariables.Add(variable);
				delete [] pszBlock;
			}
			FreeEnvironmentStrings (pszEnv);
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CEnvironmentVariablesScanner::ScanVVariables Start" ,true);
	return true;
}

BOOL CEnvironmentVariablesScanner::VariableExists(CString& name)
{
	for (int index=0; index<(int)_listVariables.GetCount(); index++)
	{
		CEnvironmentVariable* pItem = &_listVariables[index];
		if (pItem->Name() == name)
			return TRUE;
	}

	return FALSE;
}
