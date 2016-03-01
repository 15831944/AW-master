using System;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormNewNote : AWForm
	{
	    readonly SCOPE _scope;
	    readonly int _parentId;
	    readonly Note _newNote = new Note();
		
		public Note NewNote
		{
			get { return _newNote; }
		}

        public FormNewNote(Note note)
        {
            InitializeComponent();

            _newNote = note;
            _scope = note.Scope;
            _parentId = note.ParentID;
            tbNoteText.Text = note.Text;

            Text = "Edit existing note";
        }

	    public FormNewNote(SCOPE scope ,int parentID)
		{
			InitializeComponent();
			_scope = scope;
			_parentId = parentID;
		}

		private void FormNewNote_Load(object sender, EventArgs e)
		{
			_newNote.Scope = _scope;
			_newNote.ParentID = _parentId;
		}


		/// <summary>
		/// Called as we change the text for the note
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbNoteText_TextChanged(object sender, EventArgs e)
		{
			bnOK.Enabled = (tbNoteText.Text != "");
		}

		/// <summary>
		/// Called as we exit from the 'Add Note' form to actually create the note
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			_newNote.Text = tbNoteText.Text;
			_newNote.Add();
		}
	}
}

