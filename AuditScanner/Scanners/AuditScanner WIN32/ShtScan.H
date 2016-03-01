#pragma once

#include "PageBasicInformation.h"		// Page for entry of basic information (eg Asset Name)
#include "PageLocations.h"				// Location Selection Page
#include "PageUserData.h"				// User-defined data field entry

// Include Hardware Scanner for an AuditWizard Build
#include "PageScanning.h"				// Auditing page
#include "PageFinish.h"					// Data saving
//
#include "../AuditScannerCommon/WMIScanner.h"					// WMI Scanner

/////////////////////////////////////////////////////////////////////////////
// CShtScan
// ----------
// The main Wizard Property Sheet - manages all the sub-pages

class CShtScan : public CPropertySheet
{
	DECLARE_DYNAMIC(CShtScan)

// Construction
public:
	CShtScan(LPCTSTR pszCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);

// Attributes
public:
	// TRUE if running scanner in "hidden" non-interactive mode
	BOOL					m_bHidden;		

	// The various pages in the scanner wizard
	CPageBasicInformation	m_pageAsset;
	CPageLocations			m_pageLocations;
	CPageScanning			m_pageScanning;
	CPageFinish				m_pageSave;
	CDynaList<CPageUserData> _listUserDataPages;
// Operations
public:
	void SetButtons ();			// sets appropriate wizard buttons for a page
	void MoveNext();			// move to next sub-page
	void DisableScanning();		// remove all pages that do automated scanning

	// Update the Audit Cache File with basic details from the audit
	void UpdateCacheFile	(CIniFile* pAuditCacheFile);

	// Save data to the output audit data file
	void SaveAuditData		(CAuditDataFile& file);	

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CShtScan)
	public:
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CShtScan();

	// Generated message map functions
protected:
	//{{AFX_MSG(CShtScan)
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};
