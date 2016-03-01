using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Overview
{
    public partial class ServerDataWidget : DefaultWidget
    {
        public new static string _widgetName = "ServerData Widget";
        private StatisticsDAO _statisticsDAO;
        private UltraChart lStackColumnChart;

        public override string WidgetName()
        {
            return _widgetName;
        }

        public ServerDataWidget()
        {
            InitializeComponent();
            _statisticsDAO = new StatisticsDAO();

            cbServers.DataSource = _statisticsDAO.GetServers();
            cbServers.DisplayMember = "_NAME";
            cbServers.ValueMember = "_ASSETID";

            lStackColumnChart = CreateNewStackColumnChart();
            splitContainer1.Panel2.Controls.Add(lStackColumnChart);

            if (cbServers.SelectedValue != null)
            {                
                lStackColumnChart.DataSource = PopulateChartData((int)cbServers.SelectedValue);
                
            }

            this.ContextMenu = new ContextMenu();
        }

        private DataTable PopulateChartData(int assetID)
        {
            DataTable resultsDataTable = new DataTable();

            DataTable uniqueDrivesDataTable = _statisticsDAO.UniqueDriveLettersForAsset(assetID);
            DataTable serverDriveDataTable = _statisticsDAO.StatisticsServerData(assetID);

            DataColumn dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Drive Letter";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Used Space";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Free Space";
            resultsDataTable.Columns.Add(dataColumn);

            try
            {
                foreach (DataRow driveLetterRow in uniqueDrivesDataTable.Rows)
                {
                    string driveLetter = (string)driveLetterRow.ItemArray[1];

                    DataRow[] serverDriveRows = serverDriveDataTable.Select("_CATEGORY = '" + driveLetter + "'");

                    if (serverDriveRows[6].ItemArray[3].ToString() != "Fixed Drive")
                        continue;

                    DataRow newDataRow = resultsDataTable.NewRow();
                    newDataRow[0] = serverDriveRows[3].ItemArray[3].ToString();

                    int usedSpace = Convert.ToInt32(serverDriveRows[4].ItemArray[3]) - Convert.ToInt32(serverDriveRows[5].ItemArray[3]);

                    newDataRow[1] = usedSpace;
                    newDataRow[2] = Convert.ToInt32(serverDriveRows[4].ItemArray[3]) - usedSpace;

                    resultsDataTable.Rows.Add(newDataRow);
                }
            }
            catch (Exception ex)
            {
            }
            
            DateTime lastAuditDate = new AssetDAO().GetLastAuditDate(assetID);

            if (lastAuditDate != DateTime.MinValue)
                lblAuditDate.Text = "* Data correct as of " + lastAuditDate.ToLongDateString() + " at " + lastAuditDate.ToShortTimeString();
            else
                lblAuditDate.Text = String.Empty;

            return resultsDataTable;
        }

        public override void RefreshWidget()
        {
            string selectedItemText = cbServers.Text;
            //int selectedIndex = cbServers.SelectedIndex;

            DataTable serverDataTable = _statisticsDAO.GetServers();
            cbServers.DataSource = serverDataTable;
            cbServers.DisplayMember = "_NAME";
            cbServers.ValueMember = "_ASSETID";

            if (cbServers.Items.Count > 0)
            {
                cbServers.SelectedIndex = (cbServers.FindString(selectedItemText) == -1) ? 0 : cbServers.FindString(selectedItemText);

                if (cbServers.SelectedValue != null)
                    RefreshServerChart();
            }
            else
            {
                cbServers.DataSource = null;
                lStackColumnChart.DataSource = null;
                lblAuditDate.Text = String.Empty;
            }
        }

        private void cbServers_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (cbServers.SelectedValue != null && lStackColumnChart != null)			// 8.4.2 CMD Correct Startup Bug
            {
                RefreshServerChart();
            }
        }

        private void RefreshServerChart()
        {
            lStackColumnChart.DataSource = PopulateChartData((int)cbServers.SelectedValue);
            lStackColumnChart.DataBind();
        }

        private UltraChart CreateNewStackColumnChart()
        {
            UltraChart lStackColumnChart = new UltraChart();

            try
            {
                lStackColumnChart.Dock = DockStyle.Fill;
                lStackColumnChart.ChartType = ChartType.StackColumnChart;

                lStackColumnChart.ColumnChart.SeriesSpacing = 1;

                Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance chartTextAppearance = new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();
                Infragistics.UltraChart.Resources.Appearance.StackAppearance stackAppearance = new Infragistics.UltraChart.Resources.Appearance.StackAppearance();

                chartTextAppearance.ChartTextFont = new Font("Verdana", 8F);
                chartTextAppearance.Column = 0;
                chartTextAppearance.ItemFormatString = "<DATA_VALUE:00.00>";
                chartTextAppearance.Row = -2;
                chartTextAppearance.VerticalAlign = StringAlignment.Center;
                chartTextAppearance.HorizontalAlign = StringAlignment.Near;
                lStackColumnChart.BarChart.ChartText.Add(chartTextAppearance);

                lStackColumnChart.Axis.BackColor = Color.White;

                lStackColumnChart.Axis.X.Extent = 35;
                lStackColumnChart.Axis.Y.Extent = 20;
                lStackColumnChart.Axis.Y.Margin.Far.Value = 2;

                lStackColumnChart.Axis.Y.Labels.Font = new Font("Verdana", 8F);
                lStackColumnChart.Axis.Y.Labels.FontColor = Color.DimGray;
                lStackColumnChart.Axis.Y.Labels.HorizontalAlign = StringAlignment.Far;
                lStackColumnChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
                lStackColumnChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                lStackColumnChart.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 7F);
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.DimGray;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
                lStackColumnChart.Axis.Y.Labels.VerticalAlign = StringAlignment.Center;
                lStackColumnChart.Axis.Y.LineThickness = 1;
                lStackColumnChart.Axis.Y.MajorGridLines.AlphaLevel = 255;
                lStackColumnChart.Axis.Y.MajorGridLines.Color = Color.Gainsboro;
                lStackColumnChart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Solid;
                lStackColumnChart.Axis.Y.MajorGridLines.Visible = true;
                lStackColumnChart.Axis.Y.MinorGridLines.AlphaLevel = 200;
                lStackColumnChart.Axis.Y.MinorGridLines.Color = Color.LightGray;
                lStackColumnChart.Axis.Y.MinorGridLines.DrawStyle = LineDrawStyle.Dot;
                lStackColumnChart.Axis.Y.MinorGridLines.Visible = true;
                lStackColumnChart.Axis.Y.TickmarkInterval = 40;
                lStackColumnChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
                lStackColumnChart.Axis.Y.Visible = true;

                lStackColumnChart.Axis.X.Labels.ItemFormatString = String.Empty;

                lStackColumnChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8F);
                lStackColumnChart.Axis.X.Labels.SeriesLabels.FontColor = Color.DimGray;

                lStackColumnChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                lStackColumnChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

                // title
                lStackColumnChart.TitleBottom.Visible = false;
                lStackColumnChart.TitleTop.Font = new Font("Verdana", 10F, FontStyle.Regular);
                lStackColumnChart.TitleTop.HorizontalAlign = StringAlignment.Center;
                lStackColumnChart.TitleTop.Visible = false;

                lStackColumnChart.Tooltips.Font = new Font("Verdana", 8F);
                lStackColumnChart.Tooltips.FormatString = " Drive <SERIES_LABEL> <ITEM_LABEL>: <DATA_VALUE> GB ";
                lStackColumnChart.Tooltips.HighlightFillColor = Color.LightBlue;
                lStackColumnChart.Tooltips.HighlightOutlineColor = Color.LightBlue;

                // colours
                lStackColumnChart.BackgroundImageLayout = ImageLayout.Center;
                lStackColumnChart.Border.Color = Color.FromArgb(170, 173, 187);
                lStackColumnChart.ColorModel.Scaling = ColorScaling.Decreasing;

                lStackColumnChart.ColorModel.AlphaLevel = 200;
                lStackColumnChart.ColorModel.ColorBegin = Color.FromArgb(151, 189, 100);
                //lStackColumnChart.ColorModel.ColorEnd = System.Drawing.Color.Red;
                lStackColumnChart.ColorModel.ModelStyle = ColorModels.Office2007Style;
                //lColumnChart.ColorModel.Scaling = ColorScaling.None;

                lStackColumnChart.Tooltips.Overflow = TooltipOverflow.ChartArea;

                Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();

                gradientEffect1.Coloring = GradientColoringStyle.Lighten;
                gradientEffect1.Style = GradientStyle.BackwardDiagonal;
                lStackColumnChart.Effects.Effects.Add(gradientEffect1);

                lStackColumnChart.Data.UseRowLabelsColumn = false;
                lStackColumnChart.EmptyChartText = "";
                lStackColumnChart.InvalidDataReceived += lStackColumnChart_InvalidDataReceived;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }

            return lStackColumnChart;
        }

        void lStackColumnChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            Infragistics.UltraChart.Shared.Styles.LabelStyle ls = new Infragistics.UltraChart.Shared.Styles.LabelStyle();
            ls.FontColor = Color.DarkGray;

            Font font = new Font("Verdana", 12.75F, FontStyle.Italic);

            ls.Font = font;
            ls.VerticalAlign = StringAlignment.Center;
            ls.HorizontalAlign = StringAlignment.Center;

            e.LabelStyle = ls;
            e.Text = "No results were returned for this item.";
        }

        private void ServerDataWidget_VisibleChanged(object sender, EventArgs e)
        {
            RefreshWidget();
        }
    }
}

