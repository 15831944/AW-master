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
    public partial class AuditedDataTabView : UserControl, ILaytonView
    {
        #region Data
        AuditedDataTabViewPresenter presenter;
        LaytonWorkItem workItem;
        private static string _gridLayoutFile = "AuditedDataTabLayout.xml";
        private UltraTreeNode _displayedNode;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Data

        #region Data Accessors
        [CreateNew]
        public AuditedDataTabViewPresenter Presenter
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
                auditGridView.Text = value;
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
        public AuditedDataTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            // See if we have a layout file saved and if so read it to initialize the grid
            //LoadLayout();
        }

        #endregion

        #region Form Population Functions

        /// <summary>
        /// Display
        /// =======
        /// 
        /// Display the contents of the specified audited data category within this tab view.
        /// 
        /// Note that this can take 2 forms - ungrouped or grouped
        /// 
        /// Where the item is ungrouped we will display the data in 2 columns which are item and value.  This is used
        /// in the majority of hardware displays where the category is say Hardware>Processor and the displayed values are 
        /// say Speed > 2500,  Count > 2 etc.
        /// 
        /// This approach does not work for say System > Active Processes as we have multiple values to display for each
        /// active process and do not want the processes themselves displayed in the tree view.  Instead these items are grouped.
        /// What this means is that for say
        /// 
        ///		System>Active Processes>SMSS.EXE 
        ///		System>Active Processes>IEXPLORE.EXE 
        /// 
        /// The Tree view will not allow a drill down to SMSS.EXE or IEXPLORE.EXE as they are flagged as grouped.  Instead the
        /// List View will take SMSS.EXE and IEXPLORE.EXE and use these names as the first column in the List View.  The List view
        /// will then determine what values are available for each process by looking at the sub-values - for example for each 
        /// Active Process we have Name, Executable and PID.  These are added as columns to the list view and the data is grouped
        /// by the process name.
        /// 
        /// </summary>
        /// <param name="displayGroup"></param>
        public void Display(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType)
        {
            // Save the Tree Node being displayed and ensure that it is expanded
            _displayedNode = displayedNode;
            _displayedNode.Expanded = true;

            //	Call BeginUpdate to prevent drawing while we are populating the control
            this.auditGridView.BeginUpdate();
            this.Cursor = Cursors.WaitCursor;

            // Delete all entries from the current data set being displayed
            auditDataSet.Tables[0].Rows.Clear();
            auditDataSet.Tables[0].Columns.Clear();

            // We may be displaying data here for either a specific node or possible the AllAssets node and the
            // way in which we display the data depends on this so check it now
            if (displayedNode.Tag is FileSystemFolder || itemType == TreeSelectionEventArgs.ITEMTYPE.asset_filesystem)
                DisplayFileSystem(displayedNode);
            else if (displayedNode.Tag is Asset)
                DisplayForAsset(displayedNode);
            else
                DisplayForAllAssets(displayedNode);

            // Hide the DataObject column
            UltraGridColumn gridObjectColumn = this.auditGridView.DisplayLayout.Bands[0].Columns["DataObject"];
            gridObjectColumn.Hidden = true;

            // Make the item column pretty big (if it exists)
            if (this.auditGridView.DisplayLayout.Bands[0].Columns.Exists("item"))
            {
                UltraGridColumn gridItemColumn = this.auditGridView.DisplayLayout.Bands[0].Columns["Item"];
                gridItemColumn.Width = 30;
            }

            //	Restore the cursor
            this.Cursor = Cursors.Default;

            //	Call EndUpdate to resume drawing operations
            this.auditGridView.EndUpdate(true);
        }



        /// <summary>
        /// Display the data for this tab where we have selected an asset or child node thereof
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAsset(UltraTreeNode displayedNode)
        {
            // The key passed in to is the entire path down to this item whereas we only really want that 
            // which follows the asset Name portion of the key.  We therefore need to find the asset name 
            // and just recover the part we are interested in
            Asset theAsset = displayedNode.Tag as Asset;
            string itemKey = displayedNode.Key;
            string assetIdentifier = theAsset.AssetIdentifier;
            int delimiter = itemKey.IndexOf(assetIdentifier);
            //
            itemKey = itemKey.Substring(delimiter + assetIdentifier.Length + 1);

            // We need to determine whether or not the displayed node is flagged as 'Grouped' as this will affect what we 
            // display in the list view.  
            Asset displayedAsset = displayedNode.Tag as Asset;
            bool isGrouped = displayedAsset.AuditedItems.IsGrouped(itemKey);

            //if (itemKey == "Internet|Cookie")
            //    isGrouped = false;

            // Now we branch off to display as either non-grouped or grouped as required
            if (isGrouped)
                DisplayGrouped(displayedNode, itemKey, displayedAsset);
            else
                DisplayUngrouped(displayedNode, itemKey, displayedAsset);
        }


        /// <summary>
        /// Display data pertaining to the specified tree node with no grouping enforced
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayUngrouped(UltraTreeNode displayedNode, string key, Asset displayedAsset)
        {
            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // Add in columns for Item and Value
            DataColumn objectColumn = new DataColumn("DataObject", typeof(object));
            DataColumn itemColumn = new DataColumn("Item", typeof(string));
            DataColumn valueColumn = new DataColumn("Value", typeof(object));
            auditDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { objectColumn, itemColumn, valueColumn });

            //displayedNode.Nodes.Override.Sort = SortType.Descending;

            // First we add the SUB-CATEGORIES to the list
            foreach (UltraTreeNode node in displayedNode.Nodes)
            {
                auditDataSet.Tables[0].Rows.Add(new object[] { node, node.Text, "" });
            }

            string tonerColour = "black";

            // Get the ites to display within the category
            List<AuditedItem> itemsInCategory = displayedAsset.AuditedItems.GetItemsInCategory(key);
            foreach (AuditedItem item in itemsInCategory)
            {
                // we want to try and use the ink colour on the bitmap
                // get the toner name to see if it contains a value we can use
                if (item.Name == "Supply Name")
                {
                    tonerColour = item.Value;
                }
                if (item.Name == "Supply Level")
                {
                    Bitmap flag = CreateLevelBitmap(Convert.ToInt32(item.Value), tonerColour);
                    auditDataSet.Tables[0].Rows.Add(new object[] { displayedNode, item.Name, flag });
                }
                else
                {
                    auditDataSet.Tables[0].Rows.Add(new object[] { displayedNode, item.Name, item.Value + item.DisplayUnits });
                }
            }
        }

        private Bitmap CreateLevelBitmap(int aSupplyLevel, string aTonerColour)
        {
            Color fillColor = Color.Gray;

            if (aTonerColour.ToLower().Contains("cyan"))
                fillColor = Color.Cyan;

            else if (aTonerColour.ToLower().Contains("yellow"))
                fillColor = Color.Yellow;

            else if (aTonerColour.ToLower().Contains("magenta"))
                fillColor = Color.Magenta;

            else if (aTonerColour.ToLower().Contains("red"))
                fillColor = Color.Red;

            else if (aTonerColour.ToLower().Contains("green"))
                fillColor = Color.Green;

            else if (aTonerColour.ToLower().Contains("blue"))
                fillColor = Color.Blue;

            Bitmap flag = new Bitmap(200, 10);
            aSupplyLevel = aSupplyLevel * 2;

            // draw the outline
            for (int j = 0; j < 200; j++)
            {
                flag.SetPixel(j, 0, Color.Black);
                flag.SetPixel(j, 9, Color.Black);
            }
            for (int k = 0; k < 10; k++)
            {
                flag.SetPixel(0, k, Color.Black);
                flag.SetPixel(199, k, Color.Black);
            }
            for (int i = 1; i < aSupplyLevel; i++)
            {
                for (int y = 1; y < 9; y++)
                {
                    flag.SetPixel(i, y, fillColor);
                }
            }
            return flag;
        }



        /// <summary>
        /// Display data pertaining to the specified tree node where grouping is to be enforced
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayGrouped(UltraTreeNode displayedNode, string key, Asset displayedAsset)
        {
            try
            {
                // Add in columns for Object and Item as we will always have them
                DataColumn objectColumn = new DataColumn("DataObject", typeof(object));
                DataColumn itemColumn = new DataColumn("Item", typeof(string));
                auditDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { objectColumn, itemColumn });

                // OK - If we get all of the child 'categories' beneath this category then these are the 'names' of the 
                // items in the group i.e. System|Active Processes|SMSS.EXE
                // These names will be used as the first column of each row - effectively we group the attributes of
                // these names into the row
                List<string> listChildNames = displayedAsset.AuditedItems.GetChildrenInCategory(key);
                if (listChildNames.Count == 0)
                    return;

                // But what other columns do we need?  
                //
                // If we take the example of an Active process - System|Active Processes|SMSS.EXE we have audited 3 attributes 
                // per process - Name, Executable and PID - these are the same for ALL active Processes and will therefore
                // be the columns that we will add.  The easiest way to get the columns is simply to get the items in the
                // first named category and assume that all other items in this category have the same attributes
                List<AuditedItem> listColumns = displayedAsset.AuditedItems.GetItemsInCategory(listChildNames[0]);

                // Add columns for each item
                foreach (AuditedItem childColumn in listColumns)
                {
                    DataColumn column = new DataColumn(childColumn.Name, typeof(string));
                    if (!auditDataSet.Tables[0].Columns.Contains(column.ColumnName))
                        auditDataSet.Tables[0].Columns.Add(column);
                }

                // Ok now that we have added the columns we need to add the rows to the dataset also.  Note that column 1 
                // is the data object, column 2 is the category name and the remainder of the columns are the attributes
                // We therefore have 'n' + 2 columns where 'n' is the number of items in the second list plus extra columns
                // for the object and name
                int columns = listColumns.Count + 2;

                // minor fix for Bug #81 (duplicate Internet history values)
                if (key.StartsWith("Internet"))
                    columns = 3;

                foreach (string rowName in listChildNames)
                {
                    int column = 0;
                    object[] rowObjects = new object[columns];

                    rowObjects[column++] = displayedNode;

                    // The rowName is actually held as a delimited string with the full name of the category - we only want
                    // to display the last segment in the row
                    string name;
                    int lastDelimiter = rowName.LastIndexOf('|');

                    if (lastDelimiter == rowName.Length)
                        name = "";
                    else
                        name = rowName.Substring(lastDelimiter + 1);

                    if (key == "Internet|Cookie")
                    {
                        char[] charsToTrim = { '/' };
                        name = name.TrimEnd(charsToTrim);
                    }

                    rowObjects[column++] = name;

                    // Now add on the grouped values from the second list
                    List<AuditedItem> columnValues = displayedAsset.AuditedItems.GetItemsInCategory(rowName);
                    foreach (AuditedItem columnItem in columnValues)
                    {
                        try
                        {
                            rowObjects[column++] = columnItem.Value;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    // ...and add a row to the table with these values
                    auditDataSet.Tables[0].Rows.Add(rowObjects);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }


        /// <summary>
        /// Callde when we are displaying FileSystem Information
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayFileSystem(UltraTreeNode displayedNode)
        {
            DataColumn objectColumn = new DataColumn("DataObject", typeof(object));
            DataColumn itemColumn = new DataColumn("Name", typeof(string));
            auditDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { objectColumn, itemColumn });

            // First ensure that the tree is expanded
            foreach (UltraTreeNode node in displayedNode.Nodes)
            {
                auditDataSet.Tables[0].Rows.Add(new object[] { node, node.Text });
            }

            // That does it for the folders but we may also have some files.  To get these we need the FileSystemFolder 
            // object which we are displaying - luckily unless we are at the very top level this is passed as the tag of the node
            if (displayedNode.Tag is FileSystemFolder)
            {
                FileSystemFolder displayedFolder = displayedNode.Tag as FileSystemFolder;
                foreach (FileSystemFile file in displayedFolder.FilesList)
                {
                    auditDataSet.Tables[0].Rows.Add(new object[] { file, file.Name });
                }
            }
        }



        /// <summary>
        /// Display the data for this tab where we have selected an 'All Assets' node or child node thereof
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAllAssets(UltraTreeNode displayedNode)
        {
            // Determine the key for this displayed item from the key for the tree node remembering that we must
            // strip off the AllAssets key name which is the first part of the key.
            string itemKey = displayedNode.Key;
            int nDelimiter = itemKey.IndexOf(AWMiscStrings.AllAssetsNode);
            AllAssets allAssets = displayedNode.Tag as AllAssets;

            // Ensure that the tree view is already populated for this branch
            NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
            explorerView.ExpandNode(displayedNode);

            // Are we displaying a category or value/
            if (allAssets.ItemValue == "")
                DisplayAllAssetsDataCategories(displayedNode);
            else
                DisplayAllAssetsDataValues(displayedNode);
        }


        /// <summary>
        /// Called when we are expanding a category within the 'AllAssets' branch.  We are simply drilling
        /// down further on the data already contained within the tree view
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssetsDataCategories(UltraTreeNode displayedNode)
        {
            DataColumn objectColumn = new DataColumn("DataObject", typeof(object));
            DataColumn itemColumn = new DataColumn("Item", typeof(string));
            DataColumn valueColumn = new DataColumn("Value");
            auditDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { objectColumn, itemColumn, valueColumn });

            // First we add the SUB-CATEGORIES to the list
            foreach (UltraTreeNode node in displayedNode.Nodes)
            {
                auditDataSet.Tables[0].Rows.Add(new object[] { node, node.Text, "" });
            }
        }


        /// <summary>
        /// Called when we have selected a specific item value within the 'AllAssets' branch.  We are to display
        /// the assets which have this value
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayAllAssetsDataValues(UltraTreeNode displayedNode)
        {
            DataColumn objectColumn = new DataColumn("DataObject", typeof(object));
            DataColumn itemColumn = new DataColumn("Asset", typeof(string));
            auditDataSet.Tables[0].Columns.AddRange(new System.Data.DataColumn[] { objectColumn, itemColumn });

            // First we add the SUB-CATEGORIES to the list
            AllAssets allAssets = displayedNode.Tag as AllAssets;
            foreach (Asset asset in allAssets.ListAssets)
            {
                auditDataSet.Tables[0].Rows.Add(new object[] { asset, asset.Name });
            }
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

        /// <summary>
        /// Called as each row in the grid is initialized - we use this to set the appearance of the row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the application object being displayed
            UltraGridRow thisRow = e.Row;
            UltraGridCell objectCell = thisRow.Cells[0];

        }




        #endregion Form Control Functions

        #region Context Menu Handlers


        /// <summary>
        /// Called as the context menu is being displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // Properties is ONLY valid if we have a single selected row in the list view and that row
            // is showing a FileSystemFile
            if ((auditGridView.Selected.Rows.Count == 1)
            && (auditGridView.Selected.Rows[0].Cells["DataObject"].Value is FileSystemFile))
                propertiesToolStripMenuItem.Enabled = true;
            else
                propertiesToolStripMenuItem.Enabled = false;
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
        /// Display the properties of the currently selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((auditGridView.Selected.Rows.Count == 1)
            && (auditGridView.Selected.Rows[0].Cells["DataObject"].Value is FileSystemFile))
            {
                FileSystemFile file = (FileSystemFile)auditGridView.Selected.Rows[0].Cells["DataObject"].Value;
                FormFileProperties form = new FormFileProperties(file);
                form.ShowDialog();
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
            if (this.auditDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(auditGridView, headerLabel.Text);
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (this.auditDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = auditGridView.DisplayLayout.AutoFitStyle;
                auditGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = headerLabel.Text + ".pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , "AuditWizard Applications View : " + auditGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , auditGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                this.auditGridView.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.auditDataSet.Tables[0].Rows.Count == 0)
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
                                            , "AuditWizard Applications View : " + auditGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , auditGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.XPS);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }
            }
        }

        #endregion Export Functions

        #region Helper Functions

        private void CenterHeaderLabel()
        {
        }

        public void Clear()
        {
            auditDataSet.Tables[0].Rows.Clear();
        }

        public void RefreshView()
        {
            presenter.Initialize();
            base.Refresh();
        }


        /// <summary>
        /// GetSelectedAssets
        /// ====================
        /// 
        /// Returns a list of the computers selected in this view
        /// </summary>
        /// <returns></returns>
        private List<Asset> GetSelectedComputers()
        {
            List<Asset> listComputers = new List<Asset>();
            int selectedRowCount = auditGridView.Selected.Rows.Count;
            for (int isub = 0; isub < selectedRowCount; isub++)
            {
                UltraGridRow selectedRow = this.auditGridView.Selected.Rows[isub];
                Asset thisComputer = selectedRow.Cells[0].Value as Asset;
                listComputers.Add(thisComputer);
            }
            return listComputers;
        }

        #endregion Helper Functions

        #region Load/Save Layout

        /// <summary>
        /// Called to load the layout for the rid from file
        /// </summary>
        private void LoadLayout()
        {
            try
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.auditGridView.DisplayLayout.LoadFromXml(layoutFile);
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
            if (this.auditGridView != null)
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.auditGridView.DisplayLayout.SaveAsXml(layoutFile);
            }
        }

        #endregion Load/Save Layout

        private void auditGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the application object being displayed
            object dataObject = e.Row.Cells["DataObject"].Value;

            if (dataObject is Asset)
            {
                Asset asset = e.Row.Cells["DataObject"].Value as Asset;
                Bitmap icon = asset.DisplayIcon();
                e.Row.Cells[1].Appearance.Image = icon;
            }

            else if (dataObject is FileSystemFolder)
            {
                e.Row.Cells[1].Appearance.Image = Properties.Resources.folder_16;
            }

            else if (dataObject is FileSystemFile)
            {
                e.Row.Cells[1].Appearance.Image = Properties.Resources.file_16;
            }

            else
            {
                UltraTreeNode treeNode = e.Row.Cells["DataObject"].Value as UltraTreeNode;

                // Set the image for the first cell to be the same as that in the tree node
                //e.Row.Cells[1].Appearance.Image = treeNode.LeftImages[0];
            }
        }


        /// <summary>
        /// Called as we double click a row in the list - this effectively causes this item to be expanded
        /// in the tree view and hence here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auditGridView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            // Get the DataObject from the grid as this helps identify the item being double clicked.
            object dataObject = e.Row.Cells["DataObject"].Value;

            // OK - what is it?
            // In general the dataObject may be one of:
            //	UltraTreeNode - most items - this simply holds the parent item from the tree.
            //	Asset - used only in All Assets view
            //	FileSystemFile - terminus of a File System branch identifies a specific file
            //
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

            // Double-click on a FileSystemFile displays it's properties
            else if (dataObject is FileSystemFile)
            {
                propertiesToolStripMenuItem_Click(sender, null);
            }
        }


    }

    class DataGridViewProgressCell : DataGridViewImageCell
    {
        // Used to make custom cell consistent with a DataGridViewImageCell
        static Image emptyImage;
        static DataGridViewProgressCell()
        {
            emptyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        public DataGridViewProgressCell()
        {
            this.ValueType = typeof(int);
        }
        // Method required to make the Progress Cell consistent with the default Image Cell.
        // The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
        protected override object GetFormattedValue(object value,
                            int rowIndex, ref DataGridViewCellStyle cellStyle,
                            TypeConverter valueTypeConverter,
                            TypeConverter formattedValueTypeConverter,
                            DataGridViewDataErrorContexts context)
        {
            return emptyImage;
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            int progressVal = (int)value;
            float percentage = ((float)progressVal / 100.0f); // Need to convert to float before division; otherwise C# returns int which is 0 for anything but 100%.
            Brush backColorBrush = new SolidBrush(cellStyle.BackColor);
            Brush foreColorBrush = new SolidBrush(cellStyle.ForeColor);
            // Draws the cell grid
            base.Paint(g, clipBounds, cellBounds,
             rowIndex, cellState, value, formattedValue, errorText,
             cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));
            if (percentage > 0.0)
            {
                // Draw the progress bar and the text
                g.FillRectangle(new SolidBrush(Color.FromArgb(163, 189, 242)), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width - 4)), cellBounds.Height - 4);
                g.DrawString(progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
            }
            else
            {
                // draw the text
                if (this.DataGridView.CurrentRow.Index == rowIndex)
                    g.DrawString(progressVal.ToString() + "%", cellStyle.Font, new SolidBrush(cellStyle.SelectionForeColor), cellBounds.X + 6, cellBounds.Y + 2);
                else
                    g.DrawString(progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
            }
        }
    }
}
