#pragma once

#define CACHEFILE_NAME	"AuditAgent.awc"

class CAssetCacheFile : public CIniFile
{
public:
	// possible options for the station file (taken from the SCANNER.INI file)
	enum eCacheFileScope { cacheFileNone, cacheFileLocal, cacheFileBoth, cacheFileCentral };

public:
	// constructor
	CAssetCacheFile();

	// derive name and find local and/or remote file
	BOOL Load (LPCSTR pszDataPath, BOOL bAutoName);
	
	// save to local and/or remote data files
	BOOL Save ();
	
	// re-read data from central location in case it's changed
	BOOL Refresh ();
	
	// station file location - see enum above.  If no station file ensure no writes
	void SetStatus (eCacheFileScope status)
		{ _status = status; if (_status == cacheFileNone) m_dwFlags |= IF_NOWRITE; }
	eCacheFileScope GetStatus () const;
	
	// return timestamp of previous audit
	CTime GetLastAuditDate ();
	
	// return asset serial number, however it is stored!
	CString GetSerial ();
	
	// return current Asset name
	CString & GetAssetName ()
		{ return _assetName; }
	void SetAssetName (LPCSTR pszAssetName)
		{ _assetName = pszAssetName; }

	// return other asset name fields
	const CString & GetNetBiosName ()
		{ return _netBiosComputerName; }
	const CString & GetNewAssetName ()
		{ return _newComputerName; }
protected:
	CString GetCACHEFILEName (BOOL bLocal, LPCSTR pszDataPath, LPCSTR pszAssetName);

protected:
	eCacheFileScope	_status;			// see enum above

	// This is the name of the ASSET as known to AuditWizard.  It is initially set to the NetBIOS name
	// when no previous asset cache file can be found and subsequently will be read from the cache file
	CString			_assetName;

	// This is the CURRENT NetBIOS computer name.  
	CString			_netBiosComputerName;

	// This field is only set when we identify that the Asset Name is different to the NetBIOS name and
	// that this change has not yet been signalled.
	CString			_newComputerName;

	// Data Path
	CString			_dataPath;
	CString			_localFile;
};