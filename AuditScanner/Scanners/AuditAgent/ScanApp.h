#pragma once

#include "../AuditScannerCommon/AssetCacheFile.h"
#include "../AuditScannerCommon/AuditScannerConfiguration.h"
#include "../AuditScannerCommon/WmiScanner.h"
#include "../AuditScannerCommon/InstalledPatches.h"
#include "../AuditScannerCommon/SoftwareScanner.h"
#include "../AuditScannerCommon/OperatingSystemScanner.h"
#include "../AuditScannerCommon/HardwareScanner.h"
#include "../AuditScannerCommon/FileSystemScanner.h"
#include "../AuditScannerCommon/RegistryScanner.h"

#include "../AuditScannerCommon/CkSFtp.h"

extern const char* gszErrorMsgTable[];
#define ERR_CFG_OPEN	1

//
//    Base class for Win32 or Service application
//
class CScanApp : public CWinApp
{
	DECLARE_DYNAMIC(CScanApp)

public:
	CScanApp ();
	~CScanApp ();

	// load initial config
	int Initialise ();

	// Perform a full non-interactive scan if due - return TRUE if any data was collected
	BOOL ScanNonInteractive ();

	// Save any collected data
	BOOL SaveData ();

public:
	// Data Accessors
	// Date of last audit
	CTime		DateOfLastAudit ()
	{ return _dateOfLastAudit; }
	void		DateOfLastAudit	(CTime value)
	{ _dateOfLastAudit = value; }

protected:
	void AuditAssetData (void);
	void AuditUserDefinedData (void);
	static CString GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem);

	// 8.3.4 - CMD - Function to (re)create the scanners
	void CreateScanners();

	BOOL ProcessConfigRegKeys(void);

	// connect to a remote FTP site
	CFtpConnection * ConnectFtp ();

	// Refresh the audit agent configuration that we have read
	int RefreshAgentConfiguration();

	// check whether configuration file has been updated
	BOOL CheckAgentConfiguration ();
	
	// read settings from the scanner initialization file
	BOOL ReadAgentConfiguration	();
	
	// find and open local station file
	BOOL ReadAssetCacheFile();

	// Update the asset cache file - note that as the audit is always non-interactive we
	// really can't change must in this file so simply update any computer names and set
	// the date of the last audit
	void UpdateAssetCacheFile ();

	// Send a message to the AuidtWizard server
	void SendMessageToServer (int status ,LPCSTR msg);

	BOOL ScanHardware();
	BOOL ScanSoftware();
	BOOL ScanFileSystem();
	BOOL ScanRegistryKeys();

protected:
	// Validate any data path specified
	BOOL		ValidateDataPath	(void);

	CString DecryptFTPPassword(LPCSTR pszCipherText);

	// Validate any FTP details specified
	CFtpConnection* ValidateFTP		(void);

	// Validate any FTP details specified
	CFtpConnection* ValidateFTPBackup		(void);

	// derived classes must implement this, either by displaying or logging, as appropriate
	virtual BOOL Message			(LPCSTR pszMessage, UINT nFlags) = 0;	 

	// Save the results of an audit to the temp folder initially
	void	SaveResultsToTemp		(void);

	//  Upload any results files
	int		UploadResults			(void);

	// Upload a specific audit data file
	BOOL	UploadFile				(CString& auditDataFile);

	// Upload a specific audit data file to the network data folder
	BOOL	SaveResultsToNetwork	(CString& auditDataFileName);

	// Upload a specific audit data file to the specified FTP site
	BOOL	SaveResultsToFtp		(CString& auditDataFileName);

	// Upload a specific audit data file to the specified FTP site
	BOOL	SaveResultsToFtpBackup		(CString& auditDataFileName);

	// Upload a specific audit data file to the specified SFTP site
	BOOL	SaveResultsToSftp		(CString& auditDataFileName);

	// Upload a specific audit data file to the specified FTP site
	BOOL	SaveResultsToSftpBackup		(CString& auditDataFileName);

	BOOL	CheckFileExists			(CString& outputAuditFileName, CkSFtp& sftp);

	// Upload a specific audit data file to an Email
	BOOL	SaveResultsToEmail		(CString& auditDataFileName);

	// Upload a specific audit data file to the AudiTWizard TCP Server
	BOOL	SaveResultsToTcp		(CString& auditDataFileName);
	
	// Get the text from the last error
	LPSTR	GetLastErrorText( LPSTR lpszBuf, DWORD dwSize );
	 
	// WMI and Registry scanners used during the scan process
	CWMIScanner*		m_pWmiScanner;

	// Log an exception
	void	LogException (CException* pEx, LPCSTR function);

protected:
	CString			m_strExePath;

	// Date/Time of the last audit of this asset (read from the cache file)
	CTime			_dateOfLastAudit;	
	
	// The timestamp of the configuration file read
	CTime			_dateConfigurationFile;

	// The Scanner Configuration File
	CAuditScannerConfiguration	_scannerConfiguration;

	// The AuditWizard Asset Cache (AWC) file
	CAssetCacheFile*	_pAssetCacheFile;
	
	// Log file
	CLogFile			m_log;

	// Internet session object
	CMapiSession		m_mapi;						// MAPI mail connection
	CInternetSession*	m_pInternet;				// Internet connection
	CFtpConnection*		m_pFTPConnection;			// FTP Connection
	CTcpClient			m_tcp;						// TCP Connection

	// The Scanners
	CSoftwareScanner*			_pSoftwareScanner;
	COperatingSystemScanner*	_pOperatingSystem;
	CHardwareScanner*			_pHardwareScanner;
	CFileSystemScanner*			_pFileSystemScanner;
	CRegistryScanner*			_pRegistryScanner;
	CInstalledPatchList			_installedPatches;

	// Identification data
	CString			m_strAssetName;
	CString			m_strDomain;
	CString			m_strUniqueID;
	CString			m_strCat;	
	CString			m_strMake;	
	CString			m_strModel;	
	CString			m_strSerial;
	CString			m_strMacAddr;
	CString			m_strIPAddr;
	CString			m_strAssetTag;
	int				m_iChassisType;

	// Static function to use for TCP callbacks
	static void MsgCallback (CString pszMessage, LPARAM lParam);
};