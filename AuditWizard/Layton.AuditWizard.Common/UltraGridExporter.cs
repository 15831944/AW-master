using System;
using System.IO;
using System.Windows.Forms;
//
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Win.UltraDataGridView;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.DocumentExport;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Layton.AuditWizard.DataAccess;
using PageNumbering = Infragistics.Documents.Reports.Report.Section.PageNumbering;

namespace Layton.AuditWizard.Common
{
    /// <summary>
    /// This class encapulates the exporting of an UltraGrid to any of the supplied formats
    /// We use a class here rather than the native document explorer as the latter does not support
    /// the addition of a header which we really want in our reports
    /// </summary>
    /// 
    public class UltraGridExporter
    {
        /// <summary>
        /// Export the specified grid to the specified file in the specified format
        /// </summary>
        /// <param name="toFile">Name of the file to export to</param>
        /// <param name="grid">Grid View to export</param>
        /// <param name="outputFormat">Output format</param>
        public static void Export(String toFile, UltraGrid grid, FileFormat outputFormat)
        {
            try
            {
                Export(toFile, null, null, null, grid, outputFormat);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to export data to file " + toFile + ". The error was " + e.Message, "Export Failed");
            }
        }

        public static void ExportUltraGridToExcel(UltraGrid grid, string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ValidateNames = true;
            saveFileDialog.FileName = fileName + ".xls";
            saveFileDialog.Filter = "Microsoft Excel Spreadsheet (*.xls)|*.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog.FileName))
                {
                    if (MessageBox.Show(saveFileDialog.FileName + " already exists." + Environment.NewLine + Environment.NewLine +
                        "Do you want to replace it?",
                        "Save As", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                        return;
                }

                int columnsCount = grid.DisplayLayout.Bands[0].Columns.Count;
                int rowsCount = grid.DisplayLayout.Rows.Count;

                if (columnsCount <= 256 && rowsCount <= 65535)
                {
                    UltraGridExcelExporter exporter = new UltraGridExcelExporter();
                    Workbook workbook = new Workbook();
                    Worksheet worksheet = workbook.Worksheets.Add("Sheet1");							// 8.3.4 - CMD - Hardwire Excel sheet name

                    exporter.Export(grid, worksheet);

                    try
                    {
                        workbook.Save(saveFileDialog.FileName);
                    }
                    catch (IOException ex)
                    {
                        Utility.DisplayApplicationErrorMessage(ex.Message);
                        return;
                    }
                    

                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }
                else
                {
                    MessageBox.Show(
                        "The maximum amount of data that can be exported to Excel has been exceeded." +
                        Environment.NewLine + Environment.NewLine +
                        "Please reduce the number of rows and/or coulumns to be exported.",
                        "Export limit reached"
                        , MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Export the 
        /// </summary>
        /// <param name="ToFile"></param>
        /// <param name="listSections"></param>
        /// <param name="outputFormat"></param>
        public static void Export(string toFile, UltraGrid grid, ExportSectionList listSections, FileFormat outputFormat)
        {
            // Create the report itself
            Report report = new Report();

            // ...then the sections for header, footer and body
            ISection mainSection = report.AddSection();
            mainSection.PageMargins = new Infragistics.Documents.Reports.Report.Margins(50);
            ISectionHeader headerSection = mainSection.AddHeader();
            headerSection.Height = 50;
            headerSection.Repeat = true;
            ISectionFooter footerSection = mainSection.AddFooter();
            footerSection.Height = 50;

            // Add the body of the report which is the contents of the grid
            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.Export(grid, mainSection);

            // Setup the page numbering
            PageNumbering pn = mainSection.PageNumbering;

            // The Template property is the actual string that shows the page numbering. Use the [Page #] place-
            // holder for the current page and the [TotalPages] place-holder for the total amount of pages in
            // the entire document.
            pn.Template = "Page [Page #] of [TotalPages]";

            // Setting SkipFirst to true does not place page numbering on the first page of the section. This
            // is useful if the first page is a Title page.
            pn.SkipFirst = false;

            // The page numbering will be aligned with the right side of the page. Valid values off the
            // Alignment enum include Left, Center, and Right.
            pn.Alignment.Horizontal = Alignment.Right;

            // The page numbering will be located at the bottom of the page. Valid values off the
            // Alignment enum include Top and Bottom.
            pn.Alignment.Vertical = Alignment.Bottom;

            // The page numbering is at the extreme bottom of the page, so we need to change the Y Offset
            // in order to bring it in line with the rest of the page footer text.
            pn.OffsetY = -18;

            // Delete the old report if it exists
            try
            {
                if (File.Exists(toFile))
                {
                    File.Delete(toFile);
                }
            }
            catch (Exception)
            {
                //Ignore any errors
            }

            // Generate the report
            try
            {
                report.Generate();
                report.Publish(toFile, outputFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write the export file " + toFile + ", the error was " + ex.Message);
            }
        }


        /// <summary>
        /// Called to format an exportable section of the report
        /// </summary>
        /// <param name="exportFooterSection"></param>
        /// <param name="section"></param>
		protected static void FormatExportSection(Infragistics.Documents.Reports.Report.Text.IText sectionText, ExportSection formatSection)
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


        /// <summary>
        /// Create an Infragistics style based on the criteria specified within the ExportSection object
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        protected static Infragistics.Documents.Reports.Report.Text.Style CreateStyle(ExportSection section)
        {
            // Create the font which we shall use for the header
			Infragistics.Documents.Reports.Graphics.Font headerFont = new Infragistics.Documents.Reports.Graphics.Font(section.FormattedText.FontData.Name, section.FormattedText.FontData.SizeInPoints);
            headerFont.Underline = (section.FormattedText.FontData.Underline == Infragistics.Win.DefaultableBoolean.True) ? true : false;
            headerFont.Bold = (section.FormattedText.FontData.Bold == Infragistics.Win.DefaultableBoolean.True) ? true : false;

            // Create a style based on this
            Infragistics.Documents.Reports.Graphics.Color infColor = new Color(section.FormattedText.ForeColor);
            Infragistics.Documents.Reports.Graphics.SolidColorBrush Brush = new Infragistics.Documents.Reports.Graphics.SolidColorBrush(infColor);
            Infragistics.Documents.Reports.Report.Text.Style newStyle = new Infragistics.Documents.Reports.Report.Text.Style(headerFont, Brush);

            //Infragistics.Documents.Reports.Report.Text.Style newStyle = new Infragistics.Documents.Reports.Report.Text.Style(headerFont, Infragistics.Documents.Reports.Graphics.Brushes.DeepPink);


            // ..and return it
            return newStyle;
        }




        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="toFile">Name of the file to export to</param>
        /// <param name="headerText">Main Header text</param>
        /// <param name="subheadingText">Sub-heading text</param>
        /// <param name="footerText">footer text</param>
        /// <param name="grid">Grid View to export</param>
        /// <param name="outputFormat">Output format</param>
        public static void Export(String toFile
                                , String headerText
                                , String subheadingText
                                , String footerText
                                , UltraGrid grid
                                , FileFormat outputFormat)
        {

            // Create the report itself
            Report report = new Report();

            // ...then the sections for header, footer and body
            ISection mainSection = report.AddSection();
            mainSection.PageMargins = new Infragistics.Documents.Reports.Report.Margins(50);
            ISectionHeader headerSection = mainSection.AddHeader();
            headerSection.Height = 50;
            headerSection.Repeat = true;

            // Add a footer for page numbering
            ISectionFooter footerSection = mainSection.AddFooter();

            // Create place-holder for header text
            Infragistics.Documents.Reports.Report.Text.IText headerTextSection = headerSection.AddText(0, 0);

            // ...and add the header in to this section, centralized 
            headerTextSection.AddContent(headerText);
            headerTextSection.Alignment.Horizontal = Alignment.Center;
            headerTextSection.Alignment.Vertical = Alignment.Middle;

            // Set style for the header
            Infragistics.Documents.Reports.Report.Text.Style HeaderStyle = new Infragistics.Documents.Reports.Report.Text.Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 10, Infragistics.Documents.Reports.Graphics.FontStyle.Underline), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
            headerTextSection.Style = HeaderStyle;

            // Add in a sub-heading if required
            if (subheadingText != null && subheadingText != "")
            {
                headerTextSection = headerSection.AddText(0, 20);
                Infragistics.Documents.Reports.Report.Text.Style subHeaderStyle = new Infragistics.Documents.Reports.Report.Text.Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 8, Infragistics.Documents.Reports.Graphics.FontStyle.Underline), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                headerTextSection.Style = subHeaderStyle;
                headerTextSection.AddContent(subheadingText);
            }

            // Add the body of the report which is the contents of the grid
            UltraGridDocumentExporter exporter = new UltraGridDocumentExporter();
            exporter.Export(grid, mainSection);

            //-------------------------
            // Setup Footer content
            //-------------------------
            footerSection.Height = 50;

            // Do we have a footer to display?
            if (footerText != null && footerText != "")
            {
                // Create place-holder for footer text
                Infragistics.Documents.Reports.Report.Text.IText footerTextSection = footerSection.AddText(0, 0);

                // ...and add the footer text in to this section, left aligned
                footerTextSection.Alignment.Horizontal = Alignment.Left;
                footerTextSection.Alignment.Vertical = Alignment.Top;

                // Set style for the text
                Infragistics.Documents.Reports.Report.Text.Style footerStyle = new Infragistics.Documents.Reports.Report.Text.Style(new Infragistics.Documents.Reports.Graphics.Font("Verdana", 6, Infragistics.Documents.Reports.Graphics.FontStyle.Underline), Infragistics.Documents.Reports.Graphics.Brushes.DarkBlue);
                footerTextSection.Style = footerStyle;

                // ...and set the text itself
                footerTextSection.AddContent(footerText);
            }

            PageNumbering pn = mainSection.PageNumbering;

            // The Template property is the actual string that shows the page numbering. Use the [Page #] place-
            // holder for the current page and the [TotalPages] place-holder for the total amount of pages in
            // the entire document.
            pn.Template = "Page [Page #] of [TotalPages]";

            // Setting SkipFirst to true does not place page numbering on the first page of the section. This
            // is useful if the first page is a Title page.
            pn.SkipFirst = false;

            // The page numbering will be aligned with the right side of the page. Valid values off the
            // Alignment enum include Left, Center, and Right.
            pn.Alignment.Horizontal = Alignment.Right;

            // The page numbering will be located at the bottom of the page. Valid values off the
            // Alignment enum include Top and Bottom.
            pn.Alignment.Vertical = Alignment.Bottom;

            // The page numbering is at the extreme bottom of the page, so we need to change the Y Offset
            // in order to bring it in line with the rest of the page footer text.
            pn.OffsetY = -18;

            // Delete the old report if it exists
            try
            {
                if (File.Exists(toFile))
                {
                    File.Delete(toFile);
                }
            }
            catch (Exception)
            {
                //Ignore any errors
            }

            // Generate the report
            try
            {
                report.Generate();
                report.Publish(toFile, outputFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write the export file " + toFile + ", the error was " + ex.Message);
            }
        }
    }
}
