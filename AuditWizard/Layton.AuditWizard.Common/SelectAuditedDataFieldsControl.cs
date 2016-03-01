using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public delegate void CheckChangedEventHandler(object sender, UltraTreeNode node);
    public delegate void AfterSelectEventHandler(object sender, UltraTreeNode node);

    public partial class SelectAuditedDataFieldsControl : UserControl
    {
        #region data

        /// <summary>
        /// Enumeration for types of item in the selected data tree
        /// </summary>
        public enum eRootTypes { asset, os, applications, hardware, system, userdata, internet };

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>This is the list of items selected, passed in and out from this control</summary>
        List<string> _listSelectedItems = new List<string>();

        /// <summary>The root node of the tree</summary>
        private UltraTreeNode _rootNode;

        /// <summary>Data fields which determine how applications will be displayed in this tree</summary>
        protected string _publisherFilter = "";
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;

        /// <summary>Style of nodes displayed (checkboxes, tristate or standard)</summary>
        private NodeStyle _nodeStyle = NodeStyle.CheckBoxTriState;
        private NodeStyle _valueNodeStyle = NodeStyle.CheckBox;

        /// <summary>Flag to show whether or not to allow the applications branch to expand</summary>
        private bool _allowExpandApplications = true;

        /// <summary>Flag to indicate whether we should show the 'Internet' placeholder for selection</summary>
        private bool _allowInternetSelection = true;

        /// <summary>Flag to restrict the display to alertable items only</summary>
        private bool _alertableItemsOnly = false;

        /// <summary>Flag to restrict the display to the report specific items (Date of Last Audit and Location)</summary>
        private bool _reportSpecificItemsShow = false;

		/// <summary>
		/// Special nodes which may be affected by property changes
		/// </summary>
		private UltraTreeNode _applicationsNode = null;
		private UltraTreeNode _internetNode = null;
        #endregion data

        #region Properties


        public List<string> SelectedItems
        {
            get { return _listSelectedItems; }
            set { _listSelectedItems = value; }
        }

        /// <summary>
        /// Determine whether or not check boxes are shown in this tree
        /// </summary>
        [Category("DisplayStyles")]
        public NodeStyle NodeStyle
        {
            get { return _nodeStyle; }
            set
            {
                _nodeStyle = value;
                _valueNodeStyle = (_nodeStyle == NodeStyle.Standard) ? NodeStyle.Standard : NodeStyle.CheckBox;
				fieldsTree.Override.NodeStyle = _nodeStyle;
            }
        }

        [Category("DisplayStyles")]
        public bool AlertableItemsOnly
        {
            get { return _alertableItemsOnly; }
            set { _alertableItemsOnly = value; }
        }

        public bool ReportSpecificItemsShow
        {
            get { return _reportSpecificItemsShow; }
            set { _reportSpecificItemsShow = value; }
        }

        [Category("DisplayStyles")]
        public bool AllowExpandApplications
        {
            get { return _allowExpandApplications; }
            set 
			{
				_allowExpandApplications = value;
				if (_applicationsNode != null)
					_applicationsNode.Override.ShowExpansionIndicator = (_allowExpandApplications == false) ? ShowExpansionIndicator.Never : ShowExpansionIndicator.Always;
			}
        }

        [Category("DisplayStyles")]
        public bool AllowInternetSelection
        {
            get { return _allowInternetSelection; }
            set 
			{ 
				_allowInternetSelection = value;
				if (_internetNode != null)
					_internetNode.Visible = (_allowInternetSelection);
			}
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// This event is triggered when a node in the tree changes its checkstate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public event CheckChangedEventHandler CheckChanged;
        public event AfterSelectEventHandler AfterSelect;

        #endregion Events

        #region Constructor

        public SelectAuditedDataFieldsControl()
        {
            InitializeComponent();

            _publisherFilter = "";
            _showIgnored = false;
            _showIncluded = true;
        }

        #endregion Constructor

        #region Form Control Functions

        private void SelectAuditedDataFieldsControl_Load(object sender, EventArgs e)
        {
            // Don't populate the control in design mode
            if (DesignMode)
                return;

            // What style will we use here
            fieldsTree.Override.NodeStyle = _nodeStyle;

            // Populate the tree
            using (new WaitCursor())
            {
                fieldsTree.BeginUpdate();

                try
                {
                    // Add in a root node which is non-checkable 
                    _rootNode = new UltraTreeNode("Report Data Fields", "Report Data Fields");
                    _rootNode.LeftImages.Add(Properties.Resources.generalreporting16);
                    _rootNode.Override.NodeStyle = NodeStyle.Standard;
                    fieldsTree.Nodes.Add(_rootNode);

                    UltraTreeNode assetNode = new UltraTreeNode(AWMiscStrings.AssetDetails, AWMiscStrings.AssetDetails);
                    assetNode.LeftImages.Add(Properties.Resources.computer16);
                    _rootNode.Nodes.Add(assetNode);
                    assetNode.Tag = eRootTypes.asset;
                    assetNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

                    UltraTreeNode osNode = new UltraTreeNode(AWMiscStrings.OSNode, AWMiscStrings.OSNode);
                    osNode.LeftImages.Add(Properties.Resources.os_16);
                    _rootNode.Nodes.Add(osNode);
                    osNode.Tag = eRootTypes.os;
                    osNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

					_applicationsNode = new UltraTreeNode(AWMiscStrings.ApplicationsNode, AWMiscStrings.ApplicationsNode);
					_applicationsNode.LeftImages.Add(Properties.Resources.application_16);
                    _rootNode.Nodes.Add(_applicationsNode);
					_applicationsNode.Tag = eRootTypes.applications;
					_applicationsNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

                    UltraTreeNode hardwareNode = new UltraTreeNode(AWMiscStrings.HardwareNode, AWMiscStrings.HardwareNode);
                    hardwareNode.LeftImages.Add(Properties.Resources.hardware);
                    _rootNode.Nodes.Add(hardwareNode);
                    hardwareNode.Tag = eRootTypes.hardware;
                    hardwareNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

                    UltraTreeNode systemNode = new UltraTreeNode(AWMiscStrings.SystemNode, AWMiscStrings.SystemNode);
                    systemNode.LeftImages.Add(Properties.Resources.system_16);
                    _rootNode.Nodes.Add(systemNode);
                    systemNode.Tag = eRootTypes.system;
                    systemNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

                    // Add the Internet placeholder
					_internetNode = new UltraTreeNode(AWMiscStrings.InternetNode, AWMiscStrings.InternetNode);
                    _internetNode.LeftImages.Add(Properties.Resources.internet_explorer);
                    _rootNode.Nodes.Add(_internetNode);
					_internetNode.Tag = eRootTypes.internet;
					_internetNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                    UltraTreeNode userDataNode = new UltraTreeNode(AWMiscStrings.UserDataNode, AWMiscStrings.UserDataNode);
                    userDataNode.LeftImages.Add(Properties.Resources.system_16);
                    _rootNode.Nodes.Add(userDataNode);
                    userDataNode.Tag = eRootTypes.userdata;
                    userDataNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;

                    // Expand the root node so that we can see the categories of items
                    _rootNode.Expanded = true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Exception adding tree nodes in [SelectAuditedDataFieldsControl_Load], the exception text was " + ex.Message);
                    logger.Error(ex.Message);
                }

                // Finished updating the tree			
                fieldsTree.EndUpdate();
            }
        }

        #endregion Form Control Functions

        #region Public Methods

        public void SetApplicationFilters(string publisherFilter, bool showIgnored, bool showIncluded)
        {
            _publisherFilter = publisherFilter;
            _showIgnored = showIgnored;
            _showIncluded = showIncluded;
        }

        #endregion Public Methods


        #region Internal Methods

        protected void PopulateUserDataDetails(UltraTreeNode rootNode)
        {
            if (rootNode.Text != AWMiscStrings.UserDataNode) return;

            UserDataCategoryList userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
            userDataCategories.Populate();

            foreach (UserDataCategory category in userDataCategories)
            {
                if (category.Scope == UserDataCategory.SCOPE.Asset)
                {
                    UltraTreeNode categoryNode = new UltraTreeNode(AWMiscStrings.UserDataNode + @"|" + category.Name, category.Name);
                    categoryNode.LeftImages.Add(IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small));
                    //categoryNode.Tag = eRootTypes.userdata + category.CategoryID;
                    rootNode.Nodes.Add(categoryNode);
                    categoryNode.Tag = eRootTypes.userdata;

                    // Add fields within this category
                    foreach (UserDataField field in category)
                    {
                        UltraTreeNode fieldNode = new UltraTreeNode(categoryNode.Key + @"|" + field.Name, field.Name);
                        fieldNode.LeftImages.Add(IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small));
                        fieldNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                        //fieldNode.Tag = eRootTypes.userdata;
                        categoryNode.Nodes.Add(fieldNode);
                        fieldNode.Tag = eRootTypes.userdata;
                    }
                }
            }
        }

        /// <summary>
        /// Populate the Asset Details node of the tree
        /// </summary>
        /// <param name="rootNode"></param>
        protected void PopulateAssetDetails(UltraTreeNode rootNode)
        {
            if (rootNode.Text != AWMiscStrings.AssetDetails) return;

            // Add in Asset Name - we cannot disable this
            string attribute = Asset.GetAttributeName(Asset.eAttributes.assetname);
            UltraTreeNode node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            //node.Enabled = false;
            rootNode.Nodes.Add(node);

            node = new UltraTreeNode(rootNode.Key + @"|" + "Asset Tag", "Asset Tag");
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);

            // If we are ONLY displaying Alertable options then many of the asset details are not required
            if (!_alertableItemsOnly)
            {
                attribute = Asset.GetAttributeName(Asset.eAttributes.location);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.location_16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
                //
                attribute = Asset.GetAttributeName(Asset.eAttributes.domain);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.entire_network16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
                //
                attribute = Asset.GetAttributeName(Asset.eAttributes.lastaudit);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.Calendar_16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
                //
                attribute = Asset.GetAttributeName(Asset.eAttributes.stock_status);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.computer16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
                //
                attribute = Asset.GetAttributeName(Asset.eAttributes.suppliername);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.computer16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
            }

            // if we've not shown the alertable items, check if we need to show report ones
            if (_alertableItemsOnly && _reportSpecificItemsShow)
            {
                attribute = Asset.GetAttributeName(Asset.eAttributes.location);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.location_16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);

                attribute = Asset.GetAttributeName(Asset.eAttributes.lastaudit);
                node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
                node.LeftImages.Add(Properties.Resources.Calendar_16);
                node.Tag = rootNode.Tag;
                node.Override.NodeStyle = _valueNodeStyle;
                rootNode.Nodes.Add(node);
            }

            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.ipaddress);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.macaddress);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.category);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            node = new UltraTreeNode(rootNode.Key + @"|" + "Type", "Type");
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.make);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.model);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = Asset.GetAttributeName(Asset.eAttributes.serial);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.computer16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
        }


        /// <summary>
        /// Populate the Asset Details node of the tree
        /// </summary>
        /// <param name="rootNode"></param>
        protected void PopulateOperatingSystems(UltraTreeNode rootNode)
        {
            if (rootNode.Text != AWMiscStrings.OSNode) return;

            string attribute = OSInstance.GetAttributeName(OSInstance.eAttributes.family);

            UltraTreeNode node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.os_16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = OSInstance.GetAttributeName(OSInstance.eAttributes.fullname);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.os_16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
            //
            attribute = OSInstance.GetAttributeName(OSInstance.eAttributes.cdkey);
            node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
            node.LeftImages.Add(Properties.Resources.os_16);
            node.Tag = rootNode.Tag;
            node.Override.NodeStyle = _valueNodeStyle;
            rootNode.Nodes.Add(node);
			//
			attribute = OSInstance.GetAttributeName(OSInstance.eAttributes.serial);
			node = new UltraTreeNode(rootNode.Key + @"|" + attribute, attribute);
			node.LeftImages.Add(Properties.Resources.os_16);
			node.Tag = rootNode.Tag;
			node.Override.NodeStyle = _valueNodeStyle;
			rootNode.Nodes.Add(node);
		}


        /// <summary>
        /// Populate the Applications node of the tree with publishers and then applications
        /// </summary>
        /// <param name="rootNode"></param>
        protected void PopulatePublishers(UltraTreeNode rootNode)
        {
            UltraTreeNode[] publisherNodes;
            UltraTreeNode publisherNode;
            string lPublisher;

            DataTable dt = new ApplicationsDAO().GetAllPublisherNamesAsDataTable("");
            publisherNodes = new UltraTreeNode[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                lPublisher = dt.Rows[i][0].ToString();

                publisherNode = new UltraTreeNode(rootNode.Key + @"|" + lPublisher, lPublisher);
                publisherNode.Tag = rootNode.Tag;
                publisherNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

                publisherNodes[i] = publisherNode;
            }

            rootNode.Nodes.AddRange(publisherNodes);
        }

        protected void PopulateApplications(UltraTreeNode publisherNode)
        {
            //publisherNode.Nodes.Clear();

            string publisher = publisherNode.FullPath.Substring(32);
            string appName = "";
            string appVersion = "";
            string key;
            int appID = -1;
            DataRow row;
            UltraTreeNode applicationNode;

            DataTable dt = new ApplicationsDAO().GetApplicationsByPublisher(publisher, _showIncluded, _showIgnored);
            UltraTreeNode[] applicationNodes = new UltraTreeNode[dt.Rows.Count];

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                row = dt.Rows[j];
                appID = Convert.ToInt32(row["_APPLICATIONID"]);
                appName = row["_NAME"].ToString();
                appVersion = row["_VERSION"].ToString();

                if (appVersion != String.Empty)
                {
                    //key = publisherNode.Key + @"|" + appName + " (v" + appVersion + ")" + "|" + appID;
                    key = publisherNode.Key + @"|" + appName + " (v" + appVersion + ")";
                    applicationNode = new UltraTreeNode(key, appName + " (v" + appVersion + ")");
                }
                else
                {
                    //key = publisherNode.Key + @"|" + appName + "|" + appID;
                    key = publisherNode.Key + @"|" + appName;
                    applicationNode = new UltraTreeNode(key, appName);
                }

                applicationNode.Tag = publisherNode.Tag;
                applicationNode.Override.NodeStyle = _valueNodeStyle;

                applicationNodes[j] = applicationNode;
            }

            publisherNode.Nodes.AddRange(applicationNodes);
        }

        /// <summary>
        /// Populate the Hardware node of the tree
        /// </summary>
        /// <param name="parentNode"></param>

        protected void PopulateSystemPatches(UltraTreeNode parentNode)
        {
            DataTable categoryTable = new AuditedItemsDAO().GetAuditedItemCategories(parentNode.Key);
            UltraTreeNode[] categoryNodes = new UltraTreeNode[categoryTable.Rows.Count];
            UltraTreeNode categoryNode;

            for (int i = 0; i < categoryTable.Rows.Count; i++)
            {
                string category = (string)categoryTable.Rows[i]["_CATEGORY"];
                categoryNode = new UltraTreeNode(category, category.Substring(category.LastIndexOf("|") + 1));
                categoryNode.Tag = parentNode.Tag;

                categoryNodes[i] = categoryNode;
            }

            parentNode.Nodes.AddRange(categoryNodes);
        }
        //protected void PopulateSystemPatches(UltraTreeNode parentNode)
        //{
        //    AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();
        //    DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(parentNode.Key);
        //    //UltraTreeNode[] categoryNodes = new UltraTreeNode[categoryTable.Rows.Count];

        //    // Iterate through the returned rows and add the text and icon to the tree
        //    // All the returned items are categories
        //    foreach (DataRow row in categoryTable.Rows)
        //    {
        //        string category = (string)row["_CATEGORY"];
        //        string icon = (string)row["_ICON"];
        //        UltraTreeNode categoryNode;
        //        int index;
        //        string nodeText = String.Empty;

        //        // We display just the last portion of the category name as the node text
        //        index = category.LastIndexOf("|");
        //        nodeText = category.Substring(index + 1);

        //        // Add the category itself
        //        categoryNode = new UltraTreeNode(category, nodeText);
        //        categoryNode.Tag = parentNode.Tag;
        //        categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));
        //        Trace.Write("Adding node " + category + " to tree node " + parentNode.Key + "\n");

        //        try
        //        {
        //            parentNode.Nodes.Add(categoryNode);
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}


        /// <summary>
        /// Populate the Hardware node of the tree
        /// </summary>
        /// <param name="parentNode"></param>
        //protected void PopulateHardware(UltraTreeNode parentNode)
        //{
        //    AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();
        //    DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(parentNode.Key);
        //    DataRow catRow;
        //    UltraTreeNode categoryNode, nameNode;
        //    int index;
        //    string icon, nodeText, category, name, nameKey;

        //    UltraTreeNode[] nodes = new UltraTreeNode[categoryTable.Rows.Count];

        //    parentNode.Nodes.Clear();

        //    // Iterate through the returned rows and add the text and icon to the tree
        //    // All the returned items are categories
        //    for (int i = 0; i < categoryTable.Rows.Count; i++)
        //    {
        //        catRow = categoryTable.Rows[i];

        //        category = (string)catRow["_CATEGORY"];
        //        //icon = (string)catRow["_ICON"];

        //        if (category.StartsWith("System|Patches|"))
        //        {
        //            string reducedCategory = category.Substring(0, category.LastIndexOf('|'));

        //            index = reducedCategory.LastIndexOf("|");
        //            nodeText = reducedCategory.Substring(index + 1);

        //            categoryNode = new UltraTreeNode(reducedCategory, nodeText);
        //            categoryNode.Tag = parentNode.Tag;
        //            //categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));

        //            try
        //            {
        //                parentNode.Nodes.Add(categoryNode);
        //                // Recurse to check for child categories of the category we have just added
        //                PopulateSystemPatches(categoryNode);
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //        else
        //        {
        //            // We display just the last portion of the category name as the node text
        //            index = category.LastIndexOf("|");
        //            nodeText = category.Substring(index + 1);

        //            // Add the category itself
        //            categoryNode = new UltraTreeNode(category, nodeText);
        //            categoryNode.Tag = parentNode.Tag;
        //            //categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));
        //            categoryNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
        //            nodes[i] = categoryNode;
        //        }
        //    }

        //    parentNode.Nodes.AddRange(nodes);

        //    // Now check for any NAMES within this category
        //    DataTable namesTable = lwDataAccess.GetAuditedItemCategoryNames(parentNode.Key);
        //    UltraTreeNode[] namesNodes = new UltraTreeNode[namesTable.Rows.Count];

        //    // Add any NAMES after the Categories
        //    for (int j = 0; j < namesTable.Rows.Count; j++)
        //    {
        //        DataRow row = namesTable.Rows[j];

        //        category = (string)row["_CATEGORY"];
        //        name = (string)row["_NAME"];
        //        //icon = (string)row["_ICON"];

        //        // Add the NAME node
        //        nameKey = category + "|" + name;
        //        nameNode = new UltraTreeNode(nameKey, name);
        //        nameNode.Tag = parentNode.Tag;
        //        //nameNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));
        //        nameNode.Override.NodeStyle = _valueNodeStyle;

        //        namesNodes[j] = nameNode;
        //    }

        //    parentNode.Nodes.AddRange(namesNodes);

        //    // If no sub-categories or Names then set the expansion indicator to none
        //    if ((categoryTable.Rows.Count == 0) && (namesTable.Rows.Count == 0))
        //        parentNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

        //}

        protected void PopulateHardware(UltraTreeNode parentNode)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();
            DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(parentNode.Key);
            DataRow catRow;
            UltraTreeNode categoryNode, nameNode;
            UltraTreeNode[] nodes;
            int index;
            string nodeText, category, name, nameKey;

            //parentNode.Nodes.Clear();

            if (parentNode.Key == "System|Patches")
            {
                for (int i = 0; i < categoryTable.Rows.Count; i++)
                {
                    catRow = categoryTable.Rows[i];
                    category = (string)catRow["_CATEGORY"];

                    string reducedCategory = category.Substring(0, category.LastIndexOf('|'));

                    index = reducedCategory.LastIndexOf("|");
                    nodeText = reducedCategory.Substring(index + 1);

                    categoryNode = new UltraTreeNode(reducedCategory, nodeText);
                    categoryNode.Tag = parentNode.Tag;
                    categoryNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

                    if (fieldsTree.GetNodeByKey(reducedCategory) == null)
                        parentNode.Nodes.Add(categoryNode);
                }
            }
            else
            {
                nodes = new UltraTreeNode[categoryTable.Rows.Count];

                // Iterate through the returned rows and add the text and icon to the tree
                // All the returned items are categories
                for (int i = 0; i < categoryTable.Rows.Count; i++)
                {
                    catRow = categoryTable.Rows[i];
                    category = (string)catRow["_CATEGORY"];

                    // We display just the last portion of the category name as the node text
                    index = category.LastIndexOf("|");
                    nodeText = category.Substring(index + 1);

                    // Add the category itself
                    categoryNode = new UltraTreeNode(category, nodeText);
                    categoryNode.Tag = parentNode.Tag;
                    categoryNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
                    nodes[i] = categoryNode;
                }

                parentNode.Nodes.AddRange(nodes);
            }

            // Now check for any NAMES within this category - only need to do this if there were no categories
            DataTable namesTable = new DataTable();

            if (categoryTable.Rows.Count == 0)
            {
                namesTable = lwDataAccess.GetAuditedItemCategoryNames(parentNode.Key);
                UltraTreeNode[] namesNodes = new UltraTreeNode[namesTable.Rows.Count];

                // Add any NAMES after the Categories
                for (int j = 0; j < namesTable.Rows.Count; j++)
                {
                    DataRow row = namesTable.Rows[j];

                    category = (string)row["_CATEGORY"];
                    name = (string)row["_NAME"];

                    // Add the NAME node
                    nameKey = category + "|" + name;
                    nameNode = new UltraTreeNode(nameKey, name);
                    nameNode.Tag = parentNode.Tag;
                    nameNode.Override.NodeStyle = _valueNodeStyle;

                    namesNodes[j] = nameNode;
                }

                parentNode.Nodes.AddRange(namesNodes);
            }

            // If no sub-categories or Names then set the expansion indicator to none
            if ((categoryTable.Rows.Count == 0) && (namesTable.Rows.Count == 0))
                parentNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

        }

        #endregion Internal Methods


        #region Tree Control Handlers

        /// <summary>
        /// We do not want to allow the user to explicitely set the check state to indeterminate
        /// as this is a pre-set state based on the state of child items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldsTree_BeforeCheck(object sender, BeforeCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Indeterminate)
                e.Cancel = false;
        }



        /// <summary>
        /// Handles the UltraTree's AfterCheck event.
        /// We need to propogate the change to our children and parents as necessary
        /// </summary>
        private void fieldsTree_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {
            // Disable checking node states while we play around with them
            fieldsTree.AfterCheck -= fieldsTree_AfterCheck;

            // Update check state for parents / children of this node
            UpdateStates(e.TreeNode);

            // Update our parent(s) state
            VerifyParentNodeCheckState(e.TreeNode);

            // Re-establish the AfterCheck event
            fieldsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.fieldsTree_AfterCheck);

            // Inform interested parties of a change in the check state of the tree
            if (CheckChanged != null)
                CheckChanged(this, e.TreeNode);
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

            if ((forNode.Nodes.Count == 0) && (forNode.CheckedState == CheckState.Indeterminate))
                forNode.CheckedState = CheckState.Unchecked;
        }



        /// <summary>
        /// Verify the check state of the parents of the specified child node
        /// </summary>
        /// <param name="childNode"></param>
        private void VerifyParentNodeCheckState(UltraTreeNode childNode)
        {
            // get the parent node and return if it is the root node (top of tree)
            UltraTreeNode parentNode = childNode.Parent;
            if (parentNode == _rootNode)
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
                if (parentNode.Parent != _rootNode)
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
        private CheckState GetOverallChildState(UltraTreeNode parentNode)
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
                for (int index = 1; index < parentNode.Nodes.Count; index++)
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


        #endregion tree Control Handlers



        /// <summary>
        /// This function is called to restore a previously saved set of checked asset groups and assets
        /// </summary>
        /// <param name="selectAll"></param>
        /// <param name="listAssetGroups"></param>
        /// <param name="listAssets"></param>
        public void RestoreSelections()
        {
            // begin updating the tree
            fieldsTree.BeginUpdate();

            // We iterate through each of the selected items
            foreach (string checkedItem in _listSelectedItems)
            {
                // Find the node containing this group
                UltraTreeNode node = FindItem(_rootNode, checkedItem);
                if (node != null)
                    node.CheckedState = CheckState.Checked;
            }

            // end updating the tree
            fieldsTree.EndUpdate();
        }


        /// <summary>
        /// Find a specific item in the tree
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected UltraTreeNode FindItem(UltraTreeNode parentNode, string item)
        {
            UltraTreeNode node = null;

            string[] fields = item.Split('|');
            string ab = "";

            foreach (string field in fields)
            {
                node = fieldsTree.GetNodeByKey(ab + field);

                if (node != null && node.Nodes.Count == 0)
                {
                    switch (fields[0])
                    {
                        case AWMiscStrings.AssetDetails:
                            PopulateAssetDetails(node);
                            break;
                        case AWMiscStrings.OSNode:
                            PopulateOperatingSystems(node);
                            break;
                        case AWMiscStrings.ApplicationsNode:

                            if (node.FullPath.StartsWith("Report Data Fields\\Applications\\"))
                                PopulateApplications(node);
                            else
                                PopulatePublishers(node);

                            break;
                        case AWMiscStrings.HardwareNode:
                            PopulateHardware(node);
                            break;
                        case AWMiscStrings.SystemNode:
                            PopulateHardware(node);
                            break;
                        case AWMiscStrings.UserDataNode:
                            PopulateUserDataDetails(node);
                            break;
                    }
                }

                ab += field + "|";
            }

            return node;

            //if ((parentNode.FullPath.StartsWith("Report Data Fields\\System\\")) || (parentNode.FullPath.StartsWith("Report Data Fields\\Hardware\\")))
            //{
            //    PopulateHardware(parentNode);
            //}

            //// Does the key for this node match the beginning of the item passed in
            //if ((parentNode.Parent == null) || (item.StartsWith(parentNode.Key)))
            //{
            //    // We're going down the right branch - iterate the children of this node
            //    foreach (UltraTreeNode node in parentNode.Nodes)
            //    {
            //        // Is this an exact match?
            //        // treat Applications differently as they have their ApplicationID appended
            //        if (item.StartsWith("Applications|"))
            //        {
            //            if (node.Key.StartsWith(item))
            //            {
            //                foundNode = node;
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            if (node.Key == item)
            //            {
            //                foundNode = node;
            //                break;
            //            }
            //        }

            //        // No - check the descendants
            //        foundNode = FindItem(node, item);
            //        if (foundNode != null)
            //            break;
            //    }
            //}

            //return foundNode;
        }


        /// <summary>
        /// Called as we select a new node - pass this on to the caller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fieldsTree_AfterSelect(object sender, SelectEventArgs e)
        {
            if (AfterSelect != null)
            {
                PopulateNode(e.NewSelections[0]);
                AfterSelect(this, e.NewSelections[0]);
            }
        }

        private void networkTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            PopulateNode(e.TreeNode);
        }

        private void PopulateNode(UltraTreeNode node)
        {
            try
            {
                string nodePath = node.FullPath;

                if (node.Nodes.Count > 0)
                    return;

                if ((nodePath.StartsWith("Report Data Fields\\System\\")) || (nodePath.StartsWith("Report Data Fields\\Hardware\\")))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    PopulateHardware(node);
                    Cursor.Current = Cursors.Default;
                }
                else if (nodePath.StartsWith("Report Data Fields\\Applications\\"))
                {
                    PopulateApplications(node);
                }
                else if (nodePath.StartsWith("Report Data Fields\\"))
                {
                    node.Nodes.Clear();

                    switch (nodePath.Substring(nodePath.IndexOf("\\") + 1))
                    {
                        case AWMiscStrings.AssetDetails:
                            PopulateAssetDetails(node);
                            break;
                        case AWMiscStrings.OSNode:
                            PopulateOperatingSystems(node);
                            break;
                        case AWMiscStrings.ApplicationsNode:
                            PopulatePublishers(node);
                            break;
                        case AWMiscStrings.HardwareNode:
                            PopulateHardware(node);
                            break;
                        case AWMiscStrings.SystemNode:
                            PopulateHardware(node);
                            break;
                        case AWMiscStrings.UserDataNode:
                            PopulateUserDataDetails(node);
                            break;
                        default:
                            break;
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
