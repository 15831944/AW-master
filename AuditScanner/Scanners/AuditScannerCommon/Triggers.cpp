
// FILE:	Triggers.h
// PURPOSE:	Class to deal with aspects of event triggering dealt with by Audit Scanner
// AUTHOR:	JRF Thornley - copyright (c) Layton Technology 2002
// HISTORY:	20.05.2002 - JRFT - developed from original code by CMD

#include "stdafx.h"
#include "Triggers.h"
#include "ExplorerCache.h"

/*
** Save/Load a single trigger event
*/
void CEventTrigger::Serialize(CArchive & ar)
{
	WORD wVerMajor, wVerMinor;

	if (ar.IsStoring())
	{
		// write version first
		wVerMajor = TRIG_VER_MAJOR;
		wVerMinor = TRIG_VER_MINOR;
		ar << wVerMajor;
		ar << wVerMinor;

		// then the rest of the data
		ar << m_strDataField;
		ar << m_nOperator;
		ar << m_strValue;
	}
	else
	{
		// read version first
		ar >> wVerMajor;
		ar >> wVerMinor;

		// (Can compare the above with Event_VER_MAJOR/Event_VER_MINOR to  
		//				see if any translations are required as we read further)
		// DWORD dwVer = (wVerMajor*10) + wVerMinor;

		ar >> m_strDataField;
		ar >> m_nOperator;
		ar >> m_strValue;
	}
}

/*
** Save / Load a single set of event definitions
*/
void CEventDef::Serialize(CArchive & ar)
{
	WORD wVerMajor, wVerMinor;

	if (ar.IsStoring())
	{
		// write version first
		wVerMajor = EVENT_VER_MAJOR;
		wVerMinor = EVENT_VER_MINOR;
		ar << wVerMajor;
		ar << wVerMinor;

		// then the rest of the data
		ar << m_strName;
		ar << m_strLocations;
		ar << m_strAssets;
		ar << m_nAction;
		ar << m_strActionValue;
		ar << m_dwEventFlags;
		ar << m_strDataField;

		ar << GetCount();
		// then each trigger
		for (DWORD dwItem = 0 ; dwItem < GetCount() ; dwItem++)
			m_pData[dwItem].Serialize(ar);
	}
	else
	{
		DWORD dwTriggerCount;

		// read version first
		ar >> wVerMajor;
		ar >> wVerMinor;

		// (Can compare the above with Event_VER_MAJOR/Event_VER_MINOR to  
		//				see if any translations are required as we read further)
		// DWORD dwVer = (wVerMajor*10) + wVerMinor;

		ar >> m_strName;
		ar >> m_strLocations;
		ar >> m_strAssets;
		ar >> m_nAction;
		ar >> m_strActionValue;
		ar >> m_dwEventFlags;
		ar >> m_strDataField;

		Empty();
		ar >> dwTriggerCount;
		// then each trigger
		for (DWORD dw = 0 ; dw < dwTriggerCount ; dw++)
		{
			CEventTrigger Trigger;
			Trigger.Serialize(ar);
			Add(Trigger);
		}
	}
}

/*
** return TRUE if event applies to pszAsset
*/
BOOL CEventDef::CheckAsset (LPCSTR pszAsset) const
{
	// no assets specified means ANY
	if (m_strAssets.IsEmpty())
		return TRUE;
	// else parse the list of included assets
	CString strBuffer(m_strAssets);
	CString strThisOne = BreakString(strBuffer, '\t', TRUE);
	while (strThisOne.GetLength())
	{
		if (0 == strThisOne.CompareNoCase(pszAsset))
			return TRUE;
		strThisOne = BreakString(strBuffer, '\t', TRUE);
	}
	return FALSE;
}

/*
** Save / Load complete list of event definitions for all assets
*/
void CEventDefList::Serialize(CArchive & ar)
{
	if (ar.IsStoring())
	{
		// write the signature string at the top
		CString strSignature(EVENT_SIG);
		ar << strSignature;

		// then the count of items
		DWORD dwCount = GetCount();
		ar << dwCount;
		
		// then each item
		for (DWORD dwItem = 0 ; dwItem < GetCount() ; dwItem++)
			m_pData[dwItem].Serialize(ar);
	}
	else
	{
		// reading from disk - empty any current data
		Empty();

		// validate the signature string
		CString strTest;
		try
		{
			ar >> strTest;
		}
		catch (CException * pE)
		{
			pE->Delete();
			return;
		}
		
		// if signature invalid or doesn't match then don't read any further - leave list empty
		if (strTest != EVENT_SIG)
			return;

		// all is well - read record count
		DWORD dwCount;
		ar >> dwCount;
		
		// then read and add each item
		for (DWORD dwItem = 0 ; dwItem < dwCount ; dwItem++)
		{
			CEventDef rdTemp;
			rdTemp.Serialize(ar);
			Add(rdTemp);
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
//
// Pure Base Class for Scanners implemented as a trigger
//

/*
** Constructor
*/
CTriggerScanner::CTriggerScanner(LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal)
	: m_strEventName(pszEventName), m_nOpCode(nOpCode), m_strTestVal(pszTestVal)
{
	ASSERT(nOpCode == opChanged || nOpCode == opLessThan || nOpCode == opFolder || nOpCode == opNull || nOpCode == opContains);
}

///////////////////////////////////////////////////////////////////////////////
//
// Collection of variant triggers to manage an entire trigger list
//
CTriggerScanList::~CTriggerScanList()
{
	// remember the contents of our encapsulated array are pointers - they need to be cleaned out
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		delete ((m_pData[dw]));
}

int CTriggerScanList::Setup (CEventDefList const & defs, LPCSTR pszAsset)
{
	int nCount = 0;

	for (DWORD dw = 0 ; dw < defs.GetCount() ; dw++)
	{
		CEventDef const & def = defs[dw];
		if (def.CheckAsset(pszAsset))
		{
			CString strEventName = def.GetName();

			// loop through the triggers
			for (DWORD dwTrigger = 0 ; dwTrigger < def.GetCount() ; dwTrigger++)
			{
				CEventTrigger & trigger = def[dwTrigger];

				// get the type...
				CString strBuffer = trigger.GetField();
				CString strType = BreakString(strBuffer, '.', TRUE);

				if (strType == "Hardware")
				{
					int nPos = strBuffer.ReverseFind('.');
					CString strCat = strBuffer.Left(nPos);
					CString strKey = strBuffer.Mid(nPos + 1);
					strCat.Replace('.', HW_SEP);
					Add (new CTriggerScannerHw(strEventName, trigger.GetOpCode(), trigger.GetValue(), strCat, strKey));
					nCount++;
				}

				else if (strType == "System")
				{
					strBuffer.Replace('.', HW_SEP);
					Add (new CTriggerScannerSystem(strEventName, trigger.GetOpCode(), trigger.GetValue(), strBuffer));
					nCount++;
				}

				else if (strType == "Internet")
				{
					Add (new CTriggerScannerIe(strEventName, trigger.GetOpCode(), trigger.GetValue()));
					nCount++;
				}

				else if (strType == "Operating System")
				{
					CString strKey = strBuffer;
					Add (new CTriggerScannerOs(strEventName, trigger.GetOpCode(), trigger.GetValue(), strKey));
					nCount++;
				}

				else if (strType == "Software")
				{
					Add (new CTriggerScannerApp(strEventName, trigger.GetOpCode(), trigger.GetValue()));
					nCount++;
				}

				else
				{
					TRACE("Unknown Trigger Type %s\n", strType);
					ASSERT(FALSE);
				}
			}
		}
	}
	return nCount;
}

void CTriggerScanList::Save (CAuditDataFile & file) const
{
#pragma message ("CTriggerScanList::Save TODO")
}

void CTriggerScanList::Load (CAuditDataFile & file)
{
#pragma message ("CTriggerScanList::Load TODO")
}

BOOL CTriggerScanList::Scan ()
{
	CLogFile log;
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		log.Format("Scanning trigger %d\n" ,dw);
		if (!((m_pData[dw]))->Scan())
		{
			log.Write("Scan returned an error\n");
			return FALSE;
		}
		log.Write("Scan completed OK\n");
	}
	return TRUE;
}

int CTriggerScanList::Test (CUpdateFile & file)
{
	int nCount = 0;
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		nCount += ((m_pData[dw]))->Test(file);
	}
	return nCount;
}

///////////////////////////////////////////////////////////////////////////////
// Hardware scanning class

/*
** Constructor
*/
CTriggerScannerHw::CTriggerScannerHw(LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszCat, LPCSTR pszKey)
	: CTriggerScanner(pszEventName, nOpCode, pszTestVal), m_strCat(pszCat), m_strKey(pszKey)
{
	m_pDataOld = m_pDataNew = NULL;
	// try and load the correct hardware scanner based on the category name
	m_pDataNew = CHardwareScanner::HardwareScanner(m_strCat, GetCFG());
}

/*
** Destructor
*/
CTriggerScannerHw::~CTriggerScannerHw ()
{
	if (m_pDataNew)
		delete m_pDataNew;
	if (m_pDataOld)
		delete m_pDataOld;
}


/*
** Save current scan results for next time
*/
void CTriggerScannerHw::Save (CAuditDataFile & file) const
{
}

/*
** Collect results for this category
*/
BOOL CTriggerScannerHw::Scan()
{
	// first save the previous results
	if (m_pDataOld)
		delete m_pDataOld;
	m_pDataOld = m_pDataNew;
	m_pDataNew = NULL;

	// set up the scanner and execute it
	m_pDataNew = CHardwareScanner::HardwareScanner(m_strCat, GetCFG());
	if (m_pDataNew)
		return m_pDataNew->Scan();

	return FALSE;
}

/*
** Test to see whether trigger has "fired"
*/
BOOL CTriggerScannerHw::Test (CUpdateFile & file)
{
	BOOL bResult = FALSE;
	CString strNextKey;

	switch (m_nOpCode)
	{
		case opChanged:
			{
				// can only monitor changes if we have previous results
				if (!m_pDataOld || (0 == m_pDataOld->GetCount()))
					return FALSE;

				// run through all values collected at latest scan...
				// Note that all bar the 'System' category we add the category to the key to get the full name
				// System will be in it's own catregory anyway so we don't want to add it on again
				CString strFullKey;
				if (m_strCat.Compare(gszSystem) == 0)
					strFullKey = m_strKey;
				else
					strFullKey = m_strCat + HW_SEP + m_strKey;

				for (DWORD dw = 0 ; dw < m_pDataNew->GetCount() ; dw++)
				{
					// are we only looking for a specific key ?
					strNextKey = (*m_pDataNew)[dw].GetKey();
					if (m_strKey.GetLength() && strNextKey != strFullKey)
						continue;

					// ok, compare values
					CString strThisVal = m_pDataNew->FindValue(strFullKey);
					CString strLastVal = m_pDataOld->FindValue(strFullKey);
					if (strThisVal != strLastVal)
					{
						// got a change!
						CUpdateCat cat;
						cat.SetType(m_strEventName, CUpdateCat::hardware, m_strCat);
						cat.Add(CUpdateItem(m_strKey, strThisVal));
						file.Add(cat);
						bResult = TRUE;
					}
				}
			}
			break;

		case opLessThan:
			{
				// can we find the specified key in the current results ?
				if (!m_pDataNew)
					return FALSE;
				// retrieve the current value
				CString strFullKey = m_strCat + HW_SEP + m_strKey;
				CString strThisVal = m_pDataNew->FindValue(strFullKey);
				if (strThisVal.IsEmpty())
					return FALSE;
				// does it trigger ?
				int nTestVal = atoi(m_strTestVal);
				int nThisVal = atoi(strThisVal);
				if (nThisVal >= nTestVal)
					return FALSE;
				// ok, it triggers - but did it trigger last time? In which case ignore...
				CString strLastVal = m_pDataOld ? m_pDataOld->FindValue(strFullKey) : "";
				if (strLastVal.GetLength())
				{
					int nLastVal = atoi(strLastVal);
					if (nLastVal < nTestVal)
						// triggered last time too, so don't re-notify
						return FALSE;
				}
				// trigger has fired for the first time so do something about it
				CUpdateCat cat;
				cat.SetType(m_strEventName, CUpdateCat::hardware, m_strCat);
				cat.Add(CUpdateItem(m_strKey, strThisVal));
				file.Add(cat);
				return TRUE;
			}
			break;

		default:
			TRACE("Invalid Hardware Trigger operation\n");
			ASSERT(FALSE);
			break;
	}
	return bResult;
}


//=======================================================================================================
//
//    System Category Alerting Class

/*
** Constructor
*/
CTriggerScannerSystem::CTriggerScannerSystem(LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszKey)
	: CTriggerScannerHw(pszEventName, nOpCode, pszTestVal, "System", pszKey)
{
}


///////////////////////////////////////////////////////////////////////////////
// Internet Scanning Class

/*
** Constructor
*/
CTriggerScannerIe::CTriggerScannerIe(LPCSTR pszEventName, int nOpCode, LPCSTR pszSearchText)
	: CTriggerScanner(pszEventName, nOpCode, pszSearchText)
{
	// remember the search mode is always "contains", therefore we need to add wildcards...
	if (m_strTestVal.GetLength())
	{
		if (m_strTestVal.Left(1) != '*')
			m_strTestVal = '*' + m_strTestVal;
		if (m_strTestVal.Right(1) != "*")
			m_strTestVal += "*";
	}

}

/*
** Restore previous scan from disk file
*/
void CTriggerScannerIe::Load (CAuditDataFile & file)
{
#pragma message ("Load function TODO")
}

/*
** Save latest search results to disk file
*/
void CTriggerScannerIe::Save (CAuditDataFile & file) const
{
#pragma message ("Load function TODO")
}

/*
** Detect internet records with occurrences of the search text
*/
BOOL CTriggerScannerIe::Scan()
{
	// first save last results
	m_lastSearch = m_thisSearch;
	m_thisSearch.Empty();

	// do a scan
	CExplorerCache cache(CIT_HISTORY, CTime(0));
	if (!cache.Initialize())
		return FALSE;

	// iterate through results
	for (DWORD dw = 0 ; dw < cache.GetCount() ; dw++)
	{
		CString strUrl = cache[dw].m_strSourceURL;

		if (MatchString(m_strTestVal, strUrl))
			m_thisSearch.Add(strUrl);
	}
	return TRUE;
}

/*
** Test results and report any new instances of the search string
*/
BOOL CTriggerScannerIe::Test (CUpdateFile & file)
{
	CUpdateCat cat;
	cat.SetType (m_strEventName, CUpdateCat::internet, m_strTestVal);

	for (DWORD dw = 0 ; dw < m_thisSearch.GetCount() ; dw++)
	{
		// ignore if this result is in previous search (ie already reported)
		if ((DWORD)-1 != m_lastSearch.Find(m_thisSearch[dw]))
			continue;
		// otherwise generate a result
		CUpdateItem item(m_thisSearch[dw], "");
		cat.Add(item);
	}

	// was anything found?
	if (cat.GetCount())
	{
		file.Add(cat);
		return TRUE;
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////////////////
// Operating System Scanning Class

/*
** Constructor
*/
CTriggerScannerOs::CTriggerScannerOs(LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszKey)
	: CTriggerScannerHw(pszEventName, nOpCode, pszTestVal, "Operating System", pszKey)
{
}

///////////////////////////////////////////////////////////////////////////////
// Registered Application Scanning Class

/*
** Constructor
*/
CTriggerScannerApp::CTriggerScannerApp (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal)
	: CTriggerScannerHw(pszEventName, nOpCode, pszTestVal, "Applications", "")
{
}

/*
** Special comparison handling for applications
*/
BOOL CTriggerScannerApp::Test(CUpdateFile & file)
{
	CLogFile log;
	log.Write("CTriggerScannerApp::Test\n");

	BOOL bResult = FALSE;
	DWORD dw;

	// at this time we only scan for changes
	ASSERT(m_nOpCode == opChanged);

	// can only monitor changes if we have previous results
	if (!m_pDataOld || (0 == m_pDataOld->GetCount()))
	{
		log.Write("CTriggerScannerApp::Test <No previous results found>\n");
		return FALSE;
	}

	// create an update category - in case we need it
	CUpdateCat cat;
	cat.SetType (m_strEventName, CUpdateCat::regApp, "");

	log.Write("CTriggerScannerApp::Test <checking for applications being added>\n");

	// run through all app names collected at latest scan...
	for (dw = 0 ; dw < m_pDataNew->GetCount() ; dw++)
	{
		CString strAppName = (*m_pDataNew)[dw].GetKey();
		log.Format("CTriggerScannerApp::Test <Checking for %s>\n",strAppName);

		// was it there in the last scan?
		if ((DWORD)-1 == m_pDataOld->FindKey(strAppName))
		{
			log.Format("CTriggerScannerApp::Test <%s has been added>\n",strAppName);
			cat.Add (CUpdateItem("Application Added", strAppName));
			bResult = TRUE;
		}
	}

	// now repeat the process the other way round to find deleted apps
	log.Write("CTriggerScannerApp::Test <checking for applications being removed>\n");
	for (dw = 0 ; dw < m_pDataOld->GetCount() ; dw++)
	{
		CString strAppName = ((*m_pDataOld)[dw]).GetKey();
		log.Format("CTriggerScannerApp::Test <Checking for %s>\n",strAppName);

		// ís it still there in the latest scan?
		if ((DWORD)-1 == m_pDataNew->FindKey(strAppName))
		{
			log.Format("CTriggerScannerApp::Test <%s has been removed>\n",strAppName);
			cat.Add (CUpdateItem("Application Removed", strAppName));
			bResult = TRUE;
		}
	}
	
	// were any changes found ?
	if (bResult)
	{
		log.Write("CTriggerScannerApp::Test <Changes were detected>\n");
		file.Add(cat);
	}

	return bResult;
}
