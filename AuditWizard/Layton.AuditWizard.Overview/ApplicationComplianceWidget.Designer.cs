namespace Layton.AuditWizard.Overview
{
    partial class ApplicationComplianceWidget
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lbChartTitle = new System.Windows.Forms.Label();
			this.cbDashboardReportType1 = new System.Windows.Forms.ComboBox();
			this.ultraChart1 = new Infragistics.Win.UltraWinChart.UltraChart();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ultraChart1)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.Color.NavajoWhite;
			this.splitContainer1.Panel1.Controls.Add(this.lbChartTitle);
			this.splitContainer1.Panel1.Controls.Add(this.cbDashboardReportType1);
			this.splitContainer1.Panel1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.splitContainer1.Panel1MinSize = 19;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.ultraChart1);
			this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.splitContainer1.Size = new System.Drawing.Size(526, 296);
			this.splitContainer1.SplitterDistance = 20;
			this.splitContainer1.TabIndex = 0;
			// 
			// lbChartTitle
			// 
			this.lbChartTitle.AutoSize = true;
			this.lbChartTitle.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbChartTitle.Location = new System.Drawing.Point(4, 4);
			this.lbChartTitle.Name = "lbChartTitle";
			this.lbChartTitle.Size = new System.Drawing.Size(195, 13);
			this.lbChartTitle.TabIndex = 1;
			this.lbChartTitle.Text = "Overall Software Compliance";
			// 
			// cbDashboardReportType1
			// 
			this.cbDashboardReportType1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.cbDashboardReportType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDashboardReportType1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbDashboardReportType1.FormattingEnabled = true;
			this.cbDashboardReportType1.ItemHeight = 13;
			this.cbDashboardReportType1.Items.AddRange(new object[] {
            "Overall Software Compliance",
            "Over-Licensed Applications",
            "Under-Licensed Applications"});
			this.cbDashboardReportType1.Location = new System.Drawing.Point(315, 0);
			this.cbDashboardReportType1.Name = "cbDashboardReportType1";
			this.cbDashboardReportType1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cbDashboardReportType1.Size = new System.Drawing.Size(208, 21);
			this.cbDashboardReportType1.TabIndex = 0;
			this.cbDashboardReportType1.SelectedIndexChanged += new System.EventHandler(this.cbDashboardReportType_SelectedIndexChanged);
			// 
			// ultraChart1
			// 
			this.ultraChart1.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
			this.ultraChart1.Axis.X.Extent = 35;
			this.ultraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Calibri", 8.25F);
			this.ultraChart1.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
			this.ultraChart1.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
			this.ultraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X.LineThickness = 1;
			this.ultraChart1.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.X.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.X.Margin.Near.Value = 0.74626865671641784D;
			this.ultraChart1.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.X.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.X.Visible = true;
			this.ultraChart1.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
			this.ultraChart1.Axis.X2.Labels.ItemFormatString = "<ITEM_LABEL>";
			this.ultraChart1.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.X2.Labels.Visible = false;
			this.ultraChart1.Axis.X2.LineThickness = 1;
			this.ultraChart1.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.X2.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.X2.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.X2.Visible = false;
			this.ultraChart1.Axis.Y.Extent = 29;
			this.ultraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Calibri", 8.25F);
			this.ultraChart1.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
			this.ultraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
			this.ultraChart1.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.FormatString = "";
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
			this.ultraChart1.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Y.LineThickness = 1;
			this.ultraChart1.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Y.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.Y.Margin.Far.Value = 2D;
			this.ultraChart1.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Y.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.Y.TickmarkInterval = 40D;
			this.ultraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.Y.Visible = true;
			this.ultraChart1.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
			this.ultraChart1.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.FormatString = "";
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
			this.ultraChart1.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Y2.Labels.Visible = false;
			this.ultraChart1.Axis.Y2.LineThickness = 1;
			this.ultraChart1.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Y2.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Y2.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.Y2.TickmarkInterval = 40D;
			this.ultraChart1.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.Y2.Visible = false;
			this.ultraChart1.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Z.Labels.ItemFormatString = "<ITEM_LABEL>";
			this.ultraChart1.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Z.Labels.Visible = false;
			this.ultraChart1.Axis.Z.LineThickness = 1;
			this.ultraChart1.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Z.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Z.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.Z.Visible = false;
			this.ultraChart1.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Z2.Labels.ItemFormatString = "<ITEM_LABEL>";
			this.ultraChart1.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
			this.ultraChart1.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
			this.ultraChart1.Axis.Z2.Labels.Visible = false;
			this.ultraChart1.Axis.Z2.LineThickness = 1;
			this.ultraChart1.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
			this.ultraChart1.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Z2.MajorGridLines.Visible = true;
			this.ultraChart1.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
			this.ultraChart1.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
			this.ultraChart1.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
			this.ultraChart1.Axis.Z2.MinorGridLines.Visible = false;
			this.ultraChart1.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
			this.ultraChart1.Axis.Z2.Visible = false;
			this.ultraChart1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ultraChart1.ColorModel.AlphaLevel = ((byte)(100));
			this.ultraChart1.ColorModel.ColorBegin = System.Drawing.Color.Pink;
			this.ultraChart1.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
			this.ultraChart1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
			this.ultraChart1.ColorModel.Scaling = Infragistics.UltraChart.Shared.Styles.ColorScaling.Increasing;
			this.ultraChart1.Dock = System.Windows.Forms.DockStyle.Fill;
			gradientEffect1.Coloring = Infragistics.UltraChart.Shared.Styles.GradientColoringStyle.Lighten;
			gradientEffect1.Style = Infragistics.UltraChart.Shared.Styles.GradientStyle.BackwardDiagonal;
			this.ultraChart1.Effects.Effects.Add(gradientEffect1);
			this.ultraChart1.Location = new System.Drawing.Point(0, 0);
			this.ultraChart1.Name = "ultraChart1";
			this.ultraChart1.Size = new System.Drawing.Size(526, 272);
			this.ultraChart1.TabIndex = 2;
			this.ultraChart1.TitleBottom.Visible = false;
			this.ultraChart1.TitleTop.Visible = false;
			this.ultraChart1.Tooltips.Font = new System.Drawing.Font("Calibri", 9F);
			this.ultraChart1.Tooltips.HighlightFillColor = System.Drawing.Color.LightBlue;
			this.ultraChart1.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
			this.ultraChart1.Tooltips.TooltipControl = null;
			// 
			// ApplicationComplianceWidget
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ApplicationComplianceWidget";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(526, 296);
			this.VisibleChanged += new System.EventHandler(this.ApplicationComplianceWidget_VisibleChanged);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ultraChart1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbDashboardReportType1;
        private System.Windows.Forms.Label lbChartTitle;
        private Infragistics.Win.UltraWinChart.UltraChart ultraChart1;
    }
}