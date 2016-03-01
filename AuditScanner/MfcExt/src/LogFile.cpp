
// FILE:	LogFile.cpp
// PURPOSE:	Implementation of easy-to-use application log
// AUTHOR:	JRF Thornley - Copyright (C) Pmd Technology Services Ltd 1999
// HISTORY:	JRFT - 28.04.1999 - Created

// NOTES:	The header file must be included in stdafx.h
#include "stdafx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

// Initialise statics
BOOL CLogFile::m_nFlags = 0;
BOOL CLogFile::m_bOpen = FALSE;
CString CLogFile::m_strFileName;
CStdioFile CLogFile::m_file;
int CLogFile::m_nUsageCount = 0;

/*
** Constructor
*/
CLogFile::CLogFile (int nFlags/* = LOG_LOC_APP|LOG_START|LOG_FINISH*/)
{
	// Is this the very first instance ?
	if (0 == m_nUsageCount)
		m_nFlags = nFlags;
	m_nUsageCount++;
}

/*
** destructor
*/
CLogFile::~CLogFile ()
{
	// if this is the first instance being destroyed then optionally log application shutdown
	m_nUsageCount--;
	if (m_bOpen && (m_nUsageCount == 0))
	{
		// log shutdown time if required
		if (m_nFlags & LOG_FINISH)
		{
			CTime tmNow = CTime::GetCurrentTime();
			CString strBuffer;
			strBuffer.Format ("-- SESSION STOPPED %2.2d.%2.2d.%4.4d - %2.2d:%2.2d:%2.2d --\n", 
				tmNow.GetDay(), tmNow.GetMonth(), tmNow.GetYear(), tmNow.GetHour(), tmNow.GetMinute(), tmNow.GetSecond());
//			Write (strBuffer);
		}
//		m_file.Close();
		m_bOpen = FALSE;
	}
}

BOOL CLogFile::Open (LPCSTR pszFileName/* = NULL*/)
{
	ASSERT(!m_bOpen);

	if (NULL == pszFileName)
	{
		// no file specified
		char szBuffer[_MAX_PATH], szDrive[_MAX_DRIVE], szDir[_MAX_DIR], szName[_MAX_FNAME];
		::GetModuleFileName (::GetModuleHandle(NULL), szBuffer, sizeof(szBuffer));
		_splitpath (szBuffer, szDrive, szDir, szName, NULL);

		// look at stored flags to decide on naming convention
		if (m_nFlags & LOG_LOC_APP)
		{
			// put it next to the application
			m_strFileName.Format("%s%s%s.LOG", szDrive, szDir, szName);
		}
		else if (m_nFlags & LOG_LOC_TEMP)
		{
			// put it in Windows temporary path
			char szTempPath[_MAX_PATH];
			GetTempPath (sizeof(szTempPath), szTempPath);
			m_strFileName.Format("%s%s.LOG", szTempPath, szName);
		}
	}
	else
	{
		m_strFileName = pszFileName;
	}

	// try and open the file in append mode
	if (m_file.Open(m_strFileName, CFile::modeCreate | CFile::modeReadWrite | CFile::modeNoTruncate | CFile::shareDenyNone))
	{
		m_bOpen = TRUE;
		try
		{
			m_file.SeekToEnd();
		} 
		catch (CFileException * pE)
		{
			pE->Delete ();
		}

		// write the startup time if required
		if (m_nFlags & LOG_START) 
		{
			CTime tmNow = CTime::GetCurrentTime();
			CString strBuffer;
			strBuffer.Format ("-- SESSION STARTED %2.2d.%2.2d.%4.4d - %2.2d:%2.2d:%2.2d --", tmNow.GetDay(), tmNow.GetMonth(), tmNow.GetYear(), tmNow.GetHour(), tmNow.GetMinute(), tmNow.GetSecond());
			Write (strBuffer);
		}
		return TRUE;
	}
	else
	{
		return FALSE;
	}	
}

/*
** Write supplied string to file if open, optionally adding a newline and optionally flushing immediately
*/
void CLogFile::Write (LPCSTR pszText, BOOL bNewLine/*= TRUE*/, BOOL bFlush/* = TRUE*/)
{
#ifdef _DEBUG
	// TRACE can only handle 512 chars, so ensure that's all we send
	char * pTraceBuffer = new char[512];
	memset(pTraceBuffer, 0, 512);
	strncpy(pTraceBuffer, pszText, 511);
	TRACE("%s", pTraceBuffer);
	if (bNewLine)
		TRACE("\n");
	delete [] pTraceBuffer;
#endif

	if (m_bOpen)
	{
		CString strText(pszText);

		// add the newline if required
		if (bNewLine)
			strText += "\n";

		// dump to file
		try 
		{
			m_file.WriteString (strText);
			if (bFlush)
				m_file.Flush();
		} 
		catch (CFileException * pE) 
		{
			TRACE ("Error writing to Log File %s\n", m_strFileName);
			pE->Delete();
		}
	}
}

void CLogFile::Format (LPCSTR pszFormat, ... )
{
	char szBuffer[1024];
	va_list	argList;
	va_start (argList, pszFormat);
	vsprintf (szBuffer, pszFormat, argList);
	Write (szBuffer, FALSE, TRUE);
}	

void CLogFile::Flush ()
{
	if (m_bOpen)
		m_file.Flush();
}


//
//    Read the contents of the file into an internal list
//
int	CLogFile::Read	(CDynaList<CString>& list)
{
	// Rewind to the begining of the file
	m_file.Flush();
	m_file.SeekToBegin();

	// ...and read to the end
	CString strLogLine;
	while (m_file.ReadString(strLogLine))
	{
		list.Add(strLogLine);
	}

	return 0;
}
