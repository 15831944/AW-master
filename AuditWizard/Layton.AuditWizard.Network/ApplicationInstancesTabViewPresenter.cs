using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.ObjectBuilder;
//
using Layton.AuditWizard.Common;
using Layton.Cab.Interface;
//
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;

namespace Layton.AuditWizard.Network
{
    public class ApplicationInstancesTabViewPresenter
    {
        private UltraTreeNode _displayedNode = null;
        private ApplicationInstancesTabView tabView;
		TreeSelectionEventArgs.ITEMTYPE _itemType;
		
        [InjectionConstructor]
        public ApplicationInstancesTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (ApplicationInstancesTabView) value; }
        }


		/// <summary>
		/// Initialization function for this tab view
		/// </summary>
        public void Initialize()
        {
			// First initialize the tab view
            InitializeTabView();

			// ...then display the applications located for the currently selected node
			Show(_displayedNode ,_itemType);
        }


		/// <summary>
		/// Initialize the Applications Tab View - this simply clears the view
		/// </summary>
		private void InitializeTabView()
		{
			// clear the existing view
			tabView.Clear();
		}

        /// <summary>
        /// The node currently being displayed.
        /// </summary>
        public UltraTreeNode  DisplayedNode
        {
			get { return _displayedNode; }
			set { _displayedNode = value; }
        }

        /// <summary>
        /// Controls the information that is displayed for a computer.
        /// </summary>
        /// <param name="computerName">Name of the computer to display.</param>
		public void Show(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType)
        {
			// Clear the existing data
			InitializeTabView();

			// Save data passed in to us
			_displayedNode = displayedNode;
			_itemType = itemType;
			
			// ...and add in the child groups and children
			tabView.Display(displayedNode, itemType);
		}
	}
}