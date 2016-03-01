#pragma once
/*
** Class for obtaining registry information
*/
class CReg
{
public:
	// static functions for quick access
	static CString GetItemString (HKEY hKey, LPCSTR pszSubKey, LPCSTR pszItem);
	// this one can find a key where part of the address is variable (indicated by a ?)
	static CString GetItemStringEx (HKEY hParent, LPCSTR pszSubKey, LPCSTR pszItem, char chSeparator = '\\');
	// an overloaded version to grab integer values
	static DWORD GetItemInt (HKEY hKey, LPCSTR pszSubKey, LPCSTR pszItem);
	// recover a binary value
	static BOOL  GetItemBinary (HKEY hKey, LPCSTR pszSubKey, LPCSTR lpszValueName, CByteArray& return_array);

	// Check if the specified registry key value exists
	static BOOL KeyValueExists (HKEY hKey, LPCSTR pszSubKey, LPCSTR pszItem);
public:
	// constructor
	CReg();
	CReg(HKEY hOpenKey, LPCSTR pszSubKey);
	// destructor (closes key if open)
	~CReg();
	// open a key
	BOOL Open (HKEY hOpenKey, LPCSTR pszSubKey);
	// returns TRUE if key has been opened
	BOOL IsOpen () const	
		{ return (NULL != m_hKey); }
	// close current open key
	void Close();
	// open a subkey and return a new object
	CReg OpenSubKey (LPCSTR pszSubKey) const;
	// read a value from the current open key
	CString ReadValueStr (LPCSTR pszValueName) const;
	// read a value from the current open key
	DWORD ReadValueInt (LPCSTR pszValueName) const;
	// populate supplied list with all subkey names within this key
	DWORD EnumSubKeys (CDynaList<CString> & list) const;
	// build a list of all the named values under the open key
	DWORD EnumValues (CDynaList<CString> & list) const;
	// search the key and all children for a specific value name
	DWORD EnumNamedStrings (LPCSTR pszValueName, CDynaList<CString> & list, DWORD dwFlags = -1) const;

	// Save and restore a registry tree branch
	DWORD SaveRegKeyPath	(CString &Root, CString &SubKey, CString &OutFile);
	DWORD RestoreRegKeyPath (CString &Root, CString &SubKey, CString &InFile, BOOL Force);

protected:
	BOOL SetPrivilege	(LPCTSTR lpszPrivilege, BOOL bEnablePrivilege);
	BOOL IsKeyExist		(HKEY hRoot, LPCSTR KeyPath);

protected:
	HKEY	m_hKey;
	DWORD	m_cSubKeys;
	DWORD	m_cValues;
	DWORD	m_dwMaxSubKeyLength;
	DWORD	m_dwMaxNameLength;
	DWORD	m_dwMaxValueLength;
};
