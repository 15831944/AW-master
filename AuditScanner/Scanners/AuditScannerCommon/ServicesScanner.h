#pragma once



////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of an active process
//
class CSystemService
{
public:
	CSystemService(void)
	{
		_name = UNKNOWN;
		_state = "Stopped";
		_startmode = UNKNOWN;
	}
	
	CSystemService(LPCSTR name ,LPCSTR state ,LPCSTR startmode)
	{
		_name = name;
		_state = state;
		_startmode = startmode;
	}

public:
// Data Accessors
	CString	State (void)
	{ return _state; }
	void	State (LPCSTR value)
	{ _state = value; }
	//
	CString		Name (void)
	{ return _name; }
	void	Name (LPCSTR value)
	{ _name = value; }
	//
	CString	StartMode (void)
	{ return _startmode; }
	void	StartMode (CString value)
	{ _startmode = value; }

// Internal data
private:
	CString		_name;
	CString		_state;
	CString		_startmode;
};



class CServicesScanner : public CAuditDataScanner
{
public:
	CServicesScanner(void);
public:
	virtual ~CServicesScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	void	ScanServices();
	void	DetectActiveServices		(void);
	void	DetectInstalledServices		(void);
	CString CheckMuiCache				(CString& strDisplayName);

private:
	CDynaList<CSystemService>	_listServices;

private:
};