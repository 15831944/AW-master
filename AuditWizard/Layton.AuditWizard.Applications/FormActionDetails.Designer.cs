namespace Layton.AuditWizard.Applications
{
	partial class FormActionDetails
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
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            this.bnDelete = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbNotes = new System.Windows.Forms.TextBox();
            this.cbStatus = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.tbApplication = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAction = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnUpdateComputers = new System.Windows.Forms.Button();
            this.tbAssociatedComputers = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Applications.Properties.Resources.edit_action_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(311, 323);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // bnDelete
            // 
            this.bnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bnDelete.Location = new System.Drawing.Point(657, 243);
            this.bnDelete.Name = "bnDelete";
            this.bnDelete.Size = new System.Drawing.Size(87, 23);
            this.bnDelete.TabIndex = 18;
            this.bnDelete.Text = "&Delete";
            this.bnDelete.UseVisualStyleBackColor = true;
            this.bnDelete.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(12, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Notes";
            // 
            // tbNotes
            // 
            this.tbNotes.BackColor = System.Drawing.SystemColors.Window;
            this.tbNotes.Location = new System.Drawing.Point(15, 133);
            this.tbNotes.Multiline = true;
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.Size = new System.Drawing.Size(728, 104);
            this.tbNotes.TabIndex = 17;
            // 
            // cbStatus
            // 
            this.cbStatus.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem4.DataValue = 0;
            valueListItem4.DisplayText = "Outstanding";
            valueListItem5.DataValue = 1;
            valueListItem5.DisplayText = "Reviewed";
            valueListItem6.DataValue = 2;
            valueListItem6.DisplayText = "Complete";
            this.cbStatus.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5,
            valueListItem6});
            this.cbStatus.Location = new System.Drawing.Point(478, 26);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(266, 22);
            this.cbStatus.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(407, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Status:";
            // 
            // tbApplication
            // 
            this.tbApplication.BackColor = System.Drawing.SystemColors.Window;
            this.tbApplication.Location = new System.Drawing.Point(156, 53);
            this.tbApplication.Name = "tbApplication";
            this.tbApplication.ReadOnly = true;
            this.tbApplication.Size = new System.Drawing.Size(587, 21);
            this.tbApplication.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Selected Application:";
            // 
            // tbAction
            // 
            this.tbAction.BackColor = System.Drawing.SystemColors.Window;
            this.tbAction.Location = new System.Drawing.Point(156, 27);
            this.tbAction.Name = "tbAction";
            this.tbAction.ReadOnly = true;
            this.tbAction.Size = new System.Drawing.Size(224, 21);
            this.tbAction.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(12, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Action:";
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(562, 285);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 19;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(657, 285);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 20;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnUpdateComputers
            // 
            this.bnUpdateComputers.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bnUpdateComputers.Location = new System.Drawing.Point(657, 77);
            this.bnUpdateComputers.Name = "bnUpdateComputers";
            this.bnUpdateComputers.Size = new System.Drawing.Size(87, 23);
            this.bnUpdateComputers.TabIndex = 23;
            this.bnUpdateComputers.Text = "&Change";
            this.bnUpdateComputers.UseVisualStyleBackColor = true;
            this.bnUpdateComputers.Click += new System.EventHandler(this.bnUpdateComputers_Click);
            // 
            // tbAssociatedComputers
            // 
            this.tbAssociatedComputers.BackColor = System.Drawing.SystemColors.Window;
            this.tbAssociatedComputers.Location = new System.Drawing.Point(156, 79);
            this.tbAssociatedComputers.Name = "tbAssociatedComputers";
            this.tbAssociatedComputers.ReadOnly = true;
            this.tbAssociatedComputers.Size = new System.Drawing.Size(493, 21);
            this.tbAssociatedComputers.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(12, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Associated Computers:";
            // 
            // FormActionDetails
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(758, 443);
            this.Controls.Add(this.bnUpdateComputers);
            this.Controls.Add(this.tbAssociatedComputers);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnDelete);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbNotes);
            this.Controls.Add(this.cbStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbApplication);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbAction);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormActionDetails";
            this.Text = "Action Details";
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.tbAction, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbApplication, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.cbStatus, 0);
            this.Controls.SetChildIndex(this.tbNotes, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.bnDelete, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.tbAssociatedComputers, 0);
            this.Controls.SetChildIndex(this.bnUpdateComputers, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnDelete;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbNotes;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbStatus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbApplication;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbAction;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnUpdateComputers;
		private System.Windows.Forms.TextBox tbAssociatedComputers;
		private System.Windows.Forms.Label label6;
	}
}