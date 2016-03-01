using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Xml;
//
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// Thjis class encapsulates an Internet Usage Report Definition
	/// </summary>
	public class InternetReportDefinition : ReportDefinition
	{
		#region Data
	
		bool		_showAllDates;
		
		/// <summary>Only show records since the start date</summary>
		DateTime	_startDate;
		
		/// <summary>Only show records prior to the end date</summary>
		DateTime	_endDate;
		
		/// <summary>Only include records where the URL matches the wildcard</summary>
		string _filterURL;
		
		#endregion data

		#region properties

		public bool ShowAllDates 
		{
			get { return _showAllDates; }
			set { _showAllDates = value; }
		}
		
		public void SetDateRange (DateTime startDate ,DateTime endDate)
		{
			_hasChanged = _hasChanged || ((_startDate != startDate) || (_endDate != endDate));
			//ShowAllDates = false;
			_startDate = startDate;
			_endDate = endDate;
		}

		public string FilterURL
		{
			get { return _filterURL; }
			set { _hasChanged = _hasChanged || (_filterURL != value); _filterURL = value; }
		}

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
		
		#endregion properties
		
		#region XMLStrings

		// Filters
		private const string S_FILTERS = "Filters";
		private const string V_FILTERS_STARTDATE = "StartDate";
		private const string V_FILTERS_ENDDATE	= "EndDate";
		private const string V_FILTERS_URL		= "Url";

		#endregion

		#region Constructor
		
		public InternetReportDefinition() : base()
		{
			// Set the default name for this report in the header section
			ExportSection headerSection = GetReportSection(ExportSection.eSectionType.header);
			headerSection.FormattedText.RawText = "Internet Usage Report";
			//
			_name = "Internet Usage Report";
			_reportType = eReportType.internet;
			_showAllDates = true;
			_startDate = new DateTime(0);
			_endDate = new DateTime(0);
			_filterURL = "";
			
			// Initialize the data set
			_reportDataSet.DataSetName = "internetDataSet";

			// Create the tables, columns and relationships
			CreateTables();
		}
		
		#endregion Constructor

		#region Methods

		/// <summary>
		/// This function is responsible for the actual generation of the Applications Report
		/// It overrides the abstract definition in the base class and stores it's data in the DataSet
		/// </summary>
		public override void GenerateReport(UltraGrid grid)
		{
			// Clear any existing data out of the dataset
			_reportDataSet.Tables["Internet"].Rows.Clear();

			// Set the grid datasource to be this dataset
			grid.DataSource = _reportDataSet;
							
			// Generate the data for the report
			GenerateReportData();
			
			// ...and perform any required initialization of the grid
			InitializeGrid(grid);
		}



		/// <summary>
		/// Called to perform any row specific initailization as the row is displayed
		/// </summary>
		/// <param name="gridRow"></param>
		public override void InitializeGridRow(UltraGridRow gridRow)
		{
			if ((gridRow == null) || (gridRow.Cells.Count == 0))
				return;

			// Set the icon for the first visible row to that appropriate for the object
			SetFirstVisibleColumnIcon(gridRow, Properties.Resources.internet_explorer);
		}


		/// <summary>
		/// Create the tables, columns, relationships and foreign keys required for this report
		/// </summary>
		protected void CreateTables()
		{
			// Only 1 table is required for the Internet report
			DataTable internetTable = new DataTable("Internet");

			// begin the initialization of the dataset tables
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(internetTable)).BeginInit();

			// Add the table to the Database
			_reportDataSet.Tables.AddRange(new System.Data.DataTable[] { internetTable });		
		
			// Create the INTERNET table
			// Columns are :- Location ,Asset ,Date ,URL ,Page Count
			//DataColumn column1 = new DataColumn("object", typeof(object));
			//DataColumn column2 = new DataColumn("Location");
			DataColumn column3 = new DataColumn("Asset Name");
			//DataColumn column4 = new DataColumn("Found In");
			DataColumn column5 = new DataColumn("Date");
			DataColumn column6 = new DataColumn("URL");
			DataColumn column7 = new DataColumn("Pages Accessed");

			// Add these columns to the tabls
			internetTable.Columns.AddRange(new System.Data.DataColumn[] { column3, column5, column6, column7 });

			
			// End initialization of the DataSet and Table
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(internetTable)).EndInit();			
		}		



		/// <summary>
		/// This function is responsible for generating the actual data which will be displayed by the report
		/// </summary>
		protected void GenerateReportData()
		{
			// Create a string representation of the publisher filter list passed to us
			// We need to get the entire licensing information at this point
            StatisticsDAO lwDataAccess = new StatisticsDAO();
			DataTable internetTable = lwDataAccess.GetInternetHistory(_startDate, _endDate ,_filterURL);

            // Create a string representation of the publisher filter list passed to us
            // We need to get the entire licensing information at this point
            AssetGroup.GROUPTYPE displayType = AssetGroup.GROUPTYPE.userlocation;
            DataTable table = new LocationsDAO().GetGroups(new AssetGroup(displayType));
            AssetGroup _cachedAssetGroups = new AssetGroup(table.Rows[0], displayType);
            _cachedAssetGroups.Populate(true, _ignoreChildAssets, true);

            // Now apply the filter to these groups
            _cachedAssetGroups.ApplyFilters(_selectedGroups, _selectedAssets, _ignoreChildAssets);

            // Now that we have a definitive list of the assets (as objects) which we want to include in the
            // report we could really do with expanding this list so that ALL of the assets are in a single list
            // and not distributed among the publishers
            AssetList _cachedAssetList = _cachedAssetGroups.GetAllAssets();
            _selectedAssets = String.Empty;

            foreach (Asset asset in _cachedAssetList)
            {
                _selectedAssets += asset.Name + ";";
            }

            char[] charsToTrim = { ';' };
            _selectedAssets.TrimEnd(charsToTrim);

            ResetSelectedAssets();

			// ...then create InternetReportEntry objects for each returned and add to the view
			foreach (DataRow row in internetTable.Rows)
			{
				// Create the object for this Internet record
				InternetReportEntry ieEntry = new InternetReportEntry(row);
				
				// Check for this being filtered by location/asset
				if (FilterRecord(ieEntry))
					AddRecord(ieEntry);
			}
			
		}


		/// <summary>
		/// This function is called to add an application to the DataSet noting that the
		/// DataSet actually has 3 separate (but linked) tables for Applications, Instances and Licenses
		/// </summary>
		/// <param name="thisApplication"></param>
		protected void AddRecord(InternetReportEntry record)
		{
			// Get the Table from the DataSet
			DataTable internetTable = _reportDataSet.Tables["Internet"];			
			
			// Add the row to the data set
			try
			{
				internetTable.Rows.Add(new object[]
					{ //record
					//, record.Location
					 record.AssetName
					//, record.Source
					//, record.Date.ToShortDateString()
					, record.Date.ToString("yyyy-MM-dd")
					, record.Url
					, record.PagesAccessed.ToString()});
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}


		/// <summary>
		/// Should this record be included in the report based on teh selected locations and/or assets
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected bool FilterRecord (InternetReportEntry record)
		{
            // check data filter first
            if ((!_showAllDates) && (record.Date < _startDate.Date || record.Date > _endDate.Date))
                return false;

			// True if no filters applied
			if ((_selectedGroups == "") && (_selectedAssets == ""))
				return true;
				
			// Check locations specified
			foreach (string group in SelectedGroupsList)
			{
				if (record.Location == group)
					return true;
			}
			
			// Not in the groups list so check the assets list also
			foreach (string assetName in SelectedAssetsList)
			{
				if (record.AssetName == assetName)
					return true;
			}
			return false;
		}
		


		/// <summary>
		/// Perform general configuration of the grid to match the report being displayed
		/// </summary>
		/// <param name="grid"></param>
		protected void InitializeGrid(UltraGrid grid)
		{
			// Hide the 'object' column in the grid
            //UltraGridBand internetBand = grid.DisplayLayout.Bands[0];
            //internetBand.Columns["object"].Hidden = true;
			grid.Tag = this;
		}


		/// <summary>
		/// Sets the specified icon for the first visible column in this row
		/// </summary>
		/// <param name="thisRow"></param>
		/// <param name="icon"></param>
		private void SetFirstVisibleColumnIcon(UltraGridRow thisRow, Bitmap icon)
		{
			// Iterate through the columns until we find one that is visible
			foreach (UltraGridCell theCell in thisRow.Cells)
			{
				if (!theCell.Hidden)
				{
					theCell.Appearance.Image = icon;
					break;
				}
			}
		}


		#region READER Functions

		/// <summary>
		/// Read the Audit Scanner Configuration File into our internal buffer
		/// </summary>
		/// <returns></returns>
		public override bool ReadReport()
		{
			// Create a log file
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("Reading Report Definition from " + _filename, true);

			// Erase the default sections
			_listReportSections.Clear();

			XmlTextReader textReader;
			XmlSimpleElement xmlSimpleElement = new XmlSimpleElement("junk");
			XmlParser xmlParser;

			// First of all parse the file
			try
			{
				textReader = new XmlTextReader(_filename);
				xmlParser = new XmlParser();
				xmlSimpleElement = xmlParser.Parse(textReader);
				textReader.Close();
			}

			catch (Exception ex)
			{
				ourLog.Write("Exception occurred while report definition file, the error was " + ex.Message, true);
				return false;
			}

			// If we can't find the 'Report Defintion' element then this is NOT a valid file
			_isValidFile = (xmlSimpleElement.TagName == S_REPORT_DEFINITION);
			if (!_isValidFile)
			{
				ourLog.Write("The [" + S_REPORT_DEFINITION + "] section was not found and therefore the file is invalid", true);
				return false;
			}

			// Now iterate through the data recovered 
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				ProcessElementRead(childElement);
			}

			// Flag that we read this definition from a file
			ReadFromFile = true;

			return true;
		}


		/// <summary>
		/// Called as we parse a top level element from the configuration file
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string elementName = xmlSimpleElement.TagName;

			// OK what sort of element is it?
			switch (elementName)
			{
				case S_REPORT_DEFINITION:
					break;

				case S_GENERAL:
					ReadGeneral(xmlSimpleElement);
					break;

				case S_REPORT_SECTIONS:
					ReadReportSections(xmlSimpleElement);
					break;

				case S_REPORT_LABELS:
					ReadReportLabels(xmlSimpleElement);
					break;

				case S_SCOPE:
					ReadScope(xmlSimpleElement);
					break;

				case S_FILTERS:
					ReadFilters(xmlSimpleElement);
					break;

				default:
					break;
			}
			return;
		}


		/// <summary>
		/// Process the FILTERS Section
		/// </summary>
		/// <param name="xmlSimpleElement"></param>
		protected void ReadFilters(XmlSimpleElement xmlSimpleElement)
		{
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case V_FILTERS_STARTDATE:
						_startDate = Convert.ToDateTime(childElement.Text);
						break;

					case V_FILTERS_ENDDATE:
						_endDate = Convert.ToDateTime(childElement.Text);
						break;

					case V_FILTERS_URL:
						_filterURL = childElement.Text;
						break;

					default:
						break;
				}
			}
		}


		#endregion READER Functions


		#region REPORT WRITER Functions

		/// <summary>
		/// write the XML Report Definition file using the supplied data.
		/// This over-rides the abstract implementation in the base class as the write is always specific 
		/// to the report
		/// </summary>
		/// <returns></returns>
		public override int WriteReport()
		{
			try
			{
				// First of all create the object
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.OmitXmlDeclaration = false;

				using (XmlTextWriterEx writer = new XmlTextWriterEx(_filename, null))
				{
					// Add the header
					writer.Formatting = Formatting.Indented;
					writer.WriteComment("AuditWizard Report Definition File");

					// Now the Scanner Configuration Section
					writer.StartSection(S_REPORT_DEFINITION);

					// Add the 'General' section (in the base class)
					SaveGeneral(writer);

					// Add the 'Scope' section (in the base class)
					SaveScope(writer);

					// Now write our sections specific to this report 
					SaveFilters(writer);
					
					// Tidy up
					writer.Flush();
					writer.Close();
				}
			}
			catch (Exception ex)
			{
				String error = ex.Message;
				return -1;
			}

			return 0;
		}



		/// <summary>
		/// Save settings into the FILTERS section of the Report Definition file
		/// </summary>
		/// <param name="writer"></param>
		private void SaveFilters(XmlTextWriterEx writer)
		{
			writer.StartSection(S_FILTERS);
			//
			if (_startDate.Ticks != 0)
				writer.WriteSetting(V_FILTERS_STARTDATE, _startDate.ToString());
			if (_endDate.Ticks != 0)
				writer.WriteSetting(V_FILTERS_ENDDATE ,_endDate.ToString());
			if (_filterURL != "")
				writer.WriteSetting(V_FILTERS_URL ,_filterURL);
			//			
			writer.EndSection();
		}

		#endregion WRITER Functions
			
				
		#endregion Methods

	}
}
