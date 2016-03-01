
// FILE:	DosDate.cpp
// PURPOSE:	Declaration of wrapper class to handle MS-DOS datestamps
// AUTHOR:	JRF Thornley - 14.06.1999 - Copyright (C) PMD Technology Services Ltd 1999
// HISTORY:

#ifndef _DOSDATE_DEF_
#define _DOSDATE_DEF_

class CDosDate
{
public:
	CDosDate();									// default constructor
	CDosDate(DWORD dwOther);					// copy constructor
	CDosDate(FILETIME * pftOriginal);			// construction from a FILETIME structure
	CDosDate(CTime const & timOriginal);		// construction from a CTime object
	CDosDate(WORD wDate, WORD wTime);			// construction from separate date & time
	CDosDate(LPCSTR pszDate, LPCSTR pszTime);	// construction from separate date & time as strings
public:
	int GetDay ()								// day is bottom 5 bits of upper word	
		{ return (int)(HIWORD(m_dwStorage) & 0x1F); }	
	int GetMonth ()								// month is next 4 bits of upper word
		{ return (int)((HIWORD(m_dwStorage) >> 5) & 0xF); }
	int GetYear ()								// year is top 7 bits of upper word
		{ return (int)(HIWORD(m_dwStorage) >> 9); }
	void SetDate (WORD wDate);					// set date but leave time unaffected
	void SetDate (LPCSTR pszDate);				// as above but accept a string in format dd/mm/yyyy or dd/mm/yy
	void SetTime (WORD wTime);					// set time but leave date unaffected
	void SetTime (LPCSTR pszDate);				// as above but accept a string in format hh:mm:ss
	operator CTime() const;						// cast to a CString object
	operator DWORD() const						// cast to a DWORD
		{ return m_dwStorage; }
	CString GetDate () const;					// return date as a string using local format
	CString GetTime () const;					// return time as a string in format hh:mm:ss
private:
	DWORD	m_dwStorage;
};

#endif	// #ifndef _DOSDATE_DEF_

