using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.AuditWizardService
{
	public class AutoLoaderFolder
	{
		private FileSystemWatcher	_fileSystemWatcher;
		private string				_fileName;
		private string				_scannerName;
		private DateTime			_lastTimeStamp;
		private string				_dataFolder;

		public string FileName
		{ get { return _fileName; } }

		public string ScannerName
		{ get { return _scannerName; } }

		public string DataFolder
		{ 
			get { return _dataFolder; } 
			set { _dataFolder = value; }
		}

		public FileSystemWatcher GetWatcher
		{ get { return _fileSystemWatcher; } }

		public DateTime LastTimeStamp
		{ 
			get { return _lastTimeStamp; }
			set { _lastTimeStamp = value; }
		}
		
		public AutoLoaderFolder(string fileName ,string scannerName ,string dataFolder ,FileSystemWatcher fileSystemWatcher ,DateTime lastTimeStamp)
		{
			_fileName = fileName;
			_scannerName = scannerName;
			_dataFolder = dataFolder;
			_fileSystemWatcher = fileSystemWatcher;
			_lastTimeStamp = lastTimeStamp;
		}
	}

	public class AutoLoaderFolderList : List<AutoLoaderFolder>
	{
		// Locate an entry in the list
        public AutoLoaderFolder ContainsScanner(AuditScannerDefinition aAuditScannerDefinition)
		{
			foreach (AutoLoaderFolder thisFolder in this)
			{
				if (thisFolder.FileName == aAuditScannerDefinition.Filename)
				{
					return thisFolder;
				}
			}
			return null;
		}
	}


}
