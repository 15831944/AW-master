#pragma once
#include "auditdatascanner.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Printer
//
class CPrinter
{
public:
	CPrinter(void);
	~CPrinter(void)
	{}

// Data accessors
public: 
	CString& Name (void)
	{ return _name; }
	void	Name	(LPCSTR value)
	{ _name = value; }
	//
	CString& Driver(void)
	{ return _driver; }
	void	Driver (LPCSTR value)
	{ _driver = value; }
	//
	CString&	Port(void)
	{ return _port; }
	void	Port (LPCSTR value)
	{ _port = value; }
	//
	CString&	DetectedErrorState(void)
	{ return _detectedErrorState; }
	void	DetectedErrorState (LPCSTR value)
	{ _detectedErrorState = value; }
	//
	CString&	PrinterStatus(void)
	{ return _printerStatus; }
	void	PrinterStatus (LPCSTR value)
	{ _printerStatus = value; }

	CString&	DefaultPrinter(void)
	{ return _defaultPrinter; }
	void	DefaultPrinter (CString value)
	{ _defaultPrinter = value; }

// Internal Data
private:
	CString		_name;
	CString		_driver;
	CString		_port;
	CString		_printerStatus;
	CString		_detectedErrorState;
	CString		_defaultPrinter;
};



////////////////////////////////////////////////////////////////////////////
//
//    Memory Slot Scanner
//
class CPrinterScanner : public CAuditDataScanner
{
public:
	CPrinterScanner();
	~CPrinterScanner(void);


// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistry9X	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

private:
	CDynaList<CPrinter>	_listPrinters;
};
