#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "RegistryScanner.h"
#include "AuditDataScanner.h"
#include "WMIScanner.h"

// Storage Strings
#define REGISTRY_CLASS					"System|Registry"
#define V_REGKEY_NAME			"Name"
#define V_REGKEY_VALUE			"Value"

CRegistryScanner::CRegistryScanner()
{
}

CRegistryScanner::~CRegistryScanner()
{
}


void CRegistryScanner::SetOptions (CDynaList<CString>& listRegKeysToAudit)
{
	_listRegKeysToAudit = listRegKeysToAudit;
}

bool CRegistryScanner::Scan(void)
{
	CLogFile log;

	try
	{
		for (int i = 0; i < _listRegKeysToAudit.GetCount(); i++)
		{
			CStringArray registryKeyArray;

			log.Format("scanning registry key '%s' \n", _listRegKeysToAudit[i]);

			SplitString(_listRegKeysToAudit[i], ';', registryKeyArray);

			CString strValue = GetRegValue(registryKeyArray[0], registryKeyArray[1]);
			log.Format("-> found value %s \n", strValue);

			CRegistryKey keys(registryKeyArray[0] + "\\" + registryKeyArray[1], strValue);
			_listRegKeysFound.Add(keys);
		}
	}
	catch (CException *pEx)
	{
   		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		log.Format("An exception occured during CRegistryScanner::Scan - the message text was %s\n" ,szCause);
   		pEx->Delete();
		return false;
	}
	return true;
}


bool CRegistryScanner::Save	(CAuditDataFile& auditDataFile)
{
	CLogFile log;
	log.Write("CRegistryScanner::SaveData Start" ,true);

	CString itemName;

	// that however we do need to add our own registry-item section if we are going to write anything
	if (_listRegKeysFound.GetCount() != 0)
	{
		// Write a placeholder item for the registry class itself as this will ensure that the category can be displayed
		//CAuditDataFileCategory mainCategory(CLASS);
		//auditDataFile.AddAuditDataFileItem(mainCategory);

		// All registry keys go in the same class - we do not audit changes for this category
		//CAuditDataFileCategory category(CLASS ,FALSE);
		CAuditDataFileCategory category(REGISTRY_CLASS);

		// Now add the keys
		for (int isub=0; isub<(int)_listRegKeysFound.GetCount(); isub++)
		{
			// Format the registry class name for this drive
			CRegistryKey* pItem = &_listRegKeysFound[isub];
			CAuditDataFileItem e1(pItem->Name() ,pItem->Value());

			// Add the registry key if it does not already exist
			category.AddItem(e1);
		}

		// ...and add the category to the AuditDataFile
		auditDataFile.AddAuditDataFileItem(category);
	}

	//log.Write("CRegistryScanner::SaveData End" ,true);
	return true;
}

//
//    GetRegValue
//    ===========
//
//    Recover a value from the system registry
//
CString CRegistryScanner::GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem)
{
	CString strResult;

	// work out which "hive" to access
	CString strRegKey(pszRegKey);
	CString strHive = BreakString(strRegKey, '\\');
	HKEY hkHive = NULL, hkSubKey;
	if (strHive == "HKEY_CLASSES_ROOT")
		hkHive = HKEY_CLASSES_ROOT;
	if (strHive == "HKEY_CURRENT_CONFIG")
		hkHive = HKEY_CURRENT_CONFIG;
	if (strHive == "HKEY_CURRENT_USER")
		hkHive = HKEY_CURRENT_USER;
	if (strHive == "HKEY_LOCAL_MACHINE")
		hkHive = HKEY_LOCAL_MACHINE;
	if (strHive == "HKEY_USERS")
		hkHive = HKEY_USERS;

	if (hkHive)
	{
		int nStatus = RegOpenKeyEx(hkHive, strRegKey, 0, KEY_QUERY_VALUE, &hkSubKey);
		if (nStatus == ERROR_SUCCESS)
		{
			// key opened ok - look for matching item
			DWORD dwIndex = 0;
			unsigned char szThisRegValue[1024];
			DWORD dwType;
			DWORD dwRegValueLen = sizeof(szThisRegValue);

			int nStatus = RegQueryValueEx(hkSubKey ,pszRegItem ,NULL ,&dwType ,szThisRegValue ,&dwRegValueLen);
			if (nStatus == ERROR_SUCCESS)
			{
				// FOUND IT - sort out the type conversion
				switch (dwType)
				{
					case REG_BINARY:
						{
							// write as a sequence of hex values
							for (DWORD dw = 0 ; dw < dwRegValueLen ; dw++)
							{
								BYTE b = szThisRegValue[dw];
								CString strThisBit;
								strThisBit.Format("%2.2X ", b);
								strResult += strThisBit;
							}
							strResult.TrimRight();
						}
						break;

					case REG_DWORD:
//					case REG_DWORD_LITTLE_ENDIAN:
						strResult.Format("%d", *((LPDWORD)szThisRegValue));
						break;
						
					case REG_SZ:
						strResult = szThisRegValue;
						break;

					case REG_EXPAND_SZ:
						{
							char szBuffer[1024];
							ExpandEnvironmentStrings ((LPCSTR)szThisRegValue, szBuffer, sizeof(szBuffer));
							strResult = szBuffer;
						}
						break;

					case REG_MULTI_SZ:
						{
							for (char * p = (LPSTR)szThisRegValue ; *p != NULL ; p += strlen(p) + 1)
							{
								if (strResult.GetLength())
									strResult += ';';
								strResult += p;
							}
						}
						break;

					case REG_DWORD_BIG_ENDIAN:
					case REG_LINK:
					case REG_NONE:
//					case REG_QWORD:
//					case REG_QWORD_LITTLE_ENDIAN:
					case REG_RESOURCE_LIST:
					default:
						strResult.Format("Unsupported Registry Data Type %d", dwType);
						break;
				}
			}
		}
		RegCloseKey(hkSubKey);
	
	
	}
	return strResult;
}