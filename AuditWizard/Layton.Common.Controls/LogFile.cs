using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
	/// <summary>
	/// This is a singleton logging class.
	/// </summary>
	public sealed class LogFile
	{

#region LogFile control data

		static LogFile instance = null;
		static readonly object padlock = new object();

		#endregion LogFile control data

#region Data declarartions

		private static String _fileName = "";
		private static StreamWriter _fileWriter = null;
        private static FileInfo _fileInfo;

#endregion Data declarartions

#region Data Accessors

		/// <summary>Return name of th output log file</summary>
		public String FileName
		{  get { return _fileName; } }

#endregion Data Accessors		

#region Constructors

		public LogFile() 
		{
		}

#endregion Constructors

#region Functions

		public static LogFile Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new LogFile();
						Open();
					}
					return instance;
				}
			}
		}

		/// <summary>
		/// Open the output log file
		/// </summary>
		/// <returns></returns>
		private static void Open ()
		{
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "\\logs"));

            string baseName = Path.Combine(Application.StartupPath + "\\logs", "aw_svc.log");
            _fileName = baseName;

			// Now try and open a NEW file with this name and write the start message to it
			// try and create a file to write to
			try
			{
				_fileWriter = File.CreateText(_fileName);
				DateTime now = DateTime.Now;
				String buffer = String.Format("-- SESSION STARTED {0} ---" ,now.ToString());
				_fileWriter.WriteLine(buffer);
			}
			catch (Exception)
			{
				// Not a lot we can do as we cannot create the log file to report the error!
			}
		}

		
		/// <summary>
		/// Write text to the log file, optionally flushing to disk after
		/// </summary>
		/// <param name="text">text to be written</param>
		/// <param name="flush">true if we are to flush the log file to disk</param>
		public void Write (String text ,bool flush)
		{
            CheckFileSize();

			if (_fileWriter == null)
				return;
#if Debug			
			Trace.WriteLine(text);
#endif
			_fileWriter.WriteLine(DateTime.Now + " : " + text);
			if (flush)
				_fileWriter.Flush();
		}

		/// <summary>
		/// Flush logfile to disk
		/// </summary>
		public void Flush ()
		{
			if (_fileWriter != null)
				_fileWriter.Flush();
		}

        private void CheckFileSize()
        {
            if (!File.Exists(_fileName)) return;

            _fileInfo = new FileInfo(_fileName);

            if (_fileInfo.Length > 2048000)
            {
                _fileWriter.Close();
                File.Delete(_fileName);
                _fileWriter = File.CreateText(_fileName);
            }
        }

#endregion Functions
	}
}
