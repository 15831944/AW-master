using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormPicklist : ShadedImageForm
	{
		#region Data
		protected PickList _pickList = null;
		#endregion Data

		#region Properties
		public PickList Picklist
		{
			get { return _pickList; }
			set { _pickList = value; }
		}

		#endregion Properties

		#region Constructor

		public FormPicklist()
		{
			InitializeComponent();
		}

		public FormPicklist (PickList picklist) : this()
		{
			_pickList = picklist;
			if (picklist.PicklistID == 0)
			{
				this.Text = "New PickList";
				this.footerPictureBox.Image = Properties.Resources.picklist_add_corner;
			}
			else
			{
				this.Text = "PickList Properties";
				this.footerPictureBox.Image = Properties.Resources.picklist_add_corner;
			}
			//
			this.tbPickListName.Text = _pickList.Name;
		}

		#endregion Constructor

		/// <summary>
		/// Called as we try to save the picklist - ensure that the name specified does not duplicate an
		/// existing item and if OK save back to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{

            // If the name is blank then it will be invalid
            if (this.tbPickListName.Text == "")
            {
                MessageBox.Show("Please specify a unique name for this picklist", "Enter Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPickListName.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

			// Does the specified value dupliacte an existing picklist?
			PickListList listPickLists = new PickListList();
			listPickLists.Populate();
			//
			foreach (PickList list in listPickLists)
			{
				if (list.Name == this.tbPickListName.Text)
				{
					// OK the names duplicate but is this in fact the same entry?
					if (list.PicklistID == _pickList.PicklistID)
						continue;

					// Nope the name duplicates an existing entry so we can't save it
					MessageBox.Show("The specified name matches that of an existing picklist, please specify a unique name for this picklist" ,"Invalid PickList Name");
					tbPickListName.Focus();
					this.DialogResult = DialogResult.None;
					return;
				}
			}
			
			// All OK - update the picklist here and in the database and also any related user-defined data fields
            UserDataCategoryList listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Any);
            listCategories.Populate();

            foreach (UserDataCategory category in listCategories)
            {
                foreach (UserDataField field in category)
                {
                    if ((field.Type == UserDataField.FieldType.Picklist) && (field.Value1 == _pickList.Name))
                    {
                        field.Value1 = tbPickListName.Text;
                        field.Update();
                    }
                }
            }

			_pickList.Name = tbPickListName.Text;
			_pickList.Update();
		}

	}

}

