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

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// Thjis class encapsulates an Application Licensing Report Definition
	/// </summary>
	public class LicensingReportDefinition : ReportDefinition
	{
		#region Data
		
		/// <summary>This is a list of (any) publishers to limit the report to</summary>
		string	_selectedPublishers = "";

		/// <summary>This is a list of specific applications to limit the report to</summary>
		string _selectedApplications = "";
		
		/// <summary>Set to show instances of the selected applications in the data set</summary>
		bool _showInstanceDetails = true;

		/// <summary>Set to show details of any licenses declared for the selected applications in the data set</summary>
		bool	_showLicenses = true;

		/// <summary>Set to show only those applications which have a serial number and/or CD Key</summary>
		bool _showWithKeysOnly = false;
		
		/// <summary>Additional filters for included/ignored applications</summary>
		bool _showIncludedApplications = true;
        bool _showIgnoredApplications = true;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private List<int> selectedAssetList;
		
		#endregion data

		#region properties

		/// <summary>
		/// The following fields determine what details about the application are to be included
		/// </summary>
		public bool ShowInstanceDetails
		{
			get { return _showInstanceDetails; }
			set { _hasChanged = _hasChanged || (_showInstanceDetails != value); _showInstanceDetails = value; }
		}

		public bool ShowLicenses
		{
			get { return _showLicenses; }
			set { _hasChanged = _hasChanged || (_showLicenses != value); _showLicenses = value; }
		}

		/// <summary>
		/// The following fields further filtre the records which we are to show
		/// </summary>
		public bool ShowWithKeysOnly
		{
			get { return _showWithKeysOnly; }
			set { _hasChanged = _hasChanged || (_showWithKeysOnly != value); _showWithKeysOnly = value; }
		}

		public bool ShowIncludedApplications
		{
			get { return _showIncludedApplications; }
			set { _hasChanged = _hasChanged || (_showIncludedApplications != value); _showIncludedApplications = value; }
		}

		public bool ShowIgnoredApplications
		{
			get { return _showIgnoredApplications; }
			set { _hasChanged = _hasChanged || (_showIgnoredApplications != value); _showIgnoredApplications = value; }
		}

		public string SelectedPublishers
		{
			get { return _selectedPublishers; }
			set { _hasChanged = _hasChanged || (_selectedPublishers != value); _selectedPublishers = value; }
		}

		public string SelectedApplications
		{
			get { return _selectedApplications; }
			set { _hasChanged = _hasChanged || (_selectedApplications != value); _selectedApplications = value; }
		}
		
		
		#endregion properties
		
		#region XMLStrings

		// Filters
		private const string S_FILTERS = "Filters";
		private const string V_FILTERS_PUBLISHERS	= "Publishers";
		private const string V_FILTERS_APPLICATIONS = "Applications";
		private const string V_FILTERS_SHOWINSTANCES = "ShowInstances";
		private const string V_FILTERS_SHOWLICENSES = "ShowLicenses";
		private const string V_FILTERS_SHOWKEYSONLY = "ShowWithKeysOnly";

		#endregion

		#region Constructor
		
		public LicensingReportDefinition() : base()
		{	
			// Set the default name for this report in the header section
			ExportSection headerSection = GetReportSection(ExportSection.eSectionType.header);
			headerSection.FormattedText.RawText = "Application Licensing Report";
			//
			_name = "Application Licensing Report";
			_reportType = eReportType.licensing;
			_showLicenses = true;
			_showInstanceDetails = true;
			
			// Initialize the data set
			_reportDataSet.DataSetName = "licensingDataSet";

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
			// Begin initialization of the Grid
			grid.BeginUpdate();

			// Save the grid layout to a temporary file
			SaveTemporaryLayout(grid);

			// Clear any existing data out of the dataset
			_reportDataSet.Tables["Instances"].Rows.Clear();
			_reportDataSet.Tables["Licenses"].Rows.Clear();
			_reportDataSet.Tables["Applications"].Rows.Clear();

			// Set the grid datasource to be this dataset
			grid.DataSource = _reportDataSet;
							
			// Generate the data for the report
			GenerateReportData();

			// reload the temprary layout saved around the report generation
			LoadTemporaryLayout(grid);

			// ...and perform any required initialization of the grid
			//InitializeGrid(grid);

			// End of the update for the grid
			grid.EndUpdate();
		}



		/// <summary>
		/// Called to perform any row specific initailization as the row is displayed
		/// </summary>
		/// <param name="gridRow"></param>
		public override void InitializeGridRow(UltraGridRow gridRow)
		{
			if ((gridRow == null) || (gridRow.Cells.Count == 0))
				return;

			UltraGridCell objectCell = gridRow.Cells[0];

			// Set the icon for the first visible row to that appropriate for the object
			if (objectCell.Value is InstalledApplication)
				SetFirstVisibleColumnIcon(gridRow, Properties.Resources.application_16);
			else if (objectCell.Value is Asset)
				SetFirstVisibleColumnIcon(gridRow, Properties.Resources.application_16);
			else if (objectCell.Value is ApplicationLicense)
				SetFirstVisibleColumnIcon(gridRow, Properties.Resources.application_license_16);
		}


		/// <summary>
		/// Create the tables, columns, relationships and foreign keys required for this report
		/// </summary>
		protected void CreateTables()
		{
			// The Licensing Data Set will have 3 tables, Applications, Instances and Licenses
			// Create these tables here
			DataTable applicationsTable = new DataTable("Applications");
			DataTable instancesTable = new DataTable("Instances");
			DataTable licensesTable = new DataTable("Licenses");

			// begin the initialization of the dataset tables
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(applicationsTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(instancesTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(licensesTable)).BeginInit();

			// Add the tables to the Database
			_reportDataSet.Tables.AddRange(new System.Data.DataTable[] { applicationsTable, instancesTable, licensesTable });		
		
			// Set the relationship between the applications and the instances/licenses tables
			try
			{
				_reportDataSet.Relations.AddRange(new System.Data.DataRelation[] {
				new System.Data.DataRelation("Relation1", "Applications", "Instances", new string[] {
							"applicationid"}, new string[] {
							"applicationid"}, false),
				new System.Data.DataRelation("Relation2", "Applications", "Licenses", new string[] {
							"applicationid"}, new string[] {
							"applicationid"}, false)});
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception settings relationships, message is : " + ex.Message);
			}
			
			
			//		
			// Create the APPLICATIONS table
			// Columns are :- AppID, Object, Publisher, Name, Installations, Licenses, Variance, Status
			DataColumn appColumn1 = new DataColumn("object", typeof(object));
			DataColumn appColumn2 = new DataColumn("applicationid", typeof(int));
			DataColumn appColumn3 = new DataColumn("Publisher");
			DataColumn appColumn4 = new DataColumn("Name");
			DataColumn appColumn5 = new DataColumn("Installations");
			DataColumn appColumn6 = new DataColumn("Licenses");
			DataColumn appColumn7 = new DataColumn("Variance");
			DataColumn appColumn8 = new DataColumn("Status");

			// Add these columns to the tabls
			applicationsTable.Columns.AddRange(new System.Data.DataColumn[] { appColumn1, appColumn2, appColumn3, appColumn4, appColumn5, appColumn6, appColumn7, appColumn8 });

			// Add a constraint on this table
			try
			{
				applicationsTable.Constraints.AddRange(new System.Data.Constraint[] {
				new System.Data.UniqueConstraint("Constraint1", new string[] {
					        "applicationid"}, false)});
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception settings constraints, message is : " + ex.Message);
			}
			

			// Now create the INSTANCES Table
			// Columns are :- Object, Publisher, Name, Installations, Licenses, Variance, Status
			DataColumn insColumn1 = new DataColumn("object", typeof(object));
			DataColumn insColumn2 = new DataColumn("applicationid", typeof(int));
			DataColumn insColumn3 = new DataColumn("instanceid", typeof(int));
			DataColumn insColumn4 = new DataColumn("Location");
			DataColumn insColumn5 = new DataColumn("Asset Name");
			DataColumn insColumn6 = new DataColumn("Version");
			DataColumn insColumn7 = new DataColumn("Serial Number");
			DataColumn insColumn8 = new DataColumn("CD Key");

			// Add these columns to the tabls
			instancesTable.Columns.AddRange(new System.Data.DataColumn[] { insColumn1, insColumn2, insColumn3, insColumn4, insColumn5, insColumn6, insColumn7, insColumn8 });

			// Add a constraint on this table
			try
			{
				instancesTable.Constraints.AddRange(new System.Data.Constraint[] {
						new System.Data.ForeignKeyConstraint("Relation1", "Applications", new string[] {
                        "applicationid"}, new string[] {
                        "applicationid"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception settings constraints, message is : " + ex.Message);
			}

			// Now create the LICENSES table
			// Columns are :- Object, Publisher, Name, Installations, Licenses, Variance, Status
			DataColumn licColumn1 = new DataColumn("object", typeof(object));
			DataColumn licColumn2 = new DataColumn("applicationid", typeof(int));
			DataColumn licColumn3 = new DataColumn("licenseid", typeof(int));
			DataColumn licColumn4 = new DataColumn("License Type");
			DataColumn licColumn5 = new DataColumn("Per PC?");
			DataColumn licColumn6 = new DataColumn("License Count");

			// Add these columns to the tabls
			licensesTable.Columns.AddRange(new System.Data.DataColumn[] { licColumn1, licColumn2, licColumn3, licColumn4, licColumn5, licColumn6 });


			// Add a constraint on this table
			try
			{
				licensesTable.Constraints.AddRange(new System.Data.Constraint[] {
						new System.Data.ForeignKeyConstraint("Relation2", "Applications", new string[] {
                        "applicationid"}, new string[] {
                        "applicationid"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception settings constraints, message is : " + ex.Message);
			}
			
			// End initialization of the DataSet and Tables
			((System.ComponentModel.ISupportInitialize)(_reportDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(applicationsTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(instancesTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(licensesTable)).EndInit();
			
		}		



		/// <summary>
		/// This function is responsible for generating the actual data which will be displayed by the report
		/// </summary>
		protected void GenerateReportData()
		{
			// Create a string representation of the publisher filter list passed to us
			// We need to get the entire licensing information at this point
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();

            //AssetGroup.GROUPTYPE displayType = AssetGroup.GROUPTYPE.userlocation;
            //DataTable table = new LocationsDAO().GetGroups(new AssetGroup(displayType));
            //AssetGroup _cachedAssetGroups = new AssetGroup(table.Rows[0], displayType);
            //_cachedAssetGroups.Populate(true, _ignoreChildAssets, true);

            //// Now apply the filter to these groups
            //_cachedAssetGroups.ApplyFilters(_selectedGroups, _selectedAssets, _ignoreChildAssets);

            _selectedAssets = new AssetDAO().GetSelectedAssets();

            selectedAssetList = new List<int>();
		    foreach (string id in _selectedAssets.Split(','))
		    {
		        selectedAssetList.Add(Convert.ToInt32(id));
		    }

			// If we have selected all publishers and all applications then apply the publisher filter
			//if (_selectedPublishers == "" && _selectedApplications == "")
				
            _selectedPublishers = _publisherFilter;
				
			DataTable applicationsTable = lwDataAccess.GetApplications(_selectedPublishers, true, false);

			// ...then create InstalledApplication objects for each returned and add to the view
			for (int rowIndex = 0; rowIndex < applicationsTable.Rows.Count; )
			{
				// Get the data row
				DataRow row = applicationsTable.Rows[rowIndex];

				// Create the object from the data row
				InstalledApplication thisApplication = new InstalledApplication(row);

				// ...and if we are only displaying the report for a specific application and this is not it then 
				// delete this row from the table and skip it
				if ((_selectedApplications != "") &&  (!_selectedApplications.Contains(thisApplication.Name)))
				{
					applicationsTable.Rows.Remove(row);
					continue;
				}

				// Read instances and licenses of this application 
				thisApplication.LoadData();

				// If we are only displaying applications that have Serial numbers or CD Keys then apply that filter here
				if ((_showWithKeysOnly) && (!thisApplication.HaveSerialNumbers()))
				{
					applicationsTable.Rows.Remove(row);
					continue;
				}

				// Ensure that we look at the next row in the next loop
				rowIndex++;

                ResetSelectedAssets();

                bool addApplication = false;

                foreach (ApplicationInstance app in thisApplication.Instances)
                {
                    //if (FilterRecord(app.ComputerLocation, app.InstalledOnComputer))
                    //    addApplication = true;

                    if (selectedAssetList.Contains(app.InstalledOnComputerID))
                        addApplication = true;

                    //foreach (int assetID in selectedAssetList)
                    //{
                    //    if (app.InstalledOnComputerID == assetID)
                    //    {
                    //        addApplication = true;
                    //        break;
                    //    }
                    //}
                    
                }	
			
                if (addApplication)
                    AddApplication(thisApplication, _showWithKeysOnly);
			}		
		}


		/// <summary>
		/// This function is called to add an application to the DataSet noting that the
		/// DataSet actually has 3 separate (but linked) tables for Applications, Instances and Licenses
		/// </summary>
		protected void AddApplication(InstalledApplication thisApplication, bool includeOnlyCdKeys)
		{
			// First add the Application itself to the applications table
			//
			// Ensure that fields which may not have a value have something to display
			string publisher = (thisApplication.Publisher == "") ? "-" : thisApplication.Publisher;
			string isCompliant = (thisApplication.IsCompliant()) ? "Compliant" : "Not Compliant";

			// Licenses count
			string licenses;
			string variance;
			int installs;
			thisApplication.GetLicenseStatistics(out installs, out licenses, out variance);

			// Get the Applications Table from the DataSet
			DataTable applicationTable = _reportDataSet.Tables["Applications"];			
			
			// Add the row to the data set
			try
			{
				applicationTable.Rows.Add(new object[]
					{ thisApplication
					, thisApplication.ApplicationID
					, publisher 
					, thisApplication.Name 
					//, thisApplication.InstallCount() 
					, thisApplication.InstallCountFiltered(_selectedAssets)
					, licenses
					, variance
					, isCompliant});
			}
			catch (Exception ex)
			{
				//MessageBox.Show(e.Message);
                logger.Error(ex.Message);
			}

			// Now add any instances to the Instances table
			foreach (ApplicationInstance thisInstance in thisApplication.Instances)
			{
                //if (!_selectedAssets.Contains(thisInstance.InstalledOnComputer))
                //        continue;

			    bool found = false;

                //foreach (int assetId in selectedAssetList)
                //{
                //    if (assetId == thisInstance.InstalledOnComputerID)
                //    {
                //        found = true;
                //        break;
                //    }
                //}

                if (!selectedAssetList.Contains(thisInstance.InstalledOnComputerID))
                    continue;

				if ((_showWithKeysOnly) && ((thisInstance.Serial.ProductId == "") && (thisInstance.Serial.CdKey == "")))
					continue;

				AddInstance(thisInstance, thisApplication.ApplicationID);
			}

			// Now add any Licenses to the Licenses table
			foreach (ApplicationLicense thisLicense in thisApplication.Licenses)
			{
				AddLicense(thisLicense);
			}
		}

        protected bool FilterRecord(string location, string assetName)
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
		/// Add an instance of an application to the Instances table within the dataset
		/// </summary>
		/// <param name="thisInstance"></param>
		protected void AddInstance(ApplicationInstance thisInstance, int applicationID)
		{
			string version = (thisInstance.Version == "") ? "-" : thisInstance.Version;
			string serialNumber = (thisInstance.Serial.ProductId == "") ? "-" : thisInstance.Serial.ProductId;
			string cdKey = (thisInstance.Serial.CdKey == "") ? "-" : thisInstance.Serial.CdKey;

			// Get the Instances table from the DataSet
			DataTable instanceTable = _reportDataSet.Tables["Instances"];
			
			try
			{
				instanceTable.Rows.Add(new object[]
					{ thisInstance
					, thisInstance.ApplicationID
					, thisInstance.InstanceID
					, thisInstance.ComputerLocation
					, thisInstance.InstalledOnComputer
					, version
					, serialNumber
					, cdKey });
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}


		/// <summary>
		/// Add a license for an application to the Licenses table
		/// </summary>
		/// <param name="thisLicense"></param>
		protected void AddLicense(ApplicationLicense thisLicense)
		{
			DataTable licenseTable = _reportDataSet.Tables["Licenses"];
			try
			{
				licenseTable.Rows.Add(new object[]
					{ thisLicense
					, thisLicense.ApplicationID
					, thisLicense.LicenseID
					, thisLicense.LicenseTypeName
					, thisLicense.UsageCounted
					, thisLicense.Count });
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}


		/// <summary>
		/// Perform general configuration of the grid to match the report being displayed
		/// </summary>
		/// <param name="grid"></param>
		public void InitializeGrid(UltraGrid grid)
		{
			grid.Tag = this;
			try
			{
                UltraGridBand applicationsBand = grid.DisplayLayout.Bands[0];
                applicationsBand.Columns["applicationid"].Hidden = true;
                applicationsBand.Columns["object"].Hidden = true;
                applicationsBand.Columns["publisher"].CellAppearance.Image = Properties.Resources.application_16;
				//
				UltraGridBand instancesBand = grid.DisplayLayout.Bands[1];
				instancesBand.Columns["instanceid"].Hidden = true;
				instancesBand.Columns["applicationid"].Hidden = true;
				instancesBand.Columns["object"].Hidden = true;
				instancesBand.Columns["Asset Name"].CellAppearance.Image = Properties.Resources.computer16;
				//
				UltraGridBand licensesBand = grid.DisplayLayout.Bands[2];
				licensesBand.Columns["licenseid"].Hidden = true;
				licensesBand.Columns["applicationid"].Hidden = true;
				licensesBand.Columns["object"].Hidden = true;
				licensesBand.Columns["License Type"].CellAppearance.Image = Properties.Resources.application_license_16;

				// But hide any bands that we have requested not too show
				instancesBand.Hidden = !_showInstanceDetails;
				licensesBand.Hidden = !_showLicenses;
			}
			catch (Exception ex)
			{
				MessageBox.Show("An exception has occurred while initializing the grid for the Licensing Report.  The error was : " + ex.Message, "Exception Error");
			}
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
					case V_FILTERS_APPLICATIONS:
						_selectedApplications = childElement.Text;
						break;

					case V_FILTERS_PUBLISHERS:
						_selectedPublishers = childElement.Text;
						break;

					case V_FILTERS_SHOWINSTANCES:
						_showInstanceDetails = childElement.TextAsBoolean;
						break;

					case V_FILTERS_SHOWKEYSONLY:
						_showWithKeysOnly = childElement.TextAsBoolean;
						break;

					case V_FILTERS_SHOWLICENSES:
						_showLicenses = childElement.TextAsBoolean;
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
			writer.WriteSetting(V_FILTERS_APPLICATIONS, _selectedApplications);
			writer.WriteSetting(V_FILTERS_PUBLISHERS, _selectedPublishers);
			writer.WriteSetting(V_FILTERS_SHOWINSTANCES, _showInstanceDetails);
			writer.WriteSetting(V_FILTERS_SHOWKEYSONLY, _showWithKeysOnly);
			writer.WriteSetting(V_FILTERS_SHOWLICENSES, _showLicenses);
			//			
			writer.EndSection();
		}

		#endregion WRITER Functions
			
				
		#endregion Methods

	}
}
