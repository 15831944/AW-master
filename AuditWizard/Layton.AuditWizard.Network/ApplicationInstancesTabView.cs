using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class ApplicationInstancesTabView : UserControl, ILaytonView
    {
        #region Data
        ApplicationInstancesTabViewPresenter presenter;
        UltraTreeNode _displayedNode = null;
        TreeSelectionEventArgs.ITEMTYPE _itemType;

        LaytonWorkItem workItem;
        private static string _gridLayoutFile = "NetworkInstancesTabLayout.xml";
        #endregion Data

        #region Properties
        [InjectionConstructor]
        public ApplicationInstancesTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            mainSplitContainer.SplitterDistance = 104;
            headerGroupBox.SizeChanged += new EventHandler(headerGroupBox_SizeChanged);
        }

        void headerGroupBox_SizeChanged(object sender, EventArgs e)
        {
            CenterHeaderLabel();
        }

        [CreateNew]
        public ApplicationInstancesTabViewPresenter Presenter
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

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        public string HeaderText
        {
            set
            {
                headerLabel.Text = value;
                this.applicationsGridView.Text = value;
                CenterHeaderLabel();
            }
        }

        #endregion Properties

        #region Display Functions

        public void RefreshView()
        {
            presenter.Show(presenter.DisplayedNode, _itemType);
            base.Refresh();
        }

        public void Clear()
        {
            applicationsDataSet.Tables[0].Rows.Clear();
        }

        private void CenterHeaderLabel()
        {
        }



        /// <summary>
        /// This function is called from our _presenter to cause the applications for the specified asset or 'All Assets'
        /// branch to be displayed.  We are passed the UltraTreeNode which identifies the parent for which we are
        /// displaying children
        /// </summary>
        /// <param name="displayedNode"></param>
        public void Display(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType)
        {
            _displayedNode = displayedNode;
            _itemType = itemType;

            //	Call BeginUpdate to prevent drawing while we are populating the control
            this.applicationsGridView.BeginUpdate();
            this.Cursor = Cursors.WaitCursor;

            // Delete all entries from the current data set being displayed as we do not know the layout of the data
            // which we will be displaying this time through
            applicationsDataSet.Tables[0].Rows.Clear();
            applicationsDataSet.Tables[0].Columns.Clear();

            // We may be either displaying applications installed for a specific asset or alternatively may be going
            // down the 'All Assets' branch and displaying applications installed for all assets in this branch
            if (displayedNode.Tag is Asset)
                DisplayForAsset(displayedNode);

            else if (displayedNode.Tag is AllAssets)
                DisplayForAllAssets(displayedNode, itemType);

            //	Restore the cursor
            this.Cursor = Cursors.Default;

            // The first column in the grid is ALWAYS hidden
            this.applicationsGridView.DisplayLayout.Bands[0].Columns[0].Hidden = true;

            //try
            //{
            //    UltraGridColumn gridItemColumn = this.applicationsGridView.DisplayLayout.Bands[0].Columns["Publisher"];
            //    gridItemColumn.Width = 200;

            //    gridItemColumn = this.applicationsGridView.DisplayLayout.Bands[0].Columns["Name"];
            //    gridItemColumn.Width = 220;
            //}
            //catch (Exception)
            //{
            //}

            //	Call EndUpdate to resume drawing operations
            this.applicationsGridView.EndUpdate(true);
        }




        /// <summary>
        /// Display the data for this tab where we have selected an asset or child node thereof
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAsset(UltraTreeNode displayedNode)
        {
            // We are displaying ALL publishers and ALL applications for this asset - add in columns for
            // Application Object, Publisher, Name, Version, Serial Number and CD Key
            DataColumn column1 = new DataColumn("ApplicationObject", typeof(object));
            DataColumn column2 = new DataColumn("Publisher", typeof(string));
            DataColumn column3 = new DataColumn("Name", typeof(string));
            DataColumn column4 = new DataColumn("Version", typeof(string));
            DataColumn column5 = new DataColumn("Serial Number", typeof(string));
            DataColumn column6 = new DataColumn("CD Key", typeof(string));

            // Add these columns to the DataSet
            applicationsDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { column1, column2, column3, column4, column5, column6 });

            // Get the work item controller
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            Asset theAsset = displayedNode.Tag as Asset;

            // ...and from there settings which alter what we display in this view
            bool showIncluded = wiController.ShowIncludedApplications;
            bool showIgnored = wiController.ShowIgnoredApplications;
            String publisherFilter = wiController.PublisherFilter;

            // Call database function to return list of applications (for the specified publisher)
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            DataTable applicationsTable = lwDataAccess.GetInstalledApplications(theAsset, publisherFilter, showIncluded, showIgnored);

            foreach (DataRow row in applicationsTable.Rows)
            {
                ApplicationInstance newApplication = new ApplicationInstance(row);
                AddApplication(newApplication);
            }
        }


        /// <summary>
        /// Display the data for this tab where we have selected an 'All Assets' node or child node thereof
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAllAssets(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType)
        {
            // Ok we are displaying data for a node beneath 'All Assets' however this still means that we could
            // be displaying one of
            //	
            //	All Publishers
            //	Applications for a Publisher
            //	Assets for an Application
            //	Operating System Family
            //
            if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_applications)
                DisplayAllAssets_Applications(displayedNode);

            else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_publisher)
                DisplayAllAssets_Publisher(displayedNode);

            else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_application)
                DisplayAllAssets_Application(displayedNode);

            else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_os)
                DisplayAllAssets_OS(displayedNode);
        }


        /// <summary>
        /// This function is called when we have selected the 'Applications' node immediately below 'All Assets'
        /// In this case we need to display a list of Publishers defined in the database
        /// 
        /// Note - technically we should filter this list to only include publishers of applications for which there
        /// is an instance on one of the PCs below the selected level but for now we shall just display all
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssets_Applications(UltraTreeNode displayedNode)
        {
            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // We are displaying applications for a specific publisher - add in columns for
            // Application Object and Application
            DataColumn column1 = new DataColumn("ApplicationObject", typeof(object));
            DataColumn column2 = new DataColumn("Publisher", typeof(string));

            // Add these columns to the DataSet
            applicationsDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { column1, column2 });

            // get a list of the applications for this publisher from the tree
            foreach (UltraTreeNode node in displayedNode.Nodes)
            {
                applicationsDataSet.Tables[0].Rows.Add(new object[] { node, node.Text });
            }
        }

        /// <summary>
        /// This function is called when we have selected a specific publisher node below 'All Assets'
        /// In this case we need to display a list of Applications for this publisher
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssets_Publisher(UltraTreeNode displayedNode)
        {
            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // We are displaying applications for a specific publisher - add in columns for
            // Application Object and Application
            DataColumn column1 = new DataColumn("ApplicationObject", typeof(object));
            DataColumn column2 = new DataColumn("Name", typeof(string));

            // Add these columns to the DataSet
            applicationsDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { column1, column2 });

            // get a list of the applications for this publisher from the tree
            foreach (UltraTreeNode node in displayedNode.Nodes)
            {
                // Note we save the UltraTreeNode tag with this row as this should relate to the application
                applicationsDataSet.Tables[0].Rows.Add(new object[] { node, node.Text });
            }
        }


        /// <summary>
        /// This function is called when we have selected a specific 'application' node below 'All Assets'
        /// In this case we need to display a list of assets for which this application has been installed
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssets_Application(UltraTreeNode displayedNode)
        {
            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // We are displaying ALL publishers - add in columns for
            // Application Object and Publisher
            DataColumn column1 = new DataColumn("ApplicationObject", typeof(object));
            DataColumn column2 = new DataColumn("Asset", typeof(string));
            DataColumn column3 = new DataColumn("Version", typeof(string));
            DataColumn column4 = new DataColumn("Serial Number", typeof(string));
            DataColumn column5 = new DataColumn("CD Key", typeof(string));

            // Add these columns to the DataSet
            applicationsDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { column1, column2, column3, column4, column5 });

            // Get a list of assets for which this application has 
            AllAssets allAssets = displayedNode.Tag as AllAssets;
            InstalledApplication thisApplication = allAssets.Tag as InstalledApplication;

            // Read instances/licenses of this application
            thisApplication.LoadData();

            // This will give us (any) instances
            foreach (ApplicationInstance instance in thisApplication.Instances)
            {
                // Note we save the UltraTreeNode tag with this row as this should relate to the application
                Asset asset = new Asset();
                asset.Name = instance.InstalledOnComputer;
                asset.AssetID = instance.InstalledOnComputerID;
                asset.Icon = instance.InstalledOnComputerIcon;
                applicationsDataSet.Tables[0].Rows.Add(new object[] { asset, asset.Name, instance.Version, instance.Serial.ProductId, instance.Serial.CdKey });
            }
        }



        /// <summary>
        /// This function is called when we have selected a specific 'Operating System Family' node below 'All Assets'
        /// In this case we need to display a list of assets for which this Operating System has been installed
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssets_OS(UltraTreeNode displayedNode)
        {
            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // We are displaying ALL publishers - add in columns for
            // Application Object and Publisher
            DataColumn column1 = new DataColumn("ApplicationObject", typeof(object));
            DataColumn column2 = new DataColumn("Asset", typeof(string));
            DataColumn column3 = new DataColumn("Version", typeof(string));
            DataColumn column4 = new DataColumn("Serial Number", typeof(string));
            DataColumn column5 = new DataColumn("CD Key", typeof(string));

            // Add these columns to the DataSet
            applicationsDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { column1, column2, column3, column4, column5 });

            // Get a list of assets for which this application has 
            AllAssets allAssets = displayedNode.Tag as AllAssets;
            InstalledOS thisOS = allAssets.Tag as InstalledOS;

            // This will give us (any) instances
            foreach (OSInstance instance in thisOS.Instances)
            {
                // Note we save the UltraTreeNode tag with this row as this should relate to the application
                Asset asset = new Asset();
                asset.Name = instance.InstalledOnComputer;
                asset.AssetID = instance.InstalledOnComputerID;
                asset.Icon = instance.InstalledOnComputerIcon;
                applicationsDataSet.Tables[0].Rows.Add(new object[] { asset, asset.Name, instance.Version, instance.Serial.ProductId, instance.Serial.CdKey });
            }
        }

        #endregion Display Functions

        #region Grid Data Functions

        /// <summary>
        /// Add a new Installed ApplicationID to the data set to be displayed
        /// </summary>
        /// <param name="thisComputer"></param>
        public void AddApplication(ApplicationInstance newApplication)
        {
            String publisher = (newApplication.Publisher == "") ? "-" : newApplication.Publisher;
            String serialNumber = (newApplication.Serial.ProductId == "") ? "-" : newApplication.Serial.ProductId;
            String cdKey = (newApplication.Serial.CdKey == "") ? "-" : newApplication.Serial.CdKey;

            applicationsDataSet.Tables[0].Rows.Add(new object[] { newApplication
															, publisher
															, newApplication.Name
															, newApplication.Version
															, serialNumber
															, cdKey });
        }


        /// <summary>
        /// Called to show the properties of an application - note that as this grid may display a variety of different
        /// types of data we need to be certain that the item being displayed is indeed an application!
        /// </summary>
        private void ShowProperties()
        {
            if ((applicationsGridView.Selected.Rows.Count == 1)
            && (applicationsGridView.Selected.Rows[0].Cells.Count != 0)
            && ((applicationsGridView.Selected.Rows[0].Cells[0].Value is ApplicationInstance)))
            {
                UltraGridRow selectedRow = applicationsGridView.Selected.Rows[0];
                ApplicationInstance thisInstance = selectedRow.Cells[0].Value as ApplicationInstance;
                FormApplicationInstanceProperties form = new FormApplicationInstanceProperties(thisInstance);
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshView();
            }
        }

        #endregion Grid Data Functions

        #region Grid Message Functions

        /// <summary>
        /// Called as the grid layout is initialized - if we have a publisher column then set it to
        /// be the primary sort column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsGridView_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (applicationsGridView.DisplayLayout.Bands[0].Columns.Count > 1)
                applicationsGridView.DisplayLayout.Bands[0].Columns[1].SortIndicator = SortIndicator.Descending;
        }

        /// <summary>
        /// Called as each row in the grid is initialized - we use this to set the appearance of the row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void computerGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the application object being displayed
            UltraGridRow thisRow = e.Row;
            ApplicationInstance thisInstance = thisRow.Cells[0].Value as ApplicationInstance;

            // If we have a Publisher then set its icon
            if (thisRow.Cells.Exists("Publisher"))
                thisRow.Cells["Publisher"].Appearance.Image = Properties.Resources.application_publisher_16;

            // If we have an Asset then set its icon
            if (thisRow.Cells.Exists("Asset"))
            {
                Asset asset = thisRow.Cells["ApplicationObject"].Value as Asset;
                thisRow.Cells["Asset"].Appearance.Image = asset.DisplayIcon();
            }

            // If we can find an application cell then set its image also
            if (thisRow.Cells.Exists("Name"))
            {
                if (thisInstance != null)
                    thisRow.Cells["Name"].Appearance.Image =
                        (thisInstance.IsIgnored) ? Properties.Resources.application_hidden_16 : Properties.Resources.application_16;
                else
                    thisRow.Cells["Name"].Appearance.Image = Properties.Resources.application_16;
            }
        }


        private void applicationsGridView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            // Do not allow double -click on the group by row
            if (e.Row is UltraGridGroupByRow)
                return;

            // Get the DataObject from the grid as this helps identify the item being double clicked.
            object dataObject = e.Row.Cells["ApplicationObject"].Value;

            //  If we double click an UltraTreeNode this causes it to be expanded
            if (dataObject is UltraTreeNode)
            {
                UltraTreeNode treeNode = dataObject as UltraTreeNode;

                // ...and cause it to be expanded
                treeNode.Expanded = true;

                // ...and select it also
                UltraTree treeControl = treeNode.Control as UltraTree;
                treeNode.BringIntoView(true);
                _displayedNode.Selected = false;
                treeNode.Selected = true;
            }

            else
            {
                // If not an UltraTreeNode then try and display the items properties
                applicationsGridView.Selected.Rows.Clear();
                applicationsGridView.Selected.Rows.Add(e.Row);

                // ...and get it's properties
                ShowProperties();
            }
        }



        /// <summary>
        ///  Handle mouse down by selecting the item over which the mouse is placed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsGridView_MouseDown(object sender, MouseEventArgs e)
        {
            UltraGridRow row;
            UIElement element;

            if (e.Button == MouseButtons.Right)
            {
                element = applicationsGridView.DisplayLayout.UIElement.ElementFromPoint(e.Location);
                row = element.GetContext(typeof(UltraGridRow)) as UltraGridRow;
                if (row != null && row.IsDataRow)
                    row.Selected = true;
            }
        }

        #endregion Grid Message Functions

        #region Context Menu Handlers


        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // Properties is only applicable if we have selected a single application instance
            bool display = ((applicationsGridView.Selected.Rows.Count == 1)
                         && (applicationsGridView.Selected.Rows[0].Cells != null)
                         && (applicationsGridView.Selected.Rows[0].Cells.Count != 0)
                         && ((applicationsGridView.Selected.Rows[0].Cells[0].Value is ApplicationInstance)));
            propertiesToolStripMenuItem.Enabled = display;
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
        /// Called when we request the properties of an application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProperties();
        }

        #endregion Context Menu Handlers

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
                    UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
                    exporter.Export(applicationsGridView, saveFileDialog.FileName, GridExportFileFormat.PDF);
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
                    UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
                    exporter.Export(applicationsGridView, saveFileDialog.FileName, GridExportFileFormat.XPS);
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
                this.applicationsGridView.DisplayLayout.LoadFromXml(layoutFile);
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
            if (this.applicationsGridView != null)
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.applicationsGridView.DisplayLayout.SaveAsXml(layoutFile);
            }
        }

        #endregion Load/Save Layout

    }
}
