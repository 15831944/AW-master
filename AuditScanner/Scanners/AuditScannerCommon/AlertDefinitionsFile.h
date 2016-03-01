#pragma once


// #defines for the different sections with the AlertDefinitionsFile
#define S_ALERTDEFINITIONS_FILE		"AlertDefinitionsFile"
//
#define S_SETTINGS					"AlertMonitorSettings"
#define V_SETTINGS_ENABLED			"Enabled"					// Alert Monitor enabled/disabled flag
#define V_SETTINGS_CHECKINTERVAL	"SettingsCheckInterval"		// Settings check interval
#define V_SETTINGS_RESCANINTERVAL	"RescanInterval"			// Rescan interval
#define V_SETTINGS_SHOWTRAY			"ShowSystemTray"			// Flag to indicate system tray icon required 

// The 'Alert Definitions' section
#define S_ALERT_DEFINITIONS			"AlertDefinitions"			// Alert Definitions Section
#define S_ALERT_DEFINITION			"AlertDefinition"			// Individual Alert Definition item
#define V_ALERT_NAME				"Name"						// Name of the Alert Definition
#define V_ALERT_DISPLAY				"DisplayAlert"				// ** ignored as not required in scanner ***
#define V_ALERT_EMAIL				"EmailAlert"				// ** ignored as not required in scanner ***

// The 'Alert Triggers' section
#define S_ALERT_TRIGGERS			"Triggers"					// Alert Triggers Section
#define S_ALERT_TRIGGER				"AlertTrigger"				// Individual Alert Trigger Section
#define V_TRIGGER_FIELD				"TriggerField"				// Name of the field for this trigger
#define V_TRIGGER_CONDITION			"Condition"					// Condition that must be met for this trigger
#define V_TRIGGER_VALUE				"Value"						// Value to be matched depending on the condition
#define V_MONITORED_COMPUTERS		"MonitoredComputers"



//
//    CAlertTrigger
//    =============
//
//    This class defines an instance of an alert trigger
//
class CAlertTrigger
{
public:
	// Conditions which we support
	enum eTriggerCondition { opChanged, opLessThan, opFolder, opNull, opContains };

public:
	CAlertTrigger(void)
	{}

// Properties
public:
	// Field for this trigger
	CString	Field()
	{ return _field; }
	void Field(CString value)
	{ _field = value; }

	// Condition which must be matched for the trigger to fire
	eTriggerCondition	Condition()
	{ return _condition; }
	void	Condition (eTriggerCondition value)
	{ _condition = value; }
	
	// Value that must be matched (depending on the condition)
	CString& Value()
	{ return _value; }
	void	Value (CString& value)
	{ _value = value; }


private:
	CString		_field;
	CString		_value;	
	eTriggerCondition	_condition;
};





//
//    CAlertDefinition
//    ================
//
//    This class encapsulates 0 or more alert definitions
//
class CAlertDefinition 
{
public:
	CAlertDefinition()
	{}
	
	~CAlertDefinition()
	{
		// remember the contents of our encapsulated array are pointers - they need to be cleaned out
		for (int index=0; index < (int)_listAlertTriggers.GetCount(); index++)
		{
			delete _listAlertTriggers[index];
		}
		_listAlertTriggers.Empty();
	}

public:
	CString&	AlertName()
	{ return _alertName; }
	void AlertName(CString& value)
	{ _alertName = value; }
	
	CDynaList<CAlertTrigger*>& AlertTriggers()
	{ return _listAlertTriggers; }

private:
	CString		_alertName;
	CDynaList<CAlertTrigger*>	_listAlertTriggers;
}; 



//
//    CAlertDefinitionsFile
//    =====================
//
//    This class encapsulates an Alert Definitions File 
//    This acts as the interface between the AuditWizard UI and the AuditWizard Scanner with regards
//    to the AlertMonitor Functionality of AuditWizard.
//
class CAlertDefinitionsFile
{
public:
	CAlertDefinitionsFile(void);
	~CAlertDefinitionsFile(void);
	// Intialization of string map
	static void InitializeStringMap();
// Properties
public:
	// Alert Monitor Enabled Flag
	BOOL	Enabled()
	{ return _enabled; }
	void	Enabled (BOOL value)
	{ _enabled = value; }

	// Show System Tray Icon Flag
	BOOL	ShowSystemTray()
	{ return _showSystemTray; }
	void	ShowSystemTray (BOOL value)
	{ _showSystemTray = value; }

	// Settings check interval (in seconds)
	int	SettingsCheckInterval()
	{ return _settingsInterval; }
	void	SettingsCheckInterval (int value)
	{ _settingsInterval = value; }

	// Rescan Interval (in seconds)
	int	RescanInterval()
	{ return _rescanInterval; }
	void	RescanInterval (int value)
	{ _rescanInterval = value; }

	// List of Alert Triggers Defined
	CDynaList<CAlertDefinition*>&	AlertDefinitions()
	{ return _alertDefinitions; }

// Public Methods
public:
	// Load the file from the specified disk file
	int		Load	(LPCSTR pszPath);

// Internal Helper Functions
protected:
	void	InitializeDefaults();
	void	ProcessElementRead		(CMarkup xmlFile);
	void	ProcessAlertDefinitions	(CMarkup xmlFile);
	void	ProcessAlertDefinition	(CMarkup xmlFile);
	void	ProcessAlertTriggers	(CMarkup xmlFile ,CAlertDefinition* pAlertDefinition);
	void	ProcessAlertTrigger		(CMarkup xmlFile ,CAlertDefinition* pAlertDefinition);

	void	DeserializeScannerConfiguration (CMarkup xmlFile);
	
private:
	BOOL	_enabled;
	BOOL	_showSystemTray;
	int		_rescanInterval;
	int		_settingsInterval;
	
	CDynaList<CAlertDefinition*>	_alertDefinitions;
};
