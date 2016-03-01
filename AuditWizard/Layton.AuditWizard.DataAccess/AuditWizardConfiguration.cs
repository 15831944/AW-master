using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{

	public class AuditWizardConfiguration : IniFile
	{
#region Data Declarations
		private static String AuditWizard_INIFILE = "AuditWizard.ini";

		// Section and Key Names
		public const String SECTION_USERDETAILS = "User Details";
		public const String KEY_ORGANIZATION = "Organization";
		public const String KEY_USERNAME = "Name";
		public const String KEY_EMAIL = "Email";
		public const String KEY_NOTIFYUSER = "Notify on Complete";

		public const String SECTION_DATABASE = "Database";
		public const String KEY_DBNAME = "Name";
		public const String KEY_DBSERVER = "Server";
		public const String KEY_TRUSTEDCONNECTION = "TrustedConnection";

		public const String SECTION_DISCOVERY = "Discovery";
		public const String KEY_USEACTIVEDIRECTORY = "Use ActiveDirectory";
		public const String KEY_USENETBIOS = "Use NetBIOS";
		public const String KEY_CUSTOMADSTRING = "Custom ActiveDirectory String";
		public const String KEY_CUSTOMNBSTRING = "Custom NetBIOS String";
		public const String KEY_REDISCOVER_INTERVAL = "Rediscover Interval";

		public const String SECTION_SCANNER = "Scan";
        public const String KEY_SCANNETWORK = "Network Scan";
        public const String KEY_MAXIMUMTHREADS = "Maximum Threads";
        public const String KEY_DOMAINUSER = "Domain Username";
		public const String KEY_DOMAINPASSWORD = "Domain Password";
		public const String KEY_REAUDIT_INTERVAL = "Reaudit Interval";
		public const String KEY_RECHECK_INTERVAL = "Recheck Interval";
		public const String KEY_FILTER_PUBLISHERS = "Filter Publishers";
		public const String KEY_PUBLISHER_LIST = "Publisher List";

		public const String SECTION_UI		= "User Interface";
		public const String KEY_VISTATREES	= "Vista Trees";
		public const String KEY_DOMAINVIEW	= "Domain View";

		// Data held in the User Details section
		/// <summary>Name of the Organization</summary>
		private String	_organization;
		/// <summary>Name of user</summary>
		private String	_name;
		/// <summary>Email address for the above user</summary>
		private String	_email;
		/// <summary>true if we are to email the above user when the scan is complete</summary>
		private bool	_notifyOnComplete;

		// Data items held in the [Database] Section
		/// <summary>Name of the SQL Server database to connect</summary>
		private String _databaseName;
		/// <summary>Name of the SQL Server DB instance to connect to</summary>
		private String	_databaseServer;
		/// <summary>true if we are to use a trusted conection to connect to the above database</summary>
		private bool	_trustedConnection;

		// Data Items held in the [Discovery] section
		/// <summary>True if AD discovery requested</summary>
		private bool	_useActiveDirectoryDiscovery;
		/// <summary>True if NetBIOS discovery requested</summary>
		private bool	_useNetBiosDiscovery;
		/// <summary>Custom LDAP string - null or empty string = use default</summary>
		private String	_customActiveDirectoryString;
		/// <summary>Custom NetBIOS string, actually a comma delimited list of domains to scan</summary>
		private String	_customNetBiosString;
		/// <summary>Interval in MINUTES between successive network discovery attempts</summary>
		private int		_rediscoverInterval;

		// Data held in the [Scan] section
		/// <summary>True if network scan requested</summary>
		private bool _scanNetwork;
		/// <summary>Domain user name to run scan under - empty string for current user</summary>
		private String	_domainUser;
		/// <summary>Domain password to run scan under - empty string for current user</summary>
		private String	_domainPassword;
        /// <summary>Maximum number of threads used by the scanner</summary>
        private int _scanThreads;
		/// <summary>Interval in DAYS between successive audits of a single Asset</summary>
		private int		_reauditInterval;
		/// <summary>Interval in MINUTES between rechecks of the list of Computers</summary>
		private int		_recheckInterval;

		//Data held in the UI section
		private bool _vistaTrees;

		/// <summary>true if we are displaying computers in their domains, false to display by user locations</summary>
		private bool _showByDomain;

#endregion Data Declarations

#region Data Accessors

		/// <summary>Organization Name</summary>
		public String Organization
		{
			get { return _organization; }
			set { _organization = value; }
		}

		/// <summary>Users Name</summary>
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>Users Email Address</summary>
		public String Email
		{
			get { return _email; }
			set { _email = value; }
		}

		/// <summary>Notify the user when the discovery and auditing completes</summary>
		public bool NotifyOnComplete 
		{
			get { return _notifyOnComplete; }
			set { _notifyOnComplete = value; }
		}

		/// <summary>Are we to perform an Active Directory discovery</summary>
		public bool UseActiveDirectoryDiscovery
		{
			get { return _useActiveDirectoryDiscovery; }
			set { _useActiveDirectoryDiscovery = value; _useNetBiosDiscovery = !value; }
		}

		/// <summary>Are we to perform a NetBIOS discovery</summary>
		public bool UseNetBiosDiscovery
		{
			get { return _useNetBiosDiscovery; }
			set { _useNetBiosDiscovery = value; _useActiveDirectoryDiscovery = !value; }
		}

		/// <summary>Custom string specified for Active Directory discovery</summary>
		public String CustomActiveDirectoryString
		{
			get { return _customActiveDirectoryString; }
			set { _customActiveDirectoryString = value; }
		}


		/// <summary>Custom string specified for NetBIOS discovery</summary>
		public String CustomNetBiosString
		{
			get { return _customNetBiosString; }
			set { _customNetBiosString = value; }
		}

        /// <summary>Interval in seconds between successive discovery attempts</summary>
        public int RediscoverInterval
        {
            get { return _rediscoverInterval; }
            set { _rediscoverInterval = value; }
        }

        /// <summary>Maximum number of threads used by the scanner</summary>
        public int ScanThreads
        {
            get { return _scanThreads; }
            set { _scanThreads = value; }
        }

		public bool ScanNetwork
		{
			get { return _scanNetwork; }
			set { _scanNetwork = value; }
		}

		public String DomainUser
		{
			get { return _domainUser; }
			set { _domainUser = value; }
		}

		public String DomainPassword
		{
			get { return (_domainPassword == null || _domainPassword == "") ? "" : AES.Decrypt(_domainPassword); }
			set 
			{
				if (value == "")
					_domainPassword = "";
				else
					_domainPassword = AES.Encrypt(value); 
			}
		}

		public String DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		public String DatabaseServer
		{
			get { return _databaseServer; }
			set { _databaseServer = value; }
		}

		public bool TrustedConnection
		{
			get { return _trustedConnection; }
			set { _trustedConnection = value; }
		}

		/// <summary>Interval in days between successive audits</summary>
		public int ReauditInterval
		{
			get { return _reauditInterval; }
			set { _reauditInterval = value; }
		}

		/// <summary>Interval in minutes between successive scans of the computer list</summary>
		public int RecheckInterval
		{
			get { return _recheckInterval; }
			set { _recheckInterval = value; }
		}

		public bool VistaTrees
		{
			get { return _vistaTrees; }
			set { _vistaTrees = value; }
		}

		public bool ShowByDomain
		{
			get { return _showByDomain; }
			set { _showByDomain = value; }
		}

#endregion Data Accessors

		public AuditWizardConfiguration() : base (AuditWizard_INIFILE ,0)
		{
			// Load configuration into our internal structures
			LoadConfiguration();
		}

		public AuditWizardConfiguration(int flags) : base(AuditWizard_INIFILE ,flags)
		{
			// Load configuration into our internal structures
			LoadConfiguration();
		}

		protected void LoadConfiguration()
		{
			// Read the data from the generic IniFile base into our internal data fields
			//
			// User Details Section
			_organization = GetString(SECTION_USERDETAILS, KEY_ORGANIZATION, "");
			_name = GetString(SECTION_USERDETAILS, KEY_USERNAME, "");
			_email = GetString(SECTION_USERDETAILS, KEY_EMAIL, "");
			_notifyOnComplete = GetBoolean(SECTION_USERDETAILS, KEY_NOTIFYUSER, true);
			
			// Database Section
			_databaseName = GetString(SECTION_DATABASE, KEY_DBNAME, "AuditWizard");
			_databaseServer = GetString(SECTION_DATABASE, KEY_DBSERVER, "(local)");
			_trustedConnection = GetBoolean(SECTION_DATABASE, KEY_TRUSTEDCONNECTION, true);

			// Network Discovery Section
			_useActiveDirectoryDiscovery = GetBoolean(SECTION_DISCOVERY, KEY_USEACTIVEDIRECTORY, true);
			_useNetBiosDiscovery = GetBoolean(SECTION_DISCOVERY, KEY_USENETBIOS, false);
			_customActiveDirectoryString = GetString(SECTION_DISCOVERY, KEY_CUSTOMADSTRING, "");
			_customNetBiosString = GetString(SECTION_DISCOVERY, KEY_CUSTOMNBSTRING, "");
			_rediscoverInterval = GetInteger(SECTION_DISCOVERY, KEY_REDISCOVER_INTERVAL, (60 * 24));

			// Scan Section
			_scanNetwork = GetBoolean(SECTION_SCANNER, KEY_SCANNETWORK, true);
			_domainPassword = GetString(SECTION_SCANNER, KEY_DOMAINPASSWORD, "");
            _scanThreads = GetInteger(SECTION_SCANNER, KEY_MAXIMUMTHREADS, 25);
			_domainUser = GetString(SECTION_SCANNER, KEY_DOMAINUSER, "");
			_reauditInterval = GetInteger(SECTION_SCANNER, KEY_REAUDIT_INTERVAL, 7);
			_recheckInterval = GetInteger(SECTION_SCANNER, KEY_RECHECK_INTERVAL, 5);

			// UI Section
			_vistaTrees = GetBoolean(SECTION_UI, KEY_VISTATREES, false);
			_showByDomain = GetBoolean(SECTION_UI, KEY_DOMAINVIEW, true);
		}


		/// <summary>
		/// Save any configuration changes back to disk
		/// </summary>
		public void SaveConfiguration()
		{
			// Save data back into the underlying IniFile object en-mass
			//
			SetString(SECTION_USERDETAILS, KEY_ORGANIZATION, _organization, false);
			SetString(SECTION_USERDETAILS, KEY_USERNAME, _name, false);
			SetString(SECTION_USERDETAILS, KEY_EMAIL, _email, false);
			SetBoolean(SECTION_USERDETAILS, KEY_NOTIFYUSER, _notifyOnComplete, false);
			
			// Database Section
			SetString(SECTION_DATABASE, KEY_DBNAME, _databaseName, false);
			SetString(SECTION_DATABASE, KEY_DBSERVER, _databaseServer, false);
			SetBoolean(SECTION_DATABASE, KEY_TRUSTEDCONNECTION, _trustedConnection, false);

			// Network Discovery Section
			SetBoolean(SECTION_DISCOVERY, KEY_USEACTIVEDIRECTORY, _useActiveDirectoryDiscovery, false);
			SetBoolean(SECTION_DISCOVERY, KEY_USENETBIOS, _useNetBiosDiscovery, false);
			SetString(SECTION_DISCOVERY, KEY_CUSTOMADSTRING, _customActiveDirectoryString, false);
			SetString(SECTION_DISCOVERY, KEY_CUSTOMNBSTRING, _customNetBiosString, false);
			SetInteger(SECTION_DISCOVERY, KEY_REDISCOVER_INTERVAL, _rediscoverInterval, false);

			// Scan Section
			SetBoolean(SECTION_SCANNER, KEY_SCANNETWORK, _scanNetwork, false);
			SetString(SECTION_SCANNER, KEY_DOMAINPASSWORD, _domainPassword, false);
            SetInteger(SECTION_SCANNER, KEY_MAXIMUMTHREADS, _scanThreads, false);
			SetString(SECTION_SCANNER, KEY_DOMAINUSER, _domainUser, false);
			SetInteger(SECTION_SCANNER, KEY_REAUDIT_INTERVAL, _reauditInterval, false);
			SetInteger(SECTION_SCANNER, KEY_RECHECK_INTERVAL, _recheckInterval, false);

			// UI Section
			SetBoolean(SECTION_UI, KEY_VISTATREES, _vistaTrees, false);
			SetBoolean(SECTION_UI, KEY_DOMAINVIEW, _showByDomain ,false);

			// ...and force a write back to disk
			Write(true);
		}
	}
}
