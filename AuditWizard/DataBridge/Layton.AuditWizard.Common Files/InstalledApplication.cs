using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	#region Installed Application Class

	public class InstalledApplication
	{
		#region Data

		private int		_applicationID;
		private bool	_Ignored;
		private int		_installCount;
		private string	_name;
		private string	_publisher;
		private bool	_instancesLoaded = false;
		private bool	_licensesLoaded = false;
		private int		_aliasedToID;
		private bool	_userDefined;
		private int		_assignedFileID;
		
		/// <summary>
		/// This is the list of installed instances of this application
		/// </summary>
		private ApplicationInstanceList _listInstances = new ApplicationInstanceList();

		/// <summary>
		/// This is the list of licenses which have been declared for this application
		/// </summary>
		private List<ApplicationLicense> _listLicenses = new List<ApplicationLicense>();
		
		#endregion Data

		#region Constructor

		// Methods
		public InstalledApplication()
		{
			_applicationID = 0;
			_publisher = "";
			_name = "";
			_installCount = 0;
			_Ignored = false;
			_listInstances.Clear();
			_listLicenses.Clear();
			_aliasedToID = 0;
			_userDefined = false;
			_assignedFileID = 0;
		}
		
		public InstalledApplication(DataRow dataRow) : this()
		{
			try
			{
				_applicationID	= (int) dataRow["_APPLICATIONID"];
				string publisher= (string) dataRow["_PUBLISHER"];
				_publisher		= (publisher == "") ? DataStrings.UNIDENIFIED_PUBLISHER : publisher;
				_name			= (string) dataRow["_NAME"];
				IsIgnored		= (bool)dataRow["_IGNORED"];
				_aliasedToID	= (int)dataRow["_ALIASED_TOID"];
				_userDefined	= (bool)dataRow["_USER_DEFINED"];
				
				// We may have the install count
				if (dataRow.Table.Columns.Contains("INSTALLCOUNT"))
				{
					if (dataRow.IsNull("INSTALLCOUNT"))
						_installCount = 0;
					else
						_installCount = (int)dataRow["INSTALLCOUNT"];
				}
					
				// Assigned File ID
				_assignedFileID = (int)dataRow["_ASSIGNED_FILEID"];
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception occured creating an InstalledApplication Object, please check database schema.  The message was " + ex.Message);
			}
		}


		#endregion Constructor

		#region Properties

		// Properties
		public int ApplicationID
		{
			get { return this._applicationID; }
			set { this._applicationID = value; }
		}

		public bool IsIgnored
		{
			get { return this._Ignored; }
			set { this._Ignored = value; }
		}

		public string Name
		{
			get { return this._name; }
			set { _name = value; }
		}

		public string Publisher
		{
			get { return this._publisher; }
			set { _publisher = value; }
		}


		/// <summary>
		/// Return the internal list of licenses declared for this application
		/// </summary>
		public List<ApplicationLicense> Licenses
		{
			get { return this._listLicenses; }
		}


		/// <summary>
		/// Return the internal list of instances of this application
		/// </summary>
		public ApplicationInstanceList Instances
		{
			get { return this._listInstances; }
		}

		/// <summary>
		/// Returns the database ID of any FS_FILE table record which has been assigned to this application 
		/// An assigned application is one which has been detected by the presence of a file rather than from
		/// an entry contained within the Add/Remove programs section of the Windows Registry
		/// </summary>
		public int AssignedFileID
		{
			get { return _assignedFileID; }
			set { _assignedFileID = value; }
		}


		/// <summary>
		/// Returns the database ID of any application to which this application has been aliased
		/// aliasing means that this application will always be referred to as its alias
		/// </summary>
		public int AliasedToID
		{
			get { return _aliasedToID; }
			set { _aliasedToID = value; }
		}

		/// <summary>
		/// This field defines whether or not the application was originally defined by the user and was not 
		/// picked up as part of an audit.  User defined applications are not automatically purged by the
		/// database cleanup code which would normally remove orphaned applications.
		/// </summary>
		public bool UserDefined
		{
			get { return _userDefined; }
			set { _userDefined = value; }
		}

		#endregion Properties

		#region Methods

		public override string ToString()
		{
			return Name;
		}


		public int Add()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			_applicationID = lwDataAccess.ApplicationAdd(this);
			return _applicationID;
		}
		
		
		/// <summary>
		/// Load this Application with it's instances and licenses
		/// </summary>
		public void LoadData()
		{
			// Read instances of this application
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable instancesTable = lwDataAccess.GetApplicationInstances(_applicationID);
			LoadInstances(instancesTable);

			// Read licenses for this application if any
			DataTable licensesTable = lwDataAccess.GetApplicationLicenses(_applicationID);
			LoadLicenses(licensesTable);
		}



		/// <summary>
		/// Load instances of this application from the supplied DataTable
		/// </summary>
		/// <param name="instancesTable"></param>
		public void LoadInstances(DataTable instancesTable)
		{
			_listInstances.Clear();
			foreach (DataRow row in instancesTable.Rows)
			{
				try
				{
					ApplicationInstance newInstance = new ApplicationInstance(row);
					_listInstances.Add(newInstance);
				}
				catch (Exception)
				{
					// Just skip the instance as this points to an internal database consistency error
				}
			}
			_instancesLoaded = true;
		}


		/// <summary>
		/// Load licenses for this application from the supplied DataTable
		/// </summary>
		/// <param name="licensesTable"></param>
		public void LoadLicenses(DataTable licensesTable)
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
			_licensesLoaded = true;
		}



		/// <summary>
		/// Search for an application instance record for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public ApplicationInstance FindInstance (int assetID)
		{
			// First have we loaded instances yet?  If not do so now
			if (!_instancesLoaded)
				LoadData();
			
			// Loop throug (any) instances found	
			foreach (ApplicationInstance thisInstance in _listInstances)
			{
				if (thisInstance.InstalledOnComputerID == assetID)
					return thisInstance;
			}
			
			return null;
		}
		
		
		
		/// <summary>
		/// Return a total for all licenses defined
		/// </summary>
		/// <returns>
		/// -1 - Unlimited license (non-counted) found
		/// 0 - no licenses declared
		/// >0 Count of per-computer licenses declared
		/// </returns>
		public int LicenseCount()
		{
			// Assume no licenses for this application
			int licenseCount = 0;

			// ...and iterate through any defined licenses adding up the total
			foreach (ApplicationLicense theLicense in _listLicenses)
			{
				// If this license doesn't use usage counting then treat as unlimited
				if (!theLicense.UsageCounted)
				{
					licenseCount = -1;
					break;
				}
				else
				{
					licenseCount += theLicense.Count;
				}
			}
			return licenseCount;
		}

		/// <summary>
		/// Return a count of the number of installs of this application
		/// </summary>
		public int InstallCount()
		{
			return this._installCount;
		}  

		/// <summary>
		/// This function returns a simply compliant/is not compliant status by checking the application
		/// and any licenses declared and applying the following rules:
		/// 
		/// 1> Check the number of instances against (any) defined license count
		///		If licenses have been declared then these must be greater or equal to instances
		///		If no licenses declared then step 2
		/// 
		/// 2> Check serial numbers
		///		if captured but not unique then not compliant
		///		if none captured then non-compliant
		///		if captured and all unique then compliant
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsCompliant()
		{
			// If the application is IGNORED then it must be compliant
			if (IsIgnored)
				return true;

			// Do we have any licenses declared?
			int licenseCount = LicenseCount();
			if (licenseCount == -1)
				return true;

			else if (licenseCount > 0)
				return (InstallCount() <= licenseCount);

			// OK No licenses so check serial numbers
			List<String> serials = new List<String>(); ;
			foreach (ApplicationInstance thisInstance in _listInstances)
			{
				String serial = thisInstance.Serial.ProductId;

				// Ignore if no serial number
				if ((serial == "") || (serial.ToLower() == "not registered"))
					continue;

				// Reject duplicate serial numbers
				if (serials.Contains(serial))
					return false;

				// ...else add it to the list
				serials.Add(serial);
			}

			// Ok if we had no serial numbers then we will assume that we need licenses and therefore
			// this application is not compliant.
			return (serials.Count != 0);
		}


		/// <summary>
		/// This function returns textual license information which is often used in displays
		/// </summary>
		/// <param name="licenses"></param>
		/// <param name="variance"></param>
		public void GetLicenseStatistics(out String installs ,out String licenses, out String variance)
		{
			// What is the license count for this application?
			installs = InstallCount().ToString();
			int licenseCount = LicenseCount();

			// OK format the license count and variance for display
			if (LicenseCount() == -1)
			{
				licenses = "Unlimited";
				variance = "None";
			}

			else if (LicenseCount() == 0)
			{
				licenses = "None Specified";
				if (IsCompliant())
					variance = "None : Serial Numbers Checked";
				else
					variance = "Shortfall : " + InstallCount().ToString();
			}

			else
			{
				licenses = "Licenses for " + LicenseCount().ToString() + " Asset(s)";
				if (licenseCount == InstallCount())
					variance = "None : All Instances Licensed";
				else if (LicenseCount() < InstallCount())
					variance = "Shortfall : " + (InstallCount() - LicenseCount()).ToString();
				else
					variance = "Surplus : " + (LicenseCount() - InstallCount()).ToString();
			}
		}


		/// <summary>
		/// This function is called to determine whether any of the instances of this application have either
		/// a CD key or serial number
		/// </summary>
		/// <returns></returns>
		public bool HaveSerialNumbers()
		{
			foreach (ApplicationInstance thisInstance in _listInstances)
			{
				if (thisInstance.Serial.CdKey != "" || thisInstance.Serial.ProductId != "")
					return true;
			}
			return false;
		}

		#endregion Methods

	}

	#endregion InstalledApplication

	#region InstalledApplicationList Class

	public class InstalledApplicationList : List<InstalledApplication>
	{
		// Find an application in the list
		public InstalledApplication FindApplication(string application)
		{
			int index = GetIndex(application);
			return (index == -1) ? null : this[index];
		}
		
		
		/// <summary>
		/// Return the index of the application with the specified name
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public int GetIndex(string application)
		{
			int index = 0;
			foreach (InstalledApplication theApplication in this)
			{
				if (theApplication.Name == application)
					return index;
				index++;
			}

			return -1;
		}

		/// <summary>
		/// Does the list contain an application with the specified name?
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public bool Contains (string application)
		{
			return (GetIndex(application) != -1);
		}


		/// <summary>
		/// Override of ToString returns the list of application names as a semi-colon delimited string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string returnString = "";
			foreach (InstalledApplication application in this)
			{
				if (returnString != "")
					returnString += ";";
				returnString += application.Name;
			}
			return returnString;
		}
		
	}
	#endregion InstalledApplicationList Class
}

