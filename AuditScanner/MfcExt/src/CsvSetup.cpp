
// FILE:	CCsvSetup.cpp
// PURPOSE:	Helper class for managing csv file settings
// AUTHOR:	JRF Thornley - copyright (C) Layton Software 2002
// HISTORY:	JRFT - 30.09.2002 - written

#include "stdafx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/*
** Construction
*/
CCsvSetup::CCsvSetup()
{
	m_bShowTitle = FALSE;
	m_bShowColNames = TRUE;
	m_bShowIDs = FALSE;
	m_strFieldSep = ",";
	m_strRecordSep = "\n";
	m_bFileNameAsk = TRUE;
	m_bOverwriteAsk = TRUE;
	m_bRemoveDuplicates = FALSE;
}

/*
** File Serialization
*/
void CCsvSetup::Serialize(CArchive & ar)
{
	// NOTE: m_bRemoveDuplicates doesn't get stored. This is for upward compatibility...
	if (ar.IsStoring())
	{
		ar << m_bShowTitle;
		ar << m_strTitle;
		ar << m_bShowColNames;
		ar << m_bShowIDs;
		ar << m_strFieldSep;
		ar << m_strRecordSep;
		ar << m_bFileNameAsk;
		ar << m_strFileName;
		ar << m_bOverwriteAsk;
	}
	else
	{
		ar >> m_bShowTitle;
		ar >> m_strTitle;
		ar >> m_bShowColNames;
		ar >> m_bShowIDs;
		ar >> m_strFieldSep;
		ar >> m_strRecordSep;
		ar >> m_bFileNameAsk;
		ar >> m_strFileName;
		ar >> m_bOverwriteAsk;
	}
}

/*
** Assignment
*/
CCsvSetup const & CCsvSetup::operator= (CCsvSetup const & other)
{
	m_bShowTitle	= other.m_bShowTitle;
	m_strTitle		= other.m_strTitle;
	m_bShowColNames	= other.m_bShowColNames;
	m_bShowIDs		= other.m_bShowIDs;
	m_strFieldSep	= other.m_strFieldSep;
	m_strRecordSep	= other.m_strRecordSep;
	m_bFileNameAsk	= other.m_bFileNameAsk;
	m_strFileName	= other.m_strFileName;
	m_bOverwriteAsk	= other.m_bOverwriteAsk;
	m_bRemoveDuplicates = other.m_bRemoveDuplicates;
	return *this;
}

/*
** Comparison
*/
BOOL CCsvSetup::operator== (CCsvSetup const & other) const
{
	return (m_bShowTitle == other.m_bShowTitle &&
		m_strTitle == other.m_strTitle &&
		m_bShowColNames == other.m_bShowColNames &&
		m_bShowIDs == other.m_bShowIDs &&
		m_strFieldSep == other.m_strFieldSep &&
		m_strRecordSep	== other.m_strRecordSep &&
		m_bFileNameAsk	== other.m_bFileNameAsk &&
		m_strFileName	== other.m_strFileName &&
		m_bOverwriteAsk	== other.m_bOverwriteAsk &&
		m_bRemoveDuplicates == other.m_bRemoveDuplicates);
}
