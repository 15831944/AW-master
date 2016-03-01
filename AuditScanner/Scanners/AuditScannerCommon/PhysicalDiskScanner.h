#pragma once


////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Physical Disk
//
class CPhysicalDisk
{
public:
	CPhysicalDisk(void)
	{
		_manufacturer = UNKNOWN;
		_model = UNKNOWN;
		_partitions = 0;
		_size = 0;
		_cylinders = 0;
		_tracksPerCylinder = 0;
		_sectorsPerTrack = 0;
		_heads = 0;
	}

	~CPhysicalDisk(void)
	{}

// Data Accessors
public:
	CString		Manufacturer (void)
	{ return _manufacturer; }
	void	Manufacturer (LPCSTR value)
	{ _manufacturer = value; }
	//
	CString	Model (void)
	{ return _model; }
	void	Model (LPCSTR value)
	{ _model = value; }
	//
	DWORD		Partitions(void)
	{ return _partitions; }
	void	Partitions (DWORD value)
	{ _partitions = value; }
	//
	DWORD		Size (void)
	{ return _size; }
	void	Size (DWORD value)
	{ _size = value; }
	//
	DWORD		Cylinders (void)
	{ return _cylinders; }
	void	Cylinders (DWORD value)
	{ _cylinders = value; }
	//
	DWORD	TracksPerCylinder (void)
	{ return _tracksPerCylinder; }
	void	TracksPerCylinder (DWORD value)
	{ _tracksPerCylinder = value; }
	//
	DWORD	SectorsPerTrack (void)
	{ return _sectorsPerTrack; }
	void	SectorsPerTrack (DWORD value)
	{ _sectorsPerTrack = value; }
	//
	DWORD	Heads (void)
	{ return _heads; }
	void	Heads (DWORD value)
	{ _heads = value; }

// Internal data
private:
	CString		_manufacturer;
	CString		_model;	
	DWORD		_partitions;
	DWORD		_size;
	DWORD		_cylinders;
	DWORD		_tracksPerCylinder;
	DWORD		_sectorsPerTrack;
	DWORD		_heads;
};




////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates the logical Drive Scanner within AuditWizard
//

class CPhysicalDiskScanner : public CAuditDataScanner
{
public:
	CPhysicalDiskScanner(void);
public:
	virtual ~CPhysicalDiskScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	CDynaList<CPhysicalDisk>	_listPhysicalDisks;
};
