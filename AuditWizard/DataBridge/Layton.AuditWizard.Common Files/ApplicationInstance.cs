using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
//
using Microsoft.Win32;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
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
				MessageBox.Show("Exception occured creating an APPLICATIONINSTANCE Object, please check database schema.  The message was " + ex.Message);
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

		#region Data

		// Fields
		protected ApplicationSerials _applicationSerials = new ApplicationSerials();
		protected bool _onlyMicrosoft;
		private Dictionary<string, string> _publisherMappings = new Dictionary<string, string>();

		/// <summary>
		/// The Application Definitions file contains a number of different mappings which help us to both
		/// audit and manage installed applications.
		/// </summary>
		private ApplicationDefinitionsFile _definitionsFile = new ApplicationDefinitionsFile();

		// Static strings
		private static string CLASSES_INSTALLERKEY = @"SOFTWARE\Classes\Installer\Products";
		private static string DISPLAYNAME = "DisplayName";
		private static string DISPLAYVERSION = "DisplayVersion";
		private static string INSTALLPROPERTIES = "InstallProperties";
		private static string MICROSOFT = "Microsoft";
		private static string PARENTKEYNAME = "ParentKeyName";
		private static string PARENTKEYNAME_OS = "OperatingSystem";
		private static string PRODUCTID = "ProductID";
		private static string PRODUCTNAME = "ProductName";
		private static string PUBLISHER = "Publisher";
		private static string PUBLISHERALIASES_SECTION = "PublisherAliases";
		private static string RELEASETYPE = "ReleaseType";
		private static string RELEASETYPE_SP = "Service Pack";
		private static string SERIALNUMBER = "SerialNumber";
		private static string SYSTEMCOMPONENT = "SystemComponent";
		private static string UNINSTALLKEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
		private static string WINDOWS_INSTALLERKEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products";
		//
		private static string REMOTE_REGISTRY_SERVICE = @"RemoteRegistry";

		#endregion Data

		#region Properties
		// Properties
		public int KeysScanned
		{
			get { return this._applicationSerials.KeysScanned; }
		}

		public int ValuesScanned
		{
			get { return this._applicationSerials.ValuesScanned; }
		}

		#endregion Properties

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

		/// <summary>
		/// Main Detection function.  The main cause of an error here is the inability to connect to the remote
		/// systems registry.  This will most often be down to either the remote registry service not running
		/// or the user having insufficient privilege to access the remote registry
		/// </summary>
		/// <param name="remoteCredentials"></param>
		/// <param name="onlyMicrosoft"></param>
		/// <returns></returns>
		public int Detect(RemoteCredentials remoteCredentials, ref String errorText)
		{
			Layton.Common.Controls.Impersonator impersonator = null;
			String remoteHost = remoteCredentials.RemoteHost;
			int status = 0;
			bool existingServiceStatus = false;

			try
			{
				// We may want to impersonate a different user so that we can audit remote computers - if so
				// start the impersonation here
				if (remoteCredentials.Username != null && remoteCredentials.Username != "")
					impersonator = new Impersonator(remoteCredentials.Username, remoteCredentials.Domain ,remoteCredentials.Password);

				// Ensure that the remote registry service is running on the remote Asset and that we can 
				// connect.  If the service is not active we may start it
				status = ValidateRemoteRegistryService(remoteCredentials, out existingServiceStatus);
				if (status != 0)
				{
					errorText = "The Remote Registry Service was not active and could not be started";
					return status;
				}

				// Now begin the audit
				this.ReadPublisherMappings();
				this._applicationSerials.Detect(remoteHost);
				RegistryKey rootCUKey;
				RegistryAccess registryAccess = new RegistryAccess();
				RegistryKey rootLMKey = null;

				// Attempt to connect to the remote registry and open the HKEY_LOCAL_MACHINE key
				status = registryAccess.OpenRegistry(Registry.LocalMachine, remoteHost, out rootLMKey);
				if (status != 0)
				{
					errorText = registryAccess.LastErrorText;
					return status;
				}

				// Attempt to connect to the remote registry and open the HKEY_CURRENT_USER key
				status = registryAccess.OpenRegistry(Registry.CurrentUser, remoteHost, out rootCUKey);
				if (status != 0)
				{
					errorText = registryAccess.LastErrorText;
					return status;
				}
				
				// Open the Softare uninstall key under HKLM and scan it
				RegistryKey uninstallKey = rootLMKey.OpenSubKey(UNINSTALLKEY);
				if (uninstallKey != null)
				{
					this.ScanUninstallKey(uninstallKey);
					uninstallKey.Close();
				}

				// Open the software uninstall key under HKCU and scan it
				uninstallKey = rootCUKey.OpenSubKey(UNINSTALLKEY);
				if (uninstallKey != null)
				{
					this.ScanUninstallKey(uninstallKey);
					uninstallKey.Close();
				}

				// Scan the Windows installer key
				this.ScanWindowsInstaller(rootLMKey);

				// close all open keys
				rootLMKey.Close();
				rootCUKey.Close();
			}
			catch (Exception ex)
			{
				errorText = "An exception occurred while trying to connect to the system registry on Asset [" + remoteHost + "] - The error was [" + ex.Message + "]";
				return -1;
			}

			finally
			{
				// Ensure that we restore the initial status of the remote registry service on the target computer
				RestoreRemoteRegistyStatus(remoteCredentials ,existingServiceStatus);

				// Displose of any impersonator object if created
				if (impersonator != null)
					impersonator.Dispose();
			}

			// Iterate through the applications detected by the above and attempt to recover any serial number
			foreach (ApplicationInstance thisApplication in this)
			{
				ApplicationSerial thisSerial = null;
				if (thisApplication.Guid != "")
					thisSerial = this._applicationSerials.ContainsApplication(thisApplication.Guid);

				if ((thisSerial == null) && (thisApplication.Name != ""))
					thisSerial = this._applicationSerials.ContainsApplication(thisApplication.Name);

				if (thisSerial != null)
				{
					thisApplication.Serial = thisSerial;
					thisSerial.Matched = true;
				}
			}

			errorText = "";
			return 0;
		}


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

		public List<ApplicationSerial> GetUnmatchedSerials()
		{
			List<ApplicationSerial> listUnmatched = new List<ApplicationSerial>();
			foreach (ApplicationSerial thisSerial in this._applicationSerials)
			{
				if (!thisSerial.Matched)
				{
					listUnmatched.Add(thisSerial);
				}
			}
			return listUnmatched;
		}

		protected bool IsInstalledApplication(RegistryKey applicationKey)
		{
			string displayName = GetApplicationName(applicationKey, DISPLAYNAME);
			if (displayName == "")
			{
				return false;
			}
			if (applicationKey.GetValue(SYSTEMCOMPONENT) != null)
			{
				int systemComponent = (int) applicationKey.GetValue(SYSTEMCOMPONENT);
				if (systemComponent == 1)
				{
					return false;
				}
			}
			if ((displayName.Contains("Hotfix") || displayName.Contains("Security Update")) || displayName.Contains("Update for"))
			{
				return false;
			}
			string releaseType = applicationKey.GetValue(RELEASETYPE, "") as string;
			if (releaseType == RELEASETYPE_SP)
			{
				return false;
			}
			string parentKeyName = applicationKey.GetValue(PARENTKEYNAME, "") as string;
			if (parentKeyName == PARENTKEYNAME_OS)
			{
				return false;
			}
			if (this._onlyMicrosoft && !(applicationKey.GetValue(PUBLISHER, "") as string).StartsWith(MICROSOFT))
			{
				return false;
			}
			return true;
		}

		protected string RationalizePublisher(string thePublisher)
		{
			string upcPublisher = thePublisher.ToUpper();
			foreach (KeyValuePair<string, string> kvp in this._publisherMappings)
			{
				if (upcPublisher.StartsWith(kvp.Key))
				{
					return kvp.Value;
				}
			}
			return thePublisher;
		}


		/// <summary>
		/// Read the mapping of publisher names and aliases from the ini file
		/// </summary>
		protected void ReadPublisherMappings()
		{
			try
			{
				// Select the PUBLISHERS section within the definitions file
				_definitionsFile.SetSection(PUBLISHERALIASES_SECTION);

				// ...and recover a list of the publishers
				List<string> listPublishers = new List<string>();
				_definitionsFile.EnumerateKeys(PUBLISHERALIASES_SECTION, listPublishers);
				foreach (string thisKey in listPublishers)
				{
					if (thisKey != "")
					{
						string value = _definitionsFile.GetString(thisKey, "");
						if (value != "")
						{
							this._publisherMappings.Add(thisKey, value);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		protected void ScanClassesInstaller(RegistryKey rootKey)
		{
			rootKey = rootKey.OpenSubKey(CLASSES_INSTALLERKEY);
			foreach (string subKeyName in rootKey.GetSubKeyNames())
			{
				try
				{
					RegistryKey applicationKey = rootKey.OpenSubKey(subKeyName);
					string applicationName = GetApplicationName(applicationKey, PRODUCTNAME);
					string version = applicationKey.GetValue(DISPLAYVERSION, "") as string;
					string publisher = this.RationalizePublisher(applicationKey.GetValue(PUBLISHER, "") as string);
					string productID = applicationKey.GetValue(PRODUCTID, "") as string;
					if (productID == "")
					{
						productID = applicationKey.GetValue(SERIALNUMBER, "") as string;
					}
					string productGUID = subKeyName.Substring(0, 8) + "-" + subKeyName.Substring(8, 4) + "-" + subKeyName.Substring(12, 4) + "-" + subKeyName.Substring(0x10, 4) + "-" + subKeyName.Substring(20, 8);
					this.AddApplicationInstance(applicationName, publisher, productGUID, productID, version);
				}
				catch (Exception)
				{
				}
			}
		}

		protected void ScanUninstallKey(RegistryKey uninstallKey)
		{
			foreach (string subKeyName in uninstallKey.GetSubKeyNames())
			{
				try
				{
					RegistryKey applicationKey = uninstallKey.OpenSubKey(subKeyName);
					if (this.IsInstalledApplication(applicationKey))
					{
						string displayName = GetApplicationName(applicationKey, DISPLAYNAME);
						string productGUID = "";
						if (subKeyName.StartsWith("{") && subKeyName.EndsWith("}"))
						{
							productGUID = subKeyName.Substring(1, subKeyName.Length - 2);
						}
						string version = applicationKey.GetValue(DISPLAYVERSION, "") as string;
						string publisher = this.RationalizePublisher(applicationKey.GetValue(PUBLISHER, "") as string);
						string productID = applicationKey.GetValue(PRODUCTID, "") as string;
						if (productID == "")
						{
							productID = applicationKey.GetValue(SERIALNUMBER, "") as string;
						}
						this.AddApplicationInstance(displayName, publisher, productGUID, productID, version);
						applicationKey.Close();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		protected void ScanWindowsInstaller(RegistryKey rootKey)
		{
			RegistryKey installerKey = rootKey.OpenSubKey(WINDOWS_INSTALLERKEY);
			if (installerKey == null)
				return;

			// Iterate through the keys
			foreach (string subKeyName in installerKey.GetSubKeyNames())
			{
				try
				{
					RegistryKey applicationKey = installerKey.OpenSubKey(subKeyName);
					if (applicationKey != null)
					{
						RegistryKey propertiesKey = applicationKey.OpenSubKey(INSTALLPROPERTIES);
						if ((propertiesKey != null) && this.IsInstalledApplication(propertiesKey))
						{
							string applicationName = GetApplicationName(propertiesKey, DISPLAYNAME);
							string version = propertiesKey.GetValue(DISPLAYVERSION, "") as string;
							string publisher = this.RationalizePublisher(propertiesKey.GetValue(PUBLISHER, "") as string);
							string productID = propertiesKey.GetValue(PRODUCTID, "") as string;
							if (productID == "")
							{
								productID = propertiesKey.GetValue(SERIALNUMBER, "") as string;
							}
							string productGUID = subKeyName.Substring(0, 8) + "-" + subKeyName.Substring(8, 4) + "-" + subKeyName.Substring(12, 4) + "-" + subKeyName.Substring(0x10, 4) + "-" + subKeyName.Substring(20, 8);
							this.AddApplicationInstance(applicationName, publisher, productGUID, productID, version);
							applicationKey.Close();
							propertiesKey.Close();
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public ApplicationInstance SetGuid(string application, string value)
		{
			ApplicationInstance thisApplication = this.ContainsApplication(application);
			if (thisApplication != null)
			{
				thisApplication.Version = value;
			}
			return thisApplication;
		}

		public ApplicationInstance SetSerial(string application, ApplicationSerial value)
		{
			ApplicationInstance thisApplication = this.ContainsApplication(application);
			if (thisApplication != null)
			{
				thisApplication.Serial = value;
			}
			return thisApplication;
		}

		public ApplicationInstance SetVersion(string application, string value)
		{
			ApplicationInstance thisApplication = this.ContainsApplication(application);
			if (thisApplication != null)
			{
				thisApplication.Version = value;
			}
			return thisApplication;
		}
		#endregion Detection Functions

		#region Remote Registry Functions

		/// <summary>
		/// Determine whether or not the remote registry service is active on the specified computer and if
		/// it is not start it, returning both the success or failoure of this action and the initial state of the
		/// service so that this can be restored at a later date if necessary
		/// </summary>
		/// <param name="remoteCredentials"></param>
		/// <param name="serviceWasRunning"></param>
		/// <returns></returns>
		protected int ValidateRemoteRegistryService(RemoteCredentials remoteCredentials, out bool serviceWasRunning)
		{
			try
			{
				ServiceController sc = new ServiceController(REMOTE_REGISTRY_SERVICE ,remoteCredentials.RemoteHost);
				ServiceControllerStatus existingStatus = sc.Status;
				serviceWasRunning = (existingStatus == ServiceControllerStatus.Running);
				if (!serviceWasRunning)
				{
					// The service is not running - try and start it
					WriteAuditTrailMessage(AuditTrailMessage.SEVERITY.INFORMATION ,"The remote registry service was not active on " + remoteCredentials.RemoteHost + ", attemptinmg to start it...");
					sc.Start();
					sc.WaitForStatus(ServiceControllerStatus.Running);
				}
			}
			catch (Exception ex)
			{
				WriteAuditTrailMessage(AuditTrailMessage.SEVERITY.INFORMATION, "An exception occured while checking the remote registry service on " + remoteCredentials.RemoteHost + ". The error was " + ex.Message);
				serviceWasRunning = true;			// Assume it was running as we don't want to touch it now!
				return -1;
			}

			// At this point the remote registry service should be running 
			return 0;
		}

		/// <summary>
		/// Restore the initial state of the remote registry service - by this we mean stop it if it was not
		/// running initially when we began the audit
		/// </summary>
		/// <param name="remoteCredentials"></param>
		/// <param name="serviceWasRunning"></param>
		protected void RestoreRemoteRegistyStatus(RemoteCredentials remoteCredentials, bool serviceWasRunning)
		{
			// Sanity check - if the service was initially running then leave it that way
			if (serviceWasRunning)
				return;

			try
			{
				ServiceController sc = new ServiceController(REMOTE_REGISTRY_SERVICE ,remoteCredentials.RemoteHost);
				sc.Stop();
			}
			catch (Exception)
			{
			}
		}


		/// <summary>
		/// Write a message to the audit trail
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="msg"></param>
		protected void WriteAuditTrailMessage(AuditTrailMessage.SEVERITY severity, String msg)
		{
			if (this.LogEvent != null)
				this.LogEvent(new AuditTrailMessage(severity, msg));
		}

		#endregion
	}

	#endregion ApplicationInstanceList Class

}

 
