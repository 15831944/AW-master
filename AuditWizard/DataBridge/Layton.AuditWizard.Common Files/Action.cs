using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	#region Action Class

	public class Action
	{
		public enum ACTIONTYPE { uninstall ,purchase ,Ignore };
		public enum ACTIONSTATUS { outstanding ,reviewed ,complete };

		#region Data
		
		private int				_actionID;
		private ACTIONTYPE		_type;
		private int				_applicationID;
		private string			_associatedAssets;
		private string			_applicationName;
		private ACTIONSTATUS	_status;
		private string			_notes;
		
		#endregion Data

		#region Properties

		public int ActionID
		{
			get { return _actionID; }
			set { _actionID = value; }
		}

		public int ApplicationID
		{
			get { return _applicationID; }
		}

		public string ApplicationName
		{
			get { return _applicationName; }
		}

		public string AssociatedAssets
		{
			get { return _associatedAssets; }
			set { _associatedAssets = value; }
		}


		/// <summary>
		/// This is the type of the action
		/// </summary>
		public ACTIONTYPE ActionType
		{
			get { return _type; }
			set { _type = value; }
		}

		public string ActionTypeText
		{
			get
			{
				switch ((int)_type)
				{
					case (int)ACTIONTYPE.Ignore:
						return "Mark as 'Ignored'";
					case (int)ACTIONTYPE.purchase:
						return "Purchase Additional Licenses";
					case (int)ACTIONTYPE.uninstall:
						return "Uninstall Instances";
					default:
						return "Unknown Action Type";
				}
			}
		}

		/// <summary>
		/// This is the Status of the action
		/// </summary>
		public ACTIONSTATUS Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public string StatusText
		{ 
			get
			{
				switch ((int)_status)
				{
					case (int)ACTIONSTATUS.outstanding:
						return "Outstanding";
					case (int)ACTIONSTATUS.reviewed:
						return "Reviewed";
					case (int)ACTIONSTATUS.complete:
						return "Complete";
					default:
						return "Unknown Action Status";
				}
			}
		}

		/// <summary>
		/// This is the Notes
		/// </summary>
		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}

		#endregion Properties

		#region Constructor

		public Action()
		{
			_actionID = 0;
			_type = ACTIONTYPE.purchase;
			_applicationID = 0;
			_status = ACTIONSTATUS.outstanding;
			_notes = "";
		}

		public Action(ACTIONTYPE type, int applicationID, string applicationName, string associatedComputers, ACTIONSTATUS status, string notes)
			: this()
		{
			_type = type;
			_applicationID = applicationID;
			_applicationName = applicationName;
			_associatedAssets = associatedComputers;
			_status = status;
			_notes = notes;
		}

		public Action(ACTIONTYPE type, int applicationID, string applicationName, ACTIONSTATUS status, string notes)
			: this()
		{
			_type = type;
			_applicationID = applicationID;
			_applicationName = applicationName;
			_associatedAssets = "";
			_status = status;
			_notes = notes;
		}

		public Action(DataRow dataRow)
			: this()
		{
			try
			{
				_actionID			= (int) dataRow["_ACTIONID"];
				_applicationID		= (int)dataRow["_APPLICATIONID"];
				_applicationName	= (string)dataRow["_NAME"];
				ActionType			= (ACTIONTYPE)dataRow["_TYPE"];
				Status				= (ACTIONSTATUS)dataRow["_STATUS"];
				Notes				= (string) dataRow["_NOTES"];
				_associatedAssets = (string)dataRow["_ASSETS"];
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating an ACTION Object, please check database schema.  The message was " + ex.Message);		
			}
		}


		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public Action(Action other)
		{
			_actionID = other._actionID;
			_applicationID = other._applicationID;
			_applicationName = other._applicationName;
			_associatedAssets = other._associatedAssets;
			_notes = other._notes;
			_status = other._status;
			_type = other._type;
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
				Action other = obj as Action;

				if ((object)other != null)
				{
					bool equality;
					equality = other.ActionID == ActionID
							&& other.ApplicationID == ApplicationID
							&& other.ApplicationName == ApplicationName
							&& other.AssociatedAssets == AssociatedAssets
							&& other.Notes == Notes
							&& other.Status == Status
							&& other.ActionType == ActionType;
					return equality;
				}
			}
			return base.Equals(obj);
		}


		/// <summary>
		///  Set the name and ID of the application for this action
		/// </summary>
		/// <param name="applicationID"></param>
		/// <param name="applicationName"></param>
		public void SetApplication(int applicationID, string applicationName)
		{
			_applicationName = applicationName;
			_applicationID = applicationID;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Add this Action to the database
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			int status = -1;

			// If this supplier already exists then call the update instead
			if (_actionID != 0)
				return Update(null);

			// Add the ACTION to the database
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			int id = lwDataAccess.ActionAdd(this);
			if (id != 0)
			{
				AuditChanges(null);
				_actionID = id;
				status = 0;
			}

			return status;
		}


		/// <summary>
		/// Update this ACTION in the database
		/// </summary>
		/// <returns></returns>
		public int Update(Action oldaction)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (this._actionID == 0)
			{
				Add();
			}
			else
			{
				lwDataAccess.ActionUpdate(this);
				AuditChanges(oldaction);
			}

			return 0;
		}


		/// <summary>
		/// Delete the current license from the database
		/// </summary>
		public void Delete()
		{
			// Delete from the database
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			lwDataAccess.ActionDelete(this);

			// ...and audit the deletion
			AuditTrailEntry ate = BuildATE();
			ate.Type = AuditTrailEntry.TYPE.deleted;
			lwDataAccess.AuditTrailAdd(ate);
		}

		#endregion Methods

		#region Change Handling

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public List<AuditTrailEntry> AuditChanges(Action oldObject)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// The following fields may change for an Action
			//
			//	Associated Computers
			//	Notes
			//  Status
			//
			// Build a blank AuditTrailEntry
			AuditTrailEntry ate = BuildATE();

			// Is this a new item or a change to an existing item
			if (ActionID == 0)
			{
				ate.Type = AuditTrailEntry.TYPE.added;
				AddChange(listChanges, ate, "", ActionTypeText, "");
			}

			else if (oldObject != null)
			{
				// Associated Computers
				if (this._associatedAssets != oldObject.AssociatedAssets)
				{
					ate = AddChange(listChanges, ate, "Associated Computers", oldObject.AssociatedAssets, AssociatedAssets);
				}

				// Notes
				if (Notes != oldObject.Notes)
					ate = AddChange(listChanges
								  , ate
								  , "Notes"
								  , oldObject.Notes
								  , Notes);

				// Status
				if (Status != oldObject.Status)
					ate = AddChange(listChanges
								  , ate
								  , "Status"
								  , oldObject.StatusText
								  , StatusText);
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
			ate.Class = AuditTrailEntry.CLASS.action;
			ate.Type = AuditTrailEntry.TYPE.changed;
			ate.Key = _applicationName;
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


	#endregion ActionClass

	#region ActionList Class

	public class ActionList : List<Action>
	{
		public ActionList()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable actionsTable = lwDataAccess.EnumerateActions();
			foreach (DataRow row in actionsTable.Rows)
			{
				Action action = new Action(row);
				this.Add(action);
			}
		}

		/// <summary>
		/// Recover the count of actions for the specified application
		/// </summary>
		/// <param name="forApplication"></param>
		/// <returns></returns>
		public int Count (int forApplication)
		{
			int count = 0;
			foreach (Action action in this)
			{
				if (action.ApplicationID == forApplication)
					count++;
			}
			return count;
		}
	}

	#endregion ActionList Class
}
