using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
//
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsWorkItem : Layton.Cab.Interface.LaytonWorkItem
    {
		public virtual ILaytonView GetActiveTabView()
		{
			return (ILaytonView)this.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
		}

    }
}
