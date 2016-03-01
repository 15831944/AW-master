using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Xml;
//
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
	/// <summary>
	/// Thjis class encapsulates an Audit History Report Definition
	/// </summary>
	public class HistoryReportDefinition : ReportDefinition
	{
		#region Data
		
		/// <summary>
		/// These are the types of history report which can be executed
		/// </summary>
		public enum eHistoryType { lastaudit ,hasbeenaudited ,hasnotbeenaudited ,mostrecentchanges ,changesbetween };

		protected eHistoryType _subtype;
		
		/// <summary>Only show records since the start date</summary>
		protected DateTime		_startDate;
		
		/// <summary>Only show records prior to the end date</summary>
		protected DateTime		_endDate;
		
		/// <summary>Show the number of days which have elapsed since the last audit</summary>
		protected int			_days;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
		
		#endregion data

		#region properties

		public eHistoryType SubType
		{
			get { return _subtype; }
			set { _hasChanged = _hasChanged || (_subtype != value); _subtype = value; }
		}
		
		public void SetDateRange (DateTime startDate ,DateTime endDate)
		{
			_hasChanged = _hasChanged || ((_startDate != startDate) || (_endDate != endDate));
			_startDate = startDate;
			_endDate = endDate;
		}

		public int Days
		{
			get { return _days; }
			set { _hasChanged = _hasChanged || (_days != value); _days = value; }
		}
		
		#endregion properties
		
		#region XMLStrings

		// Filters
		private const string S_FILTERS			= "Filters";
		private const string V_FILTERS_STARTDATE = "StartDate";
		private const string V_FILTERS_ENDDATE	= "EndDate";
		private const string V_FILTERS_DAYS		= "Days";
		private const string V_FILTER_TYPE		= "Type";

		#endregion

		#region Constructor
		
		public HistoryReportDefinition() : base()
		{
			// Set the default name for this report in the header section
			ExportSection headerSection = GetReportSection(ExportSection.eSectionType.header);
			headerSection.FormattedText.RawText = "Audit History Report";
			//
			_name = "Audit History Report";
			_reportType = eReportType.history;
			_subtype = eHistoryType.lastaudit;
			_startDate = new DateTime(0);
			_endDate = new DateTime(0);
			_days = 7;
						
			// Initialize the data set
			_reportDataSet.DataSetName = "historyDataSet";

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
			_reportDataSet.Tables["History"].Rows.Clear();

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
			SetFirstVisibleColumnIcon(gridRow, Properties.Resources.audittrail_16);
		}


		/// <summary>
		/// Create the tables, columns, relationships and foreign keys required for this report
		/// </summary>
		protected void CreateTables()
		{
			// Only 1 table is required for the Internet report
			DataTable internetTable = new DataTable("History");

			// begin the initialization of the dataset tables
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(internetTable)).BeginInit();

			// Add the table to the Database
			_reportDataSet.Tables.AddRange(new System.Data.DataTable[] { internetTable });		
		
			// Create the INTERNET table
			// Columns are :- Location ,Asset ,Date ,URL ,Page Count
			DataColumn column1 = new DataColumn("object", typeof(object));
			DataColumn column2 = new DataColumn("Location");
			DataColumn column3 = new DataColumn("Asset Name");
			DataColumn column4 = new DataColumn("Date");
			DataColumn column5 = new DataColumn("Days" ,typeof(int));
			DataColumn column6 = new DataColumn("Category");
			DataColumn column7 = new DataColumn("Operation");

			// Add these columns to the tabls
			internetTable.Columns.AddRange(new System.Data.DataColumn[] { column1, column2, column3, column4, column5, column6, column7 });
	
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

			// OK different reports require different processing so branch here
			switch (_subtype)
			{
				case eHistoryType.changesbetween:
					GenerateChangesBetweenReportData();
					break;

				case eHistoryType.hasbeenaudited:
					GenerateLastAuditDateReportData(true);
					break;
				
				case eHistoryType.hasnotbeenaudited:
					GenerateLastAuditDateReportData(false);
					break;
								
				case eHistoryType.lastaudit:
					GenerateLastAuditDateReportData(true);
					break;
					
				case eHistoryType.mostrecentchanges:
					GenerateMostRecentReportData();
					break;
			}
		}
		
	
	
		/// <summary>
		/// This report will contain information on asset history records dated between the specified start
		/// and end dates
		/// </summary>
		protected void GenerateChangesBetweenReportData()
		{			
			// Create a string representation of the publisher filter list passed to us
			// We need to get the entire licensing information at this point
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();
			DataTable historyTable = lwDataAccess.GetAssetAuditHistory(new Asset(), _startDate ,_endDate);
			
			// For each row in the returned table we need to first see if we need to filter it based on 
			// and locations/assets specified 
			foreach (DataRow row in historyTable.Rows)
			{
				// Create the object for this History record
				AuditTrailEntry entry = new AuditTrailEntry(row);
				
				// Check for this being filtered by location/asset
				if (FilterRecord(entry.Location ,entry.AssetName))
					AddChangesRecord(entry);
			}		
		}


		/// <summary>
		/// This function is called to add an audit trail entry to the DataSet 
		/// </summary>
		/// <param name="thisApplication"></param>
		protected void AddChangesRecord(AuditTrailEntry record)
		{
			// Get the Table from the DataSet
			DataTable historyTable = _reportDataSet.Tables["History"];			
			
			// Add the row to the data set
			try
			{
				historyTable.Rows.Add(new object[]
					{ record
					, record.Location
					, record.AssetName
					, record.Date.ToShortDateString() 
					, 0										// Days not applicable to this report
					, record.ClassString
					, record.GetTypeDescription() } );
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}


		/// <summary>
		/// This report is called to identify those assets which have (or have not) been audited 
		/// within the specified period.
		/// </summary>
		protected void GenerateLastAuditDateReportData(bool hasBeenAudited)
		{
            try
            {
                // Get the Table from the DataSet
                DataTable historyTable = _reportDataSet.Tables["History"];

                // Create a string representation of the publisher filter list passed to us
                // We need to get the entire licensing information at this point
                AuditTrailDAO lwDataAccess = new AuditTrailDAO();
                DataTable dataTable = lwDataAccess.GetAssetLastAuditDate(new Asset(), _days, hasBeenAudited);                

                // For each row in the returned table we need to first see if we need to filter it based on 
                // and locations/assets specified 
                foreach (DataRow row in dataTable.Rows)
                {
                    // We don't have an explict record for this type so just unpack the fields
                    string assetName = (string)row["ASSETNAME"];
                    string location = (string)row["FULLLOCATIONNAME"];

                    DateTime date;
                    int elapsedDays;
                    string displayDate;

                    if (row["_DATE"].GetType() == typeof(DBNull))
                    {
                        displayDate = String.Empty;
                        elapsedDays = 0;
                    }
                    else
                    {
                        date = (DateTime)row["_DATE"];
                        displayDate = date.ToShortDateString() + " " + date.ToLongTimeString();
                        elapsedDays = ((TimeSpan)(DateTime.Now - date)).Days;
                    }

                    // Check for this being filtered by location/asset
                    if (FilterRecord(location, assetName))
                    {
                        // Add the row to the data set
                        try
                        {
                            historyTable.Rows.Add(new object[]
							{ null
							, location
							, assetName
							, displayDate 
							, elapsedDays
							, "n/a"
							, "n/a" });
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in GenerateLastAuditDateReportData()", ex);
                Utility.DisplayApplicationErrorMessage("There has been an error creating the report." + Environment.NewLine + Environment.NewLine +
                    "Please see the log file for further information.");
            }
		}


		/// <summary>
		/// generate the most recent history records for each asset
		/// </summary>
		protected void GenerateMostRecentReportData()
		{
			// Get a complete list of groups and assets and then apply our filters to this list
			LocationsDAO lwDataAccess = new LocationsDAO();
			AssetGroup.GROUPTYPE displayType = AssetGroup.GROUPTYPE.userlocation;
			DataTable table = lwDataAccess.GetGroups(new AssetGroup(displayType));
			AssetGroup assetGroups = new AssetGroup(table.Rows[0], displayType);
			assetGroups.Populate(true, false, true);

			// Now apply the filter to these groups
			assetGroups.ApplyFilters(_selectedGroups, _selectedAssets ,_ignoreChildAssets);

			// Now that we have a definitive list of the assets (as objects) which we want to include in the
			// report we could really do with expanding this list so that ALL of the assets are in a single list
			// and not distributed among the publishers
			AssetList listAssets = assetGroups.GetAllAssets();
			
			// OK - get the last audit trail records for these assets - the last audit date is stored in the
			// Asset object so we don't need to get that again
			foreach (Asset asset in listAssets)
			{
				// Skip any assets not audited yet
				if (asset.LastAudit.Ticks == 0)
					continue;

				// Get the audit trail records for this asset
				DataTable historyTable = new AuditTrailDAO().GetAssetAuditHistory(asset, asset.LastAudit ,DateTime.Now);

				// Add the entries in the data table as ATE records to our DataSet
				foreach (DataRow row in historyTable.Rows)
				{
					AuditTrailEntry ate = new AuditTrailEntry(row);
					AddChangesRecord(ate);
				}			
			}
		}


		/// <summary>
		/// Should this record be included in the report based on the selected locations and/or assets
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected bool FilterRecord(string location ,string assetName)
		{
			// True if no filters applied
			if ((_selectedGroups == "") && (_selectedAssets == ""))
				return true;
				
			// Check locations specified
			foreach (string group in SelectedGroupsList)
			{
				if (location == group)
					return true;
			}
			
			// Not in the groups list so check the assets list also
			foreach (string name in SelectedAssetsList)
			{
				if (assetName == name)
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
			grid.Tag = this;
            //try
            //{
            //    UltraGridBand band = grid.DisplayLayout.Bands[0];
            //    band.Columns["object"].Hidden = true;

            //    // Some reports need 'days' while others need 'category and operation - set the columns
            //    // which we will display here
            //    if ((_subtype == eHistoryType.changesbetween) 
            //    ||  (_subtype == eHistoryType.mostrecentchanges))		
            //    {
            //        band.Columns["Days"].Hidden = true;
            //        band.Columns["Category"].Hidden = false;
            //        band.Columns["Operation"].Hidden = false;
            //    }
            //    else
            //    {
            //        band.Columns["Days"].Hidden = false;
            //        band.Columns["Category"].Hidden = true;
            //        band.Columns["Operation"].Hidden = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("An exception has occurred while initializing the grid for the History Report.  The error was : " + ex.Message, "Exception Error");
            //}
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

					case V_FILTER_TYPE:
						_subtype = (eHistoryType)childElement.TextAsInt;
						break;
						
					case V_FILTERS_DAYS:
						_days = childElement.TextAsInt;
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
			writer.WriteSetting(V_FILTER_TYPE, ((int)_subtype).ToString());
			//
			if (_startDate.Ticks != 0)
				writer.WriteSetting(V_FILTERS_STARTDATE, _startDate.ToString());
			//
			if (_endDate.Ticks != 0)
				writer.WriteSetting(V_FILTERS_ENDDATE ,_endDate.ToString());
			//
			if (_days != 0)	
				writer.WriteSetting(V_FILTERS_DAYS, _days.ToString());
			//			
			writer.EndSection();
		}

		#endregion WRITER Functions
			
				
		#endregion Methods

	}
}
