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
	public class FileSystemReportDefinition : ReportDefinition
	{
		#region Data

		bool _showBasicInformation;
		bool _showVersionInformation;
		bool _showDateInformation;
		
		/// <summary>Only include files where the name matches the wildcard</summary>
		string _filterFile;
		
		#endregion data

		#region properties

		public bool ShowBasicInformation
		{
			get { return _showBasicInformation; }
			set { _showBasicInformation = value; }
		}

		public bool ShowVersionInformation
		{
			get { return _showVersionInformation; }
			set { _showVersionInformation = value; }
		}

		public bool ShowDateInformation
		{
			get { return _showDateInformation; }
			set { _showDateInformation = value; }
		}

		public string FilterFile
		{
			get { return _filterFile; }
			set { _hasChanged = _hasChanged || (_filterFile != value); _filterFile = value; }
		}
		
		#endregion properties
		
		#region XMLStrings

		// Filters
		private const string S_FILTERS				= "Filters";
		private const string V_FILTERS_FILE			= "File";
		private const string V_FILTERS_SHOWBASIC	= "ShowBasic";
		private const string V_FILTERS_SHOWVERSION	= "ShowVersion";
		private const string V_FILTERS_SHOWDATE		= "ShowDate";

		#endregion

		#region Constructor
		
		public FileSystemReportDefinition() : base()
		{
			// Set the default name for this report in the header section
			ExportSection headerSection = GetReportSection(ExportSection.eSectionType.header);
			headerSection.FormattedText.RawText = "Files Report";
			//
			_name = "Files Report";
			_reportType = eReportType.filesystem;
			_showBasicInformation = true;
			_showDateInformation = false;
			_showVersionInformation = false;
			_filterFile = "";
			
			// Initialize the data set
			_reportDataSet.DataSetName = "filesDataSet";
		}
		
		#endregion Constructor

		#region Methods

		/// <summary>
		/// This function is responsible for the actual generation of the Files Report
		/// It overrides the abstract definition in the base class and stores it's data in the DataSet
		/// </summary>
		public override void GenerateReport(UltraGrid grid)
		{

			// Create a new dataSource 
			_reportDataSet = new DataSet("auditdataDataSet");
			grid.DataSource = _reportDataSet;

			// Create the tables, columns and relationships
			CreateTables();

			// Clear any existing data out of the dataset
			//_reportDataSet.Tables["Files"].Rows.Clear();

			// Set the grid datasource to be this dataset
			//grid.DataSource = _reportDataSet;
							
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
			SetFirstVisibleColumnIcon(gridRow, Properties.Resources.file_16);
		}


		/// <summary>
		/// Create the tables, columns, relationships and foreign keys required for this report
		/// </summary>
		protected void CreateTables()
		{
			// Only 1 table is required for the Internet report
			_reportDataSet.Tables.Clear();
			//
			DataTable filesTable = new DataTable("Files");

			// begin the initialization of the dataset tables
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(filesTable)).BeginInit();

			// Add the table to the Database
			_reportDataSet.Tables.AddRange(new System.Data.DataTable[] { filesTable });		
		
			// Create the FILES table
			// Columns are :- Location ,Asset ,FileName 
			DataColumn c1 = new DataColumn("object", typeof(object));
			DataColumn c2 = new DataColumn("Location");
			DataColumn c3 = new DataColumn("Asset Name");
			DataColumn c4 = new DataColumn("File Name");

			// Add these columns to the tabls, one at a time
			filesTable.Columns.Add(c1);
			filesTable.Columns.Add(c2);
			filesTable.Columns.Add(c3);
			filesTable.Columns.Add(c4);

			// Add on additional tables based on the options set - first basic options gives us
			//	PATH, SIZE ,PUBLISHER and APPLICATION
			if (_showBasicInformation)
			{
				DataColumn b1 = new DataColumn("Path");
				DataColumn b2 = new DataColumn("Size (bytes)" ,typeof(int));
				DataColumn b3 = new DataColumn("Publisher");
				DataColumn b4 = new DataColumn("Application");

				// Add these columns to the tabls
				filesTable.Columns.Add(b1);
				filesTable.Columns.Add(b2);
				filesTable.Columns.Add(b3);
				filesTable.Columns.Add(b4);
			}

			// Version Information 
			if (_showVersionInformation)
			{
				DataColumn v1 = new DataColumn("Product Version 1");
				DataColumn v2 = new DataColumn("Product Version 2");
				DataColumn v3 = new DataColumn("File Version 1");
				DataColumn v4 = new DataColumn("File Version 2");

				// Add these columns to the tabls
				filesTable.Columns.Add(v1);
				filesTable.Columns.Add(v2);
				filesTable.Columns.Add(v3);
				filesTable.Columns.Add(v4);
			}

			// Date Information 
			if (_showDateInformation)
			{
				DataColumn d1 = new DataColumn("Created Date" ,typeof(DateTime));
				DataColumn d2 = new DataColumn("Last Accessed Date" ,typeof(DateTime));
				DataColumn d3 = new DataColumn("Modified Date" ,typeof(DateTime));

				// Add these columns to the tabls
				filesTable.Columns.Add(d1);
				filesTable.Columns.Add(d2);
				filesTable.Columns.Add(d3);
			}			
						
			// End initialization of the DataSet and Table
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(filesTable)).EndInit();			
		}		



		/// <summary>
		/// This function is responsible for generating the actual data which will be displayed by the report
		/// </summary>
		protected void GenerateReportData()
		{
			// Create a string representation of the publisher filter list passed to us
			// We need to get the entire licensing information at this point
			FileSystemDAO lwDataAccess = new FileSystemDAO();
			DataTable dataTable = lwDataAccess.EnumerateFileSystemFiles(_filterFile);

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
            //AssetList _cachedAssetList = _cachedAssetGroups.GetAllAssets();
            //_selectedAssets = String.Empty;

            //foreach (Asset asset in _cachedAssetList)
            //{
            //    _selectedAssets += asset.Name + ";";
            //}

            //char[] charsToTrim = { ';' };
            //_selectedAssets.TrimEnd(charsToTrim);

            _selectedAssets = new AssetDAO().ConvertIdListToNames(new AssetDAO().GetSelectedAssets(), ',');

            ResetSelectedAssets();

			// ...then create InternetReportEntry objects for each returned and add to the view
			foreach (DataRow row in dataTable.Rows)
			{
				FileSystemFile file = new FileSystemFile(row);
				
				// Check for this being filtered by location/asset
				if (FilterRecord(file))
					AddRecord(file);
			}
			
		}


		/// <summary>
		/// This function is called to add an application to the DataSet noting that the
		/// DataSet actually has 3 separate (but linked) tables for Applications, Instances and Licenses
		/// </summary>
		/// <param name="thisApplication"></param>
		protected void AddRecord(FileSystemFile record)
		{
			// Get the Table from the DataSet
			DataTable dataTable = _reportDataSet.Tables["Files"];			
			
			// Create a Data row
			DataRow newRow = dataTable.NewRow();
			
			// Add on the common columns
			newRow["object"] = record;
			newRow["Location"] = record.Location;
			newRow["Asset Name"] = record.AssetName;
			newRow["File Name"] = record.Name;

			// Do we need the basic information
			if (_showBasicInformation)
			{
				newRow["Path"] = record.ParentName;
				newRow["Size (bytes)"] = record.Size;
				newRow["Publisher"] = record.Publisher;
				newRow["Application"] = record.ProductName;
			}

			// Version Information 
			if (_showVersionInformation)
			{
				newRow["Product Version 1"] = record.ProductVersion1;
				newRow["Product Version 2"] = record.ProductVersion2;
				newRow["File Version 1"] = record.FileVersion1;
				newRow["File Version 2"] = record.FileVersion2;
			}

			// Date Information 
			if (_showDateInformation)
			{
				newRow["Created Date"] = record.CreatedDateTime.ToString();
				newRow["Last Accessed Date"] = record.LastAccessedDateTime.ToString();
				newRow["Modified Date"] = record.ModifiedDateTime.ToString();
			}
			
			// Add the row
			dataTable.Rows.Add(newRow);
		}


		/// <summary>
		/// Should this record be included in the report based on the selected locations and/or assets
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected bool FilterRecord(FileSystemFile record)
		{
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
					case V_FILTERS_SHOWBASIC:
						_showBasicInformation = childElement.TextAsBoolean;
						break;

					case V_FILTERS_SHOWDATE:
						_showDateInformation = childElement.TextAsBoolean;
						break;

					case V_FILTERS_SHOWVERSION:
						_showVersionInformation = childElement.TextAsBoolean;
						break;

					case V_FILTERS_FILE:
						_filterFile = childElement.Text;
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
			writer.WriteSetting(V_FILTERS_SHOWBASIC, _showBasicInformation.ToString());
			writer.WriteSetting(V_FILTERS_SHOWDATE,  _showDateInformation.ToString());
			writer.WriteSetting(V_FILTERS_SHOWVERSION, _showVersionInformation.ToString());

			if (_filterFile != "")
				writer.WriteSetting(V_FILTERS_FILE, _filterFile);
			//			
			writer.EndSection();
		}

		#endregion WRITER Functions
			
				
		#endregion Methods

	}
}
