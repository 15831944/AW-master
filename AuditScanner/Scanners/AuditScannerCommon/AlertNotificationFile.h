#pragma once

//
//    AlertNotificationFile
//    =====================
//
//    This class defines the layout of an Alert Notification File.  This file acts as the interface between
//	  the AuditWizard Scanner and the AuditWizard Service and User Interface and is in XML Format
//
//	  The file notifies the UI of any alerts which have been triggered and provides details of that trigger
//	  The file has the following layout
//
//		<AlertNotificationFile>
//			<AssetName>DEV3</AssetName>
//			<Alert>
//				<Name>Memory Alert</Name>
//				<Type>Hardware</Type>
//				<Category>Hardware|Memory|Installed Memory</Category>
//				<OldValue>1024</OldValue>
//				<NewValue>2048</NewValue>
//			</Alert>
//			<Alert>
//				<Name>CPU Alert</Name>
//				<Type>Hardware</Type>
//				<Category>Hardware|CPU|Speed</Category>
//				<OldValue>2000</OldValue>
//				<NewValue>2200</NewValue>
//			</Alert>
//		</AlertNotificationFile>
//

// #defines for the different sections with the AlertNotificationFile
#define S_ALERTNOTIFICATION_FILE	"AlertNotificationFile"
//
#define V_ASSET_NAME				"AssetName"				// Name of the Asset for which notifications generated
//
#define S_ALERT						"Alert"					// Alert Section
//
#define V_ALERT_NAME				"Name"					// Name of the Alert being notified
#define V_ALERT_TYPE				"Type"					// Type of alert being notified
#define V_ALERT_CATEGORY			"Category"				// Category of field being notified
#define V_ALERT_KEY					"Key"					// Key of field being notified
#define V_ALERT_OLDVALUE			"OldValue"				// The previous value of the field
#define V_ALERT_NEWVALUE			"NewValue"				// The New value of the field


//
//    CAlertNotification
//    ==================
//
//    This class defines an instance of an alert notification
//
class CAlertNotification
{
public:
	// Conditions which we support
	enum eNotificationType { unknown, hardware, file, software, internet };

public:
	CAlertNotification(void)
	{}

	CAlertNotification(LPCSTR alertName ,eNotificationType type ,LPCSTR category ,LPCSTR key ,LPCSTR oldValue ,LPCSTR newValue)
	{ _alertName = alertName; _type = type; _category = category; _key = key; _oldValue = oldValue; _newValue = newValue; }

// Methods
public:

// Properties
public:
	// Alert Name being notified
	CString	AlertName()
	{ return _alertName; }
	void AlertName(CString value)
	{ _alertName = value; }

	// Condition which must be matched for the trigger to fire
	eNotificationType	Type()
	{ return _type; }
	void	Type (eNotificationType value)
	{ _type = value; }
	
	
	LPCSTR	NotificationTypeAsString()
	{
		switch ((int)_type)
		{
			case (int)unknown:	return "unknown";
			case (int)hardware: return "hardware";
			case (int)file:		return "file";
			case (int)internet: return "internet";
			case (int)software: return "software";
			default:			return "unknown";
		}
	}
	
	
	void Type(LPCSTR type)
	{
		if (type == "hardware")
			_type = hardware;
		else if (type == "file")
			_type = file;
		else if (type == "internet")
			_type = internet;
		else if (type == "software")
			_type = software;
		else
			_type = unknown;
	}

	// Category for this alert - ie hardware category
	CString& Category()
	{ return _category; }
	void	Category (CString& value)
	{ _category = value; }

	// Key for this alert - ie hardware field
	CString& Key()
	{ return _key; }
	void	Key (CString& value)
	{ _key = value; }
	
	// Old value for the triggering field
	CString& OldValue()
	{ return _oldValue; }
	void	OldValue (CString& value)
	{ _oldValue = value; }
	
	// Category for this alert - ie hardware category
	CString& NewValue()
	{ return _newValue; }
	void	NewValue (CString& value)
	{ _newValue = value; }


private:
	CString				_alertName;
	eNotificationType	_type;	
	CString				_category;
	CString				_key;
	CString				_oldValue;
	CString				_newValue;
};


 



//
//    CAlertNotificationFile
//    ======================
//
//    This class encapsulates an Alert Notification File 
//    This acts as the interface between the AuditWizard UI and the AuditWizard Scanner with regards
//    to the notification of alerts which have triggered back to the AuditWizard UI.
//
class CAlertNotificationFile
{
public:
	CAlertNotificationFile(void);
	~CAlertNotificationFile(void);

// Properties
public:

	// The asset for which this file has been created
	CString& AssetName()
	{ return _assetName; }
	void	AssetName(CString& value)
	{ _assetName = value; }

	// List of Alert Notifications to send
	CDynaList<CAlertNotification*>&	AlertNotifications()
	{ return _alertNotifications; }

// Public Methods
public:
	// Write the alert notifications to the specified file
	int Write (CString& fileName);

	// Load the contents of any existing file
	int	 Load	(LPCSTR pszPath);

// Writer helper functions
protected:	
	void WriteAlert(CAlertNotification* pNotification);
	void ProcessElementRead(CMarkup xmlFile);
	void ProcessAlertNotification(CMarkup xmlFile);


private:
	CString	_assetName;
	CDynaList<CAlertNotification*>	_alertNotifications;
	
	// The Markup File Writer
	CMarkup*	m_pWriter;
};
