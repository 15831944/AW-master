namespace Layton.Common.Controls
{
	partial class FormShadedAskInput2
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
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbValue1Entered = new System.Windows.Forms.TextBox();
            this.lblInput1 = new System.Windows.Forms.Label();
            this.tbValue2Entered = new System.Windows.Forms.TextBox();
            this.lblInput2 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bnRegistryBrowse = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblDescription.Location = new System.Drawing.Point(13, 14);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(463, 37);
            this.lblDescription.TabIndex = 10;
            this.lblDescription.Text = "Describe the input required in this label";
            // 
            // tbValue1Entered
            // 
            this.tbValue1Entered.Location = new System.Drawing.Point(6, 33);
            this.tbValue1Entered.Name = "tbValue1Entered";
            this.tbValue1Entered.Size = new System.Drawing.Size(405, 21);
            this.tbValue1Entered.TabIndex = 12;
            this.tbValue1Entered.TextChanged += new System.EventHandler(this.tbValue1Entered_TextChanged);
            // 
            // lblInput1
            // 
            this.lblInput1.AutoSize = true;
            this.lblInput1.BackColor = System.Drawing.Color.Transparent;
            this.lblInput1.Location = new System.Drawing.Point(8, 14);
            this.lblInput1.Name = "lblInput1";
            this.lblInput1.Size = new System.Drawing.Size(42, 13);
            this.lblInput1.TabIndex = 11;
            this.lblInput1.Text = "Input:";
            // 
            // tbValue2Entered
            // 
            this.tbValue2Entered.Location = new System.Drawing.Point(6, 77);
            this.tbValue2Entered.Name = "tbValue2Entered";
            this.tbValue2Entered.Size = new System.Drawing.Size(405, 21);
            this.tbValue2Entered.TabIndex = 14;
            // 
            // lblInput2
            // 
            this.lblInput2.AutoSize = true;
            this.lblInput2.BackColor = System.Drawing.Color.Transparent;
            this.lblInput2.Location = new System.Drawing.Point(8, 59);
            this.lblInput2.Name = "lblInput2";
            this.lblInput2.Size = new System.Drawing.Size(42, 13);
            this.lblInput2.TabIndex = 13;
            this.lblInput2.Text = "Input:";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(384, 183);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 16;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(289, 183);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 15;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bnRegistryBrowse);
            this.groupBox1.Controls.Add(this.tbValue1Entered);
            this.groupBox1.Controls.Add(this.lblInput2);
            this.groupBox1.Controls.Add(this.tbValue2Entered);
            this.groupBox1.Controls.Add(this.lblInput1);
            this.groupBox1.Location = new System.Drawing.Point(16, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 119);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // bnRegistryBrowse
            // 
            this.bnRegistryBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bnRegistryBrowse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnRegistryBrowse.Image = global::Layton.Common.Controls.Properties.Resources.add16;
            this.bnRegistryBrowse.Location = new System.Drawing.Point(420, 54);
            this.bnRegistryBrowse.MaximumSize = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.MinimumSize = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.Name = "bnRegistryBrowse";
            this.bnRegistryBrowse.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnRegistryBrowse.Size = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.TabIndex = 35;
            this.bnRegistryBrowse.UseVisualStyleBackColor = true;
            this.bnRegistryBrowse.Click += new System.EventHandler(this.bnRegistryBrowse_Click);
            // 
            // FormShadedAskInput2
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(488, 218);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormShadedAskInput2";
            this.Text = "Enter Something";
            this.Load += new System.EventHandler(this.FormShadedAskInput2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.TextBox tbValue1Entered;
		private System.Windows.Forms.Label lblInput1;
		private System.Windows.Forms.TextBox tbValue2Entered;
		private System.Windows.Forms.Label lblInput2;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bnRegistryBrowse;
	}
}
