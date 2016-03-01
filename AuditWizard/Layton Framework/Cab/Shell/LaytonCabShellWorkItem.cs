using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Infragistics.Practices.CompositeUI.WinForms;
using Layton.Cab.Interface;
using System.Configuration;
using System.Windows.Forms;

namespace AuditWizardv8
{
    public class LaytonCabShellWorkItem : WorkItem
    {
        private LaytonFormShell formShell;
        public LaytonCabShellWorkItem()
        {
        }

        public LaytonFormShell Shell
        {
            get { return formShell; }
        }

        public void Show(LaytonFormShell shell)
        {
            this.formShell = shell;

            LaytonSettingsWorkItem settingsWorkItem = this.WorkItems.AddNew<LaytonSettingsWorkItem>(WorkItemNames.SettingsWorkItem);
            settingsWorkItem.Show();
            Workspaces.Add(settingsWorkItem.TabWorkspace, Layton.Cab.Interface.WorkspaceNames.SettingsTabWorkspace);

            LaytonToolbarsWorkItem toolbarsWorkItem = this.WorkItems.AddNew<LaytonToolbarsWorkItem>(WorkItemNames.ToolbarsWorkItem);
            toolbarsWorkItem.Show();

            LaytonProductKey productKey = LoadProductKey();
            if (productKey.IsTrial)
            {
                LaytonRegistrationForm registerForm = new LaytonRegistrationForm(productKey);
                registerForm.ShowDialog();
                productKey = registerForm.ProductKey;
            }

            this.Items.Add(productKey, MiscStrings.ProductKey);
            this.Activate();
        }

        public LaytonProductKey LoadProductKey()
        {
            // get the product key and installation settings
            string key = "";
            int productID = 0;
            string companyName = "";
            int companyID = 0;
            string installDateString = "";
            try
            {
                key = ConfigurationManager.AppSettings["ProductKey"].ToString();
            }
            catch
            {

            }
            try
            {
                productID = Convert.ToInt32(ConfigurationManager.AppSettings["ProductID"].ToString());
            }
            catch
            {

            }
            try
            {
                companyName = ConfigurationManager.AppSettings["CompanyName"].ToString();
            }
            catch
            {

            }
            try
            {
                companyID = Convert.ToInt32(ConfigurationManager.AppSettings["CompanyID"].ToString());
            }
            catch
            {

            }
            try
            {
                installDateString = ConfigurationManager.AppSettings["Code1"].ToString();
            }
            catch
            {

            }
            if (installDateString == "")
            {
                installDateString = LaytonProductKey.GenerateCode2();
                Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                config.AppSettings.Settings["Code1"].Value = installDateString;
                config.Save();
            }

            // Create the ProductKey object
            return new LaytonProductKey(key, productID, companyName, companyID, installDateString);
        }
    }

}
