//==========================================================================//
//																			//
//	ShtScan.cpp	: Implementation of scanner sheet class for AuditWizard		//
//	----------																//
//																			//
//	AUTHOR:	JRF Thornley - copyright (C) Layton Software 2001				//
//																			//
//==========================================================================//
//																			//
//  History																	//
//  --------																//
//																			//
//		40C005				Chris Drew				01-NOV-2001				//
//		Save any previous name that this asset had so that AuditWizard		//
//		can detect a name change for an asset.              				//
//																			//
//==========================================================================//

#include "stdafx.h"

#include "../AuditScannerCommon/AuditDataFile.h"	// The output audit data file
#include "Scan32.h"			// the main application class
#include "ShtScan.h"		// this class

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define TIMER_HIDE	1

#pragma message ("***** SCANNER VERSION EXPLICITELY SET HERE ***")
#define SCAN32_VERSION	"8.4.4.6"

/////////////////////////////////////////////////////////////////////////////
// CShtScan
// --------
// The main Wizard Sheet
 
IMPLEMENT_DYNAMIC(CShtScan, CPropertySheet)

CShtScan::CShtScan(LPCTSTR pszCaption, CWnd* pParentWnd, UINT iSelectPage)
	:CPropertySheet(pszCaption, pParentWnd, iSelectPage)
{
	m_bHidden = FALSE;
}

CShtScan::~CShtScan()
{
}

void CShtScan::MoveNext()
{
	// get current page number
	int nIndex = GetActiveIndex();
	// if last page exit, else move tab along one
	if (nIndex == (GetPageCount() - 1))
	{
		PressButton(PSBTN_FINISH);
	}
	else
	{
		PressButton(PSBTN_NEXT);
	}
}

void CShtScan::SetButtons()
{
	DWORD dwButtons = 0;

	// get the current page number
	int nIndex = GetActiveIndex();
	
	// is it the first page ?
	if (nIndex != 0)
		// no - allow the "Back" button to show
		dwButtons |= PSWIZB_BACK;
	
	// is it the last page ?
	int nCount = GetPageCount();
	if (nIndex == nCount - 1)
		// yes - set "Finish" button
		dwButtons |= PSWIZB_FINISH;
	else
		// no - set the "Next" button
		dwButtons |= PSWIZB_NEXT;
	
	// activate the changes
	SetWizardButtons(dwButtons);

	// If cancel disabled then remove the cancel button
	/*if ((GetCFG().FieldsMask() & CAuditScannerConfiguration::Cancel) == 0)
		GetDlgItem (IDCANCEL)->ShowWindow (SW_HIDE);*/

	if (!GetCFG().InteractiveAllowCancel())
		GetDlgItem (IDCANCEL)->ShowWindow (SW_HIDE);
}

BEGIN_MESSAGE_MAP(CShtScan, CPropertySheet)
	//{{AFX_MSG_MAP(CShtScan)
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CShtScan message handlers


BOOL CShtScan::OnInitDialog() 
{
	BOOL bResult = CPropertySheet::OnInitDialog();

	// sort out the icon and minimise button
	SetIcon (AfxGetApp()->LoadIcon (IDR_MAINFRAME), FALSE);
	SetIcon (AfxGetApp()->LoadIcon (IDR_MAINFRAME), TRUE);

	// add the minimize button to the window
	::SetWindowLong (m_hWnd, GWL_STYLE, GetStyle() | WS_MINIMIZEBOX | WS_SYSMENU);
		
	// add the minimize command to the system menu
	GetSystemMenu(FALSE)->InsertMenu (-1, MF_BYPOSITION | MF_STRING, SC_MINIMIZE, "Mi&nimize");
	GetSystemMenu(FALSE)->InsertMenu (-1, MF_BYPOSITION | MF_STRING, SC_RESTORE, "&Restore");

	// activate the new sysmenu
	DrawMenuBar();

	// minimise the window if required
	if (m_bHidden)
	{
		// Set the window as a tool window so it doesn't appear on the taskbar
		ModifyStyleEx(WS_EX_APPWINDOW, WS_EX_TOOLWINDOW);
		// initially show as minimised, then kick a timer to hide it completely
		ShowWindow(SW_SHOWMINIMIZED);
		SetTimer(TIMER_HIDE, 1, NULL);
	}

	return bResult;
}

/*
** This routine is called in order to disable the physical
** scans for peripherals. It also inhibits the save to the local station file
*/
void CShtScan::DisableScanning ()
{
	GetAPP()->_assetCacheFileScope = CAssetCacheFile::cacheFileNone;
}

//
//    SaveAuditData
//    ==============
//
//    Save the results from the audit to the output audit data file
//
void CShtScan::SaveAuditData (CAuditDataFile& auditDataFile)
{
	CString strBuffer;
	CScan32App & app = *(GetAPP());

	// write the scanner and version used
	char szAppName[_MAX_PATH + 1];
	GetModuleFileName(app.m_hInstance, szAppName, _MAX_PATH);
	CString strVersion = FindVersionString (szAppName, "ProductVersion");
	strVersion = SCAN32_VERSION;

	strBuffer.Format("AuditScanner v%s", strVersion); 
	auditDataFile.Version(strBuffer);

	auditDataFile.Computername(app.m_strAssetName);		// AuditWizard asset name

	// date stamp
	CTime & timestamp = app.DateOfThisAudit();
	auditDataFile.AuditDate(app.DateOfThisAudit());
	
	// category / domain / make / model / serial
	auditDataFile.Uniqueid(app.m_strUniqueID);
	auditDataFile.Domain(app.m_strDomain);
	auditDataFile.Category(app.m_strCat);
	auditDataFile.Make(app.m_strMake);
	auditDataFile.Model(app.m_strModel);
	auditDataFile.Serial_number(app.m_strSerial);
	auditDataFile.AssetTag(app.m_strAssetTag);

	// MAC and IP address (scannable assets only)
	if (GetAPP()->IsAssetScannable())
	{
		auditDataFile.Macaddress(app.m_strMacAddr);
		auditDataFile.Ipaddress(app.m_strIPAddr);
	}

	// location
	auditDataFile.Location(app.m_strLoc);
	
	// Save the User Defined Data Fields to the audit data file
	auditDataFile.SetUserDataList(&GetCFG().UserDataCategories());

	// Audited Items Save Now...
	m_pageScanning.Save(auditDataFile);
}


void CShtScan::OnTimer(UINT nIDEvent) 
{
	if (TIMER_HIDE == nIDEvent)
	{
		// This should only occur when the app is being hidden
		ASSERT (m_bHidden);
		// don't want a repeat performance
		KillTimer(TIMER_HIDE);
		// hide the window. Doing this directly in WM_INITDIALOG handler seems to get ignored by MFC
		ShowWindow(SW_HIDE);
	}
	
	CPropertySheet::OnTimer(nIDEvent);
}
