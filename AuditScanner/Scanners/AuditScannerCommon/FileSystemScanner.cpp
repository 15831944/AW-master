///////////////////////////////////////////////////////////////////////////////
//
//    CFileSystemScanner
//    ==================
//
//    The Main HARDWARE Scanning Class
//
#include "Stdafx.h"
#include "AuditDataFile.h"
#include "FileSystemScanner.h"
#include "ModuleVersion.h"

//
// Helper class for storing collected file data
//
void CFileSystemFile::FromString (LPCSTR pszBuffer)
{
	CString strBuffer(pszBuffer), strTemp;
	m_strName				= BreakString (strBuffer, ';', TRUE);
	m_strCompanyName		= BreakString (strBuffer, ';', TRUE);
	m_strDescription		= BreakString (strBuffer, ';', TRUE);
	m_strProductName		= BreakString (strBuffer, ';', TRUE);
	m_strProductVersion1	= BreakString (strBuffer, ';', TRUE);
	m_strProductVersion2	= BreakString (strBuffer, ';', TRUE);
	m_strFileVersion1		= BreakString (strBuffer, ';', TRUE);
	m_strFileVersion2		= BreakString (strBuffer, ';', TRUE);
	m_strOriginalFileName	= BreakString (strBuffer, ';', TRUE);
	//
	m_dwSize				= atol(BreakString (strBuffer, ';', TRUE));
	m_strCreatedDateTime	= BreakString (strBuffer, ';', TRUE);
	m_strModifiedDateTime	= BreakString (strBuffer, ';', TRUE);
	m_strLastAccessedDateTime= BreakString (strBuffer, ';', TRUE);
}


CString CFileSystemFile::ToString ()
{
	CString strBuffer;
	strBuffer.Format("%s;%s;%s;%s;%s;%s;%s;%s;%s;%d;%s;%s;%s"
					,m_strName
					,m_strCompanyName
					,m_strDescription
					,m_strProductName
					,m_strProductVersion1
					,m_strProductVersion2
					,m_strFileVersion1
					,m_strFileVersion2
					,m_strOriginalFileName
					,m_dwSize
					,m_strCreatedDateTime
					,m_strModifiedDateTime
					,m_strLastAccessedDateTime);
	return strBuffer;
}



//
//    CFileSystemFolder Class
//    =======================
//
//
//    This function iterates through this folder and all of its children and simply returns Yes/No as to
//    whether there are any child files beneath this folder
//
BOOL CFileSystemFolder::HasFiles()
{
	if (_listFiles.GetCount() != 0)
		return TRUE;
		
	// No files at this level so check each child folder
	for (int index=0; index<(int)_listFolders.GetCount(); index++)
	{
		CFileSystemFolder* pFolder = _listFolders[index];
		if (pFolder->HasFiles())
			return TRUE;
	}
	
	return FALSE;
}



//
//    CFileSystemFolderList Class
//    ===========================
//
//    Destructor needs to delete all of the folders in the list
//
CFileSystemFolderList::~CFileSystemFolderList()
{
	for (int index=0; index<(int)_listAuditedFolders.GetCount(); index++)
	{
		delete _listAuditedFolders[index];
	}
	_listAuditedFolders.Empty();
}



//
//    AddFolder
//    =========
//
//    Add a folder to the list
//
CFileSystemFolder*	CFileSystemFolderList::AddFolder (LPCSTR strFolder)
{
	// Search the existing list of folders to see if we can find a match and return that
	for (int index = 0; index<(int)_listAuditedFolders.GetCount(); index++)
	{
		CFileSystemFolder* pFolder = _listAuditedFolders[index];
		if (pFolder->Name() == strFolder)
			return pFolder;
	}
	
	// No existing folder so create a new one and return that
	CFileSystemFolder* pNewFolder = new CFileSystemFolder(strFolder);
	_listAuditedFolders.Add(pNewFolder);
	return pNewFolder;
}




//
//    CFileSystemScanner Class
//    ========================
//
void CFileSystemScanner::SetOptions (int scanFolders ,int scanFiles ,CDynaList<CString>& listFoldersToAudit ,CDynaList<CString>& listFilesToAudit)
{
	m_nScanFolders = scanFolders;
	m_nScanFiles = scanFiles;
	_listFoldersToAudit = listFoldersToAudit;
	_listFilesToAudit = listFilesToAudit;
}


bool CFileSystemScanner::Scan(CListBox* pProgressListBox)
{
	CLogFile log;

	try
	{
		_pProgressListBox = pProgressListBox;
		
		// OK first rule - are we scanning the entire disk or just specific folders?
		if (m_nScanFolders == 0)
		{
			log.Write("scanning no folders");
			return true;
		}
		else if (m_nScanFolders == 1)
		{
			log.Write("scanning all folder");
			ScanAllFolders();
		}
		else if (m_nScanFolders == 2)
		{
			log.Write("scanning specified folders");
			ScanSelectedFolders();
		}
		else
		{
			log.Write("error found, unexpected value for m_nScanFolders (%d)", m_nScanFolders);
			return false;
		}
	}
	catch (CException *pEx)
	{
		return false;
	}

	return true;
}
	
	
//
//    ScanAllFolders
//    ==============
//
//    Scan the entire hard disk
//
void CFileSystemScanner::ScanAllFolders()
{
	CString message;
	
	// Recover a list of the drives which we are to scan
	CDynaList<CString>	listDrives;
	DWORD dwDrives = EnumDrives(listDrives);
	
	// ...and scan each
	for (DWORD dw = 0 ; dw < dwDrives ; dw++)
	{
		message.Format("scanning drive %s, please wait as this may take some time...", listDrives[dw]);
		LogProgress(message);

		// scan the drive
		if (!ScanDrive(listDrives[dw]))
			break;
	}
	
	LogProgress("Auditing of the local hard disk is complete");
}	



	
//
//    ScanSelectedFolders
//    ===================
//
//    Scan the folders specified (and their children)
//
void CFileSystemScanner::ScanSelectedFolders()
{
	CLogFile log;

	try
	{
		CString message;
		
		for (int index = 0; index < (int)_listFoldersToAudit.GetCount(); index++)
		{
			// get the folder name
			CString strFolder = _listFoldersToAudit[index];
			message.Format("Scanning folder %s...", strFolder);
			LogProgress(message);
			
			// Create an entry in the Folderslist for this folder
			CFileSystemFolder* pFolder = new CFileSystemFolder(strFolder);
			_listAuditedFolders.AddFolder(pFolder);
			
			// ...and scan it
			ScanPath(pFolder);

			LogProgress("Auditing of the specified folder(s) is complete");
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}	
}	



// Save the results of this audit to the data file
bool CFileSystemScanner::Save (CAuditDataFile* pAuditDataFile)
{
	CLogFile log;

	log.Write("CFileSystemScanner::Save in");
	pAuditDataFile->SetFileSystemList(&_listAuditedFolders);
	log.Write("CFileSystemScanner::Save out");
	return true;
}


//
// Generate a list of valid files to scan
//
DWORD CFileSystemScanner::EnumDrives (CDynaList<CString> & drives)
{
	char szBuffer[1024];
	GetLogicalDriveStrings (sizeof(szBuffer), szBuffer);

	for (LPSTR p = szBuffer ; *p ; p += strlen(p) + 1)
	{
		UINT nType = GetDriveType(p);
		if (nType == DRIVE_FIXED)
		{
			CString strDrive(p);
			drives.Add(strDrive.Left(2));
		}
	}
	return drives.GetCount();
}


BOOL CFileSystemScanner::ScanDrive (LPCSTR pszDrive)
{
	CFileSystemFolder* pFolder = new CFileSystemFolder(pszDrive);
	_listAuditedFolders.AddFolder(pFolder);
		
	// ...and scan it
	return ScanPath(pFolder);
}


//
//   Recursive fn to collect results for a folder
//
BOOL CFileSystemScanner::ScanPath (CFileSystemFolder* pParentFolder)
{
	CLogFile log;
	CString strFolderToScan = pParentFolder->Name();

	log.Write("", true);
	log.Format("Scanning folder %s\n" ,strFolderToScan);

	try
	{	
		// search for subdirectories
		char szBuffer[_MAX_PATH];
		strcpy (szBuffer, strFolderToScan);
		strcat (szBuffer, "\\*.*");
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile (szBuffer, &fd);
		if (INVALID_HANDLE_VALUE != hFind) 
		{
			do 
			{
				// make full name
				CString strFullPath;
				strFullPath.Format ("%s\\%s", strFolderToScan, fd.cFileName);
				
				// is it a subdirectory ?
				if (fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) 
				{
					// yes - ignore the "." and ".." entries
					if ( strcmp(fd.cFileName, ".") && strcmp(fd.cFileName, "..")) 
					{
						// Create a new folder and add to our parent
						CFileSystemFolder* pFolder = new CFileSystemFolder(strFullPath);
						pParentFolder->AddFolder(pFolder);			
						
						// recurse to scan that path also
						ScanPath(pFolder);
					}
				} 
				else 
				{
					// its a file - do we scan it ?
					if (IncludeFile(fd.cFileName))
						ScanFile(pParentFolder, strFullPath, &fd);
				}
			} while (FindNextFile (hFind, &fd));
			FindClose(hFind);
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	
	return TRUE;
}



//
//   returns true if a file should be included in the scan
//
BOOL CFileSystemScanner::IncludeFile (LPCSTR pszName)
{
	switch (m_nScanFiles)
	{
		case 0:
			// zero means scan no files
			return FALSE;
			break;

		case 1:
			// 1 means scan all EXECUTABLE files - note that this is treated as a list of possible executable file extensions
			{
				CString strExtExe = pszName;
				CString strExtExeConst = ".exe";

				char * str = new char[strExtExe.GetLength()+1];
				strcpy(str, strExtExe);

				char * end = new char[strExtExeConst.GetLength()+1];
				strcpy(end, strExtExeConst);

				size_t str_len = strlen(str),
				end_len = strlen(end);
				char *str_end = str + str_len - end_len;
				return strcmp(str_end, end) == 0;

				break;	
			}

		case 2:
			// 2 means scan specified list of files
			if (_listFilesToAudit.GetCount())
			{
				// split the file into name and extension
				CString strExt(pszName);
				CString strName = BreakString(strExt, '.', TRUE);

				// now run through all the "possibles"
				for (DWORD dw = 0 ; dw < _listFilesToAudit.GetCount() ; dw++)
				{
					CString strTestExt(_listFilesToAudit[dw]);
					CString strTestName = BreakString(strTestExt, '.', TRUE);
					
					// does it match both name and extension
					if (MatchString(strTestName, strName) &&  MatchString(strTestExt, strExt))
						return TRUE;
				}
			}
			break;

		case 3:
			// 4 means scan all files. This should only be enabled for partial path scanning...
			return TRUE;
			break;

		default:
			break;
	}
	return FALSE;
}





//
// Collects info about a file
//
BOOL CFileSystemScanner::ScanFile (CFileSystemFolder* pParentFolder, LPCSTR pszFullName, LPWIN32_FIND_DATA pFD)
{
	CLogFile log;

	char szPath[_MAX_FNAME], szDrive[_MAX_DRIVE], szDir[_MAX_DIR];
	_splitpath (pszFullName, szDrive, szDir, NULL, NULL);
	wsprintf (szPath, "%s%s", szDrive, szDir);

	// use the static fn to do the work...
	CFileSystemFile* pFile = new CFileSystemFile();
	ScanFile (pszFullName, pFD, pFile);

	// Add this file to the parent folder
	TRACE("Adding File to folder, folder has %d files so far\n" ,pParentFolder->ListFiles().GetCount());	
	pParentFolder->AddFile(pFile);
	log.Format("Added file <<%s>>\n", pszFullName);
	
	// return success
	return TRUE;
}



//
//  static version that does the actual file scanning
//
BOOL CFileSystemScanner::ScanFile (LPCSTR pszFullPath, LPWIN32_FIND_DATA pFD, CFileSystemFile* pFile)
{
	char szName[_MAX_PATH], szExt[_MAX_EXT];
	_splitpath (pszFullPath, NULL, NULL, szName, szExt);
	strcat (szName, szExt);

	CString strFileVersion1, strFileVersion2, strProductVersion1, strProductVersion2;
	CString strCompanyName;
		
	CModuleVersion moduleVersion;
	CString filename = pszFullPath;
	moduleVersion.GetFileVersionInfo(filename.GetBuffer(0));

	// display file and product version from VS_FIXEDFILEINFO struct
	strFileVersion1.Format("%d.%d.%d.%d"
						 , HIWORD(moduleVersion.dwFileVersionMS)
						 , LOWORD(moduleVersion.dwFileVersionMS)
						 , HIWORD(moduleVersion.dwFileVersionLS)
						 , LOWORD(moduleVersion.dwFileVersionLS));
		
	strProductVersion1.Format("%d.%d.%d.%d"
						 , HIWORD(moduleVersion.dwProductVersionMS)
						 , LOWORD(moduleVersion.dwProductVersionMS)
						 , HIWORD(moduleVersion.dwProductVersionLS)
						 , LOWORD(moduleVersion.dwProductVersionLS));

	// convert the date format
	CString strCreated;
	CString strModified;
	CString strLastAccessed;
	
	CTime ctCreated(pFD->ftCreationTime);
	
	strCreated.Format("%4.4d-%2.2d-%2.2d %2.2d:%2.2d:%2d", ctCreated.GetYear(), ctCreated.GetMonth(), ctCreated.GetDay(),
					 ctCreated.GetHour(), ctCreated.GetMinute() ,ctCreated.GetSecond());
	//	
	CTime ctModified(pFD->ftLastWriteTime);
	
	
	strModified.Format("%4.4d-%2.2d-%2.2d %2.2d:%2.2d:%2d", ctModified.GetYear(), ctModified.GetMonth(), ctModified.GetDay(),
					 ctModified.GetHour(), ctModified.GetMinute() ,ctModified.GetSecond());
	//
	CTime ctLast(pFD->ftLastAccessTime);
	strLastAccessed.Format("%4.4d-%2.2d-%2.2d %2.2d:%2.2d:%2d", ctLast.GetYear(), ctLast.GetMonth(), ctLast.GetDay(),
					 ctLast.GetHour(), ctLast.GetMinute(), ctLast.GetSecond());

	// store all the results...
	pFile->Name(szName);
	pFile->Size(pFD->nFileSizeLow);
	pFile->CreatedDateTime(strCreated);
	pFile->ModifiedDateTime(strModified);
	pFile->LastAccessedDateTime(strLastAccessed);
	//
	pFile->CompanyName(FindVersionString(pszFullPath, _T("CompanyName")));
	pFile->ProductName(FindVersionString(pszFullPath, _T("ProductName")));
	pFile->Description(FindVersionString(pszFullPath, _T("Comments")));
	pFile->ProductVersion1(strProductVersion1);
	pFile->ProductVersion2(FindVersionString(pszFullPath, _T("ProductVersion")));
	pFile->FileVersion1(strFileVersion1);
	pFile->FileVersion2(FindVersionString(pszFullPath,_T("FileVersion")));
	pFile->OriginalFileName(FindVersionString(pszFullPath,_T("OriginalFilename")));		
	
	return TRUE;
}


void CFileSystemScanner::LogProgress(LPCSTR message)
{
	if (_pProgressListBox != NULL)
	{
		_pProgressListBox->AddString(message);
		_pProgressListBox->RedrawWindow();
		YieldProcess();
	}
}
