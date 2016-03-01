
// FILE:	IPAddress.cpp
// PURPOSE:	Implementation of classes for storing and manipulating IP Addresses
// AUTHOR:	C.Drew - copyright (C) Layton Technology Ltd
// HISTORY:	CMD	- 01.07.2003 - written

#include "stdafx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIPAddress::CIPAddress ()
{
	memset (&m_nOctets, 0, sizeof(m_nOctets));
}

CIPAddress::CIPAddress (LPCSTR pszAddress)
{
	*this = pszAddress;
}

CIPAddress::CIPAddress (BYTE nOctet1, BYTE nOctet2, BYTE nOctet3, BYTE nOctet4)
{
	Set (nOctet1, nOctet2, nOctet3, nOctet4);
}

CIPAddress::CIPAddress (DWORD dwAddr)
{
	m_nOctets[3] = (BYTE)(dwAddr & 0xFF);	dwAddr >>= 8;
	m_nOctets[2] = (BYTE)(dwAddr & 0xFF);	dwAddr >>= 8;
	m_nOctets[1] = (BYTE)(dwAddr & 0xFF);	dwAddr >>= 8;
	m_nOctets[0] = (BYTE)(dwAddr & 0xFF);
}

CIPAddress::~CIPAddress()
{
}

/*
** Set an IP address from 4 octet values
*/
int CIPAddress::Set	(BYTE nOctet1, BYTE nOctet2, BYTE nOctet3, BYTE nOctet4)
{
	m_nOctets[0] = nOctet1;
	m_nOctets[1] = nOctet2;
	m_nOctets[2] = nOctet3;
	m_nOctets[3] = nOctet4;

	return 0;
}

/*
** Cast to a string in dotted decimal format
*/
CIPAddress::operator CString() const
{
	CString strBuffer;
	if (0 != *((LPDWORD)m_nOctets))
		strBuffer.Format ("%d.%d.%d.%d", m_nOctets[0], m_nOctets[1], m_nOctets[2], m_nOctets[3]);
	return strBuffer;
}

const CIPAddress & CIPAddress::operator= (LPCSTR pszAddress)
{
	ASSERT(Validate(pszAddress));

	// make a temporary copy...
	CString strBuffer(pszAddress);

	for (int n = 0 ; n < 4 ; n++)
	{
		CString strOctet = BreakString (strBuffer, '.', TRUE);
		m_nOctets[n] = (BYTE)atoi(strOctet);
	}
	return *this;
}

/*
** Cast to a DWORD value
*/
CIPAddress::operator DWORD() const
{
//	return *((LPDWORD)&m_nOctets);
	DWORD dwValue;
	dwValue = (DWORD)m_nOctets[0];	dwValue <<= 8;
	dwValue += (DWORD)m_nOctets[1];	dwValue <<= 8;
	dwValue += (DWORD)m_nOctets[2];	dwValue <<= 8;
	dwValue += (DWORD)m_nOctets[3];
	return dwValue;
}

const CIPAddress & CIPAddress::operator= (DWORD dwValue)
{
	m_nOctets[3] = (BYTE)(dwValue & 0xFF);	dwValue >>= 8;
	m_nOctets[2] = (BYTE)(dwValue & 0xFF);	dwValue >>= 8;
	m_nOctets[1] = (BYTE)(dwValue & 0xFF);	dwValue >>= 8;
	m_nOctets[0] = (BYTE)(dwValue & 0xFF);
	return *this;
}

int CIPAddress::Compare (CIPAddress const & other) const
{
	if ((DWORD)*this > (DWORD)other)
		return 1;
	else if ((DWORD)*this < (DWORD)other)
		return -1;
	return 0;
}

/*
** Assignment
*/
CIPAddress const & CIPAddress::operator= (CIPAddress const & other)
{
	for (int n = 0 ; n < 4 ; n++)
		m_nOctets[n] = other.m_nOctets[n];

	return *this;
}

/*
** postfix increment operator
*/
void CIPAddress::operator++ (int)
{
	DWORD dwTemp = (DWORD)*this;
	dwTemp++;
	*this = dwTemp;
}

/*
** static function to see if a string contains a valid IP Address
*/
BOOL CIPAddress::Validate (LPCSTR pszString)
{
	// ok to send a blank string...
	if (!pszString || !(*pszString))
		return TRUE;

	// Make a temporary copy...
	CString strBuffer(pszString);
	
	// loop through the 4 octets
	for (int n = 0 ; n < 4 ; n++)
	{
		// extract this part
		CString strOctet = BreakString(strBuffer, '.', TRUE);
		// MUST contain a string
		if (strOctet.IsEmpty())
			return FALSE;
		// MUST be numeric
		if (!IsNumeric(strOctet, FALSE, FALSE))
			return FALSE;
		// MUST be between 0 and 255
		int nOctet = atoi(strOctet);
		if (nOctet < 0 || nOctet > 255)
			return FALSE;
	}
	return TRUE;
}

