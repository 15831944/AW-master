
// FILE:	IPAddress.h
// PURPOSE:	Declaration of classes for storing and manipulating IP Addresses
// AUTHOR:	C.Drew - copyright (C) Layton Technology Ltd
// HISTORY:	CMD	- 01.07.2003 - written

#ifndef _IPADDRESS_DEF_
#define _IPADDRESS_DEF_

class CIPAddress  
{
public:
	// static to validate a dotted-decimal format string prior to storage
	static BOOL Validate (LPCSTR pszString);

public:
	// Construction
	CIPAddress ();
	CIPAddress (LPCSTR pszAddress);
	CIPAddress (BYTE nOctet1, BYTE nOctet2, BYTE nOctet3, BYTE nOctet4);
	CIPAddress (DWORD dw);
	virtual ~CIPAddress();

	int Set	(BYTE nOctet1, BYTE nOctet2, BYTE nOctet3, BYTE nOctet4);

	// assignment operators
	const CIPAddress & operator= (CIPAddress const & other);
	const CIPAddress & operator= (LPCSTR pszString);
	const CIPAddress & operator= (DWORD dwValue);

	// cast operators
	operator CString() const;
	operator DWORD() const;

	// array operator to extract an octet (n must be in range 0-3)
	BYTE operator[] (int n)
		{ ASSERT(n>=0 && n<4); return m_nOctets[n]; }

	// comparison operators
	BOOL operator< (const CIPAddress & other) 
		{ return ((DWORD)*this < (DWORD)other); }
	BOOL operator<= (const CIPAddress & other) 
		{ return ((DWORD)*this <= (DWORD)other); }
	BOOL operator> (const CIPAddress & other) 
		{ return ((DWORD)*this > (DWORD)other); }
	BOOL operator>= (const CIPAddress & other) 
		{ return ((DWORD)*this >= (DWORD)other); }
	BOOL operator== (const CIPAddress & other) 
		{ return ((DWORD)*this == (DWORD)other); }
	// returns -1 or 0 or +1
	int Compare (const CIPAddress & other) const;

	// postfix increment / decrement operators
	void operator++ (int);
	void operator-- (int);

protected:
	BYTE	m_nOctets[4];
};

class CIPAddressRange  
{
public:
	// Constructors
	CIPAddressRange()
		{}
	CIPAddressRange (CIPAddress const & lower, CIPAddress const & upper)
		: m_lower(lower), m_upper(upper)
		{}
	CIPAddressRange (LPCSTR pszDesc)
	{
		CString strUpper(pszDesc);
		CString strLower = BreakString(strUpper, ',');
		m_lower = strLower;
		m_upper = strUpper;
	}

	// get / set lower value
	const CIPAddress & Lower () const
		{ return m_lower; }
	void Lower (const CIPAddress & value)
		{ m_lower = value; }

	// get / set upper value
	const CIPAddress & Upper () const
		{ return m_upper; }
	void Upper (const CIPAddress & value)
		{ m_upper = value; }

	// assignment operator
	CIPAddressRange const & operator= (CIPAddressRange const & other)
		{ m_lower = other.m_lower; m_upper = other.m_upper; return *this; }
	// comparison operator
	BOOL operator== (CIPAddressRange const & other)
		{ return (m_lower == other.m_lower && m_upper == other.m_upper); }

protected:
	CIPAddress		m_lower;
	CIPAddress		m_upper;
};

//
//    CNetAddress
//
//    Class to hold a network address specification
//
class CNetAddress : public CIPAddressRange
{
public:
	CNetAddress()
		{};
	CNetAddress(LPCSTR strAddress1, LPCSTR strAddress2, LPCSTR strUserName, LPCSTR strPassword)
		: CIPAddressRange(strAddress1, strAddress2), m_strUserName(strUserName), m_strPassword(strPassword)
		{};
	virtual ~CNetAddress()
		{};

	CString m_strUserName;
	CString m_strPassword;
};



#endif //#ifndef _IPADDRESS_DEF_