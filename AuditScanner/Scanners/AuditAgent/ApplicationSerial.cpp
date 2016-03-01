/////////////////////////////////////////////////////////////////////////////////
//
//    ApplicationSerial.cpp
//    =====================
//
//    The ApplicationSerials class is used to create and maintain a list of application
//    serial number objects which in turn are created by scanning the registry looking for potential
//    serial numbers and also by reading the application registry mappings list from the scanner
//    configuration file.
//
#include "stdafx.h"     
#include "AuditScannerConfiguration.h"
#include "ApplicationSerial.h"   

// Static strings
static char APPLICATIONS_SECTION[]	= "ApplicationSerials";
static char DIGITALPRODUCTID[]		= "DigitalProductID";
static char DISPLAYNAME[]			= "DisplayName";
static char PIDKEY[]				= "PIDKEY";
static char* SERIALNUMBER_NAMES[]	= { "PRODUCTID", "REGISTRATION", "SERIAL", "REGISTEREDPID", "LICENSENUMBER", "PID", "LMKEY", "REGISTER NO.", "REGCODE", "KEYCODE", "CD-KEY" };
static int nSerialNumberNames		= sizeof(SERIALNUMBER_NAMES) / sizeof(char *);
static char UNINSTALLKEY[]			= "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
static char SOFTWAREKEY[]			= "Software";
static char OFFICEKEYNAME[]			= "Software\\Microsoft\\Office\\";

// Maximum search depth
static int _maxDepth = 5;
#define REG_BUFFER_SIZE	1024



/////////////////////////////////////////////////////////////////////////
//
//    CApplicationSerial
//    ==================
//
//    This class represents a single application serial number and its associated application
//
CApplicationSerial::CApplicationSerial(void)
{
	_applicationName = "";
	_identifier = "";
	_cdKey = "";
	_productId = "";
	_registryKey = "";
	_registryValue = "";
	_matched = FALSE;
}

CApplicationSerial::CApplicationSerial(CString& name, CString& identifier, CString& productID, CString& cdKey)
{
	_applicationName = name;
	_identifier = identifier;
	_productId = productID;
	_cdKey = cdKey;
	_registryKey = "";
	_registryValue = "";
	_matched = false;
}

CApplicationSerial::~CApplicationSerial(void)
{
}


///////////////////////////////////////////////////////////////////////////
//
//    CApplicationSerials
//    ===================
//
//    CApplicationSerials is a list of CApplicationserial objects.
//
CApplicationSerials::CApplicationSerials()
{
	// Initialize map for Office XP
	m_mapOfficeXP["11"] = "Microsoft Office XP Professional";
	m_mapOfficeXP["12"] = "Microsoft Office XP Standard";
	m_mapOfficeXP["13"] = "Microsoft Office XP Small Business";
	m_mapOfficeXP["14"] = "Microsoft Office XP Web Server";
	m_mapOfficeXP["15"] = "Microsoft Access 2002";
	m_mapOfficeXP["16"] = "Microsoft Excel 2002";
	m_mapOfficeXP["17"] = "Microsoft FrontPage 2002";
	m_mapOfficeXP["18"] = "Microsoft PowerPoint 2002";
	m_mapOfficeXP["19"] = "Microsoft Publisher 2002";
	m_mapOfficeXP["1A"] = "Microsoft Outlook 2002";
	m_mapOfficeXP["1B"] = "Microsoft Word 2002";
	m_mapOfficeXP["1C"] = "Microsoft Access 2002 Runtime";
	m_mapOfficeXP["1D"] = "Microsoft FrontPage Server Extensions 2002";
	m_mapOfficeXP["1E"] = "Microsoft Office Multilingual User Interface Pack";
	m_mapOfficeXP["1F"] = "Microsoft Office Proofing Tools Kit";
	m_mapOfficeXP["20"] = "System Files Update";
	m_mapOfficeXP["22"] = "unused";
	m_mapOfficeXP["23"] = "Microsoft Office Multilingual User Interface Pack Wizard";
	m_mapOfficeXP["24"] = "Microsoft Office XP Resource Kit";
	m_mapOfficeXP["25"] = "Microsoft Office XP Resource Kit Tools (download from Web)";
	m_mapOfficeXP["26"] = "Microsoft Office Web Component";
	m_mapOfficeXP["27"] = "Microsoft Project 2002";
	m_mapOfficeXP["28"] = "Microsoft Office XP Professional with FrontPage";
	m_mapOfficeXP["29"] = "Microsoft Office XP Professional Subscription";
	m_mapOfficeXP["2A"] = "Microsoft Office XP Small Business Edition Subscription";
	m_mapOfficeXP["2B"] = "Microsoft Publisher 2002 Deluxe Edition";
	m_mapOfficeXP["2F"] = "Standalone IME (JPN Only)";
	m_mapOfficeXP["30"] = "Microsoft Office XP Media Content";
	m_mapOfficeXP["31"] = "Microsoft Project 2002 Web Client";
	m_mapOfficeXP["32"] = "Microsoft Project 2002 Web Server";
	m_mapOfficeXP["33"] = "Microsoft Office XP PIPC1 (Pre Installed PC) (JPN Only)";
	m_mapOfficeXP["34"] = "Microsoft Office XP PIPC2 (Pre Installed PC) (JPN Only)";
	m_mapOfficeXP["35"] = "Microsoft Office XP Media Content Delux";
	m_mapOfficeXP["3A"] = "Project 2002 Standard";
	m_mapOfficeXP["3B"] = "Project 2002 Professional";
	m_mapOfficeXP["51"] = "Microsoft Office Visio Professional 2002";
	m_mapOfficeXP["54"] = "Microsoft Office Visio Standard 2002";

	// Office 2003 Mappings (Office 11)
	m_mapOffice2003[ "11" ] = "Microsoft Office Professional Enterprise Edition 2003";
	m_mapOffice2003[ "11" ] = "Microsoft Office Professional Enterprise Edition 2003";
	m_mapOffice2003[ "12" ] = "Microsoft Office Standard Edition 2003";
	m_mapOffice2003[ "13" ] = "Microsoft Office Basic Edition 2003";
	m_mapOffice2003[ "14" ] = "Microsoft Windows SharePoint Services 2.0";
	m_mapOffice2003[ "15" ] = "Microsoft Office Access 2003";
	m_mapOffice2003[ "16" ] = "Microsoft Office Excel 2003";
	m_mapOffice2003[ "17" ] = "Microsoft Office FrontPage 2003";
	m_mapOffice2003[ "18" ] = "Microsoft Office PowerPoint 2003";
	m_mapOffice2003[ "19" ] = "Microsoft Office Publisher 2003";
	m_mapOffice2003[ "1A" ] = "Microsoft Office Outlook Professional 2003";
	m_mapOffice2003[ "1B" ] = "Microsoft Office Word 2003";
	m_mapOffice2003[ "1C" ] = "Microsoft Office Access 2003 Runtime";
	m_mapOffice2003[ "1E" ] = "Microsoft Office 2003 User Interface Pack";
	m_mapOffice2003[ "1F" ] = "Microsoft Office 2003 Proofing Tools";
	m_mapOffice2003[ "23" ] = "Microsoft Office 2003 Multilingual User Interface Pack";
	m_mapOffice2003[ "24" ] = "Microsoft Office 2003 Resource Kit";
	m_mapOffice2003[ "26" ] = "Microsoft Office XP Web Components";
	m_mapOffice2003[ "2E" ] = "Microsoft Office 2003 Research Service SDK";
	m_mapOffice2003[ "44" ] = "Microsoft Office InfoPath 2003";
	m_mapOffice2003[ "83" ] = "Microsoft Office 2003 HTML Viewer";
	m_mapOffice2003[ "92" ] = "Windows SharePoint Services 2.0 English Template Pack";
	m_mapOffice2003[ "93" ] = "Microsoft Office 2003 English Web Parts and Components";
	m_mapOffice2003[ "A1" ] = "Microsoft Office OneNote 2003";
	m_mapOffice2003[ "A4" ] = "Microsoft Office 2003 Web Components";
	m_mapOffice2003[ "A5" ] = "Microsoft SharePoint Migration Tool 2003";
	m_mapOffice2003[ "AA" ] = "Microsoft Office PowerPoint 2003 Presentation Broadcast";
	m_mapOffice2003[ "AB" ] = "Microsoft Office PowerPoint 2003 Template Pack 1";
	m_mapOffice2003[ "AC" ] = "Microsoft Office PowerPoint 2003 Template Pack 2";
	m_mapOffice2003[ "AD" ] = "Microsoft Office PowerPoint 2003 Template Pack 3";
	m_mapOffice2003[ "AE" ] = "Microsoft Organization Chart 2.0";
	m_mapOffice2003[ "CA" ] = "Microsoft Office Small Business Edition 2003";
	m_mapOffice2003[ "D0" ] = "Microsoft Office Access 2003 Developer Extensions";
	m_mapOffice2003[ "DC" ] = "Microsoft Office 2003 Smart Document SDK";
	m_mapOffice2003[ "E0" ] = "Microsoft Office Outlook Standard 2003";
	m_mapOffice2003[ "E3" ] = "Microsoft Office Professional Edition 2003 (with InfoPath 2003)";
	m_mapOffice2003[ "FD" ] = "Microsoft Office Outlook 2003 (distributed by MSN)";
	m_mapOffice2003[ "FF" ] = "Microsoft Office 2003 Edition Language Interface Pack";
	m_mapOffice2003[ "F8" ] = "Remove Hidden Data Tool";
	m_mapOffice2003[ "3A" ] = "Microsoft Office Project Standard 2003";
	m_mapOffice2003[ "3B" ] = "Microsoft Office Project Professional 2003";
	m_mapOffice2003[ "32" ] = "Microsoft Office Project Server 2003";
	m_mapOffice2003[ "51" ] = "Microsoft Office Visio Professional 2003";
	m_mapOffice2003[ "52" ] = "Microsoft Office Visio Viewer 2003";
	m_mapOffice2003[ "53" ] = "Microsoft Office Visio Standard 2003";
	m_mapOffice2003[ "55" ] = "Microsoft Office Visio for Enterprise Architects 2003";
	m_mapOffice2003[ "5E" ] = "Microsoft Office Visio 2003 Multilingual User Interface Pack";

	// Office 2007 Mappings (Office 12)
	m_mapOffice2007[ "0011" ] = "Microsoft Office Professional Plus 2007";
	m_mapOffice2007[ "0012" ] = "Microsoft Office Standard 2007";
	m_mapOffice2007[ "0013" ] = "Microsoft Office Basic 2007";
	m_mapOffice2007[ "0014" ] = "Microsoft Office Professional 2007";
	m_mapOffice2007[ "0015" ] = "Microsoft Office Access 2007";
	m_mapOffice2007[ "0016" ] = "Microsoft Office Excel 2007";
	m_mapOffice2007[ "0017" ] = "Microsoft Office SharePoint Designer 2007";
	m_mapOffice2007[ "0018" ] = "Microsoft Office PowerPoint 2007";
	m_mapOffice2007[ "0019" ] = "Microsoft Office Publisher 2007";
	m_mapOffice2007[ "001A" ] = "Microsoft Office Outlook 2007";
	m_mapOffice2007[ "001B" ] = "Microsoft Office Word 2007";
	m_mapOffice2007[ "001C" ] = "Microsoft Office Access Runtime 2007";
	m_mapOffice2007[ "0020" ] = "Microsoft Office Compatibility Pack for Word, Excel, and PowerPoint 2007 File Formats";
	m_mapOffice2007[ "0026" ] = "Microsoft Expression We";
	m_mapOffice2007[ "0029" ] = "Microsoft Office Excel 2007";
	m_mapOffice2007[ "002B" ] = "Microsoft Office Word 2007";
	m_mapOffice2007[ "002E" ] = "Microsoft Office Ultimate 2007";
	m_mapOffice2007[ "002F" ] = "Microsoft Office Home and Student 2007";
	m_mapOffice2007[ "0030" ] = "Microsoft Office Enterprise 2007";
	m_mapOffice2007[ "0031" ] = "Microsoft Office Professional Hybrid 200";
	m_mapOffice2007[ "0033" ] = "Microsoft Office Personal 200";
	m_mapOffice2007[ "0035" ] = "Microsoft Office Professional Hybrid 200";
	m_mapOffice2007[ "0037" ] = "Microsoft Office PowerPoint 2007";
	m_mapOffice2007[ "003A" ] = "Microsoft Office Project Standard 2007";
	m_mapOffice2007[ "003B" ] = "Microsoft Office Project Professional 2007";
	m_mapOffice2007[ "0044" ] = "Microsoft Office InfoPath 2007";
	m_mapOffice2007[ "0051" ] = "Microsoft Office Visio Professional 2007";
	m_mapOffice2007[ "0052" ] = "Microsoft Office Visio Viewer 2007";
	m_mapOffice2007[ "0053" ] = "Microsoft Office Visio Standard 2007";
	m_mapOffice2007[ "00A1" ] = "Microsoft Office OneNote 2007";
	m_mapOffice2007[ "00A3" ] = "Microsoft Office OneNote Home Student 2007";
	m_mapOffice2007[ "00A7" ] = "Calendar Printing Assistant for Microsoft Office Outlook 2007";
	m_mapOffice2007[ "00A9" ] = "Microsoft Office InterConnect 2007";
	m_mapOffice2007[ "00AF" ] = "Microsoft Office PowerPoint Viewer 2007 (English)";
	m_mapOffice2007[ "00B0" ] = "The Microsoft Save as PDF add-in";
	m_mapOffice2007[ "00B1" ] = "The Microsoft Save as XPS add-in";
	m_mapOffice2007[ "00B2" ] = "The Microsoft Save as PDF or XPS add-in";
	m_mapOffice2007[ "00BA" ] = "Microsoft Office Groove 2007";
	m_mapOffice2007[ "00CA" ] = "Microsoft Office Small Business 2007";
	m_mapOffice2007[ "00E0" ] = "Microsoft Office Outlook 2007";
	m_mapOffice2007[ "10D7" ] = "Microsoft Office InfoPath Forms Services";
	m_mapOffice2007[ "110D" ] = "Microsoft Office SharePoint Server 2007";
	m_mapOffice2007[ "1122" ] = "Windows SharePoint Services Developer Resources 1.2";
}


//
//    Destructor - tidy up allocated memory
//
CApplicationSerials::~CApplicationSerials(void)
{
	m_mapOfficeXP.RemoveAll();
	m_mapOffice2003.RemoveAll();
	m_mapOffice2007.RemoveAll();

	// ...and the applications list
	_listApplicationSerials.Empty();
}


//
//    Detect
//    ======
//
//    This is the main detection rountine.  The purpose of this function is to scan the windows
//    registry searching for possible application product id's / serial numbers and their associated
//    CD Keys.  Where found these will be stored along with any possible matching application.
//
//    The process is as follows:
//		1> Recover product GUIDs and application names from the UNINSTALL key
//		2> Scan the HKEY_LOCAL_MACHINE\Software key looking for possible product IDs
//		3> Try and locate a matching GUID from step 1 which matches an application from step 2
//
int CApplicationSerials::Detect(CAuditScannerConfiguration* pAuditScannerConfiguration)
{
	// Pre-populate the dictionary of product GUIDs / serial numbers with the data recovered above
	FillGuidDictionary();

	// Scan the SOFTWARE branch of the registry looking for possible product id / serial numbers
	// These are where possible matched up with a product name by using any GUID associated with the
	// product ID and comparing it to the GUID list above to obtain an application name.  Only if we do
	// manage to tie together an Application Name with it's product id do we add it to our list
	HKEY hKey;

	// Open the Software key
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, SOFTWAREKEY, 0, KEY_READ, &hKey) == ERROR_SUCCESS) 
	{
		_currentDepth = 0;
		CString key = SOFTWAREKEY;
		FindAllProductSerials(hKey ,key);
		RegCloseKey(hKey);	
	}

	// Read the Publisher\Serial Number mappings from the Scanner Configuration File
	ReadSerials(pAuditScannerConfiguration);

	// Return
	return 0;
}


void CApplicationSerials::ReadSerials (CAuditScannerConfiguration* pAuditScannerConfiguration)
{
	// OK - that's the automatic scanning handled - now we handle those entries which have been pre-defined
	// for us from within the Scanner Configuration File
	CDynaList<CApplicationRegistryMapping>& listRegistryMappings = pAuditScannerConfiguration->ApplicationRegistryMappings();

	// Loop through the mappings read 
	for (DWORD dw=0; dw<listRegistryMappings.GetCount(); dw++)
	{
		CApplicationSerial* pFoundSerial = NULL;
		CApplicationRegistryMapping* pApplicationMapping = &listRegistryMappings[dw];
		CString applicationName = pApplicationMapping->ApplicationName();

		// Iterate through the registry key/values specified for this application
		for (DWORD dw=0; dw<pApplicationMapping->GetMappings().GetCount(); dw++)
		{
			CRegistryMapping* pRegistryMapping = &((pApplicationMapping->GetMappings())[dw]);  

			// Look in our list for an entry which specifies this registry key
			pFoundSerial = ContainsRegistryKey(pRegistryMapping->RegistryKey());
			if (pFoundSerial != NULL)
				break;
		}

		// Did we find ANY of the registry keys specified for the application in our existing list of 
		// registry key mappings?
		if (pFoundSerial != NULL)
		{
			// Yes - set the application name in the work ApplicationSerial object then
			pFoundSerial->ApplicationName(applicationName);
		}
		else
		{
			// No we have not found any existing instance of any of the registry keys which were specified 
			// for the application - we add an CApplicationSerial object into our list for each key specified
			for (DWORD dw=0; dw<pApplicationMapping->GetMappings().GetCount(); dw++)
			{
				CRegistryMapping* pRegistryMapping = &((pApplicationMapping->GetMappings())[dw]);  
				CApplicationSerial applicationSerial;
				applicationSerial.Matched(TRUE);
				applicationSerial.ApplicationName(applicationName);

				// Pick up the registry key name and value name
				CString keyName = pRegistryMapping->RegistryKey();
				CString valueName = pRegistryMapping->ValueName();
				CString productID = "";
				CString digitalProductID = "";

				// ...and try and read the specified key
				productID = CReg::GetItemString(HKEY_LOCAL_MACHINE, keyName, valueName);

				// ...then try and get any associated digital product iD
				digitalProductID = GetDigitalProductId(keyName);

				// If we have found either the product key or CD key then add this to our list
				if (productID != "" || digitalProductID != "")
				{
					applicationSerial.ProductId(productID);
					applicationSerial.CdKey(digitalProductID);

					// Add this object to our list
					_listApplicationSerials.Add(applicationSerial);
				}
			}
		}
	}
}




//
//    GetDigitalProductId
//    ===================
//
//    Check for a Digital product ID and if found beneath the specified key, attempt to decode it
//    and return the resultant CD Key.
//
CString	CApplicationSerials::GetDigitalProductId(CString& keyName)
{
	CByteArray arrayDigitalProductID;
	CString strSerial = "";
	
	// Using the passed in 
	if (CReg::GetItemBinary (HKEY_LOCAL_MACHINE, keyName, DIGITALPRODUCTID ,arrayDigitalProductID))
		strSerial = DecodeDigitalProductKey(arrayDigitalProductID);

	// Return it
	return strSerial;
}




//
//    ContainsApplication
//    ===================
//
//    This function is used to determine whether or not an entry in the list
//    already exists for the specified application.  If so a pointer to the entry
//    is returned otherwise we return a null pointer.
//
CApplicationSerial* CApplicationSerials::ContainsApplication(CString& application)
{
	for (DWORD dw = 0; dw<_listApplicationSerials.GetCount(); dw++)
	{
		CApplicationSerial* pApplicationSerial = &_listApplicationSerials[dw];
		if (application == pApplicationSerial->ApplicationName())
			return pApplicationSerial;
	}
	return NULL;
}


//
//    ContainsGUID
//    ============
//
//    This function is used to determine whether or not an entry in the list
//    already exists with the specified identifier (GUID).  If so a pointer to the entry
//    is returned otherwise we return a null pointer.
//
CApplicationSerial* CApplicationSerials::ContainsIdentifier(CString& identifier)
{
	for (DWORD dw = 0; dw<_listApplicationSerials.GetCount(); dw++)
	{
		CApplicationSerial* pApplicationSerial = &_listApplicationSerials[dw];
		if (identifier == pApplicationSerial->Identifier())
			return pApplicationSerial;
	}
	return NULL;
}


//   
//    ContainsRegistryKey
//    ===================
//
// Do we have an entry for the specified registry key if so return it
//
CApplicationSerial* CApplicationSerials::ContainsRegistryKey(CString& registryKey)
{
	// Remove the hive if specified in the registry key
	registryKey.Replace("HKEY_LOCAL_MACHINE\\", "");
	for (DWORD dw = 0; dw<_listApplicationSerials.GetCount(); dw++)
	{
		CApplicationSerial* pApplicationSerial = &_listApplicationSerials[dw];
		if (registryKey == pApplicationSerial->RegistryKey())
			return pApplicationSerial;
	}
	return NULL;
}


// Decode a Digital Product ID to its native format



//
//    FillGuidDictionary
//    ==================
//
//    This function iterates through the main windows uninstall key and recovers the product GUID along
//    with product display names so that we can later match these up with serial numbers detected
//
void CApplicationSerials::FillGuidDictionary()
{
	// Connect to the HKEY_LOCAL_MACHINE hive in the registry and open the uninstall key
	HKEY hKey;

	// Open the main Windows uninstall key
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, UNINSTALLKEY, 0, KEY_READ, &hKey) == ERROR_SUCCESS) 
	{
		char szKeyName[REG_BUFFER_SIZE];
		DWORD dwLength = REG_BUFFER_SIZE;
		DWORD dwIndex = 0;

		// Loop around each of the sub-keys looking for GUID entries
		while (ERROR_NO_MORE_ITEMS != RegEnumKeyEx (hKey,  dwIndex, szKeyName, &dwLength, NULL, NULL, NULL, NULL)) 
		{
			// prepare to get next key
			dwIndex++;
			dwLength = REG_BUFFER_SIZE;
			CString subKeyName = szKeyName;
			CString productGUID = "";
			CString applicationName;
		
			// Check for a GUID - assume will start with an open brace and end with a closing brace
			if (subKeyName[0] == '{' && subKeyName[subKeyName.GetLength() - 1] == '}')
			{
				productGUID = subKeyName.Mid(1, subKeyName.GetLength() - 2);

				// ...recover the display name for this GUID for further testing
				applicationName = CReg::GetItemString(hKey, subKeyName, DISPLAYNAME);
				
				// ...and ensure that we want to keep it
				applicationName.Trim();
				applicationName.Remove('[');
				applicationName.Remove(']');
				applicationName.Remove('=');
			
				// Special case handler for Adobe Products...
				if (applicationName.Left(19) == "Add or Remove Adobe")
					applicationName.Replace("Add or Remove", "");

				// If we have an application name then add this as a mapping
				if (applicationName != "")
					_guidMappings.Add(productGUID, applicationName);
			}
		}
			
		// Close the main key again
		RegCloseKey(hKey);
	}
}



//
//    FindAllProductSerials
//    =====================
//
//    Recursively search down the Software registry key looking for any possible product serial numbers
//    and storing them for later inspection
//
void CApplicationSerials::FindAllProductSerials(HKEY hKey ,CString& fullKeyName)
{
	// Skip Software\Classes and Software\Microsoft\Windows as these are huge keys that do not contain anything of interest
	if (fullKeyName == "Software\\Classes" || fullKeyName == "Software\\Microsoft\\Windows")
	{
		_currentDepth--;
	}
	else
	{
		// Iterate through the values immediately beneath the current key
		DWORD dwIndex = 0;
		char szValueName[255];
		DWORD dwValueNameLen = 255;

		while (ERROR_SUCCESS == RegEnumValue(hKey, dwIndex++, szValueName, &dwValueNameLen, NULL, NULL, NULL, NULL))
		{
			CString valueName(szValueName);
			if (IsSerialNumber(valueName))
			{
				CString applicationName = "";
				CString digitalProductID = GetDigitalProductId(fullKeyName);
				CString productGUID = fullKeyName;

				// From the registry key name strip out the last segment and determine if it is a GUID
				int lastSegmentStart = productGUID.ReverseFind('\\');
				productGUID = productGUID.Mid(lastSegmentStart + 1);
				if (productGUID[0] == '{' && productGUID[productGUID.GetLength() - 1] == '}')
				{
					// If the value was a product GUID then strip off the braces and check to see if we
					// already have this GUID in our existing list
					productGUID = productGUID.Mid(1, productGUID.GetLength() - 2);
					if (_guidMappings.Exists(productGUID))
					{
						applicationName = _guidMappings[productGUID];
					}
					else
					{						
						// We can do some special processing for Microsoft Office Products as the GUID helps to identify
						// the actual product so let's try processing Office products now
						applicationName  = FindOfficeProductName(fullKeyName ,productGUID);		
					}
				}
				else
				{
					productGUID = "";
				}

				// Add the new Application Serial
				CApplicationSerial newSerial;
				newSerial.ApplicationName(applicationName);
				newSerial.Identifier(productGUID);
				newSerial.ProductId(CReg::GetItemString(hKey, "", valueName));
				newSerial.CdKey(digitalProductID);
				newSerial.RegistryKey(fullKeyName);
				newSerial.RegistryValue(valueName);
				_listApplicationSerials.Add(newSerial);
			}
		}

		// We are stepping down a level so increment our depth
		_currentDepth++;

		// If we have not reached the maximum depth then call ourselves recursively
		if (_currentDepth < _maxDepth)
		{
			// Iterate through sub-keys
			char szKeyName[REG_BUFFER_SIZE];
			DWORD dwLength = REG_BUFFER_SIZE;
			DWORD dwIndex = 0;

			// Loop around each of the sub-keys looking for GUID entries
			while (ERROR_NO_MORE_ITEMS != RegEnumKeyEx (hKey,  dwIndex, szKeyName, &dwLength, NULL, NULL, NULL, NULL)) 
			{
				// prepare to get next key
				dwIndex++;
				dwLength = REG_BUFFER_SIZE;
				
				// Construct the full key name
				CString subKeyName = fullKeyName + "\\" + szKeyName;
				TRACE("Scanning key : %s\n" ,subKeyName);

				// Open the sub-key
				HKEY hSubKey;
				if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, subKeyName, 0, KEY_READ, &hSubKey) == ERROR_SUCCESS) 
				{
					FindAllProductSerials(hSubKey ,subKeyName);
					RegCloseKey(hSubKey);
				}
			}
		}
		_currentDepth--;
	}
}


// Determine if the specified value could possibly equate to a serial number 
BOOL CApplicationSerials::IsSerialNumber(CString& valueName)
{
	CString upcName = valueName.MakeUpper();

	for (int isub=0; isub<nSerialNumberNames; isub++)
	{
		CString name = SERIALNUMBER_NAMES[isub];
		if (upcName.Left(name.GetLength()) == name)
			return true;
	}
	return false;
}


// 
//    Dump
//    ----
//
//    Diagnostic function to dump out the copntents of this list to a CSV file
//
void CApplicationSerials::Dump()
{
	CStdioFile outputFile;
	CString strOutputLine;
	if (!outputFile.Open("C:\\temp\\ApplicationSerialsDump.csv" ,CFile::modeCreate|CFile::modeWrite))
		return;
	strOutputLine = "Application Name,CD Key,GUID,Product ID,registry key,registry value\n";
	outputFile.WriteString(strOutputLine);

	for (DWORD dw = 0; dw<_listApplicationSerials.GetCount(); dw++)
	{
		CApplicationSerial* pApplicationSerial = &_listApplicationSerials[dw];

		strOutputLine.Format("'%s',%s,%s,%s,%s,%s\n"
			,pApplicationSerial->ApplicationName()
			,pApplicationSerial->CdKey()
			,pApplicationSerial->Identifier()
			,pApplicationSerial->Identifier()
			,pApplicationSerial->ProductId()
			,pApplicationSerial->RegistryKey()
			,pApplicationSerial->RegistryValue());
		outputFile.WriteString(strOutputLine);
	}
	outputFile.Close();
}


//
//    FindOfficeProductName
//    =====================
//
//    This function is called as we try and identify registry keys in which product IDs and CD Keys may
//    be stored.  The purpose of this function is to check the registry key being scanned (and previously 
//    identified as containing a ProductID and GUID) to see if these are for Microsoft Office products
//
//    If we do find that this is a Microsoft Office registry key then we can see whether or not we can map the
//    GUID to it's product name using Microsoft supplied tables.
//
CString CApplicationSerials::FindOfficeProductName (CString& strKeyName ,CString& productGUID)
{
	CString strApplicationName = "";

	// Does the key indicate a Microsoft Office product?  If not return
	int delimiter = strKeyName.Find(OFFICEKEYNAME);
	if (delimiter == -1) 
		return strApplicationName;

	// OK - this is a Microsoft Office Product - find the version as we can only deal with 10.0 (XP), 11.0 (2003) 
	// and 12.0 (2007) - step past the delimiter and pickup the next 4 characters
	delimiter += sizeof(OFFICEKEYNAME);
	CString strVersion = strKeyName.Mid(delimiter - 1 ,4);

	// ...and check the version
	if (strVersion == "10.0")
	{
		CString key = productGUID.Mid(2 ,2);
		m_mapOfficeXP.Lookup(key ,strApplicationName);
	}

	else if (strVersion == "11.0")
	{
		CString key = productGUID.Mid(2 ,2);
		m_mapOffice2003.Lookup(key ,strApplicationName);
	}

	else if (strVersion == "12.0")
	{
		CString key = productGUID.Mid(9 ,4);
		m_mapOfficeXP.Lookup(key ,strApplicationName);
	}

	// ...and return (any) application name found
	return strApplicationName;
}
