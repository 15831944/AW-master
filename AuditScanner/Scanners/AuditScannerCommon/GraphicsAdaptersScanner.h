#pragma once
#include "AuditDataScanner.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Graphics / Video Adapter
//
class CGraphicsAdapter
{
public:
	CGraphicsAdapter(void);
	CGraphicsAdapter(CString& adapterName ,CString& chipset ,CString& memory ,CString& driverVersion)
	{ _adapterName = adapterName; _chipset = chipset; _memory = memory; _driverVersion = driverVersion; }
	~CGraphicsAdapter(void)
	{}

// Data accessors
public: 
	CString&	AdapterName (void)
	{ return _adapterName; }
	void		AdapterName	(LPCSTR value)
	{ _adapterName = value; }
	//
	CString&	Chipset (void)
	{ return _chipset; }
	void		Chipset	(LPCSTR value)
	{ _chipset = value; }
	//
	CString&	Memory (void)
	{ return _memory; }
	void		Memory	(LPCSTR value)
	{ _memory = value; }
	//
	CString&	DriverVersion (void)
	{ return _driverVersion; }
	void		DriverVersion	(LPCSTR value)
	{ _driverVersion = value; }
	//
	int			RefreshRate (void)
	{ return _refreshRate; }
	void		RefreshRate	(int value)
	{ _refreshRate = value; }
	//
	CString&	Resolution (void)
	{ return _resolution; }
	void		Resolution	(LPCSTR value)
	{ _resolution = value; }
	//
	CString&	MonitorType (void)
	{ return _monitorType; }
	void		MonitorType	(LPCSTR value)
	{ _monitorType = value; }

// Internal Data
private:
	CString		_adapterName;
	CString		_chipset;
	CString		_memory;
	CString		_driverVersion;
	int			_refreshRate;
	CString		_resolution;
	CString		_monitorType;
};


////////////////////////////////////////////////////////////////////////////
//
//    Graphics / Video Adapters Scanner
//
class CGraphicsAdaptersScanner : public CAuditDataScanner
{
public:
	CGraphicsAdaptersScanner();
	~CGraphicsAdaptersScanner(void);

// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistry9X	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

// Public functions
public:

private:
	CDynaList<CGraphicsAdapter>		_listGraphicsAdapters;
};
