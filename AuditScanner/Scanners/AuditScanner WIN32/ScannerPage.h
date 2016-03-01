#pragma once
#include <afxdlgs.h>
// ScannerPage.h: interface for the CScannerPage class.
//
//////////////////////////////////////////////////////////////////////

class CScannerPage : public CPropertyPage  
{
	DECLARE_DYNCREATE(CScannerPage)

public:
	CScannerPage();
	CScannerPage(UINT nIDTemplate);
	virtual ~CScannerPage();

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CScannerPage)
	public:
	virtual BOOL OnQueryCancel();
	//}}AFX_VIRTUAL

	enum { SCANNING, CANCELLED ,COMPLETE};

protected:
	int		m_nScanStatus;
};
