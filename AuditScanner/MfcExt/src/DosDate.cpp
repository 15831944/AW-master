
// FILE:	DosDate.cpp
// PURPOSE:	Implementation of wrapper class to handle MS-DOS datestamps
// AUTHOR:	JRF Thornley - 14.06.1999 - Copyright (C) PMD Technology Services Ltd 1999
// HISTORY:

#include "stdafx.h"

#define new DEBUG_NEW

/*
** Default constructor
*/
CDosDate::CDosDate ()
{
	m_dwStorage = 0;
}

/*
** Copy constructor
*/
CDosDate::CDosDate (DWORD dwOther)
{
	m_dwStorage = dwOther;
}

/*
** Initialise using a Win95 FILETIME, allowing for effects of BST under Windows 95
*/
CDosDate::CDosDate (FILETIME * pftOriginal)
{
	WORD		wDate, wTime;
	FILETIME	ftLocal;

	FileTimeToLocalFileTime (pftOriginal, &ftLocal);
	FileTimeToDosDateTime (&ftLocal, &wDate, &wTime);
	m_dwStorage = MAKELONG (wTime, wDate);
}

/*
** Construct from a CTime object
*/
#pragma warning(disable:4244)
CDosDate::CDosDate (CTime const & timDate)
{
	WORD wDate = 0, wTime = 0;
	if ( (DWORD)timDate.GetTime())
	{
		// first extract and repack the date
		wDate  = (timDate.GetYear() - 1980) << 9;
		wDate += timDate.GetMonth() << 5;
		wDate += timDate.GetDay();
		// then extract and repack the time
		wTime  = timDate.GetHour() << 11;
		wTime += timDate.GetHour() << 5;
		wTime += timDate.GetSecond() >> 1;
	}
	SetDate(wDate);
	SetTime(wTime);
}
#pragma warning(default:4244)

/*
** construction from separate date & time
*/
CDosDate::CDosDate(WORD wDate, WORD wTime)
{
	m_dwStorage = MAKELONG(wTime, wDate);
}

/*
** Construct using character strings specifying date and time
*/
CDosDate::CDosDate (LPCSTR pszDate, LPCSTR pszTime)
{
	SetDate(pszDate);
	SetTime(pszTime);
}

/*
** Set the date portion from a WORD
*/
void CDosDate::SetDate (WORD wDate)
{
	m_dwStorage = MAKELONG (LOWORD(m_dwStorage), wDate);
}

/*
** Set the date portion of storage from a string
*/
#pragma warning(disable:4244 4706)
void CDosDate::SetDate (LPCSTR pszDate)
{
	int		nItem = 0;		// flag says we are decoding DAY
	WORD	nValues[3] = { 0, 0, 0 };
	char	ch;
	// store empty string as blank date
	WORD wDate = 0;
	if (strlen(pszDate) != 0)
	{
		// look through the string
		while (ch = *pszDate++)
		{
			// is it a separator ?
			if ( (ch < '0') || ( ch > '9') )
			{
				// yes - move on to next field
				nItem++;
				if (nItem == 3)
					break;
			}
			else
			{
				// no - add this digit to the current value
				nValues[nItem] *= 10;
				nValues[nItem] += (ch - '0');
			}
		}
		// extract the values
		WORD wDay = nValues[0];
		WORD wMon = nValues[1];
		WORD wYear = nValues[2];
		// CHECK VALUES HERE
		// now correct the year if 2 digits
		if (wYear < 50)
			wYear += 2000;
		else if (wYear < 100)
			wYear += 1900;
		// store them
		wDate = ((wYear - 1980) << 9 ) + (wMon << 5) + wDay;
	}
	SetDate (wDate);
}
#pragma warning(default:4244 4706)

/*
** Set the time portion of storage from a WORD
*/
void CDosDate::SetTime (WORD wTime)
{
	m_dwStorage >>= 16;
	m_dwStorage <<= 16;
	m_dwStorage |= wTime;
}

/*
** Set the time portion of storage from a string
*/
#pragma warning(disable:4244)
void CDosDate::SetTime (LPCSTR pszTime)
{
	WORD wTime = 0;
	// time is bit-packed as hhhhhmmmmmmsssss
	if (8 <= strlen(pszTime))
	{
		int nHr  = (pszTime[0] - '0') * 10;
			nHr +=  pszTime[1] - '0';
		int nMin  = (pszTime[3] - '0') * 10;
			nMin +=  pszTime[4] - '0';
		int nSec  = (pszTime[6] - '0') * 10;
			nSec +=  pszTime[7] - '0';
		// pack into result (note seconds are halved)
		wTime  = (nHr << 11 ) + (nMin << 5) + (nSec >> 1);
	}
	m_dwStorage >>= 16;
	m_dwStorage <<= 16;
	m_dwStorage |= wTime;
}
#pragma warning(default:4244)
/*
** CTime cast
*/
CDosDate::operator CTime() const
{
	WORD wDate = HIWORD(m_dwStorage);
	WORD wTime = LOWORD(m_dwStorage);
	if (wDate)
	{
		return CTime (wDate, wTime);
/*		
		// first 5 bits are the day
		int nDay = wDate & 0x1F;
		wDate >>= 5;
		// next 4 bits are the month
		int nMonth = wDate & 0x0F;
		// remainder is year - 1980
		int nYear = (wDate >> 4) + 1980;

		// now decode the time
		WORD wTime = LOWORD(m_dwStorage);
		// lowest 5 bits are seconds / 2
		int nSecs = (wTime & 0x1F) << 1;
		wTime >>= 5;
		// next 6 bits are minutes
		int nMins = (wTime & 0x3F);
		// top 5 bits are hours
		int nHours = wTime >> 6;
		// return as a CTime object
		return CTime (nYear, nMonth, nDay, nHour, nMin, nSec, 0);*/
	}
	else
	{
		// special case of null date
		return CTime(0);
	}
}

/*
** Splits an MS-DOS word into constituent day, month & year
*/
CString CDosDate::GetDate() const
{
	WORD wDate = HIWORD(m_dwStorage);
	if (!wDate)
		return CString("");
	else
	{
		// first 5 bits are the day
		int nDay = ( wDate & 0x1F );
		wDate >>= 5;
		// next 4 bits are the month
		int nMonth = ( wDate & 0x0F );
		// remainder is year - 1980
		int nYear = ( wDate >> 4 ) + 1980;
		// format up according to locale setting
		return FormatDateIntl(nYear, nMonth, nDay);
	}
}

/*
** Returns component time formatted as a string
*/
CString CDosDate::GetTime() const
{
	WORD wTime = LOWORD(m_dwStorage);
	// note that seconds are divided by 2
	int nSecs = ( wTime & 0x1F ) << 1;
	wTime >>= 5;
	// extract minutes then shift right again for hours
	int nMins = ( wTime & 0x3F );
	int nHours = wTime >> 6;
	// format as hh:mm:ss
	CString strResult;
	strResult.Format ("%2.2d:%2.2d:%2.2d", nHours, nMins, nSecs);
	return strResult;
}

