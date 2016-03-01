//////////////////////////////////////////////////////////////////////////////////
//																				//
//	CAuditDataScanner															//
//  ================															//
//																				//
//	Base Class for scanning audit data items - this is a pure virtual class		//
//																				//
//////////////////////////////////////////////////////////////////////////////////

#pragma once

#include "WMIScanner.h"
#include "AuditDataFile.h"

#define UNKNOWN			"<not specified>"
#define ONE_KILOBYTE	1024
#define ONE_MEGABYTE	1048576				// Note Megabyte can only be used for RAM see below
#define ONE_GIGABYTE	1000000000			// Gigabyte is 1000,000,000 when referring to
											// disk sizes and not 1024 Megabytes!!!
#define HARDWARE		"Hardware"

// The 'Hardware' details section  - consists of a list of 'hardware' elements
#define	S_HARDWAREITEM		"hardware-item"
#define V_HARDWARE_CLASS	"hardware-class"
class CAuditDataFile;
class CAuditDataScanner
{
public:
	CAuditDataScanner();
	virtual ~CAuditDataScanner(void);

public:
	// This function is called when we want to create an audit scanner which can be used with Alert
	// Monitor to scan a specific item
	static CAuditDataScanner* CreateAlertScanner (LPCSTR CreateAlertScanner);
	
	// Virtual functions
public:

	// This is the main scan function - it's task is to determine which scanner to use and call
	// as appropriate.  We may fail to audit using WMI as the item specified either may not be
	// available through WMI or WMI itself may be unavailable.  Either way we fallback to using
	// the registry scanner approach.
	virtual bool	Scan	(CWMIScanner* pWMIScanner);

	// These functions must be over-ridden by base classes as they implement the actual scanning

	// Perform scanning which is required regardles of whether WMI or registry scanning is used
	// This may be required if a piece of data is required that is not available using the WMI\registry 
	// scanner 
	virtual bool	ScanExceptions	(void)
	{ return true; }

	// This function will perform a scan using WMI if available
	virtual bool	ScanWMI			(CWMIScanner* pScanner) = 0;

	// This function will be called if a WMI approach has failed to scan a Windows NT on registry
	virtual bool	ScanRegistryNT	(void) = 0;

	// This function will be called if a WMI approach has failed to scan a Windows XP on registry
	virtual bool	ScanRegistryXP	(void) = 0;

	// This function will be called if a WMI approach has failed to scan a Windows 9X registry
	virtual bool	ScanRegistry9X	(void) = 0;

	// Over-ride this function to perform a save of the data
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile) = 0;

	// Return the path/name of the item which this scanner derivative returns
	CString&	ItemName()
	{ return m_strItemPath; }
	
protected:
	void	LogException (CException* pEx, LPCSTR function);

// Data
protected:
	CString	m_strItemPath;		// The String which gives locates the audited item within the heirachy
	HKEY	m_hKey;				// Handle to HKLM key on computer
	DWORD	m_dwPlatformId;		// OS Platform ID to determine if Win9X or NT
	BOOL	m_bIsXP;			// TRUE if Win XP or higher
};