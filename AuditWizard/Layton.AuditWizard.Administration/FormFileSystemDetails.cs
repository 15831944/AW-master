using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormFileSystemDetails : Layton.Common.Controls.ShadedImageForm
	{
		// Textbox used for editing items
		TextBox _tbEditor;
		private int _itemSelected = -1;
		private bool _Updated = false;
		private ListBox _listBoxEdited = null;
		//
		AuditScannerDefinition _scannerConfiguration;

		public FormFileSystemDetails(AuditScannerDefinition scannerConfiguration)
		{
			InitializeComponent();
			_scannerConfiguration = scannerConfiguration;

			// Create the edit box for the folders/files list box that will allow us to add a new item and
			// edit the text of an existing item
			_tbEditor = new TextBox();
			_tbEditor.Location = new Point(0, 0);
			_tbEditor.Size = new Size(0, 0);
			_tbEditor.Hide();

			// Set the tags for the radio buttons
			this.rbFoldersAll.Tag = (int)AuditScannerDefinition.eFolderSetting.allFolders;
			this.rbFoldersNone.Tag = (int)AuditScannerDefinition.eFolderSetting.noFolders;
			this.rbFoldersSpecified.Tag = (int)AuditScannerDefinition.eFolderSetting.specifiedFolders;
			//
			this.rbFilesAll.Tag = (int)AuditScannerDefinition.eFileSetting.allFiles;
			this.rbFilesExecutables.Tag = (int)AuditScannerDefinition.eFileSetting.allExeFiles;
			this.rbFilesNone.Tag = (int)AuditScannerDefinition.eFileSetting.noFiles;
			this.rbFilesSpecified.Tag = (int)AuditScannerDefinition.eFileSetting.specifiedFiles;
		}


		/// <summary>
		/// Initialize the form for display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormFileSystemDetails_Load(object sender, EventArgs e)
		{
			// Folder scanning
			switch ((int)_scannerConfiguration.ScanFolders)
			{
				case (int)AuditScannerDefinition.eFolderSetting.noFolders:
					this.rbFoldersNone.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFolderSetting.allFolders:
					this.rbFoldersAll.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFolderSetting.specifiedFolders:
					this.rbFoldersSpecified.Checked = true;
					break;
			}

			// Load the folder list
			List<string> listFolders = _scannerConfiguration.ListFolders;
			foreach (string folder in listFolders)
			{
				lbFolders.Items.Add(folder);
			}

			// File scanning
			switch ((int)_scannerConfiguration.ScanFiles)
			{
				case (int)AuditScannerDefinition.eFileSetting.noFiles:
					this.rbFilesNone.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFileSetting.allFiles:
					this.rbFilesAll.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFileSetting.allExeFiles:
					this.rbFilesExecutables.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFileSetting.specifiedFiles:
					this.rbFilesSpecified.Checked = true;
					break;
			}

			// Load the File list
			List<string> listFiles = _scannerConfiguration.ListFiles;
			foreach (string file in listFiles)
			{
				lbFiles.Items.Add(file);
			}
		}

		#region Folder Handling

		/// <summary>
		/// Called as the folder radio button changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rbFolders_CheckedChanged(object sender, EventArgs e)
		{
			// The list box and associated buttons are only available if we select 'Specified Folders'
			this.panelFolders.Enabled = (this.rbFoldersSpecified.Checked);
		}

		/// <summary>
		/// Called as we click on the 'Add' button to allow a new folder to be defined
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddFolder_Click(object sender, EventArgs e)
		{
			int nIndex = lbFolders.Items.Add(@"C:\");
			lbFolders.SelectedIndex = nIndex;
			CreateEditBox(lbFolders);
		}

		private void bnDeleteFolder_Click(object sender, EventArgs e)
		{
			// Confirm that the user wants to do this
			if (MessageBox.Show("Are you sure that you want to delete these folder(s)?", "Confirm Delete", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// OK Confirmed so do it
			while (lbFolders.SelectedIndices.Count > 0)
			{
				int selectedItemIndex = lbFolders.SelectedIndices[lbFolders.SelectedIndices.Count - 1];
				lbFolders.Items.RemoveAt(selectedItemIndex);
			}
		}


		/// <summary>
		/// Called as we select edit - mimic a double click as this performs the edit function
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnEditFolder_Click(object sender, EventArgs e)
		{
			if (lbFolders.SelectedItems.Count == 1)
				CreateEditBox(lbFolders);
		}


		/// <summary>
		/// Called as we hold a key down in the Folders List Box. if we press F2 then we start editing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFolders_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2)
				CreateEditBox(lbFolders);
		}


		/// <summary>
		/// Called as we press a key in the Foldes list box, enter causes the entry to be edited
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFolders_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
				CreateEditBox(lbFolders);
		}


		/// <summary>
		/// Double clicking an item edits it in the folders list box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFolders_DoubleClick(object sender, EventArgs e)
		{
			Point ptClick = this.lbFolders.PointToClient(MousePosition);
			int nIndex = this.lbFolders.IndexFromPoint(ptClick);
			if (nIndex == ListBox.NoMatches)
			{
				bnAddFolder_Click(sender, e);
			}
			else
			{
				CreateEditBox(lbFolders);
			}
		}


		/// <summary>
		/// Called to enable/disable buttons for folders based on the selection in the folders list box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFolders_SelectedIndexChanged(object sender, EventArgs e)
		{
			// If we have at least one selected item then delete is valid
			this.bnDeleteFolder.Enabled = (this.lbFolders.SelectedItems.Count != 0);

			// We can only edit if we have exactly one item selected
			this.bnEditFolder.Enabled = (this.lbFolders.SelectedItems.Count == 1);
		}

		#endregion Folder Handling

		#region File Handling

		private void rbFiles_CheckedChanged(object sender, EventArgs e)
		{
			// The list box and associated buttons are only available if we select 'Specified Files'
			this.panelFiles.Enabled = (this.rbFilesSpecified.Checked);
		}

		/// <summary>
		/// Add a new file to the list by displaying the edit box which allows the user to create the entry
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddFile_Click(object sender, EventArgs e)
		{
			int nIndex = lbFiles.Items.Add("*.xxx");
			lbFiles.SelectedIndex = nIndex;
			CreateEditBox(lbFiles);
		}


		/// <summary>
		/// Called to delete entries from the Files listbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDeleteFile_Click(object sender, EventArgs e)
		{
			// Confirm that the user wants to do this
			if (MessageBox.Show("Are you sure that you want to delete these file specification(s)?", "Confirm Delete", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// OK Confirmed so do it
			while (lbFiles.SelectedIndices.Count > 0)
			{
				int selectedItemIndex = lbFiles.SelectedIndices[lbFiles.SelectedIndices.Count - 1];
				lbFiles.Items.RemoveAt(selectedItemIndex);
			}
		}


		/// <summary>
		/// Called as we select edit - mimic a double click as this performs the edit function
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnEditFile_Click(object sender, EventArgs e)
		{
			CreateEditBox(lbFiles);
		}


		/// <summary>
		/// Called as we hold down a key in the Files List Box. if we press F2 then we start editing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFiles_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2)
				CreateEditBox(lbFiles);
		}


		/// <summary>
		/// Called as we press a key in the Files list box, enter causes the entry to be edited
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFiles_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
				CreateEditBox(lbFiles);
		}


		/// <summary>
		/// Double clicking an entry in the files list box edits it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFiles_DoubleClick(object sender, EventArgs e)
		{
			Point ptClick = this.lbFiles.PointToClient(MousePosition);
			int nIndex = this.lbFiles.IndexFromPoint(ptClick);
			if (nIndex == ListBox.NoMatches)
			{
				bnAddFile_Click(sender, e);
			}
			else
			{
				CreateEditBox(lbFiles);
			}
		}


		/// <summary>
		/// Called to enable/disable buttons based on the current items selected in the files listbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			// If we have at least one selected item then delete is valid
			this.bnDeleteFile.Enabled = (this.lbFiles.SelectedItems.Count != 0);

			// We can only edit if we have exactly one item selected
			this.bnEditFile.Enabled = (this.lbFiles.SelectedItems.Count == 1);
		}

		#endregion File Handling

		#region Editor TextBox Handlers

		/// <summary>
		/// CreateEditBox
		/// =============
		/// 
		/// Create the text box that will be displayed in the list box on top of the currently selected
		/// item to allow the textual name to be modified
		/// 
		/// </summary>
		/// <param name="lbEditing"></param>
		private void CreateEditBox(ListBox listBoxEdited)
		{
			// Get the item selected
			_listBoxEdited = listBoxEdited;
			_itemSelected = listBoxEdited.SelectedIndex;
			if (_itemSelected == -1)
				return;

			// Flag that we may need to update
			_Updated = false;

			Rectangle r = listBoxEdited.GetItemRectangle(_itemSelected);
			string itemText = (string)listBoxEdited.Items[_itemSelected];

			_tbEditor.Location = new Point(r.X + 5, r.Y);
			_tbEditor.Size = new Size(r.Width - 10, r.Height);
			_tbEditor.Show();
			listBoxEdited.Controls.AddRange(new Control[] { this._tbEditor });
			_tbEditor.Text = itemText;
			_tbEditor.Focus();
			_tbEditor.SelectAll();
			_tbEditor.KeyPress += new KeyPressEventHandler(this.EditOver);
			_tbEditor.LostFocus += new EventHandler(this.FocusOver);
		}


		/// <summary>
		/// FocusOver
		/// =========
		/// 
		/// Called when we move off the editable text box to complete the changing of the item name
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FocusOver(object sender, EventArgs e)
		{
			UpdateListItem();
		}


		/// <summary>
		/// EditOver
		/// ========
		/// 
		/// Called when we have finished editing the name of the list box item
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditOver(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				UpdateListItem();
			}
		}


		/// <summary>
		/// UpdateListItem
		/// ==============
		/// 
		/// Called when-ever we modify the name of a website to reflect the 
		/// change into the listbox, the stored object and the database
		/// 
		/// </summary>
		private void UpdateListItem()
		{
			// If we have already updated/created this item then return as no point in doing it twice
			if (_Updated)
				return;

			// Update the listbox displayed text
			this._listBoxEdited.Items[_itemSelected] = _tbEditor.Text;

			// Flag updated to prevent this from being called again unnecessarily
			_Updated = true;
			_tbEditor.Hide();
		}


#endregion Editor TextBox Handlers


		/// <summary>
		/// Called as we want to exit from the form - save any folder/files defined as required
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// First save the Folder Scan Setting
			if (rbFoldersAll.Checked)
				_scannerConfiguration.ScanFolders = AuditScannerDefinition.eFolderSetting.allFolders;
			else if (rbFoldersNone.Checked)
				_scannerConfiguration.ScanFolders = AuditScannerDefinition.eFolderSetting.noFolders;
			else
				_scannerConfiguration.ScanFolders = AuditScannerDefinition.eFolderSetting.specifiedFolders;

			// File Scan Settings
			if (this.rbFilesAll.Checked)
				_scannerConfiguration.ScanFiles = AuditScannerDefinition.eFileSetting.allFiles;
			else if (this.rbFilesExecutables.Checked)
				_scannerConfiguration.ScanFiles = AuditScannerDefinition.eFileSetting.allExeFiles;
			else if (this.rbFilesNone.Checked)
				_scannerConfiguration.ScanFiles = AuditScannerDefinition.eFileSetting.noFiles;
			else
				_scannerConfiguration.ScanFiles = AuditScannerDefinition.eFileSetting.specifiedFiles;

			// Save folders and files where set to specified
			if (rbFoldersSpecified.Checked)
			{
				_scannerConfiguration.ListFolders.Clear();
				foreach (string folder in lbFolders.Items)
				{
					_scannerConfiguration.ListFolders.Add(folder);
				}
			}

			if (rbFilesSpecified.Checked)
			{
				_scannerConfiguration.ListFiles.Clear();
				foreach (string file in lbFiles.Items)
				{
					_scannerConfiguration.ListFiles.Add(file);
				}
			}
		}
	}
}