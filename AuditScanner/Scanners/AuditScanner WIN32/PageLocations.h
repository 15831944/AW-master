#pragma once
#include "ScannerPage.h"

// Helper class to hold database location data
class CLocItem
{
public:
	CLocItem()	{}
	CLocItem(DWORD dwID, LPCSTR pszName, DWORD dwParentID) : m_strName(pszName)
		{ m_dwID = dwID, m_dwParentID = dwParentID; }
	DWORD GetID()		{ return m_dwID; }
	LPCSTR GetName()	{ return m_strName; }
	DWORD GetParentID()	{ return m_dwParentID; }
	BOOL operator== (const CLocItem & other);
protected:
	DWORD	m_dwID;
	CString	m_strName;
	DWORD	m_dwParentID;
};

/////////////////////////////////////////////////////////////////////////////
// CPageLocations dialog

class CPageLocations : public CScannerPage
{
	DECLARE_DYNCREATE(CPageLocations)

// Construction
public:
	CPageLocations();
	~CPageLocations();

// Dialog Data
	CDynaList<CLocItem>	m_locData;	// list of data read from CFG file
	CString	m_strLocation;			// currently selected location

	//{{AFX_DATA(CPageLocations)
	enum { IDD = IDD_PAGELOCATION };
	CTreeCtrl	m_tvLocation;
	//}}AFX_DATA

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPageLocations)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnKillActive();
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void AddChildLocations (DWORD dwParentID, HTREEITEM hParent);
	void SelectLocation (LPCSTR pszLocation);

	// Generated message map functions
	//{{AFX_MSG(CPageLocations)
	virtual BOOL OnInitDialog();
	afx_msg void OnBnAdd();
	afx_msg void OnEndlabeleditTvLocation(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnBeginlabeleditTvLocation(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	
private:
	CImageListEx	m_imageList;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.
