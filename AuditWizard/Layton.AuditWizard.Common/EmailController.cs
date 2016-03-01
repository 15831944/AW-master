using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Windows.Forms;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;
using Layton.HtmlReports;

namespace Layton.AuditWizard.Common
{
    public class EmailController
    {
        private System.Timers.Timer dailyTimer = new System.Timers.Timer();

        public void ConfigureTimer()
        {
            DateTime nextEmail = DateTime.Now.Date;

            // set the report to get sent at 9am - if we are past this then the email will not go out 
            // until the next day.
            if (DateTime.Now.Hour >= 9)
                nextEmail = nextEmail.AddDays(1);

            // Ensure that hours are set to 9AM dead
            nextEmail = nextEmail.AddHours(9);

            // configure the email timer
            dailyTimer.Elapsed += emailTimer_Elapsed;
            dailyTimer.AutoReset = false;

            TimeSpan ts = nextEmail.Subtract(DateTime.Now);

            // Bug #329 - adding extra 20 secs as the interval was always slightly early causing two emails to be sent
            dailyTimer.Interval = ts.TotalMilliseconds + 20000;

            LogFile ourLog = LogFile.Instance;
            ourLog.Write("Email timer configured to fire at " + nextEmail + " in " + ts.TotalMinutes + " minutes time", true);
        }

        public void Start()
        {
            // set the daily timer to go off everyday
            ConfigureTimer();
            dailyTimer.Start();
        }

        public void Stop()
        {
            dailyTimer.Stop();
        }

        public bool SendSQLReportByEmail(string aReportName, int aSqlReportId, string aAssetIds)
        {
            LogFile ourLog = LogFile.Instance;

            UltraGrid lReportGrid = CreateNewSummaryGrid();
            lReportGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

            // get the report conditions list for this id
            DataTable lReportDataTable = new ReportsDAO().GetReportById(aSqlReportId);

            if (lReportDataTable.Rows.Count == 0)
                return false;

            string lReportData = lReportDataTable.Rows[0][2].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));
            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            lReportGrid.DataSource = new StatisticsDAO().PerformQuery(lReportDataList[0]);
            lReportGrid.BindingContext = new BindingContext();
            lReportGrid.DataBind();

            //string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Replace(" | ", "_") + ".pdf";
            string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Substring(26) + ".pdf";
            List<string> fileNames = new List<string>();

            MemoryStream stream = new MemoryStream();
            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.AutoSize = Infragistics.Win.UltraWinGrid.DocumentExport.AutoSize.None;
            exporter.TargetPaperOrientation = PageOrientation.Landscape;
            exporter.TargetPaperSize = new Infragistics.Documents.Reports.Report.PageSize(842F, 595F);

            Style lHeaderStyle = new Style(new Font("Calibri", 12), Brushes.Black);
            Report report = new Report();
            ISection section = report.AddSection();
            section.PageMargins.All = 0;
            section.PageAlignment = ContentAlignment.Center;

            section.PageOrientation = PageOrientation.Landscape;
            int lSectionWidth = 820;

            section.PageNumbering.Template = "Page [Page #] of [TotalPages]";
            section.PageNumbering.SkipFirst = true;
            section.PageNumbering.Alignment = new PageNumberAlignment(Alignment.Right, Alignment.Bottom);
            section.PageNumbering.OffsetY = -10;

            // Create a style which includes the Font and Brush. 
            Style pageNumberStyle = new Style(new Font("Calibri", 9.25f), Brushes.DarkGray);

            // Assign the style to the PageNumbering object. 
            section.PageNumbering.Style = pageNumberStyle;
            section.PageNumbering.Format = PageNumberFormat.Decimal;

            ISectionHeader s1header = section.AddHeader();
            s1header.Height = 30;
            s1header.Repeat = true;

            ISectionFooter s1footer = section.AddFooter();
            s1footer.Height = 20;
            s1footer.Repeat = false;

            IText s1headertitle = s1header.AddText(0, 0);
            s1headertitle.AddContent(aReportName.Substring(26), lHeaderStyle);
            s1headertitle.Alignment = TextAlignment.Center;
            IText s1headertimestamp = s1footer.AddText(0, 0);
            s1headertimestamp.AddContent("Report generated: ", pageNumberStyle);
            s1headertimestamp.AddContent(DateTime.Now.ToString(), pageNumberStyle);

            s1headertimestamp.Alignment = TextAlignment.Right;

            // compliant grid
            IText text1 = section.AddText();
            text1.Alignment = TextAlignment.Center;

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                col.Width = lSectionWidth / lReportGrid.DisplayLayout.Bands[0].Columns.Count;
                col.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            }

            exporter.Export(lReportGrid, section);

            report.Publish(tempFileName, FileFormat.PDF);
            fileNames.Add(tempFileName);

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.SendEmail(
                "AuditWizard Scheduled Report - " + aReportName,
                "AuditWizard has generated the following report in Adobe PDF format." + Environment.NewLine + Environment.NewLine +
                aReportName + Environment.NewLine + Environment.NewLine + "Please open the attachment to view the report.",
                fileNames);

            try
            {
                File.Delete(tempFileName);
            }
            catch (Exception ex)
            {
                ourLog.Write(ex.Message, true);
            }

            NewsFeed.AddNewsItem(NewsFeed.Priority.Information, aReportName + " scheduled report has been processed.");

            return true;
        }

        public bool SendCustomReportByEmail(string aReportName, int aCustomReportId, string aAssetIds)
        {
            LogFile ourLog = LogFile.Instance;

            UltraGrid lReportGrid = CreateNewSummaryGrid();
            lReportGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

            // get the report conditions list for this id
            DataTable lReportDataTable = new ReportsDAO().GetReportById(aCustomReportId);

            if (lReportDataTable.Rows.Count == 0)
                return false;

            string lReportData = lReportDataTable.Rows[0][2].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));
            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            CustomReport cr = new CustomReport(true, false);
            lReportGrid.DataSource = cr.CreateDataTableForCustomReport(lReportDataList, aAssetIds);
            lReportGrid.BindingContext = new BindingContext();
            lReportGrid.DataBind();

            bool lDisplayAsAssetRegister = false;

            foreach (string lReportCondition in lReportDataList)
            {
                if (lReportCondition.StartsWith("ASSET_REGISTER:"))
                {
                    lDisplayAsAssetRegister = Convert.ToBoolean(lReportCondition.Substring(15));
                    break;
                }
            }

            if (lDisplayAsAssetRegister)
                lReportGrid.DisplayLayout.Bands[0].SortedColumns.Add(lReportGrid.DisplayLayout.Bands[0].Columns[0], false, true);

            //string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Replace(" | ", "_") + ".pdf";
            string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Substring(16) + ".pdf";
            List<string> fileNames = new List<string>();

            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.AutoSize = AutoSize.None;
            exporter.TargetPaperOrientation = PageOrientation.Landscape;
            exporter.TargetPaperSize = new PageSize(842F, 595F);

            Style lHeaderStyle = new Style(new Font("Calibri", 12), Brushes.Black);
            Report report = new Report();
            ISection section = report.AddSection();
            section.PageMargins.All = 0;
            section.PageAlignment = ContentAlignment.Center;

            section.PageOrientation = PageOrientation.Landscape;
            int lSectionWidth = 820;

            section.PageNumbering.Template = "Page [Page #] of [TotalPages]";
            section.PageNumbering.SkipFirst = true;
            section.PageNumbering.Alignment = new PageNumberAlignment(Alignment.Right, Alignment.Bottom);
            section.PageNumbering.OffsetY = -10;

            // Create a style which includes the Font and Brush. 
            Style pageNumberStyle = new Style(new Font("Calibri", 9.25f), Brushes.DarkGray);

            // Assign the style to the PageNumbering object. 
            section.PageNumbering.Style = pageNumberStyle;
            section.PageNumbering.Format = PageNumberFormat.Decimal;

            ISectionHeader s1header = section.AddHeader();
            s1header.Height = 30;
            s1header.Repeat = true;

            ISectionFooter s1footer = section.AddFooter();
            s1footer.Height = 20;
            s1footer.Repeat = false;

            IText s1headertitle = s1header.AddText(0, 0);
            s1headertitle.AddContent(aReportName.Substring(16), lHeaderStyle);
            s1headertitle.Alignment = TextAlignment.Center;
            IText s1headertimestamp = s1footer.AddText(0, 0);
            s1headertimestamp.AddContent("Report generated: ", pageNumberStyle);
            s1headertimestamp.AddContent(DateTime.Now.ToString(), pageNumberStyle);

            s1headertimestamp.Alignment = TextAlignment.Right;

            // compliant grid
            IText text1 = section.AddText();
            text1.Alignment = TextAlignment.Center;

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                col.Width = lSectionWidth / lReportGrid.DisplayLayout.Bands[0].Columns.Count;
                col.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            }

            exporter.Export(lReportGrid, section);

            report.Publish(tempFileName, FileFormat.PDF);
            fileNames.Add(tempFileName);

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.SendEmail(
                "AuditWizard Scheduled Report - " + aReportName,
                "AuditWizard has generated the following report in Adobe PDF format." + Environment.NewLine + Environment.NewLine +
                aReportName + Environment.NewLine + Environment.NewLine + "Please open the attachment to view the report.",
                fileNames);

            try
            {
                File.Delete(tempFileName);
            }
            catch (Exception ex)
            {
                ourLog.Write(ex.Message, true);
            }

            NewsFeed.AddNewsItem(NewsFeed.Priority.Information, aReportName + " scheduled report has been processed.");
            return true;
        }

        public bool SendComplianceReportByEmail(string aReportName, int aCustomReportId, string aAssetIds)
        {
            LogFile ourLog = LogFile.Instance;

            // get the report conditions list for this id
            DataTable lReportDataTable = new ReportsDAO().GetReportById(aCustomReportId);

            if (lReportDataTable.Rows.Count == 0)
                return false;

            string lReportData = lReportDataTable.Rows[0][2].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));
            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            string fileName = GenerateComplianceReportPDF(aReportName, lReportDataList, aAssetIds);

            List<string> fileNames = new List<string>();
            fileNames.Add(fileName);

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.SendEmail(
                "AuditWizard Scheduled Report - " + aReportName,
                "AuditWizard has generated the following report in Adobe PDF format." + Environment.NewLine + Environment.NewLine +
                aReportName + Environment.NewLine + Environment.NewLine + "Please open the attachment to view the report.",
                fileNames);

            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                ourLog.Write(ex.Message, true);
            }

            NewsFeed.AddNewsItem(NewsFeed.Priority.Information, aReportName + " scheduled report has been processed.");
            return true;
        }

        private string GenerateComplianceReportPDF(string aReportName, List<string> aReportDataList, string aAssetIds)
        {
            UltraChart lReportChart = CreateChart();
            UltraGrid lReportGrid = CreateNewSummaryGrid();
            lReportGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            lReportGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFixed;

            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.AutoSize = AutoSize.None;
            exporter.TargetPaperOrientation = PageOrientation.Landscape;
            exporter.TargetPaperSize = new PageSize(842F, 595F);

            MemoryStream stream = new MemoryStream();
            ComplianceReport cr = new ComplianceReport();
            //string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Replace(" | ", "_") + ".pdf";
            string tempFileName = Application.StartupPath + "\\reports\\" + aReportName.Substring(20) + ".pdf";

            string assetIds = cr.CreateDataTableForComplianceReport(aReportDataList, true, true, aAssetIds);

            // generate the compliance details
            lReportChart.DataSource = new StatisticsDAO().StatisticsComplianceOverview(assetIds);
            lReportChart.BindingContext = new BindingContext();
            lReportChart.DataBind();
            lReportChart.SaveTo(
                stream,
                System.Drawing.Imaging.ImageFormat.Jpeg,
                new System.Drawing.Size(750, 600),
                new System.Drawing.SizeF(100F, 100F));

            Style lHeaderStyle = new Style(new Font("Calibri", 12), Brushes.Black);
            Report report = new Report();
            ISection section = report.AddSection();
            section.PageMargins.All = 0;
            section.PageAlignment = ContentAlignment.Center;

            section.PageOrientation = PageOrientation.Landscape;
            //int lSectionWidth = (int)section.PageSize.Width;
            int lSectionWidth = 820;

            section.PageNumbering.Template = "Page [Page #] of [TotalPages]";
            section.PageNumbering.SkipFirst = true;
            section.PageNumbering.Alignment = new PageNumberAlignment(Alignment.Right, Alignment.Bottom);
            section.PageNumbering.OffsetY = -10;

            // Create a style which includes the Font and Brush. 
            Style pageNumberStyle = new Style(new Font("Calibri", 9.25f), Brushes.DarkGray);

            // Assign the style to the PageNumbering object. 
            section.PageNumbering.Style = pageNumberStyle;
            section.PageNumbering.Format = PageNumberFormat.Decimal;

            ISectionHeader s1header = section.AddHeader();
            s1header.Height = 30;
            s1header.Repeat = false;

            ISectionFooter s1footer = section.AddFooter();
            s1footer.Height = 20;
            s1footer.Repeat = false;

            IText s1headertitle = s1header.AddText(0, 0);
            s1headertitle.AddContent(aReportName.Substring(20), lHeaderStyle);
            s1headertitle.Alignment = TextAlignment.Center;

            IText s1headertimestamp = s1footer.AddText(0, 0);
            s1headertimestamp.AddContent("Report generated: ", pageNumberStyle);
            s1headertimestamp.AddContent(DateTime.Now.ToString(), pageNumberStyle);

            s1headertimestamp.Alignment = TextAlignment.Right;

            IText text = section.AddText();
            text.Alignment = TextAlignment.Center;
            section.PageOrientation = PageOrientation.Landscape;

            section.AddImage(new Image(stream));
            section.PageAlignment = ContentAlignment.Center;

            // compliant grid
            section.AddPageBreak();
            IText text1 = section.AddText();
            text1.Alignment = TextAlignment.Center;
            text1.AddContent("Compliant Assets", lHeaderStyle);
            text1.AddLineBreak();
            text1.AddLineBreak();

            lReportGrid.DataSource = cr.CreateComplianceGrid("Compliant", assetIds, aReportDataList, aAssetIds);
            lReportGrid.BindingContext = new BindingContext();
            lReportGrid.DataBind();

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                col.Width = lSectionWidth / lReportGrid.DisplayLayout.Bands[0].Columns.Count;
                col.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            }

            exporter.Export(lReportGrid, section);

            // non-compliant grid  
            section.AddPageBreak();
            IText text2 = section.AddText();
            text2.Alignment = TextAlignment.Center;
            text2.AddContent("Non-Compliant Assets", lHeaderStyle);
            text2.AddLineBreak();
            text2.AddLineBreak();

            lReportGrid.DataSource = cr.CreateComplianceGrid("Non-Compliant", assetIds, aReportDataList, aAssetIds);
            lReportGrid.BindingContext = new BindingContext();
            lReportGrid.DataBind();

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                col.Width = lSectionWidth / lReportGrid.DisplayLayout.Bands[0].Columns.Count;
                col.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            }


            exporter.Export(lReportGrid, section);

            report.Publish(tempFileName, FileFormat.PDF);

            return tempFileName;
        }

        public bool SendReportByEmail(string aReportName, string aAssetIds)
        {
            LogFile ourLog = LogFile.Instance;

            bool lNoChart = (
                aReportName.Equals("Software Report | Application Licensing") ||
                aReportName.StartsWith("Software Report | License Keys by Publisher") ||
                aReportName.StartsWith("Asset Management Report | Internet History") ||
                aReportName.StartsWith("Asset Management Report | File System") ||
                aReportName.StartsWith("Asset Management Report | Audit Trail History") ||
                aReportName.StartsWith("Software Report | Internet Browsers by Asset")
                );

            List<string> fileNames = new List<string>();
            MemoryStream stream = new MemoryStream();
            UltraGrid lReportGrid = CreateNewSummaryGrid();
            string lReportName = aReportName.Replace(@"/", "_");

            string tempFileName = Application.StartupPath + "\\reports\\" + lReportName.Replace(" | ", "_") + ".pdf";

            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.AutoSize = AutoSize.None;
            exporter.TargetPaperOrientation = PageOrientation.Landscape;
            exporter.TargetPaperSize = new PageSize(842F, 595F);

            Style lHeaderStyle = new Style(new Font("Calibri", 12), Brushes.Black);
            Report report = new Report();
            ISection section = report.AddSection();
            section.PageMargins.All = 0;

            section.PageOrientation = PageOrientation.Landscape;
            int lSectionWidth = 820;

            section.PageNumbering.Template = "Page [Page #] of [TotalPages]";
            section.PageNumbering.SkipFirst = true;
            section.PageNumbering.Alignment = new PageNumberAlignment(Alignment.Right, Alignment.Bottom);
            section.PageNumbering.OffsetY = -10;

            // Create a style which includes the Font and Brush. 
            Style pageNumberStyle = new Style(new Font("Calibri", 9.25f), Brushes.Black);

            // Assign the style to the PageNumbering object. 
            section.PageNumbering.Style = pageNumberStyle;
            section.PageNumbering.Format = PageNumberFormat.Decimal;

            ISectionHeader s1header = section.AddHeader();
            s1header.Height = 60;
            s1header.Repeat = false;

            ISectionFooter s1footer = section.AddFooter();
            s1footer.Height = 10;
            s1footer.Repeat = false;

            IText s1headertimestamp = s1footer.AddText(0, 0);
            s1headertimestamp.AddContent("Report generated: ", pageNumberStyle);
            s1headertimestamp.AddContent(DateTime.Now.ToString(), pageNumberStyle);

            s1headertimestamp.Alignment = TextAlignment.Right;

            if (!lNoChart)
            {
                IText s1headertitle = s1header.AddText(0, 0);
                s1headertitle.AddContent(aReportName, lHeaderStyle);
                s1headertitle.Alignment = TextAlignment.Center;

                if (aReportName.StartsWith("Software Report | Over/Under Licensed by Publisher"))
                {
                    UltraChart reportChart = CreateNewColumnChart();
                    DataTable reportData = GetChartData(aReportName, aAssetIds);
                    reportChart.DataSource = reportData;
                    reportChart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;

                    double yMax = 0;

                    foreach (DataRow row in reportData.Rows)
                    {
                        double value = Math.Abs(Convert.ToDouble(row.ItemArray[1]));

                        if (value > yMax)
                            yMax = value;
                    }

                    reportChart.Axis.Y.RangeMax = yMax;
                    reportChart.Axis.Y.RangeMin = yMax * -1;

                    reportChart.BindingContext = new BindingContext();
                    reportChart.DataBind();
                    reportChart.SaveTo(
                        stream,
                        System.Drawing.Imaging.ImageFormat.Jpeg,
                        new System.Drawing.Size(600, 600),
                        new System.Drawing.SizeF(100F, 100F));
                }
                else
                {
                    // create the chart
                    UltraChart reportChart = CreateChart();
                    reportChart.DataSource = GetChartData(aReportName, aAssetIds);
                    reportChart.BindingContext = new BindingContext();
                    reportChart.DataBind();
                    reportChart.SaveTo(
                        stream,
                        System.Drawing.Imaging.ImageFormat.Jpeg,
                        new System.Drawing.Size(600, 600),
                        new System.Drawing.SizeF(100F, 100F));
                }

                section.AddImage(new Image(stream));

                // compliant grid
                section.AddPageBreak();
            }

            section.PageAlignment = ContentAlignment.Center;

            IText text1 = section.AddText();
            text1.Alignment = TextAlignment.Center;
            text1.AddContent(aReportName, lHeaderStyle);
            text1.AddLineBreak();
            text1.AddLineBreak();
            text1.AddLineBreak();
            text1.AddLineBreak();

            lReportGrid.InitializeLayout += new InitializeLayoutEventHandler(lReportGrid_InitializeLayout);

            if (aReportName == "Software Report | Application Licensing")
            {
                ReportDefinition licensingReport = new LicensingReportDefinition();
                licensingReport.GenerateReport(lReportGrid);
            }
            else if (aReportName.StartsWith("Software Report | License Keys by Publisher"))
            {
                lReportGrid.DataSource = new ApplicationsDAO().GetLicenseKeysByPublisher(aReportName.Split('|')[2].Trim(), true, false);
            }
            else if (aReportName == "Asset Management Report | Internet History")
            {
                ReportDefinition internetReport = new InternetReportDefinition();
                internetReport.GenerateReport(lReportGrid);
            }
            else if (aReportName == "Asset Management Report | File System")
            {
                ReportDefinition fileReport = new FileSystemReportDefinition();
                fileReport.GenerateReport(lReportGrid);
            }
            else if (aReportName.StartsWith("Asset Management Report | Audit Trail History"))
            {
                lReportGrid.DataSource = new AuditTrailDAO().GetAuditTrailByDate("", "");
            }
            else if (aReportName.StartsWith("Software Report | Internet Browsers by Asset"))
            {
                lReportGrid.DataSource = new AuditedItemsDAO().GetInstalledBrowsersByAsset();
            }
            else
            {
                lReportGrid.DataSource = GetGridData(aReportName, aAssetIds);
            }

            lReportGrid.BindingContext = new BindingContext();
            lReportGrid.DataBind();

            int lVisibleColumnCount = 0;

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                if (!col.Hidden)
                    lVisibleColumnCount++;
            }

            foreach (UltraGridColumn col in lReportGrid.DisplayLayout.Bands[0].Columns)
            {
                if (!col.Hidden)
                {
                    col.Width = lSectionWidth / lVisibleColumnCount;
                    col.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
                    col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                }
            }

            exporter.Export(lReportGrid, section);

            report.Publish(tempFileName, FileFormat.PDF);
            fileNames.Add(tempFileName);

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.SendEmail(
                "AuditWizard Scheduled Report - " + aReportName,
                "AuditWizard has generated the following report in Adobe PDF format." + Environment.NewLine + Environment.NewLine +
                aReportName + Environment.NewLine + Environment.NewLine + "Please open the attachment to view the report.",
                fileNames);

            try
            {
                File.Delete(tempFileName);
            }
            catch (Exception ex)
            {
                ourLog.Write(ex.Message, true);
            }

            NewsFeed.AddNewsItem(NewsFeed.Priority.Information, aReportName + " scheduled report has been processed.");

            return true;
        }

        void lReportGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                foreach (UltraGridColumn column in band.Columns)
                {
                    switch (column.Key)
                    {
                        case "object":
                            column.Hidden = true;
                            break;
                        case "applicationid":
                            column.Hidden = true;
                            break;
                        case "instanceid":
                            column.Hidden = true;
                            break;
                        case "licenseid":
                            column.Hidden = true;
                            break;
                        default:
                            column.Hidden = false;
                            break;
                    }
                }
            }
        }

        private UltraGrid CreateNewSummaryGrid()
        {
            UltraGrid lResultsGrid = new UltraGrid();

            try
            {
                Infragistics.Win.Appearance lGridAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lRowAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lHeaderAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lAlternateRowAppearance = new Infragistics.Win.Appearance();

                ((System.ComponentModel.ISupportInitialize)(lResultsGrid)).BeginInit();
                lResultsGrid.SuspendLayout();

                lGridAppearance.BackColor = System.Drawing.Color.White;
                lResultsGrid.DisplayLayout.Appearance = lGridAppearance;

                lRowAppearance.BackColor = System.Drawing.Color.White;
                lRowAppearance.ForeColor = System.Drawing.Color.Black;
                lRowAppearance.FontData.Name = "Verdana";
                lRowAppearance.FontData.SizeInPoints = 8F;

                lResultsGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                lResultsGrid.Dock = DockStyle.Fill;

                lRowAppearance.BorderColor = System.Drawing.Color.LightGray;
                lResultsGrid.DisplayLayout.Override.RowAppearance = lRowAppearance;
                lResultsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
                lResultsGrid.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
                lResultsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                lResultsGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
                lResultsGrid.DisplayLayout.Override.RowAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

                lResultsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.Override.CellPadding = 3;

                lResultsGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
                lResultsGrid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.SingleSummaryBasedOnDataType;
                lResultsGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Single;

                lResultsGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.NotAllowed;
                lResultsGrid.DisplayLayout.Override.AllowColSwapping = AllowColSwapping.NotAllowed;
                lResultsGrid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
                lResultsGrid.DisplayLayout.Override.SelectTypeCol = SelectType.None;

                lHeaderAppearance.BackColor2 = System.Drawing.Color.White;
                lHeaderAppearance.BackColor = System.Drawing.Color.FromArgb(221, 236, 255);
                lHeaderAppearance.ForeColor = System.Drawing.Color.Black;
                lHeaderAppearance.FontData.Name = "Verdana";
                lHeaderAppearance.FontData.SizeInPoints = 8F;
                lHeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                lHeaderAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                lHeaderAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                lResultsGrid.DisplayLayout.Override.HeaderAppearance = lHeaderAppearance;

                lAlternateRowAppearance.BackColor = System.Drawing.Color.FromArgb(246, 252, 255);
                lResultsGrid.DisplayLayout.Override.RowAlternateAppearance = lAlternateRowAppearance;

                lResultsGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;

                ((System.ComponentModel.ISupportInitialize)(lResultsGrid)).EndInit();
                lResultsGrid.ResumeLayout();
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }

            return lResultsGrid;
        }

        private UltraChart CreateChart()
        {
            UltraChart inventoryChart = new UltraChart();
            Infragistics.UltraChart.Resources.Appearance.PaintElement paintElement1 = new Infragistics.UltraChart.Resources.Appearance.PaintElement();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect1 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            Infragistics.UltraChart.Resources.Appearance.PieChartAppearance pieChartAppearance1 = new Infragistics.UltraChart.Resources.Appearance.PieChartAppearance();

            inventoryChart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
            // 
            // ultraChart1
            // 
            inventoryChart.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            paintElement1.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.None;
            paintElement1.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            inventoryChart.Axis.PE = paintElement1;
            inventoryChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            inventoryChart.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.X.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.X.LineThickness = 1;
            inventoryChart.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.X.MajorGridLines.Visible = true;
            inventoryChart.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.X.MinorGridLines.Visible = false;
            inventoryChart.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.X.Visible = true;
            inventoryChart.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.X2.Labels.ItemFormatString = "";
            inventoryChart.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.X2.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.X2.Labels.Visible = false;
            inventoryChart.Axis.X2.LineThickness = 1;
            inventoryChart.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.X2.MajorGridLines.Visible = true;
            inventoryChart.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.X2.MinorGridLines.Visible = false;
            inventoryChart.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.X2.Visible = false;
            inventoryChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            inventoryChart.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.Y.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Y.LineThickness = 1;
            inventoryChart.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Y.MajorGridLines.Visible = true;
            inventoryChart.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Y.MinorGridLines.Visible = false;
            inventoryChart.Axis.Y.TickmarkInterval = 10;
            inventoryChart.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.Y.Visible = true;
            inventoryChart.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Y2.Labels.ItemFormatString = "";
            inventoryChart.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            inventoryChart.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Y2.Labels.Visible = false;
            inventoryChart.Axis.Y2.LineThickness = 1;
            inventoryChart.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Y2.MajorGridLines.Visible = true;
            inventoryChart.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Y2.MinorGridLines.Visible = false;
            inventoryChart.Axis.Y2.TickmarkInterval = 10;
            inventoryChart.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.Y2.Visible = false;
            inventoryChart.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Z.Labels.ItemFormatString = "";
            inventoryChart.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            inventoryChart.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Z.Labels.Visible = false;
            inventoryChart.Axis.Z.LineThickness = 1;
            inventoryChart.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Z.MajorGridLines.Visible = true;
            inventoryChart.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Z.MinorGridLines.Visible = false;
            inventoryChart.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.Z.Visible = false;
            inventoryChart.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Z2.Labels.ItemFormatString = "";
            inventoryChart.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            inventoryChart.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            inventoryChart.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            inventoryChart.Axis.Z2.Labels.Visible = false;
            inventoryChart.Axis.Z2.LineThickness = 1;
            inventoryChart.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            inventoryChart.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Z2.MajorGridLines.Visible = true;
            inventoryChart.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
            inventoryChart.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            inventoryChart.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            inventoryChart.Axis.Z2.MinorGridLines.Visible = false;
            inventoryChart.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            inventoryChart.Axis.Z2.Visible = false;
            inventoryChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            inventoryChart.Border.Thickness = 0;
            inventoryChart.ColorModel.AlphaLevel = ((byte)(100));
            inventoryChart.ColorModel.ColorBegin = System.Drawing.Color.Pink;
            inventoryChart.ColorModel.ColorEnd = System.Drawing.Color.DarkRed;
            inventoryChart.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
            inventoryChart.Dock = System.Windows.Forms.DockStyle.Fill;
            gradientEffect1.Coloring = Infragistics.UltraChart.Shared.Styles.GradientColoringStyle.Lighten;
            gradientEffect1.Style = Infragistics.UltraChart.Shared.Styles.GradientStyle.BackwardDiagonal;
            inventoryChart.Effects.Effects.Add(gradientEffect1);
            inventoryChart.EmptyChartText = "";
            inventoryChart.Location = new System.Drawing.Point(0, 0);
            inventoryChart.Name = "inventoryChart";
            pieChartAppearance1.Labels.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pieChartAppearance1.Labels.FontColor = System.Drawing.Color.DimGray;
            pieChartAppearance1.Labels.FormatString = "<ITEM_LABEL>  (<DATA_VALUE:#0>)";
            pieChartAppearance1.Labels.LeaderLineColor = System.Drawing.Color.DimGray;
            pieChartAppearance1.RadiusFactor = 85;
            inventoryChart.PieChart = pieChartAppearance1;
            inventoryChart.Size = new System.Drawing.Size(526, 249);
            inventoryChart.TabIndex = 2;
            inventoryChart.Tooltips.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            inventoryChart.Tooltips.FormatString = "<ITEM_LABEL>";
            inventoryChart.Tooltips.HighlightFillColor = System.Drawing.Color.LightBlue; ;
            inventoryChart.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
            inventoryChart.PieChart.OthersCategoryPercent = 0;

            return inventoryChart;
        }

        private DataTable GetGridData(string aReportName, string aAssetIds)
        {
            DataTable reportData = new DataTable();
            StatisticsDAO lStatisticsDAO = new StatisticsDAO();

            string lReportName = aReportName.Split('|')[1].Trim();
            string lPublisher = (aReportName.Split('|').Length == 3) ? aReportName.Split('|')[2].Trim() : "";

            if (lReportName.StartsWith("Application Compliance by Publisher"))
            {
                reportData = lStatisticsDAO.StatisticsComplianceByType(lPublisher, aAssetIds);
            }
            else if (lReportName.StartsWith("Over/Under Licensed by Publisher"))
            {
                reportData = lStatisticsDAO.StatisticsSoftwareComplianceByPublisher(lPublisher, aAssetIds);
            }
            else
            {
                // based on the selected report, return the relevant DataTable
                switch (lReportName)
                {
                    case "Asset status":
                        reportData = lStatisticsDAO.StatisticsAssetStatesAsGrid(aAssetIds);
                        break;

                    case "Asset by location":
                        reportData = lStatisticsDAO.StatisticsAssetLocationsAsGrid(aAssetIds);
                        break;

                    case "Audit age":
                        reportData = lStatisticsDAO.AuditHistoryStatistics(aAssetIds);
                        break;

                    case "Audit deployment status":
                        reportData = lStatisticsDAO.StatisticsAssetAgentVersionsAsGrid(aAssetIds);
                        break;

                    case "Discovery agent versions":
                        reportData = lStatisticsDAO.StatisticsAssetAgentVersionsAsGrid(aAssetIds);
                        break;

                    case "Operating systems":
                        reportData = lStatisticsDAO.StatisticsTopOSAsGrid(aAssetIds);
                        break;

                    case "Agent deployment status":
                        reportData = lStatisticsDAO.StatisticsAgentStatesAsGrid(aAssetIds);
                        break;

                    case "Top 15 Software Vendors":
                        reportData = lStatisticsDAO.StatisticsTopPublishersAsGrid(true, true, aAssetIds);
                        break;

                    case "Over-Licensed Applications":
                        reportData = lStatisticsDAO.StatisticsOverLicensedApplications(aAssetIds);
                        break;

                    case "Under-Licensed Applications":
                        reportData = lStatisticsDAO.StatisticsUnderLicensedApplications(aAssetIds);
                        break;

                    case "Top 15 Manufacturers":
                        reportData = lStatisticsDAO.StatisticsTopManufacturersAsGrid(aAssetIds);
                        break;

                    case "Microsoft Office Versions":
                        reportData = lStatisticsDAO.StatisticsMsOfficeAsGrid(1000, true, true, aAssetIds);
                        break;

                    case "Top 10 System Processors":
                        reportData = lStatisticsDAO.StatisticsTopAuditedItemAsGrid(10, "Hardware|CPU", "name", aAssetIds);
                        break;

                    case "System RAM Capacity":
                        reportData = lStatisticsDAO.StatisticsTopMemoryCapacityAsGrid(aAssetIds);
                        break;

                    case "Processor Speeds":
                        reportData = lStatisticsDAO.StatisticsTopProcessorSpeedsAsGrid(aAssetIds);
                        break;

                    case "Audited / Unaudited":
                        reportData = lStatisticsDAO.StatisticsAssetsAuditedAsGrid(aAssetIds);
                        break;

                    case "Assets By Type":
                        reportData = lStatisticsDAO.StatisticsAssetsByTypeAsGrid(aAssetIds);
                        break;

                    case "Support Expiry Date":
                        reportData = lStatisticsDAO.StatisticsSupportExpiryDateAsGrid(aAssetIds);
                        break;

                    case "Overall Application Compliance":
                        reportData = lStatisticsDAO.StatisticsComplianceByType("", aAssetIds);
                        break;

                    case "Default Internet Browsers":
                        reportData = lStatisticsDAO.StatisticsDefaultBrowser();
                        break;
                }
            }

            return reportData;
        }

        private DataTable GetChartData(string aReportName, string aAssetIds)
        {
            DataTable reportData = new DataTable();
            StatisticsDAO lStatisticsDAO = new StatisticsDAO();

            string lReportName = aReportName.Split('|')[1].Trim();
            string lPublisher = (aReportName.Split('|').Length == 3) ? aReportName.Split('|')[2].Trim() : "";

            if (lReportName.StartsWith("Application Compliance by Publisher"))
            {
                reportData = lStatisticsDAO.StatisticsComplianceByType(lPublisher, aAssetIds);
            }
            else if (lReportName.StartsWith("Over/Under Licensed by Publisher"))
            {
                reportData = lStatisticsDAO.StatisticsSoftwareComplianceByPublisher(lPublisher, aAssetIds);
            }
            else
            {
                // based on the selected report, return the relevant DataTable
                switch (lReportName)
                {
                    case "Asset status":
                        reportData = lStatisticsDAO.StatisticsAssetStates(aAssetIds);
                        break;

                    case "Asset by location":
                        reportData = lStatisticsDAO.StatisticsAssetLocations(aAssetIds);
                        break;

                    case "Audit age":
                        reportData = lStatisticsDAO.AuditHistoryStatistics(aAssetIds);
                        break;

                    case "Audit deployment status":
                        reportData = lStatisticsDAO.StatisticsAssetAgentVersions(aAssetIds);
                        break;

                    case "Discovery agent versions":
                        reportData = lStatisticsDAO.StatisticsAssetAgentVersions(aAssetIds);
                        break;

                    case "Audited/Unaudited assets":
                        reportData = lStatisticsDAO.StatisticsAuditedAssets();
                        break;

                    case "Operating systems":
                        reportData = lStatisticsDAO.StatisticsTopOS(aAssetIds);
                        break;

                    case "Agent deployment status":
                        reportData = lStatisticsDAO.StatisticsAgentStates(aAssetIds);
                        break;

                    case "Top 15 Software Vendors":
                        reportData = lStatisticsDAO.StatisticsTopPublishers(true, true, aAssetIds);
                        break;

                    case "Over-Licensed Applications":
                        reportData = lStatisticsDAO.StatisticsOverLicensedApplications(aAssetIds);
                        break;

                    case "Under-Licensed Applications":
                        reportData = lStatisticsDAO.StatisticsUnderLicensedApplications(aAssetIds);
                        break;

                    case "Top 15 Manufacturers":
                        reportData = lStatisticsDAO.StatisticsTopManufacturers(aAssetIds);
                        break;

                    case "Microsoft Office Versions":
                        reportData = lStatisticsDAO.StatisticsMsOffice(1000, true, true, aAssetIds);
                        break;

                    case "Top 10 System Processors":
                        reportData = lStatisticsDAO.StatisticsTopAuditedItem(10, "Hardware|CPU", "name", aAssetIds);
                        break;

                    case "System RAM Capacity":
                        reportData = lStatisticsDAO.StatisticsTopMemoryCapacity(aAssetIds);
                        break;

                    case "Processor Speeds":
                        reportData = lStatisticsDAO.StatisticsTopProcessorSpeeds(aAssetIds);
                        break;

                    case "Audited / Unaudited":
                        reportData = lStatisticsDAO.StatisticsAssetsAudited(aAssetIds);
                        break;

                    case "Assets By Type":
                        reportData = lStatisticsDAO.StatisticsAssetsByType(aAssetIds);
                        break;

                    case "Support Expiry Date":
                        reportData = lStatisticsDAO.StatisticsSupportExpiryDate(aAssetIds);
                        break;

                    case "Overall Application Compliance":
                        reportData = lStatisticsDAO.StatisticsComplianceByType("", aAssetIds);
                        break;

                    case "Default Internet Browsers":
                        reportData = lStatisticsDAO.StatisticsDefaultBrowser();
                        break;
                }
            }

            return reportData;
        }

        private UltraChart CreateNewColumnChart()
        {
            UltraChart lColumnChart = new UltraChart();

            try
            {
                lColumnChart.Dock = DockStyle.Fill;
                lColumnChart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart;

                lColumnChart.ColumnChart.SeriesSpacing = 1;
                lColumnChart.ColumnChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.InterpolateSimple;

                Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance chartTextAppearance = new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();

                chartTextAppearance.ChartTextFont = new System.Drawing.Font("Verdana", 7F);
                chartTextAppearance.Column = 0;
                chartTextAppearance.ItemFormatString = "<DATA_VALUE:00.00>";
                chartTextAppearance.Row = -2;
                chartTextAppearance.VerticalAlign = System.Drawing.StringAlignment.Center;
                chartTextAppearance.HorizontalAlign = System.Drawing.StringAlignment.Near;
                lColumnChart.BarChart.ChartText.Add(chartTextAppearance);

                lColumnChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
                lColumnChart.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
                lColumnChart.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
                lColumnChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
                lColumnChart.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
                lColumnChart.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                lColumnChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
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

                lColumnChart.Axis.X.Labels.ItemFormatString = String.Empty;

                lColumnChart.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
                lColumnChart.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;

                // title
                lColumnChart.TitleBottom.Visible = false;
                lColumnChart.TitleTop.Visible = false;

                // colours
                lColumnChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                lColumnChart.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(173)))), ((int)(((byte)(187)))));

                lColumnChart.ColorModel.AlphaLevel = ((byte)(255));
                lColumnChart.ColorModel.ColorBegin = System.Drawing.Color.FromArgb(151, 189, 100);
                lColumnChart.ColorModel.ColorEnd = System.Drawing.Color.FromArgb(151, 189, 100);
                lColumnChart.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.DataValueLinearRange;
                lColumnChart.ColorModel.Scaling = Infragistics.UltraChart.Shared.Styles.ColorScaling.Increasing;

                lColumnChart.Data.UseRowLabelsColumn = false;

                // tooltips
                lColumnChart.Tooltips.Font = new System.Drawing.Font("Verdana", 7.75F);
                lColumnChart.Tooltips.FormatString = " <SERIES_LABEL> (<DATA_VALUE>) ";
                lColumnChart.Tooltips.HighlightFillColor = System.Drawing.Color.LightBlue;
                lColumnChart.Tooltips.HighlightOutlineColor = System.Drawing.Color.LightBlue;

                lColumnChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
                lColumnChart.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;

                lColumnChart.Axis.Y.Margin.Far.MarginType = Infragistics.UltraChart.Shared.Styles.LocationType.Pixels;
                lColumnChart.Axis.X.Margin.Far.MarginType = Infragistics.UltraChart.Shared.Styles.LocationType.Pixels;
                lColumnChart.Axis.Y.Margin.Far.Value = 40;
                lColumnChart.Axis.X.Margin.Far.Value = 40;

                lColumnChart.Axis.X.Extent = 40;
                lColumnChart.Axis.Y.Extent = 30;

            }
            catch (Exception ex)
            {
            }

            return lColumnChart;
        }

        protected static void FormatExportSection(IText sectionText, ExportSection formatSection)
        {
            // Format the section
            // Horizontal Alignment
            if (formatSection.FormattedText.HorizontalAlignment == Infragistics.Win.HAlign.Center)
                sectionText.Alignment.Horizontal = Alignment.Center;
            else if (formatSection.FormattedText.HorizontalAlignment == Infragistics.Win.HAlign.Left)
                sectionText.Alignment.Horizontal = Alignment.Left;
            else
                sectionText.Alignment.Horizontal = Alignment.Right;

            // Vertical Alignment
            if (formatSection.FormattedText.VerticalAlignment == Infragistics.Win.VAlign.Top)
                sectionText.Alignment.Vertical = Alignment.Top;
            else if (formatSection.FormattedText.VerticalAlignment == Infragistics.Win.VAlign.Bottom)
                sectionText.Alignment.Horizontal = Alignment.Bottom;
            else
                sectionText.Alignment.Horizontal = Alignment.Middle;

            // Create a style based on the settings
            sectionText.Style = CreateStyle(formatSection);

            // Set the text
            sectionText.AddContent(formatSection.FormattedText.GetFormattedText);
        }

        protected static Style CreateStyle(ExportSection section)
        {
            // Create the font which we shall use for the header
            Infragistics.Documents.Reports.Graphics.Font headerFont = new Infragistics.Documents.Reports.Graphics.Font(section.FormattedText.FontData.Name, section.FormattedText.FontData.SizeInPoints);
            headerFont.Underline = (section.FormattedText.FontData.Underline == Infragistics.Win.DefaultableBoolean.True) ? true : false;
            headerFont.Bold = (section.FormattedText.FontData.Bold == Infragistics.Win.DefaultableBoolean.True) ? true : false;

            // Create a style based on this
            Infragistics.Documents.Reports.Graphics.Color infColor = new Infragistics.Documents.Reports.Graphics.Color(section.FormattedText.ForeColor);
            Infragistics.Documents.Reports.Graphics.SolidColorBrush Brush = new Infragistics.Documents.Reports.Graphics.SolidColorBrush(infColor);
            Infragistics.Documents.Reports.Report.Text.Style newStyle = new Infragistics.Documents.Reports.Report.Text.Style(headerFont, Brush);

            //Infragistics.Documents.Reports.Report.Text.Style newStyle = new Infragistics.Documents.Reports.Report.Text.Style(headerFont, Infragistics.Documents.Reports.Graphics.Brushes.DeepPink);


            // ..and return it
            return newStyle;
        }

        /// <summary>
        /// Send an email to the nominated recipients indicating the current Status of AuditWizard
        /// Note that this can be sent either as a Test Email from the User Interface or from the AuditWizard Service
        /// and may include Licensing Statistics, Support Contract Alerts and Alert Monitor Generated Alerts
        /// </summary>
        /// <returns></returns>
        public bool SendStatusEmail(bool test, bool dailyReport, AlertList listAlerts)
        {
            try
            {
                // Get the email configuration and ensure that it is valid
                EmailConfiguration emailConfiguration = new EmailConfiguration();

                if (!emailConfiguration.IsValid()) return false;

                // Recover the frequency setting for software compliancy emails	
                SettingsDAO lSettingsDao = new SettingsDAO();
                string mailFrequency = lSettingsDao.GetSetting(MailSettingsKeys.MailFrequency, false);
                bool emailAsHtml = lSettingsDao.GetSettingAsBoolean("EmailAsHtml", true);

                // Send the email if we have requested a test or if we have set an appropriate frequency
                if ((test) || (mailFrequency != MailFrequencyValues.Never))
                {
                    List<string> mailSubjectAndBody =
                        emailAsHtml ? GenerateMailSubjectAndBody(dailyReport, listAlerts) : GenerateMailSubjectAndBodyAsText(dailyReport, listAlerts);

                    emailConfiguration.SendEmail(mailSubjectAndBody[0], mailSubjectAndBody[1], emailAsHtml);
                }
            }

            catch (Exception e)
            {
                // If we are running a test then re-throw the exception so that the caller can catch the error otherwise we
                // simply add it to the EventLog as we assume that we are running from the service.
                if (!test)
                    System.Diagnostics.EventLog.WriteEntry("AuditWizardEmailService", "Failure sending email: " + e.Message, System.Diagnostics.EventLogEntryType.Information);
                else
                    throw (e);
            }
            return true;
        }



        void emailTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // determine if we should send an email today
            SettingsDAO lwDataAccess = new SettingsDAO();
            string mailFrequency = lwDataAccess.GetSetting(MailSettingsKeys.MailFrequency, false);

            if (mailFrequency == MailFrequencyValues.Daily ||
                (mailFrequency == MailFrequencyValues.Weekly) && (DateTime.Now.DayOfWeek == DayOfWeek.Monday) ||
                (mailFrequency == MailFrequencyValues.Monthly) && (DateTime.Now.Day == 1))
            {
                // We need to create a list of support contracts which we need to alert the user about
                AlertList listAlerts = new AlertList();
                CreateSupportContractAlerts(listAlerts);

                // send it
                SendStatusEmail(false, true, listAlerts);
            }

            dailyTimer.Elapsed -= emailTimer_Elapsed;
            ConfigureTimer();
        }




        /// <summary>
        /// Called as part of the alert processing - the function will create new alerts for any support contracts
        /// which are due to expire within a defined period and which have been flagged as requiring email notification
        /// </summary>
        protected void CreateSupportContractAlerts(AlertList listAlerts)
        {
            // Create a log file
            LogFile ourLog = LogFile.Instance;
            ourLog.Write("Creating Support Contract Alerts", true);

            try
            {
                // We also need to add in any support contract alerts as these are not going to be returned 
                // by the above call as they are not time dependant
                ApplicationInstanceDAO lwDataAccess = new ApplicationInstanceDAO();
                DataTable alertsTable = lwDataAccess.EnumerateSupportContractAlerts();                
                // ...and process each one
                foreach (DataRow thisRow in alertsTable.Rows)
                {
                    
                    // Calculate the number of days to go
                    DateTime supportExpiryDate = (DateTime)thisRow["_SUPPORT_EXPIRES"];
                    TimeSpan daysLeft = supportExpiryDate.Date - DateTime.Now.Date;
                    int days = daysLeft.Days;

                    // Format a message that we will report
                    string alertMessage;
                    if (days < -1)
                        alertMessage = String.Format("The support contract for {0} expired {1} days ago on {2}"
                                                        , (string)thisRow["_NAME"]
                                                        , Math.Abs(days)
                                                        , supportExpiryDate.ToShortDateString());
                    else if (days == -1)
                        alertMessage = String.Format("The support contract for {0} expired yesterday on {1}"
                                                        , (string)thisRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());
                    else if (days == 0)
                        alertMessage = String.Format("The support contract for {0} expires today {1}"
                                                        , (string)thisRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());

                    else if (days == 1)
                        alertMessage = String.Format("The support contract for {0} will expire tomorrow on {1}"
                                                        , (string)thisRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());
                    else
                        alertMessage = String.Format("The support contract for {0} will expire in {1} days on {2}"
                                                        , (string)thisRow["_NAME"]
                                                        , days
                                                        , supportExpiryDate.ToShortDateString());

                    // Construct an Alert object from this information							
                    Alert newAlert = new Alert(Alert.AlertType.support
                                              , Alert.AlertCategory.expired
                                              , alertMessage
                                              , ""
                                              , "");

                    // Add the alert to our list
                    listAlerts.Add(newAlert);

                    NewsFeed.AddNewsItem(NewsFeed.Priority.Information, alertMessage);
                }

                DataTable tableAssetAlert = lwDataAccess.EnumerateSupportContractAssetAlerts();
                foreach (DataRow assetRow in tableAssetAlert.Rows)
                {
                    // Calculate the number of days to go
                    DateTime supportExpiryDate = (DateTime)assetRow["_SUPPORT_EXPIRY"];
                    TimeSpan daysLeft = supportExpiryDate.Date - DateTime.Now.Date;
                    int days = daysLeft.Days;

                    // Format a message that we will report
                    string alertMessage;
                    if (days < -1)
                        alertMessage = String.Format("The support contract for asset {0} expired {1} days ago on {2}"
                                                        , (string)assetRow["_NAME"]
                                                        , Math.Abs(days)
                                                        , supportExpiryDate.ToShortDateString());
                    else if (days == -1)
                        alertMessage = String.Format("The support contract for asset {0} expired yesterday on {1}"
                                                        , (string)assetRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());
                    else if (days == 0)
                        alertMessage = String.Format("The support contract for asset {0} expires today {1}"
                                                        , (string)assetRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());

                    else if (days == 1)
                        alertMessage = String.Format("The support contract for asset {0} will expire tomorrow on{1}"
                                                        , (string)assetRow["_NAME"]
                                                        , supportExpiryDate.ToShortDateString());
                    else
                        alertMessage = String.Format("The support contract for asset {0} will expire in {1} days on {2}"
                                                        , (string)assetRow["_NAME"]
                                                        , days
                                                        , supportExpiryDate.ToShortDateString());

                    // Construct an Alert object from this information							
                    Alert newAlert = new Alert(Alert.AlertType.support
                                              , Alert.AlertCategory.expired
                                              , alertMessage
                                              , ""
                                              , "");

                    // Add the alert to our list
                    listAlerts.Add(newAlert);

                    NewsFeed.AddNewsItem(NewsFeed.Priority.Information, alertMessage);
                }               
            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [CreateSupportContractAlerts], Exception Text is is " + ex.Message, true);
            }
        }

        private List<string> GenerateMailSubjectAndBodyAsText(bool dailyReport, AlertList listAlerts)
        {
            string mailSubject = "AuditWizard Status Report for " + DateTime.Now;
            string nl = Environment.NewLine;
            string reportText = "";

            if (dailyReport)
            {
                List<string> computerStatistics = GetComputerStatisticsReportAsText();
                List<string> applicationStatistics = GetApplicationStatisticsReportAsText();

                reportText = String.Format(
                    "--------------------------" + nl +
                    "{0}" + nl +
                    "--------------------------\r\n" + nl +

                    "Asset Summary" + nl + nl +
                    "Assets Discovered: {1}" + nl +
                    "Assets Audited: {2}" + nl +
                    "Assets Not Audited: {3}" + nl +
                    "Assets in Stock: {4}" + nl +
                    "Assets In Use: {5}" + nl +
                    "Assets Pending Disposal: {6}" + nl +
                    "Assets Disposed: {7}" + nl +
                    "---------------------------" + nl +
                    "" + nl +
                    "Application Summary" + nl + nl +
                    "Publishers Identified: {8}" + nl +
                    "Total Applications Audited: {9}" + nl +
                    "Number of Unique Applications: {10}" + nl +
                    "Number of Filter Publishers: {11}" + nl +
                    "Compliant Applications: {12}" + nl +
                    "Non-Compliant Applications: {13}" + nl +
                    "Ignored Applications: {14}" + nl +
                    "None Specified Applications: {15}" + nl +
                    "" + nl + "---------------------------" + nl + nl,
                    "AUDITWIZARD SUMMARY REPORT",
                    computerStatistics[0],
                    computerStatistics[1],
                    computerStatistics[2],
                    computerStatistics[3],
                    computerStatistics[4],
                    computerStatistics[5],
                    computerStatistics[6],
                    applicationStatistics[0],
                    applicationStatistics[1],
                    applicationStatistics[2],
                    applicationStatistics[3],
                    applicationStatistics[4],
                    applicationStatistics[5],
                    applicationStatistics[6],
                    applicationStatistics[7]);

                DataTable lComplianceDataTable = new StatisticsDAO().StatisticsComplianceByType(String.Empty);

                reportText += (int)lComplianceDataTable.Rows[1][1] == 0
                                  ? "Congratulations!  AuditWizard has determined that you are correctly licensed for all audited applications"
                                  : "AuditWizard has determined that you are NOT correctly licensed for all audited applications.  Please refer to the details shown above and that displayed within the AuditWizard Manager for an in-depth analysis of which applications are incorrectly licensed";

                if (listAlerts != null && listAlerts.Count != 0)
                    reportText += AddSupportContractAlertsAsText(listAlerts);
            }

            else
            {
                // Remove any support contract alerts as these are not included in alert Monitor reports
                AlertList alertMonitorAlerts = new AlertList();

                if (listAlerts != null)
                {
                    foreach (Alert thisAlert in listAlerts)
                    {
                        if (thisAlert.Type != Alert.AlertType.support)
                            alertMonitorAlerts.Add(thisAlert);
                    }
                }

                // Now add the Alert Monitor Alerts
                reportText += AddAlertMonitorAlertsAsText(alertMonitorAlerts);
            }

            reportText +=
                    nl + "Thank you for using AuditWizard. Please refer to the user guide to modify the email settings for the email reports." +
                    nl + nl + "Sincerely," + nl + nl + "AuditWizard Team" + nl + nl +
                    "The information contained in this report has been automatically generated based on data collected from Computers on " +
                    " your network and manually entered. Although Layton Technology, Inc. have taken all possible steps to ensure the " +
                    "accuracy of this data we cannot guarantee that (a) all computers owned by your organization have been audited or (b) " +
                    "the accuracy of any information manually entered. As such, this report should be treated as a guide to assist in " +
                    "ensuring software licensing compliancy. Layton Technology, Inc. cannot be held liable for any omissions or inaccuracies " +
                    "which may be contained within this report.";

            return new List<string> { mailSubject, reportText };
        }

        /// <summary>
        /// Generate the subject and body text for the email message as HTML
        /// </summary>
        /// <returns></returns>
        private List<string> GenerateMailSubjectAndBody(bool dailyReport, AlertList listAlerts)
        {
            string mailSubject = "AuditWizard Status Report for " + DateTime.Now;
            string mailBody = dailyReport ? StatusEmailHtmlBlocks.HeaderSummary : StatusEmailHtmlBlocks.HeaderAlert;
            mailBody += StatusEmailHtmlBlocks.Opening;

            // A Daily report contains the license statistics and support contract alerts
            if (dailyReport)
            {
                mailBody += AddLicenseStatisticsToReport();

                // Now add the support contract alerts 
                if (listAlerts != null && listAlerts.Count != 0)
                    mailBody += AddSupportContractAlerts(listAlerts);
            }

            // Alert Monitor Email being sent
            else
            {
                // Remove any support contract alerts as these are not included in alert Monitor reports
                AlertList alertMonitorAlerts = new AlertList();

                if (listAlerts != null)
                {
                    foreach (Alert thisAlert in listAlerts)
                    {
                        if (thisAlert.Type != Alert.AlertType.support)
                            alertMonitorAlerts.Add(thisAlert);
                    }
                }

                // Now add the Alert Monitor Alerts
                mailBody += AddAlertMonitorAlerts(alertMonitorAlerts);
            }

            // ...and add the footer etc
            mailBody += StatusEmailHtmlBlocks.Ending;
            mailBody += "</table></td><td width=200 height='100%' valign='top'>";
            mailBody += StatusEmailHtmlBlocks.RightPanel;
            mailBody += "</td></tr>";
            mailBody += StatusEmailHtmlBlocks.Footer;

            // Add the subject and body to the email
            return new List<string> { mailSubject, mailBody };
        }

        /// <summary>
        /// Called to add the License Statistics to the email body
        /// We add this as a row to an existing table which requires 5 columns
        /// </summary>
        private string AddLicenseStatisticsToReport()
        {
            // First add the image for the License Statistics Title
            string returnHTML = "<TR>"
                                + "<TD></TD>"
                                + "<TD colspan=3><img src='http://laytontechnology.com/auditwizardv8/computersummarytitle.jpg'><br><br></td>"
                                + "<TD></TD>"
                                + "</TR>";

            // Add a row for the Computer Statistics Report		
            returnHTML += "<TR>"
                     + "<TD></TD>"
                     + "<TD colspan=3>" + GetComputerStatisticsReport() + "<BR><BR></TD>"
                     + "<TD></TD>"
                     + "</TR>";

            // Header for the Application Statistics Title
            returnHTML += "<TR>"
                     + "<TD></TD>"
                     + "<TD colspan=3><img src='http://laytontechnology.com/auditwizardv8/applicationsummarytitle.jpg'><br><br></td>"
                     + "<TD></TD>"
                     + "</TR>";

            // Add a row for the Application Statistics Report		
            returnHTML += "<TR>"
                     + "<TD></TD>"
                     + "<TD colspan=3>" + GetApplicationStatisticsReport() + "<BR><BR></TD>"
                     + "<TD></TD>"
                     + "</TR>";

            // What's the overall status?  Are we compliant?
            //int compliant, noncompliant;
            //_listPublishers.CompliantApplicationCount(out compliant, out noncompliant);

            DataTable lComplianceDataTable = new StatisticsDAO().StatisticsComplianceByType(String.Empty);

            if ((int)lComplianceDataTable.Rows[1][1] == 0)
            {
                returnHTML += "<tr><td></td><td colspan=3><img src='http://laytontechnology.com/auditwizardv8/complianttitle.jpg'><br><br></td><td></td></tr>";
                returnHTML += "<tr><td></td><td colspan=3>";
                returnHTML += "Congratulations!  AuditWizard has determined that you are correctly licensed for all audited applications";
            }
            else
            {
                returnHTML += "<tr><td></td><td colspan=3><img src='http://laytontechnology.com/auditwizardv8/noncomplianttitle.jpg'><br><br></td><td></td></tr>";
                returnHTML += "<tr><td></td><td colspan=3>";
                returnHTML += "AuditWizard has determined that you are NOT correctly licensed for all audited applications.  Please refer to the details shown above and that displayed within the AuditWizard Manager for an in-depth analysis of which applications are incorrectly licensed";
            }

            return returnHTML;
        }

        /// <summary>
        /// Called to add the Support Contract Alerts to the email body
        /// We add this as a row to an existing table which requires 5 columns
        /// </summary>
        /// <param name="supportAlerts"></param>
        private string AddSupportContractAlertsAsText(AlertList supportAlerts)
        {
            // Now add an image for the Support Contract Alerts
            string reportText = Environment.NewLine + Environment.NewLine + "---------------------------" + Environment.NewLine + Environment.NewLine;

            // Then add the alerts themselves or a placeholder if there are no alerts
            if (supportAlerts.Count == 0)
            {
                reportText += "No Support Contract Alerts were found";
            }

            else
            {
                // Add the Introducer
                reportText += "Below is a list of Application Support Contract Alerts Generated for Today:" + Environment.NewLine + Environment.NewLine;
                //			
                foreach (Alert thisAlert in supportAlerts)
                {
                    reportText += Environment.NewLine + thisAlert.Message + Environment.NewLine;
                }
            }

            return reportText + Environment.NewLine + "---------------------------";
        }


        /// <summary>
        /// Called to add the Support Contract Alerts to the email body
        /// We add this as a row to an existing table which requires 5 columns
        /// </summary>
        /// <param name="supportAlerts"></param>
        private string AddSupportContractAlerts(AlertList supportAlerts)
        {
            // Now add an image for the Support Contract Alerts
            string html = "<TR>"
                          + "<TD></TD>"
                          + "<TD colspan=3><img src='http://laytontechnology.com/auditwizardv8/SupportContractTitle.png'><br><br></td>"
                          + "<TD></TD>"
                          + "</TR>";

            // Then add the alerts themselves or a placeholder if there are no alerts
            if (supportAlerts.Count == 0)
            {
                html += StatusEmailHtmlBlocks.SupportContractIntroducerNone;
            }

            else
            {
                // Add the Introducer
                html += StatusEmailHtmlBlocks.SupportContractIntroducer;
                //			
                foreach (Alert thisAlert in supportAlerts)
                {
                    html += "<tr><td></td><td colspan=3>" + thisAlert.Message + "<br><br></td><td></td></tr>";
                }
            }

            return html;
        }

        /// <summary>
        /// Called to add the Alert Monitor Alerts to the email body
        /// We add this as a row to an existing table which requires 5 columns
        /// </summary>
        /// <param name="alerts"></param>
        private string AddAlertMonitorAlertsAsText(AlertList alerts)
        {
            string reportText = "";

            // Now add the AlertMonitor Alerts (if any)
            if (alerts.Count == 0)
            {
                reportText += "No AlertMontitor Alerts were found";
            }

            else
            {
                // Add the Introducer
                reportText += "Below is a list of AlertMonitor Alerts Recently Generated:" +
                    Environment.NewLine + Environment.NewLine + "-------------------------------" + Environment.NewLine + Environment.NewLine;

                // Add a row for the Alert Monitor Report		
                reportText += GetAlertMonitorReportAsText(alerts);
            }

            return reportText;
        }

        /// <summary>
        /// Called to add the Alert Monitor Alerts to the email body
        /// We add this as a row to an existing table which requires 5 columns
        /// </summary>
        /// <param name="alerts"></param>
        private string AddAlertMonitorAlerts(AlertList alerts)
        {
            string html = "<TR>"
                     + "<TD></TD>"
                     + "<TD colspan=3><img src='http://laytontechnology.com/auditwizardv8/alertmonitortitle.png'><br><br></td>"
                     + "<TD></TD>"
                     + "</TR>";

            // Now add the AlertMonitor Alerts (if any)
            if (alerts.Count == 0)
            {
                html += StatusEmailHtmlBlocks.AlertIntroducerNone;
            }

            else
            {
                // Add the Introducer
                html += StatusEmailHtmlBlocks.AlertIntroducer;

                // Add a row for the Alert Monitor Report		
                html += "<TR>"
                         + "<TD></TD>"
                         + "<TD colspan=3>" + GetAlertMonitorReport(alerts) + "<BR><BR></TD>"
                         + "<TD></TD>"
                         + "</TR>";
            }

            return html;
        }

        private List<string> GetComputerStatisticsReportAsText()
        {
            AssetStatistics statistics = new AssetStatistics(new StatisticsDAO().AssetStatistics().Rows[0]);

            return new List<string> 
            { 
                statistics.Discovered.ToString(), 
                statistics.Audited.ToString(),
                statistics.NotAudited.ToString(),
                statistics.Stock.ToString(),
                statistics.InUse.ToString(),
                statistics.PendingDisposal.ToString(),
                statistics.Disposed.ToString()
            };
        }

        private string GetComputerStatisticsReport()
        {
            // Recover Asset Statistics
            AssetStatistics statistics = new AssetStatistics(new StatisticsDAO().AssetStatistics().Rows[0]);

            // We create a Grid View to display the results in however we will only have a single column
            // and will add in a textual description of each result line
            GridView statisticsGrid = new GridView(System.Drawing.Color.SteelBlue, System.Drawing.Color.White, 12, 20, System.Drawing.Color.Navy, System.Drawing.Color.CornflowerBlue);
            statisticsGrid.AddColumn(400);	// Title
            statisticsGrid.AddColumn(100);	// Value

            // Add the data 
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleDiscovered, statistics.Discovered.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleAudited, statistics.Audited.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleNotAudited, statistics.NotAudited.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleStock, statistics.Stock.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleInUse, statistics.InUse.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitlePending, statistics.PendingDisposal.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { AssetStatistics.TitleDisposed, statistics.Disposed.ToString() }));

            return statisticsGrid.GenerateHtmlCode();
        }

        private List<string> GetApplicationStatisticsReportAsText()
        {
            DataTable statisticsTable = new StatisticsDAO().AuditStatistics();
            AuditStatistics statistics = new AuditStatistics(statisticsTable.Rows[0]);
            DataTable lComplianceDataTable = new StatisticsDAO().StatisticsComplianceByType(String.Empty, String.Empty);

            return new List<string> 
            { 
                statistics.Publishers.ToString(), 
                statistics.TotalApplications.ToString(),
                statistics.UniqueApplications.ToString(),
                statistics.FilterPublishers.ToString(),
                lComplianceDataTable.Rows[0][1].ToString(),
                lComplianceDataTable.Rows[1][1].ToString(),
                lComplianceDataTable.Rows[2][1].ToString(),
                lComplianceDataTable.Rows[3][1].ToString()
            };
        }

        private string GetApplicationStatisticsReport()
        {
            // Recover Asset Statistics
            StatisticsDAO lwDataAccess = new StatisticsDAO();
            DataTable statisticsTable = lwDataAccess.AuditStatistics();
            AuditStatistics statistics = new AuditStatistics(statisticsTable.Rows[0]);
            //
            int publishersCount = statistics.Publishers;
            int totalCount = statistics.TotalApplications;
            int uniqueCount = statistics.UniqueApplications;
            int filterCount = statistics.FilterPublishers;

            // We create a Grid View to display the results in however we will only have a single column
            // and will add in a textual description of each result line
            GridView statisticsGrid = new GridView(System.Drawing.Color.SteelBlue, System.Drawing.Color.White, 12, 20, System.Drawing.Color.Navy, System.Drawing.Color.CornflowerBlue);
            statisticsGrid.AddColumn(400);	// Title
            statisticsGrid.AddColumn(100);	// Value

            // Add the data 
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.PublishersAudited, publishersCount.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.TotalApplications, totalCount.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.UniqueApplications, uniqueCount.ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.PublishersInFilter, filterCount.ToString() }));

            // Compliancy Counts - this requires us to get the publishers list as the statistics are derived 
            // from the information contained therein
            // We need to pull the publisher filter list from the database
            //string publisherFilter = new SettingsDAO().GetPublisherFilter();
            //_listPublishers = new ApplicationPublisherList(publisherFilter, true, false);

            //int compliant, noncompliant;
            //_listPublishers.CompliantApplicationCount(out compliant, out noncompliant);

            DataTable lComplianceDataTable = new StatisticsDAO().StatisticsComplianceByType(String.Empty, String.Empty);

            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.LicensedApplications, lComplianceDataTable.Rows[0][1].ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.NonLicensedApplications, lComplianceDataTable.Rows[1][1].ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.IgnoredApplications, lComplianceDataTable.Rows[2][1].ToString() }));
            statisticsGrid.AddRow(new List<String>(new string[] { StatisticTitles.NonSpecifiedApplications, lComplianceDataTable.Rows[3][1].ToString() }));

            // Generate the code for this grid
            return statisticsGrid.GenerateHtmlCode();
        }

        /// <summary>
        /// Generate the HTML code for an Alert Monitor table
        /// </summary>
        /// <param name="alerts"></param>
        /// <returns></returns>
        private string GetAlertMonitorReport(AlertList alerts)
        {
            // We create a Grid View to display the results 
            // We will add the following columns :- date, PC ,Reason, Item ,Old Value , New Value
            GridView alertsGrid = new GridView(System.Drawing.Color.SteelBlue, System.Drawing.Color.White, 12, 20, System.Drawing.Color.Navy, System.Drawing.Color.CornflowerBlue);
            alertsGrid.AddColumn(80);	// Date
            alertsGrid.AddColumn(80);	// Alert Name
            alertsGrid.AddColumn(80);	// Asset name
            alertsGrid.AddColumn(80);	// Item
            alertsGrid.AddColumn(80);	// Alert category (changed/added/deleted)
            alertsGrid.AddColumn(80);	// Old Value
            alertsGrid.AddColumn(80);	// New Value

            // Headers
            List<string> headersList = new List<string>();
            headersList.Add("Date");
            headersList.Add("Alert Name");
            headersList.Add("Asset Name");
            headersList.Add("Item");
            headersList.Add("Category");
            headersList.Add("Old Value");
            headersList.Add("New Value");

            // Add the header row first
            alertsGrid.AddRow(headersList);

            // Add the data 
            foreach (Alert alert in alerts)
            {
                alertsGrid.AddRow(new List<String>(new string[] { alert.AlertedOnDate.ToString()
																	,alert.AlertName
																	, alert.AssetName
																	, alert.Message
																	, alert.TypeAsString
																	, alert.Field1
																	, alert.Field2 }));
            }

            // Generate the code for this grid
            return alertsGrid.GenerateHtmlCode();
        }

        /// <summary>
        /// Generate the HTML code for an Alert Monitor table
        /// </summary>
        /// <param name="alerts"></param>
        /// <returns></returns>
        private string GetAlertMonitorReportAsText(AlertList alerts)
        {
            string reportText = "";

            // Add the data 
            foreach (Alert alert in alerts)
            {
                reportText += String.Format(
                   @"{0}" + Environment.NewLine + Environment.NewLine +
                   @"Alert Name : {1} " + " " + Environment.NewLine +
                   @"Asset Name : {2} " + " " + Environment.NewLine +
                   @"Category : {3} " + " " + Environment.NewLine +
                   @"Item : {4} " + " " + Environment.NewLine +
                   @"Old Value : {5} " + " " + Environment.NewLine +
                   @"New Value : {6} " + " " + Environment.NewLine,
                   alert.AlertedOnDate,
                   alert.AlertName,
                   alert.AssetName,
                   alert.TypeAsString,
                   alert.Message,
                   alert.Field1,
                   alert.Field2);

                reportText += Environment.NewLine + "-------------------------------" + Environment.NewLine + Environment.NewLine;
            }

            return reportText;
        }
    }
}
