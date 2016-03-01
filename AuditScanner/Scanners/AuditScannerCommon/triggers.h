
// FILE:	Triggers.h
// PURPOSE:	Class to deal with aspects of event triggering dealt with by Audit Scanner
// AUTHOR:	JRF Thornley - copyright (c) Layton Technology 2002
// HISTORY:	20.05.2002 - JRFT - developed from original code by CMD

#ifndef _TRIGGERS_DEF_
#define _TRIGGERS_DEF_

#include "UpdateFile.h"

#define EVENT_FILE	"Alerts.dat"

#define EVENT_SIG		"AuditWizard Event Definition File. Do Not Edit"
#define EVENT_VER_MAJOR	1
#define EVENT_VER_MINOR	1
#define TRIG_VER_MAJOR	1
#define TRIG_VER_MINOR	1

class CEventTrigger
{
public:
	enum { opChanged, opLessThan, opFolder, opNull, opContains };
public:
	void Serialize (CArchive & ar);
	const CString & GetField () const
		{ return m_strDataField; }
	int GetOpCode () const
		{ return m_nOperator; }
	const CString & GetValue () const
		{ return m_strValue; }
protected:
	CString	m_strDataField;		// name of data field for trigger
	int		m_nOperator;		// Operation to check
	CString	m_strValue;			// value to check for
};

class CEventDef : public CDynaList<CEventTrigger>
{
public:
	// save / load to disk
	void Serialize (CArchive & ar);
	// return TRUE if event applies to pszAsset
	BOOL CheckAsset (LPCSTR pszAsset) const;
	// return name of event, as designated by AudWiz manager
	const CString & GetName() const
		{ return m_strName; }
protected:
	CString			m_strName;			// Name of event
	CString			m_strLocations;		// Locations string
	CString			m_strAssets;		// Assets string
	int				m_nAction;			// Action to take on event trigger
	CString			m_strActionValue;	// Value associated with teh action field
	DWORD			m_dwEventFlags;		// Flags to determine event options
	CString			m_strDataField;		// Data field used by event
};

/*
** This is effectively the EVENTS.DAT file
*/
class CEventDefList : public CDynaList<CEventDef>
{
public:
	void Serialize (CArchive & ar);
};

/*
** Base class for a scan operation implemented as a trigger
*/
class CTriggerScanner
{
public:
	enum { opChanged, opLessThan, opFolder, opNull, opContains };
public:
	// constructor
	CTriggerScanner(LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal);
	// destructor
	virtual ~CTriggerScanner()
		{}
	// collect data
	virtual BOOL Scan () = 0;
	// see if trigger event has happened, and generate event log if so
	virtual BOOL Test (CUpdateFile & file) = 0;
	// serialize any collected data
	virtual void Save (CAuditDataFile & file) const = 0;
	virtual void Load (CAuditDataFile & file) = 0;
protected:
	CString m_strEventName;	// as defined by the manager
	int		m_nOpCode;		// how to check for trigger firing
	CString	m_strTestVal;	// test value defined by manager
};

/*
** Collection of variant trigger scanners
*/
class CTriggerScanList : public CDynaList<CTriggerScanner *>
{
public:
	// construction
	CTriggerScanList()
		{}
	// destruction
	virtual ~CTriggerScanList();
	// build list of scanners (returns number of scanner objects set up)
	int Setup (CEventDefList const & defs, LPCSTR pszAsset);
	// serialise scan results
	void Save (CAuditDataFile & file) const;
	void Load (CAuditDataFile & file);
	// collect scan results
	BOOL Scan();
	// test for any trigger fired (returns number of triggers)
	int Test (CUpdateFile & file);
protected:
};

/*
** Hardware trigger
*/
class CTriggerScannerHw : public CTriggerScanner
{
public:
	CTriggerScannerHw (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszCat, LPCSTR pszKey);
	virtual ~CTriggerScannerHw();
	virtual BOOL Scan();
	virtual BOOL Test(CUpdateFile & file);
	virtual void Save(CAuditDataFile & file) const;
	virtual void Load(CAuditDataFile & file);
protected:
	CString		m_strCat;
	CString		m_strKey;
	CBaseScanner *	m_pDataNew;
	CBaseScanner *	m_pDataOld;
};

/*
** Internet History Trigger
*/
class CTriggerScannerIe : public CTriggerScanner
{
public:
	CTriggerScannerIe (LPCSTR pszEventName, int nOpCode, LPCSTR pszSearchText);
	virtual ~CTriggerScannerIe()
		{}
	virtual BOOL Scan();
	virtual BOOL Test(CUpdateFile & file);
	virtual void Save(CAuditDataFile & file) const;
	virtual void Load(CAuditDataFile & file);
protected:
	CDynaList<CString>	m_thisSearch;
	CDynaList<CString>	m_lastSearch;
};

/*
** Operating System Trigger - a subset of hardware in effect
*/
class CTriggerScannerOs : public CTriggerScannerHw
{
public:
	CTriggerScannerOs (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszKey);
	virtual ~CTriggerScannerOs()
		{}
protected:
};


/*
** System Trigger - a subset of hardware in effect
*/
class CTriggerScannerSystem : public CTriggerScannerHw
{
public:
	CTriggerScannerSystem (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal, LPCSTR pszKey);
	virtual ~CTriggerScannerSystem()
		{}
protected:
};


/*
** Registered Apps Trigger
*/
class CTriggerScannerApp : public CTriggerScannerHw
{
public:
	CTriggerScannerApp (LPCSTR pszEventName, int nOpCode, LPCSTR pszTestVal);
	virtual ~CTriggerScannerApp()
		{}
	virtual BOOL Test(CUpdateFile & file);
};

#endif
