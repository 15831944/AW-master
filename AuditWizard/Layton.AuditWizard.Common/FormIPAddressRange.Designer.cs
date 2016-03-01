namespace Layton.AuditWizard.Common
{
	partial class FormIPAddressRange
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Exit selecting the displayed application", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Exit without selecting an application", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.startIPAddress = new IPAddressControlLib.IPAddressControl();
            this.endIPAddress = new IPAddressControlLib.IPAddressControl();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = null;
            this.footerPictureBox.Location = new System.Drawing.Point(14, 132);
            this.footerPictureBox.Size = new System.Drawing.Size(364, 10);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(170, 103);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 4;
            this.bnOK.Text = "&OK";
            ultraToolTipInfo2.ToolTipText = "Exit selecting the displayed application";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnOK, ultraToolTipInfo2);
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(266, 103);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 5;
            this.bnCancel.Text = "&Cancel";
            ultraToolTipInfo1.ToolTipText = "Exit without selecting an application";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnCancel, ultraToolTipInfo1);
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ultraLabel1
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Start IP Address:";
            // 
            // ultraLabel2
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance2;
            this.ultraLabel2.Location = new System.Drawing.Point(14, 59);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel2.TabIndex = 2;
            this.ultraLabel2.Text = "End IP Address:";
            // 
            // startIPAddress
            // 
            this.startIPAddress.AutoHeight = true;
            this.startIPAddress.BackColor = System.Drawing.SystemColors.Window;
            this.startIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.startIPAddress.Location = new System.Drawing.Point(157, 30);
            this.startIPAddress.MinimumSize = new System.Drawing.Size(102, 21);
            this.startIPAddress.Name = "startIPAddress";
            this.startIPAddress.ReadOnly = false;
            this.startIPAddress.Size = new System.Drawing.Size(196, 21);
            this.startIPAddress.TabIndex = 1;
            this.startIPAddress.Text = "...";
            // 
            // endIPAddress
            // 
            this.endIPAddress.AutoHeight = true;
            this.endIPAddress.BackColor = System.Drawing.SystemColors.Window;
            this.endIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.endIPAddress.Location = new System.Drawing.Point(157, 56);
            this.endIPAddress.MinimumSize = new System.Drawing.Size(102, 21);
            this.endIPAddress.Name = "endIPAddress";
            this.endIPAddress.ReadOnly = false;
            this.endIPAddress.Size = new System.Drawing.Size(196, 21);
            this.endIPAddress.TabIndex = 3;
            this.endIPAddress.Text = "...";
            // 
            // FormIPAddressRange
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(367, 260);
            this.Controls.Add(this.endIPAddress);
            this.Controls.Add(this.startIPAddress);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormIPAddressRange";
            this.Text = "Add/Edit TCP/IP Address Range";
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.Controls.SetChildIndex(this.startIPAddress, 0);
            this.Controls.SetChildIndex(this.endIPAddress, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private IPAddressControlLib.IPAddressControl endIPAddress;
		private IPAddressControlLib.IPAddressControl startIPAddress;
	}
}