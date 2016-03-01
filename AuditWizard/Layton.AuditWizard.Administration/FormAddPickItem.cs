using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;
using Infragistics.Win.UltraWinExplorerBar;

namespace Layton.AuditWizard.Administration
{
    public partial class FormAddPickItem : ShadedImageForm
    {
        #region Data
		protected PickItem _pickItem = null;
        private UltraExplorerBarItem m_activeItem = null;
		#endregion Data

		#region Properties
		public PickItem PickItem
		{
            get { return _pickItem; }
            set { _pickItem = value; }
		}

		#endregion Properties

		#region Constructor		

        public FormAddPickItem(PickItem pickitem,UltraExplorerBarItem activeItem): this()
		{
            m_activeItem=activeItem;
            _pickItem = pickitem;
            if (pickitem.PickItemID == 0)
			{
                this.Text = "New PickItem";
                this.footerPictureBox.Image = Properties.Resources.pickitem_add_corner;
			}
			else
			{
				this.Text = "Pick Item Properties";
                this.footerPictureBox.Image = Properties.Resources.pickitem_add_corner;
			}
			//
            this.tbPickItemName.Text = _pickItem.Name;
		}		

        public FormAddPickItem()
        {
            InitializeComponent();
        }

        #endregion Constructor

        private void bnOK_Click(object sender, EventArgs e)
        {                    
			
            string strPickItemName = tbPickItemName.Text.ToString();

			// If the name is blank then it will be invalid
			if (strPickItemName == "")
			{
                MessageBox.Show("Please specify a unique name for this pickitem", "Enter Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPickItemName.Focus();
                this.DialogResult = DialogResult.None;
                return;
			}

			// Get the picklist so we check it's items
			PickList picklist = m_activeItem.Tag as PickList;

			// Does the specified value duplicate an existing pickitem?
            foreach (PickItem item in picklist)
            {

                if ((item.Name == strPickItemName) && (item.PickItemID != _pickItem.PickItemID))
                {
                    MessageBox.Show(
                        "The specified name matches that of an existing item in this picklist, please specify a unique name for this pickitem",
                        "Duplicate Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    tbPickItemName.Focus();
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
            // If it's a new pickitem then we need to add it to the parent list also
            if (_pickItem.PickItemID == 0)
                picklist.Add(_pickItem);

            // Update the PickItem in the database
            _pickItem.Name = strPickItemName;
            _pickItem.Update();

        }
    }
}
