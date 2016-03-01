#include "stdafx.h"
#include "AlertDefinitionsFile.h"
#include <map>

// Value-Defintions of the different String values
static enum StringValue { changed, lessthan, equals, greaterthan, greaterequals, lessequals };

// Map to associate the strings with the enum values
static std::map<std::string, StringValue> s_mapStringValues;
                          

//////////////////////////////////////////////////////////////////////////////////////////
//
//    CAlertDefinitionsFile
//    =====================
//
//    Constructor for the CAuditDataFile class
//
CAlertDefinitionsFile::CAlertDefinitionsFile(void)
{
	// Set any default values for the scanner configuration settings
	InitializeDefaults();
}

CAlertDefinitionsFile::~CAlertDefinitionsFile(void)
{
	// remember the contents of our encapsulated array are pointers - they need to be cleaned out
	for (int index=0; index < (int)_alertDefinitions.GetCount(); index++)
	{
		delete _alertDefinitions[index];
	}
	_alertDefinitions.Empty();
}


//
//    InitializeDefaults
//    ==================
//
//    Initialize the scanner configuration with default values
//
void CAlertDefinitionsFile::InitializeDefaults()
{
	_enabled = false;
	_showSystemTray = false;
	_settingsInterval = 30;
	_rescanInterval = 60;
}




//
//	Load
//  ====
//
//  Load the specified XML file into memory
//
//	Inputs:
//		pszPath - Fully qualified path to the XML file to be loaded
//
//  Returns:
//		-1 - File does not exist or failed to open file
//		0  - Success
//		1  - File exists but is in an invalid format
//
int CAlertDefinitionsFile::Load(LPCSTR pszPath)
{
	// Reset the configuration to factory defaults including releasing any memory allocated
	InitializeDefaults();

	// Now read the file
	CString csText;
	CFile file;
	if (!file.Open(pszPath, CFile::modeRead))
		return -1;
	int nFileLen = (int)file.GetLength();

	// Allocate buffer for binary file data
	unsigned char* pBuffer = new unsigned char[nFileLen + 2];

	// ...and read the file into this buffer
	nFileLen = file.Read( pBuffer, nFileLen );
	file.Close();

	// Terminate the buffer
	pBuffer[nFileLen] = '\0';
	pBuffer[nFileLen+1] = '\0'; // in case 2-byte encoded
	csText = (LPCSTR)pBuffer;
	delete [] pBuffer;

	// Load the XML buffer into an internal XML object
	CMarkup xmlFile;
	if (!xmlFile.SetDoc(csText))
		return -1;

	// Ok the file has now been read - let's process it
	xmlFile.ResetPos();

	// We must have the main alert monitor section otherwise this is an invalid file
	/*if (!xmlFile.FindElem(S_SETTINGS))
		return -1;*/

	// Process the file as we can be confident that it is an Alert Monitor Definitions File
	//ProcessElementRead(xmlFile);
	DeserializeScannerConfiguration(xmlFile);
	return 0;
}

void CAlertDefinitionsFile::DeserializeScannerConfiguration(CMarkup xmlFile)
{
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == "AlertMonitorDefinition")
		{
			ProcessAlertDefinition(xmlFile);
		}

	}
}



// 
//    ProcessElementRead
//    ==================
//
//    We have parsed the 'ScannerConfiguration' element so know that the XML file is an AuditWizard
//    Scanner Configuration file and can now continue to parse the items within this section noting 
//    that we terminate parsing when we reach the end of the section.
//
void CAlertDefinitionsFile::ProcessElementRead(CMarkup xmlFile)
{
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_ALERT_DEFINITIONS)
			ProcessAlertDefinitions(xmlFile);

		else if (elementName == V_SETTINGS_ENABLED)
			_enabled = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_SETTINGS_CHECKINTERVAL)
			_rescanInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == V_SETTINGS_RESCANINTERVAL)
			_settingsInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == V_SETTINGS_SHOWTRAY)
			_showSystemTray = xmlFile.GetChildDataAsBoolean();
	}
}




// 
//    ProcessAlertDefinitions
//    =======================
//
//    We have parsed the 'Alert Definitions' element
//
void CAlertDefinitionsFile::ProcessAlertDefinitions(CMarkup xmlFile)
{
	// Step into the 'Alert Definitions' section
	xmlFile.IntoElem();

	// Get the list of elements in this section - should be a list of <Alert Definition> 
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_ALERT_DEFINITION)
			ProcessAlertDefinition(xmlFile);
	}

	xmlFile.OutOfElem();
}


// 
//    ProcessAlertDefinition
//    ======================
//
//    We have parsed the 'Alert Definition' element
//
void CAlertDefinitionsFile::ProcessAlertDefinition(CMarkup xmlFile)
{
	// Step into the 'Alert Definition' section
	xmlFile.IntoElem();

	// Create a new Alert Definition
	CAlertDefinition* pAlertDefinition = new CAlertDefinition();

	// Get the list of elements in this section - should be a list of <Alert Definition> 
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_ALERT_NAME)
			pAlertDefinition->AlertName(xmlFile.GetChildData());

		else if (elementName == V_ALERT_DISPLAY)
			continue;
			
		else if (elementName == V_ALERT_EMAIL)
			continue;
			
		else if (elementName == S_ALERT_TRIGGERS)
			ProcessAlertTriggers(xmlFile ,pAlertDefinition);
	}	

	// Add this Alert Definition to our internal list
	_alertDefinitions.Add(pAlertDefinition);
	
	// back out of the element
	xmlFile.OutOfElem();
}






// 
//    ProcessAlertTriggers
//    ====================
//
//    We have parsed the 'Alert Triggers' element
//
void CAlertDefinitionsFile::ProcessAlertTriggers(CMarkup xmlFile ,CAlertDefinition* pAlertDefinition)
{
	// Step into the 'Alert Definition' section
	xmlFile.IntoElem();

	// Get the list of elements in this section - should be a list of <Alert Trigger> 
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_ALERT_TRIGGER)
			ProcessAlertTrigger(xmlFile ,pAlertDefinition);
	}
	xmlFile.OutOfElem();
}


//void CAlertDefinitionsFile::InitializeStringMap()
//{
//  s_mapStringValues["First Value"] = changed;
//  s_mapStringValues["Second Value"] = lessthan;
//  s_mapStringValues["Third Value"] = equals;
//  s_mapStringValues["Fourth Value"] = greaterthan;
//  s_mapStringValues["Fifth Value"] = greaterequals;
//  s_mapStringValues["Sixth Value"] = lessequals;
// 
//}

// 
//    ProcessAlertTrigger
//    ===================
//
//    We have parsed the 'Alert Trigger' element
//
void CAlertDefinitionsFile::ProcessAlertTrigger(CMarkup xmlFile ,CAlertDefinition* pAlertDefinition)
{
	//InitializeStringMap();
	// Step into the 'Alert Definition' section
	xmlFile.IntoElem();

	// Create a new Alert Trigger
	CAlertTrigger* pAlertTrigger = new CAlertTrigger();
	
	// Get the list of elements in this section - should be a list of <Alert Definition> 
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_TRIGGER_FIELD)
			pAlertTrigger->Field(xmlFile.GetChildData());

		else if (elementName == V_TRIGGER_CONDITION)
		{
			// User input
			static char szInput[_MAX_PATH];

			CString strCondition=xmlFile.GetChildData();
			/*strcpy(szInput, strCondition);
			switch(s_mapStringValues[szInput])
			{
			case changed:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)0);
					break;
				}
			case lessthan:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
					break;
				}
			case equals:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
					break;
				}
			case greaterthan:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
					break;
				}
			case greaterequals:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
					break;
				}
			case lessequals:
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
					break;
				}
			}*/

			if(strCondition == "changed")
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)0);
				}
			else if(strCondition == "lessthan")
				{
					pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)1);
				}			
			else if(strCondition == "equals")
			{
				pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)6);
			}
			else if(strCondition == "greaterthan")
			{
				pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)5);
			}
			else if(strCondition == "greaterequals")
			{
				pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)8);
			}
			else if(strCondition == "lessequals")
			{
				pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)7);
			}
			else
			{
				pAlertTrigger->Condition((CAlertTrigger::eTriggerCondition)xmlFile.GetChildDataAsInt());
			}
				
		}

		else if (elementName == V_TRIGGER_VALUE)
			pAlertTrigger->Value(xmlFile.GetChildData());
	}
	
	// Add the trigger to our internal list
	pAlertDefinition->AlertTriggers().Add(pAlertTrigger);

	// Out of the element
	xmlFile.OutOfElem();
}