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
    public partial class AlertsTabView : UserControl, ILaytonView
    {
        #region Data

        AlertsTabViewPresenter presenter;
        private LaytonWorkItem workItem;

        /// <summary>
        /// The Grid layout file name
        /// </summary>
        private static string _gridLayoutFile = "AlertsTabLayout.xml";

        #endregion Data

        #region Constructor

        [InjectionConstructor]
        public AlertsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            // Restore any saved layout for the grid
            LoadLayout();
        }

        #endregion Constructor

        #region Properties

        [CreateNew]
        public AlertsTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
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
                this.alertsGridView.Text = value;
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
            alertsDataSet.Tables[0].Rows.Clear();
        }


        /// <summary>
        /// Add a new alert to the data set to be displayed
        /// </summary>
        /// <param name="alert"></param>
        public void AddAlert(Alert alert)
        {
            // Add the row to the data set
            alertsDataSet.Tables[0].Rows.Add(new object[] 
				{ alert
				, alert.AlertedOnDate
				, alert.TypeAsString
				, alert.CategoryAsString
				, alert.StatusAsString
				, alert.Message });
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
        private void alertsGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the object being displayed
            UltraGridRow thisRow = e.Row;
            UltraGridCell objectCell = thisRow.Cells[0];
            Alert alert = objectCell.Value as Alert;

            // Set the 'alert' image to either be dismissed or active
            if (alert.Status == Alert.AlertStatus.active)
                thisRow.Cells[1].Appearance.Image = Properties.Resources.Alert_active_16;
            else
                thisRow.Cells[1].Appearance.Image = Properties.Resources.Alert_dismissed_16;
        }

        #endregion Form Message Functions

        /// <summary>
        ///  Called as we click on a row in the grid - we shall edit the action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alertsGridView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            viewDetailsToolStripMenuItem_Click(sender, null);
        }

        #region Context Menu Handlers

        /// <summary>
        /// Called to display the full details of this action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Have we right-clicked on an item
            if (alertsGridView.Selected.Rows.Count != 1)
                return;

            UltraGridRow selectedRow = alertsGridView.Selected.Rows[0];
            UltraGridCell objectCell = selectedRow.Cells["AlertObject"];
            Alert alert = objectCell.Value as Alert;
            ViewDetails(alert);
        }

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

        /// <summary>
        /// Called to display full details of the alert - we refresh if we OK from the form
        /// </summary>
        /// <param name="action"></param>
        private void ViewDetails(Alert alert)
        {
            FormAlertDetails form = new FormAlertDetails(alert);
            form.ShowDialog();
            this.RefreshView();
        }


        /// <summary>
        /// Called to delete the alert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteAlertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you certain that you want to delete the selected alert(s)?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            // OK get the selected alert(s) and delete them
            foreach (UltraGridRow selectedRow in alertsGridView.Selected.Rows)
            {
                UltraGridCell objectCell = selectedRow.Cells["AlertObject"];
                Alert alert = objectCell.Value as Alert;

                // Delete the alert from the database
                alert.Delete();
            }

            if (alertsGridView.Selected.Rows.Count == 1)
                MessageBox.Show("Alert Deleted", "Alert Deleted");
            else
                MessageBox.Show(alertsGridView.Selected.Rows.Count.ToString() + " Alert Deleted", "Alert Deleted");

            this.RefreshView();
        }


        /// <summary>
        /// Mark the selected alert(s) as dismissed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissAlertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // OK get the selected alert(s) and delete them
            foreach (UltraGridRow selectedRow in alertsGridView.Selected.Rows)
            {
                UltraGridCell objectCell = selectedRow.Cells["AlertObject"];
                Alert alert = objectCell.Value as Alert;

                // Delete the alert from the database
                alert.Dismiss();
            }

            if (alertsGridView.Selected.Rows.Count == 1)
                MessageBox.Show("Alert Dismissed", "Alert Dismissed");
            else
                MessageBox.Show(alertsGridView.Selected.Rows.Count.ToString() + " Alert Dismissed", "Alert Dismissed");

            this.RefreshView();
        }



        #endregion Context Menu Handlers

        #region Export Functions

        /// <summary>
        /// Export the graph data to an XLS format file
        /// </summary>
        public void ExportToXLS()
        {
            // If there are no rows in the grid then we cannot export
            if (alertsDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(alertsGridView, headerLabel.Text);
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (this.alertsDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = alertsGridView.DisplayLayout.AutoFitStyle;
                alertsGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = headerLabel.Text + ".pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , "AuditWizard Applications View : " + alertsGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , alertsGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                this.alertsGridView.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.alertsDataSet.Tables[0].Rows.Count == 0)
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
                                            , "AuditWizard Applications View : " + alertsGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , alertsGridView
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
                this.alertsGridView.DisplayLayout.LoadFromXml(layoutFile);
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
            if (this.alertsGridView != null)
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.alertsGridView.DisplayLayout.SaveAsXml(layoutFile);
            }
        }

        #endregion Load/Save Layout

        private void alertsGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Is this the <delete> key in which case we handle it as if the user has selected delete
            // from the context menu
            if (e.KeyChar == (char)Keys.Delete)
                deleteAlertToolStripMenuItem_Click(sender, e);
        }


    }
}
