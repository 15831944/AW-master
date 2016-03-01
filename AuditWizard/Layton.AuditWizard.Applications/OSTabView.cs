using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;
//
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Layton.AuditWizard.Applications
{
    [SmartPart]
    public partial class OSTabView : UserControl, ILaytonView
	{
		#region Data

		OSTabViewPresenter presenter;
		private LaytonWorkItem workItem;
		private Infragistics.Win.Appearance _compliantAppearance = new Infragistics.Win.Appearance();
		private Infragistics.Win.Appearance _noncompliantAppearance = new Infragistics.Win.Appearance();
        private Infragistics.Win.Appearance _notSpecifiedAppearance = new Infragistics.Win.Appearance();

		/// <summary>
		/// The Grid layout file name
		/// </summary>
		private static string _gridLayoutFile = "OSTabLayout.xml";
		
		#endregion Data

		#region Constructor

		[InjectionConstructor]
        public OSTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Initialize the compliant/non-compliant appearances
			_noncompliantAppearance.ForeColor = System.Drawing.Color.Red;
            _compliantAppearance.ForeColor = System.Drawing.Color.Green;
            _notSpecifiedAppearance.ForeColor = System.Drawing.Color.FromArgb(255, 110, 0);

			// Restore any saved layout for the grid
			LoadLayout();
		}

		#endregion Constructor

		#region Properties

		[CreateNew]
		public OSTabViewPresenter Presenter
		{
			set 
            { 
                presenter = value; 
                presenter.View = this; 
                //presenter.Initialize(); 
            }
			get 
            { 
                return presenter; 
            }
		}

		public void RefreshView()
		{
			presenter.Initialize();
			base.Refresh();
		}

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

		/// <summary>
		/// Set the text to be displayed in the header of this view
		/// </summary>
		public string HeaderText
		{
			set 
			{ 
				headerLabel.Text = value;
				this.OSGridView.Text = value;
				CenterHeaderLabel(); 
			}
		}

		/// <summary>
		/// Set the image to be displayed in the header of this view
		/// </summary>
		public Image HeaderImage
		{
			set { headerLabel.Appearance.Image = value; }
		}

		#endregion Properties

		#region Form Data Functions

		public void Clear()
		{
			OSDataSet.Tables[0].Rows.Clear();

			// If we are displaying a specific Operating System then there is little point in having the
			// name column itself displayed so let's make it hidden
			this.OSGridView.DisplayLayout.Bands[0].Columns["Name"].Hidden = !presenter.IsAllOSDisplayed;
		}

        public int GetLicenseCount(List<ApplicationLicense> aLicenseList)
        {
            // Assume no licenses for this application
            int licenseCount = 0;

            // ...and iterate through any defined licenses adding up the total
            foreach (ApplicationLicense theLicense in aLicenseList)
            {
                // If this license doesn't use usage counting then treat as unlimited
                if (!theLicense.UsageCounted)
                {
                    licenseCount = -1;
                    break;
                }
                else
                {
                    licenseCount += theLicense.Count;
                }
            }
            return licenseCount;
        }


		/// <summary>
		/// Add a new application to the data set to be displayed
		/// </summary>
		/// <param name="thisComputer"></param>
		public void AddOS(InstalledOS theOS)
		{
			// Licenses count
			String licenses = "";
			String variance = "";

			// Is the OS compliant?
			//bool isCompliant = theOS.IsCompliant();

            string isCompliant = (theOS.IsCompliant()) ? "Compliant" : "Non-Compliant";

			// What is the license count for this application?
			int licenseCount = GetLicenseCount(theOS.Licenses);

			// OK format the license count and variance for display
			if (licenseCount == -1)
			{
				licenses = "Unlimited";
				variance = "None";
			}

			else if (licenseCount == 0)
			{
				licenses = "None Specified";
                isCompliant = licenses;
                variance = "Shortfall : " + theOS.InstallCount.ToString();
			}

			else
			{
				licenses = "Licenses for " + licenseCount.ToString() + " Asset(s)";
				if (licenseCount == theOS.InstallCount)
					variance = "None : All Instances Licensed";
				else if (licenseCount < theOS.InstallCount)
					variance = "Shortfall : " + (theOS.InstallCount - licenseCount).ToString();
				else
					variance = "Surplus : " + (licenseCount - theOS.InstallCount).ToString();
			}

			// Add the row to the data set
			OSDataSet.Tables[0].Rows.Add(new object[] 
				{ theOS
				, theOS.Name 
				, licenses
				, theOS.InstallCount.ToString() 
				, variance
				, isCompliant });
		}

		public List<InstalledOS> GetSelectedOS()
		{
			List<InstalledOS> listOS = new List<InstalledOS>();
			int selectedRowCount = OSGridView.Selected.Rows.Count;
			for (int isub = 0; isub < selectedRowCount; isub++)
			{
				UltraGridRow selectedRow = this.OSGridView.Selected.Rows[isub];
				InstalledOS thisOS = selectedRow.Cells[0].Value as InstalledOS;
				listOS.Add(thisOS);
			}
			return listOS;
		}

		#endregion Form Data Functions

		#region Form Display Functions

		private void CenterHeaderLabel()
		{
			//int centerX = headerGroupBox.Width / 2;
			//int headerCenterX = headerLabel.Width / 2;
			//headerLabel.Left = centerX - headerCenterX;
		}

		/// <summary>
		/// If the view is re-sized we need to ensure that the header box remains centralized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void headerGroupBox_SizeChanged(object sender, EventArgs e)
		{
			CenterHeaderLabel();
		}

		#endregion Form Display Functions

		#region Form Message Functions

		/// <summary>
		/// Called as each row in the grid is initialized - we use this to set the appearance of the row
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OSGridView_InitializeRow(object sender, InitializeRowEventArgs e)
		{
			// Get the application object being displayed
			UltraGridRow thisRow = e.Row;
			UltraGridCell objectCell = thisRow.Cells[0];
			InstalledOS thisOS = objectCell.Value as InstalledOS;

			// Set the appearance and icon based on the compliancy status
            if (thisOS.LicenseCount == 0)
                thisRow.Appearance = _notSpecifiedAppearance;
            else if (thisOS.IsCompliant())
                thisRow.Appearance = _compliantAppearance;
            else
                thisRow.Appearance = _noncompliantAppearance;
		}
		
		#endregion Form Message Functions

		#region Context Menu Handlers

		/// <summary>
		/// Called to export the contents of the displayed grid to excel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportXlsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportToXLS();
		}


		/// <summary>
		/// Called to export the data from the grid to a PDF
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportPDFToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportToPDF();
		}

		/// <summary>
		/// Handle Export to XPS selected from the context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportXPSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportToXPS();
		}

		#endregion Context Menu Handlers

		#region Export Functions

		/// <summary>
		/// Export the graph data to an XLS format file
		/// </summary>
		public void ExportToXLS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.OSDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(OSGridView, headerLabel.Text);
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.OSDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// We need to temporarily set the grid view to 'Resize all columns' in order to get
				// the resultant PDF file formatted correctly.
				AutoFitStyle oldStyle = OSGridView.DisplayLayout.AutoFitStyle;
				OSGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Applications View : " + OSGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, OSGridView
											, Infragistics.Documents.Reports.Report.FileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}

				// Populate the old autofit style
				this.OSGridView.DisplayLayout.AutoFitStyle = oldStyle;
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.OSDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".xps";
				saveFileDialog.Filter = "XML Paper Specification (*.xps)|*.xps";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Applications View : " + OSGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, OSGridView
											, Infragistics.Documents.Reports.Report.FileFormat.XPS);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}
			}
		}

#endregion Export Functions

		#region Load/Save Layout

		/// <summary>
		/// Called to load the layout for the rid from file
		/// </summary>
		private void LoadLayout()
		{
			try
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.OSGridView.DisplayLayout.LoadFromXml(layoutFile);
			}
			catch (Exception)
			{
				return;
			}
		}


		/// <summary>
		/// Called to request the grid to save it's layout to disk
		/// </summary>
		public void SaveLayout()
		{
			if (this.OSGridView != null)
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.OSGridView.DisplayLayout.SaveAsXml(layoutFile);
			}
		}

		#endregion Load/Save Layout

        private void newLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// We can only create a license for a single specified item
			UltraGridRow selectedRow = this.OSGridView.Selected.Rows[0];

			InstalledOS thisOS = selectedRow.Cells[0].Value as InstalledOS;
            ((ApplicationsWorkItemController)WorkItem.Controller).NewLicense(thisOS);
        }
	}
}
