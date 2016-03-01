using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
//using System.ServiceProcess;
using System.Windows.Forms;
//
using Microsoft.Win32;
//
//using Layton.Common.Controls;

namespace Layton.AuditWizard.DataAccess
{
	#region ApplicationInstance Class

	public class ApplicationInstance
	{
		#region Data

		// Fields
		protected string _guid;
		protected int _instanceid;
		protected int _applicationID;
		protected string _installedOnComputer;
		protected int	 _installedOnComputerID;
		protected string _installedOnComputerIcon;
		protected string _computerLocation;
		protected string _name;
		protected string _publisher;
		protected ApplicationSerial _serial;
		protected string _source;
		protected string _version;
		protected bool _ignored;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#endregion Data

		#region Properties

		// Properties
		/// <summary>This is the product GUID if any held in the registry for this application</summary>
		public string Guid
		{
			get { return this._guid; }
			set { this._guid = value; }
		}

		/// <summary>Database ID of this instance (in Applications_instances table)</summary>
		public int InstanceID
		{
			get { return this._instanceid; }
			set { this._instanceid = value; }
		}

		/// <summary>Database ID of this instance (in Applications_instances table)</summary>
		public int ApplicationID
		{
			get { return this._applicationID; }
			set { this._applicationID = value; }
		}

		/// <summary>Name of the computer on which this instance has been installed</summary>
		public string InstalledOnComputer
		{
			get { return this._installedOnComputer; }
			set { _installedOnComputer = value; }
		}

		/// <summary>Icon to display for the computer on which this instance has been installed</summary>
		public string InstalledOnComputerIcon
		{
			get { return this._installedOnComputerIcon; }
			set { _installedOnComputerIcon = value; }
		}

		/// <summary>Name of the location for the computer</summary>
		public string ComputerLocation
		{
			get { return this._computerLocation; }
			set { _computerLocation = value; }
		}

		/// <summary>ID of the computer on which this instance has been installed</summary>
		public int InstalledOnComputerID
		{
			get { return this._installedOnComputerID; }
			set { _installedOnComputerID = value; }
		}

		/// <summary>Name of the application of which this is an instance</summary>
		public string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		/// <summary>Name of the publisher of this application</summary>
		public string Publisher
		{
			get { return this._publisher; }
			set { this._publisher = (value == "") ? DataStrings.UNIDENIFIED_PUBLISHER : value; }
		}

		/// <summary>Serial number object for this instance</summary>
		public ApplicationSerial Serial
		{
			get { return this._serial; }
			set { this._serial = value; }
		}

		/// <summary>Where this instance was identified</summary>
		public string Source
		{
			get { return this._source; }
			set { this._source = value; }
		}

		/// <summary>Specific version of the application of which this is an instance</summary>
		public string Version
		{
			get { return this._version; }
			set { this._version = value; }
		}

		public bool IsIgnored
		{
			get { return _ignored; }
			set { _ignored = value; }
		}
		#endregion Properties

		#region Constructor

		// Methods
		public ApplicationInstance()
		{
			this._instanceid = 0;
			this._applicationID = 0;
			this._name = "";
			this._installedOnComputer = "";
			this._installedOnComputerIcon = "";
			this._installedOnComputerID = 0;
			this._computerLocation = "";
			this._serial = new ApplicationSerial();
			this._version = "";
			this._publisher = "";
			this._guid = "";
			this._source = "";
			this._ignored = false;
		}

		
		// Copy Constructor
		public ApplicationInstance(ApplicationInstance theInstance)
		{
			InstanceID = theInstance.InstanceID;
			ApplicationID = theInstance.ApplicationID;
			Name = theInstance.Name;
			InstalledOnComputer = theInstance.InstalledOnComputer;
			InstalledOnComputerIcon = theInstance.InstalledOnComputerIcon;
			InstalledOnComputerID = theInstance.InstalledOnComputerID;
			ComputerLocation = theInstance.ComputerLocation;
			Serial = new ApplicationSerial(theInstance.Serial);
			Version = theInstance.Version;
			Publisher = theInstance.Publisher;
			Guid = theInstance.Guid;
			Source = theInstance.Source;
			IsIgnored = theInstance.IsIgnored;
		}

		// Constructor from DataRow
		public ApplicationInstance(DataRow dataRow) : this()
		{
			try
			{
				this.InstanceID = (int)dataRow["_INSTANCEID"];
				this.ApplicationID = (int)dataRow["_APPLICATIONID"];
				this.Name = (string)dataRow["_NAME"];
				this.InstalledOnComputerID = (int)dataRow["_ASSETID"];
				this.InstalledOnComputer = (string)dataRow["ASSETNAME"];
				this.InstalledOnComputerIcon = (string)dataRow["ASSETICON"];
				this.ComputerLocation = (string)dataRow["FULLLOCATIONNAME"];
				string productID = (string)dataRow["_PRODUCTID"];
				string cdKey = (string)dataRow["_CDKEY"];
				this.Serial = new ApplicationSerial("", "", productID, cdKey);
				this.Version = (string)dataRow["_VERSION"];
				this.Publisher = (string)dataRow["_PUBLISHER"];
				this.Guid = (string)dataRow["_GUID"];
				this.IsIgnored = (bool)dataRow["_IGNORED"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an APPLICATIONINSTANCE Object, please check database schema.  The message was " + ex.Message);
			}
		}


		/// <summary>
		/// Equality test - check to see if this instance matches another
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType().Equals(this.GetType()))
			{
				ApplicationInstance other = obj as ApplicationInstance;

				if ((object)other != null)
				{
					bool equality;
					equality = other.InstanceID == InstanceID
							&& other.Name == Name
							&& other.InstalledOnComputer == InstalledOnComputer
							&& other.InstalledOnComputerID == InstalledOnComputerID
							&& other.Serial == Serial
							&& other.Version == Version
							&& other.Publisher == Publisher
							&& other.Guid == Guid;
					return equality;
				}

			}
			return base.Equals(obj);
		}

		#endregion Constructor

		#region Public Methods

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public List<AuditTrailEntry> ListChanges(ApplicationInstance oldObject)
		{
			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// The Publisher, Application Name, Version and Installed Asset must not change
			// as these are basic properties of the application instance.
			// Build a blank AuditTrailEntry
			AuditTrailEntry ate = BuildATE();
			if (this.Serial.ProductId != oldObject.Serial.ProductId)
				ate = AddChange(listChanges ,ate ,"Serial Number" ,oldObject.Serial.ProductId ,Serial.ProductId);

			// CD Key
			if (this.Serial.CdKey != oldObject.Serial.CdKey)
				ate = AddChange(listChanges ,ate ,"CD Key" ,oldObject.Serial.CdKey ,Serial.CdKey);

			// Return the constructed list
			return listChanges;
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Construct a raw Audit Trail Entry and initialize it
		/// </summary>
		/// <returns></returns>
		protected AuditTrailEntry BuildATE()
		{
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.application_changes;
			ate.Type = AuditTrailEntry.TYPE.changed;
			ate.AssetID = InstalledOnComputerID;
			ate.AssetName = this.InstalledOnComputer;
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
			ate.Key = Name + " | " + key;
			ate.OldValue = oldValue;
			ate.NewValue = newValue;
			listChanges.Add(ate);
			ate = BuildATE();
			return ate;
		}
		#endregion Protected Methods
	}
	#endregion ApplicationInstance Class

	#region ApplicationInstanceList Class

	/// <summary>
	/// This class implements a list of ApplicationInstance objects
	/// </summary>
	public class ApplicationInstanceList : List<ApplicationInstance>
	{
		#region Events

		// Events
		public delegate bool LogProgress(AuditTrailMessage thisMessage);
		public event LogProgress LogEvent;

		#endregion Events

		#region Methods

		// Find an application in the list
		public ApplicationInstance FindInstance(string computer)
		{
			int index = GetIndex(computer);
			return (index == -1) ? null : this[index];
		}


		/// <summary>
		/// Return the index of the application instance for the specified computer
		/// </summary>
		/// <param name="computer"></param>
		/// <returns></returns>
		public int GetIndex(string computer)
		{
			int index = 0;
			foreach (ApplicationInstance theInstance in this)
			{
				if (theInstance.InstalledOnComputer == computer)
					return index;
				index++;
			}

			return -1;
		}

		/// <summary>
		/// Does the list contain an application instance for the specified computer?
		/// </summary>
		/// <param name="computer"></param>
		/// <returns></returns>
		public bool Contains(string computer)
		{
			return (GetIndex(computer) != -1);
		}

		// Methods
		protected void AddApplicationInstance(string name, string publisher, string guid, string serialNumber, string version)
		{
			ApplicationInstance existingApplication = this.ContainsApplication(name);
			if ((existingApplication == null) && (guid != ""))
			{
				existingApplication = this.ContainsGuid(guid);
			}

			if (existingApplication != null)
			{
				if (existingApplication.Publisher == "")
					existingApplication.Publisher = publisher;

				if (existingApplication.Guid == "")
					existingApplication.Guid = guid;

				if ((existingApplication.Serial == null) && (serialNumber != ""))
					existingApplication.Serial = new ApplicationSerial(name, guid, serialNumber, "");

				if (existingApplication.Version == "")
					existingApplication.Version = version;
			}
			else
			{
				ApplicationInstance newApplication = new ApplicationInstance();
				newApplication.Name = name;
				newApplication.Publisher = publisher;
				newApplication.Version = version;
				newApplication.Guid = guid;
				if (serialNumber != "")
					newApplication.Serial = new ApplicationSerial(name, guid, serialNumber, "");
				newApplication.Source = "Windows Installer Key";

				// ...add to our list
				base.Add(newApplication);
			}
		}

		public ApplicationInstance ContainsApplication(string value)
		{
			foreach (ApplicationInstance thisApplication in this)
			{
				if (thisApplication.Name == value)
					return thisApplication;
			}
			return null;
		}

		public ApplicationInstance ContainsGuid(string value)
		{
			foreach (ApplicationInstance thisApplication in this)
			{
				if (thisApplication.Guid == value)
					return thisApplication;
			}
			return null;
		}

		#endregion Methods

		#region Detection Functions


		public static string GetApplicationName(RegistryKey rootKey, string valueName)
		{
			string applicationName = "";
			applicationName = rootKey.GetValue(valueName, "") as string;
			try
			{
				applicationName = applicationName.Trim();
				applicationName.Replace("[", "");
				applicationName.Replace("]", "");
				applicationName.Replace("=", "");
				if (applicationName.StartsWith("Add or Remove Adobe"))
				{
					applicationName = applicationName.Replace("Add or Remove", "");
				}
			}
			catch (Exception)
			{
			}
			return applicationName;
		}

		#endregion Detection Functions
	}

	#endregion ApplicationInstanceList Class

}

 
