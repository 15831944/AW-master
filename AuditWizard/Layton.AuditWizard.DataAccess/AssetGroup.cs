using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace Layton.AuditWizard.DataAccess
{
	/// <summary>
	/// This object encapsules a group for a computer - this is most typically a domain or workgroup but could
	/// also be a user defined location
	/// </summary>
	public class AssetGroup
	{
		#region Data
		/// <summary>
		/// This is the delimiter which we will use to separate locations
		/// </summary>
		public const char LOCATIONDELIMITER = '\\';
		
		
		public enum GROUPTYPE { domain, userlocation };

		/// <summary>
		/// Each computer group may in turn have child groups within it
		/// </summary>
		private List<AssetGroup> _groups = new List<AssetGroup>();

		/// <summary>A computer group may also contain computers / assets</summary>
		private List<Asset> _assets = new List<Asset>();

		/// <summary>This is the database ID of the group</summary>	
		private int _groupID;

		/// <summary>this is the full path to this group</summary>
		private string _fullname;

		/// <summary>This is the name given to the group</summary>
		private string _name;

		/// <summary>This is the database ID of the parent group</summary>
		private int		_parentID;

		/// <summary>This is the starting IP address for this group</summary>
		private string _start_ipaddress;

		/// <summary>This is the ending IP address for this group</summary>
		private string _end_ipaddress;

		/// <summary>This is the type of group (domain / user location)</summary>
		private GROUPTYPE _groupType;

		/// <summary>This flag indicates whether or not the 'populate' function has been calld for this group
		/// this can be different from whether or not any child groups or assets exist</summary>
		private bool _populated;

		#endregion Data

		#region Properties

		public int GroupID
		{
			get { return _groupID; }
			set { _groupID = value; }
		}

		public GROUPTYPE GroupType
		{
			get { return _groupType; }
			set { _groupType = value; }
		}

		public string FullName
		{
			get { return _fullname; }

			set 
			{ 
				_fullname = value; 
				int index = _fullname.LastIndexOf(LOCATIONDELIMITER);
				if (index == 0)
					_name = _fullname;
				else
					_name = _fullname.Substring(index+1);				
			}
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int ParentID
		{
			get { return _parentID; }
			set { _parentID = value; }
		}

		public string StartIP
		{
			get { return _start_ipaddress; }
			set { _start_ipaddress = value; }
		}

		public string EndIP
		{
			get { return _end_ipaddress; }
			set { _end_ipaddress = value; }
		}

		public bool Populated
		{
			get { return _populated; }
			set { _populated = value; }
		}

		/// <summary>
		/// return a list of the child groups of this group
		/// </summary>
        public List<AssetGroup> Groups
		{
			get { return _groups; }
		}

		/// <summary>
		/// Return a list of the child assets of this group
		/// </summary>
		public List<Asset> Assets
		{
			get { return _assets; }
		}

		#endregion

		#region Constructors

		public AssetGroup()
		{
			_groupID = 0;
			_fullname = "";
			_name = "";
			_parentID = 0;
			_start_ipaddress = "";
			_end_ipaddress = "";
			_groupType = GROUPTYPE.domain;
			_populated = false;
		}


		/// <summary>
		/// Create a blank object of the correct group type
		/// </summary>
		/// <param name="groupType"></param>
		public AssetGroup(GROUPTYPE groupType)
			: base()
		{
			_groupType = groupType;
		}


		public AssetGroup(DataRow dataRow, GROUPTYPE groupType) : base()
		{
			GroupType = groupType;
			Name	= (string)dataRow["_NAME"];

			// The parent of this item may be null if the root item
			if (dataRow.IsNull("_PARENTID"))
				this.ParentID = 0;
			else
				this.ParentID = (int)dataRow["_PARENTID"];

			if (groupType == GROUPTYPE.userlocation)
			{
				GroupID		= (int)dataRow["_LOCATIONID"];
				FullName	= (string)dataRow["_FULLNAME"];
				this.StartIP = (string)dataRow["_START_IPADDRESS"];
				this.EndIP = (string)dataRow["_END_IPADDRESS"];
			}
			else
			{
				this.GroupID = (int)dataRow["_DOMAINID"];
				this.FullName = Name;
			}
		}

		#endregion Constructors

		#region Methods


		/// <summary>
		/// Return the icon which should display for this group.
		/// </summary>
		/// <returns></returns>
		public Bitmap DisplayIcon()
		{
			return (GroupType == AssetGroup.GROUPTYPE.domain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
		}


		/// <summary>
		/// Add this group to the database (only applicable to user locations)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			// If this Location already exists then call the update instead
			if (_groupID != 0)
				return Update(null);

			// If we have not specified a parent but do have a full name then we have a problem in that we 
			// do not necessarily have a parent location in the database and in any case we do not know the ID
			// of our parent so should look for it now...
			if ((ParentID == 0) && (_fullname.Contains(@"\")))
			{
				_groupID = ValidateParentPath(_fullname);
			}
			else
			{
				// Add the location to the database
				LocationsDAO lwDataAccess = new LocationsDAO();
				int groupID = lwDataAccess.GroupAdd(this);
				if (groupID != 0)
				{
					AuditChanges(null);
					_groupID = groupID;
				}
			}
			
			return 0;
		}


		/// <summary>
		/// Update this Group defininition in the database
		/// </summary>
		/// <returns></returns>
		public int Update(AssetGroup oldgroup)
		{
			LocationsDAO lwDataAccess = new LocationsDAO();
			if (this._groupID == 0)
			{
				Add();
			}
			else
			{
				lwDataAccess.GroupAdd(this);
				AuditChanges(oldgroup);
			}

			return 0;
		}



		/// <summary>
		/// Delete the current location from the database
		/// </summary>
		public bool Delete()
		{
			LocationsDAO lwDataAccess = new LocationsDAO();

			// As locations are hierarchical we cannot just delete a location without first deleting ALL 
			// children and we cannot do that if any of our descendants are still being referenced
			// Get a list of all of our children
			AssetGroupList children = new AssetGroupList(lwDataAccess.GetGroups(this) ,GroupType);

			// Loop through each child and try and delete them first before we delete ourselves - this actually 
			// causes recursion through this deletion function as children may have children and so on...
			foreach (AssetGroup childGroup in children)
			{
				if (!childGroup.Delete())
					return false;
			}

			// Only now can we delete ourselves as all of our children have been handled.
			if (lwDataAccess.GroupDelete(this) != 0)
				return false;

			// ...and audit the deletion
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.location;
			ate.Type = AuditTrailEntry.TYPE.deleted;
			ate.Key = _name;
			ate.AssetID = 0;
			ate.AssetName = "";
			ate.Username = System.Environment.UserName;
			new AuditTrailDAO().AuditTrailAdd(ate);
			return true;
		}


		/// <summary>
		/// return whether or not the specified named group is already a child of this group
		/// </summary>
		/// <param name="childname"></param>
		/// <returns></returns>
		public AssetGroup IsChildGroup(string childname)
		{
			// First get a list of our current children
			LocationsDAO lwDataAccess = new LocationsDAO();
			AssetGroupList listChildren = new AssetGroupList(lwDataAccess.GetGroups(this), _groupType);
			return listChildren.FindGroup(childname);
		}


		/// <summary>
		/// return whether or not the specified AssetGroup is a child of this group
		/// </summary>
		/// <param name="childname"></param>
		/// <returns></returns>
		public AssetGroup IsChildGroup(AssetGroup group)
		{
			return IsChildGroup(group.Name);
		}


		/// <summary>
		/// Populate this Asset Group with it's children (both child groups and assets)
		/// Note that this may recurse through its children populating them also
		/// Child assets may be excluded !!!
		/// </summary>
		/// <returns></returns>
		public int Populate(bool recurse, bool ignoreChildAssets, bool applyStates)
		{
			// First get child groups
			LocationsDAO lwDataAccess = new LocationsDAO();
			_groups = new AssetGroupList(lwDataAccess.GetGroups(this), _groupType);

			// ...then get child assets
			if (!ignoreChildAssets)
				_assets = new AssetList(new AssetDAO().GetAssets(GroupID, GroupType, applyStates), ignoreChildAssets);

			// If we have been requested to recurse then go through each group
			if (recurse)
			{
				foreach (AssetGroup childGroup in Groups)
				{
					childGroup.Populate(recurse, ignoreChildAssets, applyStates);
				}
			}

			// Show that we have populated this group
			Populated = true;

			return 0;
		}


		/// <summary>
		/// If this group has associated IP addresses does the specified IP address 
		/// fall within the defined range.
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <returns></returns>
		public bool ContainsIP(string ipAddress)
		{
			if (ipAddress == "")
				return false;
			
			// Convert the supplied IP address to a .NET IPAddress 	
			try
			{			
				// If no starting IP then no match for sure
				if (this.StartIP == "")
					return false;
				
				// OK break the start and end IP address into individual entries
				string[] startIpAddresses = this.StartIP.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
				string[] endIpAddresses = this.EndIP.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		
				// ...and check if the IP address we have supplied fits into that range
				for (int index =0; index<startIpAddresses.Length; index++)
				{
					if ((IPAddressComparer.IsGreaterOrEqual(ipAddress ,startIpAddresses[index]))
					&&  (IPAddressComparer.IsLessOrEqual(ipAddress ,endIpAddresses[index])))
						return true;
				}
			}
			catch (Exception)
			{
			}
			
			return false;
		}


		/// <summary>
		/// Recover 
		/// </summary>
		/// <param name="forGroup"></param>
		/// <param name="listAssets"></param>
		public AssetList GetAllAssets()
		{
			// CMD 8.4.2 - Always Populate as we cannot be sure of the criteria previously set
			Populate(true ,false ,true);
				
			AssetList listAssets = new AssetList();
			//
			foreach (AssetGroup childGroup in this.Groups)
			{
				listAssets.AddRange(childGroup.GetAllAssets());
			}

			// Add on any assets here		
			listAssets.AddRange(this.Assets);
			return listAssets;
		}
		
			
		
		/// <summary>
		/// This function is called to apply a filtor the list of groups and assets and remove any
		/// not included in the filter.
		/// </summary>
		/// <param name="_groupFiler"></param>
		/// <param name="_assetFilter"></param>
		public void ApplyFilters(string groupFilter ,string assetFilter ,bool ignoreChildAssets)
		{
			// Do not remove any groups or assets if no filters applied
			if ((groupFilter == "") && (assetFilter == ""))
				return;
		
			// Convert the input strings into lists as these are easier to manipulate
			List<string> listFilterGroups = Utility.ListFromString(groupFilter, ';' ,true);
			List<string> listFilterAssets = Utility.ListFromString(assetFilter, ';' ,true);
			
			// Filter the groups and assets BENEATH this one
			// Iterate through the groups identified and check to see which are in the filter
			for (int index = 0; index < this.Groups.Count; )
			{
				AssetGroup thisGroup = this.Groups[index];
				if (FilterGroup(listFilterGroups ,listFilterAssets ,thisGroup))
					this.Groups.RemoveAt(index);
				else
					index++;
			}
			
			// We still have to deal with the assets at this level though
			for (int index = 0; index < this.Assets.Count; )
			{
				Asset asset = this.Assets[index];
				if (!listFilterAssets.Contains(asset.Name))
					this.Assets.RemoveAt(index);
				else
					index++;
			}
			
		}

		
		/// <summary>
		/// Apply the filters to a specific group and its descendants
		/// </summary>
		/// <param name="listFilterGroups"></param>
		/// <param name="listFilterGroups"></param>
		/// <param name="thisGroup"></param>
		/// <returns></returns>
		protected bool FilterGroup(List<string> listFilterGroups, List<string> listFilterAssets, AssetGroup thisGroup)
		{
		
			// is this group explitily mentioned in the filter list?
			if (listFilterGroups.Contains(thisGroup.FullName))
				return false;

			// OK this group is not mentioned but we should check it's children
			for (int index = 0; index < thisGroup.Groups.Count; )
			{
				// Get the (next) child group
				AssetGroup childGroup = thisGroup.Groups[index];

				// Check to see if we need to filter it and if so remove it from the list now
				// If it is not going to be filtered then simply iterate to the next child
				if (FilterGroup(listFilterGroups ,listFilterAssets ,childGroup))
					thisGroup.Groups.Remove(childGroup);
				else
					index++;
			}

			// OK Child groups have now been handled (if any) - check to see if this group
			// should remain owing to an asset within the group being in the filter
			for (int index = 0; index < thisGroup.Assets.Count; )
			{
				Asset asset = thisGroup.Assets[index];
				if (!listFilterAssets.Contains(asset.Name))
					thisGroup.Assets.RemoveAt(index);
				else
					index++;
			}

			// If we have no child groups or assets left then this group should be filtered
			return ((thisGroup.Groups.Count == 0) && (thisGroup.Assets.Count == 0));
		}			
		
		
	
		/// <summary>
		/// This function is typically called prior to adding a new location where we have not set the
		/// parentID as we do not know it yet - in this case we need to ensure that the path to the new location
		/// has been created as we cannot create an isolated child
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>	
		protected int ValidateParentPath(string path)
		{
			LocationsDAO lwDataAccess = new LocationsDAO();

			// We are passed the path of the item in question for whom we are checking for the existance of their
			// parent.  To do this we must build the name of that parent
			int index = path.LastIndexOf(LOCATIONDELIMITER);
			if (index == 0)
				return 0;
				
			// ...and make the name of our parent
			string ourParentPath = path.Substring(0, index);

			// Check the database for this location
			int parentID = lwDataAccess.LocationFind(ourParentPath);
			
			// Does our parent exist?  If so then we can create ourselves and pass this back
			if (parentID != 0)
			{
				AssetGroup ourGroup = new AssetGroup();
				ourGroup.FullName = path;
				ourGroup.GroupType = GROUPTYPE.userlocation;
				ourGroup.ParentID = parentID;
				ourGroup.Add();
		
				// ... and return the ID of the newly added group as the parent
				return ourGroup.GroupID;
			}

			else
			{
				// Our parent did not exist - iterate back up the tree
				return ValidateParentPath(ourParentPath);
			}
		}
		
		/// <summary>
		/// Return the specified asset or null if none
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Asset FindAsset (string name)
		{
			foreach (Asset asset in _assets)
			{
				if (asset.Name == name)
					return asset;
			}
			return null;
		}
		#endregion Methods

		#region Change Handling

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// We really aren't that interested in changes to these details just adding and deletion
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public List<AuditTrailEntry> AuditChanges(AssetGroup oldObject)
		{
            AuditTrailDAO lwDataAccess = new AuditTrailDAO();

			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// Is this a new item or an update to an existing one
			if (GroupID == 0)
			{
				AuditTrailEntry ate = new AuditTrailEntry();
				ate.Date = DateTime.Now;
				ate.Class = AuditTrailEntry.CLASS.location;
				ate.Type = AuditTrailEntry.TYPE.added;
				ate.Key = _name;
				ate.AssetID = 0;
				ate.AssetName = "";
				ate.Username = System.Environment.UserName;
				listChanges.Add(ate);
			}

			// Add all of these changes to the Audit Trail
			foreach (AuditTrailEntry entry in listChanges)
			{
				lwDataAccess.AuditTrailAdd(entry);
			}

			// Return the constructed list
			return listChanges;
		}

		#endregion Change Handling

		/// <summary>
		/// Override ToString to return the location name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}

	#region AssetGroupList Class
    
	public class AssetGroupList : List<AssetGroup>
	{
		public AssetGroupList()
		{
		}

		/// <summary>
		/// This constructor takes a data table with a list of asset groups which will be added
		/// </summary>
		/// <param name="locations"></param>
		public AssetGroupList(DataTable groups ,AssetGroup.GROUPTYPE groupType)
		{
			foreach (DataRow row in groups.Rows)
			{
				this.Add(new AssetGroup(row, groupType));
			}
		}		
		
		
		/// <summary>
		/// Populate the asset group list
		/// </summary>
		public void Populate (bool recurse ,bool ignoreChildAssets, bool applyStates)
		{
			foreach (AssetGroup group in this)
			{
				group.Populate(true, false, applyStates);
			}
		}
		
		
		// Find an application in the list
		public AssetGroup FindGroup (string name)
		{
			int index = GetIndex(name);
			return (index == -1) ? null : this[index];
		}


		/// <summary>
		/// Return the index of the Asset Group with the specified name
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public int GetIndex(string name)
		{
			int index = 0;
			foreach (AssetGroup thisGroup in this)
			{
				if (thisGroup.Name == name)
					return index;
				index++;
			}

			return -1;
		}

		/// <summary>
		/// Does the list contain an Asset Group with the specified name?
		/// </summary>
		/// <param name="Location"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			return (GetIndex(name) != -1);
		}
		
		
		
		/// <summary>
		/// Search through the groups in this list searching for one which best matches the supplied IP address
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <returns></returns>
		public AssetGroup FindByIP(String ipAddress)
		{
			// loop through the groups
			foreach (AssetGroup group in this)
			{
				if (group.ContainsIP(ipAddress))
					return group;
			}
		
			return null;
		}


		/// <summary>
		/// Override of ToString returns the list of asset group names as a semi-colon delimited string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
            StringBuilder sb = new StringBuilder();

            foreach (AssetGroup group in this)
            {
                sb.Append(group.GroupID + ",");
            }
            return sb.ToString().TrimEnd(',');
		}


		/// <summary>
		/// return a list of ALL of the assets defined 
		/// </summary>
		/// <returns></returns>
		public AssetList GetAllAssets()
		{
			AssetList listAssets = new AssetList();
			//
			foreach (AssetGroup childGroup in this)
			{
				listAssets.AddRange(childGroup.GetAllAssets());
			}

			// return the entire list
			return listAssets;
		}		
	}

	#endregion AssetGroupList Class

    #region InteractiveAssetGroupList

    public class InteractiveAssetGroupList
    {
        private int _groupID;

        public int GroupID
        {
            get { return _groupID; }
            set { _groupID = value; }
        }
    }

    #endregion


}
