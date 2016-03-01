
// FILE:	TreeCtrl.cpp
// PURPOSE:	Implementation of various sub-classed tree controls
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2000,2001
// HISTORY:	JRFT - 15.01.2000 - CheckTreeCtrl created
//			JRFT - 24.02.2000 - DragTreeCtrl created
//			JRFT - 19.06.2001 - "Drop" action adjusted in DragTreeCtrl
//			JRFT - 04.07.2001 - DragTreeCtrl now selects item being dragged, and prevents drag to self or descendant
// NOTES	Always include "TreeCtrl.h" in stdafx.h

#include "stdafx.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

////////////////////////////////////////////////////////////////////////////
// CTreeCtrlEx
//

IMPLEMENT_DYNAMIC(CTreeCtrlEx, CTreeCtrl)

/*
** add a new item to the tree
*/
HTREEITEM CTreeCtrlEx::Insert (LPCSTR pszText, int nIcon, LPARAM lParam/* = 0*/, HTREEITEM hParent/* = NULL*/, HTREEITEM hInsertAfter/* = TVI_SORT*/, int nChildren/* = 1*/)
{
	CString strTemp(pszText);
	TV_INSERTSTRUCT tvInsert;
	tvInsert.hParent		= hParent;
	tvInsert.hInsertAfter	= hInsertAfter;
	tvInsert.item.mask			= TVIF_TEXT | TVIF_IMAGE | TVIF_SELECTEDIMAGE | TVIF_PARAM | TVIF_CHILDREN;
	tvInsert.item.pszText		= strTemp.LockBuffer();
	tvInsert.item.iImage		= nIcon;
	tvInsert.item.iSelectedImage= nIcon;
	tvInsert.item.lParam		= lParam;
	tvInsert.item.cChildren		= nChildren;
	return InsertItem(&tvInsert);
}

/*
** adds a new item, taking into account parent's expansion state
*/
HTREEITEM CTreeCtrlEx::InsertItemEx (LPCSTR pszText, int nIcon, LPARAM lParam, HTREEITEM hParent, int nChildren/* = 1*/)
{
	// first ensure the parent knows it now has at least one child
	int nParentChildCount = GetChildCount(hParent);
	SetChildCount(hParent, ++nParentChildCount);

	// is the parent already expanded ?
	if (0 == (TVIS_EXPANDEDONCE & GetItemState(hParent, TVIS_EXPANDEDONCE)))
	{
		// no - expand it (which should automatically add the new item)
		Expand(hParent, TVE_EXPAND);
		// now find and remove the item being added...
		HTREEITEM hChild = FindChild(hParent, pszText);
		if (NULL != hChild)
			DeleteItem(hChild);
		// (note we delete it so that it can be re-added at the end of the list, rather than in alphabetical order)
	}

	// now add the new item at the end of the list
	HTREEITEM hNewItem = Insert(pszText, nIcon, lParam, hParent, TVI_LAST, nChildren);
	// ensure it is visible
	EnsureVisible(hNewItem);
//	SelectItem(hNewItem);
	return hNewItem;
}

/*
** copy hItem from existing parent to hNewParent
*/
HTREEITEM CTreeCtrlEx::CopyItem (HTREEITEM hItem, HTREEITEM hNewParent)
{
	// make an exact copy of the item
	TV_INSERTSTRUCT tvInsert;
	// load existing data into the structure
	TV_ITEM & item = tvInsert.item;
	item.hItem	= hItem;
	item.mask	= TVIF_CHILDREN | TVIF_IMAGE | TVIF_PARAM | TVIF_SELECTEDIMAGE | TVIF_STATE | TVIF_STATE | TVIF_TEXT;
	item.stateMask	= 0xFFFF;
	char szBuffer[255];
	item.pszText = szBuffer;
	item.cchTextMax = 254;
	GetItem(&item);
	// now insert a copy under the new parent
	tvInsert.hParent = hNewParent;
	tvInsert.hInsertAfter = TVI_LAST;
	HTREEITEM hNewItem = InsertItem (&tvInsert);

	// now copy the children - unless they have not yet been added
	UINT nState = GetItemState (hItem, TVIS_EXPANDEDONCE);
	if (nState != 0)
	{
		// children already exist - if any ??
		if (ItemHasChildren(hItem))
		{
			for (HTREEITEM hChild = GetChildItem(hItem) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
			{
				CopyItem (hChild, hNewItem);
			}
		}
	}
	return hNewItem;
}

/*
** move hItem from existing parent to hNewParent
*/
HTREEITEM CTreeCtrlEx::MoveItem (HTREEITEM hItem, HTREEITEM hNewParent)
{
	// first check that it is a new location - can't move an item to itself
	HTREEITEM hOldParent = GetParentItem(hItem);
	if (hOldParent == hNewParent)
	{
		TRACE ("\nAttempt to move item to itself");
		return hItem;
	}

	// make an exact copy of the item
	HTREEITEM hNewItem = CopyItem (hItem, hNewParent);
	// delete the existing copy
	DeleteItem (hItem);
	// and return the new one
	return hNewItem;
}

/*
** return the unselected image index of hItem
*/
int CTreeCtrlEx::GetItemImage (HTREEITEM hItem) const
{
	int nImage, nSelectedImage;
	CTreeCtrl::GetItemImage(hItem, nImage, nSelectedImage);
	return nImage;
}

/*
** return handle of child item with matching text
*/
HTREEITEM CTreeCtrlEx::FindChild (HTREEITEM hParent, LPCSTR pszText) const
{
	HTREEITEM hChild;
	for (hChild = GetChildItem(hParent) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
		if (GetItemText(hChild) == pszText)
			break;
	return hChild;
}

/*
** return the child count for hItem
*/
int CTreeCtrlEx::GetChildCount (HTREEITEM hItem)
{
	TV_ITEM item;
	item.mask = TVIF_CHILDREN;
	item.hItem = hItem;
	GetItem(&item);
	return item.cChildren;
}

/*
** manually set the child count for hItem
*/
void CTreeCtrlEx::SetChildCount (HTREEITEM hItem, int nCount)
{
	TV_ITEM item;
	item.mask = TVIF_CHILDREN;
	item.hItem = hItem;
	item.cChildren = nCount;
	SetItem(&item);
}

/*
** Add a new child item to a tree that performs "active" population, then selects it
*/
HTREEITEM CTreeCtrlEx::AddNewChild (HTREEITEM hParent, LPCSTR pszItemLabel, int nIcon, DWORD dwItemData, int nChildCount/* = 1*/)
{
	// Note that this relies on the new item already being in the database...

	// first expand the parent item (it must now contain at least one child)
	SetChildCount (hParent, 1);
	Expand (hParent, TVE_EXPAND);

	// now if the item hadn't previously been expanded the new item will now appear, so remove it
	HTREEITEM hChild;
	for (hChild = GetChildItem(hParent) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
	{
		if (GetItemImage(hChild) == nIcon && GetItemText(hChild) == pszItemLabel)
		{
			DeleteItem (hChild);
			break;
		}
	}

	// add the new item at the end of the list
	hChild = Insert  (pszItemLabel, nIcon, dwItemData, hParent, TVI_LAST, nChildCount);
	EnsureVisible (hChild);
	SelectItem (hChild);
	return hChild;
}

/*
** remove all current children and collapse item ready for repopulation
*/
void CTreeCtrlEx::RemoveChildren (HTREEITEM hItem)
{
	Expand (hItem, TVE_COLLAPSE);
	HTREEITEM hChild;
	while (NULL != (hChild = GetChildItem(hItem)))
		DeleteItem (hChild);
	SetChildCount (hItem, 1);
	SetItemState (hItem, 0, TVIS_EXPANDEDONCE);
}

/*
** Return a string describing the currently selected node in the tree
*/
CString CTreeCtrlEx::GetSelectedItemPath (char chSep/* = '\\'*/)
{
    CString strPathName;

    HTREEITEM hItem = GetSelectedItem();
    while (hItem != NULL) 
	{
        CString string = GetItemText (hItem);
        if ((string.Right (1) != chSep) && !strPathName.IsEmpty ())
            string += chSep;
        strPathName = string + strPathName;
        hItem = GetParentItem (hItem);
    }
    return strPathName;
}


/////////////////////////////////////////////////////////////////////////////
// CCheckTreeCtrl

IMPLEMENT_DYNAMIC(CCheckTreeCtrl, CTreeCtrlEx)

/*
** Set or reset the check state of an item
*/
void CCheckTreeCtrl::SetItemCheck (HTREEITEM hItem, UINT nState, BOOL bForceChildren/* = TRUE*/, BOOL bAdjustParent/* = TRUE*/)
{
	ASSERT (nState <= CTV_PARTIAL);

	// if we are "checking", first clear any children that are already checked
	if (CTV_CHECKED == nState)
		UncheckChildren (hItem);

	// if we are "unchecking" do the parent first
	if (bAdjustParent && (nState == CTV_UNCHECKED || nState == CTV_PARTIAL))
		AdjustParentState (hItem, nState);

	SetItemState (hItem, (nState << 12), TVIS_STATEIMAGEMASK);

	// ...if we are "checking" then do parent last
	if (bAdjustParent && nState == CTV_CHECKED)
		AdjustParentState (hItem, nState);

	// copy setting to children
	if (bForceChildren)
		SetChildItemStates (hItem, nState, bForceChildren);
}

void CCheckTreeCtrl::UncheckChildren (HTREEITEM hParent)
{
	// run through children
	for (HTREEITEM hChild = GetChildItem(hParent) ; NULL != hChild ; hChild = GetNextSiblingItem (hChild))
	{
		UINT nOldState = GetItemCheck(hChild);
		if (nOldState == CTV_CHECKED)
		{
			// notify the parent window
			NM_CHECKTREEVIEW nm;
			nm.hdr.code		= CTVN_CHECKCHANGED;
			nm.hdr.hwndFrom	= GetSafeHwnd();
			nm.hdr.idFrom	= GetDlgCtrlID();
			nm.hItem		= hChild;
			nm.nOldState	= CTV_CHECKED;
			nm.nNewState	= CTV_UNCHECKED;
			GetParent()->SendMessage(WM_NOTIFY, (WPARAM)GetDlgCtrlID(), (LPARAM)&nm);

			SetItemState (hChild, (CTV_UNCHECKED << 12), TVIS_STATEIMAGEMASK);
		}
		// partial items need recursing
		else if (nOldState == CTV_PARTIAL)
			UncheckChildren (hChild);
	}
}

/*
** Get current check state of an item
*/
UINT CCheckTreeCtrl::GetItemCheck (HTREEITEM hItem) const
{
	UINT nState = GetItemState (hItem, TVIS_STATEIMAGEMASK);
	return (nState >> 12);
}

/*
** Protected member to adjust state of a changed item's parent
*/
void CCheckTreeCtrl::AdjustParentState (HTREEITEM hItem, UINT nState)
{
	// get the parent item
	HTREEITEM hParent = GetParentItem(hItem);
	if (hParent == NULL)
		return;

	UINT nOldState = GetItemState(hParent, TVIS_STATEIMAGEMASK) >> 12;
	UINT nNewState = nOldState;

	// loop through all its children
	UINT nLast = 0;
	for (HTREEITEM hChild = GetChildItem(hParent) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
	{
		UINT nChildState;
		if (hChild == hItem)
			nChildState = nState;
		else
			nChildState = GetItemState (hChild, TVIS_STATEIMAGEMASK) >> 12;

		// ignore if state icon is disabled
		if (nChildState == 0)
			continue;
		nNewState = nChildState;
		// is there an item different to previous siblings ?
		if (nLast && nLast != nNewState)
		{
			// yes - set item to "partial" and GET OUT!
			nNewState = CTV_PARTIAL;
			break;
		}
		nLast = nNewState;
	}

	// has the state changed ?
	if (nOldState != nNewState)
	{
		// if we are "unchecking" then do parent first
		if (nNewState == CTV_UNCHECKED || nNewState == CTV_PARTIAL)
			AdjustParentState (hParent, nNewState);

		// Update the parent item
		SetItemState (hParent, (nNewState << 12), TVIS_STATEIMAGEMASK);

		// notify the parent window
		NM_CHECKTREEVIEW nm;
		nm.hdr.code		= CTVN_CHECKCHANGED;
		nm.hdr.hwndFrom	= GetSafeHwnd();
		nm.hdr.idFrom	= GetDlgCtrlID();
		nm.hItem		= hParent;
		nm.nOldState	= nOldState;
		nm.nNewState	= nNewState;
		GetParent()->SendMessage(WM_NOTIFY, (WPARAM)GetDlgCtrlID(), (LPARAM)&nm);

		// if we are "checking" do parent last
		if (nNewState == CTV_CHECKED)
			AdjustParentState (hParent, nNewState);
	}
}

/*
** Protected member to cascade a changed item state to its children
*/
void CCheckTreeCtrl::SetChildItemStates (HTREEITEM hParent, UINT nState, BOOL bForce)
{
	// find all children for this item - but only if expanded
//	BOOL bExpanded = TVIS_EXPANDED & GetItemState(hParent, TVIS_EXPANDED);
//	Expand(hParent, TVE_EXPAND);
	for (HTREEITEM hChild = GetChildItem(hParent) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
	{
		// check whether children needs setting
		if (bForce || GetItemState(hChild, TVIS_STATEIMAGEMASK))
			SetItemState (hChild, (nState << 12), TVIS_STATEIMAGEMASK);
		// recurse for grandchildren!
		SetChildItemStates (hChild, nState, bForce);
	}
}

BEGIN_MESSAGE_MAP(CCheckTreeCtrl, CTreeCtrlEx)
	//{{AFX_MSG_MAP(CCheckTreeCtrl)
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CCheckTreeCtrl message handlers

/*
** Respond to a left button click on the state icon
** by toggling the item check state on or off
*/
void CCheckTreeCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// is it a request to change the item state ?
	UINT n, nOldState, nNewState;
	HTREEITEM hItem = HitTest (point, &n);
	if (n & TVHT_ONITEMSTATEICON)
	{
		// yes - get the current item state...
		nOldState = GetItemState (hItem, TVIS_STATEIMAGEMASK) >> 12;
		
		if (nOldState == 2)
			nNewState = 1;
		else
			nNewState = 2;

		SetItemCheck (hItem, nNewState);

		// notify the parent window
		NM_CHECKTREEVIEW nm;
		nm.hdr.code	= CTVN_CHECKCHANGED;
		nm.hdr.hwndFrom	= GetSafeHwnd();
		nm.hdr.idFrom		= GetDlgCtrlID();
		nm.hItem = hItem;
		nm.nOldState = nOldState;
		nm.nNewState = nNewState;
		GetParent()->SendMessage(WM_NOTIFY, (WPARAM)GetDlgCtrlID(), (LPARAM)&nm);

		// don't call the base class as we've done it ourselves
		return;
	}	
	CTreeCtrl::OnLButtonDown(nFlags, point);
}

CDragTreeCtrl::CDragTreeCtrl()
{
	m_bDragging	= FALSE;
	m_hDragItem	= NULL;
	// attributes for auto-scrolling & auto-expansion
	m_nDelayInterval = 500;
	m_nScrollInterval = 200;
	m_nScrollMargin = 30;
}

CDragTreeCtrl::~CDragTreeCtrl()
{
}

IMPLEMENT_DYNAMIC(CDragTreeCtrl, CTreeCtrlEx)

BEGIN_MESSAGE_MAP(CDragTreeCtrl, CTreeCtrlEx)
	//{{AFX_MSG_MAP(CDragTreeCtrl)
	ON_NOTIFY_REFLECT(TVN_BEGINDRAG, OnBegindrag)
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDragTreeCtrl message handlers

/*
** Begin a drag operation
*/
void CDragTreeCtrl::OnBegindrag(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;
	*pResult = 0;

	// get the item being dragged
	TV_ITEM item = pNMTreeView->itemNew;

	// abort if item is root, or derived class prevents it
	if (item.hItem == GetRootItem() || !CanDragItem(item.hItem))
	{
		SelectItem(NULL);
		return;
	}

	// make the item the current selection
	SelectItem(item.hItem);

	// create the drag image
	m_pDragImage = CreateDragImage(item.hItem);
	// Calculate the "hot-spot" ie cursor location relative to upper-left corner of drag item rectangle
	CRect rect;
	GetItemRect (item.hItem, rect, TRUE);
	CPoint pt(pNMTreeView->ptDrag);
	CPoint ptHotSpot = pt;
	ptHotSpot.x -= rect.left;
	ptHotSpot.y -= rect.top;
	// meanwhile, point needs to hold the offset of the drag image rectangle, relative to the hotspot
	CPoint ptClient(0,0);
	ClientToScreen(&ptClient);
	CRect rectTV;
	GetWindowRect(rectTV);
	pt.x += ptClient.x - rectTV.left;
	pt.y += ptClient.y - rectTV.top;

	// begin the drag operation
	SetCapture();
	m_pDragImage->BeginDrag (0, ptHotSpot);
	m_pDragImage->DragEnter (this, pt);
	// store the asset handle for later and set internal flag
	m_hDragItem = item.hItem;
	m_bDragging = TRUE;

	TRACE ("\nCDragTreeCtrl::OnBegindrag() returning");
	
	*pResult = 0;
}

/*
** Process an ongoing drag & drop operation
*/
void CDragTreeCtrl::OnMouseMove(UINT nFlags, CPoint point) 
{
	CTreeCtrl::OnMouseMove(nFlags, point);
	// is this a drag and drop operation ?
	if (m_bDragging)
	{
		TRACE ("\nCDragTreeCtrl::OnMouseMove() processing move");
		// always reset timer when mouse moves
		KillTimer(1);
		// update the dragging image
		m_pDragImage->DragMove(point);
		// get target item
		HTREEITEM hTarget = HighlightDropTarget(point);
		// modify cursor if its a valid drop item
		::SetCursor( hTarget == NULL ? 
			AfxGetApp()->LoadStandardCursor(IDC_NO) :
			(HCURSOR)::GetClassLong(m_hWnd, GCL_HCURSOR));
		// Set timer if over an expandable item or at edge of scrollable screen
		CRect rect;
		GetClientRect(rect);
		int cy = rect.Height();
		if ((point.y >= 0 && point.y <= m_nScrollMargin) ||			// top of screen
			(point.y <= cy && point.y >= cy - m_nScrollMargin) ||	// bottom of screen
			(hTarget != NULL && ItemHasChildren(hTarget) && !IsItemExpanded(hTarget)))	// over expandable item
		{
			SetTimer(1, m_nDelayInterval, NULL);
			TRACE ("\nStarting Timer from OnMouseMove");
		}
	}
}

HTREEITEM CDragTreeCtrl::HighlightDropTarget (CPoint point)
{
	TRACE ("\nCDragTreeCtrl::HighlightDropTarget");
	// get the destination item
	UINT nFlags = TVHT_ONITEM;
	HTREEITEM hTarget = HitTest(point, &nFlags);
	// ensure drop target is not same item or descended from item being dropped
	HTREEITEM hParent = hTarget;
	while (hParent)
	{
		if (hParent == m_hDragItem)
		{
			hTarget = NULL;
			break;
		}
		hParent = GetParentItem(hParent);
	}

	if (NULL != hTarget)
	{
		// ask derived class whether we can drop here
		if (!CanDropHere(hTarget))
			hTarget = NULL;
	}
	m_pDragImage->DragShowNolock(FALSE);
	SelectDropTarget (hTarget);
	m_pDragImage->DragShowNolock(TRUE);
	return hTarget;
}

/*
** Completes the drag and drop operation
** JRFT - 19.06.2001 - now leaves the target item in its previous state - ie doesn't force it to expand
*/
void CDragTreeCtrl::OnLButtonUp(UINT nFlags, CPoint point) 
{
	if (m_bDragging)
	{
		TRACE ("\nCDragTreeCtrl::OnLButtonUp");
		// Stop timer if its running
		KillTimer(1);
		// stop the drop operation
		m_pDragImage->DragLeave(this);
		m_pDragImage->EndDrag ();
		// an asset is being moved - process it
		HTREEITEM hTarget = GetDropHilightItem();
		if (hTarget != NULL)
		{
			// is the destination item already expanded
			if (0 != (TVIS_EXPANDED & GetItemState (hTarget, TVIS_EXPANDED)))
			{
				// yes - get the derived class to do its updates, and see if it wants us to move it 
				if (DropItem (m_hDragItem, hTarget))
				{
					// yes - move it
					HTREEITEM hItem = MoveItem(m_hDragItem, hTarget);
					SelectItem(hItem);
				}
			}
			else
			{
				// otherwise temporarily expand the item and ensure it has a child count
				Expand(hTarget, TVE_EXPAND);
				SetItemState (hTarget, TVIS_EXPANDEDONCE, TVIS_EXPANDEDONCE);
				SetChildCount (hTarget, 1);
				// get the current parent item as this will end up selected
				HTREEITEM hOldParent = GetParentItem(m_hDragItem);
				// again let the derived class do any updates, and see if it wants us to move the item
				if (DropItem (m_hDragItem, hTarget))
					MoveItem (m_hDragItem, hTarget);
				// collapse the destination branch so as to leave it as previous
				Expand (hTarget, TVE_COLLAPSE);
				SelectItem(hOldParent);
			}
		}
		// release the mouse
		ReleaseCapture();
		m_bDragging = FALSE;
		SelectDropTarget(NULL);
		// get rid of the system-created image list
		delete m_pDragImage;
		m_pDragImage = NULL;
	}
	CTreeCtrl::OnLButtonUp(nFlags, point);
}

/*
** Respond to timer message during drag & drop by scrolling window or expanding an item
*/
void CDragTreeCtrl::OnTimer(UINT nIDEvent) 
{
	CTreeCtrl::OnTimer(nIDEvent);

	// ensure timer is destined for us
	if (nIDEvent != 1)
		return;

	// always reset the timer
	SetTimer(1, m_nScrollInterval, NULL);
	// get cursor position and window height
	DWORD dwPos = ::GetMessagePos();
	CPoint point(LOWORD(dwPos), HIWORD(dwPos));
	ScreenToClient(&point);
	CRect rect;
	GetClientRect(rect);
	int cy = rect.Height();
	// scroll if cursor is near the top
	if (point.y >= 0 && point.y <= m_nScrollMargin)
	{
		HTREEITEM hFirstVisible = GetFirstVisibleItem();
		// switch image off while we scroll
		m_pDragImage->DragShowNolock(FALSE);
		SendMessage (WM_VSCROLL, MAKEWPARAM(SB_LINEUP, 0), NULL);
		m_pDragImage->DragShowNolock(TRUE);
		// kill the timer if window was fully scrolled
		if (GetFirstVisibleItem() == hFirstVisible)
			KillTimer(1);
		else
		{
			HighlightDropTarget(point);
			return;
		}
	}
	// ...or near the bottom
	else if (point.y <= cy && point.y >= cy - m_nScrollMargin)
	{
		HTREEITEM hFirstVisible = GetFirstVisibleItem();
		// switch image off and scroll
		m_pDragImage->DragShowNolock(FALSE);
		SendMessage(WM_VSCROLL, MAKEWPARAM(SB_LINEDOWN, 0), NULL);
		m_pDragImage->DragShowNolock(TRUE);
		// kill timer if window was fully scrolled already
		if (GetFirstVisibleItem() == hFirstVisible)
			KillTimer(1);
		else
		{
			HighlightDropTarget(point);
			return;
		}
	}
	// is cursor "hovering over an expandable item ?
	UINT nFlags;
	HTREEITEM hItem = HitTest(point, &nFlags);
	if (hItem != NULL && ItemHasChildren(hItem) && !IsItemExpanded(hItem))
	{
		// expand it...
		m_pDragImage->DragShowNolock(FALSE);
		Expand(hItem, TVE_EXPAND);
		m_pDragImage->DragShowNolock(TRUE);
		KillTimer(1);
		return;
	}
}

CString CTreeCtrlEx::SaveSelection ()
{
	CString strBuffer, strResult;
	HTREEITEM hItem = GetSelectedItem();

	while (hItem)
	{
		strBuffer.Format("%s|", GetItemText(hItem));
		strResult = strBuffer + strResult;
		hItem = GetParentItem(hItem);
	}
	return strResult;
}

void CTreeCtrlEx::RestoreSelection (LPCSTR pszSelection)
{
	CString strBuffer(pszSelection);
	HTREEITEM hParent = NULL;

	// read a chunk...
	CString strThisBit = BreakString(strBuffer, '|', TRUE);
	while (strThisBit.GetLength())
	{
		Expand(hParent, TVE_EXPAND);
		for (HTREEITEM hChild = GetChildItem(hParent) ; hChild != NULL ; hChild = GetNextSiblingItem(hChild))
		{
			if (strThisBit == GetItemText(hChild))
			{
				hParent = hChild;
				break;
			}
		}
		// get next section
		strThisBit = BreakString(strBuffer, '|', TRUE);
	}

	SelectItem(hParent);
}

