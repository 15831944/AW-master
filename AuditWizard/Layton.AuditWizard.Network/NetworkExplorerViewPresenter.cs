using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    public class NetworkExplorerViewPresenter
    {
        private NetworkExplorerView explorerView;

		/// <summary>
		/// This is the most important piece of data within the whole of the Network View - this is the 
		/// master item which contains the entirety of the Asset Groups and Assets contained within the database
		/// 
		/// It is built up as we go but at any one time will show a map of all of the groups and assets which are
		/// displayed in this View
		/// </summary>
		private AssetGroup _rootAssetGroup = null;
		//
		public AssetGroup RootAssetGroup
		{ get { return _rootAssetGroup; } }

        [InjectionConstructor]
        public NetworkExplorerViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { explorerView = (NetworkExplorerView) value; }
        }

        public void Initialize()
        {
			InitializeNetworkView();
        }



		/// <summary>
		/// This function is called to (re)initialize the network explorer view 
		/// this is a tree control defined within NetworkExplorerView
		/// </summary>
        private void InitializeNetworkView()
        {
            // clear the existing view
            explorerView.Clear();

			// Are we displaying the computers grouped into domains or user locations - this is held as a flag
			// in the work item controller
			NetworkWorkItemController wiController = explorerView.WorkItem.Controller as NetworkWorkItemController;
			bool showByDomain = wiController.DomainViewStyle;

			// First of all we need to create the root group (domain or location)
			LocationsDAO lwDataAccess = new LocationsDAO();
			AssetGroup.GROUPTYPE displayType = (showByDomain) ? AssetGroup.GROUPTYPE.domain : AssetGroup.GROUPTYPE.userlocation;
			DataTable table = lwDataAccess.GetGroups(new AssetGroup(displayType));
			_rootAssetGroup = new AssetGroup(table.Rows[0], displayType);

			// Add the root node to the tree first
			UltraTreeNode rootNode = new UltraTreeNode("root|" + _rootAssetGroup.FullName, _rootAssetGroup.Name);
            Bitmap rootImage = (showByDomain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
            rootNode.Override.NodeAppearance.Image = rootImage;
            rootNode.Override.ExpandedNodeAppearance.Image = rootImage;
			rootNode.Tag = _rootAssetGroup;

			// Set the root node in the Explorer view - note that this will automatically expand the node which will
			// cause it to be populated
			explorerView.RootNode = rootNode;
        }
    }
}
