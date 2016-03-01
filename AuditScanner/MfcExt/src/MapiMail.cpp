
// FILE:	MapiMail.cpp
// PURPOSE:	Implementation of Mapi Wrapper classes
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 07.05.2001 - written
//			JRFT - 21.10.2001 - auto name resolution taken out of send function

#include "stdafx.h"



#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

///////////////////////////////////////////////////////////////////////////////
//
// M A P I   M E S S A G E   W R A P P E R   C L A S S
//

/*
** Clear storage
*/
void CMapiMsg::Reset()
{
	m_strOriginator.Empty();
	m_To.Empty();
	m_CC.Empty();
	m_BCC.Empty();
	m_strSubject.Empty();
	m_strBody.Empty();
	m_Attachments.Empty();
	m_AttachmentTitles.Empty();
}

/*
** save an attachment (simply a file copy really)
*/
BOOL CMapiMsg::SaveAttach(DWORD dwIndex, LPCSTR pszSaveAs)
{
	// check that there are this number of attached files
	ASSERT (dwIndex < m_Attachments.GetCount());
	// do it, assuming it's not already there
	if (m_Attachments[dwIndex] != pszSaveAs)
	{
		return CopyFile (m_Attachments[dwIndex], pszSaveAs, FALSE);
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////////////////
//
//  M A P I   S E S S I O N   W R A P P E R   C L A S S
//

/*
** Construction / Destruction
*/
CMapiSession::CMapiSession()
{
	m_hSession = 0;
	m_nLastError = 0;
	m_hMapi = NULL;
	m_pfnLogon			= NULL;
	m_pfnLogoff			= NULL;
	m_pfnSendMail		= NULL;
	m_pfnResolveName	= NULL;
	m_pfnFreeBuffer		= NULL;
	m_pfnFindNext		= NULL;
	m_pfnReadMail		= NULL;
	m_pfnDeleteMail		= NULL;

	Initialise();
}

CMapiSession::~CMapiSession()
{
	//Logoff if logged on
	if (LoggedOn())
		Logoff();

	//Unload the MAPI dll
	Deinitialise();
}

/*
** Connect to MAPI dll and map functions
*/
void CMapiSession::Initialise() 
{
	// First make sure the "WIN.INI" entry for MAPI is present as well as the actual MAPI32 dll
	BOOL bMapiInstalled = (GetProfileInt(_T("MAIL"), _T("MAPI"), 0) != 0) && 
		(SearchPath(NULL, _T("MAPI32.DLL"), NULL, 0, NULL, NULL) != 0);
	
	if (bMapiInstalled)
	{
		// Load the MAPI dll and get the function pointers used
		m_hMapi = LoadLibrary(_T("MAPI32.DLL"));
		if (m_hMapi)
		{
			m_pfnLogon			= (LPMAPILOGON)			GetProcAddress(m_hMapi, "MAPILogon");
			m_pfnLogoff			= (LPMAPILOGOFF)		GetProcAddress(m_hMapi, "MAPILogoff");
			m_pfnSendMail		= (LPMAPISENDMAIL)		GetProcAddress(m_hMapi, "MAPISendMail");
			m_pfnResolveName	= (LPMAPIRESOLVENAME)	GetProcAddress(m_hMapi, "MAPIResolveName");
			m_pfnFreeBuffer		= (LPMAPIFREEBUFFER)	GetProcAddress(m_hMapi, "MAPIFreeBuffer");
			m_pfnFindNext		= (LPMAPIFINDNEXT)		GetProcAddress(m_hMapi, "MAPIFindNext");
			m_pfnReadMail		= (LPMAPIREADMAIL)		GetProcAddress(m_hMapi, "MAPIReadMail");
			m_pfnDeleteMail		= (LPMAPIDELETEMAIL)	GetProcAddress(m_hMapi, "MAPIDeleteMail");

			//If any of the functions are not installed then fail the load
			if (m_pfnLogon == NULL
			||	m_pfnLogoff == NULL
			||	m_pfnSendMail == NULL
			||	m_pfnResolveName == NULL
			||	m_pfnFreeBuffer == NULL
			||	m_pfnFindNext == NULL
			||	m_pfnReadMail == NULL
			||	m_pfnDeleteMail == NULL)
			{
				TRACE(_T("Failed to get one of the functions pointer in MAPI32.DLL\n"));
				Deinitialise();
			}
		}
	}
	else
		TRACE(_T("Mapi is not installed on this computer\n"));
}

/*
** Release the loaded MAPI dll
*/
void CMapiSession::Deinitialise()
{
	if (m_hMapi)
	{
		// Unload the MAPI dll and reset the function pointers to NULL
		FreeLibrary(m_hMapi);
		m_hMapi				= NULL;
		m_pfnLogon			= NULL;
		m_pfnLogoff			= NULL;
		m_pfnSendMail		= NULL;
		m_pfnResolveName	= NULL;
		m_pfnFreeBuffer		= NULL;
		m_pfnFindNext		= NULL;
		m_pfnReadMail		= NULL;
		m_pfnDeleteMail		= NULL;
	}
}

/*
** Log on to the mapi session
*/
BOOL CMapiSession::Logon (const CString& strProfileName, const CString & strPassword/* = CString()*/, CWnd* pParentWnd/* = NULL*/)
{
	ASSERT(Installed());	// MAPI must be installed
	ASSERT(m_pfnLogon);		// Function pointer must be valid
	BOOL bSuccess = FALSE;

	// Just in case we are already logged in
	Logoff();

	// see which strings are populated
	int nProfileLength = strProfileName.GetLength();
//	int nPasswordLength = strPassword.GetLength();
	LPSTR pszProfileName = NULL;
	LPSTR pszPassword = NULL;
	if (nProfileLength)
	{
		pszProfileName = (LPTSTR)(LPCTSTR)strProfileName;
		pszPassword = (LPTSTR)(LPCTSTR)strPassword;
	}

	// Setup the flags & UI parameters used in the MapiLogon call
	FLAGS flags = 0;
	ULONG nUIParam = 0;
	if (!nProfileLength)
	{
		// No profile name given, then we must interactively request a profile name
		if (pParentWnd)
		{
			nUIParam = (ULONG) pParentWnd->GetSafeHwnd();
			flags |= MAPI_LOGON_UI;
		}
		else
		{
			// No CWnd given, just use the main window of the app as the parent window
			if (AfxGetMainWnd())
			{
				nUIParam = (ULONG) AfxGetMainWnd()->GetSafeHwnd();
				flags |= MAPI_LOGON_UI;
			}
		}
	}
	
	// First try to acquire a new MAPI session using the supplied settings using the MAPILogon functio
	m_nLastError = m_pfnLogon (nUIParam,		// Window handle (if used)
		pszProfileName,							// Profile Name (if used)
		pszPassword,							// Password (if used)
		flags | MAPI_NEW_SESSION,				// flags
		0,										// reserved
		&m_hSession);							// returned session handle
	if (m_nLastError != SUCCESS_SUCCESS && m_nLastError != MAPI_E_USER_ABORT)
	{
		// Failed to create a new mapi session, try to acquire a shared mapi session
		TRACE(_T("Failed to logon to MAPI using a new session, trying to acquire a shared one\n"));
		m_nLastError = m_pfnLogon (nUIParam,	// Window Handle (if supplied)
			NULL,								// Profile Name
			NULL,								// Password
			0,									// flags
			0,									// reserved
			&m_hSession);						// returned session handle
		if (m_nLastError == SUCCESS_SUCCESS)
		{
			bSuccess = TRUE;
		}
		else
		{
			TRACE(_T("Failed to logon to MAPI using a shared session, Error:%d\n"), m_nLastError);
		}
	}
	else if (m_nLastError == SUCCESS_SUCCESS)
	{
		bSuccess = TRUE;
	}

	return bSuccess;
}

/*
** Disconnect an active session
*/
BOOL CMapiSession::Logoff()
{
	ASSERT(Installed());	// MAPI must be installed
	ASSERT(m_pfnLogoff);	// Function pointer must be valid
	BOOL bSuccess = FALSE;

	if (m_hSession)
	{
		// Call the MAPILogoff function
		m_nLastError = m_pfnLogoff (m_hSession,	// active session
			0,									// Window handle
			0,									// flags
			0);									// reserved
		if (m_nLastError != SUCCESS_SUCCESS)
		{
			TRACE(_T("Failed in call to MapiLogoff, Error:%d"), m_nLastError);
			bSuccess = FALSE;
		}
		else
		{
			bSuccess = TRUE;
		}
		m_hSession = 0;
	}    
	return bSuccess;
}

/*
** Resolve an email address
*/
BOOL CMapiSession::Resolve(const CString & strName, lpMapiRecipDesc* lppRecip)
{
	ASSERT(Installed());		// MAPI must be installed
	ASSERT(m_pfnResolveName);	// Function pointer must be valid
	ASSERT(LoggedOn());			// Must be logged on to MAPI
	ASSERT(m_hSession);			// MAPI session handle must be valid

	// Call the MAPIResolveName function
	LPSTR lpszAsciiName = (LPTSTR)(LPCTSTR)strName;
	m_nLastError = m_pfnResolveName (m_hSession,	// active session
		0,											// window handle
		lpszAsciiName,								// name to be resolved
		MAPI_DIALOG,								// flags (run a dialog if name can't be resolved)
		0,											// reserved
		lppRecip);									// returns a new recipient structure if successful
	
	if (m_nLastError != SUCCESS_SUCCESS)
	{
		TRACE(_T("Failed to resolve the name: %s, Error:%d\n"), strName, m_nLastError);
	}
	return (m_nLastError == SUCCESS_SUCCESS);
}

/*
** Send a message
*/
BOOL CMapiSession::Send(CMapiMsg& message)
{
	ASSERT(Installed());		// MAPI must be installed
	ASSERT(m_pfnSendMail);		// Function pointer must be valid
	ASSERT(m_pfnFreeBuffer);	// Function pointer must be valid
	ASSERT(LoggedOn());			// Must be logged on to MAPI

	// Initialise the function return value
	BOOL bSuccess = FALSE;  

	// Create the MapiMessage structure to match the message parameter sent into us
	MapiMessage mapiMessage;
	ZeroMemory(&mapiMessage, sizeof(mapiMessage));
	mapiMessage.lpszSubject = (LPSTR)(LPCSTR)message.m_strSubject;
	mapiMessage.lpszNoteText = (LPSTR)(LPCSTR)message.m_strBody;
	mapiMessage.nRecipCount = message.m_To.GetCount() + message.m_CC.GetCount() + message.m_BCC.GetCount(); 
	ASSERT(mapiMessage.nRecipCount); //Must have at least 1 recipient!

	// Allocate the recipients array & temp array for names
	mapiMessage.lpRecips = new MapiRecipDesc[mapiMessage.nRecipCount];
	CDynaList<CString> strNames;

	// Setup the "To" recipients
	int nRecipIndex = 0;
	DWORD dwToSize = message.m_To.GetCount();
	for (DWORD dw = 0 ; dw < dwToSize; dw++)
	{
		MapiRecipDesc& recip = mapiMessage.lpRecips[nRecipIndex];
		ZeroMemory(&recip, sizeof(MapiRecipDesc));
		recip.ulRecipClass = MAPI_TO;
		strNames.Add(message.m_To[dw]);

/*		//Try to resolve the name
		lpMapiRecipDesc lpTempRecip;  
		if (Resolve(strNames[nRecipIndex], &lpTempRecip))
		{
			// Resolve worked, put the resolved name back
			strNames[nRecipIndex] = lpTempRecip->lpszName;
			// Free up the memory allocated by MAPI
			m_pfnFreeBuffer(lpTempRecip);
		}
*/		recip.lpszName = (LPSTR)(LPCSTR)strNames[nRecipIndex++];
	}

	// Setup the "CC" recipients
	DWORD dwCCSize = message.m_CC.GetCount();
	for (DWORD dw = 0; dw < dwCCSize ; dw++)
	{
		MapiRecipDesc& recip = mapiMessage.lpRecips[nRecipIndex];
		ZeroMemory(&recip, sizeof(MapiRecipDesc));
		recip.ulRecipClass = MAPI_CC;
		strNames[nRecipIndex] = message.m_CC[dw];
		recip.lpszName = (LPSTR)(LPCSTR)strNames[nRecipIndex++];
	}

	// Setup the "BCC" recipients
	DWORD dwBCCSize = message.m_BCC.GetCount();
	for (DWORD dw = 0 ; dw < dwBCCSize ; dw++)
	{
		MapiRecipDesc& recip = mapiMessage.lpRecips[nRecipIndex];
		ZeroMemory(&recip, sizeof(MapiRecipDesc));
		recip.ulRecipClass = MAPI_BCC;
		strNames[nRecipIndex] = message.m_BCC[dw];
		recip.lpszName = (LPSTR)(LPCSTR)strNames[nRecipIndex++];
	}

	// Setup the attachments 
	DWORD dwAttachmentSize = message.m_Attachments.GetCount();
	DWORD dwTitleSize = message.m_AttachmentTitles.GetCount();
	if (dwTitleSize)
	{ 
		ASSERT(dwTitleSize == dwAttachmentSize); //If you are going to set the attachment titles then you must set 
		//the attachment title for each attachment
	}
	if (dwAttachmentSize)
	{
		mapiMessage.nFileCount = dwAttachmentSize;
		mapiMessage.lpFiles = new MapiFileDesc[dwAttachmentSize];
		for (DWORD dw = 0 ; dw < dwAttachmentSize ; dw++)
		{
			MapiFileDesc & file = mapiMessage.lpFiles[dw];
			ZeroMemory(&file, sizeof(MapiFileDesc));
			file.nPosition = 0xFFFFFFFF;
			CString & strFilename = message.m_Attachments[dw];
			file.lpszPathName = (LPSTR)(LPCSTR)strFilename;
			file.lpszFileName = file.lpszPathName;
			if (dwTitleSize)
			{
				CString & strTitle = message.m_AttachmentTitles[dw];
				file.lpszFileName = (LPSTR)(LPCSTR)strTitle;
			}
		}
	}

	//Do the actual send using MAPISendMail
	m_nLastError = m_pfnSendMail (m_hSession,	// session handle
		0,										// window handle
		&mapiMessage,							// message to send
		0,										// flags
		0);										// reserved
	if (m_nLastError == SUCCESS_SUCCESS)
	{
		bSuccess = TRUE;
	}
	else
	{
		TRACE(_T("Failed to send mail message, Error:%d\n"), m_nLastError);
	}

	//Tidy up the Attachements
	if (dwAttachmentSize)
		delete [] mapiMessage.lpFiles;
	
	//Free up the Recipients memory
	delete [] mapiMessage.lpRecips;

	return bSuccess;
}

/*
** Scan incoming mail folder for messages, returns count of messages found
*/
DWORD CMapiSession::ScanMessages (BOOL bUnreadOnly/* = TRUE*/)
{
	ASSERT (Installed());
	ASSERT (m_pfnFindNext);
	ASSERT (LoggedOn());

	// empty current storage (if any)
	m_msgIdList.Empty();

	// then run through any mails and add their IDs to the list
	char szThisMsg[512];
	memset (szThisMsg, 0, 512);
	while (SUCCESS_SUCCESS == m_pfnFindNext (m_hSession,		// active session
		0,														// window handle
		NULL,													// message type filter
		szThisMsg,												// seed message ID	
		MAPI_LONG_MSGID | bUnreadOnly ? MAPI_UNREAD_ONLY : 0,	// flags
		0,														// reserved
		szThisMsg))												// returned message ID
	{
		m_msgIdList.Add(szThisMsg);
	}
	return m_msgIdList.GetCount();
}


/*
** Read a mail message following a successful call to ScanMessages
*/
BOOL CMapiSession::ReadMessage(DWORD dwIndex, CMapiMsg & msg, BOOL bHeaderOnly)
{
	ASSERT (Installed());
	ASSERT (m_pfnReadMail);
	ASSERT (LoggedOn());
	ASSERT (dwIndex < m_msgIdList.GetCount());

	// format up the ID, then call the MAPI function
	lpMapiMessage pTempMsg;
	char szMsgID[512];
	strcpy (szMsgID, m_msgIdList[dwIndex]);

	m_nLastError = m_pfnReadMail (m_hSession,				// active session
		0,													// window handle
		szMsgID,											// message ID to read 
		bHeaderOnly ? MAPI_PEEK | MAPI_ENVELOPE_ONLY : 0,	// flags
		0,													// reserved
		&pTempMsg);											// returned message structure
	if (SUCCESS_SUCCESS == m_nLastError)
	{
		// it worked! write everything into the caller's message structure
		msg.Reset();
		msg.m_strOriginator	= pTempMsg->lpOriginator->lpszName;
		// loop through the rest so as to put in the right list
		for (ULONG nRecip = 0 ; nRecip < pTempMsg->nRecipCount ; nRecip++)
		{
			switch (pTempMsg->lpRecips[nRecip].ulRecipClass)
			{
				case MAPI_TO:
					msg.m_To.Add(pTempMsg->lpRecips[nRecip].lpszName);
					break;
				case MAPI_CC:
					msg.m_CC.Add(pTempMsg->lpRecips[nRecip].lpszName);
					break;
				case MAPI_BCC:
					msg.m_BCC.Add(pTempMsg->lpRecips[nRecip].lpszName);
					break;
				default:
					ASSERT(FALSE);
			}
		}
		// subject and text
		msg.m_strSubject = pTempMsg->lpszSubject;
		msg.m_strBody = pTempMsg->lpszNoteText;
		// loop through attachments
		for (ULONG nAttach = 0 ; nAttach < pTempMsg->nFileCount ; nAttach++)
		{
			msg.m_Attachments.Add(pTempMsg->lpFiles[nAttach].lpszPathName);
			msg.m_AttachmentTitles.Add(pTempMsg->lpFiles[nAttach].lpszFileName);
		}

		// ok, let the MAPI system clear allocated memory
		m_pfnFreeBuffer(pTempMsg);
		return TRUE;
	}
	else
		return FALSE;
}

/*
** Delete a message following a successfull call to ScanMessages
*/
BOOL CMapiSession::DeleteMessage(DWORD dwIndex)
{
	ASSERT (Installed());
	ASSERT (m_pfnDeleteMail);
	ASSERT (LoggedOn());

	// get the message ID to delete
	ASSERT (dwIndex < m_msgIdList.GetCount());
	char szMsgID[512];
	strcpy (szMsgID, m_msgIdList[dwIndex]);
	// do it
	UINT nError = m_pfnDeleteMail (m_hSession,	// session handle
		0,										// parent window handle
		szMsgID,								// message ID to delete
		0,										// flags always zero for this call
		0);										// reserved
	if (nError != SUCCESS_SUCCESS)
	{
		TRACE(_T("Failed to send mail message, Error:%d\n"), nError);
	}
	m_nLastError = nError;
	return (nError != SUCCESS_SUCCESS);
}

