#pragma once


////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Logical Drive
//
class CLogicalDrive
{
public:
	CLogicalDrive(void)
	{
		_fileSystem = UNKNOWN;
		_deviceName = UNKNOWN;
		_description = UNKNOWN;
		_volumeName = UNKNOWN;
		_providerName = UNKNOWN;
		_driveLetter = UNKNOWN;
		_totalSize = 0;
		_freeSpace = 0;		
		_deviceType	 = UNKNOWN;	
	}

public:
	enum DEVICETYPE { eDeviceUnknown=0, eDeviceNoRoot ,eDeviceRemovable ,eDeviceLocal ,eDeviceNetwork ,eDeviceCompact ,eDeviceRAM };

// Data Accessors
	CString		DeviceName (void)
	{ return _deviceName; }
	void	DeviceName (LPCSTR value)
	{ _deviceName = value; }
	//
	CString	Description (void)
	{ return _description; }
	void	Description (LPCSTR value)
	{ _description = value; }
	//
	CString	VolumeName (void)
	{ return _volumeName; }
	void	VolumeName (LPCSTR value)
	{ _volumeName = value; }
	//
	CString	FileSystem (void)
	{ return _fileSystem; }
	void	FileSystem (LPCSTR value)
	{ _fileSystem = value; }
	//
	CString	DriveLetter (void)
	{ return _driveLetter; }
	void	DriveLetter (LPCSTR value)
	{ _driveLetter = value; }
	//
	CString	ProviderName (void)
	{ return _providerName; }
	void	ProviderName (LPCSTR value)
	{ _providerName = value; }
	//
	DWORD	TotalSize (void)
	{ return _totalSize; }
	void	TotalSize (DWORD value)
	{ _totalSize = value; }
	//
	DWORD	FreeSpace (void)
	{ return _freeSpace; }
	void	FreeSpace (DWORD value)
	{ _freeSpace = value; }
	//
	CString	DeviceType (void)
	{ return _deviceType; }
	void	DeviceType (LPCSTR value)
	{ _deviceType = value; }

// Internal data
private:
	CString		_deviceName;		// Device netbios or DNS name
	CString		_description;		// Drive type (HD, FD, CDRom, REMOVABLE, NETWORK)
	CString		_volumeName;		// OS volume name
	CString		_fileSystem;		// File system type (FAT, FAT32, NTFS...)
	CString		_providerName;		// Map point for network mapped devices
	CString		_driveLetter;		// OS drive letter
	DWORD		_totalSize;			// Total size in MB
	DWORD		_freeSpace;			// Free space in MB
	CString		_deviceType;		// Type of device
};




////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates the logical Drive Scanner within AuditWizard
//

class CLogicalDriveScanner : public CAuditDataScanner
{
public:
	CLogicalDriveScanner(void);
public:
	virtual ~CLogicalDriveScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	BOOL			ScanDrives		();
	BOOL			GetDiskSpace	(LPCSTR pszDirectory, LPDWORD pFree, LPDWORD pTotal);
	BOOL			GetUniversalName (char * pszBuffer, LPCSTR pszDrive);

private:
	CDynaList<CLogicalDrive>	_listLogicalDrives;
};
