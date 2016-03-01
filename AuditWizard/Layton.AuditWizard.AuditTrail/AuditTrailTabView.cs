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
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;
//
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Layton.AuditWizard.AuditTrail
{
    [SmartPart]
    public partial class AuditTrailTabView : UserControl, ILaytonView
	{
		#region data
		private LaytonWorkItem workItem;
		private List<AuditTrailEntry> _listDisplayedRows = new List<AuditTrailEntry>();

		/// <summary>This holds the current filter settings</summary>
		private AuditTrailFilterEventArgs _auditTrailFilterEventArgs = new AuditTrailFilterEventArgs();

		/// <summary>
		/// Grid layout filename
		/// </summary>
		private static string _gridLayoutFile = "AuditTrailTabLayout.xml";
		#endregion data

		#region Properties

		public List<AuditTrailEntry> DisplayedEntries
		{
			get { return _listDisplayedRows; }
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

		#region Constructor

		[InjectionConstructor]
		public AuditTrailTabView([ServiceDependency] WorkItem workItem)
		{
			this.workItem = workItem as LaytonWorkItem;
			InitializeComponent();

			// Hook into the terminating event so that we can save the layout on exit
			this.workItem.Terminating += new EventHandler(workItem_Terminating);
		}

		protected void workItem_Terminating(object sender, EventArgs e)
		{
			//SaveLayout();
		}

#endregion Constructor

		#region Methods

        public void RefreshViewSinglePublisher()
        {
        }

		public void RefreshView()
        {
            base.Refresh();

			// Clear any existing data
			auditTrailDataSet.Tables[0].Rows.Clear();
			_listDisplayedRows.Clear();

            // Initialize the columns to be displayed based on what we are displaying
			if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.license)
            {
                HeaderText = "Audit Trail - Application License Changes";
                auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = true;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

			else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.application_changes)
			{
				HeaderText = "Audit Trail - Application Property Changes";
				auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = true;
				auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

			else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.action)
			{
				HeaderText = "Audit Trail - Action Changes";
				auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = true;
				auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                //auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                //auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

			else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.supplier)
			{
				HeaderText = "Audit Trail - Supplier Changes";
				auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = true;
				auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

			else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.user)
			{
				HeaderText = "Audit Trail - User Changes";
				auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = true;
				auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

            else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.asset)
            {
                HeaderText = "Audit Trail - Asset Changes";
                auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = true;
			}

			else if (_auditTrailFilterEventArgs.RequiredClass == AuditTrailEntry.CLASS.all)
			{
				HeaderText = "Audit Trail - All Entries";
				auditTrailGridView.DisplayLayout.Bands[0].Columns["computer"].Hidden = false;
				auditTrailGridView.DisplayLayout.Bands[0].Columns["username"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["oldvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["newvalue"].Hidden = false;
                auditTrailGridView.DisplayLayout.Bands[0].Columns["licensetype"].Hidden = false;
			}

			// Call database function to return list of Audit Trail Entries
            AuditTrailDAO lwDataAccess = new AuditTrailDAO();
			DataTable ateTable = lwDataAccess.GetAuditTrailEntries((int)_auditTrailFilterEventArgs.RequiredClass);

			// We cannot use this table directly as we do not require some of the columns and others 
			// need formatting before display.  We therefore will re-construct the ATE entries from 
			// the recovered data table.
			// ...and add these to the tab view
            foreach (DataRow row in ateTable.Rows)
            {
                AuditTrailEntry ate = new AuditTrailEntry(row);
                AddAuditTrailEntry(ate);
            }
        }


		/// <summary>
		/// Add an Audit Trail Entry to the data set associated with the grid
		/// </summary>
		/// <param name="ate"></param>
		protected void AddAuditTrailEntry(AuditTrailEntry ate)
		{			
			// Get any filter dates
			DateTime startDate = _auditTrailFilterEventArgs.StartDate.Date;
			DateTime endDate = _auditTrailFilterEventArgs.EndDate.Date.AddDays(1);

			// ...and check date in range first
			if (ate.Date < startDate.Date || ate.Date > endDate.Date)
				return;

			// Check the entry type against what we are expecting to add and reject if invalid
			if ((_auditTrailFilterEventArgs.RequiredClass != AuditTrailEntry.CLASS.all)
			&&  (ate.Class != _auditTrailFilterEventArgs.RequiredClass))
				return;

			// Some items modified may be delimited by '|'.  Split these here
			// For Application Property Changes these will be in the form
			//	APPLICATION | PROPERTY
			//
			// For license changes this will be in the form
			//	APPLICATION | LICENSE TYPE | PROPERTY
            String keyValue1 = "";
            String keyValue2 = "";
            String keyValue3 = "";
			String[] keyParts = ate.Key.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (keyParts.Length > 0)
                keyValue1 = keyParts[0];
            if (keyParts.Length > 1)
                keyValue2 = keyParts[1];
            if (keyParts.Length > 2)
                keyValue3 = keyParts[2];
            

			// OK this record has survived all filters so add it
			_listDisplayedRows.Add(ate);
			auditTrailDataSet.Tables[0].Rows.Add(new object[] 
				{ate,
				 ate.Date.ToString("yyyy-MM-dd HH:mm"),
				 ate.AssetName,
				 ate.Username,
				 ate.GetTypeDescription(),
				 keyValue1,
				 (keyValue3 == "") ? ate.OldValue : keyValue3 + " : " + ate.OldValue,
				 ate.NewValue,
				 keyValue2
				});
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

		private void CenterHeaderLabel()
		{
			//int centerX = headerGroupBox.Width / 2;
			//int headerCenterX = headerLabel.Width / 2;
			//headerLabel.Left = centerX - headerCenterX;
		}
		#endregion Methods

		#region Event Handlers

		/// <summary>
		/// This is the handler for the AuditTrailFilterChanged event which is fired when 
		/// one or more of the filter values for the Audit Trail display have been updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(EventTopics.AuditTrailFilterChanged)]
		public void AuditTrailFilterChangedHandler(object sender, AuditTrailFilterEventArgs e)
		{
			_auditTrailFilterEventArgs = e;
			//
			RefreshView();
		}
		#endregion Event Handlers

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

		private void deleteItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteEntries();
		}

		private void auditTrailGridView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteEntries();
				e.Handled = true;
			}
		}

		#endregion Context Menu Handlers

		#region Export Functions

		/// <summary>
		/// Export the graph data to an XLS format file
		/// </summary>
		public void ExportToXLS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.auditTrailDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(auditTrailGridView, headerLabel.Text);
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.auditTrailDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// We need to temporarily set the grid view to 'Resize all columns' in order to get
				// the resultant PDF file formatted correctly.
				AutoFitStyle oldStyle = auditTrailGridView.DisplayLayout.AutoFitStyle;
				auditTrailGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, auditTrailGridView.Text 
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, auditTrailGridView
											, Infragistics.Documents.Reports.Report.FileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}

				// Populate the old autofit style
				this.auditTrailGridView.DisplayLayout.AutoFitStyle = oldStyle;
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.auditTrailDataSet.Tables[0].Rows.Count == 0)
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
                                            , auditTrailGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, auditTrailGridView
											, Infragistics.Documents.Reports.Report.FileFormat.XPS);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}
			}
		}

		#endregion Export Functions

		public void DeleteEntries()
		{
			if (MessageBox.Show("You are about to delete " + auditTrailGridView.Selected.Rows.Count.ToString() + " entries from the database, are you sure that you want to continue?", "Confirm Delete") == DialogResult.OK)
			{
				AuditTrailDAO lwDataAccess = new AuditTrailDAO();
				foreach (UltraGridRow selectedRow in auditTrailGridView.Selected.Rows)
				{
					lwDataAccess.AuditTrailDelete(selectedRow.Cells[0].Value as AuditTrailEntry);
				}

				RefreshView();
			}
		}

		#region Load/Save Layout

		/// <summary>
		/// Called to load the layout for the rid from file
		/// </summary>
		private void LoadLayout()
		{
			try
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.auditTrailGridView.DisplayLayout.LoadFromXml(layoutFile);
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
			if (this.auditTrailGridView != null)
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.auditTrailGridView.DisplayLayout.SaveAsXml(layoutFile);
			}
		}

		#endregion Load/Save Layout

	}
}
