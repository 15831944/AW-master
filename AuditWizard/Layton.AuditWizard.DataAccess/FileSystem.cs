using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	#region FileSystemFile Class
	
	public class FileSystemFile	
	{
		#region Data
		
		/// <summary>Database ID of this file rrcord, 0 if not saved</summary>
		private int _fileID;
		
		/// <summary>Database ID of the parent folder</summary>
		private int		_parentID;
		
		/// <summary>Full path for the parent folder</summary>
		private string _parentName;
		
		/// <summary>Name of this file (no path)</summary>
		private string	_name;

		/// <summary>Database ID of the asset to whom this file record belongs</summary>
		private int		_assetID;
	
		/// <summary>Name of the asset to whom this file record belongs</summary>
		private string	_location;
		private string	_assetname;

		/// <summary>Size in bytes of this file</summary>
		private int		_size;
		
		/// <summary>Date of creation of this file</summary>
		private DateTime _createdDateTime;
		
		/// <summary>Date this file was last modified</summary>
		private DateTime _modifiedDateTime;
		
		/// <summary>Date that this file was last accessed</summary>
		private DateTime _lastAccessedDateTime;

		/// <summary>Name of the publisher of this file recovered from VersionInfo</summary>
		private string	_publisher;
		
		/// <summary>Name of the product associated with this file recovered from VersionInfo</summary>
		private string	_productName;

		/// <summary>Description of this file recovered from VersionInfo</summary>
		private string	_description;

		/// <summary>Product Version of this file recovered from the Version String</summary>
		private string	_productVersion1;

		/// <summary>Product Version of this file recovered from VersionInfo</summary>
		private string	_productVersion2;

		/// <summary>File Version of this file recovered from the Version String</summary>
		private string	_fileVersion1;

		/// <summary>File Version of this file recovered from VersionInfo</summary>
		private string	_fileVersion2;

		/// <summary>Original name of this file</summary>
		private string	_originalFileName;

		/// <summary>The database ID of (any) application to which this file has been assigned</summary>
		private int _assignApplicationID;

		/// <summary>The name of (any) application to which this file has been assigned</summary>
		private string _assignApplication;
		
		#endregion Data

		#region Properties

		public int FileID
		{
			get { return _fileID; }
			set { _fileID = value; }
		}

		public int ParentID
		{
			get { return _parentID; }
			set { _parentID = value; }
		}

		public int AssetID
		{
			get { return _assetID; }
			set { _assetID = value; }
		}

		public string Location
		{
			get { return _location; }
			set { _location = value; }
		}

		public string AssetName
		{
			get { return _assetname; }
			set { _assetname = value; }
		}
		
		public int Size
		{
			get { return _size; }
			set { _size = value; }
		}

		public string ParentName
		{
			get { return _parentName; }
			set { _parentName = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public DateTime CreatedDateTime
		{
			get { return _createdDateTime; }
			set { _createdDateTime = value; }
		}

		public DateTime ModifiedDateTime
		{
			get { return _modifiedDateTime; }
			set { _modifiedDateTime = value; }
		}

		public DateTime LastAccessedDateTime
		{
			get { return _lastAccessedDateTime; }
			set { _lastAccessedDateTime = value; }
		}

		public string Publisher
		{
			get { return _publisher; }
			set { _publisher = value; }
		}

		public string ProductName
		{
			get { return _productName; }
			set { _productName = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string ProductVersion1
		{
			get { return _productVersion1; }
			set { _productVersion1 = value; }
		}

		public string ProductVersion2
		{
			get { return _productVersion2; }
			set { _productVersion2 = value; }
		}

		public string FileVersion1
		{
			get { return _fileVersion1; }
			set { _fileVersion1 = value; }
		}

		public string FileVersion2
		{
			get { return _fileVersion2; }
			set { _fileVersion2 = value; }
		}

		public string OriginalFileName
		{
			get { return _originalFileName; }
			set { _originalFileName = value; }
		}

		public int AssignApplicationID
		{
			get { return _assignApplicationID; }
		}

		public string AssignApplication
		{
			get { return _assignApplication; }
		}
		
		#endregion Properties

		#region Constructor

		public FileSystemFile()
		{
			_fileID = 0;
			_parentID = 0;
			_name = "";
			_parentName = "";
			_assetID = 0;
			_assetname = "";
			_location = "";
			_size = 0;
			_createdDateTime = new DateTime(0);
			_modifiedDateTime = new DateTime(0);
			_lastAccessedDateTime = new DateTime(0);
			_publisher = "";
			_productName = "";
			_description = "";
			_productVersion1 = "";
			_productVersion2 = "";
			_fileVersion1 = "";
			_fileVersion2 = "";
			_originalFileName = "";
			_assignApplicationID = 0;
			_assignApplication = "";
		}

		public FileSystemFile(DataRow row) : this()
		{
			_fileID		= (int)row["_FILEID"];
			_parentID	= row.IsNull("_PARENTID") ? 0 : (int)row["_PARENTID"];
			_name		= row.IsNull("_NAME") ? String.Empty : (string)row["_NAME"];
			_parentName = row.IsNull("PARENTNAME") ? String.Empty : (string)row["PARENTNAME"];
			_assetID	= row.IsNull("_ASSETID") ? 0 : (int)row["_ASSETID"];
			_size		= row.IsNull("_SIZE") ? 0 : (int)row["_SIZE"];
			//
			if (!row.IsNull("_CREATED_DATE"))
				_createdDateTime = (DateTime)row["_CREATED_DATE"];
			//
			if (!row.IsNull("_MODIFIED_DATE"))
				_modifiedDateTime = (DateTime)row["_MODIFIED_DATE"];
			//
			if (!row.IsNull("_LASTACCESSED_DATE"))
				_lastAccessedDateTime = (DateTime)row["_LASTACCESSED_DATE"];
			//
            _publisher = row.IsNull("_PUBLISHER") ? String.Empty : (string)row["_PUBLISHER"];
            _productName = row.IsNull("_PRODUCTNAME") ? String.Empty : (string)row["_PRODUCTNAME"];
            _productVersion1 = row.IsNull("_PRODUCT_VERSION1") ? String.Empty : (string)row["_PRODUCT_VERSION1"];
            _productVersion2 = row.IsNull("_PRODUCT_VERSION2") ? String.Empty : (string)row["_PRODUCT_VERSION2"];
            _fileVersion1 = row.IsNull("_FILE_VERSION1") ? String.Empty : (string)row["_FILE_VERSION1"];
            _fileVersion2 = row.IsNull("_FILE_VERSION2") ? String.Empty : (string)row["_FILE_VERSION2"];
            _originalFileName = row.IsNull("_ORIGINAL_FILENAME") ? String.Empty : (string)row["_ORIGINAL_FILENAME"];
            _assignApplicationID = row.IsNull("_ASSIGN_APPLICATIONID") ? 0 : (int)row["_ASSIGN_APPLICATIONID"];
			
			// We may have the asset name and location in this row also
			if (row.Table.Columns.Contains("FULLLOCATIONNAME"))
				_location = row.IsNull("FULLLOCATIONNAME") ? String.Empty : (string)row["FULLLOCATIONNAME"];

			if (row.Table.Columns.Contains("ASSETNAME"))
                _assetname = row.IsNull("ASSETNAME") ? String.Empty : (string)row["ASSETNAME"];
		}


		/// <summary>
		/// Add this FileSystemFolder to the database - note we do not update ANY folders just ignore
		/// any which already have a database ID
		/// </summary>
		/// <returns></returns>
		public int Save()
		{
            FileSystemDAO lwDataAccess = new FileSystemDAO();
			if (FileID != 0)
				return -1;

			// Save this file	
			FileID = lwDataAccess.FileSystemFile_Add(this);
			
			// ...and return
			return 0;
		}
		#endregion Constructor
		
		#region methods

		/// <summary>
		/// Assign this file to the specified application - note passing 0 will unassign the file
		/// </summary>
		/// <param name="applicationID"></param>
		public void Assign (int applicationID)
		{
            FileSystemDAO lwDataAccess = new FileSystemDAO();
			_assignApplicationID = applicationID;
			lwDataAccess.FileSystemFile_Assign(this);		
		}

		#endregion methods

	}

	#endregion FileSystemFile Class
	

	#region Folder Class

	public class FileSystemFolder
	{
		#region Data

		// ID of this folder in the database
		private int		_folderID;

		/// <summary>The full path is used only when uploading USB/PDA audits and is NOT stored in the database</summary>
		private string _fullpath;

		/// <summary>The name is just the partial name of the folder without its path</summary>
		private string _name;
		
		/// <summary>Database ID of the parent folder, NULL If none</summary>
		private int		_parentID;

		/// <summary>Database ID of the asset to whom this folder belongs</summary>
		private int _assetID;
		
		/// <summary>List of files within this folder</summary>
		private List<FileSystemFile> _listFiles = new List<FileSystemFile>();
		
		/// <summary>List of folders beneath this folder</summary>
		private FileSystemFolderList _listFolders = new FileSystemFolderList();
		
		#endregion Data

		#region Properties

		public int FolderID
		{
			get { return _folderID; }
			set { _folderID = value; }
		}

		public int ParentID
		{
			get { return _parentID; }
			set { _parentID = value; }
		}

		public string FullPath
		{
			get { return _fullpath; }
			set 
			{ 
				_fullpath = value; 
				int delimiter = value.LastIndexOf(@"\");
				if (delimiter != -1)
					Name = value.Substring(delimiter + 1);
				else
					Name = value;
			}
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int AssetID
		{
			get { return _assetID; }
			set { _assetID = value; }
		}

		public FileSystemFolderList FoldersList
		{
			get { return _listFolders; }
		}

		public List<FileSystemFile> FilesList
		{
			get { return _listFiles; }
		}
			
		public string DisplayName
		{
			get
			{ 
				if (_parentID == 0)
					return _name;
				else 
				{
					int delimiter = _name.LastIndexOf('\\');
					return (delimiter == -1) ? _name : _name.Substring(delimiter + 1);
				}
			}
		}
		
		#endregion Properties

		#region Constructor

		public FileSystemFolder()
		{
			_folderID = 0;
			_parentID = 0;
			_assetID = 0;
		}

		public FileSystemFolder (DataRow row) : this()
		{
			_folderID = (int)row["_folderID"];
			_parentID = row.IsNull("_PARENTID") ? 0 : (int)row["_PARENTID"];
			_name = (string)row["_NAME"];
			_assetID = (int)row["_ASSETID"];
		}

		#endregion Constructor

		public override string ToString()
		{
			return Name;
		}


		/// <summary>
		/// Add this FileSystemFolder to the database - note we do not update ANY folders just ignore
		/// any which already have a database ID
		/// </summary>
		/// <returns></returns>
		public int Save()
		{
            FileSystemDAO lwDataAccess = new FileSystemDAO();
			if (FolderID != 0)
				return -1;
			
			// Save this folder	
			FolderID = lwDataAccess.FileSystemFolder_Add(this);
			
			// Iterate through any child folders and save them also remembering to set their parent ID to 
			// point to us
			foreach (FileSystemFolder childFolder in _listFolders)
			{
				childFolder.ParentID = FolderID;
				childFolder.AssetID = AssetID;
				childFolder.Save();
			}

			// Iterate through any child files and save them also remembering to set their parent ID to 
			// point to us
			foreach (FileSystemFile childFile in _listFiles)
			{
				childFile.ParentID = FolderID;
				childFile.AssetID = AssetID;
				childFile.Save();
			}

			return 0;
		}



		/// <summary>
		/// Delete this Folder, and any sub-folders and files
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			return 0;
		}


		/// <summary>
		/// Find a folder with the specified ID either at this level or beneath us
		/// </summary>
		/// <param name="folderID"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolder(int folderID)
		{
			// Are we the folder being looked for?
			if (this.FolderID == folderID)
				return this;

			// No - ok check our children then
			foreach (FileSystemFolder subFolder in _listFolders)
			{
				FileSystemFolder returnFolder = subFolder.FindFolder(folderID);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}


		/// <summary>
		/// Find a folder with the specified name either at this level or beneath us
		/// </summary>
		/// <param name="folderID"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolder(string folderName)
		{
			// Are we the folder being looked for?
			if (String.Compare(this.Name, folderName, true) == 0)
				return this;

			// No - ok check our children then
			foreach (FileSystemFolder subFolder in _listFolders)
			{
				FileSystemFolder returnFolder = subFolder.FindFolder(folderName);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}


		/// <summary>
		/// Find a folder with the specified PATH either at this level or beneath us
		/// </summary>
		/// <param name="folderID"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolderByPath(string path)
		{
			// Are we the folder being looked for?
			if (String.Compare(this.FullPath, path, true) == 0)
				return this;

			// No - ok check our children then
			foreach (FileSystemFolder subFolder in _listFolders)
			{
				FileSystemFolder returnFolder = subFolder.FindFolderByPath(path);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}
		
	}

	#endregion FileSystemFolder Class


	#region FileSystemFolderList Class

	public class FileSystemFolderList : List<FileSystemFolder>
	{
		#region Data
		#endregion Data

		#region Properties
		#endregion Properties

		#region Constructor

		public FileSystemFolderList()
		{
		}


		/// <summary>
		/// This constructor takes a list of folders and files and try to rebuild the heirachy
		/// </summary>
		/// <param name="folders"></param>
		/// <param name="files"></param>
		public FileSystemFolderList (DataTable folders ,DataTable files)
		{
			foreach (DataRow row in folders.Rows)
			{
				// Create a new folder for this row
				FileSystemFolder folder = new FileSystemFolder(row);
			
				// If our parent is non-zero then we need to add this folder to hopefully one which already
				// exists otherwise we add it to the root list
				if (folder.ParentID == 0)
				{
					this.Add(folder);
				}
				else
				{
					// Find the folder which is our parent
					FileSystemFolder parentFolder = FindFolder(folder.ParentID);
					if (parentFolder == null)
						continue;
						
					// ...and add us to the folder
					parentFolder.FoldersList.Add(folder);
				}
			}
			
			// Folders populated - now do the files
			foreach (DataRow row in files.Rows)
			{
				// Create a new file for this row
				FileSystemFile file = new FileSystemFile(row);

				// Find the folder which is our parent
				FileSystemFolder parentFolder = FindFolder(file.ParentID);
				if (parentFolder == null)
					continue;

				// ...and add us to the folder
				parentFolder.FilesList.Add(file);
			}		
		}
		
		#endregion Constructor

		#region Methods


		/// <summary>
		/// Find the parent folder passed the database ID of a possible child
		/// </summary>
		/// <param name="childFolderName"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolder(int parentID)
		{
			// Iterate through our list and see if we find any matches
			foreach (FileSystemFolder folder in this)
			{
				if (folder.FolderID == parentID)
					return folder;
					
				// Not this folder so try its children
				FileSystemFolder returnFolder = folder.FindFolder(parentID);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}


		/// <summary>
		/// Find the parent folder passed the database ID of a possible child
		/// </summary>
		/// <param name="childFolderName"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolder(string folderName)
		{
			// Iterate through our list and see if we find any matches
			foreach (FileSystemFolder folder in this)
			{
				if (String.Compare(folder.Name, folderName, true) == 0)
					return folder;

				// Not this folder so try its children
				FileSystemFolder returnFolder = folder.FindFolder(folderName);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}

		/// <summary>
		/// Find the parent folder given the full path of the folder
		/// </summary>
		/// <param name="childFolderName"></param>
		/// <returns></returns>
		public FileSystemFolder FindFolderByPath(string path)
		{
			// Iterate through our list and see if we find any matches
			foreach (FileSystemFolder folder in this)
			{
				if (String.Compare(folder.FullPath, path, true) == 0)
					return folder;

				// Not this folder so try its children
				FileSystemFolder returnFolder = folder.FindFolderByPath(path);
				if (returnFolder != null)
					return returnFolder;
			}

			return null;
		}
	
		#endregion Methods

	}

	#endregion FileSystemFolder Class



}



