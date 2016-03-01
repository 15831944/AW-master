#pragma once

//
//    AlertCacheFile
//    ==============
//
//    This class defines the layout of an Alert Cache File.  
//	  This file hold sthe current value of any fields being monitored by AlertMonitor 
//
//    It is written to by the various scanners and read internally, storing the values read
//    into local data
#define S_ALERTCACHE_FILE	"AlertCacheFile"


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
			case (int)software: return "sofwtare";
			default:			return "unknown";
		}
	}

	// Category for this alert - ie hardware category
	CString& Category()
	{ return _category; }
	void	Category (CString& value)
	{ _category = value; }
	
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

// Writer helper functions
protected:	
	void WriteAlert(CAlertNotification* pNotification);


private:
	CString	_assetName;
	CDynaList<CAlertNotification*>	_alertNotifications;
	
	// The Markup File Writer
	CMarkup*	m_pWriter;
};
