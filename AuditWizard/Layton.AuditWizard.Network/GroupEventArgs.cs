using System;
using System.Collections.Generic;
using System.Text;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
//
using Infragistics.Win.UltraWinTree;

namespace Layton.AuditWizard.Network
{
	public class TreeSelectionEventArgs : EventArgs
	{
		public enum ITEMTYPE { 
							   assetgroup							// A Group / Location is selected
							 , asset								// An Asset is selected
							 , asset_summary						// Asset Summary is selected
							 , asset_applications					// Asset > All Applications Selected
							 , asset_auditdata_category				// Asset audited data category selected
							 , asset_auditdata						// Asset audited data field selected
						     , asset_userdata						// Asset user data field selected
							 , asset_history						// Asset history record selected
							 , asset_publisher						// Application Publisher selected
							 , asset_application					// Individual application selected
							 , asset_os								// OS Family field selected
							 , asset_filesystem						// File System selected
							 , allassets							// 'All Assets' branch selected
							}
							
		private ITEMTYPE _itemType;
		private UltraTreeNode _selectedNode;

		public TreeSelectionEventArgs(ITEMTYPE itemType, UltraTreeNode selectedNode)
		{
			_itemType = itemType;
			_selectedNode = selectedNode;
		}

		public ITEMTYPE ItemType
		{ get { return _itemType; } }

		public object SelectedNode
		{ get { return _selectedNode; } }
	}

	
	public class ViewStyleEventArgs : EventArgs
	{
		private bool _showByDomain;
		
		public bool ShowByDomain
		{ 
			get { return _showByDomain; }
			set { _showByDomain = value; }
		}

		public ViewStyleEventArgs(bool showByDomain)
		{
			_showByDomain = showByDomain;
		}
	}


	public class GroupEventArgs : EventArgs
	{
		private List<AssetGroup> groups;

		public GroupEventArgs(List<AssetGroup> value)
		{
			groups = value;
		}

		public List<AssetGroup> ComputerGroups
		{
			get { return groups; }
		}
	}

}
