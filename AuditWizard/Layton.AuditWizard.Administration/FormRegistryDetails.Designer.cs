namespace Layton.AuditWizard.Administration
{
	partial class FormRegistryDetails
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("");
            this.label9 = new System.Windows.Forms.Label();
            this.bnEditKey = new System.Windows.Forms.Button();
            this.bnDeleteKey = new System.Windows.Forms.Button();
            this.bnAddKey = new System.Windows.Forms.Button();
            this.lvRegistryKeys = new Infragistics.Win.UltraWinListView.UltraListView();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.lvRegistryKeys)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(14, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(606, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "The list below details the registry keys which will be audited.  Note that only s" +
                "tring values are supported.";
            // 
            // bnEditKey
            // 
            this.bnEditKey.Enabled = false;
            this.bnEditKey.Location = new System.Drawing.Point(516, 282);
            this.bnEditKey.Name = "bnEditKey";
            this.bnEditKey.Size = new System.Drawing.Size(92, 23);
            this.bnEditKey.TabIndex = 40;
            this.bnEditKey.Text = "&Edit Key";
            this.bnEditKey.UseVisualStyleBackColor = true;
            this.bnEditKey.Click += new System.EventHandler(this.bnEditKey_Click);
            // 
            // bnDeleteKey
            // 
            this.bnDeleteKey.Enabled = false;
            this.bnDeleteKey.Location = new System.Drawing.Point(614, 282);
            this.bnDeleteKey.Name = "bnDeleteKey";
            this.bnDeleteKey.Size = new System.Drawing.Size(92, 23);
            this.bnDeleteKey.TabIndex = 41;
            this.bnDeleteKey.Text = "&Delete Key";
            this.bnDeleteKey.UseVisualStyleBackColor = true;
            this.bnDeleteKey.Click += new System.EventHandler(this.bnDeleteKey_Click);
            // 
            // bnAddKey
            // 
            this.bnAddKey.Location = new System.Drawing.Point(418, 282);
            this.bnAddKey.Name = "bnAddKey";
            this.bnAddKey.Size = new System.Drawing.Size(92, 23);
            this.bnAddKey.TabIndex = 39;
            this.bnAddKey.Text = "&Add Key";
            this.bnAddKey.UseVisualStyleBackColor = true;
            this.bnAddKey.Click += new System.EventHandler(this.bnAddKey_Click);
            // 
            // lvRegistryKeys
            // 
            this.lvRegistryKeys.Location = new System.Drawing.Point(19, 29);
            appearance2.Image = global::Layton.AuditWizard.Administration.Properties.Resources.registrykey_16;
            this.lvRegistryKeys.MainColumn.ItemAppearance = appearance2;
            this.lvRegistryKeys.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Descending;
            this.lvRegistryKeys.MainColumn.Text = "Registry Key";
            this.lvRegistryKeys.MainColumn.Width = 390;
            this.lvRegistryKeys.Name = "lvRegistryKeys";
            this.lvRegistryKeys.Size = new System.Drawing.Size(687, 237);
            ultraListViewSubItemColumn1.Text = "Value Name";
            ultraListViewSubItemColumn1.Width = 285;
            this.lvRegistryKeys.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.lvRegistryKeys.TabIndex = 38;
            this.lvRegistryKeys.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvRegistryKeys.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvRegistryKeys_ItemSelectionChanged);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(658, 389);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 44;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(564, 389);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 43;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvRegistryKeys);
            this.groupBox1.Controls.Add(this.bnAddKey);
            this.groupBox1.Controls.Add(this.bnEditKey);
            this.groupBox1.Controls.Add(this.bnDeleteKey);
            this.groupBox1.Location = new System.Drawing.Point(17, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(728, 322);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            // 
            // FormRegistryDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(771, 427);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.label9);
            this.Name = "FormRegistryDetails";
            this.Text = "Audit Registry Keys";
            this.Load += new System.EventHandler(this.FormRegistryDetails_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lvRegistryKeys)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button bnEditKey;
		private System.Windows.Forms.Button bnDeleteKey;
		private System.Windows.Forms.Button bnAddKey;
		private Infragistics.Win.UltraWinListView.UltraListView lvRegistryKeys;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}
