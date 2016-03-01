#include "stdafx.h"

#include "productLicence.h"

// useful masks
#define MASK_ALL_PRODUCTS	0x00FF			// AWL_PRODUCT1 - AWL_PRODUCT8
#define MASK_ALL_OPTIONS	0xFF00			// AWL_OPTION1 - AWL_OPTION8
#define MASK_ALL_FLAGS		(MASK_ALL_PRODUCTS | MASK_ALL_OPTIONS | AWL_EVAL | AWL_SPARE)

// mask to extract the 14 bit CRC value
#define MASK_CRC		0xFFFC0000

#define XOR_KEY_1	0xA7FFFBC6
#define XOR_KEY_2	0x12BBA84F
#define XOR_KEY_3	0xC4F339D9
#define XOR_KEY_4	0x9B67DDA8
#define XOR_KEY_5	0x8C98AC75

IMPLEMENT_DYNAMIC(CProductLicense, CObject)

CProductLicense::CProductLicense ()
{
	m_dwFlags = 0;
	m_wCount = 10;
	m_dwDate = 0;
	m_dwCompanyID = 0;
}

CProductLicense::CProductLicense (DWORD dwFlags, WORD wCount, CTime ctDate, int nDuration, eDurationUnits units, DWORD dwCompanyID)
{
	m_dwFlags = dwFlags;
	m_wCount = wCount;
	StoreDate (ctDate, nDuration, units);
	m_dwCompanyID = dwCompanyID;
}

CProductLicense::~CProductLicense ()
{
}

CProductLicense const & CProductLicense::operator= (CProductLicense const & other)
{
	m_dwFlags	= other.m_dwFlags;
	m_wCount	= other.m_wCount;
	m_dwDate	= other.m_dwDate;
	m_dwCompanyID	= other.m_dwCompanyID;
	return *this;
}

CString CProductLicense::ToString () const
{
	CString strTemp, strResult;

	// ensure CRC is up to date before generating the key
	((CProductLicense *)this)->SetCRC();

	// apply the encryption
	((CProductLicense *)this)->Encrypt ();

	// translate into Base32 alphanumeric
	strTemp.Format ("%s%s%s%s",
		DwordToBase32(m_dwFlags),
		WordToBase32(m_wCount),
		DwordToBase32(m_dwDate),
		DwordToBase32(m_dwCompanyID));
	ASSERT(strTemp.GetLength() == 25);

	//split into 5 character blocks
	strResult.Format ("%5s-%5s-%5s-%5s-%5s", strTemp.Mid(0,5), strTemp.Mid(5,5), strTemp.Mid(10,5), strTemp.Mid(15,5), strTemp.Mid(20,5));

	// remove the encryption again
	((CProductLicense *)this)->Encrypt ();

	return strResult;
}

BOOL CProductLicense::FromString (LPCSTR pszString)
{
	// string must be 5 x 5 chars long with 4 separators
	if (!pszString || strlen(pszString) != 29)
		return FALSE;
	
	// remove the separators
	CString strBuffer(pszString);
	strBuffer.Remove ('-');
	// note the unequal way it is split...
	CString strFlags = strBuffer.Mid(0,7);
	CString strCount = strBuffer.Mid(7,4);
	CString strDate = strBuffer.Mid(11,7);
	CString strCompany = strBuffer.Mid(18,7);
	
	// unpack each one
	DWORD dwFlags = Base32ToDword (strFlags);
	WORD wCount = Base32ToWord (strCount);;
	DWORD dwDate = Base32ToDword (strDate);
	DWORD dwCompany = Base32ToDword (strCompany);

	// store them in a temporary object
	CProductLicense licTemp;
	licTemp.m_dwFlags = dwFlags;
	licTemp.m_wCount = wCount;
	licTemp.m_dwDate = dwDate;
	licTemp.m_dwCompanyID = dwCompany;
	
	// ...and reset the encryption
	licTemp.Encrypt ();	

	// check the CRC
	if (licTemp.CalculateCRC() != licTemp.GetCRC())
		return FALSE;
	
	// it's ok so store it
	*this = licTemp;
	return TRUE;
}

void CProductLicense::Serialize (CArchive & ar)
{
	if (ar.IsStoring())
	{
		// always ensure the CRC is correct before writing
		SetCRC ();
		// encrypt the values
		Encrypt();
		ar << m_dwFlags;
		ar << m_wCount;
		ar << m_dwDate;
		ar << m_dwCompanyID;
	}
	else
	{
		ar >> m_dwFlags;
		ar >> m_wCount;
		ar >> m_dwDate;
		ar >> m_dwCompanyID;
		// reverse the encryption
		Encrypt();
		// check the CRC
		if (CalculateCRC() != GetCRC())
			// << Throw an exception >>
			ASSERT(FALSE);
	}
}


DWORD CProductLicense::CalculateCRC () const
{
	// we want a 14 bit CRC value to fill the available space

	// start with a simple sum of all the 1 bits in the data words
	DWORD dwResult = CountBits (m_dwFlags & MASK_ALL_FLAGS)
		+ CountBits (m_wCount)
		+ CountBits (m_dwDate)
		+ CountBits (m_dwCompanyID);
	// That has a value of 0 < val < 146 so occupies 8 bits
	// Fill the remaining 6 by repeating the low 2 bits of the count, flags & Company ID
	dwResult <<= 2;
	dwResult |= (m_wCount & 0x03);
	dwResult <<= 2;
	dwResult |= (m_dwFlags & 0x03);
	dwResult <<= 2;
	dwResult |= (m_dwCompanyID & 0x03);

	return dwResult;
}

DWORD CProductLicense::GetCRC () const
{
	// CRC is in the top 14 bits of the "FLAGS" dword
	DWORD dwCRC = m_dwFlags >> 18;
	return dwCRC;
}

void CProductLicense::SetCRC ()
{
	// calculate the new CRC value
	DWORD dwCRC = CalculateCRC();
	// remove the old one from the flags setting
	DWORD dwMask = MASK_CRC;
	m_dwFlags &= ~dwMask;
	// shift the new one up and store it
	dwCRC <<= 18;
	m_dwFlags |= dwCRC;
}

/*
** Product Flag(s)
*/
BOOL CProductLicense::IsProductSet (DWORD dwProductFlag) const
{
	ASSERT(dwProductFlag == (dwProductFlag & MASK_ALL_PRODUCTS));
	// this may be more than one flag, so take care
	return (dwProductFlag == (m_dwFlags & dwProductFlag));
}
void CProductLicense::SetProduct (DWORD dwProductFlag, BOOL bSet/* = TRUE*/)
{
	ASSERT(dwProductFlag == (dwProductFlag & MASK_ALL_PRODUCTS));
	if (bSet)
		m_dwFlags |= dwProductFlag;
	else
		m_dwFlags &= ~dwProductFlag;
}

/*
** Option Flags
*/
BOOL CProductLicense::IsOptionSet (DWORD dwOptionFlag) const
{
	ASSERT(dwOptionFlag == (dwOptionFlag & MASK_ALL_OPTIONS));
	return (dwOptionFlag == (m_dwFlags & dwOptionFlag));
}
void CProductLicense::SetOption (DWORD dwOptionFlag, BOOL bSet/* = TRUE*/)
{
	ASSERT(dwOptionFlag == (dwOptionFlag & MASK_ALL_OPTIONS));
	if (bSet)
		m_dwFlags |= dwOptionFlag;
	else
		m_dwFlags &= ~dwOptionFlag;
}

CTime CProductLicense::GetStartDate () const
{
	CDosDate dd(HIWORD(m_dwDate), 0);
	return CTime(dd);
}

void CProductLicense::SetStartDate (CTime ctStart)
{
	CDosDate dd(ctStart);
	m_dwDate = MAKELONG(LOWORD(m_dwDate), HIWORD((DWORD)dd));
}

WORD CProductLicense::GetDuration () const
{
	WORD wResult = LOWORD(m_dwDate);
	wResult &= 0x3FFF;

	return wResult;
}

void CProductLicense::SetDuration (WORD wDuration)
{
	WORD wMask14 = 0x3FFF;
	// low 14 bits of m_dwDate
	ASSERT (wDuration == (wDuration & wMask14));
	// pack it together with the existing units
	wDuration |= LOWORD(m_dwDate) & ~wMask14;
	// and store it
	m_dwDate = MAKELONG(wDuration, HIWORD(m_dwDate));
}

void CProductLicense::SetDurationUnits (eDurationUnits n)
{
	WORD wUnits = (WORD)n;
	wUnits <<= 14;
	// clear existing ones
	WORD wLoWord = GetDuration();
	wLoWord |= wUnits;
	m_dwDate = MAKELONG(wLoWord, HIWORD(m_dwDate));
}

/*
** Return expiry Date
*/
CTime CProductLicense::GetExpiryDate () const
{
	// Find the start date, duration and units
	CDosDate dd(HIWORD(m_dwDate), 0);
	eDurationUnits units = GetDurationUnits ();
	int nDuration = GetDuration ();

	int day = dd.GetDay();
	int month = dd.GetMonth();
	int year = dd.GetYear() + 1980;

	switch (units)
	{
		case days:
			day += nDuration;
			break;
		case weeks:
			day += (nDuration * 7);
			break;
		case months:
			month += nDuration;
			break;
		case years:
			year += nDuration;
			break;
	}
	// tidy up
	while (day > DaysInMonth(month, year))
	{
		day -= DaysInMonth(month, year);
		month++;
		// this might bugger the months up too...
		while (month > 12)
		{
			year++;
			month -= 12;
		}
	}
	while (month > 12)
	{
		year++;
		month -= 12;
	}

	return CTime (year, month, day, 0, 0, 0);
}

/*
** Return remaining time of eval / support
*/
int CProductLicense::GetRemainingDays () const
{
	CTime ctExpiry = GetExpiryDate();
	CTime ctCurrent = CTime::GetCurrentTime ();

	if (ctExpiry > ctCurrent)
		return 0;
	CTimeSpan tsRemaining = ctExpiry - ctCurrent;
	// round it to nearest day
	DWORD dwSeconds = tsRemaining.GetTotalSeconds ();
	int nDays = (int)((dwSeconds + 43200) / 86400);

	return nDays;
}

/*
** Company Identifier key
*/
DWORD CProductLicense::GetCompanyID () const
{
	return m_dwCompanyID;
}

void CProductLicense::SetCompanyID (DWORD dwCompanyID)
{
	m_dwCompanyID = dwCompanyID;
}
	
/*
** Return status of a flag
*/
BOOL CProductLicense::IsFlagSet (DWORD dwFlag) const
{
	return ((m_dwFlags & dwFlag) != 0);
}

/*
** Set a flag
*/
void CProductLicense::SetFlag (DWORD dwFlag, BOOL bSet/* = TRUE*/)
{
	if (bSet)
		m_dwFlags |= dwFlag;
	else
		m_dwFlags &= ~dwFlag;
}

/*
** Converts a 32 bit DWORD into a Base 32 number
** output is a 7 digit alphanumeric
*/
CString CProductLicense::DwordToBase32 (DWORD dwValue) const
{
	CString strResult;

	// a 32 bit value converts into 7 alphanumeric characters
	for (int n = 0 ; n < 7 ; n++)
	{
		// pull off 5 bits
		BYTE nThisBit = (BYTE)(dwValue & 0x1F);
		dwValue >>= 5;
		// convert resulting value to alphanumeric
		strResult = IntToBase32(nThisBit) + strResult;
	}
	return strResult;
}

CString CProductLicense::WordToBase32 (WORD wValue) const
{
	CString strResult;

	// a 16 bit value converts into 4 alphanumeric characters
	for (int n = 0 ; n < 4 ; n++)
	{
		// split off 5 bits at a time
		BYTE nThisBit = (BYTE)(wValue & 0x1F);
		wValue >>= 5;		
		strResult = IntToBase32(nThisBit) + strResult;
	}
	return strResult;
}

DWORD CProductLicense::Base32ToDword (LPCSTR pszString) const
{
	DWORD dwResult = 0;
	ASSERT(pszString && (strlen(pszString) >= 7));
	for (int n = 0 ; n < 7 ; n++)
	{
		// make room with what we already have
		dwResult <<= 5;
		BYTE nThisBit = Base32ToInt(pszString[n]);
		dwResult |= nThisBit;
	}
	return dwResult;
}

WORD CProductLicense::Base32ToWord (LPCSTR pszString) const
{
	WORD wResult = 0;
	ASSERT(pszString && (strlen(pszString) >= 4));
	for (int n = 0 ; n < 4 ; n++)
	{
		wResult <<= 5;
		BYTE nThisBit = Base32ToInt(pszString[n]);
		wResult |= nThisBit;
	}
	return wResult;
}

char CProductLicense::IntToBase32 (BYTE nValue) const
{
	ASSERT(nValue < 32);
	// ignore 0 & 1, so we start at 2,3,4,5,6,7,8,9
	char chResult = (char)(nValue + 50);
	// after that we jump to 'A'
	if (chResult > 57)
		chResult += 7;
	// avoid using 'I'
	if (chResult > 72)
		chResult++;
	// avoid using 'O'
	if (chResult > 78)
		chResult++;
	// so by rights it should never go past Z
	ASSERT(chResult <= 'Z');
	return chResult;
}

BYTE CProductLicense::Base32ToInt (char ch) const
{
	// reverse the above procedure
	ASSERT(ch <= 'Z');
	if (ch > 'O')
		ch--;
	if (ch > 'I')
		ch--;
	if (ch >= 'A')
		ch -= 7;
	BYTE bResult = (BYTE)(ch - 50);
	ASSERT(bResult < 32);
	return bResult;
}

DWORD CProductLicense::CountBits (DWORD dwValue) const
{
	DWORD dwResult = 0;
	for (int n = 0 ; n < 32 ; n++)
	{
		dwResult += (dwValue & 0x01);
		dwValue >>= 1;
	}
	return dwResult;
}

/*
** Note that running this a second time reverses the operation
*/
void CProductLicense::Encrypt ()
{
	m_dwFlags ^= XOR_KEY_1;
	m_wCount ^= XOR_KEY_2;
	m_dwDate ^= XOR_KEY_3;
	m_dwCompanyID ^= XOR_KEY_5;
}

void CProductLicense::StoreDate (CTime ctDate, int nDuration, eDurationUnits units)
{
	// upper WORD is the date, stored in DosDate format
	CDosDate dd(ctDate);
	m_dwDate = (DWORD)dd;
	// zero out the low word (ie the time part)
	m_dwDate &= 0xFFFF0000;
	// use the upper two bits of the low word for the units
	int nUnits = (int)units;
	ASSERT(nUnits >= 0 && nUnits < 4);
	m_dwDate |= nUnits << 14;
	// low 14 bits are the duration
	ASSERT(nDuration >= 0 && nDuration < 0x3FFF);
	m_dwDate |= nDuration;
}

///////////////////////////////////////////////////////////////////////////////
//	CLaytonLicenceList

BOOL CLaytonLicenceList::IsEval (DWORD dwProductCode)
{
	for (DWORD dw = 0 ; dw < GetCount() ; dw++)
	{
		if (m_pData[dw].IsProductSet(dwProductCode) && !m_pData[dwProductCode].IsEval())
		{
			// found a valid full licence, so stop looking
			return FALSE;
		}
	}
	return TRUE;
}

BOOL CLaytonLicenceList::CanRunProduct (DWORD dwProductCode)
{
	for (DWORD dw = 0 ; dw < GetCount() ; dw++)
	{
		CProductLicense & licence = m_pData[dw];
		// does the licence apply to this product?
		if (licence.IsProductSet(dwProductCode))
		{
			// yes - must be a full licence or an in-date eval
			if (!licence.IsEval() || (licence.GetRemainingDays()))
				return TRUE;
		}
	}
	return FALSE;
}


