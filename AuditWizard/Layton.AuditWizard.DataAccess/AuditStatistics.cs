using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{

	/// <summary>
	/// This class encapsulates statistics recovered from the database for application useage
	/// </summary>
	public class AuditStatistics
	{
		// Fields
		private string _mostCommonApplication;
		private int _publishers;
		private int _totalApplications;
		private int _uniqueApplications;
		private int _deployedAgents;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Non database fields
		private int _filterPublishersCount;

		// Methods
		public AuditStatistics(DataRow dataRow)
		{
			try
			{
				this._publishers = Convert.ToInt32(dataRow["publishers"]);
				this._uniqueApplications = Convert.ToInt32(dataRow["uniqueapplications"]);
				this._totalApplications = Convert.ToInt32(dataRow["totalapplications"]);
				this._deployedAgents = Convert.ToInt32(dataRow["deployedagents"]);
				this._mostCommonApplication = "";

				// Get the filtered publisher string from the database
				SettingsDAO lwDataAccess = new SettingsDAO();
				string publisherFilter = lwDataAccess.GetPublisherFilter();
				if (publisherFilter != "")
				{
					String[] publishers = publisherFilter.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					_filterPublishersCount = publishers.Length;
				}
				else
				{
					_filterPublishersCount = 0;
				}
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an AUDITSTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}


		// Properties
		public string MostCommonApplication
		{
			get { return this._mostCommonApplication; }
			set { this._mostCommonApplication = value; }
		}

		public int Publishers
		{ 
			get { return this._publishers; }
			set { this._publishers = value; }
		}

		public int DeployedAgents
		{ 
			get { return this._deployedAgents; }
			set { this._deployedAgents = value; }
		}

		public int FilterPublishers
		{
			get { return this._filterPublishersCount; }
			set { this._filterPublishersCount = value; }
		}

		public int TotalApplications
		{
			get { return this._totalApplications; }
			set { this._totalApplications = value; }
		}

		public int UniqueApplications
		{
			get { return this._uniqueApplications; }
			set { this._uniqueApplications = value; }
		}
	}

	/// <summary>
	/// This class encapsulates statistics recovered from the database for audit history
	/// </summary>
	public class AuditHistoryStatistics
	{
		// Fields
		private int _notaudited;
		private int _auditedtoday;
		private int _notaudited7;
		private int _notaudited14;
		private int _notaudited30;
		private int _notaudited90;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Methods
		public AuditHistoryStatistics(DataRow dataRow)
		{
			try
			{
				this._notaudited = Convert.ToInt32(dataRow["notaudited"]);
				this._auditedtoday = Convert.ToInt32(dataRow["today"]);
				this._notaudited7 = Convert.ToInt32(dataRow["notinlast7"]);
				this._notaudited14 = Convert.ToInt32(dataRow["notinlast14"]);
				this._notaudited30 = Convert.ToInt32(dataRow["notinlast30"]);
				this._notaudited90 = Convert.ToInt32(dataRow["over90days"]);
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an AUDITHISTORYSTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}


		// Properties
		public int NotAudited
		{
			get { return this._notaudited; }
			set { this._notaudited = value; }
		}

		public int AuditedToday
		{
			get { return this._auditedtoday; }
			set { this._auditedtoday = value; }
		}

		public int NotAudited7
		{
			get { return this._notaudited7; }
			set { this._notaudited7 = value; }
		}

		public int NotAudited14
		{
			get { return this._notaudited14; }
			set { this._notaudited14 = value; }
		}

		public int NotAudited30
		{
			get { return this._notaudited30; }
			set { this._notaudited30 = value; }
		}

		public int NotAudited90
		{
			get { return this._notaudited90; }
			set { this._notaudited90 = value; }
		}
	}


	/// <summary>
	/// This class encapsulates statistics recovered from the database for asset alerts
	/// </summary>
	public class AlertStatistics
	{
		// Fields
		private DateTime _lastAlert;
		private int _alertsToday;
		private int _alertsWeek;
		private int _alertsMonth;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Methods
		public AlertStatistics(DataRow dataRow)
		{
			try
			{
				// The date of last alert m,ay come back as null if we have not had an alert yet
				if (dataRow.IsNull("lastalert"))
					this._lastAlert = new DateTime(0);
				else 
					this._lastAlert = Convert.ToDateTime(dataRow["lastalert"]);
				//
				this._alertsToday = Convert.ToInt32(dataRow["alertstoday"]);
				this._alertsWeek = Convert.ToInt32(dataRow["alertsthisweek"]);
				this._alertsMonth = Convert.ToInt32(dataRow["alertsthismonth"]);
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an ALERTSTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public DateTime LastAlert
		{
			get { return this._lastAlert; }
			set { this._lastAlert = value; }
		}

		public int AlertsToday
		{
			get { return this._alertsToday; }
			set { this._alertsToday = value; }
		}

		public int AlertsThisWeek
		{
			get { return _alertsWeek; }
			set { this._alertsWeek = value; }
		}

		public int AlertsThisMonth
		{ 
			get { return _alertsMonth; }
			set { _alertsMonth = value; }
		}
	}



	/// <summary>
	/// This class encapsulates statistics recovered from the database for application support contract useage
	/// </summary>
	public class SupportContractStatistics
	{
		// Fields
		private int _expired;
		private int _expireToday;
		private int _expireWeek;
		private int _expireMonth;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Methods
		public SupportContractStatistics(DataRow dataRow)
		{
			try
			{
				this._expired = Convert.ToInt32(dataRow["expired"]);
				this._expireToday = Convert.ToInt32(dataRow["expiretoday"]);
				this._expireWeek = Convert.ToInt32(dataRow["expirethisweek"]);
				this._expireMonth = Convert.ToInt32(dataRow["expirethismonth"]);
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a SUPPORTCONTRACTSTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
		public int Expired
		{
			get { return this._expired; }
			set { this._expired = value; }
		}

		public int ExpireToday
		{
			get { return this._expireToday; }
			set { this._expireToday = value; }
		}

		public int ExpireWeek
		{
			get { return _expireWeek; }
			set { this._expireWeek = value; }
		}

		public int ExpireMonth
		{ 
			get { return _expireMonth; }
			set { _expireMonth = value; }
		}
	}
}

