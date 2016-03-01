using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	public partial class FormOperationsLog : Layton.Common.Controls.ShadedImageForm
	{
		public DateTime StartDateTime
		{
			get { return this.dtpStartDateTime.DateTime; }
			set { this.dtpStartDateTime.DateTime = value; }
		}
		
		public FormOperationsLog()
		{
			InitializeComponent();
			
		}

		private void FormOperationsLog_Load(object sender, EventArgs e)
		{
			// If the AuditWizard Servcice is NOT RUNNING then alert the user as none of the operations will get processed
            //AuditWizardServiceController serviceController = new AuditWizardServiceController();
            //if (serviceController.CheckStatus() != LaytonServiceController.ServiceStatus.Running)
            //{
            //    MessageBox.Show("The AuditWizard Service is not currently running but is required to process outstanding operations\n\nPlease exit from this form and start the AuditWizard Service from the Administration Tab"
            //                    , "Service not Running"
            //                    , MessageBoxButtons.OK
            //                    , MessageBoxIcon.Exclamation);
            //}
		
			Populate();
			timerRefresh.Start();
		}

		private void timerRefresh_Tick(object sender, EventArgs e)
		{
			Populate();
		}


		/// <summary>
		/// Called to (re)Populate the grid 
		/// </summary>
		private void Populate()
		{
			OperationList listOperations = new OperationList();
			listOperations.Populate(Operation.OPERATION.any, Operation.STATUS.any);

			// Add to the data set
			this.operationsDataSet.Tables[0].Rows.Clear();
			foreach (Operation operation in listOperations)
			{
				if (operation.StartDate >= dtpStartDateTime.DateTime)
				{
					if (operation.EndDate.Ticks == 0)
					{
						operationsDataSet.Tables[0].Rows.Add(new object[] { operation			
																		  , operation.StartDate
																		  , operation.AssetName
																		  , operation.OperationAsString
																		  , operation.StatusAsString
																		  , null
																		  , operation.ErrorText });
					}
					else
					{
						operationsDataSet.Tables[0].Rows.Add(new object[] { operation			
																		  , operation.StartDate
																		  , operation.AssetName
																		  , operation.OperationAsString
																		  , operation.StatusAsString
																		  , operation.EndDate
																		  , operation.ErrorText });
					}
				}
			}
		}

		private void bnDelete_Click(object sender, EventArgs e)
		{
			foreach (UltraGridRow row in operationsGridView.Selected.Rows)
			{
				Operation operation = row.Cells[0].Value as Operation;
				operation.Delete();
			}
			Populate();
		}


		private void bnServiceLog_Click(object sender, EventArgs e)
		{
            string destinationFile = Path.Combine(Application.StartupPath + "\\logs", AuditWizardServiceController.AuditWizardServiceLog);
			System.Diagnostics.Process.Start(destinationFile);

		}

		private void dtpStartDateTime_ValueChanged(object sender, EventArgs e)
		{
			Populate();
		}


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
			if (this.operationsDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
                UltraGridExporter.ExportUltraGridToExcel(operationsGridView, "operationslog");
			}
		}


		/// <summary>
		/// Export to PDF
		/// </summary>
		public void ExportToPDF()
		{
			// If there are no rows in the grid then we cannot export
			if (this.operationsDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// We need to temporarily set the grid view to 'Resize all columns' in order to get
				// the resultant PDF file formatted correctly.
				AutoFitStyle oldStyle = operationsGridView.DisplayLayout.AutoFitStyle;
				operationsGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = "operationslog.pdf";
				saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Operations Log"
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, operationsGridView
											, Infragistics.Documents.Reports.Report.FileFormat.PDF);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}

				// Populate the old autofit style
				this.operationsGridView.DisplayLayout.AutoFitStyle = oldStyle;
			}
		}


		/// <summary>
		/// Export to XPS
		/// </summary>
		public void ExportToXPS()
		{
			// If there are no rows in the grid then we cannot export
			if (this.operationsDataSet.Tables[0].Rows.Count == 0)
			{
				MessageBox.Show("There is no data to Export", "Export Error");
			}

			else
			{
				// First browse for the folder / file that we will save
				SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
				saveFileDialog.FileName = "operationslog.xps";
				saveFileDialog.Filter = "XML Paper Specification (*.xps)|*.xps";

				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					UltraGridExporter.Export(saveFileDialog.FileName
											, "AuditWizard Operations Log"
											, "Generated by AuditWizard from Layton Technology, Inc."
											, DataStrings.Disclaimer
											, operationsGridView
											, Infragistics.Documents.Reports.Report.FileFormat.XPS);
					DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
				}
			}
		}

		#endregion Export Functions
		
	}
}

