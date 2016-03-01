namespace Layton.AuditWizard.Common
{
	partial class FormSelectPublisher
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lvPublishers = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.bnNewPublisher = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvPublishers)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Common.Properties.Resources.select_publishers_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(73, 391);
            this.footerPictureBox.Size = new System.Drawing.Size(385, 120);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(253, 361);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 22;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(352, 361);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 23;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lvPublishers
            // 
            this.lvPublishers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::Layton.AuditWizard.Common.Properties.Resources.application_publisher_16;
            this.lvPublishers.Appearance = appearance2;
            this.lvPublishers.ItemSettings.DefaultImage = global::Layton.AuditWizard.Common.Properties.Resources.application_publisher_16;
            this.lvPublishers.Location = new System.Drawing.Point(14, 67);
            this.lvPublishers.Name = "lvPublishers";
            this.lvPublishers.Size = new System.Drawing.Size(430, 288);
            this.lvPublishers.TabIndex = 24;
            this.lvPublishers.Text = "ultraListView1";
            this.lvPublishers.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvPublishers.ViewSettingsDetails.ColumnHeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            this.lvPublishers.ViewSettingsList.MultiColumn = false;
            this.lvPublishers.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvPublishers_ItemSelectionChanged);
            // 
            // ultraLabel1
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 12);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(414, 38);
            this.ultraLabel1.TabIndex = 25;
            this.ultraLabel1.Text = "Please select a Publisher from the list below to which the application should be " +
                "assigned.  All future references to this application will automatically pickup t" +
                "he selected Publisher.";
            // 
            // bnNewPublisher
            // 
            this.bnNewPublisher.Location = new System.Drawing.Point(14, 362);
            this.bnNewPublisher.Name = "bnNewPublisher";
            this.bnNewPublisher.Size = new System.Drawing.Size(92, 24);
            this.bnNewPublisher.TabIndex = 26;
            this.bnNewPublisher.Text = "&New";
            this.bnNewPublisher.Visible = false;
            this.bnNewPublisher.Click += new System.EventHandler(this.bnNewPublisher_Click);
            // 
            // FormSelectPublisher
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(458, 509);
            this.Controls.Add(this.bnNewPublisher);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.lvPublishers);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormSelectPublisher";
            this.Text = "Select Publisher";
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.lvPublishers, 0);
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.bnNewPublisher, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvPublishers)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private Infragistics.Win.UltraWinListView.UltraListView lvPublishers;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraButton bnNewPublisher;
	}
}