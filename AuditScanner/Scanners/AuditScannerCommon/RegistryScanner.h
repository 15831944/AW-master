#pragma once

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of an registry key
//
class CRegistryKey
{
public:
	CRegistryKey(void)
	{
		_name = "<not specified>";
		_value = "<not specified>";
	}
	
	CRegistryKey(LPCSTR name ,LPCSTR value)
	{
		_name = name;
		_value = value;
	}

public:
// Data Accessors
	CString		Name (void)
	{ return _name; }
	void	Name (LPCSTR value)
	{ _name = value; }
	//
	CString		Value (void)
	{ return _value; }
	void	Value (LPCSTR value)
	{ _value = value; }

// Internal data
private:
	CString		_value;
	CString		_name;
};


class CRegistryScanner
{
public:
	//CRegistryScanner(void);

		// constructor
	//CRegistryScanner()
	//	{}
	//// destructor
	//~CRegistryScanner()
	//	{}

	CRegistryScanner();
	virtual ~CRegistryScanner(void);	

// Base class over-rides
public:
	bool	Scan		(void) ;
	bool	Save	(CAuditDataFile& pAuditDataFile);
	CString GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem);
	void SetOptions (CDynaList<CString>& listRegKeysToAudit);

protected:
	CDynaList<CString> _listRegKeysToAudit;

private:
	CDynaList<CRegistryKey>	_listRegKeysFound;
};