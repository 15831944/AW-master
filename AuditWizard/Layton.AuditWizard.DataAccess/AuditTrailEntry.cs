using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	public class AuditTrailEntry
	{
		public enum V7_HIST_OPS { eHistOpNew = 1, eHistOpDelete = 2, eHistOpChange = 4, eHistOpAlertIE, eHistOpAlertFolder };

		public enum TYPE { added, deleted, changed };
		
		// Class of ATE - note that this is split into two section, ATE entries relating to an audit of an asset
		// and those relating to changes made manually by the user
		public enum CLASS { 
							all = -1, 
							application_installs,		// Application has been installed or uninstalled
							auditdata,					// A piece of audit data has been modified
							audited,					// An Asset has been audited for the first time
							reaudited,					// An Asset has been re-audited
							//
							application_changes = 100,	// An attribute for an application has been modified
							license,					// An attribute of an application license has been changed
							action,						// An Action has been added/modified/deleted
							supplier,					// A Supplier has been added/modified/deleted
							user,						// A User Definition has been added/modified/deleted
							asset,						// An asset defintion has been added/modified/deleted
							location,					// A Location Definition has been added/modified/deleted
						  };					

		#region Data Definitions

		private int _auditTrailID;
		private int _assetID;
		private string _assetName;
		private string _location;
		private string _username;
		private DateTime _date;
		private CLASS _class;
		private string _key;
		private string _oldValue;
		private string _newValue;
		private TYPE _type;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#endregion Data Definitions

		#region Properties

		/// <summary>The database ID of this entry</summary>
		public int AuditTrailID
		{
			get { return _auditTrailID; }
			set { _auditTrailID = value; }
		}

		/// <summary>The database ID of the asset on which this change took place</summary>
		public int AssetID
		{
			get { return _assetID; }
			set { _assetID = value; }
		}

		/// <summary>The name of the asset on which this change took place</summary>
		public string AssetName
		{
			get { return _assetName; }
			set { _assetName = value; }
		}

		/// <summary>The name of the location for the asset</summary>
		public string Location
		{
			get { return _location; }
			set { _location = value; }
		}

		/// <summary>The name of the user who executed the change</summary>
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		/// <summary>The date/time at which this change took place</summary>
		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		/// <summary>The key holds the name of the item which has changed</summary>
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>For a value change, this field holds the original value assigned to 'key'</summary>
		public string OldValue
		{
			get { return _oldValue; }
			set { _oldValue = value; }
		}

		/// <summary>For a value change, this field holds the new value assigned to 'key'</summary>
		public string NewValue
		{
			get { return _newValue; }
			set { _newValue = value; }
		}

		/// <summary>This is the CLASS of item which has changed</summary>
		public CLASS Class
		{
			get { return _class; }
			set { _class = value; }
		}

		/// <summary>This is the type of change which took place</summary>
		public TYPE Type
		{
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>
		/// Return the Audit Trail Entry type in it's textual representation
		/// </summary>
		public String TypeString
		{
			get
			{
				String type;
				switch ((int)_type)
				{
					case (int)TYPE.added:
                        type = (Class == CLASS.application_installs) ? "Installed" : "Added";
                        break;

                    case (int)TYPE.changed:	
                        type = "Modified";	
                        break;

					case (int)TYPE.deleted:
						type = (Class == CLASS.application_installs) ? "Uninstalled" : "Deleted";
						break;

					default:
                        type = "<none>";	
                        break;
				}
				return type;
			}
		}

		static public TYPE TranslateV7Type(V7_HIST_OPS v7type)
		{
			switch (v7type)
			{
				case V7_HIST_OPS.eHistOpChange:
					return TYPE.changed;
				case V7_HIST_OPS.eHistOpDelete:
					return TYPE.deleted;
				case V7_HIST_OPS.eHistOpNew:
					return TYPE.added;
				default:
					return TYPE.changed;
			}
		}


		public String GetTypeDescription()
		{
			// If the class is for an audit data change then the description is formatted from the key - which
			// is the data field that has changed and the old and new values
			string description = "";
			if (_class == CLASS.auditdata)
			{
				if (_type == TYPE.changed)
					description = String.Format("{0} changed from {1} to {2}", Key, OldValue, NewValue);
				else if (_type == TYPE.added)
					description = String.Format("{0} added with value {1}", Key, NewValue);
				else if (_type == TYPE.deleted)
					description = String.Format("{0} removed - original value was {0}", Key, OldValue);
			}
			
			else if (_class == CLASS.application_installs)
			{
				description = ClassString + " [" + Key + "] " + TypeString;
			}

			else if (_class == CLASS.audited || _class == CLASS.reaudited)
			{
				description = ClassString;
			}

			else
			{
				description = TypeString;
			}

			return description;
		}

		/// <summary>
		/// Return the audit trail entry class in it's textual representation
		/// </summary>
		public String ClassString 
		{
			get
			{
				String classString;
				switch ((int)_class)
				{
					case (int)CLASS.application_installs:	classString = "Application";		break;
					case (int)CLASS.license:				classString = "License";			break;
					case (int)CLASS.application_changes:	classString = "Application Property Change"; break;
					case (int)CLASS.action:					classString = "Action"; break;
					case (int)CLASS.supplier:				classString = "Supplier"; break;
					case (int)CLASS.user:					classString = "User"; break;
					case (int)CLASS.auditdata:				classString = "Audited Data"; break;
					case (int)CLASS.audited:				classString = "Initial Audit"; break;
					case (int)CLASS.reaudited:				classString = "Reaudit"; break;
					default:								classString = "<none>"; break;
				}
				return classString;
			}
		}


		#endregion Properties

		#region Constructor

		public AuditTrailEntry()
		{
			_auditTrailID = 0;
			_assetID = 0;
			_assetName = "";
			_location = "";
			_date = DateTime.Now;
			_username = Environment.UserName;
			_key = "";
			_oldValue = "";
			_newValue = "";
			_class = CLASS.application_installs;
			_type = TYPE.changed;
		}

		public AuditTrailEntry(DataRow dataRow)
		{
			try
			{
				_auditTrailID	= (int)dataRow["_AUDITTRAILID"];
				_assetID		= (int)dataRow["_ASSETID"];
				_assetName		= (dataRow.IsNull("ASSETNAME")) ? "" : (String)dataRow["ASSETNAME"];
				_location		= (dataRow.IsNull("FULLLOCATIONNAME")) ? "" : (String)dataRow["FULLLOCATIONNAME"];
				_username		= (String)dataRow["_USERNAME"];
				_date			= (DateTime)dataRow["_DATE"];
				_class			= ((CLASS)(int)dataRow["_CLASS"]);
				_type			= ((TYPE) (int)dataRow["_TYPE"]);
				_key			= (String)dataRow["_KEY"];
				_oldValue		= (String)dataRow["_VALUE1"];
				_newValue		= (String)dataRow["_VALUE2"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an AUDITTRAIL Object, please check database schema.  The message was " + ex.Message);
			}
		}

		#endregion Constructor
	}
}
