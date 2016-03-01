using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Network
{
	/// <summary>
	/// The AuditedDataTabViewPresenter object handles the presentation of the data which is displayed within
	/// the AuditedDataTabView.  
	/// 
	/// </summary>
    public class AuditedDataTabViewPresenter
    {
		/// <summary>
		/// Our parent tab view
		/// </summary>
        private AuditedDataTabView tabView;

		/// <summary>
		/// The current Node being displayed by this object
		/// </summary>
		UltraTreeNode _displayedNode = null;
		TreeSelectionEventArgs.ITEMTYPE _itemType;
		
        [InjectionConstructor]
        public AuditedDataTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (AuditedDataTabView) value; }
        }


		/// <summary>
		/// Called to (re)initialize the display.  This function updates the data displayed by the main
		/// AuditedDataTabView to reflect the currently selected item in the NetworkExplorerView
		/// </summary>
        public void Initialize()
        {
			// Perform generic initialization of the tab view
			InitializeTabView();

			// Display the current group if any
			if (_displayedNode != null)
				tabView.Display(_displayedNode, _itemType);
        }


		/// <summary>
		/// this is the main display function called to display expanded information pertaining to the specified 
		/// group.  We need to display the sub-groups and assets within this group
		/// </summary>
		/// <param name="group"></param>
		public void Show(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType)
		{
			_displayedNode = displayedNode;
			_itemType = itemType;

			// Initialize the tab view
			InitializeTabView();

			// ...and add in the child groups and children
			tabView.Display(displayedNode ,itemType);
		}


        private void InitializeTabView()
        {
            // clear the existing view
            tabView.Clear();
        }
   }
}
