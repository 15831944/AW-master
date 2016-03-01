namespace Layton.AuditWizard.Administration
{
	partial class FormInternetDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInternetDetails));
            this.cbHistory = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label10 = new System.Windows.Forms.Label();
            this.cbCookies = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nupLimitDays = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupLimitDays)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("footerPictureBox.Image")));
            this.footerPictureBox.Location = new System.Drawing.Point(1, 216);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // cbHistory
            // 
            this.cbHistory.BackColor = System.Drawing.Color.Transparent;
            this.cbHistory.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbHistory.Checked = true;
            this.cbHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHistory.Location = new System.Drawing.Point(17, 56);
            this.cbHistory.Name = "cbHistory";
            this.cbHistory.Size = new System.Drawing.Size(304, 20);
            this.cbHistory.TabIndex = 28;
            this.cbHistory.Text = "History of Web Sites Visited";
            this.cbHistory.CheckedChanged += new System.EventHandler(this.cbHistory_CheckedChanged);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(14, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(422, 32);
            this.label10.TabIndex = 27;
            this.label10.Text = "Select the Internet Explorer Information to be audited.";
            // 
            // cbCookies
            // 
            this.cbCookies.BackColor = System.Drawing.Color.Transparent;
            this.cbCookies.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbCookies.Checked = true;
            this.cbCookies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCookies.Location = new System.Drawing.Point(17, 82);
            this.cbCookies.Name = "cbCookies";
            this.cbCookies.Size = new System.Drawing.Size(304, 20);
            this.cbCookies.TabIndex = 30;
            this.cbCookies.Text = "Internet Cookies Stored";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(349, 177);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 33;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(254, 177);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 32;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Limit Results to Last";
            // 
            // nupLimitDays
            // 
            this.nupLimitDays.Location = new System.Drawing.Point(139, 112);
            this.nupLimitDays.Name = "nupLimitDays";
            this.nupLimitDays.Size = new System.Drawing.Size(52, 21);
            this.nupLimitDays.TabIndex = 35;
            this.nupLimitDays.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Days";
            // 
            // FormInternetDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(448, 336);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nupLimitDays);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.cbCookies);
            this.Controls.Add(this.cbHistory);
            this.Controls.Add(this.label10);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormInternetDetails";
            this.Text = "Internet Audit Details";
            this.Load += new System.EventHandler(this.FormInternetDetails_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label10, 0);
            this.Controls.SetChildIndex(this.cbHistory, 0);
            this.Controls.SetChildIndex(this.cbCookies, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.nupLimitDays, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupLimitDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbHistory;
		private System.Windows.Forms.Label label10;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbCookies;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nupLimitDays;
		private System.Windows.Forms.Label label2;

	}
}
