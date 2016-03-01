using System;
using System.Collections;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
    public partial class FormEnvVarBrowser : AWForm
    {
        public string EnvVariable { get; set; }

        public FormEnvVarBrowser()
        {
            InitializeComponent();

            foreach (DictionaryEntry dictionaryEntry in Environment.GetEnvironmentVariables())
            {
                lvEnvVar.Items.Add(dictionaryEntry.Key.ToString());
            }
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            EnvVariable = lvEnvVar.SelectedItems[0].Text;
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lvEnvVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnOk.Enabled = (lvEnvVar.SelectedItems.Count == 1);
        }
    }
}
