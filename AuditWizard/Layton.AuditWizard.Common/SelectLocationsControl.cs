//=========================================================================================================
//
//  30-May-2013			Chris Drew		AuditWizard 8.4.1
//
//	Request 40612 / Problem 181 - These requests relate to a performance issue / memory exception
//	when dealing with large databases.  The problem was caused by this control loading all locations
//	and all assets into the selection tree on load.  This was very inefficient as in the case of 40612
//	(Find Asset), assets could never be selected and as such are not required in the tree and in the
//  case of Problem 181 (Report Select Location) it is likely that the user will select locations/groups 
//	rather than assets.
//
//	As such the changes are 2-fold.  Firstly add flags to prevent the control loading top level assets 
//  and a flag for child assets, note that child assets are however NEVER pre-loaded into the tree.
//
//  Secondly the control has been changed to support 'Load on Demand' if child assets are required.  This
//	means that the child assets are only loaded into the tree if requested by expanding their parent group.
//  This has had a dramatic effect on performance as only groups are loaded as the control loads.
//
//=========================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class SelectLocationsControl : UserControl
	{
		#region Data

		private bool _showByDomain = false;
        private AssetGroup _rootAssetGroup = new AssetGroup();
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StringBuilder selectedItems;

		#endregion Data

		#region Properties
		public bool ShowByDomain
		{
			get { return _showByDomain; }
			set { _showByDomain = value; }
		}
		#endregion Properties

		#region Constructor

		public SelectLocationsControl()
		{
			InitializeComponent();
		}

#endregion Constructor

		#region Form Control

		private void SelectLocationsControl_Load(object sender, EventArgs e)
		{
		}

		#endregion Form Control

		#region Methods

		public void Populate(bool showByDomain, bool ignoreTopAssets, bool ignoreChildAssets)						// CMD 8.4.2 Include additional parameter
		{
			LocationsDAO lwDataAccess = new LocationsDAO();
			AssetGroup.GROUPTYPE displayType = (_showByDomain) ? AssetGroup.GROUPTYPE.domain : AssetGroup.GROUPTYPE.userlocation;
			DataTable table = lwDataAccess.GetGroups(new AssetGroup(displayType));
			_rootAssetGroup = new AssetGroup(table.Rows[0], displayType);
			
			// +CMD 8.4.1 - Changes required to prevent performance issues with large databases.  We can now elect to not display assets and also
			// the tree is load on demand if we do require assets so that we only load them if required.
			//
			// If we need to show assets at the root then populate this now
			if (!ignoreTopAssets)
				_rootAssetGroup.Populate(false, false, true);

			// Populate all levels for this group beneath this group again optionally including assets
			_rootAssetGroup.Populate(true, true, true);		

			// Load on demand is active if we are displaying child assets
			locationsTree.Override.ShowExpansionIndicator = (ignoreChildAssets) ? ShowExpansionIndicator.CheckOnDisplay : ShowExpansionIndicator.CheckOnExpand;

			// -CMD 8.4.1
			// begin updating the tree
			locationsTree.BeginUpdate();
			
			// Add the root node to the tree first
			try
			{
				UltraTreeNode rootNode = new UltraTreeNode(null, _rootAssetGroup.Name);
				Bitmap rootImage = (_showByDomain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
				rootNode.Override.NodeAppearance.Image = rootImage;
				rootNode.Override.ExpandedNodeAppearance.Image = rootImage;
				rootNode.Tag = _rootAssetGroup;
				rootNode.CheckedState = CheckState.Unchecked;

				// ...add the children to the tree beneath it now
				AddChildNodes(rootNode, _rootAssetGroup);

				// Set the root node in the Explore view 
				locationsTree.Nodes.Add(rootNode);

				// Expand the root node
				rootNode.Expanded = true;
			}
			catch (Exception ex)
			{
				//MessageBox.Show("Exception adding tree nodes in [SelectLocationsControl::AddApplicationNode], the exception text was " + ex.Message);
                logger.Error(ex.Message);
			}

			// Finished updating the tree			
			locationsTree.EndUpdate();
		}

		
		/// <summary>
		/// Add the children of this AssetGroup into the tree
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="parentGroup"></param>
		protected void AddChildNodes (UltraTreeNode parentNode, AssetGroup parentGroup)
		{
			// first add child groups
			foreach (AssetGroup group in parentGroup.Groups)
			{
				UltraTreeNode newNode = new UltraTreeNode(parentNode.Key + group.Name, group.Name);
				Bitmap rootImage = (_showByDomain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
				newNode.Override.NodeAppearance.Image = rootImage;
                newNode.Key = "g_" + group.GroupID;
				newNode.Tag = group;
				newNode.CheckedState = CheckState.Unchecked;
				parentNode.Nodes.Add(newNode);
				
				// recurse to add any child nodes beneath this group
				AddChildNodes(newNode ,group);
			}

			// ...then add the assets for this group
            UltraTreeNode[] nodes = new UltraTreeNode[parentGroup.Assets.Count];
            UltraTreeNode node;
            Bitmap image;
            Asset asset;

            for (int i = 0; i < parentGroup.Assets.Count; i++)
            {
                asset = parentGroup.Assets[i];
                node = new UltraTreeNode(null, asset.Name);
                node.Key = "a_" + asset.AssetID.ToString();
                image = (_showByDomain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
                node.Override.NodeAppearance.Image = (asset.StockStatus == Asset.STOCKSTATUS.disposed) ? Properties.Resources.computer_disposed_16 : Properties.Resources.computer16;
                node.Tag = asset;
                node.CheckedState = CheckState.Unchecked;
                nodes[i] = node;
            }

            parentNode.Nodes.AddRange(nodes);
		}

		/// <summary>
		/// This function is called to restore a previously saved set of checked asset groups and assets
		/// </summary>
		/// <param name="selectAll"></param>
		/// <param name="listAssetGroups"></param>
		/// <param name="listAssets"></param>
		public void RestoreSelections (string selectedGroups ,string selectedAssets)
		{
			// begin updating the tree
			locationsTree.BeginUpdate();

            if (selectedGroups == "" && selectedAssets == "")
            {
                locationsTree.Nodes[0].CheckedState = CheckState.Checked;
            }
            else if (selectedGroups != "" && new LocationsDAO().IsRootLocation(Convert.ToInt32(selectedGroups.Split(',')[0])))
            {
                locationsTree.Nodes[0].CheckedState = CheckState.Checked;
            }
            else
            {
                // Is the entire tree checked?
                //bool selectAll = ((selectedGroups == "") && (selectedAssets == ""));
                //locationsTree.Nodes[0].CheckedState = (selectAll) ? CheckState.Checked : CheckState.Unchecked;

                // We iterate through each of the Publishers first
                List<string> listGroups = Utility.ListFromString(selectedGroups, ',', true);

                // We iterate through each of the groups first
                foreach (string checkedGroupId in listGroups)
                {
                    // Find the node containing this group
                    //UltraTreeNode node = FindAssetGroup(locationsTree.Nodes[0], checkedGroup);
                    UltraTreeNode node = locationsTree.GetNodeByKey("g_" + checkedGroupId);
                    if (node != null)
                        node.CheckedState = CheckState.Checked;
                }

                // Now iterate through each of the child assets
                List<string> listAssets = Utility.ListFromString(selectedAssets, ',', true);
                foreach (string checkedAssetId in listAssets)
                {
                    // Find the node containing this asset
                    UltraTreeNode node = locationsTree.GetNodeByKey("a_" + checkedAssetId);
                    if (node != null)
                        node.CheckedState = CheckState.Checked;
                }
            }

			// end updating the tree
			locationsTree.EndUpdate();
		}


		/// <summary>
		/// Search the entire tree looking for the node containing the specified Asset group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		protected UltraTreeNode FindAssetGroup(UltraTreeNode parentNode, string groupToFind)
		{
			foreach (UltraTreeNode childNode in parentNode.Nodes)
			{
				if ((childNode.Tag is AssetGroup) && ((childNode.Tag as AssetGroup).FullName == groupToFind))
					return childNode;

				// Ok not this specific node but check any children of this node also
				UltraTreeNode childNodeFound = FindAssetGroup(childNode, groupToFind);
				if (childNodeFound != null)
					return childNodeFound;
			}

			///Node not found so return null
			return null;
		}



		/// <summary>
		/// Search the entire tree looking for the node containing the specified Asset
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		protected UltraTreeNode FindAsset(UltraTreeNode parentNode, string assetToFind)
		{
			foreach (UltraTreeNode childNode in parentNode.Nodes)
			{
				if ((childNode.Tag is Asset) && ((childNode.Tag as Asset).Name == assetToFind))
					return childNode;

				// Ok not this specific node but check any children of this node also
				UltraTreeNode childNodeFound = FindAsset(childNode, assetToFind);
				if (childNodeFound != null)
					return childNodeFound;
			}

			///Node not found so return null
			return null;
		}

        public bool GetSelectedItemsAlertMonitor(out AssetGroupList listSelectedGroups, out AssetList listSelectedAssets)
        {
            // Allocate the lists that we will return
            listSelectedGroups = new AssetGroupList();
            listSelectedAssets = new AssetList();

            // Are all nodes selected as this is SO much easier to handle as we just return TRUE to indicate that
            // all nodes have been selected - the return lists are empty
            UltraTreeNode rootNode = locationsTree.Nodes[0];

            // OK unfortunately not all nodes are selected so we now need to populate the lists based on what has 
            // been selected.  Note that if a node is checked then by definition ALL assets and locations beneath that
            // node are deemed to be checked but we do not bother iterating them
            GetSelectedItems(rootNode, listSelectedGroups, listSelectedAssets);

            // return false to show that there is a selection
            return false;
        }

        /// <summary>
        /// Return lists of all groups and assets selected in the tree
        /// </summary>
        /// <param name="listSelectedGroups">return list of groups selected</param>
        /// <param name="listSelectedAssets">return list of assets selected</param>
        /// <returns>true if all nodes selected, false if specific nodes selected and returned in lists</returns>
        /// 
		public StringBuilder GetSelectedGroups()
        {
            UltraTreeNode rootNode = locationsTree.Nodes[0];
            selectedItems = new StringBuilder();
	
			// We only need to actually check anything if the check state of the root is indeterminate as it does not make sense 
			// to have nothing selected so we assume all in that case and if checked then all are selected
			if (rootNode.CheckedState == CheckState.Indeterminate)
				GetSelectedGroups(rootNode);

            return selectedItems;
        }


		/// <summary>
		/// Recursive routine to traverse the locations tree and build up a list of the Asset Groups and Assets
		/// which are checked
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="listSelectedGroups"></param>
		/// <param name="listSelectedAssets"></param>
		protected void GetSelectedGroups(UltraTreeNode parentNode)
		{
			AssetGroup assetGroup = parentNode.Tag as AssetGroup;

			// If this node is unchecked then all children are also unchecked
			if (parentNode.CheckedState == CheckState.Unchecked)
			{
				return;
			}


			// Indeterminate means that some children will be checked and some won't - iterate through the 
			// groups first by calling ourselves recursively for each child group
			else if (parentNode.Tag is AssetGroup)
			{
				// This is an asset group - we only add if it is CHECKED (not if indeterminate)
				if (parentNode.CheckedState == CheckState.Checked)
				{
					AssetGroup group = parentNode.Tag as AssetGroup;
					selectedItems.Append(group.GroupID + ";");
				}

				// But in all cases (CHECKED OR INDETERMINATE) we add child groups
				foreach (UltraTreeNode childNode in parentNode.Nodes)
				{
					// recurse if this child is a n Asset Group
					if (childNode.Tag is AssetGroup)
					{
						GetSelectedGroups(childNode);
					}
				}
			}
		}


		/// <summary>
		/// Return lists of all groups and assets selected in the tree
		/// </summary>
		/// <param name="listSelectedGroups">return list of groups selected</param>
		/// <param name="listSelectedAssets">return list of assets selected</param>
		/// <returns>true if all nodes selected, false if specific nodes selected and returned in lists</returns>
		/// 
		public bool	GetSelectedItems (out AssetGroupList listSelectedGroups ,out AssetList listSelectedAssets)
		{
			// Allocate the lists that we will return
			listSelectedGroups = new AssetGroupList();
			listSelectedAssets = new AssetList();
			
			// Are all nodes selected as we just return TRUE to indicate that
			// all nodes have been selected - the return lists are empty
			UltraTreeNode rootNode = locationsTree.Nodes[0];

            if (rootNode.CheckedState == CheckState.Checked)						// CMD 8.4.2 UNCOMMENTED
                return true;														// CMD 8.4.2 UNCOMMENTED
		
			// OK unfortunately not all nodes are selected so we now need to populate the lists based on what has 
			// been selected.  Note that if a node is checked then by definition ALL assets and locations beneath that
			// node are deemed to be checked but we do not bother iterating them
			GetSelectedItems(rootNode ,listSelectedGroups ,listSelectedAssets);
			
			// return false to show that there is a selection
			return false;
		}
		
		
		
		/// <summary>
		/// Recursive routine to traverse the locations tree and build up a list of the Asset Groups and Assets
		/// which are checked
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="listSelectedGroups"></param>
		/// <param name="listSelectedAssets"></param>
		protected void GetSelectedItems (UltraTreeNode parentNode ,AssetGroupList listSelectedGroups ,AssetList listSelectedAssets)
		{
			AssetGroup assetGroup = parentNode.Tag as AssetGroup;
			
			// is this node checked?  If so we simply add it to the groups selected and return
			if (parentNode.CheckedState == CheckState.Checked)
			{
				listSelectedGroups.Add(assetGroup);
				return;			
			}
			
			// If this node is unchecked then all children are also unchecked
			else if (parentNode.CheckedState == CheckState.Unchecked)
			{
				return;
			}
			
			// Indeterminate means that some children will be checked and some won't - iterate through the 
			// groups first by calling ourselves recursively for each child group
	
			else 
			{
				foreach (UltraTreeNode childNode in parentNode.Nodes)
				{
					// recurse if this child is a n Asset Group
					if (childNode.Tag is AssetGroup)
					{
						GetSelectedItems(childNode ,listSelectedGroups ,listSelectedAssets);
					}
					
					// or check thge state of an asset
					else if (childNode.Tag is Asset)
					{
						if (childNode.CheckedState == CheckState.Checked)
							listSelectedAssets.Add(childNode.Tag as Asset);
					}
				}
			}
		}
		
		#endregion Methods
		
		#region Tree Control Handlers

		/// <summary>
		/// Called as we select a new node within the tree - we may need to populate the node if we have 
		/// not previously done so
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void networkTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
		{
			UltraTreeNode node = e.TreeNode;
			ExpandNode(node);
		}

		/// <summary>
		/// Expand a node within the tree
		/// </summary>
		/// <param name="node"></param>
		public void ExpandNode(UltraTreeNode expandingNode)
		{
			// Nothing to do if already expanded
			if (expandingNode.HasNodes == true)
				return;

			//	Call BeginUpdate to prevent drawing while we are populating the control
			this.locationsTree.BeginUpdate();
			this.Cursor = Cursors.WaitCursor;

			// get the asset group and populate it
			AssetGroup expandingGroup = expandingNode.Tag as AssetGroup;
			expandingGroup.Populate(false ,false ,true);

			// Add the top level groups (and assets if available) to the tree
			foreach (AssetGroup group in expandingGroup.Groups)
			{
				UltraTreeNode childNode = new UltraTreeNode(null, group.Name);

				// Set the correct image for this group type
				Bitmap image = (group.GroupType == AssetGroup.GROUPTYPE.domain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
				childNode.LeftImages.Add(image);
				
				// Set the check state of this child to mirror that of the parent
				//childNode.Override.NodeStyle = NodeStyle.CheckBoxTriState;
				childNode.CheckedState = expandingNode.CheckedState;
				
				// Set the tag for the node to be the AssetGroup object
				childNode.Tag = group;

				// ...and add the node to the tree
				expandingNode.Nodes.Add(childNode);
			}

			// Add any assets which have been defined outside of groups to the tree also
			foreach (Asset asset in expandingGroup.Assets)
			{
				// Create the new tree node for this asset
				UltraTreeNode childNode = new UltraTreeNode(null, asset.Name);
				childNode.LeftImages.Add(Properties.Resources.computer16);

				// Set the check state of this child to mirror that of the parent 
				childNode.Override.NodeStyle = NodeStyle.CheckBox;
				childNode.CheckedState = expandingNode.CheckedState;
				childNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
				
				// Set the tag for assets to be the asset
				childNode.Tag = asset;
				expandingNode.Nodes.Add(childNode);
			}

			//	Restore the cursor and finish updating the tree
			this.Cursor = Cursors.Default;
			this.locationsTree.EndUpdate(true);
		}




		/// <summary>
		/// We do not want to allow the user to explicitely set the check state to indeterminate
		/// as this is a pre-set state based on the state of child items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void locationsTree_BeforeCheck(object sender, BeforeCheckEventArgs e)
		{
			if (e.NewValue == CheckState.Indeterminate)
				e.Cancel = false;
		}



		/// <summary>
		/// Handles the UltraTree's AfterCheck event.
		/// We need to propogate the change to our children and parents as necessary
		/// </summary>
		private void locationsTree_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
		{
			// Disable checking node states while we play around with them
			locationsTree.AfterCheck -= locationsTree_AfterCheck;
			
			// Update check state for parents / children of this node
			UpdateStates(e.TreeNode);

			// Update our parent(s) state
			VerifyParentNodeCheckState(e.TreeNode);
		
			// Re-establish the AfterCheck event
			locationsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.locationsTree_AfterCheck);
		}


		/// <summary>
		/// Called as we change the state of a node - we must propogate this state down to all
		/// of our children
		/// </summary>
		/// <param name="forNode"></param>
		private void UpdateStates(UltraTreeNode forNode)
		{
			// Set out children to the same state as we are propogating the change down the line
			foreach (UltraTreeNode childNode in forNode.Nodes)
			{
				childNode.CheckedState = forNode.CheckedState;
				UpdateStates(childNode);
			}
		}
		
		
		
		/// <summary>
		/// Verify the check state of the parent9s) of the specified child node
		/// </summary>
		/// <param name="childNode"></param>
		private void VerifyParentNodeCheckState(UltraTreeNode childNode)
		{
			// get the parent node and return if it is null (top of tree)
			UltraTreeNode parentNode = childNode.Parent;
			if (parentNode == null)
				return;
				
			// Save the parent's current state
			CheckState currentState = parentNode.CheckedState;
			CheckState childNewState = childNode.CheckedState;
			bool notifyParent = false;
						
			// Get the parent state based on the current state of its children
			CheckState newState = GetOverallChildState(parentNode);

			// We must notify our parent if our status is going to change
			notifyParent = (newState != currentState);

			// should we notify the parent? ( has our state changed? )
			if (notifyParent)
			{
				// change state
				parentNode.CheckedState = newState;
				if (parentNode.Parent != null)
					VerifyParentNodeCheckState(parentNode);
			}
		}



		/// <summary>
		/// Traverse the children beneath the specified node and check their individual state
		/// If ALL are checked then the return state is checked
		/// If ALL are unchecked then the return state is unchecked
		/// Otherwise return state is indeterminate
		/// </summary>
		/// <param name="parentNode"></param>
		/// <returns></returns>
		private CheckState GetOverallChildState (UltraTreeNode parentNode)
		{
			CheckState returnState;
			
			// Get the child count
			int childCount = parentNode.Nodes.Count;
			
			// Sanity check - we should never get here with no children but who knows
			if (childCount == 0)
				returnState = CheckState.Indeterminate;
				
			else if (childCount == 1)
				returnState = parentNode.Nodes[0].CheckedState;
			
			else
			{
				// Start off with the check state of the first child	
				returnState = parentNode.Nodes[0].CheckedState;

				// ...and loop through the rest
				for (int index=1; index<parentNode.Nodes.Count; index++)
				{
					if (returnState != parentNode.Nodes[index].CheckedState)
					{
						returnState = CheckState.Indeterminate;
						break;
					}
				}
			}
			
			return returnState;		
		}


		#endregion Tree Control Handlers

	}
}
