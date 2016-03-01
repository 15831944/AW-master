using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class FormSelectPublishersAndApplications : Layton.Common.Controls.ShadedImageForm
	{
		#region Data
		
		protected string _selectedPublishers;
		protected string _selectedApplications;
		protected string _publisherFilter = "";
		protected bool   _showIncluded = true;
		protected bool	 _showIgnored = false;

		protected SelectApplicationsControl.eSelectionType _selectionType = SelectApplicationsControl.eSelectionType.all;

		#endregion Data

		#region Properties
		
		public string PublisherFilter
		{
			set { _publisherFilter = value; }
		}
		
		public bool ShowIncluded
		{
			set { _showIncluded = value; }
		}

		public bool ShowIgnored
		{
			set { _showIgnored = value; }
		}

		public SelectApplicationsControl.eSelectionType SelectionType
		{
			set { _selectionType = value; }
		}
	
		#endregion Data

		public FormSelectPublishersAndApplications(string selectedPublishers, string selectedApplications)
		{
			InitializeComponent();

			_selectedPublishers = selectedPublishers;
			_selectedApplications = selectedApplications;
		}

		private void FormSelectPublishersAndApplications_Load(object sender, EventArgs e)
		{
			// If we are selecting a single application pass this through to the control
			selectApplicationsControl.SelectionType = _selectionType;

			// Populate the selection control
			selectApplicationsControl.PopulatePublishers(_publisherFilter ,_showIncluded ,_showIgnored, false);
			
			// ...then restore any previous selections if all selection
			if (_selectionType == SelectApplicationsControl.eSelectionType.all)
			{
				selectApplicationsControl.RestoreSelections(_selectedPublishers, _selectedApplications);
			}
			else
			{
				this.Text = "Select Application";
				this.footerPictureBox.Image = Properties.Resources.select_application_corner;
			}
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			// If not all selection then ensure that the user has selected a single application
			if (_selectionType != SelectApplicationsControl.eSelectionType.all)
			{
				InstalledApplication selectedApplication = selectApplicationsControl.GetSelectedApplication();
				if (selectedApplication == null)
				{
					MessageBox.Show("You must select an application", "Validation Error");
					this.DialogResult = DialogResult.None;
					return;
				}
			}
		}		
		
		/// <summary>
		/// Return lists of items selected in the locations control - this will return true if all items are
		/// selected and false if the selected items are located in the returned lists which may be empty)
		/// </summary>
		/// <param name="listSelectedGroups"></param>
		/// <param name="listSelectedAssets"></param>
		/// <returns></returns>
		public bool GetSelectedItems(out ApplicationPublisherList listSelectedPublishers, out InstalledApplicationList listSelectedApplications)
		{
			return selectApplicationsControl.GetSelectedItems(out listSelectedPublishers, out listSelectedApplications);
		}

		public InstalledApplication SelectedApplication()
		{
			return selectApplicationsControl.GetSelectedApplication();
		}
	}
}

