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

namespace Layton.AuditWizard.Applications
{
	public partial class FormAddApplication : ShadedImageForm
	{
		private string _publisherFilter;
		private bool _showIncluded;
		private bool _showIgnored;
	
		public FormAddApplication(string publisher ,string publisherFilter ,bool showIncluded ,bool showIgnored)
		{
			InitializeComponent();

			_publisherFilter = publisherFilter;
			_showIgnored = showIgnored;
			_showIncluded = showIncluded;

			if (!string.IsNullOrEmpty(publisher))
				tbPublisher.Text = publisher;
		}


		private void bnSetPublisher_Click(object sender, EventArgs e)
		{
			FormSelectPublisher selectPublisher = new FormSelectPublisher(true);
			if (selectPublisher.ShowDialog() == DialogResult.OK)
			{
				tbPublisher.Text = selectPublisher.SelectedPublisher;

				// Ensure that this publisher is in the publisher filter otherwise this application could disapp[ear
				SettingsDAO lwDataAccess = new SettingsDAO();
				string publisherFilter = lwDataAccess.GetPublisherFilter();
				if ((publisherFilter != "") && (!publisherFilter.Contains(tbPublisher.Text)))
				{
					publisherFilter = publisherFilter + ";" + tbPublisher.Text;
					lwDataAccess.SetPublisherFilter(publisherFilter);
				}
			}
		}

		private void tbPublisher_TextChanged(object sender, EventArgs e)
		{
			// Enable OK only if we have both a publisher and application name
			bnOK.Enabled = (tbPublisher.Text != "" && tbName.Text != "");
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			// Create an installed application object
			InstalledApplication newApplication = new InstalledApplication();
			newApplication.Publisher = tbPublisher.Text;
			newApplication.Name = tbName.Text;
			newApplication.UserDefined = true;
			newApplication.Add();			
		}
	}
}

