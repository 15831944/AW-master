using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    public class SummaryTabViewPresenter
    {
        private SummaryTabView tabView;
        UltraTree tree;

        public UltraTree Tree
        {
            get { return tree; }
            set { tree = value; }
        }

        [InjectionConstructor]
		public SummaryTabViewPresenter()
        {
            tree = new UltraTree();
        }

        public ILaytonView View
        {
			set { tabView = (SummaryTabView)value; }
        }

        public void Initialize()
        {
            InitializeTabView();
			Show(null);
        }


		/// <summary>
		/// Called to show summary details for a specific computer
		/// </summary>
		public void Show(UltraTreeNode displayedNode)
        {
			// Initialize the tab view
            InitializeTabView();
            if (displayedNode != null)
            {
				Asset theComputer = displayedNode.Tag as Asset;
                tabView.Tree = tree;
				tabView.Display(theComputer);
                
			}
		}

        private void InitializeTabView()
        {
        }
    }
}
