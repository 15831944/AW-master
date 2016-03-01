using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Common
{
	#region Note Class

	public class Note
	{
		#region Data
		private int			_noteID;
		private DateTime	_dateofnote;
		private SCOPE		_scope;
		private int			_parentID;
		private string		_user;
		private string		_text;

#endregion Data

		#region Properties

		public int NoteID
		{
			get { return _noteID; }
			set { _noteID = value; }
		}

		public DateTime DateOfNote
		{
			get { return _dateofnote; }
			set { _dateofnote = value; }
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

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public string User
		{
			get { return _user; }
		}

		#endregion Properties

		#region Constructors

		public Note()
		{
			_noteID = 0;
			_text = "";
			_dateofnote = DateTime.Now;
			_scope = SCOPE.Asset;
			_parentID = 0;
			_user = System.Environment.UserName;
		}

		public Note(DataRow dataRow)
		{
			try
			{
				this.NoteID = (int)dataRow["_NOTEID"];
				this.DateOfNote = (DateTime) dataRow["_DATE"];
				this.Scope = (SCOPE)dataRow["_SCOPE"];
				this.ParentID = (int)dataRow["_PARENTID"];
				this.Text = (string)dataRow["_TEXT"];
				this._user = (string)dataRow["_USER"];
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating a NOTE Object, please check database schema.  The message was " + ex.Message);
			}
		}
#endregion Constructors

		#region Methods

		/// <summary>
		/// Add a new note to the database (or possibly update an existing item)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (NoteID == 0)
				NoteID = lwDataAccess.NoteAdd(this);
			else
				lwDataAccess.NoteUpdate(this);
			return 0;
		}


		/// <summary>
		/// Update an existing note in the database (or possibly add a new one)
		/// </summary>
		/// <returns></returns>
		public int Update()
		{
			return Add();
		}

		/// <summary>
		/// Delete this note from the database
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			lwDataAccess.NoteDelete(this);
			return 0;
		}

		#endregion Methods
	}

	#endregion Note Class

	#region NoteList Class

	public class NoteList : List<Note>
	{
		public int Populate(SCOPE scope ,int parentID)
		{
			// Ensure that the list is empty initially
			this.Clear();

			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable table = lwDataAccess.EnumerateNotes(scope, parentID);

			// Iterate through the returned rows in the table and create AssetType objects for each
			foreach (DataRow row in table.Rows)
			{
				AddNote(row);
			}
			return this.Count;
		}

		/// <summary>
		/// Add a new Note to the list given a database row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Note AddNote(DataRow row)
		{
			// Create the assettype object
			Note note = new Note(row);
			this.Add(note);
			return note;
		}

		public Note FindNoteByDate(string date)
		{
			foreach (Note note in this)
			{
				if (date == note.DateOfNote.ToString())
					return note;
			}
			return null;
		}
	}

	#endregion NoteList Class

}
