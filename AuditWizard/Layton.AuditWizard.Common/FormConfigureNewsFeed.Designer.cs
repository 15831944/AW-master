namespace Layton.AuditWizard.Common
{
    partial class FormConfigureNewsFeed
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
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.cbAssetUpdated = new System.Windows.Forms.CheckBox();
            this.lbPrinterSupplies = new System.Windows.Forms.Label();
            this.tbPrinterSupplies = new System.Windows.Forms.TrackBar();
            this.lbLicenses = new System.Windows.Forms.Label();
            this.tbLicenses = new System.Windows.Forms.TrackBar();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbDiskSpace = new System.Windows.Forms.Label();
            this.tbDiskSpace = new System.Windows.Forms.TrackBar();
            this.bnSave = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrinterSupplies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLicenses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDiskSpace)).BeginInit();
            this.SuspendLayout();
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.cbAssetUpdated);
            this.gbSettings.Controls.Add(this.lbPrinterSupplies);
            this.gbSettings.Controls.Add(this.tbPrinterSupplies);
            this.gbSettings.Controls.Add(this.lbLicenses);
            this.gbSettings.Controls.Add(this.tbLicenses);
            this.gbSettings.Controls.Add(this.lbTitle);
            this.gbSettings.Controls.Add(this.lbDiskSpace);
            this.gbSettings.Controls.Add(this.tbDiskSpace);
            this.gbSettings.Location = new System.Drawing.Point(14, 12);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(500, 247);
            this.gbSettings.TabIndex = 0;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "News Feed Settings";
            // 
            // cbAssetUpdated
            // 
            this.cbAssetUpdated.AutoSize = true;
            this.cbAssetUpdated.Location = new System.Drawing.Point(16, 200);
            this.cbAssetUpdated.Name = "cbAssetUpdated";
            this.cbAssetUpdated.Size = new System.Drawing.Size(140, 17);
            this.cbAssetUpdated.TabIndex = 8;
            this.cbAssetUpdated.Text = "An Asset is Updated";
            this.cbAssetUpdated.UseVisualStyleBackColor = true;
            // 
            // lbPrinterSupplies
            // 
            this.lbPrinterSupplies.AutoSize = true;
            this.lbPrinterSupplies.Location = new System.Drawing.Point(13, 160);
            this.lbPrinterSupplies.Name = "lbPrinterSupplies";
            this.lbPrinterSupplies.Size = new System.Drawing.Size(238, 13);
            this.lbPrinterSupplies.TabIndex = 6;
            this.lbPrinterSupplies.Text = "Less Than % Printer Supplies Remaining";
            // 
            // tbPrinterSupplies
            // 
            this.tbPrinterSupplies.Location = new System.Drawing.Point(285, 144);
            this.tbPrinterSupplies.Maximum = 100;
            this.tbPrinterSupplies.Name = "tbPrinterSupplies";
            this.tbPrinterSupplies.Size = new System.Drawing.Size(203, 45);
            this.tbPrinterSupplies.TabIndex = 5;
            this.tbPrinterSupplies.TickFrequency = 5;
            this.tbPrinterSupplies.Value = 20;
            this.tbPrinterSupplies.Scroll += new System.EventHandler(this.tbPrinterSupplies_Scroll);
            // 
            // lbLicenses
            // 
            this.lbLicenses.AutoSize = true;
            this.lbLicenses.Location = new System.Drawing.Point(13, 114);
            this.lbLicenses.Name = "lbLicenses";
            this.lbLicenses.Size = new System.Drawing.Size(238, 13);
            this.lbLicenses.TabIndex = 4;
            this.lbLicenses.Text = "Greater Than % Software Licenses Used";
            // 
            // tbLicenses
            // 
            this.tbLicenses.Location = new System.Drawing.Point(285, 98);
            this.tbLicenses.Maximum = 100;
            this.tbLicenses.Name = "tbLicenses";
            this.tbLicenses.Size = new System.Drawing.Size(203, 45);
            this.tbLicenses.TabIndex = 3;
            this.tbLicenses.TickFrequency = 5;
            this.tbLicenses.Value = 80;
            this.tbLicenses.Scroll += new System.EventHandler(this.tbLicenses_Scroll);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Location = new System.Drawing.Point(6, 28);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(95, 13);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Alert me when:";
            // 
            // lbDiskSpace
            // 
            this.lbDiskSpace.AutoSize = true;
            this.lbDiskSpace.Location = new System.Drawing.Point(13, 68);
            this.lbDiskSpace.Name = "lbDiskSpace";
            this.lbDiskSpace.Size = new System.Drawing.Size(212, 13);
            this.lbDiskSpace.TabIndex = 1;
            this.lbDiskSpace.Text = "Less Than % Disk Space Remaining";
            // 
            // tbDiskSpace
            // 
            this.tbDiskSpace.Location = new System.Drawing.Point(285, 52);
            this.tbDiskSpace.Maximum = 100;
            this.tbDiskSpace.Name = "tbDiskSpace";
            this.tbDiskSpace.Size = new System.Drawing.Size(203, 45);
            this.tbDiskSpace.TabIndex = 0;
            this.tbDiskSpace.TickFrequency = 5;
            this.tbDiskSpace.Value = 20;
            this.tbDiskSpace.Scroll += new System.EventHandler(this.tbDiskSpace_Scroll);
            // 
            // bnSave
            // 
            this.bnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnSave.Location = new System.Drawing.Point(358, 274);
            this.bnSave.Name = "bnSave";
            this.bnSave.Size = new System.Drawing.Size(75, 23);
            this.bnSave.TabIndex = 2;
            this.bnSave.Text = "OK";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(this.bnSave_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(439, 274);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 3;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // FormConfigureNewsFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(526, 309);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnSave);
            this.Controls.Add(this.gbSettings);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigureNewsFeed";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure News Feed";
            this.Load += new System.EventHandler(this.FormConfigureNewsFeed_Load);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrinterSupplies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLicenses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDiskSpace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.Label lbLicenses;
        private System.Windows.Forms.TrackBar tbLicenses;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Label lbDiskSpace;
        private System.Windows.Forms.TrackBar tbDiskSpace;
        private System.Windows.Forms.Button bnSave;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label lbPrinterSupplies;
        private System.Windows.Forms.TrackBar tbPrinterSupplies;
        private System.Windows.Forms.CheckBox cbAssetUpdated;
    }
}