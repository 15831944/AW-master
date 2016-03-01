//==========================================================================//
//																			//
//	Page1.cpp	: First page of the AuditWizard scanner						//
//	----------																//
//																			//
//	AUTHOR:	JRF Thornley - copyright (C) Layton Software 2001				//
//																			//
//==========================================================================//
//																			//
//  History																	//
//  --------																//
//																			//
//		40C005				Chris Drew				01-NOV-2001				//
//		Save any previous name that this asset had so that AuditWizard		//
//		can detect a name change for an asset.              				//
//																			//
//==========================================================================//

#include "stdafx.h"
#include "scan32.h"
#include "PageBasicInformation.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPageBasicInformation property page

IMPLEMENT_DYNCREATE(CPageBasicInformation, CScannerPage)

CPageBasicInformation::CPageBasicInformation() : CScannerPage(CPageBasicInformation::IDD)
{
	//{{AFX_DATA_INIT(CPageBasicInformation)
	m_strAssetName = _T("");
	m_strCat = _T("");
	m_strMake = _T("");
	m_strModel = _T("");
	m_strSerial = _T("");
	//}}AFX_DATA_INIT
}


CPageBasicInformation::~CPageBasicInformation()
{
}


/*
** Data Validation
*/
void CPageBasicInformation::DoDataExchange(CDataExchange* pDX)
{
	CScannerPage::DoDataExchange(pDX);

	//{{AFX_DATA_MAP(CPageBasicInformation)
	DDX_Text(pDX, IDC_ED_NAME, m_strAssetName);
	DDX_CBString(pDX, IDC_CB_CAT, m_strCat);
	DDX_Text(pDX, IDC_ED_MAKE, m_strMake);
	DDX_Text(pDX, IDC_ED_MODEL, m_strModel);
	DDX_Text(pDX, IDC_ED_SERIAL, m_strSerial);
	//}}AFX_DATA_MAP

	// storing data ?
	if (pDX->m_bSaveAndValidate)
	{
		// saving - don't allow user to continue unless asset has been filled
		if (m_strAssetName.IsEmpty())
		{
			AfxMessageBox ("You must enter an Asset Name", MB_ICONEXCLAMATION);
			pDX->Fail();
		}

		// save all mods back to the application class
		//GetAPP()->SetAssetName(m_strAssetName);
		GetAPP()->m_strAssetName	= m_strAssetName;
		GetAPP()->m_strCat			= m_strCat;
		GetAPP()->m_strMake			= m_strMake;
		GetAPP()->m_strModel		= m_strModel;
		GetAPP()->m_strSerial		= m_strSerial;
	}
}

BEGIN_MESSAGE_MAP(CPageBasicInformation, CScannerPage)
	//{{AFX_MSG_MAP(CPageBasicInformation)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPageBasicInformation message handlers


BOOL CPageBasicInformation::OnSetActive() 
{
	// Enable / disable appropriate buttons
	((CShtScan*)GetParent())->SetButtons();

	return CScannerPage::OnSetActive();
}

BOOL CPageBasicInformation::OnInitDialog() 
{
	// Call the base class implementation
	CScannerPage::OnInitDialog();

	// load the data from the application class
	//m_strAssetName	= GetAPP()->GetAssetName();
	m_strAssetName	= GetAPP()->m_strAssetName;
	m_strCat		= GetAPP()->m_strCat;
	m_strMake		= GetAPP()->m_strMake;
	m_strModel		= GetAPP()->m_strModel;
	m_strSerial		= GetAPP()->m_strSerial;

	UpdateData(FALSE);
	
	// populate the category combo box from the pipe delimited list passed through in the configuration
	CDynaList<CString>& listAssetTypes = GetCFG().AssetTypes();
	//
	CComboBox * pCB = (CComboBox*)GetDlgItem(IDC_CB_CAT);
	pCB->ResetContent();

	// loop through the list
	for (DWORD dw = 0 ; dw < listAssetTypes.GetCount() ; dw++)
	{
		int nIndex = pCB->AddString(listAssetTypes[dw]);
	}

	// little tweak - add the stored category if it wasn't in the list
	if (m_strCat.GetLength() && (CB_ERR == pCB->FindStringExact(-1, m_strCat)))
		pCB->AddString(m_strCat);

	// Select the category 
	int index = pCB->FindStringExact(0, m_strCat);
	if (index == -1)
		pCB->SetCurSel(0);
	else
		pCB->SetCurSel(index);
		
//    Now lets determine which fields we should display on this form as the user 
//    may disable them.
	//if (((GetCFG().FieldsMask() & CAuditScannerConfiguration::AssetName) == 0)
	//||  (GetCFG().AutoName()))
	//	GetDlgItem(IDC_ED_NAME)->EnableWindow(FALSE);

	//if ((GetCFG().FieldsMask() & CAuditScannerConfiguration::Make) == 0)
	//	GetDlgItem(IDC_ED_MAKE)->EnableWindow(FALSE);
	//
	//if ((GetCFG().FieldsMask() & CAuditScannerConfiguration::Model) == 0)
	//	GetDlgItem(IDC_ED_MODEL)->EnableWindow(FALSE);

	//if ((GetCFG().FieldsMask() & CAuditScannerConfiguration::Category) == 0)
	//	GetDlgItem(IDC_CB_CAT)->EnableWindow(FALSE);
	//
	//if ((GetCFG().FieldsMask() & CAuditScannerConfiguration::SerialNumber) == 0)
	//	GetDlgItem(IDC_ED_SERIAL)->EnableWindow(FALSE);

	//if (((GetCFG().FieldsMask() & CAuditScannerConfiguration::AssetName) == 0) ||  (GetCFG().AutoName()))
		GetDlgItem(IDC_ED_NAME)->EnableWindow(FALSE);

	if (!GetCFG().InteractiveAllowChangeManufacturer())
		GetDlgItem(IDC_ED_MAKE)->EnableWindow(FALSE);
	
	if (!GetCFG().InteractiveAllowChangeModel())
		GetDlgItem(IDC_ED_MODEL)->EnableWindow(FALSE);

	if (!GetCFG().InteractiveAllowChangeCategory())
		GetDlgItem(IDC_CB_CAT)->EnableWindow(FALSE);
	
	if (!GetCFG().InteractiveAllowChangeSerial())
		GetDlgItem(IDC_ED_SERIAL)->EnableWindow(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
}