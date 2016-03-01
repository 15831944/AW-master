#include "stdafx.h"
#include "AuditScannerConfiguration.h"

#define  VERSION		"8.0"

// String storage for sections and values in the XML audit Data File
#define S_SCANNER_CONFIGURATION		"scannerconfiguration" //"AuditScannerDefinition"

// General Section  (TOP LEVEL)
#define S_GENERAL					"general"
#define V_GENERAL_AUTHOR			"author"
#define V_GENERAL_VERSION			"version"
#define	V_GENERAL_AUTONAME			"autoname"
#define V_GENERAL_MODE				"mode"
#define V_GENERAL_HIDDEN			"hidden"
#define V_GENERAL_REAUDITINTERVAL	"reauditinterval"
#define V_GENERAL_SHUTDOWN			"shutdown"

// UPLOAD Section
#define S_UPLOAD					"upload"
#define V_UPLOAD_VIA_NETWORK		"uploadvianetwork"
#define V_UPLOAD_VIA_TCP			"uploadviatcp"
#define V_UPLOAD_VIA_FTP			"uploadviaftp"
#define	V_UPLOAD_VIA_EMAIL			"uploadviaemail"
//
#define V_UPLOAD_DEPLOYPATH_SINGLE	"deploypathsingle"
#define V_UPLOAD_DEPLOYPATH_ROOT	"deployrootfolder"
#define V_UPLOAD_SCANPATH			"scanpath"
#define V_UPLOAD_DATAPATH			"datapath"
//
#define V_UPLOAD_EMAIL_ADDRESS		"email"
#define V_UPLOAD_SERVER				"AuditWizardServer"

// FTP Settings appear as a sub-section within 'Upload'
#define S_FTPUPLOAD					"ftpupload"
#define V_FTP_COPYTONETWORK			"copytonetwork"
#define V_FTP_SITE					"site"
#define V_FTP_PORT					"port"
#define V_FTP_DEFAULTDIRECTORY		"defaultdirectory"

// FTP Login Settings appear as a sub-section within 'General>FTP Upload'
#define S_FTPANONYMOUS				"login"
#define V_FTP_USER					"username"
#define V_FTP_PASSWORD				"password"

// Collection Section (TOP LEVEL)
#define S_COLLECTION				"collection"

// Hardware Collection Settings appear as a sub-section within 'Collection'
#define S_COLLECTION_HARDWARE		"hardware"
#define V_HARDWARE_SYSTEM			"system"
#define V_HARDWARE_NETWORKDRIVES	"networkdrives"

// Software Collection Settings appear as a sub-section within 'Collection'
#define S_COLLECTION_SOFTWARE		"software"
#define V_SOFTWARE_APPLICATIONS		"installedapplications"
#define V_SOFTWARE_OS				"operatingsystem"

// File System Settings
#define S_COLLECTION_FILESYSTEM		"filesystem"
#define V_FILESYSTEM_SCANFOLDERS	"folders"
#define V_FILESYSTEM_FOLDERSPEC		"folderspec"
#define V_FILESYSTEM_SCANFILES		"files"
#define V_FILESYSTEM_FILESPEC		"filespec"

// Internet Collection Settings appear as a sub-section within 'Collection'
#define S_COLLECTION_INTERNET		"internet"
#define V_INTERNET_COOKIES			"cookies"
#define V_INTERNET_HISTORY			"history"
#define V_INTERNET_DETAILS			"details"
#define V_INTERNET_DAYS				"days"

// Mobile Device Collection Settings appear as a sub-section within 'Collection'
#define S_COLLECTION_MOBILEDEVICES	"mobiledevicescanning"
#define V_MOBILE_SCANFILES			"files"
#define V_MOBILE_FILESPEC			"filespec"

// USB Device Collection Settings appear as a sub-section within 'Collection'
#define S_COLLECTION_USBDEVICES		"usbdevicescanning"
#define V_USB_SCANFILES				"files"
#define V_USB_FILESPEC				"filespec"

// Alert Monitor Section (TOP LEVEL)
#define S_ALERTMONITOR				"alertmonitor"
#define V_ALERT_ENABLED				"enabled"
#define V_ALERT_SETTINGSINTERVAL	"settingsinterval"
#define V_ALERT_INTERVAL			"interval"
#define V_ALERT_TRAYICON			"trayicon"
#define V_ALERT_EXCLUSIONS			"exclusions"

// Interactive Section (TOP LEVEL)
#define S_INTERACTIVE				"interactive"
#define V_INTERACTIVE_BASICDATA		"basicdata"
#define V_INTERACTIVE_LOCATION		"location"
#define V_INTERACTIVE_ASSETDATA		"assetdata"
#define V_INTERACTIVE_ASSETNAME		"assetname"
#define V_INTERACTIVE_MAKE			"make"
#define V_INTERACTIVE_MODEL			"model"
#define V_INTERACTIVE_CATEGORY		"category"
#define V_INTERACTIVE_SERIAL		"serialnumber"
#define V_INTERACTIVE_CANCEL		"cancel"
#define V_INTERACTIVE_ADDLOCATION	"addlocation"
//
#define S_INTER_ASSET_CATEGORIES	"Categories"
#define V_INTER_ASSET_CATEGORY		"Category"
//
#define S_INTER_LOCATIONS			"Locations"
#define S_INTER_LOCATION			"Location"
#define V_INTER_LOCATIONNAME		"Name"
#define V_INTER_LOCATIONID			"ID"
#define V_INTER_LOCATIONPARENTID	"ParentID"

// Serial Number Mappings Section (TOP LEVEL)
#define S_APPLICATION_MAPPINGS		"serialnumbermappings"

// The Application Settings appear as a sub-section within 'Serial Number Mappings' 
#define S_APPLICATION				"application"
#define V_APPLICATION_NAME			"name"

// The Registry Key Settings appear as a sub-section within 'Serial Number Mappings>Application'
#define S_APPLICATION_REGISTRYKEY	"registrykey"
#define V_REGISTRYKEY_NAME			"name"
#define V_REGISTRYKEY_VALUE			"value"

// The publisher mappings 
#define S_PUBLISHER_MAPPINGS		"publishermappings"
#define S_PUBLISHER					"publisher"
#define V_PUBLISHER_NAME			"name"
#define V_PUBLISHER_ALIAS			"alias"

// User Data Collection Settings appear as a sub-section within 'Interactive'
#define S_INTER_USERCATEGORIES		"userdatacategories"
#define S_INTER_USERCATEGORY		"userdatacategory"
#define V_INTER_USERCATEGORY_NAME	"userdatacategory_name"
//
#define S_INTER_USERFIELD			"userfield"
#define V_INTER_USERFIELD_NAME		"name"
#define V_INTER_USERFIELD_TYPE		"type"
#define V_INTER_USERFIELD_LENGTH	"length"
#define V_INTER_USERFIELD_MANDATORY "mandatory"
#define V_INTER_USERFIELD_CASE		 "case"
#define V_INTER_USERFIELD_MINIMUM	"minimum"
#define V_INTER_USERFIELD_MAXIMUM	"maximum"
#define V_INTER_USERFIELD_TABORDER	"taborder"
#define V_INTER_USERFIELD_PICKLIST	"picklist"
#define V_INTER_USERFIELD_VARIABLE	"variable"
#define V_INTER_USERFIELD_REGKEY	"key"
#define V_INTER_USERFIELD_REGVALUE	"value"

// PickList and PickItems (INTERACTIVE AUDITS ONLY)
#define S_INTER_PICKLISTS			"picklists"
#define S_INTER_PICKLIST			"picklist"
#define V_INTER_PICKLIST_NAME		"name"
//
#define S_INTER_PICKITEM			"pickitem"
#define V_INTER_PICKITEM_NAME		"name"





//
#define ALLFIELDS	(int)CAuditScannerConfiguration::AssetName | (int)CAuditScannerConfiguration::Make | (int)CAuditScannerConfiguration::Model | (int)CAuditScannerConfiguration::Category | (int)CAuditScannerConfiguration::SerialNumber | (int)CAuditScannerConfiguration::Cancel | (int)CAuditScannerConfiguration::AddLocation;
#define ALLSCREENS	(int)CAuditScannerConfiguration::BasicInfo | (int)CAuditScannerConfiguration::Location | (int)CAuditScannerConfiguration::AssetData;

CAuditScannerConfiguration::CAuditScannerConfiguration()
{
	// Set any default values for the scanner configuration settings
	InitializeDefaults();
}

//
//    InitializeDefaults
//    ==================
//
//    Initialize the scanner configuration with default values
//

void CAuditScannerConfiguration::InitializeDefaults()
{
	// get the windows temp paths
	char szBuffer[_MAX_PATH];
	::GetTempPath (sizeof(szBuffer), szBuffer);
	_tempFolder = szBuffer;

	// General Settings
	_autoName = true;
	_scannerMode = CAuditScannerConfiguration::modeNonInteractive;
	_hidden = true;
	_reAuditInterval = 0;
	_screensMask = ALLSCREENS;
	_fieldsMask = ALLFIELDS;
	_AuditWizardServer = "";

	// Set defaults for FTP fields
	_FTPAnonymous = true;
	_FTPPort = 21;
	_FTPUser = "";
	_FTPPassword = "";
	_FTPDefDir = "";
	_FTPSite = "";

	// Alert Monitor Settings
	_enableAlertMonitor = false;
	_alertTrayIcon = false;
	_alertUploadInterval = 60;
	_alertInterval = 60;
	_settingsInterval = 30;
	_alertExclusions = "";

	// Deployment Settings
	_deployPathSingle = true;
	_deployPathRoot = "";
	_deployPathScanner = "";
	_deployPathData = "";

	// Hardware Scanning Settings
	_hardwareScan = true;
	_hardwareNetworkDrives = false;
	_hardwareSystem = true;
	_listRegistryKeys.Empty();

	// Software Scanning Settings
	_softwareScan = true;
	_softwareScanApplications = true;
	_softwareScanOs = true;
	_scanFolders = CAuditScannerConfiguration::noFolders;
	//_listFolders.Empty();
	//_listFolders.Add("c:\\Program Files");
	_scanFiles = CAuditScannerConfiguration::allExeFiles;
	//_listFiles.Empty();
	//_listFiles.Add("*.exe");
	//_listFiles.Add("*.com");
	//_listFiles.Add("*.sys");
	_captureFiles = false;
	_listCapture.Empty();
	//_listCapture.Add("autoexec.bat");
	//_listCapture.Add("config.sys");
	//_listCapture.Add("win.ini");

	// Internet Scanning Options
	_IEScan = true;
	_IEHistory = true;
	_IECookies = true;
	_IEDetails = false;
	_IEDays = 7;

	// USB auditing Scanning Settings
	_scanUSB = false;
	_scanUSBFiles = CAuditScannerConfiguration::noFiles;
	_listUSBFiles.Empty();
	_listUSBFiles.Add("*.exe");

	// Mobile device auditing Scanning Settings
	_scanMDA = false;
	_scanMobileFiles = CAuditScannerConfiguration::noFiles;
	_listMobileFiles.Empty();
	_listMobileFiles.Add("*.exe");
	
	// Ensure that we release any resources previously allocated before reading the file
	_rootLocation = "";
	_listLocations.Empty();
	_listAssetTypes.Empty();
	_listApplicationRegistryMappings.Empty();
	_publisherMappings.Empty();
	_listRegistryKeys.Empty();

	_displayBasicInformationScreen = false;
	_displayLocationScreen = false;
	_displayUserDataScreen = false;
	_interactiveAllowCancel = false;
	_interactiveAllowChangeCategory = false;
	_interactiveAllowChangeManufacturer = false;
	_interactiveAllowChangeModel = false;
	_interactiveAllowChangeSerial = false;

	// File System Scan
	_scanFileSystem = false;

	// Pickup Windows Temp path
	char szTempPath[_MAX_PATH + 1];
	GetTempPath(_MAX_PATH, szTempPath);
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
int CAuditScannerConfiguration::Load(LPCSTR pszPath)
{
	CLogFile log;

	// Reset the configuration to factory defaults including releasing any memory allocated
	InitializeDefaults();

	// Now read the file noting that this will throw an exception if the open fails
	unsigned char* pBuffer;
	int trycount = 0;
	ULONGLONG dwLength = 0;

	log.Write("CAuditScannerConfiguration::Load - Attempt to read scanner configuration file at " + CTime::GetCurrentTime().Format("%Y-%m-%d %H:%M:%S"));
	while (trycount < 10)
	{
		CFile* pFile = NULL;
		try
		{
			pFile = new CFile(pszPath, CFile::modeRead | CFile::shareDenyNone);
			dwLength = pFile->GetLength();

			// Allocate buffer for binary file data
			pBuffer = new unsigned char[dwLength + 2];

			// ...and read the file into this buffer
			dwLength = pFile->Read( pBuffer, dwLength );

			// Close the file again and break out of the while loop
			pFile->Close();
			break;
		}
		catch (CFileException* pEx)
		{
			pFile->Close();

			// If the exception was a sharing violation or too many open files then wait a while and try again
			if (pEx->m_cause == CFileException::sharingViolation || pEx->m_cause == CFileException::tooManyOpenFiles)
			{
				Sleep(1000);
				trycount++;
				log.Format("CAuditScannerConfiguration::Load - Failed to read read scanner configuration file (%d) at %s", trycount, CTime::GetCurrentTime().Format("%Y-%m-%d %H:%M:%S"));
			}
			else
			{
				// ...any other error just re-throw it
				log.Format("CAuditScannerConfiguration::Load - Failed to read read scanner configuration file at %s", CTime::GetCurrentTime().Format("%Y-%m-%d %H:%M:%S"));
				throw;
			}
		}
	}

	log.Format("CAuditScannerConfiguration::Load - scanner configuration file read and should now be closed at %s", CTime::GetCurrentTime().Format("%Y-%m-%d %H:%M:%S"));

	// Terminate the buffer
	CString csText;
	pBuffer[dwLength] = '\0';
	pBuffer[dwLength+1] = '\0'; // in case 2-byte encoded
	csText = (LPCSTR)pBuffer;
	delete [] pBuffer;

	// Load the XML buffer into our internal XML object
	CMarkup xmlFile;
	if (!xmlFile.SetDoc(csText))
		return -1;

	// Ok the file has now been read - let's process it
	xmlFile.ResetPos();

	// We must have the main scanner configuration section otherwise this is an invalid file
	/*if (!xmlFile.FindElem(S_SCANNER_CONFIGURATION))
		return -1;*/

	if (!xmlFile.FindElem("AuditScannerDefinition"))
		return -1;

	// Process the file as we can be confident that it is a scanner configuration file
	//ProcessElementRead(xmlFile);
	DeserializeScannerConfiguration(xmlFile);
	return 0;
}

void CAuditScannerConfiguration::DeserializeScannerConfiguration(CMarkup xmlFile)
{
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// general configuration settings
		if (elementName == "Shutdown")
		{
			_shutdown = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "Hidden")
		{
			_hidden = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "ReAuditInterval")
		{
			_reAuditInterval = xmlFile.GetChildDataAsInt();
		}
		else if (elementName == "DeployPathRoot")
		{
			_deployPathRoot = xmlFile.GetChildData();
		}
		else if (elementName == "DeployPathSingle")
		{
			_deployPathSingle = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "DeployPathScanner")
		{
			_deployPathScanner = xmlFile.GetChildData();
		}
		else if (elementName == "DeployPathData")
		{
			_deployPathData = xmlFile.GetChildData();
		}

		// interactive mode
		else if (elementName == "ScannerMode")
		{
			CString scannerMode = xmlFile.GetChildData();

			if (scannerMode == "modeNonInteractive")
			{
				_scannerMode = (eAuditMode)0;
			}
			else if (scannerMode == "modeInteractive")
			{
				_scannerMode = (eAuditMode)1;
			}
			else if (scannerMode == "modeFirstTimeInteractive")
			{
				_scannerMode = (eAuditMode)2;
			}
			else
			{
				_scannerMode = (eAuditMode)0;
			}
		}

		else if (elementName == "InteractiveAllowCancel")
		{
			_interactiveAllowCancel = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveDisplayAssetInformation")
		{
			_displayBasicInformationScreen = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveEnableAssetCategory")
		{
			_interactiveAllowChangeCategory = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveEnableAssetMake")
		{
			_interactiveAllowChangeManufacturer = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveEnableAssetModel")
		{
			_interactiveAllowChangeModel = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveEnableAssetSerial")
		{
			_interactiveAllowChangeSerial = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveDisplayLocations")
		{
			_displayLocationScreen = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveEnableAddLocation")
		{
			_interactiveAllowAddLocation = xmlFile.GetChildDataAsBoolean();
		}		

		else if (elementName == "InteractiveDisplayAssetData")
		{
			_displayUserDataScreen = xmlFile.GetChildDataAsBoolean();
		}

		else if (elementName == "InteractiveCategories")
		{
			ListFromString(_listAssetTypes ,xmlFile.GetChildData(), ';');
		}
		else if (elementName == "RootLocation")
		{
			_rootLocation = xmlFile.GetChildData();
		}
		else if (elementName == "InteractiveLocations")
		{
			xmlFile.IntoElem();

			// Get the list of elements in this section
			while (xmlFile.FindChildElem(""))
			{
				CString elementName = xmlFile.GetChildTagName();

				// The Collection element really is a collection of more element groups, one for each Location
				if (elementName == "AssetGroup")
				{
					xmlFile.IntoElem();

					// Create a dummy location
					CAuditLocation newLocation;
					
					// Get the list of elements in this section
					while (xmlFile.FindChildElem(""))
					{
						CString elementName = xmlFile.GetChildTagName();

						// The Collection element really is a collection of more element groups, one for each Location
						if (elementName == "Name")
						{
							newLocation.Name(xmlFile.GetChildData());
						}
						
						else if (elementName == "GroupID")
						{
							newLocation.ID(xmlFile.GetChildDataAsInt());
						}
						
						else if (elementName == "ParentID")
						{
							newLocation.ParentID(xmlFile.GetChildDataAsInt());
						}
					}

					// Add this location to our list
					_listLocations.Add(newLocation);
					
					// Step out of this element
					xmlFile.OutOfElem();
				}
			}

			xmlFile.OutOfElem();
		}
		else if (elementName == "InteractivePicklists")
		{
			xmlFile.IntoElem();

			// Get the list of elements in this section
			while (xmlFile.FindChildElem(""))
			{
				CString elementName = xmlFile.GetChildTagName();

				// The Collection element really is a collection of more element groups, one for each Location
				if (elementName == "InteractivePickList")
				{
					xmlFile.IntoElem();
	
					// This is the Picklist being parsed
					CPicklist picklist;

					// Get the list of elements in this section
					while (xmlFile.FindChildElem(""))
					{
						CString elementName = xmlFile.GetChildTagName();

						// The Collection element really is a collection of more element groups, one for each pickitem
						if (elementName == "Name")
						{
							picklist.Name(xmlFile.GetChildData());
						}
						
						else if (elementName == "PickListItems")
						{
							xmlFile.IntoElem();

							while (xmlFile.FindChildElem(""))
							{
								CString elementName = xmlFile.GetChildTagName();

								if (elementName == "PickItem")
								{
									xmlFile.IntoElem();

									// Get the list of elements in this section
									while (xmlFile.FindChildElem(""))
									{
										CString elementName = xmlFile.GetChildTagName();

										if (elementName == "Name")
											picklist.Add(xmlFile.GetChildData());
									}

									xmlFile.OutOfElem();
								}
							}
							
							// Step out of the category
							xmlFile.OutOfElem();
						}
					}

					// Add the category to our internal list of user data categories
					_listPicklists.Add(picklist);
					
					// Step out of this category
					xmlFile.OutOfElem();
				}
			}

			xmlFile.OutOfElem();
		}
		else if (elementName == "InteractiveUserDataCategories")
		{
			xmlFile.IntoElem();

			while (xmlFile.FindChildElem(""))
			{
				CUserDataCategory dataCategory;	
				CUserDataField dataField;

				CString elementName = xmlFile.GetChildTagName();

				// The Collection element really is a collection of more element groups, one for each Location
				if (elementName == "UserDataCategories")
				{
					xmlFile.IntoElem();

					// Get the list of elements in this section
					while (xmlFile.FindChildElem(""))
					{
						CString elementName = xmlFile.GetChildTagName();

						// The Collection element really is a collection of more element groups, one for each Location
						if (elementName == "Name")
						{
							dataCategory.Name(xmlFile.GetChildData());
						}
						
						else if (elementName == "UserFields")
						{
							xmlFile.IntoElem();

							while (xmlFile.FindChildElem(""))
							{
								CString elementName = xmlFile.GetChildTagName();

								if (elementName == "UserDataField")
								{
									xmlFile.IntoElem();

									// Get the list of elements in this section
									while (xmlFile.FindChildElem(""))
									{
										CString elementName = xmlFile.GetChildTagName();

										if (elementName == "Name")
											dataField.Label(xmlFile.GetChildData());
										
										else if (elementName == "Type")
										{
											CString fieldType = xmlFile.GetChildData();

											if (fieldType == "Text")
											{
												dataField.DataType(0);
											}
											else if (fieldType == "Number")
											{
												dataField.DataType(1);
											}
											else if (fieldType == "Picklist")
											{
												dataField.DataType(2);
											}
											else if (fieldType == "Environment")
											{
												dataField.DataType(3);
											}
											else if (fieldType == "Registry")
											{
												dataField.DataType(4);
											}
											else if (fieldType == "Date")
											{
												dataField.DataType(5);
											}
											else if (fieldType == "Currency")
											{
												dataField.DataType(6);
											}
											else
											{
												dataField.DataType(0);
											}

											//dataField.DataType(xmlFile.GetChildData());
										}
										
										else if (elementName == "Length")
											dataField.Length(xmlFile.GetChildDataAsInt());
										
										else if (elementName == "IsMandatory")
											dataField.Mandatory(xmlFile.GetChildDataAsBoolean());
										
										else if (elementName == "InputCase")
										{
											dataField.InputCase(xmlFile.GetChildData());
										}
										
										else if (elementName == "MinimumValue")
											dataField.MinimumValue(xmlFile.GetChildDataAsInt());
										
										else if (elementName == "MaximumValue")
											dataField.MaximumValue(xmlFile.GetChildDataAsInt());
										
										else if (elementName == "Picklist")
											dataField.Picklist(xmlFile.GetChildData());
										
										else if (elementName == "EnvironmentVariable")
											dataField.EnvironmentVariable(xmlFile.GetChildData());
										
										else if (elementName == "RegistryKey")
											dataField.RegistryKey(xmlFile.GetChildData());
										
										else if (elementName == "RegistryValue")
											dataField.RegistryValue(xmlFile.GetChildData());	
									}

									dataCategory.Add(dataField);
									xmlFile.OutOfElem();
								}
							}

							// Add the field to the category
							//dataCategory.Add(dataField);
							
							// Step out of the category
							xmlFile.OutOfElem();
						}
					}

					// Add the category to our internal list of user data categories
					_listUserDataCategories.Add(dataCategory);
					
					// Step out of this category
					xmlFile.OutOfElem();
				}
			}
			xmlFile.OutOfElem();	
		}

		// alert monitor settings
		else if (elementName == "AlertMonitorEnabled")
			_enableAlertMonitor = xmlFile.GetChildDataAsBoolean();
			
		else if (elementName == "AlertMonitorSettingSecs")
			_settingsInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == "AlertMonitorAlertSecs")
			_alertInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == "AlertMonitorDisplayTray")
			_alertTrayIcon = xmlFile.GetChildDataAsBoolean();
		
		else if (elementName == "AlertMonitorDefinition")
		{
			xmlFile.IntoElem();
			if (xmlFile.FindChildElem("MonitoredComputers"))
			{
				xmlFile.IntoElem();
				while (xmlFile.FindChildElem("string"))
				{
					_listMonitoredComputers.Add(xmlFile.GetChildData());
				}
				xmlFile.OutOfElem();
			}
			xmlFile.OutOfElem();
		}

		else if (elementName == "HardwareScan")
		{
			_hardwareScan = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "RegistryScan")
		{
			_registryScan = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "SoftwareScan")
		{
			_softwareScan = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "IEScan")
		{
			_IEScan = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "ScanMDA")
		{
			_scanMDA = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "ScanUSB")
		{
			_scanUSB = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "FileSystemScan")
		{
			_scanFileSystem = xmlFile.GetChildDataAsBoolean(); 
		}

		// other settings
		else if (elementName == "ListRegistryKeys")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				_listRegistryKeys.Add(xmlFile.GetChildData());
			}
			xmlFile.OutOfElem();
		}
		else if (elementName == "HardwareSystem")
		{
			_hardwareSystem = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "HardwareNetworkDrives")
		{
			_hardwareNetworkDrives = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "HardwarePhysicalDisks")
		{
			_hardwarePhysicalDisks = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "HardwareSettings")
		{
			_hardwareSettings = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "HardwareSecurity")
		{
			_hardwareSecurity = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "HardwareActiveSystem")
		{
			_hardwareActiveSystem = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "SoftwareScanApplications")
		{
			_softwareScanApplications = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "SoftwareScanOs")
		{
			_softwareScanOs = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "ScanFolders")
		{
			CString foldersToScan = xmlFile.GetChildData();

			if (foldersToScan == "noFolders")
			{
				_scanFolders = (eFolderSetting)0;
			}
			else if (foldersToScan == "allFolders")
			{
				_scanFolders = (eFolderSetting)1;
			}
			else if (foldersToScan == "specifiedFolders")
			{
				_scanFolders = (eFolderSetting)2;
			}
			else
			{
				_scanFolders = (eFolderSetting)0;
			}
		}
		else if (elementName == "ListFolders")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				_listFolders.Add(xmlFile.GetChildData());
			}
			xmlFile.OutOfElem();
		}
		else if (elementName == "ScanFiles")
		{
			CString fileTypesToScan = xmlFile.GetChildData();

			if (fileTypesToScan == "noFiles")
			{
				_scanFiles = (eFileSetting)0;
			}
			else if (fileTypesToScan == "allExeFiles")
			{
				_scanFiles = (eFileSetting)1;
			}
			else if (fileTypesToScan == "specifiedFiles")
			{
				_scanFiles = (eFileSetting)2;
			}
			else if (fileTypesToScan == "allFiles")
			{
				_scanFiles = (eFileSetting)3;
			}
			else
			{
				_scanFiles = (eFileSetting)0;
			}
		}
		else if (elementName == "ListFiles")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				CString tmpElement = xmlFile.GetChildData();
				_listFiles.Add(tmpElement);
			}
			xmlFile.OutOfElem();
		}
		else if (elementName == "CaptureFiles")
		{
			_captureFiles = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "ListCapture")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				_listCapture.Add(xmlFile.GetChildData());
			}
			xmlFile.OutOfElem();
		}
		else if (elementName == "ScanMobileFiles")
		{
			_scanMobileFiles = (eFileSetting)xmlFile.GetChildDataAsInt();
		}
		else if (elementName == "ListMobileFiles")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				_listMobileFiles.Add(xmlFile.GetChildData());
			}
			xmlFile.OutOfElem();
		}
		else if (elementName == "ScanUSBFiles")
		{
			CString usbFileTypesToScan = xmlFile.GetChildData();

			if (usbFileTypesToScan == "noFiles")
			{
				_scanUSBFiles = (eFileSetting)0;
			}
			else if (usbFileTypesToScan == "allExeFiles")
			{
				_scanUSBFiles = (eFileSetting)1;
			}
			else if (usbFileTypesToScan == "specifiedFiles")
			{
				_scanUSBFiles = (eFileSetting)2;
			}
			else if (usbFileTypesToScan == "allFiles")
			{
				_scanUSBFiles = (eFileSetting)1;
			}
			else
			{
				_scanUSBFiles = (eFileSetting)0;
			}
		}
		else if (elementName == "ListUSBFiles")
		{
			xmlFile.IntoElem();
			while (xmlFile.FindChildElem("string"))
			{
				_listUSBFiles.Add(xmlFile.GetChildData());
			}
			xmlFile.OutOfElem();
		}		
		else if (elementName == "IEHistory")
		{
			_IEHistory = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "IECookies")
		{
			_IECookies = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "IEDays")
		{
			_IEDays = xmlFile.GetChildDataAsInt();
		}
		else if (elementName == "IEDetails")
		{
			_IEDetails = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "UploadSetting")
		{
			CString uploadType = xmlFile.GetChildData();

			if (uploadType == "network")
				_uploadSetting = CAuditScannerConfiguration::network;

			else if (uploadType == "tcp")
				_uploadSetting = CAuditScannerConfiguration::tcp;

			else if (uploadType == "ftp")
				_uploadSetting = CAuditScannerConfiguration::ftp;

			else if (uploadType == "email")
				_uploadSetting = CAuditScannerConfiguration::email;
		}
		else if (elementName == "FTPCopyToNetwork")
		{
			_ftpCopyToNetwork = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "FTPAnonymous")
		{
			_FTPAnonymous = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "FTPPort")
		{
			_FTPPort = xmlFile.GetChildDataAsInt();
		}
		else if (elementName == "FTPUser")
		{
			_FTPUser = xmlFile.GetChildData();
		}
		else if (elementName == "FTPPassword")
		{
			_FTPPassword = xmlFile.GetChildData();
		}
		else if (elementName == "FTPSite")
		{
			_FTPSite = xmlFile.GetChildData();
		}
		else if (elementName == "FTPDefDir")
		{
			_FTPDefDir = xmlFile.GetChildData();
		}
		else if (elementName == "FTPType")
		{
			_FTPType = xmlFile.GetChildData();
		}


		else if (elementName == "FTPAnonymousBackup")
		{
			_FTPAnonymousBackup = xmlFile.GetChildDataAsBoolean();
		}
		else if (elementName == "FTPPortBackup")
		{
			_FTPPortBackup = xmlFile.GetChildDataAsInt();
		}
		else if (elementName == "FTPUserBackup")
		{
			_FTPUserBackup = xmlFile.GetChildData();
		}
		else if (elementName == "FTPPasswordBackup")
		{
			_FTPPasswordBackup = xmlFile.GetChildData();
		}
		else if (elementName == "FTPSiteBackup")
		{
			_FTPSiteBackup = xmlFile.GetChildData();
		}
		else if (elementName == "FTPDefDirBackup")
		{
			_FTPDefDirBackup = xmlFile.GetChildData();
		}
		else if (elementName == "FTPTypeBackup")
		{
			_FTPTypeBackup = xmlFile.GetChildData();
		}


		else if (elementName == "EmailAddress")
		{
			_emailAddress = xmlFile.GetChildData();
		}
		else if (elementName == "SerialNumberMappingsList")
		{
			xmlFile.IntoElem();

			// Get the list of elements in this section
			while (xmlFile.FindChildElem("SerialNumberMapping"))
			{
				xmlFile.IntoElem();

				// First try and read the name of the application
				if (xmlFile.FindChildElem("ApplicationName"))
				{
					CString applicationName = xmlFile.GetChildData();
					CApplicationRegistryMapping applicationRegistryMapping(applicationName);

					// ...reset our position and loop through the registry key elements
					xmlFile.ResetChildPos();

					if (xmlFile.FindChildElem("RegistryKeysList"))
					{
						xmlFile.IntoElem();

						while (xmlFile.FindChildElem("SerialNumberMappingsRegistryKey"))	
						{
							xmlFile.IntoElem();
							CString registryKey = "";
							CString registryValue = "";

							// Try and pick up the registry key name
							if (xmlFile.FindChildElem("RegistryKeyName"))
								registryKey = xmlFile.GetChildData();

							// Try and pick up the registry value name
							if (xmlFile.FindChildElem("RegistryKeyValue"))
								registryValue = xmlFile.GetChildData();

							// If we have at least the registry key then add it as a mapping
							if (!registryKey.IsEmpty())
								applicationRegistryMapping.AddMapping(registryKey ,registryValue);

							// Out of the registry key/value
							xmlFile.OutOfElem();
						}

						xmlFile.OutOfElem();
					}

					//xmlFile.OutOfElem();

					// If we have at least an application name and 1 key then we should add this entry to
					// our internal list
					if (applicationRegistryMapping.GetMappings().GetCount() != 0)
						_listApplicationRegistryMappings.Add(applicationRegistryMapping);
				}

				xmlFile.OutOfElem();
			}

			xmlFile.OutOfElem();
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
void CAuditScannerConfiguration::ProcessElementRead(CMarkup xmlFile)
{
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_GENERAL)
			ProcessGeneralElementRead(xmlFile);

		else if (elementName == S_UPLOAD)
			ProcessUploadElementRead(xmlFile);

		else if (elementName == S_ALERTMONITOR)
			ProcessAlertMonitorElementRead(xmlFile);

		else if (elementName == S_INTERACTIVE)
			ProcessInteractiveElementRead(xmlFile);

		else if (elementName == S_COLLECTION)
			ProcessCollectionElementRead(xmlFile);

		else if (elementName == S_APPLICATION_MAPPINGS)
			ProcessApplicationMappingsElementRead(xmlFile);

		else if (elementName == S_PUBLISHER_MAPPINGS)
			ProcessPublisherMappingsElementRead(xmlFile);
	}
}



// 
// We have parsed the 'General' element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
//
void CAuditScannerConfiguration::ProcessGeneralElementRead(CMarkup xmlFile)
{
	// Step into the 'General' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_GENERAL_AUTHOR)
			_author = xmlFile.GetChildData();

		else if (elementName == V_GENERAL_VERSION)
			_version = xmlFile.GetChildData();

		else if (elementName == V_GENERAL_AUTONAME)
			_autoName = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_GENERAL_HIDDEN)
			_hidden = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_GENERAL_MODE)
			_scannerMode = (eAuditMode)xmlFile.GetChildDataAsInt();

		else if (elementName == V_GENERAL_REAUDITINTERVAL)
			_reAuditInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == V_GENERAL_SHUTDOWN)
			_shutdown = xmlFile.GetChildDataAsBoolean();
	}

	xmlFile.OutOfElem();
}



// 
// We have parsed the 'Upload' element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
//
void CAuditScannerConfiguration::ProcessUploadElementRead(CMarkup xmlFile)
{
	// Step into the 'Upload' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_UPLOAD_VIA_NETWORK)
			_uploadSetting = CAuditScannerConfiguration::network;

		else if (elementName == V_UPLOAD_VIA_TCP)
			_uploadSetting = CAuditScannerConfiguration::tcp;

		else if (elementName == V_UPLOAD_VIA_FTP)
			_uploadSetting = CAuditScannerConfiguration::ftp;

		else if (elementName == V_UPLOAD_VIA_EMAIL)
			_uploadSetting = CAuditScannerConfiguration::email;
		
		else if (elementName == V_UPLOAD_SCANPATH)
			_deployPathScanner = xmlFile.GetChildData();
		
		else if (elementName == V_UPLOAD_DATAPATH)
			_deployPathData = xmlFile.GetChildData();
		
		else if (elementName == V_UPLOAD_EMAIL_ADDRESS)
			_emailAddress = xmlFile.GetChildData();

		else if (elementName == V_UPLOAD_SERVER)
			_AuditWizardServer = xmlFile.GetChildData();

		else if (elementName == S_FTPUPLOAD)
			ProcessUploadFtpElementRead(xmlFile);
	}

	xmlFile.OutOfElem();
}


//
//    ProcessUploadFtpElementRead
//    ============================
//
//    Process the child FTP section located within the Upload element
//
void CAuditScannerConfiguration::ProcessUploadFtpElementRead(CMarkup xmlFile)
{
	// We need to step into the 'FTP Upload' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		if (elementName == V_FTP_SITE)
			_FTPSite = xmlFile.GetChildData();

		else if (elementName == V_FTP_PORT)
			_FTPPort = xmlFile.GetChildDataAsInt();

		else if (elementName == V_FTP_DEFAULTDIRECTORY)
			_FTPDefDir = xmlFile.GetChildData();

		else if (elementName == S_FTPANONYMOUS)
			ProcessUploadFtpAnonymousRead(xmlFile);
	}

	// Before we return we need to move up to our parent level so that we can 
	// continue to process sibling elements
	xmlFile.OutOfElem();
}


//
//    ProcessUploadFtpAnonymousRead
//    ==============================
//
//    Process FTP Anonymous settings within the General section
//
void CAuditScannerConfiguration::ProcessUploadFtpAnonymousRead(CMarkup xmlFile)
{
	_FTPAnonymous = false;

	// We need to step into the 'FTP Upload' section as we wre already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_FTP_USER)
			_FTPUser = xmlFile.GetChildData();

		else if (elementName == V_FTP_PASSWORD)
			_FTPPassword = xmlFile.GetChildData();
	}

	// Before we return we need to move up to our parent level so that we can 
	// continue to process sibling elements
	xmlFile.OutOfElem();
}



//
//    ProcessAlertMonitorElementRead
//    ==============================
//    
//    We have parsed the 'Alert Monitor' element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end 
//    of the section.
//
void CAuditScannerConfiguration::ProcessAlertMonitorElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();
	_enableAlertMonitor = true;

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_ALERT_ENABLED)
			_enableAlertMonitor = xmlFile.GetChildDataAsBoolean();
			
		else if (elementName == V_ALERT_SETTINGSINTERVAL)
			_settingsInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == V_ALERT_INTERVAL)
			_alertInterval = xmlFile.GetChildDataAsInt();

		else if (elementName == V_ALERT_TRAYICON)
			_alertTrayIcon = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_ALERT_EXCLUSIONS)
			_alertExclusions = xmlFile.GetChildData();
	}

	xmlFile.OutOfElem();
}


//
//    ProcessInteractiveElementRead
//    =============================
//
//    We have parsed the 'Interactive' element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end 
//    of the section.
//
void CAuditScannerConfiguration::ProcessInteractiveElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		
		if (elementName == S_INTER_ASSET_CATEGORIES)
		{
			ProcessInterCategoriesElementRead(xmlFile);		
		}
		
		else if (elementName == S_INTER_LOCATIONS)
		{
			ProcessInterLocationsElementRead(xmlFile);		
		}
		
		else if (elementName == S_INTER_USERCATEGORIES)
		{
			ProcessUserDataCategories(xmlFile);		
		}
		
		else if (elementName == S_INTER_PICKLISTS)
		{
			ProcessPicklists(xmlFile);		
		}
		
		else if (elementName == V_INTERACTIVE_BASICDATA)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_screensMask |= (int)BasicInfo;
			else
				_screensMask &= (~(int)BasicInfo);
		}

		else if (elementName == V_INTERACTIVE_LOCATION)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_screensMask |= (int)Location;
			else
				_screensMask &= (~(int)Location);
		}

		else if (elementName == V_INTERACTIVE_ASSETDATA)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_screensMask |= (int)AssetData;
			else
				_screensMask &= (~(int)AssetData);
		}

		else if (elementName == V_INTERACTIVE_ASSETNAME)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)AssetName;
			else
				_fieldsMask &= (~(int)AssetName);
		}

		else if (elementName == V_INTERACTIVE_MAKE)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)Make;
			else
				_fieldsMask &= (~(int)Make);
		}

		else if (elementName == V_INTERACTIVE_MODEL)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)Model;
			else
				_fieldsMask &= ~(int)Model;
		}

		else if (elementName == V_INTERACTIVE_CATEGORY)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)Category;
			else
				_fieldsMask &= ~(int)Category;
		}

		else if (elementName == V_INTERACTIVE_SERIAL)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)SerialNumber;
			else
				_fieldsMask &= ~(int)SerialNumber;
		}

		else if (elementName == V_INTERACTIVE_CANCEL)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)Cancel;
			else
				_fieldsMask &= ~(int)Cancel;
		}

		else if (elementName == V_INTERACTIVE_ADDLOCATION)
		{
			if (xmlFile.GetChildDataAsBoolean())
				_fieldsMask |= (int)AddLocation;
			else
				_fieldsMask &= ~(int)AddLocation;
		}
	}

	xmlFile.OutOfElem();
}




// We have parsed the 'Interactive>Asset categories' (S_INTER_CATEGORIES) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessInterCategoriesElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups
		if (elementName == V_INTER_ASSET_CATEGORY)
		{
			ListFromString(_listAssetTypes ,xmlFile.GetChildData(), ';');
		}
	}

	xmlFile.OutOfElem();
}





// We have parsed the 'Interactive>Locations' (S_INTER_LOCATIONS) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessInterLocationsElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == S_INTER_LOCATION)
		{
			ProcessInterLocationElementRead(xmlFile);
		}
	}

	xmlFile.OutOfElem();
}



// We have parsed and Individual 'Interactive>Locations>Location' (S_INTER_LOCATION) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessInterLocationElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Create a dummy location
	CAuditLocation newLocation;
	
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == V_INTER_LOCATIONNAME)
		{
			newLocation.Name(xmlFile.GetChildData());
		}
		
		else if (elementName == V_INTER_LOCATIONID)
		{
			newLocation.ID(xmlFile.GetChildDataAsInt());
		}
		
		else if (elementName == V_INTER_LOCATIONPARENTID)
		{
			newLocation.ParentID(xmlFile.GetChildDataAsInt());
		}
	}

	// Add this location to our list
	_listLocations.Add(newLocation);
	
	// Step out of this element
	xmlFile.OutOfElem();
}




// We have parsed the 'Interactive>UserDataCategories' (S_INTER_USERCATEGORIES) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessUserDataCategories(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == S_INTER_USERCATEGORY)
		{
			ProcessUserDataCategory(xmlFile);
		}
	}

	xmlFile.OutOfElem();
}



// We have parsed the 'Interactive>UserDataCategories>UserDatacategory' (S_INTER_USERCATEGORY) element 
// so now parse the items within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessUserDataCategory(CMarkup xmlFile)
{
	xmlFile.IntoElem();
	
	// This is the User Data Category being parsed
	CUserDataCategory dataCategory;

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == V_INTER_USERCATEGORY_NAME)
		{
			dataCategory.Name(xmlFile.GetChildData());
		}
		
		else if (elementName == S_INTER_USERFIELD)
		{
			ProcessUserDataField(xmlFile ,dataCategory);
		}
	}

	// Add the category to our internal list of user data categories
	_listUserDataCategories.Add(dataCategory);
	
	// Step out of this category
	xmlFile.OutOfElem();
}






// We have parsed the 'Interactive>UserDataCategories>UserDatacategory>UserDataField' element 
// so now parse the items within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessUserDataField(CMarkup xmlFile ,CUserDataCategory& category)
{
	xmlFile.IntoElem();
	
	// This is the User Data Field being parsed
	CUserDataField dataField;

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == V_INTER_USERFIELD_NAME)
			dataField.Label(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_TYPE)
			dataField.DataType(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_LENGTH)
			dataField.Length(xmlFile.GetChildDataAsInt());
		
		else if (elementName == V_INTER_USERFIELD_MANDATORY)
			dataField.Mandatory(xmlFile.GetChildDataAsBoolean());
		
		else if (elementName == V_INTER_USERFIELD_CASE)
			dataField.InputCase(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_MINIMUM)
			dataField.MinimumValue(xmlFile.GetChildDataAsInt());
		
		else if (elementName == V_INTER_USERFIELD_MAXIMUM)
			dataField.MaximumValue(xmlFile.GetChildDataAsInt());
		
		else if (elementName == V_INTER_USERFIELD_PICKLIST)
			dataField.Picklist(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_VARIABLE)
			dataField.EnvironmentVariable(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_REGKEY)
			dataField.RegistryKey(xmlFile.GetChildData());
		
		else if (elementName == V_INTER_USERFIELD_REGVALUE)
			dataField.RegistryValue(xmlFile.GetChildData());		
	}

	// Add the field to the category
	category.Add(dataField);
	
	// Step out of the category
	xmlFile.OutOfElem();
}




//
// We have parsed the 'Interactive>Picklists' element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessPicklists(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each Location
		if (elementName == S_INTER_PICKLIST)
		{
			ProcessPicklist(xmlFile);
		}
	}

	xmlFile.OutOfElem();
}



// We have parsed the 'Interactive>UPicklists>Picklist' element 
// so now parse the items within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessPicklist(CMarkup xmlFile)
{
	xmlFile.IntoElem();
	
	// This is the Picklist being parsed
	CPicklist picklist;

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups, one for each pickitem
		if (elementName == V_INTER_PICKLIST_NAME)
		{
			picklist.Name(xmlFile.GetChildData());
		}
		
		else if (elementName == S_INTER_PICKITEM)
		{
			ProcessPickitem(xmlFile ,picklist);
		}
	}

	// Add the category to our internal list of user data categories
	_listPicklists.Add(picklist);
	
	// Step out of this category
	xmlFile.OutOfElem();
}



// We have parsed the 'Interactive>UPicklists>PickList>Pickitem' element 
// so now parse the items within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessPickitem(CMarkup xmlFile ,CPicklist& picklist)
{
	xmlFile.IntoElem();
	
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_INTER_PICKITEM_NAME)
			picklist.Add(xmlFile.GetChildData());
	}
	
	// Step out of the category
	xmlFile.OutOfElem();
}





// We have parsed the 'Collection' (S_COLLECTION) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
// Note that we may come across sub-sections such as 
// 
// Hardware
// Software
// Internet
// Mobile
// USB
// 
// Which we need to parse separately
void CAuditScannerConfiguration::ProcessCollectionElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	//_hardwareScan = <HardwareScan>true</HardwareScan> from xml file

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		// The Collection element really is a collection of more element groups
		if (elementName == S_COLLECTION_HARDWARE)
			ProcessHardwareCollectionElementRead(xmlFile);

		else if (elementName == S_COLLECTION_INTERNET)
			ProcessInternetCollectionElementRead(xmlFile);

		else if (elementName == S_COLLECTION_SOFTWARE)
			ProcessSoftwareCollectionElementRead(xmlFile);

		else if (elementName == S_COLLECTION_FILESYSTEM)
			ProcessFileSystemElementRead(xmlFile);

		else if (elementName == "windowsregistry")
			ProcessRegistryKeysCollectionElementRead(xmlFile);

		else if (elementName == S_COLLECTION_MOBILEDEVICES)
			ProcessMobileCollectionElementRead(xmlFile);

		else if (elementName == S_COLLECTION_USBDEVICES)
			ProcessUsbCollectionElementRead(xmlFile);
	}

	xmlFile.OutOfElem();
}


//
//    ProcessHardwareCollectionElementRead
//    ====================================
//
//    We have parsed the 'Hardware Collection' (S_COLLECTION_HARDWARE) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessHardwareCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'Hardware Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_HARDWARE_NETWORKDRIVES)
			_hardwareNetworkDrives = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_HARDWARE_SYSTEM)
			_hardwareSystem = xmlFile.GetChildDataAsBoolean();
	}

	// Step back out of 'Hardware Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}


//
//    ProcessInternetCollectionElementRead
//    ====================================
//
//    We have parsed the 'Internet Collection' (S_COLLECTION_INTERNET) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessInternetCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'Internet Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_INTERNET_COOKIES)
			_IECookies = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_INTERNET_HISTORY)
			_IEHistory = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_INTERNET_DETAILS)
			_IEDetails = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_INTERNET_DAYS)
			_IEDays = xmlFile.GetChildDataAsInt();
	}

	// Step back out of 'Internet Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}



//
//    ProcessSoftwareCollectionElementRead
//    ====================================
//
//    We have parsed the 'Software Collection' (S_COLLECTION_SOFTWARE) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessSoftwareCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'Software Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_SOFTWARE_APPLICATIONS)
			_softwareScanApplications = xmlFile.GetChildDataAsBoolean();

		else if (elementName == V_SOFTWARE_OS)
			_softwareScanOs = xmlFile.GetChildDataAsBoolean();
	}

	// Step back out of 'Software Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}




//
//    ProcessFileSystemElementRead
//    =============================
//
//    We have parsed the 'Software Collection' (S_COLLECTION_FILESYSTEM) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessFileSystemElementRead(CMarkup xmlFile)
{
	// We need to step into the 'File System Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_FILESYSTEM_SCANFILES)
			_scanFiles = (eFileSetting)xmlFile.GetChildDataAsInt();

		else if (elementName == V_FILESYSTEM_SCANFOLDERS)
			_scanFolders = (eFolderSetting)xmlFile.GetChildDataAsInt();

		else if (elementName == V_FILESYSTEM_FILESPEC)
			ListFromString(_listFiles ,xmlFile.GetChildData(), ';');

		else if (elementName == V_FILESYSTEM_FOLDERSPEC)
			ListFromString(_listFolders ,xmlFile.GetChildData(), ';');
	}

	// Step back out of 'File System Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}

//
//    ProcessRegistryKeysCollectionElementRead
//    ====================================
//
//    We have parsed the 'Hardware Collection' (S_COLLECTION_HARDWARE) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessRegistryKeysCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'Hardware Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		CString registryKey = xmlFile.GetChildData();

		if (elementName == "key")
		{
			_listRegistryKeys.Add(registryKey);
		}
	}

	// Step back out of 'Hardware Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}

//
//    ProcessMobileCollectionElementRead
//    ==================================
//
//    We have parsed the 'Mobile Collection' (S_COLLECTION_MOBILE) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessMobileCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'Mobile Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Indicate that Mobile Device Scanning is to be enabled
	_scanMDA = true;
	
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_MOBILE_FILESPEC)
			ListFromString(_listMobileFiles ,xmlFile.GetChildData(), ';');

		else if (elementName == V_MOBILE_SCANFILES)
			_scanMobileFiles = (eFileSetting)xmlFile.GetChildDataAsInt();
	}

	// Step back out of 'Mobile Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}



//
//    ProcessUsbCollectionElementRead
//    ===============================
//
//    We have parsed the 'USB Collection' (S_COLLECTION_USB) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessUsbCollectionElementRead(CMarkup xmlFile)
{
	// We need to step into the 'USB Collection' section as we are already reading as a child and
	// cannot read grandchildren
	xmlFile.IntoElem();

	// Indicate that USB Device Scanning is to be enabled
	_scanUSB = true;

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_USB_FILESPEC)
		{
			_listUSBFiles.Empty();
			ListFromString(_listUSBFiles ,xmlFile.GetChildData(), ';');
		}

		else if (elementName == V_USB_SCANFILES)
			_scanUSBFiles = (eFileSetting)xmlFile.GetChildDataAsInt();
	}

	// Step back out of 'USB Collection' so that the caller can continue to process other
	// 'Collection' elements
	xmlFile.OutOfElem();
}



// We have parsed the 'SerialNumberMappings' (S_APPLICATION_MAPPINGS) element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessApplicationMappingsElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(S_APPLICATION))
	{
		ProcessApplicationElementRead(xmlFile);
	}

	xmlFile.OutOfElem();
}



// We have parsed the 'Application' element within 'SerialNumberMappings' (S_APPLICATION_MAPPINGS) 
// so now parse the items within this section noting that we terminate parsing when we reach the end 
// of the section.
// 
void CAuditScannerConfiguration::ProcessApplicationElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// First try and read the name of the application
	if (xmlFile.FindChildElem(V_APPLICATION_NAME))
	{
		CString applicationName = xmlFile.GetChildData();
		CApplicationRegistryMapping applicationRegistryMapping(applicationName);

		// ...reset our position and loop through the registry key elements
		xmlFile.ResetChildPos();
	
		//
		while (xmlFile.FindChildElem(S_APPLICATION_REGISTRYKEY))	
		{
			xmlFile.IntoElem();
			CString registryKey = "";
			CString registryValue = "";

			// Try and pick up the registry key name
			if (xmlFile.FindChildElem(V_REGISTRYKEY_NAME))
				registryKey = xmlFile.GetChildData();

			// Try and pick up the registry value name
			if (xmlFile.FindChildElem(V_REGISTRYKEY_VALUE))
				registryValue = xmlFile.GetChildData();

			// If we have at least the registry key then add it as a mapping
			if (!registryKey.IsEmpty())
				applicationRegistryMapping.AddMapping(registryKey ,registryValue);

			// Out of the registry key/value
			xmlFile.OutOfElem();
		}

		// If we have at least an application name and 1 key then we should add this entry to
		// our internal list
		if (applicationRegistryMapping.GetMappings().GetCount() != 0)
			_listApplicationRegistryMappings.Add(applicationRegistryMapping);
	}

	xmlFile.OutOfElem();
}



//
//    ProcessPublisherMappingsElementRead
//    ===================================
//
//    We have parsed the 'PublisherMappings' (S_PUBLISHER_MAPPINGS) element so now parse the items 
//    within this section noting that we terminate parsing when we reach the end of the section.
//
void CAuditScannerConfiguration::ProcessPublisherMappingsElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(S_PUBLISHER))
	{
		ProcessPublisherElementRead(xmlFile);
	}
	xmlFile.OutOfElem();
}


// 
//    ProcessPublisherElementRead
//    ===========================
//
//    This function handles a 'Publisher' element within the 'PublisherMappings' section.
//
void CAuditScannerConfiguration::ProcessPublisherElementRead(CMarkup xmlFile)
{
	xmlFile.IntoElem();
	
	// We need to pick up both the publisher base name and it's alias
	if (xmlFile.FindChildElem(V_PUBLISHER_NAME))
	{
		CString name = xmlFile.GetChildData();
		//xmlFile.ResetPos();

		// Can we find the alias also?
		if (xmlFile.FindChildElem(V_PUBLISHER_ALIAS))
		{
			CString alias = xmlFile.GetChildData();
			// Add this mapping
			CPublisherMapping mapping(name ,alias);
			_publisherMappings.Add(mapping);
		}
	}

	// Out of this publisher so that we can pickup the next (if any)
	xmlFile.OutOfElem();
}



//
//    RationalizePublisherName
//    ========================
//
//    Return a rationalized publisher name given the base name.  This scans our list of publisher name
//    and alias mapping to see if the supplied name actually has an alias
//
CString	CAuditScannerConfiguration::RationalizePublisherName (CString& publisherName)
{
	return _publisherMappings.RationalizePublisherName(publisherName);
}


//
//    AlertMonitorEnabled
//    ===================
//
//    Return a flag to indicate whether or not Alert Monitor is enabled (for this asset)
//
//    Alert Monitor is disabled if the 'enabled' flag has not been set OR if the asset name appears
//    within the list of excluded assets.
//
BOOL	CAuditScannerConfiguration::AlertMonitorEnabled(CString& forAsset)
{
	CDynaList<CString> listExclusions;
	
	//ListFromString(listExclusions, _listMonitoredComputers ,';');

	return ((_enableAlertMonitor == TRUE) && (_listMonitoredComputers.Find(forAsset) != -1));
}