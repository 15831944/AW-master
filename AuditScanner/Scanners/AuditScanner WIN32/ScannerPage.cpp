// ScannerPage.cpp: implementation of the CScannerPage class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "scan32.h"
#include "ScannerPage.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
IMPLEMENT_DYNCREATE(CScannerPage, CPropertyPage)

CScannerPage::CScannerPage() 
{
	m_nScanStatus = SCANNING;
}

CScannerPage::CScannerPage(UINT nIDTemplate) 
	: CPropertyPage(nIDTemplate)
{
	m_nScanStatus = SCANNING;
}

CScannerPage::~CScannerPage()
{

}

BOOL CScannerPage::OnQueryCancel() 
{
	// can't cancel if in non-interactive mode or if the CANCEL button has been disabled
	/*if ((GetAPP()->ScannerMode() == CAuditScannerConfiguration::modeNonInteractive) 
	||  ((GetCFG().FieldsMask() & CAuditScannerConfiguration::Cancel) == 0))
		return FALSE;*/

	if (!GetCFG().InteractiveAllowCancel())
	{
		AfxMessageBox ("The administrator has specified that this audit cannot be cancelled.", MB_ICONINFORMATION);
		return FALSE;
	}

	if (m_nScanStatus != CANCELLED)
	{
		// check with user that they really, really want to
		if (IDYES == AfxMessageBox("Are you sure you want to cancel the audit?", MB_YESNO, MB_ICONQUESTION)) 
		{
			m_nScanStatus = CANCELLED;
			return TRUE;
		}
	}

	return FALSE;
}

