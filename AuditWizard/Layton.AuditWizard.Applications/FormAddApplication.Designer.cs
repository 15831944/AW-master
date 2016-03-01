namespace Layton.AuditWizard.Applications
{
	partial class FormAddApplication
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
            this.bnSetPublisher = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPublisher = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_add_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(81, 193);
            this.footerPictureBox.Size = new System.Drawing.Size(378, 122);
            // 
            // bnSetPublisher
            // 
            this.bnSetPublisher.Location = new System.Drawing.Point(407, 82);
            this.bnSetPublisher.Name = "bnSetPublisher";
            this.bnSetPublisher.Size = new System.Drawing.Size(38, 24);
            this.bnSetPublisher.TabIndex = 32;
            this.bnSetPublisher.Text = "...";
            this.bnSetPublisher.UseVisualStyleBackColor = true;
            this.bnSetPublisher.Click += new System.EventHandler(this.bnSetPublisher_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(8, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Publisher:";
            // 
            // tbPublisher
            // 
            this.tbPublisher.BackColor = System.Drawing.SystemColors.Window;
            this.tbPublisher.Location = new System.Drawing.Point(120, 82);
            this.tbPublisher.Name = "tbPublisher";
            this.tbPublisher.Size = new System.Drawing.Size(279, 21);
            this.tbPublisher.TabIndex = 29;
            this.tbPublisher.Text = "New Publisher";
            this.tbPublisher.TextChanged += new System.EventHandler(this.tbPublisher_TextChanged);
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.SystemColors.Window;
            this.tbName.Location = new System.Drawing.Point(120, 112);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(279, 21);
            this.tbName.TabIndex = 31;
            this.tbName.Text = "New Application";
            this.tbName.TextChanged += new System.EventHandler(this.tbPublisher_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(8, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Application:";
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(253, 159);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 33;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(352, 159);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 34;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(7, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(439, 47);
            this.label3.TabIndex = 35;
            this.label3.Text = "To create a new Application you must specify both the name of the Publisher and t" +
                "he name of the Application.  Click \'...\' to select an existing Publisher or crea" +
                "te a new one for this Application.";
            // 
            // FormAddApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(458, 313);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnSetPublisher);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPublisher);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormAddApplication";
            this.Text = "Define New Application";
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbPublisher, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bnSetPublisher, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnSetPublisher;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPublisher;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label label3;
	}
}
