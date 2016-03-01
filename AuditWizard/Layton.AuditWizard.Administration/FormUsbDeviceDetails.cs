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
	public partial class FormUsbDeviceDetails : Layton.Common.Controls.ShadedImageForm
	{
		// Textbox used for editing items
		TextBox _tbEditor;
		private int _itemSelected = -1;
		private bool _Updated = false;
		private ListBox _listBoxEdited = null;
		//
		AuditScannerDefinition _scannerConfiguration;

		public FormUsbDeviceDetails(AuditScannerDefinition scannerConfiguration)
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
			this.rbFilesAll.Tag = (int)AuditScannerDefinition.eFileSetting.allFiles;
			this.rbFilesNone.Tag = (int)AuditScannerDefinition.eFileSetting.noFiles;
			this.rbFilesSpecified.Tag = (int)AuditScannerDefinition.eFileSetting.specifiedFiles;
		}


		/// <summary>
		/// Initialize the form for display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormUsbDeviceDetails_Load(object sender, EventArgs e)
		{
			// Usb Device File scanning
			switch ((int)_scannerConfiguration.ScanUSBFiles)
			{
				case (int)AuditScannerDefinition.eFileSetting.noFiles:
					this.rbFilesNone.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFileSetting.allFiles:
					this.rbFilesAll.Checked = true;
					break;

				case (int)AuditScannerDefinition.eFileSetting.specifiedFiles:
					this.rbFilesSpecified.Checked = true;
					break;
			}

			// Load the File list
			List<string> listFiles = _scannerConfiguration.ListUSBFiles;
			foreach (string file in listFiles)
			{
				lbFiles.Items.Add(file);
			}
		}

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
			lbFiles_DoubleClick(sender, e);
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
			// File Scan Settings
			if (this.rbFilesAll.Checked)
				_scannerConfiguration.ScanUSBFiles = AuditScannerDefinition.eFileSetting.allFiles;
			else if (this.rbFilesNone.Checked)
				_scannerConfiguration.ScanUSBFiles = AuditScannerDefinition.eFileSetting.noFiles;
			else
				_scannerConfiguration.ScanUSBFiles = AuditScannerDefinition.eFileSetting.specifiedFiles;

			if (rbFilesSpecified.Checked)
			{
				_scannerConfiguration.ListUSBFiles.Clear();
				foreach (string file in lbFiles.Items)
				{
					_scannerConfiguration.ListUSBFiles.Add(file);
				}
			}
		}
	}
}