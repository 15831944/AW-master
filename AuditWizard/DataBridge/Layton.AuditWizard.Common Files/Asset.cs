using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// This object encapsulates the definition of a computer within AuditWizard
	/// </summary>
	#region Asset Class

	public class Asset
	{
		public enum AGENTSTATUS { notdeployed, deployed, Running }
		public enum STOCKSTATUS { stock ,inuse, pendingdisposal, disposed }

		#region Data
		// Fields
		private string		_uniqueID;
		private int			_assetID;
		private string		_ipaddress;
		private DateTime	_lastAudit;
		private string		_location;
		private string		_fullLocation;
		private int			_locationID;
		private int			_parentAssetID;
		private string		_domain;
		private int			_domainID;
		private string		_name;
		private bool		_requestaudit;
		private AGENTSTATUS _agentStatus;
		private string		_agentVersion;
		private string		_macAddress;
		private int			_assetTypeID;
		private string		_assetTypeName;
		private string		_make;
		private string		_model;
		private string		_serial;
		private string		_icon;
		private bool		_auditable;
		private bool		_alerts_enabled;
		private object _tag;
		private STOCKSTATUS	_stockStatus;

		// Supplier
		private string		_supplierName;
		private int			_supplierID;

		// Enum for attributes - this should match up with the names below
		public enum eAttributes { assetname ,location ,domain ,lastaudit ,ipaddress ,macaddress ,category ,make ,model ,serial ,stock_status ,suppliername};
		public static readonly ICollection<string> _listAttributes = new List<string>(new string[] 
		{ 
			 "Asset Name"
			,"Location"
			,"Domain"
			,"Date of last Audit"
			,"IP Address"
			,"MAC Address"
			,"Category"
			,"Make"
			,"Model"
			,"Serial Number" 
			,"Stock Status" 
			,"Supplier Name" 
		});

		/// <summary>
		/// Each asset (potentially) has a list of audited items associated with it
		/// </summary>
		AuditedItemList _listAuditedItems = null;

		/// <summary>
		/// Each asset may have audited the file system
		/// </summary>
		FileSystemFolderList _listFolders = null;

		/// <summary>
		/// Each asset may have zero or more child assets
		/// </summary>
		AssetList			_childAssets = null;
		
		#endregion Data

		#region Properties

		// Properties
		[Category("Identity"), ReadOnly(true), Description("This is the name of the Domain or Active Directory location for the Asset.")]
		public string Domain
		{
			get { return this._domain; }
			set { this._domain = (value == "") ? "<None>" : RationalizeDomainName(value); }
		}

		// Properties
		[Category("Identity"), ReadOnly(true), Description("This is the database ID of the Domain or Active Directory location for the Asset.")]
		public int DomainID
		{
			get { return this._domainID; }
			set { this._domainID = value; }
		}

		[Category("Status"), Description("Flag to indicate whether or not this Asset should be audited now."), ReadOnly(true)]
		public bool RequestAudit
		{
			get { return this._requestaudit; }
			set { this._requestaudit = value; }
		}

		[Browsable(false)]
		public int AssetID
		{
			get { return this._assetID; }
			set { this._assetID = value; }
		}

		[Browsable(false)]
		public int ParentAssetID
		{
			get { return this._parentAssetID; }
			set { this._parentAssetID = value; }
		}

		[ReadOnly(true), Description("This is the IP Address of the Asset."), Category("Identity")]
		public string IPAddress
		{
			get { return this._ipaddress; }
			set { this._ipaddress = value; }
		}

		[Category("Status"), ReadOnly(true), Description("This is the date on which the Asset was last audited.")]
		public DateTime LastAudit
		{
			get { return this._lastAudit; }
			set { this._lastAudit = value; }
		}

		[ReadOnly(true), Category("Status"), Description("This is the date on which the Asset was last audited.")]
		public string LastAuditDateString
		{
			get
			{
				if (this._lastAudit.Ticks != 0L)
				{
					return this._lastAudit.ToString();
				}
				return "<not audited>";
			}
		}

		[Description("This is the location as defined within AuditWizard for this Asset."), ReadOnly(true), Category("Identity")]
		public string Location
		{
			get { return this._location; }
			set { this._location = value; }
		}

		[Description("This is the expanded location as defined within AuditWizard for this Asset."), ReadOnly(true), Category("Identity")]
		public string FullLocation
		{
			get { return this._fullLocation; }
			set { this._fullLocation = value; }
		}

		[Description("This is the location id as defined within AuditWizard for this Asset."), ReadOnly(true), Category("Identity")]
		public int LocationID
		{
			get { return this._locationID; }
			set { this._locationID = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the actual name of the Asset.")]
		public string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is a unique identifier string for the Asset.")]
		public string AssetIdentifier
		{
			get { return String.Format("{0}|{1}" ,_name ,_uniqueID); }
		}

		[ReadOnly(true), Category("Identity"), Description("This is a unique ID string for the Asset.")]
		public string UniqueID
		{
			get { return this._uniqueID; }
			set { this._uniqueID = value; }
		}

		[Description("This indicates whether or not the AuditWizard Audit Agent is deployed and/or running on this computer."), ReadOnly(true), Category("Status")]
		public AGENTSTATUS AgentStatus
		{
			get { return this._agentStatus; }
			set { this._agentStatus = value; }
		}

		[Description("This returns the textual version number of any audit agent on this computer."), ReadOnly(true), Category("Status")]
		public string AgentVersion
		{
			get { return (AgentStatus == Asset.AGENTSTATUS.notdeployed) ? "" : _agentVersion; }
		}

		[Description("This returns a textual indication of the current status of any audit agent on this computer."), ReadOnly(true), Category("Status")]
		public string AgentStatusText
		{
			get
			{
				return (AgentStatus == Asset.AGENTSTATUS.notdeployed) ? "Not Deployed"
			  : (AgentStatus == Asset.AGENTSTATUS.deployed) ? "Deployed [Not Running]" : "Deployed [Running]";
			}
		}

		[ReadOnly(true), Category("Identity"), Description("This is the MAC address for the Asset.")]
		public string MACAddress
		{
			get { return this._macAddress; }
			set { this._macAddress = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the index of the Type record in the database for the Asset.")]
		public int AssetTypeID
		{
			get { return this._assetTypeID; }
			set { this._assetTypeID = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the type of the Asset.")]
		public string TypeAsString
		{
			get { return this._assetTypeName; }
			set { this._assetTypeName = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the Make of the Asset.")]
		public string Make
		{
			get { return this._make; }
			set { this._make = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the model of the Asset.")]
		public string Model
		{
			get { return this._model; }
			set { this._model = value; }
		}

		[ReadOnly(true), Category("Identity"), Description("This is the serial number of the Asset.")]
		public string SerialNumber
		{
			get { return this._serial; }
			set { this._serial = value; }
		}
		
		public string Key
		{
			get { return this.UniqueID + "|" + this.Name; }
		}

		public AuditedItemList AuditedItems
		{
			get 
			{
				if (_listAuditedItems == null)
					PopulateAuditedItems();
				return _listAuditedItems; 
			}
		}

		public FileSystemFolderList FileSystemFolders
		{
			get
			{
				if (_listFolders == null)
					PopulateFileSystem();
				return _listFolders;
			}
		}

		/// <summary>
		/// Return a list of the child assets of this asset
		/// </summary>
		public AssetList ChildAssets
		{
			get
			{
				if (_childAssets == null)
					PopulateChildAssets();
				return _childAssets;
			}
		}

		[ReadOnly(true), Description("This is the name of the icon to display for the Asset."), Category("Identity")]
		public string Icon
		{
			get { return this._icon; }
			set { this._icon = value; }
		}

		[Category("Status"), Description("Flag to indicate whether or not this Asset may be audited."), ReadOnly(true)]
		public bool Auditable
		{
			get { return this._auditable; }
			set { this._auditable = value; }
		}

		[Category("Status"), Description("Flag to indicate whether or not alerts should be generated for this asset."), ReadOnly(true)]
		public bool AlertsEnabled
		{
			get { return this._alerts_enabled; }
			set { this._alerts_enabled = value; }
		}

		[ReadOnly(true), Description("This is an associated user defined object."), Category("Identity")]
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		[Description("This is the current stock status of this asset."), ReadOnly(true), Category("Identity")]
		public STOCKSTATUS StockStatus
		{
			get { return this._stockStatus; }
			set { _stockStatus = value; }
		}

		public static string GetStockStatusText (STOCKSTATUS stockStatus)
		{
			switch (stockStatus)
			{
				case STOCKSTATUS.disposed: return "Disposed"; 
				case STOCKSTATUS.inuse: return "In Use"; 
				case STOCKSTATUS.pendingdisposal: return "Pending Disposal"; 
				case STOCKSTATUS.stock: return "Stock"; 
				default: return "Unknown";
			}
		}
		
		public static Dictionary<STOCKSTATUS, string> StockStatuses ()
		{
			Dictionary<STOCKSTATUS ,string> dictionary = new Dictionary<STOCKSTATUS,string>();
			dictionary.Add(STOCKSTATUS.stock ,GetStockStatusText(STOCKSTATUS.stock));
			dictionary.Add(STOCKSTATUS.inuse, GetStockStatusText(STOCKSTATUS.inuse));
			dictionary.Add(STOCKSTATUS.pendingdisposal, GetStockStatusText(STOCKSTATUS.pendingdisposal));
			dictionary.Add(STOCKSTATUS.disposed, GetStockStatusText(STOCKSTATUS.disposed));
			return dictionary;
		}

		public int SupplierID
		{
			get { return _supplierID; }
			set { _supplierID = value; }
		}

		public string SupplierName
		{
			get { return _supplierName; }
			set { _supplierName = value; }
		}

		#endregion Properties

		#region Constructors

		public Asset()
		{
			_assetID = 0;
			_parentAssetID = 0;
			_uniqueID = "";
			_name = "";
			_location = "";
			_fullLocation = "";
			_locationID = 1;
			_domain = "<None>";
			_domainID = 1;
			_ipaddress = "";
			_lastAudit = new DateTime(0L);
			_requestaudit = false;
			_agentStatus = AGENTSTATUS.notdeployed;
			_agentVersion = "";
			_assetTypeID = 2;
			_assetTypeName = "";
			_macAddress = "";
			_make = "";
			_model = "";
			_serial = "";
			_icon = "";
			_auditable = false;
			_alerts_enabled = true;
			_tag = null;
			_stockStatus = STOCKSTATUS.inuse;
			_supplierID = 1;
			_supplierName = "";
			//
			_listAuditedItems = null;
			_listFolders = null;
			_childAssets = null;
			
		}

		public Asset(DataRow dataRow) : this()
		{
			try
			{
				_assetID		= (int) dataRow["_ASSETID"];
				_parentAssetID  = (int)dataRow["_PARENT_ASSETID"];
				_uniqueID		= (string)dataRow["_UNIQUEID"];
				_name			= (string) dataRow["_NAME"];
				_locationID		= (int) dataRow["_LOCATIONID"];
				_location		= (string)dataRow["LOCATIONNAME"];
				_fullLocation	= (string)dataRow["FULLLOCATIONNAME"];
				_domainID		= (int)dataRow["_DOMAINID"];
				_domain			= (string)dataRow["DOMAINNAME"];
				_ipaddress		= (string)dataRow["_IPADDRESS"];
				_requestaudit	= (bool)dataRow["_REQUESTAUDIT"];
				_lastAudit		= (dataRow.IsNull("_LASTAUDIT")) ? new DateTime(0L) : (DateTime)dataRow["_LASTAUDIT"];
				_agentStatus	= (AGENTSTATUS)dataRow["_AGENT_STATUS"];
				_agentVersion	= (string)dataRow["_AGENT_VERSION"];
				_assetTypeID	= (int)dataRow["_ASSETTYPEID"];
				TypeAsString	= (string)dataRow["ASSETTYPENAME"];
				_macAddress		= (string)dataRow["_MACADDRESS"];
				_make			= (string)dataRow["_MAKE"];
				_model			= (string)dataRow["_MODEL"];
				_serial			= (string)dataRow["_SERIAL_NUMBER"];
				_icon			= (string)dataRow["ICON"];
				_auditable		= (bool)dataRow["AUDITABLE"];
				_alerts_enabled = (bool)dataRow["_ALERTS_ENABLED"];
				_stockStatus = (STOCKSTATUS)dataRow["_STOCK_STATUS"];

				// Supplier
				this.SupplierID = (int)dataRow["_SUPPLIERID"];
				this.SupplierName = (string)dataRow["SUPPLIER_NAME"];
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception occured creating an ASSET Object, please check database schema.  The message was " + ex.Message);		
			}
		}
		
		
		public Asset(AuditDataFile auditDataFile) : this()
		{
			UniqueID = auditDataFile.Uniqueid;
			Name = auditDataFile.AssetName;
			Domain = auditDataFile.Domain;
			IPAddress = auditDataFile.Ipaddress;
			LastAudit = auditDataFile.AuditDate;
			Location = auditDataFile.Location;
			AssetTypeID = 2;
			MACAddress = auditDataFile.Macaddress;
			Make = auditDataFile.Make;
			Model = auditDataFile.Model;
			SerialNumber = auditDataFile.Serial_number;
		}

		public Asset(int computerID, string uniqueID ,string name, int locationID, int domainID, string ipaddress, DateTime lastAudit, bool hidden)
			: this()
		{
			_assetID = computerID;
			_uniqueID = uniqueID;
			_name = name;
			_location = "";
			_locationID = locationID;
			_domainID = domainID;
			_domain = "";
			_ipaddress = ipaddress;
			_lastAudit = lastAudit;
		}


		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public Asset(Asset other)
		{
			_assetID = other.AssetID;
			_parentAssetID = other._parentAssetID;
			_uniqueID = other._uniqueID;
			_name = other._name;
			_location = other._location;
			_fullLocation = other._fullLocation;
			_locationID = other._locationID;
			_domain = other._domain;
			_domainID = other._domainID;
			_ipaddress = other._ipaddress;
			_lastAudit = other._lastAudit;
			_requestaudit = other._requestaudit;
			_agentStatus = other._agentStatus;
			_agentVersion = other._agentVersion;
			_assetTypeID = other._assetTypeID;
			_assetTypeName = other._assetTypeName;
			_macAddress = other._macAddress;
			_make = other._make;
			_model = other._model;
			_serial = other._serial;
			_icon = other._icon;
			_auditable = other._auditable;
			_alerts_enabled = other._alerts_enabled;
			_tag = other._tag;
			_stockStatus = other._stockStatus;
			_supplierID = other._supplierID;
			_supplierName = other._supplierName;
			_listAuditedItems = other._listAuditedItems;
			_listFolders = other._listFolders;
			_childAssets = other._childAssets;
		}

		#endregion Constructors

		#region Methods

		public void UpdateStockStatus (STOCKSTATUS newStockStatus)
		{
			// Ignore if no change actually made
			if (newStockStatus == _stockStatus)
				return;
				
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			// Audit the status change
			Asset oldAsset = new Asset(this);

			// Update the object and the database
			_stockStatus = newStockStatus;
			lwDataAccess.AssetUpdateStockStatus(_assetID, newStockStatus);

			// ...then audit the change
			AuditChanges(oldAsset);
		}


		/// <summary>
		/// Return the icon which should display for this asset.
		/// </summary>
		/// <returns></returns>
		public Bitmap DisplayIcon()
		{
			// Load the primary image for this asset - noting that for the 'standard' types 
			// (computer, laptop, server, domaincontroller) that we will add the StockStatus to the
			// name to more accurately affect the asset
			string displayIcon = _icon;
			if (displayIcon == "computer.png" || displayIcon == "laptop.png" || displayIcon == "domaincontroller.png" || displayIcon == "server.png")
			{
				string stockStub = "";
				switch (_stockStatus)
				{
				case STOCKSTATUS.disposed: stockStub = "_disposed"; break;
				case STOCKSTATUS.inuse: break; 
				case STOCKSTATUS.pendingdisposal: stockStub = "_pending"; break; 
				case STOCKSTATUS.stock: stockStub = "_stock"; break;
				}
				//
				displayIcon = displayIcon.Substring(0 ,displayIcon.Length - 4) + stockStub + ".png";
			}
						
			// From this we can get the icon
			Bitmap baseImage = IconMapping.LoadIcon(displayIcon, IconMapping.ICONSIZE.small);
			Bitmap displayImage = new Bitmap(16, 16);

			System.Drawing.Graphics graphics = Graphics.FromImage(displayImage);
			graphics.DrawImage(baseImage, new Point(0, 0));

			// Now add on (any) overlays for the agent status and reaudit request
			// First icon is a little indicator to show if a reaudit has been requested
			if (RequestAudit)
			{
				Bitmap reauditImage = Properties.Resources.asset_reaudit;
				graphics.DrawImage(reauditImage, new Point(0, 9));
			}

			// ..then the AuditAgent Status image in the top left 
			if (AgentStatus == Asset.AGENTSTATUS.deployed)
			{
				Bitmap agentImage = Properties.Resources.agent_stopped;
				graphics.DrawImage(agentImage, new Point(0, 0));
			}

			else if (AgentStatus == Asset.AGENTSTATUS.Running)
			{
				Bitmap agentImage = Properties.Resources.agent_active;
				graphics.DrawImage(agentImage, new Point(0, 0));
			}

			// Audited?  - Looks too messy so do not display this icon
			//if (LastAudit.Ticks != 0L)
			//{
			//    Bitmap agentImage = Properties.Resources.audited;
			//    graphics.DrawImage(agentImage, new Point(8, 3));
			//}

			// Return these icons
			return displayImage;
		}

		
		public string GetAttributeValue(string attribute)
		{
			List<string> listAttributes = (List<string>)_listAttributes;
			
			if (attribute == listAttributes[(int)eAttributes.assetname])
				return _name;
				
			else if (attribute == listAttributes[(int)eAttributes.category])
				return TypeAsString;

			else if (attribute == listAttributes[(int)eAttributes.domain])
				return _domain;
				
			else if (attribute == listAttributes[(int)eAttributes.ipaddress])
				return _ipaddress;

			else if (attribute == listAttributes[(int)eAttributes.lastaudit])
				return (_lastAudit.Ticks == 0) ? "<not audited>" : _lastAudit.ToString();
				
			else if (attribute == listAttributes[(int)eAttributes.location])
				return _fullLocation;
				
			else if (attribute == listAttributes[(int)eAttributes.macaddress])
				return _macAddress;
				
			else if (attribute == listAttributes[(int)eAttributes.make])
				return _make;

			else if (attribute == listAttributes[(int)eAttributes.model])
				return _model;

			else if (attribute == listAttributes[(int)eAttributes.serial])
				return _serial;

			else if (attribute == listAttributes[(int)eAttributes.stock_status])
				return GetStockStatusText(_stockStatus);

			else if (attribute == listAttributes[(int)eAttributes.suppliername])
				return _supplierName;

			else 
				return "";				
		}
		
		public static string GetAttributeName(eAttributes attribute)
		{
			List<string> listAttributes = (List<string>)_listAttributes;
			return listAttributes[(int)attribute];		
		}
		
		
		public static string TranslateDeploymentStatus (AGENTSTATUS status)
		{
			return (status == Asset.AGENTSTATUS.notdeployed) ? "Not Deployed"
					: (status == Asset.AGENTSTATUS.deployed) ? "Deployed [Not Running]" : "Deployed [Running]";
		}

		public override string ToString()
		{
			return this.Name;
		}

		protected string RationalizeDomainName(string domainName)
		{
			// Do we have a 'period' in the name?  
			int period = domainName.IndexOf('.');
			if (period == -1)
				return domainName;
			else
				return domainName.Substring(0, period);
		}


		/// <summary>
		/// Load all of the hardware / system items audited for this asset
		/// </summary>
		/// <returns>Count of items added to the list</returns>
		public int PopulateAuditedItems()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			_listAuditedItems = new AuditedItemList(lwDataAccess.GetAuditedItems(AssetID, "" ,true));
			return _listAuditedItems.Count;
		}


		public void SetAsSibling(Asset siblingAsset)
		{
			Location = siblingAsset.Location;
			LocationID = siblingAsset.LocationID;
			Domain = siblingAsset.Domain;
			DomainID = siblingAsset.DomainID;
		}


		/// <summary>
		/// Add a new asset to the database (or possibly update an existing item)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (AssetID == 0)
			{
				// Add the asset to the database
				_assetID = lwDataAccess.AssetAdd(this);

				// ...and log this event in the audit trail
				AuditTrailEntry ate = new AuditTrailEntry();
				ate.Date = DateTime.Now;
				ate.Class = AuditTrailEntry.CLASS.asset;
				ate.Type = AuditTrailEntry.TYPE.added;
				ate.Key = _name;
				ate.AssetID = _assetID;
				ate.AssetName = _name;
				ate.Username = System.Environment.UserName;
				lwDataAccess.AuditTrailAdd(ate);			
			}
			else
			{
				lwDataAccess.AssetUpdate(this);
			}
			return 0;
		}


		/// <summary>
		/// Update an existing asset in the database (or possibly add a new one)
		/// </summary>
		/// <returns></returns>
		public int Update()
		{
			return Add();
		}

		public int Update (Asset oldAsset)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (this._assetID == 0)
			{
				return Add();
			}
			else
			{
				lwDataAccess.AssetUpdate(this);
				AuditChanges(oldAsset);
			}	
			return 0;
		}
		
		
		
		/// <summary>
		/// Delete this asset from the database
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			// ...and audit the deletion
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.asset;
			ate.Type = AuditTrailEntry.TYPE.deleted;
			ate.Key = _name;
			ate.AssetID = 0;
			ate.AssetName = "";
			ate.Username = System.Environment.UserName;
			lwDataAccess.AuditTrailAdd(ate);

			// Delete the asset
			lwDataAccess.AssetDelete(this);
			return 0;
		}
		

		#region File System Handlers

		/// <summary>
		/// Load all of the File Systems audited for this asset
		/// </summary>
		/// <returns>Count of items added to the list</returns>
		public int PopulateFileSystem()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			_listFolders = new FileSystemFolderList(lwDataAccess.EnumerateFileSystemFolders(AssetID), lwDataAccess.EnumerateFileSystemFiles(AssetID));
			return _listFolders.Count;
		}


		#endregion File System Handlers

		#region Child Asset Handlers

		/// <summary>
		/// Load all of the Child Assets for this asset
		/// </summary>
		/// <returns>Count of items added to the list</returns>
		public int PopulateChildAssets()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			_childAssets = new AssetList(lwDataAccess.EnumerateChildAssets(AssetID), false);
			return _childAssets.Count;
		}


		#endregion File System Handlers

		#region Change Handling

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public void AuditChanges(Asset oldObject)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// The following fields are auditable for an Asset
			//
			//	Stock Status
			//	Category
			//  Type
			//	Make
			//	Model
			//	Serial
			//	Supplier
			//
			// Build a blank AuditTrailEntry
			AuditTrailEntry ate = BuildATE();

			// We only audit changes made for existing assets so ignore if we weren't passed an original
			if (oldObject != null)
			{
				// Stock Status
				if (_stockStatus != oldObject._stockStatus)
					AddChange(listChanges, ate, "Stock Status", Asset.GetStockStatusText(oldObject._stockStatus) ,Asset.GetStockStatusText(_stockStatus));
				
				// Asset Category / Tuype is the same thing
				if (_assetTypeID != oldObject._assetTypeID)
					AddChange(listChanges , ate , "Asset Type" ,oldObject.TypeAsString , this.TypeAsString);

				// Make
				if (_make != oldObject._make)
					AddChange(listChanges , ate , "Make" ,oldObject._make, this._make);

				// Model
				if (_model != oldObject._model)
					AddChange(listChanges , ate , "Model" ,oldObject._model, this._model);

				// Serial
				if (_serial != oldObject._serial)
					AddChange(listChanges, ate, "Serial Number", oldObject._serial, this._serial);


				// Add all of these changes to the Audit Trail
				foreach (AuditTrailEntry entry in listChanges)
				{
					lwDataAccess.AuditTrailAdd(entry);
				}
			}
		
			// Return the constructed list
			return;
		}


		/// <summary>
		/// Construct a raw Audit Trail Entry and initialize it
		/// </summary>
		/// <returns></returns>
		protected AuditTrailEntry BuildATE()
		{
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.asset;
			ate.Type = AuditTrailEntry.TYPE.changed;
			ate.Key = _name;
			ate.AssetID = _assetID;
			ate.AssetName = _name;
			ate.Username = System.Environment.UserName;

			return ate;
		}


		/// <summary>
		/// Wrapper around adding an Audit Trail Entry to the list and re-creating the base ATE object
		/// </summary>
		/// <param name="listChanges"></param>
		/// <param name="ate"></param>
		/// <param name="key"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		protected AuditTrailEntry AddChange(List<AuditTrailEntry> listChanges, AuditTrailEntry ate, String key, String oldValue, String newValue)
		{
			ate.Key = ate.Key + "|" + key;
			ate.OldValue = oldValue;
			ate.NewValue = newValue;
			listChanges.Add(ate);
			ate = BuildATE();
			return ate;
		}

		#endregion Change Handling


		#endregion Methods
	}

	#endregion Asset Class

	#region AssetList class

	/// <summary>
	/// This object implements a list of <seealso cref="Asset"/>objects
	/// </summary>
	public class AssetList : List<Asset>
	{
		/// <summary>
		/// Base constructor 
		/// </summary>
		public AssetList()
		{
		}

		/// <summary>
		/// This constructor takes a data table with a list of assets which will be added.
		/// Note we can optionally exclude Child Assets from the returned list
		/// </summary>
		public AssetList(DataTable assets ,bool ignoreChildAssets)
		{
			foreach (DataRow row in assets.Rows)
			{
				Asset newAsset = new Asset(row);
				if (ignoreChildAssets && newAsset.ParentAssetID != 0)
					continue;
				this.Add(new Asset(row));
			}
		}

		
		// Methods
		public new int Add(Asset theComputer)
		{
			Asset findComputer = this.GetAssetById(theComputer.AssetID);
			if (findComputer != null)
			{
				findComputer.Name = theComputer.Name;
				findComputer.Location = theComputer.Location;
				findComputer.Domain = theComputer.Domain;
				findComputer.IPAddress = theComputer.IPAddress;
				findComputer.LastAudit = theComputer.LastAudit;
				findComputer.RequestAudit = theComputer.RequestAudit;
				findComputer.AgentStatus = theComputer.AgentStatus;
			}
			else
			{
				base.Add(theComputer);
			}
			return 0;
		}

		public Asset GetAssetById(int thisID)
		{
			foreach (Asset thisComputer in this)
			{
				if (thisComputer.AssetID == thisID)
				{
					return thisComputer;
				}
			}
			return null;
		}

		public override string ToString()
		{
			string computerString = "";
			foreach (Asset computer in this)
			{
				if (computerString == "")
					computerString = computer.Name;
				else
					computerString = computerString + ";" + computer.Name;
			}
			return computerString;
		}

	}
	#endregion ComputerList class
}

