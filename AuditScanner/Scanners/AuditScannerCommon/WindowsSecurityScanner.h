#pragma once

#include "WindowsUpdate.h"
#include "firewall.h"

class CWindowsSecurityScanner :	public CAuditDataScanner
{
public:
	CWindowsSecurityScanner(void);
public:
	virtual ~CWindowsSecurityScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	void ScanWindowsSecurity();

private:
	CWindowsUpdate	_updateInformation;
	CFirewallInfo	_firewallInformation;

	
};
