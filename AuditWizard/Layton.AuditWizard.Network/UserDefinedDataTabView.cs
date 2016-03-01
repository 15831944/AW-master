using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win;
using Infragistics.Win.Layout;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class UserDefinedDataTabView : UserControl, ILaytonView
    {
        #region Data
        UserDefinedDataTabViewPresenter presenter;
        LaytonWorkItem workItem;
        private Asset _displayedAsset;
        private UserDataCategory _displayedCategory;

        #endregion Data

        #region Data Accessors
        [CreateNew]
        public UserDefinedDataTabViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
            get { return presenter; }
        }

        public string HeaderText
        {
            set
            {
                headerLabel.Text = value;
            }
        }

        public Image HeaderImage
        {
            set { headerLabel.Appearance.Image = value; }
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        #endregion Data Accessors

        #region Constructor
        [InjectionConstructor]
        public UserDefinedDataTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

        #endregion

        #region Form Control Functions
        /// <summary>
        /// Handle the re-sizing of the header box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void headerGroupBox_SizeChanged(object sender, EventArgs e)
        {
        }

        public void RefreshView()
        {
            presenter.Initialize();
            base.Refresh();
        }

        #endregion Form Control Functions


        /// <summary>
        /// </summary>
        /// <param name="displayedNode">The selected UltraTreeNode for which we shall be displaying details 
        /// The Tag property holds the Asset being displayed</param>
        /// <param name="displayedCategory">The uSer Defined Data Category to be displayed</param>
        public void Display(UltraTreeNode displayedNode, UserDataCategory displayedCategory)
        {
            _displayedCategory = displayedCategory;
            HeaderText = _displayedCategory.Name;

            // Single or ALL assets?
            if (displayedNode.Tag is UserDataCategory)
                DisplayForAsset(displayedNode);

            else
                DisplayForAllAssets(displayedNode);

            UltraGridColumn gridItemColumn = this.auditGridView.DisplayLayout.Bands[0].Columns[0];
            gridItemColumn.Width = 200;
        }


        /// <summary>
        /// Display user data fields for a specific asset
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAsset(UltraTreeNode displayedNode)
        {
            _displayedAsset = _displayedCategory.Tag as Asset;

            // Get the values for the user defined data fields for this category
            _displayedCategory.GetValuesFor(_displayedAsset.AssetID);

            // Now add the field names and values to the dataset
            auditDataSet.Clear();
            foreach (UserDataField field in _displayedCategory)
            {
                string fieldValue = field.GetValueFor(_displayedAsset.AssetID);
                auditDataSet.Tables[0].Rows.Add(new object[] { field.Name, fieldValue });
            }

            // Set the default icon to be the icon for the category
            //auditGridView.DisplayLayout.Bands[0].Columns[0].CellAppearance.Image = IconMapping.LoadIcon(_displayedCategory.Icon, IconMapping.ICONSIZE.small);
        }




        /// <summary>
        /// Display user data fields for the All Assets node
        /// </summary>
        /// <param name="displayedNode"></param>
        protected void DisplayForAllAssets(UltraTreeNode displayedNode)
        {


        }



        /// <summary>
        /// Called to edit the user data values for this asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editUserDataFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserDataEdit form = new FormUserDataEdit(_displayedAsset, _displayedCategory);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshView();
        }

    }
}
