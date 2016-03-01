
// FILE:	IniFile.cpp
// PURPOSE:	Implementation of an INI file wrapper for 32 bit MFC Applications
// AUTHOR:	JRF Thornley - Copyright (C) PMD Technology Services Ltd 1999
// HISTORY:	JRFT - 25.03.1999 - created

#include "stdafx.h"			// ensure "IniFile.h" is included in stdafx.h

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

int CIniItem::Write (CStdioFile & file) const
{
	int nRetCode = CIniFile::errorOK;
	CString strBuffer;
	strBuffer.Format("%s=%s\n", m_strKey, m_strValue);
	try 
	{
		file.WriteString (strBuffer);
	}
	catch (CFileException * pE)
	{
		nRetCode = pE->m_cause;
		pE->Delete();
	}
	return nRetCode;
}

/*
** Read a value from an INI file section
*/
CString CIniSection::GetKey (LPCSTR pszKey, LPCSTR pszDefault) const
{
	// find the key...
	DWORD dwKey = FindKey(pszKey);
	if ((DWORD)-1 == dwKey)
	{
		return CString(pszDefault);
	}
	else
	{
		return m_pData[dwKey].GetValue();
	}
}

/*
** Set a value in an INI file section
*/
void CIniSection::SetKey(LPCSTR pszKey, LPCSTR pszValue)
{
	// strip out illegal chars
	CString strKey(pszKey);
	strKey.Remove ('[');
	strKey.Remove (']');
	CString strVal(pszValue);
	strVal.Remove ('[');
	strVal.Remove (']');

	DWORD dwKey = FindKey(strKey);
	// not found ? add it...
	if (NOT_FOUND == dwKey)
	{
		Add (CIniItem(strKey, strVal));
	}
	else
	{
		m_pData[dwKey].SetValue(strVal);
	}
}

/*
** protected member to find a key within the section
*/
DWORD CIniSection::FindKey (LPCSTR pszKey) const
{
	for (DWORD dwKey = 0 ; dwKey < m_dwCount ; dwKey++)
		if (m_pData[dwKey].GetKey() == pszKey)
			return dwKey;
	return NOT_FOUND;
}

int CIniSection::Write (CStdioFile & file) const
{
	int nRetCode = CIniFile::errorOK;
	// write the section heading
	CString strBuffer;
	strBuffer.Format ("[%s]\n", m_strName);
	try
	{
		file.WriteString(strBuffer);
	}
	catch (CFileException * pE)
	{
		nRetCode = pE->m_cause;
		pE->Delete();
		return nRetCode;
	}
	// write the keys
	for (DWORD dwKey = 0 ; (nRetCode == CIniFile::errorOK) && (dwKey < m_dwCount) ; dwKey++)
		nRetCode = m_pData[dwKey].Write(file);
	return nRetCode;
}

#pragma warning (disable:4706) // assignment within conditional expression
int CIniSection::EnumKeys (LPSTR pszBuffer, int /*nMaxLen*/) const
{
	LPSTR pDest = pszBuffer;
	for (DWORD dwKey = 0 ; dwKey < m_dwCount ; dwKey++)
	{
		CString strKey = m_pData[dwKey].GetKey();
		LPCSTR pSrc = strKey;
		while (*pDest++ = *pSrc++)
			;
	}
	*pDest++ = '\0';
	return (pDest - pszBuffer);
}
#pragma warning (default:4706) // assignment within conditional expression

int CIniSection::EnumKeys (CDynaList<CString> & list) const
{
	for (DWORD dwKey = 0 ; dwKey < m_dwCount ; dwKey++)
		list.Add(m_pData[dwKey].GetKey());
	return list.GetCount();
}

void CIniSection::RemoveKey (LPCSTR pszKey)
{
	// find the key if it exists
	DWORD dwKey;
	for (dwKey = 0 ; dwKey < m_dwCount ; dwKey++)
	{
		if (m_pData[dwKey].GetKey() == pszKey)
			break;
	}
	if (dwKey < m_dwCount)
		Remove (dwKey);
}

///////////////////////////////////////////////////////////////////////////////

/*
** One and only consstructor - uses program base name if file name is null
*/
CIniFile::CIniFile (LPCSTR pszName/* = NULL*/, DWORD dwFlags/* = 0*/) : m_dwFlags(dwFlags)
{
	if (pszName)
	{
		m_strFileName = pszName;
	}
	else
	{
		// derive file name using base name of executable file + INI
		char szBuffer[_MAX_PATH], szDrive[_MAX_DRIVE], szDir[_MAX_DIR], szName[_MAX_FNAME];
		HINSTANCE hInst = ::GetModuleHandle(NULL);
		::GetModuleFileName(hInst, szBuffer, sizeof(szBuffer));
		_splitpath (szBuffer, szDrive, szDir, szName, NULL);
		m_strFileName.Format ("%s%s%s.INI", szDrive, szDir, szName);
	}
	// error reading is acceptable - file may not exist yet...
	if (0 == (m_dwFlags & IF_NOREAD))
		Read();
}

/*
** Destructor always flushes file to disk
*/
CIniFile::~CIniFile ()
{
	if (0 == (m_dwFlags & IF_NOWRITE))
		Write();
}

/*
** Return list of section names
*/
int CIniFile::EnumSections (CDynaList<CString> & list) const
{
	list.Empty();
	for (DWORD dwSec = 0 ; dwSec < m_dwCount ; dwSec++)
		list.Add(m_pData[dwSec].GetName());
	return list.GetCount();
}

/*
** Empty a complete section
*/
void CIniFile::RemoveSection (LPCSTR pszSection, BOOL bFlushNow/* = FALSE*/)
{
	DWORD dwSection = FindSection(pszSection);
	if ((DWORD)-1 != dwSection)
	{
		Remove (dwSection);
		m_dwChanges++;
	}
	if (bFlushNow)
		Write();
}

void CIniFile::RemoveAll (BOOL bFlushNow/* = FALSE*/)
{
	Empty();
	m_dwChanges++;
	m_dwSection = NOT_FOUND;
	if (bFlushNow)
		Write();
}

/*
** Set current section to the title of the specified window
*/
void CIniFile::SetSection (CWnd * pWnd)
{
	ASSERT (pWnd->GetSafeHwnd());
	CString strSection;
	pWnd->GetWindowText(strSection);
	SetSection(strSection);
}

/*
** return a list of key names
*/
int CIniFile::EnumKeys (LPCSTR pszSection, CDynaList<CString> & list) const
{
	list.Empty();
	DWORD dwSection = FindSection(pszSection);
	if ((DWORD)-1 != dwSection)
		return m_pData[dwSection].EnumKeys(list);
	else
		return 0;
}

void CIniFile::RemoveKey (LPCSTR pszEntry, BOOL bFlushNow/* = FALSE*/)
{
	// find the section
	if (NOT_FOUND != m_dwSection)
	{
		m_pData[m_dwSection].RemoveKey(pszEntry);
		m_dwChanges++;
	}
	if (bFlushNow)
		Write();
}

/*
** Retrieve a string
*/
CString CIniFile::GetString (LPCSTR pszKey, LPCSTR pszDefault/* = NULL*/) const
{
	CString strResult(pszDefault);

	// find the section if it exists
	if (NOT_FOUND != m_dwSection)
	{
		strResult = m_pData[m_dwSection].GetKey(pszKey, pszDefault);
	}
	return strResult;
}

/*
** Retrieve an integer
*/
int CIniFile::GetInt (LPCSTR pszKey, int nDefault/* = 0*/) const
{
	CString strDefault;
	strDefault.Format("%d", nDefault);
	CString strResult = GetString(pszKey, strDefault);
	return atoi(strResult);
}

/*
** Retrieve a Boolean
*/
BOOL CIniFile::GetBool (LPCSTR lpszEntry, BOOL bDefault/* = 0*/) const
{
	char szDefault[2];
	if (bDefault)
		strcpy (szDefault, "Y");
	else
		strcpy (szDefault, "N");
	CString string = GetString (lpszEntry, szDefault);
	if ('Y' == string[0] || 'y' == string[0] || 'T' == string[0] || 't' == string[0] || '1' == string[0])
		return TRUE;
	return FALSE;
}

/*
** Retrieve a DWORD, stored as a hex string
*/
DWORD CIniFile::GetHex (LPCSTR pszSection, LPCSTR pszKey, DWORD dwDefault/* = 0*/) const
{
	CString strBuffer;
	strBuffer.Format("%X", dwDefault);
	strBuffer = GetString (pszSection, pszKey, strBuffer);
	DWORD dwResult;
	sscanf (strBuffer, "%X", &dwResult);
	return dwResult;
}

/*
** Retrieve  a DWORD, stored as a hex string
*/
DWORD CIniFile::GetHex (LPCSTR pszKey, DWORD dwDefault/* = 0*/) const
{
	CString strBuffer;
	strBuffer.Format("%X", dwDefault);
	strBuffer = GetString (pszKey, strBuffer);
	DWORD dwResult;
	sscanf (strBuffer, "%X", &dwResult);
	return dwResult;
}

/*
** Retrieve a date/time stored in "yyyy-mm-dd hh:mm:ss" format
*/
CTime CIniFile::GetTime (LPCSTR pszKey, CTime timeDefault/* = CTime::GetCurrentTime()*/ ,BOOL bTimeOnly/*=FALSE*/) const
{
	CTime result = timeDefault;
	CTime ctNow = CTime::GetCurrentTime();

	CString strBuffer = GetString (pszKey);
	if (!strBuffer.IsEmpty())
	{
		LPCSTR p = (LPCSTR)strBuffer;
		int nYear(0) ,nMonth(0) ,nDay(0) ,nHour(0) ,nMinute(0) ,nSecond(0);

		if (!bTimeOnly)
		{
			nYear = atoi(p);
			p+=5;					// Step past year
			nMonth = atoi(p);
			p+=3;					// Step past month
			nDay = atoi(p);
			p+=3;					// Step past day				
		}

		else
		{
			nYear = ctNow.GetYear();
			nMonth = ctNow.GetMonth();
			nDay = ctNow.GetDay();
		}

		nHour = atoi(p);
		p+=3;						// Step past hour
		nMinute = atoi(p);
		p+=3;						// Step past minute
		nSecond = atoi(p);
		result = CTime(nYear, nMonth, nDay, nHour, nMinute, nSecond);
	}
	return result;
}

/*
** Retrieve a stored window position
*/
BOOL CIniFile::GetWindowPos (CWnd * pWnd, LPCSTR pszKey /* = NULL*/) const
{
	CString strKey = (pszKey != NULL) ? pszKey : "Position";
	
	// read as a string
	CString strBuffer = GetString(strKey);
	if (strBuffer.GetLength())
	{
		char * pBuffer = new char[strBuffer.GetLength() + 2];
		memset (pBuffer, 0, strBuffer.GetLength() + 2);
		for (int n = 0 ; n < strBuffer.GetLength() ; n++)
		{
			char ch = strBuffer[n];
			if (ch != ';')
				pBuffer[n] = ch;
		}
		LPCSTR p = pBuffer;
		int x = atoi(p);
		p += strlen(p) + 1;
		int y = atoi(p);
		p += strlen(p) + 1;
		int cx = atoi(p);
		p += strlen(p) + 1;
		int cy = atoi(p);
		// reposition window accordingly
		pWnd->MoveWindow (x, y, cx, cy, TRUE);
		delete [] pBuffer;
		return TRUE;
	}
	else
		return FALSE;
}

/*
** Store a string
*/
void CIniFile::WriteString (LPCSTR pszKey, LPCSTR pszValue, BOOL bFlushNow/* = FALSE*/)
{
	ASSERT (NOT_FOUND != m_dwSection);
	m_pData[m_dwSection].SetKey(pszKey, pszValue);
	m_dwChanges++;
	if (bFlushNow)
		Write();
}
/*
** Store an integer
*/
void CIniFile::WriteInt (LPCSTR pszKey, int nValue, BOOL bFlushNow/* = FALSE*/)
{
	char szBuffer[256];
	wsprintf (szBuffer, "%d", nValue);
	WriteString (pszKey, szBuffer, bFlushNow);
}
/*
** Store a Boolean
*/
void CIniFile::WriteBool (LPCSTR pszKey, BOOL bValue, BOOL bFlushNow/* = FALSE*/)
{
	char szBuffer[256];
	strcpy (szBuffer, bValue ? "Y" : "N");
	WriteString (pszKey, szBuffer, bFlushNow);
}

/*
** Store an integer, written as a hex string
*/
void CIniFile::WriteHex (LPCSTR pszSection, LPCSTR pszKey, DWORD dwValue, BOOL bFlushNow/* = FALSE*/)
{
	CString strBuffer;
	strBuffer.Format("%X", dwValue);
	WriteString (pszSection, pszKey, strBuffer, bFlushNow);
}
/*
** Store an integer, written as a hex string
*/
void CIniFile::WriteHex (LPCSTR pszKey, DWORD dwValue, BOOL bFlushNow/* = FALSE*/)
{
	char szBuffer[256];
	wsprintf (szBuffer, "%X", dwValue);
	WriteString (pszKey, szBuffer, bFlushNow);
}

/*
** Store a Window's current position
*/
void CIniFile::WriteWindowPos (CWnd * pWnd, LPCSTR pszKey /*= NULL*/)
{
	CString strKey = (pszKey != NULL) ? pszKey : "Position";

	// store window position in a single string
	WINDOWPLACEMENT wp;
	pWnd->GetWindowPlacement (&wp);
	CString strBuffer;
	strBuffer.Format ("%d;%d;%d;%d",	wp.rcNormalPosition.left,
										wp.rcNormalPosition.top,
										wp.rcNormalPosition.right - wp.rcNormalPosition.left,
										wp.rcNormalPosition.bottom - wp.rcNormalPosition.top);
	WriteString (strKey, strBuffer);
}

int CIniFile::Read (LPCSTR pszFileName/* = NULL*/)
{
	// check that reading is permitted
	ASSERT(0 == (IF_NOREAD & m_dwFlags));

	// update filename if one was specified
	if (NULL != pszFileName)
		m_strFileName = pszFileName;

	int nResult = errorBadPath;

	// try and open the file
	CStdioFile file;
	if (file.Open(m_strFileName, CFile::modeRead | CFile::shareDenyNone))
	{
		// file opened ok - clear any existing data...
		Empty();
		CString strBuffer;
		while (file.ReadString(strBuffer))
		{
			strBuffer.TrimRight();
			if (strBuffer.IsEmpty())
				continue;
			// is it the start of a section ?
			switch (strBuffer[0])
			{
				case '[':
					{
						// extract the name
						int n = strBuffer.Find(']');
						CString strName = strBuffer.Mid(1, (n - 1));
						m_dwSection = AddSection(strName);
					}
					break;

				case '\r':
				case '\n':
					// blank lines
					break;

				default:
					{
						// assume all others are keys - take off the carriage return first...
						if (strBuffer[strBuffer.GetLength() - 1] == '\n')
							strBuffer = strBuffer.Left(strBuffer.GetLength() - 1);
						// split the string at the equals sign
						int nEquals = strBuffer.Find('=');

						// Only add the line if we have an equals in it - all others are
						// invalid lines
						if (nEquals != -1)
						{
							CString strKey = strBuffer.Left(nEquals);
							CString strValue = strBuffer.Mid(nEquals + 1);
							WriteString (strKey, strValue);
						}
					}
					break;
			}
		}
		nResult = errorOK;
	}
	m_dwChanges = 0;
	return nResult;
}
					
int CIniFile::Write () const
{
	// ensure that writing is permitted
	ASSERT(0 == (IF_NOWRITE & m_dwFlags));

	int nRetCode = errorOK;

	// try and create a file to write to
	try
	{
		CStdioFile file (m_strFileName, CFile::modeCreate | CFile::modeWrite);
		// run through the ini file sections
		for (DWORD dwSection = 0 ; (nRetCode == CIniFile::errorOK) && (dwSection < m_dwCount) ; dwSection++)
		{
			// if not first one then write a blank line
			if (dwSection)
			{
				CString strBuffer("\n");
				file.WriteString(strBuffer);
			}
			nRetCode = m_pData[dwSection].Write (file);
		}

		// reset change count (cheat the const operator)
		((CIniFile*)this)->m_dwChanges = 0;
	}
	catch (CFileException * pE)
	{
		// return the cause of the error
		nRetCode = pE->m_cause;
		pE->Delete();
	}
	return nRetCode;
}

/*
** Adds a new section to the INI file if it does not already exist
*/
DWORD CIniFile::AddSection (LPCSTR pszName)
{
	// see if it already exists
	for (DWORD dwSec = 0 ; dwSec < m_dwCount ; dwSec++)
	{
		if (m_pData[dwSec].GetName() == pszName)
			return dwSec;
	}
	CIniSection newSection(pszName);
	return Add(newSection);
}

/*
** Finds an existing section in the buffer
*/
DWORD CIniFile::FindSection (LPCSTR pszName) const
{
	for (DWORD dwSec = 0 ; dwSec < m_dwCount ; dwSec++)
	{
		if (m_pData[dwSec].GetName() == pszName)
			return dwSec;
	}
	return NOT_FOUND;
}
