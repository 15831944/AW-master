using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.Common
{

#region InstalledOS class

	/// <summary>
	/// This class defines an instance of an Operating System
	/// </summary>
	public class InstalledOS
	{
#region Static Strings
		public static string UNKNOWN_OS = "<unknown>";
#endregion Static Strings

#region Local Data

		// Fields
		private int _osid;

		/// <summary>
		/// Number of installations detected
		/// </summary>
		private int _installCount;

		/// <summary>
		/// This is the actual name of the OS
		/// </summary>
		private string _name;

		/// <summary>
		/// This is the actual version of the OS
		/// </summary>
		private string _version;

		/// <summary>
		/// Flafg to show whether or not we have loaded data into this object
		/// </summary>
		private bool	_dataLoaded = false;
		
		/// <summary>
		/// This is the list of licenses defined for this OS
		/// </summary>
		private List<ApplicationLicense> _listLicenses = new List<ApplicationLicense>();

		/// <summary>
		/// This is the list of installed instances of this Operating System
		/// </summary>
		private List<OSInstance> _listInstances = new List<OSInstance>();

#endregion Local Data

#region Data Accessors

		public int OSid
		{
			get { return _osid; }
			set { _osid = value; }
		}

		public string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public string Version
		{
			get { return this._version; }
			set { _version = value; }
		}

		public int LicenseCount
		{
			get { return _listLicenses.Count; }
		}

		public int InstallCount
		{
			get { return _listInstances.Count; }
		}  

		public List<ApplicationLicense> Licenses
		{
			get { return _listLicenses; }
		}

		/// <summary>
		/// Return the internal list of instances of this application
		/// </summary>
		public List<OSInstance> Instances
		{
			get { return this._listInstances; }
		}

#endregion Data Accessors

#region Constructor
		// Methods
		public InstalledOS(string name, int id)
		{
			this._name = (name == "") ? UNKNOWN_OS : name;
			this._osid = id;
			this._version = "";
		}

		public InstalledOS(DataRow dataRow)
		{
			try
			{
				_osid = (int)dataRow["_APPLICATIONID"];
				_name = String.Format("{0} {1}", (string)dataRow["_NAME"], (string)dataRow["_VERSION"]);
				_installCount = (int)dataRow["installCount"];
				_version = (string)dataRow["_VERSION"];
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating an INSTALLEDOS Object, please check database schema.  The message was " + ex.Message);
			}
		}
#endregion Constructor
	
#region Methods
		/// <summary>
		/// Iterate through the operating system list and recover their compliancy status - if any
		/// are not compliant then the publisher as a whole is non-compliant
		/// </summary>
		public bool IsCompliant()
		{
			return true;
		}


		/// <summary>
		/// Load this Application with it's instances and licenses
		/// </summary>
		public void LoadData()
		{
			// Read instances of this application
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable instancesTable = lwDataAccess.GetApplicationInstances(_osid);
			LoadInstances(instancesTable);

			// Read licenses for this application if any
			DataTable licensesTable = lwDataAccess.GetApplicationLicenses(_osid);
			LoadLicenses(licensesTable);
			
			// Flag that the data has already been loaded
			_dataLoaded = true;
		}


		/// <summary>
		/// Load instances of this Operating System from the supplied DataTable
		/// </summary>
		/// <param name="instancesTable"></param>
		protected void LoadInstances(DataTable instancesTable)
		{
			_listInstances.Clear();
			foreach (DataRow row in instancesTable.Rows)
			{
				try
				{
					OSInstance newInstance = new OSInstance(row);
					_listInstances.Add(newInstance);
				}
				catch (Exception)
				{
					// Just skip the instance as this points to an internal database consistency error
				}
			}
		}



		/// <summary>
		/// Load licenses for this application from the supplied DataTable
		/// </summary>
		/// <param name="licensesTable"></param>
		protected void LoadLicenses(DataTable licensesTable)
		{
			_listLicenses.Clear();
			foreach (DataRow row in licensesTable.Rows)
			{
				try
				{
					ApplicationLicense newLicense = new ApplicationLicense(row);
					_listLicenses.Add(newLicense);
				}
				catch (Exception)
				{
					// Just skip the license as this points to an internal database consistency error
				}
			}
		}


		/// <summary>
		/// Return (any) instance found for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public OSInstance GetInstanceForAsset(int assetID)
		{
			// Load instances if not done so already
			if (!_dataLoaded)
				LoadData();
				
			// Iterate through the instances looking for one for this asset
			foreach (OSInstance instance in _listInstances)
			{
				if (instance.InstalledOnComputerID == assetID)
					return instance;
			}
			return null;			
		}
#endregion Methods
	
	}

#endregion Operating System Class

#region InstalledOSList class

	public class InstalledOSList : List<InstalledOS>
	{

#region Constructor

		/// <summary>
		/// Constructor = this will populate the InstalledOSList
		/// </summary>
		public InstalledOSList()
		{
			// Now build the list of Operationg Systems
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable osTable = lwDataAccess.GetOperatingSystems();

			// Iterate through the returned data 
			foreach (DataRow row in osTable.Rows)
			{
				try
				{
					InstalledOS theOS = new InstalledOS(row);
					this.Add(theOS);

					// Read instances of this OS
					theOS.LoadData();
				}

				catch (Exception)
				{
					// Just skip the OS as this points to an internal database consistency error
				}
			}
		}
#endregion Constructor

#region Methods


		/// <summary>
		/// Return a compliancy status either for all data read or for a specific publisher
		/// </summary>
		/// <param name="publisher">If null, check all publishers otherwise check the specified publisher</param>
		/// <returns></returns>
		public bool IsCompliant()
		{
			foreach (InstalledOS theOS in this)
			{
				if (!theOS.IsCompliant())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Return a count of the total number of licenses declared 
		/// </summary>
		/// <returns></returns>
		public int LicenseCount()
		{
			int licenseCount = 0;
			foreach (InstalledOS theOS in this)
			{
				licenseCount += theOS.LicenseCount;
			}
			return licenseCount;
		}
		
		
		
		/// <summary>
		/// Return (any) OS Instance which we can find for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public OSInstance GetInstanceForAsset(int assetID)
		{
			foreach (InstalledOS installedOS in this)
			{
				OSInstance foundInstance = installedOS.GetInstanceForAsset(assetID);
				if (foundInstance != null)
					return foundInstance;
			}
			return null;
		}
		
#endregion
	}
#endregion InstalledApplicationList class

}

