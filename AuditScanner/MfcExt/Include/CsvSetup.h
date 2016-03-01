
// FILE:	CCsvSetup.h
// PURPOSE:	Helper class for managing csv file settings
// AUTHOR:	JRF Thornley - copyright (C) Layton Software 2002
// HISTORY:	JRFT - 30.09.2002 - written

#ifndef _CSVSETUP_DEF_
#define _CSVSETUP_DEF_

class CCsvSetup
{
public:
	CCsvSetup();
	void Serialize (CArchive & ar);
	CCsvSetup const & operator= (CCsvSetup const & other);
	BOOL operator== (CCsvSetup const & other) const;

	BOOL ShowTitle () const					{ return m_bShowTitle; }
	const CString & GetTitle() const		{ return m_strTitle; }
	BOOL ShowColNames () const				{ return m_bShowColNames; }
	BOOL ShowRecIDs() const					{ return m_bShowIDs; }
	const CString & GetFieldSep () const	{ return m_strFieldSep; }
	const CString & GetRecordSep ()	const	{ return m_strRecordSep; }
	BOOL AskForFileName () const			{ return m_bFileNameAsk; }
	const CString & GetFileName () const	{ return m_strFileName; }
	BOOL OverwriteAsk () const				{ return m_bOverwriteAsk; }
	BOOL RemoveDuplicates () const			{ return m_bRemoveDuplicates; }

	void SetShowTitle (BOOL bShow)		{ m_bShowTitle = bShow; }
	void SetTitle (LPCSTR pszTitle)		{ m_strTitle = pszTitle; }
	void SetShowColNames (BOOL bShow)	{ m_bShowColNames = bShow; }
	void SetShowRecIDs (BOOL bShow)		{ m_bShowIDs = bShow; }
	void SetFieldSep (LPCSTR pszSep)	{ m_strFieldSep = pszSep; }
	void SetRecordSep (LPCSTR pszSep)	{ m_strRecordSep = pszSep; }
	void SetFileNameAsk (BOOL bAsk)		{ m_bFileNameAsk = bAsk; }
	void SetFileName (LPCSTR pszFile)	{ m_strFileName = pszFile; }
	void SetOverwriteAsk (BOOL bAsk)	{ m_bOverwriteAsk = bAsk; }
	void SetRemoveDuplicates (BOOL bSet){ m_bRemoveDuplicates = bSet; }
protected:
	BOOL	m_bShowTitle;
	CString	m_strTitle;
	BOOL	m_bShowColNames;
	BOOL	m_bShowIDs;
	BOOL	m_bRemoveDuplicates;
	CString	m_strFieldSep;
	CString	m_strRecordSep;
	BOOL	m_bFileNameAsk;
	CString	m_strFileName;
	BOOL	m_bOverwriteAsk;
};

#endif//#ifndef _CSVSETUP_DEF_