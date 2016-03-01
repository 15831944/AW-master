#pragma once

class CMemoryScanner :	public CAuditDataScanner
{
public:
	CMemoryScanner(void);
public:
	virtual ~CMemoryScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	int				_totalRam;
	int				_availableRam;
	int				_totalPageFile;
	int				_availablePageFile;
	int				_totalVirtualMemory;
	int				_availableVirtualMemory;
};
