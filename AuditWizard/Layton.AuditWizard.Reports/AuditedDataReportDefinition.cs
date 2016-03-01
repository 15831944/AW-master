using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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
    /// Thjis class encapsulates an Internet Usage Report Definition
    /// </summary>
    public class AuditedDataReportDefinition : ReportDefinition
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;

        /// <summary>This is the list of fields (names) to be included in the report</summary>
        List<string> _listSelectedFields;

        /// <summary>
        /// This field when set alters the display so that it appears as an Asset register - what this means is
        /// that we have two tables - the first contains JUST the asset name whereas the second contains two 
        /// columns - Field and Value with the data fields displayed as rows
        /// </summary>
        private bool _showAssetRegister;

        /// <summary>
        /// This dictionary contains all of the fields currently defined within the data Set
        /// </summary>
        private AuditDataReportColumnList _auditDataReportColumns = new AuditDataReportColumnList();

        /// <summary>
        /// Publisher filters are loaded each time the report is loaded and are transient
        /// </summary>
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;

        #region Cached Data

        /// <summary>
        /// We cache some data here as it may be used multiple times throughout the running of a report
        /// </summary>
        protected AssetGroup _cachedAssetGroups = null;

        /// <summary>The cached asset list is built from the above list but simply contains a list of assets</summary>
        protected AssetList _cachedAssetList = null;

        #endregion Cached Data

        #endregion data

        #region properties

        public bool ShowIncluded
        {
            set { _showIncluded = value; }
        }

        public bool ShowIgnored
        {
            set { _showIgnored = value; }
        }

        public List<string> SelectedFields
        {
            get { return _listSelectedFields; }
            set { _listSelectedFields = value; }
        }

        public bool ShowAssetRegister
        {
            get { return _showAssetRegister; }
            set { _showAssetRegister = value; }
        }

        #endregion properties

        #region XMLStrings

        // Filters
        private const string S_FILTERS = "Filters";
        private const string V_FILTERS_SELECTEDFIELDS = "SelectedFields";
        private const string V_FILTERS_SELECTEDFIELD = "SelectedField";
        private const string V_FILTERS_SHOWASSETREGISTER = "ShowAsAssetRegister";

        #endregion

        #region Constructor

        public AuditedDataReportDefinition()
            : base()
        {
            // Set the default name for this report in the header section
            ExportSection headerSection = GetReportSection(ExportSection.eSectionType.header);
            headerSection.FormattedText.RawText = "Audited Data Report";
            _name = "Audited Data Report";
            _reportType = eReportType.auditeddata;

            // Allocate the list of selected fields and pre-select that Asset Name
            _listSelectedFields = new List<string>();
            _listSelectedFields.Add(AWMiscStrings.AssetDetails + "|" + Asset.GetAttributeName(Asset.eAttributes.assetname));

            // Initialize the data set
            _reportDataSet.DataSetName = "auditdataDataSet";
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// This function is responsible for the actual generation of the Applications Report
        /// It overrides the abstract definition in the base class and stores it's data in the DataSet
        /// </summary>
        public override void GenerateReport(UltraGrid grid)
        {
            // Delete any cached data as we are re-running the report
            _cachedAssetGroups = null;

            // Begin initialization of the Grid
            grid.BeginUpdate();

            // Save the grid layout to a temporary file
            if (grid.Rows.Count != 0)
                SaveTemporaryLayout(grid);

            // Create a new dataSource 
            _reportDataSet = new DataSet("auditdataDataSet");
            grid.DataSource = _reportDataSet;

            // We will always need the list of assets so we may as well get it now
            LocationsDAO lwDataAccess = new LocationsDAO();
            AssetGroup.GROUPTYPE displayType = AssetGroup.GROUPTYPE.userlocation;
            DataTable table = lwDataAccess.GetGroups(new AssetGroup(displayType));
            _cachedAssetGroups = new AssetGroup(table.Rows[0], displayType);
            _cachedAssetGroups.Populate(true, _ignoreChildAssets, true);

            // Now apply the filter to these groups
            _cachedAssetGroups.ApplyFilters(_selectedGroups, _selectedAssets, _ignoreChildAssets);

            // Now that we have a definitive list of the assets (as objects) which we want to include in the
            // report we could really do with expanding this list so that ALL of the assets are in a single list
            // and not distributed among the publishers
            _cachedAssetList = _cachedAssetGroups.GetAllAssets();

            // Create the list of report columns which will maintain the data for this report
            _auditDataReportColumns.Populate(_listSelectedFields
                                            , _dictionaryLabels
                                            , _publisherFilter
                                            , _showIncluded
                                            , _showIgnored);

            // Create the tables, columns and relationships as these may have changed since we loaded the report
            CreateTables();

            // Clear any existing data out of the dataset
            _reportDataSet.Tables["AuditData"].Rows.Clear();

            // Generate the data for the report
            GenerateReportData();

            // reload the temprary layout saved around the report generation
            //LoadTemporaryLayout(grid);

            // ...and perform any required initialization of the grid
            InitializeGrid(grid);

            grid.EndUpdate();
        }



        /// <summary>
        /// Called to perform any row specific initailization as the row is displayed
        /// </summary>
        /// <param name="gridRow"></param>
        public override void InitializeGridRow(UltraGridRow gridRow)
        {
        }


        /// <summary>
        /// Create the tables, columns, relationships and foreign keys required for this report
        /// </summary>
        protected void CreateTables()
        {
            // begin the initialization of the dataset tables
            ((System.ComponentModel.ISupportInitialize)(_reportDataSet)).BeginInit();

            // Delete any existing tables in the DataSet
            _reportDataSet.Tables.Clear();

            // Only 1 table is required regardless of whether we are displaying in 'Standard' or 
            // 'Asset register' format - its just the columns which differ
            DataTable dataTable = new DataTable("AuditData");
            ((System.ComponentModel.ISupportInitialize)(dataTable)).BeginInit();

            // Add the table to the Database
            _reportDataSet.Tables.AddRange(new System.Data.DataTable[] { dataTable });

            // Are we generating a 'standard' or an asset register report?
            if (_showAssetRegister)
                CreateAssetRegisterTables(dataTable);
            else
                CreateStandardTables(dataTable);

            // End initialization of the DataSet and Table
            ((System.ComponentModel.ISupportInitialize)(_reportDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(dataTable)).EndInit();
        }




        /// <summary>
        /// Create the columns required within an 'Asset Register' report.
        /// We have 3 columns only - Asset Name , Data Field and Value
        /// </summary>
        protected void CreateAssetRegisterTables(DataTable dataTable)
        {
            // Create the 3 columns required by this report format - Asset Name , Data Field and Value
            DataColumn column1 = new DataColumn("Asset Name");
            DataColumn column2 = new DataColumn("Field");
            DataColumn column3 = new DataColumn("Value");

            // ...and add these to the data table
            dataTable.Columns.AddRange(new DataColumn[] { column1, column2, column3 });
        }


        /// <summary>
        /// Create the columns required in teh report for a 'Standard' report
        /// </summary>
        protected void CreateStandardTables(DataTable dataTable)
        {
            try
            {
                // In the 'Standard' display we have a column for each data field in the report and 
                // create a row of data for the values of these columns.
                //
                // Create the columns based on the fields which we have previously expanded out
                foreach (AuditDataReportColumn reportColumn in _auditDataReportColumns)
                {
                    DataColumn column = new DataColumn(reportColumn.FieldName);
                    column.Caption = reportColumn.ColumnLabel;
                    if (!dataTable.Columns.Contains(column.ColumnName))
                        dataTable.Columns.Add(column);
                }
            }

            catch (Exception ex)
            {
                logger.Error("Error in CreateStandardTables()", ex);
                MessageBox.Show("Failed to add columns to the data table");
            }
        }


        #region Report Data Generation Functions

        /// <summary>
        /// This function is responsible for generating the actual data which will be displayed by the report
        /// We need to create one value for each asset per field in the report
        /// </summary>
        protected void GenerateReportData()
        {
            _auditDataReportColumns.GenerateReportData(_cachedAssetList);

            // Now that we have the data we can populate the report data set itself
            // How we do this is determined by whether we have a Standard or Asset Register Format
            if (_showAssetRegister)
                PopulateAssetRegisterReport();
            else
                PopulateStandardReport();
        }

        /// <summary>
        /// Populate the data set for an 'Asset Register' format report where there are only 3 columns
        /// in the report - Asset Name , Field and Value and the rows are the fields and values listed
        /// vertically grouped beneath their asset.
        /// </summary>
        protected void PopulateAssetRegisterReport()
        {
            // Get the Table from the DataSet
            DataTable dataTable = _reportDataSet.Tables["AuditData"];

            // OK - this report is ASSET based so we loop through each asset listing the field and value
            // for each in turn.
            foreach (Asset asset in _cachedAssetList)
            {
                // Loop through each field in the report and create a row containing the
                // asset name ,field name and value	
                foreach (AuditDataReportColumn reportColumn in _auditDataReportColumns)
                {
                    // Get the value of this column for this asset
                    AuditDataReportColumnValue columnValue = reportColumn.GetValueForAsset(asset.AssetID);

                    if (columnValue != null)
                    {
                        // In an ASSET REGISTER report we have special handling for APPLICATIONS in that we do NOT
                        // include them at all if the application is determined to not be installed on this asset
                        if ((reportColumn.FieldType != AuditDataReportColumn.eFieldType.applications) || ((string)columnValue.DataValue != AuditDataReportColumn.ApplicationNotInstalled))
                        {
                            // Create a new row in the report
                            dataTable.Rows.Add(new object[] { asset.Name, reportColumn.ShortFieldName, (string)columnValue.DataValue });
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Populate the data set for a 'Standard' format report where the fields in the report 
        /// are the columns and the values for those fields form the rows
        /// </summary>
        protected void PopulateStandardReport()
        {
            // Get the Table from the DataSet
            DataTable dataTable = _reportDataSet.Tables["AuditData"];

            // Count of rows is number of assets
            int rowCount = _cachedAssetList.Count - 1;

            // Loop for each row
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                // Create a new row
                DataRow newRow = dataTable.NewRow();

                // ...and add the columns to that row taking the values from the report columns
                int columnIndex = 0;

                foreach (AuditDataReportColumn reportColumn in _auditDataReportColumns)
                {
                    AuditDataReportColumnValue columnValue = reportColumn.Values[rowIndex];

                    try
                    {
                        newRow[columnIndex] = columnValue.DataValue;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                    columnIndex++;
                }

                // Add the row to the table
                dataTable.Rows.Add(newRow);
            }
        }


        #endregion Report Data Generation Functions


        /// <summary>
        /// Perform general configuration of the grid to match the report being displayed
        /// </summary>
        /// <param name="grid"></param>
        protected void InitializeGrid(UltraGrid grid)
        {
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
        protected void ReadFilters(XmlSimpleElement reader)
        {
            // Clear the existing list of selected fields
            _listSelectedFields.Clear();

            // We should only be able to find a 'SELECTEDFIELDS' section here
            foreach (XmlSimpleElement childElement in reader.ChildElements)
            {
                switch (childElement.TagName)
                {
                    case V_FILTERS_SELECTEDFIELDS:
                        ReadSelectedFields(childElement);
                        break;

                    case V_FILTERS_SHOWASSETREGISTER:
                        _showAssetRegister = childElement.TextAsBoolean;
                        break;
                }
            }
        }


        /// <summary>
        /// The 'SelectedFields' section should consist of 1 or mnore field definitions
        /// </summary>
        /// <param name="xmlSimpleElement"></param>
        protected void ReadSelectedFields(XmlSimpleElement reader)
        {
            // We should only be able to find a 'SELECTEDFIELDS' section here
            foreach (XmlSimpleElement childElement in reader.ChildElements)
            {
                switch (childElement.TagName)
                {
                    case V_FILTERS_SELECTEDFIELD:
                        _listSelectedFields.Add(childElement.Text);
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

            // Write the 'Show as Asset Register' first
            writer.WriteSetting(V_FILTERS_SHOWASSETREGISTER, _showAssetRegister);

            // We write the fields into a SelectedFields section each within their own SelectedField section
            writer.StartSection(V_FILTERS_SELECTEDFIELDS);

            foreach (string item in _listSelectedFields)
            {
                writer.WriteSetting(V_FILTERS_SELECTEDFIELD, item);
            }

            // End of SelectedFields Section
            writer.EndSection();

            // End of 'Filters' section
            writer.EndSection();
        }

        #endregion WRITER Functions


        #endregion Methods

    }
}
