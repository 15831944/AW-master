namespace Layton.AuditWizard.Common
{
	partial class FormNewNote
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.tbNoteText = new System.Windows.Forms.TextBox();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lbNoteText = new System.Windows.Forms.Label();
            this.lbNoteName = new System.Windows.Forms.Label();
            this.tbNoteName = new System.Windows.Forms.TextBox();
            this.gbNewNote = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbNewNote.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbNoteText
            // 
            this.tbNoteText.Location = new System.Drawing.Point(20, 89);
            this.tbNoteText.Multiline = true;
            this.tbNoteText.Name = "tbNoteText";
            this.tbNoteText.Size = new System.Drawing.Size(530, 300);
            this.tbNoteText.TabIndex = 2;
            this.tbNoteText.TextChanged += new System.EventHandler(this.tbNoteText_TextChanged);
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(419, 467);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 3;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(518, 467);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 4;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lbNoteText
            // 
            this.lbNoteText.AutoSize = true;
            this.lbNoteText.BackColor = System.Drawing.Color.Transparent;
            this.lbNoteText.Location = new System.Drawing.Point(17, 70);
            this.lbNoteText.Name = "lbNoteText";
            this.lbNoteText.Size = new System.Drawing.Size(67, 13);
            this.lbNoteText.TabIndex = 32;
            this.lbNoteText.Text = "Note Text:";
            // 
            // lbNoteName
            // 
            this.lbNoteName.AutoSize = true;
            this.lbNoteName.Location = new System.Drawing.Point(17, 19);
            this.lbNoteName.Name = "lbNoteName";
            this.lbNoteName.Size = new System.Drawing.Size(75, 13);
            this.lbNoteName.TabIndex = 36;
            this.lbNoteName.Text = "Note Name:";
            // 
            // tbNoteName
            // 
            this.tbNoteName.Location = new System.Drawing.Point(20, 38);
            this.tbNoteName.Name = "tbNoteName";
            this.tbNoteName.Size = new System.Drawing.Size(530, 21);
            this.tbNoteName.TabIndex = 1;
            // 
            // gbNewNote
            // 
            this.gbNewNote.Controls.Add(this.lbNoteName);
            this.gbNewNote.Controls.Add(this.tbNoteName);
            this.gbNewNote.Controls.Add(this.lbNoteText);
            this.gbNewNote.Controls.Add(this.tbNoteText);
            this.gbNewNote.Location = new System.Drawing.Point(20, 45);
            this.gbNewNote.Name = "gbNewNote";
            this.gbNewNote.Size = new System.Drawing.Size(590, 416);
            this.gbNewNote.TabIndex = 38;
            this.gbNewNote.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Enter the new note details:";
            // 
            // FormNewNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(630, 506);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbNewNote);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormNewNote";
            this.Text = "Create new note";
            this.Load += new System.EventHandler(this.FormNewNote_Load);
            this.gbNewNote.ResumeLayout(false);
            this.gbNewNote.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbNoteText;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label lbNoteText;
        private System.Windows.Forms.Label lbNoteName;
        private System.Windows.Forms.TextBox tbNoteName;
        private System.Windows.Forms.GroupBox gbNewNote;
        private System.Windows.Forms.Label label1;
	}
}
