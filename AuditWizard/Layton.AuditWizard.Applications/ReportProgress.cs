using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.Applications
{
    public partial class ReportProgress : Form
    {
        public ReportProgress()
        {
            InitializeComponent();
        }

        private void FormProgress_Load(object sender, EventArgs e)
        {
            // Start the worker thread to perform tha ctual task
            backgroundWorker1.RunWorkerAsync();            
        }
    }
}
