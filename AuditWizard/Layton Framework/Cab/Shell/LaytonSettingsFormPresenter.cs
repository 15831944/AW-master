using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.Utility;
using Infragistics.Win.UltraWinTabControl;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace AuditWizardv8
{
    public class LaytonSettingsFormPresenter
    {
        LaytonSettingsFormShell formShell;
        WorkItem workItem;
        
        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public LaytonSettingsFormPresenter([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem;
        }

        public LaytonSettingsFormShell View
        {
            get { return formShell; }
            set { formShell = value; }
        }

        public void Initialize()
        {
            workItem.Activated += new EventHandler(workItem_Activated);
            formShell.FormClosed += new System.Windows.Forms.FormClosedEventHandler(formShell_FormClosed);
        }

        void workItem_Activated(object sender, EventArgs e)
        {
            foreach (UltraTab tab in View.TabWorkspace.Tabs)
            {
                View.AddListItem(tab.Appearance.Image as System.Drawing.Image, tab.Text, tab);
            }

            View.ShowDialog();
            workItem.Deactivate();
        }

        void formShell_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (SettingsFormClosed != null)
                SettingsFormClosed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the <see cref="SettingsFormShell"/> is closed by the user.
        /// </summary>
        [EventPublication(EventTopics.SettingsFormClosed, PublicationScope.Global)]
        public event EventHandler SettingsFormClosed;
    }
}
