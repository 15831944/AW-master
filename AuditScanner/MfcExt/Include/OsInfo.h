
// FILE:	OsInfo.h
// PURPOSE:	Class for detection of host operating system
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)
// NOTES:	Include this file in stdafx.h

#ifndef _OSINFO_DEF_
#define _OSINFO_DEF_

class COsInfo
{
public:
	enum { unknown, win95, win98, winME, winNT3, winNT4, win2K ,winXP};
public:
	// constructor
	COsInfo();
	// destructor
	~COsInfo();
	// return OS name
	const CString & GetName()		{ return m_strName; }
	// return OS Version
	const CString &	GetVersion()	{ return m_strVersion; }
	// return OS Product ID / Serial Number
	const CString &	GetProductID()	{ return m_strProductID; }
	// return OS CD Key
	const CString &	GetCDKey()		{ return m_strCDKey; }
	// return OS class (see enum above)
	int GetClass()					{ return m_nClass; }
	// return TRUE if Operating System is NT or derivative
	BOOL IsNT()						{ return (m_nClass == winNT3 || m_nClass == winNT4 || m_nClass == win2K || m_nClass == winXP); }
	// return TRUE if running on a 64 bit oS
	BOOL Is64BitWindows()			{ return m_bIs64BitWindows; }
protected:
	// run detection and store internally
	BOOL Detect();
	CString GetAdvancedProductInfo();

protected:
	static CString	m_strName;
	static CString	m_strVersion;
	static CString	m_strProductID;
	static CString	m_strCDKey;
	static int		m_nClass;
	static BOOL		m_bIs64BitWindows;
	int		m_iMajorVersion;
	int		m_iMinorVersion;
public:
	int VersionMajor(void)
	{
		return m_iMajorVersion;
	}
	int VersionMinor(void)
	{
		return m_iMinorVersion;
	}

};

#endif //#define _OSINFO_DEF_