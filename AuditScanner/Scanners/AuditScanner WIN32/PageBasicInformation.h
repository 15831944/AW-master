#pragma once

#include "resource.h"
#include "ScannerPage.h"

/////////////////////////////////////////////////////////////////////////////
// CPageBasicInformation dialog

class CPageBasicInformation : public CScannerPage
{
	DECLARE_DYNCREATE(CPageBasicInformation)

// Construction
public:
	CPageBasicInformation();
	~CPageBasicInformation();

// Dialog Data
	//{{AFX_DATA(CPageBasicInformation)
	enum { IDD = IDD_PAGE_BASICINFORMATION };
	CString	m_strAssetName;
	CString	m_strCat;
	CString	m_strMake;
	CString	m_strModel;
	CString	m_strSerial;
	//}}AFX_DATA

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPageBasicInformation)
	public:
	virtual BOOL OnSetActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CPageBasicInformation)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.