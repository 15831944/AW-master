
// code to enumerate which security groups a user is a member of

#ifndef _USERGROUPS_DEF_
#define _USERGROUPS_DEF_

#include "lmcons.h"
#include "lmaccess.h"
#include "lmerr.h"
#include "lmapibuf.h"
#include "lmwksta.h"

#pragma warning (push)
#pragma warning (disable : 4706)

// Typedefs for getting function pointers
typedef BOOL (WINAPI * PFNENUMPROCESSES)(
    DWORD * lpidProcess,
    DWORD   cb,
    DWORD * cbNeeded
	);


/*
** Convert a string to wide format
*/
void inline ConvertCToT (WCHAR * pszDest, const CHAR * pszSrc)
{
	while (*pszDest++ = (WCHAR)*pszSrc++);
}

void inline ConvertTToC (CHAR * pszDest, const WCHAR * pszSrc)
{
	while (*pszDest++ = (CHAR)*pszSrc++);
}
#pragma warning (pop)

DWORD inline enumGroupsForUser (LPCSTR pszUserName, CDynaList<CString> & groups)
{
	DWORD dwResult = 0;

	// first get the active Primary Domain Controller
	LPWORD pPDC;
	LPBYTE pGroups;
	NET_API_STATUS status = NetGetAnyDCName (NULL, NULL, (LPBYTE*)&pPDC);
	if (NERR_Success == status)
	{
		DWORD dwMaxEntries;

		// User name needs conversion to WCHAR
		CString strUser = pszUserName;
		WCHAR *wUser = new WCHAR[strUser.GetLength()+1];
        MultiByteToWideChar(CP_ACP, 0, strUser, -1, wUser, strUser.GetLength()+1);

		// try and enumerate groups
		status = NetUserGetGroups ((LPCWSTR)pPDC, wUser, 0, &pGroups, MAX_PREFERRED_LENGTH, &dwResult, &dwMaxEntries);

		delete [] wUser;

		if (NERR_Success == status)
		{
			// iterate through them
			LPGROUP_USERS_INFO_0 pTmp = (LPGROUP_USERS_INFO_0)pGroups;
			 
			for (DWORD dw = 0 ; dw < dwResult ; dw++, pTmp++)
			{
				ASSERT(pTmp);
				groups.Add (pTmp->grui0_name);
			}
			NetApiBufferFree (pGroups);
		}
		NetApiBufferFree (pPDC);
	}
	return dwResult;
}

#include <Tlhelp32.h>

HANDLE inline GetExplorerProcessHandle()  //Needed to impersonate the logged in user...
{
	// ToolHelp Function Pointers.
	HANDLE (WINAPI *lpfCreateToolhelp32Snapshot)(DWORD, DWORD);
	BOOL (WINAPI *lpfProcess32First)(HANDLE, LPPROCESSENTRY32);
	BOOL (WINAPI *lpfProcess32Next)(HANDLE, LPPROCESSENTRY32);

	// Other data
	HMODULE hKernel32 = NULL;
	PROCESSENTRY32 pe32;
	ZeroMemory (&pe32,sizeof(pe32));
	HANDLE temp = NULL;
	
	try
	{
		hKernel32 = ::LoadLibrary("KERNEL32.DLL");
		if (hKernel32 == NULL)
			throw "Failed to load kernel32";

		lpfCreateToolhelp32Snapshot = 
			(HANDLE (WINAPI *)(DWORD,DWORD))
            GetProcAddress(hKernel32, "CreateToolhelp32Snapshot");

		lpfProcess32First =
			(BOOL (WINAPI *)(HANDLE,LPPROCESSENTRY32))
            GetProcAddress(hKernel32, "Process32First");

		lpfProcess32Next =
			(BOOL (WINAPI *)(HANDLE,LPPROCESSENTRY32))
            GetProcAddress(hKernel32, "Process32Next");
		
		// If we failed to get any of the above functions then we must bail
		if (lpfProcess32Next == NULL || lpfProcess32First == NULL || lpfCreateToolhelp32Snapshot == NULL)
			throw "Failed to obtain function pointers";

		// OK - no problems withthe functions so continue
		BOOL   bResult    = FALSE;
		HANDLE hSnapshot  = INVALID_HANDLE_VALUE;

		hSnapshot = lpfCreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
		{
			if (INVALID_HANDLE_VALUE != hSnapshot)
			{
				pe32.dwSize = sizeof(PROCESSENTRY32); 
					
				if (lpfProcess32First (hSnapshot, &pe32))
				{
					do
					{
						TRACE ("Process is %s\n",pe32.szExeFile);
						if (!_stricmp(pe32.szExeFile,"explorer.exe"))
						{
							temp = OpenProcess (PROCESS_ALL_ACCESS,FALSE, pe32.th32ProcessID); 
							break;
						}
					} 
					while (lpfProcess32Next(hSnapshot,&pe32));
				}
			
				// Close the process handle again
				CloseHandle (hSnapshot);
			}
		}
	}

	catch (char *str)
	{
		CLogFile log;
		log.Format ("GetExplorerProcessHandle() error >>%s<<", str);
    }
	catch (DWORD)
	{
	}

	// Close any open library
	if (hKernel32 != NULL)
		::FreeLibrary(hKernel32);

    return temp;
}

CString inline GetActiveUser ()
{
	CString strUser;

	HANDLE hToken;
	TOKEN_USER		oUser[16];
	DWORD			u32Needed;
	TCHAR			szUserName[256], szDomainName[256];
	DWORD			userNameSize, domainNameSize;
	SID_NAME_USE	sidType;
	
	ZeroMemory (oUser, sizeof(oUser));
	ZeroMemory (szUserName, sizeof(szUserName));
	ZeroMemory (szDomainName, sizeof(szDomainName));
	
	HANDLE hExplorer = GetExplorerProcessHandle();
	if (hExplorer)
	{
		if (OpenProcessToken (hExplorer, TOKEN_QUERY, &hToken))
		{
			GetTokenInformation (hToken,TokenUser, &oUser[0], sizeof(oUser), &u32Needed);
			userNameSize = sizeof (szUserName) - 1;
			domainNameSize = sizeof (szDomainName) - 1;
			
			LookupAccountSid (NULL, oUser[0].User.Sid, szUserName, &userNameSize, szDomainName, &domainNameSize, &sidType);

			CloseHandle (hToken);
		}
		CloseHandle (hExplorer);
	}

	strUser = szUserName;
	return strUser;
}
/*
CString inline GetActiveUser ()
{
	CString strResult;

	LPBYTE pPDC;
	NET_API_STATUS status = NetGetAnyDCName (NULL, NULL, &pPDC);
	if (NERR_Success == status)
	{
/*		DWORD dwMaxEntries;

		// *** NB. Note really sure whether PDC name is already in wide char format or not... ***
		WCHAR * pwszPDC = new WCHAR[strlen((char*)pPDC) + 1];
		ConvertCToT (pwszPDC, (char*)pPDC);
		// User name should definitely need conversion
		WCHAR * pwszUserName = new WCHAR[strlen(pszUserName) + 1];
		ConvertCToT (pwszUserName, pszUserName);
		
		// try and enumerate groups
		status = NetUserGetGroups (pwszPDC, pwszUserName, 0, &pGroups, MAX_PREFERRED_LENGTH, &dwResult, &dwMaxEntries);

		delete [] pwszUserName;
		delete [] pwszPDC;

		if (NERR_Success == status)
		{
			// iterate through them
			LPGROUP_USERS_INFO_0 pTmp = (LPGROUP_USERS_INFO_0)pGroups;
			 
			for (DWORD dw = 0 ; dw < dwResult ; dw++, pTmp++)
			{
				ASSERT(pTmp);
				groups.Add (pTmp->grui0_name);
			}
			NetApiBufferFree (pGroups);
		}
		NetApiBufferFree (pPDC);
	}

	LPWKSTA_USER_INFO_0 pBuf = NULL;
	status = NetWkstaUserGetInfo (NULL, 0, (LPBYTE *)&pBuf);

	if (status == NERR_Success)
	{
		if (pBuf != NULL)
		{
			char szUser[1024];
			ConvertTToC (szUser, (WCHAR*)(pBuf->wkui0_username));
			strResult = szUser;

//			wprintf(L"\n\tUser:          %s\n", pBuf->wkui1_username);
//			wprintf(L"\tDomain:        %s\n", pBuf->wkui1_logon_domain);
//			wprintf(L"\tOther Domains: %s\n", pBuf->wkui1_oth_domains);
//			wprintf(L"\tLogon Server:  %s\n", pBuf->wkui1_logon_server);
		}
		NetApiBufferFree (pBuf);
	}

//	AfxMessageBox (strResult);

	return strResult;


	LPBYTE pBuff;
	DWORD dwEntries, dwTotalEntries ;

	status = NetWkstaUserEnum (NULL,	// server name
		1,											// level
		&pBuff,									// receiving buffer, allocated within call
		MAX_PREFERRED_LENGTH,						// how much data we are prepared to allow it to allocate
		&dwEntries,									// returns number of records in pBuffer
		&dwTotalEntries,								// returns total number of records
		NULL);										// resume handle

	// the way we have configured the call should prevent us having insufficient space
	ASSERT(status != ERROR_MORE_DATA);

	if (NERR_Success == status)
	{
		CString strUsers;

		// run through the records
		LPWKSTA_USER_INFO_1 p = (LPWKSTA_USER_INFO_1)pBuff;
		for (DWORD dw = 0 ; dw < dwEntries ; dw++)
		{
			// Note that the definition of wkui0_username as TCHAR is a misnomer, as it is always stored in
			// wide character format, irrespective of the current environment. Thus we need to cast it to WCHAR
			// and then convert it.
			char szUser[1024];
			ConvertTToC (szUser, (WCHAR*)(p->wkui1_username));

			strUsers += szUser;
			strUsers += '\n';

			// the currently active user name has a $ appended...
			if (szUser[strlen(szUser) - 1] == '$')
			{
				strResult = szUser;
				strResult = strResult.Left(strResult.GetLength() - 1);
			}
			p++;
		}
		AfxMessageBox (strUsers);

		NetApiBufferFree (pBuff);
	}

	DWORD dwLen = 0;
	GetUserName (NULL, &dwLen);
	char * pBuffer = new char [dwLen];
	GetUserName (pBuffer, &dwLen);

	strResult = pBuffer;
	delete [] pBuffer;
	return strResult;
}
*/
#endif//#ifndef _USERGROUPS_DEF_
