#include "stdafx.h"
#include "Scan32.h"
#include "PageScanning.h"

// constants
#define TIMER_ID_START	1
#define TIMER_ID_FINISH	2


IMPLEMENT_DYNCREATE(CPageScanning, CScannerPage)

CPageScanning::CPageScanning() : CScannerPage(CPageScanning::IDD), m_tsElapsed(0)
{
	//{{AFX_DATA_INIT(CPageScanning)
	//}}AFX_DATA_INIT
	_pHardwareScanner = NULL;
	_pSoftwareScanner = NULL;
	_pOperatingSystem = NULL;
	_pInternetExplorerScanner = NULL;
	_pFileSystemScanner = NULL;
	_pWmiScanner = NULL;
	_pRegistryScanner = NULL;
}

CPageScanning::~CPageScanning()
{
	// Clean up all scanners
	if (_pHardwareScanner != NULL)
		delete _pHardwareScanner;
	//	
	if (_pSoftwareScanner != NULL)
		delete _pSoftwareScanner;
	//
	if (_pOperatingSystem != NULL)
		delete _pOperatingSystem;
	//
	if (_pFileSystemScanner != NULL)
		delete _pFileSystemScanner;
	//
	if (_pInternetExplorerScanner != NULL)
		delete _pInternetExplorerScanner;
	//
	if (_pRegistryScanner != NULL)
		delete _pRegistryScanner;	
}

void CPageScanning::DoDataExchange(CDataExchange* pDX)
{
	CScannerPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPageScanning)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CPageScanning, CScannerPage)
	//{{AFX_MSG_MAP(CPageScanning)
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/*
** Page becoming active
*/
BOOL CPageScanning::OnSetActive() 
{
	// set up navigation buttons
	CShtScan* pParent = (CShtScan*)GetParent();
	pParent->SetButtons();

	return CScannerPage::OnSetActive();
}


/*
** Once-only initialisation when dialog first displayed
*/
BOOL CPageScanning::OnInitDialog() 
{
	// Call the base class implementation
	CScannerPage::OnInitDialog();

	// Auto-run the scan by firing a timer message
	SetTimer (TIMER_ID_START, 100, NULL);	
	TRACE("CPageScanning -> Starting Timer ID %d\n", TIMER_ID_START);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPageScanning::OnTimer(UINT nIDEvent) 
{
	CLogFile log;

	TRACE("CPageScanning::OnTimer(%d)\n", nIDEvent);
	// which timer is it ?
	switch (nIDEvent) 
	{
		case TIMER_ID_START:
			// kill it
			KillTimer(TIMER_ID_START);		
			TRACE("CPageScanning -> Killing Timer ID %d\n", TIMER_ID_START);
			
			// ...and start the scan
			if (!Scan())
			{
				// if it fails, close the application
				KillTimer(TIMER_ID_FINISH);	
				TRACE("CPageScanning -> Killing Timer ID %d\n", TIMER_ID_FINISH);
				((CPropertySheet*)GetParent())->PressButton(PSBTN_FINISH);
				break;
			}
			else
				return;

		case TIMER_ID_FINISH:
			// kill it
			KillTimer(TIMER_ID_FINISH);		TRACE("CPageScanning -> Killing Timer ID %d\n", TIMER_ID_FINISH);
			// ..then move on to next page
			((CShtScan*)GetParent())->MoveNext();
			break;

		default:
			break;
	}
	
	CScannerPage::OnTimer(nIDEvent);
}

/*
** Main routine to do the hardware scan
*/
BOOL CPageScanning::Scan() 
{
	CLogFile log;
	BOOL bResult = FALSE;

	try
	{
		// disable UI while scan proceeds
		CWaitCursor wc;
		((CPropertySheet*)GetParent())->SetWizardButtons(0);

		// perform the scan
		CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);

		log.Write("", true);
		log.Write("===================================================================================");
		log.Write("starting main scanning function");
		log.Write("===================================================================================");

		// Audit the hardware on this system
		if (!ScanHardware())
			return false;

		// Audit the Operating System and Installed Software
		if (!ScanSoftware())
			return false;

		// Audit Internet Explorer Settings
		if (!ScanInternetExplorer())
			return false;
		
		// Scan the File System
		if (!ScanFileSystem())
			return false;

		// Scan the registry keys
		if (!ScanRegistryKeys())
			return false;

		// Indicate the we have completed the audit and enable the Back/Next buttons
		pListBox->AddString("The Audit of this System is now Complete");
		
		// auto-advance to next page - elapsed time depends on mode
		SetTimer (TIMER_ID_FINISH, (GetAPP()->ScannerMode() == CAuditScannerConfiguration::modeNonInteractive) ? 100 : 5000, NULL);
		
		// and switch back/next buttons back on
		((CShtScan*)GetParent())->SetButtons();
	}
	catch (CException *pEx)
	{
		throw pEx;;

		//return FALSE;
	}

	// Return status
	return TRUE;
}


BOOL CPageScanning::ScanHardware()
{
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanHardware() in");

	// Do we want to audit the hardware?  
	// If not we can return right now as nothing to do
	/*if (auditScannerConfiguration.HardwareScan() == CAuditScannerConfiguration::noFolders)
		return true;*/
	if (GetCFG().HardwareScan() == false)
	{
		log.Write("ScanHardware set to false so not scanning.");
		return true;
	}

	// Create the hardware scanner and call its scan function
	CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);
	pListBox->AddString("Auditing the System Hardware...");
	pListBox->RedrawWindow();
	YieldProcess();
	
	// Create and configure the scanner itself
	_pHardwareScanner = new CHardwareScanner();
	_pHardwareScanner->ScanPhysicalDisks(GetCFG().HardwarePhysicalDisks());
	_pHardwareScanner->ScanActiveSystem(GetCFG().HardwareActiveSystem());
	_pHardwareScanner->ScanSecurity(GetCFG().HardwareSecurity());
	_pHardwareScanner->ScanSettings(GetCFG().HardwareSettings());

	//if(!_pHardwareScanner->Scan(_pWmiScanner))
	//	return false;

	_pHardwareScanner->Scan(_pWmiScanner);

	log.Write("CPageScanning::ScanHardware() out");
	log.Write("", TRUE);

	return true;
}


//
//    ScanSoftware
//    ============
//
//    This function is called to audit the operating System and Installed software
//
BOOL	CPageScanning::ScanSoftware()
{
	try
	{
		CLogFile log;

		log.Write("", true);
		log.Write("===================================================================================");
		log.Write("CPageScanning::ScanSoftware() in");

		CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);

		if (GetCFG().SoftwareScan() == false)
		{
			log.Write("ScanSoftware set to false so not scanning.");
			return true;
		}

		// Operating System
		if (GetCFG().SoftwareScanOs())
		{
			log.Write("scanning operating system");
			pListBox->AddString("Auditing the Operating System...");
			pListBox->RedrawWindow();
			YieldProcess();
			_pOperatingSystem = new COperatingSystemScanner();
			_pOperatingSystem->Scan();
			_installedPatches.Scan(_pWmiScanner);
		}
		
		// Registered Apps
		if (GetCFG().SoftwareScanApplications())
		{
			log.Write("scanning software applications");
			pListBox->AddString("Auditing the Installed Software...");
			pListBox->RedrawWindow();
			YieldProcess();
			CAuditScannerConfiguration auditScannerConfiguration = GetCFG();
			_pSoftwareScanner = new CSoftwareScanner();
			_pSoftwareScanner->Scan(&auditScannerConfiguration);
		}

		log.Write("CPageScanning::ScanSoftware() out");
		log.Write("", true);
	}
	catch (CException *pEx)
	{
		CLogFile log;
   		TCHAR   szCause[255];
   		pEx->GetErrorMessage(szCause, 255);
		log.Format("An exception has occured during CPageScanning::ScanSoftware - the exception text was '%s' \n" ,szCause);
   		pEx->Delete();
		return false;
	}

	return true;
}


//
//    ScanInternetExplorer
//    ====================
//
//    This function is called to audit the Internet Explorer History
//
BOOL	CPageScanning::ScanInternetExplorer()
{	
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanInternetExplorer() in");

	CAuditScannerConfiguration auditScannerConfiguration = GetCFG();
	/*if (auditScannerConfiguration.IEScan() == false)
	{
		log.Write("ScanInternetExplorer set to false so not scanning.");
		return true;
	}*/

	CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);
	pListBox->AddString("Auditing the Internet Explorer History...");
	pListBox->RedrawWindow();
	YieldProcess();	
	
	/*if (auditScannerConfiguration.IEHistory() || auditScannerConfiguration.IECookies())
	{*/
		_pInternetExplorerScanner = new CInternetExplorerScanner();

		// CMD 8.3.6
		// The following 2 lines were commented out meaning that IE History and Cookies would ALWAYS be recovered
		// I have added the individual checks back in and commented out the following 2 lines which set the flags
		// base don whether or not IE scanning had been enabled.
		_pInternetExplorerScanner->ScanHistory(auditScannerConfiguration.IEHistory());
		_pInternetExplorerScanner->ScanCookies(auditScannerConfiguration.IECookies());
		//_pInternetExplorerScanner->ScanHistory(auditScannerConfiguration.IEScan());		
		//_pInternetExplorerScanner->ScanCookies(auditScannerConfiguration.IEScan());
		_pInternetExplorerScanner->DetailedScan(auditScannerConfiguration.IEDetails());
		_pInternetExplorerScanner->LimitDays(auditScannerConfiguration.IEDays());

		/*if (!_pInternetExplorerScanner->Scan(_pWmiScanner))
			return false;*/

		_pInternetExplorerScanner->Scan(_pWmiScanner);
	//}

	log.Write("CPageScanning::ScanInternetExplorer() out");
	log.Write("", TRUE);

	return TRUE;
}


//
//    ScanFileSystem
//    ===============
//
//    This function is called to audit the File System
//
BOOL	CPageScanning::ScanFileSystem()
{	
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanFileSystem() in");

	CAuditScannerConfiguration auditScannerConfiguration = GetCFG();
	CString message;
	
	// Do we want to audit the file system?  
	// If not we can return right now as nothing to do
	/*if (auditScannerConfiguration.ScanFolders() == CAuditScannerConfiguration::noFolders)
		return true;*/

	if (auditScannerConfiguration.ScanFileSystem() == false)
	{
		log.Write("ScanFileSystem set to false so not scanning.");
		return true;
	}
		
	// OK We are doing a file scan so log this 
	CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);
	pListBox->AddString("Auditing the Local Hard Disks...");
	pListBox->RedrawWindow();
	YieldProcess();
	
	// Initiate the audit - recover a list of the drives which we need to scan
	_pFileSystemScanner = new CFileSystemScanner();
	_pFileSystemScanner->SetOptions((int)auditScannerConfiguration.ScanFolders()
								 , (int)auditScannerConfiguration.ScanFiles()
								 , auditScannerConfiguration.ListFolders()	
								 , auditScannerConfiguration.ListFiles());	


	// ...and scan the file system
	/*if (!_pFileSystemScanner->Scan(pListBox))
		return false;*/

	_pFileSystemScanner->Scan(pListBox);

	log.Write("CPageScanning::ScanFileSystem() out");
	log.Write("", true);

	// ...finally return
	return true;
}

BOOL CPageScanning::ScanRegistryKeys()
{
	CLogFile log;

	log.Write("", TRUE);
	log.Write("===================================================================================");
	log.Write("CPageScanning::ScanRegistryKeys() in");

	CAuditScannerConfiguration auditScannerConfiguration = GetCFG();
	CString message;
	
	if (auditScannerConfiguration.RegistryScan() == false)
	{
		log.Write("RegistryScan set to false so not scanning.");
		return true;
	}
		
	// OK We are doing a file scan so log this 
	CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);
	pListBox->AddString("Auditing the Registry...");
	pListBox->RedrawWindow();
	YieldProcess();
	
	// Initiate the audit - recover a list of the drives which we need to scan
	_pRegistryScanner = new CRegistryScanner();

	_pRegistryScanner->SetOptions(auditScannerConfiguration.ListRegistryKeys());

	// ...and scan the file system
	/*if (!_pRegistryScanner->Scan())
		return false;*/

	_pRegistryScanner->Scan();

	log.Write("CPageScanning::ScanFileSystem() out");
	log.Write("", true);

	return true;
}


//
//	  Save
//    ====
//
//    Save the audited data to the AUD file
//
BOOL CPageScanning::Save (CAuditDataFile& auditDataFile)
{
	CListBox* pListBox = (CListBox*)GetDlgItem(IDC_LB_PROGRESS);

	// Call the save function of the hardware scanner
	if (_pHardwareScanner != NULL)
	{
		pListBox->AddString("Saving Hardware Data...");
		pListBox->RedrawWindow();
		YieldProcess();
		_pHardwareScanner->Save(auditDataFile);
	}

	// Save Operating System
	if (_pOperatingSystem != NULL)
	{
		pListBox->AddString("Saving Hardware Data...");
		pListBox->RedrawWindow();
		YieldProcess();
		_pOperatingSystem->Save(auditDataFile);

		// Installed Patches
		for (DWORD dw = 0; dw<_installedPatches.GetCount(); dw++)
		{
			auditDataFile.AddInstalledPatch(_installedPatches[dw]);
		}
	}
	
	// Registered Apps
	if (_pSoftwareScanner != NULL)
		_pSoftwareScanner->Save(auditDataFile);

	// Save Internet Explorer History
	if (_pInternetExplorerScanner != NULL)
		_pInternetExplorerScanner->SaveData(&auditDataFile);	
	
	// File System Scan
	if (_pFileSystemScanner != NULL)
		_pFileSystemScanner->Save(&auditDataFile);

	if (_pRegistryScanner != NULL)
		_pRegistryScanner->Save(auditDataFile);
	
	return TRUE;
}


//
//  This page is losing the focus
//
BOOL CPageScanning::OnKillActive() 
{
	// if a timer is pending then kill it off
	KillTimer(TIMER_ID_FINISH);	
	return CScannerPage::OnKillActive();
}