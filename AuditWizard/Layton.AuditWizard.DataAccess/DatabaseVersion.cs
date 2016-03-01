using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	public class DatabaseVersion
	{
		#region Data
		/// <summary>This is the name of the database (descriptive)</summary>
		private string	_title;

		/// <summary>Major version of the database</summary>
		private int		_majorVersion;

		/// <summary>Minor version of the database</summary>
		private int		_minorVersion;

		/// <summary>Date the database was initially created</summary>
		private DateTime _createdDate;

		/// <summary>Microsoft SQL Server database version string</summary>
		private string	_sqlVersion;

		/// <summary>Not used</summary>
		private string	_user;

		/// <summary>Not Used</summary>
		private string	_password;

		#endregion Data

		#region Properties

		/// <summary>
		/// The descriptive title assigned to this database
		/// </summary>
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		/// <summary>
		/// Numeric Major version of the database
		/// </summary>
		public int MajorVersion 
		{
			get { return _majorVersion; }
			set { _majorVersion = value; }
		}

		/// <summary>
		/// Numeric Minor version of this database
		/// </summary>
		public int MinorVersion
		{
			get { return _minorVersion; }
			set { _minorVersion = value; }
		}

		/// <summary>
		/// Date and Time the database was created
		/// </summary>
		public DateTime CreatedDate
		{
			get { return _createdDate; }
			set { _createdDate = value; }
		}

		/// <summary>
		/// Version string returned from the SQL Server database, identifies the exact database server on which
		/// the database has been created
		/// </summary>
		public string SqlVersion
		{
			get { return _sqlVersion; }
			set { _sqlVersion = value; }
		}

		public string VersionString
		{
			get { return string.Format("AuditWizard v8 COM Interface Version {0}.{1}", _majorVersion ,_minorVersion); }
		}

		#endregion Properties

		#region Constructor

		public DatabaseVersion()
		{
			_majorVersion = 0;
			_minorVersion = 0;
			_createdDate = DateTime.Now;
			_sqlVersion = "";
			_title = "";
			_user = "";
			_password = "";

			// Recover the version data from the database
            VersionDAO lwDataAccess = new VersionDAO();
			DataTable table = lwDataAccess.GetDatabaseVersion();

			// ...and populate this object
			if (table.Rows.Count == 1)
			{
				DataRow row = table.Rows[0];
				try
				{
					_majorVersion = (int)row["_MAJOR"];
					_minorVersion = (int)row["_MINOR"];
					_createdDate = (DateTime)row["_DATE"];
					_sqlVersion = (string)row["_SQLVER"];
					_title = (string)row["_TITLE"];
					_user = (string)row["_USER"];
					_password = (string)row["_PASSWORD"];
				}
				catch (Exception)
				{
					_majorVersion = 0;
					_minorVersion = 0;
				}
			}
		}

		#endregion Constructor

		#region Methods

		#endregion Methods
	}
}
