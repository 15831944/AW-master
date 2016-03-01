#pragma once

class CInstalledObject
{
public:
	CInstalledObject(void)
	{ _name = UNKNOWN ,_version = UNKNOWN; }

	CInstalledObject(LPCSTR name ,LPCSTR version)
	{ _name = name; _version = version; }

	~CInstalledObject(void)
	{}

// Data accessors
public: 
	CString& Name (void)
	{ return _name; }
	void	Name	(LPCSTR value)
	{ _name = value; }
	//
	CString& Version(void)
	{ return _version; }
	void	Version	(LPCSTR value)
	{ _version = value; }

private:
	CString		_name;
	CString		_version;
};


class CObjectScanner :	public CAuditDataScanner
{
public:
	CObjectScanner(void);
public:
	virtual ~CObjectScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

private:
	CDynaList<CInstalledObject>	_listObjects;
};
