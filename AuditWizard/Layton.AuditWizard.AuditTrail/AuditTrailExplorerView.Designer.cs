namespace Layton.AuditWizard.AuditTrail
{
    partial class AuditTrailExplorerView
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include details of changes (other that changes picked up from an audit) made to a" +
                    "ssets within the database.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include details of any Users created, deleted or modified.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Show ALL Audit Trail Entries", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include details of any Suppliers created, deleted or modified.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include details of any application licenses created, deleted or modified.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Include details of any changes in the application properties (Product ID, Support" +
                    " Contracts etc)", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            this.label5 = new System.Windows.Forms.Label();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.rbAssetChanges = new System.Windows.Forms.RadioButton();
            this.rbUsers = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbSuppliers = new System.Windows.Forms.RadioButton();
            this.rdLicenseChanges = new System.Windows.Forms.RadioButton();
            this.rdPropertyChanges = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(7, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(244, 61);
            this.label5.TabIndex = 14;
            this.label5.Text = "Select which Audit Trail Entries should be displayed by clicking the buttons belo" +
                "w.";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // rbAssetChanges
            // 
            this.rbAssetChanges.BackColor = System.Drawing.Color.Transparent;
            this.rbAssetChanges.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.computer32;
            this.rbAssetChanges.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rbAssetChanges.Location = new System.Drawing.Point(10, 328);
            this.rbAssetChanges.Name = "rbAssetChanges";
            this.rbAssetChanges.Size = new System.Drawing.Size(247, 39);
            this.rbAssetChanges.TabIndex = 22;
            this.rbAssetChanges.Text = "Asset Changes";
            this.rbAssetChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo1.ToolTipText = "Include details of changes (other that changes picked up from an audit) made to a" +
                "ssets within the database.";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbAssetChanges, ultraToolTipInfo1);
            this.rbAssetChanges.UseVisualStyleBackColor = false;
            this.rbAssetChanges.CheckedChanged += new System.EventHandler(this.rdAssets_CheckedChanged);
            // 
            // rbUsers
            // 
            this.rbUsers.BackColor = System.Drawing.Color.Transparent;
            this.rbUsers.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.userdetails_32;
            this.rbUsers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rbUsers.Location = new System.Drawing.Point(10, 283);
            this.rbUsers.Name = "rbUsers";
            this.rbUsers.Size = new System.Drawing.Size(247, 39);
            this.rbUsers.TabIndex = 21;
            this.rbUsers.Text = "User Changes";
            this.rbUsers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo2.ToolTipText = "Include details of any Users created, deleted or modified.";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbUsers, ultraToolTipInfo2);
            this.rbUsers.UseVisualStyleBackColor = false;
            this.rbUsers.CheckedChanged += new System.EventHandler(this.rbUsers_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.BackColor = System.Drawing.Color.Transparent;
            this.rbAll.Checked = true;
            this.rbAll.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.AuditTrail;
            this.rbAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rbAll.Location = new System.Drawing.Point(10, 373);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(247, 39);
            this.rbAll.TabIndex = 20;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All Audit Trail Entries";
            this.rbAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo3.ToolTipText = "Show ALL Audit Trail Entries";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbAll, ultraToolTipInfo3);
            this.rbAll.UseVisualStyleBackColor = false;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
            // 
            // rbSuppliers
            // 
            this.rbSuppliers.BackColor = System.Drawing.Color.Transparent;
            this.rbSuppliers.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.supplier_32;
            this.rbSuppliers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rbSuppliers.Location = new System.Drawing.Point(10, 238);
            this.rbSuppliers.Name = "rbSuppliers";
            this.rbSuppliers.Size = new System.Drawing.Size(247, 39);
            this.rbSuppliers.TabIndex = 19;
            this.rbSuppliers.Text = "Supplier Changes";
            this.rbSuppliers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo4.ToolTipText = "Include details of any Suppliers created, deleted or modified.";
            this.ultraToolTipManager1.SetUltraToolTip(this.rbSuppliers, ultraToolTipInfo4);
            this.rbSuppliers.UseVisualStyleBackColor = false;
            this.rbSuppliers.CheckedChanged += new System.EventHandler(this.rbSuppliers_CheckedChanged);
            // 
            // rdLicenseChanges
            // 
            this.rdLicenseChanges.BackColor = System.Drawing.Color.Transparent;
            this.rdLicenseChanges.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.application_license_32;
            this.rdLicenseChanges.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdLicenseChanges.Location = new System.Drawing.Point(10, 193);
            this.rdLicenseChanges.Name = "rdLicenseChanges";
            this.rdLicenseChanges.Size = new System.Drawing.Size(247, 39);
            this.rdLicenseChanges.TabIndex = 17;
            this.rdLicenseChanges.Text = "Application License Changes";
            this.rdLicenseChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo5.ToolTipText = "Include details of any application licenses created, deleted or modified.";
            this.ultraToolTipManager1.SetUltraToolTip(this.rdLicenseChanges, ultraToolTipInfo5);
            this.rdLicenseChanges.UseVisualStyleBackColor = false;
            this.rdLicenseChanges.CheckedChanged += new System.EventHandler(this.rdLicenseChanges_CheckedChanged);
            // 
            // rdPropertyChanges
            // 
            this.rdPropertyChanges.BackColor = System.Drawing.Color.Transparent;
            this.rdPropertyChanges.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.application_properties_32;
            this.rdPropertyChanges.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rdPropertyChanges.Location = new System.Drawing.Point(10, 149);
            this.rdPropertyChanges.Name = "rdPropertyChanges";
            this.rdPropertyChanges.Size = new System.Drawing.Size(247, 39);
            this.rdPropertyChanges.TabIndex = 16;
            this.rdPropertyChanges.Text = "Application Property Changes";
            this.rdPropertyChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ultraToolTipInfo6.ToolTipText = "Include details of any changes in the application properties (Product ID, Support" +
                " Contracts etc)";
            this.ultraToolTipManager1.SetUltraToolTip(this.rdPropertyChanges, ultraToolTipInfo6);
            this.rdPropertyChanges.UseVisualStyleBackColor = false;
            this.rdPropertyChanges.CheckedChanged += new System.EventHandler(this.rdPropertyChanges_CheckedChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.AuditTrail_96;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(-5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(206, 102);
            this.label2.TabIndex = 7;
            this.label2.Text = "Audit \r\nTrail";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AuditTrailExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rbAssetChanges);
            this.Controls.Add(this.rbUsers);
            this.Controls.Add(this.rbAll);
            this.Controls.Add(this.rbSuppliers);
            this.Controls.Add(this.rdLicenseChanges);
            this.Controls.Add(this.rdPropertyChanges);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "AuditTrailExplorerView";
            this.Size = new System.Drawing.Size(258, 471);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdPropertyChanges;
        private System.Windows.Forms.RadioButton rdLicenseChanges;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private System.Windows.Forms.RadioButton rbSuppliers;
		private System.Windows.Forms.RadioButton rbUsers;
		private System.Windows.Forms.RadioButton rbAll;
		private System.Windows.Forms.RadioButton rbAssetChanges;
    }
}
