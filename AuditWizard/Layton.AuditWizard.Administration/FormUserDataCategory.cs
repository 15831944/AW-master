using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    public partial class FormUserDataCategory : AWForm
    {
        private readonly UserDataCategory _category;
        private readonly bool _editing;

        public FormUserDataCategory(UserDataCategory category, bool editing)
        {
            InitializeComponent();
            _category = category;
            _editing = editing;

            SetHeaderText();

            tbCategoryName.Text = category.Name;
            tbAppliesTo.Text = category.AppliesToName;
            tbIconFile.Text = category.Icon;

            // If we are NOT scope asset then we disable the 'Applies To' fields
            lblAppliesTo.Enabled = (_category.Scope == UserDataCategory.SCOPE.Asset);
            tbAppliesTo.Enabled = (_category.Scope == UserDataCategory.SCOPE.Asset);
            bnSelect.Enabled = (_category.Scope == UserDataCategory.SCOPE.Asset);
        }

        private void SetHeaderText()
        {
            Text = _category.CategoryID == 0 ? "New User Data Category" : "User Data Category Properties";
            Text += " [" + _category.ScopeAsString + "]";
        }


        /// <summary>
        /// This function is called when we click to select an asset type which is to be associated with this
        /// user data category.  We can either select a class of asset or a specific asset type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnSelect_Click(object sender, EventArgs e)
        {
            FormSelectAssetType form = new FormSelectAssetType(_category.AppliesTo, _category.AppliesToName);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _category.AppliesToDetails(form.SelectedAssetType, form.SelectedAssetTypeName);
                tbAppliesTo.Text = _category.AppliesToName;
            }
        }

        /// <summary>
        /// Called as we browse for a new image file to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowse_Click(object sender, EventArgs e)
        {
            string iconsFolder = Path.Combine(Application.StartupPath, "Icons");
            openFileDialog.InitialDirectory = iconsFolder;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path as if this is NOT within the ICONS folder then we will need to copy the
                // specified file from it's current location into the icons folder
                string iconName = openFileDialog.FileName;
                string iconPath = Path.GetDirectoryName(iconName);
                string iconFileName = Path.GetFileName(iconName);

                if (iconPath != iconsFolder)
                {
                    try
                    {
                        File.Copy(iconName, Path.Combine(iconsFolder, iconFileName));
                    }
                    catch (IOException)
                    {
                        // an IOException likely refers to file already existing so we can safely ignore this exception
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to copy selected icon into the programs icon folder, reason: " + ex.Message, "Copy Failed");
                        return;
                    }
                }

                // Set the name of the new file in the display
                tbIconFile.Text = iconFileName;
            }
        }


        /// <summary>
        /// Called as the displayed text changes as this should cause the preview icon to change also
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbIconFile_TextChanged(object sender, EventArgs e)
        {
            if (tbIconFile.Name == "")
                return;

            // Format the name of the icon to be displayed and display it - if we cannot find the image then we
            // will display a generic internal image instead just in case something has been deleted
            string iconsFolder = Path.Combine(Application.StartupPath, "Icons");
            string iconName = Path.Combine(iconsFolder, tbIconFile.Text);

            // Does the image exist
            pbIcon.Image = File.Exists(iconName) ? new Bitmap(iconName) : Properties.Resources.missing_icon_32;
        }


        /// <summary>
        /// Called as we try an exit from this form - we need to ensure that the category name is unique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnOK_Click(object sender, EventArgs e)
        {
            // Update the asset type definition with any changes
            _category.Name = tbCategoryName.Text;
            _category.Icon = tbIconFile.Text;

            // Ensure that this Category does not duplicate an existing one
            UserDataCategoryList listCategories = new UserDataCategoryList(_category.Scope);
            listCategories.Populate();
            UserDataCategory existingCategory = listCategories.FindCategory(_category.Name);
            if ((existingCategory != null) && (existingCategory.CategoryID != _category.CategoryID))
            {
                MessageBox.Show("A User Defined Data Category with this name already exists, please enter a unique name for this category", "Category Exists");
                DialogResult = DialogResult.None;
                return;
            }

            if (!_editing)
                _category.TabOrder = new UserDataDefinitionsDAO().GetCountUserDataCategories(UserDataCategory.SCOPE.Asset);

            // ...Update or add the category
            _category.Add();
        }
    }
}