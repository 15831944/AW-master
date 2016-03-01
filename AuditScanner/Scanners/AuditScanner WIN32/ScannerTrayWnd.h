#pragma once
// ScannerTrayWnd.h : header file
//
#include "../../MfcExt/Include/TrayIcon.h"

/////////////////////////////////////////////////////////////////////////////
// CScannerTrayWnd window

class CScannerTrayWnd : public CWnd
{
// Construction
public:
	CScannerTrayWnd();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CScannerTrayWnd)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CScannerTrayWnd();

	void	SetActive (BOOL bActive);

	// Generated message map functions
protected:
	//{{AFX_MSG(CScannerTrayWnd)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnClose();
	//}}AFX_MSG
	LRESULT OnTrayNotification(WPARAM wParam, LPARAM lParam);
	void OnProperties();
	DECLARE_MESSAGE_MAP()

private:
	CTrayIcon	m_TrayIcon;
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.