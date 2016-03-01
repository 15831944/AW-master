using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
	#region Document Class

	public class Document
	{
		#region Data
		private int			_DocumentID;
		private SCOPE		_scope;
		private int			_parentID;
		private string		_name;
		private string		_path;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

#endregion Data

		#region Properties

		public int DocumentID
		{
			get { return _DocumentID; }
			set { _DocumentID = value; }
		}

		public SCOPE Scope
		{
			get { return _scope; }
			set { _scope = value; }
		}

		public int ParentID
		{
			get { return _parentID; }
			set { _parentID = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		#endregion Properties

		#region Constructors

		public Document()
		{
			_DocumentID = 0;
			_scope = SCOPE.Asset;
			_parentID = 0;
			_path = "";
			_name = "";
		}

		public Document(DataRow dataRow)
		{
			try
			{
				this.DocumentID = (int)dataRow["_DOCUMENTID"];
				this.Scope = (SCOPE)dataRow["_SCOPE"];
				this.ParentID = (int)dataRow["_PARENTID"];
				this.Name = (string)dataRow["_NAME"];
				this.Path = (string)dataRow["_PATH"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a DOCUMENT Object, please check database schema.  The message was " + ex.Message);
			}
		}
#endregion Constructors

		#region Methods

		/// <summary>
		/// Add a new Document to the database (or possibly update an existing item)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
            DocumentsDAO lwDataAccess = new DocumentsDAO();
			if (DocumentID == 0)
				DocumentID = lwDataAccess.DocumentAdd(this);
			else
				lwDataAccess.DocumentUpdate(this);
			return 0;
		}


		/// <summary>
		/// Update an existing Document in the database (or possibly add a new one)
		/// </summary>
		/// <returns></returns>
		public int Update()
		{
			return Add();
		}

		/// <summary>
		/// Delete this Document from the database
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			// First remove the reference to the document from the database
            DocumentsDAO lwDataAccess = new DocumentsDAO();
			lwDataAccess.DocumentDelete(this);

			// ...then delete the file itself if it is located in the \documents folder
			string documentsFolder = System.IO.Path.Combine(Application.StartupPath, "Documents");
			if (Path.StartsWith(documentsFolder))
			{
				try
				{
					File.Delete(Path);
				}
				catch (Exception e)
				{
					MessageBox.Show("Failed to delete document " + Path + " \nThe error was " + e.Message);
				}
			}

			return 0;
		}

		/// <summary>
		/// View the specified document
		/// </summary>
		public void View()
		{
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = _path;
			if (!proc.Start())
				MessageBox.Show("Failed to view the specified document file, please check the application mappings");
		}

		#endregion Methods
	}

	#endregion Document Class

	#region DocumentList Class

	public class DocumentList : List<Document>
	{
		public int Populate(SCOPE scope ,int parentID)
		{
			// Ensure that the list is empty initially
			this.Clear();

            DocumentsDAO lwDataAccess = new DocumentsDAO();
			DataTable table = lwDataAccess.EnumerateDocuments(scope, parentID);

			// Iterate through the returned rows in the table and create AssetType objects for each
			foreach (DataRow row in table.Rows)
			{
				AddDocument(row);
			}
			return this.Count;
		}

		/// <summary>
		/// Add a new Document to the list given a database row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Document AddDocument(DataRow row)
		{
			// Create the assettype object
			Document Document = new Document(row);
			this.Add(Document);
			return Document;
		}
	}

	#endregion DocumentList Class

}
