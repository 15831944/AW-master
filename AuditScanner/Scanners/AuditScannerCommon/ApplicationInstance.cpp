#include "stdafx.h"
#include "AuditScannerConfiguration.h"
#include "ApplicationSerial.h"
#include "ApplicationInstance.h"

// Static strings used when scanning the registry
static char CLASSES_INSTALLERKEY[] = "SOFTWARE\\Classes\\Installer\\Products";
static char DISPLAYNAME[] = "DisplayName";
static char DISPLAYVERSION[] = "DisplayVersion";
static char INSTALLPROPERTIES[] = "InstallProperties";
static char MICROSOFT[] = "Microsoft";
static char PARENTKEYNAME[] = "ParentKeyName";
static char PARENTKEYNAME_OS[] = "OperatingSystem";
static char PRODUCTID[] = "ProductID";
static char DIGITALPRODUCTID[]		= "DigitalProductID";
static char PRODUCTNAME[] = "ProductName";
static char PUBLISHER[] = "Publisher";
static char PUBLISHERALIASES_SECTION[] = "PublisherAliases";
static char RELEASETYPE[] = "ReleaseType";
static char RELEASETYPE_SP[] = "Service Pack";
static char SERIALNUMBER[] = "SerialNumber";
static char PRODUCTGUID[] = "ProductGuid";
static char SYSTEMCOMPONENT[] = "SystemComponent";
static char UNINSTALLKEY[] = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
static char WINDOWS_INSTALLERKEY[] = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products";
static char IE_INSTALLERKEY[] = "SOFTWARE\\Microsoft\\Internet Explorer";
//
static char REMOTE_REGISTRY_SERVICE[] = "RemoteRegistry";
//
#define REG_BUFFER_SIZE 1024

/////////////////////////////////////////////////////////////////////////////////
//
//    CApplicationInstance 
//    ====================
//
//    This class represents an instance of an installed application
//
CApplicationInstance::CApplicationInstance(void)
{
	_name = "";
	_version = "";
	_publisher = "";
	_guid = "";
}


/////////////////////////////////////////////////////////////////////////////////
//
//    CApplicationInstanceList 
//    ========================
//
//    This class represents an list of installed applications
//

void CApplicationInstanceList::AddApplicationInstance(CString name, CString publisher, CString guid, CString serialNumber, CString version)
{
	CApplicationSerial newSerial;

	// Quick frig, remove any '?' from the name as this is not supported in the database
	name.Remove('™');

	// Check to see if this application already exists in our list
	CApplicationInstance* pExistingApplication = ContainsApplication(name);

	// If the application doesn't exist and we have a GUID then try and find a match for the application
	// based on the GUID
	if ((pExistingApplication == NULL) && (guid != ""))
		pExistingApplication = ContainsGuid(guid);

	// If we have found an existing match then update any missing fields
	if (pExistingApplication != NULL)
	{
		if (pExistingApplication->Publisher() == "")
			pExistingApplication->Publisher(publisher);

		if (pExistingApplication->Guid() == "")
			pExistingApplication->Guid(guid);

		if ((pExistingApplication->Serial().ApplicationName() == ""))
		{
			newSerial.ApplicationName(name);
			newSerial.Identifier(guid);
			newSerial.ProductId(serialNumber);
			pExistingApplication->Serial(newSerial);
		}
		
		if (pExistingApplication->Version() == "")
			pExistingApplication->Version(version);
	}
	
	else
	{
		// No Existing match so create a new instance and add to our list
		CApplicationInstance newApplication;		

		newApplication.Name(name);
		newApplication.Publisher(publisher);
		newApplication.Version(version);
		newApplication.Guid(guid);
		newApplication.Source("Windows Installer Key");

		newSerial.ApplicationName(name);
		newSerial.Identifier(guid);
		newSerial.ProductId(serialNumber);
		newApplication.Serial(newSerial);		

		// ...add to our list
		_listApplicationInstances.Add(newApplication);
	}
}


//
//    ContainsApplication
//    ===================
//
//    This function determines if the specified application already exists in our internal
//	  list and if it does returns a reference to it
CApplicationInstance* CApplicationInstanceList::ContainsApplication(CString& value)
{
	for (DWORD dw=0; dw<_listApplicationInstances.GetCount(); dw++)
	{
		CApplicationInstance* pThisApplication = &_listApplicationInstances[dw];
		if (pThisApplication->Name() == value)
			return pThisApplication;
	}
	return NULL;
}


//
//    ContainsGuid
//    ============
//
//    Search our list of application instances and return the first match where the guid matches that
//    supplied.  Returns a NULL pointer if no match
//
CApplicationInstance* CApplicationInstanceList::ContainsGuid(CString& value)
{
	for (DWORD dw=0; dw<_listApplicationInstances.GetCount(); dw++)
	{
		CApplicationInstance* pThisApplication = &_listApplicationInstances[dw];
		if (pThisApplication->Guid() == value)
			return pThisApplication;
	}
	return NULL;
}


//
//    Detect
//    ======
//
//    This is the main detection function.  
//    The purpose of this function is 
//
//		1> Call the Detect function of the CApplicationSerials object to recover possible product IDs
//		2> Scan various Windows registry keys looking for installed applications
//		3> Match the entries recovered in (2) with any serial numbers recovered by (1) and store them
//
int CApplicationInstanceList::Detect(CAuditScannerConfiguration* pAuditScannerConfiguration)
{
	try
	{
		CLogFile log;

		log.Write("CApplicationInstanceList::Detect in");

		// Ensure that the list is empty
		_listApplicationInstances.Empty();

		// Save the pointer to the scanner configuration file object as we may need this later
		_pAuditScannerConfiguration = pAuditScannerConfiguration;

		// Call the application serials detection function as this will perform a preliminary scan of the
		// windows registry looking for potential serial number / product id fields
		_applicationSerials.Detect(pAuditScannerConfiguration);
		//_applicationSerials.Dump();																// 8.3.4 - CMD - Commented out as a diagnostic only

		// Open the Softare uninstall key under HKLM and scan it
		HKEY hKey; 
		LONG result;
		result = RegOpenKeyEx (HKEY_LOCAL_MACHINE, UNINSTALLKEY, 0, KEY_READ, &hKey);
		if (ERROR_SUCCESS == result) 
		{
			ScanUninstallKey (hKey);
			RegCloseKey (hKey);
		}
		else
		{
			log.Format("Failed to open HKLM %s [1], Reason : %d", UNINSTALLKEY, result);
		}

		result = RegOpenKeyEx (HKEY_LOCAL_MACHINE, UNINSTALLKEY, 0, KEY_READ | KEY_WOW64_64KEY, &hKey);
		if (ERROR_SUCCESS == result)
		{
			ScanUninstallKey (hKey);
			RegCloseKey (hKey);
		}
		else
		{
			log.Format("Failed to open HKLM %s [2], Reason : %d", UNINSTALLKEY, result);
		}

		// repeat with the USER hive...
		result = RegOpenKeyEx (HKEY_CURRENT_USER, UNINSTALLKEY, 0, KEY_READ, &hKey);
		if (ERROR_SUCCESS == result) 
		{
			ScanUninstallKey (hKey);
			RegCloseKey (hKey);
		}
		else
		{
			log.Format("Failed to open HKCU %s, Reason : %d", UNINSTALLKEY, result);
		}

		// Scan Windows\Installer key
		result = RegOpenKeyEx (HKEY_LOCAL_MACHINE, WINDOWS_INSTALLERKEY, 0, KEY_READ, &hKey);
		if (ERROR_SUCCESS == result) 
		{
			ScanWindowsInstaller (hKey);
			RegCloseKey (hKey);
		}
		else
		{
			log.Format("Failed to open HKLM %s [2], Reason : %d", WINDOWS_INSTALLERKEY, result);
		}

		// Odds and ends
		ScanExceptions();

		//log.Write("processing each application");
		// Iterate through the applications detected by the above and attempt to recover any serial number
		for (DWORD dw=0; dw<_listApplicationInstances.GetCount(); dw++)
		{
			CApplicationInstance* pApplicationInstance = &_listApplicationInstances[dw];			
			CApplicationSerial* pThisSerial = NULL;

			// no need to attempt this for Internet Explorer
			if (pApplicationInstance->Name() == "Internet Explorer")
				continue;

			// If the installed application specified a GUID then scan the list of serial numbers looking 
			// for an entry with the same GUID.  If we find one then we can recover the serial number from there
			if (pApplicationInstance->Guid() != "")
			{
				pThisSerial = _applicationSerials.ContainsIdentifier(pApplicationInstance->Guid());
			}

			// If we have still not found a serial number and we have recovered the application name then try
			// scanning the serial numbers list for the named application and recover any serial number found
			if ((pThisSerial == NULL) && (pApplicationInstance->Name() != ""))
				pThisSerial = _applicationSerials.ContainsApplication(pApplicationInstance->Name());

			// If we have now identified a serial number then copy the details into our application instance
			if (pThisSerial != NULL)
			{
				pThisSerial->Matched(TRUE);
				pApplicationInstance->Serial(*pThisSerial);
			}
		}
	}
	catch (CException *pEx)
	{
		throw pEx;;
	}

	return 0;
}


//
//    GetApplicationName
//    ==================
//
//    Recover the application name from the specified registry key location and pre-parse it to 
//    remove invalid characters
//
CString	CApplicationInstanceList::GetApplicationName (HKEY hKey ,CString& subKeyName ,CString& valueName)
{
	CString applicationName = CReg::GetItemString(hKey, subKeyName, valueName);
	if (applicationName != "")
	{
		// Trim and remove invalid characters
		applicationName.Trim();
		applicationName.Replace("[" ,"");
		applicationName.Replace("]" ,"");
		applicationName.Replace("=" ,"");

		// Special case for Adobe products which can have leading text which will get in the way of
		// the actual product name
		if (applicationName.Left(19) == "Add or Remove Adobe")
			applicationName = applicationName.Mid(19);
	}

	return applicationName;
}


//
//    IsInstalledApplication
//    ======================
//
//    Determine if the installed application entry at the current registry location is a valid entry, that 
//    is obeys the rules which determine whether or not it is actually an application.
//
//	  The rules applied are as follows:
//		1> Must have a 'DisplayName'
//		2> Must NOT be flagged as a 'SystemComponent'
//		3> The display name must not contain the text 'Hotfix', 'Security Update' or 'Update for'
//		4> The 'Release Type' must NOT be a service pack
//		5> It's 'ParentKey' attribute must not indicate an Operating System
//		
BOOL	CApplicationInstanceList::IsInstalledApplication(HKEY hKey ,CString& subKeyName)
{
	CString displayName = GetApplicationName(hKey, subKeyName, (CString)DISPLAYNAME);

	if (displayName == "")
		return FALSE;

	// Discard any hotfix of security update items
	if ((displayName.Find("Hotfix") != -1)
	 || (displayName.Find("Security Update") != -1) 
	 || (displayName.Find("Update for") != -1)
	 || (displayName.Find(" MUI ") != -1)
	 || (displayName.Find("Microsoft Office Proof") != -1)
	 || (displayName.Find("Microsoft Office Proofing") != -1))
		return FALSE;

	// If we have a 'systemcomponent' value then recover it
	// One exception to this is Microsoft Exchange Server which we list even though it is a system component
	/*DWORD systemComponent = CReg::GetItemInt(hKey ,subKeyName ,SYSTEMCOMPONENT);
	if ((displayName != "Microsoft Exchange Server") && (systemComponent == 1))
		return FALSE;*/

	// Check for any Release Type and ensure that it is not a service pack
	if (CReg::GetItemString(hKey ,subKeyName ,RELEASETYPE) == RELEASETYPE_SP)
		return FALSE;

	// Check that the Parent key is not an OS entry
	if (CReg::GetItemString(hKey ,subKeyName ,PARENTKEYNAME) == PARENTKEYNAME_OS)
		return FALSE;

	// All valid
	return TRUE;
}


//
//    RationalizePublisher
//    ====================
//
//    Rationalize the publisher names given the mappings held in the scanner configuration file
//
CString CApplicationInstanceList::RationalizePublisher(CString& thePublisher)
{
	CString rationalizedName = _pAuditScannerConfiguration->RationalizePublisherName(thePublisher);
	return rationalizedName;
}



//
//    ScanUninstallKey
//    ================
//
//    Scan the UNINSTALL registry key beneath the supplied registry hive lookimg for applications that 
//    have been installed.
//
void CApplicationInstanceList::ScanUninstallKey(HKEY hKey)
{
	char szBuffer[REG_BUFFER_SIZE];
	DWORD dwLength = REG_BUFFER_SIZE;
	DWORD dwIndex = 0;

	// Enumerate the sub-keys
	while (ERROR_NO_MORE_ITEMS != RegEnumKeyEx (hKey,  dwIndex, szBuffer, &dwLength, NULL, NULL, NULL, NULL)) 
	{
		// prepare to get next key
		dwIndex++;
		dwLength = REG_BUFFER_SIZE;

		// Is this an installed application?
		CString subKeyName = szBuffer;

		if (IsInstalledApplication(hKey ,subKeyName))
		{
			CString displayName = GetApplicationName(hKey ,subKeyName, (CString)DISPLAYNAME);

			CString productGUID = "";			

			// Is the sub-key name a GUID - if so recover the GUID as this can be used to find a serial number
			if (subKeyName[0] == '{' && subKeyName[subKeyName.GetLength() - 1] == '}')
				productGUID = subKeyName.Mid(1, subKeyName.GetLength() - 2);			

			// Recover other attributes for the application
			CString version = CReg::GetItemString(hKey ,subKeyName ,DISPLAYVERSION);
			CString publisher = CReg::GetItemString(hKey ,subKeyName ,PUBLISHER);
			publisher = RationalizePublisher(publisher);

			// Recover (any) product ID / Serial Number specified for the application
			CString productID = CReg::GetItemString(hKey ,subKeyName ,PRODUCTID);

			if (productID.IsEmpty())
				productID = CReg::GetItemString(hKey ,subKeyName ,SERIALNUMBER);			

			// Add this instance to our internal list
			AddApplicationInstance(displayName, publisher, productGUID, productID, version);
		}
	}
}



//
//    ScanWindowsInstaller
//    ====================
//
//    Scan the Windows installer registry key for installed applications not detected previously
//
void CApplicationInstanceList::ScanWindowsInstaller(HKEY rootKey)
{
	char szBuffer[REG_BUFFER_SIZE];
	DWORD dwLength = REG_BUFFER_SIZE;
	DWORD dwIndex = 0;

	// Enumerate the sub-keys beneath the Windows installer
	while (ERROR_NO_MORE_ITEMS != RegEnumKeyEx (rootKey,  dwIndex, szBuffer, &dwLength, NULL, NULL, NULL, NULL)) 
	{
		// prepare to get next key
		dwIndex++;
		dwLength = REG_BUFFER_SIZE;
		CString subKeyName = szBuffer;

		CLogFile log;

		// Open the Install Features sub-key
		CString productKey;
		productKey.Format("%s\\%s" ,WINDOWS_INSTALLERKEY ,subKeyName);
		HKEY hProductKey;

		if (RegOpenKeyEx (rootKey, subKeyName, 0, KEY_READ, &hProductKey) == ERROR_SUCCESS) 
		{
			// Is this a valid installed application?
			if (IsInstalledApplication(rootKey ,subKeyName))
			{
				CString applicationName = GetApplicationName(rootKey ,subKeyName, (CString)DISPLAYNAME);

				CString version = CReg::GetItemString(rootKey ,subKeyName ,DISPLAYVERSION);
				CString publisher = CReg::GetItemString(rootKey ,subKeyName ,PUBLISHER);
				publisher = RationalizePublisher(publisher);

				// Recover (any) product ID / Serial Number specified for the application
				CString productID = CReg::GetItemString(rootKey ,subKeyName ,PRODUCTID);
				if (productID.IsEmpty())
					productID = CReg::GetItemString(rootKey ,subKeyName ,SERIALNUMBER);

				// Format the product GUID from the sub-key name
				CString productGUID = subKeyName.Mid(0, 8) + "-" + subKeyName.Mid(8, 4) + "-" + subKeyName.Mid(12, 4) + "-" + subKeyName.Mid(0x10, 4) + "-" + subKeyName.Mid(20, 8);

				// Add this instance to our internal list
				AddApplicationInstance(applicationName, publisher, productGUID, productID, version);
			}
			RegCloseKey(hProductKey);
		}
	}
}




//
//    ScanExceptions
//    ==============
//
//    Scan for applications which have been proven to not be picked up by any 'standard' methods
//
//	  Currently this includes:
//		Sophos SweepNT
//		Novell Client
//
void CApplicationInstanceList::ScanExceptions ()
{
	CString applicationName;
	CString publisher;
	CString productID;
	CString productGUID("");
	CString version("");

	HKEY hKey;
	
	CString strExceptionKey = "SOFTWARE\\Sophos\\SweepNT";

	// See if we have the SOPHOS Sweep NT primary registry key
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, strExceptionKey, 0, KEY_READ, &hKey) == ERROR_SUCCESS) 
	{
		// OK so it's probably installed - get any display version
		CString strDisplayVersion = CReg::GetItemString(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "Version");	
		RegCloseKey (hKey);

		applicationName = "Sophos SweepNT";
		publisher = "Sophos Inc.";
		productID = "|" + strDisplayVersion + "|" + publisher;

		// Valid application so add to our list
		AddApplicationInstance(applicationName, publisher, productGUID, productID, version);
	}

	// Novell Client
	strExceptionKey = "Software\\Novell\\NetWareWorkstation\\CurrentVersion";
	if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_LOCAL_MACHINE, strExceptionKey, 0, KEY_READ, &hKey)) 
	{
		// OK so it's probably installed - get the Display Name
		applicationName = CReg::GetItemString(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "Title");	
		RegCloseKey (hKey);
		publisher = "Novell Inc.";
		productID = "";

		// ...and then the version
		int nMajorVersion = CReg::GetItemInt(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "MajorVersion");	
		int nMinorVersion = CReg::GetItemInt(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "MinorVersion");	
		CString strBuildNumber = CReg::GetItemString(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "");	

		// format the version
		version.Format("%d.%d %s" ,nMajorVersion ,nMinorVersion ,strBuildNumber);

		// Valid application so add to our list
		AddApplicationInstance(applicationName, publisher, productGUID, productID, version);
	}

	// IE (Bug #525)
	strExceptionKey = "SOFTWARE\\Microsoft\\Internet Explorer";
	if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_LOCAL_MACHINE, strExceptionKey, 0, KEY_READ, &hKey)) 
	{
		// OK so it's probably installed - get the Display Name
		applicationName = "Internet Explorer";	
		RegCloseKey (hKey);

		publisher = "Microsoft Corporation, Inc.";
		version = CReg::GetItemString(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "svcVersion");
		if (version == "")
			version = CReg::GetItemString(HKEY_LOCAL_MACHINE, strExceptionKey.GetBuffer(0), "Version");
		productID = CReg::GetItemString(HKEY_LOCAL_MACHINE ,"SOFTWARE\\Microsoft\\Internet Explorer\\Registration" ,PRODUCTID);

		CByteArray arrayDigitalProductID;
		
		// Using the passed in 
		if (CReg::GetItemBinary (HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Internet Explorer\\Registration", DIGITALPRODUCTID ,arrayDigitalProductID))
		{
			// if we have an empty DigitalPRoductID we will hit an exception
			// so check here that the byte array larger size is > 1
			if (arrayDigitalProductID.GetSize() > 1)
				productGUID = DecodeDigitalProductKey(arrayDigitalProductID);
		}		

		// Valid application so add to our list
		CApplicationInstance newApplication;
		CApplicationSerial newSerial;

		newApplication.Name(applicationName);
		newApplication.Publisher(publisher);
		newApplication.Version(version);
		newApplication.Guid(productGUID);
		newApplication.Source("Windows Installer Key");

		newSerial.ApplicationName(applicationName);
		newSerial.ProductId(productID);
		newSerial.CdKey(productGUID);
		newApplication.Serial(newSerial);		

		// ...add to our list
		_listApplicationInstances.Add(newApplication);
	}
}




//
//    SetGUID
//    =======
//
//    Set the product GUID for the specified application instance
//
CApplicationInstance* CApplicationInstanceList::SetGuid(CString& application, CString& value)
{
	CApplicationInstance* thisInstance = ContainsApplication(application);
	if (thisInstance != NULL)
		thisInstance->Guid(value);
	return thisInstance;
}


//
//    SetSerial
//    =========
//
//    Set the serial number attributes for the specified application instance
CApplicationInstance* CApplicationInstanceList::SetSerial(CString& application, CApplicationSerial* pSerial)
{
	CApplicationInstance* thisInstance = ContainsApplication(application);
	if (thisInstance != NULL)
		thisInstance->Serial(*pSerial);
	return thisInstance;
}



//
//    SetVersion
//    ==========
//
//    Set the version for the specified application instance
//
CApplicationInstance* CApplicationInstanceList::SetVersion(CString& application, CString& value)
{
	CApplicationInstance* thisInstance = ContainsApplication(application);
	if (thisInstance != NULL)
		thisInstance->Version(value);
	return thisInstance;
}