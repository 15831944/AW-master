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
	public partial class FormAddDocument : Layton.Common.Controls.ShadedImageForm
	{
        Document _document = new Document();

        public Document NewDocument
        {
            get { return _document; }
        }

		public string DocumentName
		{
			get { return tbName.Text; }
		}

		public string DocumentPath
		{
			get { return tbPath.Text; }
		}

		public bool CopyToLocal
		{
			get { return cbCopyFile.Checked; }
		}

		public FormAddDocument()
		{
			InitializeComponent();
		}

		private void bnBrowse_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
				tbPath.Text = openFileDialog1.FileName;
		}

		private void tbPath_TextChanged(object sender, EventArgs e)
		{
			this.bnOK.Enabled = (tbPath.Text != "");
		}
	}
}

