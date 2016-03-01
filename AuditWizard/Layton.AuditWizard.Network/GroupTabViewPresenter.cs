using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	/// <summary>
	/// The GroupTabViewPresenter object handles the presentation of the data which is displayed within
	/// the GroupTabView.  The GroupTabView itself displays the Groups and computers defined within the
	/// AuditWizard database.
	/// 
	/// The data presentation is passed over to the ComputerTabViewPresenter when we select an individual
	/// computer in the main TabView
	/// 
	/// </summary>
    public class GroupTabViewPresenter
    {
		/// <summary>
		/// Our parent tab view
		/// </summary>
        private GroupTabView tabView;

		/// <summary>
		/// The current Group being displayed by this object
		/// </summary>
        private AssetGroup _currentGroup = null;

        [InjectionConstructor]
        public GroupTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (GroupTabView) value; }
        }


		/// <summary>
		/// Called to (re)initialize the display.  This function updates the data displayed by the main
		/// GroupTabView to reflect the currently selected item in the NetworkExplorerView
		/// </summary>
        public void Initialize()
        {
			// Perform generic initialization of the tab view
			InitializeTabView();

			// Display the current group if any
			if (_currentGroup != null)
				tabView.DisplayGroup(_currentGroup);
        }


		/// <summary>
		/// this is the main display function called to display expanded information pertaining to the specified 
		/// group.  We need to display the sub-groups and assets within this group
		/// </summary>
		/// <param name="group"></param>
		public void Show(AssetGroup group)
		{
			_currentGroup = group;

			// get our controller and from there the current display moderators
			NetworkWorkItemController wiController = tabView.WorkItem.Controller as NetworkWorkItemController;

			// Populate this group if not done so already
			if (!group.Populated)
				group.Populate(false, false ,true);

			// Initialize the tab view
			InitializeTabView();

			// ...and add in the child groups and children
			tabView.DisplayGroup(group);
		}


        private void InitializeTabView()
        {
            // clear the existing view
            tabView.Clear();
        }
   }
}
