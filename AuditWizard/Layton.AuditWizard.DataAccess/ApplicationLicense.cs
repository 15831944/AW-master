using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	/// <summary>
	/// This class defines an instance of a license for an application
	/// </summary>
	public class ApplicationLicense
	{
		#region Data
		// Fields
		private int _licenseID;
		private int _applicationID;
		private string _applicationName;
		private int _count;
		private int _licenseTypeID;
		private string _licenseTypeName;
		private bool _usageCounted;

		// Support Data
		protected bool _supported;
		protected DateTime _supportExpiryDate;
		protected int _supportAlertDays;
		protected bool _supportAlertEmail;
		protected string _supportAlertReciepients;

		// Supplier
		private string _supplierName;
		private int _supplierID;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#endregion Data
	
		#region Properties

		// Properties
		public int ApplicationID
		{
			get { return this._applicationID; }
			set { this._applicationID = value; }
		}

		public String ApplicationName
		{
			get { return this._applicationName; }
			set { this._applicationName = value; }
		}

		public int Count
		{
			get { return this._count; }
			set { this._count = value; }
		}

		public int LicenseID
		{
			get { return this._licenseID; }
			set { this._licenseID = value; }
		}

		public int LicenseTypeID
		{
			get { return this._licenseTypeID; }
			set { this._licenseTypeID = value; }
		}

		public string LicenseTypeName
		{
			get { return this._licenseTypeName; }
			set { this._licenseTypeName = value; }
		}

		public bool UsageCounted
		{
			get { return this._usageCounted; }
			set { this._usageCounted = value; }
		}

		/// <summary>Is this product supported (or at least this instance)</summary>
		public bool Supported
		{
			get { return this._supported; }
			set { this._supported = value; }
		}

		/// <summary>If supported the exiry date of said support contract</summary>
		public DateTime SupportExpiryDate
		{
			get { return this._supportExpiryDate; }
			set { this._supportExpiryDate = value; }
		}

		/// <summary>If supported the number of days prior to support expiring to begin alerting</summary>
		public int SupportAlertDays
		{
			get { return this._supportAlertDays; }
			set { this._supportAlertDays = value; }
		}

		/// <summary>If supported whether or not to alert the administrator via email</summary>
		public bool SupportAlertEmail
		{
			get { return this._supportAlertEmail; }
			set { this._supportAlertEmail = value; }
		}

		public string SupportAlertRecipients
		{
			get { return this._supportAlertReciepients; }
			set { this._supportAlertReciepients = value; }
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

		#region Constructor
		// Methods
		public ApplicationLicense()
		{
			this._licenseID = 0;
			this._applicationID = 0;
			this._applicationName = "";
			this._licenseTypeID = 0;
			this._licenseTypeName = "";
			this._usageCounted = false;
			this._count = 0;
			this._supported = false;
			this._supportExpiryDate = DateTime.Now;
			this._supportAlertDays = -1;
			this._supportAlertEmail = false;
			this._supportAlertReciepients = "";
			this._supplierID = 1;
			this._supplierName = "";
		}

		public ApplicationLicense(DataRow dataRow) : this()
		{
			try
			{
				this.LicenseID			= (int) dataRow["_LICENSEID"];
				this._applicationID		= (int)dataRow["_APPLICATIONID"];
				this._applicationName	= (String)dataRow["APPLICATION_NAME"];
				this._licenseTypeID = (int)dataRow["_LICENSETYPEID"];
				this._licenseTypeName	= (string) dataRow["LICENSE_TYPES_NAME"];
				this._usageCounted		= (bool) dataRow["_COUNTED"];
				this._count				= (int) dataRow["_COUNT"];

				// Support data
				this.Supported = (bool)dataRow["_SUPPORTED"];
				if (this.Supported)
				{
					this.SupportExpiryDate = (DateTime)dataRow["_SUPPORT_EXPIRES"];
					this.SupportAlertDays = (int)dataRow["_SUPPORT_ALERTDAYS"];
					this.SupportAlertEmail = (bool)dataRow["_SUPPORT_ALERTBYEMAIL"];
					this.SupportAlertRecipients = (string)dataRow["_SUPPORT_ALERTRECIPIENTS"];
				}

				// Supplier
				this.SupplierID = (int)dataRow["_SUPPLIERID"];
				this.SupplierName = (string)dataRow["SUPPLIER_NAME"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an APPLICATIONLICENSE Object, please check database schema.  The message was " + ex.Message);
			}
		}
		
		// Copy Constructor
		public ApplicationLicense(ApplicationLicense theLicense)
		{
			this.LicenseID = theLicense.LicenseID;
			this.ApplicationID = theLicense.ApplicationID;
			this.ApplicationName = theLicense.ApplicationName;
			this.LicenseTypeID = theLicense.LicenseTypeID;
			this.LicenseTypeName = theLicense.LicenseTypeName;
			this.UsageCounted = theLicense.UsageCounted;
			this.Count = theLicense.Count;
			//
			this.Supported = theLicense.Supported;
			this.SupportExpiryDate = theLicense.SupportExpiryDate;
			this.SupportAlertDays = theLicense.SupportAlertDays;
			this.SupportAlertEmail = theLicense.SupportAlertEmail;
			this.SupportAlertRecipients = theLicense.SupportAlertRecipients;
			//
			this.SupplierID = theLicense.SupplierID;
			this.SupplierName = theLicense.SupplierName;
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
				ApplicationLicense other = obj as ApplicationLicense;

				if ((object)other != null)
				{
					bool equality;
					equality = other.LicenseID == LicenseID
							&& other.ApplicationID == ApplicationID
							&& other.ApplicationName == ApplicationName
							&& other.LicenseTypeID == LicenseTypeID
							&& other.LicenseTypeName == LicenseTypeName
							&& other.UsageCounted == UsageCounted
							&& other.Supported == Supported
							&& other.SupplierID == SupplierID
							&& other.SupplierName == SupplierName;

					// If still equal and we are supported then check the support fields also
					if (equality && Supported)
					{
						equality = other.SupportExpiryDate == SupportExpiryDate
							&& other.SupportAlertDays == other.SupportAlertDays
							&& other.SupportAlertEmail == other.SupportAlertEmail
							&& other.SupportAlertRecipients == other.SupportAlertRecipients;
					}

					return equality;
				}

			}
			return base.Equals(obj);
		}



#endregion Constructor

		#region Methods

		/// <summary>
		/// Add this APPLICATION LICENSE to the database
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			int status = -1;

			// If already exists then call the update instead
			if (_licenseID != 0)
				return Update(null);

			// Add the supplier to the database
			LicensesDAO lwDataAccess = new LicensesDAO();
			int id = lwDataAccess.LicenseAdd(this);
			if (id != 0)
			{
				AuditChanges(null);
				_licenseID = id;
				status = 0;
			}

			return status;
		}


		/// <summary>
		/// Update this defininition in the database
		/// </summary>
		/// <returns></returns>
		public int Update(ApplicationLicense oldLicense)
		{

			LicensesDAO lwDataAccess = new LicensesDAO();
			if (_licenseID == 0)
			{
				Add();
			}
			else if (oldLicense != this)
			{
				lwDataAccess.LicenseUpdate(this);
				AuditChanges(oldLicense);
			}

			return 0;
		}


		/// <summary>
		/// Delete the current license from the database
		/// </summary>
		public void Delete()
		{
			// Delete from the database
			LicensesDAO lwDataAccess = new LicensesDAO();
			lwDataAccess.LicenseDelete(this);

			// ...and audit the deletion
			AuditTrailEntry ate = BuildATE();
			ate.Type = AuditTrailEntry.TYPE.deleted;
			new AuditTrailDAO().AuditTrailAdd(ate);
		}

		#endregion Methods

		#region Change Handling

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public List<AuditTrailEntry> AuditChanges(ApplicationLicense oldObject)
		{
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();

			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// License ID and ApplicationID must not change as these are basic properties of 
			// the application license - Build a blank AuditTrailEntry
			AuditTrailEntry ate = BuildATE();

			// Is this a new license rather than one we are modifying?
			if (LicenseID == 0)
			{
				ate.Type = AuditTrailEntry.TYPE.added;
				AddChange(listChanges, ate, "", "", "");
			}

			else if (oldObject != null)
			{
				// License Type ID - note that if we change this we ignore related changes such as to the
				// usage counted or count fields as these will change if the type is changed
				if (this.LicenseTypeID != oldObject.LicenseTypeID)
				{
					ate = AddChange(listChanges, ate, "License Type", oldObject.LicenseTypeName, LicenseTypeName);
				}
				else
				{
					// Usage Counted
					if (this.UsageCounted != oldObject.UsageCounted)
						ate = AddChange(listChanges
									   , ate
									   , "Usage Counted"
									   , (oldObject.UsageCounted) ? "Yes" : "No"
									   , (UsageCounted) ? "Yes" : "No");

					// Usage Count
					if (this.Count != oldObject.Count)
						ate = AddChange(listChanges
										, ate
										, "Usage Count"
										, oldObject.Count.ToString()
										, Count.ToString());
				}

				// Supported Status
				if (Supported != oldObject.Supported)
					ate = AddChange(listChanges
								  , ate
								  , "Support Status"
								  , (oldObject.Supported) ? "Yes" : "No"
								  , (Supported) ? "Yes" : "No");

				// If we are supported then we need to log other support fields also
				if (Supported)
				{
					// Support Expiry Date
					if (SupportExpiryDate != oldObject.SupportExpiryDate)
						ate = AddChange(listChanges, ate, "Support Expiry Date", oldObject.SupportExpiryDate.ToString(), SupportExpiryDate.ToString());

					// Support Alert Days
					if (SupportAlertDays != oldObject.SupportAlertDays)
						ate = AddChange(listChanges, ate, "Support Expiry Alert Days", oldObject.SupportAlertDays.ToString(), SupportAlertDays.ToString());

					// Support Alert by Email Status
					if (SupportAlertEmail != oldObject.SupportAlertEmail)
						ate = AddChange(listChanges
									  , ate
									  , "Support Alert by Email"
									  , (oldObject.SupportAlertEmail) ? "Yes" : "No"
									  , (SupportAlertEmail) ? "Yes" : "No");

					// Support Alert Email Recipients
					if (this.SupportAlertRecipients != oldObject.SupportAlertRecipients)
						ate = AddChange(listChanges, ate, "Support Alert Email Recipients", oldObject.SupportAlertRecipients.ToString(), SupportAlertRecipients);
				}

				// Supplier
				if (SupplierID != oldObject.SupplierID)
					ate = AddChange(listChanges
								  , ate
								  , "Supplier"
								  , oldObject.SupplierName
								  , SupplierName);

			}

			// Add all of these changes to the Audit Trail
			foreach (AuditTrailEntry entry in listChanges)
			{
				lwDataAccess.AuditTrailAdd(entry);
			}

			// Return the constructed list
			return listChanges;
		}


		/// <summary>
		/// Construct a raw Audit Trail Entry and initialize it
		/// </summary>
		/// <returns></returns>
		protected AuditTrailEntry BuildATE()
		{
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.license;
			ate.Type = AuditTrailEntry.TYPE.changed;
			ate.Key = ApplicationName + "|" + _licenseTypeName;
			ate.AssetID = 0;
			ate.AssetName = "";
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
	}
}

