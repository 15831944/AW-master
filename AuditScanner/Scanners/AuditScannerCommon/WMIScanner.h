#pragma once
#include "LaytonWMI.h"

// Auditing Objects
// Forward references
class CSystemBiosScanner;
class CSystemProcessorScanner;

class CWMIScanner
{
public:
	CWMIScanner(void);
	~CWMIScanner(void);

public:
	// Attempt to connect to WMI (on the local PC)
	BOOL Connect			();

	// Are we connected to the local WMI namespace
	BOOL IsConnected		();
	
	// Disconnect
	BOOL Disconnect			();

	// Get the WMI connection object
	CLaytonWMI&		GetWMIConnection	()
	{ return m_WMI; }


// Audit Scanner Functions
public:
	BOOL	GetBiosInformation		(CSystemBiosScanner* pSystemBios);
	BOOL	GetDomainInformation	(CString& strDomain);
	BOOL	GetProcessorInformation	(CSystemProcessorScanner* pSystemProcessor);

// Data
protected: 
	CLaytonWMI		m_WMI;				// Base Class to access WMI
	BOOL			m_bConnected;		// Flag to show connected to WMI namespace
};
