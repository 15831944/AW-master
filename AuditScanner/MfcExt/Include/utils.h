
// FILE:	Utils.h
// PURPOSE:	General purpose Windows functions
// AUTHOR:	JRF Thornley - copyright (C) PMD Technology Services Ltd
// HISTORY:	JRFT - 09.08.1999 - Written

#ifndef _UTILS_DEF_
#define _UTILS_DEF_

/*
** Extended combo-box with case sensitive string selection
*/
class CIdsComboBox : public CComboBox
{
public:
	int SelectString (int nStartAfter, LPCTSTR lpszString)
	{
		int nSel = FindStringExact(nStartAfter, lpszString);
		if (CB_ERR != nSel)
			SetCurSel(nSel);
		return nSel;
	}
	int FindStringExact (int nIndexStart, LPCSTR lpszFind)
	{
		int nCount = GetCount();
		if (CB_ERR == nIndexStart)
			nIndexStart++;
		for (int n = nIndexStart ; n < nCount ; n++)
		{
			CString strThisItem;
			GetLBText(n, strThisItem);
			if (strThisItem == lpszFind)
				return n;
		}
		return CB_ERR;
	}
};

/*
** Utility class to allow fixed intervals to be added to CTime objects
*/
class CDateSpan
{
public:
	enum eUnit { day, month, year };
public:
	CDateSpan ()
		{ Set (day, 0); }
	CDateSpan (eUnit units, int nValue)
		{ Set (units, nValue); }
	void operator= (DWORD dwValue)
		{ m_dwStorage = dwValue; }
	operator DWORD() const
		{ return m_dwStorage; }
	void SetUnit (eUnit units)
		{ Set (units, GetValue()); }
	void SetValue (int nValue)
		{ Set (GetUnits(), nValue); }
	void Set (eUnit units, int nValue)
		{ m_dwStorage = (((DWORD)units) << 30) + (nValue & 0x3FFF); }
	eUnit GetUnits () const
		{ return (eUnit)(m_dwStorage >> 30); }
	int GetValue () const
		{ return (m_dwStorage & 0x3FFF); }

	CTime Add (CTime ctOther) const;
	CTime Subtract (CTime ctOther) const;

protected:
	DWORD	m_dwStorage;
};

/*
** Yields processing to allow screen updates or user input
*/
void inline YieldProcess()
{
	// preserve caller's screen cursor
	HCURSOR hOldCursor = ::GetCursor();
	MSG	message;
	while (::PeekMessage(&message, NULL, 0, 0, PM_REMOVE))
	{
		::TranslateMessage (&message);
		::DispatchMessage (&message);
	}
	::SetCursor(hOldCursor);
}

/*
** typedef for multi-purpose progress monitoring via callback functions
*/
typedef int (* PFN_PROGRESS_CALLBACK)(int nCode, LPARAM lParam, LPARAM lParamUser);

// progress codes as sent in the nCode parameter above
#define PROG_SETTITLE	1	// lParam sends informational text about current operation as LPCSTR
#define PROG_SETRANGE	2	// lParam holds total number of elements to be processed
#define PROG_SETPOS		3	// lParam holds current element number 
#define PROG_COMPLETE	4	// process is complete
#define PROG_ERROR		5	// process has failed - lParam sends a text string to display
#define PROG_OTHER		6	// lParam holds other info, which will be known to caller
#define PROG_INCREMENT	7	// increment current element number, ignore lParam

// progress return codes
#define PROG_STOP		0	// no process running
#define PROG_RUN		1	// process running
#define PROG_CANCEL		2	// cancel operation pending

// Additional Window functions
void ForceOntoScreen (CWnd * pWnd);
BOOL CopyMenuItem (CMenu * pDest, CMenu * pSrc, UINT nReqID);

// flags for DrawBitmap
#define DBF_NONE	0x00
#define DBF_STRETCH	0x01
#define DBF_TILE	0x02

BOOL DrawBitmap (CDC * pDC, int nBitmapID, int x, int y, int cx, int cy, DWORD dwFlags = DBF_NONE);
BOOL inline DrawBitmap (CDC * pDC, int nBitmapID, CRect const & rect, DWORD dwFlags = DBF_NONE)
	{ return DrawBitmap(pDC, nBitmapID, rect.left, rect.top, rect.right-rect.left, rect.bottom-rect.top, dwFlags); }
void DrawBitmapTrans (HDC hDC, HBITMAP hBitmap, int x, int y, COLORREF clrTransp);

BOOL WriteWindowToDIB (LPCSTR szFile, CWnd *pWnd);
HANDLE DDBToDIB (CBitmap& bitmap, DWORD dwCompression, CPalette* pPal) ;
BOOL WriteDIB (LPCSTR szFile, HANDLE hDIB);
void SplitString(CString &str, char Sep, CStringArray &SplitArray);

/*
** Additional String Functions
*/
// Find a separator in a string, ignoring any that are enclosed in quotes
int FindSeparator(LPCSTR pszString, char chSep);
// split a substring out of a longer string using a separator
CString BreakString		(CString & strSource, char chSeparator = ' ', BOOL bRemove = TRUE, BOOL bIgnoreInStrings = TRUE, BOOL bStripQuotes = FALSE);
// append a string, inserting separator if string isn't empty
void	AppendString	(CString & string, LPCSTR pszAdd, LPCSTR pszSep = ", ");
CString	PrepareSQLString (CString const & string, int nMaxLen = 0);			// encode an SQL string
CString RemoveSQLString (CString & string, BOOL bNoRemove = FALSE);			// decode an SQL String
CString PrepareSQLDate	(CTime time);
CString TitleCase		(const CString & string);									// return title case conversion
void	SplitURL		(LPCSTR pszURL, CString & strSite, CString & strPage);
BOOL	MatchString		(LPCSTR pszWildcards, LPCSTR pszStr);						// perform wildcard matching
BOOL	MatchFileSpec	(LPCSTR pszFileSpec, LPCSTR pszFile);						// does pszFile match the spec pszWildcards ?
CString ListToString	(CDynaList<CString> const & list, char chSep = ';');
DWORD 	ListFromString	(CDynaList<CString> & list, LPCSTR pszString, char chSep = ';');

// Date and Time Functions
CString FormatDateIntl(int nYear, int nMonth, int nDay, char nSep = '/');		// does a country-specific date write and returns length
CString DateToString (CTime const & timDate, char chSep = '/');
CString TimeToString (CTime const & time, BOOL bIncludeSeconds = FALSE);
CString DateTimeToString (CTime const & timDateTime, BOOL bIncludeSeconds = FALSE, char chSep = '/');
CTime StringToDate (LPCSTR pszDate, char chSep = '/');
// Format a count of seconds into a readable string
CString inline FormatSeconds (DWORD dwSeconds)
{
	CString string;
	// is it an hour or more
	if (dwSeconds >= 3600) {
		string.Format ("%dh%2.2dm%2.2ds", (dwSeconds / 3600), ((dwSeconds / 60) % 60), (dwSeconds % 60));
	} else if (dwSeconds >= 60) {
		string.Format ("%dm%2.2ds", (dwSeconds / 60), (dwSeconds % 60));
	} else
		string.Format ("%ds", dwSeconds);
	return string;
}
int DaysInMonth (int nMonth, int nYear);
int DaysInYear (int nYear);
BOOL IsLeapYear (int nYear);
// removes any time element from a CTime object, leaving just the date
CTime TruncateDate (CTime const & ctOriginal);
CTime	ZuluToLocalTime (CTime& ctZuluTime);
CTime	LocaltoZuluTime (CTime& ctLocalTime);

// File / Directory Functions
LPCSTR FindFileExtension (LPCSTR pszFullName);
LPCSTR EndWithBackslash (CString & string);
int FindFiles (CStringList & names, LPCSTR pszPath, LPCSTR pszFileSpec = NULL);
CString GetAlternatePathName (LPCSTR pszPath);
CString MakePathName (LPCSTR pszDir, LPCSTR pszFile);						// complete a pathname including backslash separator
BOOL IsUNCPathValid (LPCSTR pszPath, BOOL bFailIfNotFound);
// return TRUE of pszFolderPath exists
BOOL FolderExists (LPCSTR pszFolderPath);
// create a new folder or path
BOOL FolderCreate (LPCSTR pszFolderPath);

// Miscellaneous
void HandleWin32Error (CWnd * pParent = NULL, LPCSTR pszPrefix = NULL, LPCSTR pszTitle = NULL);
CString GetWin32Error (LPCSTR pszPrefix = NULL);
void HandleMAPIError (int nErrorNo, CWnd * pParent = NULL, LPCSTR pszPrefix = NULL, LPCSTR pszTitle = NULL);
BOOL IsNumeric (LPCSTR pszValue, BOOL bAllowNegative = TRUE, BOOL bAllowFloat = TRUE);
BOOL BrowseFolder (CWnd * pParent, LPCSTR pszTitle, LPSTR pszPath ,BOOL bNetworkOnly = FALSE);						// runs a Shell32 Directory Browse
BOOL CopyFileIfNewer (LPCSTR lpExistingFileName, LPCSTR lpNewFileName);
CString FormatFileSize (DWORD dwSize);
CString FormatMegabytes (DWORD dwMB);
CString FormatMHz (DWORD dwMhz);
CTimeSpan BuildTimeSpan (int nMinutes);
// wraps the Windows API call
CString GetComputerName ();
// Return the active logged on user, even from a Windows Service
CString GetActiveUser ();

// Decrypts a Microsoft Serial Number into the CD key
CString DecodeDigitalProductKey (CByteArray& digitalProductId, bool use2010Method = false);

/*
** Implements a timer for testing program execution speed - traces a message when it goes out of scope
** Use the DEBUG_TIMER macro to ensure that release build doesn't compile it
*/
class CStopWatch
{
public:
	CStopWatch(LPCSTR pszOpName) : m_strOpName(pszOpName)
	{
		Start ();
	}
	void Start (LPCSTR pszOpName = NULL)
	{
		if (pszOpName)
			m_strOpName = pszOpName;
		m_liResult.QuadPart = 0;
		QueryPerformanceCounter(&m_liStart);
	}
	void Stop ()
	{
		LARGE_INTEGER liStop, liFreq, liElapsed;
		QueryPerformanceCounter(&liStop);
		QueryPerformanceFrequency(&liFreq);
		liElapsed.QuadPart = liStop.QuadPart - m_liStart.QuadPart;
		m_liResult.QuadPart = (liElapsed.QuadPart * 1000) / liFreq.QuadPart;
	}
	DWORD GetSeconds ()
	{
		return (DWORD)(m_liResult.QuadPart / 1000);
	}
	DWORD GetMilliSeconds ()
	{
		return (DWORD)(m_liResult.QuadPart % 1000);
	}
	CString GetResult ()
	{
		CString strResult;
		strResult.Format ("Elapsed Time for %s : %d.%3.3d secs", m_strOpName, (DWORD)(m_liResult.QuadPart / 1000), (DWORD)(m_liResult.QuadPart % 1000));
		return strResult;
	}
	~CStopWatch()
	{
		Stop();
		TRACE ("%s\n", GetResult());
	}
protected:
	CString			m_strOpName;
	LARGE_INTEGER	m_liStart, m_liResult;
};

#ifdef _DEBUG
#define DEBUG_TIMER(pszOpName) CStopWatch sw(pszOpName);
#else
#define DEBUG_TIMER(pszOpName)
#endif

#endif