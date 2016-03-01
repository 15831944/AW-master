
// FILE:	TreeCtrl.h
// PURPOSE:	Declarations of various subclasses of the MFC CTreeCtrl object
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2000,2001
// HISTORY:	JRFT - 15.01.2000 - CheckTreeCtrl created
//			JRFT - 24.02.2000 - DragTreeCtrl created
//			JRFT - 01.06.2000 - FileTreeCtrl created

#ifndef _TREECTRL_DEF_
#define _TREECTRL_DEF_

//#include "resource.h"		// project resources - needed for icon references in CFileTreeCtrl

/////////////////////////////////////////////////////////////////////////////
// CTreeCtrlEx - sub-classed control with additional helper functions
//
class CTreeCtrlEx : public CTreeCtrl
{
	DECLARE_DYNAMIC(CTreeCtrlEx)

public:
	// add a new item to the tree
	HTREEITEM Insert (LPCSTR pszText, int nIcon, LPARAM lParam = 0, HTREEITEM hParent = NULL, HTREEITEM hInsertAfter = TVI_SORT, int nChildren = 1);
	// adds a new item, taking into account parent's expansion state
	HTREEITEM InsertItemEx (LPCSTR pszText, int nIcon, LPARAM lParam, HTREEITEM hParent, int nChildren = 1);
	// copy hItem from existing parent to hNewParent
	HTREEITEM CopyItem (HTREEITEM hItem, HTREEITEM hNewParent);
	// move hItem from existing parent to hNewParent
	HTREEITEM MoveItem (HTREEITEM hItem, HTREEITEM hNewParent);
	// return the unselected image index of hItem
	int GetItemImage (HTREEITEM hItem) const;
	// return handle of child item with matching text
	HTREEITEM FindChild (HTREEITEM hParent, LPCSTR pszText) const;
	// return the child count for hItem
	int GetChildCount (HTREEITEM hItem);
	// manually set the child count for hItem
	void SetChildCount (HTREEITEM hItem, int nCount);
	// add a new child item to a tree that performs "active" population, then selects it
	HTREEITEM AddNewChild (HTREEITEM hParent, LPCSTR pszItemLabel, int nIcon, DWORD dwItemData, int nChildCount = 1);
	// remove all current children and collapse item ready for repopulation
	void RemoveChildren (HTREEITEM hItem);
	// save currently selected item in text form
	CString SaveSelection ();
	// restore previously selected tree item
	void RestoreSelection (LPCSTR pszSelection);
	// return TRUE if item is currently shown expanded
	BOOL IsItemExpanded(HTREEITEM hItem)
		{ return (GetItemState (hItem, TVIS_EXPANDED) & TVIS_EXPANDED); }
	// returnb the selected item as a fully expanded string
	CString GetSelectedItemPath (char chSep = '\\');
};

/////////////////////////////////////////////////////////////////////////////
// CCheckTreeCtrl sub-classed control
//
// provides "check-box" functionality which auto-cascades up and down the tree

#define CTV_UNCHECKED	1
#define CTV_CHECKED		2
#define CTV_PARTIAL		3

#define CTVN_CHECKCHANGED	(WM_USER + 1)

typedef struct tagNM_CHECKTREEVIEW
{
	NMHDR		hdr;
	HTREEITEM	hItem;
	UINT		nOldState;
	UINT		nNewState;
} NM_CHECKTREEVIEW, *LPNM_CHECKTREEVIEW;

class CCheckTreeCtrl : public CTreeCtrlEx
{
	DECLARE_DYNAMIC(CCheckTreeCtrl)
// Construction
public:
	CCheckTreeCtrl()			{}
	virtual ~CCheckTreeCtrl()	{}

// Attributes
public:

// Operations
public:
	virtual void SetItemCheck (HTREEITEM hItem, UINT nState, BOOL bForceChildren = TRUE, BOOL bAdjustParent = TRUE);
	virtual UINT GetItemCheck (HTREEITEM hCheck) const;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCheckTreeCtrl)
	//}}AFX_VIRTUAL

// Implementation
protected:
	void UncheckChildren (HTREEITEM hParent);
	void SetChildItemStates (HTREEITEM hParent, UINT nState, BOOL bForce);
	void AdjustParentState (HTREEITEM hItem, UINT nState);

// Generated message map functions
protected:
	//{{AFX_MSG(CCheckTreeCtrl)
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////
// CDragTreeCtrl sub-classed control
//
// provides drag & drop capability simply by overriding three functions

class CDragTreeCtrl : public CTreeCtrlEx
{
	DECLARE_DYNAMIC(CDragTreeCtrl)

// Construction
public:
	CDragTreeCtrl();

// Attributes
protected:
	BOOL		m_bDragging;
	HTREEITEM	m_hDragItem;
	CImageList*	m_pDragImage;
//	HIMAGELIST	m_hDragImage;
	int			m_nScrollMargin;
	int			m_nDelayInterval;
	int			m_nScrollInterval;

// Overrides
public:
	// Return TRUE if it is ok to begin dragging hItem
	virtual BOOL CanDragItem (HTREEITEM hItem) = 0;
	// return TRUE if it is ok to drop m_hDragItem onto hItem
	virtual BOOL CanDropHere (HTREEITEM hItem) = 0;
	// confirms that hItem was dropped onto hTarget - return TRUE to action the move in the tree too
	virtual BOOL DropItem (HTREEITEM hItem, HTREEITEM hTarget) = 0;
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDragTreeCtrl)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CDragTreeCtrl();
	HTREEITEM HighlightDropTarget (CPoint point);

	// Generated message map functions
protected:
	//{{AFX_MSG(CDragTreeCtrl)
	afx_msg void OnBegindrag(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////
//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // #ifndef _TREECTRL_DEF_
