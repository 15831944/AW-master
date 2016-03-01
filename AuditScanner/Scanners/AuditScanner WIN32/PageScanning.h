#pragma once

#include "resource.h"
#include "../AuditScannerCommon/HardwareScanner.h"
#include "../AuditScannerCommon/InstalledPatches.h"
#include "../AuditScannerCommon/SoftwareScanner.h"
#include "../AuditScannerCommon/OperatingSystemScanner.h"
#include "../AuditScannerCommon/InternetExplorerScanner.h"
#include "../AuditScannerCommon/FileSystemScanner.h"
#include "../AuditScannerCommon/RegistryScanner.h"

/////////////////////////////////////////////////////////////////////////////
// CPageScanning
// -----------
// The Wizard Page for hardware details

class CPageScanning : public CScannerPage
{
	DECLARE_DYNCREATE(CPageScanning)

// Construction
public:
	CPageScanning();
	~CPageScanning();

	// Collected Data
	CHardwareScanner*			_pHardwareScanner;
	CInternetExplorerScanner*	_pInternetExplorerScanner;
	CSoftwareScanner*			_pSoftwareScanner;
	COperatingSystemScanner*	_pOperatingSystem;
	CFileSystemScanner*			_pFileSystemScanner;
	CInstalledPatchList			_installedPatches;	
	CRegistryScanner*			_pRegistryScanner;	

	// Save data
	BOOL Save	 (CAuditDataFile & file);

	// Set the WMI scanner for us to use
	void	WmiScanner	(CWMIScanner* pWmiScanner)
	{ _pWmiScanner = pWmiScanner; }

// Dialog Data
	CTimeSpan	m_tsElapsed;	// time taken to complete hw scan

	//{{AFX_DATA(CPageScanning)
	enum { IDD = IDD_PAGE_SCANNING };
	//}}AFX_DATA

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPageScanning)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnKillActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	BOOL Scan();
	BOOL ScanHardware();
	BOOL ScanSoftware();
	BOOL ScanInternetExplorer();
	BOOL ScanFileSystem();
	BOOL ScanRegistryKeys();
	
	// Generated message map functions
	//{{AFX_MSG(CPageScanning)
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	// The WMI scanner which we will use
	CWMIScanner*			_pWmiScanner;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.
