using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    /// <summary>
    /// This class encapsulates an AuditWizard Scanner Configuration.  This is used as the 
    /// main interface between the User Console and the Audit Scanner itself.  The file is
    /// maintained in an XML format.
    /// 
    /// The following is a sample of the file format
    /// 
    /// </summary>
    ///
    [Serializable]
    public class AuditScannerDefinition
    {
        #region Enumerations

        public enum eUploadSetting { network, ftp, removablemedia, tcp, email };

        /// <summary>this enumeration defines how scanning of folders is to behave</summary>
        public enum eFolderSetting { noFolders, allFolders, specifiedFolders };

        /// <summary> This enumeration defines how scanning of files is to behave</summary>
        public enum eFileSetting { noFiles, allExeFiles, specifiedFiles, allFiles };

        /// <summary> This enumeration defines which screens are going to be available during an interactive audit</summary>
        public enum eScreens { BasicInfo = 1, Location = 2, AssetData = 4 };
        public static int ALLSCREENS = (int)eScreens.BasicInfo | (int)eScreens.Location | (int)eScreens.AssetData;

        /// <summary> operating modes</summary>
        public enum eAuditMode { modeNonInteractive, modeInteractive, modeFirstTimeInteractive };

        /// <summary> Enabling of individual data collection fields on page 1 of scanner</summary>
        public enum eFields { AssetName = 0x01, Make = 0x02, Model = 0x04, Category = 0x08, SerialNumber = 0x10, Cancel = 0x20, AddLocation = 0x40 };
        public static int ALLFIELDS = (int)eFields.AssetName | (int)eFields.Make | (int)eFields.Model | (int)eFields.Category | (int)eFields.SerialNumber | (int)eFields.Cancel | (int)eFields.AddLocation;

        /// <summary>Access required</summary>
        public enum ACCESS { read, write };
        #endregion Enumerations

        #region Data

        #region Static strings for various scanner definition attributes

        public static string _scannerExecutable = "AuditScanner.exe";
        public static string _scannerConfiguration = "AuditScanner.xml";
        public static string _scannerZip = "AuditScanner.zip";
        public static string _scannerSfx = "AuditScannerSfx.exe";

        #endregion Static strings for various scanner definition attributes

        private string _author = "";
        private string _description = "Default Scanner Configuration";

        /// <summary>This is the ID of the scanner in the database</summary>
        private int _scannerID = 0;

        /// <summary>This is the name of the scanner</summary>
        private string _scannerName;

        // This flag indicates whether or not the file is valid
        private bool _isValidFile = true;

        // This is the filename for the current scanner configuration
        private string _filename;

        /// <summary>Version of the audit scanner configuration file (read from the file or hardwired above if writing)</summary>
        private string _version;

        private string _uniqueAssetID;

        /// <summary>This is the name of the computer on which the AuditWizard uploader server is located</summary>
        private string _server;

        /// <summary>This field is set after we detect a 'significant' change to the scanner which would
        /// mean that we should deploy it to anyone who has a copy.  The flag is set internally 
        /// when-ever a significant field is updated but it is up to the user to clear the flag when
        /// the change has been handled.
        /// </summary>
        private bool _needsDeploying;

        /// <summary>Flag to request that the scanner shutdown when running in AlertMonitor mode</summary>
        private bool _shutdown;

        /// <summary>
        /// The applications definitions file is read as part of this object as it contains 
        /// both registry mappings and publisher aliases both of which form part of the scanner 
        /// configuration file.
        /// </summary>
        ApplicationDefinitionsFile _applicationDefinitionsFile = new ApplicationDefinitionsFile();

        private List<string> _listScannerFiles = new List<string>();

        /// <summary>
        /// This is a Memory Stream which may be populated for later saving to say the database
        /// </summary>
        MemoryStream _xmlMemoryStream = null;
        private string _tempFolder;		// Windows temporary folder

        /// <summary>
        /// List of files that are required for USB / Mobile Device Scanning
        /// </summary>
        private static string _USBFiles = "LyncUSB.exe";
        private static string _MDAFiles = "LyncAL.exe,LyncAS.exe,LyncBB.exe,LyncHS.dll,LyncPalmApp.prc,LyncPN.dll,LyncPPC.dll,LyncReg.exe,LyncInst.dll,LyncSP.exe,LyncSym.exe,SCRuntimeSetup22.exe";


        #region Configuration Data

        // Scanner settings - general
        private bool _deployPathSingle;				// TRUE if specifying a simplified root path for the scanner
        private string _deployPathRoot;				// Scanner ROOT folder 
        private string _deployPathScanner;				// Path to store scanner
        private string _deployPathData;				// data path to store scanner results (usually same as upload path above)
        private bool _autoName;						// TRUE if using Asset Name as Asset Name
        private eAuditMode _scannerMode;				// Scanner operating mode
        private bool _hidden;						// TRUE if scanner runs invisibly
        private int _reAuditInterval;				// number of days between audits

        // Upload options
        private eUploadSetting _uploadSetting;			// How audit results will be uploaded

        // Interactive Screen Design Fields
        private bool _interAllowCancel;					// Allow the user to cancel an audit
        private bool _interDisplayAssetInformation;		// Display the asset information screen
        private bool _interEnableAssetCategory;			// Allow data entry into Asset Information -> Category
        private bool _interEnableAssetSerial;			// Allow data entry into Asset Information -> Serial
        private bool _interEnableAssetMake;				// Allow data entry into Asset Information -> Make
        private bool _interEnableAssetModel;			// Allow data entry into Asset Information -> Model
        private bool _interDisplayLocations;			// Display the Asset Location screen
        private bool _interEnableAddLocation;			// Allow the user to specify a new location
        private bool _interDisplayAssetData;			// Display the asset data field screen(s)

        private string _interCategories;
        private AssetGroupList _interLocations = new AssetGroupList();
        private string _rootLocation;
        //private InteractiveAssetGroupList _interLocations = new InteractiveAssetGroupList();
        private List<InteractivePickList> _interPicklists = new List<InteractivePickList>();
        private List<UserDataCategories> _interUserDataCategories = new List<UserDataCategories>();

        // Scanner settings - Hardware
        private bool _hardwareScan;						// TRUE if we are to scan hardware
        private bool _hardwareActiveSystem;				// true if we are to audit active system settings
        private bool _hardwareSettings;					// true if we are to audit system settings (environment etc)
        private bool _hardwareNetworkDrives;			// true if we are to audit network drives
        private bool _hardwarePhysicalDisks;			// true if we are to audit physical disks
        private bool _hardwareSecurity;					// true if we are to audit security information

        // Scanner settings - Software
		private bool _softwareScan;						// TRUE if we are to scan SOFTWARE
		private bool _softwareScanApplications;			// TRUE if we are to scan installed applications
		private bool _softwareScanOs;					// TRUE if we are to scan the Operating System
        private eFolderSetting _scanFolders;			// see enum above
        List<String> _listFolders = new List<string>();

        // Scanner settings - FileSystem
        private bool _filesystemScan;					// TRUE if we are to scan the file system

        // Scanner settings - Windows Registry
        private bool _registryScan;						// TRUE if we are to scan Windows Registry Information

        // A list of registry keys that should be scanned
        List<string> _listRegistryKeys = new List<string>();

        // List of folders that are to be scanned
        private eFileSetting _scanFiles;				// see enum above
        List<String> _listFiles = new List<string>();

        // List of files to scan for
        private bool _captureFiles;			// TRUE if we are to capture any file contents
        List<string> _listCapture = new List<string>();

        // List of files to capture

        // Mobile and USB device settings
        private bool _scanMDA;				// TRUE if we are to audit Mobile devices
        private bool _scanUSB;				// TRUE if we are to audit USB devices
        private eFileSetting _scanMobileFiles;		// see enum above
        List<String> _listMobileFiles = new List<string>();

        // List of file to scan for on mobile devices
        private eFileSetting _scanUSBFiles;			// see enum above
        private List<string> _listUSBFiles = new List<string>();
        // List of files to scan for on USB devices

        // Scanner settings - Internet Explorer
        private bool _IEScan;					// True if we are to scan Internet activity
        private bool _IEHistory;				// TRUE if we are to audit Internet History
        private bool _IECookies;				// TRUE if we are to audit Internet cookies
        private bool _IEDetails;				// TRUE if we are to perform a detailed scan
        private int _IEDays;					// Number of days to limit results to

        // FTP Upload options
        private bool _FTPCopyToNetwork;			// Copy audit files to network also        

        private string _FTPTypeBackup;
        private bool _FTPAnonymousBackup;		// FTP Site allows anonymous access
        private int _FTPPortBackup;				// FTP Port to use
        private string _FTPUserBackup;			// Username for FTP access if not anonymous
        private string _FTPPasswordBackup;		// Password for FTP access if not anonymous
        private string _FTPSiteBackup;			// FTP Site name/address
        private string _FTPDefDirBackup;		// Default Directory for FTP uploads

        private string _FTPType;
        private bool _FTPAnonymous;				// FTP Site allows anonymous access
        private int _FTPPort;					// FTP Port to use
        private string _FTPUser;				// Username for FTP access if not anonymous
        private string _FTPPassword;			// Password for FTP access if not anonymous
        private string _FTPSite;				// FTP Site name/address
        private string _FTPDefDir;				// Default Directory for FTP uploads

        // Email Upload options
        private string _emailAddress;			// Email address to upload to

        // AutoUpload Flag
        private bool _autoUpload;			// Enable auto-upload

        private bool _testMode = false;

        // AlertMonitor Settings
        private bool _alertMonitorEnabled;
        private bool _alertMonitorDisplayTray;
        private int _alertMonitorSettingSecs;
        private int _alertMonitorAlertsSecs;
        private AlertDefinition _alertMonitorDefinition = new AlertDefinition();

        // Publisher Mappings
        private List<SerialNumberMapping> _serialNumberMappings = new List<SerialNumberMapping>();

        #endregion Scanner Configuration Data

        #endregion Data

        #region Properties
        /// <summary>Return whether this is a valid scanner configuration file</summary>
        public bool IsValidFile
        { get { return _isValidFile; } }

        public string ScannerName
        {
            get { return _scannerName; }
            set
            {
                _scannerName = value;
                _filename = Path.Combine(Application.StartupPath, @"scanners\" + _scannerName + ".xml");
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// TRUE if a singnificant change has taken place in the scanner configuration requiring
        /// the distribution of the updated file, false otherwise
        /// </summary>
        public bool NeedsDeploying
        {
            get { return _needsDeploying; }
            set { _needsDeploying = value; }
        }

        /// <summary>
        /// TRUE if we should request the scanner to shut down, false normally
        /// </summary>
        public bool Shutdown
        {
            get { return _shutdown; }
            set { _shutdown = value; }
        }

        /// <summary>
        /// Base Filename of the scanner configuration file
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public bool RegistryScan
        {
            get { return _registryScan; }
            set { _needsDeploying = _needsDeploying | (_registryScan != value); _registryScan = value; }
        }

        public bool HardwareScan
        {
            get { return _hardwareScan; }
            set { _needsDeploying = _needsDeploying | (_hardwareScan != value); _hardwareScan = value; }
        }

        public bool IEScan
        {
            get { return _IEScan; }
            set { _needsDeploying = _needsDeploying | (_IEScan != value); _IEScan = value; }
        }

        public string ScannerConfigurationFile
        {
            get
            {
                string scannerConfigurationFile = Path.Combine(DeployPathScanner, _scannerConfiguration);
                return scannerConfigurationFile;
            }
        }

        /// <summary>
        /// This is a very important function as it creates the memory stream containing the latest
        /// settings for this scanner definition.
        /// </summary>
        public MemoryStream XmlMemoryStream
        {
            get { return _xmlMemoryStream; }
        }


        #region General Auditing Settings

        public bool DeployPathSingle
        {
            get { return _deployPathSingle; }
            set { _deployPathSingle = value; }
        }

        public string DeployPathRoot
        {
            get { return _deployPathRoot; }
            set
            {
                _needsDeploying = _needsDeploying | (_deployPathRoot != value);
                _deployPathRoot = value;
                _deployPathSingle = true;
                _deployPathData = Path.Combine(_deployPathRoot, "data");
                _deployPathScanner = Path.Combine(_deployPathRoot, "scanner");
            }
        }

        public string DeployPathScanner
        {
            get { return _deployPathScanner; }
            set { _deployPathScanner = value; }
        }

        public string DeployPathData
        {
            get { return _deployPathData; }
            set
            {
                _needsDeploying = _needsDeploying | (_deployPathData != value);
                _deployPathData = value;
                if (_deployPathData == "")
                    DeployPathSingle = true;
            }
        }

        public bool AutoName
        {
            get { return _autoName; }
            set { _needsDeploying = _needsDeploying | (_autoName != value); _autoName = value; }
        }

        public eAuditMode ScannerMode
        {
            get { return _scannerMode; }
            set { _needsDeploying = _needsDeploying | (_scannerMode != value); _scannerMode = value; }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set { _needsDeploying = _needsDeploying | (_hidden != value); _hidden = value; }
        }

        public int ReAuditInterval
        {
            get { return _reAuditInterval; }
            set { _needsDeploying = _needsDeploying | (_reAuditInterval != value); _reAuditInterval = value; }
        }
        #endregion General Auditing Settings

        #region Interactive Screen Design Fields

        public bool InteractiveAllowCancel
        {
            get { return _interAllowCancel; }
            set { _needsDeploying = _needsDeploying | (_interAllowCancel != value); _interAllowCancel = value; }
        }

        public bool InteractiveDisplayAssetInformation
        {
            get { return _interDisplayAssetInformation; }
            set { _needsDeploying = _needsDeploying | (_interDisplayAssetInformation != value); _interDisplayAssetInformation = value; }
        }

        public bool InteractiveEnableAssetCategory
        {
            get { return _interEnableAssetCategory; }
            set { _needsDeploying = _needsDeploying | (_interEnableAssetCategory != value); _interEnableAssetCategory = value; }
        }

        public bool InteractiveEnableAssetSerial
        {
            get { return _interEnableAssetSerial; }
            set { _interEnableAssetSerial = value; }
        }

        public bool InteractiveEnableAssetMake
        {
            get { return _interEnableAssetMake; }
            set { _needsDeploying = _needsDeploying | (_interEnableAssetMake != value); _interEnableAssetMake = value; }
        }

        public bool InteractiveEnableAssetModel
        {
            get { return _interEnableAssetModel; }
            set { _needsDeploying = _needsDeploying | (_interEnableAssetModel != value); _interEnableAssetModel = value; }
        }

        public bool InteractiveDisplayLocations
        {
            get { return _interDisplayLocations; }
            set { _needsDeploying = _needsDeploying | (_interDisplayLocations != value); _interDisplayLocations = value; }
        }

        public bool InteractiveEnableAddLocation
        {
            get { return _interEnableAddLocation; }
            set { _needsDeploying = _needsDeploying | (_interEnableAddLocation != value); _interEnableAddLocation = value; }
        }

        public bool InteractiveDisplayAssetData
        {
            get { return _interDisplayAssetData; }
            set { _needsDeploying = _needsDeploying | (_interDisplayAssetData != value); _interDisplayAssetData = value; }
        }

        public string InteractiveCategories
        {
            get { return _interCategories; }
            set { _interCategories = value; }
        }

        public string RootLocation
        {
            get { return _rootLocation; }
            set { _rootLocation = value; }
        }

        public AssetGroupList InteractiveLocations
        {
            get { return _interLocations; }
            set { _interLocations = value; }
        }

        //public InteractiveAssetGroupList InteractiveLocations
        //{
        //    get { return _interLocations; }
        //    set { _interLocations = value; }
        //}

        public List<InteractivePickList> InteractivePicklists
        {
            get { return _interPicklists; }
            set { _interPicklists = value; }
        }

        public List<UserDataCategories> InteractiveUserDataCategories
        {
            get { return _interUserDataCategories; }
            set { _interUserDataCategories = value; }
        }

        #endregion

        #region Scanner Settings Hardware Fields

        public List<String> ListRegistryKeys
        {
            get { return _listRegistryKeys; }
            set { _listRegistryKeys = value; }
        }
        public bool HardwareSystem
        {
            get { return _hardwareActiveSystem; }
            set { _needsDeploying = _needsDeploying | (_hardwareActiveSystem != value); _hardwareActiveSystem = value; }
        }

        public bool HardwareNetworkDrives
        {
            get { return _hardwareNetworkDrives; }
            set { _needsDeploying = _needsDeploying | (_hardwareNetworkDrives != value); _hardwareNetworkDrives = value; }
        }

        public bool HardwarePhysicalDisks
        {
            get { return _hardwarePhysicalDisks; }
            set { _needsDeploying = _needsDeploying | (_hardwarePhysicalDisks != value); _hardwarePhysicalDisks = value; }
        }

        public bool HardwareSettings
        {
            get { return _hardwareSettings; }
            set { _needsDeploying = _needsDeploying | (_hardwareSettings != value); _hardwareSettings = value; }
        }

        public bool HardwareSecurity
        {
            get { return _hardwareSecurity; }
            set { _needsDeploying = _needsDeploying | (_hardwareSecurity != value); _hardwareSecurity = value; }
        }

        public bool HardwareActiveSystem
        {
            get { return _hardwareActiveSystem; }
            set { _needsDeploying = _needsDeploying | (_hardwareActiveSystem != value); _hardwareActiveSystem = value; }
        }



        #endregion Scanner Settings Hardware Fields

        #region Scanner Settings Software Fields

		public bool SoftwareScan
		{
			get { return _softwareScan; }
			set { _needsDeploying = _needsDeploying | (_softwareScan != value); _softwareScan = value; }
		}

		public bool SoftwareScanApplications
		{
			get { return _softwareScanApplications; }
			set { _needsDeploying = _needsDeploying | (_softwareScanApplications != value); _softwareScanApplications = value; }
		}

        public bool SoftwareScanOs
        {
            get { return _softwareScanOs; }
            set { _needsDeploying = _needsDeploying | (_softwareScanOs != value); _softwareScanOs = value; }
        }

        public eFolderSetting ScanFolders
        {
            get { return _scanFolders; }
            set { _scanFolders = value; }
        }

        public List<String> ListFolders
        {
            get { return _listFolders; }
            set { _listFolders = value; }
        }

        public eFileSetting ScanFiles
        {
            get { return _scanFiles; }
            set { _scanFiles = value; }
        }

        public List<String> ListFiles
        {
            get { return _listFiles; }
            set { _listFiles = value; }
        }

        public bool CaptureFiles
        {
            get { return _captureFiles; }
            set { _captureFiles = value; }
        }

        public List<String> ListCapture
        {
            get { return _listCapture; }
            set { _listCapture = value; }
        }

        public bool FileSystemScan
        {
            get { return _filesystemScan; }
            set { _needsDeploying = _needsDeploying | (_filesystemScan != value); _filesystemScan = value; }
        }

        #endregion Scanner Settings Hardware Fields

        #region USB / PDA Auditing Fields
        public bool ScanMDA
        {
            get { return _scanMDA; }
            set { _needsDeploying = _needsDeploying | (_scanMDA != value); _scanMDA = value; }
        }

        public bool ScanUSB
        {
            get { return _scanUSB; }
            set { _needsDeploying = _needsDeploying | (_scanUSB != value); _scanUSB = value; }
        }

        public eFileSetting ScanMobileFiles
        {
            get { return _scanMobileFiles; }
            set { _needsDeploying = _needsDeploying | (_scanMobileFiles != value); _scanMobileFiles = value; }
        }

        public List<String> ListMobileFiles
        {
            get { return _listMobileFiles; }
            set { _needsDeploying = _needsDeploying | (_listMobileFiles != value); _listMobileFiles = value; }
        }

        public eFileSetting ScanUSBFiles
        {
            get { return _scanUSBFiles; }
            set { _needsDeploying = _needsDeploying | (_scanUSBFiles != value); _scanUSBFiles = value; }
        }

        public List<string> ListUSBFiles
        {
            get { return _listUSBFiles; }
            set { _needsDeploying = _needsDeploying | (_listUSBFiles != value); _listUSBFiles = value; }
        }
        #endregion USB / PDA Auditing Fields

        #region Internet Explorer Auditing Fields

        public bool IEHistory
        {
            get { return _IEHistory; }
            set { _needsDeploying = _needsDeploying | (_IEHistory != value); _IEHistory = value; }
        }

        public bool IECookies
        {
            get { return _IECookies; }
            set { _needsDeploying = _needsDeploying | (_IECookies != value); _IECookies = value; }
        }

        public int IEDays
        {
            get { return _IEDays; }
            set { _needsDeploying = _needsDeploying | (_IEDays != value); _IEDays = value; }
        }

        public bool IEDetails
        {
            get { return _IEDetails; }
            set { _needsDeploying = _needsDeploying | (_IEDetails != value); _IEDetails = value; }
        }

        #endregion Internet Explorer Auditing Fields

        #region Upload Fields

        public eUploadSetting UploadSetting
        {
            get { return _uploadSetting; }
            set { _needsDeploying = _needsDeploying | (_uploadSetting != value); _uploadSetting = value; }
        }

        public bool AutoUpload
        {
            get { return _autoUpload; }
            set { _needsDeploying = _needsDeploying | (_autoUpload != value); _autoUpload = value; }
        }

        #endregion

        #region FTP Upload Fields

        public bool FTPCopyToNetwork
        {
            get { return _FTPCopyToNetwork; }
            set { _needsDeploying = _needsDeploying | (_FTPCopyToNetwork != value); _FTPCopyToNetwork = value; }
        }

        public string FTPType
        {
            get { return _FTPType; }
            set { _FTPType = value; }
        }

        public string FTPTypeBackup
        {
            get { return _FTPTypeBackup; }
            set { _FTPTypeBackup = value; }
        }

        public bool FTPAnonymousBackup
        {
            get { return _FTPAnonymousBackup; }
            set { _FTPAnonymousBackup = value; }
        }

        public int FTPPortBackup
        {
            get { return _FTPPortBackup; }
            set { _FTPPortBackup = value; }
        }

        public string FTPUserBackup
        {
            get { return _FTPUserBackup; }
            set { _FTPUserBackup = value; }
        }

        public string FTPPasswordBackup
        {
            get { return _FTPPasswordBackup; }
            set { _FTPPasswordBackup = value; }
        }

        public string FTPSiteBackup
        {
            get { return _FTPSiteBackup; }
            set { _FTPSiteBackup = value; }
        }

        public string FTPDefDirBackup
        {
            get { return _FTPDefDirBackup; }
            set { _FTPDefDirBackup = value; }
        }

        public bool FTPAnonymous
        {
            get { return _FTPAnonymous; }
            set { _needsDeploying = _needsDeploying | (_FTPAnonymous != value); _FTPAnonymous = value; }
        }

        public int FTPPort
        {
            get { return _FTPPort; }
            set { _needsDeploying = _needsDeploying | (_FTPPort != value); _FTPPort = value; }
        }

        public string FTPUser
        {
            get { return _FTPUser; }
            set { _needsDeploying = _needsDeploying | (_FTPUser != value); _FTPUser = value; }
        }

        public string FTPPassword
        {
            get { return _FTPPassword; }
            set { _needsDeploying = _needsDeploying | (_FTPPassword != value); _FTPPassword = value; }
        }

        public string FTPSite
        {
            get { return _FTPSite; }
            set { _needsDeploying = _needsDeploying | (_FTPSite != value); _FTPSite = value; }
        }

        public string FTPDefDir
        {
            get { return _FTPDefDir; }
            set { _needsDeploying = _needsDeploying | (_FTPDefDir != value); _FTPDefDir = value; }
        }

        #endregion FTP Upload Fields

        #region Email Fields


        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _needsDeploying = _needsDeploying | (_emailAddress != value); _emailAddress = value; }
        }

        #endregion Email Fields

        #region Alert Monitor Fields

        public bool AlertMonitorEnabled
        {
            get { return _alertMonitorEnabled; }
            set { _alertMonitorEnabled = value; }
        }

        public bool AlertMonitorDisplayTray
        {
            get { return _alertMonitorDisplayTray; }
            set { _alertMonitorDisplayTray = value; }
        }

        public int AlertMonitorSettingSecs
        {
            get { return _alertMonitorSettingSecs; }
            set { _alertMonitorSettingSecs = value; }
        }

        public int AlertMonitorAlertSecs
        {
            get { return _alertMonitorAlertsSecs; }
            set { _alertMonitorAlertsSecs = value; }
        }

        public AlertDefinition AlertMonitorDefinition
        {
            get { return _alertMonitorDefinition; }
            set { _alertMonitorDefinition = value; }
        }


        #endregion

        public List<SerialNumberMapping> SerialNumberMappingsList
        {
            get { return _serialNumberMappings; }
            set { _serialNumberMappings = value; }
        }

        public string Version
        { 
            get { return _version; } 
        }

        public string UniqueAssetID
        {
            get { return _uniqueAssetID; }
        }

        #endregion Properties

        #region Constructor

        public AuditScannerDefinition(string scannerName)
        {
            // The scanner name is used so that the user can distinguish between different 
            // scanners that they have configured and is also the root of the disk file to 
            // which the configuration is read/saved
            ScannerName = scannerName;

            // Initialize all other values
            InitializeDefaults();

            // Setup the list of files that will need to be copied if the scanner is deployed
            _listScannerFiles.Add(_scannerExecutable);
            _listScannerFiles.Add(_scannerConfiguration);

            // Set the AuditWizardrver name - this is the name of the computer running AuditWizard
            _server = System.Environment.MachineName;
        }

        /// <summary>
        /// This constructor does not require the name of the scanner configuration but just 
        /// assumes 'default'.
        /// </summary>
        public AuditScannerDefinition()
        {
        }

        #endregion constructor

        #region Methods

        /// <summary>
        /// Override the ToString function to return the name of the scanner
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _scannerName;
        }



        /// <summary>
        /// This function is called to perform an immediate audit of the local PC
        /// </summary>
        /// <returns></returns>
        public bool AuditMyPC()
        {
            using (new WaitCursor())
            {
                // We save a number of scanner settings first as we will make changes to ensure that the
                // scanner will run to audit the PC - we restore these settings once we have deployed
                bool deployPathSingle = _deployPathSingle;
                string deployPathRoot = _deployPathRoot;
                string deployPathScanner = _deployPathScanner;
                string deployPathData = _deployPathData;
                bool autoName = _autoName;
                eAuditMode scannerMode = _scannerMode;
                bool hidden = _hidden;
                int reAuditInterval = _reAuditInterval;
                eUploadSetting uploadSetting = _uploadSetting;

                // Change the scanner settings as required
                _deployPathSingle = false;
                _tempFolder = System.IO.Path.GetTempPath();
                _deployPathScanner = _tempFolder;
                _deployPathData = _tempFolder;
                _autoName = true;
                _scannerMode = eAuditMode.modeNonInteractive;
                _hidden = true;
                _reAuditInterval = 0;
                _uploadSetting = eUploadSetting.network;
                _testMode = true;

                // Now deploy the scanner and its configuration to the temp folder
                if (Deploy(false))
                {
                    // Get the name of the scanner executable
                    string scannerPath = Path.Combine(_tempFolder, _scannerExecutable);

                    //..and run it waiting for the scanner to complete
                    Process scannerProcess = new Process();
                    scannerProcess.StartInfo.FileName = scannerPath;
                    scannerProcess.Start();
                    scannerProcess.WaitForExit(60000);				// Wait for 1 minute

                    // Now try and upload the results of this audit
                    UploadAuditMyPC();
                }


                // Restore any settings which we changed
                _testMode = false;
                _deployPathSingle = deployPathSingle;
                _deployPathRoot = deployPathRoot;
                _deployPathScanner = deployPathScanner;
                _deployPathData = deployPathData;
                _autoName = autoName;
                _scannerMode = scannerMode;
                _hidden = hidden;
                _reAuditInterval = reAuditInterval;
                _uploadSetting = uploadSetting;

            }

            // All done so return now
            return true;
        }

        /// <summary>
        /// This function is called to copy the scanner executables and any support files to the specified
        /// deployment scanner folder
        /// </summary>
        /// <returns></returns>
        public bool Deploy(bool configurationOnly)
        {
            try
            {
                string sourceFile;
                string destinationFile;

                // First copy the scanner executable itself
                if (!configurationOnly)
                {
                    sourceFile = Path.Combine(Application.StartupPath, _scannerExecutable);
                    destinationFile = Path.Combine(DeployPathScanner, _scannerExecutable);
                    Utility.CopyFileIfNewer(sourceFile, destinationFile);
                }

                // ...then the configuration file - ensure we save ourselves first
                destinationFile = Path.Combine(DeployPathScanner, _scannerConfiguration);
                //WriteTo(destinationFile);

                if (!AuditWizardSerialization.SerializeObjectToFile(this, destinationFile))
                    return false;

                // If we are auditing USB devices then we should deploy the contents of the '\USBScan' folder
                if (_scanUSB)
                {
                    List<string> listUsbFiles = Utility.ListFromString(_USBFiles, ',', true);
                    string sourceFolder = Path.Combine(Application.StartupPath, "USBScan");
                    foreach (string file in listUsbFiles)
                    {
                        sourceFile = Path.Combine(sourceFolder, file);
                        destinationFile = Path.Combine(DeployPathScanner, file);
                        Utility.CopyFileIfNewer(sourceFile, destinationFile);
                    }
                }

                // If we are auditing Mobile devices then we should deploy the contents of the '\PDAScan' folder
                if (_scanMDA)
                {
                    List<string> listUsbFiles = Utility.ListFromString(_MDAFiles, ',', true);
                    string sourceFolder = Path.Combine(Application.StartupPath, "PDAScan");
                    foreach (string file in listUsbFiles)
                    {
                        sourceFile = Path.Combine(sourceFolder, file);
                        destinationFile = Path.Combine(DeployPathScanner, file);
                        Utility.CopyFileIfNewer(sourceFile, destinationFile);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Failed to deploy the scanner files to [" + DeployPathScanner + "].  The error was : " + ex.Message, "Deploy Error");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deploy the AuditScanner to the specified folder as a self extracting executable
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool DeployToExecutable(string folder)
        {
            // We need to deploy the current scanner to the temp folder so that we can add the generated
            // files to the SFX.  To do this we will need to over-ride the configuration tempoarily
            bool deployPathSingle = _deployPathSingle;
            string deployPathRoot = _deployPathRoot;
            string deployPathScanner = _deployPathScanner;
            string deployPathData = _deployPathData;

            // Over-ride settings to point the scanner to the temp folder
            _deployPathSingle = false;
            _deployPathScanner = _tempFolder;
            _deployPathData = _tempFolder;

            // Deploy the scanner
            Deploy(false);

            // Populate the settings changed above
            _deployPathSingle = deployPathSingle;
            _deployPathRoot = deployPathRoot;
            _deployPathScanner = deployPathScanner;
            _deployPathData = deployPathData;

            // We need to create a self extracting executable which contains the scanner executable
            // and its associated configuration file.		
            try
            {
                string fileName = Path.Combine(folder, _scannerZip);

                Chilkat.Zip zipFile = new Chilkat.Zip();
                zipFile.UnlockComponent("LAYTONZIP_PfobUFZa7BtC");

                // We will never actually write the zip file, as we'll be writing a self-extracting EXE instead.
                zipFile.NewZip(fileName);

                // Add the scanner executable and configuration files to the zip
                string sourceFile = Path.Combine(_tempFolder, _scannerExecutable);
                zipFile.AppendFiles(sourceFile, false);
                //
                sourceFile = Path.Combine(_tempFolder, _scannerConfiguration);
                zipFile.AppendFiles(sourceFile, false);

                // Add AlertMonitor definitions if enabled
                SettingsDAO lwDataAccess = new SettingsDAO();
                bool alertsEnabled = lwDataAccess.GetSettingAsBoolean(DatabaseSettings.Setting_AlertMonitorEnable, false);
                if (alertsEnabled)
                {
                    sourceFile = Path.Combine(_tempFolder, AlertMonitorSettings.AlertMonitorDefinitionsFile);
                    zipFile.AppendFiles(sourceFile, false);
                }

                // Set the title bar for the EXE
                zipFile.ExeTitle = "AuditWizard self-extracting Scanner EXE";

                // Write a self-extracing EXE.  The LastError property will contain error information if it fails
                if (!zipFile.WriteExe(Path.Combine(folder, _scannerSfx)))
                    MessageBox.Show("Failed to create the AuditScanner Self Extracting Executable, the error was " + zipFile.LastErrorText, "SFX Create Failed");

                // Make sure all resources are released.
                zipFile.CloseZip();
            }

            catch (Exception ex)
            {
                MessageBox.Show("An exception occurred creating the AuditScanner Self Extracting Executable, the error was " + ex.Message, "Error Creating Scanner");
                return false;
            }

            return true;
        }

        /// <summary>
        /// This function checks to see whether or not EVEYONE has FULL CONTROL to the specified folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected void CheckFullControl(string path)
        {
            NTAccount everyoneAccount = new NTAccount("Everyone");
            SecurityIdentifier securityIdentifier = everyoneAccount.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            AuthorizationRuleCollection rules = directorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

            // Iterate through the rules and see if we can find an entry for everyone
            bool everyoneOK = false;
            foreach (FileSystemAccessRule accessRule in rules)
            {
                if (securityIdentifier.CompareTo(accessRule.IdentityReference as SecurityIdentifier) == 0)
                {
                    // There is an entry for 'Everyone' - what is it specifying though?
                    if ((accessRule.FileSystemRights | FileSystemRights.FullControl) != 0)
                        everyoneOK = true;
                }
            }

            // If everyone does not have full control then warn the user and ask if we should add it
            if (!everyoneOK)
                AddDirectorySecurity(path, @"Everyone", FileSystemRights.FullControl, AccessControlType.Allow);
        }


        // Adds an ACL entry on the specified directory for the specified account.
        protected void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }


        /// <summary>
        /// Return a list of scanner modes and their internal values
        /// </summary>
        /// <returns></returns>
        public IndexedStringList ScannerModes()
        {
            // Add the scanner modes in order to the list
            IndexedStringList list = new IndexedStringList();
            list.Add(new IndexedStringListEntry("Non Interactive", (int)eAuditMode.modeNonInteractive, null));
            list.Add(new IndexedStringListEntry("Interactive", (int)eAuditMode.modeInteractive, null));
            list.Add(new IndexedStringListEntry("First Time Interactive", (int)eAuditMode.modeFirstTimeInteractive, null));
            return list;
        }

        /// <summary>
        /// Return a list of Folder Settings and their internal values
        /// </summary>
        /// <returns></returns>
        public IndexedStringList FolderSettings()
        {
            // Add the scanner modes in order to the list
            IndexedStringList list = new IndexedStringList();
            list.Add(new IndexedStringListEntry("No Folders", (int)eFolderSetting.noFolders, null));
            list.Add(new IndexedStringListEntry("All Folders", (int)eFolderSetting.allFolders, null));
            list.Add(new IndexedStringListEntry("Specified Folders", (int)eFolderSetting.specifiedFolders, null));
            return list;
        }

        /// <summary>
        /// Return a list of File Settings and their internal values
        /// </summary>
        /// <returns></returns>
        public IndexedStringList FileSettings()
        {
            // Add the scanner modes in order to the list
            IndexedStringList list = new IndexedStringList();
            list.Add(new IndexedStringListEntry("No Files", (int)eFileSetting.noFiles, null));
            list.Add(new IndexedStringListEntry("All Executable Files", (int)eFileSetting.allExeFiles, null));
            list.Add(new IndexedStringListEntry("Specified Files", (int)eFileSetting.specifiedFiles, null));
            list.Add(new IndexedStringListEntry("All Files", (int)eFileSetting.specifiedFiles, null));
            return list;
        }


        /// <summary>
        /// Called to initialize all of the settings within the scanner configuration
        /// </summary>
        private void InitializeDefaults()
        {
            // get the windows temp paths		
            _tempFolder = System.IO.Path.GetTempPath();
        }


        public bool Delete()
        {
            try
            {
                File.Delete(_filename);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #region Utility Functions


        /// <summary>
        /// Upload function called as part of the 'Audit My PC code - note that we actually upload all 
        /// audit data files that we find in the Windows Temp folder
        /// </summary>
        protected void UploadAuditMyPC()
        {
            // Now perform the upload of this data file
            try
            {
                List<AuditFileInfo> listAuditFiles = new List<AuditFileInfo>();
                AuditUploader auditLoader = new AuditUploader(_deployPathData, 9999);
                auditLoader.EnumerateFiles(listAuditFiles);

                // sort the list by audit scan date
                listAuditFiles.Sort();

                // Iterate through all files identified and upload
                foreach (AuditFileInfo auditFileInfo in listAuditFiles)
                {
                    AuditDataFile auditDataFile = auditFileInfo.AuditFile;

                    // Now perform the upload of this data file
                    try
                    {
                        auditLoader.UploadAuditDataFile(auditDataFile);
                    }

                    // Any exceptions from the upload should be caught and reported - the exception will abort the 
                    // upload process.
                    catch (Exception)
                    {
                    }
                }
            }

            catch (Exception)
            {
            }
        }

        #endregion Utility Functions

        #endregion Methods

        #region SerializableDictionary Class

        public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
        {

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();

                if (wasEmpty)
                    return;

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");
                    reader.ReadStartElement("key");
                    TKey key = (TKey)keySerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");
                    TValue value = (TValue)valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    this.Add(key, value);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }

                reader.ReadEndElement();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                foreach (TKey key in this.Keys)
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("value");

                    TValue value = this[key];
                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }

            #endregion

        #endregion

        }
    }

}