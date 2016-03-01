// ExplorerCache.cpp: implementation of the CExplorerCache class.
//
//////////////////////////////////////////////////////////////////////
// HISTORY:	JRFT - 21.05.2001 - now links to WinInet.dll at runtime
//
//		40C009		Chris Drew	07-NOV-2001
//		Restrict range of Internet items recovered
//
#include "stdafx.h"
#include "ExplorerCache.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CExplorerCache::CExplorerCache(CITYPE eType ,CTime& BaseDate) 
	: m_bInitialized(FALSE), m_eType(eType), m_BaseDate(BaseDate)
{
	// try and link to WININET.DLL
	m_hWinInet = NULL;
	if (0 != SearchPath(NULL, "WININET.DLL", NULL, 0, NULL, NULL))
	{
		// DLL is there - try and load it
		m_hWinInet = LoadLibrary ("WININET.DLL");
		if (m_hWinInet)
		{
			m_pFindFirstUrlCacheEntry = (LPFINDFIRSTURLCACHEENTRY)GetProcAddress(m_hWinInet, "FindFirstUrlCacheEntryA");
			ASSERT (m_pFindFirstUrlCacheEntry);
			m_pFindNextUrlCacheEntry = (LPFINDNEXTURLCACHEENTRY)GetProcAddress(m_hWinInet, "FindNextUrlCacheEntryA");
			ASSERT (m_pFindNextUrlCacheEntry);
			m_pFindCloseUrlCache = (LPFINDCLOSEURLCACHE)GetProcAddress(m_hWinInet, "FindCloseUrlCache");
			ASSERT (m_pFindCloseUrlCache);
		}
	}
}

CExplorerCache::~CExplorerCache()
{
	if (m_hWinInet)
		FreeLibrary(m_hWinInet);
}



//
//    Initialize
//    ==========
//
//    Recover the required cache items.
//
BOOL CExplorerCache::Initialize	()
{
	// ensure WinInet library is present
	if (NULL == m_hWinInet)
		return FALSE;

    BOOL bDone = FALSE;
    INTERNET_CACHE_ENTRY_INFO* lpCacheEntry = NULL;  
 
    DWORD  dwTrySize, dwEntrySize = 4096; // start buffer size    
    HANDLE hCacheDir = NULL;    
    DWORD  dwError = ERROR_INSUFFICIENT_BUFFER;

//
//    Set up the search criteria.
//
	switch (m_eType)
	{
		case CIT_COOKIES:
			m_strSearchPattern = "cookie:";
			break;

		case CIT_HISTORY:
			m_strSearchPattern = "visited:";
			break;
		
		default:
			m_strSearchPattern = "temp:";
			break;
	}

//
//   ...and read the cache
//
   do 
    {                               
        switch (dwError)
        {
            // need a bigger buffer
            case ERROR_INSUFFICIENT_BUFFER: 

                delete [] lpCacheEntry;            
                lpCacheEntry = (INTERNET_CACHE_ENTRY_INFO*) new char[dwEntrySize];
                lpCacheEntry->dwStructSize = dwEntrySize;
                dwTrySize = dwEntrySize;
                BOOL bSuccess;

                if (hCacheDir == NULL)  
				{	
					hCacheDir = m_pFindFirstUrlCacheEntry(m_strSearchPattern ,lpCacheEntry ,&dwTrySize);
					bSuccess = (hCacheDir != NULL);
				}
				
				else
				{
                    bSuccess = m_pFindNextUrlCacheEntry(hCacheDir, lpCacheEntry, &dwTrySize);
				}

                if (bSuccess)
                    dwError = ERROR_SUCCESS;    
                else
                {
                    dwError = GetLastError();
                    dwEntrySize = dwTrySize; // use new size returned
                }
                break;

            // we are done
            case ERROR_NO_MORE_ITEMS:
                bDone = TRUE;
                break;

             // we have got an entry
            case ERROR_SUCCESS:
			{
				// have we found a cookie, or a History Entry ?
				if ( ((m_eType == CIT_COOKIES) && ((lpCacheEntry->CacheEntryType & COOKIE_CACHE_ENTRY) != 0) )
				||   ((m_eType == CIT_HISTORY) && ((lpCacheEntry->CacheEntryType & URLHISTORY_CACHE_ENTRY) != 0)))
				{				 
					char * pStart = strchr(lpCacheEntry->lpszSourceUrlName ,'@');
					// strip leading stuff off the string
					if (pStart == NULL)
						pStart = lpCacheEntry->lpszSourceUrlName;
					else
						pStart++;
		
					// +40C009
					// filter out anything not starting "http://" as we only want internet activity
					// also ignore if we have specified a base date and this is before it
					CTime ItemTime(lpCacheEntry->LastAccessTime);
					if ((m_eType == CIT_COOKIES) 
					||  ((m_eType == CIT_HISTORY) &&  (memcmp("http:" ,pStart ,5) == 0)))
					{
						if (m_BaseDate <= ItemTime)
						{
							CCacheItem item;
							item.m_strSourceURL = pStart;
							item.m_strLocalFile = lpCacheEntry->lpszLocalFileName;
							item.m_ctLastAccessed = lpCacheEntry->LastAccessTime;
							m_data.Add(item);
						}
					}
					// -40C009
				}

                // get ready for next entry
                dwTrySize = dwEntrySize;
                if (m_pFindNextUrlCacheEntry(hCacheDir, lpCacheEntry, &dwTrySize))
				{
                    dwError = ERROR_SUCCESS;          
				}
                else
                {
                    dwError = GetLastError();
                    dwEntrySize = dwTrySize; // use new size returned
                }                    
                break;
			}

            // unknown error
            default:
                bDone = TRUE;                
                break;
        }

        if (bDone)
        {   
            delete [] lpCacheEntry; 
            if (hCacheDir)
                m_pFindCloseUrlCache(hCacheDir);         
                                  
        }
    } 
	while (!bDone);

	m_bInitialized = TRUE;
	return TRUE;
}

