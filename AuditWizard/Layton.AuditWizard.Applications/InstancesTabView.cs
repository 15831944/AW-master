using System;
using System.Drawing;
using System.IO;
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
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    [SmartPart]
    public partial class InstancesTabView : UserControl, ILaytonView
	{
		#region Data
		InstancesTabViewPresenter presenter;
        LaytonWorkItem workItem;

		/// <summary>
		/// The Grid layout file name
		/// </summary>
		private static string _gridLayoutFile = "ApplicationInstancesTabLayout.xml";
		#endregion Data

		#region Properties

		[CreateNew]
		public InstancesTabViewPresenter Presenter
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
		public InstancesTabView([ServiceDependency] WorkItem workItem)
		{
			this.workItem = workItem as LaytonWorkItem;
			InitializeComponent();

			// Restore any saved layout for the grid
			LoadLayout();
		}

		#endregion Constructor

		#region Form handlers

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

		#endregion Form handlers

		#region Event Handlers

		/// <summary>
		/// This is the handler for the ApplicationInstallsSelectionChanged which is fired when we select
		/// a different publisher as need to ensure that we display only those applications 
		/// for the selected publisher.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(EventTopics.ApplicationInstallsSelectionChanged)]
		public void ApplicationInstallsSelectionChangedHandler(object sender, ApplicationInstallsEventArgs e)
		{
			presenter.ShowApplicationInstances(e.SelectedNodeObject);
		}

		#endregion Event Handlers

		#region Grid Display Handlers

		public void Clear()
		{
			instancesDataSet.Tables[0].Rows.Clear();
		}


		/// <summary>
		/// Add a new application to the data set to be displayed
		/// </summary>
		/// <param name="thisComputer"></param>
		public void AddInstance (ApplicationInstance thisInstance)
		{
			String version = (thisInstance.Version == "") ? "-" : thisInstance.Version;
			String serialNumber = (thisInstance.Serial.ProductId == "") ? "-" : thisInstance.Serial.ProductId;
			String cdKey = (thisInstance.Serial.CdKey == "") ? "-" : thisInstance.Serial.CdKey;

			instancesDataSet.Tables[0].Rows.Add(new object[] 
				{ thisInstance.InstalledOnComputer
				, version
				, serialNumber
				, cdKey});
		}

		#endregion Grid Display Handlers

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
			if (this.instancesDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(instancesGridView, headerLabel.Text);
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.instancesDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = headerLabel.Text + ".pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Applications View : " + instancesGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, instancesGridView
											, Infragistics.Documents.Reports.Report.FileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.instancesDataSet.Tables[0].Rows.Count == 0)
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
											, "AuditWizard Applications View : " + instancesGridView.Text
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, instancesGridView
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
				this.instancesGridView.DisplayLayout.LoadFromXml(layoutFile);
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
			if (this.instancesGridView != null)
			{
				string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
				this.instancesGridView.DisplayLayout.SaveAsXml(layoutFile);
			}
		}

		#endregion Load/Save Layout               
	}
}
