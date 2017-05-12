using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

using Layton.AuditWizard.DataAccess;

namespace AuditWizardDataBridge
{
#region AuditWizard DataBridge Interface Definition
	/// <summary>
	/// This is the definition of the Interface to the AuditWizard databridge
	/// </summary>
[Guid("D2F067B4-4BD9-4B3F-8155-46F191D4641C")]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]   // Dual interface provides both early and late binding

	public interface IAuditWizardDataBridgeInterface
    {
		/// <summary>
		/// This is a test function which will just echo what was supplied to it
		/// </summary>
		/// <param name="baseString"></param>
		/// <returns></returns>
		string TestFunction(string baseString);

		/// <summary>Open the database</summary>
		/// <param name="databasePath"></param>
		/// <param name="connectionstring"></param>
		/// <returns></returns>
		int Open (string sqlServer ,string sqlUsername ,string sqlPassword ,ref string error);
		
		/// <summary>Logon to the database if security is enabled</summary>
		/// <param name="instanceID"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		int Logon (int instanceID ,string username ,string password);

		/// <summary>
		/// close the database
		/// </summary>
		/// <param name="instanceID"></param>
		void	Close (int instanceID);

		/// <summary>
		/// return the last database error
		/// </summary>
		/// <returns></returns>
		string	GetDBError ();

		/// <summary>
		/// Enable or disable trace mode
		/// </summary>
		/// <param name="enable"></param>
		void	Trace		(bool enable);
	
		/// <summary>
		/// Recover the database version string
		/// </summary>
		/// <returns></returns>
		string GetVersionInfo(int instanceID);

		/// <summary>
		/// Return the database id of the specified (named) asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		int		GetAssetID	(int instanceID, string assetName);
	
		/// <summary>
		/// Get the root company name
		/// </summary>
		/// <param name="instanceID"></param>
		/// <returns></returns>
		string	GetCompanyName	(int instanceID);

		/// <summary>
		/// Return a list of locations (and assets) beneath the location specified
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="hintValue"></param>
		/// <param name="includeAssets"></param>
		/// <returns></returns>
		object[] ExpandLocation	(int instanceID, int hintValue, bool includeAssets);
		
		/// <summary>
		/// Return the name of the location for the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <returns></returns>
		string	GetAssetLocation(int instanceID, int assetID);

		/// <summary>
		/// Return the value(s) for the specified data field for the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <returns></returns>
		object[] GetValues(int instanceID, int assetID, string field, int hintValue);
		
		/// <summary>
		/// Expand the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <returns></returns>
		object[] ExpandAsset(int instanceID, int assetID, string field, int hintValue);

		/// <summary>
		/// Return a match to the specified asset find string
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="findBy"></param>
		/// <param name="findString"></param>
		/// <param name="exactMatches"></param>
		/// <returns></returns>		
		object[] FindAsset(int instanceID, int findBy, string findString, bool exactMatches);

		/// <summary>
		/// Enumerate the assets at the specified location
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="locationID"></param>
		/// <returns></returns>
		object[] EnumAssets(int instanceID, int locationID);

		/// <summary>
		/// Return the name of the specified asset given its ID
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <returns></returns>
		string	GetAssetName	(int instanceID ,int assetID);

		/// <summary>
		/// Return whether or not database security has been enabled
		/// </summary>
		/// <returns></returns>
		bool	IsSecurityEnabled();

		/// <summary>
		/// Return whether or not the database has been opened
		/// </summary>
		/// <returns></returns>
		bool	IsOpen();
	}

#endregion AuditWizard DataBridge Interface Definition


#region AuditWizardDataBridge Class

	/// <summary>
	/// Properties and methods of this class will be visible to COM
	/// </summary>
	[ComVisible(true)]

	/// <summary>
	/// Unique identifier required from 'Create GUID' tool
	/// </summary>
	[Guid("C6659361-1625-4746-931C-36014B144438")]

	/// <summary>
	/// Name used when invoking this component 
	/// [e.g. CreateObject("AuditWizardDataBridge.AuditWizardDataBridgeImplementation")]
	/// </summary>	
	[ProgId("AuditWizardDataBridge.AuditWizardDataBridgeImplementation")]
	
	/// <summary>
	/// 'ClassInterfaceType.None' is only way to expose functionality through interfaces implemented explicitly by the class
	/// </summary>
	[ClassInterface(ClassInterfaceType.None)]

	/// <summary>
	/// Identifies the interface that will be exposed as COM event sources for the attributed class
	/// </summary>
	[ComSourceInterfaces(typeof(IAuditWizardDataBridgeInterface))] 

	public class AuditWizardDataBridge : IAuditWizardDataBridgeInterface
	{
		/// <summary>
		/// These fields define the version of the Auditwizard database supported by this DataBridge
		/// </summary>
		public static int SUPPORTED_DATABASE_MAJOR_VERSION = 8;
		public static int SUPPORTED_DATABASE_MINOR_MINIMUM_VERSION =4;
		public static int SUPPORTED_DATABASE_MINOR_MAXIMUM_VERSION = 5;

#region Data

		/// <summary>
		/// ItemType enumeration used to determine what we are expanding
		/// </summary>
		public enum ITEMTYPE { 
							   assetgroup							// A Group / Location is selected
							 , asset								// An Asset is selected
							 , asset_summary						// Asset Summary is selected
							 , asset_applications					// Asset > All Applications Selected
							 , asset_auditdata_category				// Asset audited data category selected
							 , asset_auditdata						// Asset audited data field selected
						     , asset_userdata						// Asset user data field selected
							 , asset_history						// Asset history record selected
							 , asset_publisher						// Application Publisher selected
							 , asset_application					// Individual application selected
							 , asset_os								// OS Family field selected
							 , asset_filesystem						// File System selected
							 , allassets							// 'All Assets' branch selected
							}


		#region Icon values
		public enum ICONS
		{
			ICON_LOCATION	= 2, 
			ICON_ASSET		= 3, 
			ICON_ATTR		= 4, 
			ICON_FOLDER		= 8, 
			ICON_APP		= 13,
			ICON_HW			= 15, 
			ICON_HIST		= 21,
			ICON_SUMMARY	= 26,
			ICON_IE			= 31, 
			ICON_IE_HIST	= 32,
			ICON_IE_COOKIE	= 33, 
			ICON_DATE		= 35,
			ICON_PERIPHERAL = 39,
			ICON_SYSTEM		= 28,
			ICON_PUBLISHER	= 55
		};

#endregion Icon mappings

		/// <summary>
		/// This flag indicates whether or not the database has been opened
		/// </summary>
		private bool	_databaseOpen = false;

		/// <summary>Flag to indicate whether or not we are logged in</summary>
		private bool	_loggedIn = false;

		/// <summary>Flag to indicate whether or not trace mode has been enabled</summary>
		private bool	_tracing = false;

		/// <summary>
		/// This is the text for any error encountered
		/// </summary>
		private string	_lastError = "No error stored";

		/// <summary>
		/// This is the Database Version object for the currently opened database
		/// </summary>
		DatabaseVersion	_databaseVersion = null;

		/// <summary>
		/// This is the currently logged in user where security has been enabled
		/// </summary>
		private User _loggedInUser = null;

		/// <summary>
		/// Flag to indicate whether or not security is enabled for this database
		/// </summary>
		private bool _securityEnabled = true;

		#endregion Data

		#region Constructor

		public AuditWizardDataBridge()
		{
#if DEBUG
			_tracing = true;
#endif
		}

		#endregion Constructor

		#region IAuditWizardDataBridgeInterface Implementation

		public string TestFunction(string baseString)
		{
			string returnValue = "You entered : " + baseString;
			return returnValue;
		}

			
		/// <summary>
		/// This function is called initially to open the database given the 
		/// </summary>
		/// <param name="databasePath"></param>
		/// <param name="connectionstring"></param>
		/// <returns></returns>
		public int Open(string serverName ,string username ,string password ,ref string error)
		{
			error = "None encountered";

			// Get an instance of the database
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			awDataAccess.ShowConnectionOnOpenError = false;

			// Configure the connection
			AuditWizardConnection awConnection = awDataAccess.DatabaseConnection;
			awConnection.ServerName = serverName;
			awConnection.UseTrustedConnection = false;
			awConnection.ServerUserName = username;
			awConnection.ServerPassword = password;
			
			// Try and open the database
			SqlConnection conn = awDataAccess.CreateOpenConnection2();
			if (conn == null)
			{
				error = awDataAccess.LastError;
				return 0;
			}

			// Try and obtain the database version as this will also confirm that we can open and access
			// the database.
			DatabaseVersion version = new DatabaseVersion();
			
			// Check the version against that which we support
			if ((version.MajorVersion != SUPPORTED_DATABASE_MAJOR_VERSION)
			|| (version.MinorVersion < SUPPORTED_DATABASE_MINOR_MINIMUM_VERSION)
			|| (version.MinorVersion > SUPPORTED_DATABASE_MINOR_MAXIMUM_VERSION))
			{
				error = string.Format("Incorrect Database Version Detected.  This AuditWizard Databridge supports an AuditWizard Database between (0).(1) and (2).{3}.  The actual database version recovered in {4}.(5)"
									  , SUPPORTED_DATABASE_MAJOR_VERSION, SUPPORTED_DATABASE_MINOR_MINIMUM_VERSION
									  , SUPPORTED_DATABASE_MAJOR_VERSION, SUPPORTED_DATABASE_MINOR_MAXIMUM_VERSION
									  , version.MajorVersion, version.MinorVersion);
				return 0;
			}

			// OK if security is not enabled then we can flag that we are logged in otherwise we will have to wait
			// for the user to actually login
			_securityEnabled = awDataAccess.SecurityStatus();
			if (_securityEnabled == false)
				_loggedIn = true;

			// Return an instance ID - this is fixed as we only allow one at a time
			_databaseOpen = true;
			int instanceID = 12345;
			return instanceID;
		}



		/// <summary>
		/// Logon to the database
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public int Logon(int instanceID, string username, string password)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

			// If the database is not open then return an error immediately
			if (!_databaseOpen)
			{
				_lastError = "The database has not been opened";
				return -1;
			}

			// If already logged in then simply return a 0 (success) status
			if (_loggedIn)
				return 0;

			// Validate the user/password specified
			_loggedInUser = awDataAccess.UserCheckPassword(username, password);
			if (_loggedInUser == null)
			{
				_lastError = "The username or password specified is invalid";
				return -1;
			}

			return 0;
		}


		public void Close(int instanceID)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			_loggedIn = false;
			_databaseOpen = false;
			return;
		}


		/// <summary>
		/// return the last database error
		/// </summary>
		/// <returns></returns>
		public string GetDBError()
		{
			return _lastError;
		}


		/// <summary>
		/// Enable or disable trace mode
		/// </summary>
		/// <param name="enable"></param>
		public void Trace(bool enable)
		{
			_tracing = enable;
		}



		/// <summary>
		/// Recover the database version string
		/// </summary>
		/// <returns></returns>
		public string GetVersionInfo(int instanceID)
		{
			string returnValue = "No Version";

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
				returnValue = _databaseVersion.VersionString;

			return returnValue;
		}



		/// <summary>
		/// Return the database id of the specified (named) asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public int GetAssetID(int instanceID, string assetName)
		{
			int assetID = 0;

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				assetID = awDataAccess.AssetFind(assetName, "");
			}

			return assetID;
		}



		/// <summary>
		/// Get the root company name
		/// </summary>
		/// <param name="instanceID"></param>
		/// <returns></returns>
		public string GetCompanyName(int instanceID)
		{
			string returnValue = "";

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				DataTable table = awDataAccess.GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.userlocation));
				AssetGroup rootAssetGroup = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.userlocation);
				returnValue = rootAssetGroup.Name;
			}

			return returnValue;
		}



		/// <summary>
		/// Return a list of locations (and assets) beneath the location specified
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="hintValue"></param>
		/// <param name="includeAssets"></param>
		/// <returns></returns>
		public object[] ExpandLocation(int instanceID, int hintValue, bool includeAssets)
		{
			List<string> listValues = new List<string>();

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				// Return a list of all of the locations beneath this one and add their Names to the list
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				
				// Create the parent Asset Group (location) setting its ID to the hint value specified
				AssetGroup parentGroup = new AssetGroup(AssetGroup.GROUPTYPE.userlocation);
				parentGroup.GroupID = hintValue;

				// Calling populate with recurse disabled with add the child groups and assets to this group
				parentGroup.Populate(false ,false ,false);

				// ...and add these locations to the tree as children of the selected node
				foreach (AssetGroup childGroup in parentGroup.Groups)
				{
					listValues.Add(Pack(childGroup.Name, ICONS.ICON_LOCATION, true, childGroup.GroupID));
				}

				// If we are to return assets also then we need to add these
				if (includeAssets)
				{
					foreach (Asset childAsset in parentGroup.Assets)
					{
						listValues.Add(Pack(childAsset.Name, ICONS.ICON_ASSET, true, childAsset.AssetID));
					}
				}
			}

			// Return the list as an array of objects
			return ConvertToObjectArray(listValues);
		}



		/// <summary>
		/// Return the name of the location for the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public string GetAssetLocation(int instanceID, int assetID)
		{
			string returnValue = "";

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				// Return a list of all of the locations beneath this one and add their Names to the list
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

				// Try and get the asset details
				Asset asset = awDataAccess.AssetGetDetails(assetID);
				if (asset != null)
					returnValue = asset.FullLocation;
			}
			
			return returnValue;
		}

		/// <summary>
		/// Return the value(s) for the specified data field for the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <returns></returns>
		public object[] GetValues(int instanceID, int assetID, string field, int hintValue)
		{
			List<string> listValues = new List<string>();
			
			// Tracing - add input fields
			//listValues.Add("AssetID|field|Hint Value");
			//listValues.Add(assetID.ToString() + "|" + field + "|" + hintValue.ToString());
			//return ConvertToObjectArray(listValues);

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				// Return a list of all of the locations beneath this one and add their Names to the list
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

				// Which branch are we recovering values for?
				if (field.StartsWith(GetIconName(ICONS.ICON_ATTR)))
				{
					GetValuesForUserData(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_SUMMARY)))
				{
					GetAssetSummaryValues(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_APP)))
				{
					GetValuesForPublisher(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_IE)))
				{
					GetValuesForInternet(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_HW)))
				{
					GetValuesForHardware(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_SYSTEM)))
				{
					GetValuesForHardware(assetID, field, hintValue, listValues);
				}

				else if (field.StartsWith(GetIconName(ICONS.ICON_HIST)))
				{
					GetValuesForHistory(assetID, field, hintValue, listValues);
				}
			}

			// Return the list as an array of objects
			return ConvertToObjectArray(listValues);
		}



		/// <summary>
		/// Expand the specified asset
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <returns></returns>
		public object[] ExpandAsset(int instanceID, int assetID, string field, int hintValue)
		{
			List<string> listValues = new List<string>();

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				// Return a list of all of the locations beneath this one and add their Names to the list
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

				// Now act as required for the item being expanded...
				if (field == "")
					ExpandAssetRoot(listValues ,hintValue);
	
				else if (field.StartsWith(GetIconName(ICONS.ICON_IE)))
					ExpandAssetInternet(assetID ,field ,hintValue ,listValues);
				
				else if (field.StartsWith(GetIconName(ICONS.ICON_APP)))
					ExpandAssetPublishers(assetID, hintValue, listValues);
				
				else if (field.StartsWith(GetIconName(ICONS.ICON_HW)))
					ExpandAssetHardware(assetID, GetIconName(ICONS.ICON_HW), field, hintValue, listValues);
				
				else if (field.StartsWith(GetIconName(ICONS.ICON_SYSTEM)))
					ExpandAssetHardware(assetID, GetIconName(ICONS.ICON_SYSTEM), field, hintValue, listValues);

				else if (field.StartsWith(GetIconName(ICONS.ICON_ATTR)))
					ExpandAssetUserDefinedData(assetID, hintValue, listValues);

				else if (field.StartsWith(GetIconName(ICONS.ICON_HIST)))
					ExpandAssetHistory(assetID, hintValue, listValues);
			}

			// Return the list as an array of objects
			return ConvertToObjectArray(listValues);
		}



		/// <summary>
		/// Return a match to the specified asset find string
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="findBy"></param>
		/// <param name="findString"></param>
		/// <param name="exactMatches"></param>
		/// <returns></returns>		
		public object[] FindAsset(int instanceID, int findBy, string findString, bool exactMatches)
		{
			List<string> listValues = new List<string>();

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				AssetList listAssets = new AssetList(awDataAccess.GetAssets(0 ,AssetGroup.GROUPTYPE.userlocation ,false) ,true);

				// Loop through this list searching for assets which match the criteria
				foreach (Asset asset in listAssets)
				{
					if (IsMatchingAsset(asset ,findBy, findString, exactMatches))
						listValues.Add(Pack(asset.Name, ICONS.ICON_ASSET, true, asset.AssetID));
				}			
			}

			// Return the list as an array of objects
			return ConvertToObjectArray(listValues);
		}


		/// <summary>
		/// Enumerate the assets at the specified location
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="locationID"></param>
		/// <returns></returns>
		public object[] EnumAssets(int instanceID, int locationID)
		{
			List<string> listValues = new List<string>();

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				AssetList listAssets = new AssetList(awDataAccess.GetAssets(locationID, AssetGroup.GROUPTYPE.userlocation ,false), true);

				// Iterate through the returned list of Asset objects and format them for return as strings
				foreach (Asset asset in listAssets)
				{
					if (asset.Auditable)
						listValues.Add(Pack(asset.Name, ICONS.ICON_PERIPHERAL, true, asset.AssetID));
					else
						listValues.Add(Pack(asset.Name, ICONS.ICON_ASSET, true, asset.AssetID));
				}
			}

			// Return the list as an array of objects
			return ConvertToObjectArray(listValues);
		}



		/// <summary>
		/// Return the name of the specified asset given its ID
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public string GetAssetName(int instanceID, int assetID)
		{
			string assetName = "";

			// First ensure that the database is open and the user logged in
			if (ValidateOpenAndLoggedIn() == 0)
			{
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				Asset asset = awDataAccess.AssetGetDetails(assetID);
				assetName = asset.Name;
			}

			return assetName;
		}



		/// <summary>
		/// Return whether or not database security has been enabled
		/// </summary>
		/// <returns></returns>
		public bool IsSecurityEnabled()
		{
			return _securityEnabled;
		}


		/// <summary>
		/// Return whether or not the database has been opened
		/// </summary>
		/// <returns></returns>
		public bool IsOpen()
		{
			return _databaseOpen;
		}


		#endregion IAuditWizardDataBridgeInterface Implementation

		#region Helper Functions

		#region Database Helper Functions

		/// <summary>
		/// Helper function which will be called before most functions to ensure that the database has been
		/// opened and that the user is logged in.
		/// </summary>
		/// <returns></returns>
		private int ValidateOpenAndLoggedIn()
		{

			// If the database is not open then return an error immediately
			if (!_databaseOpen)
			{
				_lastError = "The database has not been opened";
				return -1;
			}

			// If already logged in then simply return a 0 (success) status
			if (!_loggedIn)
			{
				_lastError = "You must login to AuditWizard before accessing the database";
				return -1;
			}

			return 0;
		}

		#endregion Database Helper Functions

		#region Return Data Helper Functions

		/// <summary>
		/// Called to pack return values into a pipe separated string
		/// </summary>
		/// <param name="item"></param>
		/// <param name="icon"></param>
		/// <param name="hasChildren"></param>
		/// <param name="hintValue"></param>
		/// <returns></returns>
		protected string Pack (string item ,ICONS icon ,bool hasChildren ,Int64 hintValue)
		{
			return string.Format("{0}|{1}|{2}|{3}" ,item ,(int)icon ,(hasChildren) ? 1 : 0 ,hintValue);
		}


		/// <summary>
		/// This is used to convert a list of strings passed to an array of objects
		/// </summary>
		/// <param name="arrayIn"></param>
		/// <returns></returns>
		public object[] ConvertToObjectArray(List<string> listStrings)
		{
			object[] arrayOut = new object[listStrings.Count];
			for (int i = 0; i < listStrings.Count; i++)
			{
				arrayOut[i] = listStrings[i];
			}

			return arrayOut;
		}

		#endregion Return Data Helper Functions

		#region Find Helper Functions

		/// <summary>
		/// Called to match the supplied asset against the search criteria
		/// </summary>
		/// <param name="asset"></param>
		/// <param name="findBy"></param>
		/// <param name="findString"></param>
		/// <param name="exactMatches"></param>
		/// <returns></returns>
		protected bool IsMatchingAsset(Asset asset, int findBy, string findString, bool exactMatches)
		{
			return (exactMatches) ? IsExactMatchAsset(asset, findBy, findString) : IsNonExactMatchAsset(asset, findBy, findString);
		}



		protected bool IsExactMatchAsset(Asset asset, int findBy, string findString)
		{
			bool status = false;

			switch (findBy)
			{
				// Find by Asset Name
				case 0:
					if (asset.Name == findString)
						status = true;
					break;

				// Find by IP Address
				case 1:
					if (asset.IPAddress == findString)
						status = true;
					break;

				// Find by logged on username
				case 2:
					// Get the network user name for this asset
					string username = "";
					AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
					DataTable table = awDataAccess.GetAuditedItemValues(asset, "Hardware|Network", "Username");
					if (table.Rows.Count != 0)
					{
						DataRow row = table.Rows[0];
						username = (string)row["_VALUE"];
						if (username == findString)
							status = true;
					}
					break;

				// Find by Asset Tag (Serial Number)
				case 3:
					// We actually already have the serial number as it is an attributes of the asset
					if (asset.SerialNumber == findString)
						status = true;
					break;

				// Find by Site (return all assets at the specified site)
				case 4:
					// We have the site also as it is an attribute of the asset
					status = asset.Location.EndsWith(findString);
					break;
			}

			return status;
		}


		protected bool IsNonExactMatchAsset(Asset asset, int findBy, string findString)
		{
			bool status = false;

			switch (findBy)
			{
				// Find by Asset Name - matches if we find the supplied string ANYWHERE in the asset name
				case 0:
					status = asset.Name.Contains(findString);
					break;

				// Find by IP Address - matches if we find the supplied string ANYWHERE in the IP address
				case 1:
					status = asset.IPAddress.Contains(findString);
					break;

				// Find by logged on username - matches if we find the supplied string ANYWHERE in the Username
				case 2:
					// Get the network user name for this asset
					string username = "";
					AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
					DataTable table = awDataAccess.GetAuditedItemValues(asset, "Hardware|Network", "Username");
					if (table.Rows.Count != 0)
					{
						DataRow row = table.Rows[0];
						username = (string)row["_VALUE"];
						status = username.Contains(findString);					
					}
					break;

				// Find by Asset Tag (Serial Number)
				case 3:
					// We actually already have the serial number as it is an attributes of the asset
					status = asset.SerialNumber.Contains(findString);
					break;

				// Find by Site (return all assets at the specified site)
				case 4:
					// We have the site also as it is an attribute of the asset
					status = asset.Location.Contains(findString);
					break;
			}

			return status;
		}

		#endregion Find Helper Functions


		#region Expand Functions

		/// <summary>
		/// This function is called when we are expanding the root item for an asset
		/// </summary>
		/// <param name="listValues"></param>
		/// <param name="hintValue"></param>
		private void ExpandAssetRoot(List<string> listValues ,int hintValue)
		{
			// If this is an 'auditable' asset then we must add one set of items otherwise the
			// items are somewhat different
			//
			//	'Applications'			
			//	'Hardware'				(Only if audited)
			//	'System'				(Only if audited)
			//  'Interet Explorer'		(Only if audited)
			//	'History'
			//  'User Defined Data category #1'
			//  ...
			//  'User Defined Data category #n'
			//
			// All Assets have Summary
			listValues.Add(Pack(GetIconName(ICONS.ICON_SUMMARY), ICONS.ICON_SUMMARY, false, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_ATTR), ICONS.ICON_ATTR, true, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_APP), ICONS.ICON_APP, true, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_HW), ICONS.ICON_HW, true, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_SYSTEM), ICONS.ICON_SYSTEM, true, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_IE), ICONS.ICON_IE, true, 0));
			listValues.Add(Pack(GetIconName(ICONS.ICON_HIST), ICONS.ICON_HIST, true, 0));
		}


		/// <summary>
		/// This function is called when we are expanding the root item for an asset
		/// </summary>
		/// <param name="listValues"></param>
		/// <param name="hintValue"></param>
		private void ExpandAssetPublishers(int assetID ,int hintValue ,List<string> listValues)
		{
			if (hintValue == 0)
			{
				ApplicationPublisherList listPublishers = new ApplicationPublisherList("", true, true);

				// Add the publishers to the return list 
				foreach (ApplicationPublisher thePublisher in listPublishers)
				{
					listValues.Add(Pack(thePublisher.Name, ICONS.ICON_PUBLISHER, false, 1));
				}
			}
		}



		/// <summary>
		/// This function is called when we are expanding the INTERNET item for an asset
		/// </summary>
		/// <param name="listValues"></param>
		/// <param name="hintValue"></param>
		private void ExpandAssetInternet(int assetID, string field ,int hintValue, List<string> listValues)
		{
			if (hintValue == 0)
			{
				listValues.Add(Pack(GetIconName(ICONS.ICON_IE_HIST), ICONS.ICON_IE_HIST, true, 1));
				listValues.Add(Pack(GetIconName(ICONS.ICON_IE_COOKIE), ICONS.ICON_IE_COOKIE, false, 1));
			}

			else
			{
				// Only Internet History can be expanded and should display a list of the dates for which
				// history has been recovered
				List<string> historyDates = GetAssetInternetHistoryDates(assetID);
				foreach (string date in historyDates)
				{
					listValues.Add(Pack(date, ICONS.ICON_DATE, false, 1));
				}
			}
		}



		/// <summary>
		/// This function is called when we are expanding the Hardware branch for an asset
		/// The 'hint value' passed to us in fact the database ID of the parent for which we
		/// want to return child categories.  
		/// 
		/// If 0, this indicates that we are returning top level categories directly beneath 'Hardware' for this asset
		/// 
		/// One MAJOR complication is that the we are only returning ONE level at a time and some items
		/// do not have their parent level actually in the database - for example
		/// 
		///		Hardware | Adapters | Network | Network Adapter #0
		/// 
		/// We only have the level Hardware actually in the database - all the levels in the middle are assumed
		/// however for the purposes of the DataBridge we need to fill in the levels in between so that the 
		/// caller is able to display them one level at a time
		/// 
		/// </summary>
		/// <param name="listValues"></param>
		/// <param name="hintValue"></param>
		private void ExpandAssetHardware(int assetID, string rootType ,string field ,int hintValue, List<string> listValues)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

			// Formulate the parent category name, if the hint value is 0 then we are expanding Hardware item itself
			// otherwise we must get the name of the item at the database ID specified bu hint value
			string parentCategory;
			if (hintValue == 0)
			{
				parentCategory = rootType;
			}
			else
			{
				// Recover the WHOLE category name noting that this may not actually be the name which we need as we
				// may be filling in levels which do not actually exist in the database.  The way we do this is to make 
				// use of the 'field' parameter this tells us where about in the category string we are
				DataTable table = awDataAccess.GetAuditedItem(hintValue);
				if (table.Rows.Count != 1)
					return;
				AuditedItem auditedItem = new AuditedItem(table.Rows[0]);
				parentCategory = auditedItem.Category;
				int index = parentCategory.IndexOf(field);
				if (index != -1)
					parentCategory = parentCategory.Substring(0 ,index + field.Length);
			}

			// Count the levels in the parent category as we will be stripping back the returned items to only show
			// immediate child items.
			List<string> listParentCategories = ListFromString(parentCategory, '|', true);
			int parentLevel = listParentCategories.Count;

			// Now get the audited item category names beneath this - note that this will return all child, grandchild items etc
			// as we need to be able to fill in the gaps in the tree
			AuditedItemList listAuditedItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, parentCategory, true));

			// Add the Category names to the return list
			List<string> addedCategories = new List<string>();
			foreach (AuditedItem item in listAuditedItems)
			{
				// Split the name of the category into its components i.e. Hardware|Network is split into Hardware and Network
				List<string> listCategories = ListFromString(item.Category, '|', true);

				// If this item is flagged as being grouped then we do not include the last segment of the item name
				// as this is the item on which we shall group 
				if (item.Grouped)
					listCategories.RemoveAt(listCategories.Count - 1);

				// Now check to see if the item has any children - if the item hasmore levels than the parent then
				// we assume it has otherwise we need to look in the database
				bool hasChildren;
				if ((listCategories.Count - 1) <= parentLevel)
				{
					AuditedItemList listChildItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, item.Category, true));
					hasChildren = (listChildItems.Count != 0);
				}
				else
				{
					hasChildren = true;
				}
				
				// As we are only returning immediate children we need to recover the name at the level immediately
				// below that of the oparent item.
				string name = listCategories[parentLevel];

				// Have we already added this category?
				if (addedCategories.Contains(name))
					continue;
				addedCategories.Add(name);

				// Now pack the data to return to the caller
				listValues.Add(Pack(name, ICONS.ICON_HW, hasChildren, item.ItemID));
			}
		}



		/// <summary>
		/// Expand User Defined Data Categories - note that we do now show children beneath the categories and therefore
		/// know that we will always just be adding the categories and never the items
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void ExpandAssetUserDefinedData(int assetID, int hintValue, List<string> listValues)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();

			// We can only expand the root item so return a list of the user defined data categories
			UserDataCategoryList listUserDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
			listUserDataCategories.Populate();

			// Add these to the return list
			foreach (UserDataCategory category in listUserDataCategories)
			{
				listValues.Add(Pack(category.Name, ICONS.ICON_ATTR, false, category.CategoryID));
			}
		}




		/// <summary>
		/// This function is called when we are expanding the HISTORY item for an asset
		/// We display a list of the History Dates
		/// </summary>
		/// <param name="listValues"></param>
		/// <param name="hintValue"></param>
		private void ExpandAssetHistory(int assetID, int hintValue, List<string> listValues)
		{
			List<string> historyDates = GetAssetHistoryDates(assetID);
			foreach (string date in historyDates)
			{
				listValues.Add(Pack(date, ICONS.ICON_DATE, false, 1));
			}
		}

#endregion Expand Functions

		#region GetValues Functions


		/// <summary>
		/// This function is called to return the Asset Summary values.  These are basic properties of the 
		/// asset such as its category, type, make, model and serial number
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetAssetSummaryValues (int assetID, string field, int hintValue, List<string> listValues)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			Asset asset = awDataAccess.AssetGetDetails(assetID);

			// The first item which we pack into the return list tells the caller what headings to display
			string value = "Item|Value";
			listValues.Add(value);

			// Now add Status, Make, Model and Serial Number
			Dictionary<Asset.STOCKSTATUS, string> stockStatuses = Asset.StockStatuses();
			listValues.Add("Status|" + stockStatuses[asset.StockStatus]);
			listValues.Add("Make|" + asset.Make);
			listValues.Add("Model|" + asset.Model);
			listValues.Add("Date of Last Audit|" + asset.LastAuditDateString);
			listValues.Add("IP Address|" + asset.IPAddress);
			listValues.Add("MAC Address|" + asset.MACAddress);
			listValues.Add("Serial Number|" + asset.SerialNumber);
		}


		/// <summary>
		/// This function is called to return the list of installed applications, for the specified asset
		/// and publisher.
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetValuesForPublisher(int assetID, string field, int hintValue, List<string> listValues)
		{
			// If the hint value is 0 then we should just return the list of publishers
			if (hintValue == 0)
			{
				listValues.Add("Publisher|");
				ApplicationPublisherList listPublishers = new ApplicationPublisherList("", true, true);

				// Add the publishers to the return list 
				foreach (ApplicationPublisher thePublisher in listPublishers)
				{
					listValues.Add(thePublisher.Name+"|");
				}
			}

			else
			{
				// A specific publisher so should return a list of the applications beneath a specific publisher
				// First split the field into its components
				//	0 - Applications Title
				//  1 - Publisher
				string publisher = "";
				string[] fieldParts = field.Split('|');
				if (fieldParts.Length > 1)
					publisher = fieldParts[1];
				else
					publisher = fieldParts[0];

				// The first item which we pack into the return list tells the caller what headings to display
				string value = "Application|Version|Serial Number|CD Key";
				listValues.Add(value);

				// OK now get a list of applications for this asset and publisher
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				Asset asset = new Asset();
				asset.AssetID = assetID;
				DataTable applicationsTable = awDataAccess.GetInstalledApplications(asset, publisher, true, true);

				foreach (DataRow row in applicationsTable.Rows)
				{
					ApplicationInstance newApplication = new ApplicationInstance(row);
					value = String.Format("{0}|{1}|{2}|{3}"
										 , newApplication.Name
										 , newApplication.Version
										 , newApplication.Serial.ProductId
										 , newApplication.Serial.CdKey);
					listValues.Add(value);
				}
			}
		}



		/// <summary>
		/// This function is called to return the list of installed applications, for the specified asset
		/// and publisher.
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetValuesForUserData(int assetID, string field, int hintValue, List<string> listValues)
		{
			// If the hint value is 0 then we should just shhow the user data categories
			if (hintValue == 0)
			{
				listValues.Add("Category Name|");
				UserDataCategoryList userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
				userDataCategories.Populate();
				foreach (UserDataCategory category in userDataCategories)
				{
					listValues.Add(category.Name + "|");
				}
			}

			else
			{
				// The first item which we pack into the return list tells the caller what headings to display
				listValues.Add("Name|Value");

				// The hint value will gie us the ID of the user defined data category for which we want to return
				// field values for the specified asset so call the database function to get the values
				UserDataCategoryList userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
				userDataCategories.Populate();
				UserDataCategory category = userDataCategories.FindCategory(hintValue);
				category.GetValuesFor(assetID);

				// Now add the field names and values to the eturn list
				foreach (UserDataField dataField in category)
				{
					string fieldValue = dataField.GetValueFor(assetID);
					listValues.Add(String.Format("{0}|{1}" , dataField.Name , fieldValue));
				}
			}
		}




		/// <summary>
		/// This function is called to return values for Internet Information.
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetValuesForInternet(int assetID, string field, int hintValue, List<string> listValues)
		{
			// If the hint value is 0 then we should just shhow the user data categories
			if (hintValue == 0)
			{
				listValues.Add("Item|");
				listValues.Add(GetIconName(ICONS.ICON_IE_HIST) + "|");
				listValues.Add(GetIconName(ICONS.ICON_IE_COOKIE) + "|");
			}

			// If the field ends with 'History|' then we need to display the dates for the history
			else if (field.EndsWith(GetIconName(ICONS.ICON_IE_HIST)))
			{
				listValues.Add("Date|");
				List<string> historyDates = GetAssetInternetHistoryDates(assetID);
				foreach (string date in historyDates)
				{
					listValues.Add(date + "|");
				}
			}

			else if (field.EndsWith(GetIconName(ICONS.ICON_IE_COOKIE)))
			{
				listValues.Add("URL|Date");
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				AuditedItemList listInternetItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, "Internet|Cookie", true));
				//
				foreach (AuditedItem item in listInternetItems)
				{
					int delimiter = item.Category.LastIndexOf('|');
					string cookieURL = item.Category.Substring(delimiter + 1);
					listValues.Add(cookieURL + "|" + item.Value);
				}
			}

			else
			{
				// We must be displaying the Internet History records for a specific date so recover them
				listValues.Add("URL|Numer of Pages");
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				AuditedItemList listInternetItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, field, true));
				//
				foreach (AuditedItem item in listInternetItems)
				{
					int delimiter = item.Category.LastIndexOf('|');
					string historyURL = item.Category.Substring(delimiter + 1);
					listValues.Add(historyURL + "|" + item.Value);
				}
			}
		}




		/// <summary>
		/// This function is called to return values for Asset History.
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetValuesForHistory(int assetID, string field, int hintValue, List<string> listValues)
		{
			// If the hint value is 0 then we should just show the dates for which History is available
			if (hintValue == 0)
			{
				listValues.Add("Date|");
				List<string> historyDates = GetAssetHistoryDates(assetID);
				foreach (string date in historyDates)
				{
					listValues.Add(date + "|");
				}
			}

			// Otherwise we are displaying the history records for a specific date
			else
			{
				// Add the headings row as we need these regardless
				listValues.Add("Date|Operation|User");

				// Now determine the date for which we want records
				int delimiter = field.LastIndexOf('|');
				string historyDateString = field.Substring(delimiter + 1);
				DateTime historyDate = Convert.ToDateTime(historyDateString);

				// Create the asset object and request the history records for this asset and date
				Asset asset = new Asset();
				asset.AssetID = assetID;
				AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
				DataTable historyTable = awDataAccess.GetAssetAuditHistory(asset, historyDate, historyDate);

				// Add the entries in the data table as ATE records to our DataSet
				foreach (DataRow row in historyTable.Rows)
				{
					AuditTrailEntry ate = new AuditTrailEntry(row);
					string description = ate.GetTypeDescription();
					description.Replace('|', '>');
					listValues.Add(ate.Date.ToString() + "|" + description + "|" + ate.Username);
				}
			}
		}



		/// <summary>
		/// This function is called to return values for the specified hardware item and asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="field"></param>
		/// <param name="hintValue"></param>
		/// <param name="listValues"></param>
		private void GetValuesForHardware(int assetID, string field, int hintValue, List<string> listValues)
		{
			// The hint value will give us the ID of the hardware category, the field gives us the name of the
			// actual hardware category being displayed.  We need to trim back the hardware category to only show 
			// those values at the same level as the field.
			//
			// For example, the Hardware Category might be Hardware|Adapters|Network|Network Adapter #0 but the field
			// might be 'Adapters' showing that we are actually displaying values at the 'Adapters' level and not 
			// at the hardware category level.
			//
			// We first need to get the textual hardware category based on its ID passed in as the hint value
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			DataTable table = awDataAccess.GetAuditedItem(hintValue);
			if (table.Rows.Count != 1)
				return;
			AuditedItem auditedItem = new AuditedItem(table.Rows[0]);

			// Now we branch off to display as either non-grouped or grouped as required
			if (auditedItem.Grouped)
			{
				GetValuesForHardwareGrouped(assetID, field, auditedItem, listValues);
			}
			else
				GetValuesForHardwareNotGrouped(assetID, field, auditedItem, listValues);
		}


		/// <summary>
		/// Display item values for a Hardware category which is marked as 'grouped'
		/// </summary>
		/// <param name="field"></param>
		/// <param name="auditedItem"></param>
		/// <param name="listValues"></param>
		private void GetValuesForHardwareGrouped(int assetID, string field, AuditedItem auditedItem ,List<string> listValues)
		{
			Asset asset = new Asset();
			asset.AssetID = assetID;
			
			// We know that the first column will be 'name' so add it
			string columnNames = "Name|";

			// OK - If we get all of the child 'categories' beneath this category then these are the 'names' of the 
			// items in the group i.e. System|Active Processes|SMSS.EXE
			//
			// 'field' at this point is the parent category that is in the case above 'field' will be set to
			// 'System|Active Processes' so we need to first pull a list of each of the categories beneath this
			// which will in effect be the names of the services.
			//
			// These names will be used as the first column of each row - effectively we group the attributes of
			// these names into the row
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			AuditedItemList listAuditedItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, field ,true));

			// Get the individual children in this category (in the exacmple this will be the process names)
			List<string> listChildNames = listAuditedItems.GetChildrenInCategory(field);
			if (listChildNames.Count == 0)
				return;

			// But what other columns do we need?  
			//
			// If we take the example of an Active process - System|Active Processes|SMSS.EXE we have audited 3 attributes 
			// per process - Name, Executable and PID - these are the same for ALL active Processes and will therefore
			// be the columns that we will add.  The easiest way to get the columns is simply to get the items in the
			// first named category and assume that all other items in this category have the same attributes
			List<AuditedItem> listColumns = listAuditedItems.GetItemsInCategory(listChildNames[0]);

			// Add columns for each item
			foreach (AuditedItem childColumn in listColumns)
			{
				columnNames = columnNames + "|" + childColumn.Name;
			}
			listValues.Add(columnNames);

			// Now we have the columns sorted we can add as the first item in the list and then move on to the 
			// values which will be displayed for each column.
			//
			// Note that column 0 is the category name and the remainder of the columns are the attributes
			// We therefore have 'n' + 1 columns where 'n' is the number of items in the second list plus 
			// an extra column forthe item name
			foreach (string rowName in listChildNames)
			{
				// The rowName is actually held as a delimited string with the full name of the category - we only want
				// to display the last segment in the row
				string name;
				int lastDelimiter = rowName.LastIndexOf('|');
				if (lastDelimiter == rowName.Length)
					name = "";
				else
					name = rowName.Substring(lastDelimiter + 1);

				// The first value on the row is the rowName (the process name in our example)
				string rowValues = name + "|";

				// Now add on the grouped values from the second list
				List<AuditedItem> columnValues = listAuditedItems.GetItemsInCategory(rowName);
				foreach (AuditedItem columnItem in columnValues)
				{
					rowValues = rowValues + "|" + columnItem.Value;
				}

				// Add this row to our return list
				listValues.Add(rowValues);
			}			
		}


		/// <summary>
		/// Display item values for the specified hardare/system category where grouping is not in force
		/// </summary>
		/// <param name="field"></param>
		/// <param name="auditedItem"></param>
		/// <param name="listValues"></param>
		private void GetValuesForHardwareNotGrouped(int assetID, string field, AuditedItem auditedItem, List<string> listValues)
		{
			Asset asset = new Asset();
			asset.AssetID = assetID;

			// The first item which we pack into the return list tells the caller what headings to display
			listValues.Add("Item|Value");

			// Get the parent category from the auditeditem
			string parentCategory = auditedItem.Category;
			int index = parentCategory.IndexOf(field);
			if (index != -1)
				parentCategory = parentCategory.Substring(0 ,index + field.Length);

			// OK get a list of the data field names in this category if any
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			DataTable namesTable = awDataAccess.GetAuditedItemCategoryNames(parentCategory);

			// Add any NAMES after the Categories
			foreach (DataRow row in namesTable.Rows)
			{
				string fieldValue = "";
				string fieldName = (string)row["_NAME"];

				// Get the value for this category, asset and value
				DataTable valuesTable = awDataAccess.GetAuditedItemValues(asset, parentCategory, fieldName);
				if (valuesTable.Rows.Count != 0)
				{
					DataRow namesRow = valuesTable.Rows[0];
					fieldValue = (string)namesRow["_VALUE"];
				}

				string value = String.Format("{0}|{1}", fieldName, fieldValue);
				listValues.Add(value);
			}
		}

		#endregion GetValues Functions

		#region Helper Functions

		private List<string> GetAssetInternetHistoryDates(int assetID)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			AuditedItemList listInternetItems = new AuditedItemList(awDataAccess.GetAuditedItems(assetID, "Internet|History", true));

			// Iterate through the items recovering the dates as this is what we shall display in the expansion
			// The date will be item 2 in the list
			List<string> historyDates = new List<string>();
			foreach (AuditedItem item in listInternetItems)
			{
				List<string> listFields = ListFromString(item.Category, '|', true);
				if (listFields.Count >= 3)
				{
					string date = listFields[2];
					if (!historyDates.Contains(date))
						historyDates.Add(date);
				}
			}

			return historyDates;
		}



		/// <summary>
		/// Return a list of the Dates for which asset history is available
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		private List<string> GetAssetHistoryDates(int assetID)
		{
			AuditWizardDataAccess awDataAccess = new AuditWizardDataAccess();
			Asset asset = new Asset();
			asset.AssetID = assetID;
			DataTable historyTable = awDataAccess.GetAssetAuditHistory(asset, new DateTime(0), new DateTime(0));
			List<string> historyDates = new List<string>();

			// Add the entries in the data table as ATE records to our DataSet
			foreach (DataRow row in historyTable.Rows)
			{
				AuditTrailEntry ate = new AuditTrailEntry(row);
				if (!historyDates.Contains(ate.Date.ToString()))
					historyDates.Add(ate.Date.ToString());
			}

			return historyDates;
		}

		///
		/// <summary>
		/// Return a textual name given an icon id
		/// </summary>
		private string GetIconName(ICONS iconID)
		{
			switch (iconID)
			{
				case ICONS.ICON_LOCATION:	return "Location";
				case ICONS.ICON_ASSET:		return "Asset";
				case ICONS.ICON_SUMMARY:	return "Summary";
				case ICONS.ICON_PERIPHERAL: return "Peripheral";
				case ICONS.ICON_ATTR:		return "User Defined Data";
				case ICONS.ICON_FOLDER:		return "Folders/Files";
				case ICONS.ICON_APP:		return "Applications";
				case ICONS.ICON_HW:			return "Hardware";
				case ICONS.ICON_HIST:		return "History";
				case ICONS.ICON_IE:			return "Internet";
				case ICONS.ICON_IE_HIST:	return "History";
				case ICONS.ICON_IE_COOKIE:	return "Cookies";
				case ICONS.ICON_SYSTEM:		return "System";
				case ICONS.ICON_DATE:		return "Date";
				default: return ""; break;
			}
		}

		/// <summary>
		/// Construct a List of strings from the delimited string passed in
		/// </summary>
		/// <param name="source">Delimited string</param>
		/// <param name="separator">Single character separator</param>
		/// <returns></returns>
		protected List<string> ListFromString(string source, char separator, bool removeBlankEntries)
		{
			List<string> outputList = new List<string>();

			// Set options as required
			StringSplitOptions options = StringSplitOptions.None;
			if (removeBlankEntries)
				options = StringSplitOptions.RemoveEmptyEntries;

			string[] tokens = source.Split(new char[] { separator }, options);
			outputList.AddRange(tokens);
			return outputList;
		}
		#endregion Helper Functions

		#endregion Helper Functions
	}

	#endregion AuditWizardDataBridge Class
}
