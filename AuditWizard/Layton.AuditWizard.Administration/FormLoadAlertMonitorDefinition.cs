using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Layton.AuditWizard.Common;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Administration
{
    public partial class FormLoadAlertMonitorDefinition : Form
    {
        private AlertDefinition _selectedAlertDefinition = null;
        private string _fileName = null;

        public AlertDefinition SelectedAlertDefinition
        { 
            get { return _selectedAlertDefinition; } 
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; } 
        }

        public FormLoadAlertMonitorDefinition()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called as the form is loaded to recover all scanner configurations and display them in the list box
        /// We now read the scanner definitions from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoadAlertMonitorDefintion_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the path to the scanner configurations
                string scannerPath = Path.Combine(Application.StartupPath, "scanners/alertmonitors/");

                string alertMonitorDirectory = Path.Combine(Application.StartupPath, "scanners/alertmonitors/");

                DirectoryInfo di = new DirectoryInfo(scannerPath);
                FileInfo[] rgFiles = di.GetFiles("*.xml");
                foreach (FileInfo fi in rgFiles)
                {
                    lbAlertMonitors.Items.Add(fi.Name.Replace(".xml", ""));
                }
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.lbAlertMonitors.SelectedIndex;
            if (selectedIndex != -1)
            {
                _fileName = this.lbAlertMonitors.Items[selectedIndex].ToString();
            }
        }

        private void lbAlertMonitors_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.lbAlertMonitors.SelectedIndex;
            if (selectedIndex != -1)
            {
                bnOK.Enabled = true;
            }
            else
            {
                bnOK.Enabled = false;
            }
        }
    }
}
