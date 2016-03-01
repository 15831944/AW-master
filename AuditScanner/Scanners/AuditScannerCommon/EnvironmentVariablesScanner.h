#pragma once


////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of an environment variable
//
class CEnvironmentVariable
{
public:
	CEnvironmentVariable(void)
	{
		_name = UNKNOWN;
		_value = UNKNOWN;
	}
	
	CEnvironmentVariable(LPCSTR name ,LPCSTR value)
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



class CEnvironmentVariablesScanner : public CAuditDataScanner
{
public:
	CEnvironmentVariablesScanner(void);
public:
	virtual ~CEnvironmentVariablesScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	BOOL	ScanVariables();
	BOOL	VariableExists	(CString& name);

private:
	CDynaList<CEnvironmentVariable>	_listVariables;
};