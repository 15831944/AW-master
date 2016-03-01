#pragma once

#include "PublisherMappings.h"
#include "ApplicationRegistryMapping.h"
#include "Picklists.h"
#include "UserDataCategory.h"

//
//   CAuditLocation class
//
//    This class defines a single location parsed from the configuration file
//
class CAuditLocation
{
public:
	CAuditLocation()
	{ _name=""; _id=0; _parentid=1; }
	virtual ~CAuditLocation()
	{}
	
public:
	CString&	Name (void)
	{ return _name; }
	void		Name (CString& value)
	{ _name = value; }
	//
	int			ParentID (void)
	{ return _parentid; }
	void		ParentID(int value)
	{ _parentid = value; }
	//
	int			ID (void)
	{ return _id; }
	void		ID(int value)
	{ _id = value; }
	
private:
	CString	_name;
	int		_parentid;
	int		_id;	
};






//
//    CAuditScannerConfiguration Class
//    ================================
//
class CAuditScannerConfiguration
{
public:
	CAuditScannerConfiguration();
	virtual ~CAuditScannerConfiguration()
	{}

public:
	// This enumeration defines how the results of the audit will be saved
	enum	eUploadSetting		{ network ,ftp ,tcp ,email };
	
	// this enumeration defines how scanning of folders is to behave
	enum	eFolderSetting		{ noFolders, allFolders, specifiedFolders };

	// This enumeration defines how scanning of files is to behave
	enum	eFileSetting		{ noFiles, allExeFiles, specifiedFiles, allFiles };

	// This enumeration defines which screens are going to be available during an interactive audit
	enum	eScreens			{ BasicInfo=1, Location=2, AssetData=4 };
	
	// operating modes
	enum	eAuditMode			{ modeNonInteractive, modeInteractive, modeFirstTimeInteractive };

	// Enabling of individual data collection fields on page 1 of scanner
	enum	eFields				{ AssetName=0x01, Make=0x02, Model=0x04, Category=0x08 ,SerialNumber=0x10, Cancel=0x20 ,AddLocation=0x40};

public:
	// Load the configuration file from the specified disk file
	int		Load	(LPCSTR pszPath);

	// Save the Audit Configuration file
	int		Save	(LPCSTR pszPath = NULL);


public:
	// Upload Settings
	eUploadSetting UploadSetting()
	{ return _uploadSetting; }
	void UploadSetting(eUploadSetting value)
	{ _uploadSetting = value; }

	bool DeployPathSingle()
	{ return _deployPathSingle; }
	void DeployPathSingle(bool value)
	{ _deployPathSingle = value; }

	CString DeployPathRoot()
	{ return _deployPathRoot; }
	void DeployPathRoot(CString& value)
	{
		_deployPathRoot = value;
		if (_deployPathSingle)
		{
			_deployPathData = MakePathName(_deployPathRoot, "data");
			_deployPathScanner = MakePathName(_deployPathRoot, "scanner");
		}
	}

	CString DeployPathScanner()
	{ return _deployPathScanner; }
	void DeployPathScanner(CString& value)
	{ _deployPathScanner = value; }

	CString DeployPathData()
	{ return _deployPathData; }
	void DeployPathData(CString& value)
	{ _deployPathData = value; }

	CString EmailAddress()
	{ return _emailAddress; }
	void EmailAddress (CString& value)
	{ _emailAddress = value; }

	// General Settings
	CString AuditWizardServer()
	{ return _AuditWizardServer; }
	void AuditWizardServer (CString& value)
	{ _AuditWizardServer = value; }

	bool AutoName()
	{ return _autoName; }
	void AutoName(bool value)
	{ _autoName = value; }

	eAuditMode ScannerMode()
	{ return _scannerMode; }
	void ScannerMode(eAuditMode value)
	{ _scannerMode = value; }

	bool Hidden()
	{ return _hidden; }
	void Hidden(bool value)
	{ _hidden = value; }

	int ReAuditInterval()
	{ return _reAuditInterval; }
	void ReAuditInterval(int value)
	{ _reAuditInterval = value; }

	int ScreensMask()
	{ return _screensMask; }
	void ScreensMask(int value)
	{ _screensMask = value; }

	int FieldsMask()
	{ return _fieldsMask; }
	void FieldsMask(int value)
	{ _fieldsMask = value; }

	// Alert Monitor Settings
	BOOL AlertMonitorEnabled(CString& forAsset);
	void AlertMonitorEnabled(bool value)
	{ _enableAlertMonitor = value; }
	BOOL IsAlertMonitorEnabled()
	{ return _enableAlertMonitor; }
	CDynaList<CString> AlertMonitoredAssets()
	{ return _listMonitoredComputers; }

	bool AlertTrayIcon()
	{ return _alertTrayIcon; }
	void AlertTrayIcon(bool value)
	{ _alertTrayIcon = value; }

	int AlertUploadInterval()
	{ return _alertUploadInterval; }
	void AlertUploadInterval(int value)
	{ _alertUploadInterval = value; }

	int AlertInterval()
	{ return _alertInterval; }
	void AlertInterval(int value)
	{ _alertInterval = value; }

	CString& AlertExclusions()
	{ return _alertExclusions; }
	void AlertExclusions(CString& value)
	{ _alertExclusions = value; }

	int SettingsInterval()
	{ return _settingsInterval; }
	void SettingsInterval(int value)
	{ _settingsInterval = value; }

	// Collection (Hardware) Settings
	bool HardwareScan()
	{ return _hardwareScan; }
	void HardwareScan(bool value)
	{ _hardwareScan = value; }

	CDynaList<CString>& ListRegistryKeys()
	{ return _listRegistryKeys; }

	CDynaList<CString>& ListMonitoredComputers()
	{ return _listMonitoredComputers; }

	bool HardwareSystem()
	{ return _hardwareSystem; }
	void HardwareSystem(bool value)
	{ _hardwareSystem = value; }

	bool HardwareNetworkDrives()
	{ return _hardwareNetworkDrives; }
	void HardwareNetworkDrives(bool value)
	{ _hardwareNetworkDrives = value; }

	bool HardwarePhysicalDisks()
	{ return _hardwarePhysicalDisks; }
	void HardwarePhysicalDisks(bool value)
	{ _hardwarePhysicalDisks = value; } 

	bool HardwareSettings()
	{ return _hardwareSettings; }
	void HardwareSettings(bool value)
	{ _hardwareSettings = value; } 

	bool HardwareSecurity()
	{ return _hardwareSecurity; }
	void HardwareSecurity(bool value)
	{ _hardwareSecurity = value; } 

	bool HardwareActiveSystem()
	{ return _hardwareActiveSystem; }
	void HardwareActiveSystem(bool value)
	{ _hardwareActiveSystem = value; } 

	bool RegistryScan()
	{ return _registryScan; }
	void RegistryScan(bool value)
	{ _registryScan = value; }

	// Collection (Software) Settings
	bool SoftwareScan()
	{ return _softwareScan; }
	void SoftwareScan(bool value)
	{ _softwareScan = value; }

	bool SoftwareScanApplications()
	{ return _softwareScanApplications; }
	void SoftwareScanApplications(bool value)
	{ _softwareScanApplications = value; }

	bool SoftwareScanOs()
	{ return _softwareScanOs; }
	void SoftwareScanOs(bool value)
	{ _softwareScanOs = value; }

	eFolderSetting ScanFolders()
	{ return _scanFolders; }
	void ScanFolders(eFolderSetting value)
	{ _scanFolders = value; }

	CDynaList<CString>& ListFolders()
	{ return _listFolders; }

	eFileSetting ScanFiles()
	{ return _scanFiles; }
	void ScanFiles(eFileSetting value)
	{ _scanFiles = value; }

	CDynaList<CString>& ListFiles()
	{ return _listFiles; }

	bool ScanFileSystem()
	{ return _scanFileSystem; }
	void ScanFileSystem(bool value)
	{ _scanFileSystem = value; }

	bool CaptureFiles()
	{ return _captureFiles; }
	void CaptureFiles(bool value)
	{ _captureFiles = value; }

	CDynaList<CString>& ListCapture()
	{ return _listCapture; }

	// Collection (Mobile Device) Settings
	bool ScanMDA()
	{ return _scanMDA; }
	void ScanMDA(bool value)
	{ _scanMDA = value; }

	// Collection (USB Device) Settings
	bool ScanUSB()
	{ return _scanUSB; }
	void ScanUSB(bool value)
	{ _scanUSB = value; }

	eFileSetting ScanMobileFiles()
	{ return _scanMobileFiles; }
	void ScanMobileFiles(eFileSetting value)
	{ _scanMobileFiles = value; }

	CDynaList<CString>& ListMobileFiles()
	{ return _listMobileFiles; }

	eFileSetting ScanUSBFiles()
	{ return _scanUSBFiles; }
	void ScanUSBFiles(eFileSetting value)
	{ _scanUSBFiles = value; }

	CDynaList<CString>& ListUSBFiles()
	{ return _listUSBFiles; }

	// Collection (Internet Explorer) Settings
	bool IEScan()
	{ return _IEScan; }
	void IEScan(bool value)
	{ _IEScan = value; }

	bool IEHistory()
	{ return _IEHistory; }
	void IEHistory(bool value)
	{ _IEHistory = value; }

	bool IECookies()
	{ return _IECookies; }
	void IECookies(bool value)
	{ _IECookies = value; }

	bool IEDetails()
	{ return _IEDetails; }
	void IEDetails(bool value)
	{ _IEDetails = value; }

	int IEDays()
	{ return _IEDays; }
	void IEDays(int value)
	{ _IEDays = value; }

	CString FTPType()
	{ return _FTPType; }
	void FTPType(CString& value)
	{ _FTPType = value; }

	bool FTPAnonymous()
	{ return _FTPAnonymous; }
	void FTPAnonymous(bool value)
	{ _FTPAnonymous = value; }

	int FTPPort()
	{ return _FTPPort; }
	void FTPPort(int value)
	{ _FTPPort = value; }

	CString FTPUsername()
	{ return _FTPUser; }
	void FTPUsername(CString& value)
	{ _FTPUser = value; }

	CString FTPPassword()
	{ return _FTPPassword; }
	void FTPPassword(CString& value)
	{ _FTPPassword = value; }

	CString FTPSite()
	{ return _FTPSite; }
	void FTPSite(CString& value)
	{ _FTPSite = value; }

	CString FTPDefDir()
	{ return _FTPDefDir; }
	void FTPDefDir(CString& value)
	{ _FTPDefDir = value; }

	CString FTPTypeBackup()
	{ return _FTPTypeBackup; }
	void FTPTypeBackup(CString& value)
	{ _FTPTypeBackup = value; }

	bool FTPAnonymousBackup()
	{ return _FTPAnonymousBackup; }
	void FTPAnonymousBackup(bool value)
	{ _FTPAnonymousBackup = value; }

	int FTPPortBackup()
	{ return _FTPPortBackup; }
	void FTPPortBackup(int value)
	{ _FTPPortBackup = value; }

	CString FTPUsernameBackup()
	{ return _FTPUserBackup; }
	void FTPUsernameBackup(CString& value)
	{ _FTPUserBackup = value; }

	CString FTPPasswordBackup()
	{ return _FTPPasswordBackup; }
	void FTPPasswordBackup(CString& value)
	{ _FTPPasswordBackup = value; }

	CString FTPSiteBackup()
	{ return _FTPSiteBackup; }
	void FTPSiteBackup(CString& value)
	{ _FTPSiteBackup = value; }

	CString FTPDefDirBackup()
	{ return _FTPDefDirBackup; }
	void FTPDefDirBackup(CString& value)
	{ _FTPDefDirBackup = value; }
	
	CDynaList<CAuditLocation>& Locations()
	{ return _listLocations; }

	CString RootLocation()
	{ return _rootLocation; }
	void RootLocation(CString& value)
	{ _rootLocation = value; }
	
	CDynaList<CString>& AssetTypes()
	{ return _listAssetTypes; }

	CDynaList<CApplicationRegistryMapping>& ApplicationRegistryMappings ()
	{ return _listApplicationRegistryMappings; }

	CUserDataCategoryList& UserDataCategories()
	{ return _listUserDataCategories; }

	CDynaList<CPicklist>& Picklists()
	{ return _listPicklists; }

	// section for items added by JML that are needed but are missing
	bool AutoUpload()
	{ return _autoUpload; }
	void AutoUpload(bool value)
	{ _autoUpload = value; }

	bool FTPCopyToNetwork()
	{ return _ftpCopyToNetwork; }
	void FTPCopyToNetwork(bool value)
	{ _ftpCopyToNetwork = value; }


	// Return the internal version string
	CString Version()
	{ return _version; }

	// Return the state of the Shutdown flag
	bool	Shutdown()
	{ return _shutdown; }

	bool InteractiveAllowCancel()
	{ return _interactiveAllowCancel; }
	void InteractiveAllowCancel(bool value)
	{ _interactiveAllowCancel = value; }

	bool DisplayLocationScreen()
	{ return _displayLocationScreen; }
	void DisplayLocationScreen(bool value)
	{ _displayLocationScreen = value; }

	bool DisplayBasicInformationScreen()
	{ return _displayBasicInformationScreen; }
	void DisplayBasicInformationScreen(bool value)
	{ _displayBasicInformationScreen = value; }

	bool DisplayUserDataScreen()
	{ return _displayUserDataScreen; }
	void DisplayUserDataScreen(bool value)
	{ _displayUserDataScreen = value; }

	bool InteractiveAllowChangeCategory()
	{ return _interactiveAllowChangeCategory; }
	void InteractiveAllowChangeCategory(bool value)
	{ _interactiveAllowChangeCategory = value; }

	bool InteractiveAllowChangeManufacturer()
	{ return _interactiveAllowChangeManufacturer; }
	void InteractiveAllowChangeManufacturer(bool value)
	{ _interactiveAllowChangeManufacturer = value; }

	bool InteractiveAllowChangeSerial()
	{ return _interactiveAllowChangeSerial; }
	void InteractiveAllowChangeSerial(bool value)
	{ _interactiveAllowChangeSerial = value; }

	bool InteractiveAllowChangeModel()
	{ return _interactiveAllowChangeModel; }
	void InteractiveAllowChangeModel(bool value)
	{ _interactiveAllowChangeModel = value; }

	bool InteractiveAllowAddLocation()
	{ return _interactiveAllowAddLocation; }
	void InteractiveAllowAddLocation(bool value)
	{ _interactiveAllowAddLocation = value; }

public:
	// Return a rationalized publisher name
	CString	RationalizePublisherName (CString& publisherName);

	// return the picklist with the specified name if any
	CPicklist*	GetPicklist(CString& name)
	{
		for (int index=0; index < (int)_listPicklists.GetCount(); index++)
		{
			if ((_listPicklists[index]).Name() == name)
				return &_listPicklists[index];
		}
		return NULL;	
	}
	
	
	// Internal Writer Functions
protected:
	// Initialize default values for the scanner confiuration settings
	void	InitializeDefaults					();
	void	DeserializeScannerConfiguration		(CMarkup xmlFile);
	void	ProcessElementRead					(CMarkup xmlFile);
	void	ProcessGeneralElementRead			(CMarkup xmlFile);
	void	ProcessUploadElementRead			(CMarkup xmlFile);
	void	ProcessUploadFtpElementRead			(CMarkup xmlFile);
	void	ProcessUploadFtpAnonymousRead		(CMarkup xmlFile);
	void	ProcessAlertMonitorElementRead		(CMarkup xmlFile);
	void	ProcessInteractiveElementRead		(CMarkup xmlFile);
	void	ProcessCollectionElementRead		(CMarkup xmlFile);
	void	ProcessHardwareCollectionElementRead(CMarkup xmlFile);
	void	ProcessInternetCollectionElementRead(CMarkup xmlFile);
	void	ProcessSoftwareCollectionElementRead (CMarkup xmlFile);
	void	ProcessFileSystemElementRead		(CMarkup xmlFile);
	void	ProcessRegistryKeysCollectionElementRead(CMarkup xmlFile);
	void	ProcessMobileCollectionElementRead (CMarkup xmlFile);
	void	ProcessUsbCollectionElementRead		(CMarkup xmlFile);
	void	ProcessApplicationMappingsElementRead	(CMarkup xmlFile);
	void	ProcessApplicationElementRead		(CMarkup xmlFile);
	void	ProcessPublisherMappingsElementRead	(CMarkup xmlFile);
	void	ProcessPublisherElementRead			(CMarkup xmlFile);
	void	ProcessInterCategoriesElementRead	(CMarkup xmlFile);
	void	ProcessInterLocationsElementRead	(CMarkup xmlFile);
	void	ProcessInterLocationElementRead		(CMarkup xmlFile);
	void	ProcessUserDataCategories			(CMarkup xmlFile);
	void	ProcessUserDataCategory				(CMarkup xmlFile);
	void	ProcessUserDataField				(CMarkup xmlFile ,CUserDataCategory& category);
	void	ProcessPicklists					(CMarkup xmlFile);
	void	ProcessPicklist						(CMarkup xmlFile);
	void	ProcessPickitem						(CMarkup xmlFile ,CPicklist& picklist);

protected:
	// Internal data 
	CString				_tempFolder;			// Windows temporary folder
	CString				_author;				// Author of the file
	CString				_version;				// Version read from file

	// Scanner settings - general
	bool				_autoName;				// TRUE if using Computer Name as Asset Name
	eAuditMode			_scannerMode;			// Scanner operating mode
	bool				_hidden;				// TRUE if scanner runs invisibly
	int					_reAuditInterval;		// number of days between audits
	int					_screensMask;			// Mask for enabled screens
	int					_fieldsMask;			// Mask for enabled fields
	bool				_shutdown;				// Control Function - shutdown flag
	CString				_AuditWizardServer;		// The AuditWizard Server HostName

	// Scanner Settings - Upload
	eUploadSetting		_uploadSetting;			// How data will be saved 
	CString				_emailAddress;			// Email address to save to
	bool				_deployPathSingle;		// TRUE if specifying a simplified root path for the scanner
	CString				_deployPathRoot;		// Scanner ROOT folder 
	CString				_deployPathScanner;		// Path to store scanner
	CString				_deployPathData;		// data path to store scanner results (usually same as upload path above)
			
	// FTP Upload options
	CString				_FTPType;				// FTP or SFTP
	bool				_FTPAnonymous;			// FTP Site allows anonymous access
	int					_FTPPort;				// FTP Port to use
	CString				_FTPUser;				// Username for FTP access if not anonymous
	CString				_FTPPassword;			// Password for FTP access if not anonymous
	CString				_FTPSite;				// FTP Site name/address
	CString				_FTPDefDir;				// Default Directory for FTP uploads

	CString				_FTPTypeBackup;				// FTP or SFTP (Backup)
	bool				_FTPAnonymousBackup;			// FTP Site allows anonymous access (Backup)
	int					_FTPPortBackup;				// FTP Port to use (Backup)
	CString				_FTPUserBackup;				// Username for FTP access if not anonymous (Backup)
	CString				_FTPPasswordBackup;			// Password for FTP access if not anonymous (Backup)
	CString				_FTPSiteBackup;				// FTP Site name/address (Backup)
	CString				_FTPDefDirBackup;				// Default Directory for FTP uploads (Backup)
	
	// Alert Monitor Settings
	bool				_enableAlertMonitor;	// TRUE if Alert Monitor features are ON
	bool				_alertTrayIcon;			// TRUE if Alert Monitor should run in the system tray
	int					_alertUploadInterval;	// number of seconds between manager checking for uploads
	int					_alertInterval;			// number of seconds between scanner checking for upload changes
	int					_settingsInterval;		// number of seconds between scanner checking for config changes
	CString				_alertExclusions;		// List of PCs to be excluded from Alert Monitor operations
	CDynaList<CString> _listMonitoredComputers;

	// Scanner settings - Hardware
	bool				_hardwareScan;			// TRUE if we are to scan hardware
	bool				_hardwareSystem;		// true if we are to audit system and environmental data
	bool				_hardwareNetworkDrives;	// true if we are to audit network drives
	bool				_hardwarePhysicalDisks;
	bool				_hardwareSettings;
	bool				_hardwareSecurity;
	bool				_hardwareActiveSystem;
	bool				_registryScan;
	CDynaList<CString> _listRegistryKeys;		//A list of registry keys that should be scanned
	
	// Scanner settings - Software
	bool				_softwareScan;			// TRUE if we are to scan software
	bool				_softwareScanApplications;	// TRUE if we are to scan installed applications
	bool				_softwareScanOs;		// TRUE if we are to scan the Operating System

	// List of folders that are to be scanned
	eFileSetting		_scanFiles;				// see enum above
	CDynaList<CString>	_listFiles;
	
	// List of files to scan for
	bool				_captureFiles;			// TRUE if we are to capture any file contents
	CDynaList<CString>	_listCapture;
		
	// Mobile and USB device settings
	bool				_scanMDA;				// TRUE if we are to audit Mobile devices
	bool				_scanUSB;				// TRUE if we are to audit USB devices
	eFileSetting		_scanMobileFiles;		// see enum above
	CDynaList<CString>	_listMobileFiles;		// List of files
	eFileSetting		_scanUSBFiles;			// see enum above
	CDynaList<CString>	_listUSBFiles;			// List of files

	// Scanner settings - Internet Explorer
	bool				_IEScan;				// True if we are to scan Internet activity
	bool				_IEHistory;				// TRUE if we are to audit Internet History
	bool				_IECookies;				// TRUE if we are to audit Internet cookies
	bool				_IEDetails;				// TRUE if we are to perform a detailed Internet scan
	int					_IEDays;				// Limit scans to entries in the last n days

	// Scanner settings - File System Scan
	bool				_scanFileSystem;
	eFolderSetting		_scanFolders;			// see enum above
	CDynaList<CString>	_listFolders;

	// new values added by JML
	bool				_autoUpload;
	bool				_ftpCopyToNetwork;

	bool				_interactiveAllowCancel;
	bool				_displayLocationScreen;
	bool				_displayBasicInformationScreen;
	bool				_displayUserDataScreen;
	bool				_interactiveAllowChangeCategory;
	bool				_interactiveAllowChangeManufacturer;
	bool				_interactiveAllowChangeModel;
	bool				_interactiveAllowAddLocation;
	bool				_interactiveAllowChangeSerial;
	CString				_rootLocation;
	
	// Location Structure
	CDynaList<CAuditLocation>	_listLocations;
	
	// Asset Types (categories)
	CDynaList<CString>	_listAssetTypes;

	// Application Registry Key Mappings
	CDynaList<CApplicationRegistryMapping> _listApplicationRegistryMappings;

	// Publisher Mappings
	CPublisherMappings	_publisherMappings;
	
	// List of User Defined Data Categories
	CUserDataCategoryList	_listUserDataCategories;
	
	// List of Picklists defined
	CDynaList<CPicklist>	_listPicklists;
};