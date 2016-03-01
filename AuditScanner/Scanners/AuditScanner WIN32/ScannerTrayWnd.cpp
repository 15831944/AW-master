// ScannerTrayWnd.cpp : implementation file
//

#include "stdafx.h"
#include "scan32.h"
#include "ScannerTrayWnd.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define WMU_ICON_NOTIFY          WM_USER+10

/////////////////////////////////////////////////////////////////////////////
// CScannerTrayWnd

CScannerTrayWnd::CScannerTrayWnd()
{
}

CScannerTrayWnd::~CScannerTrayWnd()
{
}


BEGIN_MESSAGE_MAP(CScannerTrayWnd, CWnd)
	//{{AFX_MSG_MAP(CScannerTrayWnd)
	ON_WM_CREATE()
	ON_WM_CLOSE()
	//}}AFX_MSG_MAP
	ON_MESSAGE(WMU_ICON_NOTIFY, OnTrayNotification)
	ON_COMMAND(ID_PROPERTIES, OnProperties)
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CScannerTrayWnd message handlers

int CScannerTrayWnd::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CWnd::OnCreate(lpCreateStruct) == -1)
		return -1;
	
    // Create the tray icon
    if (!m_TrayIcon.Create(this, WMU_ICON_NOTIFY, _T("AuditWizard Scanner"), NULL, IDR_SCAN_POPUP))
        return -1;

    m_TrayIcon.SetIcon(IDI_SCANNER_SLEEP);
	
	return 0;
}


LRESULT CScannerTrayWnd::OnTrayNotification(WPARAM wParam, LPARAM lParam)
{
    // Delegate all the work back to the default implementation in
    // CTrayIcon.
    return m_TrayIcon.OnTrayNotification(wParam, lParam);
}


/////////////////////////////////////////////////////////////////////////////
// CScannerTrayWnd/CTrayIcon menu message handlers

void CScannerTrayWnd::OnProperties()
{
#pragma message ("No system tray implemented currently")
}

void	CScannerTrayWnd::SetActive (BOOL bActive)
{
	if (bActive)
		m_TrayIcon.SetIcon(IDI_SCANNER_ACTIVE);
	else
		m_TrayIcon.SetIcon(IDI_SCANNER_SLEEP);
}

void CScannerTrayWnd::OnClose() 
{
	m_TrayIcon.RemoveIcon();
	CWnd::OnClose();
}
