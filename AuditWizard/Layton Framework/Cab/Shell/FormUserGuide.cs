using System.Windows.Forms;

namespace AuditWizardv8
{
    public partial class FormUserGuide : Form
    {
        public FormUserGuide()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel linkLabel = sender as LinkLabel;

            if (linkLabel != null)
            {
                System.Diagnostics.Process.Start(linkLabel.Text);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://laytonsupport.com/index.php?title=AuditWizard-Release_Notes");
        }
    }
}
