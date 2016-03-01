using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Layton.AuditWizard.DataAccess
{
    /// <summary>
    /// This class encapsulates the AuditWizard stand-alone data file.  
	/// This data file acts as the interface between the AuditWizard scanner and the AuditWizard User
	/// interface.  As such this class is only actually interested in reading the Audit Data File as the
	/// scanner is not .NET and cannot therefore access this object
    /// </summary>
    public class AuditDataFile
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public enum CREATEDBY { awscanner ,usbscanner ,mdascanner };
		
        public enum ACCESS { read, write };
		public static string _fileExtension = ".adf";

    #region Data
        private string _filename;
		private bool _fileIsValid;

		public string FileName
		{ 
			get { return _filename; } 
			set { _filename = value; }
		}

		public bool FileIsValid
		{ get { return _fileIsValid; } }

		// Who created the Audit File?
		private CREATEDBY	_createdBy = CREATEDBY.awscanner;
		
		public CREATEDBY CreatedBy
		{
			get { return _createdBy; }
			set { _createdBy = value; }
		}
		
		//
		// Data recovered from the General Section
		//
		private string _assetName;					// Name of the computer as known to AW
		private string _netbios_name;					// NetBIOS name of the computere
		private string _newname;						// New name to be applied within AW
		private string _uniqueid;						// Unique identifier for this computer
		private DateTime _auditDate;					// Date of the audit
        private string _agentVersion;
		private string _category;						// Type of this asset
		private string _domain;							// Domain / workgroup for this computer
		private string _make;							// make of this computer
		private string _model;							// model of this computer
		private string _serial_number;					// Asset serial number
		private string _macaddress;						// Primary MAC address
		private string _ipaddress;						// Primary IP address
		private string _location;						// Asset location
		private string _parentAssetName;				// Name of (any) parent asset
        private string _assetTag;

		// Operating system data
		private string _osVersion;						// Name of the Operating System
		private string _osFamily;						// and its family
		private string _osSerial;						// and serial number
		private string _osCDKey;						// CD Key
		private string _osIEVersion;					// Internet Explorer Version
		private bool   _is64bit;						// Is a 64 bit OS

		// Audited User Defined Data Fields
		private List<AuditedItem> _listAuditedUserData = new List<AuditedItem>();
		
		// Audited Items
		private List<AuditedItem> _listAuditedItems = new List<AuditedItem>();

		// Internet Items
		private List<AuditedItem> _listInternetItems = new List<AuditedItem>();

		// Audited Software Applications
		private ApplicationInstanceList _listAuditedApplications = new ApplicationInstanceList();

		/// <summary>List of patches installed on the audited computer</summary>
		private List<InstalledPatch> _listPatches = new List<InstalledPatch>();
		
		/// <summary>List of File System folders audited</summary>
		private FileSystemFolderList	_listFolders = new FileSystemFolderList();
		
    #endregion Data

    #region Properties

        public string AgentVersion 
        {
            get { return _agentVersion; }
            set { _agentVersion = value; } 
        }

        public DateTime CreationDate
        { get { return _auditDate; } }

		public string AssetName
		{
			get { return _assetName; }
			set { _assetName = value; }
		}

		public string ParentAssetName
		{
			get { return _parentAssetName; }
			set { _parentAssetName = value; }
		}

		public string Netbios_name
		{
			get { return _netbios_name; }
			set { _netbios_name = value; }
		}

		public string Newname
		{
			get { return _newname; }
			set { _newname = value; }
		}

		public string Uniqueid
		{
			get { return _uniqueid; }
			set { _uniqueid = value; }
		}

		public DateTime AuditDate
		{
			get { return _auditDate; }
			set { _auditDate = value; }
		}

		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string Domain
		{
			get { return _domain; }
			set { _domain = value; }
		}

		public string Make
		{
			get { return _make; }
			set { _make = value; }
		}

		public string Model
		{
			get { return _model; }
			set { _model = value; }
		}

		public string Serial_number
		{
			get { return _serial_number; }
			set { _serial_number = value; }
		}

		public string Macaddress
		{
			get { return _macaddress; }
			set { _macaddress = value; }
		}

		public string Ipaddress
		{
			get { return _ipaddress; }
			set { _ipaddress = value; }
		}

		public string Location
		{
			get { return _location; }
			set { _location = value; }
		}

		public List<AuditedItem> AuditedUserDataItems
		{
			get { return _listAuditedUserData; }
			set { _listAuditedUserData = value; }
		}

		public List<AuditedItem> AuditedItems
		{
			get { return _listAuditedItems; }
			set { _listAuditedItems = value; }
		}

		public List<AuditedItem> AuditedInternetItems
		{
			get { return _listInternetItems; }
			set { _listInternetItems = value; }
		}

		public ApplicationInstanceList AuditedApplications
		{
			get { return _listAuditedApplications; }
			set { _listAuditedApplications = value; }
		}

		public OSInstance AuditedOS
		{
			get
			{
				OSInstance osInstance = new OSInstance();
				osInstance.Name = OSFamily;
				osInstance.Version = OSVersion;
				osInstance.Serial = new ApplicationSerial("", "", OSSerial, OSCDKey);
				return osInstance;
			}
		}

		public string OSVersion
		{
			get { return _osVersion; }
			set { _osVersion = value; }
		}

		public string OSFamily
		{
			get { return _osFamily; }
			set { _osFamily = value; }
		}

		public string OSSerial
		{
			get { return _osSerial; }
			set { _osSerial = value; }
		}

		public string OSCDKey
		{
			get { return _osCDKey; }
			set { _osCDKey = value; }
		}

		public string OSIEVersion
		{
			get { return _osIEVersion; }
			set { _osIEVersion = value; }
		}

		public FileSystemFolderList AuditedFolders
		{
			get { return _listFolders; }
		}

        public string AssetTag
        {
            get { return _assetTag; }
            set { _assetTag = value; }
        }

        #endregion Properties

    #region XMLStrings
        // String storage for sections and values in the XML audit Data File
		private const string S_AUDIT_FILE = "AuditDataFile";
		//
        private const string S_GENERAL			= "general";
		private const string V_GENERAL_COMPUTER	= "computername";		// Name of the computer in AW	
		private const string V_GENERAL_NETBIOS	= "netbioscomputername";// NetBIOS Asset Name
		private const string V_GENERAL_NEWCOMPUTER = "newcomputername";	// New name to be given to computer
		private const string V_GENERAL_UNIQUEID	= "computerid";			// Unique ID 
        private const string V_GENERAL_AUDITDATE= "auditdate";			// Date of audit
		private const string V_GENERAL_CATEGORY = "category";			// Asset category
		private const string V_GENERAL_DOMAIN	= "domain";				// Domain assigned to asset
		private const string V_GENERAL_MAKE		= "make";				// Make assigned to asset
		private const string V_GENERAL_MODEL	= "model";				// Model assigned to asset
		private const string V_GENERAL_SERIAL	= "serial";				// Serial number for asset
		private const string V_GENERAL_MACADDRESS = "macaddress";		// Primary MAC address
		private const string V_GENERAL_SCANNERVERSION = "scannerversion";// Scanner Version
		private const string V_GENERAL_IPADDRESS = "ipaddress";			// Primary IP address
		private const string V_GENERAL_LOCATION = "location";			// Selected location

		// The 'User Defined Data' section
		private const string S_USERDATA			= "userdata";
		private const string S_USERDATA_ITEM	= "userdata_item";		// A user defined data item
		private const string V_USERDATA_CATEGORY = "category";			// Category of the user defined data field
		private const string V_USERDATA_NAME	= "name";				// Name of the user defined data field
		private const string V_USERDATA_VALUE	= "value";				// Value set for the suer defined data field

		// The 'AuditData' details section - consists of a list of 'Audited Item' elements
		private const string S_AUDITED_ITEMS	= "auditeditems";
		private const string S_AUDITED_ITEM		= "auditeditem";		// An audited item
		private const string V_ITEM				= "item";				// An audited item
		private const string V_ITEM_CLASS		= "class";				// Class of the item
		private const string V_ITEM_NAME		= "name";				// Name of the item
		private const string V_ITEM_VALUE		= "value";				// Value of the item
		private const string V_ITEM_UNITS		= "displayunits";		// Units display text
		private const string V_ITEM_TYPE		= "datatype";			// datatype
		private const string V_ITEM_HISTORIED	= "historied";			// historied (generates history records on change)
		private const string V_ITEM_GROUPED		= "grouped";			// Group items beneath this category

		// The 'internet-items' section - consists of a list of 'Internet Item' elements
		private const string S_INTERNET_ITEMS	= "internet-items";
		private const string S_INTERNET_ITEM	= "internet-item";		// An audited internet item

		// The 'Applications' details section  - consists of a list of 'application-instance' elements
		private const string S_APPLICATIONS		= "applications";
		private const string S_APPLICATION		= "applicationinstance";
		private const string V_APPLICATION_NAME = "name";
		private const string V_APPLICATION_PUBLISHER = "publisher";
		private const string V_APPLICATION_VERSION = "version";
		private const string V_APPLICATION_PRODUCTID = "productid";
		private const string V_APPLICATION_CDKEY = "cdkey";

		// The Operating System section appears within the 'Applications' section
		private const string S_OPERATING_SYSTEM = "operatingsystem";
		private const string V_OS_VERSION		= "version";
		private const string V_OS_FAMILY		= "family";
		private const string V_OS_CDKEY			= "cdkey";
		private const string V_OS_SERIAL		= "serial";
		private const string V_OS_IEVERSION		= "ieversion";
		private const string V_OS_IS64BIT		= "is64bit";

		// The Installed Patches section appears within the 'Applications' section
		private const string S_PATCHES			= "patches";
		private const string S_PATCHES_PATCH	= "patch";
		private const string V_PATCH_PRODUCT	= "product";
		private const string V_PATCH_NAME		= "name";
		private const string V_PATCH_SERVICEPACK = "servicepack";
		private const string V_PATCH_DESCRIPTION = "description";
		private const string V_PATCH_INSTALLDATE = "installdate";
		private const string V_PATCH_INSTALLEDBY = "installedby";

		// FileSystem Scanner
		private const string S_FILESYSTEM		= "filesystem";
		private const string S_FOLDER			= "folder";
		private const string V_FOLDER_NAME		= "name";
		private const string S_FILE				= "file";
		private const string V_FILE_NAME		= "name";
		private const string V_FILE_SIZE		= "size";
		private const string V_FILE_PUBLISHER	= "pub";
		private const string V_FILE_DESCRIPTION	= "desc";
		private const string V_FILE_PRODUCTNAME = "app";
		private const string V_FILE_PVERSION1 = "pv1";
		private const string V_FILE_PVERSION2	= "pv2";
		private const string V_FILE_FVERSION1	= "fv1";
		private const string V_FILE_FVERSION2	= "fv2";
		private const string V_FILE_MODIFIED	= "mod";
		private const string V_FILE_LASTACCESSED = "la";
		private const string V_FILE_CREATED		= "cre";
		private const string V_FILE_FILENAME	= "fn";
		
    #endregion

    #region Constructor

		public AuditDataFile ()
		{
			_assetName = "";
			_netbios_name = "";
			_newname = "";
			_uniqueid = "";
			_auditDate = DateTime.Now;
			_category = "";
			_domain = "";
			_make = "";
			_model = "";
			_serial_number = "";
			_macaddress = "";
			_ipaddress = "";
			_location = "";
			_parentAssetName = "";

            _assetTag = "";
            _agentVersion = "";

			// Operating system data
			_osVersion = "";
			_osFamily = "";
			_osSerial = "";
			_osCDKey = "";
			_osIEVersion = "";
		
			// Assume the file is of a valid format initially
			_fileIsValid = true;
		}
    #endregion constructor

    #region READER Functions

		/// <summary>
		/// Read the Audit Data File into our internal buffer
		/// </summary>
		/// <returns></returns>
		public bool Read(string fileName)
		{
			// Save the name of the file read
			_filename = fileName;

			XmlTextReader textReader = null;
			XmlSimpleElement xmlSimpleElement = new XmlSimpleElement("junk");
			XmlParser xmlParser;

			// First of all parse the file
            try
            {
                textReader = new XmlTextReader(fileName);
                xmlParser = new XmlParser();
                xmlSimpleElement = xmlParser.Parse(textReader);
                textReader.Close();
            }
            catch (Exception ex)
            {
				if (textReader != null)
					textReader.Close();
                logger.Error(ex.Message);
                _fileIsValid = false;
                return false;
            }

			// Now iterate through the data recovered 
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				ProcessElementRead(childElement);
			}

			// We must have at least a 'General' section for this file to be valid
			_fileIsValid = (_assetName != "");
			return _fileIsValid;
		}


		/// <summary>
		/// Read an audit data file (from an existing XML stream in an XmlTextReader)
		/// </summary>
		/// <param name="xmlDocument"></param>
		/// <returns></returns>
		public bool Read(XmlTextReader textReader)
		{
			// Save the name of the file read (actually just indicate that this was from a TCP stream)
			_filename = "TCP/IP Audit Stream Received";

			XmlSimpleElement xmlSimpleElement = new XmlSimpleElement("junk");
			XmlParser xmlParser;

			// First of all parse the file
			try 
			{
				xmlParser = new XmlParser();
				xmlSimpleElement = xmlParser.Parse(textReader);
				textReader.Close();
            }

			catch (Exception)
			{
				_fileIsValid = false;
				return false;
			}

			// Now iterate through the data recovered 
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				ProcessElementRead(childElement);
			}

			// We must have at least a 'General' section for this file to be valid
			_fileIsValid = (_assetName != "");
			return _fileIsValid;
		}


		/// <summary>
		/// Called as we parse a top level element from the configuration file
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string elementName = xmlSimpleElement.TagName;

			// OK what sort of element is it?
			switch (elementName)
			{
				case S_AUDIT_FILE:
					break;

				case S_GENERAL:
					ProcessGeneralElementRead(xmlSimpleElement);
					break;

				case S_USERDATA:
					ProcessUserDataElementRead(xmlSimpleElement);
					break;

				case S_AUDITED_ITEMS:
					ProcessAuditedItemsElementRead(xmlSimpleElement);
					break;

				case S_INTERNET_ITEMS:
					ProcessInternetItemsElementRead(xmlSimpleElement);
					break;

				case S_APPLICATIONS:
				    ProcessApplicationsElementRead(xmlSimpleElement);
				    break;

				case S_FILESYSTEM:
					ProcessFileSystemElementRead(xmlSimpleElement);
					break;
					
				default:
					break;
			}
			return;
		}



		/// <summary>
		/// We have parsed the 'General' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessGeneralElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case V_GENERAL_COMPUTER:
						_assetName = childElement.Text;
						break;

					case V_GENERAL_NETBIOS:
						_netbios_name = childElement.Text;
						break;

					case V_GENERAL_NEWCOMPUTER:
						_newname = childElement.Text;
						break;

					case V_GENERAL_UNIQUEID:
						_uniqueid = childElement.Text;
						break;

					case V_GENERAL_AUDITDATE:
						_auditDate = DateTime.ParseExact(childElement.Text, "yyyy-MM-dd HH:mm:ss", null);
						break;

					case V_GENERAL_CATEGORY:
						_category = childElement.Text;
						break;

                    case "scannerversion":
                        _agentVersion = childElement.Text;
                        break;

					case V_GENERAL_DOMAIN:
						_domain = childElement.Text;
						break;

					case V_GENERAL_MAKE:
						_make = childElement.Text;
						break;

					case V_GENERAL_MODEL:
						_model = childElement.Text;
						break;

					case V_GENERAL_SERIAL:
						_serial_number = childElement.Text;
						break;

                    case "assettag":
                        _assetTag = childElement.Text;
                        break;

					case V_GENERAL_MACADDRESS:
						_macaddress = childElement.Text;
						break;

					case V_GENERAL_IPADDRESS:
						_ipaddress = childElement.Text;
						break;

					case V_GENERAL_LOCATION:
						_location = childElement.Text;
						break;

					default:
						break;
				}
			}
		}



		#region User Data Field Functions
		
		/// <summary>
		/// We have parsed the 'userdata_Items' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessUserDataElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_USERDATA_ITEM:
						ProcessUserDataFieldElementRead(childElement);
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// We have parsed a 'userdata_item' element 
		/// </summary>
		/// <param name="xmlElement">the 'Auditeditem' element</param>
		protected void ProcessUserDataFieldElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string category;
			string fieldName;
			string fieldValue;
			
			// Each 'Userdata_item' should have a 'category', 'field' and 'value' attribute
			// We need ALL of these attributes to consider this element valid
			if ((!xmlSimpleElement.Attributes.ContainsKey(V_USERDATA_CATEGORY))
			||  (!xmlSimpleElement.Attributes.ContainsKey(V_USERDATA_NAME))
			||  (!xmlSimpleElement.Attributes.ContainsKey(V_USERDATA_VALUE)))
				return;
				
			category = xmlSimpleElement.Attributes[V_USERDATA_CATEGORY];
			fieldName = xmlSimpleElement.Attributes[V_USERDATA_NAME];
			fieldValue = xmlSimpleElement.Attributes[V_USERDATA_VALUE];

			//
			_listAuditedUserData.Add(new AuditedItem(category ,fieldName ,fieldValue ,"" ,AuditedItem.eDATATYPE.text ,true));
		}

		#endregion User Data Field Functions

		/// <summary>
		/// We have parsed the 'AuditedItems' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessAuditedItemsElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_AUDITED_ITEM:
						ProcessAuditedItemElementRead(childElement);
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// We have parsed an 'AuditedItem' element 
		/// </summary>
		/// <param name="xmlElement">the 'Auditeditem' element</param>
		protected void ProcessAuditedItemElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string itemClass = "";

			// Each auditeditem element should have a 'class' attribute and optionally a 'historied' attribute
			// which we need to obtain first
			if (!xmlSimpleElement.Attributes.ContainsKey(V_ITEM_CLASS))
				return;
			itemClass = xmlSimpleElement.Attributes[V_ITEM_CLASS];

			// Get the 'historied' attribute of the class (if present)
			bool historied = true;
			if (xmlSimpleElement.Attributes.ContainsKey(V_ITEM_HISTORIED))
				historied = (xmlSimpleElement.Attributes[V_ITEM_HISTORIED] == "true");

			// Get the grouped attribute (note this defaults to false)
			bool grouped = false;
			if (xmlSimpleElement.Attributes.ContainsKey(V_ITEM_GROUPED))
				grouped = (xmlSimpleElement.Attributes[V_ITEM_GROUPED] == "true");


			// Recover the elements for this item
			// Each element may have name, value, displayunits datatype and histories attributes
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				string name = "";
				string value = "";
				string units = "";
				bool childHistoried = historied;

				AuditedItem.eDATATYPE datatype = AuditedItem.eDATATYPE.text;

				if ((!childElement.Attributes.ContainsKey(V_ITEM_NAME))
				|| (!childElement.Attributes.ContainsKey(V_ITEM_VALUE)))
					continue;

				// Valid item so recover name and value
				name = childElement.Attributes[V_ITEM_NAME];
				value = childElement.Attributes[V_ITEM_VALUE];

				// Maybe it will have display units and a datatype
				if (childElement.Attributes.ContainsKey(V_ITEM_UNITS))
					units = childElement.Attributes[V_ITEM_UNITS];
				//
				if (childElement.Attributes.ContainsKey(V_ITEM_TYPE))
					datatype = AuditedItem.TranslateDatatype(childElement.Attributes[V_ITEM_TYPE]);
				
				// We can over-ride the parent historied attribute here - not that this defaults to 
				// what-ever the parent category is set to
				if (childElement.Attributes.ContainsKey(V_ITEM_HISTORIED))
					childHistoried = (childElement.Attributes[V_ITEM_HISTORIED] == "true");

				// Add this to the list of audited data items
				AuditedItem auditedItem = new AuditedItem(itemClass, name, value, units, datatype, childHistoried);
				auditedItem.Grouped = grouped;                
				//
				_listAuditedItems.Add(auditedItem);

                // if this was Internal System ID (UUID) then save
                if (_uniqueid == "" && itemClass == "Hardware|BIOS" && name == "Internal System ID")
                    _uniqueid = value;
			}
		}


		/// <summary>
		/// We have parsed the 'internet-Items' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessInternetItemsElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_INTERNET_ITEM:
						ProcessInternetItemElementRead(childElement);
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// We have parsed an 'AuditedItem' element 
		/// </summary>
		/// <param name="xmlElement">the 'internet-item' element</param>
		protected void ProcessInternetItemElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string itemClass = "";

			// Each auditeditem element should have a 'class' attribute and optionally a 'historied' attribute
			// which we need to obtain first
			if (!xmlSimpleElement.Attributes.ContainsKey(V_ITEM_CLASS))
				return;
			itemClass = xmlSimpleElement.Attributes[V_ITEM_CLASS];

			// Get the 'historied' attribute of the class (if present)
			bool historied = true;
			if (xmlSimpleElement.Attributes.ContainsKey(V_ITEM_HISTORIED))
				historied = (xmlSimpleElement.Attributes[V_ITEM_HISTORIED] == "true");

			// Get the grouped attribute (note this defaults to false)
			bool grouped = false;
			if (xmlSimpleElement.Attributes.ContainsKey(V_ITEM_GROUPED))
				grouped = (xmlSimpleElement.Attributes[V_ITEM_GROUPED] == "true");

			// Recover the elements for this item
			// Each element may have name, value, displayunits datatype and histories attributes
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				string name = "";
				string value = "";
				string units = "";
				AuditedItem.eDATATYPE datatype = AuditedItem.eDATATYPE.text;

				if ((!childElement.Attributes.ContainsKey(V_ITEM_NAME))
				|| (!childElement.Attributes.ContainsKey(V_ITEM_VALUE)))
					continue;

				// Valid item so recover name and value
				name = childElement.Attributes[V_ITEM_NAME];
				value = childElement.Attributes[V_ITEM_VALUE];

				// Add this to the list of audited data items
				AuditedItem auditedItem = new AuditedItem(itemClass, name, value, units, datatype, false);
				auditedItem.Grouped = grouped;
				_listInternetItems.Add(auditedItem);
			}
		}


		/// <summary>
		/// We have parsed the 'Applications' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessApplicationsElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_APPLICATION:
						ProcessApplicationElementRead(childElement);
						break;
				
					case S_OPERATING_SYSTEM:
						ProcessOperatingSystemElementRead(childElement);
						break;
						
					default:
						break;
				}
			}
		}



		/// <summary>
		/// We have parsed an 'Application' element 
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessApplicationElementRead(XmlSimpleElement xmlSimpleElement)
		{
			// Each audited application should consist of 
			//  1> Name
			//  2> Publisher
			//  3> Version
			//  4> Product ID
			//  5> CD Key
			//  Try and read these here
			string name = "";
			string publisher = "";
			string version = "";
			string productId= "";
			string cdKey = "";
			//
			name		= xmlSimpleElement.ChildElementText(V_APPLICATION_NAME);
			publisher	= xmlSimpleElement.ChildElementText(V_APPLICATION_PUBLISHER);
			version		= xmlSimpleElement.ChildElementText(V_APPLICATION_VERSION);
			productId	= xmlSimpleElement.ChildElementText(V_APPLICATION_PRODUCTID);
			cdKey		= xmlSimpleElement.ChildElementText(V_APPLICATION_CDKEY);

			// Create a new application instance and add to our list
			ApplicationInstance thisInstance = new ApplicationInstance();
			thisInstance.Name = name;
			thisInstance.Publisher = publisher;
			thisInstance.Version = version;
			thisInstance.Serial = new ApplicationSerial(name ,"" ,productId ,cdKey);
			_listAuditedApplications.Add(thisInstance);
		}


		/// <summary>
		/// We have parsed an 'Operating System' element 
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessOperatingSystemElementRead(XmlSimpleElement xmlSimpleElement)
		{
			//  Try and read the operating system attributes here
			_osVersion = "";
			_osFamily = "";
			_osSerial = "";
			_osCDKey = "";
			_osIEVersion = "";
			

			//
			_osVersion = xmlSimpleElement.ChildElementText(V_OS_VERSION);

			// OS Family may have the text '(64 Bit)' added to it if the 64 bit flag is set in the ADF
			_osFamily = xmlSimpleElement.ChildElementText(V_OS_FAMILY);
			if (xmlSimpleElement.ChildElementText(V_OS_IS64BIT) == "true")
				_osFamily += " (64 Bit)";

			_osSerial = xmlSimpleElement.ChildElementText(V_OS_SERIAL);
			_osCDKey = xmlSimpleElement.ChildElementText(V_OS_CDKEY);
			_osIEVersion = xmlSimpleElement.ChildElementText(V_OS_IEVERSION);
		}


		/// <summary>
		/// We have parsed the 'Patches' element so now parse the items 
		/// within this section noting that we terminate parsing when we reach the end 
		/// of the section.
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessPatchesElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_PATCHES_PATCH:
						ProcessPatchElementRead(childElement);
						break;

					default:
						break;
				}
			}
		}



		/// <summary>
		/// We have parsed a 'Patch' element 
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessPatchElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string product = "";
			string name = "";
			string servicepack = "";
			string installDate = "";
			string installedBy = "";
			string description = "";
			//
			product		= xmlSimpleElement.ChildElementText(V_PATCH_PRODUCT);
			name		= xmlSimpleElement.ChildElementText(V_PATCH_NAME);
			servicepack = xmlSimpleElement.ChildElementText(V_PATCH_SERVICEPACK);
			installDate = xmlSimpleElement.ChildElementText(V_PATCH_INSTALLDATE);
			installedBy = xmlSimpleElement.ChildElementText(V_PATCH_INSTALLEDBY);
			description	= xmlSimpleElement.ChildElementText(V_PATCH_DESCRIPTION);

			// Create a new patch and add to our list
			_listPatches.Add(new InstalledPatch(product, name, servicepack, description, installedBy, installDate));
		}






		/// <summary>
		/// We have parsed the 'File System' element 
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessFileSystemElementRead(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_FOLDER:
						ProcessFileSystemFolderRead(childElement);
						break;

					default:
						break;
				}
			}
		}



		/// <summary>
		/// We have parsed the 'Folder' element within the 'File System' section
		/// NOTE: This is a TOP-LEVEL folder and may itself have 0 or more sub-folders beneath it
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessFileSystemFolderRead(XmlSimpleElement xmlSimpleElement)
		{
			// Each folder has its name as an attribute 
			FileSystemFolder folder = new FileSystemFolder();
			folder.Name = xmlSimpleElement.Attributes[V_FOLDER_NAME];
			
			// As this is a top level folder we simpld add it to our folder list
			_listFolders.Add(folder);
			
			// Beneath this folder we may have 0 or more child folders and 0 or more child files
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_FOLDER:
						ProcessFileSystemChildFolderRead(childElement, folder);
						break;

					case S_FILE:
						ProcessFileSystemFileRead(childElement, folder);
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// We have parsed a 'Folder' element within a parent folder
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessFileSystemChildFolderRead(XmlSimpleElement xmlSimpleElement ,FileSystemFolder parentFolder)
		{
			// Each folder has its name as an attribute 
			FileSystemFolder childFolder = new FileSystemFolder();
			childFolder.Name = xmlSimpleElement.Attributes[V_FOLDER_NAME];

			// As this is a child folder we simpld add it to our parent folder list
			parentFolder.FoldersList.Add(childFolder);

			// Beneath this folder we may also have 0 or more child folders and 0 or more child files
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case S_FOLDER:
						ProcessFileSystemChildFolderRead(childElement, childFolder);
						break;

					case S_FILE:
						ProcessFileSystemFileRead(childElement, childFolder);
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// We have parsed a 'File' element within the 'File System\Folder' section
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessFileSystemFileRead(XmlSimpleElement xmlSimpleElement ,FileSystemFolder parentFolder)
		{
			// Each file has a number of attributes which we may have in the audit file
			FileSystemFile file = new FileSystemFile();
			parentFolder.FilesList.Add(file);
			
			// Each file MUST have its name as an attribute (Skip this file if not)
			if (!xmlSimpleElement.Attributes.ContainsKey(V_FILE_NAME))
				return;
			file.Name = xmlSimpleElement.Attributes[V_FILE_NAME];
		
			// Other attributes are optional really
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_SIZE))
				file.Size = Convert.ToInt32(xmlSimpleElement.Attributes[V_FILE_SIZE]);
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_PUBLISHER))
				file.Publisher = xmlSimpleElement.Attributes[V_FILE_PUBLISHER];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_DESCRIPTION))
				file.Description = xmlSimpleElement.Attributes[V_FILE_DESCRIPTION];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_PRODUCTNAME))
				file.ProductName = xmlSimpleElement.Attributes[V_FILE_PRODUCTNAME];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_PVERSION1))
				file.ProductVersion1 = xmlSimpleElement.Attributes[V_FILE_PVERSION1];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_PVERSION2))
				file.ProductVersion2 = xmlSimpleElement.Attributes[V_FILE_PVERSION2];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_FVERSION1))
				file.FileVersion1 = xmlSimpleElement.Attributes[V_FILE_FVERSION1];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_FVERSION2))
				file.FileVersion2 = xmlSimpleElement.Attributes[V_FILE_FVERSION2];
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_LASTACCESSED))
			{
				try
				{
					file.LastAccessedDateTime = DateTime.Parse(xmlSimpleElement.Attributes[V_FILE_LASTACCESSED]);
				}
				catch (Exception)
				{}
			}
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_CREATED))
			{
				try
				{
					string date = xmlSimpleElement.Attributes[V_FILE_CREATED];
					file.CreatedDateTime = DateTime.Parse(date);
				}
				catch (Exception)
				{ }
			}
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_MODIFIED))
			{
				try
				{
					file.ModifiedDateTime = DateTime.Parse(xmlSimpleElement.Attributes[V_FILE_MODIFIED]);
				}
				catch (Exception)
				{ }
			}
			//
			if (xmlSimpleElement.Attributes.ContainsKey(V_FILE_FILENAME))
				file.Description = xmlSimpleElement.Attributes[V_FILE_FILENAME];
			
		}

		#endregion Reader Functions

#region Writer Functions

		/// <summary>
        /// write the XML audit data file using the supplied data
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public int Write (string filename)
        {
			// Save the name of the file to be written
			_filename = filename;

            try
            {
                XmlTextWriterEx writer = new XmlTextWriterEx(filename, Encoding.UTF8);

                // Use indenting for readability.
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteComment("AuditWizard audit data file");

				// Now the Scanner Configuration Section
				writer.StartSection(S_AUDIT_FILE);

				// Add the 'General' section
				WriteGeneral(writer);

				// Write the 'Audited Items' section
				WriteAuditedItems(writer);

				// Write 'Audited Applications' section
				WriteApplications(writer);

                // ...and the element
                writer.WriteEndElement();

                // ...and the document as a whole
                writer.WriteEndDocument();

				writer.Flush();
				writer.Close();
            }
            catch (Exception ex)
            {
				String error = ex.Message;
            }

            return 0;
        }


		/// <summary>
		/// WriteGeneral - write the attributes held in the 'General' section
		/// </summary>
		/// <param name="writer"></param>
		private void WriteGeneral(XmlTextWriterEx writer)
		{
			writer.StartSection(S_GENERAL);		// ...add a 'General' section
			//
			writer.WriteSetting(V_GENERAL_COMPUTER, _assetName);
			writer.WriteSetting(V_GENERAL_NETBIOS, _netbios_name);
			writer.WriteSetting(V_GENERAL_NEWCOMPUTER, _newname);
			writer.WriteSetting(V_GENERAL_UNIQUEID, _uniqueid);
			writer.WriteSetting(V_GENERAL_CATEGORY, _category);
			writer.WriteSetting(V_GENERAL_AUDITDATE, _auditDate.ToString("yyyy-MM-dd HH:mm:SS"));
			writer.WriteSetting(V_GENERAL_DOMAIN, _domain);
			writer.WriteSetting(V_GENERAL_MAKE, _make);
			writer.WriteSetting(V_GENERAL_MODEL, _model);
			writer.WriteSetting(V_GENERAL_SERIAL, _serial_number);
			writer.WriteSetting(V_GENERAL_MACADDRESS, _macaddress);
			writer.WriteSetting(V_GENERAL_IPADDRESS, _ipaddress);
			writer.WriteSetting(V_GENERAL_LOCATION, _location);		
			writer.WriteSetting(V_GENERAL_SCANNERVERSION, "AuditScanner v8.3");	
			writer.EndSection();				// ...out of 'General' section
		}


		private void WriteAuditedItems (XmlTextWriterEx writer)
		{
			writer.StartSection(S_AUDITED_ITEMS);		// ...add an 'Audited Items' section
			
			// Step into this section and write the audited item
			string activeCategory = "";
			foreach (AuditedItem auditedItem in _listAuditedItems)
			{
				if (activeCategory != auditedItem.Category)
				{
					// Close any preceding audited item and open another
					if (activeCategory != "")
						writer.EndSection();
					writer.StartSection(S_AUDITED_ITEM);

					// Class
					writer.WriteStartAttribute(V_ITEM_CLASS);
					writer.WriteString(auditedItem.Category);
					writer.WriteEndAttribute();
					activeCategory = auditedItem.Category;
				}

				// Write the item itself
				writer.WriteStartElement(V_ITEM);
				//
				writer.WriteStartAttribute(V_ITEM_NAME);
				writer.WriteString(auditedItem.Name);
				writer.WriteEndAttribute();
				//
				writer.WriteStartAttribute(V_ITEM_VALUE);
				writer.WriteString(auditedItem.Value);
				writer.WriteEndAttribute();
				//
				writer.WriteEndElement();
			}
		}

		private void WriteApplications (XmlTextWriterEx writer)
		{
			// Now write the applications section
            writer.StartSection(S_APPLICATIONS);
                
            // and iterate through the audited applications
			foreach (ApplicationInstance thisInstance in _listAuditedApplications)
            {
				writer.StartSection(S_APPLICATION);
				writer.WriteSetting(V_APPLICATION_NAME, thisInstance.Name);
				writer.WriteSetting(V_APPLICATION_PUBLISHER, thisInstance.Publisher);
                writer.WriteSetting(V_APPLICATION_VERSION, thisInstance.Version);
                writer.WriteSetting(V_APPLICATION_PRODUCTID ,(thisInstance.Serial == null) ? "" : thisInstance.Serial.ProductId);
                writer.WriteSetting(V_APPLICATION_CDKEY ,(thisInstance.Serial == null) ? "" : thisInstance.Serial.CdKey);
                writer.EndSection();
			}

			// Start the Operating System section and write out it's attributes
			writer.StartSection(S_OPERATING_SYSTEM);
			writer.WriteSetting(V_OS_FAMILY ,_osFamily);
			writer.WriteSetting(V_OS_VERSION ,_osVersion);
			writer.WriteSetting(V_OS_SERIAL ,_osSerial);
			writer.WriteSetting(V_OS_CDKEY ,_osCDKey);
			writer.WriteSetting(V_OS_IEVERSION ,_osIEVersion);
			writer.EndSection();

			// Start the Patches section
			writer.StartSection(S_PATCHES);

			// ...and add in the patches
			foreach (InstalledPatch patch in _listPatches)
			{
				writer.StartSection(S_PATCHES_PATCH);
				writer.WriteSetting(V_PATCH_PRODUCT ,patch.PatchedProduct);
				writer.WriteSetting(V_PATCH_NAME ,patch.Name);
				writer.WriteSetting(V_PATCH_SERVICEPACK ,patch.ServicePack);
				writer.WriteSetting(V_PATCH_DESCRIPTION ,patch.Description);
				writer.WriteSetting(V_PATCH_INSTALLDATE ,patch.InstallDate);
				writer.WriteSetting(V_PATCH_INSTALLEDBY ,patch.InstalledBy);
				writer.EndSection();
			}

			// Close the Patches section
			writer.EndSection();

			// Close the applications section
			writer.EndSection();
        }

    #endregion Writer Functions
    
    }

    public class XmlSanitizedString
    {
        private readonly string value;

        public XmlSanitizedString(string s)
        {
            this.value = XmlSanitizedString.SanitizeXmlString(s);
        }

        /// <summary>
        /// Get the XML-santizied string.
        /// </summary>
        public override string ToString()
        {
            return this.value;
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        private static string SanitizeXmlString(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return xml;
            }

            var buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (XmlSanitizedString.IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static bool CheckXmlString(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return true;
            }

            var buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (!XmlSanitizedString.IsLegalXmlChar(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */        ||
                 character == 0xA /* == '\n' == 10  */        ||
                 character == 0xD /* == '\r' == 13  */        ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}

