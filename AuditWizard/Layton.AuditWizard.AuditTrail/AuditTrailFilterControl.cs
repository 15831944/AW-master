using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditTrail
{
    public partial class AuditTrailFilterControl : UserControl
    {

		/// <summary>This holds the current filter settings - we are only interested however in the start
		/// and end date</summary>
		private AuditTrailFilterEventArgs _auditTrailFilterEventArgs = new AuditTrailFilterEventArgs();
		
		public AuditTrailFilterControl()
        {
            InitializeComponent();
            RefreshData(null);
        }

        /// <summary>
        /// Refreshes the list of values in the 'Filter by Users', 'Filter by Computers' 
		/// and 'Filter by Application' comboboxes.
        /// </summary>
        public void RefreshData(AuditTrailFilterEventArgs e)
        {
			if (e != null)
			{
				// If neither the date, time or class has changed then we can ignore this refresh request
				if ((_auditTrailFilterEventArgs.StartDate == e.StartDate)
				&&  (_auditTrailFilterEventArgs.EndDate == e.EndDate)
				&&  (_auditTrailFilterEventArgs.RequiredClass == e.RequiredClass))
					return;

				_auditTrailFilterEventArgs = e;
			}

			// As this is a filter we need to pickup the entire list of audit trail entries from which
			// we can build the filter lists
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();
			DataTable ateTable = lwDataAccess.GetAuditTrailEntries((int)_auditTrailFilterEventArgs.RequiredClass);

			// Get any filter dates
			DateTime startDate = _auditTrailFilterEventArgs.StartDate;
			DateTime endDate = _auditTrailFilterEventArgs.EndDate;

			// Now iterate through the rows returned and add data for these entries to our filter combos
			// assuming that they are within date range
			applicationComboBox.BeginUpdate();
			computerComboBox.BeginUpdate();
			userComboBox.BeginUpdate();

			// Get (any) currently selected item so that we can try and restore the selection after the refresh
			string selectedApplication = "";
			string selectedComputer = "";
			string selectedUser = "";
			if (applicationComboBox.SelectedItem != null)
				selectedApplication = applicationComboBox.SelectedItem.DisplayText;
			if (computerComboBox.SelectedItem != null)
				selectedComputer = computerComboBox.SelectedItem.DisplayText;
			if (userComboBox.SelectedItem != null)
				selectedUser = userComboBox.SelectedItem.DisplayText;

			// Now clear the current contents
			applicationComboBox.Items.Clear();
			computerComboBox.Items.Clear();
			userComboBox.Items.Clear();
			
			// Add a 'blank' ie no filter entry to each combo
			applicationComboBox.Items.Add(MiscStrings.NoFilter);
			computerComboBox.Items.Add(MiscStrings.NoFilter);
			userComboBox.Items.Add(MiscStrings.NoFilter);

			// ...then add the real filters
			foreach (DataRow row in ateTable.Rows)
			{
				// ...and check date in range first
				AuditTrailEntry ate = new AuditTrailEntry(row);
				if (ate.Date.Date < startDate.Date || ate.Date.Date > endDate.Date)
					continue;

				// Add the Application to the applications combo if not already there
				if ((ate.Class == AuditTrailEntry.CLASS.application_installs)
				||  (ate.Class == AuditTrailEntry.CLASS.action) 
				||  (ate.Class == AuditTrailEntry.CLASS.license))
				{
					// The application name may have a '|' delimiter to split it off from an attribute (such as
					// notes) of the application which changed.  We need to split that off now to leave just
					// the application
					String application;
					if (ate.Key.Contains("|"))
						application = ate.Key.Substring(0 ,ate.Key.IndexOf("|"));
					else
						application = ate.Key;

                    if (applicationComboBox.Items.ValueList.FindByDataValue(application) == null)
						applicationComboBox.Items.Add(application, application);
				}
				
				// Add the computer to the combo box if not already there
                if ((ate.AssetName != "") && computerComboBox.Items.ValueList.FindByDataValue(ate.AssetName) == null)
                    computerComboBox.Items.Add(ate.AssetName, ate.AssetName);

				// Add the user to the combo box if not already there
                if ((ate.Username != "") && (userComboBox.Items.ValueList.FindByDataValue(ate.Username) == null))
                    userComboBox.Items.Add(ate.Username, ate.Username);
			}

			// Restore (any) selection or select the 'No Filter'
			if ((selectedUser == "") || (userComboBox.FindStringExact(selectedUser) == -1))
				userComboBox.SelectedIndex = 0;
			else
				userComboBox.SelectedIndex = (userComboBox.FindStringExact(selectedUser));
			//
			if ((selectedComputer == "") || (computerComboBox.FindStringExact(selectedComputer) == -1))
				computerComboBox.SelectedIndex = 0;
			else
				computerComboBox.SelectedIndex = (computerComboBox.FindStringExact(selectedComputer));
			//
			if ((selectedApplication == "") || (applicationComboBox.FindStringExact(selectedApplication) == -1))
				applicationComboBox.SelectedIndex = 0;
			else
				applicationComboBox.SelectedIndex = (applicationComboBox.FindStringExact(selectedApplication));

			// End update of the combo boxes
			userComboBox.EndUpdate();
			computerComboBox.EndUpdate();
			applicationComboBox.EndUpdate();
		}

        /// <summary>
        /// Event for when the 'Filter by User' value has changed
        /// </summary>
        public event EventHandler<DataEventArgs<string>> SelectedUserChanged;

        /// <summary>
        /// Fires the <see cref="SelectedUserChanged"/> event.
        /// </summary>
        /// <param name="userName">Name of the new selected user</param>
        internal void FireSelectedUserChanged(string userName)
        {
            if (SelectedUserChanged != null)
                SelectedUserChanged(this, new DataEventArgs<string>(userName));
        }

        /// <summary>
        /// Event for when the 'Filter by Asset' value has changed
        /// </summary>
        public event EventHandler<DataEventArgs<string>> SelectedComputerChanged;

        /// <summary>
        /// Fires the <see cref="SelectedComputerChanged"/> event.
        /// </summary>
        /// <param name="computerName">Name of the new selected computer</param>
        internal void FireSelectedComputerChanged(string computerName)
        {
            if (SelectedComputerChanged != null)
                SelectedComputerChanged(this, new DataEventArgs<string>(computerName));
        }


		/// <summary>
		/// Event for when the 'Filter by Application' value has changed
		/// </summary>
		public event EventHandler<DataEventArgs<string>> SelectedApplicationChanged;

		/// <summary>
		/// Fires the <see cref="SelectedApplicationChanged"/> event.
		/// </summary>
		/// <param name="userName">Name of the new selected application</param>
		internal void FireSelectedApplicationChanged(string applicationName)
		{
			if (SelectedApplicationChanged != null)
				SelectedApplicationChanged(this, new DataEventArgs<string>(applicationName));
		}


        private void usersComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            FireSelectedUserChanged(userComboBox.Text);
        }

		private void computerComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			FireSelectedComputerChanged(computerComboBox.Text);
		}

		private void applicationComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			FireSelectedApplicationChanged(applicationComboBox.Text);
		}
	}
}
