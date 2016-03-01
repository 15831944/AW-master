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
	public partial class FormSelectApplication : ShadedImageForm
	{
		public string SelectedApplication
		{
			get { return cbApplications.Text; }
		}

		public FormSelectApplication()
		{
			InitializeComponent();
		}

		private void FormSelectApplication_Load(object sender, EventArgs e)
		{
			ApplicationsDAO lwDataAccess = new ApplicationsDAO();
			DataTable applicationsTable = lwDataAccess.GetApplications("", true ,false);
			//
			foreach (DataRow row in applicationsTable.Rows)
			{
				cbApplications.Items.Add(row["_NAME"]);
			}
		}
	}
}