//==================================================================================//
//																					//
//	MicroAuditScan.h																//
//  ================																//
//																					//
//	This is the header file for the main module of the Layton Technology			//
//  MicroAudit Scanner.																//
//																					//
//	(c) Copyright 2008 Layton Technology, Inc - all rights reserved					//
//																					//
//==================================================================================//
//																					//
//	16-Jan-2008			Chris Drew													//
//	Module added																	//
//																					//
//==================================================================================//
//																					//
// NOTES:	Encapsulates the functionality of a private Windows Profile file, but	//
//			without using any of the related API calls. File name can be derived	//
//			automatically from the module handle, and a section of the file can		//
//			be kept as "current". To improve performance the file contents are held	//
//			in RAM and only flushed to file when the object is destroyed, though	//
//			a flush can be explicitly requested when a value is written.			//
//																					//
// USAGE:	1. Include this in stdafx.h - must follow an inclusion of "arrays.h"	//
//			2. Add IniFile.cpp to your Project										//
//			3. Add a public member object to the application class					//
//				"CIniFile m_IniFile"												//
//			5. To read values call SetSection() then either GetString(), GetInt(),	//
//				GetBool()															//
//			6. To write values call SetSection() then either WriteString(),			//
//				WriteInt(), WriteBool()												//
//			7. To list section keys call EnumKeys(), to enumerate sections call		//
//				EnumSections()														//
//			8. For easy access define a macro in the application class header file	//
//					#define GetINI() (((CYourAppClass*)AfxGetApp())->m_IniFile)		//
//																					//
//==================================================================================//
#pragma once

#ifndef NOT_FOUND
#define NOT_FOUND 0xFFFFFFFF
#endif

/*
** Helper class to handle an INI file key entry
*/
class CIniItem
{
	friend class CIniSection;
public:
	CIniItem() {}
	BOOL operator== (const CIniItem & other) const;
protected:
	CIniItem(LPCSTR pszKey, LPCSTR pszValue) : m_strKey(pszKey), m_strValue(pszValue) {}
	CString const & GetKey() const				{ return m_strKey; }
	CString const & GetValue() const			{ return m_strValue; }
	void SetValue (LPCSTR pszValue)				{ m_strValue = pszValue; }
	int Write (CStdioFile & file) const;
protected:
	CString	m_strKey;
	CString	m_strValue;
};

/*
** Helper class to handle an INI file section
*/
class CIniSection : public CDynaList<CIniItem>
{
	friend class CIniFile;
public:
	CIniSection () {}
	BOOL operator== (const CIniSection & other) const;
protected:
	CIniSection (LPCSTR pszName) : m_strName(pszName)	{}	
	void SetName (LPCSTR pszName)				{ m_strName = pszName; }
	const CString & GetName() const				{ return m_strName; }
//	const CIniItem & operator[] (int n)	const	{ return m_data[n]; }
	void SetKey (LPCSTR pszKey, LPCSTR pszValue);
	CString GetKey (LPCSTR pszKey, LPCSTR pszDefault) const;
	int EnumKeys (LPSTR pBuffer, int nMaxLen) const;
	int EnumKeys (CDynaList<CString> & list) const;
	void RemoveKey (LPCSTR pszKey);
	int Write (CStdioFile & file) const;
	int Read (CStdioFile & file);
protected:
	DWORD FindKey (LPCSTR pszKey) const;
	CString			m_strName;
};

// Flags for INI file operations - may be combined
#define IF_NOREAD	0x01		// inhibit from reading any existing file
#define IF_NOWRITE	0x02		// inhibit from writing changes

/*
** The main public INI file class
*/
class CIniFile : public CDynaList<CIniSection>
{
public:
	// return codes from the "Open" function
	enum { errorOK=0, errorBadPath=3, errorAccessDenied=5, errorDiskFull=13 };
public:
	// one and only constructor
	CIniFile (LPCSTR pszFileName = NULL, DWORD dwFlags = 0);
	// class destructor
	virtual ~CIniFile ();
	// Read data into memory (if file not found then an empty object is created)
	int Read(LPCSTR pszFileName = NULL);
	// Update disk file
	int Write() const;
	// Select a current section, adding if necessary
	void SetSection (LPCSTR pszSection) const
		{ ((CIniFile*)this)->m_dwSection = ((CIniFile*)this)->AddSection(pszSection); }
	// set section to the title of a window
	void SetSection (CWnd * pWnd);
	// return a string value
	CString	GetString (LPCSTR pszKey, LPCSTR pszDefault = NULL) const;
	CString GetString (LPCSTR pszSection, LPCSTR pszKey, LPCSTR pszDefault) const
		{ SetSection(pszSection); return GetString(pszKey, pszDefault); }
	// return an integer value
	int	GetInt (LPCSTR pszKey, int nDefault = 0) const;
	int GetInt (LPCSTR pszSection, LPCSTR pszKey, int nDefault = 0) const
		{ SetSection(pszSection); return GetInt(pszKey, nDefault); }
	// return a boolean value (stored as Y/N or y/n)
	BOOL GetBool (LPCSTR pszKey, BOOL bDefault = FALSE) const;
	BOOL GetBool (LPCSTR pszSection, LPCSTR pszKey, BOOL bDefault = FALSE) const
		{ SetSection(pszSection); return GetBool(pszKey, bDefault); }
	// return an integer value (stored in hex format)
	DWORD GetHex (LPCSTR pszSection, LPCSTR pszKey, DWORD dwDefault = 0) const;
	DWORD GetHex (LPCSTR pszKey, DWORD dwDefault = 0) const;
	// return a Time value (stored as yyyy-mm-dd hh:mm:ss
	CTime GetTime (LPCSTR pszKey, CTime timDefault = CTime::GetCurrentTime() ,BOOL bTimeOnly=FALSE) const;
	// read a window position and resize the window
	BOOL	GetWindowPos(CWnd * pWnd, LPCSTR pszKey = NULL) const;
	// return a list of valid section names
	int EnumSections	(CDynaList<CString> & list) const;
//	int EnumSections	(LPSTR pszBuffer, int nMaxLen) const;
	// return a list of valid keys for a given section
	int EnumKeys		(LPCSTR pszSection, CDynaList<CString> & list) const;
//	int EnumKeys		(LPCSTR pszSection, LPSTR pszBuffer, int nMaxLen) const;
	// write a string value
	void WriteString	(LPCSTR pszKey, LPCSTR pszValue, BOOL bFlushNow = FALSE);
	void WriteString	(LPCSTR pszSection, LPCSTR pszKey, LPCSTR pszValue, BOOL bFlushNow = FALSE)
		{ SetSection(pszSection); WriteString (pszKey, pszValue, bFlushNow); }
	// write an integer value
	void WriteInt		(LPCSTR pszKey, int nValue, BOOL bFlushNow = FALSE);
	void WriteInt		(LPCSTR pszSection, LPCSTR pszKey, int nValue, BOOL bFlushNow = FALSE)
		{ SetSection(pszSection); WriteInt (pszKey, nValue, bFlushNow); }
	// write a boolean value as Y or N
	void WriteBool		(LPCSTR pszKey, BOOL bValue, BOOL bFlushNow = FALSE);
	void WriteBool		(LPCSTR pszSection, LPCSTR pszKey, BOOL bValue, BOOL bFlushNow = FALSE)
		{ SetSection(pszSection); WriteBool (pszKey, bValue, bFlushNow); }
	// write a hex value
	void WriteHex		(LPCSTR pszKey, DWORD dwValue, BOOL bFlushNow = FALSE);
	void WriteHex		(LPCSTR pszSection, LPCSTR pszKey, DWORD dwValue, BOOL bFlushNow = FALSE);
	// store a window position
	void WriteWindowPos	(CWnd * pWnd, LPCSTR pszKey = NULL);
	// completely remove a key
	void RemoveKey		(LPCSTR pszKey, BOOL bFlushNow = FALSE);
	// completely remove a section
	void RemoveSection	(LPCSTR pszSection, BOOL bFlushNow = FALSE);
	// empty the file
	void RemoveAll		(BOOL bFlushNow = FALSE);
	// return fully qualified path name
	const CString & GetName () const	{ return m_strFileName; }
	void SetName (LPCSTR pszName)		{ m_strFileName = pszName; }
protected:
	DWORD FindSection (LPCSTR pszSection) const;
	DWORD AddSection (LPCSTR pszSection);
protected:
	CString	m_strFileName;		// The INI file name
	DWORD	m_dwSection;		// Currently selected section
	DWORD	m_dwChanges;		// number of changes since last read or write
	DWORD	m_dwFlags;			// see constructor
};
