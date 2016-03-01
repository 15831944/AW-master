#pragma once
#include "AuditDataScanner.h"
#include "AlertNotificationFile.h"
#include "AlertDefinitionsFile.h"
#include "SoftwareScanner.h"
#include "InternetExplorerScanner.h"
#include "RegistryScanner.h"
#include "OperatingSystemScanner.h"

//
//    CAlertTriggerScanner
//    ====================
//
//    this is the base class used during the checking of an AlertMonitor trigger.
//
class CAlertTriggerScanner
{
public:
	enum { opChanged, opLessThan, opFolder, opNull, opContains, opGreaterThan, opEquals, opLessEqual, opGreaterEqual };
public:
	// constructor
	CAlertTriggerScanner(LPCSTR pszAlertName, int condition, LPCSTR pszTestValue)
	{
		_alertName = pszAlertName;
		_condition = condition;
		_testValue = pszTestValue;
	}
	
	// destructor
	virtual ~CAlertTriggerScanner()
	{
	}
		
	// collect data
	virtual BOOL Scan () = 0;
	
	// see if trigger event has happened, and generate event log if so
	virtual BOOL Test (CAlertNotificationFile& file ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile) = 0;
	
	// serialize any collected data
	virtual void Save (CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile) = 0;

	// Create a specialized Alert Trigger Scanner
	CAuditDataScanner* CreateAlertScanner (LPCSTR pszCatName);

protected:
	CString			_alertName;				// Name of the parent alert definition
	int				_condition;				// how to check for trigger firing
	CString			_testValue;				// test value defined by manager
};




//
//    CAlertTriggerScannerList
//    ========================
//
//    This class encompasses a collection of variant audit trigger scanners
//
class CAlertTriggerScannerList : public CDynaList<CAlertTriggerScanner *>
{
public:
	// construction
	CAlertTriggerScannerList()
		{}
	virtual ~CAlertTriggerScannerList();

	// build list of scanners (returns number of scanner objects set up)
	int Setup (CAuditScannerConfiguration* pScannerConfiguration ,CWMIScanner* pWMIScanner ,CAlertDefinitionsFile& definitionsFile, LPCSTR pszAsset, CTime& tmLastAudit);

	// serialise scan results
	void Save (CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

	// collect scan results
	BOOL Scan();

	// test for any alert trigger fired (returns number of triggers)
	int Test (CAlertNotificationFile& file ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	
	// the scanner configuration file
	CAuditScannerConfiguration* _pScannerConfiguration;
};


//
//    CAlertTriggerScanner_HW
//    =======================
//    
//    This class is used when we have an alert trigget watching a hardware value
//
class CAlertTriggerScanner_HW : public CAlertTriggerScanner
{
public:
	CAlertTriggerScanner_HW (LPCSTR pszAlertName, int condition, LPCSTR pszTestValue, LPCSTR pszCategory);
	virtual ~CAlertTriggerScanner_HW();
	virtual BOOL Scan();
	virtual BOOL Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	virtual void Save(CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

public:
	CString&	Category()
	{ return _category; }
	void	Category (CString& value)
	{ _category = value; }
	//
	CString&	Key()
	{ return _key; }
	void	Key (CString& value)
	{ _key = value; }
	//	
	void SetWMIScanner(CWMIScanner* pWMIScanner)
	{ _pWMIScanner = pWMIScanner; }

protected:
	BOOL TestValue (CAlertNotificationFile& notificationFile ,CAuditDataFileItem* pOldItem ,CAuditDataFileItem* pNewItem);
		
protected:
	CString		_category;
	CString		_key;
	CAuditDataScanner* _pAuditDataScanner;
	CWMIScanner*	_pWMIScanner;			// The WMI scanner needed by most of the hardware scanning classes
};






//
//
//    CAlertTriggerScanner_IE
//    =======================
//    
//    This class is used when we have an alert trigger watching acess to a specific Internet URL
//
class CAlertTriggerScanner_IE : public CAlertTriggerScanner
{
public:
	CAlertTriggerScanner_IE (LPCSTR pszAlertName , LPCSTR pszTestVal ,CAuditScannerConfiguration* pScannerConfiguration);
	virtual ~CAlertTriggerScanner_IE();
	virtual BOOL Scan();
	virtual BOOL Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	virtual void Save(CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

protected:
	CAuditDataFileCategory* FindInternetCategory(CDynaList<CAuditDataFileCategory>* pListCatagories, CString name) const;
	CTime ConvertTimeString(CString& strTime);
	CTime ConvertTimeString2(CString& strTime);

protected:
	// the software scanner
	CInternetExplorerScanner*	_pInternetExplorerScanner;
	CInternetHistoryList _listHistoryMatches;

	CAlertNotification* _pAlertNotificationHistory;
	CAlertNotification* _pAlertNotificationCookie;
};




//
////
////    CAlertTriggerScanner_OS
////    ======================
////    
////    This class is used when we have an alert trigger watching acess to a specific Internet URL
////
//class CAlertTriggerScanner_OS : public CAlertTriggerScanner
//{
//public:
//	CAlertTriggerScanner_OS (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszKey);
//	virtual ~CAlertTriggerScanner_OS()
//		{}
//protected:
//};




//
//
//    CAlertTriggerScanner_SW
//    =======================
//
//    This class is used when we have an alert trigger monitoring software installs or uninstalls
//
class CAlertTriggerScanner_SW : public CAlertTriggerScanner
{
public:
	CAlertTriggerScanner_SW (LPCSTR pszAlertName ,CAuditScannerConfiguration* pScannerConfiguration);
	virtual ~CAlertTriggerScanner_SW();
	virtual BOOL Scan();
	virtual BOOL Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	virtual void Save(CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

protected:
	BOOL FindApplication (CString& name ,CDynaList<CApplicationInstance>* pInstances);
	
protected:
	// the scanner configuration file
	CAuditScannerConfiguration* _pScannerConfiguration;
	
	// the software scanner
	CSoftwareScanner*	_pSoftwareScanner;
};

class CAlertTriggerScanner_Registry : public CAlertTriggerScanner
{
public:
	//CAlertTriggerScanner_Registry (LPCSTR pszAlertName ,CAuditScannerConfiguration* pScannerConfiguration);
	CAlertTriggerScanner_Registry (LPCSTR pszAlertName, int condition, LPCSTR pszTestValue, LPCSTR pszCategory);
	virtual ~CAlertTriggerScanner_Registry();
	virtual BOOL Scan();
	virtual BOOL Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	virtual void Save(CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

public:
	CString&	Category()
	{ return _category; }
	void	Category (CString& value)
	{ _category = value; }
	//
	CString&	Key()
	{ return _key; }
	void	Key (CString& value)
	{ _key = value; }

protected:
	// the scanner configuration file
	CAuditScannerConfiguration* _pScannerConfiguration;
	
	// the software scanner
	CRegistryScanner*	_pRegistryScanner;

	CString		_category;
	CString		_key;
};

class CAlertTriggerScanner_OS : public CAlertTriggerScanner
{
public:
	CAlertTriggerScanner_OS (LPCSTR pszAlertName, int condition, LPCSTR pszTestValue, LPCSTR pszCategory);
	virtual ~CAlertTriggerScanner_OS();
	virtual BOOL Scan();
	virtual BOOL Test (CAlertNotificationFile& notificationFile ,CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);
	virtual void Save(CAuditDataFile& oldDataFile ,CAuditDataFile& newDataFile);

public:
	CString&	Category()
	{ return _category; }
	void	Category (CString& value)
	{ _category = value; }
	//
	CString&	Key()
	{ return _key; }
	void	Key (CString& value)
	{ _key = value; }

protected:
	// the scanner configuration file
	CAuditScannerConfiguration* _pScannerConfiguration;
	
	// the software scanner
	COperatingSystemScanner*	_pOperatingSystemScanner;

	CString		_category;
	CString		_key;
};

//
//  8.3.4 - CMD
// Used to hold an Internet Site
class CInternetSite
{
public:
	CString _siteName;
	int	_count;
};

