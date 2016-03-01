#pragma once

// Includes for objects referenced directly within this file
#include "InstalledPatches.h"
#include "ApplicationInstance.h"

class CFileSystemFolder;

// #defines for the different sections with the AuditDataFile
// String storage for sections and values in the XML audit Data File
#define S_AUDIT_FILE			"AuditDataFile"
//
#define S_GENERAL				"general"
#define V_GENERAL_VERSION		"scannerversion"
#define V_GENERAL_COMPUTER		"computername"			// Name of the computer in AW	
#define V_GENERAL_UNIQUEID		"computerid"			// Unique ID 
#define V_GENERAL_DOMAIN		"domain"				// Windows Domain / Workgroup
#define V_GENERAL_AUDITDATE		"auditdate"				// Date of audit
#define V_GENERAL_CATEGORY		"category"				// Asset category
#define V_GENERAL_MAKE			"make"					// Make assigned to asset
#define V_GENERAL_MODEL			"model"					// Model assigned to asset
#define V_GENERAL_SERIAL		"serial"				// Serial number for asset
#define V_GENERAL_MACADDRESS	"macaddress"			// Primary MAC address
#define V_GENERAL_IPADDRESS		"ipaddress"				// Primary IP address
#define V_GENERAL_LOCATION		"location"				// Selected location

// The 'User Defined Data' section
#define S_USERDATA				"userdata"
#define S_USERDATA_ITEM			"userdata_item"			// A user defined data item
#define V_USERDATA_CATEGORY		"category"				// Category of the user defined data field
#define V_USERDATA_NAME			"name"					// Name of the user defined data field
#define V_USERDATA_VALUE		"value"					// Value set for the suer defined data field

// The 'AuditData' details section - consists of a list of 'Audited Item' elements
#define S_AUDITED_ITEMS			"auditeditems"
#define S_AUDITED_ITEM			"auditeditem"			// An audited item
#define V_ITEM_CATEGORY			"category"				// Category of the item
#define V_ITEM_NAME				"name"					// Name of the item
#define V_ITEM_VALUE			"value"					// Value of the item
#define V_ITEM_UNITS			"displayunits"			// Units display text
#define V_ITEM_TYPE				"datatype"				// datatype
#define V_ITEM_HISTORIED		"historied"				// historied

// The 'Applications' details section  - consists of a list of 'application-instance' elements
#define S_APPLICATIONS			"applications"
#define S_APPLICATIONINSTANCE	"applicationinstance"
#define V_APPLICATION_NAME		"name"
#define V_APPLICATION_PUBLISHER "publisher"
#define V_APPLICATION_VERSION	"version"
#define V_APPLICATION_PRODUCTID "productid"
#define V_APPLICATION_CDKEY		"cdkey"

// The Operating System section appears within the 'Applications' section
#define S_OPERATING_SYSTEM		"operatingsystem"
#define V_OS_VERSION			"version"
#define V_OS_FAMILY				"family"
#define V_OS_SERIAL				"serial"
#define V_OS_CDKEY				"cdkey"
#define V_OS_IS64BIT			"is64bit"
#define V_OS_IEVERSION			"ieversion"

// The Installed Patches section appears within the 'Applications' section
#define S_PATCHES				"patches"
#define S_PATCH					"patch"
#define V_PATCH_PRODUCT			"product"
#define V_PATCH_NAME			"name"
#define V_PATCH_SERVICEPACK		"servicepack"
#define V_PATCH_DESCRIPTION		"description"
#define V_PATCH_INSTALLDATE		"installdate"
#define V_PATCH_INSTALLEDBY		"installedby"

// Hardware Section
#define S_HARDWARE				"hardware"	

// Internet Section
#define S_INTERNET_ITEMS		"internet-items"
#define S_INTERNET_ITEM			"internet-item"	
	
// File System
#define S_FILESYSTEM			"filesystem"
#define S_FOLDER				"folder"
#define V_FOLDER_NAME			"name"
#define S_FILE					"file"
#define V_FILE_NAME				"name"
#define V_FILE_SIZE				"size"
#define V_FILE_PUBLISHER		"pub"
#define V_FILE_DESCRIPTION		"desc"
#define V_FILE_PRODUCTNAME		"app"
#define V_FILE_PVERSION1		"pv1"
#define V_FILE_PVERSION2		"pv2"
#define V_FILE_FVERSION1		"fv1"
#define V_FILE_FVERSION2		"fv2"
#define V_FILE_MODIFIED			"mod"
#define V_FILE_LASTACCESSED		"la"
#define V_FILE_CREATED			"cre"
#define V_FILE_FILENAME			"fn"

// Forward References
class CFileSystemFolderList;
class CUserDataCategoryList;

//
//    CAuditDataFileItem
//    ==================
//
//    This class represents a single item within a CAuditDataFileCategory.  Each item may optionally have
//    one or more attributes.
//
class CAuditDataFileItem
{
public:
	// DataTypes
	enum eDATATYPE { text ,numeric ,date ,ipaddress };

public:
	CAuditDataFileItem(void)
	{}

	// Constructor for textual values
	CAuditDataFileItem(LPCSTR name, LPCSTR value, LPCSTR units="", eDATATYPE datatype=text ,BOOL historied=TRUE);

	// Constructor for numeric values
	CAuditDataFileItem(LPCSTR name, DWORD value, LPCSTR units="", eDATATYPE datatype=numeric ,BOOL historied=TRUE);

// Properties
public:
	//Name of the audited data item (for example 'Available Ram')
	CString Name()
	{ return _name; }
	void Name(CString value)
	{ _name = value; }

	// Value of the above named audited data item (for example '32')
	CString Value()
	{ return _value; }
	void Value(CString value)
	{ _value = value; }

	// Textual units representation displayed with the data value (for example 'MB')
	CString DisplayUnits()
	{ return _displayUnits; }
	void DisplayUnits(CString value)
	{ _displayUnits = value; }

	// Datatype of the value audited - see above for enumeration
	eDATATYPE Datatype()
	{ return _datatype; }
	void DataType (eDATATYPE value)
	{ _datatype = value; }

	// Is this individual item historied?
	BOOL	Historied()
	{ return _historied; }
	void	Historied(BOOL value)
	{ _historied = value; }

// Data
private:
	CString		_name;
	CString		_value;
	CString		_displayUnits;
	eDATATYPE	_datatype;
	BOOL		_historied;
};



//
// CAuditDataFileCategory
// ======================
//
// The CAuditDataFileCategory class encapsulates a single item within an AuditDataFile.
// This consists of the 'class' of the item ('Hardware\Display\Display Adapter #0\')
// and a list of the items which will appear within the Category - for example in the case
// of the above display adapter the items would be 'Name' ,Chipset' and 'Memory'.  Each of these
// items will in turn have their own attributes - namely value; units; type
//
class CAuditDataFileCategory
{
public:
	CAuditDataFileCategory(void)
	{ _name=""; _historied=TRUE; _grouped=FALSE; }

	CAuditDataFileCategory(LPCSTR name, BOOL historied=TRUE )
	{ _name = name; _historied=historied; _grouped=FALSE; }

	CAuditDataFileCategory(LPCSTR name, BOOL historied , BOOL grouped)
	{ _name = name; _historied=historied; _grouped=grouped; }

	CString& Name	()
	{ return _name; }

	void	Name	(LPCSTR value)
	{ _name = value; }

	BOOL	Historied ()
	{ return _historied; }
	void	Historied (BOOL value)
	{ _historied = value; }

	BOOL	Grouped ()
	{ return _grouped; }
	void	Grouped (BOOL value)
	{ _grouped = value; }

	// Add a new Item to this category
	int	AddItem (CAuditDataFileItem& item)
	{ return _listItems.Add(item); }

	// Return items list
	CDynaList<CAuditDataFileItem>* Items()
	{ return &_listItems; }
	
	// Return an audited data file item with the specified name
	CAuditDataFileItem*	FindAuditDataItem(LPCSTR name);

// Data
protected:
	CString	_name;
	BOOL	_historied;
	BOOL	_grouped;
	CDynaList<CAuditDataFileItem> _listItems;
};


/////////////////////////////////////////////////////////////////////////////////////
//
//    CAuditDataFile Class
//    ====================
//
//    This class implements the CAuditDataFile object which writes audited data file
//    to an XML format file.  
//
class CAuditDataFile
{
public:
	CAuditDataFile(void);
	~CAuditDataFile(void);

// Properties
public:
	CString Version ()
	{ return _version; }
	void	Version (CString& value)
	{ _version = value; }

	CTime CreationDate ()
	{ return _auditDate; }

	CString Computername ()
	{ return _computername; }
	void Computername (CString& value)
	{ _computername = value; }

	/*CString Netbios_name ()
	{ return _netbios_name; }
	void Netbios_name (CString& value)
	{ _netbios_name = value; }

	CString Newname ()
	{ return _newname; }
	void Newname (CString& value)
	{ _newname = value; }*/

	CString Uniqueid ()
	{ return _uniqueid; }
	void Uniqueid (CString& value)
	{ _uniqueid = value; }

	CTime	AuditDate ()
	{ return _auditDate; }
	void AuditDate (CTime& value)
	{ _auditDate = value; }

	CString Category ()
	{ return _category; }
	void Category (CString& value)
	{ _category = value; }

	CString Domain ()
	{ return _domain; }
	void Domain (CString& value)
	{ _domain = value; }

	CString Make ()
	{ return _make; }
	void Make (CString& value)
	{ _make = value; }

	CString Model ()
	{ return _model; }
	void Model (CString& value)
	{ _model = value; }

	CString Serial_number ()
	{ return _serial_number; }
	void Serial_number (CString& value)
	{ _serial_number = value; }

	CString AssetTag ()
	{ return _assetTag; }
	void AssetTag (CString& value)
	{ _assetTag = value; }

	CString Macaddress ()
	{ return _macaddress; }
	void Macaddress (CString& value)
	{ _macaddress = value; }

	CString Ipaddress ()
	{ return _ipaddress; }
	void Ipaddress (CString& value)
	{ _ipaddress = value; }

	CString Location ()
	{ return _location; }
	void Location (CString& value)
	{ _location = value; }

	// OS Information
	CString OSVersion	()
	{ return _osVersion; }
	void	OSVersion	(CString& value)
	{ _osVersion = value; }

	bool	OSIs64Bit()
	{ return _osIs64Bit; }
	void	OSIs64Bit	(bool value)
	{ _osIs64Bit = value; }

	CString OSFamily	()
	{ return _osFamily; }
	void	OSFamily (CString& value)
	{ _osFamily = value; }

	CString OSProductID	()
	{ return _osProductID; }
	void	OSProductID	(CString& value)
	{ _osProductID = value; }

	CString OSCDKey	()
	{ return _osCDKey; }
	void	OSCDKey	(CString& value)
	{ _osCDKey = value; }

	CString OSIEVersion	()
	{ return _osIEVersion; }
	void	OSIEVersion (CString& value)
	{ _osIEVersion = value; }

	// This is the list of applications audited on the local PC
	CDynaList<CApplicationInstance>* AuditedApplications()
	{ return &_listAuditedApplications; }

	// This is the list of Internet Items audited on the local PC
	CDynaList<CAuditDataFileCategory>* InternetItems()
	{ return &_listInternetItems; }
	

// Helper Methods
	// Add a new audited application to our list
	int	AddAuditedApplication (CApplicationInstance& auditedApplication)
	{ return _listAuditedApplications.Add(auditedApplication); }

	// Add a new installed patch to our list
	int	AddInstalledPatch (CInstalledPatch& patch)
	{ return _listInstalledPatches.Add(patch); }

	// Add a new Audited Item to our list
	int	AddAuditDataFileItem (CAuditDataFileCategory& item)
	{ return _listAuditedItems.Add(item); }

	// Add a new Audited Internet History Item to our list
	int	AddInternetItem (CAuditDataFileCategory& item)
	{ return _listInternetItems.Add(item); }

	// Retrieve the current writer object in use
	CMarkup&	GetWriter ()
	{ return *m_pWriter; }

	// Set the User Defined Data categories List
	void SetUserDataList (CUserDataCategoryList* pUserDataCategoryList)
	{ _pListUserDataCategories = pUserDataCategoryList; }

	// Set the FileSystem List
	void SetFileSystemList (CFileSystemFolderList* pFileSystemFolderList)
	{ _pFileSystemFolderList = pFileSystemFolderList; }

public:
	int  Write	(CString& fileName);
	int	 Load	(LPCSTR pszPath);
	void Clean	();
	
	// Return an audited data file category with the specified name
	CAuditDataFileCategory*	FindAuditDataCategory(LPCSTR category);
	

// XML File Writers
protected:
	void	WriteGeneral				();
	void	WriteUserDefinedDataItems	();
	void	WriteAuditedItems			();
	void	WriteInternetItems			();
	void	WriteAuditedApplications	();
	void	WriteAuditedItemAttributes	(CAuditDataFileItem* pItem);
	void	WriteFileSystem				();
	void	WriteFileSystemFolder		(CFileSystemFolder* pFolder);
	void	WriteFileSystemFiles		(CFileSystemFolder* pFolder);

// XML File Readers
protected:
	void	ProcessElementRead			(CMarkup xmlFile);
	void	ProcessGeneralElementRead	(CMarkup xmlFile);
	void	ProcessAuditedItems			(CMarkup xmlFile);
	void	ProcessAuditedItem			(CMarkup xmlFile);
	void	ProcessInternetItems		(CMarkup xmlFile);					// 8.3.4 - CMD
	void	ProcessInternetItem			(CMarkup xmlFile);					// 8.3.4 - CMD
	void	ProcessApplications			(CMarkup xmlFile);
	void	ProcessApplication			(CMarkup xmlFile);
	void	ProcessOSItems				(CMarkup xmlFile);

// Internal Data
private:
	CString _filename;

    // Version of the audit data file (read from the file or hardwired above if writing)</summary>
	CString _version;

	//
	// Data recovered from the General Section
	//
	CString _computername;					// Name of the computer as known to AW
	CString _netbios_name;					// NetBIOS name of the computere
	CString _newname;						// New name to be applied within AW
	CString _uniqueid;						// Unique identifier for this computer
	CTime	_auditDate;						// Date of the audit
	CString _category;						// Category of this asset
	CString _domain;						// Windows Domain or Workgroup for this computer
	CString _make;							// make of this computer
	CString _model;							// model of this computer
	CString _serial_number;					// Computer serial number
	CString _macaddress;					// Primary MAC address
	CString _ipaddress;						// Primary IP address
	CString _location;						// Computer location
	CString	_assetTag;

	// Audited Items (in categories)
	CDynaList<CAuditDataFileCategory>	_listAuditedItems;

	// Audited Internet History Items (in categories)
	CDynaList<CAuditDataFileCategory>	_listInternetItems;

	// Audited Software Applications
	CDynaList<CApplicationInstance>		_listAuditedApplications;

	// Audited System Patches
	CDynaList<CInstalledPatch>			_listInstalledPatches;

	// User Defined Data
	CUserDataCategoryList*				_pListUserDataCategories;

	// The File System
	CFileSystemFolderList*				_pFileSystemFolderList;

	// Operating System Information
	CString _osVersion;						// Version of the Operating System
	CString _osFamily;						// and its family
	CString	_osProductID;					// Product ID
	CString	_osCDKey;						// CD Key
	CString _osIEVersion;					// Internet Explorer Version
	bool	_osIs64Bit;						// 64 bit windows (or not)
	
	// The Markup File Writer
	CMarkup*	m_pWriter;

};
