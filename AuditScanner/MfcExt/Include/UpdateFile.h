
// FILE:	UpdateFile.h
// PURPOSE:	Classes to handle transmission of partial audit information from scanner to manager
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology
// HISTORY:	JRFT - 01.07.2002 - written
// NOTES:	This is intended to cater for any incremental change notification for
//	any type of scanned information. Hence the ability to list more than one category
//	of information, and for each category to list more than one changed data field.
//	Eg if a monitored file has changed then the file will contain a CUpdateCat record
//	with the type set to file and the category set to the folder name. This would then
//	contain a list of CUpdateItems where each one contained a key/value that had changed
//	such as "CRC" and the new value.


#ifndef _UPDATEFILE_DEF_
#define _UPDATEFILE_DEF_

/*
** A record of a changed item of audit information
*/
class CUpdateItem
{
public:
	CUpdateItem ()
		{}
	CUpdateItem (LPCSTR pszKey, LPCSTR pszVal);
	const CString & GetKey() const
		{ return m_strKey; }
	const CString & GetVal() const
		{ return m_strVal; }
	void SetVal (LPCSTR pszNewVal)
		{ m_strVal = pszNewVal; }
	void Serialize (CArchive & ar);
protected:
	CString	m_strKey;
	CString	m_strVal;
};

/*
** File comprising any changed audit information for a given asset
*/
class CUpdateCat : public CDynaList<CUpdateItem>
{
public:
	enum eUpdateType { unknown, hardware, file, regApp, internet };
public:
	// default constructor
	CUpdateCat () : m_nType(unknown)
		{}
	// set the category that the info belongs to
	void SetType (LPCSTR pszEventName, eUpdateType nType, LPCSTR pszCat);
	// merge/update from a second object's records
	CUpdateCat const & operator+= (CUpdateCat const & other);
	// serialization
	void Serialize (CArchive & ar);
	DWORD Find (LPCSTR pszKey) const;
	// return name of the event that triggered this update
	CString const & EventName (void) const
		{ return m_strEventName; }

	//
	eUpdateType	Type	(void)
	{ return m_nType; }

	CString&	Category	(void)
	{ return m_strCat; }

protected:
	CString		m_strEventName;	// Original event def name that defined this trigger event
	eUpdateType	m_nType;		// type of trigger, see enum above
	CString		m_strCat;		// additional category info, eg hardware sub-category
};

/*
** File pertaining to a single asset, containing a list of one or more update lists
*/
class CUpdateFile : public CDynaList<CUpdateCat>
{
public:
	CUpdateFile ()
		{}
	CUpdateFile (LPCSTR pszAsset);
	
	CString & GetAssetName ()
		{ return m_strAsset; }
	// look for a category with a matching event name
	DWORD Find (LPCSTR pszEventName) const;
	// merge another file
	CUpdateFile const & operator+= (CUpdateFile const & other);

	// store the complete record to specified data path
	BOOL Write (LPCSTR pszPath);
	void Serialize (CArchive & ar);
protected:
	CString		m_strAsset;
};

#endif//#ifndef _UPDATEFILE_DEF_