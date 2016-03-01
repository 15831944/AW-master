#include "stdafx.h"

#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "FileSystemScanner.h"
#include "UserDataCategory.h"
#include "AuditDataFile.h"

//////////////////////////////////////////////////////////////////////////////////////////
//
//    CAuditDataFileItem
//    ==================
//
//    Class implementation to define a single audited item within the Audit Data File
//
CAuditDataFileItem::CAuditDataFileItem(LPCSTR name, LPCSTR value, LPCSTR units/*=""*/, eDATATYPE datatype/*=text*/ ,BOOL historied/*=TRUE*/)
{
	_name = name;
	_value = value;
	_displayUnits = units;
	_datatype = datatype;
	_historied = historied;
}

//
CAuditDataFileItem::CAuditDataFileItem(LPCSTR name, DWORD value, LPCSTR units/*=""*/, eDATATYPE datatype/*=numeric*/ ,BOOL historied/*=TRUE*/)
{
	CString strBuffer;
	strBuffer.Format("%d" ,value);
	_name = name;
	_value = strBuffer;
	_displayUnits = units;
	_datatype = datatype;
	_historied = historied;
}



//////////////////////////////////////////////////////////////////////////////////////////
//
//    CAuditDataFileCategory
//    ==================
//
//    Class implementation to define a category of audited data items within the Audit Data File
//


//
//    This function loops through the Audited Data File items and returns the first match 
//    to the supplied category name
//
CAuditDataFileItem*	CAuditDataFileCategory::FindAuditDataItem(LPCSTR name)
{
	for (int index = 0; index < (int)_listItems.GetCount(); index++)
	{
		if (_listItems[index].Name() == name)
			return &_listItems[index];
	}
	return NULL;
}


//////////////////////////////////////////////////////////////////////////////////////////
//
//    CAuditDataFile
//    ==============
//
//    Constructor for the CAuditDataFile class
//
CAuditDataFile::CAuditDataFile(void)
{
	m_pWriter = NULL;
	_pListUserDataCategories = NULL;
	_pFileSystemFolderList = NULL;
}

CAuditDataFile::~CAuditDataFile(void)
{
	if (m_pWriter != NULL)
		delete m_pWriter;
}



//
//    This function loops through the Audited Data File categories and returns the first match 
//    to the supplied category name
//
CAuditDataFileCategory*	CAuditDataFile::FindAuditDataCategory(LPCSTR category)
{
	for (int index = 0; index < (int)_listAuditedItems.GetCount(); index++)
	{
		if (_listAuditedItems[index].Name() == category)
			return &_listAuditedItems[index];
	}
	return NULL;
}


//////////////////////////////////////////////////////////////////////////////////////////
//
//    Functions to read an Audit data File - used maily for Alert Monitoring
//
//
//////////////////////////////////////////////////////////////////////////////////////////


//
// Clear
// =====
//
// This function is called prior to beginning to save this file to ensure that all existing contents
// are reset to their default, clean value
//
void CAuditDataFile::Clean()
{
	// Audited Items (in categories)
	_listAuditedItems.Empty();

	// Audited Internet History Items (in categories)
	_listInternetItems.Empty();

	// Audited Software Applications
	_listAuditedApplications.Empty();

	// Audited System Patches
	_listInstalledPatches.Empty();

	// User Defined Data (must delete also)
	delete _pListUserDataCategories;
	_pListUserDataCategories = NULL;

	// The File System (Must delete also)
	delete _pFileSystemFolderList;
	_pFileSystemFolderList = NULL;
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
int CAuditDataFile::Load(LPCSTR pszPath)
{
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

	// Load the XML buffer into our internal XML object
	CMarkup xmlFile;
	if (!xmlFile.SetDoc(csText))
		return -1;

	// Ok the file has now been read - let's process it
	xmlFile.ResetPos();

	// We must have the main Audit Data File section otherwise this is an invalid file
	if (!xmlFile.FindElem(S_AUDIT_FILE))
		return -1;

	// Process the file as we can be confident that it is an audit data file
	ProcessElementRead(xmlFile);
	return 0;
}



// 
//    ProcessElementRead
//    ==================
//
//    We have parsed the 'AuditDateFile' element so know that the XML file is an AuditWizard
//    Audit data and can now continue to parse the items within this section noting 
//    that we terminate parsing when we reach the end of the section.
//
void CAuditDataFile::ProcessElementRead(CMarkup xmlFile)
{
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_GENERAL)
			ProcessGeneralElementRead(xmlFile);

		else if (elementName == S_AUDITED_ITEMS)
			ProcessAuditedItems(xmlFile);

		else if (elementName == S_INTERNET_ITEMS)
			ProcessInternetItems(xmlFile);

		else if (elementName == S_APPLICATIONS)
			ProcessApplications(xmlFile);			
	}
}




// 
// We have parsed the 'General' element so now parse the items 
// within this section noting that we terminate parsing when we reach the end 
// of the section.
//
void CAuditDataFile::ProcessGeneralElementRead(CMarkup xmlFile)
{
	// Step into the 'General' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_GENERAL_COMPUTER)
			_computername = xmlFile.GetChildData();		
		/*else if (elementName == V_GENERAL_NETBIOS)
			_netbios_name = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_NEWCOMPUTER)
			_newname = xmlFile.GetChildData();*/
		else if (elementName == V_GENERAL_UNIQUEID)
			_uniqueid = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_CATEGORY)
			_category = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_DOMAIN)
			_domain = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_MAKE)
			_make = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_MODEL) 
			_model = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_SERIAL)
			_serial_number = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_MACADDRESS)
			_macaddress = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_IPADDRESS)
			_ipaddress = xmlFile.GetChildData();
		else if (elementName == V_GENERAL_LOCATION)
			_location = xmlFile.GetChildData();
	}

	xmlFile.OutOfElem();
}


//
//    ProcessOSItems
//    ===================
//
//    process the list of OS items held in the file
//
void CAuditDataFile::ProcessOSItems(CMarkup xmlFile)
{
	// Step into the 'Audited Items' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_OS_FAMILY)
			_osFamily = xmlFile.GetChildData();

		else if (elementName == V_OS_VERSION)
			_osVersion = xmlFile.GetChildData();

		else if (elementName == V_OS_SERIAL)
			_osProductID = xmlFile.GetChildData();

		else if (elementName == V_OS_CDKEY)
			_osCDKey = xmlFile.GetChildData();

		else if (elementName == V_OS_IS64BIT)
			_osIs64Bit = (xmlFile.GetChildData() == "true");

		else if (elementName == V_OS_IEVERSION)
			_osIEVersion = xmlFile.GetChildData();
	}

	// Out of section
	xmlFile.OutOfElem();
}


//
//    ProcessAuditedItems
//    ===================
//
//    process the list of audited items held in the file
//
void CAuditDataFile::ProcessAuditedItems(CMarkup xmlFile)
{
	// Step into the 'Audited Items' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == S_AUDITED_ITEM)
			ProcessAuditedItem(xmlFile);
	}

	// Out of 'Audited Items' section
	xmlFile.OutOfElem();
}



//
//    ProcessInternetItems
//    ===================
//
//    process the list of audited Internet items held in the file
//
void CAuditDataFile::ProcessInternetItems(CMarkup xmlFile)
{
	// Step into the 'Internet Items' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == S_INTERNET_ITEM)
			ProcessInternetItem(xmlFile);
	}

	// Out of 'Internet Items' section
	xmlFile.OutOfElem();
}



//
//    ProcessInternetItem
//    ==================
//
//    Process an 'Internet Item'
//
void CAuditDataFile::ProcessInternetItem(CMarkup xmlFile)
{
	// Create a new object to hold this item
	CAuditDataFileCategory newCategory; 
	
	// Check the child attributes first as each AuditedItem may have some
	newCategory.Name(xmlFile.GetChildAttrib("class"));

	// Now Step into the 'Internet Item' to get its children
	xmlFile.IntoElem();
	
	// Get the list of elements in this section - each is an Item
	while (xmlFile.FindChildElem(""))
	{
		CAuditDataFileItem item;
		//
		item.Name(xmlFile.GetChildAttrib(V_ITEM_NAME));			// Name attribute
		item.Value(xmlFile.GetChildAttrib(V_ITEM_VALUE));		// Value
		item.DisplayUnits(xmlFile.GetChildAttrib(V_ITEM_UNITS));// Display Units
		item.DataType((CAuditDataFileItem::eDATATYPE)xmlFile.GetChildAttribAsInt(V_ITEM_TYPE ,0));
		item.Historied(xmlFile.GetChildAttribAsBoolean(V_ITEM_HISTORIED , "true"));
		//
		newCategory.AddItem(item);
	}

	// Add the category to the list of Internet items
	_listInternetItems.Add(newCategory);
	
	// Step out of the 'Internet Item' again
	xmlFile.OutOfElem();
}



//
//    ProcessAuditedItem
//    ==================
//
//    Process an 'Audited Item'
//
void CAuditDataFile::ProcessAuditedItem(CMarkup xmlFile)
{
	// Create a new object to hold this item
	CAuditDataFileCategory newCategory; 
	
	// Check the child attributes first as each AuditedItem may have some
	newCategory.Name(xmlFile.GetChildAttrib("class"));
	newCategory.Historied(xmlFile.GetChildAttribAsBoolean("historied" ,true));
	newCategory.Grouped(xmlFile.GetChildAttribAsBoolean("grouped" ,false));

	// Now Step into the 'Audited Item' to get its children
	xmlFile.IntoElem();
	
	// Get the list of elements in this section - each is an AuditedItem
	while (xmlFile.FindChildElem(""))
	{
		CAuditDataFileItem item;
		//
		item.Name(xmlFile.GetChildAttrib(V_ITEM_NAME));			// Name attribute
		item.Value(xmlFile.GetChildAttrib(V_ITEM_VALUE));		// Value
		item.DisplayUnits(xmlFile.GetChildAttrib(V_ITEM_UNITS));// Display Units
		item.DataType((CAuditDataFileItem::eDATATYPE)xmlFile.GetChildAttribAsInt(V_ITEM_TYPE ,0));
		item.Historied(xmlFile.GetChildAttribAsBoolean(V_ITEM_HISTORIED , "true"));
		//
		newCategory.AddItem(item);
	}

	// Add the category to the list of audited items
	_listAuditedItems.Add(newCategory);
	
	// Step out of the 'Audited Item' again
	xmlFile.OutOfElem();
}





//
//    ProcessApplications
//    ===================
//
//    process the list of Applicationss held in the file
//
void CAuditDataFile::ProcessApplications(CMarkup xmlFile)
{
	// Step into the 'Applications' section
	xmlFile.IntoElem();

	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == S_APPLICATIONINSTANCE)
			ProcessApplication(xmlFile);
		if (elementName == S_OPERATING_SYSTEM)
			ProcessOSItems(xmlFile);
	}

	// Out of 'Applications' section
	xmlFile.OutOfElem();
}






//
//    ProcessApplication
//    ==================
//
//    Process an 'Application'
//
void CAuditDataFile::ProcessApplication(CMarkup xmlFile)
{
	// Now Step into the 'Application' to get its children
	xmlFile.IntoElem();

	// Create an application instanbce to hold the data we are to read
	CApplicationInstance newInstance;

	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();

		if (elementName == V_APPLICATION_NAME)
			newInstance.Name(xmlFile.GetChildData());	

		else if (elementName == V_APPLICATION_PUBLISHER)
			newInstance.Publisher(xmlFile.GetChildData());
			
		else if (elementName == V_APPLICATION_VERSION)
			newInstance.Version(xmlFile.GetChildData());

		else if (elementName == V_APPLICATION_PRODUCTID)
			newInstance.Serial().ProductId(xmlFile.GetChildData());
			
		else if (elementName == V_APPLICATION_CDKEY)
			newInstance.Serial().CdKey(xmlFile.GetChildData());
	}

	// Add the Application Instance
	_listAuditedApplications.Add(newInstance);
	
	// Close the applications section
	xmlFile.OutOfElem();
}



//////////////////////////////////////////////////////////////////////////////////////////
//
//    Functions to read an Audit data File - used maily for Alert Monitoring
//
//
//////////////////////////////////////////////////////////////////////////////////////////


//
//    Write
//    =====
//
//    Save the audit data file to the specified disk file
//
int CAuditDataFile::Write (CString& fileName)
{
	// Create the xmlFile output object
	m_pWriter = new CMarkup();
	m_pWriter->SetDoc("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" );
	//m_pWriter->SetDoc("<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n" );

	// Now the Scanner Configuration Section
	m_pWriter->AddElem(S_AUDIT_FILE);

	// Add the General Section as a child
	WriteGeneral();

	// Write the User Defined Data Section
	WriteUserDefinedDataItems();
	
	// Write the 'Audited Items' section
	WriteAuditedItems();

	// Write the 'Internet History' section
	WriteInternetItems();

	// Write 'Audited Applications' section
	WriteAuditedApplications();

	// Write 'File System' section
	WriteFileSystem();

	// ...force the CMarkup object to actually write the data to disk
	CString data = m_pWriter->GetDoc();
	m_pWriter->Save(fileName ,data);

	// Delete the m_pWriter-> again
	delete m_pWriter;
	m_pWriter = NULL;

	// return success
	return 0;
}



//
//    WriteGeneral
//    ============
//
//    Write the 'General' section of the Audit Data File
//
void CAuditDataFile::WriteGeneral()
{
	m_pWriter->AddChildElem(S_GENERAL);		// ...add a 'General' section
	m_pWriter->IntoElem();
	//
	m_pWriter->AddChildElem(V_GENERAL_VERSION, _version);
	m_pWriter->AddChildElem(V_GENERAL_COMPUTER, _computername);
	/*m_pWriter->AddChildElem(V_GENERAL_NETBIOS, _netbios_name);
	m_pWriter->AddChildElem(V_GENERAL_NEWCOMPUTER, _newname);*/
	m_pWriter->AddChildElem(V_GENERAL_UNIQUEID, _uniqueid);
	m_pWriter->AddChildElem(V_GENERAL_CATEGORY, _category);
	CString dateTimeString;
	dateTimeString = _auditDate.Format("%Y-%m-%d %H:%M:%S");
	m_pWriter->AddChildElem(V_GENERAL_AUDITDATE ,dateTimeString);
	m_pWriter->AddChildElem(V_GENERAL_DOMAIN, _domain);
	m_pWriter->AddChildElem(V_GENERAL_MAKE, _make);
	m_pWriter->AddChildElem(V_GENERAL_MODEL, _model);
	m_pWriter->AddChildElem(V_GENERAL_SERIAL, _serial_number);
	m_pWriter->AddChildElem("assettag", _assetTag);
	m_pWriter->AddChildElem(V_GENERAL_MACADDRESS, _macaddress);
	m_pWriter->AddChildElem(V_GENERAL_IPADDRESS, _ipaddress);
	m_pWriter->AddChildElem(V_GENERAL_LOCATION, _location);	
	//
	m_pWriter->OutOfElem();

	return;
}


//
//    WriteUserDefinedDataItems
//    =========================
//
//    Write the 'User Defined Data' section of the Audit Data File
//    This will result in the following XML
//
//		<userdata>
//
//			<userdata_item category="Warranty Information" field="Period" value="12 Months" />
//			<userdata_item category="Warranty Information" field="Provider" value="Acme Inc." />
//			<userdata_item category="Purchasing Information" field="Date of Purchase" value="12/02/09" />
//
//		</userdata>
//
void CAuditDataFile::WriteUserDefinedDataItems()
{
	// If we have no user defined data then skip this
	if (_pListUserDataCategories == NULL)
			return;

	// Add the 'User Defined Data' category to the XML and step into it
	m_pWriter->AddChildElem(S_USERDATA);
	m_pWriter->IntoElem();

	// Iterate through the Categories
	for (int index=0; index<_pListUserDataCategories->GetCount(); index++)
	{
		CUserDataCategory* pCategory = &_pListUserDataCategories->ListCategories()[index];
		
		// Now iterate through the fields
		for (int fieldIndex = 0; fieldIndex < pCategory->GetCount(); fieldIndex++)
		{
			CUserDataField* pField = &(pCategory->ListDataFields())[fieldIndex];
		
			// ...start a section for this field and write its value
			m_pWriter->AddChildElem(S_USERDATA_ITEM);

			// Add the category, fieldname and value as attributes
			m_pWriter->AddChildAttrib(V_USERDATA_CATEGORY ,pCategory->Name());
			m_pWriter->AddChildAttrib(V_USERDATA_NAME ,pField->Label());
			m_pWriter->AddChildAttrib(V_USERDATA_VALUE ,pField->CurrentValue());
		}
	}
	
	// Close the 'UserData' section
	m_pWriter->OutOfElem();
}



//
//    WriteAuditedItems
//    =================
//
//    Write the 'AuditedItems' section of the Audit Data File
//    This will result in the following XML
//
//		<audited-items>
//	
//			<audited-item class="Hardware\Memory">
//				<item name="total-ram" value="128" units="MB" type="numeric" />
//				<item name="available-ram" value="128" units="MB" type="numeric" historied="false" />
//				<item name="total-pagefile" value="128" units="MB" type="numeric"  historied="false"/>
//			</audited-item>
//
//			<audited-item class="Hardware\Display\Display Adapter #0">
//				<item name="name" value="ATI 3D" units="" type="text" />
//				<item name="chipset" value="ATI" units="" type="text" />
//				<item name="memory" value="32" units="MB" type="numeric" />
//			</audited-item>
//
//			<audited-item class="Hardware\Display\Display Adapter #1">
//				<item name="name" value="S3" units="" type="text" />
//				<item name="chipset" value="S3 3D" units="" type="text" />
//				<item name="memory" value="256" units="MB" type="numeric" />
//			</audited-item>
//
//		</audited-items>
//
//    Note that the items are actually stored within CAuditDataFileCategory objects.  The category identifies the
//    class of item, each item has a name and a list of attributes - the attributes in the above examples are 
//	  value, units and type.
//
void CAuditDataFile::WriteAuditedItems()
{
	CString value;

	// Add the 'Audited Items' category to the XML and step into it
	m_pWriter->AddChildElem(S_AUDITED_ITEMS);
	m_pWriter->IntoElem();

	for (int isub=0; isub < (int)_listAuditedItems.GetCount(); isub++)
	{
		CAuditDataFileCategory* pCategory = &_listAuditedItems[isub];
		m_pWriter->AddChildElem(S_AUDITED_ITEM);

		// Add the class as an attribute of the <audited-item>
		m_pWriter->AddChildAttrib("class" ,pCategory->Name());

		// Add the historied attribute for the category (if not set - default is yes)
		if (!pCategory->Historied())
			m_pWriter->AddChildAttrib("historied", "false");

		// Add the GROUPED attribute for the category (if set - default is no)
		if (pCategory->Grouped())
			m_pWriter->AddChildAttrib("grouped", "true");

		// Step into the <audited-item>
		m_pWriter->IntoElem();
	
		// Add the child items
		CDynaList<CAuditDataFileItem>* pListItems = pCategory->Items();
		for (int jsub=0; jsub < (int)pListItems->GetCount(); jsub++)
		{
			CAuditDataFileItem* pItem = &(*pListItems)[jsub];

			// Add the item beneath the category
			m_pWriter->AddChildElem("item");
			
			// Name is the first attribute
			m_pWriter->AddChildAttrib(V_ITEM_NAME, pItem->Name());
			
			/// ...then add the attributes for the item to the item
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pItem->Value());

			// Add Display units if specified
			if (!pItem->DisplayUnits().IsEmpty())
				m_pWriter->AddChildAttrib(V_ITEM_UNITS , pItem->DisplayUnits());
			
			// Add datatype if not the default (0)
			if ((int)pItem->Datatype() != 0)
			{
				int datatype = (int)pItem->Datatype();
				value.Format("%d" ,datatype);
				m_pWriter->AddChildAttrib(V_ITEM_TYPE ,value);
			}

			// Disable history if requested on an item by item basis
			if (!pItem->Historied())
				m_pWriter->AddChildAttrib(V_ITEM_HISTORIED , "false");
		}

		// Step out of the audited item now
		m_pWriter->OutOfElem();
	}


	// Now write the Patches element - this actually goes into the AuditedItems and will be displayed beneath 
	// the 'System' branch
	if (_listInstalledPatches.GetCount() != 0)
	{
		// and iterate through the Patches
		for (DWORD dw=0; dw<_listInstalledPatches.GetCount(); dw++)
		{
			// Get this patch instance
			CInstalledPatch* pPatch = &_listInstalledPatches[dw];
			
			// Declare each as an 'Audited Item'
			m_pWriter->AddChildElem(S_AUDITED_ITEM);

			// Format the class for this patch which will consist of 'System|Patches' and the name of the
			// product and patch
			CString itemClass;
			itemClass.Format("System|Patches|%s|%s", pPatch->Product() ,pPatch->Name());
			
			// Set the class/category name
			m_pWriter->AddChildAttrib("class" ,itemClass);
			m_pWriter->AddChildAttrib("grouped", "true");

			// Step into the <audited-item> patch so that we can add its sub-attributes - note these are
			// added as items in their own right but will be grouped when displayed
			m_pWriter->IntoElem();
			
			// Add the item beneath the category
			m_pWriter->AddChildElem("item");
			m_pWriter->AddChildAttrib(V_ITEM_NAME, V_PATCH_SERVICEPACK);
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pPatch->ServicePack());
			//
			m_pWriter->AddChildElem("item");
			m_pWriter->AddChildAttrib(V_ITEM_NAME, V_PATCH_DESCRIPTION);
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pPatch->Description());
			//
			m_pWriter->AddChildElem("item");
			m_pWriter->AddChildAttrib(V_ITEM_NAME, V_PATCH_INSTALLDATE);
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pPatch->InstallDate());
			//
			m_pWriter->AddChildElem("item");
			m_pWriter->AddChildAttrib(V_ITEM_NAME, V_PATCH_INSTALLEDBY);
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pPatch->Installer());

			// Out of the <audited-item> element
			m_pWriter->OutOfElem();
		}

		// Out of the 'Patches' element
		m_pWriter->OutOfElem();
	}

	
	// Close the <audited-items> section
	m_pWriter->OutOfElem();
}





//
//    WriteInternetItems
//    ==================
//
//    Write the 'Internet History' section of the Audit Data File
//    This will result in the following XML
//
//		<internet>
//			<internet-item class="Internet|History|www.ebay.co.uk historied="false" ,grouped="true">
//				<item name="Date" value="07/01/2009" />
//				<item name="Number of Pages" value="5" />
//			</internet-item>
//
//			<internet-item class="Internet|History|www.ebay.co.uk historied="false" ,grouped="true">
//				<item name="Date" value="07/01/2009" />
//				<item name="Number of Pages" value="5" />
//			</internet-item>
//
//			<internet-item class="Internet|History|www.ebay.co.uk historied="false" ,grouped="true">
//				<item name="Date" value="07/01/2009" />
//				<item name="Number of Pages" value="5" />
//			</internet-item>
//
//		</internet>
//
//    Note that the items are actually stored within CAuditDataFileCategory objects.  The category identifies the
//    class of item, each item has a name and a list of attributes - the attributes in the above examples are 
//	  value, units and type.
//
void CAuditDataFile::WriteInternetItems()
{
	CString value;

	// Add the 'Internet-Items' category to the XML and step into it
	m_pWriter->AddChildElem(S_INTERNET_ITEMS);
	m_pWriter->IntoElem();

	for (int isub=0; isub < (int)_listInternetItems.GetCount(); isub++)
	{
		CAuditDataFileCategory* pCategory = &_listInternetItems[isub];
		m_pWriter->AddChildElem(S_INTERNET_ITEM);

		// Add the class as an attribute of the <audited-item>
		m_pWriter->AddChildAttrib("class" ,pCategory->Name());

		// Add the historied attribute for the category (if not set - default is yes)
		if (!pCategory->Historied())
			m_pWriter->AddChildAttrib("historied", "false");

		// Add the GROUPED attribute for the category (if set - default is no)
		if (pCategory->Grouped())
			m_pWriter->AddChildAttrib("grouped", "true");

		// Step into the <audited-item>
		m_pWriter->IntoElem();
	
		// Add the child items
		CDynaList<CAuditDataFileItem>* pListItems = pCategory->Items();
		for (int jsub=0; jsub < (int)pListItems->GetCount(); jsub++)
		{
			CAuditDataFileItem* pItem = &(*pListItems)[jsub];

			// Add the item beneath the category
			m_pWriter->AddChildElem("item");
			
			// Name is the first attribute
			m_pWriter->AddChildAttrib(V_ITEM_NAME, pItem->Name());
			
			/// ...then add the attributes for the item to the item
			m_pWriter->AddChildAttrib(V_ITEM_VALUE, pItem->Value());

			// Add Display units if specified
			if (!pItem->DisplayUnits().IsEmpty())
				m_pWriter->AddChildAttrib(V_ITEM_UNITS , pItem->DisplayUnits());
			
			// Add datatype if not the default (0)
			if ((int)pItem->Datatype() != 0)
			{
				int datatype = (int)pItem->Datatype();
				value.Format("%d" ,datatype);
				m_pWriter->AddChildAttrib(V_ITEM_TYPE ,value);
			}
		}

		// Step out of the audited item now
		m_pWriter->OutOfElem();
	}
	
	// Close the <audited-items> section
	m_pWriter->OutOfElem();
}



//
//    WriteAuditedApplications
//    ========================
//
//    Write the 'AuditedApplications' section of the Audit Data File
//
void CAuditDataFile::WriteAuditedApplications()
{
	m_pWriter->AddChildElem(S_APPLICATIONS);
	m_pWriter->IntoElem();

	if (_listAuditedApplications.GetCount() != 0)
	{
		// and iterate through the audited applications
		for (DWORD dw=0; dw<_listAuditedApplications.GetCount(); dw++)
		{
			// Each application sits in its own child element
			CApplicationInstance* pInstance = &_listAuditedApplications[dw];
			m_pWriter->AddChildElem(S_APPLICATIONINSTANCE);
			m_pWriter->IntoElem();
			m_pWriter->AddChildElem(V_APPLICATION_NAME, pInstance->Name());
			m_pWriter->AddChildElem(V_APPLICATION_PUBLISHER, pInstance->Publisher());
			m_pWriter->AddChildElem(V_APPLICATION_VERSION, pInstance->Version());
			m_pWriter->AddChildElem(V_APPLICATION_PRODUCTID ,pInstance->Serial().ProductId());
			m_pWriter->AddChildElem(V_APPLICATION_CDKEY ,pInstance->Serial().CdKey());
			m_pWriter->OutOfElem();
		}
	}

	// Now write out the OS element which is part of installed applications if we have the information
	if (_osVersion != "" || _osFamily != "")
	{
		m_pWriter->AddChildElem(S_OPERATING_SYSTEM);
		m_pWriter->IntoElem();
		m_pWriter->AddChildElem(V_OS_FAMILY ,_osFamily);
		m_pWriter->AddChildElem(V_OS_VERSION ,_osVersion);
		m_pWriter->AddChildElem(V_OS_SERIAL ,_osProductID);
		m_pWriter->AddChildElem(V_OS_CDKEY ,_osCDKey);
		m_pWriter->AddChildElem(V_OS_IS64BIT ,(_osIs64Bit) ? "true" : "false");
		m_pWriter->AddChildElem(V_OS_IEVERSION ,_osIEVersion);
		m_pWriter->OutOfElem();
	}

	// Close the applications section
	m_pWriter->OutOfElem();
}






//
//    WriteFileSystem
//    ===============
//
//    Write the 'File System' section of the Audit Data File
//    This will result in the following XML
//
//		<filesystem>
//	
//			<folder name="C:\Program Files">
//				<file name="setup.exe" size="32554" created="1002334" modified="322323" lastmodified="2323233"
//					lastaccessed="3232323" publisher="Microsoft Corporation, Inc" ,product="xyyxxx" 
//					pversion1="2.2.11" ,pversion2="2.2.11.2332" ,aversion1="2.2" ,aversion2="2.2"
//					fullfilename="c:\temp\setup.exe" />
//
//				<file name="setup.exe" size="32554" created="1002334" modified="322323" lastmodified="2323233"
//					lastaccessed="3232323" publisher="Microsoft Corporation, Inc" ,product="xyyxxx" 
//					pversion1="2.2.11" ,pversion2="2.2.11.2332" ,aversion1="2.2" ,aversion2="2.2"
//					fullfilename="c:\temp\setup.exe" />
//			</folder>
//
void CAuditDataFile::WriteFileSystem()
{
	CString value;

	// return if the file system was not audited
	if (_pFileSystemFolderList == NULL)
		return;
		
	// Add the 'File System' category to the XML and step into it
	m_pWriter->AddChildElem(S_FILESYSTEM);
	m_pWriter->IntoElem();

	// Now iterate through the TOP LEVEL folders scanned - each folder may have child folders and files
	CDynaList<CFileSystemFolder*>& fileSystemFolders = _pFileSystemFolderList->ListAuditedFolders();
	// 
	for (int folderIndex=0; folderIndex < (int)fileSystemFolders.GetCount(); folderIndex++)
	{
		// Get the file system folder object
		CFileSystemFolder* pFolder = fileSystemFolders[folderIndex];
		
		// Are there any child files in this folder?  If not skip the folder
		if (!pFolder->HasFiles())
			continue;
		
		// Introduce it in the XML file
		m_pWriter->AddChildElem(S_FOLDER);

		// Add the class as an attribute of the folder
		m_pWriter->AddChildAttrib(V_FOLDER_NAME ,pFolder->Name());

		// ...and step into the folder element to add the files
		m_pWriter->IntoElem();
		
		// Does this folder have any child folders, if so process these first
		for (int childFolderIndex = 0; childFolderIndex < (int)pFolder->ListFolders().GetCount(); childFolderIndex++)
		{
			CFileSystemFolder* pChildFolder = pFolder->ListFolders()[childFolderIndex];
			WriteFileSystemFolder(pChildFolder);
		}
				
		// Each folder may have 0 or more files so write these also as children of the folder
		WriteFileSystemFiles(pFolder);

		// Step out of the FOLDER now
		m_pWriter->OutOfElem();
	}
	
	// Close the <FILE SYSTEM> section
	m_pWriter->OutOfElem();
}



//
//    WriteFileSystemFolder
//    =====================
//
//    Called to (recursively) write the contents of the Folder to the audit data file
//
void CAuditDataFile::WriteFileSystemFolder(CFileSystemFolder* pFolder)
{
	// Are there any child files in this folder?  If not skip the folder
	if (!pFolder->HasFiles())
		return;
		
	// Introduce it in the XML file
	m_pWriter->AddChildElem(S_FOLDER);

	// Add the class as an attribute of the folder
	m_pWriter->AddChildAttrib(V_FOLDER_NAME ,pFolder->ShortName());

	// ...and step into the folder element to add the files
	m_pWriter->IntoElem();
		
	// Does this folder have any child folders, if so process these first
	for (int childFolderIndex = 0; childFolderIndex < (int)pFolder->ListFolders().GetCount(); childFolderIndex++)
	{
		CFileSystemFolder* pChildFolder = pFolder->ListFolders()[childFolderIndex];
		WriteFileSystemFolder(pChildFolder);
	}
				
	// Each folder may have 0 or more files so write these also as children of the folder
	WriteFileSystemFiles(pFolder);

	// Step out of the FOLDER now
	m_pWriter->OutOfElem();
}



void CAuditDataFile::WriteFileSystemFiles(CFileSystemFolder* pFolder)
{
	CString value;
	
	// Iterate through (any) files defined for this folder
	for (int fileIndex = 0; fileIndex < (int)pFolder->ListFiles().GetCount(); fileIndex++)
	{
		CFileSystemFile* pFile = pFolder->ListFiles()[fileIndex];
		
		// Introduce it in the XML file
		m_pWriter->AddChildElem(S_FILE);

		// Add its attributes - Name is the first attribute
		m_pWriter->AddChildAttrib(V_ITEM_NAME, pFile->Name());
		m_pWriter->AddChildAttrib(V_FILE_PUBLISHER, pFile->CompanyName());
		m_pWriter->AddChildAttrib(V_FILE_DESCRIPTION, pFile->Description());
		m_pWriter->AddChildAttrib(V_FILE_PRODUCTNAME, pFile->ProductName());
		m_pWriter->AddChildAttrib(V_FILE_PVERSION1, pFile->ProductVersion1());
		m_pWriter->AddChildAttrib(V_FILE_PVERSION2, pFile->ProductVersion2());
		m_pWriter->AddChildAttrib(V_FILE_FVERSION1, pFile->FileVersion1());
		m_pWriter->AddChildAttrib(V_FILE_FVERSION2, pFile->FileVersion2());
		m_pWriter->AddChildAttrib(V_FILE_FILENAME, pFile->OriginalFileName());
		m_pWriter->AddChildAttrib(V_FILE_MODIFIED, pFile->ModifiedDateTime());
		m_pWriter->AddChildAttrib(V_FILE_LASTACCESSED, pFile->LastAccessedDateTime());
		m_pWriter->AddChildAttrib(V_FILE_CREATED, pFile->CreatedDateTime());
		//
		value.Format("%d" ,pFile->Size());
		m_pWriter->AddChildAttrib(V_FILE_SIZE, value);
	}
}