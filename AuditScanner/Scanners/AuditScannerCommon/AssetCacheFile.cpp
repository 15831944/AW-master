//////////////////////////////////////////////////////////////////////////
//
//    CAssetCacheFile
//    ===============
//
//    Thie file holds cached information about an audited PC
//
//////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "AssetCacheFile.h"

CAssetCacheFile::CAssetCacheFile ()
{
	// Assume that we will be using both cache files
	_status = cacheFileBoth;
}



//
//    Load
//    ====
//
//    Reads existing data from local or central data file
//
BOOL CAssetCacheFile::Load (LPCSTR pszDataPath, BOOL bAutoName)
{
	//CLogFile	log;
	//CString		strBuffer;
	//BOOL		bLocalFile = FALSE, bCentralFile= FALSE;

	//_dataPath = pszDataPath;
	//CString	netBiosComputerName("");

	//// auto-naming ?
	//if (bAutoName)
	//{
	//	// Before we load any existing data we default the name of the asset to the NetBIOS name
	//	char szBuffer[MAX_COMPUTERNAME_LENGTH + 1];
	//	DWORD dwLen = sizeof(szBuffer);
	//	::GetComputerName(szBuffer, &dwLen);
	//	netBiosComputerName = szBuffer;
	//	_assetName = netBiosComputerName;

	//	log.Format("Loading asset cache file - asset name set as %s\n" ,_assetName);
	//}

	//// do we want to try and read it locally ?
	//if (_status == cacheFileLocal || _status == cacheFileBoth)
	//{
	//	// yes, look for cache file on the first available HDD
	//	for (char chDrv = 'A' ; chDrv <= 'Z' ; chDrv++)
	//	{
	//		// is this a fixed drive ?
	//		char szDrive[4];
	//		wsprintf (szDrive, "%c:\\", chDrv);
	//		if (DRIVE_FIXED == GetDriveType(szDrive))
	//		{
	//			// yes - build file name
	//			_localFile.Format("%s%s", szDrive, CACHEFILE_NAME);
	//			// ..and try to open it
	//			log.Format ("Reading local cache file %s... \n", _localFile);
	//			if (CIniFile::errorOK == CIniFile::Read(_localFile))
	//			{
	//				// success!
	//				log.Format("read OK\n");
	//	
	//				// ...and update the asset name - may be used for remote file
	//				SetSection("Asset");
	//				_assetName = GetString("Asset Name", _assetName);
	//				bLocalFile = TRUE;
	//			}
	//			else
	//			{
	//				log.Format("failed\n");
	//			}
	//			break;
	//		}
	//	}
	//}

	//// now do we want to read a remote file?
	//if (_status == cacheFileBoth || _status == cacheFileCentral)
	//{
	//	// one way or another we MUST have an asset name to proceed
	//	if (_assetName.GetLength())
	//	{
	//		strBuffer.Format("%s\\%s.awc", pszDataPath, _assetName);
	//		log.Format("Reading central asset cache file %s...\n", strBuffer);
	//		if (CIniFile::errorOK == CIniFile::Read(strBuffer))
	//		{
	//			log.Format("read OK\n");

	//			// also check for presence of a different asset name
	//			SetSection("Asset");
	//			CString strNewName = GetString("Asset Name");
	//			if (strNewName.GetLength())
	//				_assetName = strNewName;
	//			bCentralFile = TRUE;
	//		}
	//		else
	//		{
	//			log.Format("failed\n");
	//		}
	//	}
	//}

	//// finally, we need to check for a computer name change, if auto-name is enabled
	//if (bAutoName)
	//{
	//	// Check the recovered asset name against stored computer name
	//	SetSection("Asset");
	//	CString strOldCompName = GetString("NetBIOS Name");
	//	if (strOldCompName.GetLength() && (strOldCompName != netBiosComputerName))
	//		// ok, write this as a REQUEST to the manager for a name change
	//		_newComputerName = netBiosComputerName;
	//	
	//	// Set the NetBIOS name to be the same as the asset name
	//	_netBiosComputerName = netBiosComputerName;
	//}


	//return (bLocalFile || bCentralFile);

	return false;
}



//    
//    Save
//    ====
//
//    Save data back to one or both station files
//
BOOL CAssetCacheFile::Save ()
{
	BOOL bLocal = TRUE, bCentral = TRUE;

	// local save
	if (_status == cacheFileLocal || _status == cacheFileBoth)
	{
		m_strFileName = _localFile;
		if (0 != Write())
			bLocal = FALSE;
	}

	// central save
	if (_status == cacheFileCentral || _status == cacheFileBoth)
	{
		m_strFileName.Format("%s\\%s.awc", _dataPath, _assetName);
		if (0 != Write())
			bCentral = FALSE;
	}
	return (bLocal && bCentral);
}



//
//    Refresh
//    =======
//
//    Check for latest remote update
//
BOOL CAssetCacheFile::Refresh ()
{
	CString strFileToRead;

	// is there a remote file ?
	switch (_status)
	{
		case cacheFileCentral:
		case cacheFileBoth:
			strFileToRead.Format("%s\\%s.awc", _dataPath, _assetName);
			break;

		case cacheFileLocal:
			strFileToRead = _localFile;
			break;

		default:
			return FALSE;
	}
	return (errorOK == Read(strFileToRead));
}



CString CAssetCacheFile::GetSerial ()
{
	// This is a special case - if user has stored one under asset data then prefer that to the normal one
	SetSection("User Data");
	if (GetString("Serial Number").GetLength())
	{
		return GetString("Serial Number");
	}
	else
	{
		// none found so look under asset as above
		SetSection("Asset");
		return GetString("Serial");				//40C012
	}
}

CTime CAssetCacheFile::GetLastAuditDate ()
{
	CTime timResult(0);

	SetSection("Asset");
	CString strBuffer = GetString("Date");
	if (!strBuffer.IsEmpty())
	{
		LPCSTR p = (LPCSTR)strBuffer;
		int nYear	= atoi(p);
		int nMonth	= atoi(p + 5);
		int nDay	= atoi(p + 8);
		int nHour	= atoi(p + 11);
		int nMinute = atoi(p + 14);
		int nSecond = atoi(p + 17);
		timResult = CTime(nYear,nMonth,nDay,nHour,nMinute,nSecond);
	}
	return timResult;
}
	
