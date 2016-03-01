
// FILE:	CMapi.h
// PURPOSE:	Wrapper classes for MAPI email
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 07.05.01 - written
#pragma once

#include <mapi.h>
#include <mapix.h>

/*
** Class to contain a MAPI mail message
*/
class CMapiMsg
{
public:
	void Reset();
	BOOL SaveAttach(DWORD dwIndex, LPCSTR pszSaveAs);
public:
	CString				m_strOriginator;
	CDynaList<CString>	m_To;
	CDynaList<CString>	m_CC;
	CDynaList<CString>	m_BCC;
	CString				m_strSubject;			// The Subject of the message
	CString				m_strBody;				// The Body of the message
	CDynaList<CString>	m_Attachments;			// Files attached to the message
	CDynaList<CString>	m_AttachmentTitles;		// Titles to display for attached files
};

/*
** Class to handle a MAPI connection
*/
class CMapiSession
{
public:
	// Construction / Destruction
	CMapiSession();
	~CMapiSession();

	// return TRUE if mapi library is installed and available
	BOOL Installed() const
		{ return (m_hMapi != NULL); }
	// return TRUE if successfully logged on to a MAPI session
	BOOL LoggedOn() const	
		{ return (m_hSession != 0); }
	// return last error code
	ULONG GetLastError() const
		{ return m_nLastError; }
	// Display last MAPI error message
	void HandleMapiError ();

	// Logon / Logoff
	BOOL Logon(const CString& sProfileName, const CString& sPassword = CString(), CWnd* pParentWnd = NULL);
	BOOL Logoff();

	// Send a message
	BOOL Send(CMapiMsg& msg);

	// receive / delete messages
	DWORD ScanMessages (BOOL bUnreadOnly = TRUE);
	BOOL ReadMessage (DWORD dwIndex, CMapiMsg & msg, BOOL bHeaderOnly);
	BOOL DeleteMessage (DWORD dwIndex);
	
protected:
	void Initialise();
	void Deinitialise(); 
	// resolve entered name against address book
	BOOL Resolve(const CString& sName, lpMapiRecipDesc* lppRecip);

protected:
	LHANDLE				m_hSession;			// Mapi Session handle
	ULONG				m_nLastError;		// Last Mapi error value
	HINSTANCE			m_hMapi;			// Instance handle of the MAPI dll
	LPMAPILOGON			m_pfnLogon;			// MAPILogon function pointer
	LPMAPILOGONEX		m_pfnLogonEx;		// Extended version
	LPMAPILOGOFF		m_pfnLogoff;		// MAPILogoff function pointer
	LPMAPISENDMAIL		m_pfnSendMail;		// MAPISendMail function pointer
	LPMAPIRESOLVENAME	m_pfnResolveName;	// MAPIResolveName function pointer
	LPMAPIFREEBUFFER	m_pfnFreeBuffer;	// MAPIFreeBuffer function pointer
	LPMAPIFINDNEXT		m_pfnFindNext;
	LPMAPIREADMAIL		m_pfnReadMail;
	LPMAPIDELETEMAIL	m_pfnDeleteMail;	// MAPIDeleteMail function pointer
	CDynaList<CString>	m_msgIdList;		// list of currently available messages to read
};
