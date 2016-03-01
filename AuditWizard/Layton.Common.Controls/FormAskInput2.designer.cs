namespace Layton.Common.Controls
{
	partial class FormAskInput2
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
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lblInput1 = new System.Windows.Forms.Label();
            this.lblInput2 = new System.Windows.Forms.Label();
            this.tbValue1Entered = new System.Windows.Forms.TextBox();
            this.tbValue2Entered = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(296, 143);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 5;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(391, 143);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 6;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lblInput1
            // 
            this.lblInput1.AutoSize = true;
            this.lblInput1.BackColor = System.Drawing.Color.Transparent;
            this.lblInput1.Location = new System.Drawing.Point(16, 53);
            this.lblInput1.Name = "lblInput1";
            this.lblInput1.Size = new System.Drawing.Size(42, 13);
            this.lblInput1.TabIndex = 1;
            this.lblInput1.Text = "Input:";
            // 
            // lblInput2
            // 
            this.lblInput2.AutoSize = true;
            this.lblInput2.BackColor = System.Drawing.Color.Transparent;
            this.lblInput2.Location = new System.Drawing.Point(16, 98);
            this.lblInput2.Name = "lblInput2";
            this.lblInput2.Size = new System.Drawing.Size(42, 13);
            this.lblInput2.TabIndex = 3;
            this.lblInput2.Text = "Input:";
            // 
            // tbValue1Entered
            // 
            this.tbValue1Entered.Location = new System.Drawing.Point(14, 70);
            this.tbValue1Entered.Name = "tbValue1Entered";
            this.tbValue1Entered.Size = new System.Drawing.Size(464, 21);
            this.tbValue1Entered.TabIndex = 2;
            this.tbValue1Entered.TextChanged += new System.EventHandler(this.tbValue1Entered_TextChanged);
            // 
            // tbValue2Entered
            // 
            this.tbValue2Entered.Location = new System.Drawing.Point(14, 114);
            this.tbValue2Entered.Name = "tbValue2Entered";
            this.tbValue2Entered.Size = new System.Drawing.Size(464, 21);
            this.tbValue2Entered.TabIndex = 4;
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblDescription.Location = new System.Drawing.Point(15, 14);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(463, 37);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Describe the input required in this label";
            // 
            // FormAskInput2
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(492, 171);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.tbValue1Entered);
            this.Controls.Add(this.lblInput1);
            this.Controls.Add(this.tbValue2Entered);
            this.Controls.Add(this.lblInput2);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAskInput2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Enter Something";
            this.Load += new System.EventHandler(this.FormAskInput2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label lblInput1;
		private System.Windows.Forms.Label lblInput2;
		private System.Windows.Forms.TextBox tbValue1Entered;
		private System.Windows.Forms.TextBox tbValue2Entered;
		private System.Windows.Forms.Label lblDescription;
	}
}