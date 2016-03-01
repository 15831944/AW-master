using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{

#region LyncAuditFile Class

	public class LyncAuditFile
	{
		#region Data Declarations

		AuditDataFile _usbAuditFile = null;
		string		  _fileName = "";
		
		#endregion Data Accessors

		public AuditDataFile ConvertedFile()
		{
			return _usbAuditFile;
		}

		public string FileName
		{
			get { return _fileName; }
		}

#region Static string definitions

		public static string _fileExtension = ".AUD";
		public static string _swaFileExtension = ".SWA";
		public const String SECTION_ASSET = "Asset";
		public const String KEY_NAME = "Name";
		public const String KEY_PARENT = "Parent";
		public const String KEY_SCANNER = "Scanner";
		public const String KEY_DATE = "Date";
		public const String KEY_CATEGORY = "Category";
		public const String KEY_BUSTYPE = "BusType";
		public const String KEY_HOSTPC = "HostPC";
		public const String KEY_NUMBEROFDRIVES = "NumberOfDrives";
		//
		public const String SECTION_HARDWARE = "Hardware";
		
#endregion Static string definitions

		public LyncAuditFile(string audFile)
		{
			_usbAuditFile = new AuditDataFile();
			_usbAuditFile.FileName = audFile;
			_fileName = Path.GetFileName(audFile);
			
			// Open and try and read the AUD file
			IniFile usbIniFile = new IniFile();
			usbIniFile.Read(audFile);
			
			// Now we need to take this file apart and create an ADF file from the pieces
			usbIniFile.SetSection(SECTION_ASSET);
			
			// Check the scanner name - this MUST contain the text 'USB' or 'PDA' to be valid
			string scannerType = usbIniFile.GetString(KEY_SCANNER, "");
			if (scannerType.Contains("USB"))
				_usbAuditFile.CreatedBy = AuditDataFile.CREATEDBY.usbscanner;
			else if (scannerType.Contains("PDA"))
				_usbAuditFile.CreatedBy = AuditDataFile.CREATEDBY.mdascanner;
			else
				return;
				
			// Name of Asset
			_usbAuditFile.AssetName = usbIniFile.GetString(KEY_NAME, "");
			_usbAuditFile.ParentAssetName = usbIniFile.GetString(KEY_PARENT, "");
			_usbAuditFile.AuditDate = usbIniFile.GetTime(KEY_DATE ,DateTime.Now ,false);
			_usbAuditFile.Category = usbIniFile.GetString(KEY_CATEGORY, "Disk Drive");
            _usbAuditFile.AgentVersion = usbIniFile.GetString(KEY_SCANNER, "LyncUSB");
			
			// The following fields do not seem to be uploaded by AuditWizard 
			string busType = usbIniFile.GetString(KEY_BUSTYPE ,"");
			string hostName = usbIniFile.GetString(KEY_HOSTPC ,"");
			string numberOfDrives = usbIniFile.GetString(KEY_NUMBEROFDRIVES ,"1");
			
			// We MAY have a hardware section which again we can convert into AuditedItems hopefully
			if (usbIniFile.FindSection(SECTION_HARDWARE) != -1)
			{
				List<string> listHardwareKeys = new List<string>();
				usbIniFile.EnumerateKeys(SECTION_HARDWARE, listHardwareKeys);
				
				// Loop through the keys returned
				foreach (string key in listHardwareKeys)
				{
					string value = usbIniFile.GetString(SECTION_HARDWARE, key, "");
					
					// The key is formatted as a comma separated list - the last item is the field name
					int delimiter = key.LastIndexOf(';');
					string category = "Hardware|" + key.Substring(0 ,delimiter);
					category = category.Replace(';' ,'|');
					string field = key.Substring(delimiter + 1);
										
					// Now create an AuditedDataItem for this field and add to the file
					AuditedItem item = new AuditedItem(category, field, value, "", AuditedItem.eDATATYPE.text, false);
					_usbAuditFile.AuditedItems.Add(item);
				}
			}
			
			// We may additionally have an SWA file for this asset - if so we should load its data into the 
			// audit data file which we are generating also.
			string swaFileName = Path.Combine(Path.GetDirectoryName(audFile), Path.GetFileNameWithoutExtension(audFile));
			swaFileName += ".swa";
			if (!File.Exists(swaFileName))
				return;


			// The SWA file is a simple text file with lines either containing a folder or file specification
			// Unfortunately the folders are in reverse order to that which we would expect so we need to tempoarily
			// store them in a list and then construct the container and build from the bottom up
			List<FileSystemFolder> listFolders = new List<FileSystemFolder>();
			FileSystemFolder currentFolder = null;
			string line;
			System.IO.StreamReader file = new System.IO.StreamReader(swaFileName);
			while ((line = file.ReadLine()) != null)
			{
				if (line.Contains(";"))
				{
					ProcessFileRead(line, currentFolder);
				}
				else
				{
					currentFolder = new FileSystemFolder();
					currentFolder.FullPath = line;
					listFolders.Add(currentFolder);
				}
			}

			// Close the file again
			file.Close();
			
			// Now we need to post-process the folders list so that we can upload it as a contiguous, heirarchial
			// list of folders - not as easy as it sounds as the Lync files may have missing nodes in the tree
			PostProcessFolders(listFolders);			
		}
		
		
		/// <summary>
		/// Recursively process a file system folder
		/// </summary>
		/// <param name="line"></param>
		/// <param name="?"></param>
		protected void ProcessFileRead(string line, FileSystemFolder currentFolder)
		{
			try
			{
				// BUG WORKAROUND
				// Owing to a bug in the way in which the file is written we end up with individual files being 
				// concatenated so we will try and split the line on '0;' as this terminates each file
				// There are 5 entries for each file and we must split up by stepping through the line as we
				// cannot just replace ';0' with some other separator as it may appear validly within the line
				List<string> listParts = Utility.ListFromString(line, ';', true);

				// If we have more than 5 parts then we are only interested in the last 5
				int startIndex = 0;
				if (listParts.Count > 5)
					startIndex = listParts.Count - 5;

				// Create the file system file object and set the file name
				FileSystemFile newFile = new FileSystemFile();
				if (startIndex == 0)
					newFile.Name = listParts[startIndex];
				else
					newFile.Name = listParts[startIndex].Substring(1);

				// get the last modified date - if this throws an exceptionwe ignore it
				string date = listParts[startIndex + 1];
				string time = listParts[startIndex + 2];
				time = time.Replace('-', ':');
				string datetime = date + " " + time;
				newFile.ModifiedDateTime = Convert.ToDateTime(datetime);

				// Last File Size
				newFile.Size = Convert.ToInt32(listParts[startIndex + 3]);

				// Add the file to the folder
				currentFolder.FilesList.Add(newFile);
			}
			catch (Exception)
			{ }
		}		
		
		/// <summary>
		/// Called as part of the post-processing of the folder list as we read the folders from the SWA
		/// file in the reverse order of what we really would like.  We need to reverse the list and then ensure that 
		/// we fill in any gaps in the tree.  What this means is that we may find two folders
		/// 
		/// F:\
		/// F:\TEST\SUBLEVEL
		/// 
		/// We need to ensure that we add F:\, F:\TEST and F:\TEST\SUBLEVEL to avoid any gaps in the tree as the
		/// main display would not like that
		/// </summary>
		/// <param name="listFolders"></param>
		protected void PostProcessFolders(List<FileSystemFolder> listFolders)
		{
			// Reverse the order of the folders as this makes it easier to process
			listFolders.Reverse();
			
			// Now process the folders
			foreach (FileSystemFolder folder in listFolders)
			{
				// Add this folder to those in the AuditDataFile
				AddFolder(folder);			
			}
		}			
			

		protected void AddFolder(FileSystemFolder folder)
		{
			// Split the folder up into it component parts
			List<string> pathParts = Utility.ListFromString(folder.FullPath ,'\\' ,false);
			string folderSoFar = "";
			
			// Try and find each part of the folder path and add any not defined
			FileSystemFolder currentFolder = null;
			foreach (string part in pathParts)
			{
				if (folderSoFar != "")
					folderSoFar += @"\";
				folderSoFar += part;
				
				// Does this folder exist in our list
				FileSystemFolder parentFolder = _usbAuditFile.AuditedFolders.FindFolderByPath(folderSoFar);
				
				// No - OK well add it as a child of the last folder we did find or at the top level otherwise
				if (parentFolder == null)
				{
					if (folder.FullPath == folderSoFar)
					{
						parentFolder = folder;
					}				
					else
					{				
						parentFolder = new FileSystemFolder();
						parentFolder.FullPath = folderSoFar;
					}
					
					// Now add this folder to its parent
					if (currentFolder == null)
					{
						_usbAuditFile.AuditedFolders.Add(parentFolder);
					}
					else
					{
						currentFolder.FoldersList.Add(parentFolder);
					}
				}

				currentFolder = parentFolder;
			}	
		}
		
	}
	
	#endregion Lync Audit File


	#region LyncAuditFileList

	public class LyncAuditFileList
	{
		#region Data

		private List<LyncAuditFile> _listLyncAuditFiles = new List<LyncAuditFile>();

		#endregion Data

		#region Properties

		public List<LyncAuditFile> LyncAuditFiles
		{
			get { return _listLyncAuditFiles; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Called to load all available alert notification files from the specified folder and 
		/// then delete the physical file
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public int Populate(string folder)
		{
			LogFile ourLog = LogFile.Instance;

			try
			{
				DirectoryInfo di = new DirectoryInfo(folder);
				string fileSpec = "*" + LyncAuditFile._fileExtension;
				FileInfo[] rgFiles = di.GetFiles(fileSpec);

				foreach (FileInfo fi in rgFiles)
				{
					// Try and read the file as an audit data file, if we fail skip this file
					LyncAuditFile thisFile = new LyncAuditFile(Path.Combine(fi.DirectoryName ,fi.Name));
					if (thisFile.ConvertedFile() == null)
					{
						ourLog.Write("The AUD file is not of a valid format for USB/Mobile Devices, skipping", true);
						continue;
					}

					_listLyncAuditFiles.Add(thisFile);
				}
			}

			// Any exceptions here probably indicate a locked file so we shall just ignore and 
			// have another go later
			catch (Exception ex)
			{
				ourLog.Write("Exception processing USB/PDA Audit Files, the error was " + ex.Message, true);
			}

			return _listLyncAuditFiles.Count;
		}

		#endregion Methods
	}

	#endregion LyncAuditFileList

}
