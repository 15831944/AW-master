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
    public class ComputerTabViewPresenter
    {
        private ComputerTabView tabView;
        private UltraTreeNode _displayedNode;

        [InjectionConstructor]
		public ComputerTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { tabView = (ComputerTabView)value; }
        }

        public void Initialize()
        {
			InitializeTabView();
			Show(_displayedNode);
        }


		/// <summary>
		/// Called to show Asset details for a specific computer
		/// </summary>
		public void Show(UltraTreeNode displayedNode)
        {
			_displayedNode = displayedNode;
			
			// Initialize the tab view
            InitializeTabView();

			// Get the asset that we are displaying
			Asset asset = displayedNode.Tag as Asset;
			tabView.Display(asset);
		}

        private void InitializeTabView()
        {
        }
    }
}
