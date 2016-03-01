#pragma once

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols
#include "ShtScan.h"		// the main scanner property sheet (wizard)
#include "ScannerTrayWnd.h"
#include "../AuditScannerCommon/AssetCacheFile.h"
#include "../AuditScannerCommon/AuditScannerConfiguration.h"
#include "../AuditScannerCommon/WmiScanner.h"

// USEFUL MACROS
#define GetAPP()			((CScan32App*)AfxGetApp())
#define GetCFG()			((GetAPP()->_scannerConfiguration))
#define GetCACHEFILE()		(GetAPP()->_pAssetCacheFile)

/////////////////////////////////////////////////////////////////////////////
// CScan32App:
// -----------
// The main application class

class CScan32App : public CWinApp
{
public:
	// Enum for enabled screens/field
	enum eScreens	{ BasicInfo=1, Location=2, AssetData=4 };
	enum eFields	{ AssetName=0x01, Make=0x02, Model=0x04, Category=0x08 ,SerialNumber=0x10, Cancel=0x20, AddLocation=0x40};

public:
	CScan32App();
	~CScan32App();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CScan32App)
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

	// Data Accessors
	// Date of last audit
	CTime		DateOfLastAudit ()
	{ return _dateOfLastAudit; }
	void		DateOfLastAudit	(CTime value)
	{ _dateOfLastAudit = value; }

	// The time of the current audit
	CTime		DateOfThisAudit ()
	{ return _dateOfThisAudit; }

	// return current asset name
	//CString & GetAssetName()
	//{ return _pAssetCacheFile->GetAssetName(); }
	//void SetAssetName (LPCSTR pszNewName)
	//{ _pAssetCacheFile->SetAssetName(pszNewName); }
	CString & GetAssetName()
	{ return m_strAssetName; }
	void SetAssetName (LPCSTR pszNewName)
	{ m_strAssetName = pszNewName; }

	// TRUE if doing a diagnostic trace of the scanner operation
	BOOL		TraceMode ()
	{ return _traceMode; }
	void		TraceMode (BOOL value)
	{ _traceMode = value; }
	
	// TRUE if doing a test-scan (ie don't run alert monitor features)
	BOOL		TestMode ()
	{ return _testMode;	}
	void		TestMode (BOOL value)
	{ _testMode = value; }

	// The Path to the scanner folder
	CString		ScannerPath ()
	{ return _scannerPath; }

	// The path to the Windows temp folder
	CString		TempPath ()
	{ return _tempPath; }

	// The mode in which the audit scanner is running
	CAuditScannerConfiguration::eAuditMode ScannerMode ()
	{ return _scannerMode; }

	// The Scanner Configuration File
	CAuditScannerConfiguration	_scannerConfiguration;
	CAssetCacheFile::eCacheFileScope _assetCacheFileScope;	
										// Cache file scope (local, central, none etc)

	// The WMI Scanner used by various hardware scanners
	CWMIScanner*	GetWMIScanner()
	{ return m_pWmiScanner; }


	CLogFile*		m_pLogFile;			// For logging Application status / faults etc
	//CAssetCacheFile* _pAssetCacheFile;	// The local audit cache file (may not be used)
	CTime			m_timCfgFile;		// datestamp from last time config was read
	CShtScan *		m_pWizard;			// The main wizard Property Sheet
	CMapiSession	m_mapi;				// MAPI mail connection
	//
	CString			m_strAssetName;
	CString			m_strUniqueID;		// A unique system identifier
	CString			m_strDomain;		// The domain in which this computer is located
	CString			m_strCat;			// collected Asset Category
	CString			m_strMake;			// collected Asset Make
	CString			m_strModel;			// collected Asset Model
	CString			m_strSerial;		// collected Asset Serial Number
	CString			m_strMacAddr;
	CString			m_strIPAddr;
	CString			m_strLoc;			// collected Asset Location
	CString			m_strAssetTag;
	int				m_iChassisType;		// for additional check than battery
	CString			m_strLastAuditDate; // for storing the audit date and save after a successfull audit

	// FTP Information
	CInternetSession*	m_pInetSession;
	CFtpConnection*	m_pFTPConnection;
	 
	// WMI and Registry scanners used during the scan process
	CWMIScanner*	m_pWmiScanner;
	
// Implementation
public:

	// Translate a path to a long path
	void	GetLongPathName (LPCSTR lpPath ,LPSTR lpLongPath ,DWORD dwBufSize);

	// Validate the FTP Server details
	CFtpConnection* ValidateFTP	(void);
	CFtpConnection* ValidateFTPBackup (void);

	CString DecryptFTPPassword(LPCSTR pszCipherText);

	// Is this a scannable asset - for now return true always
	bool	IsAssetScannable ()
	{ return true; }

protected:
	// read and store any command line parameters
	void ParseCmdLine ();
	
	// check whether configuration file has been updated
	BOOL CheckConfigFile ();
	
	// read settings from the scanner initialization file
	BOOL ReadAuditConfiguration();

	// check the specified data path exists
	BOOL ValidateDataPath();
	
	// find and open local station file
	BOOL ReadAssetCacheFile();
	
	// create the property sheet pages
	BOOL BuildWizardPages();
	BOOL AddUserDataPages();
	
	// Check for a previously active scanner
	void CheckActive (void);
	BOOL CheckCloseDownRequested(void);

	BOOL ProcessConfigRegKeys(void);
	BOOL SetLastAuditDate(void);
	CTime GetLastAuditDate(void);
	
	// Open the log file
	void OpenLogFile (CLogFile & log);
	
	// Audit this PC now
	BOOL AuditNow (CLogFile & log);

	// The main processing loop
	BOOL Process	(bool firstTime);

	// Update and save the asset cache file
	void UpdateAssetCacheFile ();

	// Audit Helper functions
	void AuditUserDefinedData		(void);
	void AuditAssetData				(void);

	// PDA/USB installers
	void InstallMDA (BOOL bUninstall=FALSE);
	void InstallUSB (BOOL bUninstall=FALSE);

	// Create a system tray icon for the scanner
	void CheckSystemTray			(void);
	BOOL CheckAlertMonitorTriggers	(void);
	
public:
	// read a value from the registry
	static CString GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem);

protected:

	//{{AFX_MSG(CScan32App)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	LRESULT OnTrayNotification(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()

private:
	BOOL			_pAssetCacheFileRead;
	
	// Name of the ini file to use
	CString			_scannerConfigurationName;

	// The mode in which the scanner will run (see enumeration above)
	CAuditScannerConfiguration::eAuditMode _scannerMode;		

	// Date/Time of the last audit of this asset (read from the cache file)
	CTime			_dateOfLastAudit;	

	// The time of the current audit
	CTime			_dateOfThisAudit;

	// TRUE if doing a diagnostic trace of the scanner operation
	BOOL			_traceMode;		
	
	// TRUE if doing a test-scan (ie don't run alert monitor features)
	BOOL			_testMode;		

	// path to scanner executable for config file etc.
	CString			_scannerPath;	

	// Windows Temp folder in long format
	CString			_tempPath;	

	// Flag to show that we have or haven't reported that the re-audit interval has expired
	BOOL			_reportAuditNotRequired;

	BOOL			_firstTimeAudit;

	// Time at which the next Alert Monitor scanning should be performed
	CTime			_timeNextTriggerCheck;

	// The system tray icon
	CScannerTrayWnd*	_pTrayIconWnd;
	
};