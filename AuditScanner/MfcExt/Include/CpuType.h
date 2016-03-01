
// FILE:	CpuType.h
// PURPOSE:	Class for identification of host CPU type
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)
// NOTES:	Include this file in stdafx.h

#ifndef _CPUTYPE_DEF_
#define _CPUTYPE_DEF_

class CCpuID
{
public:
	// possible manufacturers
	enum { unknown, intel, cyrix, amd };
public:
	// constructor - does the initial check for availability
	CCpuID();
	// return level of CPUID support, if any
	int GetMaxLevel ()
		{ return m_nMaxLevel; }
	// return chip manufacturer - see enum above
	int GetMfr ()
		{ return m_nMfr; }
	// run a cpuid instruction 
	void RunCPUID (DWORD & EAX, DWORD & EBX, DWORD & ECX, DWORD & EDX);
protected:
	static int m_nMfr;
	static int m_nMaxLevel;
};

class CCpuType
{
public:
	// types returned by GetLevel()
	enum { unknown, level3, level4, level5, level6 };
public:
	// constructor
	CCpuType();
	// destructor
	~CCpuType();
	// return processor level (see enum above)
	int GetLevel();
	// return processor manufacturer, may be blank
	const CString & GetMfr();
	// return processor type string
	const CString & GetType();
	// return CPUID code
	DWORD GetCpuID();
	// Return a rationalized version of the processor name
	const CString GetRationalizedName(void);
	// Rationalize a processor name
	CString RationalizeName(CString& strProcessorName);

protected:
	// detect the processor type
	void Detect();
	// Is the processor a 386 or older ?
	BOOL Is386();
	// specific CPUID detection for Intels
	void DetectIntel();
	// specific CPUID detection for Cyrix
	void DetectCyrix();
	// specific CPUID detection for AMDs
	void DetectAMD();
	// specific algorithm to identify cyrix chips without using CPUID
	BOOL IsCyrix();
	// return brand string using CPUID, available on some Intel and AMD chips
	BOOL GetBrandString(CString & string);
	// clean up returned brand strings and find known CPU types
	void ParseBrandString (CString & string);
	// Trim multiple white spaces
	CString TrimSpaces (CString& strInput);
	CString TrimSpeeds (CString& strInput);


protected:
	static int			m_nLevel;
	static CString		m_strMfr;
	static CString		m_strType;
	static DWORD		m_dwCpuID;
};

class CCpuCache
{
public:
	// default constructor
	CCpuCache();
	// one and only destructor
	~CCpuCache();
	// return cache size
	BOOL GetSize (LPINT pInstructionCache, LPINT pDataCache, LPINT pL2Cache);
protected:
	// run CPUID (if possible) to obtain cache information
	BOOL Detect();
	// Checks whether CPUID instruction is available
//	BOOL CheckCPUID();
//	BOOL CheckCPUID2();
//	void RunCPUID(DWORD & EAX, DWORD & EBX, DWORD & ECX, DWORD & EDX);
	void ProcessFlags (DWORD dwRegister);
protected:
	static int	m_nInstructionCache;
	static int	m_nDataCache;
	static int	m_nL2Cache;
};

#endif //#define _CPUTYPE_DEF_