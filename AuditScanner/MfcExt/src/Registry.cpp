
// FILE:	Registry.cpp
// PURPOSE:	Implementation of classes for obtaining Registry Information
// AUTHOR:	JRF Thornley - copyright (C) InControl Desktop Systems Ltd 2001
// HISTORY:	JRFT - 16.07.2001 - adapted from original C Code
//
//    40C004 - Support REG_EXPAND_SZ as string values in the registry
//
#include "stdafx.h"

#define	HKCU	"HKCU"
#define	HKLM	"HKLM"

#ifndef REG_FORCE_RESTORE
#define REG_FORCE_RESTORE 0x00000008L
#endif 


CString CReg::GetItemStringEx (HKEY hParent, LPCSTR pszSubKey, LPCSTR pszItem, char chSeparator)
{
	CString strResult;

	// any subkey left to open or enumerate ?
	CString strNextBit(pszSubKey);
	if (strNextBit.IsEmpty())
	{
		// No - read item (first get max length for all values within the key)
		DWORD dwLength = 0, dwType;
		if (ERROR_SUCCESS == RegQueryInfoKey (hParent, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, &dwLength, NULL, NULL))
		{
			// ok, allocate a buffer and read the item
			char * pBuffer = strResult.GetBuffer(dwLength + 1);
			if (ERROR_SUCCESS == RegQueryValueEx (hParent, pszItem, NULL, &dwType, (LPBYTE)pBuffer, &dwLength))
			{
				// all is well, but check the type
				if (REG_SZ != dwType && REG_MULTI_SZ != dwType && REG_EXPAND_SZ != dwType)	//40C004
				{
					// NO - wipe it out
					pBuffer[0] = '\0';
				}
			}
			strResult.ReleaseBuffer();
		}
	}
	else
	{
		// else get next part of key to open
		CString strThisBit = BreakString (strNextBit, chSeparator);
		// is it a wildcard?
		if (strThisBit == "?")
		{
			// yes. We need to enumerate then open the first sub-key. First get some info...
			DWORD dwSubKeys, dwMaxSubKeyLength;
			RegQueryInfoKey (hParent, NULL, NULL, NULL, &dwSubKeys, &dwMaxSubKeyLength, NULL, NULL, NULL, NULL, NULL, NULL);
			// loop through all the subkeys
			for (DWORD dw = 0 ; dw < dwSubKeys ; dw++)
			{
				// get the name
				DWORD dwLength = dwMaxSubKeyLength + 1;
				char * pThisBit = strThisBit.GetBuffer(dwLength);
				RegEnumKeyEx (hParent, dw, pThisBit, &dwLength, NULL, NULL, NULL, NULL);
				strThisBit.ReleaseBuffer ();
				// now open it and recurse to see if we find anything?
				HKEY hSubKey;
				if (ERROR_SUCCESS == ::RegOpenKeyEx (hParent, strThisBit, NULL, KEY_READ, &hSubKey))
				{
					strResult = GetItemStringEx (hSubKey, strNextBit, pszItem, chSeparator);
					::RegCloseKey (hSubKey);
					// if we found anything then drop back out
					if (strResult.GetLength())
						break;
					// else keep looping...
				}
			}
		}
		else
		{
			// else just open the next subkey and recurse to read it
			HKEY hSubKey;
			if (ERROR_SUCCESS == ::RegOpenKeyEx (hParent, strThisBit, NULL, KEY_READ, &hSubKey))
			{
				// if it opened then recurse to carry on down the chain
				strResult = GetItemStringEx (hSubKey, strNextBit, pszItem, chSeparator);
				// then clean up as we back out.
				::RegCloseKey (hSubKey);
			}
		}
	}
	return strResult;;
}

/*
** Open and return a registry value as a string - returns empty string if not found
*/
CString CReg::GetItemString (HKEY hKey, LPCSTR pszSubKey, LPCSTR pszItem)
{
	CString strResult;
	HKEY	hSubKey;
	DWORD	dwType, dwLength;

	// try and load the key
	if (ERROR_SUCCESS == RegOpenKeyEx (hKey, pszSubKey, 0, KEY_READ, &hSubKey))
	{
		// opened ok - get a maximum length for its values
		if (ERROR_SUCCESS == RegQueryInfoKey (hSubKey,	// key to query
											  NULL,		// lpClass (not used)
											  NULL,		// class size (not used)
											  NULL,		// reserved
											  NULL,		// count of subkeys
											  NULL,		// maximum subkey name length
											  NULL,		// maximum class length
											  NULL,		// count of values within key
											  NULL,		// maximum value name length
											  &dwLength,// maximum value length
											  NULL,		// security descriptor
											  NULL))	// last write time
		{
			// ok, allocate a buffer and read the item
			char * pBuffer = strResult.GetBuffer(dwLength + 1);
			if (ERROR_SUCCESS == RegQueryValueEx (hSubKey, pszItem, NULL, &dwType, (LPBYTE)pBuffer, &dwLength))
			{
				// all is well, but check the type
				if (REG_SZ != dwType && REG_MULTI_SZ != dwType && REG_EXPAND_SZ != dwType)	//40C004
				{
					// NO - wipe it out
					pBuffer[0] = '\0';
				}
			}
			strResult.ReleaseBuffer();
		}

		RegCloseKey (hSubKey);
	}
	return strResult;
}

/*
** Return a numeric registry value
*/
DWORD CReg::GetItemInt (HKEY hKey, LPCSTR pszSubKey, LPCSTR pszItem)
{
	HKEY	hSubKey;
	DWORD	dwType;
	char	szBuffer[128];
	DWORD	dwSize;

	// try and load the key
	if (ERROR_SUCCESS != RegOpenKeyEx (hKey, pszSubKey, 0, KEY_EXECUTE, &hSubKey))
		return 0L;

	// try and read the item
	dwSize = sizeof(szBuffer);
	if (ERROR_SUCCESS == RegQueryValueEx (hSubKey, pszItem, NULL, &dwType, (LPBYTE)szBuffer, &dwSize))
	{
		// check the type is correct
		if (REG_DWORD != dwType)
		{
			return 0L;
		}
	}

	// close the key
	RegCloseKey (hSubKey);
	return *((LPDWORD)szBuffer);
}



//
//    Return a binary registry value
//
BOOL CReg::GetItemBinary (HKEY hKey, LPCSTR pszSubKey, LPCSTR lpszValueName, CByteArray& return_array)
{
	HKEY	hSubKey;
	ASSERT(lpszValueName != NULL);

	if (NULL == lpszValueName )
		return (FALSE);
	
	// try and load the sub-key
	if (ERROR_SUCCESS != RegOpenKeyEx (hKey, pszSubKey, 0, KEY_EXECUTE, &hSubKey))
		return 0L;

	DWORD dwBufferSize = 1024;
	LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize );

	if (NULL == lpbMemoryBuffer)
		return (FALSE);

	BOOL bReturn = TRUE;

	// Now get the actual value from the registry
	DWORD dwDataType = REG_BINARY;
	int nErrorCode = ::RegQueryValueEx (hSubKey
										,(TCHAR *) lpszValueName
										,NULL
										,&dwDataType
										,lpbMemoryBuffer
										,&dwBufferSize);
	if (ERROR_SUCCESS == nErrorCode)
	{
		return_array.RemoveAll();
		DWORD dwIndex = 0;
		while( dwIndex < dwBufferSize )
		{
			return_array.Add( lpbMemoryBuffer[dwIndex]);
			dwIndex++;
		}
		bReturn = TRUE;
	}
	else
	{
		bReturn = FALSE;
	}

	// close the key
	RegCloseKey (hSubKey);

	// Free the memory buffer allocated
	::free( lpbMemoryBuffer );

	// ...and return the status
	return (bReturn);
}



/*
** Constructor
*/
CReg::CReg()
{
	m_hKey				= NULL;
	m_cSubKeys			= 0;
	m_cValues			= 0;;
	m_dwMaxSubKeyLength	= 0;
	m_dwMaxNameLength	= 0;
	m_dwMaxValueLength	= 0;
}

CReg::CReg(HKEY hOpenKey, LPCSTR pszSubKey)
{
	m_hKey				= NULL;
	m_cSubKeys			= 0;
	m_cValues			= 0;;
	m_dwMaxSubKeyLength	= 0;
	m_dwMaxNameLength	= 0;
	m_dwMaxValueLength	= 0;
	Open(hOpenKey, pszSubKey);
}

/*
** Destructor (closes key if open)
*/
CReg::~CReg()
{
	if (IsOpen())
		Close();
}

/*
** Open a key
*/
BOOL CReg::Open (HKEY hOpenKey, LPCSTR pszSubKey)
{
	// close existing open key
	if (IsOpen())
		Close();

	// try and open the key
	if (ERROR_SUCCESS == RegOpenKeyEx(hOpenKey, pszSubKey, NULL, KEY_READ, &m_hKey))
	{
		// opened ok - get info and store internally
		RegQueryInfoKey (m_hKey, NULL, NULL, NULL, &m_cSubKeys, &m_dwMaxSubKeyLength, NULL, &m_cValues, &m_dwMaxNameLength, &m_dwMaxValueLength, NULL, NULL);
	}
	return IsOpen();
}

/*
** Close current Key
*/
void CReg::Close()
{
	ASSERT(IsOpen());

	RegCloseKey(m_hKey);
	
	m_hKey				= NULL;
	m_cSubKeys			= 0;
	m_cValues			= 0;;
	m_dwMaxSubKeyLength	= 0;
	m_dwMaxNameLength	= 0;
	m_dwMaxValueLength	= 0;
}

/*
** Opens a subkey of the currently opened key, returning a new object
*/
CReg CReg::OpenSubKey (LPCSTR pszSubKey) const
{
	ASSERT(IsOpen());
	return CReg(m_hKey, pszSubKey);
}

/*
** Return the value of a string entry within the open key
*/
CString CReg::ReadValueStr (LPCSTR pszValueName) const
{
	ASSERT(IsOpen());

	CString strResult;
	DWORD dwType, dwLength = m_dwMaxValueLength + 1;

	// pre-allocate the buffer then try and read
	char * pBuffer = strResult.GetBuffer(dwLength);
	if (ERROR_SUCCESS == RegQueryValueEx(m_hKey, pszValueName, NULL, &dwType, (LPBYTE)pBuffer, &dwLength))
	{
		// read ok....but is it really a string?
		if (REG_SZ != dwType && REG_MULTI_SZ != dwType && REG_EXPAND_SZ != dwType)	//40C004
		{
			// no - clear the buffer
			pBuffer[0] = '\0';
		}
	}
	strResult.ReleaseBuffer();
	return strResult;
}

/*
** Return the value of a numeric entry within the open key
*/
DWORD CReg::ReadValueInt (LPCSTR pszValueName) const
{
	ASSERT(IsOpen());

	// allocate memory and read...
	DWORD dwResult = (DWORD)-1, dwType, dwLength = m_dwMaxValueLength + 1;
	LPBYTE pBuffer = new BYTE [dwLength];
	// try and read the item
	if (ERROR_SUCCESS == RegQueryValueEx (m_hKey, pszValueName, NULL, &dwType, pBuffer, &dwLength))
	{
		// check the type is correct
		if (REG_DWORD == dwType)
			dwResult = *((LPDWORD)pBuffer);
	}
	delete [] pBuffer;

	return dwResult;
}

/*
** Populate supplied list with all subkey names within this key
*/
DWORD CReg::EnumSubKeys (CDynaList<CString> & list) const
{
	ASSERT(IsOpen());

	// allocate a temporary buffer for the names
	char * pNameBuffer = new char [m_dwMaxSubKeyLength + 1];
	// simply run through them...
	for (DWORD dwSub = 0 ; dwSub < m_cSubKeys ; dwSub++)
	{
		DWORD dwLength = m_dwMaxSubKeyLength + 1;
		if (ERROR_SUCCESS == RegEnumKeyEx (m_hKey, dwSub, pNameBuffer, &dwLength, NULL, NULL, NULL, NULL))
		{
			list.Add(CString(pNameBuffer));
		}
	}
	delete [] pNameBuffer;
	return list.GetCount();
}

/*
**	Populate supplied list with all value names within the open key
*/
DWORD CReg::EnumValues (CDynaList<CString> & list) const
{
	ASSERT(IsOpen());

	char * pNameBuffer = new char[m_dwMaxNameLength + 1];
	for (DWORD dw = 0 ; dw < m_cValues ; dw++)
	{
		DWORD dwLength = m_dwMaxNameLength + 1;
		if (ERROR_SUCCESS == RegEnumValue (m_hKey, dw, pNameBuffer, &dwLength, NULL, NULL, NULL, NULL))
			list.Add (CString(pNameBuffer));
	}
	delete [] pNameBuffer;
	return list.GetCount();
}

/*
** Search a key and find all unique values for a given item name, either within the key or its descendants
*/
DWORD CReg::EnumNamedStrings (LPCSTR pszValueName, CDynaList<CString> & list, DWORD dwFilter/* = -1*/) const
{
	ASSERT(IsOpen());

	DWORD dwCount = 0;

	// is there a matching string in this key ?
	CString strValue = ReadValueStr(pszValueName);
	if (strValue.GetLength())
	{
		// yes - are we filtering the characteristics ?
		if (-1 == dwFilter || dwFilter == ReadValueInt("Characteristics"))
		{
			// ok, add to list
			list.AddNonMatching(strValue);
			dwCount++;
		}
	}

	// now enumerate all the sub-keys
	CDynaList<CString> subKeys;
	EnumSubKeys(subKeys);
	for (DWORD dwSub = 0 ; dwSub < subKeys.GetCount() ; dwSub++)
	{
		CReg subKey = OpenSubKey(subKeys[dwSub]);
		// recurse to search them
		if (subKey.IsOpen())
			dwCount += subKey.EnumNamedStrings(pszValueName, list);
	}

	return dwCount;
}


//
//    KeyValueExists
//    ==============
//
//    Return whether the specified key value exists - even if it has a null value
//
BOOL CReg::KeyValueExists (HKEY hKey, LPCSTR pszSubKey, LPCSTR/* pszItem*/)
{
	BOOL bResult;
	CString strResult;
	HKEY	hSubKey;

	// try and load the key
	bResult = (ERROR_SUCCESS == RegOpenKeyEx (hKey, pszSubKey, 0, KEY_READ, &hSubKey));
	if (bResult)
		RegCloseKey (hSubKey);
	return bResult;
}



//
//  SaveRegKeyPath
//  ==============
//
//  Save a registry key path to the specified file.  Note that we require privileges 
//  to use this function.  The function will attempt to allocate security tokens to allow
//  use but you do need to be an administrator already.
//
//	Return TRUE if success, otherwise it returns FALSE
//
DWORD CReg::SaveRegKeyPath(CString &Root, CString &SubKey, CString &OutFile)
{
	HKEY	hKey = NULL;
	DWORD	dwError = 0;
	HKEY	hRoot;

	// Give the current process SE_BACKUP_NAME privilege to allow it to do the backup
	SetPrivilege(SE_BACKUP_NAME,TRUE);

	// Get the root
	hRoot = (Root.CompareNoCase(HKCU) == 0) ? HKEY_CURRENT_USER : HKEY_LOCAL_MACHINE;
	if (RegOpenKeyEx(hRoot, SubKey ,0 ,KEY_READ ,&hKey) == ERROR_SUCCESS)  
	{
		// First try and delete the output file as it must not exist
		try { CFile::Remove(OutFile); } catch (CFileException) {}

		// ...then save the registry tree branch
		dwError = RegSaveKey(hKey ,OutFile ,NULL);		
		RegCloseKey(hKey);	
	} 

	else 
	{ 
		if (IsKeyExist(hRoot ,SubKey) == FALSE)
			dwError = ERROR_FILE_NOT_FOUND;

		else 
			dwError = GetLastError();
	}
	
	// Remove the privilege again
	SetPrivilege(SE_BACKUP_NAME ,FALSE);

	return dwError;
}



//
//    RestoreRegKeyPath
//    =================
//
//    Restore registry key SubKey from file InFile
//	  If Force=TRUE then we force the restore operation
//	  Return TRUE if success, otherwise it returns FALSE
//
DWORD CReg::RestoreRegKeyPath (CString &Root, CString &SubKey, CString &InFile, BOOL Force)
{
	HKEY	hKey = NULL;
	DWORD	dwError;
	HKEY	hRoot;
	
	SetPrivilege(SE_RESTORE_NAME,TRUE);
	SetPrivilege(SE_BACKUP_NAME,TRUE);

	// Set the root registry hive to use
	hRoot = (Root.CompareNoCase(HKCU) == 0) ? HKEY_CURRENT_USER : HKEY_LOCAL_MACHINE;

	// If the file does not exist then we cannot restore it
	SECFileSystem fs;
	if (!fs.FileExists(InFile))
	{
		dwError = ERROR_FILE_NOT_FOUND;
	}
	else 
	{
		HKEY	hhKey;
		char	lpClass[80];
		DWORD	lpDisposition = 0;
		if (RegCreateKeyEx(hRoot,SubKey,0,lpClass, REG_OPTION_BACKUP_RESTORE,
							KEY_ALL_ACCESS, NULL, &hhKey, &lpDisposition) == ERROR_SUCCESS)  
		{
			dwError = RegRestoreKey(hhKey ,InFile ,(Force == FALSE) ? REG_NO_LAZY_FLUSH : REG_FORCE_RESTORE);
			RegCloseKey(hhKey);
		} 
		else 
		{
			dwError = -1; 
		}
	}	

	SetPrivilege(SE_RESTORE_NAME,FALSE);
	SetPrivilege(SE_BACKUP_NAME,FALSE);

	return dwError;
}



//
//    SetPrivilege
//    =============
//
//    Internal helper function to give the current user sufficient privileges to be able
//    to save and restore registry paths 
//
//    Inputs:
//		lpszPrivilege - Privilege to act on
//		bEnablePrivilege - True if give privilege, false to remove
//
BOOL CReg::SetPrivilege (LPCTSTR lpszPrivilege, BOOL bEnablePrivilege)
{
	TOKEN_PRIVILEGES tp;
	LUID luid;
	HANDLE hToken; 

	OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken);
	if ( !LookupPrivilegeValue(NULL, lpszPrivilege, &luid) )    
		return FALSE; 
	
	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	
	if (bEnablePrivilege)
		tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	else
	    tp.Privileges[0].Attributes = 0;

	AdjustTokenPrivileges(hToken, FALSE, &tp, 0, (PTOKEN_PRIVILEGES) NULL, 0); 

	return ( (GetLastError()!=ERROR_SUCCESS)?FALSE:TRUE);
}



//	Test if the key KeyPath exist
//	Return TRUE if it exists, otherwise return FALSE
BOOL CReg::IsKeyExist(HKEY hRoot, LPCSTR KeyPath)
{
	HKEY hKey;
	BOOL bReturn;

	if (hRoot == NULL) 
		return FALSE;
	if (KeyPath == NULL || (!lstrlen(KeyPath))) 
		return FALSE;
	
	RegOpenKeyEx (hRoot, KeyPath, 0, KEY_EXECUTE, &hKey);

	if ((RegOpenKeyEx (hRoot, KeyPath, 0, KEY_EXECUTE, &hKey) == ERROR_FILE_NOT_FOUND) 
	||  (RegOpenKeyEx (hRoot, KeyPath, 0, KEY_EXECUTE, &hKey) != ERROR_SUCCESS))
		  bReturn = FALSE;
	else 
		bReturn = TRUE;
	
	RegCloseKey (hKey);

	return bReturn;
}
