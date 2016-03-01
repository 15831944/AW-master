
// FILE:	ListCtrl.h
// PURPOSE:	Declarations of GP sub-classed List Controls
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 29.03.2001 - written

#ifndef _LISTCTRL_DEF_
#define _LISTCTRL_DEF_

/*
** User-defined messages for implementing in-place combo box edits of list items
*/

// Notification an edit is beginning. wParam is the ctrl ID, lParam a ptr to the CListCtrlComboBox object 
#define WM_LVN_BEGINCOMBOEDIT	(WM_USER + 1)
// Notification that in-place editing has changed the text. wParam is the ctrl ID, lParam a ptr to the CListCtrlComboBox object, parent returns nonzero to accept the change
#define WM_LVN_COMBOEDITCHANGE	(WM_USER + 2)

// Sorted list datatypes
enum SLITYPE { sliInteger ,sliDate ,sliIPAddress ,sliString ,sliInterval};

/////////////////////////////////////////////////////////////////////////////
//
//	Helper class for in-place editing of list control items
//
class CListCtrlComboBox : public CComboBox
{
	DECLARE_DYNAMIC(CListCtrlComboBox)
// Construction
public:
	CListCtrlComboBox();

// Attributes
public:
	int	m_nRow;
	int	m_nCol;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CEditListCtrlComboBox)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CListCtrlComboBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CEditListCtrlComboBox)
	afx_msg void OnKillFocus(CWnd* pNewWnd);
	afx_msg void OnSelendok();
	afx_msg void OnSelendcancel();
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

///////////////////////////////////////////////////////////////////////////////
//
// Extended list control
//

class CListCtrlEx : public CListCtrl
{
public:
	enum eColType { colTypeNormal, colTypeCombo };
public:
	DECLARE_DYNAMIC(CListCtrlEx)

public:
	// Construction / Destruction
	CListCtrlEx();
	// Return number of columns currently defined
	int GetColCount()
		{ return m_nColCount; }
	// return first or only selected item, -1 if no item selected
	int GetSelectedItem ();
	// return TRUE if specified item is selected
	BOOL IsItemSelected (int nItem)
		{ return (0 != (LVIS_SELECTED & GetItemState(nItem, LVIS_SELECTED))); }
	// select a single item, and reset all other selections
	void SelectItem (int nItem);
	// auto-size all column widths
	void AutoSizeCols(BOOL bUseHeaders = TRUE);
	// return the icon index of an item
	int GetItemImage (int nItem) const;
	// updaet the icon index of an item
	void SetItemImage (int nItem, int nIcon);
	// Override to insert a column (maintains internal column count)
	virtual int InsertColumn(int nCol, const LVCOLUMN* pColumn, eColType = colTypeNormal);
	// Override to insert a column (maintains internal column count)
	virtual int InsertColumn(int nCol, LPCTSTR lpszColumnHeading,
		int nFormat = LVCFMT_LEFT, int nWidth = -1, int nSubItem = -1, eColType = colTypeNormal);
	// Override to delete a column
	virtual BOOL DeleteColumn(int nCol);
	// override to delete all columns
	virtual void DeleteAllCols();
	// write an integer value as a text string
	BOOL SetItemInt (int nItem, int nSubItem, int nData);
	// write a hex value as a text string in 0xHHHH format
	BOOL SetItemHex (int nItem, int nSubItem, DWORD dwData);
	// write a boolean value as a text string either "yes" or "no"
	BOOL SetItemBool (int nItem, int nSubItem, BOOL bData);
	// Export contents of list view to text or csv file (depends on file extension)
	BOOL Export(LPCSTR pszTitle = NULL, char chSep = ',', BOOL bSelectedOnly = FALSE);
	// Export contents of list view and print them
	BOOL Print (LPCSTR pszTitle = NULL, BOOL bSelectedOnly = FALSE);
	// return name of a column
	CString GetColName (int nCol) const;
	// return list of column headings
	void GetColumnHeadings	(CDynaList<CString>& listHeadings);
	// return a list of current column widths
	DWORD GetColumnWidths (CDynaList<int> & widths);
	// set all column widths
	void SetColumnWidths (CDynaList<int> const & widths);
	// return list of row data
	void GetRow	(int nRow ,CDynaList<CString>& listData);

/*
** In-place editing via encapsulated combo box
*/
	// override to set the type of a column, for in-place editing
	virtual void SetColType (int nCol, eColType type);
	// called immediately before display of the in-place combo
	virtual int OnBeginEditCombo (int/* nRow*/, int/* nCol*/, CSortList &/* list*/)
	{
		// MUST be overridden by derived class wishing to use this feature
		ASSERT(FALSE);
		return 0;
	}
	// called when data changes. Default behaviour is simply to update the list control
	virtual void OnEditComboChanged (int nRow, int nCol, LPCSTR pszNewValue);

protected:
	BOOL ExportText(LPCSTR pszFileName, LPCSTR pszTitle, BOOL bSelectedOnly = FALSE);
	BOOL ExportCSV(LPCSTR pszFileName, LPCSTR pszTitle ,char chSep, BOOL bSelectedOnly = FALSE);
private:
	int	m_nColCount;			// standard control doesn't keep count!

	CDynaList<eColType>	m_colTypes;
	CListCtrlComboBox	m_cb;

	// Generated message map functions
protected:
	//{{AFX_MSG(CEditListCtrl)
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	//}}AFX_MSG

protected:
	DECLARE_MESSAGE_MAP()
};

// forward declaration
class CDragTreeCtrl;

class CDragListCtrl : public CListCtrlEx
{
public:
	DECLARE_DYNAMIC(CDragListCtrl)

public:
	CDragListCtrl ();

protected:
	BOOL			m_bDragging;		// TRUE whilst a drag'n'drop operation is in progress
	CImageList*		m_pDragImage;		// The image being dragged
	CDynaList<int>	m_dropList;			// list of selected items being dragged

	CWnd *			m_pDropWnd;			// current drop target window
	DWORD			m_dwDropItem;		// current drop target item

// Overrides
public:
	virtual BOOL CanDragItems (int nItem) = 0;
	virtual BOOL CanDropHere (int nItem) = 0;
	virtual void DropItem (int nItem, int nDropOn) = 0;
	virtual void DropItemTV (CDragTreeCtrl * pTV, HTREEITEM hTarget, int nItem);

protected:
	CImageList * CreateDragImageEx (CPoint * pPoint);

	// Generated message map functions
protected:
	//{{AFX_MSG(CDragListCtrl)
	afx_msg void OnBegindrag(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};


//
//    Implement a sortable list control with various data types to make this 
//    task easier
//
class CSortListItem
{
public:
 //   DECLARE_DYNAMIC(CSortListItem)

	virtual ~CSortListItem()
	{}

	// The type of the list item
	SLITYPE	m_nType;

	// A display units field - added to the column data
	CString		m_strDisplayUnits;

	// Comparison function (pure virtual)
	virtual int		Compare	(CSortListItem* pItemToCompare) = 0;
};

class CSortListItemInteger : public CSortListItem
{
public:

	CSortListItemInteger(int nData)
	{ m_nData = nData; m_nType = sliInteger; }

	virtual ~CSortListItemInteger()
	{}

	int		m_nData;

	virtual int	Compare	(CSortListItem* pItemToCompare)
	{
		if (m_nType != sliInteger)
			return 0;

		CSortListItemInteger* pItem = (CSortListItemInteger*) pItemToCompare;
		if (m_nData > pItem->m_nData)
			return 1;
		else if (m_nData == pItem->m_nData)
			return 0;
		else 
			return -1;
	}
};

class CSortListItemString : public CSortListItem
{
public:
	CSortListItemString(CString	strData)
	{ m_strData = strData; m_nType = sliString; }

	virtual ~CSortListItemString()
	{}

	CString		m_strData;

	virtual int	Compare	(CSortListItem* pItemToCompare)
	{
		if (m_nType != sliString)
			return 0;
		CSortListItemString* pItem = (CSortListItemString*) pItemToCompare;
		return m_strData.Compare(pItem->m_strData);
	}
};

class CSortListItemDate : public CSortListItem
{
public:
	CSortListItemDate(CTime	ctData)
	{ m_ctData = ctData; m_nType = sliDate; }

	virtual ~CSortListItemDate()
	{}

	CTime m_ctData;

	virtual int	Compare	(CSortListItem* pItemToCompare)
	{
		if (m_nType != sliDate)
			return 0;

		CSortListItemDate* pItem = (CSortListItemDate*) pItemToCompare;
		if (m_ctData > pItem->m_ctData)
			return 1;
		else if (m_ctData == pItem->m_ctData)
			return 0;
		else 
			return -1;
	}
};

class CSortListItemInterval : public CSortListItem
{
public:
	CSortListItemInterval(CTimeSpan	tsData)
	{ m_tsData = tsData; m_nType = sliInterval; }

	virtual ~CSortListItemInterval()
	{}

	CTimeSpan m_tsData;

	virtual int	Compare	(CSortListItem* pItemToCompare)
	{
		if (m_nType != sliInterval)
			return 0;

		CSortListItemInterval* pItem = (CSortListItemInterval*) pItemToCompare;
		if (m_tsData > pItem->m_tsData)
			return 1;
		else if (m_tsData == pItem->m_tsData)
			return 0;
		else 
			return -1;
	}
};

class CSortListItemIP : public CSortListItem
{
public:
	CSortListItemIP(CIPAddress	IPAddress)
	{ m_ipData = IPAddress; m_nType = sliIPAddress; }

	virtual ~CSortListItemIP()
	{}

	CIPAddress		m_ipData;

	virtual int	Compare	(CSortListItem* pItemToCompare)
	{
		if (m_nType != sliIPAddress)
			return 0;

		CSortListItemIP* pItem = (CSortListItemIP*) pItemToCompare;
		return m_ipData.Compare(pItem->m_ipData);
	}
};


class CSortListRow : public CDynaList<CSortListItem*>
{
public:
	CSortListRow()
	{ m_dwData = 0; m_crRowColor = 0; }

	virtual ~CSortListRow();

	// A data member for each row
	DWORD m_dwData;

	// A Color value for esach row - this allows us to colr the rows individually
	COLORREF m_crRowColor;
};

/////////////////////////////////////////////////////////////////////////////
// CSortHeaderCtrl window

class CSortHeaderCtrl : public CHeaderCtrl
{
public:
	CSortHeaderCtrl();


// Implementation
public:
	virtual ~CSortHeaderCtrl();
	int SetSortImage(int nCol, BOOL bAsc);

	int GetSortedColumn()
	{ return m_nSortColumn; }

	BOOL IsSortedAscending()
	{ return m_bSortAscending; }

	static const int NOT_SORTED;

protected:

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSortHeaderCtrl)
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CSortHeaderCtrl)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	void SetOwnerDraw(int nCol);

	int		m_nSortColumn;
	BOOL	m_bSortAscending;

	DECLARE_MESSAGE_MAP()
};

///////////////////////////////////////////////////////////////////////////////
// CSortListCtrl
//
// A sortable list control
// Note that this is not code compatible with the standard list control 
// as we need to know various bits of information about the list items and data
//
class CSortListCtrl : public CDragListCtrl
{
public:
	DECLARE_DYNAMIC(CSortListCtrl)

public:
	CSortListCtrl ();

	virtual ~CSortListCtrl ();

public: 
    enum EHighlight {HIGHLIGHT_NORMAL, HIGHLIGHT_ALLCOLUMNS, HIGHLIGHT_ROW};
    enum EBackground {BACK_NORMAL ,BACK_HIGHFIRST};

	//
    BOOL	SetCurSel				(int row);    
	void	RepaintSelectedItems	(void);
    void	SetBackground			(EBackground background ,COLORREF Color);
	int		SetHighlightType		(EHighlight hilite);    
	void	SetForegroundColor		(COLORREF foreColor) 
		{ m_crForegroundColor = foreColor; }    

	// Enable the display of a sort item in the column header
	//void	EnableSortIcon			(BOOL bEnable, int nSortColumn);

	// Insert an item
	int		InsertItem (int nItem, LPCSTR pData, int nImage ,int nItemData=-1);
	int		InsertItem (int nItem, CTime& ctData, int nImage ,int nItemData=-1);
	int		InsertItem (int nItem, int nValue, int nImage ,int nItemData=-1);
	int		InsertItem (int nItem, CIPAddress& ipData, int nImage ,int nItemData=-1);
	int		InsertItem (int nItem, CTimeSpan& tsData, int nImage ,int nItemData=-1);

	// Set an item
	BOOL	SetItem		(int nItem, int nSubItem, UINT nMask, LPCSTR pData, int nImage, UINT nState, UINT nStateMask, LPARAM lParam);
	BOOL	SetItem		(int nItem, int nSubItem, UINT nMask, CTime& pData, int nImage, UINT nState, UINT nStateMask, LPARAM lParam);
	BOOL	SetItem		(int nItem, int nSubItem, UINT nMask, int nValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam);
	BOOL	SetItem		(int nItem, int nSubItem, UINT nMask, CIPAddress& ipData, int nImage, UINT nState, UINT nStateMask, LPARAM lParam);
	BOOL	SetItem		(int nItem, int nSubItem, UINT nMask, CTimeSpan& tsData, int nImage, UINT nState, UINT nStateMask, LPARAM lParam);

	BOOL	SetItemText	(int nItem, int nSubItem, LPCSTR pData);
	BOOL	SetItemText	(int nItem, int nSubItem, CTime& ctData);
	BOOL	SetItemText	(int nItem, int nSubItem, int nValue);
	BOOL	SetItemText	(int nItem, int nSubItem, CIPAddress& ipData);
	BOOL	SetItemText	(int nItem, int nSubItem, CTimeSpan tsData);
	BOOL SetItemSize (int nItem, int nSubItem, DWORD dwSize);
	BOOL SetItemDate (int nItem, int nSubItem, CTime ctDate);
	BOOL SetItemTime (int nItem, int nSubItem, CTime ctTime, BOOL bIncludeSeconds);

	// Set the data for an item
	BOOL SetItemData    (int nItem, DWORD dwData);

	// Get the item data 
	DWORD GetItemData	(int nItem) const;

	// Delete All Items
	BOOL DeleteAllItems ();

	// Delete a single item
	BOOL DeleteItem(int nItem);

	// Delete a column
	BOOL DeleteColumn(int nCol);
	void DeleteAllCols();

	// Set a 'Display Units' string
	void SetColumnUnits (int nColumn, LPCSTR szUnits);

	void OnColumnclick(NMHDR* pNMHDR, LRESULT* pResult); 

	//  Set the background color for a row
	void SetRowBackground	(int nRow ,COLORREF bkColor);
  
	// Resort the column currently sorted using the same order
    void ReSort				(void);

	// Return the column on which we are currently sorted
	int	 GetSortedColumn	(void)
	{ return m_HeaderCtrl.GetSortedColumn(); }

	// Is the list sorted ina scending order
	BOOL IsSortedAscending	(void)
	{ return m_HeaderCtrl.IsSortedAscending(); }

	void	SortColumn		(int nCol, BOOL bAsc);

	// Drag and drop operations - default implementations do nothing

//	virtual BOOL CanDragItems (int /*nItem*/)
//	{ return FALSE; }
//
//	virtual BOOL CanDropHere (int /*nItem*/)
//	{ return FALSE; }
//
//	virtual void DropItem (int /*nItem*/, int /*nDropOn*/)
//	{ return; }
//
//	virtual void DropItemTV (CDragTreeCtrl* /*pTV*/, HTREEITEM /*hTarget*/, int /*nItem*/)
//	{ return; }
//

public:
    //{{AFX_VIRTUAL(CSortListCtrl)
protected:
    virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
    virtual BOOL OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult);
    //}}AFX_VIRTUAL
    
protected:
    //{{AFX_MSG(CSortListCtrl)
    afx_msg void OnPaint();
    afx_msg void OnKillFocus(CWnd* pNewWnd);
    afx_msg void OnSetFocus(CWnd* pOldWnd);
    //}}AFX_MSG 

    DECLARE_MESSAGE_MAP()

protected:
	CString Store			(int nItem ,int nSubItem ,void* pData ,SLITYPE type);
	void	SetHeader		(void);

protected:
	CDynaList<CSortListRow*>	m_listData;

protected:
    int			m_nHighlightType;				// One of EHighligh enums
    COLORREF	m_crForegroundColor;
	EBackground m_eBackground;
    COLORREF	m_crBackgroundColor;
	BOOL		m_bSetup;

	// Column unit strings
	CDynaList<CString>		m_listColumnUnits;

//	void SetSortIcon();
//	void CreateSortIcons();
//	void OnSysColorChange();

	// The header control
	CSortHeaderCtrl m_HeaderCtrl;

	static int CALLBACK SortCallback (LPARAM lParam1, LPARAM lParam2, LPARAM pThis);

	static const int NOT_SORTED;
};

#endif