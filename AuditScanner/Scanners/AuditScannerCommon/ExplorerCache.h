// ExplorerCache.h: interface for the CExplorerCache class.
//
//////////////////////////////////////////////////////////////////////
// HISTORY:	JRFT - 21.05.2001 - now links to WinInet.dll at runtime

#if !defined(AFX_ExplorerCache_H__602021C5_018E_11D5_B303_00E07D9456BD__INCLUDED_)
#define AFX_ExplorerCache_H__602021C5_018E_11D5_B303_00E07D9456BD__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "wininet.h"

/*
** Function definitions for WININET.DLL - at least the ones used here...
*/
typedef __declspec(dllimport) HANDLE (WINAPI FINDFIRSTURLCACHEENTRY) (IN LPCSTR lpszUrlSearchPattern, OUT LPINTERNET_CACHE_ENTRY_INFO lpFirstCacheEntryInfo, IN OUT LPDWORD lpdwFirstCacheEntryInfoBufferSize);
typedef __declspec(dllimport) BOOL (WINAPI FINDNEXTURLCACHEENTRY) (IN HANDLE hEnumHandle, OUT LPINTERNET_CACHE_ENTRY_INFOA lpNextCacheEntryInfo, IN OUT LPDWORD lpdwNextCacheEntryInfoBufferSize);
typedef __declspec(dllimport) BOOL (WINAPI FINDCLOSEURLCACHE) (IN HANDLE hEnumHandle);
/*
** corresponding fn ptr defs
*/
typedef FINDFIRSTURLCACHEENTRY * LPFINDFIRSTURLCACHEENTRY;
typedef FINDNEXTURLCACHEENTRY * LPFINDNEXTURLCACHEENTRY;
typedef FINDCLOSEURLCACHE * LPFINDCLOSEURLCACHE;

// Enum of the types of entry in the cache
enum CITYPE { CIT_COOKIES = 1, CIT_HISTORY = 2 };

/*
** Class to hold a single entry collected from the Internet Browser
*/
class CCacheItem : public CObject
{
public:
	CCacheItem & operator= (CCacheItem const & other)
		{ m_CacheItemType = other.m_CacheItemType; m_strSourceURL = other.m_strSourceURL; m_strLocalFile = other.m_strLocalFile; m_ctLastAccessed = other.m_ctLastAccessed; return *this; }
public:
	CITYPE		m_CacheItemType;
	CString		m_strSourceURL;
	CString		m_strLocalFile;
	CTime		m_ctLastAccessed;
};

/*
** The main Explorer Cache class
*/
class CExplorerCache
{
public:
	// constructor
	CExplorerCache(CITYPE eType, CTime& BaseDate);
	// destructor
	virtual ~CExplorerCache();
	// Initialize and collect data
	BOOL	Initialize	();
	// return count of results
	DWORD GetCount() const
		{ ASSERT(m_bInitialized); return m_data.GetCount(); }
	// retrieve a single item
	const CCacheItem & operator[] (DWORD dwIndex) const
		{ ASSERT(dwIndex < m_data.GetCount()); return m_data[dwIndex]; }

protected:
	CITYPE		m_eType;							// data type to search for
	BOOL		m_bInitialized;						// TRUE once data has been retrieved
	CString		m_strSearchPattern;	
	CDynaList<CCacheItem>	m_data;					// collected data
	CTime&		m_BaseDate;							// Base time to collect from //40C009
	
	HINSTANCE	m_hWinInet;									// Handle for dynamic DLL access
	LPFINDFIRSTURLCACHEENTRY	m_pFindFirstUrlCacheEntry;	// Fn Ptrs for dynamic DLL access
	LPFINDNEXTURLCACHEENTRY		m_pFindNextUrlCacheEntry;
	LPFINDCLOSEURLCACHE			m_pFindCloseUrlCache;
};

#endif // !defined(AFX_ExplorerCache_H__602021C5_018E_11D5_B303_00E07D9456BD__INCLUDED_)
