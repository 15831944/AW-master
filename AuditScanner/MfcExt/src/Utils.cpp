
// FILE:	Utils.cpp
// PURPOSE:	General purpose Windows functions
// AUTHOR:	JRF Thornley - copyright (C) PMD Technology Services Ltd
// HISTORY:	JRFT - 09.08.1999 - Written
//
//		40C010		Chris Drew		22-NOV-2001
//		User user rather than system locale when displaying dates
//
#include "stdafx.h"

#include <shlobj.h>		// for directory browser function
#include <direct.h>		// for _chdir function

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/*
** Subtracts a CDateSpan from a CTime, returning a new (previous) CTime object
*/
CTime CDateSpan::Subtract (CTime ctOther) const
{
	// extract the individual fields from the start date
	int nYear = ctOther.GetYear();
	int nMonth = ctOther.GetMonth();
	int nDay = ctOther.GetDay();
	int nHour = ctOther.GetHour();
	int nMin = ctOther.GetMinute();
	int nSec = ctOther.GetSecond();

	// subtract the interval as specified
	switch (GetUnits())
	{
		case year:
			nYear -= GetValue();
			break;
		case month:
			nMonth -= GetValue();
			while (nMonth < 1)
			{
				nYear--;
				nMonth += 12;
			}
			break;
		case day:
			nDay -= GetValue();
			while (nDay < 1)
			{
				nMonth--;
				if (nMonth < 1)
				{
					nYear--;
					nMonth += 12;
				}
				nDay += DaysInMonth(nMonth, nYear);
			}
			break;
		default:
			ASSERT(FALSE);
	}
	// construct and return a new object
	return CTime(nYear, nMonth, nDay, nHour, nMin, nSec);
}

/*
** Retrieves latest Windows 32 Error and displays
*/
void HandleWin32Error (CWnd * pParent/* = NULL*/, LPCSTR pszPrefix/* = NULL*/, LPCSTR pszTitle/* = NULL*/)
{
	CString strTitle;
	CString strMessage;

	DWORD dwError = ::GetLastError();

	if (NULL == pParent)
		pParent = CWnd::GetSafeOwner();

	LPSTR lpMsgBuf;
	FormatMessage (	FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL,
					dwError, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), 
					(LPTSTR) &lpMsgBuf, 0, NULL);

	// prepend user's prefix string if required
	if (pszPrefix)
		strMessage.Format("%s\n\n%s", pszPrefix, lpMsgBuf);
	else
		strMessage = lpMsgBuf;

	if (pszTitle)
		strTitle = pszTitle;
	else
		strTitle = AfxGetApp()->m_pszAppName;

	::MessageBox (pParent->GetSafeHwnd(), strMessage, strTitle, MB_OK | MB_ICONEXCLAMATION);

	LocalFree(lpMsgBuf);
}

/*
** As above, but merely formats the message up for the caller to deal with
*/
CString GetWin32Error (LPCSTR pszPrefix/* = NULL*/)
{
	CString strMsg;

	DWORD dwError = ::GetLastError();

	LPSTR lpMsgBuf;
	FormatMessage (	FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL,
					dwError, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), 
					(LPTSTR) &lpMsgBuf, 0, NULL);

	// prepend user's prefix string if required
	if (pszPrefix)
		strMsg.Format("%s\n\n%s", pszPrefix, lpMsgBuf);
	else
		strMsg = lpMsgBuf;

	LocalFree(lpMsgBuf);
	return strMsg;
}


/*
** Report a MAPI error
*/
void HandleMAPIError (int nErrorNo, CWnd * pParent/* = NULL*/, LPCSTR pszPrefix/* = NULL*/, LPCSTR pszTitle/* = NULL*/)
{
	CString strTitle;
	CString strMessage;

	if (NULL == pParent)
		pParent = CWnd::GetSafeOwner();

	CString strErrorDesc;
	switch (nErrorNo)
	{
		case 0:		strErrorDesc="Operation Succeeded";			break;	// SUCCESS_SUCCESS
		case 1:		strErrorDesc="User aborted";				break;	// MAPI_E_USER_ABORT
		case 2:		strErrorDesc="MAPI Failure";				break;	// MAPI_E_FAILURE
		case 3:		strErrorDesc="Logon Failure";				break;	// MAPI_E_LOGON_FAILURE / MAPI_E_LOGIN_FAILURE
		case 4:		strErrorDesc="Disk Full";					break;	// MAPI_E_DISK_FULL
		case 5:		strErrorDesc="Insufficient Memory";			break;	// MAPI_E_INSUFFICIENT_MEMORY
		case 6:		strErrorDesc="Access Denied";				break;	// MAPI_E_ACCESS_DENIED
		case 8:		strErrorDesc="Too Many Sessions";			break;	// MAPI_E_TOO_MANY_SESSIONS
		case 9:		strErrorDesc="Too Many Files";				break;	// MAPI_E_TOO_MANY_FILES
		case 10:	strErrorDesc="Too Many Recipients";			break;	// MAPI_E_TOO_MANY_RECIPIENTS
		case 11:	strErrorDesc="Attachment not found";		break;	// MAPI_E_ATTACHMENT_NOT_FOUND
		case 12:	strErrorDesc="Failed to open attachement";	break;	// MAPI_E_ATTACHMENT_OPEN_FAILURE
		case 13:	strErrorDesc="Failed to write attachment";	break;	// MAPI_E_ATTACHMENT_WRITE_FAILURE
		case 14:	strErrorDesc="Unknown Recipient";			break;	// MAPI_E_UNKNOWN_RECIPIENT
		case 15:	strErrorDesc="Bad Recipient Type";			break;	// MAPI_E_BAD_RECIPTYPE
		case 16:	strErrorDesc="No Messages";					break;	// MAPI_E_NO_MESSAGES
		case 17:	strErrorDesc="Invalid Message";				break;	// MAPI_E_INVALID_MESSAGE
		case 18:	strErrorDesc="Text too large";				break;	// MAPI_E_TEXT_TOO_LARGE
		case 19:	strErrorDesc="Invalid Session";				break;	// MAPI_E_INVALID_SESSION
		case 20:	strErrorDesc="Type not supported";			break;	// MAPI_E_TYPE_NOT_SUPPORTED
		case 21:	strErrorDesc="Ambiguous Recipient";			break;	// MAPI_E_AMBIGUOUS_RECIPIENT / MAPI_E_AMBIG_RECIP
		case 22:	strErrorDesc="Message in use";				break;	// MAPI_E_MESSAGE_IN_USE
		case 23:	strErrorDesc="Network Failure";				break;	// MAPI_E_NETWORK_FAILURE
		case 24:	strErrorDesc="Invalid edit fields";			break;	// MAPI_E_INVALID_EDITFIELDS
		case 25:	strErrorDesc="Invalid Recipients";			break;	// MAPI_E_INVALID_RECIPS
		case 26:	strErrorDesc="Not Supported";				break;	// MAPI_E_NOT_SUPPORTED
		default:	strErrorDesc="Unknown Error";				break;
	}

	// prepend user's prefix string if required
	if (pszPrefix)
		strMessage.Format("%s\n\n%s", pszPrefix, strErrorDesc);
	else
		strMessage = strErrorDesc;

	if (pszTitle)
		strTitle = pszTitle;
	else
		strTitle = AfxGetApp()->m_pszAppName;

	::MessageBox (pParent->GetSafeHwnd(), strMessage, strTitle, MB_OK | MB_ICONEXCLAMATION);
}

///////////////////////////////////////////////////////////////////////////////
// WINDOW ROUTINES
void ForceOntoScreen (CWnd * pWnd)
{
	int cxScreen = GetSystemMetrics(SM_CXSCREEN);
	int cyScreen = GetSystemMetrics(SM_CYSCREEN);
	CRect rect;
	pWnd->GetWindowRect (&rect);
	// first ensure size can fit on screen
	int cx = min (cxScreen, (rect.right - rect.left));
	int cy = min (cyScreen, (rect.bottom - rect.top));
	// ensure not off left side of screen
	int x = max (0, rect.left);
	// nor off top of screen
	int y = max (0, rect.top);
	// nor off right-hand side
	x = min ( (cxScreen - cx), x);
	// nor off bottom of screen
	y = min ( (cyScreen - cy), y);
	// change window size and position if necessary
	if (x != rect.left || y != rect.top || cx != (rect.right - rect.left) || cy != (rect.bottom - rect.top))
		pWnd->MoveWindow (x, y, cx, cy);
}

/*
** Copy a menu item from one CMenu to another
** Recurses through submenus if item not found at top menu level
*/
BOOL CopyMenuItem (CMenu * pDest, CMenu * pSrc, UINT nCmdID)
{
	// loop through the source menu items
	for (int nItem = 0 ; nItem < (int)pSrc->GetMenuItemCount() ; nItem++) 
	{
		UINT nID = pSrc->GetMenuItemID(nItem);
		if (nID == nCmdID)
		{
			// found it - get details and append to destination menu
			CString strText;
			pSrc->GetMenuString(nID, strText, MF_BYCOMMAND);
			pDest->AppendMenu(MF_STRING, nID, strText);
			return TRUE;
		}
		// is it a submenu ?
		else if (-1 == nID)
		{
			// yes - get it and recurse to see if required item is within it
			CMenu * pSubMenu = pSrc->GetSubMenu(nItem);
			if (CopyMenuItem(pDest, pSubMenu, nCmdID))
			{
				return TRUE;
			}
		}
	}		
	return FALSE;
}

/*
** Finds a separator in a string, ignoring any that are enclosed in quotes
*/
int FindSeparator(LPCSTR pszString, char chSep)
{
	int nLen = strlen(pszString);
	BOOL bInString = FALSE;
	for (int n = 0 ; n < nLen ; n++)
	{
		if (pszString[n] == '\"')
		{
			bInString ^= 1;
		}
		else if (pszString[n] == chSep && !bInString)
			return n;
	}
	return -1;
}

/*
** Split off the leading word of a string
*/
CString BreakString (CString & strSource, char chSeparator/* = ' '*/, BOOL bRemove/* = TRUE*/, BOOL bIgnoreInStrings/* = TRUE*/, BOOL bStripQuotes/* = FALSE*/)
{
	CString	strResult;
	int		nSpace;

	// look for the specified separator
	if (bIgnoreInStrings)
		nSpace = FindSeparator(strSource, chSeparator);
	else
		nSpace = strSource.Find(chSeparator);
	
	// was it found ?
	if (-1 != nSpace)
	{
		// yes, split the string
		strResult = strSource.Left(nSpace);
		// optionally remove from source string
		if (bRemove)
			strSource = strSource.Right(strSource.GetLength() - (nSpace + 1));
	}
	else
	{
		// no separator - return entire string
		strResult = strSource;
		strResult.TrimRight();

		// optionally wiping out the source
		if (bRemove)
			strSource.Empty();
	}
	if (bStripQuotes && (strResult.GetLength() >= 2))
	{
		if (strResult[0] == '\"' && strResult[strResult.GetLength() - 1] == '\"')
			strResult = strResult.Mid(1, (strResult.GetLength() - 2));
	}
	return strResult;
}

// append a string, inserting a separator if string isn't empty
void AppendString (CString & string, LPCSTR pszAdd, LPCSTR pszSep/* = ", "*/)
{
	if (!pszAdd || !(*pszAdd))
		return;
	if (string.GetLength())
		string += pszSep;
	string += pszAdd;
}

/*
** finds single quotes in a string and replaces with SQL safe
** equivalent of two quotes. Also surrounds string with single
** quotes ready for insertion in a query.
** JRFT - 16.04.2002 - now optionally limits length of string
*/
CString PrepareSQLString (CString const & string, int nMaxLen/* = 0*/)
{
	// To avoid repeated memory allocations, initialise the new string to same length as old one...
	CString newString (' ', string.GetLength());
	// ..then re-initialise it
	newString = "\'";

	// get string length and scan it
	int nLen = string.GetLength();
	int nNewLen = 0;

	for (int n = 0 ; n < nLen ; n++)
	{
		// are we checking length?
		if (nMaxLen && ((nNewLen + 1) > nMaxLen))
			break;

		// is it a single quote ?
		if (string[n] == '\'')
		{
			// ok - replace with two quotes
			newString += "\'\'";
		}
		else
		{
			newString += string[n];
		}
		nNewLen++;
	}
	// finish off with a quote
	newString += '\'';
	return newString;
}

CString RemoveSQLString (CString & string, BOOL bNoRemove/* = FALSE*/)
{
	const char chQuote = '\'';
	CString strNew;
	int nPos = 0;

	// must start with a single quote
	if (string[nPos++] != chQuote)
		return strNew;
	while (TRUE)
	{
		// get a character
		char ch = string[nPos++];
		// is it a quote ?
		if (ch != chQuote)
		{
			// no - store it
			strNew += ch;
		}
		else 
		{
			// anything after it ?
			if (nPos < (string.GetLength() - 1) && string[nPos++] == chQuote)
			{
				// another quote - store a single one and continue
				strNew += chQuote;
			}
			else
			{
				// end of string
				break;
			}
		}
	}
	if (!bNoRemove)
		string = string.Right(string.GetLength() - nPos);
	return strNew;
}

CString PrepareSQLDate (CTime ctDate)
{
	CString strResult;
	strResult.Format("\'%4.4d%2.2d%2.2d %2.2d:%2.2d:%2.2d\'", ctDate.GetYear(), ctDate.GetMonth(), ctDate.GetDay(),
		ctDate.GetHour(), ctDate.GetMinute(), ctDate.GetSecond());
	return strResult;
}

/*
** return title case equivalent of a string
*/
CString TitleCase (const CString & string)
{
	CString strNewString;
	BOOL bInitial = TRUE;
	int nLength = string.GetLength();
	for (int n = 0 ; n < nLength ; n++)
	{
		// if inital character then must be converted to upper case
		char ch = string[n];
		if (ch == ' ')
		{
			bInitial = TRUE;
		}
		else
		{
			if (bInitial)
				ch = (char)toupper(ch);
			else
				ch = (char)tolower(ch);
			bInitial = FALSE;
		}
		strNewString += ch;
	}
	return strNewString;
}

/*
** Extracts the site and page from a URL into caller's buffers
*/
void SplitURL (LPCSTR pszURL, CString & strSite, CString & strPage)
{
	char cSite[512];
	LPCSTR pStart, pEnd;
	// is there a "//" indicating the start of a WWW address ?
	LPSTR pWWW = (LPSTR)strstr(pszURL, "//");
	pStart = pWWW ? (pWWW + 2) : pszURL;

	// take the first '/' as the split
	pEnd = strchr(pStart, '/');

	// was a split found ?
	if (pEnd)
	{
		// ok to split...
		int nSiteLen = pEnd - pStart;
		strncpy (cSite, pStart, nSiteLen);
		cSite[nSiteLen] = '\0';
		strSite = cSite;

		// remaining text is page
		strPage = (pEnd + 1);
	}
	else
	{
		// no separate page - stick the whole lot in the site
		strSite = pStart;
	}
}

/*
** Performs a non case-sensitive match on two single characters
*/
static BOOL chricmp (register char ch1, register char ch2)
{
	// convert to upper case
	if (ch1 > 96)	ch1 &= 0xDF;
	if (ch2 > 96)	ch2 &= 0xDF;
	return (ch1 == ch2);
}

/*
** Perform Wildcard string matching (both '?' and '*' allowable in pszWildcards)
*/
static BOOL Scan(LPCSTR & Wildcards, LPCSTR & str);
BOOL MatchString (LPCSTR pszWildcards, LPCSTR pszStr)
{
	BOOL bYes = 1;

	//iterate and delete '?' and '*' one by one
	while (*pszWildcards != '\0' && bYes && *pszStr != '\0')
	{
		if (*pszWildcards == '?') 
			pszStr ++;
		else if (*pszWildcards == '*')
		{
			bYes = Scan(pszWildcards, pszStr);
			pszWildcards --;
		}
		else
		{
			bYes = chricmp(*pszWildcards, *pszStr);
//			bYes = (*pszWildcards == *pszStr);
			pszStr ++;
		}
		pszWildcards ++;
	}
	
	while (*pszWildcards == '*' && bYes)
		pszWildcards ++;

	return bYes && *pszStr == '\0' && *pszWildcards == '\0';
}

/*
** Helper function for MatchString
*/
static BOOL Scan(LPCSTR & Wildcards, LPCSTR & str)
{
	// remove the '?' and '*'
	for(Wildcards ++; *str != '\0' && (*Wildcards == '?' || *Wildcards == '*'); Wildcards ++)
		if (*Wildcards == '?') 
			str ++;
	while ( *Wildcards == '*') 
		Wildcards ++;
	
	// if str is empty and Wildcards has more characters or,
	// Wildcards is empty, return 
	if (*str == '\0' && *Wildcards != '\0') 
		return FALSE;
	if (*str == '\0' && *Wildcards == '\0')	
		return TRUE; 
	// else search substring
	else
	{
		LPCSTR wdsCopy	= Wildcards;
		LPCSTR strCopy	= str;
		BOOL bYes		= 1;
		do 
		{
			if (!MatchString(Wildcards, str))	
				strCopy ++;
			Wildcards = wdsCopy;
			str		  = strCopy;
			while (!chricmp(*Wildcards, *str) && (*str != '\0')) 
				str ++;
			wdsCopy = Wildcards;
			strCopy = str;
		}
		while ((*str != '\0') ? !MatchString(Wildcards, str) : (bYes = FALSE) != FALSE);

		if (*str == '\0' && *Wildcards == '\0')	
			return TRUE;

		return bYes;
	}
}

BOOL MatchFileSpec (LPCSTR pszFileSpec, LPCSTR pszFile)
{
	// break into file + extension
	CString strSpecExt(pszFileSpec);	CString strSpec = BreakString(strSpecExt, '.');
	CString strFileExt(pszFile);		CString strFile = BreakString(strFileExt, '.');

	// both need to match
	return (MatchString (strSpec, strFile) && MatchString (strSpecExt, strFileExt));
}

/*
** Formats a date into a string, taking regional ordering of fields into account
** Requires a buffer which must be at least 10+1 chars and returns actual length written
*/
CString FormatDateIntl (int nYear, int nMonth, int nDay, char chSep/* = '/'*/)
{
	CString strResult;

	// obtain locale information once only
	static int nLocaleValue = -1;
	if (-1 == nLocaleValue)
	{
		char szBuffer[12];
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IDATE, szBuffer, sizeof(szBuffer));  //40C010
		nLocaleValue = atoi(szBuffer);
	}

	// order the date fields according to locale setting
	switch (nLocaleValue)
	{
		case 0:	// MDY
			strResult.Format ("%2.2d%c%2.2d%c%4.4d", nMonth, chSep, nDay, chSep, nYear);
			break;

		case 1:	// DMY
			strResult.Format ("%2.2d%c%2.2d%c%4.4d", nDay, chSep, nMonth, chSep, nYear);
			break;

		case 2:	// YMD
			strResult.Format ("%4.4d%c%2.2d%c%2.2d", nYear, chSep, nMonth, chSep, nDay);
			break;

		default:
			TRACE("Unknown Locale Setting");
			ASSERT(FALSE);
			break;
	}
	// all formats are 10 characters at the moment
	return strResult;
}

/*
** Formats a CTime as a date, according to active Locale Setting
*/
CString DateToString (CTime const & timDate, char chSep/*='/'*/)
{
	if (timDate.GetTime())
		return FormatDateIntl(timDate.GetYear(), timDate.GetMonth(), timDate.GetDay(), chSep);
	else
		return "";
}

/*
** Formats a CTime as a time in the format hh:mm:ss
*/

CString TimeToString (CTime const & time, BOOL bIncludeSeconds/* = FALSE*/)
{
	CString strResult;
	
	if (bIncludeSeconds)
		strResult.Format ("%2.2d:%2.2d:%2.2d", time.GetHour(), time.GetMinute(), time.GetSecond());
	else
		strResult.Format ("%2.2d:%2.2d", time.GetHour(), time.GetMinute());
	return strResult;
}

/*
** Formats a CTime as a date and time, according to active Locale Setting
*/
CString DateTimeToString (CTime const & timDateTime, BOOL bIncludeSeconds/* = FALSE*/, char chSep/* = '/'*/)
{
	if (timDateTime.GetTime())
	{
		// start by getting the date and time as above
		CString strDate = FormatDateIntl(timDateTime.GetYear(), timDateTime.GetMonth(), timDateTime.GetDay(), chSep);
		CString strTime = TimeToString(timDateTime, bIncludeSeconds);
		// add the two together and return
		return CString(strDate + ' ' + strTime);
	}
	else
		return "";
}

/*
** The opposite conversion to DateToString()
*/
CTime StringToDate (LPCSTR pszDate, char chSep/* = '/'*/)
{
	if (!pszDate || !*pszDate)
		return CTime(0);

	// obtain locale information once only
	static int nLocaleValue = -1;
	if (-1 == nLocaleValue)
	{
		char szBuffer[12];
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IDATE, szBuffer, sizeof(szBuffer));
		nLocaleValue = atoi(szBuffer);
	}

	// extract the date fields according to locale setting
	CString strDay, strMonth, strYear, strBuffer(pszDate) ,strLastSegment;
	switch (nLocaleValue)
	{
		case 0:	// MDY
			strMonth = BreakString (strBuffer, chSep);
			strDay = BreakString (strBuffer, chSep);
			strYear = BreakString (strBuffer, chSep);
			strLastSegment = strYear;
			break;

		case 1:	// DMY
			strDay = BreakString (strBuffer, chSep);
			strMonth = BreakString (strBuffer, chSep);
			strYear = BreakString (strBuffer, chSep);
			strLastSegment = strYear;
			break;

		case 2:	// YMD
			strYear = BreakString (strBuffer, chSep);
			strMonth = BreakString (strBuffer, chSep);
			strDay = BreakString (strBuffer, chSep);
			strLastSegment = strDay;
			break;

		default:
			TRACE("Unknown Locale Setting %d\n", nLocaleValue);
			ASSERT(FALSE);
			break;
	}

	// is there a time stored too ?
	// Note that our use of BreakString will probably have left the time appended to the last token
	BreakString(strLastSegment ,' ');

	CString strHour, strMinute, strSecond;
	if (strLastSegment.GetLength())
		strHour = BreakString (strLastSegment, ':');
	if (strLastSegment.GetLength())
		strMinute = BreakString (strLastSegment, ':');
	if (strLastSegment.GetLength())
		strSecond = BreakString (strLastSegment, ':');

	int nYear = atoi(strYear);
	int nMonth = atoi(strMonth);
	int nDay = atoi(strDay);
	int nHour = atoi(strHour);
	int nMinute = atoi(strMinute);
	int nSecond = atoi(strSecond);

	return CTime (nYear, nMonth, nDay, nHour, nMinute, nSecond);
}

/*
** return number of days in nMonth, allowing for leap years etc
*/
int DaysInMonth (int nMonth, int nYear)
{
	switch (nMonth)
	{
		case 4:
		case 6:
		case 9:
		case 10:
			return 30;
		case 2:
			return 28 + IsLeapYear(nYear);
		default:
			return 31;
	}
}

int DaysInYear (int nYear)
{
	return 365 + IsLeapYear(nYear);
}

BOOL IsLeapYear (int nYear)
{
	// a leap year if divisible by 4, unless divisible by 100 but not 400
	return ((nYear%4)==0 && (((nYear%100)!= 0) || ((nYear%400) == 0)));
}

/*
** removes any time element from a CTime object, leaving just the date
*/
CTime TruncateDate (CTime const & ctOriginal)
{
	int nYear = ctOriginal.GetYear();
	int nMonth = ctOriginal.GetMonth();
	int nDay = ctOriginal.GetDay();
	return CTime(nYear, nMonth, nDay, 0, 0, 0);
}

LPCSTR FindFileExtension (LPCSTR pszFile)
{
	int nLen = strlen(pszFile);
	LPCSTR pszTemp = pszFile;
	for (int n = 0 ; n < nLen ; n++, pszTemp++)
		if (*pszTemp == '.')
			return pszTemp + 1;
	return pszTemp;
}


LPCSTR EndWithBackslash (CString & string)
{
	int nLen = string.GetLength();
	if (nLen && string[nLen - 1] == '\\')
		return (LPCSTR)string;
	string += '\\';
	return string;
}

/*
** Scans a directory / filespec and builds a list of matching
** file names in the CStringList object
*/
int FindFiles (CStringList & names, LPCSTR pszPath, LPCSTR pszFileSpec/* = NULL*/)
{
	WIN32_FIND_DATA	fd;

	// empty the destination string list
	names.RemoveAll();
	// build the full path filter description
	CString strTemp = pszPath;
	EndWithBackslash (strTemp);
	if (pszFileSpec)
		strTemp += pszFileSpec;
	else
		strTemp += "*.*";

	HANDLE hFind = FindFirstFile (strTemp, &fd);
	int nCount = 0;
	if (INVALID_HANDLE_VALUE != hFind)
	{
		// store name here
		names.AddHead(fd.cFileName);
		nCount++;
		// get subsequent matches (if any)
		while (FindNextFile(hFind, &fd))
		{
			names.AddHead(fd.cFileName);
			nCount++;
		}
		FindClose(hFind);
	}
	return nCount;
}

/*
** draws a resource bitmap at x,y in the Device context pDC
*/
BOOL DrawBitmap (CDC * pDC, int nBitmapID, int x, int y, int cx, int cy, DWORD dwFlags)
{
	// load the bitmap
	CBitmap	bitmap;
	bitmap.LoadBitmap (nBitmapID);
	// and select into a memory context
	CDC dcMemory;
	dcMemory.CreateCompatibleDC(pDC);
	dcMemory.SelectObject (&bitmap);

	// how are we drawing it ?
	if (dwFlags & DBF_STRETCH)
	{
		// stretching - find size of original bitmap
		BITMAP bm;
		bitmap.GetBitmap(&bm);
		return pDC->StretchBlt (x, y, cx, cy, &dcMemory, 0, 0, bm.bmWidth, bm.bmHeight, SRCCOPY);
	}
	
	else if (dwFlags & DBF_TILE)
	{
		BITMAP bm;
		bitmap.GetBitmap(&bm);

		// tiling the bitmap...
		for (int y1 = y ; y1 < (y + cy) ; y1 += bm.bmHeight)
		{
			for (int x1 = x ; x1 < (x + cx) ; x1 += bm.bmWidth)
			{
				pDC->BitBlt (x1, y1, cx, cy, &dcMemory, 0, 0, SRCCOPY);
			}
		}
		return TRUE;
	}
	
	else
	{
		return pDC->BitBlt (x, y, cx, cy, &dcMemory, 0, 0, SRCCOPY);
	}
}

/*
** Draw a bitmap with a transparent background
*/
void DrawBitmapTrans (HDC hDC, HBITMAP hBitmap, int x, int y, COLORREF clrTransp)
{
	BITMAP     bm;
	COLORREF   cColor;
	HBITMAP    bmAndBack, bmAndObject, bmAndMem, bmSave;
	HBITMAP    bmBackOld, bmObjectOld, bmMemOld, bmSaveOld;
	HDC        hdcMem, hdcBack, hdcObject, hdcTemp, hdcSave;
	POINT      ptSize;

	hdcTemp = CreateCompatibleDC (hDC);
	SelectObject (hdcTemp, hBitmap);		// Select the bitmap

	GetObject (hBitmap, sizeof(BITMAP), (LPSTR)&bm);
	ptSize.x = bm.bmWidth;					// Get width of bitmap
	ptSize.y = bm.bmHeight;					// Get height of bitmap
	DPtoLP (hdcTemp, &ptSize, 1);			// Convert from device to logical points

	// Create some DCs to hold temporary data
	hdcBack   = CreateCompatibleDC(hDC);
	hdcObject = CreateCompatibleDC(hDC);
	hdcMem    = CreateCompatibleDC(hDC);
	hdcSave   = CreateCompatibleDC(hDC);

	// Create a bitmap for each DC. DCs are required for a number of GDI functions.

	// Monochrome DC
	bmAndBack   = CreateBitmap(ptSize.x, ptSize.y, 1, 1, NULL);

	// Monochrome DC
	bmAndObject = CreateBitmap(ptSize.x, ptSize.y, 1, 1, NULL);

	bmAndMem    = CreateCompatibleBitmap (hDC, ptSize.x, ptSize.y);
	bmSave      = CreateCompatibleBitmap (hDC, ptSize.x, ptSize.y);

	// Each DC must select a bitmap object to store pixel data.
	bmBackOld   = (HBITMAP)::SelectObject (hdcBack, bmAndBack);
	bmObjectOld = (HBITMAP)::SelectObject (hdcObject, bmAndObject);
	bmMemOld    = (HBITMAP)::SelectObject (hdcMem, bmAndMem);
	bmSaveOld   = (HBITMAP)::SelectObject (hdcSave, bmSave);

	// Set proper mapping mode.
	SetMapMode (hdcTemp, GetMapMode(hDC));

	// Save the bitmap sent here, because it will be overwritten.
	BitBlt(hdcSave, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0, SRCCOPY);

	// Set the background color of the source DC to the color.
	// contained in the parts of the bitmap that should be transparent
	cColor = SetBkColor (hdcTemp, clrTransp);

	// Create the object mask for the bitmap by performing a BitBlt
	// from the source bitmap to a monochrome bitmap.
	BitBlt(hdcObject, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0, SRCCOPY);

	// Set the background color of the source DC back to the original
	// color.
	SetBkColor(hdcTemp, cColor);

	// Create the inverse of the object mask.
	BitBlt(hdcBack, 0, 0, ptSize.x, ptSize.y, hdcObject, 0, 0, NOTSRCCOPY);
	
	// Copy the background of the main DC to the destination.
	BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, hDC, x, y, SRCCOPY);

	// Mask out the places where the bitmap will be placed.
	BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, hdcObject, 0, 0, SRCAND);

	// Mask out the transparent colored pixels on the bitmap.
	BitBlt(hdcTemp, 0, 0, ptSize.x, ptSize.y, hdcBack, 0, 0, SRCAND);

	// XOR the bitmap with the background on the destination DC.
	BitBlt(hdcMem, 0, 0, ptSize.x, ptSize.y, hdcTemp, 0, 0, SRCPAINT);

	// Copy the destination to the screen.
	BitBlt(hDC, x, y, ptSize.x, ptSize.y, hdcMem, 0, 0, SRCCOPY);

	// Place the original bitmap back into the bitmap sent here.
	BitBlt(hdcTemp, 0, 0, ptSize.x, ptSize.y, hdcSave, 0, 0, SRCCOPY);

	// Delete the memory bitmaps.
	DeleteObject(SelectObject(hdcBack, bmBackOld));
	DeleteObject(SelectObject(hdcObject, bmObjectOld));
	DeleteObject(SelectObject(hdcMem, bmMemOld));
	DeleteObject(SelectObject(hdcSave, bmSaveOld));

	// Delete the memory DCs.
	DeleteDC(hdcMem);
	DeleteDC(hdcBack);
	DeleteDC(hdcObject);
	DeleteDC(hdcSave);
	DeleteDC(hdcTemp);
}

/*
** Return a 16 bit equivalent of a 32 bit path
*/
CString GetAlternatePathName (LPCSTR pszPath)
{
	CString strResult;
	LPSTR p = strResult.GetBuffer(_MAX_PATH);
	DWORD dwResult = GetShortPathName(pszPath, p, _MAX_PATH);
	if (!dwResult)
		HandleWin32Error(CWnd::GetSafeOwner());
	strResult.ReleaseBuffer();
	if (ERROR_INVALID_PARAMETER == dwResult)
		return (strResult = pszPath);
	else
		return strResult;
}

/*
** Composes a fully qualified path name from dir + file
*/
CString MakePathName (LPCSTR pszDir, LPCSTR pszFile)
{
	CString strTemp(pszDir);
	EndWithBackslash(strTemp);
	strTemp += pszFile;
	return strTemp;
}

/*
** Checks a path is in UNC format, and optionally that it exists
*/
BOOL IsUNCPathValid (LPCSTR pszPath, BOOL bFailIfNotFound)
{
	// must start with two backslashes
	if (!pszPath						// pointer is NULL
	||	strlen(pszPath) < 2				// not long enough to contain two slashes
	||	strncmp(pszPath, "\\\\", 2))	// no pair of leading backslashes
	{
		return FALSE;
	}
	else if (bFailIfNotFound)
	{
		// validate that path exists too
		if (_chdir(pszPath))
			return FALSE;
	}
	return TRUE;
}

/*
**	return TRUE of pszFolderPath exists
*/
BOOL FolderExists (LPCSTR pszFolderPath)
{
#ifdef WIN32
	DWORD dwRetVal = GetFileAttributes (pszFolderPath);
	if (0xFFFFFFFF == dwRetVal)
		return FALSE;

	else if (dwRetVal & FILE_ATTRIBUTE_DIRECTORY)
		return TRUE;
	
	return FALSE;
#else
	CFileStatus FileStatus;

	if (CFile::GetStatus (pszFolderPath, FileStatus) == TRUE  &&
		FileStatus.m_attribute & directory)
	{
		return TRUE;
	}
	else if (FileSystemExists(GetFullPathName(pszFolderPath)))
	{
		return TRUE;
	}
	return FALSE;
#endif
}

/*
**	Create the path pszFolderPath
*/
BOOL FolderCreate (LPCSTR pszFolderPath)
{
	CString strFolderPath(pszFolderPath), strDir;
	BOOL bRetVal = TRUE;

	// Error if no directory specified.
	if (strFolderPath.IsEmpty())
		return FALSE;

	EndWithBackslash (strFolderPath);

	// Create each directory in the path
	int	nIndex = 0;
	int	nFSIndex = 0;
	BOOL bDone = FALSE;
	while (!bDone)
	{
		// Extract one directory
		nIndex = strFolderPath.Find(_T('\\'));
		nFSIndex = strFolderPath.Find(_T('/'));
		if ((nFSIndex >= 0) && (nFSIndex < nIndex))	
			nIndex = nFSIndex;
		if (nIndex != -1)
		{
			strDir = strDir + strFolderPath.Left (nIndex);
			strFolderPath = strFolderPath.Right (strFolderPath.GetLength() - nIndex - 1);

			// The first time through, we might have a drive name
			if (strDir.GetLength() >= 1  &&  strDir[strDir.GetLength() - 1] != ':')
			{
				SECURITY_ATTRIBUTES security_attrib;
				security_attrib.nLength = sizeof(SECURITY_ATTRIBUTES);
				security_attrib.lpSecurityDescriptor = NULL;
				security_attrib.bInheritHandle = TRUE;

				bRetVal = CreateDirectory ((const TCHAR *)strDir, &security_attrib);
			}
			strDir = strDir + '\\';
		}
		else
		{
			// We're finished
			bDone = TRUE;
		}
	}
	// Return the last CreateDirectory() return value.
	return bRetVal;
}

/*
** Set text for a tab of a Property Sheet
*/
/*
void SetPropShtTabText (CPropertySheet * pSht, int nTab, LPCSTR pszText)
{
	ASSERT (pSht->GetSafeHwnd());
	// get the tab control
	CTabCtrl * pTab = pSht->GetTabControl();
	ASSERT (pTab->GetSafeHwnd());
	// set up the new text
	char * pBuffer = new char [strlen(pszText) + 1];
	strcpy (pBuffer, pszText);
	TCITEM item;
	item.mask = TCIF_TEXT;
	item.pszText = pBuffer;
	// action the change
	pTab->SetItem (nTab, &item);
	// clean up
	delete [] pBuffer;
}
*/

/*
** Validate whether a string contains a valid numeric value
*/
BOOL IsNumeric (LPCSTR pszValue, BOOL bAllowSigned/* = TRUE*/, BOOL bAllowFloat/* = TRUE*/)
{
	int nLen = strlen(pszValue);
	int nDecimalCount = 0;

	for (int n = 0 ; n < nLen ; n++)
	{
		char ch = pszValue[n];
		// digits are always ok
		if (ch >= '0' && ch <= '9')
			continue;
		// plus or minus is ok at start of string
		if (bAllowSigned && (n == 0) && (ch == '-' || ch == '+'))
			continue;
		// a single decimal may be ok 
		if (bAllowFloat && (nDecimalCount == 0) && (ch == '.'))
		{
			nDecimalCount++;
			continue;
		}
		// must be an invalid character
		return FALSE;
	}
	// all characters check out ok
	return TRUE;
}

/********************************************
** Helper Functions for folder browser
**
** Return the label of a tree view item
*/
CString TreeView_GetItemText (HWND hTree, HTREEITEM hItem)
{
	ASSERT(hTree != NULL);
	ASSERT(hItem != NULL);
	TVITEM item;
	char szBuffer[1024];
	memset(&item, 0, sizeof(item));
	item.mask	= TVIF_TEXT;
	item.hItem	= hItem;
	item.pszText	= szBuffer;
	item.cchTextMax	= sizeof(szBuffer);
	TreeView_GetItem(hTree, &item);
	return CString(szBuffer);
}

int TreeView_GetItemIcon (HWND hTree, HTREEITEM hItem)
{
	ASSERT(hTree != NULL);
	ASSERT(hItem != NULL);
	TVITEM item;
	memset (&item, 0, sizeof(item));
	item.mask	= TVIF_IMAGE;
	item.hItem	= hItem;
	TreeView_GetItem(hTree, &item);
	return item.iImage;
}

HTREEITEM TreeView_FindComputer (HWND hTree, HTREEITEM hParent, LPCSTR pszComputer)
{
	// loop through all child items
	TreeView_Expand(hTree, hParent, TVE_EXPAND);
	for (HTREEITEM hChild = TreeView_GetChild(hTree, hParent) ; hChild != NULL ; hChild = TreeView_GetNextSibling(hTree, hChild))
	{
		// what type of item is it ?
		int nIcon = TreeView_GetItemIcon (hTree, hChild);
		switch (nIcon)
		{
			case 0x0D:	// "Entire Network" icon
			case 0x12:	// Domain icon
				{
					// recurse...
					HTREEITEM hResult = TreeView_FindComputer (hTree, hChild, pszComputer);
					if (hResult)
						return hResult;
				}
				break;

			case 0x0F:	// Computer icon
				{
					CString strBuffer = TreeView_GetItemText(hTree, hChild);
					if (0 == strBuffer.CompareNoCase(pszComputer))
						return hChild;
				}
				break;

			default:
				CString strText = TreeView_GetItemText(hTree, hChild);
				TRACE("Icon 0x%X, Text %s\n", nIcon, strText);
				break;
		}

	}
	// Nothing found - collapse this branch
	TreeView_Expand(hTree, hParent, TVE_COLLAPSE);
	return NULL;
}

HTREEITEM TreeView_FindFolder(HWND hTree, HTREEITEM hParent, LPCSTR pszFolderName)
{
	// loop through all child items
	TreeView_Expand(hTree, hParent, TVE_EXPAND);
	for (HTREEITEM hChild = TreeView_GetChild(hTree, hParent) ; hChild != NULL ; hChild = TreeView_GetNextSibling(hTree, hChild))
	{
		// check that it IS a folder...
		int nIcon = TreeView_GetItemIcon (hTree, hChild);
		TRACE("Icon 0x%4.4X, Text %s\n", nIcon, TreeView_GetItemText(hTree, hChild));

		// check for a text match
		CString strBuffer = TreeView_GetItemText(hTree, hChild);
		if (0 == strBuffer.CompareNoCase(pszFolderName))
			return hChild;
	}
	// not found ? collapse the item again...
	TreeView_Expand(hTree, hParent, TVE_COLLAPSE);
	return NULL;
}

/*
** Callback for use in Shell BrowseForFolder dialog
** Simply sets the initial path to theat sent by the user as the original lParam parameter
*/
int CALLBACK BrowseFolderCallbackProc(HWND hWnd, UINT nMsg, LPARAM /*wParam*/, LPARAM lParam)
{
	switch (nMsg)
	{
		case BFFM_INITIALIZED:
			// dialog has started - set the previous path - if any
			if (lParam)
			{
				// special handling for unc paths...copy locally
				char szBuffer[_MAX_PATH];
				strcpy(szBuffer, (char*)lParam);
				char * pszPath = szBuffer;
				if (strlen(pszPath) >= 2 && pszPath[0] == '\\' && pszPath[0] == '\\')
				{
					HWND hTree = ::GetDlgItem(hWnd, 0x3741);
					ASSERT(hTree != NULL);
					HTREEITEM hRoot = TreeView_GetRoot(hTree);
					HTREEITEM hNewItem = NULL, hComputer = NULL, hFolder = NULL;
					// find the network item
					for (HTREEITEM hChild = TreeView_GetChild(hTree, hRoot) ; hChild != NULL ; hChild = TreeView_GetNextSibling(hTree, hChild))
					{
						CString strLabel = TreeView_GetItemText(hTree, hChild);
						if (-1 != strLabel.Find("Network"))
						{
							hNewItem = hChild;
							break;
						}
					}
					// ok, have we found the network placeholder?
					if (hNewItem != NULL)
					{
						// yes - start parsing the string - first job is to find the computer...
						pszPath += 2;
						char * pszCompName = strtok(pszPath, "\\");

						// assuming we find a computer name then search down the tree for it...
						if (pszCompName)
						{
							hComputer = TreeView_FindComputer(hTree, hNewItem, pszCompName);
							if (hComputer)
							{
								// now keep parsing folders until we run out of string
								hFolder = hComputer;
								while (char * pszFolder = strtok(NULL, "\\"))
								{
									hFolder = TreeView_FindFolder(hTree, hFolder, pszFolder);
									if (NULL == hFolder)
										break;
									else
										hNewItem = hFolder;
								}
							}
						}
						TreeView_SelectItem(hTree, hNewItem);
					}
				}
				else
					// normally let the API sort it out...
					::SendMessage(hWnd, BFFM_SETSELECTION, TRUE, lParam);
			}
			break;

		default:
			TRACE ("BrowseFolderCallbackProc : HWND 0x%X, nMsg = 0x%X\n", hWnd, nMsg);
			break;
	}
	return 0;
}

/*
** Browse for a folder - pszPath is sent the initial path and returns the one chosen
** by user. Returns TRUE if user made a selection, else FALSE
*/
BOOL BrowseFolder (CWnd * pParent, LPCSTR pszTitle, LPSTR pszPath ,BOOL bNetworkOnly/*=FALSE*/)
{
	BOOL bResult = FALSE;
    LPMALLOC pMalloc;

    
	// Gets the Shell's default allocator
    if (::SHGetMalloc(&pMalloc) == NOERROR)
    {
        BROWSEINFO bi;
        char pszBuffer[MAX_PATH + 1];
        LPITEMIDLIST pidl;
        // Get help on BROWSEINFO struct - it's got all the bit settings.
        bi.hwndOwner		= pParent->GetSafeHwnd();
		if (!bNetworkOnly)
	        bi.pidlRoot		= NULL;
		else
		{
			LPITEMIDLIST pidlRoot = NULL;
			SHGetSpecialFolderLocation(NULL, CSIDL_NETWORK, &pidlRoot);
			bi.pidlRoot		= pidlRoot;
		}
		
        bi.pszDisplayName	= pszBuffer;
        bi.lpszTitle		= pszTitle;
        bi.ulFlags			= BIF_RETURNFSANCESTORS | BIF_RETURNONLYFSDIRS;
		// use a callback (see above) to initialise the dialog with a previous selection
        bi.lpfn				= BrowseFolderCallbackProc;
		// ensure the supplied path doesn't have a trailing backslash - it doesn't like it!
		if (pszPath && *pszPath)
		{
			int nLen = strlen(pszPath);
			if (pszPath[nLen - 1] == '\\')
				pszPath[nLen - 1] = '\0';
			bi.lParam		= (LPARAM)(LPCSTR)pszPath;
		}
		else
			bi.lParam		= NULL;
        // This next call issues the dialog box.
        if ((pidl = ::SHBrowseForFolder(&bi)) != NULL)
        {
            if (::SHGetPathFromIDList(pidl, pszBuffer))
            { 
	            // At this point pszBuffer contains the selected path - write back to the buffer
				strcpy (pszPath, pszBuffer);
				bResult = TRUE;
            }
            // Free the PIDL allocated by SHBrowseForFolder.
            pMalloc->Free(pidl);
        }
        // Release the shell's allocator.
        pMalloc->Release();
    }
	return bResult;
}


/*
** Same as Windows API function CopyFile() except that if file has same datestamp the copy is skipped
*/
BOOL CopyFileIfNewer (LPCSTR pszSourceFileName, LPCSTR pszDestFileName)
{
	CFileStatus fsSource, fsDest;

	// does the destination file already exist?
	if (CFile::GetStatus (pszDestFileName, fsDest))
	{
		// yes - skip the copy if destination file is as new or newer
		CFile::GetStatus (pszSourceFileName, fsSource);
		if (fsSource.m_mtime <= fsDest.m_mtime)
			return TRUE;
	}

	return CopyFile (pszSourceFileName, pszDestFileName, FALSE);
}

#define KB	0x00000400
#define MB	0x00100000
#define GB	0x40000000

// Formats a file size into byte, kilobytes, megabytes or gigabytes
CString FormatFileSize (DWORD dwSize)
{
	CString strResult;

	if (dwSize >= GB)
		strResult.Format ("%d GB", dwSize >> 30);
	else if (dwSize >= MB)
		strResult.Format ("%d MB", dwSize >> 20);
	else if (dwSize >= KB)
		strResult.Format ("%d KB", dwSize >> 10);
	else
		strResult.Format ("%d Bytes", dwSize);
	return strResult;
}


//
// +REQ 26798
//    Correct odd values being incorrectly formatted.  While this is not a perfect fix as it is dealing
//    with flawed data it will do until the scanner is fixed to report the memory in a more consistent 
//    manner - that is not to read from DMI information.
//
CString FormatMegabytes (DWORD dwMB)
{
	CString strResult;
	int nFrigFactor = 10;

	// Now check for less than 1GB
	if ((dwMB + nFrigFactor) < 1024)
	{
		strResult.Format ("%d MB", dwMB);
	}
	else
	{
		// First of all add a 'frig' factor just to help with odd values
		dwMB += nFrigFactor;

		// How many 'whole' GB do we have
		int nGB = dwMB / 1024;

		// ...and what's left?
		int nMBRemainder = dwMB % 1024;

		// ,,,express this as a two digit decimal
		int nFraction = (nMBRemainder * 100) / 1024;

		// ...and format this
		if (nFraction == 0)
			strResult.Format("%d GB" ,nGB);
		else
			strResult.Format ("%d.%d GB", nGB, nFraction);
	}

	return strResult;
}



//
//    FormatMHz
//    =========
//
//    Display a Mhz value in a clearer format by potentially translating to GHz
//
CString FormatMHz (DWORD dwMHz)
{
	CString strResult;


#pragma message ("*** The code here has been commented out as it causes display issues in LicenseWizard as you cannot easily compare values if they are displayed in different units")

	// is it smaller than 1GHz ?
//	if (dwMHz < 1000)
		strResult.Format ("%d MHz", dwMHz);
	// otherwise express as GHz, but to 2 s.f.
//	else
//	{
//		DWORD dwGHz = dwMHz / 1000, dwFraction = 0;
//		if (dwGHz < 10)
//		{
//			// append the fraction
//			DWORD dwRemainder = dwMHz - (dwGHz * 1000);
//			dwFraction = (dwRemainder + 50) / 100;
//		}
//		if (dwFraction)
//			strResult.Format ("%d.%d GHz", dwGHz, dwFraction);
//		else
//			strResult.Format ("%d GHz", dwGHz);
//	}
	return strResult;
}

//
//    BuildTimeSpan
//    =============
//
//    Given a number of minutes, return a CTimeSpan object
//
CTimeSpan	BuildTimeSpan(int nMinutes) 
{
	int nHours = 0, nDays = 0;
	if (nMinutes > 59)
	{
		nHours = nMinutes / 60;
		nMinutes %= 60;
	}
	if (nHours > 23)
	{
		nDays = nHours / 23;
		nDays %= 23;
	}

	CTimeSpan ts (nDays ,nHours ,nMinutes ,0);
	return ts;
}

/*
** Wrap the Windows API call of the same name
*/
CString GetComputerName ()
{
	char szCompName[MAX_COMPUTERNAME_LENGTH + 1];
	memset (szCompName, 0, sizeof(szCompName));
	DWORD dwNameLen = sizeof(szCompName);
	::GetComputerName (szCompName, &dwNameLen);
	return CString(szCompName);
}

//
//    ListToString
//    ============
//
//    Convert a dynalist of strings to a semi-colon seperated string
//
CString ListToString	(CDynaList<CString> const & list, char chSep/* = ';'*/)
{
	CString strResult;
	for (DWORD dw = 0 ; dw < list.GetCount() ; dw++)
	{
		if (dw != 0)
			strResult += chSep;
		strResult += list[dw];
	}
	return strResult;
}


//
//    ListFromString
//    ==============
//
//    Build a CDynalist of strings from a delimited string
//
DWORD ListFromString (CDynaList<CString> & list, LPCSTR pszString, char chSeparator/* = ';'*/)
{
	CString strBuffer(pszString), strValue;
	DWORD dwCount = 0;
	
	list.Empty();
	if (!strBuffer.IsEmpty())
	{
		// Split the string into its components noting that we may have empty strings which we must include 
		// in the list
		int nSpace = strBuffer.Find(chSeparator);
		while (nSpace != -1)
		{
			CString strResult = strBuffer.Left(nSpace);
			list.Add(strResult);
			strBuffer = strBuffer.Right(strBuffer.GetLength() - (nSpace + 1));
			nSpace = strBuffer.Find(chSeparator);
		}

		// End of string but we must add the trailing section
		//if (!strBuffer.IsEmpty())
		list.Add(strBuffer);
	}
	return list.GetCount();
}



//
//    ZuluToLocalTime
//    ================
//
//    Convert a CTime object in ZULU time to its local time equivalent
//
CTime	ZuluToLocalTime (CTime& ctZuluTime)
{
	struct tm* osTime;
	osTime = ctZuluTime.GetLocalTm(NULL);

	// Now convert back to a CTime
	CTime ctLocalTime(mktime(osTime));
	return ctLocalTime;
}


//
//    LocaltoZuluTime
//    ===============
//
//    Convert a CTime object in local time to its zulu time equivalent
//
CTime	LocaltoZuluTime (CTime& ctLocalTime)
{
	struct tm* osTime;
	osTime = ctLocalTime.GetGmtTm(NULL);

	// Now convert back to a CTime
	CTime ctZuluTime(mktime(osTime));

	return ctZuluTime;
}

BOOL WriteWindowToDIB (LPCSTR szFile, CWnd *pWnd)
{
	CBitmap 	bitmap;
	CWindowDC	dc(pWnd);
	CDC 		memDC;
	CRect		rect;

	memDC.CreateCompatibleDC (&dc); 

	pWnd->GetWindowRect (rect);

	bitmap.CreateCompatibleBitmap(&dc, rect.Width(),rect.Height() );
	
	CBitmap* pOldBitmap = memDC.SelectObject(&bitmap);
	memDC.BitBlt(0, 0, rect.Width(),rect.Height(), &dc, 0, 0, SRCCOPY); 

	// Create logical palette if device support a palette
	CPalette pal;
	if( dc.GetDeviceCaps(RASTERCAPS) & RC_PALETTE )
	{
		UINT nSize = sizeof(LOGPALETTE) + (sizeof(PALETTEENTRY) * 256);
		LOGPALETTE *pLP = (LOGPALETTE *) new BYTE[nSize];
		pLP->palVersion = 0x300;

		pLP->palNumEntries = (WORD)GetSystemPaletteEntries (dc, 0, 255, pLP->palPalEntry );

		// Create the palette
		pal.CreatePalette( pLP );

		delete[] pLP;
	}

	memDC.SelectObject(pOldBitmap);

	// Convert the bitmap to a DIB
	HANDLE hDIB = DDBToDIB( bitmap, BI_RGB, &pal );

	if( hDIB == NULL )
		return FALSE;

	// Write it to file
	WriteDIB( szFile, hDIB );

	// Free the memory allocated by DDBToDIB for the DIB
	GlobalFree( hDIB );
	return TRUE;
}

HANDLE DDBToDIB (CBitmap& bitmap, DWORD dwCompression, CPalette* pPal)
{
	BITMAP			bm;
	BITMAPINFOHEADER	bi;
	LPBITMAPINFOHEADER 	lpbi;
	DWORD			dwLen;
	HANDLE			hDIB;
	HANDLE			handle;
	HDC 			hDC;
	HPALETTE		hPal;


	ASSERT (bitmap.GetSafeHandle());

	// The function has no arg for bitfields
	if (dwCompression == BI_BITFIELDS)
		return NULL;

	// If a palette has not been supplied use defaul palette
	hPal = (HPALETTE) pPal->GetSafeHandle();
	if (hPal==NULL)
		hPal = (HPALETTE) GetStockObject(DEFAULT_PALETTE);

	// Get bitmap information
	bitmap.GetObject(sizeof(bm),(LPSTR)&bm);

	// Initialize the bitmapinfoheader
	bi.biSize		= sizeof(BITMAPINFOHEADER);
	bi.biWidth		= bm.bmWidth;
	bi.biHeight 		= bm.bmHeight;
	bi.biPlanes 		= 1;
	bi.biBitCount		= (WORD)(bm.bmPlanes * bm.bmBitsPixel);
	bi.biCompression	= dwCompression;
	bi.biSizeImage		= 0;
	bi.biXPelsPerMeter	= 0;
	bi.biYPelsPerMeter	= 0;
	bi.biClrUsed		= 0;
	bi.biClrImportant	= 0;

	// Compute the size of the  infoheader and the color table
	int nColors = (1 << bi.biBitCount);
	if( nColors > 256 ) 
		nColors = 0;
	dwLen  = bi.biSize + nColors * sizeof(RGBQUAD);

	// We need a device context to get the DIB from
	hDC = GetDC(NULL);
	hPal = SelectPalette(hDC,hPal,FALSE);
	RealizePalette(hDC);

	// Allocate enough memory to hold bitmapinfoheader and color table
	hDIB = GlobalAlloc(GMEM_FIXED,dwLen);

	if (!hDIB){
		SelectPalette(hDC,hPal,FALSE);
		ReleaseDC(NULL,hDC);
		return NULL;
	}

	lpbi = (LPBITMAPINFOHEADER)hDIB;

	*lpbi = bi;

	// Call GetDIBits with a NULL lpBits param, so the device driver 
	// will calculate the biSizeImage field 
	GetDIBits(hDC, (HBITMAP)bitmap.GetSafeHandle(), 0L, (DWORD)bi.biHeight,
			(LPBYTE)NULL, (LPBITMAPINFO)lpbi, (DWORD)DIB_RGB_COLORS);

	bi = *lpbi;

	// If the driver did not fill in the biSizeImage field, then compute it
	// Each scan line of the image is aligned on a DWORD (32bit) boundary
	if (bi.biSizeImage == 0){
		bi.biSizeImage = ((((bi.biWidth * bi.biBitCount) + 31) & ~31) / 8) 
						* bi.biHeight;

		// If a compression scheme is used the result may infact be larger
		// Increase the size to account for this.
		if (dwCompression != BI_RGB)
			bi.biSizeImage = (bi.biSizeImage * 3) / 2;
	}

	// Realloc the buffer so that it can hold all the bits
	dwLen += bi.biSizeImage;
	handle = GlobalReAlloc(hDIB, dwLen, GMEM_MOVEABLE);
	if (handle)
		hDIB = handle;
	else
	{
		GlobalFree(hDIB);

		// Reselect the original palette
		SelectPalette(hDC,hPal,FALSE);
		ReleaseDC(NULL,hDC);
		return NULL;
	}

	// Get the bitmap bits
	lpbi = (LPBITMAPINFOHEADER)hDIB;

	// FINALLY get the DIB
	BOOL bGotBits = GetDIBits( hDC, (HBITMAP)bitmap.GetSafeHandle(),
				0L,				// Start scan line
				(DWORD)bi.biHeight,		// # of scan lines
				(LPBYTE)lpbi 			// address for bitmap bits
				+ (bi.biSize + nColors * sizeof(RGBQUAD)),
				(LPBITMAPINFO)lpbi,		// address of bitmapinfo
				(DWORD)DIB_RGB_COLORS);		// Use RGB for color table

	if( !bGotBits )
	{
		GlobalFree(hDIB);
		
		SelectPalette(hDC,hPal,FALSE);
		ReleaseDC(NULL,hDC);
		return NULL;
	}

	SelectPalette(hDC,hPal,FALSE);
	ReleaseDC(NULL,hDC);
	return hDIB;
}

BOOL WriteDIB (LPCSTR szFile, HANDLE hDIB)
{
	BITMAPFILEHEADER	hdr;
	LPBITMAPINFOHEADER	lpbi;

	if (!hDIB)
		return FALSE;

	CFile file;
	if( !file.Open( szFile, CFile::modeWrite|CFile::modeCreate) )
		return FALSE;

	lpbi = (LPBITMAPINFOHEADER)hDIB;

	int nColors = 1 << lpbi->biBitCount;

	// Fill in the fields of the file header 
	hdr.bfType		= ((WORD) ('M' << 8) | 'B');	// is always "BM"
	hdr.bfSize		= GlobalSize (hDIB) + sizeof( hdr );
	hdr.bfReserved1 	= 0;
	hdr.bfReserved2 	= 0;
	hdr.bfOffBits		= (DWORD) (sizeof( hdr ) + lpbi->biSize +
						nColors * sizeof(RGBQUAD));

	// Write the file header 
	file.Write( &hdr, sizeof(hdr) );

	// Write the DIB header and the bits 
	file.Write( lpbi, GlobalSize(hDIB) );

	return TRUE;
}


//	Split string str and return the slices in the CString array SplitArray
//	This function works even if there is nothing beteween separators such 
//	as if str="toto,tata,,,mama,,"
void SplitString(CString &str, char Sep, CStringArray &SplitArray)
{
	CString str1;
	//2048 is enough! Otherwise we have to make dynamic dimension
	int SepPos[2048];	
	int i=0,j=0;
	SepPos[j]=str.Find(Sep,i);

	while (SepPos[j] >= 0)
	{
		if (SepPos[j] - i <= 0)
			str1 = CString("");
		else 
			str1 = str.Mid(i, SepPos[j] - i);

		str.TrimLeft();
		str.TrimRight();
		SplitArray.Add(str1);
		i = SepPos[j] + 1;
		j++;
		SepPos[j] = str.Find(Sep, i);
	}

	//	Add the last one
	str1 = str.Mid(i, str.GetLength() - i);
	str1.TrimLeft();
	str1.TrimRight();
	SplitArray.Add(str1);
}




//
//    DecodeDigitalProductKey
//    =======================
//
//    Attempt to decode a product ID which has been encrypted.  This is common with Microsoft Product
//    Serial Numbers wherethe actual value stored in the registry is not actually the CD key
//
CString DecodeDigitalProductKey (CByteArray& digitalProductId, bool use2010Method /*=FALSE*/)
{

	// Possible alpha-numeric characters in product key.
	char digits[] = 
	{
		'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 
		'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9',
	};

	// Offset of first byte of encoded product key in 'DigitalProductIdxxx" REG_BINARY value. 
	// Offset = 34H for old keys and 808 for 2010 on keys
	int keyStartIndex;
	if (use2010Method)
		keyStartIndex = 808;
	else
		keyStartIndex = 52;

	// Offset of last byte of encoded product key in 'DigitalProductIdxxx" REG_BINARY value. 
	// Offset = 15 on from the start index regardless of method
	const int keyEndIndex = keyStartIndex + 15;

	// Length of decoded product key
	const int decodeLength = 29;

	// Length of decoded product key in byte-form. Each byte represents 2 chars.
	const int decodeStringLength = 15;

	// Array of containing the decoded product key.
	char decodedChars[decodeLength + 1];
	memset(decodedChars ,0 ,decodeLength + 1);

	// Return now if not enough characters
	if (digitalProductId.GetSize() <= keyEndIndex)
		return "";

	// Extract byte 52 to 67 inclusive.
	CByteArray hexPid;
	for (int i = keyStartIndex; i <= keyEndIndex; i++)
	{
		hexPid.Add(digitalProductId[i]);
	}

	for (int i = decodeLength - 1; i >= 0; i--)
	{
		// Every sixth char is a separator.
		if ((i + 1) % 6 == 0)
		{
			decodedChars[i] = '-';
		}
		else
		{
			// Do the actual decoding.
			int digitMapIndex = 0;
			for (int j = decodeStringLength - 1; j >= 0; j--)
			{
				int byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
				hexPid[j] = (byte)(byteValue / 24);
				digitMapIndex = byteValue % 24;
				decodedChars[i] = digits[digitMapIndex];
			}
		}
	}
	
	CString strDecoded = decodedChars;
	return strDecoded;
}
