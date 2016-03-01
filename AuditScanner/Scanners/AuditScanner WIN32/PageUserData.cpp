#include "stdafx.h"
#include "scan32.h"
#include "../AuditScannerCommon/UserDataCategory.h"
#include "PageUserData.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define CX_MARGIN	5
#define	CY_MARGIN	5
#define	CY_LABEL	16
#define	CY_EDIT		14


	
////////////////////////////////////////////////////////////////////////////
// CPageUserData property page

IMPLEMENT_DYNCREATE(CPageUserData, CScannerPage)

CPageUserData::CPageUserData(CUserDataCategory* pUserDataCategory) : CScannerPage(CPageUserData::IDD)
{
	//{{AFX_DATA_INIT(CPageUserData)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT	
	_pUserDataCategory = pUserDataCategory;
}

CPageUserData::~CPageUserData()
{
}


void CPageUserData::FailDDX (int nCtl, LPCSTR pszMessage)
{
	AfxMessageBox (pszMessage, MB_ICONEXCLAMATION);
	CDynaList<CUserDataField>&	listDataFields = _pUserDataCategory->ListDataFields();
	::SetFocus (listDataFields[nCtl].GetHandle());
	AfxThrowUserException();
}



//
//   The main data validation routine. Extracts and tests entered data
//   before saving it to the internal field list for later serialization
//
void CPageUserData::DoDataExchange(CDataExchange* pDX)
{
	CScannerPage::DoDataExchange(pDX);
	TRACE("\nCPageUserData::DoDataExchange...");

	//{{AFX_DATA_MAP(CPageUserData)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP

	//

	if (pDX->m_bSaveAndValidate)
	{
		CString strMessage;

		// Loop through each field defined within this category
		CDynaList<CUserDataField>& listDataFields = _pUserDataCategory->ListDataFields();
		for (int index = 0; index < (int)listDataFields.GetCount(); index++)
		{
			CUserDataField* pUserDataField = &listDataFields[index];
					
			// first check for mandatory field not filled
			CString strBuffer;
			::GetWindowText (pUserDataField->GetHandle(), strBuffer.GetBuffer(256), 256);
			
			strBuffer.ReleaseBuffer();
			if (strBuffer.IsEmpty() && pUserDataField->Mandatory())
			{
				CString strError;
				strError.Format("You must enter a value for the field '%s'" ,pUserDataField->Label());
				FailDDX(index ,strError);
			}
			
			// Further validation depends on type
			switch (pUserDataField->DataType())
			{
				case CUserDataField::typeText:
				{				
					// and check case rule
					switch (pUserDataField->InputCase())
					{
						case CUserDataField::any:
							break;
						case CUserDataField::upper:
							strBuffer.MakeUpper();
							break;
						case CUserDataField::lower:
							strBuffer.MakeLower();
							break;
						case CUserDataField::title:
							strBuffer = TitleCase(strBuffer);
							break;
						default:
							break;	// this covers 'A' option (Any case)
					}
						
					// update screen in case of changes
					::SetWindowText (pUserDataField->GetHandle(), (LPCSTR)strBuffer);
				}
				break;

				case CUserDataField::typeNumeric:
				break;

				case CUserDataField::typePicklist:
				{
					// get the selection index from the control
					int nIndex = ::SendMessage (pUserDataField->GetHandle(), CB_GETCURSEL, 0, 0L);

					// ...and load the string value
					int nLength = ::SendMessage (pUserDataField->GetHandle(), CB_GETLBTEXTLEN, nIndex, 0L);
					LPSTR pBuffer = strBuffer.GetBuffer (nLength + 1);
					::SendMessage (pUserDataField->GetHandle(), CB_GETLBTEXT, nIndex, (LPARAM)pBuffer);
					strBuffer.ReleaseBuffer();
				}
				break;

				case CUserDataField::typeEnvVar:				
					break;

				case CUserDataField::typeRegKey:
					break;

				case CUserDataField::typeDate:
				{
					strBuffer = StringToDate(strBuffer, '/').Format("%Y-%m-%d");

					/*CTime st;

					if (::SendMessage(pUserDataField->GetHandle(), DTM_GETSYSTEMTIME, 0, (LPARAM)&st) == GDT_NONE)
					{
						if (pUserDataField->Mandatory())
						{
							CString strError;
							strError.Format("You must enter a value for the field '%s'" ,pUserDataField->Label());
							FailDDX(index ,strError);
						}
						strBuffer = "";
					}
					else
					{
						strBuffer = StringToDate(strBuffer, '/').Format("%Y-%m-%d");
					}*/

					break;
				}

				case CUserDataField::typeCurrency:
					break;

				default:
					// Assertion failure means field.m_nType is illegal
					ASSERT(FALSE);


			}
			
			// value has passed validation - store it
			pUserDataField->CurrentValue(strBuffer);
		}
	}
}


BEGIN_MESSAGE_MAP(CPageUserData, CScannerPage)
	//{{AFX_MSG_MAP(CPageUserData)
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_WM_VSCROLL()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageUserData message handlers

BOOL CPageUserData::OnSetActive() 
{
	// Use parent fn to set buttons up
	((CShtScan*)GetParent())->SetButtons();
	
	return CScannerPage::OnSetActive();
}

/*
** Initial Dialog Creation - create additional fields too
*/
BOOL CPageUserData::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	// Create the required controls
	CreateControls ();


	return FALSE;  // return TRUE unless you set the focus to a control
}

/*
** For some reason MFC doesn't seem to call DoDataExchange when user presses Finish...
*/
BOOL CPageUserData::OnWizardFinish() 
{
	UpdateData(TRUE);
	
	return CScannerPage::OnWizardFinish();
}

void CPageUserData::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	
	// set up the property sheet's title
	CString strTitle;
	strTitle.Format("Audit Scanner - Asset Data for [%s]" ,_pUserDataCategory->Name());
	GetParent()->SetWindowText(strTitle);
}

void CPageUserData::OnSize(UINT nType, int cx, int cy) 
{
	CScannerPage::OnSize(nType, cx, cy);
	
	if (cx)
	{
		CDC * pDC = GetDC();
		pDC->MoveTo (0, 0);
		pDC->LineTo (cx, 0);
		pDC->LineTo (cx, cy);
		pDC->LineTo (0, cy);
		pDC->LineTo (0, 0);
	}
}



//
//    CreateControls
//    ==============
//
//    Create the controls to display on this form based on those defined in the category
//
void CPageUserData::CreateControls ()
{
	CDlgUnit du(this);
	CREATESTRUCT      *cs;

	// quick blag to get the font size
	HFONT hFont = (HFONT)SendDlgItemMessage (IDC_STATIC, WM_GETFONT);

	//GetDlgItem(IDC_LBL1)->ShowWindow(SW_HIDE);

	// Loop through each control
	CDynaList<CUserDataField>&	listDataFields = _pUserDataCategory->ListDataFields();
	for (int index = 0; index < (int)listDataFields.GetCount(); index++)
	{
		CUserDataField* pUserDataField = &listDataFields[index];

		// label
		CString strLabel;
		if (pUserDataField->Mandatory())
			strLabel = "* " + pUserDataField->Label();
		else
			strLabel = pUserDataField->Label();

		HWND hLabel = ::CreateWindowEx (0,	// extended style
			"static",						// class name
			strLabel,						// window text
			WS_CHILD | WS_VISIBLE,			// window style(s)
			0, 0,							// position
			0, 0,							// size
			this->m_hWnd,					// parent
			(HMENU)(index + 200),				// child ID
			AfxGetApp()->m_hInstance,		// application
			NULL);
		::SendMessage (hLabel, WM_SETFONT, (WPARAM)hFont, MAKELPARAM(TRUE, 0));

		// then the data entry control - set styles etc depending on control type
		CString strCtlType("edit");
		DWORD dwStyle = WS_CHILD | WS_BORDER | WS_VISIBLE;
		switch (pUserDataField->DataType())
		{
			case CUserDataField::typePicklist:
				strCtlType = "combobox";
				dwStyle |= WS_TABSTOP | CBS_DROPDOWNLIST | CBS_SORT | WS_VSCROLL;
				break;

			case CUserDataField::typeDate:
				strCtlType = DATETIMEPICK_CLASS;
				//dwStyle |= WS_BORDER | WS_CHILD | WS_VISIBLE | DTS_SHORTDATEFORMAT | DTS_SHOWNONE; 
				dwStyle |= WS_BORDER | WS_CHILD | WS_VISIBLE | DTS_SHORTDATEFORMAT; 
				break;

			case CUserDataField::typeNumeric:
				strCtlType = "edit";
				dwStyle |= ES_NUMBER;
				break;

			case CUserDataField::typeCurrency:
				strCtlType = "edit";
				dwStyle |= ES_NUMBER;
				break;

			case CUserDataField::typeEnvVar:
			case CUserDataField::typeRegKey:
				dwStyle |= ES_AUTOHSCROLL | ES_READONLY;
				break;

			default:
				dwStyle |= WS_TABSTOP | ES_AUTOHSCROLL;
				break;
		}		
		
		HWND hCtl = ::CreateWindowEx (WS_EX_CLIENTEDGE,	// extended style
			strCtlType,									// window class
			"",											// window text
			dwStyle,									// window style
			0, 0,										// window position
			0, 0,										// window size
			this->m_hWnd,								// parent window
			(HMENU)(index + 100),							// child ID
			AfxGetApp()->m_hInstance,					// application instance handle
			NULL);										// lParam for initialisation

		::SendMessage (hCtl, WM_SETFONT, (WPARAM)hFont, MAKELPARAM(TRUE, 0));
		
		// store the window handle
		pUserDataField->SetHandle(hCtl);
		
		// set focus to first child control
		if (index == 0)
			::SetFocus (hCtl);
		
		// if a picklist then populate it
		if (pUserDataField->DataType() == CUserDataField::typePicklist)
		{
			// get the picklist name
			CString strPicklist = pUserDataField->Picklist();
			CPicklist* pPicklist = GetCFG().GetPicklist(strPicklist);
			
			// loop through them
			if (pPicklist != NULL)
			{
				for (DWORD dw = 0 ; dw < pPicklist->GetCount() ; dw++)
				{
					CString pickitem = (*pPicklist)[dw];
					::SendMessage (hCtl, CB_ADDSTRING, 0, (LPARAM)(LPCSTR)pickitem);
				}
			}
		}
		

		// finally load any stored data from the parent application
		CString strValue = pUserDataField->CurrentValue();
		//
		switch (pUserDataField->DataType()) 
		{
			case CUserDataField::typeText:
			case CUserDataField::typeEnvVar:
			case CUserDataField::typeRegKey:
				::SetWindowText(hCtl, strValue);
				break;

			case CUserDataField::typeNumeric:
				{
					// write it as an integer to set blank or text to '0'
					int nValue = atoi(strValue);
					strValue.Format ("%d", nValue);
					::SetWindowText (hCtl, strValue);
				}
				break;

			case CUserDataField::typePicklist:
				{
					int n = ::SendMessage (hCtl, CB_FINDSTRINGEXACT, 0, (LPARAM)(LPCSTR)strValue);
					::SendMessage (hCtl, CB_SETCURSEL, n, 0L);
				}
				break;
		}
	}
	
	// set the initial positioning
	m_nScrollOffset = 0;
	MoveControls ();
}



void CPageUserData::MoveControls ()
{
	CDlgUnit du(this);
	DWORD dw;

	// sort out the basic geometry...
	CRect rcClient;
	GetClientRect (&rcClient);
	int cxMargin = du.XToScreen (CX_MARGIN);
	int cyMargin = du.YToScreen (CY_MARGIN);

	HFONT hFont = (HFONT)SendDlgItemMessage (IDC_LBL1, WM_GETFONT);
	HDC hDC = ::GetDC(GetSafeHwnd());
	::SelectObject (hDC, hFont);

	// measure the text of all the labels to find the longest
	int cxLabel = 0;
	CDynaList<CUserDataField>&	listDataFields = _pUserDataCategory->ListDataFields();
	for (dw = 0 ; dw < listDataFields.GetCount() ; dw++)
	{
		CUserDataField* pUserDataField = &listDataFields[dw];
		CString strLabel(pUserDataField->Label());
		SIZE size;
		VERIFY(::GetTextExtentPoint32(hDC, strLabel, strLabel.GetLength(), &size));
		cxLabel = max(size.cx + 4, cxLabel);
	}

	// limit it to no more than 50% of available width
	int cxMaxLen = (rcClient.right - (3 * cxMargin)) / 2;
	cxLabel = min(cxLabel, cxMaxLen);
	
	// set edit control accordingly
	int cxEdit = (rcClient.right - 3 * cxMargin) - cxLabel;
	
	// height of label, plus offset from top of edit
	int cyLabel = du.YToScreen (CY_LABEL);
	int cyLabelOffset = du.YToScreen (CY_EDIT) - cyLabel;
	
	// pitch from top of one control to the next
	int cyPitch = du.YToScreen(CY_EDIT) + cyMargin;

	// starting position for first row of controls
	int y = cyMargin + 45;

	// total required height
	int cyControls = (listDataFields.GetCount() * cyPitch) + y + 40;

	// set scroll bar accordingly
	int yScroll = max (cyControls - rcClient.Height(), 0);
	SetScrollRange (SB_VERT, 0, yScroll, TRUE);

	// loop through each control
	for (dw = 0 ; dw < listDataFields.GetCount() ; dw++)
	{
		CUserDataField* pUserDataField = &listDataFields[dw];
		int x = cxMargin;

		// move the label
		CWnd * pLabel = GetDlgItem(dw + 200);
		pLabel->MoveWindow (x, y + cyLabelOffset + 5, cxLabel, cyLabel, TRUE);

		// then the edit control (or combo)
		x += (cxLabel + cxMargin);
		int cyEdit = du.YToScreen (CY_EDIT);
		if (pUserDataField->DataType() == CUserDataField::typePicklist)
			cyEdit *= 5;

		CWnd * pEdit = GetDlgItem(dw + 100);
		pEdit->MoveWindow (x, y, cxEdit, cyEdit, TRUE);

		// move down to next line
		y += cyPitch;
	}
}

void CPageUserData::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	// work out the increments for scrolling
	CDlgUnit du(this);
	int cyLine = du.YToScreen (CY_EDIT) + du.YToScreen(CY_MARGIN);
	CRect rcClient;
	GetClientRect (&rcClient);
	int cyPage = rcClient.bottom;

	// get current position
	int nNewPos = 0, nMin, nMax;
	GetScrollRange (SB_VERT, &nMin, &nMax);

	switch (nSBCode)
	{
		case SB_BOTTOM:
			nNewPos = nMax;
			break;					
		case SB_LINEDOWN:
			nNewPos = m_nScrollOffset + cyLine;
			break;
		case SB_LINEUP:
			nNewPos = m_nScrollOffset - cyLine;
			break;
		case SB_PAGEDOWN:
			nNewPos = m_nScrollOffset + cyPage;
			break;
		case SB_PAGEUP:
			nNewPos = m_nScrollOffset - cyPage;
			break;
		case SB_THUMBPOSITION:
		case SB_THUMBTRACK:
			nNewPos = nPos;
			break;
		case SB_TOP:
			nNewPos = 0;
			break;
		case SB_ENDSCROLL:
			return;
	}

	// range check
	nNewPos = max(nNewPos, 0);
	nNewPos = min(nNewPos, nMax);

	// do the scroll
	int cyToScroll = m_nScrollOffset - nNewPos;
	if (cyToScroll)
	{
		TRACE("Scrolling by %d", cyToScroll);

		// translate into pixels
		ScrollWindow (0, cyToScroll, NULL, NULL);
		SetScrollPos (SB_VERT, nNewPos, TRUE);
		m_nScrollOffset = nNewPos;
	}
//	CScannerPage::OnVScroll(nSBCode, nPos, pScrollBar);
}

BOOL CPageUserData::OnCommand(WPARAM wParam, LPARAM lParam) 
{
	TRACE("Ctrl ID = %d, Notification = %X\n", LOWORD(wParam), HIWORD(wParam));
	if (HIWORD(wParam) == EN_SETFOCUS || HIWORD(wParam) == CBN_SETFOCUS || HIWORD(wParam) == BN_SETFOCUS)
	{
		TRACE ("Command ID %d receiving focus...\n", LOWORD(wParam));

		// get the window co-ordinates
		CWnd * pCtl = GetDlgItem(LOWORD(wParam));
		CRect rcEdit;
		pCtl->GetWindowRect(&rcEdit);
		GetDesktopWindow()->MapWindowPoints(this, &rcEdit);

		// is it within this window's client area ?
		CRect rcClient;
		GetClientRect (&rcClient);

		// scroll window into place if necessary...
		while (rcEdit.bottom > rcClient.bottom)
		{
			OnVScroll(SB_LINEDOWN, 0, NULL);
			// repeat the test...
			pCtl->GetWindowRect(&rcEdit);
			GetDesktopWindow()->MapWindowPoints(this, &rcEdit);
		}
		// ...also might need to scroll upwards...
		while (rcEdit.top < 0)
		{
			OnVScroll(SB_LINEUP, 0, NULL);
			// repeat test
			pCtl->GetWindowRect(&rcEdit);
			GetDesktopWindow()->MapWindowPoints(this, &rcEdit);
		}
	}	
	return CScannerPage::OnCommand(wParam, lParam);
}
