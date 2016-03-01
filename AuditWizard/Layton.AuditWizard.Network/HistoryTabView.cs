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
//
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class HistoryTabView : UserControl, ILaytonView
	{
		#region Data
		HistoryTabViewPresenter presenter;
        LaytonWorkItem workItem;
		private static string _gridLayoutFile = "HistoryTabLayout.xml";
		private UltraTreeNode _displayedNode;
		private IconMappings _iconMappings = null;
		#endregion Data

		#region Data Accessors
        [CreateNew]
        public HistoryTabViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
			get { return presenter; }
        }

        public string HeaderText
        {
            set
            {
                headerLabel.Text = value;
				historyGridView.Text = value;
                CenterHeaderLabel();
            }
        }

        public Image HeaderImage
        {
            set { headerLabel.Appearance.Image = value; }
		}

		public LaytonWorkItem WorkItem
		{
			get { return workItem; }
		}

#endregion Data Accessors

		#region Constructor
		[InjectionConstructor]
		public HistoryTabView([ServiceDependency] WorkItem workItem)
		{
			this.workItem = workItem as LaytonWorkItem;
			InitializeComponent();

			// See if we have a layout file saved and if so read it to initialize the grid
			LoadLayout();

			// Set the format for the 'Date' column to be the default date/time format for the locale
			UltraGridColumn dateColumn = historyGridView.DisplayLayout.Bands[0].Columns["Date"];
			dateColumn.Format = "G";

			// Populate icons list
			_iconMappings = new IconMappings(new AuditedItemsDAO());
		}

		#endregion 

		#region Form Population Functions

		/// <summary>
		/// Display
		/// =======
		/// 
		/// Displays the audit history for this asset within this tab view.
		/// 
		/// </summary>
		/// <param name="displayNode">UltraTreeNode holding the asset for which history is to be displayed</param>
		public void Display (UltraTreeNode displayedNode)
		{
			_displayedNode = displayedNode;
			Asset displayedAsset = _displayedNode.Tag as Asset;

			//	Call BeginUpdate to prevent drawing while we are populating the control
			this.historyGridView.BeginUpdate();
			this.Cursor = Cursors.WaitCursor;

			// Delete all entries from the current data set being displayed
			historyDataSet.Tables[0].Rows.Clear();

			// Recover the asset audit history records for this asset
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();
			DataTable historyTable = lwDataAccess.GetAssetAuditHistory(displayedAsset ,new DateTime(0) ,new DateTime(0));

			// Add the entries in the data table as ATE records to our DataSet
			foreach (DataRow row in historyTable.Rows)
			{
				AuditTrailEntry ate = new AuditTrailEntry(row);
				historyDataSet.Tables[0].Rows.Add(new object[] { ate, ate.Date, ate.GetTypeDescription(), ate.Username });
			}

			//	Restore the cursor
			this.Cursor = Cursors.Default;

			//	Call EndUpdate to resume drawing operations
			this.historyGridView.EndUpdate(true);
		}


		#endregion Form Population Functions

		#region Form Control Functions
		/// <summary>
		/// Handle the re-sizing of the header box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        void headerGroupBox_SizeChanged(object sender, EventArgs e)
        {
            CenterHeaderLabel();
        }

		private void CenterHeaderLabel()
		{
		}

		public void Clear()
		{
			historyDataSet.Tables[0].Rows.Clear();
		}

		public void RefreshView()
		{
			presenter.Initialize();
			base.Refresh();
		}

#endregion Form Control Functions

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
			if (this.historyDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(historyGridView, headerLabel.Text);
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.historyDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// We need to temporarily set the grid view to 'Resize all columns' in order to get
				// the resultant PDF file formatted correctly.
				AutoFitStyle oldStyle = historyGridView.DisplayLayout.AutoFitStyle;
				historyGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Applications View : " + historyGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, historyGridView
											, Infragistics.Documents.Reports.Report.FileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}

				// Populate the old autofit style
				this.historyGridView.DisplayLayout.AutoFitStyle = oldStyle;
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.historyDataSet.Tables[0].Rows.Count == 0)
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
											, "AuditWizard Applications View : " + historyGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, historyGridView
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
				this.historyGridView.DisplayLayout.LoadFromXml(layoutFile);
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
			if (this.historyGridView != null)
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.historyGridView.DisplayLayout.SaveAsXml(layoutFile);
			}
		}

		#endregion Load/Save Layout

		private void historyGridView_InitializeRow(object sender, InitializeRowEventArgs e)
		{
			// Get the application object being displayed - this is always a tree node in this view
			AuditTrailEntry ate = e.Row.Cells["DataObject"].Value as AuditTrailEntry;

			// The icon displayed will depend on the type of entry - we can have 
			//		application installs/uninstalls
			//		audited data changes
			//		audit/reaudit
			if (ate.Class == AuditTrailEntry.CLASS.application_installs)
			{
				e.Row.Cells["Operation"].Appearance.Image = Properties.Resources.application_16;
			}

			else if (ate.Class == AuditTrailEntry.CLASS.audited || ate.Class == AuditTrailEntry.CLASS.reaudited)
			{
				e.Row.Cells["Operation"].Appearance.Image = Properties.Resources.computer16;
			}

			else
			{
				// Recover the Description
				string description = ate.GetTypeDescription();

				// Strip off everything after the last colon (if any)
				int delimiter = description.LastIndexOf(':');
				if (delimiter != -1)
					description = description.Substring(0, delimiter);

				// Now check this against the database to see if we can find an icon for it
				IconMapping iconMapping = _iconMappings.GetIconMapping(description);
				if (iconMapping != null)
					e.Row.Cells["Operation"].Appearance.Image = IconMapping.LoadIcon(iconMapping.Icon, IconMapping.Iconsize.Small);
			}
		}
	}
}
