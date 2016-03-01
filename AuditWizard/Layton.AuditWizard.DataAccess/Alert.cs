using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
	#region Alert Class
	
	public class Alert
	{
		public enum AlertType { support, installs, hardware, alertmonitor };
		public enum AlertCategory { expired, added, deleted, changed };
		public enum AlertStatus	 { active, dismissedtoday ,dismissed };

		#region Data
		private int				_alertID;
		private AlertType		_type;
		private AlertCategory	_category;
		private string			_message;
		private string			_field1;
		private string			_field2;
		private DateTime		_alertedOnDate;
		private AlertStatus		_status;
		private string			_assetName;
		private string			_alertName;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		#endregion Data

		#region Properties

		public int AlertID
		{
			get { return _alertID; }
			set { _alertID = value; }
		}

		/// <summary>
		/// This is the type of the alert
		/// </summary>
		public AlertType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>
		/// This is the category of the alert
		/// </summary>
		public AlertCategory Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string CategoryAsString
		{
			get
			{
				if (_type == AlertType.alertmonitor)
					return "Alert Triggered";
				else
				{	
					switch ((int)_category)
					{
						case (int)AlertCategory.added:
							return "Added";
						case (int)AlertCategory.changed:
							return "Changed";
						case (int)AlertCategory.deleted:
							return "Deleted";
						case (int)AlertCategory.expired:
							return "Expired";
						default:
							return "Unknown Type";
					}
				}
			}
		}

		public string TypeAsString 
		{
			get
			{
				switch ((int)_type)
				{
					case (int)AlertType.hardware:
						return "Hardware";
					case (int)AlertType.installs:
						return "Install/Uninstall";
					case (int)AlertType.support:
						return "Support Expiry";
					case (int)AlertType.alertmonitor:
						return "AlertMonitor";
					default:
						return "Unknown Type";
				}
			}
		}


		public string StatusAsString
		{
			get
			{
				switch ((int)_status)
				{
					case (int)AlertStatus.active:
						return "Active";
					case (int)AlertStatus.dismissed:
						return "Dismissed";
					case (int)AlertStatus.dismissedtoday:
						return "Dismissed Today";
					default:
						return "Unknown Status";
				}
			}
		}

		/// <summary>
		/// This is the message which willbe displayed for the alert
		/// </summary>
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		/// <summary>
		/// The contents of this field depend on the alert generated
		/// </summary>
		public string Field1
		{
			get { return _field1; }
			set { _field1 = value; }
		}

		/// <summary>
		/// The content of this field depends on the alert being generated
		/// </summary>
		public string Field2
		{
			get { return _field2; }
			set { _field2 = value; }
		}

		/// <summary>
		/// This is the last date on which this alert was reported.  The importance of this field is that we 
		/// do not want to alert more than once per day
		/// </summary>
		public DateTime AlertedOnDate
		{
			get { return _alertedOnDate; }
			set { _alertedOnDate = value; }
		}

		/// <summary>
		/// This is the current status of the alert
		/// </summary>
		public AlertStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		/// <summary>
		/// This is the name of the alert which was triggered (if any)
		/// </summary>
		public string AlertName
		{
			get { return _alertName; }
			set { _alertName = value; }
		}

		/// <summary>
		/// This is the message which willbe displayed for the alert
		/// </summary>
		public string AssetName
		{
			get { return _assetName; }
			set { _assetName = value; }
		}

		#endregion Properties

		#region Constructor

		public Alert()
		{
			_alertID = 0;
			_type = AlertType.support;
			_category = AlertCategory.expired;
			_message = "";
			_field1 = "";
			_field2 = "";
			_status = AlertStatus.active;
			_alertedOnDate = DateTime.Today;
		}

		public Alert(AlertType type, AlertCategory category, string message, string field1, string field2)
			: this()
		{
			_type = type;
			_category = category;
			_message = message;
			_field1 = field1;
			_field2 = field2;
		}

		public Alert(DataRow dataRow)
			: this()
		{
			try
			{
				_alertID		= (int) dataRow["_ALERTID"];
				_type			= (AlertType)(int)dataRow["_TYPE"];
				_category		= (AlertCategory)(int)dataRow["_CATEGORY"];
				_message		= (string)dataRow["_MESSAGE"];
				_field1			= (string)dataRow["_FIELD1"];
				_field2			= (string) dataRow["_FIELD2"];
				_status			= (AlertStatus)(int)dataRow["_STATUS"];
				_alertedOnDate	= (DateTime)dataRow["_ALERTDATE"];
				_assetName		= (string)dataRow["_ASSETNAME"];
				_alertName		= (string)dataRow["_ALERTNAME"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an ALERT Object, please check database schema.  The message was " + ex.Message);
			}
		}


		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public Alert(Alert other)
		{
			_alertID = other._alertID;
			_type = other._type;
			_category = other._category;
			_message = other._message;
			_field1 = other._field1;
			_field2 = other._field2;
			_status = other._status;
			_alertedOnDate = other._alertedOnDate;
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
				Alert other = obj as Alert;

				if ((object)other != null)
				{
					bool equality;
					equality = other.AlertID == AlertID
							&& other.Type == Type
							&& other.Category == Category
							&& other.Message == Message
							&& other.Field1 == Field1
							&& other.Field2 == Field2
							&& other.Status == Status
							&& other.AlertedOnDate == AlertedOnDate;
					return equality;
				}
			}
			return base.Equals(obj);
		}


		#endregion Constructor

		public int Add()
		{
			// Add from the database
			AlertsDAO lwDataAccess = new AlertsDAO();
			_alertID = lwDataAccess.AlertAdd(this);
			return _alertID;
		}
		
		/// <summary>
		/// Delete the current Alert from the database
		/// </summary>
		public void Delete()
		{
			// Delete from the database
			AlertsDAO lwDataAccess = new AlertsDAO();
			lwDataAccess.AlertDelete(this);
		}


		/// <summary>
		/// Dismiss this alert
		/// </summary>
		public void Dismiss()
		{
			AlertsDAO lwDataAccess = new AlertsDAO();
			Status = AlertStatus.dismissed;
			lwDataAccess.AlertSetStatus(this);
		}
		


		/// <summary>
		/// Format a textual representation of an alert triggered
		/// </summary>
		/// <returns></returns>
		public string GenerateAlertMessageText()
		{
			if (_type != AlertType.alertmonitor)
				return Message;
						
			string message = "Date: " + _alertedOnDate.ToString() + "  Alert Triggered: " + _alertName + "  PC : " + _assetName;
			if (_message.StartsWith("Internet"))
				message = message + "Reason: " +  _message + "  URL: " + _field1;
			else 
			{
				message = message + "Reason: " + CategoryAsString;
				if (_category == AlertCategory.changed)
					message = message + " " + _message + " Changed from '" + _field1 + "' to '" + _field2 + "'";	
			}
			return message;
		}
	}
	
	#endregion Alert Class

	#region AlertList Class

	public class AlertList : List<Alert>
	{
		public int Populate(DateTime dtSince)
		{
			// Ensure that the list is empty initially
			this.Clear();

			AlertsDAO lwDataAccess = new AlertsDAO();
			DataTable table = lwDataAccess.EnumerateAlerts(dtSince);

			// Iterate through the returned rows in the table and create AssetType objects for each
			foreach (DataRow row in table.Rows)
			{
				AddAlert(row);
			}
			return this.Count;
		}


		/// <summary>
		/// Add a new Operation to the list given a database row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Alert AddAlert(DataRow row)
		{
			// Create the assettype object
			Alert alert = new Alert(row);
			this.Add(alert);
			return alert;
		}
	}

	#endregion OperationList Class
	
}
