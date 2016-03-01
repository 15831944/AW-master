using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//
using Layton.Common.Controls;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
	public class HistoryReportEntry
	{
		#region Data
		private int			_HistoryReportEntryID;
		private string		_location;
		private string		_assetname;
		private string		_source;
		private DateTime	_date;
		private string		_url;
		private int			_pagesAccessed;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

#endregion Data

		#region Properties

		public int HistoryReportEntryID
		{
			get { return _HistoryReportEntryID; }
			set { _HistoryReportEntryID = value; }
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

		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}

		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public int PagesAccessed
		{
			get { return _pagesAccessed; }
			set { _pagesAccessed = value; }
		}

		#endregion Properties

		#region Constructors

		public HistoryReportEntry()
		{
			_HistoryReportEntryID = 0;
			_location = "";
			_assetname = "";
			_source = "";
			_url = "";
			_pagesAccessed = 0;
		}

		public HistoryReportEntry(DataRow dataRow)
		{
			try
			{
				this.HistoryReportEntryID = (int)dataRow["_AUDITEDITEMID"];
				this.AssetName = (string)dataRow["ASSETNAME"];
				this.Location = (string)dataRow["FULLLOCATIONNAME"];
			
				// recover the category as this determines how the remainder of the data will be formatted
				string category = (string)dataRow["_CATEGORY"];
				List<string> categoryParts = Utility.ListFromString(category, '|', true);
			
				// Check the Internet Record Type which is item 1 in the list
				if (categoryParts.Count < 3)
					return;
				_source = categoryParts[1];
			
				// Handle Internet History Records first
				if (_source == "History")
				{
					_date = Convert.ToDateTime(categoryParts[2]);
					_url = categoryParts[3];
					_pagesAccessed = Convert.ToInt32((string)dataRow["_VALUE"]);
				}
			
				else if (_source == "Cookie")
				{
					_date = Convert.ToDateTime((string)dataRow["_VALUE"]);
					_url = categoryParts[2];
					_pagesAccessed = 0;
				}
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a HISTORYREPORTENTRY Object, please check database schema.  The message was " + ex.Message);
			}
		}
#endregion Constructors

	}
}
