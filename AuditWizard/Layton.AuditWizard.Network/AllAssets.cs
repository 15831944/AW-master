using System;
using System.Collections.Generic;
using System.Text;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	public class AllAssets
	{
		#region data
		
		public enum eNodeType { root, category ,item_value ,value }
		
		/// <summary>This lets us know the location / asset group that we are under</summary>
		private AssetGroup	_parentGroup;
		
		/// <summary>This field indicates the type of node within All Assets that this is</summary>
		private eNodeType	_nodeType;
		
		/// <summary>This is the name of the branch being populated (below All Assets)</summary>
		private string		_branchName;
		
		/// <summary>This is the ID of the asset for whom a value is stored</summary>
		private AssetList	_listAssets = new AssetList();
		
		/// <summary>This is the value stored for this item for this asset</summary>
		private string		_itemValue;
		
		private object		_tag;
		
		#endregion data
		
		#region Properties

		public AssetGroup ParentGroup
		{
			get { return _parentGroup; }
			set { _parentGroup = value; }
		}

		public eNodeType AllAssetType
		{
			get { return _nodeType; }
			set { _nodeType = value; }
		}

		public string BranchName
		{
			get { return _branchName; }
			set { _branchName = value; }
		}

		public string ItemValue
		{
			get { return _itemValue; }
			set { _itemValue = value; }
		}

		public AssetList ListAssets
		{
			get { return _listAssets; }
		}

		public object	Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
		
		#endregion Properties
		
		#region Constructor

		public AllAssets(AssetGroup parentGroup)
		{
			_parentGroup = parentGroup;
			_nodeType = eNodeType.root;
			_branchName = "";
			_itemValue = "";
			_tag = null;
		}

		public AllAssets(AssetGroup parentGroup, eNodeType nodeType)
		{
			_parentGroup = parentGroup;
			_nodeType = nodeType;
			_branchName = "";
			_itemValue = "";
			_tag = null;
		}

		public AllAssets(AssetGroup parentGroup, string branchName)
		{
			_parentGroup = parentGroup;
			_nodeType = eNodeType.category;
			_branchName = branchName;
			_itemValue = "";
			_tag = null;
		}

		public AllAssets(AssetGroup parentGroup, int assetID ,string itemValue)
		{
			_parentGroup = parentGroup;
			_nodeType = eNodeType.value;
			_branchName = "";
			_itemValue = itemValue;
			_tag = null;
		}
		
		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="other"></param>
		public AllAssets(AllAssets other)
		{
			_parentGroup = other.ParentGroup;
			_nodeType = other.AllAssetType;
			_branchName = other.BranchName;
			_itemValue = other.ItemValue;
			_listAssets = other.ListAssets;
			_tag = other.Tag;
		}
		
		#endregion Constructor

		#region Methods

		public void AddAsset (int assetID)
		{
		
		}
		#endregion Methods
	}
}
