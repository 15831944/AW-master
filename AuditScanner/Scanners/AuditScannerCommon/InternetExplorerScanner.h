#pragma once
#include "wininet.h"

////////////////////////////////////////////////////////////////////////////////////////////
//
//    CInternetExplorerCacheItem
//    ==========================
//
//    Main class for initiating the audit of Internet Explorer usage information
//

//
// Function definitions for WININET.DLL - at least the ones used here...
//
typedef __declspec(dllimport) HANDLE (WINAPI FINDFIRSTURLCACHEENTRY) (IN LPCSTR lpszUrlSearchPattern, OUT LPINTERNET_CACHE_ENTRY_INFO lpFirstCacheEntryInfo, IN OUT LPDWORD lpdwFirstCacheEntryInfoBufferSize);
typedef __declspec(dllimport) BOOL (WINAPI FINDNEXTURLCACHEENTRY) (IN HANDLE hEnumHandle, OUT LPINTERNET_CACHE_ENTRY_INFOA lpNextCacheEntryInfo, IN OUT LPDWORD lpdwNextCacheEntryInfoBufferSize);
typedef __declspec(dllimport) BOOL (WINAPI FINDCLOSEURLCACHE) (IN HANDLE hEnumHandle);

//
// corresponding fn ptr defs
//
typedef FINDFIRSTURLCACHEENTRY * LPFINDFIRSTURLCACHEENTRY;
typedef FINDNEXTURLCACHEENTRY * LPFINDNEXTURLCACHEENTRY;
typedef FINDCLOSEURLCACHE * LPFINDCLOSEURLCACHE;

// Enum of the types of entry in the cache
enum CITYPE { CIT_COOKIES = 1, CIT_HISTORY = 2 };

//
// Class to hold a single entry collected from the Internet Browser
//
class CInternetExplorerCacheItem : public CObject
{
public:
	CInternetExplorerCacheItem & operator= (CInternetExplorerCacheItem const & other)
		{ m_CacheItemType = other.m_CacheItemType; m_strSourceURL = other.m_strSourceURL; m_strLocalFile = other.m_strLocalFile; m_ctLastAccessed = other.m_ctLastAccessed; return *this; }
public:
	CITYPE		m_CacheItemType;
	CString		m_strSourceURL;
	CString		m_strLocalFile;
	CTime		m_ctLastAccessed;
};



//
//	The main Internet Explorer Cache class
//
class CInternetExplorerCache
{
public:
	// constructor
	CInternetExplorerCache(CITYPE eType);
	// destructor
	virtual ~CInternetExplorerCache();
	// Initialize and collect data
	BOOL	Initialize	();
	// return count of results
	DWORD GetCount() const
		{ ASSERT(m_bInitialized); return m_data.GetCount(); }
	// retrieve a single item
	const CInternetExplorerCacheItem & operator[] (DWORD dwIndex) const
		{ ASSERT(dwIndex < m_data.GetCount()); return m_data[dwIndex]; }
	
	// Flag to return whether or not the IE history has been initialized
	BOOL	IsInitialized() const
	{ return m_bInitialized; }
	
protected:
	CITYPE		m_eType;							// data type to search for
	BOOL		m_bInitialized;						// TRUE once data has been retrieved
	CString		m_strSearchPattern;	
	CDynaList<CInternetExplorerCacheItem>	m_data;	// collected data
	
	HINSTANCE	m_hWinInet;							// Handle for dynamic DLL access
	LPFINDFIRSTURLCACHEENTRY	m_pFindFirstUrlCacheEntry;	// Fn Ptrs for dynamic DLL access
	LPFINDNEXTURLCACHEENTRY		m_pFindNextUrlCacheEntry;
	LPFINDCLOSEURLCACHE			m_pFindCloseUrlCache;
};




////////////////////////////////////////////////////////////////////////////////////////////
//
//    CInternetHistoryEntry
//    =====================
//
//    This class holds an Internet History Entry when basic scaning is in force
//
class CInternetHistoryEntry
{
public:
	CInternetHistoryEntry()
	{}
	CInternetHistoryEntry(CTime rawDate, CString& date ,CString& url)
	{ _rawDate = rawDate, _date = date; _url = url; _pageCount = 1; }
	virtual ~CInternetHistoryEntry(void)
	{}

public:
	// Data Accessors
	CString&	Date (void)
	{ return _date; }
	CString&	URL (void)
	{ return _url; }
	int			PageCount (void)
	{ return _pageCount; }
	CTime		RawDate(void)
	{ return _rawDate; }

public:
	void		AddPage(void)
	{ _pageCount++; }

private:
	CTime _rawDate;
	CString	_date;
	CString _url;
	int		_pageCount;
};


class CInternetHistoryList : public CDynaList<CInternetHistoryEntry>
{
public:
	// Add an entry to the list (or update an existing one if a match on date/url is found)
	int	AddEntry(CTime rawDate, CString& date ,CString& url);
};


////////////////////////////////////////////////////////////////////////////////////////////
//
//    CInternetExplorerScanner
//    ========================
//
//    Main class for initiating the audit of Internet Explorer usage information
//
class CInternetExplorerScanner : public CAuditDataScanner
{
public:
	CInternetExplorerScanner(void);
	virtual ~CInternetExplorerScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);
	void			PreProcessInternetHistory	(CInternetHistoryList& historyList);
	
	CInternetExplorerCache* GetInternetCookies()
	{ return pInternetCookiesCache; }
	
public:
	void			ScanHistory		(BOOL trueOrFalse)
	{ _scanHistory = trueOrFalse; }
	//
	void			ScanCookies		(BOOL trueOrFalse)
	{ _scanCookies = trueOrFalse; }
	//
	void			DetailedScan	(BOOL trueOrFalse)
	{ _detailedScan = trueOrFalse; }
	//
	void			LimitDays		(int days)
	{ _limitDays = days; }
	
protected:
	void			ScanInternetExplorer	(void);
	void			ScanInternetExplorer	(CITYPE eType);
	void			SaveDetailedInternetHistory	(CAuditDataFile* pAuditDataFile ,CTime& limitTime);
	void			SaveBasicInternetHistory	(CAuditDataFile* pAuditDataFile ,CTime& limitTime);
	CString			GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem);

private:
	CInternetExplorerCache* pInternetHistoryCache;
	CInternetExplorerCache* pInternetCookiesCache;
	BOOL			_scanHistory;
	BOOL			_scanCookies;
	BOOL			_detailedScan;
	int				_limitDays;
};



