#pragma once

#include "ScannerPage.h"


/////////////////////////////////////////////////////////////////////////////
// CPageUserData dialog

class CPageUserData : public CScannerPage
{
	DECLARE_DYNCREATE(CPageUserData)

// Construction
public:
	CPageUserData()
	{ _pUserDataCategory = NULL; }
	
	CPageUserData(CUserDataCategory* pUserDataCategory);
	~CPageUserData();

// Dialog Data
	//{{AFX_DATA(CPageUserData)
	enum { IDD = IDD_PAGEUSERDATA };
		// NOTE - ClassWizard will add data members here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_DATA
	
// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPageUserData)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnWizardFinish();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnCommand(WPARAM wParam, LPARAM lParam);
	//}}AFX_VIRTUAL

// Implementation
protected:
	void FailDDX (int nID, LPCSTR pszMessage);

	// geometric functions for handling the variable number of controls
	void CreateControls ();
	void MoveControls ();

	int	m_nScrollOffset;		// current amount of scroll

	// Generated message map functions
	//{{AFX_MSG(CPageUserData)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	
private:
	CUserDataCategory*	_pUserDataCategory;

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.
