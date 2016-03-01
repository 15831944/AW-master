using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	public partial class FormFileAssign : Layton.Common.Controls.ShadedImageForm
	{
		FileSystemFile _file = null;
		
		public FormFileAssign(FileSystemFile file)
		{
			InitializeComponent();
			//
			_file = file;
			//
			tbFileName.Text = _file.Name;
			tbProductName.Text = _file.ProductName;
			tbPublisher.Text = _file.Publisher;
			tbDescription.Text = _file.Description;
            tbVersion.Text = _file.FileVersion1;
		}

		/// <summary>
		/// Assign the file given the details specified
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAssignFile_Click(object sender, EventArgs e)
		{
			// OK we are going to assign this file - first we need to create the application itself
			InstalledApplication application = new InstalledApplication();
			application.Name = tbProductName.Text;
			application.Publisher = tbPublisher.Text;
            application.Version = tbVersion.Text;
			application.Add();
			_file.Assign(application.ApplicationID);
		}
	}
}

