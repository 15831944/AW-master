using System;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class NotesControl : UserControl
    {
        /// <summary>Notes list</summary>
        private readonly NoteList _notes = new NoteList();
        private SCOPE _scope;
        private int _parentID;
        private readonly InstalledApplication _application;

        public NoteList Notes
        {
            get { return _notes; }
        }

        public NotesControl()
        {
            InitializeComponent();
        }

        public NotesControl(InstalledApplication application)
        {
            _application = application;
            InitializeComponent();
        }

        /// <summary>
        /// This function MUST be called to populate this control with any existing notes for this item
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="parentID"></param>
        public void LoadNotes(SCOPE scope, int parentID)
        {
            lvExistingNotes.Items.Clear();
            tbNoteText.Text = "";

            _scope = scope;
            _parentID = parentID;

            // Recover all notes for this asset
            _notes.Populate(scope, parentID);

            foreach (Note note in _notes)
            {
                AddNoteToListView(note);
            }

            if (lvExistingNotes.Items.Count > 0)
                lvExistingNotes.Items[0].Selected = true;

            lbNoteDate.Visible = lvExistingNotes.Items.Count > 0;
            lbCharsLeft.Visible = lvExistingNotes.Items.Count > 0;
            bnDeleteNote.Enabled = lvExistingNotes.Items.Count > 0;
            lbNotePrompt.Visible = lvExistingNotes.Items.Count == 0;

            if (tbNoteText.Text == "")
                tbNoteText.Focus();
        }

        /// <summary>
        /// Display the form for adding a new note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnNewNote_Click(object sender, EventArgs e)
        {
            AddNewNote("New note");
            tbNoteText.Focus();
            tbNoteText.Select(0, tbNoteText.Text.Length);
        }

        private void AddNewNote(string noteText)
        {
            Note note = new Note { ParentID = _parentID, Scope = _scope, Text = noteText };
            note.NoteID = new NotesDAO().NoteAdd(note);
            AddNoteToListView(note);
            _notes.Add(note);

            tbNoteText.Text = note.Text;

            lbNoteDate.Visible = true;
            lbCharsLeft.Visible = true;

            lbNoteDate.Text = String.Format(
                    "Last update: {0} by {1}",
                    note.DateOfNote.ToString("dd MMM HH:mm"),
                    note.User);

            lbCharsLeft.Text = String.Format("{0} chars remaining", 4000 - tbNoteText.Text.Length);

            if (_application != null)
            {
                AuditTrailEntry ate = new AuditTrailEntry();
                ate.Date = DateTime.Now;
                ate.Class = AuditTrailEntry.CLASS.application_changes;
                ate.Type = AuditTrailEntry.TYPE.added;
                ate.Key = _application.Name + "|Notes";
                ate.AssetID = 0;
                ate.AssetName = "";
                ate.NewValue = note.Text;
                ate.Username = Environment.UserName;
                new AuditTrailDAO().AuditTrailAdd(ate);
            }

            lbNotePrompt.Visible = false;
            bnDeleteNote.Enabled = true;
        }

        private void AddNoteToListView(Note note)
        {
            string[] split = note.Text.Split(Environment.NewLine.ToCharArray());
            ListViewItem item = new ListViewItem { Text = split[0], Tag = note.NoteID, ToolTipText = split[0] };
            lvExistingNotes.Items.Add(item);
            item.Selected = true;
        }


        /// <summary>
        /// Delete the currently selected note (after confirmation)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnDeleteNote_Click(object sender, EventArgs e)
        {
            if (lvExistingNotes.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a note from the list.", "Delete Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure that you want to delete this note?", "Confirm Delete",MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) 
                return;

            ListViewItem item = lvExistingNotes.SelectedItems[0];
            Note selectedNote = _notes.FindNoteById((int)item.Tag);

            if (selectedNote == null)
                return;

            selectedNote.Delete();

            if (_application != null)
            {
                AuditTrailEntry ate = new AuditTrailEntry();
                ate.Date = DateTime.Now;
                ate.Class = AuditTrailEntry.CLASS.application_changes;
                ate.Type = AuditTrailEntry.TYPE.deleted;
                ate.Key = _application.Name + "|Notes";
                ate.AssetID = 0;
                ate.AssetName = "";
                ate.OldValue = selectedNote.Text;
                ate.Username = Environment.UserName;
                new AuditTrailDAO().AuditTrailAdd(ate);
            }

            int index = lvExistingNotes.SelectedIndices[0];
            lvExistingNotes.Items.RemoveAt(index);
            tbNoteText.Text = "";

            if (lvExistingNotes.Items.Count > 0)
            {
                lvExistingNotes.Items[0].Selected = true;
            }
            else
            {
                lbNoteDate.Visible = false;
                lbCharsLeft.Visible = false;
                lbNotePrompt.Visible = true;
                bnDeleteNote.Enabled = false;
            }
        }

        private void lvExistingNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvExistingNotes.SelectedItems.Count != 1)
            {
                tbNoteText.Text = "";
                lbNotePrompt.Visible = true;
                lbNoteDate.Visible = false;
                lbCharsLeft.Visible = false;

                return;
            }

            lbNotePrompt.Visible = false;
            lbCharsLeft.Visible = true;

            ListViewItem item = lvExistingNotes.SelectedItems[0];
            Note selectedNote = _notes.FindNoteById((int)item.Tag);

            if (selectedNote != null)
            {
                tbNoteText.Text = selectedNote.Text;
                lbNoteDate.Text = String.Format(
                    "Last update: {0} by {1}",
                    selectedNote.DateOfNote.ToString("dd MMM HH:mm"),
                    selectedNote.User);

                lbNoteDate.Visible = true;
            }
            else
            {
                lbNoteDate.Visible = false;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            lvExistingNotes.Columns[0].Width = splitContainer1.Panel1.Width - 10;
        }

        private void tbNoteText_Leave(object sender, EventArgs e)
        {
            if (lvExistingNotes.SelectedItems.Count != 1)
                return;

            ListViewItem item = lvExistingNotes.SelectedItems[0];
            Note selectedNote = _notes.FindNoteById((int)item.Tag);

            if (selectedNote != null)
            {
                string oldNoteText = selectedNote.Text;
                selectedNote.Text = tbNoteText.Text;
                selectedNote.Update();

                if (_application != null)
                {
                    AuditTrailEntry ate = new AuditTrailEntry();
                    ate.Date = DateTime.Now;
                    ate.Class = AuditTrailEntry.CLASS.application_changes;
                    ate.Type = AuditTrailEntry.TYPE.changed;
                    ate.Key = _application.Name + "|Notes";
                    ate.AssetID = 0;
                    ate.AssetName = "";
                    ate.OldValue = oldNoteText;
                    ate.NewValue = selectedNote.Text;
                    ate.Username = Environment.UserName;
                    new AuditTrailDAO().AuditTrailAdd(ate);
                }

                LoadNotes(_scope, _parentID);
            }
        }

        private void tbNoteText_TextChanged(object sender, EventArgs e)
        {
            if (lvExistingNotes.SelectedItems.Count != 1)
            {
                if (tbNoteText.Text == "")
                    return;

                AddNewNote(tbNoteText.Text);
            }

            ListViewItem item = lvExistingNotes.SelectedItems[0];
            item.Text = tbNoteText.Text.Split(Environment.NewLine.ToCharArray())[0];

            lbCharsLeft.Text = String.Format("{0} chars remaining", 4000 - tbNoteText.Text.Length);
        }
    }
}
