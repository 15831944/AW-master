#pragma once
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "AuditDataFile.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates the scanning of Hardware Items
//

class CHardwareScanner
{
public:
	CHardwareScanner();
	virtual ~CHardwareScanner(void);


public:
	void			ScanPhysicalDisks		(BOOL trueOrFalse)
	{ _scanPhysicalDisks = trueOrFalse; }
	
	void			ScanActiveSystem		(BOOL trueOrFalse)
	{ _scanActiveSystem = trueOrFalse; }
	
	void			ScanSecurity		(BOOL trueOrFalse)
	{ _scanSecurity = trueOrFalse; }
	
	void			ScanSettings		(BOOL trueOrFalse)
	{ _scanSettings = trueOrFalse; }

	void			Clear ()
	{ m_listHardware.Empty(); }

// Public Methods
public:
	bool	Scan	(CWMIScanner* pWMIScanner);

	// Save results to the audit data file
	bool	Save	(CAuditDataFile& auditDataFile);

private:
	CDynaList<CAuditDataScanner*> m_listHardware;

private:
	BOOL	_scanPhysicalDisks;
	BOOL	_scanActiveSystem;
	BOOL	_scanSecurity;
	BOOL	_scanSettings;

};
