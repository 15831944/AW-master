using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{

#region AuditTrailMessage Class

	public class AuditTrailMessage
	{
		/// <summary>
		/// The severity of the entry
		/// </summary>
		public enum SEVERITY { INFORMATION ,SUCCESS ,FAILURE ,ERROR };

		private DateTime _date;
		private String	_message;
		private SEVERITY _severity;

		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		public String Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public SEVERITY Severity
		{
			get { return _severity; }
			set { _severity = value; }
		}

		public String SeverityText
		{
			get { return GetSeverityText(_severity) ; }
		}

		public SEVERITY SeverityValue (String severityText)
		{
			SEVERITY severityValue;
			switch (severityText)
			{
				case "[INFORMATION]":
					severityValue = SEVERITY.INFORMATION;
					break;
				case "[ERROR]":
					severityValue = SEVERITY.ERROR;
					break;
				case "[FAILURE]":
					severityValue = SEVERITY.FAILURE;
					break;
				case "[SUCCESS]":
					severityValue = SEVERITY.SUCCESS;
					break;
				default:
					severityValue = SEVERITY.INFORMATION;
					break;
			}
			return severityValue;
		}

		public AuditTrailMessage (SEVERITY severity ,String message)
		{
			_severity = severity;
			_message = message;
			_date = DateTime.Now;
		}

		/// <summary>
		/// Constructor which accepts a pipe separated text string
		/// </summary>
		/// <param name="message"></param>
		public AuditTrailMessage(String message)
		{
			String[] msgParts = message.Split('|');
			try
			{
				if (msgParts.Length == 3)
				{
					_date = DateTime.Parse(msgParts[0]);
					_severity = SeverityValue(msgParts[1]);
					_message = msgParts[2];
				}
				else
				{
					throw new Exception();
				}
			}
			catch (Exception)
			{
				_severity = SEVERITY.INFORMATION;
				_message = "";
				_date = DateTime.Now;
			}
		}

#region Helpers
		/// <summary>
		/// Return the severity as a textual representation
		/// </summary>
		/// <param name="severity"></param>
		/// <returns>Textual representation of the severity</returns>
		protected String GetSeverityText(SEVERITY severity)
		{
			String severityText;
			switch (severity)
			{
				case SEVERITY.INFORMATION:
					severityText = "[INFORMATION]";
					break;

				case SEVERITY.SUCCESS:
					severityText = "[SUCCESS]";
					break;

				case SEVERITY.FAILURE:
					severityText = "[FAILURE]";
					break;

				case SEVERITY.ERROR:
					severityText = "[ERROR]";
					break;

				default:
					severityText = "[UNKNOWN]";
					break;
			}

			return severityText;
		}
#endregion Helpers

	}

#endregion AuditTrailMessage Class


	public class  AuditTrailFile
	{

		// Flags for INI file operations - may be combined
		public enum ACCESS { READ, WRITE };

#region Data declarartions

		private ACCESS _access;
		private String	_fileName = "";
		private StreamWriter _fileWriter = null;
		private StreamReader _fileReader = null;
		private List<AuditTrailMessage> _listMessages = new List<AuditTrailMessage>();

#endregion Data declarartions

#region Data Accessors

		/// <summary>Return name of th output log file</summary>
		public String FileName
		{ get { return _fileName; } }

		public List<AuditTrailMessage> Messages
		{
			get { return _listMessages; }
		}

#endregion Data Accessors		

		public AuditTrailFile(string filename ,ACCESS access)
		{
			_fileName = filename;
			_access = access;

			// Open the file with the required access
			Open();
		}


#region Functions

		/// <summary>
		/// Open the output audit trail file
		/// </summary>
		/// <returns></returns>
		private void Open()
		{
			// Now build the fully qualified log file name basing it on the name of the
			// running executable and the current application folder
			if (_fileName == null || _fileName == "")
			{
				String baseName = Path.Combine(Application.StartupPath, Application.ProductName);
				_fileName = baseName + ".atf";
			}

			// Open the file with the required access - write creates a new file
			try
			{
				if (_access == ACCESS.READ)
				{
					FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read ,FileShare.ReadWrite);
					_fileReader = new StreamReader(fs);
					Read();
				}
				else
				{
					FileStream fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write ,FileShare.ReadWrite);
					_fileWriter = new StreamWriter(fs);
					AddMessage(AuditTrailMessage.SEVERITY.INFORMATION, "-- SESSION STARTED ---");
				}
			}
			catch (Exception)
			{
			}
		}


		/// <summary>
		/// Write text to the file, optionally flushing to disk after
		/// </summary>
		/// <param name="text">text to be written</param>
		/// <param name="flush">true if we are to flush the log file to disk</param>
		protected void Write(String text)
		{
			if (_fileWriter != null)
			{
				_fileWriter.WriteLine(text);
				Flush();
			}
		}


		/// <summary>
		/// Read the contents of the file into our internal list
		/// </summary>
		protected void Read()
		{
			String lineRead;
			lineRead = _fileReader.ReadLine();
			while (lineRead != null)
			{
				// Process the line read from the file				
				lineRead.Trim();

				// Is there anything left to process
				if (lineRead.Length != 0)
				{
					AuditTrailMessage newMessage = new AuditTrailMessage(lineRead);
					_listMessages.Add(newMessage);
				}

				// Read the next line and loop back to process it
				lineRead = _fileReader.ReadLine();
			}
			_fileReader.Close();
		}



		/// <summary>
		/// Flush logfile to disk
		/// </summary>
		public void Flush()
		{
			_fileWriter.Flush();
		}


		public void AddMessage (AuditTrailMessage msg)
		{
			String severityText = msg.SeverityText;
			String formattedmsg = msg.Date.ToString() + "|" + severityText + "|" + msg.Message;
			Write(formattedmsg);
		}

		public void AddMessage(AuditTrailMessage.SEVERITY severity, String msg)
		{
			AddMessage(new AuditTrailMessage(severity ,msg));
		}
#endregion Functions

	}
}
