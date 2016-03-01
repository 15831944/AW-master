using System;
using System.Collections.Generic;
using System.Text;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public class StatusEmailHtmlBlocks
    {
        // This is the HTML code which will start the HTML page
        public const string HeaderSummary = "<HTML>"
                                    + "<HEAD>"
                                    + "<TITLE>AuditWizard Status Report</TITLE>"
                                    + "<STYLE type=text/css>body {margin:0px; padding:0px; } table {margin-bottom:0px; font-size: 12px; font-family: Verdana, Verdana, Helvetica, sans-serif;} disclaimer {margin-bottom:0px; font-size: 8px; font-family: Verdana, Verdana, Helvetica, sans-serif;}</STYLE>"
                                    + "</HEAD>"
                                    + "<BODY><CENTER>"
                                    + "<TABLE width=800 border='0' cellpadding='0' cellspacing='0'>"
                                    + "<TR>"
                                    + "<TD valign='bottom' colspan='2'><img src='http://laytontechnology.com/auditwizardv8/summary_email_header.jpg'></TD>"
                                    + "</TR>"
                                    + "<TR>"
                                    + "<TD width=600 valign='top'>"
                                    + "   <TABLE>";

        // This is the HTML code which will start the HTML page for an alerts email
        public const string HeaderAlert = "<HTML>"
                                    + "<HEAD>"
                                    + "<TITLE>AuditWizard Status Report</TITLE>"
                                    + "<STYLE type=text/css>body {margin:0px; padding:0px; } table {margin-bottom:0px; font-size: 12px; font-family: Verdana, Verdana, Helvetica, sans-serif;} disclaimer {margin-bottom:0px; font-size: 8px; font-family: Verdana, Verdana, Helvetica, sans-serif;}</STYLE>"
                                    + "</HEAD>"
                                    + "<BODY><CENTER>"
                                    + "<TABLE width=800 border='0' cellpadding='0' cellspacing='0'>"
                                    + "<TR>"
                                    + "<TD valign='bottom' colspan='2'><img src='http://laytontechnology.com/auditwizardv8/alerts_email_header.jpg'></TD>"
                                    + "</TR>"
                                    + "<TR>"
                                    + "<TD width=600 valign='top'>"
                                    + "   <TABLE>";


        // This is the HTML code which appears at the beginning of the email body
        public const string Opening = "<TR>"
                                    + "<TD width=10><BR></TD>"
                                    + "<TD width=220></TD>"
                                    + "<TD width=30></TD>"
                                    + "<TD width=300></TD>"
                                    + "<TD width=10></TD>"
                                    + "</TR>";

        // This is the HTML code which will appear as the footer for the email
        public const string Footer = "<TR>"
                                    + "<TD colSpan=2 valign='top'><IMG src='http://laytontechnology.com/auditwizardv8/emailbasebar.jpg'></TD>"
                                    + "</TR>"
                                    + "</TABLE>"
                                    + "</CENTER>"
                                    + "<p>" + DataStrings.Disclaimer + "</p>"
                                    + "</BODY>"
                                    + "</HTML>";


        // This is the HTML code which will appear at the end of the email
        public const string Ending = "<tr>"
                                    + "<td></td>"
                                    + "<td colspan=3><br><br>Thank you for using AuditWizard. Please refer to the user guide to modify the email settings for the email reports.<br><br>Sincerely,<br><br>AuditWizard Team<br><br></td>"
                                    + "<td></td>"
                                    + "</tr>";

        // This is the HTML code for the panel which appears to the right of the Compliance Email
        public const string RightPanel = "<table cellspacing='0' cellpadding='0' border='0' height='100%'>"
                                        + "<tr>"
                                        + "<td valign='top'>"
                                        + "<img src='http://laytontechnology.com/auditwizardv8/light_blue_corner_arc.gif'></td>"
                                        + "<td valign='top'>"
                                        + "<img src='http://laytontechnology.com/auditwizardv8/right_panel_top.jpg'></td>"
                                        + "</tr>"
                                        + "<tr>"
                                        + "<td></td>"
                                        + "<td valign='top'><map id='aw_link_map' name='aw_link_map'>"
                                        + "<area shape='rect' coords='22,7,171,248' href='http://www.auditwizard.com/pages/auditwizard.asp' target='_blank' />"
                                        + "<img style='border:none;' usemap='#aw_link_map' src='http://laytontechnology.com/auditwizardv8/right_info_panel.jpg'></td>"
                                        + "</tr>"
                                        + "<tr>"
                                        + "<td></td>"
                                        + "<td bgcolor='#B7DEFF' height='100%'></td>"
                                        + "</tr>"
                                        + "<tr>"
                                        + "<td></td>"
                                        + "<td valign='bottom'>"
                                        + "<img src='http://laytontechnology.com/auditwizardv8/right_grad_logo_panel.jpg'></td>"
                                        + "</tr>"
                                        + "</table>";

        public const string SupportContractIntroducer =
                                          "<TR>"
                                        + "<TD></TD>"
                                        + "<TD colSpan=3><BR>Below is a list of Application Support Contract Alerts Generated for Today:<BR><BR></TD>"
                                        + "<TD></TD>"
                                        + "</TR>";

        public const string SupportContractIntroducerNone =
                                          "<TR>"
                                        + "<TD></TD>"
                                        + "<TD colSpan=3><BR>No Support Contract Alerts were found<BR><BR><BR></TD>"
                                        + "<TD></TD>"
                                        + "</TR>";

        public const string AlertIntroducer =
                                          "<TR>"
                                        + "<TD></TD>"
                                        + "<TD colSpan=3><BR>Below is a list of AlertMonitor Alerts Recently Generated :<BR><BR></TD>"
                                        + "<TD></TD>"
                                        + "</TR>";

        public const string AlertIntroducerNone =
                                          "<TR>"
                                        + "<TD></TD>"
                                        + "<TD colSpan=3><BR>No AlertMontitor Alerts were found<BR><BR></TD>"
                                        + "<TD></TD>"
                                        + "</TR>";

        // This is the HTML code for the disclaimer which appears at the bottom of the email
        public const string Disclaimer = "<p>" + DataStrings.Disclaimer + "</p>";
    }
}
