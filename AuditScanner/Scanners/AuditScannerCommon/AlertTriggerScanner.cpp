//////////////////////////////////////////////////////////////////////////////////////////
//																						//
//    AlertTriggerScanner.cpp															//
//    =======================															//
//																						//
//--------------------------------------------------------------------------------------//
//																						//
//	History																				//
//		17-NOV-2014		Chris Drew				AW 8.4.3								//
//		Problem 334 - IP Address change not being detected by Alert Monitor				//
//		Firstly you cannot check the Asset Data Field IP address as this is not			//
//		tested by AlertMonitor.  Secondly the 'historied' check was reversed meaning	//
//		that many hardware fields would not be tested as they were flagged as historied //
//		(the default) whereas the code was saying not to test any fields flagged as		//
//		historied.  Checking the AuditDataItems (and the comment) this was the wrong	//
//		way around.																		//
//																						//
//////////////////////////////////////////////////////////////////////////////////////////


#include "stdafx.h"

#include "AlertTriggerScanner.h"
#include "AlertDefinitionsFile.h"
#include "AlertNotificationFile.h"

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
#include "RegistryScanner.h"
#include "OperatingSystemScanner.h"


//
//	CreateAlertScanner
//  ==================
//
//  This function is called as part of the AlertMonitor processing - its task is to determine what type of 
//  scanner is required to recover the required data based on the Category passed in and return it
//
CAuditDataScanner* CAlertTriggerScanner::CreateAlertScanner (LPCSTR pszCatName)
{
	// build a temporary array of all possible category scanners
	CDynaList<CAuditDataScanner*> tempList;
	
	// First lets build the list of individual hardware scanners that we will use
	tempList.Add(new CGraphicsAdaptersScanner());
	tempList.Add(new CNetworkAdaptersScanner());
	tempList.Add(new CNetworkInformationScanner());
	tempList.Add(new CSystemBiosScanner());
	tempList.Add(new CSystemProcessorScanner());
	tempList.Add(new CMemorySlotScanner());
	tempList.Add(new CMemoryScanner());
	tempList.Add(new CPrinterScanner());
	tempList.Add(new CPhysicalDiskScanner());
	tempList.Add(new CLogicalDriveScanner());
	tempList.Add(new CActiveProcessScanner());
	tempList.Add(new CServicesScanner());
	tempList.Add(new CEnvironmentVariablesScanner());
	tempList.Add(new CLocaleScanner());
	tempList.Add(new CObjectScanner());
	tempList.Add(new CUsersScanner());
	tempList.Add(new CWindowsSecurityScanner());

	// loop through and see if we can find the requested category by matching the category name passed in 
	// with the item name stored in each AuditItemScanner object
	CAuditDataScanner* pResult = NULL;
	for (int index=0; index < (int)tempList.GetCount() ; index++)
	{
		// get the Item Name for this scanner
		CString itemName = tempList[index]->ItemName();
		
		// ...and compare against what we are looking for
		if (0 == strncmp(itemName, pszCatName, itemName.GetLength()))
		{
			pResult = tempList[index];
			TRACE("An Alert Trigger Scanner for category %s was allocated\n" ,itemName);
			break;
		}
		else
		{
			delete tempList[index];
		}
	}
	
	return pResult;
}




/////////////////////////////////////////////////////////////////////////////////
//
//    CAlertTriggerScannerList Class
//
//    This class maintains a list of variant CAlertTriggerScanner derived objects
//
CAlertTriggerScannerList::~CAlertTriggerScannerList()
{
	// remember the contents of our encapsulated array are pointers - they need to be cleaned out
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		delete ((m_pData[dw]));
}


//
//    Setup
//    =====
//
//    This function initializes the list of variant CAlertTriggerScanner derived objects
//    taking as input the name of the asset being scanned and the AlertDefinitiions file
//    within which we can find the list of Alerts defined.
//
//    We need to create a CAlertTriggerScanner object for each alert trigger defined within
//    this file.
//
int CAlertTriggerScannerList::Setup (CAuditScannerConfiguration* pScannerConfiguration ,CWMIScanner* pWMIScanner ,CAlertDefinitionsFile& definitionsFile, LPCSTR pszAsset, CTime& tmLastAudit)
{
	// Save the scanner configuration in case we need it later
	_pScannerConfiguration = pScannerConfiguration;
	
	int nCount = 0;

	// Loop through the Alert Definitions defined 
	CDynaList<CAlertDefinition*>& alertDefinitions = definitionsFile.AlertDefinitions();
	 
	for (int index=0; index < (int)alertDefinitions.GetCount(); index++)
	{
		// get the Alert Definition itself
		CAlertDefinition* pAlertDefinition = alertDefinitions[index];	
		CString alertName = pAlertDefinition->AlertName();
		
		// Now loop through the triggers within this alert definition
		CDynaList<CAlertTrigger*>& alertTriggers = pAlertDefinition->AlertTriggers();
		for (int triggerIndex = 0; triggerIndex < (int)alertTriggers.GetCount(); triggerIndex++)
		{	
			// Get the Alert Trigger itself
			CAlertTrigger* pAlertTrigger = alertTriggers[triggerIndex];
	
			// We need to determine what category of item is being checked by this trigger - this is the first
			// value in the pipe delimited Field
			CString strBuffer = pAlertTrigger->Field();
			CString strType = BreakString(strBuffer, '|', FALSE);

			// check for Registry
			if (strBuffer.Left(15) == "System|Registry")
			{
				//CAlertTriggerScanner_Registry* pScanner = new CAlertTriggerScanner_Registry(alertName, _pScannerConfiguration);
				CAlertTriggerScanner_Registry* pScanner = new CAlertTriggerScanner_Registry(alertName
																			, pAlertTrigger->Condition()
																			, pAlertTrigger->Value()
																			, pAlertTrigger->Field()); 

				Add (pScanner);
				nCount++;
			}

			else if (strType == "Hardware" || strType == "System")
			{
				// Create a new Hardware Alert Scanner for this and add to our internal list of scanners
				CAlertTriggerScanner_HW* pScanner = new CAlertTriggerScanner_HW(alertName
																			, pAlertTrigger->Condition()
																			, pAlertTrigger->Value()
																			, pAlertTrigger->Field());
				pScanner->SetWMIScanner(pWMIScanner);				// Ensure we tell the scanner about WMI
				Add (pScanner);
				TRACE("Hardware AlertMonitor Trigger Scanner allocated for alert %s Value %s Field %s\n", alertName ,pAlertTrigger->Value() ,pAlertTrigger->Field());
				nCount++;
			}

			else if (strType == "Internet")
			{
				// Create a new Internet Alert Scanner for this and add to our internal list of scanners
				CAlertTriggerScanner_IE* pScanner = new CAlertTriggerScanner_IE(alertName
																			, pAlertTrigger->Value()
																			, _pScannerConfiguration);
				Add (pScanner);
				nCount++;
			}

			else if (strType == "Operating Systems")
			{
				// Create a new OS Alert Scanner for this and add to our internal list of scanners
				CString strKey = strBuffer;
				CAlertTriggerScanner_OS* pScanner = new CAlertTriggerScanner_OS(alertName
																			, pAlertTrigger->Condition()
																			, pAlertTrigger->Value()
																			, strKey);
				Add (pScanner);
				nCount++;
			}

			else if (strType == "Applications")
			{
				// Create a new Internet Alert Scanner for this and add to our internal list of scanners
				CAlertTriggerScanner_SW* pScanner = new CAlertTriggerScanner_SW(alertName, _pScannerConfiguration);
				Add (pScanner);
				nCount++;
			}

			else
			{
				TRACE("Unknown Trigger Type %s\n", strType);
			}
		}
	}

	return nCount;
}



//
//    Save
//    ====
//
//    This function is called to force each of our alert trigger scanners to save their current state to 
//    the specified ini file.  The ini file is in fact our asset cache file but this is not relevent really
//
void CAlertTriggerScannerList::Save (CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	// Ensure that the file is clean before we save to it
	newCacheFile.Clean();
	
	// Call each Alert Scanner in turn and request them to save themselves
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		((m_pData[dw]))->Save(oldCacheFile, newCacheFile);
}



//
//    Scan
//    ====
//
//    This function is called when we need to recover the latest value for all trigger fields.  We loop
//    through each of our alert trigger scanners calling their individual Scan functions which will recover
//    the latest value for each field
//
BOOL CAlertTriggerScannerList::Scan ()
{
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		if (!((m_pData[dw]))->Scan())
			return FALSE;
	}
	return TRUE;
}



//
//    Test
//    ====
//
//    This function is called when we are checking to see if any changes have occured which need to be alerted
//    We loop through our Alert Trigger Scanners calling each Test function and return a count of those which
//    have indicated that an alert should be generated.
//
int CAlertTriggerScannerList::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile)
{
	int nCount = 0;
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		nCount += ((m_pData[dw]))->Test(notificationFile ,oldDataFile ,newDataFile);
	}
	return nCount;
}







///////////////////////////////////////////////////////////////////////////////
// Hardware scanning class

//
// Constructor
//
CAlertTriggerScanner_HW::CAlertTriggerScanner_HW(LPCSTR pszAlertName
											  , int condition
											  , LPCSTR pszTestVal
											  , LPCSTR pszCat)
	: CAlertTriggerScanner(pszAlertName, condition, pszTestVal), _category(pszCat)
{
	_pAuditDataScanner = NULL;
	_pWMIScanner = NULL;
	
	// try and load the correct hardware scanner based on the category name
	_pAuditDataScanner = CreateAlertScanner(_category);
	
	// We should now be able to determine what the key or value name is - For example if the category
	// is passed in as 'Hardware|CPU|Speed' we will be returned the 'Hardware|CPU' scanner so we know
	// from this that the key or value name is 'Speed'.
	//
	// If however we are passed 'Hardware|CPU' then again we will match with the 'Hardware|CPU' scanner 
	// but this time we will identify that there is no key and must therefore check all values in the
	// whole category.
	CString scannerCategory = _pAuditDataScanner->ItemName();
	if (_category == scannerCategory)
	{
		_key = "";
	}
	else
	{
		int delimiter = _category.ReverseFind('|');
		_key = _category.Mid(delimiter + 1);
		_category = _category.Left(delimiter);		
	}
}


//
// Destructor
//
CAlertTriggerScanner_HW::~CAlertTriggerScanner_HW ()
{
	if (_pAuditDataScanner)
		delete _pAuditDataScanner;
}


//
//   Collect results for this category
//
BOOL CAlertTriggerScanner_HW::Scan()
{
	_pAuditDataScanner = NULL;

	// set up the scanner and execute it
	_pAuditDataScanner = CreateAlertScanner(_category);
	if (_pAuditDataScanner)
		return _pAuditDataScanner->Scan(_pWMIScanner);

	return FALSE;
}



//
// Test to see whether trigger has "fired"
//
BOOL CAlertTriggerScanner_HW::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile)
{
	BOOL bRet=FALSE;
	CString strNextKey;

	TRACE("CAlertTriggerScanner_HW::Test, checking values for Category %s with key %s\n" ,_category ,_key);

	// See if we can locate a cached value for this hardware category / key, we exit if we can't find a match
	CAuditDataFileCategory* pOldAuditedCategory = oldDataFile.FindAuditDataCategory(_category);
	if (pOldAuditedCategory == NULL)
	{
		TRACE("CAlertTriggerScanner_HW::Test, No OLD cached value found for this CATEGORY, exiting\n");
		return FALSE;
	}
	
	// Recover the new values for this category also
	CAuditDataFileCategory* pNewAuditedCategory = newDataFile.FindAuditDataCategory(_category);
	if (pNewAuditedCategory == NULL)
	{
		TRACE("CAlertTriggerScanner_HW::Test, No NEW value found for this CATEGORY, exiting\n");
		return FALSE;
	}
			
	// OK we have found the Category and can now start testing values - we may be either testing a single value or possible
	// all values audited by the scanner.
	CDynaList<CAuditDataFileItem>* pOldAuditDataFileItems = pOldAuditedCategory->Items();
	CDynaList<CAuditDataFileItem>* pNewAuditDataFileItems = pOldAuditedCategory->Items();
	//
	for (int index=0; index < (int)pOldAuditDataFileItems->GetCount(); index++)
	{
		// Get the 'old' item from the category
		CAuditDataFileItem* pOldItem = &(*pOldAuditDataFileItems)[index];
		
		// Try and find a match in the new category
		CAuditDataFileItem* pNewItem = pNewAuditedCategory->FindAuditDataItem(pOldItem->Name());
		if (pNewItem == NULL)
		{
			TRACE("CAlertTriggerScanner_HW::Test, No NEW item found for the field %s, skipping\n" ,pOldItem->Name());
			continue;
		}

		// OK we have both the old and the new item but do we need to test it		
		if ((_key == "") || (_key == pOldItem->Name()))
		{
			// We do NOT test items which are flagged as not being historied as these are items which we
			// expect to change and therefore do not want alerts for.  We do however still need to check for threshholds so 
			// we only skip alert monitor processing if we are looking for CHANGES.
			//if (!pOldItem->Historied())
			if (pOldItem->Historied() || _condition != opChanged)								// CMD 8.4.2 Reverse this check as we check historied items not the other way around
			{
				if (TestValue(notificationFile ,pOldItem ,pNewItem))
				{
					bRet=TRUE;
				}
			}
			else
			{
				TRACE("CAlertTriggerScanner_HW::Test, Skipping test of field %s as it is flagged as not historied\n" ,pOldItem->Name());
			}			
		}
	}	
	
	return bRet;
	
}


//
//   TestValue
//   =========
//
//   Called to test a specific audited item against an old version
//	
BOOL CAlertTriggerScanner_HW::TestValue (CAlertNotificationFile& notificationFile ,CAuditDataFileItem* pOldItem ,CAuditDataFileItem* pNewItem)
{
	BOOL bRet=FALSE;	
	TRACE("CAlertTriggerScanner_HW::TestValue  Comparing OLD VALUE %s against NEW VALUE %s\n" ,pOldItem->Value(), pNewItem->Value());
	switch(_condition)
	{
	case opChanged:
		{
			if( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , pOldItem->Value()
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
	case opLessThan:
		{
			if((atoi(pNewItem->Value()) <  atoi(_testValue)))//&&( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , _testValue
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
		case opGreaterThan:
		{
			if((atoi(pNewItem->Value()) > atoi(_testValue)))//&&( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , _testValue
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
		case opEquals:
		{
			if((atoi(pNewItem->Value()) ==  atoi(_testValue)))//&&( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , _testValue
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
		case opLessEqual:
		{
			if((atoi(pNewItem->Value()) <=  atoi(_testValue)))//&&( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , _testValue
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
		case opGreaterEqual:
		{
			if((atoi(pNewItem->Value()) >=  atoi(_testValue)))//&&( pOldItem->Value() != pNewItem->Value())
			{
				TRACE("CAlertTriggerScanner_HW::TestValue, Generating an Alert Notification\n");
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , _category
																	  , pOldItem->Name()
																	  , _testValue
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bRet= TRUE;
			}
		}
		break;
	}	
	return bRet;
}




//
//    Save
//    ====
//
//    Write the current value(s) audited by this scanner to the AlertMonitor cache file which has been 
//    passed to us.
//
void CAlertTriggerScanner_HW::Save(CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	_pAuditDataScanner->SaveData(&newCacheFile);
}

///////////////////////////////////////////////////////////////////////////////
// 
//    Registry Keys scanning class

//
// Constructor
//

CAlertTriggerScanner_Registry::CAlertTriggerScanner_Registry(LPCSTR pszAlertName
											  , int condition
											  , LPCSTR pszTestVal
											  , LPCSTR pszCat)
	: CAlertTriggerScanner(pszAlertName, condition, pszTestVal), _category(pszCat)
{
	_pRegistryScanner = new CRegistryScanner();	
}

//
// Destructor
//
CAlertTriggerScanner_Registry::~CAlertTriggerScanner_Registry ()
{
	if (_pRegistryScanner != NULL)
		delete _pRegistryScanner;
}


//
//   Collect results for this category
//
BOOL CAlertTriggerScanner_Registry::Scan()
{
	// need to set up the registry key in the correct format
	// from : System|Registry|HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\AuditWizard Agent\ImagePath
	// to   : HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\AuditWizard Agent;ImagePath

	CDynaList<CString> regsitryKey;

	int nLen = strlen(_category);
	int lastPosition = -1;
	int lastVerticalLine = -1;

	for (int n = 0 ; n < nLen ; n++)
	{
		if (_category[n] == '\\')
			lastPosition = n;

		if (_category[n] == '|')
			lastVerticalLine = n;
	}

	if (lastPosition != -1)
		_category.SetAt(lastPosition, ';');

	if (lastVerticalLine != -1)
		_category = _category.Right( _category.GetLength() - lastVerticalLine - 1);
	
	regsitryKey.Add(_category);

	_pRegistryScanner->SetOptions(regsitryKey);
	_pRegistryScanner->Scan();

	return TRUE;
}

//
//    Save
//    ====
//
//    Write the current value(s) audited by this scanner to the AlertMonitor cache file which has been 
//    passed to us.
//
void CAlertTriggerScanner_Registry::Save(CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	_pRegistryScanner->Save(newCacheFile);
}

//
// Test to see whether trigger has "fired"
//
BOOL CAlertTriggerScanner_Registry::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile)
{
	BOOL bResult = FALSE;
	CString strNextKey;

	TRACE("CAlertTriggerScanner_Registry::Test, checking values for Category %s with key %s\n" ,_category ,_key);

	// See if we can locate a cached value for this hardware category / key, we exit if we can't find a match
	CAuditDataFileCategory* pOldAuditedCategory = oldDataFile.FindAuditDataCategory("System|Registry");
	if (pOldAuditedCategory == NULL)
	{
		TRACE("CAlertTriggerScanner_Registry::Test, No OLD cached value found for this CATEGORY, exiting\n");
		return FALSE;
	}
	
	// Recover the new values for this category also
	CAuditDataFileCategory* pNewAuditedCategory = newDataFile.FindAuditDataCategory("System|Registry");
	if (pNewAuditedCategory == NULL)
	{
		TRACE("CAlertTriggerScanner_Registry::Test, No NEW value found for this CATEGORY, exiting\n");
		return FALSE;
	}
			
	// OK we have found the Category and can now start testing values - we may be either testing a single value or possible
	// all values audited by the scanner.
	CDynaList<CAuditDataFileItem>* pOldAuditDataFileItems = pOldAuditedCategory->Items();
	CDynaList<CAuditDataFileItem>* pNewAuditDataFileItems = pOldAuditedCategory->Items();
		
	for (int index=0; index < (int)pOldAuditDataFileItems->GetCount(); index++)
	{
		// Get the 'old' item from the category
		CAuditDataFileItem* pOldItem = &(*pOldAuditDataFileItems)[index];
		
		// Try and find a match in the new category
		CAuditDataFileItem* pNewItem = pNewAuditedCategory->FindAuditDataItem(pOldItem->Name());
		if (pNewItem == NULL)
		{
			TRACE("CAlertTriggerScanner_HW::Test, No NEW item found for the field %s, skipping\n" ,pOldItem->Name());
			continue;
		}

		// OK we have both the old and the new item but do we need to test it		
		if ((_key == "") || (_key == pOldItem->Name()))
		{	

			if (pOldItem->Value() != pNewItem->Value())
			{
				CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::hardware
																	  , NULL//_category
																	  , pOldItem->Name()
																	  , pOldItem->Value()
																	  , pNewItem->Value());					
				// Add this change to the notification file
				notificationFile.AlertNotifications().Add(pAlertNotification);
				bResult = TRUE;	
			}			
		}
	}
	
	return bResult;
}

///////////////////////////////////////////////////////////////////////////////
// 
//    OS scanning class

//
// Constructor
//

CAlertTriggerScanner_OS::CAlertTriggerScanner_OS(LPCSTR pszAlertName
											  , int condition
											  , LPCSTR pszTestVal
											  , LPCSTR pszCat)
	: CAlertTriggerScanner(pszAlertName, condition, pszTestVal), _category(pszCat)
{
	_pOperatingSystemScanner = new COperatingSystemScanner();	
}

//
// Destructor
//
CAlertTriggerScanner_OS::~CAlertTriggerScanner_OS ()
{
	if (_pOperatingSystemScanner != NULL)
		delete _pOperatingSystemScanner;
}


//
//   Collect results for this category
//
BOOL CAlertTriggerScanner_OS::Scan()
{
	_pOperatingSystemScanner->Scan();
	return TRUE;
}

//
//    Save
//    ====
//
//    Write the current value(s) audited by this scanner to the AlertMonitor cache file which has been 
//    passed to us.
//
void CAlertTriggerScanner_OS::Save(CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	_pOperatingSystemScanner->Save(newCacheFile);
}

//
// Test to see whether trigger has "fired"
//
BOOL CAlertTriggerScanner_OS::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile)
{
	BOOL bResult = FALSE;
	BOOL bChangesFound = FALSE;
	CString strNextKey;

	CString oldValue;
	CString newValue;

	TRACE("CAlertTriggerScanner_OS::Test, checking values for Category %s with key %s\n" ,_category ,_key);

	if (_category == "Operating Systems|Family")
	{
		if (oldDataFile.OSFamily() != newDataFile.OSFamily())
		{
			oldValue = oldDataFile.OSFamily();
			newValue = newDataFile.OSFamily();

			bChangesFound = TRUE;
		}
	}
	
	else if (_category == "Operating Systems|Version")
	{
		if (oldDataFile.OSVersion() != newDataFile.OSVersion())
		{
			oldValue = oldDataFile.OSVersion();
			newValue = newDataFile.OSVersion();

			bChangesFound = TRUE;
		}
	}

	else if (_category == "Operating Systems|CD Key")
	{
		if (oldDataFile.OSCDKey() != newDataFile.OSCDKey())
		{
			oldValue = oldDataFile.OSCDKey();
			newValue = newDataFile.OSCDKey();

			bChangesFound = TRUE;
		}
	}

	else if (_category == "Operating Systems|Serial Number")
	{
		if (oldDataFile.OSProductID() != newDataFile.OSProductID())
		{
			oldValue = oldDataFile.OSProductID();
			newValue = newDataFile.OSProductID();

			bChangesFound = TRUE;
		}
	}

	if (bChangesFound)
	{
		CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																	  , CAlertNotification::software
																	  , NULL//_category
																	  , _category
																	  , oldValue
																	  , newValue);	

		// Add this change to the notification file
		notificationFile.AlertNotifications().Add(pAlertNotification);
		bResult = TRUE;	
	}

	
	return bResult;
}

///////////////////////////////////////////////////////////////////////////////
// 
//    Software Applications scanning class

//
// Constructor
//
CAlertTriggerScanner_SW::CAlertTriggerScanner_SW(LPCSTR pszAlertName ,CAuditScannerConfiguration* pScannerConfiguration)
	: CAlertTriggerScanner(pszAlertName, opChanged, "")
{
	_pScannerConfiguration = pScannerConfiguration;
	_pSoftwareScanner = new CSoftwareScanner();
}

//
// Destructor
//
CAlertTriggerScanner_SW::~CAlertTriggerScanner_SW ()
{
	if (_pSoftwareScanner != NULL)
		delete _pSoftwareScanner;
}

//
//   Collect results for this category
//
BOOL CAlertTriggerScanner_SW::Scan()
{
	_pSoftwareScanner->Scan(_pScannerConfiguration);

	return TRUE;
}



//
//    Save
//    ====
//
//    Write the current value(s) audited by this scanner to the AlertMonitor cache file which has been 
//    passed to us.
//
void CAlertTriggerScanner_SW::Save(CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	_pSoftwareScanner->Save(newCacheFile);
}


//
// Test to see whether trigger has "fired"
//
BOOL CAlertTriggerScanner_SW::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile)
{
	BOOL bResult = FALSE;
	CString strNextKey;

	TRACE("CAlertTriggerScanner_SW::Test, checking for applications installs and uninstalls\n");

	// Loop through the New list of applications and determine if there are any in the new list 
	// which were not in the old as these must have been installed
	CDynaList<CApplicationInstance>* pNewApplications = newDataFile.AuditedApplications();
	CDynaList<CApplicationInstance>* pOldApplications = oldDataFile.AuditedApplications();
	
	// If we have NO applications then we can assume that this has recently been added as an alert and we should
	// skip checking this time
	if (pOldApplications->GetCount() == 0)
		return FALSE;
		
	// Otherwise start checking the alerts
	for (int index = 0; index < (int)pNewApplications->GetCount(); index++)
	{
		CApplicationInstance* pNewInstance = &(*pNewApplications)[index];
		if (!FindApplication(pNewInstance->Name(), pOldApplications))
		{
			TRACE("CAlertTriggerScanner_SW::TestValue, Generating a Alert Notification for SW Installations\n");
			CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																		  , CAlertNotification::software
																		  , ""
																		  , pNewInstance->Name()
																		  , "Not Installed"
																		  , "Installed");					
			// Add this change to the notification file
			notificationFile.AlertNotifications().Add(pAlertNotification);
			bResult = TRUE;
		}
	}
	
	// Now we check the 'Old' applications list for items which are not in the new list meaning
	// that they have been uninstalled
	for (int index = 0; index < (int)pOldApplications->GetCount(); index++)
	{
		CApplicationInstance* pOldInstance = &(*pOldApplications)[index];
		if (!FindApplication(pOldInstance->Name(), pNewApplications))
		{
			TRACE("CAlertTriggerScanner_SW::TestValue, Generating a Alert Notification for SW Uninstallations\n");
			CAlertNotification* pAlertNotification = new CAlertNotification(_alertName
																		  , CAlertNotification::software
																		  , ""
																		  , pOldInstance->Name()
																		  , "Installed"
																		  , "Not Installed");					
			// Add this change to the notification file
			notificationFile.AlertNotifications().Add(pAlertNotification);
			bResult = TRUE;
		}
	}
		
	return bResult;
}


BOOL CAlertTriggerScanner_SW::FindApplication (CString& name ,CDynaList<CApplicationInstance>* pInstances)
{
	for (int index=0; index < (int)pInstances->GetCount(); index++)
	{
		CApplicationInstance* pInstance = &(*pInstances)[index];
		if (name == pInstance->Name())
			return TRUE;
	}
	return FALSE;
}




///////////////////////////////////////////////////////////////////////////////
// 
//    Internet Access scanning class

//
// Constructor
//
CAlertTriggerScanner_IE::CAlertTriggerScanner_IE(LPCSTR pszAlertName , LPCSTR pszTestVal ,CAuditScannerConfiguration* pScannerConfiguration)
	: CAlertTriggerScanner(pszAlertName, opChanged, pszTestVal)
{
	_pInternetExplorerScanner = new CInternetExplorerScanner();
	_pInternetExplorerScanner->ScanHistory(pScannerConfiguration->IEHistory());
	_pInternetExplorerScanner->ScanCookies(pScannerConfiguration->IECookies());
	_pInternetExplorerScanner->DetailedScan(FALSE);

	// We only want to recover IE history since the last audit
	_pInternetExplorerScanner->LimitDays(pScannerConfiguration->IEDays());	
	_pAlertNotificationHistory = NULL;
	_pAlertNotificationCookie = NULL;
}


//
// Destructor
//
CAlertTriggerScanner_IE::~CAlertTriggerScanner_IE ()
{
	if (_pInternetExplorerScanner != NULL)
		delete _pInternetExplorerScanner;
}


//
//   Collect results for this category
//
//   Note that as the Internet trigger is re-created each time around the main loop all knowledge of previous triggered Internet events 
//   is stored in the cache file.
//
//   We generate Internet alerts here and save into the Alert Member variables.  
//   Note that we only ever generate a SINGLE ALERT per day regardless of how many matches we actually have.  So if for example we have a site list
//   of 'ebay;microsoft' (that is 2 sites), we will only generate an alert notification for the first web site visited which matches EITHER
//   of the alertable sites.  We will then ignore all further alerts which are picked up today.  Although not perfect this ensures that the number of 
//   alerts generated is kept to a minimum and avoids the need to store details of these alerts in a cache which will have to be re-read each time the
//   scanner runs.
//
BOOL CAlertTriggerScanner_IE::Scan()
{
	_pInternetExplorerScanner->Scan(NULL);

	// The test value may be a semi-colon delimited list - try and split it into individual entries.
	CDynaList<CString> listSites;
	ListFromString(listSites ,_testValue ,';');

	// +8.3.7
	// We are only interested in history from today as yesterdays and earlier should already have been alerted 
	// We already handle multiple alerts in the same day so this allows us to handle the case when the scanner 
	// is restarted and potentially will read the same Internet history data and re-alert on sites which have 
	// already been alerted on.
	//
	// For IE we will do the test here also as we need to be able to save a record of any notifications generated
	// Pre-Process the Internet History and combine it into a smaller table
	CInternetHistoryList historyList;
	_pInternetExplorerScanner->PreProcessInternetHistory(historyList);
	CInternetExplorerCache* pInternetCookies = _pInternetExplorerScanner->GetInternetCookies();
	CTime now = CTime::GetCurrentTime();

	// Iterate through the individual mappings
	for (int index=0; ((index < (int)listSites.GetCount()) && (_pAlertNotificationHistory == NULL)); index++)
	{
		CString thisSite = listSites[index];

		// See if there are any matches in Internet History
		for (int histIndex=0; histIndex < (int)historyList.GetCount(); histIndex++)
		{
			// Get the Internet record
			CInternetHistoryEntry* pHistoryEntry = &historyList[histIndex];

			// We are checking this Internet event - get the URL and compare against the site
			CString url = pHistoryEntry->URL();
			if (url.Find(thisSite) != -1)
			{
				// +8.3.7
				// Only check this record if AFTER for today
				if ((pHistoryEntry->RawDate().GetDay() == now.GetDay())
				&&  (pHistoryEntry->RawDate().GetMonth() == now.GetMonth())
				&&  (pHistoryEntry->RawDate().GetYear() == now.GetYear()))
				{
					TRACE("CAlertTriggerScanner_IE::TestValue, Generating an Alert Notification for Internet Access\n");
					_pAlertNotificationHistory = new CAlertNotification(_alertName
																		 , CAlertNotification::internet
																		 , ""
																		 , "Internet History"
																		 , pHistoryEntry->URL()
																		 , _testValue);	
					break;
				}
			}
		}
	}

	// Repeat this operation for Cookies
	for (int index=0; ((index < (int)listSites.GetCount()) && (_pAlertNotificationCookie == NULL)); index++)
	{
		CString thisSite = listSites[index];

		// See if there are any matches in cookies
		for (int cookieIndex = 0; cookieIndex < (int)pInternetCookies->GetCount(); cookieIndex++)
		{
			// retrieve the item
			CInternetExplorerCacheItem const & cookie = (*pInternetCookies)[cookieIndex];
			if (cookie.m_strSourceURL.Find(thisSite) != -1)
			{
				// +8.3.7
				// Only check this record if AFTER for today
				if ((cookie.m_ctLastAccessed.GetDay() == now.GetDay())
				&&  (cookie.m_ctLastAccessed.GetMonth() == now.GetMonth())
				&&  (cookie.m_ctLastAccessed.GetYear() == now.GetYear()))
				{
					TRACE("CAlertTriggerScanner_IE::TestValue, Generating an Alert Notification for Internet Access\n");
					_pAlertNotificationCookie = new CAlertNotification(_alertName
																	  , CAlertNotification::internet
																	  , ""
																	  , "Internet Cookie"
																	  , cookie.m_strSourceURL
																	  , _testValue);
					break;
				}
			}
		}
	}

	return TRUE;
}





//
//    Save
//    ====
//
//    During the 'Save' phase we check to see if we have generated an alert this time around (in the Scan phase') 
//	  If we have generated a notification we must check for any previous notification being present in the old cache file.
//
//		Have we Generated a New Notification this Scan?
//			YES
//				Do we have a Previous Notification stored in the Cache File?
//				YES
//					Was the old notification generated Today?
//						YES 
//							Delete the new notification as we only alert once per day
//						NO 
//							Save the new notification into the New Cache file so that we know in future that we have created it.
//				NO
//					Save the new notification into the New Cache file so that we know in future that we have created it.
//			NO
//				Do we have a Previous Notification stored in the Cache File?
//				YES
//					Was the old notification generated Today?
//						YES 
//							Re-Save the old notification into the New Cache file so that we know in future that we have created it.
//
void CAlertTriggerScanner_IE::Save(CAuditDataFile& oldCacheFile, CAuditDataFile& newCacheFile)
{
	CString historyCacheEntryName;
	CString cookieCacheEntryName;
	CTime now = CTime::GetCurrentTime();
	
	// Get a list of the Internet Alerts held in the OLD cache file
	CDynaList<CAuditDataFileCategory>* pListInternetItems = oldCacheFile.InternetItems();
	historyCacheEntryName = "HISTORY|" + _alertName + "|" + _testValue;
	cookieCacheEntryName = "COOKIE|" + _alertName + "|" + _testValue;

	// Find an existing entry in the Old cache file for this history and cookie alert (if any)
	CAuditDataFileCategory* pHistoryCacheEntry = FindInternetCategory(pListInternetItems, historyCacheEntryName);
	CAuditDataFileCategory* pCookieCacheEntry = FindInternetCategory(pListInternetItems, cookieCacheEntryName);

	// Have we recorded an alert notification for any web site for which this alert has been triggered
	if (_pAlertNotificationHistory != NULL)
	{
		// Any previous cache entry found?
		if (pHistoryCacheEntry != NULL)
		{
			// Yes - we have previously generated an alert notification for this alert / web site
			// Check the time as alerts only last for 1 day - we get this from the first item
			CDynaList<CAuditDataFileItem>* pAuditDataFileItems = pHistoryCacheEntry->Items();
			CAuditDataFileItem* pItem = &(*pAuditDataFileItems)[0];
			CTime triggeredTime = ConvertTimeString(pItem->DisplayUnits());

			// Was the old notification generated today ?
			if (now.GetDay() == triggeredTime.GetDay())
			{
				// Yes - for today so not expired - clear the new alert
				delete _pAlertNotificationHistory;
				_pAlertNotificationHistory = NULL;
			}
			else
			{
				// No - Expired - we need to alert again 
				// To do this we keep the notification live but re-write an entry to the NEW CACHE FILE
				pHistoryCacheEntry = new CAuditDataFileCategory(historyCacheEntryName ,false);
				CString dateTimeString;
				dateTimeString = now.Format("%Y-%m-%d %H:%M:%S");
				CAuditDataFileItem historyItem(_pAlertNotificationHistory->OldValue(), "1", dateTimeString);
				pHistoryCacheEntry->Items()->Add(historyItem);
			}
		}

		else
		{
			// NO - This alert has not been previously notified - retain it and write details to the NEW CACHE FILE
			// Note that the triggered time is now
			pHistoryCacheEntry = new CAuditDataFileCategory(historyCacheEntryName ,false);
			CString dateTimeString;
			dateTimeString = now.Format("%Y-%m-%d %H:%M:%S");
			CAuditDataFileItem historyItem(_pAlertNotificationHistory->OldValue(), "1", dateTimeString);
			pHistoryCacheEntry->Items()->Add(historyItem);
		}
	}

	// If we have a record to store in the New cache then do so now.
	if (pHistoryCacheEntry != NULL)
		newCacheFile.AddInternetItem(*pHistoryCacheEntry);

	//
	// Repeat for COOKIES
	//
	if (_pAlertNotificationCookie != NULL)
	{
		// Any previous cache entry found?
		if (pCookieCacheEntry != NULL)
		{
			// Yes - we have previously generated an alert notification for this alert / web site
			// Check the time as alerts only last for 1 day - we get this from the first item
			CDynaList<CAuditDataFileItem>* pAuditDataFileItems = pCookieCacheEntry->Items();
			CAuditDataFileItem* pItem = &(*pAuditDataFileItems)[0];
			CTime triggeredTime = ConvertTimeString(pItem->DisplayUnits());

			// Was the old notification generated today ?
			if (now.GetDay() == triggeredTime.GetDay())
			{
				// Yes - for today so not expired - clear the new alert
				_pAlertNotificationCookie = NULL;
			}
			else
			{
				// No - Expired - we need to alert again 
				// To do this we keep the notification live but re-write an entry to the NEW CACHE FILE
				pCookieCacheEntry = new CAuditDataFileCategory(cookieCacheEntryName ,false);
				CString dateTimeString;
				dateTimeString = now.Format("%Y-%m-%d %H:%M:%S");
				CAuditDataFileItem cookieItem(_pAlertNotificationCookie->OldValue(), "1", dateTimeString);
				pCookieCacheEntry->Items()->Add(cookieItem);
			}
		}

		else
		{
			// NO - This alert has not been previously notified - retain it and write details to the NEW CACHE FILE
			// Note that the triggered time is now
			pCookieCacheEntry = new CAuditDataFileCategory(cookieCacheEntryName ,false);
			CString dateTimeString;
			dateTimeString = now.Format("%Y-%m-%d %H:%M:%S");
			CAuditDataFileItem cookieItem(_pAlertNotificationCookie->OldValue(), "1", dateTimeString);
			pCookieCacheEntry->Items()->Add(cookieItem);
		}
	}

	// If we have a record to store in the New cache then do so now.
	if (pCookieCacheEntry != NULL)
		newCacheFile.AddInternetItem(*pCookieCacheEntry);

}



//
//  Search the list of previously saved Internet triggers looking for a match to the trigger name specified
//
CAuditDataFileCategory* CAlertTriggerScanner_IE::FindInternetCategory(CDynaList<CAuditDataFileCategory>* pListCatagories, CString name) const
{
	for (int index = 0; index < pListCatagories->GetCount(); index++)
	{
		CAuditDataFileCategory* pInstance = &(*pListCatagories)[index];
		if (name == pInstance->Name())
			return pInstance;
	}

	return NULL;
}

CTime CAlertTriggerScanner_IE::ConvertTimeString(CString& strTime)
{
	LPCSTR p = (LPCSTR)strTime;
	int nYear	= atoi(p);
	int nMonth	= atoi(p + 5);
	int nDay	= atoi(p + 8);
	int nHour	= atoi(p + 11);
	int nMinute = atoi(p + 14);
	int nSecond = atoi(p + 17);
	return CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
}

CTime CAlertTriggerScanner_IE::ConvertTimeString2(CString& strTime)
{
	LPCSTR p = (LPCSTR)strTime;
	int nDay	= atoi(p);
	int nMonth	= atoi(p + 3);
	int nYear	= atoi(p + 6);
	return CTime(nYear,nMonth,nDay,0 ,0 ,0);
}


//
// Test to see whether trigger has "fired"
// We have already processed outstanding triggers - we are now looking at the cached results to see if
// the timeout has expired for any and therefore they need creating as notifications are written to file
//
BOOL CAlertTriggerScanner_IE::Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldCacheFile ,CAuditDataFile& newCacheFile)
{
	BOOL bResult = FALSE;

	TRACE("CAlertTriggerScanner_IE::Test, checking for access to the specified Internet sites\n");
	
	// Do we have a History Notification?
	if (_pAlertNotificationHistory != NULL)
	{
		notificationFile.AlertNotifications().Add(_pAlertNotificationHistory);
		bResult = true;
	}

	// Do we have a Cookie Notification?
	if (_pAlertNotificationCookie != NULL)
	{
		notificationFile.AlertNotifications().Add(_pAlertNotificationCookie);
		bResult = true;
	}

	// Return the overall status to signal whether or not we have generated a notification
	return bResult;
}
