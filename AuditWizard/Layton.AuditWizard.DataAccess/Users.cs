using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	public class User
	{
		#region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Enumerator for the access levels
		public enum ACCESSLEVEL { administrator=0, auditor ,reader } ;

		// Fields
		private int			_userid;
		private string		_logon;
		private string		_password;
		private string		_firstName;
		private string		_lastName;
		private ACCESSLEVEL _accessLevel;
		private int			_rootLocation;
		private string		_rootLocationString;

		#endregion Data

		#region Properties

		// Properties
		[Browsable(false)]
		public int UserID
		{
			get { return this._userid; }
			set { this._userid = value; }
		}

		[ReadOnly(true), Description("This is the Logon name for the user."), Category("Identity")]
		public string Logon
		{
			get { return this._logon; }
			set { this._logon = value; }
		}

		[ReadOnly(true), Description("This is the (encrypted) password for the user."), Category("Identity")]
		public string Password
		{
			get { return this._password; }
			set { this._password = value; }
		}

		[ReadOnly(true), Description("This is the first name specified for the user."), Category("Identity")]
		public string FirstName
		{
			get { return this._firstName; }
			set { this._firstName = value; }
		}

		[ReadOnly(true), Description("This is the last name for the user."), Category("Identity")]
		public string LastName
		{
			get { return this._lastName; }
			set { this._lastName = value; }
		}

		[ReadOnly(true), Description("This is the access level defined for the user."), Category("Identity")]
		public ACCESSLEVEL AccessLevel
		{
			get { return this._accessLevel; }
			set { this._accessLevel = value; }
		}

		public string AccessLevelAsString
		{
			get 
			{ 	
				switch ((int)_accessLevel)
				{
					case (int)ACCESSLEVEL.administrator:
						return "Administrator Access";
					case (int)ACCESSLEVEL.auditor:
						return "Auditor Access";
					case (int)ACCESSLEVEL.reader:
						return "Read-Only Access";
					default:
						return "Unknown Access Level";
				}
			}
		}
		[ReadOnly(true), Description("This is the id of the locations pecified as the root for the user."), Category("Identity")]
		public int RootLocationID 
		{
			get { return this._rootLocation; }
		}

		[ReadOnly(true), Description("This is the textual representation of the location specified as the root for the user."), Category("Identity")]
		public string RootLocationAsString
		{
			get { return this._rootLocationString; }
		}


#endregion

		#region Constructor

		// Methods
		public User()
		{
			this._userid = 0;
			this._logon = "";
			this._password = "";
			this._firstName = "";
			this._lastName = "";
			this._accessLevel = ACCESSLEVEL.administrator;
			this._rootLocation = 1;
			this._rootLocationString = "<All Locations>";
		}

		public User(DataRow dataRow) : this()
		{
			try
			{
				_userid	= (int) dataRow["_USERID"];
				Logon = (string)dataRow["_LOGIN"];
				FirstName = (string) dataRow["_FIRSTNAME"];
				LastName  = (string) dataRow["_LASTNAME"];
				AccessLevel = (ACCESSLEVEL) dataRow["_ACCESSLEVEL"];
				RootLocation((int)dataRow["_ROOTLOCATION"], (string)dataRow["_NAME"]);
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a USER Object, please check database schema.  The message was " + ex.Message);
			}
		}

		public User(int userid, string logon, string firstname, string lastname, ACCESSLEVEL accessLevel, int rootLocation ,string rootLocationName)
		{
			_userid = userid;
			Logon = logon;
			FirstName = firstname;
			LastName  = lastname;
			AccessLevel = accessLevel;
			RootLocation(rootLocation ,rootLocationName);
		}

		#endregion Constructors

		#region Methods

		#region Methods

		/// <summary>
		/// Add this Action to the database
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			int status = -1;

			// If this user already exists then call the update instead
			if (_userid != 0)
				return Update(null);

			// Add the user to the database
			UsersDAO lwDataAccess = new UsersDAO();
			int id = lwDataAccess.UserAdd(this);
			if (id != 0)
			{
				AuditChanges(null);
				_userid = id;
				status = 0;
			}

			return status;
		}


		/// <summary>
		/// Update this USER in the database
		/// </summary>
		/// <returns></returns>
		public int Update(User oldUser)
		{
			UsersDAO lwDataAccess = new UsersDAO();
			if (this._userid == 0)
			{
				Add();
			}
			else
			{
				lwDataAccess.UserUpdate(this);
				AuditChanges(oldUser);
			}

			return 0;
		}


		/// <summary>
		/// Delete the current USER from the database
		/// </summary>
		public void Delete()
		{
			// Delete from the database
            UsersDAO lwDataAccess = new UsersDAO();
			lwDataAccess.UserDelete(this);

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
		public List<AuditTrailEntry> AuditChanges(User oldObject)
		{
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();

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

			// If the ld of the old object was 0 then this is a NEW item and not a change to an existing item
			if (UserID == 0)
			{
				ate.Type = AuditTrailEntry.TYPE.added;
				AddChange(listChanges, ate, "", "", "");
			}

			else if (oldObject != null)
			{
				// First / Last Name
				if (this._firstName != oldObject._firstName)
					ate = AddChange(listChanges, ate, "First Name", oldObject._firstName, _firstName);

				if (this._lastName != oldObject._lastName)
					ate = AddChange(listChanges, ate, "Last Name", oldObject._lastName, _lastName);

				// Access Level
				if (_accessLevel != oldObject._accessLevel)
					ate = AddChange(listChanges, ate, "Access Level", oldObject.AccessLevelAsString ,AccessLevelAsString);
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
			ate.Class = AuditTrailEntry.CLASS.user;
			ate.Type = AuditTrailEntry.TYPE.changed;
			ate.Key = _logon;
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
		
		public void RootLocation (int locationID ,string locationString)
		{
			_rootLocation = locationID;
			_rootLocationString = locationString;
		}

		public override string ToString()
		{
			return this.Logon;
		}

		#endregion Methods
	}

	#region UserList class

	/// <summary>
	/// This object implements a list of <seealso cref="User"/>objects
	/// </summary>
	public class UserList : List<User>
	{
		// Methods
		public new int Add(User user)
		{
			User findUser = Find(user.UserID);
			if (findUser != null)
			{
				findUser.Logon = user.Logon;
				findUser.FirstName = user.FirstName;
				findUser.LastName = user.LastName;
				findUser.AccessLevel = user.AccessLevel;
				findUser.RootLocation(user.RootLocationID ,user.RootLocationAsString);
			}
			else
			{
				base.Add(user);
			}
			return 0;
		}

		public User Find (int thisID)
		{
			foreach (User thisUser in this)
			{
				if (thisUser.UserID == thisID)
					return thisUser;
			}
			return null;
		}
	}
	#endregion UserList class

}
