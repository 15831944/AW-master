using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;
//
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Practices.CompositeUI;
using Layton.Cab.Interface;

namespace Layton.AuditWizard.Overview
{
    public partial class ApplicationComplianceWidget : DefaultWidget
    {
        public new static string _widgetName = "ApplicationComplianceWidget";
        private string _selectedReport;
        private LaytonWorkItem _workItem;

        public override string WidgetName()
        {
            return _widgetName;
        }

        public override void RefreshWidget()
        {
            // JML_LINDE
            RefreshComplianceChart();
        }

        public ApplicationComplianceWidget([ServiceDependency] WorkItem workItem)
        {
            _workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            cbDashboardReportType1.SelectedIndex = 0;
            _selectedReport = cbDashboardReportType1.SelectedItem.ToString();

            this.ContextMenu = new ContextMenu();
		}

        void lChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
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

        private UltraChart CreateNewColumnChart(UltraChart lColumnChart)
        {
            try
            {
                lColumnChart.Dock = DockStyle.Fill;
                lColumnChart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart;

                lColumnChart.ColumnChart.SeriesSpacing = 0;
                lColumnChart.ColumnChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.InterpolateSimple;

                Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance chartTextAppearance = new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();

                chartTextAppearance.ChartTextFont = new System.Drawing.Font("Verdana", 8F);
                chartTextAppearance.Column = 0;
                chartTextAppearance.ItemFormatString = "<DATA_VALUE:00.00>";
                chartTextAppearance.Row = -2;
                chartTextAppearance.VerticalAlign = StringAlignment.Center;
                chartTextAppearance.HorizontalAlign = StringAlignment.Near;
                lColumnChart.BarChart.ChartText.Add(chartTextAppearance);

                lColumnChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8F);
                lColumnChart.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
                lColumnChart.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                lColumnChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
                lColumnChart.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
                lColumnChart.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                lColumnChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8F);
                lColumnChart.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
                lColumnChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Center;
                lColumnChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
                lColumnChart.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
                lColumnChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
                lColumnChart.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
                lColumnChart.Axis.Y.LineThickness = 1;
                lColumnChart.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
                lColumnChart.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
                lColumnChart.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Solid;
                lColumnChart.Axis.Y.MajorGridLines.Visible = true;
                lColumnChart.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(200));
                lColumnChart.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
                lColumnChart.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
                lColumnChart.Axis.Y.MinorGridLines.Visible = true;
                lColumnChart.Axis.Y.TickmarkInterval = 40;
                lColumnChart.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                lColumnChart.Axis.Y.Visible = true;
                lColumnChart.Axis.X.Margin.Near.Value = 0;
                lColumnChart.Axis.Y.Margin.Far.Value = 2;

                lColumnChart.Axis.X.Labels.ItemFormatString = String.Empty;

                lColumnChart.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
                lColumnChart.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;

                // title
                lColumnChart.TitleBottom.Visible = false;
                lColumnChart.TitleTop.Font = new System.Drawing.Font("Verdana", 12.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Regular))));
                lColumnChart.TitleTop.HorizontalAlign = System.Drawing.StringAlignment.Center;
                lColumnChart.TitleTop.Visible = false;

                // colours
                lColumnChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                lColumnChart.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(173)))), ((int)(((byte)(187)))));

                lColumnChart.ColorModel.AlphaLevel = ((byte)(200));
                lColumnChart.ColorModel.ColorBegin = System.Drawing.Color.Red;
                lColumnChart.ColorModel.ColorEnd = System.Drawing.Color.Red;
                lColumnChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
                //lColumnChart.ColorModel.Scaling = ColorScaling.None;

                Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            
                gradientEffect1.Coloring = Infragistics.UltraChart.Shared.Styles.GradientColoringStyle.Lighten;
                gradientEffect1.Style = Infragistics.UltraChart.Shared.Styles.GradientStyle.BackwardDiagonal;
                lColumnChart.Effects.Effects.Add(gradientEffect1);

                lColumnChart.Data.UseRowLabelsColumn = false;

                // tooltips
                lColumnChart.Tooltips.Font = new System.Drawing.Font("Verdana", 8F);
                lColumnChart.Tooltips.FormatString = " <SERIES_LABEL> (<DATA_VALUE>) ";
                lColumnChart.Tooltips.HighlightFillColor = System.Drawing.Color.LightBlue;
                lColumnChart.Tooltips.HighlightOutlineColor = System.Drawing.Color.LightBlue;

                lColumnChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8F);
                lColumnChart.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;

                lColumnChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(lChart_InvalidDataReceived);
                lColumnChart.ChartDrawItem += new Infragistics.UltraChart.Shared.Events.ChartDrawItemEventHandler(lColumnChart_ChartDrawItem);
                lColumnChart.ChartDataClicked += new Infragistics.UltraChart.Shared.Events.ChartDataClickedEventHandler(this.complianceChart_ChartDataClicked);

                DataTable reportData = new StatisticsDAO().StatisticsComplianceByType("", "");

                lColumnChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;

                double yMax = 0;

                foreach (DataRow row in reportData.Rows)
                {
                    double value = Math.Abs(Convert.ToDouble(row.ItemArray[1]));

                    if (value > yMax)
                        yMax = value;
                }

                lColumnChart.Axis.Y.RangeMax = yMax;
                lColumnChart.Axis.Y.RangeMin = 0;
                lColumnChart.DataSource = reportData;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }

            return lColumnChart;
        }

        private UltraChart CreateNewStackColumnChart(string reportType)
        {
            UltraChart lStackColumnChart = new UltraChart();

            try
            {
                lStackColumnChart.Dock = DockStyle.Fill;
                lStackColumnChart.ChartType = ChartType.StackColumnChart;
                lStackColumnChart.Tooltips.Overflow = TooltipOverflow.ChartArea;

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

                lStackColumnChart.Axis.Y.Labels.Font = new Font("Verdana", 8F);
                lStackColumnChart.Axis.Y.Labels.FontColor = Color.DimGray;
                lStackColumnChart.Axis.Y.Labels.HorizontalAlign = StringAlignment.Far;
                lStackColumnChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
                lStackColumnChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                lStackColumnChart.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
                lStackColumnChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8F);
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

                //lStackColumnChart.Axis.X.Margin.Near.Value = 0.74626865671641784;
                lStackColumnChart.Axis.Y.Margin.Far.Value = 2;

                lStackColumnChart.Axis.X.Extent = 13;
                lStackColumnChart.Axis.Y.Extent = 30;

                lStackColumnChart.Axis.X.Labels.ItemFormatString = String.Empty;

                lStackColumnChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8F);
                lStackColumnChart.Axis.X.Labels.SeriesLabels.FontColor = Color.DimGray;

                lStackColumnChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                lStackColumnChart.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

                // title
                lStackColumnChart.TitleBottom.Visible = false;
                lStackColumnChart.TitleTop.Font = new Font("Verdana", 12.75F, FontStyle.Regular);
                lStackColumnChart.TitleTop.HorizontalAlign = StringAlignment.Center;
                lStackColumnChart.TitleTop.Visible = false;

                lStackColumnChart.Tooltips.Font = new Font("Calibri", 8.25F);
                lStackColumnChart.Tooltips.FormatString = " <SERIES_LABEL>, <ITEM_LABEL>: <DATA_VALUE> ";
                lStackColumnChart.Tooltips.HighlightFillColor = Color.LightBlue;
                lStackColumnChart.Tooltips.HighlightOutlineColor = Color.LightBlue;

                // colours
                lStackColumnChart.BackgroundImageLayout = ImageLayout.Center;
                lStackColumnChart.Border.Color = Color.FromArgb(170, 173, 187);
                //lStackColumnChart.ColorModel.AlphaLevel = ((byte)(255));

                lStackColumnChart.ColorModel.AlphaLevel = 200;
                lStackColumnChart.ColorModel.ColorBegin = Color.Red;
                lStackColumnChart.ColorModel.ColorEnd = Color.Red;
                lStackColumnChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
                //lColumnChart.ColorModel.Scaling = ColorScaling.None;

                Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();

                gradientEffect1.Coloring = GradientColoringStyle.Lighten;
                gradientEffect1.Style = GradientStyle.BackwardDiagonal;
                lStackColumnChart.Effects.Effects.Add(gradientEffect1);

                // use different colour depending on report type
                DataTable reportData;

                if (reportType == "Under-Licensed Applications")
                {
                    reportData = new StatisticsDAO().StatisticsUnderLicensedApplicationsForDashboard();
                    lStackColumnChart.ColorModel.ColorBegin = Color.FromArgb(211, 78, 78);
                }
                else
                {
                    reportData = new StatisticsDAO().StatisticsOverLicensedApplicationsForDashboard();
                    lStackColumnChart.ColorModel.ColorBegin = Color.FromArgb(151, 189, 100);
                }

                lStackColumnChart.ColorModel.ModelStyle = ColorModels.Office2007Style;
                lStackColumnChart.ColorModel.Scaling = ColorScaling.Decreasing;

                lStackColumnChart.Data.UseRowLabelsColumn = false;
                lStackColumnChart.EmptyChartText = "";
                lStackColumnChart.InvalidDataReceived += lChart_InvalidDataReceived;
                lStackColumnChart.ChartDataClicked += licenseChart_ChartDataClicked;

                lStackColumnChart.DataSource = reportData;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }

            return lStackColumnChart;
        }

        void lColumnChart_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            if ((e.Primitive.GetType() == typeof(Infragistics.UltraChart.Core.Primitives.Box)) && e.HasData)
            {
                switch (e.Primitive.Series.Label)
                {
                    case "Non-Compliant":
                        e.Primitive.PE.Fill = Color.FromArgb(211, 78, 78);
                        e.Primitive.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                        break;

                    case "Not Defined":
                        e.Primitive.PE.Fill = Color.FromArgb(211, 168, 78);
                        e.Primitive.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                        break;

                    case "Ignored":
                        e.Primitive.PE.Fill = Color.DarkGray;
                        e.Primitive.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                        break;

                    default:
                        e.Primitive.PE.Fill = Color.FromArgb(151, 189, 100);
                        e.Primitive.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                        break;

                }
            }
        }

        private void cbDashboardReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshComplianceChart();
        }

        private void RefreshComplianceChart()
        {
            UltraChart ultraChart2 = null;

            if (cbDashboardReportType1.SelectedIndex == 0)
            {
                ultraChart2 = CreateNewColumnChart(ultraChart1);
                lbChartTitle.Text = "Overall Software Compliance";
            }
            else if (cbDashboardReportType1.SelectedIndex == 1)
            {
                ultraChart2 = CreateNewStackColumnChart("Over-Licensed Applications");
                lbChartTitle.Text = "Over-Licensed Applications";
            }
            else if (cbDashboardReportType1.SelectedIndex == 2)
            {
                ultraChart2 = CreateNewStackColumnChart("Under-Licensed Applications");
                lbChartTitle.Text = "Under-Licensed Applications";
            }

            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(ultraChart2);

            _selectedReport = cbDashboardReportType1.SelectedItem.ToString();
        }

        private void ApplicationComplianceWidget_VisibleChanged(object sender, EventArgs e)
        {
            RefreshWidget();
        }

        private void complianceChart_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {
            ((OverviewWorkItemController)_workItem.Controller).DisplayDrilldownData("Compliance : " + e.RowLabel);
        }

        private void licenseChart_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {
            ((OverviewWorkItemController)_workItem.Controller).DisplayDrilldownData("Installations for : " + e.RowLabel);
        }
    }
}
