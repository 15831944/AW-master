using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace AuditWizardv8
{
    internal class LaytonToolbarsWorkItem : WorkItem
    {
        private LaytonToolbarsController toolbarsController;

        public LaytonToolbarsWorkItem()
        { 
        }

        public void Show()
        {
            toolbarsController = this.Items.AddNew<LaytonToolbarsController>(ControllerNames.ToolbarsController);
        }
    }
}
