#pragma once

/*
** Utility class to enumerate PC drives by type
*/
class CDriveList : public CDynaList<CString>
{
public:
	enum { fixed = 1, removable = 2, network = 4, cdrom = 8 };
public:
	CDriveList (int nTypesToList)
	{
		char szBuffer[1024];
		GetLogicalDriveStrings (sizeof(szBuffer), szBuffer);

		for (LPSTR p = szBuffer ; *p ; p += strlen(p) + 1)
		{
			switch (GetDriveType(p))
			{
				case DRIVE_REMOVABLE:
					if (0 == (nTypesToList & removable))
						continue;
					break;
				case DRIVE_FIXED:
					if (0 == (nTypesToList & fixed))
						continue;
					break;
				case DRIVE_REMOTE:
					if (0 == (nTypesToList & network))
						continue;
					break;
				case DRIVE_CDROM:
					if (0 == (nTypesToList & cdrom))
						continue;
					break;
				default:
					continue;	// unknown drive type
			}
			// add to internal list
			Add(p);
		}
	}
};





//
//	Collected data record for a single file
//
class CFileSystemFile
{
public:
	CFileSystemFile ()
	{
		m_strName = "";
		m_dwSize = 0;
		m_strCompanyName = "";
		m_strProductName = "";
		m_strDescription = "";
		m_strProductVersion1 = "";
		m_strProductVersion2 = "";
		m_strFileVersion1 = "";
		m_strFileVersion2 = "";
		m_strOriginalFileName = "";
		m_strModifiedDateTime = "";
		m_strCreatedDateTime = "";
		m_strLastAccessedDateTime = "";
	}

public:
	const CString & Name()
		{ return m_strName; }
	void Name (LPCSTR value)
	{ m_strName = value; m_strName.Trim();  }
	
	//	
	DWORD Size ()
	{ return m_dwSize; }
	void	Size (DWORD value)
	{ m_dwSize = value; }
	
	//
	CString& ModifiedDateTime ()
		{ return m_strModifiedDateTime; }
	void ModifiedDateTime (CString& value)
		{ value.Trim(); m_strModifiedDateTime = value; }
	//
	CString& CreatedDateTime ()
		{ return m_strCreatedDateTime; }
	void CreatedDateTime (CString& value)
		{ value.Trim(); m_strCreatedDateTime = value; }
	//
	CString& LastAccessedDateTime ()
		{ return m_strLastAccessedDateTime; }
	void LastAccessedDateTime (CString& value)
		{ value.Trim(); m_strLastAccessedDateTime = value; }
	//
	const CString & CompanyName ()
		{ return m_strCompanyName; }
	void CompanyName (CString& value)
		{ value.Trim(); m_strCompanyName = value; }
	//
	const CString & ProductVersion1 ()
		{ return m_strProductVersion1; }
	void ProductVersion1 (CString& value)
		{ value.Trim(); m_strProductVersion1 = value; }
	//
	const CString & ProductVersion2 ()
		{ return m_strProductVersion2; }
	void ProductVersion2 (CString& value)
		{ value.Trim(); m_strProductVersion2 = value; }
	//
	const CString & FileVersion1 ()
		{ return m_strFileVersion1; }
	void FileVersion1 (CString& value)
		{ value.Trim(); m_strFileVersion1 = value; }
	//
	const CString & FileVersion2 ()
		{ return m_strFileVersion2; }
	void FileVersion2 (CString& value)
		{ value.Trim(); m_strFileVersion2 = value; }
	//
	const CString & Description()
		{ return m_strDescription; }
	void Description (CString& value)
		{ value.Trim(); m_strDescription = value; }
	//
	const CString & ProductName()
		{ return m_strProductName; }
	void ProductName (CString& value)
		{ value.Trim(); m_strProductName = value; }
	//
	const CString & OriginalFileName()
		{ return m_strOriginalFileName; }
	void OriginalFileName (CString& value)
		{ value.Trim(); m_strOriginalFileName = value; }

	// comparison
	BOOL operator== (CFileSystemFile const & other) const
	{
		return (m_dwSize == other.m_dwSize						&&
			m_strName == other.m_strName						&&
			m_strModifiedDateTime == other.m_strModifiedDateTime	&&
			m_strCreatedDateTime == other.m_strCreatedDateTime	&&
			m_strCompanyName == other.m_strCompanyName			&&
			m_strProductName == other.m_strProductName			&&
			m_strProductVersion1 == other.m_strProductVersion1	&&
			m_strProductVersion2 == other.m_strProductVersion2	&&
			m_strFileVersion1 == other.m_strFileVersion1		&&
			m_strFileVersion2 == other.m_strFileVersion2		&&
			m_strDescription == other.m_strDescription			&&
			m_strOriginalFileName == other.m_strOriginalFileName &&
			m_strLastAccessedDateTime == other.m_strLastAccessedDateTime);
	}

	// conversion for easy storage
	void FromString (LPCSTR pszBuffer);
	CString ToString ();
	
protected:
	CString		m_strName;
	DWORD		m_dwSize;
	CString		m_strCreatedDateTime;
	CString		m_strModifiedDateTime;
	CString		m_strLastAccessedDateTime;

	CString		m_strCompanyName;
	CString		m_strProductName;
	CString		m_strDescription;
	CString		m_strProductVersion1;
	CString		m_strProductVersion2;
	CString		m_strFileVersion1;
	CString		m_strFileVersion2;
	CString		m_strOriginalFileName;
};





//
//    CFileSystemFolder
//    ================
//
//    This class represents a single folder scanned during the audit process.  Each folder consists
//    of the folder name and 0 or more CSwFile objects
//
class CFileSystemFolder
{
	friend class CFileSystemScanner;

public:
	CFileSystemFolder ()
	{
		_name = "";
		_listFiles.Empty();
	}
	
	CFileSystemFolder (LPCSTR name)
	{
		_name = name;
	}
	
	// Destructor needs to delete all of the folders and files in the list
	~CFileSystemFolder()
	{
		for (int index=0; index < (int)_listFolders.GetCount(); index++)
		{
			delete _listFolders[index];
		}
		_listFolders.Empty();

		for (int index=0; index < (int)_listFiles.GetCount(); index++)
		{
			delete _listFiles[index];
		}
		_listFiles.Empty();
	}
	
	// return the full name of this folder
	CString&	Name(void)
	{ return _name; }
	void		Name (CString& value)
	{ _name = value; }
	
		
	// return the last portion of this folder name
	CString		ShortName()
	{
		int delimiter = _name.ReverseFind('\\');
		if (delimiter == -1)
			return _name;
		else
			return _name.Mid(delimiter + 1);
	}


	// return the list of folders audited
	CDynaList<CFileSystemFolder*>& ListFolders()
	{ return _listFolders; }

	// return the list of Files audited
	CDynaList<CFileSystemFile*>& ListFiles()
	{ return _listFiles; }
	
	// Add a Folder to the list
	void	AddFolder(CFileSystemFolder* pFolder)
	{
		_listFolders.Add(pFolder);
	}
	
	// Add a file to the list
	void	AddFile(CFileSystemFile* pFile)
	{
		_listFiles.Add(pFile);
	}
	
	BOOL	HasFiles();

protected:

private:
	CDynaList<CFileSystemFolder*>	_listFolders;
	CDynaList<CFileSystemFile*>		_listFiles;
	CString	_name;
};




//
//    CFileSystemFolderList
//    =====================
//
//    This class encompasses a list of folders audited.
//
class CFileSystemFolderList
{
public:
	CFileSystemFolderList()
	{
		_listAuditedFolders.Empty();
	}
	
	// Destructor
	virtual ~CFileSystemFolderList();

public:
	// Add a new folder (or return a pointer to an existing one)
	CFileSystemFolder*	AddFolder (LPCSTR strFolder);

	// return the list of folders audited
	CDynaList<CFileSystemFolder*>& ListAuditedFolders()
	{ return _listAuditedFolders; }

	void	AddFolder (CFileSystemFolder* pFolder)
	{	_listAuditedFolders.Add(pFolder); }
	
private:
	CDynaList<CFileSystemFolder*> _listAuditedFolders;
};




//
//    CFileSystemScanner
//    ==================
//
//    This is the primary file for actually performing the scan of the file system
//
class CFileSystemScanner
{
public:
	// constructor
	CFileSystemScanner()
		{}
	// destructor
	~CFileSystemScanner()
		{}
		
	// set scanning options 
	void SetOptions (int scanFolders ,int scanFiles ,CDynaList<CString>& listFoldersToAudit ,CDynaList<CString>& listFilesToAudit);

	// Initiate the scan	
	bool Scan		(CListBox* pProgressListBox);

	// Save results to the audit data file
	bool Save		(CAuditDataFile* pAuditDataFile);

protected:	
	
	// returns true if options set above require any scans to run
	BOOL IsEnabled ()
		{ return (m_nScanFolders != 0 || m_bCapture); }
	
	// count drives to scan
	DWORD EnumDrives (CDynaList<CString> & drives);
	
	// count total size of files in a drive
	DWORD CountFileSizes (LPCSTR pszPath);
	
	// scan a drive
	BOOL ScanDrive (LPCSTR pszDrive);

	// collect individual file's details
	BOOL ScanFile (CFileSystemFolder* pSwFolder, LPCSTR pszFullName, LPWIN32_FIND_DATA pFD);
	static BOOL ScanFile (LPCSTR pszFullPath, LPWIN32_FIND_DATA pFD, CFileSystemFile* pSwFileInfo);

protected:
	// Scan ALL folders
	void ScanAllFolders();
	
	// Scan selected folders
	void ScanSelectedFolders();

	// scans a specific path
	BOOL ScanPath (CFileSystemFolder* pFolder);

	// returns true if a folder should be included in the scan
	BOOL IncludeFolder (LPCSTR pszFullName);

	// returns true if a file should be included in the scan
	BOOL IncludeFile (LPCSTR pszName);

	// collects a files details and stores to database
	BOOL ScanFile (DWORD dwFolderID, LPCSTR pszPath, LPWIN32_FIND_DATA pFd);

	// returns an ascii file description, used for calculating CRCs
	static CString GetFileDesc (LPCSTR pszFile ,BOOL includeVersion);

	void LogProgress(LPCSTR message);

protected:
	// Results of the scan are held in the CFileSystemFolderList object
	CFileSystemFolderList	_listAuditedFolders;
	
	// Folders / Files that are to be audited
	CDynaList<CString> _listFoldersToAudit;
	CDynaList<CString> _listFilesToAudit;
	
	// settings
	int		m_nScanFolders;
	int		m_nScanFiles;
	BOOL	m_bCapture;
	BOOL	m_bCollectCRC;
	CListBox* _pProgressListBox;
};