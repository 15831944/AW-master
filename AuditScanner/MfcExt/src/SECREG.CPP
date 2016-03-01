// 
// Stingray Software Extension Classes
// Copyright (C) 1995 Stingray Software Inc.
// All Rights Reserved
// 
// This source code is only intended as a supplement to the
// Stingray Extension Class product.
// See the SEC help files for detailed information
// regarding using SEC classes.
// 
//  Author:		AAB
//  Description:	Implementation of SECRegistry
//  Created:		Aug 16 4:01:33 1995
//

#include "stdafx.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

#define new DEBUG_NEW

#if defined(WIN32)
// The following static data members are not accessed
// correctly when compiled as an extension dll.  Please 
// use the corresponding constants defined in WINREG.H 

//HKEY SECRegistry::hKeyLocalMachine = HKEY_LOCAL_MACHINE;
//HKEY SECRegistry::hKeyClassesRoot  = HKEY_CLASSES_ROOT;
//HKEY SECRegistry::hKeyUsers		 = HKEY_USERS;
//HKEY SECRegistry::hKeyCurrentUser  = HKEY_CURRENT_USER;
#endif

//@doc SECRegistry
//@mfunc Constructs a SECRegistry object.
//@xref <c SECRegistry>
SECRegistry::SECRegistry()
{
	// Warning:  Initialize is a virtual method. Calling virtual methods in 
	// constructors is not a good idea if the method is expected to be
	// overridden in a derived class.
	// Rob 1/99

	Initialize();
}
	
//@doc SECRegistry
//@mfunc Destructor.
//@xref <c SECRegistry>
SECRegistry::~SECRegistry()
{
	if ( m_hRegistry != (HKEY) NULL || m_hKey != (HKEY) NULL )
	{
		Close();	
	}

	Initialize();
}

//@doc SECRegistry
//@mfunc Initializes data members.
//@rdesc void 
//@comm This method is only intended to be called in the implementation of
// the SECRegistry class.  Override this method to perform custom 
// initialization.
//@xref <c SECRegistry>
void SECRegistry::Initialize( void )
{
	ASSERT_VALID( this );

	/*
	** Make sure everything is zeroed out
	*/

	m_strClassName.Empty();
	m_strComputerName.Empty();
	m_strKeyName.Empty();
	m_strRegistryName.Empty();

	// default initialization
	m_hRegistry = (HKEY) NULL;

	m_hKey								= (HKEY) NULL;
	m_bRemote							= FALSE;
	m_lErrorCode						= 0L;
	m_dwNumberOfSubkeys					= 0;
	m_bCloseKeyOnDisconnect             = TRUE;

#ifdef WIN32
	m_dwLongestSubkeyNameLength			= 0;
	m_dwLongestClassNameLength			= 0;
	m_dwNumberOfValues					= 0;
	m_dwLongestValueNameLength			= 0;
	m_dwLongestValueDataLength			= 0;
	m_dwSecurityDescriptorLength		= 0;
	m_fileTimeLastWrite.dwLowDateTime	= 0;
	m_fileTimeLastWrite.dwHighDateTime	= 0;
#endif
}

#ifdef WIN32 // WIN32 methods

//@doc SECRegistry
//@mfunc Closes the SECRegistry object connection
//@rdesc Nonzero if successful, otherwise 0.
//@comm Releases the handle of the SECRegistry current key.
// Encapsulates the RegCloseKey API.  This method is available
// in both 16 and 32 bit versions.
//@xref <c SECRegistry>  <mf SECRegistry::Open>
BOOL SECRegistry::Close( void )
{
	ASSERT_VALID( this );

	BOOL bSuccess = TRUE;

	if ( (m_hKey != (HKEY)NULL)  && (m_hKey != m_hRegistry))
	{
		m_lErrorCode = ::RegCloseKey( m_hKey );

		if ( m_lErrorCode != ERROR_SUCCESS )
			bSuccess = FALSE;
	} 
	

	m_hKey = (HKEY)NULL;


	if ( m_hRegistry != (HKEY)NULL )
	{
		if (m_bCloseKeyOnDisconnect) 
		{
			m_lErrorCode = ::RegCloseKey( m_hRegistry );

			if ( m_lErrorCode != ERROR_SUCCESS )
				bSuccess = FALSE;
		}

		m_hRegistry = (HKEY) NULL;
	}

	Initialize();

	return bSuccess;
}
#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Establishes a connection with the registry.
//@rdesc Nonzero if a connection to the key was established, otherwise 0.
//@comm This method is called as the first step in accessing the registry.  
// It connects to an existing key in the registry on the 
// local or remote  computer named by lpszComputerName.  If 
// lpszComputerName is NULL or contains an empty string, 
// the local system is assumed.
//
//Once a connection to the registry is established, use the 
// <mf SECRegistry::Open> method to open keys in the registry.
//@devnote 32-bit only.  Also, if connecting to a remote registry,
// and the name of the remote system cannot be found, this method
// will not return until a time-out occurs.
//@parm  HKEY | hKeyToOpen | The handle of the registry key to connect to.
//@parm  LPCTSTR | lpszComputerName  | Name of the computer to connect to.
//@xref <c SECRegistry> <mf SECRegistry::Open> <mf SecRegistry::Close>
//@ex | SECRegistry reg;
//BOOL rc = reg.Connect(HKEY_LOCAL_MACHINE, "RemoteSystemName");
BOOL SECRegistry::Connect( HKEY hKeyToOpen, LPCTSTR lpszComputerName,BOOL bCloseKeyOnDisconnect)
{
	ASSERT_VALID( this );

	/*
	** lpszComputerName can be NULL
	*/

	if ( m_hRegistry != (HKEY) NULL || m_hKey != (HKEY) NULL )
	{
		Close();	
	}

	BOOL bSaveCloseKey = m_bCloseKeyOnDisconnect;
	m_bCloseKeyOnDisconnect = bCloseKeyOnDisconnect;

	m_bRemote = ((NULL != lpszComputerName) && (_tcslen(lpszComputerName)));

	if ( hKeyToOpen == HKEY_CLASSES_ROOT || hKeyToOpen == HKEY_CURRENT_USER )
	{
		if (m_bRemote) 
		{
			// NT won't allow you to connect to these hives via RegConnectRegistry
			// on a remote system, so we'll just skip that step  

			m_lErrorCode = ERROR_INVALID_HANDLE;
		}
		else
		{
			m_hRegistry = hKeyToOpen;
			m_hKey = m_hRegistry;
			m_lErrorCode = ERROR_SUCCESS;
		}
	}
	else
	{
			m_lErrorCode = ::RegConnectRegistry( (TCHAR *) lpszComputerName, hKeyToOpen, &m_hRegistry );
			m_hKey = m_hRegistry;
	}

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		if (!m_bRemote)
		{
			m_strComputerName = GetComputerName();
		}
		else
		{
			m_strComputerName = lpszComputerName;
		}


		// Derive the root key string
		KeyToStr(hKeyToOpen,m_strRegistryName);

		return( TRUE );
	}
	else
	{
		// if unsuccessful, do not allow state to change
		m_bCloseKeyOnDisconnect = bSaveCloseKey;
		return( FALSE );
	}
}
#else
// This is a replacement version of the Connect method
// for CE that just calls the Open method
BOOL SECRegistry::Connect( HKEY hKeyToOpen, LPCTSTR lpszComputerName,BOOL bCloseKeyOnDisconnect)
{
	ASSERT_VALID( this );
	
	if ( m_hRegistry != (HKEY) NULL || m_hKey != (HKEY) NULL )
	{
		Close();	
	}
	
	BOOL bSaveCloseKey = m_bCloseKeyOnDisconnect;
	m_bCloseKeyOnDisconnect = bCloseKeyOnDisconnect;
	
	m_bRemote = FALSE;
	
	if ( hKeyToOpen == HKEY_CLASSES_ROOT || hKeyToOpen == HKEY_CURRENT_USER )
	{
		m_hRegistry = hKeyToOpen;
		m_hKey = m_hRegistry;
		m_lErrorCode = ERROR_SUCCESS;
	}
	else
	{
		if ((hKeyToOpen == HKEY_LOCAL_MACHINE) || (hKeyToOpen == HKEY_USERS))
		{
			m_lErrorCode = ::RegOpenKeyEx( hKeyToOpen,NULL,0,0, &m_hRegistry );
			m_hKey = m_hRegistry;
		}
		else
		{
			m_lErrorCode = ERROR_INVALID_HANDLE;
		}
	}
	
	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		
		TCHAR szComputerName[ MAX_PATH ] = _T("");
		// CE REIMPL
		// Get the local device name as the computer name
		m_strComputerName = szComputerName;		
		// Derive the root key string
		KeyToStr(hKeyToOpen,m_strRegistryName);
		
		return( TRUE );
	}
	else
	{
		// if unsuccessful, do not allow state to change
		m_bCloseKeyOnDisconnect = bSaveCloseKey;
		return( FALSE );
	}
}
#endif //UNDER_CE (WindowsCE)
//@doc SECRegistry
//@mfunc Creates key in registry.
//@rdesc Nonzero if subkey was successfully created, otherwise 0.
//@syntax Create( LPCTSTR lpszSubkeyName,
// LPCTSTR lpszClassName, CreateOptions	options,
// CreatePermissions permissions, LPSECURITY_ATTRIBUTES	pSecurityAttributes,
// CreationDisposition * pDisposition);
//@syntax Create(LPCTSTR lpszSubkeyName, LPCTSTR lpszClassName);
//@parm LPCTSTR | lpszSubkeyName | (32-bit) Name of the subkey to create.
//@parm LPCTSTR | name_of_subkey | (16-bit) Name of the subkey to create.
//@parm LPCTSTR | lpszClassName | Name of the class to associate 
// with the created key.
//@parm CreateOptions | options | Specifies special options for 
// the created key.  See Comments for more information.
//@parm CreatePermissions | permissions | Specifies access permissions 
// for the key.  See Comments for more information.
//@parm LPSECURITY_ATTRIBUTES | pSecurityAttributes | Points to a 
// SECURITY_ATTRIBUTES structure containing security attributes for 
// the new key.
//@parm CreationDisposition * | pDisposition | Points to a variable to 
// receive disposition information.  See Comments for further information.
//@comm Creates a new registry subkey.  Based on RegCreateKey API for 16-bit, 
// and RegCreateKeyEx for 32-bit.  Possible values for options include :
//@flag REG_OPTION_VOLATILE | This key is volatile; the information is stored 
// in memory and is not preserved when the system is restarted. The 
// RegSaveKey function does not save volatile keys.
//@flag REG_OPTION_NON_VOLATILE | This key is not volatile; the information 
// is stored in a file and is preserved when the system is restarted. 
// The RegSaveKey function saves keys that are not volatile.
//
//Possible values for permissions include:
//
//@flag KEY_ALL_ACCESS | Combination of KEY_QUERY_VALUE, KEY_ENUMERATE_SUB_KEYS, 
// KEY_NOTIFY, KEY_CREATE_SUB_KEY, KEY_CREATE_LINK, and KEY_SET_VALUE access.
//@flag KEY_CREATE_LINK | Permission to create a symbolic link.
//@flag KEY_CREATE_SUB_KEY | Permission to create subkeys.
//@flag KEY_ENUMERATE_SUB_KEYS | Permission to enumerate subkeys.
//@flag KEY_EXECUTE | Permission for read access.
//@flag KEY_NOTIFY | Permission for change notification.
//@flag KEY_QUERY_VALUE | Permission to query subkey data.
//@flag KEY_READ | Combination of KEY_QUERY_VALUE, KEY_ENUMERATE_SUB_KEYS, 
// and KEY_NOTIFY access.
//@flag KEY_SET_VALUE | Permission to set subkey data.
//@flag KEY_WRITE | Combination of KEY_SET_VALUE and KEY_CREATE_SUB_KEY 
// access.
//
//Possible return values for pDisposition include:
//
//@flag REG_CREATED_NEW_KEY | The key did not exist and was created.
//@flag REG_OPENED_EXISTING_KEY | The key existed and was simply opened 
// without being changed.
//@xref <c SECRegistry> <mf SECRegistry::Open> <mf SecRegistry::Close>
BOOL SECRegistry::Create( LPCTSTR				lpszSubkeyName,
						 LPCTSTR				lpszClassName,
						 CreateOptions			options,
						 CreatePermissions		permissions,
						 LPSECURITY_ATTRIBUTES	pSecurityAttributes,
						 CreationDisposition *	pDisposition
						 )
{
	ASSERT_VALID( this );
	ASSERT( lpszSubkeyName != NULL );

	if ( NULL == lpszSubkeyName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD disposition = 0;

	if ( NULL == lpszClassName )
	{
		lpszClassName = _T("");
	}

	CString strSubkey(lpszSubkeyName);

	NormalizeKey(strSubkey);

	// The key handle (can) be equal to the registry handle.
	// If this is the case, closing the key also closes the
	// registry, so we should avoid this.	
	if ( (m_hKey != (HKEY) NULL) && (m_hKey != m_hRegistry) ) {
		m_lErrorCode = ::RegCloseKey(m_hKey);
		ASSERT( ERROR_SUCCESS == m_lErrorCode );
	}

	m_lErrorCode = ::RegCreateKeyEx( m_hRegistry,
					 strSubkey,
					 (DWORD) 0,
					 (TCHAR *) lpszClassName,
					 options,
					 permissions,
					 pSecurityAttributes,
					 &m_hKey,
					 &disposition );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		if ( pDisposition != NULL )
		{
			*pDisposition = (CreationDisposition) disposition;
		}

		m_strKeyName = lpszSubkeyName;

		return( TRUE );
	}
	else
	{
		return( FALSE );
	}
}

//@doc SECRegistry
//@mfunc Deletes the named registry value of the current key.
//@syntax (32-bit) BOOL DeleteKey( LPCTSTR lpszKeyToDelete, BOOL bRecursive);
//@syntax (16-bit) BOOL DeleteKey( LPCTSTR lpszKeyToDelete);
//@rdesc Nonzero if key was deleted, otherwise 0.
//@parm  LPCTSTR | lpszKeyToDelete | Name of the key to delete.
//@parmopt  BOOL | bRecursive | FALSE | Indicates recursive delete
// operation is requested.
//@comm Deletes the named registry key. If bRecursive is set to
// TRUE, it will delete keys in the registry which have subkeys.
//
// The name of the key to delete can be a relative name (to the 
// currently open key), or can be an absolute key name.
//
// Unless the current key was deleted by this operation, the 
// current open key is not affected for this SECRegistry instance.
// If however, the current key was deleted, the current key is 
// set to the parent key of the deleted key.
//
//@devnote The recursive function is not available in the 16 bit
// version.
//@xref <c SECRegistry> <mf SECRegistry::DeleteValue> <mf SECRegistry::Close>
//@end
BOOL SECRegistry::DeleteKey( LPCTSTR lpszKeyToDelete, BOOL bRecursive /* = FALSE */)
{
	ASSERT_VALID( this );
	ASSERT( lpszKeyToDelete != NULL );

	SECRegistry reg;
	BOOL bSubKey = FALSE; // assume the key is an absolute path
	BOOL bSuccess = TRUE; // assume no failure so far

	if (NULL == lpszKeyToDelete)
	{

		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	CString strKeyToDelete(lpszKeyToDelete);

	/*
	** Whoa Nelly! If this is a recursive delete, fail if the 
	** key to delete is a system root key
	*/

	if (bRecursive && (NULL != lpszKeyToDelete) && !(_tcslen(lpszKeyToDelete)))
	{

		if (   (m_hRegistry == HKEY_CLASSES_ROOT)
			|| (m_hRegistry == HKEY_CURRENT_USER)
			|| (m_hRegistry == HKEY_LOCAL_MACHINE)
			|| (m_hRegistry == HKEY_USERS)
#ifndef UNDER_CE
			|| (m_hRegistry == HKEY_PERFORMANCE_DATA)

#if(WINVER >= 0x0400)			
			|| (m_hRegistry == HKEY_CURRENT_CONFIG)
			|| (m_hRegistry == HKEY_DYN_DATA)
#endif /* WINVER >= 0x0400 */
#endif  //UNDER_CE (WindowsCE)

			)
		{

			// A recursive delete from an empty key name will remove 
			// all subkeys from the root key.  This assert is here 
			// as a warning that this condition was present.
			ASSERT(FALSE);

			m_lErrorCode = ERROR_INVALID_PARAMETER;
			return FALSE;
		}

	}

//	if (!reg.Connect(m_hRegistry, m_bRemote?m_strComputerName:(LPCTSTR)NULL, FALSE))
	if (!reg.Connect(StrToKey(m_strRegistryName), m_bRemote?m_strComputerName:(LPCTSTR)NULL, FALSE))
	{
		m_lErrorCode = reg.m_lErrorCode;
		return FALSE;
	}

	/*
	** Can the key be opened relative to the current key?
	*/

	if (strKeyToDelete.Find('\\') != 0) {
		CString strKey = ConcatenateKeys(m_strKeyName, lpszKeyToDelete);

		if (reg.Open(strKey))
			bSubKey = TRUE;
	}

	/*
	** else, try it as an absolute key name...
	*/
	if (!bSubKey) {
		if (!reg.Open(lpszKeyToDelete))
		{
			m_lErrorCode = reg.m_lErrorCode;
			return FALSE;
		}
	}


	if (bRecursive)
	{
		/*
		** Now do a recursive pre-order traversal.
		*/
		CString strEnumKey;
		
		while(reg.EnumerateKeys(0, strEnumKey) && bSuccess) {
			bSuccess = reg.DeleteKey(strEnumKey, TRUE);
			m_lErrorCode = reg.m_lErrorCode;
			}	

	}

	/*
	** Now delete the leaf whether recursive or not...
	*/

	if (bSuccess)
	{

		/*
		** Is the key a full path?
		*/

		if (bSubKey)
		{
			/*
			** User had not given us a full path so assume the name of the key he passed us
			** is a key off of the current key
			*/

			m_lErrorCode = ::RegDeleteKey( m_hKey, lpszKeyToDelete);
		}
		else
		{
			/*
			** You can't delete a key given a full path. What you have to do is back up 
			** one level and then do a delete
			*/
			
			CString strFullKeyName = lpszKeyToDelete;
			CString strParent;
			CString strChild;

			if ( strFullKeyName.Find( '\\' ) == (-1) )
			{
				strParent = "";
				strChild = lpszKeyToDelete;
			}

			else 
			{
				int nLastBackslashLocation = strFullKeyName.GetLength() - 1;

				/*
				** We know this loop will succeed because a back slash was found 
				** in the above if statement
				*/

				while( strFullKeyName[ nLastBackslashLocation ] != '\\' )
				{
					nLastBackslashLocation--;
				}

				CString strOpenKeyName = m_strKeyName;

				strParent = strFullKeyName.Left( nLastBackslashLocation );
				strChild = strFullKeyName.Right( ( strFullKeyName.GetLength() - nLastBackslashLocation ) - 1 );
			}

			/*
			** Now we open the parent key and delete the child
			** (Use the temporary SECRegistry object so as not to
			** change the current key of this object!)
			*/

			if ( reg.Open( strParent ) == TRUE )
			{
				m_lErrorCode = ::RegDeleteKey( reg.m_hKey, strChild );

				/*
				** Now, make sure that the current key is still valid
				** (i.e., that it wasn't a key that was deleted)
				*/

				if (!reg.Open(m_strKeyName)) 
					Open(strParent);

			}
			else
			{
				m_lErrorCode = reg.m_lErrorCode;
				return( FALSE );
			}
		}
	}

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}						

//@doc SECRegistry
//@mfunc Deletes the named registry value of the current key.
//@rdesc Nonzero if value was deleted, otherwise 0.
//@parm  LPCTSTR | lpszValueToDelete  | Name of value to delete.
//@comm Deletes the named registry value of the current key of SECRegistry.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::DeleteKey> <mf SECRegistry::Close>
BOOL SECRegistry::DeleteValue( LPCTSTR lpszValueToDelete )
{
	ASSERT_VALID( this );

	/*
	** lpszValueToDelete can be NULL
	*/

	m_lErrorCode = ::RegDeleteValue( m_hKey, (LPTSTR) lpszValueToDelete );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}


//@doc SECRegistry
//@mfunc Enumerates subkeys of the currently open key.
//@syntax (32-bit) EnumerateKeys(const DWORD dwSubkeyIndex,
// CString& strSubkeyName);
//@syntax (32-bit) EnumerateKeys(const DWORD dwSubkeyIndex,
// CString& strSubkeyName,
// CString& strClassName );
//@syntax (32-bit) EnumerateKeys(const DWORD dwSubkeyIndex,
// LPTSTR lpszSubkeyName,
// LPDWORD lpdwSizeSubkeyName,
// LPTSTR lpszClassName, 
// LPDWORD lpdwSizeClassName);
//@syntax (16-bit) EnumerateKeys( const DWORD dwIndex, 
// LPTSTR lpszBuffer, 
// DWORD dwBufferSize);
//@rdesc Nonzero if subkey was enumerated, otherwise 0.
//@comm The 32-bit version encapsulates RegEnumerateKeys API, 
// the 16-bit version encapsulates RegEnumKey.
//@parm const DWORD | dwSubkeyIndex | Index of desired subkey
//@parm CString& | strSubkeyName | String to receive the
// enumerated key name.
//@parm CString& | strClassName | String to receive the 
// enumerated class name.
//@parm LPTSTR | lpszSubkeyName | Buffer to receive the
// enumerated key name
//@parm LPDWORD | lpdwSizeSubkeyName | The size of the 
// lpszSubkeyName buffer in bytes
//@parm LPTSTR | lpszClassName | Buffer to receive the 
// enumerated class name
//@parm LPDWORD | lpdwSizeClassName | The size of the 
// lpdwSizeClassName buffer in bytes.
//@xref <c SECRegistry> <mf SECRegistry::EnumerateValues>
//@ex | UINT nIndex = 0;
//CString strKeyName;
//while (EnumerateKeys(nIndex++, strKeyName))
//    m_listbox.AddString(strKeyName);
//@end
BOOL SECRegistry::EnumerateKeys(const DWORD dwSubkeyIndex,
								LPTSTR lpszSubkeyName,
								LPDWORD lpdwSizeSubkeyName,
								LPTSTR lpszClassName, /* = NULL */
								LPDWORD lpdwSizeClassName /* = NULL */) 
{
	ASSERT(lpszSubkeyName);
	ASSERT(lpdwSizeSubkeyName);
	// lpszClassName can be NULL
	// lpdwClassNameSize can be NULL

	m_lErrorCode = ::RegEnumKeyEx( m_hKey, 
					dwSubkeyIndex, 
					lpszSubkeyName, 
					lpdwSizeSubkeyName,
					NULL,
					lpszClassName,
					lpdwSizeClassName,
					&m_fileTimeLastWrite );

	return m_lErrorCode;	

}

BOOL SECRegistry::EnumerateKeys(const DWORD dwSubkeyIndex,
								CString& strSubkeyName) 
{
	TCHAR szSubkeyName[ 2048 ];
	DWORD dwSizeSubkeyName = sizeof( szSubkeyName ) - 1;

	LONG lResult;
	
	lResult = EnumerateKeys(dwSubkeyIndex, 
							(LPTSTR)szSubkeyName, &dwSizeSubkeyName, 
							NULL, NULL);

	if ((ERROR_SUCCESS == lResult) || (ERROR_MORE_DATA == lResult))
	{
		strSubkeyName = szSubkeyName;
		return TRUE;
	}

	return FALSE;

}


BOOL SECRegistry::EnumerateKeys(const DWORD dwSubkeyIndex,
								CString& strSubkeyName,
								CString& strClassName )
{
	ASSERT_VALID( this );

	TCHAR szSubkeyName[ 2048 ];
	TCHAR szClassName [ 2048 ];

	DWORD dwSizeSubkeyName = sizeof( szSubkeyName ) - 1;
	DWORD dwSizeClassName  = sizeof( szClassName  ) - 1;

	LONG lResult;

	lResult = EnumerateKeys(dwSubkeyIndex,
							szSubkeyName, &dwSizeSubkeyName,
							szClassName, &dwSizeClassName);

	if ((ERROR_SUCCESS == lResult) || (ERROR_MORE_DATA == lResult))
	{
		strSubkeyName = szSubkeyName;
		strClassName  = szClassName;

		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Enumerates values of the currently open key.
// EnumerateValues( const DWORD dwValueIndex,
// CString&	strValueName,
// KeyValueTypes& type_code );
//@syntax EnumerateValues( const DWORD dwValueIndex,
// CString&	strValueName,
// KeyValueTypes& type_code,
// LPBYTE lpbDataBuffer,
// DWORD& dwSizeDataBuffer );
//@syntax EnumerateValues( const DWORD dwValueIndex,
// CString& strValueName,
// KeyValueTypes* pTypeCode,
// LPBYTE lpbDataBuffer,
// LPDWORD	lpdwSizeDataBuffer);
//@rdesc Nonzero if value was deleted, otherwise 0.
//@parm const DWORD | dwValueIndex | Index of value to query.
//@parm CString& | strValueName | String to receive the 
// enumerated value name.
//@parm KeyValueTypes& | type_code | KeyValueType to receive
// the data type.
//@parm LPBYTE | lpbDataBuffer | A data buffer to receive
// the value data.
//@parm DWORD& | dwSizeDataBuffer | The size of the value
// data buffer in bytes.
//@comm Encapsulates the RegEnumValue() API.
//@devnote For 32-bit only.  Check SECREG.H for optional parameters.
//@xref <c SECRegistry> <mf SECRegistry::EnumerateKeys>
//@end
BOOL SECRegistry::EnumerateValues( const DWORD dwValueIndex,
								  CString&		 strValueName,
								  KeyValueTypes& type_code,
								  LPBYTE			lpbDataBuffer,
								  DWORD&			dwSizeDataBuffer )
{
	return EnumerateValues(	dwValueIndex, strValueName, 
							&type_code, lpbDataBuffer, &dwSizeDataBuffer);
}

BOOL SECRegistry::EnumerateValues( const DWORD dwValueIndex,
					CString&		 strValueName,
					KeyValueTypes& type_code )
{
	return EnumerateValues(	dwValueIndex, strValueName, 
							&type_code, NULL, NULL);
}


BOOL SECRegistry::EnumerateValues( const DWORD dwValueIndex,
					CString&		 strValueName,
					KeyValueTypes* pTypeCode,			/* = NULL */
					LPBYTE			lpbDataBuffer,		/* = NULL */
					LPDWORD		  lpdwSizeDataBuffer	/* = NULL */)
{

	ASSERT_VALID( this );

	/*
	** pTypeCode, lpbDataBuffer and dwSizeDataBuffer can be NULL
	*/

	DWORD dwTempCodeType;

	TCHAR szTempName[ 2048 ];

	DWORD dwTempNameSize = sizeof( szTempName );
	
	m_lErrorCode = ::RegEnumValue( m_hKey,
					dwValueIndex,
					szTempName,
					&dwTempNameSize,
					NULL,
					&dwTempCodeType,
					lpbDataBuffer,
					lpdwSizeDataBuffer );

	
	// LH: check for success or "more data" return codes
	// Note:  This ERROR_MORE_DATA return code is not listed in the 
	//		  Windows documentation for this function.
	if (( ERROR_SUCCESS == m_lErrorCode )  || ( ERROR_MORE_DATA == m_lErrorCode ))
	{
		if (pTypeCode)
		{
			*pTypeCode = (KeyValueTypes) dwTempCodeType;
		}

		strValueName  = szTempName;
		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Writes the attributes of the currently open key.
//@rdesc Nonzero if registry was flushed, otherwise 0.
//@comm  Encapsulates RegFlushKey() API.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::Open> <mf SECRegistry::Close>
BOOL SECRegistry::Flush( void )
{
	ASSERT_VALID( this );

	m_lErrorCode = ::RegFlushKey( m_hKey );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Retrieves a REG_BINARY value.
//@rdesc Nonzero if successful, otherwise 0.
//@parm  LPCTSTR | lpszValueName | The value name.
//@parm  CByteArray& | return_array  | A CByteArray
// to receive the binary data.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::GetValue> <mf SECRegistry::SetBinaryValue>
BOOL SECRegistry::GetBinaryValue( LPCTSTR lpszValueName, CByteArray& return_array )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwBufferSize = m_dwLongestValueDataLength;

	LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize );

	if ( NULL == lpbMemoryBuffer )
	{
		m_lErrorCode = ::GetLastError();
		return( FALSE );
	}

	BOOL bReturn = TRUE;

	KeyValueTypes type = typeBinary;

	if ( QueryValue( lpszValueName, type, lpbMemoryBuffer, dwBufferSize ) == TRUE )
	{
		/*
		** We've got data so give it back to the caller
		*/

		return_array.RemoveAll();

		DWORD dwIndex = 0;

		while( dwIndex < dwBufferSize )
		{
			return_array.Add( lpbMemoryBuffer[ dwIndex ] );
			dwIndex++;
		}

		bReturn = TRUE;
	}
	else
	{
		bReturn = FALSE;
	}

	::free( lpbMemoryBuffer );

	return( bReturn );
}

//@doc SECRegistry
//@mfunc Retrieves a REG_DWORD value (32-bit only).
//@rdesc BOOL 
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName | The value name.
//@parm  DWORD& | dwReturnValue | A DWORD to receive
// the data.
//@xref <c SECRegistry> <mf SECRegistry::GetValue> <mf SECRegistry::SetDoubleWordValue>
BOOL SECRegistry::GetDoubleWordValue( LPCTSTR lpszValueName, DWORD& dwReturnValue )
{
	 ASSERT_VALID( this );
	 ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwBufferSize = sizeof( DWORD );

	KeyValueTypes type = typeDoubleWord;

	return( QueryValue( lpszValueName, type, (LPBYTE) &dwReturnValue, dwBufferSize ) );
}

#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Encapsulates the RegGetSecurity() call.
//@rdesc Nonzero if security information was successfully retrieved, otherwise 0.
//@devnote For 32-bit only.
//@parm  const SECURITY_INFORMATION | security_info | Descriptor structure 
// of what security information is being requested.
//@parm PSECURITY_DESCRIPTOR | lpbDataBuffer | A buffer to receive the
// requested information.
//@parm DWORD& | dwSizeDataBuffer | Size of retrieved buffer.
//@xref <c SECRegistry> <mf SECRegistry::GetValue>
BOOL SECRegistry::GetSecurity( const SECURITY_INFORMATION security_info,
							  PSECURITY_DESCRIPTOR	lpbDataBuffer,
							  DWORD&					dwSizeDataBuffer )
{
	ASSERT_VALID( this );
	ASSERT( lpbDataBuffer != NULL );

	if ( NULL == lpbDataBuffer )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegGetKeySecurity( m_hKey,
										security_info,
										lpbDataBuffer,
										&dwSizeDataBuffer );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}
#endif //UNDER_CE (WindowsCE)
//@doc SECRegistry
//@mfunc Retrieves a REG_SZ value.
//@rdesc Nonzero if successful, otherwise 0.
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName | The name of the value.
//@parm  CString& | strReturn | A CString to receive the string data.
//@xref <c SECRegistry> <mf SECRegistry::GetValue> <mf SECRegistry::SetStringValue>
BOOL SECRegistry::GetStringValue( LPCTSTR lpszValueName, CString& strReturn )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	TCHAR szTemp[ 2048 ];
	DWORD dwBufferSize = 2048;

#if defined(WIN32)
	::ZeroMemory( szTemp, sizeof( szTemp ) );
#endif //win32

	KeyValueTypes type = typeString;

	if ( QueryValue( lpszValueName, type, (LPBYTE) szTemp, dwBufferSize ) == TRUE )
	{
		strReturn = szTemp;
		return( TRUE );
	}
	else
	{
		strReturn.Empty();
		return( FALSE );
	}
}

//@doc SECRegistry
//@mfunc Retrieves a REG_MULTI_SZ value.
//@rdesc Nonzero if successful, otherwise 0.
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName | The value name.
//@parm  CStringArray& | return_array | A CStringArray to receive
// the string data.
//@xref <c SECRegistry> <mf SECRegistry::GetValue> <mf SECRegistry::SetStringArrayValue>
BOOL SECRegistry::GetStringArrayValue( LPCTSTR lpszValueName, CStringArray& return_array )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwBufferSize = m_dwLongestValueDataLength;

	LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize );

	if ( NULL == lpbMemoryBuffer )
	{
		m_lErrorCode = ::GetLastError();
		return( FALSE );
	}

	BOOL bReturn = TRUE;

	KeyValueTypes type = typeMultipleString; // A double NULL terminated string

	if ( QueryValue( lpszValueName, type, lpbMemoryBuffer, dwBufferSize ) == TRUE )
	{
		/*
		** We've got data so give it back to the caller
		*/

		LPTSTR lpszStrings = (LPTSTR) lpbMemoryBuffer;

		return_array.RemoveAll();

		DWORD dwLen = 0;
		while(((dwLen*sizeof(TCHAR)) < (dwBufferSize-sizeof(TCHAR))))
		{
			return_array.Add( (LPCTSTR) lpszStrings );
			int nLen = (int)_tcslen( (LPCTSTR) lpszStrings) + 1;
			lpszStrings += nLen;
			dwLen += nLen;
		}

		bReturn = TRUE;
	}
	else
	{
		bReturn = FALSE;
	}

	::free( lpbMemoryBuffer );

	return( bReturn );
}

//@doc SECRegistry
//@mfunc Retrieves the value of the current key.
//@syntax virtual BOOL GetValue(LPCTSTR lpszValueName, CByteArray& return_array);
//@syntax virtual BOOL GetValue(LPCTSTR lpszValueName, DWORD& dwReturnValue);
//@syntax virtual BOOL GetValue(LPCTSTR lpszValueName, CStringArray& return_array);
//@syntax virtual BOOL GetValue(LPCTSTR lpszValueName, CString& strReturn);
//@rdesc Nonzero if value was successfully retrieved, otherwise 0.
//@comm Retrieves data from named value.  Encapsulates RegQueryValueEx API.
//
//These methods assume the type of data is known when called.  This can
// be determined by either enumerating the data with 
// <mf SECRegistry::EnumerateValues>, or calling <mf SECRegistry::QueryValue>.
//@devnote For 32-bit only.
//@parm LPCTSTR | lpszValueName | The value name
//@parm CByteArray& |return_array | A CByteArray to receive binary data.
//@parm DWORD& | dwReturnValue | A DWORD to receive DWORD data
//@parm CStringArray& | return_array | A CStringArray to receive
// multi-string data.
//@parm CString& | strReturn | A CString to receive string data.
//@xref <c SECRegistry> <mf SECRegistry::Open> <mf SECRegistry::Close>
// <mf SECRegistry::EnumerateValues> <mf SECRegistry::QueryValue>
// <mf SECRegistry::GetBinaryValue> <mf SECRegistry::GetDoubleWordValue>
// <mf SECRegistry::GetStringValue> <mf SECRegistry::GetStringArrayValue>
// <mf SECRegistry::SetValue>
BOOL SECRegistry::GetValue( LPCTSTR lpszValueName, CByteArray& return_array )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( GetBinaryValue( lpszValueName, return_array ) );
}

BOOL SECRegistry::GetValue( LPCTSTR lpszValueName, DWORD& dwReturnValue )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( GetDoubleWordValue( lpszValueName, dwReturnValue ) );
}

BOOL SECRegistry::GetValue( LPCTSTR lpszValueName, CString& strReturn )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( GetStringValue( lpszValueName, strReturn ) );
}

BOOL SECRegistry::GetValue( LPCTSTR lpszValueName, CStringArray& return_array )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( GetStringArrayValue( lpszValueName, return_array ) );
}
#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Encapsulates the RegLoadKey API.
//@rdesc Nonzero if loaded correctly, otherwise 0.
//@parm  LPCTSTR | lpszSubkeyName | Nonzero if loaded correctly, otherwise 0.
//@parm  LPCTSTR | lpszFileName | Name of file that stores the hive of registry info.
//@comm Deletes the named registry value.  Based on the RegLoadKey API.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::Save>
BOOL SECRegistry::Load( LPCTSTR lpszSubkeyName, LPCTSTR lpszFileName )
{
	ASSERT_VALID( this );
	ASSERT( lpszSubkeyName != NULL );
	ASSERT( lpszFileName != NULL );

	if ( NULL == lpszSubkeyName  || NULL == lpszFileName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegLoadKey( m_hRegistry, lpszSubkeyName, lpszFileName );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Indicates when a registry key has changed.  
//@rdesc BOOL 
//@comm Encapsulates RegNotifyChangeKeyValue API to indicate when a registry 
// key has changed.  If bWaitForChange is TRUE, the wait is performed 
// synchronously, and control returns immediately to the program.  
// Otherwise the function will wait for the event to occur.  
// The changes_to_be_reported parameter accepts the same arguments as 
// the fdwNotifyFilter parameter in the RegNotifyChangeKeyValue API, and 
// can include any combination of the following values:
//@flag REG_NOTIFY_CHANGE_NAME | Changes to key names that occur in the 
// specified key or in the specified key and its subkeys cause a change 
// notification. This includes key creations and deletions.
//@flag REG_NOTIFY_CHANGE_ATTRIBUTES | Attribute changes that occur in a 
// key or in a key and its subkeys cause a change notification.
//@flag REG_NOTIFY_CHANGE_LAST_SET | Changes to the last write time that 
// occur in a key or in a key and its subkeys cause a change notification.
//@flag REG_NOTIFY_CHANGE_SECURITY | Security-descriptor changes that occur 
// in a key or in a key and its subkeys cause a change notification.
//@devnote For 32-bit only.
//@parm  const HANDLE | hEvent | Handle of signalled event.
//@parm  const NotifyChangeFilter | reported_changes | Type of change 
// to be reported (see Comments).
//@parm  const BOOL | bAllSubkeys | Boolean flag to indicate whether any 
// change in subkeys should cause notification.
//@parm  const BOOL | bWaitForChange | Specifies how a change should be 
// reported.
//@xref <c SECRegistry>
BOOL SECRegistry::NotifyChange( const HANDLE	hEvent,
							   const NotifyChangeFilter reported_changes,
							   const BOOL		bAllSubkeys,
							   const BOOL		bWaitForChange )
{
	ASSERT_VALID( this );

	m_lErrorCode = ::RegNotifyChangeKeyValue( m_hKey,
					bAllSubkeys,
					reported_changes,
					hEvent,
					bWaitForChange);

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		return( TRUE );
	}
	return( FALSE );
}
#endif //UNDER_CE (WindowsCE)

//@doc SECRegistry
//@mfunc Opens a registry key.
//@syntax (32-bit) BOOL Open( LPCTSTR lpszSubkey, 
// const CreatePermissions security_access_mask );
//@syntax (16-bit) BOOL Open( LPCTSTR lpszSubkey);
//@rdesc Nonzero if key was successfully opened, otherwise 0.
//@parm  LPCTSTR | lpszSubkey | Name of key to open.
//@parm  const CreatePermissions | security_access_mask  | Security flags 
// to associate with the open key (see Comments).
//@comm Opens the specified key with the requested permissions.  The 32-bit 
// version encapsulates the RegOpenKeyEx API, while the 16-bit version 
// encapsulates the RegOpenKey API.  The security_access_mask can be 
// a combination of the following parameters:
//@flag KEY_ALL_ACCESS | Combination of KEY_QUERY_VALUE, KEY_ENUMERATE_SUB_KEYS, 
// KEY_NOTIFY, KEY_CREATE_SUB_KEY, KEY_CREATE_LINK, and KEY_SET_VALUE access.
//@flag KEY_CREATE_LINK | Permission to create a symbolic link.
//@flag KEY_CREATE_SUB_KEY | Permission to create subkeys.
//@flag KEY_ENUMERATE_SUB_KEYS | Permission to enumerate subkeys.
//@flag KEY_EXECUTE | Permission for read access.
//@flag KEY_NOTIFY | Permission for change notification.
//@flag KEY_QUERY_VALUE | Permission to query subkey data.
//@flag KEY_READ | Combination of KEY_QUERY_VALUE, KEY_ENUMERATE_SUB_KEYS, 
// and KEY_NOTIFY access.
//@flag KEY_SET_VALUE | Permission to set subkey data.
//@flag KEY_WRITE | Combination of KEY_SET_VALUE and 
// KEY_CREATE_SUB_KEY access.
//@xref <c SECRegistry> <mf SECRegistry::Close>
BOOL SECRegistry::Open( LPCTSTR lpszSubkey, const CreatePermissions security_access_mask )
{
	ASSERT_VALID( this );

	/*
	** lpszSubkey can be NULL
	*/

	// The registry key member can be null at this point.  If so,
	// use the default registry key (HKEY_CLASSES_ROOT)
	if ((HKEY) NULL == m_hRegistry ) 
		m_hRegistry = HKEY_CLASSES_ROOT;

	// The key handle (can) be equal to the registry handle.
	// If this is the case, closing the key also closes the
	// registry, so we should avoid this.
	if ( (m_hKey != (HKEY) NULL) && (m_hKey != m_hRegistry) ) {
		m_lErrorCode = ::RegCloseKey(m_hKey);
		ASSERT( ERROR_SUCCESS == m_lErrorCode );
	}

	CString strSubkey(lpszSubkey);

	NormalizeKey(strSubkey);

	m_lErrorCode = ::RegOpenKeyEx( m_hRegistry, strSubkey, NULL, security_access_mask, &m_hKey );

	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		QueryInfo();
		m_strKeyName = lpszSubkey;

		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Queries for information about the currently opened key
//@rdesc Nonzero if successful, othewise 0.
//@comm Queries for information about the currently opened key.  
// The following public members are updated to reflect the query:
//
//m_strClassName				class name<nl>
//m_dwNumberOfSubkeys			number of subkeys<nl>
//m_dwLongestSubkeyName			length of longest subkey name<nl>
//m_dwLongestClassNameLength	length of longest subkey class name<nl>
//m_dwNumberOfValues			number of subkey values<nl>
//m_dwLongestValueNameLength	length of longest value name<nl>
//m_dwLongestValueDataLength	longest value data length<nl>
//m_dwSecurityDescription		security descriptor length<nl>
//m_fileTimeLastWrite			last write time of buffer<nl>
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::QueryValue>
BOOL SECRegistry::QueryInfo( void )
{
	ASSERT_VALID( this );

	TCHAR szClassName[ 2048 ];

	DWORD dwSizeClassName = sizeof( szClassName ) - 1;
#ifndef UNDER_CE
#if defined(WIN32) 
	::ZeroMemory( szClassName, sizeof( szClassName ) );
#endif
#endif //UNDER_CE (WindowsCE)		
	m_lErrorCode = ::RegQueryInfoKey( m_hKey,
					szClassName,
					&dwSizeClassName,
					(LPDWORD) NULL,
					&m_dwNumberOfSubkeys,
					&m_dwLongestSubkeyNameLength,
					&m_dwLongestClassNameLength,
					&m_dwNumberOfValues,
					&m_dwLongestValueNameLength,
					&m_dwLongestValueDataLength,
					&m_dwSecurityDescriptorLength,
					&m_fileTimeLastWrite );
	
	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		m_strClassName = szClassName;
		m_timeLastWrite = CTime( m_fileTimeLastWrite );
		 
		return( TRUE );
	}
	return( FALSE );
}


//@doc SECRegistry
//@mfunc Encapsulates the RegQueryValueEx API.
//@syntax (32-bit) BOOL QueryValue( LPCTSTR	lpszValueName,
// KeyValueTypes& value_type,
// LPBYTE lpbBuffer,
// DWORD& dwBufferSize );
//@syntax (16-bit) BOOL QueryValue(  LPCTSTR lpszSubkeyName,
// SECPBYTE lpbBuffer,
// LONG& lBufferSize );
//@rdesc Nonzero if value was successfully queried, otherwise 0.
//@comm Queries the value of the currently opened registry key and copies 
// the value data into the buffer.  Encapsulates the RegQueryValue API.
//@parm LPCTSTR | lpszValueName | Name of value to query.
//@parm LPCTSTR | lpszSubkeyName | name of subkey to query.
//@parm KeyValueTypes& | value_type | Buffer to hold value type.
//@parm LPBYTE | lpbBuffer | Buffer to hold value data.
//@parm DWORD&  | dwBufferSize | Size of buffer pointed to by lpbBuffer; 
//@parm DWORD&  | dwBufferSize | Size of buffer pointed to by lpbBuffer; 
// on return contains size of data actually copied to buffer.
//@parm LONG& | lBufferSize | Size of buffer pointed to by lpbBuffer; 
// on return contains size of data actually copied to buffer.
//@xref <c SECRegistry> <mf SECRegistry::QueryInfo>
BOOL SECRegistry::QueryValue( LPCTSTR	lpszValueName,
							 KeyValueTypes&	value_type,
							 LPBYTE			lpbBuffer,
							 DWORD&			dwBufferSize )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	/*
	** lpbBuffer and dwBufferSize can be NULL
	*/

	if ( NULL == lpszValueName )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwTempDataType = value_type;

	m_lErrorCode = ::RegQueryValueEx( m_hKey,
					(TCHAR *) lpszValueName,
					NULL,
					&dwTempDataType,
					lpbBuffer,
					&dwBufferSize );


	if ( ERROR_SUCCESS == m_lErrorCode )
	{
		value_type = (KeyValueTypes) dwTempDataType;
		return( TRUE );
	}
	return( FALSE );
}
#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Encapsulates the RegReplaceKey API.
//@rdesc Nonzero if value was successfully replaced, otherwise 0.
//@parm LPCTSTR | lpszSubkeyName | Name of subkey to replace.
//@parm     LPCTSTR lpszNewFile Name of the file storing registration information.
//@parm     LPCTSTR lpszBackupFile Name of the file to store a backup of the key being 
//@comm Replaces the subkey data with the information contained in the lpszNewFile.  
// Encapsulates the RegReplaceKey API.	
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::Load> <mf SECRegistry::UnLoad>
// <mf SECRegistry::Restore>
BOOL SECRegistry::Replace( LPCTSTR lpszSubkeyName,
						  LPCTSTR lpszNewFile,
						  LPCTSTR lpszBackupFile )
{
	ASSERT_VALID( this );
	ASSERT( lpszSubkeyName != NULL );
	ASSERT( lpszNewFile != NULL );
	ASSERT( lpszBackupFile != NULL );

	if (  NULL == lpszSubkeyName  ||
		NULL == lpszNewFile  ||
		NULL == lpszBackupFile  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegReplaceKey( m_hKey, 
					lpszSubkeyName,
					lpszNewFile,
					lpszBackupFile );

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}

//@doc SECRegistry
//@mfunc Encapsulates the RegRestoreKey API.
//@rdesc Nonzero if key was successfully restored, otherwise 0.
//@parm  LPCTSTR | lpszSavedTreeFile | Name of file with registry information.
//@parm  const DWORD | dwVolatilityFlags | Specifies volatility of the key.
//@comm Overwrites the current registry key with the information contained in the saved tree file.  
// Encapsulates the RegRestoreKey API.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::Load> <mf SECRegistry::Replace>
BOOL SECRegistry::Restore( LPCTSTR lpszSavedTreeFile, const DWORD dwVolatilityFlags )
{
	ASSERT_VALID( this );
	ASSERT( lpszSavedTreeFile != NULL );

	if (  NULL == lpszSavedTreeFile  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegRestoreKey( m_hKey,
					lpszSavedTreeFile,
					dwVolatilityFlags );
	
	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}	
	return( FALSE );
}


//@doc SECRegistry
//@mfunc Encapsulates the RegSaveKey API.
//@rdesc Nonzero if key was successfully saved, otherwise 0.
//@comm  Saves the registration information for the current key.  
// Encapsulates the RegSaveKey API. 
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszDestFile | Specifies the file where 
// registration information associated with the current key will be stored.
//@parm  LPSECURITY_ATTRIBUTES | pSecurityAttributes  | Points to a 
// SECURITY_ATTRIBUTES structure for the new file.
//@xref <c SECRegistry> <mf SECRegistry::Load> <mf SECRegistry::Replace>
// <mf SECRegistry::Restore>
BOOL SECRegistry::Save( LPCTSTR lpszDestFile, LPSECURITY_ATTRIBUTES pSecurityAttributes )
{
	ASSERT_VALID( this );
	ASSERT( lpszDestFile != NULL );

	if (  NULL == lpszDestFile  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegSaveKey( m_hKey, lpszDestFile, pSecurityAttributes );

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}
#endif //UNDER_CE (WindowsCE)
//@doc SECRegistry
//@mfunc Sets a value for the key name specified as REG_BINARY.
//@rdesc Nonzero if successful, otherwise 0.
//@parm  LPCTSTR | lpszValueName |  The value name.
//@parm  const CByteArray& | bytes_to_write | The binary data to set.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::SetValue> <mf SECRegistry::GetBinaryValue>
BOOL SECRegistry::SetBinaryValue( LPCTSTR lpszValueName, const CByteArray& bytes_to_write )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwBufferSize = (DWORD)bytes_to_write.GetSize();

	LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize );

	if (  NULL == lpbMemoryBuffer  )
	{
		m_lErrorCode = ::GetLastError();
		return( FALSE );
	}

	DWORD dwIndex = 0;

	while( dwIndex < dwBufferSize )
	{
		lpbMemoryBuffer[ dwIndex ] = bytes_to_write[ dwIndex ];
		dwIndex++;
	}

	BOOL bReturn = SetValue( lpszValueName, typeBinary, lpbMemoryBuffer, dwBufferSize );

	::free( lpbMemoryBuffer );

	return( bReturn );
}

//@doc SECRegistry
//@mfunc Sets a value for the key name specified as REG_DWORD.
//@rdesc Nonzero if successful, otherwise 0.
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName | The value name.
//@parm  DWORD | dwValue  | The value to set.
//@xref <c SECRegistry> <mf SECRegistry::SetValue> <mf SECRegistry::GetDoubleWordValue>
BOOL SECRegistry::SetDoubleWordValue( LPCTSTR lpszValueName, DWORD dwValue )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetValue( lpszValueName, typeDoubleWord, (const PBYTE) &dwValue, sizeof( DWORD ) ) );
}
#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Encapsulates the RegSetKeySecurity API.
//@rdesc Nonzero if security was successfully set, otherwise 0.
//@parm  const SECURITY_INFORMATION& | SecurityInformation | Descriptor structure 
// of what security information is being set
//@parm const PSECURITY_DESCRIPTOR  | pSecurityDescriptor  | Points to the 
// security attributes being set for the currently open key.
//@comm Sets the security for the currently open registry key.  
// Encapsulates the RegSetKeySecurity API.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::SetValue>
BOOL SECRegistry::SetSecurity( const SECURITY_INFORMATION& SecurityInformation,
							  const PSECURITY_DESCRIPTOR  pSecurityDescriptor )
{
	ASSERT_VALID( this );
	ASSERT( pSecurityDescriptor != NULL );

	if (  NULL == pSecurityDescriptor  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegSetKeySecurity( m_hKey, SecurityInformation, pSecurityDescriptor );

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		 return( TRUE );
	}
	return( FALSE );
}
#endif //UNDER_CE (WindowsCE)

//@doc SECRegistry
//@mfunc Sets a value for the key name specified as REG_SZ (32-bit only).
//@rdesc Nonzero if successful, otherwise 0.
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName |  The value name.
//@parm  const CString& | string_value | The string data to set.
//@xref <c SECRegistry> <mf SECRegistry::SetValue> <mf SECRegistry::SetStringArrayValue>
// <mf SECRegistry::GetStringValue>
BOOL SECRegistry::SetStringValue( LPCTSTR lpszValueName, const CString& string_value )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetValue( lpszValueName, typeString, (const PBYTE) (const TCHAR *) string_value, string_value.GetLength()*sizeof(TCHAR) + 1 ) );
}

//@doc SECRegistry
//@mfunc Sets a value for the key name specified as REG_MULTI_SZ.
//@rdesc Nonzero if successful, otherwise 0.
//@devnote For 32-bit only.
//@parm  LPCTSTR | lpszValueName |  The value name.
//@parm  const CStringArray& | string_array | The string data to set.
//@xref <c SECRegistry> <mf SECRegistry::SetValue> <mf SECRegistry::SetStringValue>
// <mf SECRegistry::GetStringArrayValue>
BOOL SECRegistry::SetStringArrayValue( LPCTSTR lpszValueName, const CStringArray& string_array )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	DWORD dwBufferSize = 0;

	/*
	** Find out how big our buffer needs to be...
	*/

	int nIndex		= 0;
	int nNumStrings = (int)string_array.GetSize();

	while( nIndex < nNumStrings )
	{
		 dwBufferSize += string_array[nIndex].GetLength() + 1;
		 nIndex++;
	}

	/*
	** Don't forget the second NULL needed for double null terminated strings...
	*/
	dwBufferSize++;

	dwBufferSize *= sizeof(TCHAR);

	LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize  );

	if (  NULL == lpbMemoryBuffer  )
	{
		m_lErrorCode = ::GetLastError();
		return( FALSE );
	}
#ifndef UNDER_CE
	::ZeroMemory( lpbMemoryBuffer, dwBufferSize );
#endif //UNDER_CE (WindowsCE)

	/*
	** OK, now add the strings to the memory buffer
	*/

	LPTSTR lpszString = (LPTSTR) lpbMemoryBuffer;

	nIndex				 = 0;
	int nStringLength  = 0;

	while( nIndex < nNumStrings )
	{
		 _tcscpy( &lpszString[ nStringLength ], string_array[nIndex] );
		 nStringLength += string_array[nIndex].GetLength() + 1;

		nIndex++;
	}

	nStringLength++;

	BOOL bReturn = TRUE;

	KeyValueTypes type = typeMultipleString; // A double NULL terminated string

	if ( SetValue( lpszValueName, type, lpbMemoryBuffer, dwBufferSize ) != TRUE )
	{
		bReturn = FALSE;
	}

	::free( lpbMemoryBuffer );

	return( bReturn );
}

//@doc SECRegistry
//@mfunc Sets a value for the key name specified.
//@syntax (32-bit) virtual BOOL SetValue(LPCTSTR lpszValueName, const CByteArray& bytes_to_write);
//@syntax (32-bit) virtual BOOL SetValue(LPCTSTR lpszValueName, DWORD dwValue);
//@syntax (32-bit) virtual BOOL SetValue(LPCTSTR lpszValueName, const CStringArray& strings_to_write);
//@syntax (32-bit) virtual BOOL SetValue(LPCTSTR lpszValueName, const CString& strWrite);
//@syntax (32-bit) virtual BOOL SetValue(LPCTSTR lpszValueName, const KeyValueTypes type_of_value_to_set, LPBYTE lpbValueData, const DWORD dwSize);
//@syntax (16-bit) virtual BOOL SetValue(LPCTSTR lpSubKey, const CString& strWrite);
//@rdesc Nonzero if value was successfully set, otherwise 0.
//@parm LPCTSTR | lpszValueName | Name of value to set (32-bit).
//@parm LPCTSTR | lpSubKey | Name of the subkey to set value for )16-bit).
//@parm const CByteArray& | bytes_to_write | Array of bytes to write as binary type.
//@parm DWORD | dwValue | DWORD value to write.
//@parm const CStringArray& | strings_to_write | Array of strings to write.
//@parm const CString&  | strWrite | Single string to write.
//@parm const KeyValueTypes | type_of_value_to_set | Type of value to be set.  
// Can be one of the following:
//@flag typeBinary |
//@flag typeDoubleWord |
//@flag typeDoubleWordLittleEndian |
//@flag typeDoubleWordBigEndian | 
//@flag typeUnexpandedString |
//@flag typeSymbolicLink |
//@flag typeMultipleString |
//@flag typeNone |
//@flag typeResourceList |
//@flag typeString |
//@parm LPBYTE  | lpbValueData | Pointer to buffer of data to be set.
//@parm const DWORD | dwSize | Size of buffer pointed to by lpbBuffer.
//@comm Sets the named value with the data value passed in.  The 32-bit version
// encapsulates the RegSetValueEx API, 16-bit encapsulates the RegSetValue API.
//@xref <c SECRegistry> <mf SECRegistry::GetValue>
BOOL SECRegistry::SetValue( LPCTSTR lpszValueName, const CByteArray& bytes_to_write )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetBinaryValue( lpszValueName, bytes_to_write ) );
}

BOOL SECRegistry::SetValue( LPCTSTR lpszValueName, DWORD dwValue )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetDoubleWordValue( lpszValueName, dwValue ) );
}

BOOL SECRegistry::SetValue( LPCTSTR lpszValueName, const CStringArray& strings_to_write )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetStringArrayValue( lpszValueName, strings_to_write ) );
}

BOOL SECRegistry::SetValue( LPCTSTR lpszValueName, const CString& strWrite )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );

	if (  NULL == lpszValueName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	return( SetStringValue( lpszValueName, strWrite ) );
}

BOOL SECRegistry::SetValue( LPCTSTR		lpszValueName, 
				 const KeyValueTypes	type_of_value_to_set, 
				 LPBYTE					lpbValueData, 
				 const DWORD			dwSize )
{
	ASSERT_VALID( this );
	ASSERT( lpszValueName != NULL );
	ASSERT( lpbValueData != NULL );

	if (  NULL == lpszValueName  ||  NULL == lpbValueData  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegSetValueEx( m_hKey,
									lpszValueName,
									0,
									type_of_value_to_set,
									lpbValueData,
									dwSize );
	
	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}
#ifndef UNDER_CE
//@doc SECRegistry
//@mfunc Encapsulates the RegUnLoadKey API.
//@rdesc Nonzero if key was successfully unloaded, otherwise 0.
//@parm  LPCTSTR lpszSubkey 
//@comm Unloads the specified key and its subkeys from the registry.  
// Encapsulates the RegUnLoadKey API.
//@devnote For 32-bit only.
//@xref <c SECRegistry> <mf SECRegistry::Load>
BOOL SECRegistry::UnLoad( LPCTSTR lpszSubkey )
{
	ASSERT_VALID( this );
	ASSERT( lpszSubkey != NULL );

	if (  NULL == lpszSubkey  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	m_lErrorCode = ::RegUnLoadKey( m_hKey, lpszSubkey );

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}
#endif //UNDER_CE (WindowsCE)
// Utility routine to convert a string to the HKEY_ equiv.
//@doc SECRegistry
//@mfunc Converts an HKEY constant to its string equivalent.
//@rdesc Nonzero if successful, otherwise 0.
//@parm HKEY | hKey | HKEY handle to convert.
//@parm CString& | strKey | String to receive conversion
//@comm This method maps the standard HKEY values to strings
//
//The keys this method recognizes (and their mapped strings) include:<nl>
//HKEY_CLASSES_ROOT		"HKEY_CLASSES_ROOT"<nl>
//HKEY_CURRENT_USER		"HKEY_CURRENT_USER"<nl>
//HKEY_LOCAL_MACHINE	"HKEY_LOCAL_MACHINE"<nl>
//HKEY_USERS			"HKEY_USERS"<nl>
//HKEY_CURRENT_CONFIG	"HKEY_CURRENT_CONFIG"<nl>
//HKEY_DYN_DATA			"HKEY_DYN_DATA"<nl>
//
//If a the key cannot be matched, the string reference
// is not altered.
//@xref <c SECRegistry> <mf SECRegistry::StrToKey>
BOOL SECRegistry::KeyToStr(HKEY hKey,CString& strKey) {

	// assume failure
	BOOL rc = FALSE;

	if ( HKEY_CLASSES_ROOT == hKey ) {
		strKey = _T("HKEY_CLASSES_ROOT");
		rc = TRUE;
	}
	else if ( HKEY_CURRENT_USER  == hKey ) {
		strKey = _T("HKEY_CURRENT_USER");
		rc = TRUE;
	}
	else if ( HKEY_LOCAL_MACHINE == hKey ) {
		strKey = _T("HKEY_LOCAL_MACHINE");
		rc = TRUE;
	}
	else if ( HKEY_USERS == hKey ) {
		strKey = _T("HKEY_USERS");
		rc = TRUE;
	}
#ifndef UNDER_CE
	else if ( HKEY_CURRENT_CONFIG  == hKey ) {
		strKey = _T("HKEY_CURRENT_CONFIG");
		rc = TRUE;
	}
	else if ( HKEY_DYN_DATA  == hKey ) {
		strKey = _T("HKEY_DYN_DATA");
		rc = TRUE;
	}
#endif //UNDER_CE (WindowsCE)

	// if no match was found, do not modify string,
	// simply return false

	return rc;
}
	
//@doc SECRegistry
//@mfunc Converts an string equivalent to its HKEY constant.
//@rdesc The matching HKEY successful, otherwise 0.
//@parm CString& | strKey | String to receive conversion
//@comm This method maps the standard strings to HKEY values.
// See <mf SECRegistry::KeyToStr> for recognized strings.
//@xref <c SECRegistry> <mf SECRegistry::KeyToStr>
HKEY SECRegistry::StrToKey(const CString& strKey){
	HKEY hKey=NULL;
	if(strKey==_T("HKEY_CLASSES_ROOT"))
		hKey=HKEY_CLASSES_ROOT;
	else if(strKey==_T("HKEY_CURRENT_USER"))
		hKey=HKEY_CURRENT_USER;
	else if(strKey==_T("HKEY_LOCAL_MACHINE"))
		hKey=HKEY_LOCAL_MACHINE;
	else if(strKey==_T("HKEY_USERS"))
		hKey=HKEY_USERS;
#ifndef UNDER_CE
	else if(strKey==_T("HKEY_CURRENT_CONFIG"))
		hKey=HKEY_CURRENT_CONFIG;
	else if(strKey==_T("HKEY_DYN_DATA"))
		hKey=HKEY_DYN_DATA;
#endif //UNDER_CE (WindowsCE)
		
	return hKey;		
}

#else // WIN16 routines

BOOL SECRegistry::Close( void )
{
	ASSERT_VALID( this );

	BOOL bSuccess = TRUE;

	if ( m_hKey != (HKEY) NULL )
	{
		m_lErrorCode = ::RegCloseKey( m_hKey );

		if ( m_lErrorCode != ERROR_SUCCESS )
			bSuccess = FALSE;

		// if the key handle is equal to the registry handle, 
		// let's null out the registry handle so that we 
		// don't try to close the same handle twice below
		// (thus avoiding a return code of SUCCESS_INVALID_HANDLE)
		if (m_hKey == m_hRegistry) 
			m_hRegistry = (HKEY) NULL;

		m_hKey = (HKEY) NULL;

	}

	if ( m_hRegistry != (HKEY) NULL )
	{
		if (m_bCloseKeyOnDisconnect)
		{
			m_lErrorCode = ::RegCloseKey( m_hRegistry );

			if ( m_lErrorCode != ERROR_SUCCESS )
				bSuccess = FALSE;
		}

		m_hRegistry = (HKEY) NULL;
	}

	Initialize();

	return bSuccess;
}

BOOL SECRegistry::Create( LPCTSTR lpszSubkeyName, 
				LPCTSTR lpszClassName
			  )
{
	ASSERT_VALID( this );
	ASSERT( lpszSubkeyName != NULL );

	if (  NULL == lpszSubkeyName  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}


	if (  NULL == lpszClassName  )
	{
		lpszClassName = "";
	}
	
	// The key handle (can) be equal to the registry handle.
	// If this is the case, closing the key also closes the
	// registry, so we should avoid this.
	if ( (m_hKey != (HKEY) NULL) && (m_hKey != m_hRegistry) ) {
		m_lErrorCode = ::RegCloseKey(m_hKey);
		ASSERT(  ERROR_SUCCESS == m_lErrorCode  );
	}

	m_lErrorCode = ::RegCreateKey( m_hRegistry,
					lpszSubkeyName,
					&m_hKey);
	
	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		m_strKeyName = lpszSubkeyName;		 
		return( TRUE );
	}
	return( FALSE );
}

BOOL SECRegistry::DeleteKey( LPCTSTR lpszKeyToDelete)
{
	ASSERT_VALID( this );
	ASSERT( lpszKeyToDelete != NULL );

	if (  NULL == lpszKeyToDelete  )
	{
		m_lErrorCode = ERROR_INVALID_PARAMETER;
		return( FALSE );
	}

	/*
	** You can't delete a key given a full path. What you have to do is back up one level and then do a delete
	*/

	CString strFullKeyName = lpszKeyToDelete;

	if ( strFullKeyName.Find( '\\' ) == (-1) )
	{
		/*
		** User had not given us a full path so assume the name of the key he passed us
		** is a key off of the current key
		*/

		m_lErrorCode = ::RegDeleteKey( m_hKey, lpszKeyToDelete);
	}
	else
	{
		int nLastBackslashLocation = strFullKeyName.GetLength() - 1;

		/*
		** We know this loop will succeed because a back slash was found in the above if statement
		*/

		while( strFullKeyName[ nLastBackslashLocation ] != '\\' )
		{
			nLastBackslashLocation--;
		}

		CString strOpenKeyName = m_strKeyName;

		CString strParent = strFullKeyName.Left( nLastBackslashLocation );
		CString strChild  = strFullKeyName.Right( ( strFullKeyName.GetLength() - nLastBackslashLocation ) - 1 );

		/*
		** Now we open the parent key and delete the child
		*/

		if ( Open( strParent ) == TRUE )
		{
			m_lErrorCode = ::RegDeleteKey( m_hKey, strChild );
		}
		else
		{
			m_strKeyName = strOpenKeyName;
			return( FALSE );
		}
	}

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}						

BOOL SECRegistry::EnumerateKeys( const DWORD dwIndex, LPTSTR lpszBuffer, DWORD dwBufferSize)
{
	ASSERT_VALID( this );

	m_lErrorCode = ::RegEnumKey( m_hKey, 
					dwIndex,
					lpszBuffer,
					dwBufferSize);	

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
} 

//@doc SECRegistry
//@mfunc Determines the number of subkeys in the curently open key.
//@rdesc Nonzero if the key was enumerated, otherwise 0.
//@parm  LPCTSTR | lpszBuffer | The key to be enumerated.
//@parm  UINT | &nKeys | The number of subkeys result.
//@devnote For 16-bit only.
//@xref <c SECRegistry> <mf SECRegistry::EnumerateKeys>
BOOL SECRegistry::GetSubkeys( LPCTSTR lpszBuffer, UINT &nKeys)
{
	// Determine number of subkeys  
	char temp_buf[1000]; 
	HKEY hKeyHolder = m_hKey;
	nKeys = 0;

	Open(lpszBuffer);
	while (EnumerateKeys((DWORD)nKeys++, temp_buf, 1000));
	
	// Close the enumerator and reset the original key
	Close();
	m_hKey = hKeyHolder;
	
	// Make sure the count is accurate since the last EnumerateKeys call has to fail
	nKeys--; 
	return TRUE;
}



BOOL SECRegistry::Open( LPCTSTR lpszSubkey)
{
	ASSERT_VALID( this );

	/*
	** lpszSubkey can be NULL
	*/

	// The key handle (can) be equal to the registry handle.
	// If this is the case, closing the key also closes the
	// registry, so we should avoid this.
	if ( (m_hKey != (HKEY) NULL) && (m_hKey != m_hRegistry) ) {
		m_lErrorCode = ::RegCloseKey(m_hKey);
		ASSERT( m_lErrorCode == ERROR_SUCCESS );
	}

	m_lErrorCode = ::RegOpenKey( m_hRegistry, lpszSubkey, &m_hKey );

	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		m_strKeyName = lpszSubkey;
		return( TRUE );
	}
	return( FALSE );
}

BOOL SECRegistry::QueryValue(  LPCTSTR lpszSubkeyName,
					 SECPBYTE	lpbBuffer,
					 LONG&		lBufferSize )
{
	ASSERT_VALID(this); 
	LONG lCheck;

	m_lErrorCode = ::RegQueryValue(m_hKey, lpszSubkeyName, lpbBuffer, &lCheck);
	lBufferSize = lCheck;
		 
	if ( ERROR_SUCCESS == m_lErrorCode)
	{
		return ( TRUE );
	}
	return ( FALSE );
}

BOOL SECRegistry::SetValue( LPCTSTR				 lpszSubkeyName, 
				 const CString& strWrite )
{
	ASSERT_VALID( this );
	ASSERT( strWrite != NULL );

	m_lErrorCode = ::RegSetValue( m_hKey,
				 lpszSubkeyName,
				 REG_SZ, // must be REG_SZ for Win3.1
				 strWrite,
				 0);
	if (  ERROR_SUCCESS == m_lErrorCode  )
	{
		return( TRUE );
	}
	return( FALSE );
}

#endif


// Normalizes a keyname
void SECRegistry::NormalizeKey(CString& strKey, BOOL bSubkey) {

	// the convention is to remove any backslashes from the end
	// if it is a subkey, also remove it from the beginning if present

	if (strKey.GetLength() == 0) {
		return;
	}
	else {

		int nLen = strKey.GetLength();

		// is there a backslash at the end?
		if (strKey.ReverseFind('\\') == (nLen-1))
			// remove it
			strKey = strKey.Mid(0,nLen-1);

		nLen = strKey.GetLength();

		// is there a backslash at the beginning?
		if (strKey.Find('\\') == 0) {
			// if this is a subkey, remove it
			if (bSubkey)
				strKey = strKey.Mid(1);
		}

	}


}

// Concatenates a key with a subkey
CString SECRegistry::ConcatenateKeys (LPCTSTR szKey, LPCTSTR szSubkey) {

	CString strKey(szKey);
	CString strSubkey(szSubkey);

	NormalizeKey(strKey);
	NormalizeKey(strSubkey, TRUE);

	return strKey + "\\" + strSubkey;
}