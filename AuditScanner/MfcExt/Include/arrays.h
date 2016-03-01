
// FILE:	Arrays.h
// PURPOSE:	Declaration of object array template class
// AUTHOR:	JRF Thornley - Copyright (C) InControl Desktop Systems Ltd 1999,2000
// HISTORY:	JRFT - 12.04.1999 - Written
//			JRFT - 05.12.2000 - Memory now reallocates in 10% chunks

/*
** Notes.
** This template allows the user to quickly define a list class for any type of object,
** either system or user defined. The list is dynamic with an adjustable growth threshold
** and contains routines to add or to add non-matching items to the array. The complete
** List can be serialized using the provided CArchive insertion/extraction operators.
**
** Example:
** CDynaList<DWORD> dwList;		// defines a dynamic array of DWORDS
** dwList.Add(76);				// now contains a single element with value 76
** dwList.AddNonMatching(76);	// has no effect on the array
** DWORD dw = dwList[0];		// assigns dw with a copy of the sole list item
** DWORD dw2 = dwList[1];		// overflow error - will ASSERT in debug mode
*/

#ifndef _ARRAYS_DEF_
#define _ARRAYS_DEF_

template <class X> class CDynaList
{
public:
	// Default Constructor
	CDynaList()
	{
		m_dwCount = m_dwMax = 0;
		m_pData = NULL;
	}

	// Copy Constructor
	CDynaList(CDynaList const & other)
	{
		m_dwCount = 0;
		m_pData = NULL;
		*this = other;
	}

	// Destructor
	virtual ~CDynaList()
	{
		Empty();
	}

	// access
	X & operator[] (DWORD dwIndex) const
	{
		ASSERT (dwIndex < m_dwCount);
		return m_pData[dwIndex];
	}

	// array size
	DWORD GetCount() const
	{
		return m_dwCount;
	}

	// Return TRUE if list is empty
	BOOL IsEmpty() const
	{
		return (!m_dwCount);
	}

	// locate an item in the list
	DWORD Find (X const & object) const
	{
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++) {
			if (m_pData[dw] == object)
				return dw;
		}
		return (DWORD)-1;
	}
	
	// Assignment operator
	CDynaList & operator= (CDynaList const & other)
	{
		Empty();
		m_dwCount = other.m_dwCount;
		m_dwMax = other.m_dwMax;
		if (m_dwMax)
			m_pData = new X [m_dwMax];
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
			m_pData[dw] = other.m_pData[dw];
		return *this;
	}

	// Comparison
	BOOL operator== (CDynaList<X> const & other) const
	{
		if (m_dwCount != other.m_dwCount)
			return FALSE;
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
			if (!(m_pData[dw] == other.m_pData[dw]))
				return FALSE;
		return TRUE;
	}

	// Concatenation of a second list
	void operator+= (CDynaList<X> const & other)
	{
		for (DWORD dw = 0 ; dw < other.m_dwCount ; dw++)
			Add (other[dw]);
	}

	// add an item to the list
	DWORD Add (X const & newObject)
	{
		if (m_dwCount >= m_dwMax)
			Expand();
		m_pData[m_dwCount] = newObject;
		return m_dwCount++;
	}

	// add an item to the list
	DWORD AddAt (X const & newObject ,DWORD dwIndex)
	{
		ASSERT (dwIndex <= m_dwCount);
		if (m_dwCount >= m_dwMax)
			Expand();

		// Shuffle all items above the insertion point up in the list
		for (DWORD dw = m_dwCount ; dw > dwIndex ; dw--) 
			m_pData[dw] = m_pData[dw - 1];

		m_dwCount++;
		m_pData[dwIndex] = newObject;
		return dwIndex;
	}

	// add an item to the list only if not already contained
	DWORD AddNonMatching (X const & newObject)
	{
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++) {
			if (m_pData[dw] == newObject)
				return dw;
		}
		return Add(newObject);
	}

	// remove an item from list and shuffle later items up
	void Remove (DWORD dwIndex) 
	{
		ASSERT (dwIndex < m_dwCount);
		for (DWORD dw = dwIndex ; dw < (m_dwCount - 1) ; dw++) {
			m_pData[dw] = m_pData[dw + 1];
		}
		m_dwCount--;
	}

	// remove all items from the list, freeing allocated memory
	void Empty()
	{
		if (m_dwMax)
			delete [] m_pData;
		m_dwCount = m_dwMax = 0;
		m_pData = NULL;
	}

	// store / retrieve list using CArchive
	void Serialize (CArchive & ar)
	{
		if (ar.IsStoring())
		{
			ar << *this;
		}
		else
		{
			ar >> *this;
		}
	}

	// archive serialization via friend functions - see below
	friend CArchive & operator<< (CArchive & ar, const CDynaList<X> & object);
	friend CArchive & operator>> (CArchive & ar, CDynaList<X> & object);

protected:
	// reallocate memory when growth threshold exceeded
	void Expand(DWORD dwNewCount = 0)
	{
		if (dwNewCount)
		{
			m_dwMax = max(m_dwMax,dwNewCount);
		}
		else
		{
			m_dwMax += max(10, (m_dwMax / 10));
		}
		X * pTemp = new X [m_dwMax];
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
			pTemp[dw] = m_pData[dw];
		if (m_dwCount)
			delete [] m_pData;
		m_pData = pTemp;
	}
protected:
	DWORD	m_dwCount;
	DWORD	m_dwMax;
	X *		m_pData;
};

// Friend operator to write list to archive
template<class X> CArchive & operator<< (CArchive & ar, const CDynaList<X> & object)
{
	ar << object.m_dwCount;
	for (DWORD dw = 0 ; dw < object.m_dwCount ; dw++)
		ar << object.m_pData[dw];
	return ar;
}

// friend operator to read list from archive
template<class X> CArchive & operator>> (CArchive & ar, CDynaList<X> & object)
{
	object.Empty();
	ar >> object.m_dwCount;
	object.m_dwMax = object.m_dwCount;
	if (object.m_dwMax)
		object.m_pData = new X[object.m_dwMax];
	for (DWORD dw = 0 ; dw < object.m_dwCount ; dw++)
		ar >> object.m_pData[dw];
	return ar;
}

///////////////////////////////////////////////////////////////////////////////
// classes to assist in sorting arrays of referenced data
//

class CSortItem
{
	friend class CSortList;
public:
	CSortItem  ()
		{}
	CSortItem (LPCSTR pszText, DWORD dwItemID ,DWORD dwUserData = 0) 
		: m_strText(pszText), m_dwItemID(dwItemID), m_dwUserData(dwUserData)
		{}
	const CString & GetText () const
		{ return m_strText; }
	DWORD GetID() const
		{ return m_dwItemID; }
	DWORD GetData() const
		{ return m_dwUserData; }
	void SetData(DWORD dwUserData)
		{ m_dwUserData = dwUserData; }

	BOOL operator== (const CSortItem & other) const
		{ return (m_dwItemID == other.m_dwItemID && m_strText == other.m_strText && m_dwUserData == other.m_dwUserData); }
protected:
	CString	m_strText;
	DWORD	m_dwItemID;
	DWORD	m_dwUserData;
};

class CSortList : public CDynaList<CSortItem>
{
public:
	CSortList()
	{
		m_bSorted = FALSE;
	}
	// override the "sort" flag
	void SetSorted (BOOL bSorted = TRUE)
	{
		m_bSorted = bSorted;
	}
	const CSortItem & operator[] (DWORD dwIndex) const
	{
		if (!m_bSorted)
			((CSortList*)this)->Sort();
		ASSERT(dwIndex < m_dwCount);
		return m_pData[dwIndex];
	}
	void Add(CSortItem const & newItem)
	{
		m_bSorted = FALSE;
		CDynaList<CSortItem>::Add(newItem);
	}
	DWORD Find (CSortItem const & item) const
	{
		// Note that find only considers the strings, not the IDs
		if (!m_bSorted)
			((CSortList*)this)->Sort();
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			if (m_pData[dw].m_strText != item.m_strText)
				continue;
			return dw;
		}
		return (DWORD)-1;
	}
	DWORD FindID (LPCSTR pszText) const
	{
		if (!m_bSorted)
			((CSortList*)this)->Sort();
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			if (m_pData[dw].m_strText == pszText)
				return m_pData[dw].m_dwItemID;
		}
		return 0;
	}
	CString FindString (DWORD dwItemID, LPDWORD pUserData = NULL) const
	{
		if (!m_bSorted)
			((CSortList*)this)->Sort();
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			if (m_pData[dw].m_dwItemID == dwItemID)
			{
				if (pUserData)
					*pUserData = m_pData[dw].m_dwUserData;
				return m_pData[dw].m_strText;
			}
		}
		return "";
	}
	DWORD FindData (DWORD dwItemID) const
	{
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			if (m_pData[dw].m_dwItemID == dwItemID)
			{
				return m_pData[dw].m_dwUserData;
			}
		}
		return 0;
	}
protected:
	void Sort ()
	{
		if (m_dwCount <= 1)
			return;

		// get a reasonable maximum string length for sorting
		int nMaxLen = 0;
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			nMaxLen = max(nMaxLen, m_pData[dw].GetText().GetLength());
		}
		nMaxLen = min(nMaxLen, 255);
		
		// allocate memory for sorting
		DWORD dwRecLen = nMaxLen + sizeof(DWORD) + sizeof(DWORD) + 1;
		DWORD dwRecCount = m_dwCount;
		DWORD dwBufferSize = dwRecLen * dwRecCount;
		char * pBuffer = new char [dwBufferSize];
		memset (pBuffer, 0, dwBufferSize);
		char * p = pBuffer;
		
		for (DWORD dw = 0 ; dw < m_dwCount ; dw++)
		{
			CString strSource = (m_pData)[dw].GetText();
			char* pSrcData = strSource.GetBuffer(0);
			strncpy (p, pSrcData, nMaxLen);
			DWORD dwID = (m_pData)[dw].GetID();
			DWORD dwData = (m_pData)[dw].GetData();
			memcpy ((LPVOID)(p + nMaxLen + 1), (LPVOID)(LPSTR)&dwID, 4);
			memcpy ((LPVOID)(p + nMaxLen + 5), (LPVOID)(LPSTR)&dwData, 4);
			p += dwRecLen;
		}

		// now sort it
		qsort (pBuffer, m_dwCount, dwRecLen, &compare);

		// finally repopulate in sorted order
		Empty();
		p = pBuffer;
		for (DWORD dw = 0 ; dw < dwRecCount ; dw++)
		{
			DWORD dwID ,dwData;
			memcpy (&dwID, (LPVOID)(p + nMaxLen + 1), sizeof(DWORD));
			memcpy (&dwData, (LPVOID)(p + nMaxLen + 5), sizeof(DWORD));
			Add(CSortItem(p, dwID ,dwData));
			p += dwRecLen;
		}
		m_bSorted = TRUE;
		delete [] pBuffer;
	}
	static int __cdecl compare (const void * p1, const void * p2)
	{
		return _stricmp ((LPCSTR)p1, (LPCSTR)p2);
	}
protected:
	BOOL	m_bSorted;
};


#endif