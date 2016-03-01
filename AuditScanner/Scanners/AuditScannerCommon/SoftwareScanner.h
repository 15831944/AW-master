#pragma once
#include "AuditDataScanner.h"
#include "ApplicationInstance.h"
#include "AuditDataFile.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates the scanning of software applications
//

class CSoftwareScanner
{
public:
	CSoftwareScanner();
	virtual ~CSoftwareScanner(void);


// Public Methods
public:
	bool	Scan	(CAuditScannerConfiguration* auditScannerConfiguration);

	// Save results to the audit data file
	bool	Save	(CAuditDataFile& auditDataFile);

private:
	CApplicationInstanceList _applicationInstances;

};
