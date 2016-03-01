
// FILE:	ListCtrl.h
// PURPOSE:	Declarations of GP sub-classed List Controls
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 29.03.2001 - written

#ifndef _LISTCTRLEX_DEF_
#define _LISTCTRLEX_DEF_

/*
** User-defined messages for implementing in-place combo box edits of list items
*/
#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP 0x00004000
#endif

// Notification an edit is beginning. wParam is the ctrl ID, lParam a ptr to the CListCtrlComboBox2 object 
#define WM_LVN_BEGINCOMBOEDIT	(WM_USER + 1)
// Notification that in-place editing has changed the text. wParam is the ctrl ID, lParam a ptr to the CListCtrlComboBox2 object, parent returns nonzero to accept the change
#define WM_LVN_COMBOEDITCHANGE	(WM_USER + 2)

// List Control Construction flags
#define LCX_OWNERDRAW			0x01
#define LCX_COLOUR_BYCOL		0x02
#define LCX_COLOUR_BYITEM		0x04
#define LCX_NOSORT				0x08

// column types - held in first bits 0-7 of flags data
#define LCX_COLTYPE_NONE		0x0000
#define LCX_COLTYPE_STRING		0x0001
#define LCX_COLTYPE_INT			0x0002
#define LCX_COLTYPE_SIZE		0x0003
#define LCX_COLTYPE_DATE		0x0004
#define LCX_COLTYPE_TIME		0x0005
#define LCX_COLTYPE_DATETIME	0x0006
#define LCX_COLTYPE_IPADDRESS	0x0007
#define LCX_COLTYPE_BOOL		0x0008
#define LCX_COLTYPE_HEX			0x0009
#define LCX_COLTYPE_MB			0x0010
#define LCX_COLTYPE_MHZ			0x0011

// column edit types - held in bits 8-15 of flags data
#define LCX_COLEDIT_NONE		0x0000
#define LCX_COLEDIT_COMBO		0x0100
#define LCX_COLEDIT_EDIT		0x0200

// Specifies a NULL value for Integer Columns
#define LCX_COLUMN_NA			0x0FFFFFFF

/////////////////////////////////////////////////////////////////////////////
//
//	Helper class for in-place editing of list control items
//
class CListCtrlComboBox2 : public CComboBox
{
	DECLARE_DYNAMIC(CListCtrlComboBox2)
// Construction
public:
	CListCtrlComboBox2();

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
	virtual ~CListCtrlComboBox2();

	// Generated message map functions
protected:
	//{{AFX_MSG(CEditListCtrlComboBox)
	afx_msg void OnKillFocus(CWnd* pNewWnd);
	afx_msg void OnSelendok();
	afx_msg void OnSelendcancel();
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////
//
//	CSortHeaderCtrl2 - sub-classed header window
//

class CSortHeaderCtrl2 : public CHeaderCtrl
{
public:
	// constructor
	CSortHeaderCtrl2 ();
	// destructor
	virtual ~CSortHeaderCtrl2 ();

	// initialise (ok to call multiple times)
	void Init (CListCtrl * pParent);
	// set new sort column, reverses direction if already current
	void SetSortColumn (int nCol);
	// get current sort column
	int GetSortColumn ()			{ return m_nSortCol; }
	// return true if sort direction reversed
	BOOL IsSortReverse ()			{ return m_bSortReverse; }

	void Reset ()
		{ m_nSortCol = 0 ; m_bSortReverse = FALSE; }

protected:
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSortHeaderCtrl2)
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CSortHeaderCtrl2)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()

	virtual void DrawItem (LPDRAWITEMSTRUCT lpDrawItemStruct);

	void SetOwnerDraw (int nCol);

	int		m_nSortCol;
	BOOL	m_bSortReverse;
	BOOL	m_bInitialised;
};

///////////////////////////////////////////////////////////////////////////////
//
//	Helper class for storing list control column data
//

class CListCtrlColData
{
public:
	// constructor
	CListCtrlColData (LPCSTR pszName = NULL, DWORD dwFlags = 0);
	// destructor
	~CListCtrlColData ();

	// column title
	const CString & GetName ()			{ return m_strName; }
	// column data type
	DWORD GetType ()					{ return (m_dwFlags & 0x00FF); }
	// column edit type (in-place editing)
	DWORD GetEditType ()				{ return (m_dwFlags & 0xFF00); }
	// set in-place editing type
	void SetEditType (DWORD dwFlags);

	// return "raw" data element
	LPVOID operator[] (DWORD dwIndex);

	// get background colour
	COLORREF GetBCol ()					{ return m_bColour; }
	// set background colour
	void SetBCol (COLORREF clr)			{ m_bColour = clr; }
	// get foreground colour
	COLORREF GetFCol ()					{ return m_fColour; }
	// set foreground colour
	void SetFCol (COLORREF clr)			{ m_fColour = clr; }
	
	// add data
	DWORD AddRow ();
	// remove data
	void RemoveRow (DWORD dwRow);
	// remove all data
	void Empty ();
	// return contents of a row as a displayable string
	CString GetAsString (DWORD dwRow);

	// Set a data value - strings	
	void SetRow (DWORD dwRow, LPCSTR pszValue);
	// Set a data value - numeric, boolean or hex
	void SetRow (DWORD dwRow, DWORD dwValue);
	// Set a data value - date, time, datetime
	void SetRow (DWORD dwRow, CTime ctValue);
	// Set a data value - IP Address
	void SetRow (DWORD dwRow, CIPAddress ipValue);

protected:
	CString				m_strName;
	DWORD				m_dwFlags;	// holds data type and edit type
	COLORREF			m_bColour;	// background colour
	COLORREF			m_fColour;	// foreground colour
	CDynaList<LPVOID>	m_data;
};

///////////////////////////////////////////////////////////////////////////////
//
//	Helper class for storing row data

class CListCtrlRowData
{
public:
	// constructor
	CListCtrlRowData () : m_fColour(::GetSysColor(COLOR_WINDOWTEXT)), m_bColour(::GetSysColor(COLOR_WINDOW)), m_lParam(0)
		{}
	// destructor
	~CListCtrlRowData ()
		{}

	// store foreground (text) colour
	void SetFColour (COLORREF clr)		{ m_fColour = clr; }
	// get foreground (text) colour
	COLORREF GetFColour () const		{ return m_fColour; }
	// store background colour
	void SetBColour (COLORREF clr)		{ m_bColour = clr; }
	// get background colour
	COLORREF GetBColour () const		{ return m_bColour; }
	// store user data
	void SetItemData (LPARAM lParam)	{ m_lParam = lParam; }
	// get user data
	LPARAM GetItemData () const			{ return m_lParam; }

protected:
	COLORREF	m_fColour;
	COLORREF	m_bColour;
	LPARAM		m_lParam;			// this replaces the original item data
};


///////////////////////////////////////////////////////////////////////////////
//
// Extended list control
//

class CLaytonListCtrl : public CListCtrl
{
public:
	DECLARE_DYNAMIC(CLaytonListCtrl)

public:
	// Construction / Destruction
	CLaytonListCtrl (DWORD dwFlags = LCX_COLOUR_BYCOL);
	virtual ~CLaytonListCtrl ();
	// set mode flags
	void SetFlags (DWORD dwFlags);

	// Return number of columns currently defined
	DWORD GetColCount()					{ return m_cols.GetCount(); }
	// return first or only selected item, -1 if no item selected
	int GetSelectedItem ();
	// return TRUE if specified item is selected
	BOOL IsItemSelected (int nItem)		{ return (0 != (LVIS_SELECTED & GetItemState(nItem, LVIS_SELECTED))); }
	// select a single item, and reset all other selections
	void SelectItem (int nItem);
	// return the icon index of an item
	int GetItemImage (int nItem) const;
	// update the icon index of an item
	void SetItemImage (int nItem, int nIcon);

	// Insert a column
	virtual int InsertColumn (LPCSTR pszColumn, DWORD dwFlags);
	// Override to delete a column
	virtual BOOL DeleteColumn (DWORD dwCol);
	// override to delete all columns
	virtual void DeleteAllColumns ();
	// Set a column colour
	void SetColumnColour (DWORD dwCol, COLORREF fColour, COLORREF bColour);
	// auto-size all column widths
	void AutoSizeColumns (BOOL bUseHeaders = TRUE);
	// return name of a column
	CString GetColumnName (DWORD dwCol) const;
	// return list of column headings
	void GetColumnNames (CDynaList<CString> & listNames);
	// return a list of current column widths
	DWORD GetColumnWidths (CDynaList<int> & widths);
	// set all column widths
	void SetColumnWidths (CDynaList<int> const & widths);

	// Set the sort column
	void SetSortColumn (int nCol ,bool bDirection);
	int  GetSortColumn ()
	{ 	m_headerCtrl.Init (this); return m_headerCtrl.GetSortColumn(); }

	// Set the Title of this list view
	virtual void SetTitle	(CString& strTitle)
	{	m_strTitle = strTitle; }

	virtual CString GetTitle	(void)
	{	return m_strTitle; }


#define LVI_FIRST	0
#define LVI_LAST	-1	
	// Insert an item (nInsertBefore = -1 indicates put it at the end of the list)
	virtual int InsertItem (int nIcon, int nInsertBefore = LVI_LAST, LPCSTR pszText = NULL, DWORD dwItemID = 0);
	// Insert an item
//	virtual int InsertItem (DWORD dwItemID, int nIcon, LPCSTR pszText = NULL);
	// overrides to trap attempted usage of the base class versions
	int InsertItem (const LVITEM*)		{ ASSERT(FALSE); return 0; }
	int InsertItem (int, LPCTSTR)		{ ASSERT(FALSE); return 0; }
	int InsertItem (int, LPCTSTR, int)	{ ASSERT(FALSE); return 0; }
	// set data in a cell - string data type
	virtual void SetItem (int nRow, DWORD dwCol, LPCSTR pszValue);
	// set data in a cell - numeric, boolean or hex
	virtual void SetItem (int nRow, DWORD dwCol, DWORD dwValue);
	// set data in a cell - date, time, datetime
	virtual void SetItem (int nRow, DWORD dwCol, CTime ctValue);
	// set data in a cell - IP address
	virtual void SetItem (int nRow, DWORD dwCol, CIPAddress ipValue);
	// override to trap attempted usage of the base class version
	BOOL SetItemText (int, int, LPCTSTR)	{ ASSERT(FALSE); return FALSE; }	// use one of the SetItem() overloads above
	// set colour for a row of data
	void SetItemColour (int nItem, COLORREF fColour, COLORREF bColour);
	// overridden, as we now manage data storage
	void SetItemData (int nItem, LPARAM lParam);
	// overridden, as we now manage data storage
	DWORD GetItemData (int nItem);
	// remove a row of data
	BOOL DeleteItem (int nItem);
	// Remove all values
	BOOL DeleteAllItems();

	// Export contents of list view to text or csv file (depends on file extension)
	BOOL Export(LPCSTR pszTitle = NULL, char chSep = ',', BOOL bSelectedOnly = FALSE);
	// Export contents of list view and print them
	BOOL Print (LPCSTR pszTitle = NULL, BOOL bSelectedOnly = FALSE);
	// return list of row data
	void GetRow	(int nRow, CDynaList<CString> & listData);

/*
** In-place editing via encapsulated combo box
*/
	// set the type of a column, for in-place editing
	virtual void SetColEditType (DWORD dwCol, DWORD dwFlags);
	// called immediately before display of the in-place combo
	virtual int OnBeginEditCombo (int nRow, int nCol, CSortList & list);
	// called when data changes. Default behaviour is simply to update the list control
	virtual void OnEditComboChanged (int nRow, int nCol, LPCSTR pszNewValue);

protected:
	void DrawCell (int nItem, DWORD dwCol);
	int static CALLBACK m_fnCompareItems (LPARAM dwItem1, LPARAM dwItem2, LPARAM lParamCaller);
	BOOL ExportText (LPCSTR pszFileName, LPCSTR pszTitle, BOOL bSelectedOnly = FALSE);
	BOOL ExportCSV (LPCSTR pszFileName, LPCSTR pszTitle, char chSep, BOOL bSelectedOnly = FALSE);
	CSortHeaderCtrl2& GetHeader ()
	{ return m_headerCtrl; }

private:
	DWORD m_dwFlags;
	CString		m_strTitle;							// Title of this list
	CSortHeaderCtrl2 m_headerCtrl;					// sub-classed sort header control

	CDynaList<CListCtrlColData>	m_cols;				// column data & settings
	CDynaList<CListCtrlRowData> m_rows;				// row data

	CListCtrlComboBox2	m_cb;

public:
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CLaytonListCtrl)
	protected:
	virtual void PreSubclassWindow();
    virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	//}}AFX_VIRTUAL


	// Generated message map functions
protected:
	//{{AFX_MSG(CLaytonListCtrl)
	afx_msg void OnColumnclick (NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLButtonDown (UINT nFlags, CPoint point);
	//}}AFX_MSG

protected:
	DECLARE_MESSAGE_MAP()
};

// forward declaration
class CDragTreeCtrl;

class CLaytonDragListCtrl : public CLaytonListCtrl
{
public:
	DECLARE_DYNAMIC(CLaytonDragListCtrl)

public:
	CLaytonDragListCtrl ();
	virtual ~CLaytonDragListCtrl ();

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
	//{{AFX_MSG(CLaytonDragListCtrl)
	afx_msg void OnBegindrag(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

#endif
