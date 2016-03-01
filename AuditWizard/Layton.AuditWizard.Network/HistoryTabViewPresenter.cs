using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Network
{
	/// <summary>
	/// The HistoryTabViewPresenter object handles the presentation of the data which is displayed within
	/// the HistoryTabView.  
	/// 
	/// </summary>
    public class HistoryTabViewPresenter
    {
		/// <summary>
		/// Our parent tab view
		/// </summary>
        private HistoryTabView tabView;

		/// <summary>
		/// The current Node being displayed by this object
		/// </summary>
		UltraTreeNode _displayedNode = null;

        [InjectionConstructor]
        public HistoryTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (HistoryTabView) value; }
        }


		/// <summary>
		/// Called to (re)initialize the display.  This function updates the data displayed by the main
		/// HistoryTabView to reflect the currently selected item in the NetworkExplorerView
		/// </summary>
        public void Initialize()
        {
			// Perform generic initialization of the tab view
			InitializeTabView();

			// Display the current group if any
			if (_displayedNode != null)
				tabView.Display(_displayedNode);
        }


		/// <summary>
		/// This is the main display function called to display the audit data history for the specified asset
		/// </summary>
		/// <param name="group"></param>
		public void Show(UltraTreeNode displayedNode)
		{
			_displayedNode = displayedNode;

			// Initialize the tab view
			InitializeTabView();

			// ...and add in the child groups and children
			tabView.Display(displayedNode);
		}


        private void InitializeTabView()
        {
            // clear the existing view
            tabView.Clear();
        }
   }
}
