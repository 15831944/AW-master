#pragma once
#include "AuditDataScanner.h"
#include "AuditDataFile.h"

class COperatingSystemScanner
{
public:
	COperatingSystemScanner();
public:
	~COperatingSystemScanner(void);

// Functions
public:
	virtual bool	Scan			(void);

	// Save results to the hardware section of the audit data file
	bool	Save	(CAuditDataFile& auditDataFile);

	CString&		OSFamily()
	{ return _osFamily; }

	CString&		OSVersion()
	{ return _osVersion; }

	CString&		OSProductID()
	{ return _osProductID; }

	CString&		OSCDKey()
	{ return _osCDKey; }

	CString&		OSIEVersion()
	{ return _osIEVersion; }

	bool			OSIs64Bit()
	{ return _osIs64Bit; }
	
protected:
	CString _osFamily;
	CString	_osVersion;
	CString	_osProductID;
	CString	_osCDKey;
	CString _osIEVersion;
	bool	_osIs64Bit;

};
