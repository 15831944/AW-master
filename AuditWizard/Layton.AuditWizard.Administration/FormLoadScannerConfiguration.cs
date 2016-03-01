using System;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Administration
{
	public partial class FormLoadScannerConfiguration : ShadedImageForm
	{
		private AuditScannerDefinition _selectedConfiguration = null;
        //private string _fileName = null;

		public AuditScannerDefinition SelectedConfiguration
		{ get { return _selectedConfiguration; } }

        public string FileName { get; set; }

        private bool _isAuditAgent;
			
		public FormLoadScannerConfiguration()
		{
			InitializeComponent();
		}

        public FormLoadScannerConfiguration(bool isAuditAgent)
        {
            InitializeComponent();
            _isAuditAgent = isAuditAgent;
        }

        public FormLoadScannerConfiguration(bool isAuditAgent, string descriptionText)
        {
            InitializeComponent();
            _isAuditAgent = isAuditAgent;
            lblDescription.Text = descriptionText;
        }

		/// <summary>
		/// Called as the form is loaded to recover all scanner configurations and display them in the list box
		/// We now read the scanner definitions from the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormLoadScannerConfiguration_Load(object sender, EventArgs e)
		{
			try
			{
                // Get the path to the scanner configurations
			    string scannerPath = Path.Combine(Application.StartupPath, _isAuditAgent ? @"scanners\\auditagent" : @"scanners");

			    DirectoryInfo di = new DirectoryInfo(scannerPath);
				FileInfo[] rgFiles = di.GetFiles("*.xml");
				foreach (FileInfo fi in rgFiles)
				{
					// Try and read the file as a scanner definition file, if we fail skip this file
					AuditScannerDefinition configuration = new AuditScannerDefinition();
					string scannerName = fi.Name.Replace(".xml", "");
					configuration.ScannerName = scannerName;

                    if (_isAuditAgent)
                        configuration.Filename = Path.Combine(Application.StartupPath, @"scanners\auditagent\" + configuration.ScannerName + ".xml");

					// OK this is a valid scanner configuration file so add it to our list
					lbScanners.Items.Add(configuration);
				}
			}

			catch (Exception)
			{
			}
		}

		private void lbScanners_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = lbScanners.SelectedIndex;

			if (selectedIndex != -1)
			{
                AuditScannerDefinition configuration = AuditWizardSerialization.DeserializeObject(((AuditScannerDefinition)(lbScanners.SelectedItem)).Filename);

				tbDescription.Text = configuration.Description;
				bnOK.Enabled = true;
				bnDelete.Enabled = (lbScanners.Items.Count > 1);
			}
			else
			{
				tbDescription.Text = "";
				bnOK.Enabled = false;
			}
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			int selectedIndex = this.lbScanners.SelectedIndex;
            if (selectedIndex != -1)
            {
                _selectedConfiguration = this.lbScanners.Items[selectedIndex] as AuditScannerDefinition;
                this.FileName = _selectedConfiguration.Filename;
            }
		}

		private void bnDelete_Click(object sender, EventArgs e)
		{
			if (lbScanners.SelectedIndex != -1)
			{
				AuditScannerDefinition configuration = this.lbScanners.Items[lbScanners.SelectedIndex] as AuditScannerDefinition;
				if (MessageBox.Show("Are you sure that you want to delete this scanner configuration?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					configuration.Delete();
					lbScanners.Items.Remove(configuration);
				}
			}
		}
	}
}