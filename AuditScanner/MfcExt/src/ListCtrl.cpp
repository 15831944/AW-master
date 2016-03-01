
// FILE:	ListCtrl.cpp
// PURPOSE:	Implementation of GP sub-classed List Controls
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 29.03.2001 - written

#include "stdafx.h"
#include <math.h>

#define new DEBUG_NEW

// CSortListItem Dynamic creation declaration
#define MAX_COLS	64
const int CSortHeaderCtrl::NOT_SORTED = -99;
const int CSortListCtrl::NOT_SORTED = CSortHeaderCtrl::NOT_SORTED;

///////////////////////////////////////////////////////////////////////////////
//
// Helper class for in-place editing

IMPLEMENT_DYNAMIC(CListCtrlComboBox,CComboBox)

CListCtrlComboBox::CListCtrlComboBox()
{
}

CListCtrlComboBox::~CListCtrlComboBox()
{
}


BEGIN_MESSAGE_MAP(CListCtrlComboBox, CComboBox)
	//{{AFX_MSG_MAP(CEditListCtrlComboBox)
	ON_WM_KILLFOCUS()
	ON_CONTROL_REFLECT(CBN_SELENDOK, OnSelendok)
	ON_CONTROL_REFLECT(CBN_SELENDCANCEL, OnSelendcancel)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/*
** Kill the popup when it loses focus
*/
void CListCtrlComboBox::OnKillFocus(CWnd* pNewWnd) 
{
	CComboBox::OnKillFocus(pNewWnd);
	
	// then kill this window
	DestroyWindow ();
}

/*
** Update the parent when an item changes
*/
void CListCtrlComboBox::OnSelendok() 
{
	TRACE ("CListCtrlComboBox::OnSelendok()\n");
	// update the parent "cell" with the current listbox contents
	CWnd * pParent = GetParent();
	ASSERT(pParent->IsKindOf(RUNTIME_CLASS(CListCtrlEx)));
	CListCtrlEx * pListCtrl = (CListCtrlEx*)pParent;

	if (pListCtrl->GetParent()->SendMessage (WM_LVN_COMBOEDITCHANGE, pListCtrl->GetDlgCtrlID(), (LPARAM)this))
	{
		// parent wants us to update the list cell with the changed text
		CString strText;
		GetWindowText (strText);
		// and notify the parent (and via derived class)
		pListCtrl->SetItemText (m_nRow, m_nCol, strText);
//		pListCtrl->AutoSizeCols(TRUE);
		// then shift the focus back to it
		pListCtrl->SetFocus();
	}
}

void CListCtrlComboBox::OnSelendcancel()
{
	TRACE ("CListCtrlComboBox::OnSelendcancel()\n");
	
	// get hold of the parent list control
	CWnd * pParent = GetParent();
	ASSERT(pParent->IsKindOf(RUNTIME_CLASS(CListCtrlEx)));
	CListCtrlEx * pListCtrl = (CListCtrlEx*)pParent;
	// and push the focus back to it, which deletes this control
	pListCtrl->SetFocus();
}

///////////////////////////////////////////////////////////////////////////////
//
// The main extended list control class

IMPLEMENT_DYNAMIC(CListCtrlEx,CListCtrl)

BEGIN_MESSAGE_MAP(CListCtrlEx, CListCtrl)
	//{{AFX_MSG_MAP(CEditListCtrl)
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

CListCtrlEx::CListCtrlEx()
{
	m_nColCount = 0;
}

/*
** Returns first or only selected item or -1 if none selected
*/
int CListCtrlEx::GetSelectedItem()
{
	if (0 == GetItemCount() || 0 == GetSelectedCount())
		return -1;
	// there is at least one item selected - look for first one
	for (int nItem = 0 ; nItem < GetItemCount() ; nItem++)
	{
		// is this item selected?
		if (LVIS_SELECTED & GetItemState(nItem, LVIS_SELECTED))
			return nItem;
	}
	// should never get here!
	ASSERT(FALSE);
	return -1;
}

/*
** Select a single item in the list, and reset any other selections
*/
void CListCtrlEx::SelectItem (int nItem)
{
	for (int n = 0 ; n < GetItemCount() ; n++)
	{
		if (n == nItem)
			SetItemState(n, LVIS_SELECTED, LVIS_SELECTED);
		else
			SetItemState(n, 0, LVIS_SELECTED);
	}
}

void CListCtrlEx::AutoSizeCols(BOOL bUseHeaders/* = TRUE*/)
{
	// if no rows in the list use headers anyway
	if (0 == GetItemCount())
		bUseHeaders = TRUE;
	// loop through the columns
	for (int nCol = 0 ; nCol < m_nColCount ; nCol++)
	{
		if (bUseHeaders)
			SetColumnWidth(nCol, LVSCW_AUTOSIZE_USEHEADER);
		else
			SetColumnWidth(nCol, LVSCW_AUTOSIZE);
	}
}

int CListCtrlEx::GetItemImage (int nItem) const
{
	// get the item data...
	LV_ITEM item;
	item.iItem		= nItem;
	item.iSubItem	= 0;
	item.mask		= LVIF_IMAGE;
	if (!GetItem(&item))
		return -1;
	return item.iImage;
}

void CListCtrlEx::SetItemImage (int nItem, int nIcon)
{
	LV_ITEM item;
	item.iItem	= nItem;
	item.iSubItem	= 0;
	item.mask	= LVIF_IMAGE;
	item.iImage	= nIcon;
	SetItem(&item);
}

/*
** These overrides are simply to maintain an accurate column count
*/
int CListCtrlEx::InsertColumn(int nCol, const LVCOLUMN* pColumn, eColType colType/* = colTypeNormal*/)
{
	m_nColCount++;
	m_colTypes.Add(colType);
	return CListCtrl::InsertColumn(nCol, pColumn);
}

int CListCtrlEx::InsertColumn(int nCol, LPCTSTR lpszColumnHeading,	int nFormat/* = LVCFMT_LEFT*/, int nWidth/* = -1*/, int nSubItem/* = -1*/, eColType colType/* = colTypeNormal*/)
{
	m_nColCount++;
	m_colTypes.Add(colType);
	return CListCtrl::InsertColumn(nCol, lpszColumnHeading,	nFormat, nWidth, nSubItem);
}

BOOL CListCtrlEx::DeleteColumn(int nCol)
{
	m_nColCount--;
	m_colTypes.Remove (nCol);
	return CListCtrl::DeleteColumn(nCol);
}

void CListCtrlEx::DeleteAllCols()
{
	while (m_nColCount)
		DeleteColumn(0);
}

/*
** Writes an integer value as a text string
*/
BOOL CListCtrlEx::SetItemInt (int nItem, int nSubItem, int nData)
{
	CString strBuffer;
	strBuffer.Format ("%d", nData);
	return SetItemText (nItem, nSubItem, strBuffer);
}

/*
** write a hex value as a text string in 0xHHHH format
*/
BOOL CListCtrlEx::SetItemHex (int nItem, int nSubItem, DWORD dwData)
{
	CString strBuffer;
	strBuffer.Format ("0x%8.8X", dwData);
	return SetItemText (nItem, nSubItem, strBuffer);
}

/*
** write a boolean value as a text string either "yes" or "no"
*/
BOOL CListCtrlEx::SetItemBool (int nItem, int nSubItem, BOOL bData)
{
	return SetItemText (nItem, nSubItem, bData ? "yes" : "no");
}

BOOL CListCtrlEx::Export(LPCSTR pszTitle ,char chSep/* = ','*/, BOOL bSelectedOnly/* = FALSE*/)
{
	// get file name and type from user
	char szFilter[] = "Comma separated file (*.csv)|*.csv|Text Files (*.txt)|*.txt||";
	CFileDialog fd(FALSE, "csv", NULL, OFN_OVERWRITEPROMPT, szFilter, this); 
	if (IDOK == fd.DoModal())
	{
		CWaitCursor wc;
		
		// retrieve file name, type and open
		CString strPath = fd.GetPathName();
		CString strExt = fd.GetFileExt();

		// how we write the data depends on the chosen extension type
		if (!strExt.CompareNoCase("txt"))
			return ExportText(strPath, pszTitle, bSelectedOnly);
		
		else if (!strExt.CompareNoCase("csv"))
			return ExportCSV(strPath, pszTitle ,chSep, bSelectedOnly);
	}
	return FALSE;
}

BOOL CListCtrlEx::Print (LPCSTR pszTitle/* = NULL*/, BOOL bSelectedOnly/* = FALSE*/)
{
	// generate a temporary filename
	char szTempPath[_MAX_PATH], szTempFile[_MAX_PATH], szDrive[_MAX_DRIVE], szDir[_MAX_DIR], szFName[_MAX_FNAME];

	::GetTempPath (sizeof(szTempPath), szTempPath);
	::GetTempFileName (szTempPath, "listctrl_", 0, szTempFile);

	// give it a "txt" extension
	_splitpath (szTempFile, szDrive, szDir, szFName, NULL);
	wsprintf (szTempFile, "%s%s%s.txt", szDrive, szDir, szFName);

	// create the file
	if (ExportText (szTempFile, pszTitle, bSelectedOnly))
	{
		// print it
		HINSTANCE hResult = ShellExecute (GetSafeHwnd(), "print", szTempFile, NULL, NULL, 0);
		if (hResult > (HINSTANCE)32)
			return TRUE;
		else
			HandleWin32Error ();

		CFile::Remove (szTempFile);
	}
	return FALSE;
}

BOOL CListCtrlEx::ExportText(LPCSTR pszFileName, LPCSTR pszTitle, BOOL bSelectedOnly/* = FALSE*/)
{
	int nCol;

	// open the file
	try
	{
		CString strBuffer, strField;
		CStdioFile file(pszFileName, CFile::modeCreate | CFile::modeWrite);
		// write the title
		strBuffer.Format("%s\n\n", pszTitle);
		file.WriteString(strBuffer);
		// now loop through the columns and decide longest value for each
		if (m_nColCount)
		{
			LPINT pColWidths = new int[m_nColCount];
			for (nCol = 0 ; nCol < m_nColCount ; nCol++)
			{
				// use column header text for initial value
				char szColName[1024];
				LVCOLUMN lvc;
				lvc.mask = LVCF_TEXT;
				lvc.pszText = szColName;
				lvc.cchTextMax = 1024;
				GetColumn(nCol, &lvc);
				pColWidths[nCol] = strlen(szColName);
				// then loop through the data
				for (int nRow = 0 ; nRow < GetItemCount() ; nRow++)
				{
					if (bSelectedOnly && (!IsItemSelected(nRow)))
						continue;

					// retrieve the text in this column
					CString strItem = GetItemText(nRow, nCol);
					// use highest length of the two
					int nLen = strItem.GetLength();
					pColWidths[nCol] = max(pColWidths[nCol], nLen);
				}
				// add a space on
				pColWidths[nCol]++;
			}

			// now build a line of text for the headers
			strBuffer.Empty();
			for (nCol = 0 ; nCol < m_nColCount ; nCol++)
			{
				// retrieve the column heading 
				char szColName[1024];
				LVCOLUMN lvc;
				lvc.mask = LVCF_TEXT;
				lvc.pszText = szColName;
				lvc.cchTextMax = 1024;
				GetColumn(nCol, &lvc);
				// build a string padded with the correct number of spaces
				CString strFormat;
				strFormat.Format("%%-%ds", pColWidths[nCol]);
				strField.Format(strFormat, szColName);
				// append to the buffer
				strBuffer += strField;
			}
			// dump buffer to file
			strBuffer += '\n';
			file.WriteString(strBuffer);

			// now loop through the actual data
			for (int nRow = 0 ; nRow < GetItemCount() ; nRow++)
			{
				if (bSelectedOnly && (!IsItemSelected(nRow)))
					continue;

				strBuffer.Empty();
				// run through the columns
				for (int nCol = 0 ; nCol < m_nColCount ; nCol++)
				{
					// build the correct width format statement
					CString strFormat;
					strFormat.Format("%%-%ds", pColWidths[nCol]);
					// write the field and append to buffer
					strField.Format(strFormat, GetItemText(nRow, nCol));
					strBuffer += strField;
				}
				// write this line to file
				strBuffer += '\n';
				file.WriteString(strBuffer);
			}

			// finally write the row count to the end of the file
			strBuffer.Format("\n%d item(s) listed", GetItemCount());
			file.WriteString(strBuffer);

			delete [] pColWidths;
		}
	}
	catch (CFileException * pE)
	{
		pE->Delete();
		return FALSE;
	}
	return TRUE;
}

BOOL CListCtrlEx::ExportCSV (LPCSTR pszFileName, LPCSTR pszTitle ,char chSep, BOOL bSelectedOnly/* = FALSE*/)
{
	char chQuote = '\"';

	// open the file
	try
	{
		CString strBuffer;
		CStdioFile file(pszFileName, CFile::modeCreate | CFile::modeWrite);
		
		// write the title
		strBuffer.Format("%s\n\n", pszTitle);
		file.WriteString(strBuffer);

		// write the column headings
		for (int nCol = 0 ; nCol < m_nColCount ; nCol++)
		{
			// retrieve the column heading 
			char szColName[1024];
			LVCOLUMN lvc;
			lvc.mask = LVCF_TEXT;
			lvc.pszText = szColName;
			lvc.cchTextMax = 1024;
			GetColumn(nCol, &lvc);
		
			// add to buffer
			if (nCol)
			{
				strBuffer += chSep;
				strBuffer += szColName;
			} 
			else
				strBuffer = szColName;
		}

		// dump to file
		strBuffer += '\n';
		file.WriteString(strBuffer);
	
		// write the actual data
		for (int nRow = 0; nRow < GetItemCount(); nRow++)
		{
			if (bSelectedOnly && !IsItemSelected(nRow))
				continue;

			strBuffer.Empty();			// Clear buffer
			for (int nCol = 0; nCol < m_nColCount; nCol++)
			{
				if (nCol)
					strBuffer += chSep;

				CString strField = GetItemText(nRow, nCol);
			
				// if a separator is contained in the string then surround with quotes
				if (-1 != strField.Find(chSep))
					strField = chQuote + strField + chQuote;
				strBuffer += strField;
			}

			// dump this row
			strBuffer += '\n';
			file.WriteString(strBuffer);
		}

		// finally the item count
		strBuffer.Format("%d item(s)", GetItemCount());
		file.WriteString(strBuffer);		
	}
	catch (CFileException * pE)
	{
		pE->Delete();
		return FALSE;
	}
	return TRUE;
}

/*
** Return name of a column
*/
CString CListCtrlEx::GetColName (int nCol) const
{
	ASSERT(nCol < m_nColCount);

	char szColName[1024];
	LVCOLUMN lvc;
	lvc.mask	= LVCF_TEXT;
	lvc.pszText	= szColName;
	lvc.cchTextMax	= 1024;
	GetColumn(nCol, &lvc);
	return CString(szColName);
}

//
//  Return a list of the headings for this control
//
void	CListCtrlEx::GetColumnHeadings	(CDynaList<CString>& listHeadings)
{
	listHeadings.Empty();

	for (int nCol = 0 ; nCol < m_nColCount ; nCol++)
		listHeadings.Add(GetColName(nCol));
}

/*
** return a list of current column widths
*/
DWORD CListCtrlEx::GetColumnWidths (CDynaList<int> & widths)
{
	widths.Empty();
	for (int n = 0 ; n < m_nColCount ; n++)
		widths.Add (GetColumnWidth (n));
	return m_nColCount;
}

/*
** set all column widths
*/
void CListCtrlEx::SetColumnWidths (CDynaList<int> const & widths)
{
	ASSERT(widths.GetCount() == (DWORD)m_nColCount);
	for (int nCol = 0 ; nCol < m_nColCount ; nCol++)
		SetColumnWidth (nCol, widths[nCol]);
}

//
//    Get the data for a row in the list
//
void	CListCtrlEx::GetRow	(int nRow ,CDynaList<CString>& listData)
{
	listData.Empty();

	for (int nCol = 0; nCol < m_nColCount; nCol++)
		listData.Add(GetItemText(nRow, nCol));
}

/*
** override to set the type of a column, for in-place editing
*/
void CListCtrlEx::SetColType (int nCol, eColType type)
{
	ASSERT(nCol < m_nColCount);
	m_colTypes[nCol] = type;
}

/*
** called when data changes. Default behaviour is simply to update the list control
*/
void CListCtrlEx::OnEditComboChanged (int nRow, int nCol, LPCSTR pszNewValue)
{
	SetItemText (nRow, nCol, pszNewValue);
}

/////////////////////////////////////////////////////////////////////////////
// CListCtrlEx message handlers

/*
** Implements in-place combo editing
*/
void CListCtrlEx::OnLButtonDown (UINT nFlags, CPoint point) 
{
	TRACE("CListCtrlEx::OnLButtonDown(%d,[%d,%d])\n", nFlags, point.x, point.y);

	// find first and last visible items
	int nFirst = GetTopIndex();
	int nLast = min(nFirst + GetCountPerPage(), GetItemCount());

	for (int nItem = nFirst ; nItem < nLast ; nItem++)
	{
		// get the bounding rectangle for the item
		CRect rcItem;
		GetItemRect (nItem, rcItem, LVIR_BOUNDS);

		if (rcItem.PtInRect(point))
		{
			SetFocus();
			// now we need to decide which column we're in
			int nLeft = 0;
			for (int nCol = 0 ; nCol < GetColCount() ; nCol++)
			{
				int nRight = nLeft + GetColumnWidth(nCol);
				if (point.x >= nLeft && point.x < nRight)
				{
					// is in-place editing enabled?
					if (m_colTypes[nCol] == colTypeCombo)
					{
						m_cb.m_nRow = nItem;
						m_cb.m_nCol = nCol;

						// Got the column - create the combo control
						CRect rcCell (nLeft, rcItem.top, nRight, rcItem.bottom + 100);
						m_cb.Create (WS_CHILD|WS_BORDER|WS_VISIBLE|CBS_DROPDOWNLIST|WS_VSCROLL|CBS_SORT, rcCell, this, 1);
						// set the font
						HFONT hFont = (HFONT)SendMessage (WM_GETFONT, 0, 0);
						m_cb.SendMessage (WM_SETFONT, (WPARAM)hFont, 0);

						// call the parent to populate it
						if (GetParent()->SendMessage (WM_LVN_BEGINCOMBOEDIT, this->GetDlgCtrlID(), (LPARAM)&m_cb))
						{
							// parent wants to go ahead, so first try and select the current item
							CString strItemText = GetItemText (nItem, nCol);
							m_cb.SelectString (-1, strItemText);
							// show it, and hand it the focus
							m_cb.ShowWindow (SW_SHOWNORMAL);
							m_cb.ShowDropDown();							
							m_cb.SetFocus();
						}
						else
						{
							m_cb.DestroyWindow();
						}
						return;
					}
				}
				nLeft = nRight;
			}
//			SelectItem (nItem);
		}
	}
	CListCtrl::OnLButtonDown(nFlags, point);
}



///////////////////////////////////////////////////////////////////////////////
// 
// DERIVED CLASS TO IMPLEMENT DRAG'N'DROP

IMPLEMENT_DYNAMIC(CDragListCtrl,CListCtrlEx)

CDragListCtrl::CDragListCtrl ()
{
	m_bDragging = FALSE;
}

BEGIN_MESSAGE_MAP(CDragListCtrl, CListCtrlEx)
	//{{AFX_MSG_MAP(CDragListCtrl)
	ON_NOTIFY_REFLECT(LVN_BEGINDRAG, OnBegindrag)
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/*
** Extends the win32 API version in order to allow multiple selection
*/
CImageList * CDragListCtrl::CreateDragImageEx (CPoint * pPoint)
{
	HIMAGELIST hDragImage = NULL;
	int cyOffset = 0;

	for (int n = 0 ; n < GetItemCount() ; n++)
	{
		if (IsItemSelected(n))
		{			
			// first item ?
			if (!hDragImage)
			{
				hDragImage = ListView_CreateDragImage (m_hWnd, n, pPoint);
			}
			else
			{
				// append subsequent items to the existing image list
				CPoint ptTemp;
				HIMAGELIST hTempImage = ListView_CreateDragImage (m_hWnd, n, &ptTemp);
				HIMAGELIST hMergeImage = ImageList_Merge (hDragImage, 0, hTempImage, 0, 0, cyOffset);
				// the merged image becomes the new drag image
				ImageList_Destroy (hDragImage);
				ImageList_Destroy (hTempImage);
				hDragImage = hMergeImage;
			}
			// get current image list height, for next iteration
			IMAGEINFO info;
			ImageList_GetImageInfo (hDragImage, 0, &info);
			cyOffset = info.rcImage.bottom;
		}
	}

	CImageList * pDragImage = new CImageList;
	pDragImage->Attach (hDragImage);
	return pDragImage;
}

void CDragListCtrl::OnBegindrag(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW * pNMListView = (NM_LISTVIEW*)pNMHDR;

	int nItem = pNMListView->iItem;

	TRACE("Beginning to drag item %d\n", nItem);
	TRACE("Drag Point (%d,%d)\n", pNMListView->ptAction.x, pNMListView->ptAction.y);

	// ensure that (at least) the drag item is selected
	SetFocus();
	SetItemState(nItem, LVIS_SELECTED, LVIS_SELECTED);

	// ask the derived class whether we can drag the selected items
	if (!CanDragItems(nItem))
		return;

	// create the drag image from the list view selection
	CPoint ptTopLeft;
	m_pDragImage = CreateDragImageEx (&ptTopLeft);

	// add all the items to our "drop-list", and de-select them in the view
	m_dropList.Empty();
	for (int n = 0 ; n < GetItemCount() ; n++)
	{
		if (IsItemSelected(n))
			m_dropList.Add(n);
	}

	SetCapture ();
	m_bDragging = TRUE;

	m_pDropWnd = NULL;
	m_dwDropItem = (DWORD)-1;

	// Calculate the "hot-spot" offset, ie cursor location relative to top-left corner of drag image, then start the drag off
	CPoint pt(pNMListView->ptAction);
	CPoint ptHotSpot = pt - ptTopLeft;
	m_pDragImage->BeginDrag (0, ptHotSpot);

	// start the drag relative to the parent window, then it will stay active over the sibling tree view
	CWnd * pParent = GetParent();
	MapWindowPoints (pParent, &pt, 1);
	m_pDragImage->DragEnter (pParent, pt);

	*pResult = 0;
}

void CDragListCtrl::OnMouseMove (UINT nFlags, CPoint point)
{
	if (m_bDragging)
	{
		// move the image
		CPoint ptImage(point);
		MapWindowPoints(GetParent(), &ptImage, 1);
		m_pDragImage->DragMove(ptImage);

		// now allow window updates
		m_pDragImage->DragShowNolock(FALSE);

		// which window are we over ?
		CPoint pt(point);
		ClientToScreen(&pt);
		CWnd * pWnd = WindowFromPoint (pt);

		// are we moving to a different window ?
		if (pWnd != m_pDropWnd)
		{
			// yes - reset any selection in the previous window
			if (m_pDropWnd)
			{
				if (m_pDropWnd == this)
				{
					SetItemState ((int)m_dwDropItem, 0, LVIS_DROPHILITED);
					RedrawItems ((int)m_dwDropItem, (int)m_dwDropItem);
					UpdateWindow ();
				}
				else if (m_pDropWnd->IsKindOf(RUNTIME_CLASS(CDragTreeCtrl)))
				{
					((CDragTreeCtrl*)m_pDropWnd)->SelectDropTarget(NULL);
				}
			}
			m_pDropWnd = NULL;

			// initialise depending on newly selected window
			if (pWnd == this)
				m_dwDropItem = (DWORD)-1;
			else
				m_dwDropItem = NULL;
		}

		// are we moving over this window ?
		if (this == pWnd)
		{
			// look for a valid item to drop on
			int nDropItem = HitTest (point);

			if (nDropItem != (int)m_dwDropItem)
			{
				// remove the highlighting from the old drop item
				if (-1 != (int)m_dwDropItem)
				{
					SetItemState ((int)m_dwDropItem, 0, LVIS_DROPHILITED);
					RedrawItems ((int)m_dwDropItem, (int)m_dwDropItem);
					UpdateWindow ();
				}

				// is the new one allowed ?
				if (CanDropHere(nDropItem))
				{
					m_dwDropItem = (DWORD)nDropItem;
					SetItemState (nDropItem, LVIS_DROPHILITED, LVIS_DROPHILITED);
					RedrawItems (nDropItem, nDropItem);
					UpdateWindow ();
					::SetCursor ((HCURSOR)::GetClassLong(m_hWnd, GCL_HCURSOR));
				}
				else
				{
					m_dwDropItem = (DWORD)-1;
					::SetCursor (AfxGetApp()->LoadStandardCursor(IDC_NO));
				}
			}
			m_pDropWnd = pWnd;
		}
		// no - are we over an associated tree view ?
		else if (pWnd->IsKindOf(RUNTIME_CLASS(CDragTreeCtrl)))
		{
			CDragTreeCtrl * pTV = (CDragTreeCtrl*)pWnd;
			CPoint ptTV(point);
			MapWindowPoints(pWnd, &ptTV, 1);
			UINT nFlags;
			HTREEITEM hTarget = pTV->HitTest (ptTV, &nFlags);

			if (hTarget != (HTREEITEM)m_dwDropItem)
			{
				pTV->SelectDropTarget(hTarget);
				m_dwDropItem = (DWORD)hTarget;

				if (pTV->CanDropHere(hTarget))
					::SetCursor ((HCURSOR)::GetClassLong(m_hWnd, GCL_HCURSOR));
				else
					::SetCursor (AfxGetApp()->LoadStandardCursor(IDC_NO));
			}
			m_pDropWnd = pWnd;
		}

		m_pDragImage->DragShowNolock(TRUE);

	}
	else
	{
		CListCtrlEx::OnMouseMove(nFlags, point);
	}
}

void CDragListCtrl::OnLButtonUp (UINT nFlags, CPoint point)
{
	if (m_bDragging)
	{
		m_pDragImage->DragLeave(this);
		m_pDragImage->EndDrag ();

		ReleaseCapture();
		m_bDragging = FALSE;
		delete m_pDragImage;

		// do we want to execute the drop ?
		if (this == m_pDropWnd)
		{
			if (-1 != (int)m_dwDropItem)
			{
				// clean up the highlight
				SetItemState ((int)m_dwDropItem, 0, LVIS_DROPHILITED);

				// yes - loop through all the items BACKWARDS, dropping each one
				for (DWORD dw = m_dropList.GetCount() - 1 ; dw != (DWORD)-1 ; dw--)
				{
					// use the derived class fn to do the drop
					DropItem (m_dropList[dw], (int)m_dwDropItem);
				}
			}
		}
		else if (m_pDropWnd->IsKindOf(RUNTIME_CLASS(CDragTreeCtrl)))
		{
			// is the drop target valid ?
			CDragTreeCtrl * pTV = (CDragTreeCtrl*)m_pDropWnd;
			HTREEITEM hTarget = (HTREEITEM)m_dwDropItem;
			if (pTV->CanDropHere(hTarget))
			{
				// expand the target item...
				BOOL bDestExpanded = pTV->IsItemExpanded(hTarget);
				if (!bDestExpanded)
					pTV->Expand (hTarget, TVE_EXPAND);

				// loop BACKWARDS through each the selected items
				for (DWORD dw = m_dropList.GetCount() - 1 ; dw != (DWORD)-1 ; dw--)
				{
					// call member or derived class fn to execute the drop
					DropItemTV ((CDragTreeCtrl*)m_pDropWnd, hTarget, m_dropList[dw]);
				}
				// put the destination back as it was
				if (!bDestExpanded)
					pTV->Expand(hTarget, TVE_COLLAPSE);
				// and clean the tree up
				pTV->SelectDropTarget(NULL);
			}
		}
	}
	CListCtrlEx::OnLButtonUp(nFlags, point);
}

/*
** Called when user drops an item onto a valid node in an accompanying tree control
*/
void CDragListCtrl::DropItemTV (CDragTreeCtrl * pTV, HTREEITEM hTarget, int nItem)
{
	/*
	** Default behaviour is to search for the source item under the currently selected
	** tree item and move it to the destination using the DropItem() function in the
	** destination tree. But derived classes can implement different ideas...
	*/

	// Expand the source and destination tree item
	HTREEITEM hSrcParent = pTV->GetSelectedItem();
	BOOL bSrcExpanded = pTV->IsItemExpanded(hSrcParent);
	if (!bSrcExpanded)
		pTV->Expand (hSrcParent, TVE_EXPAND);

	// find the source item - it ought to be under the originally selected item
	HTREEITEM hItem = pTV->FindChild (hSrcParent, GetItemText(nItem, 0));
	if (hItem)
	{
		pTV->DropItem (hItem, hTarget);
		pTV->MoveItem (hItem, hTarget);
		DeleteItem (nItem);
	}
	
	// put the tree back as it was
	if (!bSrcExpanded)
		pTV->Expand(hSrcParent, TVE_COLLAPSE);
}

void CDragListCtrl::OnTimer(UINT nIDEvent) 
{
	CListCtrlEx::OnTimer(nIDEvent);
}

///////////////////////////////////////////////////////////////////////////////
//
//	C S o r t L i s t C t r l
//

IMPLEMENT_DYNAMIC(CSortListCtrl,CDragListCtrl)

CSortListCtrl::CSortListCtrl ()
{
	CString strNull("");
	// Initialize column unit strings to empty
	for (DWORD dw=0; dw<MAX_COLS; dw++)
		m_listColumnUnits.Add(strNull);

    m_nHighlightType = HIGHLIGHT_ALLCOLUMNS;
    m_crForegroundColor = 0;
	m_eBackground = BACK_NORMAL;
	m_bSetup = FALSE;
}


CSortListCtrl::~CSortListCtrl ()
{
	// Free up the dynalist of data
	for (DWORD dw=0; dw<m_listData.GetCount(); dw++)
	{
		CSortListRow* pItem = m_listData[dw];
		delete pItem;
	}

	m_listData.Empty(); 
	m_listColumnUnits.Empty(); 
}


//
//    Message Map for this control
//
BEGIN_MESSAGE_MAP(CSortListCtrl, CDragListCtrl)
	//{{AFX_MSG_MAP(CSortListCtrl)
	ON_WM_PAINT()
	ON_WM_KILLFOCUS()
	ON_WM_SETFOCUS()
	ON_WM_KEYUP()
	ON_WM_LBUTTONDOWN()
	ON_WM_KEYDOWN()
	ON_WM_CREATE()
//	ON_MESSAGE(LVM_SETIMAGELIST, OnSetImageList)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CAdminListCtrl message handlers

void CSortListCtrl::OnColumnclick(NMHDR* pNMHDR, LRESULT* pResult) 
{
	BOOL bSortAscending;

	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	// If clicked on already sorted column, reverse sort order
    if (pNMListView->iSubItem == GetSortedColumn()) 
	{
		bSortAscending = !IsSortedAscending();
	}
    else 
	{
        bSortAscending = TRUE;
	}
	
	SortColumn(pNMListView->iSubItem, bSortAscending);

	*pResult = 0;
}

void CSortListCtrl::SortColumn (int nCol, BOOL bAsc)
{
	// Have we subclassed the column header?  If not do so now
	SetHeader();

	// First set the sort order and column and draw the header
	// This must be called before SortItems to set sort order and column 
	if ((GetStyle() & LVS_TYPEMASK) == LVS_REPORT) 
		m_HeaderCtrl.SetSortImage(nCol, bAsc);

	// Now sort all items in column using CListCtrl sorting mechanism
	SortItems (SortCallback, (LPARAM)this);
}

void CSortListCtrl::ReSort (void)
{
    ASSERT(GetSortedColumn() != NOT_SORTED);
    SortColumn(GetSortedColumn(), IsSortedAscending());
}
	

//
//    SetHeader
//    =========
//
//    Bit of a bodge here to subclass the header control at a point where the it
//    has been created
//
void CSortListCtrl::SetHeader(void)
{
	if (!m_bSetup)
	{
		CHeaderCtrl* pHeaderCtrl = GetHeaderCtrl();
		m_HeaderCtrl.SubclassWindow(pHeaderCtrl->m_hWnd);
		m_bSetup = TRUE;
	}
}



//
//    InsertItem (CString)
//    ==========
//
//    Insert an item initially into our array and then into the list
//
int	CSortListCtrl::InsertItem (int nItem, LPCSTR pData, int nImage ,int nItemData/*=-1*/)
{
	CSortListItem* pItem = new CSortListItemString(pData);

	CString	strDisplayValue;
	if (m_listColumnUnits[0].IsEmpty())
		strDisplayValue = pData;
	else
		strDisplayValue.Format("%s %s",pData ,m_listColumnUnits[0]);

	// Create a new list row object and add it to our list in the required position 
	CSortListRow* pNewRow = new CSortListRow;
	m_listData.AddAt(pNewRow ,nItem);

	// ...add the new column to the row
	pNewRow->Add(pItem);
	
	// Insert the data and set the base class data value to be a pointer to the row object
	nItem = CDragListCtrl::InsertItem(nItem ,strDisplayValue ,nImage);
	CDragListCtrl::SetItemData(nItem ,(unsigned long)pNewRow);

	// If we were passed in a data value then set this
	if (nItemData != -1)
		SetItemData(nItem ,nItemData);

	return nItem;
}

//
//    InsertItem (INT)
//    ==========
//
//    Insert an item initially into our array and then into the list
//
int	CSortListCtrl::InsertItem (int nItem, int nValue, int nImage ,int nItemData/*=-1*/)
{
	CSortListItemInteger* pItem = new CSortListItemInteger(nValue);

	CString	strDisplayValue;
	strDisplayValue.Format("%d",nValue);

	if (!m_listColumnUnits[0].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[0];

	// Create a new list row object and add it to our list. 
	CSortListRow* pNewRow = new CSortListRow;
	m_listData.AddAt(pNewRow ,nItem);

	// ...add the new column to the row
	pNewRow->Add(pItem);
	
	// Insert the data and set the data value to be the index of the array entry
	nItem = CDragListCtrl::InsertItem(nItem ,strDisplayValue ,nImage);
	CDragListCtrl::SetItemData(nItem ,(unsigned long)pNewRow);

	// If we were passed in a data value then set this
	if (nItemData != -1)
		SetItemData(nItem ,nItemData);

	return nItem;
}

//
//    InsertItem (CTime)
//    ==========
//
//    Insert an item initially into our array and then into the list
//
int	CSortListCtrl::InsertItem (int nItem, CTime& ctValue, int nImage ,int nItemData/*=-1*/)
{
	CString	strDisplayValue;
	strDisplayValue = DateTimeToString(ctValue);

	if (!m_listColumnUnits[0].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[0];

	CSortListItemDate* pItem = new CSortListItemDate(ctValue);

	// Create a new list row object and add it to our list. 
	CSortListRow* pNewRow = new CSortListRow;
	m_listData.AddAt(pNewRow ,nItem);

	// ...add the new column to the row
	pNewRow->Add(pItem);
	
	// Insert the data and set the data value to be the index of the array entry
	nItem = CDragListCtrl::InsertItem(nItem ,strDisplayValue ,nImage);
	CDragListCtrl::SetItemData(nItem ,(unsigned long)pNewRow);

	// If we were passed in a data value then set this
	if (nItemData != -1)
		SetItemData(nItem ,nItemData);
	
	return nItem;
}


//
//    InsertItem (CTimeSpan)
//    ==========
//
//    Insert an item initially into our array and then into the list
//
int	CSortListCtrl::InsertItem (int nItem, CTimeSpan& tsValue, int nImage ,int nItemData/*=-1*/)
{
	CSortListItem* pItem = new CSortListItemInterval(tsValue);

	// Create a new list row object and add it to our list. 
	CSortListRow* pNewRow = new CSortListRow;
	m_listData.AddAt(pNewRow ,nItem);

	// ...add the new column to the row
	pNewRow->Add(pItem);

	// Get the display value
	CString strDisplayValue;
	if (tsValue.GetDays() != 0)
		strDisplayValue = tsValue.Format("%D days %H:%M:%S");
	else 
		strDisplayValue = tsValue.Format("%H:%M:%S"); 

	if (!m_listColumnUnits[0].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[0];

	// Insert the data and set the data value to be the index of the array entry
	nItem = CDragListCtrl::InsertItem(nItem ,strDisplayValue ,nImage);
	CDragListCtrl::SetItemData(nItem ,(unsigned long)pNewRow);

	// If we were passed in a data value then set this
	if (nItemData != -1)
		SetItemData(nItem ,nItemData);

	return nItem;
}

//
//    InsertItem (IPAddress)
//    ==========
//
//    Insert an item initially into our array and then into the list
//
int	CSortListCtrl::InsertItem (int nItem, CIPAddress& IPAddress, int nImage ,int nItemData/*=-1*/)
{
	CString	strDisplayValue;

	CSortListItemIP* pItem = new CSortListItemIP(IPAddress);
	strDisplayValue = IPAddress;
	if (!m_listColumnUnits[0].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[0];

	// Create a new list row object and add it to our list. 
	CSortListRow* pNewRow = new CSortListRow;
	m_listData.AddAt(pNewRow ,nItem);

	// ...add the new column to the row
	pNewRow->Add(pItem);
	
	// Insert the data and set the data value to be the index of the array entry
	nItem = CDragListCtrl::InsertItem(nItem ,strDisplayValue ,nImage);
	CDragListCtrl::SetItemData(nItem ,(unsigned long)pNewRow);

	// If we were passed in a data value then set this
	if (nItemData != -1)
		SetItemData(nItem ,nItemData);

	return nItem;
}


//
//    SetItem (String)
//    =======
//
//    Set the value for a sub-item
//
BOOL CSortListCtrl::SetItem	(int nItem, int nSubItem, UINT nMask, LPCSTR pValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam)
{
	// Do we have this column already?  If so set its value otherwise create
	// a new column storage object first
	CSortListItemString* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemString(pValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliString);
		pItem = (CSortListItemString*) (*pRow)[nSubItem];
		pItem->m_strData = pValue;
	}

	CString	strDisplayValue;
	if (m_listColumnUnits[nSubItem].IsEmpty())
		strDisplayValue = pValue;
	else
		strDisplayValue.Format("%s %s",pValue ,m_listColumnUnits[nSubItem]);

	return CDragListCtrl::SetItem(nItem ,nSubItem ,nMask ,strDisplayValue ,nImage, nState, nStateMask, lParam);
}


//
//    SetItemText (String)
//    ===========
//
//    Set the displayed value for a list column
//
BOOL	CSortListCtrl::SetItemText (int nItem, int nSubItem, LPCSTR pValue)
{
	CSortListItemString* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemString(pValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliString);
		pItem = (CSortListItemString*) (*pRow)[nSubItem];
		pItem->m_strData = pValue;
	}

	CString	strDisplayValue;
	if (m_listColumnUnits[nSubItem].IsEmpty())
		strDisplayValue = pValue;
	else
		strDisplayValue.Format("%s %s",pValue ,m_listColumnUnits[nSubItem]);

	return CDragListCtrl::SetItemText(nItem ,nSubItem ,strDisplayValue);
}


//
//    SetItem (int)
//    =======
//
//    Set the value for a sub-item
//
BOOL CSortListCtrl::SetItem	(int nItem, int nSubItem, UINT nMask, int nValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam)
{
	// Do we have this column already?  If so set its value otherwise create
	// a new column storage object first
	CSortListItemInteger* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemInteger(nValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliInteger);
		pItem = (CSortListItemInteger*) (*pRow)[nSubItem];
		pItem->m_nData = nValue;
	}

	CString strDisplayValue;
	strDisplayValue.Format("%d" ,nValue);
	if (!m_listColumnUnits[nSubItem].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[nSubItem];


	return CDragListCtrl::SetItem(nItem ,nSubItem ,nMask ,strDisplayValue ,nImage, nState, nStateMask, lParam);
}


//
//    SetItemText (int)
//    ===========
//
//    Set the displayed value for a list column
//
BOOL	CSortListCtrl::SetItemText (int nItem, int nSubItem, int nValue)
{
	CSortListItemInteger* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemInteger(nValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliInteger);
		pItem = (CSortListItemInteger*) (*pRow)[nSubItem];
		pItem->m_nData = nValue;
	}
	
	CString strDisplayValue;
	strDisplayValue.Format("%d" ,nValue);
	if (!m_listColumnUnits[nSubItem].IsEmpty())
		strDisplayValue = strDisplayValue + " " + m_listColumnUnits[nSubItem];

	return CDragListCtrl::SetItemText(nItem ,nSubItem ,strDisplayValue);
}


//
//    SetItem (CTime)
//    =======
//
//    Set the value for a sub-item
//
BOOL CSortListCtrl::SetItem	(int nItem, int nSubItem, UINT nMask, CTime& ctValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam)
{
	// Do we have this column already?  If so set its value otherwise create
	// a new column storage object first
	CSortListItemDate* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemDate(ctValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliDate);
		pItem = (CSortListItemDate*) (*pRow)[nSubItem];
		pItem->m_ctData = ctValue;
	}

	CString strDisplayValue;
	strDisplayValue = DateTimeToString(ctValue);

	return CDragListCtrl::SetItem(nItem ,nSubItem ,nMask ,strDisplayValue ,nImage, nState, nStateMask, lParam);
}


//
//    SetItemText (CTime)
//    ===========
//
//    Set the displayed value for a list column
//
BOOL	CSortListCtrl::SetItemText (int nItem, int nSubItem, CTime& ctValue)
{
	CSortListItemDate* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemDate(ctValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliDate);
		pItem = (CSortListItemDate*) (*pRow)[nSubItem];
		pItem->m_ctData = ctValue;
	}
	
	CString strDisplayValue;
	strDisplayValue = DateTimeToString(ctValue);

	return CDragListCtrl::SetItemText(nItem ,nSubItem ,strDisplayValue);
}



//
//    SetItem (CTimeSpan)
//    =======
//
//    Set the value for a sub-item
//
BOOL CSortListCtrl::SetItem	(int nItem, int nSubItem, UINT nMask, CTimeSpan& tsValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam)
{
	// Do we have this column already?  If so set its value otherwise create
	// a new column storage object first
	CSortListItemInterval* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemInterval(tsValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliInterval);
		pItem = (CSortListItemInterval*) (*pRow)[nSubItem];
		pItem->m_tsData = tsValue;
	}

	CString strDisplayValue;
	if (tsValue.GetDays() != 0)
		strDisplayValue = tsValue.Format("%D days %H:%M:%S");
	else 
		strDisplayValue = tsValue.Format("%H:%M:%S"); 

	return CDragListCtrl::SetItem(nItem ,nSubItem ,nMask ,strDisplayValue ,nImage, nState, nStateMask, lParam);
}


//
//    SetItemText (CTimeSpan)
//    ===========
//
//    Set the displayed value for a list column
//
BOOL CSortListCtrl::SetItemText (int nItem, int nSubItem, CTimeSpan tsValue)
{
	CSortListItemInterval* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemInterval(tsValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliInterval);
		pItem = (CSortListItemInterval*) (*pRow)[nSubItem];
		pItem->m_tsData = tsValue;
	}

	return CDragListCtrl::SetItemText (nItem, nSubItem, FormatSeconds (tsValue.GetTotalSeconds()));
}


//
//    SetItem (CIPAddress)
//    =======
//
//    Set the value for a sub-item
//
BOOL CSortListCtrl::SetItem	(int nItem, int nSubItem, UINT nMask, CIPAddress& ipValue, int nImage, UINT nState, UINT nStateMask, LPARAM lParam)
{
	// Do we have this column already?  If so set its value otherwise create
	// a new column storage object first
	CSortListItemIP* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemIP(ipValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliIPAddress);
		pItem = (CSortListItemIP*) (*pRow)[nSubItem];
		pItem->m_ipData = ipValue;
	}

	CString strDisplayValue;
	strDisplayValue = ipValue;

	return CDragListCtrl::SetItem(nItem ,nSubItem ,nMask ,strDisplayValue ,nImage, nState, nStateMask, lParam);
}


//
//    SetItemText (CTime)
//    ===========
//
//    Set the displayed value for a list column
//
BOOL CSortListCtrl::SetItemText (int nItem, int nSubItem, CIPAddress& ipValue)
{
	CSortListItemIP* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pItem = new CSortListItemIP(ipValue);
		pRow->Add(pItem);
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliIPAddress);
		pItem = (CSortListItemIP*) (*pRow)[nSubItem];
		pItem->m_ipData = ipValue;
	}
	
	CString strDisplayValue;
	strDisplayValue = ipValue;

	return CDragListCtrl::SetItemText(nItem ,nSubItem ,strDisplayValue);
}

/*
** Set a list item that is a file size
*/
BOOL CSortListCtrl::SetItemSize (int nItem, int nSubItem, DWORD dwSize)
{
	CSortListItemInteger* pItem; 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pRow->Add (new CSortListItemInteger(dwSize));
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliInteger);
		pItem = (CSortListItemInteger*) (*pRow)[nSubItem];
		pItem->m_nData = dwSize;
	}
	
	return CDragListCtrl::SetItemText (nItem, nSubItem, FormatFileSize (dwSize));
}

/*
** Set an item to a Date (no time)
*/
BOOL CSortListCtrl::SetItemDate (int nItem, int nSubItem, CTime ctDate)
{
	CSortListItemDate * pItem; 
	CSortListRow * pRow = (CSortListRow *)CDragListCtrl::GetItemData(nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pRow->Add(new CSortListItemDate (ctDate));
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliDate);
		pItem = (CSortListItemDate*) (*pRow)[nSubItem];
		pItem->m_ctData = ctDate;
	}
	
	return CDragListCtrl::SetItemText (nItem, nSubItem, DateToString(ctDate));
}

/*
** Set an item to a Time (no date)
*/
BOOL CSortListCtrl::SetItemTime (int nItem, int nSubItem, CTime ctTime, BOOL bIncludeSeconds)
{
	CSortListItemDate * pItem; 
	CSortListRow * pRow = (CSortListRow *)CDragListCtrl::GetItemData (nItem);

	if ((DWORD)nSubItem >= pRow->GetCount())
	{
		pRow->Add (new CSortListItemDate (ctTime));
	}
	else
	{
		ASSERT ((*pRow)[nSubItem]->m_nType == sliDate);
		pItem = (CSortListItemDate*) (*pRow)[nSubItem];
		pItem->m_ctData = ctTime;
	}
	
	return CDragListCtrl::SetItemText (nItem, nSubItem, TimeToString (ctTime, bIncludeSeconds));
}



//
//    SetColumnUnits
//    ==============
//
//    Set a textual string to be added to each value displayed in this column to show
//    additional information such as the units for the column
//
void CSortListCtrl::SetColumnUnits (int nColumn, LPCSTR szUnits)
{
	ASSERT(nColumn<MAX_COLS);
	if (nColumn > MAX_COLS)
		return;
	m_listColumnUnits[nColumn] = szUnits;

}



//
//    SetItemData
//    ===========
//
//    Sets a 32-bit data value for the specified item.
//    Each item in the list control has its data value set to point to an associated
//    CSortListRow object.  This object stores, amongst other things the actual data 
//    value.
//
BOOL CSortListCtrl::SetItemData    (int nItem, DWORD dwData)
{ 
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);
	pRow->m_dwData = dwData; 
	return TRUE; 
}


//
//    GetItemData
//    ===========
//
//    Recover the associated data value for an item in the list
//
DWORD CSortListCtrl::GetItemData	(int nItem) const
{ 
	// The item data is actually stored in the CSortListRow object for this item
	// The item data holds the index into the list of CSortListRow's for this item
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);
	return pRow->m_dwData; 
}


//
//    DeleteItem
//    ===========
//
// Delete a single item
//
BOOL CSortListCtrl::DeleteItem	(int nItem)
{ 
	ASSERT ((DWORD)nItem < m_listData.GetCount()); 

	CSortListRow* pRowToDelete = (CSortListRow*)CDragListCtrl::GetItemData(nItem);

	// Find this entry in the DynaList
	for (DWORD dw=0; dw<m_listData.GetCount(); dw++)
	{
		if (m_listData[dw] == pRowToDelete)
		{
			m_listData.Remove(dw);
			delete pRowToDelete;
			break;
		}
	}

	// ...call base class to actually delete the item from the list control
	return CDragListCtrl::DeleteItem(nItem); 
} 



//
//    DeleteAllItems
//    ==============
//
//    Delete all items from the list
//
BOOL CSortListCtrl::DeleteAllItems	()
{ 
	for (DWORD dw=0; dw<m_listData.GetCount(); dw++)
	{
		delete m_listData[dw];
	}
	
	m_listData.Empty(); 
	return CDragListCtrl::DeleteAllItems(); 
} 



//
//    Sort Call back function
//
int CALLBACK CSortListCtrl::SortCallback (LPARAM lParam1, LPARAM lParam2, LPARAM lParam3)
{
	int nResult = 0;
	CSortListCtrl * pThis = (CSortListCtrl*)lParam3;

	// The lParam passed are the item data values for the two list items
	// which in turn are the indexes of the items in our internal array
	int nSortColumn = pThis->GetSortedColumn();

	CSortListRow* pRow1 = (CSortListRow*)lParam1;
	CSortListRow* pRow2 = (CSortListRow*)lParam2;
	CSortListItem* pFirstColumn = (*pRow1)[nSortColumn];
	CSortListItem* pSecondColumn = (*pRow2)[nSortColumn];

	nResult = pFirstColumn->Compare(pSecondColumn);
	
	if (!pThis->IsSortedAscending())
		nResult *= -1;
	return nResult;
}


//
//    Owner draw functions for the List control
//

// Unless LVS_SHOWSELALWAYS is set for list control, new selection is visible 
// only when the list control receives a focus
BOOL CSortListCtrl::SetCurSel(int row)
{
    return SetItemState(row,
        LVIS_SELECTED|LVIS_FOCUSED,LVIS_SELECTED|LVIS_FOCUSED);
}

void CSortListCtrl::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);
    CRect rcItem(lpDrawItemStruct->rcItem); 
    int nItem = lpDrawItemStruct->itemID;
    CImageList* pImageList; 
 
	// Save dc state 
    int nSavedDC = pDC->SaveDC();
    
    // Get item image and state info 
    LV_ITEM lvi;
    lvi.mask = LVIF_IMAGE | LVIF_STATE; 
    lvi.iItem = nItem; 
    lvi.iSubItem = 0;
    lvi.stateMask = 0xFFFF; 
    
	// get all state flags 
    GetItem(&lvi);
    
    // Should the item be highlighted
    BOOL bHighlight = ((lvi.state & LVIS_DROPHILITED) 
					|| ((lvi.state & LVIS_SELECTED) && ((GetFocus() == this) 
					|| (GetStyle() & LVS_SHOWSELALWAYS))));
    
    // Get rectangles for drawing 
    CRect rcBounds, rcLabel, rcIcon;
    GetItemRect(nItem, rcBounds, LVIR_BOUNDS);
    GetItemRect(nItem, rcLabel, LVIR_LABEL); 
    GetItemRect(nItem, rcIcon, LVIR_ICON);
    CRect rcCol(rcBounds);
    
    CString sLabel = GetItemText(nItem, 0);
    
	// Labels are offset by a certain amount 
    // This offset is related to the width of a space character
    int offset = pDC->GetTextExtent(_T(" "), 1).cx*2;
  
    CRect rcHighlight;
    CRect rcWnd; 
    int nExt; 
    
    switch (m_nHighlightType) 
	{ 
    case HIGHLIGHT_NORMAL: 
        nExt = pDC->GetOutputTextExtent(sLabel).cx + offset; 
        rcHighlight = rcLabel;
        if( rcLabel.left + nExt < rcLabel.right )
            rcHighlight.right = rcLabel.left + nExt; 
        break; 
    
	case HIGHLIGHT_ALLCOLUMNS:
        rcHighlight = rcBounds;
        rcHighlight.left = rcLabel.left;
        break; 
    
	case HIGHLIGHT_ROW:
        GetClientRect(&rcWnd);
        rcHighlight = rcBounds;
        rcHighlight.left = rcLabel.left;
        rcHighlight.right = rcWnd.right;
        
        break;
    
	default: 
        rcHighlight = rcLabel; 
    } 
    
    // Draw the background color
	if( bHighlight )
	{
		pDC->SetTextColor(::GetSysColor(COLOR_HIGHLIGHTTEXT));
		pDC->SetBkColor(::GetSysColor(COLOR_HIGHLIGHT));
		CBrush brHighlight(::GetSysColor(COLOR_HIGHLIGHT));

		pDC->FillRect(rcHighlight, &brHighlight);
	}
	
	else
	{
		CRect rcClient, rcRow = rcItem;
		GetClientRect(&rcClient);
		rcRow.right = rcClient.right;

		// Determine if we need to paint a special background color		
		CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nItem);
		if (pRow->m_crRowColor == 0)
		{
			CBrush brWindow(::GetSysColor (COLOR_WINDOW));
			pDC->FillRect(rcRow, &brWindow);
		}

		else
		{
			// Bodge here to prevent the background being set behind the icons 
			// ...Is there a better way to identify the size of the icons?
			int nLeftOffset = rcIcon.right - rcRow.left;
			rcRow.DeflateRect(nLeftOffset ,0 ,0 ,0);
			CBrush brRow(pRow->m_crRowColor);
			pDC->FillRect(rcRow, &brRow);
		}
	}

    // Set clip region 
    rcCol.right = rcCol.left + GetColumnWidth(0); 
    CRgn rgn;
    rgn.CreateRectRgnIndirect(&rcCol); 
    pDC->SelectClipRgn(&rgn);
    rgn.DeleteObject(); 
    
	// Draw column background
	if ((!bHighlight) && (m_eBackground != BACK_NORMAL))
	{
		CBrush brBack(m_crBackgroundColor);
		pDC->FillRect(rcCol, &brBack);
	}

    // Draw state icon 
    if (lvi.state & LVIS_STATEIMAGEMASK) 
	{
		int nImage = ((lvi.state & LVIS_STATEIMAGEMASK)>>12) - 1;
		pImageList = GetImageList(LVSIL_STATE); 
        if (pImageList) 
			pImageList->Draw(pDC, nImage,CPoint(rcCol.left, rcCol.top), ILD_TRANSPARENT);                
	} 

    // Draw normal and overlay icon
    pImageList = GetImageList(LVSIL_SMALL);
    if (pImageList) 
	{
		UINT nOvlImageMask = lvi.state & LVIS_OVERLAYMASK;
        pImageList->Draw(pDC,
		                lvi.iImage, CPoint(rcIcon.left, rcIcon.top),
					    (bHighlight?ILD_BLEND50:0) | ILD_TRANSPARENT | nOvlImageMask); 
	}

    // Draw item label - Column 0. 
    rcLabel.left += offset/2;
    rcLabel.right -= offset;
    pDC->DrawText(sLabel,-1,rcLabel, DT_LEFT |
									 DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP |
						            DT_VCENTER | DT_END_ELLIPSIS); 

	// Draw labels for remaining columns
    LV_COLUMN lvc; 
    lvc.mask = LVCF_FMT | LVCF_WIDTH;
    if (m_nHighlightType == 0) 
	{
		// Highlight only first column
        pDC->SetTextColor(::GetSysColor(COLOR_WINDOWTEXT));
        pDC->SetBkColor(::GetSysColor(COLOR_WINDOW));
	} 
    rcBounds.right = rcHighlight.right > rcBounds.right ? rcHighlight.right : rcBounds.right;
    rgn.CreateRectRgnIndirect(&rcBounds);
    pDC->SelectClipRgn(&rgn);
        
    for (int nColumn = 1; GetColumn(nColumn, &lvc); nColumn++) 
	{
		rcCol.left = rcCol.right; 
		rcCol.right += lvc.cx;

		// Draw column background
        if (m_nHighlightType == HIGHLIGHT_NORMAL)
		{
			if ((nColumn == 1) && (m_eBackground != BACK_NORMAL))
			{
				CBrush brBack(m_crBackgroundColor);
				pDC->FillRect(rcCol, &brBack);
			}
			else
			{
				CBrush brWindow(::GetSysColor(COLOR_WINDOW));
				pDC->FillRect(rcCol, &brWindow);
			}
		}

		sLabel = GetItemText(nItem, nColumn); 
		if (sLabel.GetLength() == 0)
			continue;
            
		// Get the text justification 
		UINT nJustify = DT_LEFT;
        switch (lvc.fmt & LVCFMT_JUSTIFYMASK) 
		{ 
        case LVCFMT_RIGHT:
			nJustify = DT_RIGHT; 
            break;
                
        case LVCFMT_CENTER:
			nJustify = DT_CENTER;
			break;
                
		default:
			break;
		}
            
		rcLabel = rcCol;    
		rcLabel.left += offset;
		rcLabel.right -= offset;
		pDC->DrawText(sLabel, -1, rcLabel, nJustify | DT_SINGLELINE | DT_NOPREFIX | DT_VCENTER | DT_END_ELLIPSIS); 
	}

	// Draw focus rectangle if item has focus
    if (lvi.state & LVIS_FOCUSED && (GetFocus() == this))
		pDC->DrawFocusRect(rcHighlight);

    // Restore dc 
    pDC->RestoreDC( nSavedDC );
}

void CSortListCtrl::RepaintSelectedItems()
{
    CRect rcBounds, rcLabel;
    
    // Invalidate focused item so it can repaint 
    int nItem = GetNextItem(-1, LVNI_FOCUSED);
    
    if (nItem != -1) 
	{
        GetItemRect(nItem, rcBounds, LVIR_BOUNDS);
        GetItemRect(nItem, rcLabel, LVIR_LABEL);
        rcBounds.left = rcLabel.left;
        InvalidateRect(rcBounds, FALSE);
    }
    
    // Invalidate selected items depending on LVS_SHOWSELALWAYS
    if (!(GetStyle() & LVS_SHOWSELALWAYS)) 
	{
        for (nItem = GetNextItem(-1, LVNI_SELECTED); nItem != -1; 
        nItem = GetNextItem(nItem, LVNI_SELECTED))
        {
            GetItemRect(nItem, rcBounds, LVIR_BOUNDS);
            GetItemRect(nItem, rcLabel, LVIR_LABEL);
            rcBounds.left = rcLabel.left;
            InvalidateRect(rcBounds, FALSE);
        }
    }
    UpdateWindow();
}

void CSortListCtrl::OnPaint() 
{
    CDragListCtrl::OnPaint();

    // in full row select mode, we need to extend the clipping region
    // so we can paint a selection all the way to the right
    if (m_nHighlightType == HIGHLIGHT_ROW && (GetStyle() & LVS_TYPEMASK) == LVS_REPORT ) 
	{
        CRect rcBounds;
        GetItemRect(0, rcBounds, LVIR_BOUNDS);
        
        CRect rcClient;
        GetClientRect(&rcClient);
        
        if (rcBounds.right < rcClient.right) 
		{
            CPaintDC dc(this);
            CRect rcClip;
            
            dc.GetClipBox(rcClip);
            rcClip.left = min(rcBounds.right-1, rcClip.left);
            rcClip.right = rcClient.right; 
            InvalidateRect(rcClip, FALSE); 
        } 
    }
    //CPaintDC dc(this); // device context for painting
}

void CSortListCtrl::OnKillFocus(CWnd* pNewWnd) 
{
    CDragListCtrl::OnKillFocus(pNewWnd);
    // check if we are losing focus to label edit box
    if (pNewWnd != NULL && pNewWnd->GetParent() == this)
        return;
    // repaint items that should change appearance
    if ((GetStyle() & LVS_TYPEMASK) == LVS_REPORT)
        RepaintSelectedItems();
}

void CSortListCtrl::OnSetFocus(CWnd* pOldWnd) 
{
    CDragListCtrl::OnSetFocus(pOldWnd);
    // check if we are getting focus from label edit box
    if (pOldWnd!=NULL && pOldWnd->GetParent()==this)
        return;
    // repaint items that should change appearance
    if ((GetStyle() & LVS_TYPEMASK)==LVS_REPORT)
        RepaintSelectedItems();
}

int CSortListCtrl::SetHighlightType(EHighlight hilite)
{
    int oldhilite = m_nHighlightType;
    if (hilite <= HIGHLIGHT_ROW ) 
	{
        m_nHighlightType = hilite;
        Invalidate(); 
    } 
    return oldhilite;
}


//
//    SetRowBackground
//    ================
//
//    Set the background color for a row
void CSortListCtrl::SetRowBackground	(int nRow ,COLORREF bkColor)
{
	// Find the row
	ASSERT(nRow < (int)m_listData.GetCount());
	CSortListRow* pRow = (CSortListRow*)CDragListCtrl::GetItemData(nRow);
	pRow->m_crRowColor = bkColor;
}


BOOL CSortListCtrl::DeleteColumn(int nCol)
{
	m_listColumnUnits[nCol] == "/0";
	return CDragListCtrl::DeleteColumn(nCol);
}

void CSortListCtrl::DeleteAllCols()
{
	for (DWORD dw=0; dw<MAX_COLS; dw++)
		m_listColumnUnits[dw] = "";
	CDragListCtrl::DeleteAllCols();
}

//
//    OnNotify
//    ========
//
//    Windows message handler
//
BOOL CSortListCtrl::OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult) 
{
    HD_NOTIFY *pHD = (HD_NOTIFY*)lParam;
    if ((pHD->hdr.code == HDN_ITEMCHANGINGA || pHD->hdr.code == HDN_ITEMCHANGINGW) 
	&&  (GetStyle() & LVS_TYPEMASK) == LVS_REPORT)
    { 
        // Invalidate empty bottom part of control to force erase the previous 
        // position of column
        int nBottom, nLastItem = GetItemCount()-1;
        if (nLastItem < 0) 
		{
            // List is empty : invalidate whole client rect
            nBottom = 0;
        } 
		else 
		{ 
            // Get Y position of bottom of list (last item)
            RECT ItemRect;
            GetItemRect(nLastItem,&ItemRect,LVIR_BOUNDS);
            nBottom = ItemRect.bottom;
        }
        
		RECT rect;
        GetClientRect(&rect);
        if (nBottom < rect.bottom) 
		{ 
            // Set top of rect as bottom of list (last item) : rect = empty part of list
            rect.top = nBottom;
            InvalidateRect(&rect);
        }
        // NB: We must go on with default processing. 
    } 
    *pResult = 0;
    return CDragListCtrl::OnNotify(wParam, lParam, pResult);
}

void CSortListCtrl::SetBackground	(EBackground background ,COLORREF Color)
{
	m_eBackground = background;
	m_crBackgroundColor = Color;
}



//----------------------------------------------------------------------------
//
//    CSortHeaderCtrl
//
BEGIN_MESSAGE_MAP(CSortHeaderCtrl, CHeaderCtrl)
	//{{AFX_MSG_MAP(CSortHeaderCtrl)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

CSortHeaderCtrl::CSortHeaderCtrl()
{
	m_nSortColumn = NOT_SORTED;
	m_bSortAscending = TRUE;
}

CSortHeaderCtrl::~CSortHeaderCtrl()
{
}

int CSortHeaderCtrl::SetSortImage	(int nCol, BOOL bAscending)
{
	int nPrevCol = m_nSortColumn;

	m_nSortColumn = nCol;
	m_bSortAscending = bAscending;

	SetOwnerDraw(nCol);

	// Invalidate header control so that it gets redrawn
	Invalidate();

	return nPrevCol;
}

void CSortHeaderCtrl::SetOwnerDraw(int nCol)
{
	HD_ITEM hditem;
	
	hditem.mask = HDI_FORMAT;
	GetItem(nCol, &hditem);
	hditem.fmt |= HDF_OWNERDRAW;
	SetItem(nCol, &hditem);
}


//
//    DrawItem
//    ========
//
//    Owner draw function to draw the list control
//
void CSortHeaderCtrl::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
	CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);

	// Get the column rect
	CRect rcLabel(lpDrawItemStruct->rcItem);
	
	// Draw the background
	CBrush br3DFace(::GetSysColor(COLOR_3DFACE));
    pDC->FillRect(rcLabel, &br3DFace);	

	// Labels are offset by a certain amount  
	// This offset is related to the width of a space character
	int offset = pDC->GetTextExtent(_T(" "), 1).cx*2;

	// Get the column text and format
	TCHAR buf[256];
	HD_ITEM hditem;	

	hditem.mask = HDI_TEXT | HDI_FORMAT;
	hditem.pszText = buf;
	hditem.cchTextMax = 255;
	
	GetItem(lpDrawItemStruct->itemID, &hditem);

	// Determine format for drawing column label
	UINT uFormat = DT_SINGLELINE | DT_NOPREFIX | DT_VCENTER | DT_END_ELLIPSIS ;

	if (hditem.fmt & HDF_CENTER)
		uFormat |= DT_CENTER;
	else if (hditem.fmt & HDF_RIGHT)
		uFormat |= DT_RIGHT;
	else
		uFormat |= DT_LEFT;

	// Adjust the rect if the mouse button is pressed on it
	if (lpDrawItemStruct->itemState == ODS_SELECTED)
	{
		rcLabel.left++;
		rcLabel.top += 2;
		rcLabel.right++;
	}

	// Adjust the rect further if Sort arrow is to be displayed
	if (lpDrawItemStruct->itemID == static_cast<UINT>(m_nSortColumn))
	{
		rcLabel.right -= 3 * offset;
	}
	
	rcLabel.left += offset;
	rcLabel.right -= offset;
	
	// Draw column label
	pDC->DrawText(buf, -1, rcLabel, uFormat);

	// Draw the Sort arrow
	if (lpDrawItemStruct->itemID == static_cast<UINT>(m_nSortColumn))
	{
		CRect rcIcon(lpDrawItemStruct->rcItem);

		// Set up pens to use for drawing the triangle
		CPen penLight(PS_SOLID, 1, GetSysColor(COLOR_3DHILIGHT));
		CPen penShadow(PS_SOLID, 1, GetSysColor(COLOR_3DSHADOW));
		CPen *pOldPen = pDC->SelectObject(&penLight);

		if (m_bSortAscending)
		{
			// Draw triangle pointing upwards
			pDC->MoveTo(rcIcon.right - 2*offset, offset - 1);
			pDC->LineTo(rcIcon.right - 3*offset/2, rcIcon.bottom - offset);
			pDC->LineTo(rcIcon.right - 5*offset/2-2, rcIcon.bottom - offset);
			pDC->MoveTo(rcIcon.right - 5*offset/2-1, rcIcon.bottom - offset - 1);

			pDC->SelectObject(&penShadow);
			pDC->LineTo(rcIcon.right - 2*offset, offset-2);
		}
		else	
		{
			// Draw triangle pointing downwards
			pDC->MoveTo(rcIcon.right - 3*offset/2, offset-1);
			pDC->LineTo(rcIcon.right - 2*offset-1, rcIcon.bottom - offset + 1);
			pDC->MoveTo(rcIcon.right - 2*offset-1, rcIcon.bottom - offset);

			pDC->SelectObject(&penShadow);
			pDC->LineTo(rcIcon.right - 5*offset/2-1, offset - 1);
			pDC->LineTo(rcIcon.right - 3*offset/2, offset - 1);
		}
		
		// Restore the pen
		pDC->SelectObject(pOldPen);
	}
}


//
//    Destructor for a list row.
//    Remember to free up the data in the dynalist
//
CSortListRow::~CSortListRow()
{ 
	for (DWORD dw=0; dw<GetCount(); dw++)
	{
		CSortListItem* pItem = (*this)[dw];
		delete pItem;
	}
}
