///////////////////////////////////////////////////////////////////////////////
//
//    CSoftwareScanner
//    ================
//
//    The Main SOFTWARE Scanning Class
//
#include "Stdafx.h"
#include "AuditDataFile.h"
#include "SoftwareScanner.h"

CSoftwareScanner::CSoftwareScanner()
{
}

CSoftwareScanner::~CSoftwareScanner()
{
}


//
//    Scan
//    ====
//
//    Main software scanning function
//
bool CSoftwareScanner::Scan(CAuditScannerConfiguration* auditScannerConfiguration)
{
	try
	{
		_applicationInstances.Detect(auditScannerConfiguration);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CSoftwareScanner::Save	(CAuditDataFile& auditDataFile)
{
	CLogFile log;

	log.Write("CSoftwareScanner::Save in");

	for (int isub = 0 ; isub < _applicationInstances.Count(); isub++)
	{
		//log.Format("saving application %s \n", _applicationInstances[isub]->Name());
		auditDataFile.AddAuditedApplication(*(_applicationInstances[isub]));
	}
	return false;
}
