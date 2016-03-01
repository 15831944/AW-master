#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "InternetExplorerScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"Internet|Internet Explorer"
#define S_IEHISTORY					"History"
#define S_IECOOKIE					"Cookie"
#define V_IEHISTORY_URL				"URL"
#define V_IEHISTORY_LASTACCESSED	"Date"
#define V_IEHISTORY_SITE			"Site"
#define V_IEHISTORY_PAGE			"Page"
#define V_IEHISTORY_PAGECOUNT		"Number of Pages"
#define V_IECOOKIE_URL				"URL"

CInternetExplorerScanner::CInternetExplorerScanner(void)
{
	pInternetHistoryCache = new CInternetExplorerCache(CIT_HISTORY);
	pInternetCookiesCache = new CInternetExplorerCache(CIT_COOKIES);

	// Default what we are to scan
	_scanHistory = FALSE;
	_scanCookies = FALSE;
	_detailedScan = FALSE;
	_limitDays = 7;
}

CInternetExplorerScanner::~CInternetExplorerScanner(void)
{
	delete pInternetHistoryCache;
	delete pInternetCookiesCache;
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CInternetExplorerScanner::ScanWMI(CWMIScanner *pScanner)
{
	// Internet Information always collected by a common function
	try
	{
		ScanInternetExplorer();
	}
	catch (CException *pEx)
	{
		return false;
	}
	return true;
}


//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CInternetExplorerScanner::ScanRegistryXP()
{
	try
	{
		ScanInternetExplorer();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CInternetExplorerScanner::ScanRegistryNT()
{
	return ScanRegistryXP();
}

//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CInternetExplorerScanner::ScanRegistry9X()
{
	return ScanRegistryXP();
}


//
//    Scan
void CInternetExplorerScanner::ScanInternetExplorer(void)
{
	// begin the scan...
	if (_scanHistory)
	{
		try 
		{
			pInternetHistoryCache->Initialize();
		}
		catch (CException *pEx)
		{
			throw pEx;
		}
	}

	if (_scanCookies)
	{
		try
		{
			pInternetCookiesCache->Initialize();
		}
		catch (CException *pEx)
		{
			throw pEx;
		} 
	}

	return;
}


//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CInternetExplorerScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CString strValue;
	CLogFile log;

	log.Write("CInternetExplorerScanner::SaveData in");
	
	// If we have no data then return immediately
	if ((pInternetHistoryCache == NULL) || (!pInternetHistoryCache->IsInitialized()))
		return true;

	// 8.3.7
	// We need to limit the 
	// Determine the date limit for Internet records as we do not want to return everything
	CTime timeNow = CTime::GetCurrentTime();
	CTimeSpan tsLimitDays(_limitDays ,0 ,0 ,0);
	CTime limitTime = timeNow - tsLimitDays;

	// Format the Internet History category name
	CString categoryName;

	// For History, are we saving detailed or basic information?
	if (_detailedScan)
		SaveDetailedInternetHistory(pAuditDataFile ,limitTime);
	else
		SaveBasicInternetHistory(pAuditDataFile ,limitTime);

	// Now loop through looking for cookies and add them all (subject to date constraints)	
	if ((pInternetCookiesCache != NULL) && (pInternetCookiesCache->IsInitialized()))
	{
		for (DWORD dw = 0 ; dw < pInternetCookiesCache->GetCount() ; dw++)
		{
			// retrieve the item
			CInternetExplorerCacheItem const & item = (*pInternetCookiesCache)[dw];

			// Is this item before the limit date?
			// Discard it if yes
			if (item.m_ctLastAccessed < limitTime)
				continue; 
				
			// Format the category name to include the cookie URL
			categoryName.Format("%s|%s|%s" ,HARDWARE_CLASS ,S_IECOOKIE ,item.m_strSourceURL);

			// Create a CAuditDataFileCategory object with this name and flag as no history but grouped
			CAuditDataFileCategory cookiesCategory(categoryName ,FALSE ,TRUE);
			CAuditDataFileItem s1(V_IEHISTORY_LASTACCESSED ,DateTimeToString(item.m_ctLastAccessed));
			cookiesCategory.AddItem(s1);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddInternetItem(cookiesCategory);
		}
	}

	log.Write("CInternetExplorerScanner::SaveData out");

	return true;
}


//
//    Save the Internet History in detailed format
//
void CInternetExplorerScanner::SaveDetailedInternetHistory	(CAuditDataFile* pAuditDataFile ,CTime& limitTime)
{
	CLogFile log;

	log.Write("CInternetExplorerScanner::SaveDetailedInternetHistory in");

	CString categoryName;
	categoryName.Format("%s|%s" ,HARDWARE_CLASS ,S_IEHISTORY);

	// Loop through the Internet Explorer results first time and add history entries
	CString strURL, strFile;
	CTime ctDate;
	for (DWORD dw = 0 ; dw < pInternetHistoryCache->GetCount() ; dw++)
	{
		// Add a category for this history item
		CAuditDataFileCategory historyCategory(categoryName);
		CString strSite, strPage;

		// retrieve the item
		CInternetExplorerCacheItem const & item = (*pInternetHistoryCache)[dw];

		// Is this item before the limit date?
		// Discard it if yes
		if (item.m_ctLastAccessed < limitTime)
			continue; 

		// Split the URl into its components
		SplitURL(item.m_strSourceURL, strSite, strPage);
		//
		CAuditDataFileItem s1(V_IEHISTORY_LASTACCESSED ,DateTimeToString(item.m_ctLastAccessed));
		CAuditDataFileItem s2(V_IEHISTORY_SITE ,strSite);
		CAuditDataFileItem s3(V_IEHISTORY_PAGE ,strPage);

		log.Format("adding history for site %s \n", strSite);

		// Add the items to the category
		historyCategory.AddItem(s1);
		historyCategory.AddItem(s2);
		historyCategory.AddItem(s3);
		
		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddInternetItem(historyCategory);
	}

	log.Write("CInternetExplorerScanner::SaveDetailedInternetHistory out");

	return;
}



//
//    Save the Internet History in basic format
//
void CInternetExplorerScanner::SaveBasicInternetHistory	(CAuditDataFile* pAuditDataFile ,CTime& limitTime)
{
	CLogFile log;

	log.Write("CInternetExplorerScanner::SaveBasicInternetHistory in");

	// First of all we pre-process the results and create a list f entries and counts
	CInternetHistoryList historyList;
	PreProcessInternetHistory(historyList);

	// Now that we have pre-processed the list add the date, site and count to the audit data file
	CString categoryName;
	CString strPageCount;
	//
	for (DWORD dw=0; dw<historyList.GetCount(); dw++)
	{
		CInternetHistoryEntry* pHistoryEntry = &historyList[dw];
		//
		strPageCount.Format("%d" ,pHistoryEntry->PageCount());
		CAuditDataFileItem s1(V_IEHISTORY_PAGECOUNT ,strPageCount);

		// Create a audited item category based on the URL
		categoryName.Format("%s|%s|%s|%s" ,HARDWARE_CLASS ,S_IEHISTORY ,pHistoryEntry->Date() ,pHistoryEntry->URL());

		// Create a CAuditDataFileCategory object with this name and flag as no history but grouped
		CAuditDataFileCategory historyCategory(categoryName ,FALSE ,TRUE);

		log.Format("adding history for URL %s \n", pHistoryEntry->URL());

		// ...and add its attributes
		historyCategory.AddItem(s1);
		
		// ...and add the category to the AuditDataFile
		pAuditDataFile->AddInternetItem(historyCategory);
	}

	log.Write("CInternetExplorerScanner::SaveBasicInternetHistory out");
}


void CInternetExplorerScanner::PreProcessInternetHistory(CInternetHistoryList& historyList)
{
	// Determine the date limit for Internet records as we do not want to return everything
	CTime timeNow = CTime::GetCurrentTime();
	CTimeSpan tsLimitDays(_limitDays ,0 ,0 ,0);
	CTime limitTime = timeNow - tsLimitDays;

	for (DWORD dw = 0 ; dw < pInternetHistoryCache->GetCount() ; dw++)
	{
		CString strSite, strPage;
		CInternetExplorerCacheItem const & item = (*pInternetHistoryCache)[dw];

		// Is this item before the limit date?
		// Discard it if yes
		if (item.m_ctLastAccessed < limitTime)  
			continue; 

		// Split the URL into its components
		SplitURL(item.m_strSourceURL, strSite, strPage);

		// Add to our internal list
		historyList.AddEntry(item.m_ctLastAccessed, DateToString(item.m_ctLastAccessed) ,strSite);
		
		// Add to our internal list
		//CString accessedDate = DateToString(item.m_ctLastAccessed);
		////accessedDate.Format ("%s %2.2d:%2.2d", accessedDate, item.m_ctLastAccessed.GetHour(), item.m_ctLastAccessed.GetMinute());
		//
		//CString hour = "";
		//hour.Format("%2.2d", item.m_ctLastAccessed.GetHour());
		//
		//CString minute = "";
		//minute.Format("%2.2d", item.m_ctLastAccessed.GetMinute());

		//CString accessedDate1 = accessedDate + " " + hour + ":" + minute;

		////accessedDate.Format ("%s %2.2d:%2.2d", accessedDate, item.m_ctLastAccessed.GetHour(), item.m_ctLastAccessed.GetMinute());

		//historyList.AddEntry(accessedDate1 ,strSite);
	}
}


//////////////////////////////////////////////////////////////////////////////////////
//
//    Encapsulated CInternetExplorerCache class
//
CInternetExplorerCache::CInternetExplorerCache(CITYPE eType) 
	: m_bInitialized(FALSE), m_eType(eType)
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
			m_pFindNextUrlCacheEntry = (LPFINDNEXTURLCACHEENTRY)GetProcAddress(m_hWinInet, "FindNextUrlCacheEntryA");
			m_pFindCloseUrlCache = (LPFINDCLOSEURLCACHE)GetProcAddress(m_hWinInet, "FindCloseUrlCache");
		}
	}
}

CInternetExplorerCache::~CInternetExplorerCache()
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
BOOL CInternetExplorerCache::Initialize	()
{
	try
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
			
						// filter out anything not starting "http://" as we only want internet activity
						// also ignore if we have specified a base date and this is before it
						CTime ItemTime(lpCacheEntry->LastAccessTime);
						if ((m_eType == CIT_COOKIES) 
						||  ((m_eType == CIT_HISTORY) &&  (memcmp("http:" ,pStart ,5) == 0)))
						{
							CInternetExplorerCacheItem item;
							item.m_strSourceURL = pStart;
							item.m_strLocalFile = lpCacheEntry->lpszLocalFileName;
							item.m_ctLastAccessed = lpCacheEntry->LastAccessTime;
							m_data.Add(item);
						}
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
	catch (CException *pEx)
	{
		throw pEx;
	}
}



//////////////////////////////////////////////////////////////////////////////////////
//
//    CInternetHistoryList
//
int CInternetHistoryList::AddEntry	(CTime rawDate, CString& strDate ,CString& strURL)
{
	// Search the list to see if we can find an existing entry for this date/url
	for (int isub=0; isub<(int)GetCount(); isub++)
	{
		CInternetHistoryEntry* pHistoryEntry = &(m_pData[isub]);
		if ((pHistoryEntry->Date() == strDate) && (pHistoryEntry->URL() == strURL))
		{
			pHistoryEntry->AddPage();
			return 0;
		}
	}

	// No match found so add a new entry
	CInternetHistoryEntry newEntry(rawDate, strDate, strURL);
	Add(newEntry);

	return 0;
}
