namespace Layton.AuditWizard.Common
{
	partial class NotesControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbNotePrompt = new System.Windows.Forms.Label();
            this.lbCharsLeft = new System.Windows.Forms.Label();
            this.bnNewNote = new System.Windows.Forms.Button();
            this.bnDeleteNote = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvExistingNotes = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader("(none)");
            this.tbNoteText = new System.Windows.Forms.TextBox();
            this.lbNoteDate = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.lbNotePrompt);
            this.ultraGroupBox2.Controls.Add(this.lbCharsLeft);
            this.ultraGroupBox2.Controls.Add(this.bnNewNote);
            this.ultraGroupBox2.Controls.Add(this.bnDeleteNote);
            this.ultraGroupBox2.Controls.Add(this.splitContainer1);
            this.ultraGroupBox2.Controls.Add(this.lbNoteDate);
            this.ultraGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox2.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(739, 323);
            this.ultraGroupBox2.TabIndex = 25;
            this.ultraGroupBox2.Text = "Notes";
            // 
            // lbNotePrompt
            // 
            this.lbNotePrompt.AutoSize = true;
            this.lbNotePrompt.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNotePrompt.Location = new System.Drawing.Point(223, 274);
            this.lbNotePrompt.Name = "lbNotePrompt";
            this.lbNotePrompt.Size = new System.Drawing.Size(182, 13);
            this.lbNotePrompt.TabIndex = 103;
            this.lbNotePrompt.Text = "Enter a new note in box above";
            // 
            // lbCharsLeft
            // 
            this.lbCharsLeft.AutoSize = true;
            this.lbCharsLeft.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCharsLeft.Location = new System.Drawing.Point(563, 274);
            this.lbCharsLeft.Name = "lbCharsLeft";
            this.lbCharsLeft.Size = new System.Drawing.Size(131, 13);
            this.lbCharsLeft.TabIndex = 101;
            this.lbCharsLeft.Text = "4000 chars remaining";
            // 
            // bnNewNote
            // 
            this.bnNewNote.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnNewNote.Image = global::Layton.AuditWizard.Common.Properties.Resources.note_add;
            this.bnNewNote.Location = new System.Drawing.Point(22, 277);
            this.bnNewNote.MaximumSize = new System.Drawing.Size(24, 24);
            this.bnNewNote.MinimumSize = new System.Drawing.Size(24, 24);
            this.bnNewNote.Name = "bnNewNote";
            this.bnNewNote.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnNewNote.Size = new System.Drawing.Size(24, 24);
            this.bnNewNote.TabIndex = 3;
            this.toolTip1.SetToolTip(this.bnNewNote, "Add note");
            this.bnNewNote.UseVisualStyleBackColor = true;
            this.bnNewNote.Click += new System.EventHandler(this.bnNewNote_Click);
            // 
            // bnDeleteNote
            // 
            this.bnDeleteNote.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnDeleteNote.Image = global::Layton.AuditWizard.Common.Properties.Resources.note_remove;
            this.bnDeleteNote.Location = new System.Drawing.Point(52, 277);
            this.bnDeleteNote.MaximumSize = new System.Drawing.Size(24, 24);
            this.bnDeleteNote.MinimumSize = new System.Drawing.Size(24, 24);
            this.bnDeleteNote.Name = "bnDeleteNote";
            this.bnDeleteNote.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnDeleteNote.Size = new System.Drawing.Size(24, 24);
            this.bnDeleteNote.TabIndex = 4;
            this.toolTip1.SetToolTip(this.bnDeleteNote, "Delete note");
            this.bnDeleteNote.UseVisualStyleBackColor = true;
            this.bnDeleteNote.Click += new System.EventHandler(this.bnDeleteNote_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(22, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvExistingNotes);
            this.splitContainer1.Panel1MinSize = 20;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbNoteText);
            this.splitContainer1.Size = new System.Drawing.Size(672, 240);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 38;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // lvExistingNotes
            // 
            this.lvExistingNotes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvExistingNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvExistingNotes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvExistingNotes.HideSelection = false;
            this.lvExistingNotes.Location = new System.Drawing.Point(0, 0);
            this.lvExistingNotes.MultiSelect = false;
            this.lvExistingNotes.Name = "lvExistingNotes";
            this.lvExistingNotes.Size = new System.Drawing.Size(200, 240);
            this.lvExistingNotes.TabIndex = 1;
            this.lvExistingNotes.UseCompatibleStateImageBehavior = false;
            this.lvExistingNotes.View = System.Windows.Forms.View.Details;
            this.lvExistingNotes.SelectedIndexChanged += new System.EventHandler(this.lvExistingNotes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "NoteText";
            this.columnHeader1.Width = 185;
            // 
            // tbNoteText
            // 
            this.tbNoteText.BackColor = System.Drawing.SystemColors.Window;
            this.tbNoteText.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbNoteText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNoteText.Location = new System.Drawing.Point(0, 0);
            this.tbNoteText.MaxLength = 4000;
            this.tbNoteText.Multiline = true;
            this.tbNoteText.Name = "tbNoteText";
            this.tbNoteText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbNoteText.Size = new System.Drawing.Size(468, 240);
            this.tbNoteText.TabIndex = 2;
            this.tbNoteText.TextChanged += new System.EventHandler(this.tbNoteText_TextChanged);
            this.tbNoteText.Leave += new System.EventHandler(this.tbNoteText_Leave);
            // 
            // lbNoteDate
            // 
            this.lbNoteDate.AutoSize = true;
            this.lbNoteDate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNoteDate.Location = new System.Drawing.Point(223, 274);
            this.lbNoteDate.Name = "lbNoteDate";
            this.lbNoteDate.Size = new System.Drawing.Size(103, 13);
            this.lbNoteDate.TabIndex = 100;
            this.lbNoteDate.Text = "Last updated on:";
            // 
            // NotesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraGroupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "NotesControl";
            this.Size = new System.Drawing.Size(739, 323);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private System.Windows.Forms.TextBox tbNoteText;
        private System.Windows.Forms.Label lbNoteDate;
        private System.Windows.Forms.ListView lvExistingNotes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bnDeleteNote;
        private System.Windows.Forms.Button bnNewNote;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbCharsLeft;
        private System.Windows.Forms.Label lbNotePrompt;
	}
}
