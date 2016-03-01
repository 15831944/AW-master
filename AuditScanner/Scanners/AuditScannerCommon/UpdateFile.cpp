
// FILE:	UpdateFile.cpp
// PURPOSE:	Classes to handle transmission of partial audit information from scanner to manager
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology
// HISTORY:	JRFT - 01.07.2002 - written

#include "stdafx.h"

#define UPDATE_FILE_EXT	"upd"


CUpdateItem::CUpdateItem (LPCSTR pszKey, LPCSTR pszVal) : m_strKey(pszKey), m_strVal(pszVal)
{
}

/*
** Serialization
*/
void CUpdateItem::Serialize(CArchive & ar)
{
	if (ar.IsStoring())
	{
		ar << m_strKey;
		ar << m_strVal;
	}
	else
	{
		ar >> m_strKey;
		ar >> m_strVal;
	}
}

/*
** set the category that the info belongs to
*/
void CUpdateCat::SetType (LPCSTR pszEventName, eUpdateType nType, LPCSTR pszCat)
{
	m_strEventName = pszEventName;
	m_nType = nType;
	m_strCat = pszCat;
}

CUpdateCat const & CUpdateCat::operator+= (CUpdateCat const & other)
{
	// run through the source items
	for (DWORD dw = 0 ; dw < other.GetCount() ; dw++)
	{
		DWORD dwItem = Find(other[dw].GetKey());
		// does it already exist ?
		if ((DWORD)-1 == dwItem)
		{
			// no - add it
			Add(other[dw]);
		}
		else
		{
			// yes - update with latest value
			m_pData[dwItem].SetVal(other[dw].GetVal());
		}
	}
	return *this;
}

/*
** Serialization
*/
void CUpdateCat::Serialize (CArchive & ar)
{
	if (ar.IsStoring())
	{
		ar << m_strEventName;
		ar << m_nType;
		ar << m_strCat;
		ar << m_dwCount;

		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
			m_pData[dw].Serialize(ar);
	}
	else
	{	
		int nTemp;
		DWORD dwCount;

		ar >> m_strEventName;
		ar >> nTemp;
		ar >> m_strCat;
		ar >> dwCount;
		m_nType = (eUpdateType)nTemp;

		for (DWORD dw = 0 ; dw < dwCount ; dw++)
		{
			CUpdateItem uiTemp;
			uiTemp.Serialize(ar);
			Add(uiTemp);
		}
	}
}

/*
** Search for a contained item by it's key string
*/
DWORD CUpdateCat::Find (LPCSTR pszKey) const
{
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		if (m_pData[dw].GetKey() == pszKey)
			return dw;
	}
	return (DWORD)-1;
}

/*
** Explicit Constructor
*/
CUpdateFile::CUpdateFile (LPCSTR pszAsset) : m_strAsset(pszAsset)
{
}

/*
** Search for a contained category by event name
*/
DWORD CUpdateFile::Find (LPCSTR pszEventName) const
{
	for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
	{
		if (m_pData[dw].EventName() == pszEventName)
			return dw;
	}
	return (DWORD)-1;
}

/*
** Merge a second set of update data
*/
CUpdateFile const & CUpdateFile::operator+= (CUpdateFile const & other)
{
	// trawl through the categories of the new object...
	for (DWORD dwCat = 0 ; dwCat < other.m_dwCount ; dwCat++)
	{
		CUpdateCat const & newCat = other[dwCat];

		// does it already exist?
		DWORD dwThisCat = Find (newCat.EventName());
		if ((DWORD)-1 == dwThisCat)
		{
			// no - add it
			Add(newCat);
		}
		else
		{
			// update it
			m_pData[dwThisCat] += newCat;
		}
	}
	return *this;
}

/*
** store the complete record to specified data path
*/
BOOL CUpdateFile::Write (LPCSTR pszPath)
{
	// create the file name
	CString strBuffer;
	strBuffer.Format("%s.%s", m_strAsset, UPDATE_FILE_EXT);
	strBuffer = MakePathName (pszPath, strBuffer);

	// does the file exist ?
	CFile file;

	if (file.Open (strBuffer, CFile::modeRead))
	{
		// already a file - read it and merge latest data...
		CUpdateFile tempFile;
		{
			try
			{
				CArchive ar(&file, CArchive::load);
				tempFile.Serialize(ar);
				file.Close();
			}
			catch (CArchiveException * e)
			{
				TRACE("Error reading existing update file %d\n", strBuffer);
				e->Delete();
				return FALSE;
			}
		}
		tempFile += *this;
		// now write it back...
		if (file.Open(strBuffer, CFile::modeCreate|CFile::modeWrite))
		{
			try
			{
				CArchive ar(&file, CArchive::store);
				tempFile.Serialize(ar);
			}
			catch (CArchiveException * e)
			{
				TRACE("Error writing update file %d\n", strBuffer);
				e->Delete();
				return FALSE;
			}
		}
	}
	else
	{
		// no file exists so simply create one and write it
		if (file.Open(strBuffer, CFile::modeCreate|CFile::modeWrite))
		{
			try
			{
				CArchive ar(&file, CArchive::store);
				Serialize(ar);
			}
			catch (CArchiveException * e)
			{
				TRACE("Error writing update file %d\n", strBuffer);
				e->Delete();
				return FALSE;
			}
		}
	}
	return TRUE;
}


/*
** Serialization
*/
void CUpdateFile::Serialize (CArchive& ar)
{
	if (ar.IsStoring())
	{
		ar << m_strAsset;
		ar << m_dwCount;

		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
			m_pData[dw].Serialize(ar);
	}
	else
	{
		DWORD dwCount;
		ar >> m_strAsset;
		ar >> dwCount;

		for (DWORD dw = 0 ; dw < dwCount ; dw++)
		{
			CUpdateCat ucTemp;
			ucTemp.Serialize(ar);
			Add(ucTemp);
		}
	}
}
