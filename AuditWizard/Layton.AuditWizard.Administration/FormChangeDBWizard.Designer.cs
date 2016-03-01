namespace Layton.AuditWizard.Administration
{
    partial class FormChangeDBWizard
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChangeDBWizard));
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			this.wizardControl1 = new WizardBase.WizardControl();
			this.startStep1 = new WizardBase.StartStep();
			this.ultraFormattedLinkLabel2 = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
			this.intermediateStep1 = new WizardBase.IntermediateStep();
			this.ultraPictureBox2 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
			this.bnChooseDatabases = new System.Windows.Forms.Button();
			this.lbServerExample = new System.Windows.Forms.Label();
			this.tbServerName = new System.Windows.Forms.TextBox();
			this.tbNewDBName = new System.Windows.Forms.TextBox();
			this.ddlCatalog = new System.Windows.Forms.ComboBox();
			this.lbDatabaseName = new System.Windows.Forms.Label();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.tbLogin = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblLogin = new System.Windows.Forms.Label();
			this.ddlAuthType = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.rdExisting = new System.Windows.Forms.RadioButton();
			this.rdCreateNewDB = new System.Windows.Forms.RadioButton();
			this.lbChangeDB = new System.Windows.Forms.Label();
			this.intermediateStep2 = new WizardBase.IntermediateStep();
			this.ultraPictureBox1 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
			this.rdNoMigrateData = new System.Windows.Forms.RadioButton();
			this.rdMigrateData = new System.Windows.Forms.RadioButton();
			this.lbExistingMsg = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
			this.finishStep1 = new WizardBase.FinishStep();
			this.ultraPictureBox3 = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
			this.lbProgressStage = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.ultraFormattedLinkLabel3 = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
			this.ultraFormattedLinkLabel1 = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
			this.startStep1.SuspendLayout();
			this.intermediateStep1.SuspendLayout();
			this.intermediateStep2.SuspendLayout();
			this.finishStep1.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizardControl1
			// 
			this.wizardControl1.BackButtonEnabled = true;
			this.wizardControl1.BackButtonVisible = true;
			this.wizardControl1.CancelButtonEnabled = true;
			this.wizardControl1.CancelButtonVisible = true;
			this.wizardControl1.HelpButtonEnabled = true;
			this.wizardControl1.HelpButtonVisible = false;
			this.wizardControl1.Location = new System.Drawing.Point(0, 0);
			this.wizardControl1.Name = "wizardControl1";
			this.wizardControl1.NextButtonEnabled = true;
			this.wizardControl1.NextButtonVisible = true;
			this.wizardControl1.Size = new System.Drawing.Size(624, 426);
			this.wizardControl1.WizardSteps.Add(this.startStep1);
			this.wizardControl1.WizardSteps.Add(this.intermediateStep1);
			this.wizardControl1.WizardSteps.Add(this.intermediateStep2);
			this.wizardControl1.WizardSteps.Add(this.finishStep1);
			this.wizardControl1.CancelButtonClick += new System.EventHandler(this.CancelClicked);
			this.wizardControl1.FinishButtonClick += new System.EventHandler(this.FinishClicked);
			this.wizardControl1.NextButtonClick += new WizardBase.WizardNextButtonClickEventHandler(this.wizardControl_NextButtonClick);
			// 
			// startStep1
			// 
			this.startStep1.BindingImage = global::Layton.AuditWizard.Administration.Properties.Resources.wizard;
			this.startStep1.Controls.Add(this.ultraFormattedLinkLabel2);
			this.startStep1.Icon = global::Layton.AuditWizard.Administration.Properties.Resources.transparent;
			this.startStep1.Name = "startStep1";
			this.startStep1.Subtitle = "This wizard allows you to change the database used by AuditWizard.";
			this.startStep1.SubtitleFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.startStep1.Title = "AuditWizard Change Database Wizard";
			this.startStep1.TitleFont = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
			// 
			// ultraFormattedLinkLabel2
			// 
			this.ultraFormattedLinkLabel2.Location = new System.Drawing.Point(173, 72);
			this.ultraFormattedLinkLabel2.Name = "ultraFormattedLinkLabel2";
			this.ultraFormattedLinkLabel2.Size = new System.Drawing.Size(457, 56);
			this.ultraFormattedLinkLabel2.TabIndex = 3;
			this.ultraFormattedLinkLabel2.TabStop = true;
			this.ultraFormattedLinkLabel2.Value = "The default version of AuditWizard uses a SQL Server Compact v3.5 database. This " +
    "wizard allows you to change the database type used by AuditWizard.";
			// 
			// intermediateStep1
			// 
			this.intermediateStep1.BindingImage = global::Layton.AuditWizard.Administration.Properties.Resources.top_grad_red;
			this.intermediateStep1.Controls.Add(this.ultraPictureBox2);
			this.intermediateStep1.Controls.Add(this.bnChooseDatabases);
			this.intermediateStep1.Controls.Add(this.lbServerExample);
			this.intermediateStep1.Controls.Add(this.tbServerName);
			this.intermediateStep1.Controls.Add(this.tbNewDBName);
			this.intermediateStep1.Controls.Add(this.ddlCatalog);
			this.intermediateStep1.Controls.Add(this.lbDatabaseName);
			this.intermediateStep1.Controls.Add(this.tbPassword);
			this.intermediateStep1.Controls.Add(this.tbLogin);
			this.intermediateStep1.Controls.Add(this.lblPassword);
			this.intermediateStep1.Controls.Add(this.lblLogin);
			this.intermediateStep1.Controls.Add(this.ddlAuthType);
			this.intermediateStep1.Controls.Add(this.label3);
			this.intermediateStep1.Controls.Add(this.label2);
			this.intermediateStep1.Controls.Add(this.rdExisting);
			this.intermediateStep1.Controls.Add(this.rdCreateNewDB);
			this.intermediateStep1.Controls.Add(this.lbChangeDB);
			this.intermediateStep1.ForeColor = System.Drawing.Color.White;
			this.intermediateStep1.Name = "intermediateStep1";
			this.intermediateStep1.Subtitle = "Choose the type of SQL Server database you would like to use.";
			this.intermediateStep1.SubtitleFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intermediateStep1.Title = "Select database type.";
			this.intermediateStep1.TitleFont = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// ultraPictureBox2
			// 
			this.ultraPictureBox2.AutoSize = true;
			this.ultraPictureBox2.BorderShadowColor = System.Drawing.Color.Empty;
			this.ultraPictureBox2.Image = ((object)(resources.GetObject("ultraPictureBox2.Image")));
			this.ultraPictureBox2.Location = new System.Drawing.Point(474, 195);
			this.ultraPictureBox2.Name = "ultraPictureBox2";
			this.ultraPictureBox2.Size = new System.Drawing.Size(98, 182);
			this.ultraPictureBox2.TabIndex = 29;
			// 
			// bnChooseDatabases
			// 
			this.bnChooseDatabases.ForeColor = System.Drawing.Color.Black;
			this.bnChooseDatabases.Location = new System.Drawing.Point(358, 188);
			this.bnChooseDatabases.Name = "bnChooseDatabases";
			this.bnChooseDatabases.Size = new System.Drawing.Size(29, 23);
			this.bnChooseDatabases.TabIndex = 28;
			this.bnChooseDatabases.Text = "...";
			this.bnChooseDatabases.UseVisualStyleBackColor = true;
			this.bnChooseDatabases.Click += new System.EventHandler(this.bnChooseDatabases_Click);
			// 
			// lbServerExample
			// 
			this.lbServerExample.AutoSize = true;
			this.lbServerExample.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic);
			this.lbServerExample.ForeColor = System.Drawing.Color.Black;
			this.lbServerExample.Location = new System.Drawing.Point(140, 214);
			this.lbServerExample.Name = "lbServerExample";
			this.lbServerExample.Size = new System.Drawing.Size(173, 13);
			this.lbServerExample.TabIndex = 27;
			this.lbServerExample.Text = "eg COMPUTER\\SQLINSTANCE";
			// 
			// tbServerName
			// 
			this.tbServerName.ForeColor = System.Drawing.Color.Black;
			this.tbServerName.Location = new System.Drawing.Point(140, 189);
			this.tbServerName.Name = "tbServerName";
			this.tbServerName.Size = new System.Drawing.Size(212, 20);
			this.tbServerName.TabIndex = 26;
			// 
			// tbNewDBName
			// 
			this.tbNewDBName.ForeColor = System.Drawing.Color.Black;
			this.tbNewDBName.Location = new System.Drawing.Point(140, 340);
			this.tbNewDBName.Name = "tbNewDBName";
			this.tbNewDBName.Size = new System.Drawing.Size(247, 20);
			this.tbNewDBName.TabIndex = 25;
			// 
			// ddlCatalog
			// 
			this.ddlCatalog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlCatalog.ForeColor = System.Drawing.Color.Black;
			this.ddlCatalog.FormattingEnabled = true;
			this.ddlCatalog.Location = new System.Drawing.Point(140, 340);
			this.ddlCatalog.Name = "ddlCatalog";
			this.ddlCatalog.Size = new System.Drawing.Size(247, 21);
			this.ddlCatalog.TabIndex = 23;
			this.ddlCatalog.Visible = false;
			this.ddlCatalog.Click += new System.EventHandler(this.ddlCatalog_Changed);
			// 
			// lbDatabaseName
			// 
			this.lbDatabaseName.AutoSize = true;
			this.lbDatabaseName.ForeColor = System.Drawing.Color.Black;
			this.lbDatabaseName.Location = new System.Drawing.Point(12, 343);
			this.lbDatabaseName.Name = "lbDatabaseName";
			this.lbDatabaseName.Size = new System.Drawing.Size(97, 13);
			this.lbDatabaseName.TabIndex = 22;
			this.lbDatabaseName.Text = "Catalog/Database:";
			// 
			// tbPassword
			// 
			this.tbPassword.Enabled = false;
			this.tbPassword.ForeColor = System.Drawing.Color.Black;
			this.tbPassword.Location = new System.Drawing.Point(241, 307);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.Size = new System.Drawing.Size(146, 20);
			this.tbPassword.TabIndex = 21;
			// 
			// tbLogin
			// 
			this.tbLogin.Enabled = false;
			this.tbLogin.ForeColor = System.Drawing.Color.Black;
			this.tbLogin.Location = new System.Drawing.Point(241, 281);
			this.tbLogin.Name = "tbLogin";
			this.tbLogin.Size = new System.Drawing.Size(146, 20);
			this.tbLogin.TabIndex = 20;
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Enabled = false;
			this.lblPassword.ForeColor = System.Drawing.Color.Black;
			this.lblPassword.Location = new System.Drawing.Point(137, 310);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 19;
			this.lblPassword.Text = "Password:";
			// 
			// lblLogin
			// 
			this.lblLogin.AutoSize = true;
			this.lblLogin.Enabled = false;
			this.lblLogin.ForeColor = System.Drawing.Color.Black;
			this.lblLogin.Location = new System.Drawing.Point(137, 284);
			this.lblLogin.Name = "lblLogin";
			this.lblLogin.Size = new System.Drawing.Size(36, 13);
			this.lblLogin.TabIndex = 18;
			this.lblLogin.Text = "Login:";
			// 
			// ddlAuthType
			// 
			this.ddlAuthType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAuthType.ForeColor = System.Drawing.Color.Black;
			this.ddlAuthType.FormattingEnabled = true;
			this.ddlAuthType.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
			this.ddlAuthType.Location = new System.Drawing.Point(140, 244);
			this.ddlAuthType.Name = "ddlAuthType";
			this.ddlAuthType.Size = new System.Drawing.Size(247, 21);
			this.ddlAuthType.TabIndex = 15;
			this.ddlAuthType.SelectedIndexChanged += new System.EventHandler(this.ddlAuthType_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(12, 247);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Authentication Type:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(12, 193);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Server Name:";
			// 
			// rdExisting
			// 
			this.rdExisting.AutoSize = true;
			this.rdExisting.ForeColor = System.Drawing.Color.Black;
			this.rdExisting.Location = new System.Drawing.Point(15, 145);
			this.rdExisting.Name = "rdExisting";
			this.rdExisting.Size = new System.Drawing.Size(177, 17);
			this.rdExisting.TabIndex = 2;
			this.rdExisting.TabStop = true;
			this.rdExisting.Text = "Connect to an existing database";
			this.rdExisting.UseVisualStyleBackColor = true;
			this.rdExisting.CheckedChanged += new System.EventHandler(this.rdExisting_CheckedChanged);
			// 
			// rdCreateNewDB
			// 
			this.rdCreateNewDB.AutoSize = true;
			this.rdCreateNewDB.ForeColor = System.Drawing.Color.Black;
			this.rdCreateNewDB.Location = new System.Drawing.Point(15, 121);
			this.rdCreateNewDB.Name = "rdCreateNewDB";
			this.rdCreateNewDB.Size = new System.Drawing.Size(135, 17);
			this.rdCreateNewDB.TabIndex = 1;
			this.rdCreateNewDB.TabStop = true;
			this.rdCreateNewDB.Text = "Create a new database";
			this.rdCreateNewDB.UseVisualStyleBackColor = true;
			this.rdCreateNewDB.CheckedChanged += new System.EventHandler(this.rdCreateNewDB_CheckedChanged);
			// 
			// lbChangeDB
			// 
			this.lbChangeDB.AutoSize = true;
			this.lbChangeDB.ForeColor = System.Drawing.Color.Black;
			this.lbChangeDB.Location = new System.Drawing.Point(12, 85);
			this.lbChangeDB.Name = "lbChangeDB";
			this.lbChangeDB.Size = new System.Drawing.Size(448, 13);
			this.lbChangeDB.TabIndex = 0;
			this.lbChangeDB.Text = "Please select whether you would like to connect to an existing database or create" +
    " a new one.";
			// 
			// intermediateStep2
			// 
			this.intermediateStep2.BindingImage = global::Layton.AuditWizard.Administration.Properties.Resources.top_grad_red;
			this.intermediateStep2.Controls.Add(this.ultraPictureBox1);
			this.intermediateStep2.Controls.Add(this.rdNoMigrateData);
			this.intermediateStep2.Controls.Add(this.rdMigrateData);
			this.intermediateStep2.Controls.Add(this.lbExistingMsg);
			this.intermediateStep2.ForeColor = System.Drawing.Color.White;
			this.intermediateStep2.Name = "intermediateStep2";
			this.intermediateStep2.SubtitleFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intermediateStep2.Title = "New WizardControl step.";
			this.intermediateStep2.TitleFont = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// ultraPictureBox1
			// 
			this.ultraPictureBox1.AutoSize = true;
			this.ultraPictureBox1.BorderShadowColor = System.Drawing.Color.Empty;
			this.ultraPictureBox1.Image = ((object)(resources.GetObject("ultraPictureBox1.Image")));
			this.ultraPictureBox1.Location = new System.Drawing.Point(474, 195);
			this.ultraPictureBox1.Name = "ultraPictureBox1";
			this.ultraPictureBox1.Size = new System.Drawing.Size(98, 182);
			this.ultraPictureBox1.TabIndex = 25;
			// 
			// rdNoMigrateData
			// 
			this.rdNoMigrateData.AutoSize = true;
			this.rdNoMigrateData.ForeColor = System.Drawing.Color.Black;
			this.rdNoMigrateData.Location = new System.Drawing.Point(34, 165);
			this.rdNoMigrateData.Name = "rdNoMigrateData";
			this.rdNoMigrateData.Size = new System.Drawing.Size(136, 17);
			this.rdNoMigrateData.TabIndex = 3;
			this.rdNoMigrateData.TabStop = true;
			this.rdNoMigrateData.Text = "No, do not migrate data";
			this.rdNoMigrateData.UseVisualStyleBackColor = true;
			this.rdNoMigrateData.Visible = false;
			this.rdNoMigrateData.CheckedChanged += new System.EventHandler(this.rdNoMigrateData_CheckedChanged);
			// 
			// rdMigrateData
			// 
			this.rdMigrateData.AutoSize = true;
			this.rdMigrateData.ForeColor = System.Drawing.Color.Black;
			this.rdMigrateData.Location = new System.Drawing.Point(34, 131);
			this.rdMigrateData.Name = "rdMigrateData";
			this.rdMigrateData.Size = new System.Drawing.Size(204, 17);
			this.rdMigrateData.TabIndex = 2;
			this.rdMigrateData.TabStop = true;
			this.rdMigrateData.Text = "Yes, migrate existing data to database";
			this.rdMigrateData.UseVisualStyleBackColor = true;
			this.rdMigrateData.Visible = false;
			this.rdMigrateData.CheckedChanged += new System.EventHandler(this.rdMigrateData_CheckedChanged);
			// 
			// lbExistingMsg
			// 
			appearance1.ForeColor = System.Drawing.Color.Black;
			this.lbExistingMsg.Appearance = appearance1;
			this.lbExistingMsg.Location = new System.Drawing.Point(34, 96);
			this.lbExistingMsg.Name = "lbExistingMsg";
			this.lbExistingMsg.Size = new System.Drawing.Size(510, 45);
			this.lbExistingMsg.TabIndex = 1;
			this.lbExistingMsg.TabStop = true;
			this.lbExistingMsg.Value = "Do you wish to migrate your current data into the existing database?";
			this.lbExistingMsg.Visible = false;
			// 
			// finishStep1
			// 
			this.finishStep1.BackgroundImage = global::Layton.AuditWizard.Administration.Properties.Resources.transparent;
			this.finishStep1.Controls.Add(this.ultraPictureBox3);
			this.finishStep1.Controls.Add(this.lbProgressStage);
			this.finishStep1.Controls.Add(this.progressBar1);
			this.finishStep1.Controls.Add(this.ultraFormattedLinkLabel3);
			this.finishStep1.Controls.Add(this.ultraFormattedLinkLabel1);
			this.finishStep1.Name = "finishStep1";
			// 
			// ultraPictureBox3
			// 
			this.ultraPictureBox3.AutoSize = true;
			this.ultraPictureBox3.BorderShadowColor = System.Drawing.Color.Empty;
			this.ultraPictureBox3.Image = ((object)(resources.GetObject("ultraPictureBox3.Image")));
			this.ultraPictureBox3.Location = new System.Drawing.Point(474, 195);
			this.ultraPictureBox3.Name = "ultraPictureBox3";
			this.ultraPictureBox3.Size = new System.Drawing.Size(98, 182);
			this.ultraPictureBox3.TabIndex = 25;
			// 
			// lbProgressStage
			// 
			this.lbProgressStage.AutoSize = true;
			this.lbProgressStage.Location = new System.Drawing.Point(31, 134);
			this.lbProgressStage.Name = "lbProgressStage";
			this.lbProgressStage.Size = new System.Drawing.Size(0, 13);
			this.lbProgressStage.TabIndex = 6;
			this.lbProgressStage.Visible = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(34, 154);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(500, 23);
			this.progressBar1.TabIndex = 5;
			this.progressBar1.Visible = false;
			// 
			// ultraFormattedLinkLabel3
			// 
			this.ultraFormattedLinkLabel3.Location = new System.Drawing.Point(34, 94);
			this.ultraFormattedLinkLabel3.Name = "ultraFormattedLinkLabel3";
			this.ultraFormattedLinkLabel3.Size = new System.Drawing.Size(504, 23);
			this.ultraFormattedLinkLabel3.TabIndex = 1;
			this.ultraFormattedLinkLabel3.TabStop = true;
			this.ultraFormattedLinkLabel3.Value = "Press the Finish button to make your changes.";
			// 
			// ultraFormattedLinkLabel1
			// 
			this.ultraFormattedLinkLabel1.Location = new System.Drawing.Point(34, 36);
			this.ultraFormattedLinkLabel1.Name = "ultraFormattedLinkLabel1";
			this.ultraFormattedLinkLabel1.Size = new System.Drawing.Size(504, 40);
			this.ultraFormattedLinkLabel1.TabIndex = 0;
			this.ultraFormattedLinkLabel1.TabStop = true;
			this.ultraFormattedLinkLabel1.Value = resources.GetString("ultraFormattedLinkLabel1.Value");
			// 
			// FormChangeDBWizard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(621, 425);
			this.Controls.Add(this.wizardControl1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormChangeDBWizard";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Database Wizard";
			this.startStep1.ResumeLayout(false);
			this.intermediateStep1.ResumeLayout(false);
			this.intermediateStep1.PerformLayout();
			this.intermediateStep2.ResumeLayout(false);
			this.intermediateStep2.PerformLayout();
			this.finishStep1.ResumeLayout(false);
			this.finishStep1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private WizardBase.WizardControl wizardControl1;
        private WizardBase.StartStep startStep1;
        private WizardBase.IntermediateStep intermediateStep1;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel ultraFormattedLinkLabel2;
        private WizardBase.IntermediateStep intermediateStep2;
        private System.Windows.Forms.RadioButton rdCreateNewDB;
        private System.Windows.Forms.Label lbChangeDB;
        private System.Windows.Forms.RadioButton rdExisting;
        private System.Windows.Forms.ComboBox ddlCatalog;
        private System.Windows.Forms.Label lbDatabaseName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.ComboBox ddlAuthType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNewDBName;
        private WizardBase.FinishStep finishStep1;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel lbExistingMsg;
        private System.Windows.Forms.RadioButton rdNoMigrateData;
        private System.Windows.Forms.RadioButton rdMigrateData;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel ultraFormattedLinkLabel3;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel ultraFormattedLinkLabel1;
        private System.Windows.Forms.TextBox tbServerName;
        private System.Windows.Forms.Label lbServerExample;
        private System.Windows.Forms.Button bnChooseDatabases;
        private System.Windows.Forms.Label lbProgressStage;
        private System.Windows.Forms.ProgressBar progressBar1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox2;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox1;
        private Infragistics.Win.UltraWinEditors.UltraPictureBox ultraPictureBox3;
    }
}