#include "stdafx.h"
#include "scan32.h"
#include "PageLocations.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const char chSep = ';';

/////////////////////////////////////////////////////////////////////////////
// CPageLocations property page

IMPLEMENT_DYNCREATE(CPageLocations, CScannerPage)

CPageLocations::CPageLocations() : CScannerPage(CPageLocations::IDD)
{
	//{{AFX_DATA_INIT(CPageLocations)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

CPageLocations::~CPageLocations()
{
}

void CPageLocations::DoDataExchange(CDataExchange* pDX)
{
	CScannerPage::DoDataExchange(pDX);

	if (!pDX->m_bSaveAndValidate)
	{
		// read previously stored location from the application class
		m_strLocation = GetAPP()->m_strLoc;
	}

	//{{AFX_DATA_MAP(CPageLocations)
	DDX_Control(pDX, IDC_TV_LOCATION, m_tvLocation);
	//}}AFX_DATA_MAP

	if (pDX->m_bSaveAndValidate)
	{
		// get the currently selected tree control Location
		HTREEITEM hItem = m_tvLocation.GetSelectedItem();	
		// build the compound location name and save it
		m_strLocation.Empty();
		while (hItem && hItem != m_tvLocation.GetRootItem()) 
		{
			m_strLocation = m_tvLocation.GetItemText(hItem) + ";" + m_strLocation;
			hItem = m_tvLocation.GetParentItem(hItem);
		}
		// remove any trailing separator
		if (m_strLocation.GetLength())
			m_strLocation = m_strLocation.Left(m_strLocation.GetLength() - 1);
			
		// store back to application class
		GetAPP()->m_strLoc = m_strLocation;
	}
}


BEGIN_MESSAGE_MAP(CPageLocations, CScannerPage)
	//{{AFX_MSG_MAP(CPageLocations)
	ON_BN_CLICKED(IDC_BN_ADD, OnBnAdd)
	ON_NOTIFY(TVN_ENDLABELEDIT, IDC_TV_LOCATION, OnEndlabeleditTvLocation)
	ON_NOTIFY(TVN_BEGINLABELEDIT, IDC_TV_LOCATION, OnBeginlabeleditTvLocation)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageLocations message handlers

/*
** Initial Page Load
*/
BOOL CPageLocations::OnInitDialog() 
{
	CScannerPage::OnInitDialog();

	// Add images to the image list as required
	m_imageList.Create();
	m_imageList.AddBitmap(IDB_LOCATION);
	
	// reset tree control, add the root item (Company), then fill
	m_tvLocation.DeleteAllItems();

	// Set the image list for the tree
	m_tvLocation.SetImageList(&m_imageList, TVSIL_NORMAL);

	// Get the company name 
	CString strCompany = GetCFG().RootLocation();

	if (strCompany == "")
		strCompany = "Your Company";

	HTREEITEM hRoot = m_tvLocation.InsertItem (strCompany, NULL, TVI_SORT);
	AddChildLocations (1, hRoot);

	// expand the root item...
	m_tvLocation.Expand(hRoot, TVE_EXPAND);

	// restore any previously stored location
	SelectLocation (m_strLocation);

	// If we have disabled the ability to add new locations, hide the label and edit box
	//int nShow = ((GetCFG().FieldsMask() & CAuditScannerConfiguration::AddLocation) == 0) ? SW_HIDE : SW_SHOW;
	//GetDlgItem(IDC_BN_ADD)->ShowWindow(nShow);

	//int nShow = GetCFG().InteractiveAllowAddLocation();
//	GetDlgItem(IDC_BN_ADD)->ShowWindow(nShow);

	GetDlgItem(IDC_BN_ADD)->ShowWindow(FALSE);
	
	return TRUE;
}



//
//    Helper Function to recursively populate the tree control
//
void CPageLocations::AddChildLocations (DWORD dwParentID, HTREEITEM hParent)
{
	CDynaList<CAuditLocation>& listLocations = GetCFG().Locations();

	// find all locations with the right parent
	for (DWORD dw=0; dw < listLocations.GetCount(); dw++)
	{
		if (listLocations[dw].ParentID() == dwParentID)
		{
			// store in the tree control			
			HTREEITEM hItem = m_tvLocation.InsertItem(listLocations[dw].Name() ,hParent ,TVI_SORT);
			
			// store the database ID and force it to be visible
			m_tvLocation.SetItemData (hItem, listLocations[dw].ID());
			
			// and recurse to add its children
			AddChildLocations (listLocations[dw].ID(), hItem);
		}
	}	
}



//
//   Helper Function to find a hierarchical location within the tree Non-existent locations are 
//   added automatically
//
void CPageLocations::SelectLocation (LPCSTR pszLocation)
{
	// copy to local buffer and format with ASCIIZ separators
	int nLen = strlen(pszLocation);
	char * pBuffer = new char [nLen + 2];
	memset (pBuffer, 0, nLen + 2);
	strcpy (pBuffer, pszLocation);
	for (int n = 0 ; n < nLen ; n++)
	{
		if (pBuffer[n] == ';')
			pBuffer[n] = '\0';
	}

	// starting from the root, find each subsection
	HTREEITEM hParent = m_tvLocation.GetRootItem();
	HTREEITEM hItem = NULL;
	for (char * p = pBuffer ; *p ; p += strlen(p) + 1) 
	{
		// look through the tree item's children
		for (hItem = m_tvLocation.GetChildItem(hParent) ; hItem != NULL ; hItem = m_tvLocation.GetNextSiblingItem(hItem)) 
		{
			if (m_tvLocation.GetItemText(hItem) == p) 
			{
				// found it - continue to next level
				hParent = hItem;
				break;
			}
		}
		// was no location found ?
		if (hParent != hItem)
		{
			// no - must have been added during a previous audit, so add to tree again
			hParent = m_tvLocation.InsertItem (p, hParent, TVI_SORT);
		}
	}
	delete [] pBuffer;

	// select the item and bring it into view
	hItem = hParent;
	if (NULL != hItem)
	{
		m_tvLocation.EnsureVisible(hItem);
		m_tvLocation.SelectItem(hItem);
	}
}



BOOL CPageLocations::OnSetActive() 
{
	// Get the parent property Sheet
	CPropertySheet * pParent = (CPropertySheet*)GetParent();
	int nPageCount = pParent->GetPageCount();
	int nPage = pParent->GetActiveIndex();

	// set buttons up depending on where we are in the list
	DWORD dwButtonFlags = 0;
	// first Page ?
	if (nPage != 0)
		dwButtonFlags = PSWIZB_BACK;
	// last page ?
	if (nPage == (nPageCount - 1))
		dwButtonFlags |= PSWIZB_FINISH;
	else
		dwButtonFlags |= PSWIZB_NEXT;
	pParent->SetWizardButtons(dwButtonFlags);
	
	return CScannerPage::OnSetActive();
}



BOOL CPageLocations::OnKillActive() 
{
	return CScannerPage::OnKillActive();
	
	// If we have anything specified in the user location field then save this otherwise
	// we need to pull the selected location from the tree control
}



void CPageLocations::OnBnAdd() 
{
	// get the currently highlighted tree item, ignore if null
	HTREEITEM hItem = m_tvLocation.GetSelectedItem();
	if (NULL == hItem)
		return;
		
	// enter a new item in the tree and flick into edit mode
	HTREEITEM hNewLoc = m_tvLocation.InsertItem("New Location", hItem, TVI_LAST);
	m_tvLocation.SelectItem(hNewLoc);
	m_tvLocation.EditLabel(hNewLoc);	
}


void CPageLocations::OnBeginlabeleditTvLocation(NMHDR* pNMHDR, LRESULT* pResult) 
{
	TV_DISPINFO* pTVDispInfo = (TV_DISPINFO*)pNMHDR;
	
	// get the database ID of the item
	HTREEITEM hItem = pTVDispInfo->item.hItem;
	LPARAM lItemID = m_tvLocation.GetItemData(hItem);

	// only accept the edit if it's a newly added item - ie has no database ID
	if (lItemID || hItem == m_tvLocation.GetRootItem())
		*pResult = 1;
	else
		*pResult = 0;
}

void CPageLocations::OnEndlabeleditTvLocation(NMHDR* pNMHDR, LRESULT* pResult) 
{
	TV_DISPINFO* pTVDispInfo = (TV_DISPINFO*)pNMHDR;

	// make sure the text has actually changed
	if (TVIF_TEXT & pTVDispInfo->item.mask)
	{
		// get the updated text from the display info structure
		CString strNewText = pTVDispInfo->item.pszText;
		// get the handle and update the item
		m_tvLocation.SetItemText (pTVDispInfo->item.hItem, strNewText);
	}
	
	*pResult = 0;
}


BOOL CPageLocations::PreTranslateMessage(MSG* pMsg) 
{
	// intercept "Enter" kestrokes
	if (pMsg->message >= WM_KEYFIRST && pMsg->message <= WM_KEYLAST)
	{
		if (pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_RETURN)
		{
			TRACE ("\nIntercepted ENTER key");
			// we've trapped an "ENTER" key - is the Tree Control in mid-edit ?
			CEdit * pEdit = m_tvLocation.GetEditControl();
			if (NULL != pEdit && NULL != pEdit->GetSafeHwnd())
			{
				// yes - end the edit by moving the focus
				m_tvLocation.SetFocus();
				return TRUE;
			}
		}
	}
	
	return CScannerPage::PreTranslateMessage(pMsg);
}
