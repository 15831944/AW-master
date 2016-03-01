using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	/// <summary>
	/// The UserDefinedDataTabViewPresenter object handles the presentation of the data which is displayed within
	/// the UserDefinedDataTabView.  
	/// 
	/// </summary>
    public class UserDefinedDataTabViewPresenter
    {
		/// <summary>
		/// Our parent tab view
		/// </summary>
        private UserDefinedDataTabView tabView;

		/// <summary>
		/// The current Node being displayed by this object
		/// </summary>
		UltraTreeNode _displayedNode = null;
		UserDataCategory _displayedCategory = null;

        [InjectionConstructor]
        public UserDefinedDataTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (UserDefinedDataTabView) value; }
        }


		/// <summary>
		/// Called to (re)initialize the display.  This function updates the data displayed by the main
		/// UserDefinedDataTabView to reflect the currently selected item in the NetworkExplorerView
		/// </summary>
        public void Initialize()
        {
			// Perform generic initialization of the tab view
			InitializeTabView();

			// Display the current group if any
			if (_displayedNode != null)
				tabView.Display(_displayedNode, _displayedCategory);
        }


		/// <summary>
		/// this is the main display function called to display expanded information pertaining to the specified 
		/// User Defined Data category - we need to display the fields and values within this category
		/// The User Defined Data Category itself is passed as the subObject - we identify the asset as it is the 
		/// Tag within the Tree Node passed to us
		/// </summary>
		/// <param name="displayedNode">TreeNode which has been selected - its tag will identify the asset</param>
		/// <param name="subObject">This holds the User Defined Data Category that we are to display details of</param>
		public void Show(UltraTreeNode displayedNode ,object subObject)
		{
			_displayedNode = displayedNode;
			_displayedCategory = subObject as UserDataCategory;

			// Initialize the tab view
			InitializeTabView();

			// ...and add in the child groups and children
			tabView.Display(_displayedNode, _displayedCategory);
		}


        private void InitializeTabView()
        {
        }
   }
}
