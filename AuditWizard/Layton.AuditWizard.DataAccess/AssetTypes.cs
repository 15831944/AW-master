using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	#region AssetType Class

	public class AssetType
	{
		#region Data

		private int		_assetTypeID;
		private string	_name;
		private int		_parentID;
		private string	_icon;
		private bool	_auditable;
		private bool	_internal;

		#endregion Data

		#region Properties

		public int AssetTypeID
		{
			get { return _assetTypeID; }
			set { _assetTypeID = value; }
		}

		public int ParentID
		{
			get { return _parentID; }
			set { _parentID = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}

		public bool Auditable
		{
			get { return _auditable; }
			set { _auditable = value; }
		}

		public bool Internal
		{ 
			get { return _internal; }
		}
		
		#endregion Properties

		#region Constructor

		public AssetType()
		{
			_assetTypeID = 0;
			_parentID = 0;
			_name = "<new asset type>";
			_icon = "computers.png";
			_auditable = false;
			_internal = false;
		}

		public AssetType(DataRow row) : this()
		{
			_assetTypeID = (int)row["_ASSETTYPEID"];
			_parentID = row.IsNull("_PARENTID") ? 0 : (int)row["_PARENTID"];
			_name = (string)row["_NAME"];
			_auditable = (bool)row["_AUDITABLE"];
			_icon = (string)row["_ICON"];
			_internal = (bool)row["_INTERNAL"];			
		}

		#endregion Constructor

		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Called to set the parent category for this asset type
		/// </summary>
		/// <param name="parentCategory"></param>
		public void SetParentCategory(AssetType parentCategory)
		{
			this.ParentID = parentCategory.AssetTypeID;
			this.Auditable = parentCategory.Auditable;
		}

		/// <summary>
		/// Add this Asset Type to the database (or possibly update an existing item)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
            AssetTypesDAO lwDataAccess = new AssetTypesDAO();
			if (AssetTypeID == 0)
				lwDataAccess.AssetTypeAdd(this);
			else
				lwDataAccess.AssetTypeUpdate(this);
			return 0;
		}



		/// <summary>
		/// Delete this asset type, and any sub-types
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			return this.Delete(this);
		}

		protected int Delete(AssetType assettype)
		{
            AssetTypesDAO lwDataAccess = new AssetTypesDAO();

			// If this is a category then we need to delete each of the sub-types first
			if (assettype.ParentID == 0)
			{
				AssetTypeList listAssetTypes = new AssetTypeList();
				listAssetTypes.Populate();
				AssetTypeList listSubTypes = listAssetTypes.EnumerateChildren(AssetTypeID);

				// Loop through the sub-types returned and try and delete them
				foreach (AssetType subType in listSubTypes)
				{
					// Error if we failed to delete - we cannot delete the parent then
					if (this.Delete(subType) != 0)
						return -1;
				}
			}

			// Delete the asset type
			return lwDataAccess.AssetTypeDelete(this);
		}
	}

	#endregion AssetType Class

	#region AssetTypeList Class

	public class AssetTypeList : List<AssetType>
	{
		public int Populate()
		{
			// Ensure that the list is empty initially
			this.Clear();

			AssetTypesDAO lwDataAccess = new AssetTypesDAO();
			DataTable table = lwDataAccess.EnumerateAssetTypes();

			// Iterate through the returned rows in the table and create AssetType objects for each
			foreach (DataRow row in table.Rows)
			{
				AddAssetType(row);
			}
			return this.Count;
		}

		/// <summary>
		/// Add a new AssetType object to the list given a database row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public AssetType AddAssetType(DataRow row)
		{
			// Create the assettype object
			AssetType assettype = new AssetType(row);
			this.Add(assettype);
			return assettype;
		}


		/// <summary>
		/// Return a list of the Asset Type Categories
		/// </summary>
		/// <returns></returns>
		public AssetTypeList EnumerateCategories()
		{
			AssetTypeList categories = new AssetTypeList();
			foreach (AssetType assettype in this)
			{
				if (assettype.ParentID == 0)
					categories.Add(assettype);
			}
			return categories;
		}


		/// <summary>
		/// Return a list of the child asset types of the specified category
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public AssetTypeList EnumerateChildren(int category)
		{
			AssetTypeList children = new AssetTypeList();
			foreach (AssetType assettype in this)
			{
				if (assettype.ParentID == category)
					children.Add(assettype);
			}
			return children;
		}


		/// <summary>
		/// Return a string which is the name of the icon file specified for the parent of the asset type
		/// passed in or an empty string if none found
		/// </summary>
		/// <param name="assettype"></param>
		/// <returns></returns>
		public string GetParentIconFile(AssetType assettype)
		{
			string iconFile = "";
			if (assettype.ParentID != 0)
			{
				AssetType parent = GetParent(assettype);
				if (parent != null)
					iconFile = parent.Icon;
			}
			return iconFile;
		}


		/// <summary>
		/// Return the AssetType object which is the parent of that specified or NULL if not found
		/// </summary>
		/// <param name="ofAssettype"></param>
		/// <returns></returns>
		public AssetType GetParent(AssetType ofAssettype)
		{
			foreach (AssetType assettype in this)
			{
				if (assettype.AssetTypeID == ofAssettype.ParentID)
					return assettype;
			}
			return null;
		}


		/// <summary>
		/// Determine whether or not the list contains the named item
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			foreach (AssetType assettype in this)
			{
				if (assettype.Name == name)
					return true;
			}
			return false;
		}


		/// <summary>
		/// Find and return an existing asset type with the specified name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public AssetType FindByName (string name)
		{
			foreach (AssetType assettype in this)
			{
				if (assettype.Name == name)
					return assettype;
			}
			return null;
		}

        public AssetType FindByNameAndParentID(AssetType objAssetType)
        {
            foreach (AssetType assettype in this)
            {
                if ((assettype.Name == objAssetType.Name) &&(assettype.ParentID==objAssetType.ParentID))
                    return assettype;
            }
            return null;
        }


		/// <summary>
		/// returns a semi-colon delimited string of the names of the individual asset types in the list
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string returnString = "";
			foreach (AssetType assetType in this)
			{
				if (returnString != "")
					returnString += ";";
				returnString += assetType.Name;
			}
			
			return returnString;
		}

	}
	#endregion AssetTypeList Class
}
