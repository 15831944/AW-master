using System;
using System.Data;

namespace Layton.AuditWizard.DataAccess
{
	#region Discover and Audit Statistics

	/// <summary>
	/// This object encapsulates statistical information recovered relating to computers
	/// </summary>
	public class AssetStatistics
	{
		// Fields

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private int _audited;
		private int _discovered;
		private int _notaudited;
		private DateTime _mostRecentAudit;
		private int _stock;
		private int _inuse;
		private int _pending;
		private int _disposed;

		// Titles
		public const string TitleDiscovered = "Assets Discovered: ";
		public const string TitleAudited = "Assets Audited: ";
		public const string TitleNotAudited = "Assets Not Audited: ";
		public const string TitleStock = "Assets in Stock: ";
		public const string TitleInUse = "Assets In Use: ";
		public const string TitlePending = "Assets Pending Disposal: ";
		public const string TitleDisposed = "Assets Disposed: ";

		// Constructor
		public AssetStatistics()
		{
			_discovered = 0;
			_audited = 0;
			_notaudited = 0;
			_mostRecentAudit = DateTime.Now;
			_stock = 0;
			_inuse = 0;
			_pending = 0;
			_disposed = 0;
		}

		public AssetStatistics(DataRow dataRow)
		{
			try
			{
				_discovered	= Convert.ToInt32(dataRow["discovered"]);
                _audited = Convert.ToInt32(dataRow["audited"]);
                _notaudited = Convert.ToInt32(dataRow["notaudited"]);
                _stock = Convert.ToInt32(dataRow["stock"]);
                _inuse = Convert.ToInt32(dataRow["inuse"]);
                _pending = Convert.ToInt32(dataRow["pending"]);
                _disposed = Convert.ToInt32(dataRow["disposed"]);
				//
				if (dataRow.IsNull("mostrecentaudit"))
					_mostRecentAudit = new DateTime(0);
				else
					_mostRecentAudit = Convert.ToDateTime(dataRow["mostrecentaudit"]);
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an ASSETSTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public int Audited
		{
			get { return this._audited; }
			set { this._audited = value; }
		}

		public int NotAudited
		{
			get { return this._notaudited; }
			set { this._notaudited = value; }
		}

		public int Discovered
		{
			get { return this._discovered; }
			set { this._discovered = value; }
		}

		public int Stock
		{
			get { return _stock; }
			set { _stock = value; }
		}

		public int InUse
		{
			get { return _inuse; }
			set { _inuse = value; }
		}

		public int PendingDisposal
		{
			get { return _pending; }
			set { _pending = value; }
		}

		public int Disposed
		{
			get { return _disposed; }
			set { _disposed = value; }
		}

		public DateTime MostRecentAudit
		{
			get { return this._mostRecentAudit; }
			set { this._mostRecentAudit = value; }
		}
	}

	#endregion Discover and Audit Statistics

	#region Declare Licenses Statistics

	/// <summary>
	/// This object encapsulates statistical information recovered relating to the 'Declare licenses' phase
	/// </summary>
	public class DLStatistics
	{
		// Fields
		private int _uniqueApplications;
		private int _includedApplications;
		private int _licensesDeclared;
		private int _licenseCountDeclared;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Titles
		public const string TitleUniqueApplications = "Unique Applications: ";
		public const string TitleIncludedInstances = "Included Instances: ";
		public const string TitleLicensesDeclared = "Licenses Declared: ";
		public const string TitleCountDeclared = "License Count Declared: ";

		// Methods
		public DLStatistics()
		{
			_uniqueApplications = 0;
			_includedApplications = 0;
			_licenseCountDeclared = 0;
			_licensesDeclared = 0;
		}

		public DLStatistics(DataRow dataRow)
		{
			try
			{
				this._uniqueApplications = (int)dataRow["uniqueapplications"];
				this._includedApplications = (int)dataRow["includedapplicationinstances"];
				this._licensesDeclared = (int)dataRow["licensesdeclared"];
				this._licenseCountDeclared = (dataRow.IsNull("licenseinstancecount")) ? 0 : (int)dataRow["licenseinstancecount"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a DLStatistics Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public int UniqueApplications
		{
			get { return this._uniqueApplications; }
			set { this._uniqueApplications = value; }
		}

		public int IncludedApplications
		{
			get { return this._includedApplications; }
			set { this._includedApplications = value; }
		}

		public int LicensesDeclared
		{
			get { return this._licensesDeclared; }
			set { this._licensesDeclared = value; }
		}

		public int LicenseCountDeclared
		{
			get { return this._licenseCountDeclared; }
			set { this._licenseCountDeclared = value; }
		}
	}

	#endregion Declare Licenses Statistics

	#region Create Actions Statistics

	/// <summary>
	/// This object encapsulates statistical information recovered relating to the 'Create Actions' phase
	/// </summary>
	public class CAStatistics
	{
		// Fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private int _compliantApplications;
		private int _nonCompliantApplications;
		private int _actionsDeclared;

		// Titles
		public const string TitleNonComplient = "Non-Compliant Applications: ";
		public const string TitleCompliant= "Compliant Applications: ";
		public const string TitleActionsDeclared = "Actions Declared: ";

		// Methods
		public CAStatistics()
		{
			_compliantApplications = 0;
			_nonCompliantApplications = 0;
			_actionsDeclared = 0;
		}
	

		public CAStatistics(DataRow dataRow)
		{
			try
			{
				this._actionsDeclared = (int)dataRow["actioncount"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a CAStatistics Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public int CompliantApplications
		{
			get { return this._compliantApplications; }
			set { this._compliantApplications = value; }
		}

		public int NonCompliantApplications
		{
			get { return this._nonCompliantApplications; }
			set { this._nonCompliantApplications = value; }
		}

		public int ActionsDeclared
		{
			get { return this._actionsDeclared; }
			set { this._actionsDeclared = value; }
		}
	}

	#endregion Create Actions Statistics

	#region Review Actions Statistics

	/// <summary>
	/// This object encapsulates statistical information recovered relating to the 'Review Actions' phase
	/// </summary>
	public class RAStatistics
	{
		// Fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private int _actionsReviewed;
		private int _actionsNotReviewed;

		// Titles
		public const string TitleReviewed = "Actions Reviewed: ";
		public const string TitleNotReviewed = "Actions Not Revieved: ";

		// Constructor
		public RAStatistics()
		{
			_actionsNotReviewed = 0;
			_actionsReviewed = 0;
		}


		public RAStatistics(DataRow dataRow)
		{
			try
			{
				this._actionsReviewed = (int)dataRow["reviewed"];
				this._actionsNotReviewed = (int)dataRow["notreviewed"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an RAStatistics Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public int ActionsReviewed
		{
			get { return this._actionsReviewed; }
			set { this._actionsReviewed = value; }
		}

		public int ActionsNotReviewed
		{
			get { return this._actionsNotReviewed; }
			set { this._actionsNotReviewed = value; }
		}
	}

	#endregion Review Actions Statistics

}

