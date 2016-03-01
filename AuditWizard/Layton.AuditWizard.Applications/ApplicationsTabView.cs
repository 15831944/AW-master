using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
//
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Layton.AuditWizard.Applications
{
    [SmartPart]
    public partial class ApplicationsTabView : UserControl, ILaytonView
    {
        #region Data

        ApplicationsTabViewPresenter presenter;
        private LaytonWorkItem workItem;
        private Infragistics.Win.Appearance _compliantAppearance = new Infragistics.Win.Appearance();
        private Infragistics.Win.Appearance _noncompliantAppearance = new Infragistics.Win.Appearance();
        private Infragistics.Win.Appearance _ignoredAppearance = new Infragistics.Win.Appearance();
        private Infragistics.Win.Appearance _notSpecifiedAppearance = new Infragistics.Win.Appearance();

        /// <summary>
        /// Grid layout filename
        /// </summary>
        private static string _gridLayoutFile = "ApplicationsTabLayout.xml";

        #endregion Data

        #region Constructor

        [InjectionConstructor]
        public ApplicationsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            // Initialize the compliant/non-compliant appearances
            _noncompliantAppearance.ForeColor = System.Drawing.Color.Red;
            _compliantAppearance.ForeColor = System.Drawing.Color.Green;
            _ignoredAppearance.ForeColor = System.Drawing.Color.DimGray;
            _notSpecifiedAppearance.ForeColor = System.Drawing.Color.FromArgb(255, 110, 0);

            // Restore any saved layout
            //LoadLayout();
        }

        #endregion constructor

        #region Properties

        [CreateNew]
        public ApplicationsTabViewPresenter Presenter
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
                this.applicationsGridView.Text = headerLabel.Text;
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

        #region Methods

        public void Clear()
        {
            applicationsDataSet.Tables[0].Rows.Clear();

            // If we are displaying a specific publisher then there is little point in having the
            // publisher column itself displayed so let's make it hidden
            if (presenter.IsPublisherDisplayed)
                this.applicationsGridView.DisplayLayout.Bands[0].Columns["Publisher"].Hidden = true;
            else
                this.applicationsGridView.DisplayLayout.Bands[0].Columns["Publisher"].Hidden = false;
        }

        public List<InstalledApplication> CheckSelectedApplications()
        {
            return GetSelectedApplications();
        }


        /// <summary>
        /// Add a new application to the data set to be displayed
        /// </summary>
        /// <param name="thisComputer"></param>
        public void AddApplication(InstalledApplication thisApplication)
        {
            // Ensure that fields which may noit have a value have something to display
            string publisher = (thisApplication.Publisher == "") ? "-" : thisApplication.Publisher;
            string isCompliant = (thisApplication.IsCompliant()) ? "Compliant" : "Non-Compliant";

            // Licenses count
            string licenses;
            string variance;
            int installs;
            thisApplication.GetLicenseStatistics(out installs, out licenses, out variance);

            if (licenses == "None Specified")
                isCompliant = licenses;

            if (thisApplication.IsIgnored)
                isCompliant = "Ignored";

            // Add the row to the data set
            applicationsDataSet.Tables[0].Rows.Add(new object[] 
                { thisApplication,
                publisher,
                thisApplication.Name,				 				 
                licenses,
                installs, 
                variance,
                isCompliant,
                thisApplication.Version});

            //applicationsDataSet.Tables[0].Rows.Add(new object[] 
            //    { thisApplication.ApplicationID
            //    , publisher 
            //    , thisApplication.Name				 				 
            //    , licenses
            //    , installs 
            //    , variance
            //    , isCompliant
            //    , thisApplication.Version});
        }

        #endregion Methods

        #region Context Menu Handlers

        private void setIgnoredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a list of the selected applications
            List<InstalledApplication> listApplications = GetSelectedApplications();
            ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;
            if (wiController != null) 
                wiController.SetIgnore(listApplications);

            WorkItem.ExplorerView.RefreshView();
        }

        private void setIncludedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a list of the selected applications
            List<InstalledApplication> listApplications = GetSelectedApplications();
            ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;
            if (wiController != null) 
                wiController.SetIncluded(listApplications);

            WorkItem.ExplorerView.RefreshView();
        }

        /// <summary>
        /// Called as the context menu is opening - we enable/disable options as appropriate here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            int selectedRowCount = applicationsGridView.Selected.Rows.Count;

            // Disable all items other than export to begin with as we are pessimistic
            this.ignoreApplicationToolStripMenuItem.Enabled = false;
            this.showApplicationToolStripMenuItem.Enabled = false;
            this.propertiesToolStripMenuItem.Enabled = false;
            this.newLicenseToolStripMenuItem.Enabled = false;

            // If we have at least one item selected then enable the show/hide option
            if (selectedRowCount >= 1)
            {
                // Have we selected the GroupByRow as if this is the case then we do not enable any of
                // the individual item options
                if (this.applicationsGridView.Selected.Rows[0] is UltraGridGroupByRow)
                    return;

                // OK not the Group by row so must be applications - enable Hide and Show
                this.ignoreApplicationToolStripMenuItem.Enabled = true;
                this.showApplicationToolStripMenuItem.Enabled = true;
                this.newLicenseToolStripMenuItem.Enabled = true;

                // If we only have one item then also enable properties and new license
                if (selectedRowCount == 1)
                    this.propertiesToolStripMenuItem.Enabled = true;
            }
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
        /// View the properties of the currently selected application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We can only get the properties of a single item
            UltraGridRow selectedRow = this.applicationsGridView.Selected.Rows[0];
            InstalledApplication thisApplication = selectedRow.Cells[0].Value as InstalledApplication;
            ((ApplicationsWorkItemController)WorkItem.Controller).ApplicationProperties(thisApplication);
        }


        /// <summary>
        /// Create a new license for the selected application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We can only create a license for a single specified item
            //UltraGridRow selectedRow = this.applicationsGridView.Selected.Rows[0];
            //InstalledApplication thisApplication = selectedRow.Cells[0].Value as InstalledApplication;
            //((ApplicationsWorkItemController)WorkItem.Controller).NewLicense(thisApplication);

            List<InstalledApplication> lSelectedApplicationList = new List<InstalledApplication>();

            foreach (UltraGridRow lSelectedRow in this.applicationsGridView.Selected.Rows)
            {
                lSelectedApplicationList.Add(lSelectedRow.Cells[0].Value as InstalledApplication);
            }

            if (lSelectedApplicationList.Count == 1)
                ((ApplicationsWorkItemController)WorkItem.Controller).NewLicense(lSelectedApplicationList[0]);
            else
                ((ApplicationsWorkItemController)WorkItem.Controller).NewLicenses(lSelectedApplicationList);
        }


        #endregion Context Menu Handlers

        private List<InstalledOS> GetSelectedOSs()
        {
            List<InstalledOS> listOSs = new List<InstalledOS>();
            int selectedRowCount = applicationsGridView.Selected.Rows.Count;
            for (int isub = 0; isub < selectedRowCount; isub++)
            {
                UltraGridRow selectedRow = this.applicationsGridView.Selected.Rows[isub];
                InstalledOS thisOS = selectedRow.Cells["ApplicationObject"].Value as InstalledOS;
                listOSs.Add(thisOS);
            }
            return listOSs;
        }


        private List<InstalledApplication> GetSelectedApplications()
        {
            List<InstalledApplication> listApplications = new List<InstalledApplication>();
            int selectedRowCount = applicationsGridView.Selected.Rows.Count;
            for (int isub = 0; isub < selectedRowCount; isub++)
            {
                UltraGridRow selectedRow = this.applicationsGridView.Selected.Rows[isub];
                InstalledApplication thisApplication = selectedRow.Cells["ApplicationObject"].Value as InstalledApplication;
                listApplications.Add(thisApplication);
            }
            return listApplications;
        }

        #region Grid Functions

        /// <summary>
        /// Called as each row in the grid is initialized - we use this to set the appearance of the row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the application object being displayed
            UltraGridRow thisRow = e.Row;
            UltraGridCell objectCell = thisRow.Cells[0];
            InstalledApplication thisApplication = objectCell.Value as InstalledApplication;

            // Set the appearance and icon based on the compliancy status
            if (thisApplication.IsIgnored)
                thisRow.Appearance = _ignoredAppearance;
            else if (thisApplication.Licenses.Count == 0)
                thisRow.Appearance = _notSpecifiedAppearance;
            else if (thisApplication.IsCompliant())
                thisRow.Appearance = _compliantAppearance;
            else
                thisRow.Appearance = _noncompliantAppearance;

            // Set the 'application' image to either be a NotIgnore or non-NotIgnore application
            //UltraGridCell applicationCell = thisRow.Cells["Application Name"];
            //if (thisApplication.IsIgnored)
            //    applicationCell.Appearance.Image = Properties.Resources.application_hidden_16;
            //else
            //    applicationCell.Appearance.Image = Properties.Resources.application_16;
        }


        private void applicationsGridView_SelectionDrag(object sender, CancelEventArgs e)
        {
            List<InstalledApplication> listDraggedApplications = new List<InstalledApplication>();

            foreach (UltraGridRow selectedRow in ((UltraGrid)sender).Selected.Rows)
            {
                // Add any applications selected to our draggable list
                if (selectedRow.Cells["ApplicationObject"].Value is InstalledApplication)
                    listDraggedApplications.Add(selectedRow.Cells["ApplicationObject"].Value as InstalledApplication);
            }

            // If the tag is an Application object and our parent is a publisher then this node must be an 
            // application and as such can be dragged and dropped
            if (listDraggedApplications.Count != 0)
                this.applicationsGridView.DoDragDrop(listDraggedApplications, DragDropEffects.Move);
        }


        /// <summary>
        /// Called as we double click on a row - depending on the row selected we can view the properties of the item
        /// contained within the row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsGridView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            propertiesToolStripMenuItem_Click(sender, null);
        }

        #endregion Grid Functions

        #region Export Functions

        /// <summary>
        /// Export the graph data to an XLS format file
        /// </summary>
        public void ExportToXLS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.applicationsDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(applicationsGridView, headerLabel.Text);
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (this.applicationsDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = applicationsGridView.DisplayLayout.AutoFitStyle;
                applicationsGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = headerLabel.Text + ".pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , "AuditWizard Applications View : " + applicationsGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , applicationsGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                this.applicationsGridView.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.applicationsDataSet.Tables[0].Rows.Count == 0)
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
                                            , "AuditWizard Applications View : " + applicationsGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , applicationsGridView
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
        //private void LoadLayout()
        //{
        //    try
        //    {
        //        string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
        //        this.applicationsGridView.DisplayLayout.LoadFromXml(layoutFile);
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //}


        ///// <summary>
        ///// Called to request the grid to save it's layout to disk
        ///// </summary>
        //public void SaveLayout()
        //{
        //    if (this.applicationsGridView != null)
        //    {
        //        string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
        //        this.applicationsGridView.DisplayLayout.SaveAsXml(layoutFile);
        //    }
        //}

        #endregion Load/Save Layout

        public void ReportAllPublishers()
        {
            Cursor.Current = Cursors.WaitCursor;

            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(0);

            ((ApplicationsTabView)applicationsTabView).workItem.Controller.SetTabView(applicationsTabView);

            Cursor.Current = Cursors.Default;
        }

        public void ReportCompliantPublishers()
        {
            Cursor.Current = Cursors.WaitCursor;

            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(1);

            ((ApplicationsTabView)applicationsTabView).workItem.Controller.SetTabView(applicationsTabView);

            Cursor.Current = Cursors.Default;
        }

        public void ReportNonCompliantPublishers()
        {
            Cursor.Current = Cursors.WaitCursor;

            ILaytonView applicationsTabView = WorkItem.Items[ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(2);

            ((ApplicationsTabView)applicationsTabView).workItem.Controller.SetTabView(applicationsTabView);

            Cursor.Current = Cursors.Default;
        }

        private void aliasApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> appsToAlias = new List<int>();

            foreach (InstalledApplication installedApplication in GetSelectedApplications())
            {
                appsToAlias.Add(installedApplication.ApplicationID);
            }

            ((ApplicationsWorkItemController)WorkItem.Controller).AliasApplication(appsToAlias);
        }

        private void deleteApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete this application?" + Environment.NewLine + Environment.NewLine +
                    "All associated items will also be deleted.",
                    "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            List<InstalledApplication> selectedApplications = GetSelectedApplications();
            List<int> selectedApplicationIds = new List<int>();

            foreach (InstalledApplication installedApplication in selectedApplications)
            {
                selectedApplicationIds.Add(installedApplication.ApplicationID);
            }

            ((ApplicationsWorkItemController)WorkItem.Controller).DeleteApplication(selectedApplicationIds);
        }
    }
}
