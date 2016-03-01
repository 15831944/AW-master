using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win;
using Infragistics.Win.UltraWinListView;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class DocumentsControl : UserControl
	{
		/// <summary>Documents List</summary>
		DocumentList _documents = new DocumentList();

		SCOPE _scope;
		int		_parentID;
        InstalledApplication _application;

        public DocumentList Documents
        {
            get { return _documents; }
        }

		public DocumentsControl()
		{
			InitializeComponent();
		}

        public DocumentsControl(InstalledApplication application)
        {
            _application = application;
            InitializeComponent();
        }

		/// <summary>
		/// This function MUST be called to populate this control with any existing documents for this item
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="parentID"></param>
		public void LoadDocuments(SCOPE scope, int parentID)
		{
			_scope = scope;
			_parentID = parentID;

            lvDocuments.Items.Clear();

			// Recover all documents for this asset
			_documents.Populate(_scope, _parentID);

			// ...and add them to the List View
			foreach (Document document in _documents)
			{
				AddDocumentToListView(document);
			}
		}


		/// <summary>
		/// Called as we change the selection in the documents list to enable/disable buttons as required
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvDocuments_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			this.bnDeleteDocument.Enabled = (lvDocuments.SelectedItems.Count != 0);
			this.bnViewDocument.Enabled = (lvDocuments.SelectedItems.Count != 0);
		}


		/// <summary>
		/// Browse for a document to associate with this instance
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddDocument_Click(object sender, EventArgs e)
		{
			FormAddDocument form = new FormAddDocument();
			if (form.ShowDialog() == DialogResult.OK)
			{
				string documentFile = form.DocumentPath;
				string documentName = form.DocumentName;

				if (form.CopyToLocal)
				{
					string destinationFile = "";
					try
					{
						// Create a hopefully unique name for this document based on the current time
						destinationFile = Path.Combine(Application.StartupPath, "Documents");

                        Directory.CreateDirectory(destinationFile);

						string filename = Path.GetFileNameWithoutExtension(documentFile) + "_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(documentFile);
						destinationFile = destinationFile + @"\" + filename;
						File.Copy(documentFile, destinationFile);
						documentFile = destinationFile;
					}
					catch (Exception ex)
					{
						MessageBox.Show("Failed to copy " + documentFile + " to " + destinationFile + "\nThe error was " + ex.Message);
						return;
					}
				}

				// Add the document to the database and to the list of documents
				Document newDocument = new Document();
				newDocument.Scope = _scope;
				newDocument.Name = documentName;
				newDocument.Path = documentFile;
				newDocument.ParentID = _parentID;
				//
				newDocument.Add();
				AddDocumentToListView(newDocument);

                _documents.Add(newDocument);

                if (_application != null)
                {
                    AuditTrailEntry ate = new AuditTrailEntry();
                    ate.Date = DateTime.Now;
                    ate.Class = AuditTrailEntry.CLASS.application_changes;
                    ate.Type = AuditTrailEntry.TYPE.added;
                    ate.Key = _application.Name + "|Documents";
                    ate.AssetID = 0;
                    ate.AssetName = "";
                    ate.Username = System.Environment.UserName;
                    ate.NewValue = newDocument.Name;
                    new AuditTrailDAO().AuditTrailAdd(ate);
                }

			}
		}


		private void AddDocumentToListView (Document document)
		{
			UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
			subItemArray[0] = new UltraListViewSubItem();
			subItemArray[0].Value = document.Path;
			UltraListViewItem item = new UltraListViewItem(document.Name, subItemArray);
			item.Tag = document;
			lvDocuments.Items.Add(item);
		}


		/// <summary>
		/// Delete the selected document(s) from the database and if locally linked from the Documents folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDeleteDocument_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure that you want to remove the specified documents?\n\nDocuments which have been copied to the AuditWizard documents folder will be deleted", "Confirm Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				foreach (UltraListViewItem lvi in lvDocuments.SelectedItems)
				{
					Document document = lvi.Tag as Document;
					document.Delete();
					lvDocuments.Items.Remove(lvi);

                    if (_application != null)
                    {
                        AuditTrailEntry ate = new AuditTrailEntry();
                        ate.Date = DateTime.Now;
                        ate.Class = AuditTrailEntry.CLASS.application_changes;
                        ate.Type = AuditTrailEntry.TYPE.deleted;
                        ate.Key = _application.Name + "|Documents";
                        ate.AssetID = 0;
                        ate.AssetName = "";
                        ate.OldValue = document.Name;
                        ate.Username = System.Environment.UserName;
                        new AuditTrailDAO().AuditTrailAdd(ate);
                    }
				}
			}
		}

		/// <summary>
		/// View (any) document associated with this license
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnViewDocument_Click(object sender, EventArgs e)
		{
			foreach (UltraListViewItem lvi in lvDocuments.SelectedItems)
			{
				Document document = lvi.Tag as Document;
				document.View();
			}
		}
	}
}
