using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	#region AlertTrigger Class

	public class AlertTrigger
	{
		#region Data

		/// <summary>
		/// This enumeration holds the condition associated with the alert trigger
		/// </summary>
		public enum eCondition { changed ,contains ,lessthan ,greaterthan, equals ,lessequals, greaterequals, installed, notinstalled };

		/// <summary>This is the name of the audited data field to trigger on</summary>
		private string	_triggerField;
		
		/// <summary>This is the condition which must be satisfied before the alert will trigger</summary>
		private eCondition _condition;
		
		/// <summary>This is the value that the trigger should meet before an alert is generated</summary>
		private string		_value;
		
		#endregion Data

		#region Properties

		/// <summary>Audited Data field on which to trigger</summary>
		public string TriggerField
		{
			get { return _triggerField; }
			set { _triggerField = value; }
		}

		/// <summary>Condition to meet before the trigget is fired</summary>
		public eCondition Condition
		{
			get { return _condition; }
			set { _condition = value; }
		}

		public string ConditionAsText
		{
			get
			{
				switch (_condition)
				{
					case eCondition.changed:		return "Changed";
					case eCondition.contains:		return "Contains";
					case eCondition.equals:			return "Equals";
					case eCondition.greaterequals:	return "Greater or Equals";
					case eCondition.greaterthan:	return "Greater Than";
					case eCondition.lessequals:		return "Less or Equals";
					case eCondition.lessthan:		return "Less Than";
                    case eCondition.installed:      return "Installed";
                    case eCondition.notinstalled:   return "Not Installed";
					default:						return "<invalid>";
				}
			}
		}
		
		/// <summary>Value to meet depending on the trigger value</summary>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#endregion Properties

		#region Constructor

		public AlertTrigger()
		{
			_triggerField = "";
			_condition = eCondition.changed;
			_value = "";
		}

		#endregion Constructor

		#region Methods
		#endregion Methods
	}

    public class ComplianceField
    {
        #region Data

        /// <summary>
        /// This enumeration holds the condition associated with the alert trigger
        /// </summary>
        public enum eCondition { changed, contains, lessthan, greaterthan, equals, lessequals, greaterequals, installed, notinstalled };

        /// <summary>This is the name of the audited data field to trigger on</summary>
        private string _triggerField;

        /// <summary>This is the condition which must be satisfied before the alert will trigger</summary>
        private eCondition _condition;

        /// <summary>This is the value that the trigger should meet before an alert is generated</summary>
        private string _value;

        /// <summary>This is the boolean value used to link the conditions</summary>
        private string _booleanValue;

        #endregion Data

        #region Properties

        /// <summary>Audited Data field on which to trigger</summary>
        public string TriggerField
        {
            get { return _triggerField; }
            set { _triggerField = value; }
        }

        /// <summary>Condition to meet before the trigget is fired</summary>
        public eCondition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public string ConditionAsText
        {
            get
            {
                switch (_condition)
                {
                    case eCondition.changed: return "Changed";
                    case eCondition.contains: return "Contains";
                    case eCondition.equals: return "Equals";
                    case eCondition.greaterequals: return "Greater or Equals";
                    case eCondition.greaterthan: return "Greater Than";
                    case eCondition.lessequals: return "Less or Equals";
                    case eCondition.lessthan: return "Less Than";
                    case eCondition.installed: return "Installed";
                    case eCondition.notinstalled: return "Not Installed";
                    default: return "<invalid>";
                }
            }
        }

        /// <summary>Value to meet depending on the trigger value</summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string BooleanValue
        {
            get { return _booleanValue; }
            set { _booleanValue = value; }
        }

        #endregion Properties

        #region Constructor

        public ComplianceField()
        {
            _triggerField = "";
            _condition = eCondition.changed;
            _value = "";
            _booleanValue = "";
        }

        #endregion Constructor
    }

	#endregion AlertTrigger Class
	
	
	#region AlertDefinition class
	
	/// <summary>
	/// This class defines a single Alert Definition which consists of 0 or more alert triggers
	/// </summary>
	public class AlertDefinition
	{
		#region Data
			
		/// <summary>Name assigned to this alert definition</summary>
        private string _name;

        /// <summary> Alert defintion description </summary>
        private string _description;
		
		/// <summary>Set if we are to display the alert details in the UI when started</summary>
		private bool	_displayAlert;
		
		/// <summary>Set if we are to email the user on an alert</summary>
		private bool	_emailAlert;
		
		/// <summary>The list of alert triggers</summary>
        private List<AlertTrigger> _alertTriggers = new List<AlertTrigger>();

        /// <summary>The list of computers to monitor</summary>
        private List<string> _listMonitoredComputers = new List<string>();

		#endregion Data
	
		#region Properties
	
		/// <summary>Name of the Alert Definition</summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

        /// <summary>Alert defintion description</summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>Set if we are to display the alert details in the UI when started</summary>
		public bool DisplayAlert
		{
			get { return _displayAlert; }
			set { _displayAlert = value; }
		}

		/// <summary>Set if we are to email the user on an alert</summary>
		public bool EmailAlert
		{
			get { return _emailAlert; }
			set { _emailAlert = value; }
		}
	
		public List<AlertTrigger> Triggers
		{
			get { return _alertTriggers; }
		}

        public List<string> MonitoredComputers
        {
            get { return _listMonitoredComputers; }
            set { _listMonitoredComputers = value; }
        }

		#endregion Properties
			
		#region Constructor
		
		public AlertDefinition ()
		{
			//_name = "<New Alert>";
			_displayAlert = false;
			_emailAlert = false;
		}
		
		public AlertDefinition (string name ,bool displayAlert ,bool emailAlerts)
		{
			_name = name;
			_displayAlert = displayAlert;
			_emailAlert = emailAlerts;
		}

		#endregion Constructor

		#region Methods

		public override string ToString()
		{
			return _name;
		}
		#endregion Methods
	}
	
	#endregion AlertDefinition Class


	#region AlertMonitorSettings Class

	/// <summary>
	/// This class holds the settings and any alerts defined for AlertMonitor
	/// </summary>
	public class AlertMonitorSettings 
	{
	
		#region Data
		public static string AlertMonitorDefinitionsFile = "AlertMonitor.xml";
		//
		private bool	_enabled;
		private int		_settingsCheckInterval;
		private int		_rescanInterval;
		private bool	_showSystemTray;
		private	List<AlertDefinition>	_listAlertDefinitions = new List<AlertDefinition>();
		
		#endregion Data
		
		#region Properties

		/// <summary>Global enable/disabl switch</summary>
		public bool	Enabled
		{ 
			get { return _enabled; } 
			set { _enabled = value; }
		}
		
		/// <summary>Interval in second between checks of the settings file</summary>
		public int SettingsCheckInterval
		{
			get { return _settingsCheckInterval; }
			set { _settingsCheckInterval = value; }
		}
		
		/// <summary>Interval in seconds between checks for alerts</summary>
		public int RescanInterval
		{
			get { return _rescanInterval; }
			set { _rescanInterval = value; }
		}

		/// <summary>Flag to indicate show system tray icon when AlertMonitor running</summary>
		public bool ShowSystemTray
		{
			get { return _showSystemTray; }
			set { _showSystemTray = value; }
		}

		[XmlArray(ElementName = "AlertDefinitions"), XmlArrayItem(ElementName = "AlertDefinition", Type = typeof(AlertDefinition))]
		public List<AlertDefinition> AlertDefinitions
		{
			get { return _listAlertDefinitions; }
		}
		
		#endregion Properties

		#region Constructor
		
		public AlertMonitorSettings()
		{
			_enabled = false;
			_settingsCheckInterval = 30;
			_rescanInterval = 60;
			_showSystemTray = false;
		}
		
		#endregion Constructor
			
		#region Methods
		
		/// <summary>
		/// Find the named alert definition
		/// </summary>
		/// <param name="alertName"></param>
		/// <returns></returns>
		public AlertDefinition FindAlert(string alertName)
		{
			foreach (AlertDefinition definition in _listAlertDefinitions)
			{
				if (definition.Name == alertName)
					return definition;
			}
			return null;
		}
		
		
		/// <summary>
		/// Load the AlertMonitor settings from the database (NOT CURRENTLY USED AS WE SAVE TO XML)
		/// </summary>
		public void LoadSettings()
		{
		}
	
		
		#endregion Methods
		
		#region Serialization Support
		
		public void Save()
		{
			string filePath = Path.Combine(Application.StartupPath, "Scanners");
			filePath = Path.Combine(filePath, AlertMonitorDefinitionsFile);

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(AlertMonitorSettings)); 
				TextWriter textWriter = new StreamWriter(filePath); 
				serializer.Serialize(textWriter, this); 
				
				// We now need to serialize the AlertDefinitions
				textWriter.Close(); 
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to save the Alert Monitor Definitions to '" + filePath + "'.  reason: " + ex.Message, "Save Failed");
			}
			
			// Save enabled/disabled to the database also
			SettingsDAO lwDataAccess = new SettingsDAO();
			lwDataAccess.SetSetting(DatabaseSettings.Setting_AlertMonitorEnable, _enabled.ToString(), false);
		
		}
		
		
		/// <summary>
		/// Called to load the AlertMonitor settings (and alerts) from the saved XML file
		/// </summary>
		/// <returns></returns>
		public static AlertMonitorSettings Load()
		{
			AlertMonitorSettings alertMonitorSettings = null;
			
			string filePath = Path.Combine(Application.StartupPath, "Scanners");
			filePath = Path.Combine(filePath, AlertMonitorDefinitionsFile);
			try
			{
				if (File.Exists(filePath))
				{
					XmlSerializer deserializer = new XmlSerializer(typeof(AlertMonitorSettings));
					TextReader textReader = new StreamReader(filePath);
					alertMonitorSettings = (AlertMonitorSettings)deserializer.Deserialize(textReader);
					textReader.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to read the Alert Monitor Definitions from '" + filePath + "'.  reason: " + ex.Message, "Save Failed");
			}
			
			// If we failed to read the settings for some reason create a default object
			if (alertMonitorSettings == null)
				alertMonitorSettings = new AlertMonitorSettings();
				
			return alertMonitorSettings;
		}
		
		#endregion
	}

	#endregion AlertMonitorSettings Class

}
