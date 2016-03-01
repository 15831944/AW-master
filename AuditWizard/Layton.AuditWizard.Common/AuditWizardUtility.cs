using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public class AuditWizardUtility
	{
		public static UltraTreeNode CreateKeyedTreeNode(UltraTreeNode parentNode, string newNodeName)
		{
			if (parentNode == null)
				return new UltraTreeNode(newNodeName, newNodeName);
			else
				return new UltraTreeNode(String.Format("{0}|{1}", parentNode.Key, newNodeName), newNodeName);
		}

		public static UltraTreeNode CreateKeyedTreeNode(string parentKey, string newNodeName)
		{
			return new UltraTreeNode(String.Format("{0}|{1}", parentKey, newNodeName), newNodeName);
		}

		public static UltraTreeNode CreateKeyedTreeNode(UltraTreeNode parentNode, Asset forAsset)
		{
            string key = String.Format("{0}|{1}|{2}", forAsset.AssetID, parentNode.Key, forAsset.AssetIdentifier);
            //string key = String.Format("{0}|{1}", parentNode.Key, forAsset.AssetIdentifier);
			return new UltraTreeNode(key, forAsset.Name);
		}

		public static UltraTreeNode CreateKeyedTreeNode(string parentKey, Asset forAsset)
		{
			string key = String.Format("{0}|{1}", parentKey, forAsset.AssetIdentifier);
			return new UltraTreeNode(key, forAsset.Name);
		}
	}
}
