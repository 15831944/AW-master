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
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    [SmartPart]
    public partial class LicensesTabView : UserControl, ILaytonView
	{
		#region Data

		LicensesTabViewPresenter presenter;
		private LaytonWorkItem workItem;

		/// <summary>
		/// The Grid layout file name
		/// </summary>
		private static string _gridLayoutFile = "LicensesTabLayout.xml";
		
		#endregion Data

		#region Constructor
		[InjectionConstructor]
        public LicensesTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Restore any saved layout for the grid
			LoadLayout();
		}
		#endregion Constructor

		#region Properties

		[CreateNew]
		public LicensesTabViewPresenter Presenter
		{
			set
			{
				presenter = value;
				presenter.View = this;
				presenter.Initialize();
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
				this.licensesGridView.Text = value;
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

		#region Grid Data Handlers

		public void Clear()
		{
			licensesDataSet.Tables[0].Rows.Clear();
		}

		/// <summary>
		/// Add a new application license to the data set to be displayed
		/// </summary>
		/// <param name="thisComputer"></param>
		public void AddApplicationLicense(ApplicationLicense theLicense)
		{
			DataRow thisRow = licensesDataSet.Tables[0].Rows.Add(new object[] 
				{ theLicense
				, theLicense.LicenseTypeName
				, (theLicense.UsageCounted) ? "Yes" : "No"
				, theLicense.Count});
		}


		/// <summary>
		/// Called when we double click on a data row within the grid - this should edit the license
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void licensesGridView_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
		{
			EditLicense();
		}


		/// <summary>
		/// Return the ApplicationLicense object associated with the currently selected row
		/// </summary>
		/// <returns></returns>
		public ApplicationLicense GetSelectedLicense()
		{
			if (licensesGridView.Selected.Rows.Count == 0)
				return null;

			UltraGridRow selectedRow = this.licensesGridView.Selected.Rows[0];
			ApplicationLicense theLicense = selectedRow.Cells[0].Value as ApplicationLicense;
			return theLicense;
		}


		/// <summary>
		/// Called to create a new license for this application
		/// </summary>
		protected void NewLicense()
		{
			// Get our controller
			ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;
			wiController.NewLicense();
		}


		/// <summary>
		/// Called to edit the currently selected license (if any)
		/// </summary>
		protected void EditLicense()
		{
			// Sanity check ensure only row selected
			if (licensesGridView.Selected.Rows.Count != 1)
				return;

			//...and get the license object
			UltraGridRow selectedRow = this.licensesGridView.Selected.Rows[0];
			ApplicationLicense theLicense = selectedRow.Cells[0].Value as ApplicationLicense;

			// Get our controller
			ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;

			// ...and request it to edit the currently selected license
			wiController.EditLicense(theLicense);
		}


		protected void DeleteLicense()
		{
			// Sanity check ensure only row selected
			if (licensesGridView.Selected.Rows.Count == 0)
				return;

            foreach (UltraGridRow selectedRow in this.licensesGridView.Selected.Rows)
            {
                //...and get the license object
                //UltraGridRow selectedRow = this.licensesGridView.Selected.Rows[0];
                ApplicationLicense theLicense = selectedRow.Cells[0].Value as ApplicationLicense;

                // Get our controller
                ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;

                // ...and request it to delete the currently selected license
                wiController.DeleteLicense(theLicense);
            }

            // refresh the tab view
            ApplicationsWorkItem appWorkItem = WorkItem as ApplicationsWorkItem;
            appWorkItem.ExplorerView.RefreshView();
            appWorkItem.GetActiveTabView().RefreshView();
		}

		#endregion Grid Data Handlers

		#region Grid Display Handlers

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

		#endregion Grid Display Handlers

		#region Grid Message Handlers

		/// <summary>
		/// Called as we press a key - some keys are handled in special ways such as delete/insert and enter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void licensesGridView_KeyDown(object sender, KeyEventArgs e)
		{
			// Handle the delete key - this should delete the currently selected item
			if (e.KeyCode == Keys.Delete)
			{
				DeleteLicense();
				e.Handled = true;
			}

			// The insert key will create a new license
			else if (e.KeyCode == Keys.Insert)
			{
				NewLicense();
				e.Handled = true;
			}

			// The enter key will edit an existing license
			else if (e.KeyCode == Keys.Enter)
			{
				EditLicense();
				e.Handled = true;
			}

		}

		#endregion Grid Message Handlers

		#region Event Handlers

		/// <summary>
		/// This is the handler for the ApplicationLicenseSelectionChanged event which is fired when we select
		/// the License node beneath an application 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(EventTopics.ApplicationLicenseSelectionChanged)]
		public void ApplicationLicenseSelectionChangedHandler(object sender, ApplicationLicenseEventArgs e)
		{
			presenter.ShowApplicationLicenses(e.SelectedNodeObject);
		}

		#endregion Event Handlers

		#region Context Menu Handlers

		private void licensesContextMenu_Opening(object sender, CancelEventArgs e)
		{
			if (licensesGridView.Selected.Rows.Count == 0)
			{
				newlicenseToolStripMenuItem.Enabled = true;
				editLicenseToolStripMenuItem.Enabled = false;
			}

			else if (licensesGridView.Selected.Rows.Count == 1)
			{
				newlicenseToolStripMenuItem.Enabled = true;
				editLicenseToolStripMenuItem.Enabled = true;
			}
		}


		/// <summary>
		/// Called when we select 'Edit License' from the context menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void editLicenseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditLicense();
		}



		private void newlicenseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewLicense();
		}


		/// <summary>
		/// Called to delete the currently selected application license
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void deleteLicenseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteLicense();
		}

#endregion Context Menu Handlers

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
		/// Called to export the data from the grid to an XPS format file
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
			if (this.licensesDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(licensesGridView, headerLabel.Text);
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.licensesDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// We need to temporarily set the grid view to 'Resize all columns' in order to get
				// the resultant PDF file formatted correctly.
				AutoFitStyle oldStyle = licensesGridView.DisplayLayout.AutoFitStyle;
				licensesGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
					exporter.Export(licensesGridView, saveFileDialog.FileName, GridExportFileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}

				// Populate the old autofit style
				this.licensesGridView.DisplayLayout.AutoFitStyle = oldStyle;
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
            if (this.licensesDataSet.Tables[0].Rows.Count == 0)
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
                    UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
                    exporter.Export(licensesGridView, saveFileDialog.FileName, GridExportFileFormat.XPS);
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
				this.licensesGridView.DisplayLayout.LoadFromXml(layoutFile);
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
			if (this.licensesGridView != null)
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.licensesGridView.DisplayLayout.SaveAsXml(layoutFile);
			}
		}

		#endregion Load/Save Layout
	}
}
