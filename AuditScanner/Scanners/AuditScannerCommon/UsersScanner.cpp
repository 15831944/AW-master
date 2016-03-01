#include "stdafx.h"
#include <lm.h>

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "UsersScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Local User Accounts"
#define V_USER_NAME					"Name"
#define V_USER_STATUS				"Account Disabled"
#define V_USER_DESCRIPTION			"Description"
#define V_USER_LASTLOGIN			"Last Login"
#define V_USER_LOGONCOUNT			"Logon Count"


// Function pointer for dynamic linking
typedef NET_API_STATUS (NET_API_FUNCTION* pNetUserEnum)	(LPCWSTR    servername,
														 DWORD      level,
														 DWORD      filter,
														 LPBYTE     *bufptr,
														 DWORD      prefmaxlen,
														 LPDWORD    entriesread,
														 LPDWORD    totalentries,
														 LPDWORD	resume_handle);


typedef NET_API_STATUS (NET_API_FUNCTION* pNetApiBufferFree)	(LPVOID Buffer);


CUsersScanner::CUsersScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
}

CUsersScanner::~CUsersScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CUsersScanner::ScanWMI(CWMIScanner *pScanner)
{
	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool CUsersScanner::ScanRegistryXP()
{
	try
	{
		ScanUsers();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool CUsersScanner::ScanRegistryNT()
{
	try
	{
		ScanUsers();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool CUsersScanner::ScanRegistry9X()
{
	try
	{
		ScanUsers();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}



//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CUsersScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CUsersScanner::SaveData Start" ,true);

	CString name;
	if (_listUsers.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		//CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		//pAuditDataFile->AddAuditDataFileItem(mainCategory);

		for (int isub=0; isub < (int)_listUsers.GetCount(); isub++)
		{
			// Format the hardware class name for this drive
			CUserAccount* pUser = &_listUsers[isub];
			name.Format("%s|%s" ,HARDWARE_CLASS ,pUser->User());

			// Each User has its own category - Users are GROUPED
			CAuditDataFileCategory category(name ,TRUE ,TRUE);

			// Each audited item gets added an a CAuditDataFileItem to the category
			if (pUser->IsDisabled())
			{
				CAuditDataFileItem u1(V_USER_STATUS ,"True");
				category.AddItem(u1);
			}
			else
			{
				CAuditDataFileItem u1(V_USER_STATUS ,"False");
				category.AddItem(u1);
			}

			CAuditDataFileItem u2(V_USER_DESCRIPTION ,pUser->Description());
			CAuditDataFileItem u3(V_USER_LASTLOGIN ,DateTimeToString(pUser->LastLogon()));
			CAuditDataFileItem u4(V_USER_LOGONCOUNT ,pUser->LogonCount());

			// Add the items to the category
			category.AddItem(u2);
			category.AddItem(u3);
			category.AddItem(u4);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CUsersScanner::SaveData End" ,true);
	return true;
}



//
//    ScanUsers
//    =========
//
//    This function does the buulk of the work if WMI has not been successful.  It is used for all OS
//    platforms
//
void CUsersScanner::ScanUsers()
{
	LPUSER_INFO_3 pBuf = NULL;
	LPUSER_INFO_3 pTmpBuf;
	DWORD dwLevel = 3;
	DWORD dwPrefMaxLen = MAX_PREFERRED_LENGTH;
	DWORD dwEntriesRead = 0;
	DWORD dwTotalEntries = 0;
	DWORD dwResumeHandle = 0;
	DWORD i;
	NET_API_STATUS nStatus;
	LPTSTR pszServerName = NULL;
	pNetUserEnum		fnNetUserEnum;				// Function pointer
	pNetApiBufferFree	fnNetApiBufferFree;			// Function pointer

	CLogFile log;
	log.Write("CUsersScanner::ScanUsers Start" ,true);

	try
	{
		// First task is to try and load the NetApi32.lib library - if this does nbot exist or we cannot
		// locate the function then we cannot procede
		HINSTANCE hNetapi32 = LoadLibrary("Netapi32.dll");
		if (hNetapi32 < (HINSTANCE) HINSTANCE_ERROR) 
		{
			log.Write("Failed to load the 'Netapi32.dll' module, exiting ScanUsers"  ,true);
			return;
		}

		// Get the function addresses that we will need.  If any are not available then return
		fnNetUserEnum = (pNetUserEnum) GetProcAddress(hNetapi32, "NetUserEnum");
		fnNetApiBufferFree = (pNetApiBufferFree) GetProcAddress(hNetapi32, "NetApiBufferFree");
		if (fnNetApiBufferFree == NULL || fnNetUserEnum == NULL)
		{
			log.Write("Failed to obtain the function address of 'NetUserEnum' or 'NetApiBufferFree', exiting ScanUsers"  ,true);
			return;
		}

		// Ensure that the list is empty
		_listUsers.Empty();

		do 
		{
			nStatus = fnNetUserEnum (NULL
									,dwLevel
									,FILTER_NORMAL_ACCOUNT
									,(LPBYTE*)&pBuf
									,dwPrefMaxLen
									,&dwEntriesRead
									,&dwTotalEntries
									,&dwResumeHandle);

			// If the call succeeds,
			if ((nStatus == NERR_Success) || (nStatus == ERROR_MORE_DATA))
			{
				if ((pTmpBuf = pBuf) != NULL)
				{
					// Loop through the entries.
					for (i = 0; (i < dwEntriesRead); i++)
					{
						if (pTmpBuf == NULL)
							return;

						// Add this user to our list
						CString strUser = pTmpBuf->usri3_name;
						CString strDescription = pTmpBuf->usri3_comment;
						CString strFullName = pTmpBuf->usri3_full_name;
						BOOL bDisabled = (pTmpBuf->usri3_flags & UF_ACCOUNTDISABLE);
						CTime ctLastLogon = pTmpBuf->usri3_last_logon;
						int nLogonCount = pTmpBuf->usri3_num_logons;

						CUserAccount us(strUser ,strFullName ,strDescription ,bDisabled ,ctLastLogon ,nLogonCount);
						_listUsers.Add(us);
						pTmpBuf++;
					}
				}
			}

			// Otherwise, print the system error.
			else
				return;

			// Free the allocated buffer.
			if (pBuf != NULL)
			{
				fnNetApiBufferFree(pBuf);
				pBuf = NULL;
			}
		}

		// Continue to call NetUserEnum while there are more entries. 
		while (nStatus == ERROR_MORE_DATA);

		// Check again for allocated memory.
		if (pBuf != NULL)
			fnNetApiBufferFree(pBuf);

		FreeLibrary(hNetapi32);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	log.Write("CUsersScanner::ScanUsers End" ,true);
}
