using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinChart;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Overview
{
	public sealed partial class InventoryWidget : DefaultWidget
    {
        #region Data

        public new static string _widgetName = "Inventory Widget";
	    private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StatisticsDAO lStatisticsDAO = new StatisticsDAO();
        private LocationsDAO lLocationsDAO = new LocationsDAO();
        private TabPage _currentTab;
        private bool _rootLevel;
        private string _lastAssetType;
        private static UltraGrid _resultsGrid;

        #endregion

        public InventoryWidget()
        {
            InitializeComponent();
            CreateNewGrid();
            RefreshWidget();
            _rootLevel = true;
            ContextMenu = new ContextMenu();
        }

		public override string WidgetName()
		{
			return _widgetName;
		}

        public override void RefreshWidget()
        {
            PopulateTabPages();

            string selectedItemText = comboBoxLocations.Text;

            PopulateComboBox();

            comboBoxLocations.SelectedIndex = 
                (comboBoxLocations.FindString(selectedItemText) == -1) ? 0 : comboBoxLocations.FindString(selectedItemText);

            RedrawPieChart();
        }

        void inventoryChart_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {
            UltraChart currentChart = GetChartFromControls();

            if (_rootLevel)
            {
                if (currentChart != null)
                    currentChart.DataSource = lStatisticsDAO.GetDashboardInventoryItems(e.RowLabel, (int)comboBoxLocations.SelectedValue);

                _lastAssetType = e.RowLabel;
                _rootLevel = false;
                _resultsGrid.Visible = false;

                lbDisplayInfo.Text = String.Format("Displaying {0} -> {1} by manufacturer in '{2}'", tabControl1.SelectedTab.Text, e.RowLabel, comboBoxLocations.Text);
            }
            else
            {
                // user clicked on second level so need to display assets which have this manufacturer
                DataTable assetTable = lStatisticsDAO.GetDashboardInventoryAssets(e.RowLabel, _lastAssetType, tabControl1.SelectedTab.Text, (int)comboBoxLocations.SelectedValue);

                _resultsGrid.Visible = true;
                _resultsGrid.DataSource = assetTable;
                _resultsGrid.Dock = DockStyle.Fill;

                currentChart.Visible = false;
                tabControl1.SelectedTab.Controls.Add(_resultsGrid);

                _resultsGrid.DisplayLayout.Bands[0].Columns[0].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                _resultsGrid.DisplayLayout.Bands[0].Columns[0].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                _resultsGrid.DisplayLayout.Bands[0].Columns[1].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                _resultsGrid.DisplayLayout.Bands[0].Columns[1].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                _resultsGrid.DisplayLayout.Bands[0].Columns[2].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                _resultsGrid.DisplayLayout.Bands[0].Columns[2].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                lbDisplayInfo.Text = String.Format
                    ("{0} asset(s) of Type '{1}' in '{2}' with manufacturer of '{3}'", 
                    _resultsGrid.Rows.Count, _lastAssetType, comboBoxLocations.Text, e.RowLabel);         
            }
        }

        private void PopulateTabPages()
        {
            DataTable lParentAssetTypesDataTable = lStatisticsDAO.GetParentAssetTypes();
            TabPage tabPage;
            List<string> lCurrentAssetTypes = new List<string>();
            string tabName;

            foreach (DataRow parentAssetTypeRow in lParentAssetTypesDataTable.Rows)
            {
                tabName = parentAssetTypeRow.ItemArray[0].ToString();
                tabPage = new TabPage(tabName);

                if (!CheckTabPageExists(tabName))
                {
                    tabPage.Controls.Add(CreateChart());
                    tabControl1.TabPages.Add(tabPage);
                }

                lCurrentAssetTypes.Add(tabName);
            }

            foreach (TabPage currentTabPage in tabControl1.TabPages)
            {
                if (!lCurrentAssetTypes.Contains(currentTabPage.Text))
                    tabControl1.TabPages.Remove(currentTabPage);
            }

            _currentTab = tabControl1.TabPages[0];
        }

        private bool CheckTabPageExists(string aTabName)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage.Text == aTabName)
                    return true;
            }

            return false;
        }

        private void PopulateComboBox()
        {
            try
            {
                DataTable lLocationsDataTable = lLocationsDAO.GetAllLocationsLight();
                DataTable newTable = lLocationsDataTable.Clone();

                DataRow resultRow = newTable.NewRow();
                resultRow[0] = -1;
                resultRow[1] = "All Locations";
                newTable.Rows.Add(resultRow);

                foreach (DataRow row in lLocationsDataTable.Rows)
                {
                    newTable.ImportRow(row);
                }

                comboBoxLocations.DataSource = newTable;
                comboBoxLocations.DisplayMember = "_FULLNAME";
                comboBoxLocations.ValueMember = "_LOCATIONID";
            }
            catch (Exception ex)
            {
                string sd = ex.Message;
            }
        }

        private DataTable PopulatePieChart(string aAssetType, int aLocationID)
        {
            return lStatisticsDAO.GetTopLevelDashboardInventoryItems(aAssetType, aLocationID);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentTab = tabControl1.SelectedTab;
            RedrawPieChart();
        }

        private void RedrawPieChart()
        {
            _rootLevel = true;
            string lTabName = _currentTab.Text;
            UltraChart currentChart = GetChartFromControls();

			if (currentChart != null && comboBoxLocations.SelectedValue != null && comboBoxLocations.SelectedValue is int)
            {
                 currentChart.DataSource = PopulatePieChart(lTabName, (int)comboBoxLocations.SelectedValue);
                currentChart.Visible = true;
                _resultsGrid.Visible = false;
            }

            if (comboBoxLocations.Text == "System.Data.DataRowView")
                comboBoxLocations.Text = "All Locations";

            lbDisplayInfo.Text = String.Format("Displaying {0} by Type in '{1}'", lTabName.ToUpper(), comboBoxLocations.Text);
        }

        private UltraChart GetChartFromControls()
        {
            UltraChart currentChart = null;            

            foreach (Control control in _currentTab.Controls)
            {
                if (control.GetType() == typeof(UltraChart))
                {
                    currentChart = (UltraChart)control;
                    break;
                }
            }

            return currentChart;
        }

        private void comboBoxLocations_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RedrawPieChart();
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            RedrawPieChart();
        }

        private void ultraChart1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            LabelStyle ls = new LabelStyle();
            ls.FontColor = Color.DarkGray;

            Font font = new Font("Verdana", 12.75F, FontStyle.Italic);

            ls.Font = font;
            ls.VerticalAlign = StringAlignment.Center;
            ls.HorizontalAlign = StringAlignment.Center;

            e.LabelStyle = ls;
            e.Text = "No results were returned for this item.";
        }

        private void CreateNewGrid()
        {
            _resultsGrid = new UltraGrid();

            try
            {
                Infragistics.Win.Appearance lGridAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lRowAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lRowAlternateAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lSelectedRowAppearance = new Infragistics.Win.Appearance();

                lGridAppearance.BackColor = Color.White;
                _resultsGrid.DisplayLayout.Appearance = lGridAppearance;

                lSelectedRowAppearance.BackColor = Color.FromArgb(238, 243, 223);
                lSelectedRowAppearance.ForeColor = Color.Black;
                _resultsGrid.DisplayLayout.Override.SelectedRowAppearance = lSelectedRowAppearance;

                lRowAppearance.BackColor = Color.White;

                _resultsGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                _resultsGrid.Dock = DockStyle.Fill;

                lRowAppearance.BorderColor = Color.LightGray;
                lRowAppearance.TextVAlignAsString = "Middle";
                lRowAppearance.ForeColor = Color.DimGray;
                lRowAlternateAppearance.BackColor = Color.FromArgb(246, 252, 255);
                _resultsGrid.DisplayLayout.Override.RowAppearance = lRowAppearance;
                _resultsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
                _resultsGrid.DisplayLayout.RowConnectorStyle = RowConnectorStyle.None;
                _resultsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

                _resultsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.Override.BorderStyleFilterOperator = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.Override.BorderStyleFilterRow = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.Override.CellPadding = 4;
                _resultsGrid.DisplayLayout.Override.RowAlternateAppearance = lRowAlternateAppearance;

                _resultsGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                _resultsGrid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.BasedOnDataType;
                _resultsGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                _resultsGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Single;

                _resultsGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinGroup;
                _resultsGrid.DisplayLayout.Override.AllowColSwapping = AllowColSwapping.WithinGroup;

                _resultsGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                //lResultsGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.White;
                _resultsGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.FromArgb(221, 236, 255);
                _resultsGrid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.DimGray;
                _resultsGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                _resultsGrid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.White;
                _resultsGrid.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;

                _resultsGrid.DisplayLayout.Override.FilterCellAppearance.BackColor = Color.FromArgb(240, 248, 251);

                // Set the view style to OutlookGroupBy. Without it, group by box won't show up
                //lResultsGrid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

                Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
                appearance14.BackColor = Color.FromArgb(216, 228, 248);
                appearance14.BackColor2 = Color.FromArgb(157, 185, 235);
                //appearance14.BackColor = Color.White;
                appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
                //appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.None;
                appearance14.FontData.BoldAsString = "True";
                appearance14.FontData.Name = "Verdana";
                appearance14.FontData.SizeInPoints = 10F;
                appearance14.ForeColor = Color.FromArgb(60, 127, 177);
                //appearance14.ForeColor = Color.DimGray;
                _resultsGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
                _resultsGrid.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                _resultsGrid.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;

                Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
                appearance15.BackColor = Color.Transparent;
                appearance15.FontData.Name = "Verdana";
                _resultsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance15;

                _resultsGrid.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
                _resultsGrid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
                _resultsGrid.DisplayLayout.Override.SelectTypeCol = SelectType.None;

                _resultsGrid.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;                
                _resultsGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFixed;
                _resultsGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;

                _resultsGrid.InitializeRow += lResultsGrid_InitializeRow;

                _resultsGrid.ContextMenuStrip = contextMenuStripExportData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
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
        /// Export the graph data to an XLS format file
        /// </summary>
        public void ExportToXLS()
        {
            // If there are no rows in the grid then we cannot export
            if (_resultsGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(_resultsGrid, "AuditWizardData");
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (_resultsGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = _resultsGrid.DisplayLayout.AutoFitStyle;
                _resultsGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = "AuditWizardData.pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , ""
                                            , ""
                                            , ""
                                            , _resultsGrid
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);

                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                _resultsGrid.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (_resultsGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = "AuditWizardData.xps";
                saveFileDialog.Filter = "XML Paper Specification (*.xps)|*.xps";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , ""
                                            , ""
                                            , ""
                                            , _resultsGrid
                                            , Infragistics.Documents.Reports.Report.FileFormat.XPS);

                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }
            }
        }

        void lResultsGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.PerformAutoSize();
        }

        private UltraChart CreateChart()
        {
            UltraChart inventoryChart = new UltraChart();
            Infragistics.UltraChart.Resources.Appearance.PaintElement paintElement1 = new Infragistics.UltraChart.Resources.Appearance.PaintElement();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            Infragistics.UltraChart.Resources.Appearance.PieChartAppearance pieChartAppearance1 = new Infragistics.UltraChart.Resources.Appearance.PieChartAppearance();

            inventoryChart.Tooltips.Overflow = TooltipOverflow.ChartArea;
            inventoryChart.ChartType = ChartType.PieChart;
            // 
            // ultraChart1
            // 
            inventoryChart.Axis.BackColor = Color.FromArgb(255, 248, 220);
            paintElement1.ElementType = PaintElementType.None;
            paintElement1.Fill = Color.FromArgb(255, 248, 220);
            inventoryChart.Axis.PE = paintElement1;
            inventoryChart.Axis.X.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.X.Labels.FontColor = Color.DimGray;
            inventoryChart.Axis.X.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            inventoryChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.X.Labels.SeriesLabels.FontColor = Color.DimGray;
            inventoryChart.Axis.X.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.X.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.X.LineThickness = 1;
            inventoryChart.Axis.X.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.X.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.X.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.X.MajorGridLines.Visible = true;
            inventoryChart.Axis.X.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.X.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.X.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.X.MinorGridLines.Visible = false;
            inventoryChart.Axis.X.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.X.Visible = true;
            inventoryChart.Axis.X2.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.X2.Labels.FontColor = Color.Gray;
            inventoryChart.Axis.X2.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.X2.Labels.ItemFormatString = "";
            inventoryChart.Axis.X2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X2.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.X2.Labels.SeriesLabels.FontColor = Color.Gray;
            inventoryChart.Axis.X2.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.X2.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.X2.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.X2.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.X2.Labels.Visible = false;
            inventoryChart.Axis.X2.LineThickness = 1;
            inventoryChart.Axis.X2.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.X2.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.X2.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.X2.MajorGridLines.Visible = true;
            inventoryChart.Axis.X2.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.X2.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.X2.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.X2.MinorGridLines.Visible = false;
            inventoryChart.Axis.X2.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.X2.Visible = false;
            inventoryChart.Axis.Y.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Y.Labels.FontColor = Color.DimGray;
            inventoryChart.Axis.Y.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            inventoryChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.DimGray;
            inventoryChart.Axis.Y.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Y.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Y.LineThickness = 1;
            inventoryChart.Axis.Y.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Y.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Y.MajorGridLines.Visible = true;
            inventoryChart.Axis.Y.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Y.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.Y.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Y.MinorGridLines.Visible = false;
            inventoryChart.Axis.Y.TickmarkInterval = 10;
            inventoryChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.Y.Visible = true;
            inventoryChart.Axis.Y2.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Y2.Labels.FontColor = Color.Gray;
            inventoryChart.Axis.Y2.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Y2.Labels.ItemFormatString = "";
            inventoryChart.Axis.Y2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y2.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Y2.Labels.SeriesLabels.FontColor = Color.Gray;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Y2.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Y2.Labels.Visible = false;
            inventoryChart.Axis.Y2.LineThickness = 1;
            inventoryChart.Axis.Y2.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Y2.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.Y2.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Y2.MajorGridLines.Visible = true;
            inventoryChart.Axis.Y2.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Y2.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.Y2.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Y2.MinorGridLines.Visible = false;
            inventoryChart.Axis.Y2.TickmarkInterval = 10;
            inventoryChart.Axis.Y2.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.Y2.Visible = false;
            inventoryChart.Axis.Z.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Z.Labels.FontColor = Color.DimGray;
            inventoryChart.Axis.Z.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Z.Labels.ItemFormatString = "";
            inventoryChart.Axis.Z.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Z.Labels.SeriesLabels.FontColor = Color.DimGray;
            inventoryChart.Axis.Z.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Z.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Z.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Z.Labels.Visible = false;
            inventoryChart.Axis.Z.LineThickness = 1;
            inventoryChart.Axis.Z.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Z.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.Z.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Z.MajorGridLines.Visible = true;
            inventoryChart.Axis.Z.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Z.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.Z.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Z.MinorGridLines.Visible = false;
            inventoryChart.Axis.Z.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.Z.Visible = false;
            inventoryChart.Axis.Z2.Labels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Z2.Labels.FontColor = Color.Gray;
            inventoryChart.Axis.Z2.Labels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Z2.Labels.ItemFormatString = "";
            inventoryChart.Axis.Z2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z2.Labels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
            inventoryChart.Axis.Z2.Labels.SeriesLabels.FontColor = Color.Gray;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Z2.Labels.VerticalAlign = StringAlignment.Center;
            inventoryChart.Axis.Z2.Labels.Visible = false;
            inventoryChart.Axis.Z2.LineThickness = 1;
            inventoryChart.Axis.Z2.MajorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Z2.MajorGridLines.Color = Color.Gainsboro;
            inventoryChart.Axis.Z2.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Z2.MajorGridLines.Visible = true;
            inventoryChart.Axis.Z2.MinorGridLines.AlphaLevel = 255;
            inventoryChart.Axis.Z2.MinorGridLines.Color = Color.LightGray;
            inventoryChart.Axis.Z2.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
            inventoryChart.Axis.Z2.MinorGridLines.Visible = false;
            inventoryChart.Axis.Z2.TickmarkStyle = AxisTickStyle.Smart;
            inventoryChart.Axis.Z2.Visible = false;
            inventoryChart.BackgroundImageLayout = ImageLayout.Center;
            inventoryChart.Border.Thickness = 0;
            inventoryChart.ColorModel.AlphaLevel = 100;
            inventoryChart.ColorModel.ColorBegin = Color.Pink;
            inventoryChart.ColorModel.ColorEnd = Color.DarkRed;
            inventoryChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            inventoryChart.Dock = DockStyle.Fill;
            gradientEffect1.Coloring = GradientColoringStyle.Lighten;
            gradientEffect1.Style = GradientStyle.BackwardDiagonal;
            inventoryChart.Effects.Effects.Add(gradientEffect1);
            inventoryChart.EmptyChartText = "";
            inventoryChart.Location = new Point(0, 0);
            inventoryChart.Name = "inventoryChart";
            pieChartAppearance1.Labels.Font = new Font("Verdana", 7.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pieChartAppearance1.Labels.FontColor = Color.DimGray;
            pieChartAppearance1.Labels.FormatString = "<ITEM_LABEL>  (<DATA_VALUE:#0>)";
            pieChartAppearance1.Labels.LeaderLineColor = Color.DimGray;
            pieChartAppearance1.RadiusFactor = 85;
            inventoryChart.PieChart = pieChartAppearance1;
            inventoryChart.Size = new Size(526, 249);
            inventoryChart.TabIndex = 2;
            inventoryChart.Tooltips.Font = new Font("Verdana", 7.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            inventoryChart.Tooltips.FormatString = "<ITEM_LABEL>";
            inventoryChart.Tooltips.HighlightFillColor = Color.LightBlue;;
            inventoryChart.Tooltips.HighlightOutlineColor = Color.DarkGray;
            inventoryChart.InvalidDataReceived += ultraChart1_InvalidDataReceived;
            inventoryChart.ChartDataClicked += inventoryChart_ChartDataClicked;
            //inventoryChart.ChartDrawItem += new Infragistics.UltraChart.Shared.Events.ChartDrawItemEventHandler(inventoryChart_ChartDrawItem);
            inventoryChart.PieChart.OthersCategoryPercent = 0;

            return inventoryChart;
        }

        private void InventoryWidget_VisibleChanged(object sender, EventArgs e)
        {
            RefreshWidget();
        }
    }
}

