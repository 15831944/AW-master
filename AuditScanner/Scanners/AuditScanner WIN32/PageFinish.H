#pragma once

// FILE:	PageFinish.h
// PURPOSE:	Declaration of Property Page for saving Audit Scanner data
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 20.03.2001 - written

#include "ScannerPage.h"
#include "../AuditScannerCommon/CkSFtp.h"

/////////////////////////////////////////////////////////////////////////////
// CPageFinish dialog

class CPageFinish : public CScannerPage
{
	DECLARE_DYNCREATE(CPageFinish)

// Construction
public:
	CPageFinish();
	~CPageFinish();

// Dialog Data
	//{{AFX_DATA(CPageFinish)
	enum { IDD = IDD_PAGEFINISH };
	CListBox	m_lbResults;
	//}}AFX_DATA
	CString		m_strBaseName;

	// TCP Connection
	CTcpClient		m_tcp;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPageFinish)
	public:
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void SaveData();
	CString BuildFileName(LPCSTR strExt);	// Build a full storage file name
	// Generated message map functions
	//{{AFX_MSG(CPageFinish)
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

	// Rather than including the profile (which is AudWiz specific) lets just define
	// the scanner 
	enum eDeploy { deployManual, deployNetwork, deployEmail, deployWeb };

	// Save the resultant audit file to various locations
	void SaveResultsToTemp			(CString& outputAuditFileName);
	void SaveResultsToFtp			(CString& outputAuditFileName);
	void SaveResultsToSftp			(CString& outputAuditFileName);
	void SaveResultsToFtpBackup		(CString& outputAuditFileName);
	void SaveResultsToSftpBackup	(CString& outputAuditFileName);
	void SaveResultsToEmail			(CString& outputAuditFileName);
	void SaveResultsToTcp			(CString& outputAuditFileName);
	void SaveResultsToNetwork		(void);
	bool CheckFileExists			(CString& outputAuditFileName, CkSFtp& sftp);

	// Static function to use for TCP callbacks
	static void MsgCallback (CString pszMessage, LPARAM lParam);

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.