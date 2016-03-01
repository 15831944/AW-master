using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinListView;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormUserLocation : ShadedImageForm
	{
		private AssetGroup _parentLocation;
		private AssetGroup _location;
		public AssetGroup UserLocation
		{
			get { return _location; }
		}

		public FormUserLocation(AssetGroup parentLocation, AssetGroup theLocation)
		{
			InitializeComponent();

			_parentLocation = parentLocation;
			_location = theLocation;

			// If this entry is null or the InstanceID is 0 then we are creating a new Location type
			if (_location == null)
			{
				_location = new AssetGroup();
				_location.GroupType = AssetGroup.GROUPTYPE.userlocation;
			}

			this.Text = (_location.GroupID == 0) ? "New Location" : "Edit Location";
			tbParent.Text = (_parentLocation ==  null) ? "" : _parentLocation.FullName;
			tbChild.Text = (_location.GroupID == 0) ? "New Location" : _location.Name;

			// Add (any) IP address ranges to the list
			string[] startIpAddresses = _location.StartIP.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			string[] endIpAddresses = _location.EndIP.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

			ulvTcpRanges.BeginUpdate();
			for (int isub = 0; isub < startIpAddresses.Length; isub++)
			{
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = endIpAddresses[isub];
				UltraListViewItem item = new UltraListViewItem(startIpAddresses[isub], subItemArray);
				ulvTcpRanges.Items.Add(item);
			}
			ulvTcpRanges.EndUpdate();
		}


		/// <summary>
		/// Called as want to exit from this form potentially saving the definition or updating
		/// an existing entry
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			if (tbChild.Text == "")
			{
				MessageBox.Show("You must enter a name for this new Location");
				DialogResult = DialogResult.None;
				return;
			}

			// If we are creating a new Location then we must ensure that we have specified a 
			// name which does not already exist for the parent
			if (_parentLocation != null)
			{
				AssetGroup existingLocation = _parentLocation.IsChildGroup(tbChild.Text);
				if (existingLocation != null)
				{
					// OK this named location exists - is it the same location though
					if (existingLocation.GroupID != _location.GroupID)
					{
						MessageBox.Show("The Location " + tbChild.Text + " already exists at this level.  Please enter a different name for this new Location", "Duplicate Name");
						DialogResult = DialogResult.None;
						return;
					}
				}
			}

			// OK all valid so add this new entry (or update it if already exists)
			LocationsDAO lwDataAccess = new LocationsDAO();
			_location.Name = tbChild.Text;

			// Set the parent name noting that this is the same as our name if we are at the top of the 
			// tree and have no parent
			if (_parentLocation == null)
				_location.FullName = _location.Name;
			else
				_location.FullName = _parentLocation.FullName + @"\" + _location.Name;

			// Create '|' delimited strings for the starting and ending IP addresses
			string startIP = "";
			string endIP = "";
			foreach (UltraListViewItem item in ulvTcpRanges.Items)
			{
				if (startIP != "")
					startIP += "|";
				if (endIP != "")
					endIP += "|";
				startIP += item.Text;
				endIP += item.SubItems[0].Text;
			}
			_location.StartIP = startIP;
			_location.EndIP = endIP;


			// Add or replace the location
			_location.GroupID = lwDataAccess.GroupAdd(_location);
		}

	

		/// <summary>
		/// Called to add a new IP address range to the list displayed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAdd_Click(object sender, EventArgs e)
		{
			FormIPAddressRange form = new FormIPAddressRange("", "");
			if (form.ShowDialog() == DialogResult.OK)
			{
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = form.Upper;
				UltraListViewItem item = new UltraListViewItem(form.Lower, subItemArray);
				ulvTcpRanges.Items.Add(item);
			}
		}


		/// <summary>
		/// Called to delete 1 or more IP address ranges from the list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnRemove_Click(object sender, EventArgs e)
		{
			// Confirm the user really wants to delete the IP address ranges selected
			if (MessageBox.Show("Are you sure that you want to delete the delected IP address range(s)?" ,"Confirm Delete" ,MessageBoxButtons.YesNo) == DialogResult.No)
				return;

            UltraListViewItem item;
            object[] selectedItems = ulvTcpRanges.SelectedItems.All;

            ulvTcpRanges.BeginUpdate();

            for (int i = 0; i < selectedItems.Length; i++)
            {
                item = selectedItems[i] as UltraListViewItem;
                ulvTcpRanges.Items.Remove(item);
            }

            ulvTcpRanges.EndUpdate();
		}


		/// <summary>
		/// Called as the selection changes in the list. Enable or disable menu items as appropriate 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvTcpRanges_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			bnEdit.Enabled = (ulvTcpRanges.SelectedItems.Count == 1);
			bnRemove.Enabled = (ulvTcpRanges.SelectedItems.Count != 0);
		}


		/// <summary>
		/// Called to edit an existing IP address range
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnEdit_Click(object sender, EventArgs e)
		{
			// Sanity check ensure one item selected and only one
			if (ulvTcpRanges.SelectedItems.Count != 1)
				return;

			// get the selected item
			UltraListViewItem selectedItem = ulvTcpRanges.SelectedItems[0];
			FormIPAddressRange form = new FormIPAddressRange(selectedItem.Text, selectedItem.SubItems[0].Text);
			if (form.ShowDialog() == DialogResult.OK)
			{
				selectedItem.Value = form.Lower;
				selectedItem.SubItems[0].Value = form.Upper;
			}
		}	
	}
}