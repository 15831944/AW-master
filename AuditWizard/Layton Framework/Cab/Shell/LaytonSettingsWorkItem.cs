using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.Commands;
using System.Configuration;

namespace AuditWizardv8
{
    public class LaytonSettingsWorkItem : WorkItem
    {
        private IWorkspace tabWorkspace;

        public LaytonSettingsWorkItem()
        {
        }

        public IWorkspace TabWorkspace
        {
            get { return tabWorkspace; }
        }

        public void Show()
        {
            LaytonSettingsFormShell settings = this.Items.AddNew<LaytonSettingsFormShell>("settingsFormShell");
            tabWorkspace = settings.TabWorkspace;
            settings.SetPanelSize(Properties.Settings.Default.settingsPanelSize);
        }
    }
}