namespace Layton.AuditWizard.Administration
{
    partial class SecurityTabView
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
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("UsersTable", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserObject");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityTabView));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("firstName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("lastName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("accessLevel", -1, 20951407);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("logon");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(20951407);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGridUsers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersDataSet = new System.Data.DataSet();
            this.usersTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.firstName = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbEnableSecurity = new System.Windows.Forms.CheckBox();
            this.activityTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.emailButton = new System.Windows.Forms.Button();
            this.advancedButton = new System.Windows.Forms.Button();
            this.smtpTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.neverRadioButton = new System.Windows.Forms.RadioButton();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.instructionLabel = new Infragistics.Win.Misc.UltraLabel();
            this.footerPictureBox = new System.Windows.Forms.PictureBox();
            this.usersTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridUsers)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usersDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.activityTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTabControl)).BeginInit();
            this.usersTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ultraGridUsers);
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(817, 420);
            // 
            // ultraGridUsers
            // 
            this.ultraGridUsers.ContextMenuStrip = this.contextMenuStrip1;
            this.ultraGridUsers.DataSource = this.usersDataSet;
            appearance27.BackColor = System.Drawing.Color.Transparent;
            appearance27.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGridUsers.DisplayLayout.Appearance = appearance27;
            ultraGridBand1.CardSettings.CaptionField = "Logon";
            ultraGridBand1.CardSettings.LabelWidth = 90;
            ultraGridBand1.CardSettings.Style = Infragistics.Win.UltraWinGrid.CardStyle.StandardLabels;
            ultraGridBand1.CardSettings.Width = 200;
            ultraGridBand1.CardView = true;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            ultraGridColumn1.CellAppearance = appearance4;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            ultraGridBand1.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            ultraGridBand1.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.FontData.BoldAsString = "True";
            appearance5.FontData.SizeInPoints = 10F;
            appearance5.ForeColor = System.Drawing.Color.Lavender;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            appearance5.TextHAlignAsString = "Left";
            ultraGridBand1.Override.CardCaptionAppearance = appearance5;
            ultraGridBand1.Override.CardSpacing = 5;
            ultraGridBand1.Override.CellSpacing = 0;
            appearance6.TextHAlignAsString = "Right";
            ultraGridBand1.Override.HeaderAppearance = appearance6;
            this.ultraGridUsers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGridUsers.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance53.FontData.Name = "Verdana";
            appearance53.FontData.SizeInPoints = 10F;
            this.ultraGridUsers.DisplayLayout.CaptionAppearance = appearance53;
            this.ultraGridUsers.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance42.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance42.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance42.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance42.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridUsers.DisplayLayout.GroupByBox.Appearance = appearance42;
            appearance43.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridUsers.DisplayLayout.GroupByBox.BandLabelAppearance = appearance43;
            this.ultraGridUsers.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance44.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance44.BackColor2 = System.Drawing.SystemColors.Control;
            appearance44.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance44.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridUsers.DisplayLayout.GroupByBox.PromptAppearance = appearance44;
            this.ultraGridUsers.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGridUsers.DisplayLayout.MaxRowScrollRegions = 1;
            appearance45.BackColor = System.Drawing.SystemColors.Window;
            appearance45.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraGridUsers.DisplayLayout.Override.ActiveCellAppearance = appearance45;
            appearance46.BackColor = System.Drawing.SystemColors.Highlight;
            appearance46.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ultraGridUsers.DisplayLayout.Override.ActiveRowAppearance = appearance46;
            this.ultraGridUsers.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ultraGridUsers.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGridUsers.DisplayLayout.Override.CardAreaAppearance = appearance47;
            appearance48.BorderColor = System.Drawing.Color.Silver;
            appearance48.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGridUsers.DisplayLayout.Override.CellAppearance = appearance48;
            this.ultraGridUsers.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ultraGridUsers.DisplayLayout.Override.CellPadding = 0;
            appearance49.BackColor = System.Drawing.SystemColors.Control;
            appearance49.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance49.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance49.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance49.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridUsers.DisplayLayout.Override.GroupByRowAppearance = appearance49;
            appearance50.TextHAlignAsString = "Left";
            this.ultraGridUsers.DisplayLayout.Override.HeaderAppearance = appearance50;
            this.ultraGridUsers.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGridUsers.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance51.BackColor = System.Drawing.SystemColors.Window;
            appearance51.BorderColor = System.Drawing.Color.Silver;
            this.ultraGridUsers.DisplayLayout.Override.RowAppearance = appearance51;
            this.ultraGridUsers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridUsers.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance52.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGridUsers.DisplayLayout.Override.TemplateAddRowAppearance = appearance52;
            this.ultraGridUsers.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridUsers.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "Administrator";
            valueListItem2.DataValue = 0;
            valueListItem2.DisplayText = "Auditor";
            valueListItem3.DataValue = 0;
            valueListItem3.DisplayText = "Read-Only";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.ultraGridUsers.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.ultraGridUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridUsers.Enabled = false;
            this.ultraGridUsers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGridUsers.Location = new System.Drawing.Point(0, 39);
            this.ultraGridUsers.Name = "ultraGridUsers";
            this.ultraGridUsers.Size = new System.Drawing.Size(817, 381);
            this.ultraGridUsers.TabIndex = 0;
            this.ultraGridUsers.Text = "ultraGrid1";
            this.ultraGridUsers.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultraGridUsers_BeforeCellUpdate);
            this.ultraGridUsers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridUsers_InitializeLayout);
            this.ultraGridUsers.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultraGrid1_BeforeRowsDeleted);
            this.ultraGridUsers.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridUsers_AfterCellUpdate);
            this.ultraGridUsers.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ultraGridUsers_KeyPress);
            this.ultraGridUsers.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.ultraGridUsers_DoubleClickRow);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newUserToolStripMenuItem,
            this.deleteUserToolStripMenuItem,
            this.editUserToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(142, 70);
            // 
            // newUserToolStripMenuItem
            // 
            this.newUserToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.useradd_16;
            this.newUserToolStripMenuItem.Name = "newUserToolStripMenuItem";
            this.newUserToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newUserToolStripMenuItem.Text = "&New User";
            this.newUserToolStripMenuItem.Click += new System.EventHandler(this.newUserToolStripMenuItem_Click);
            // 
            // deleteUserToolStripMenuItem
            // 
            this.deleteUserToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.userdelete_16;
            this.deleteUserToolStripMenuItem.Name = "deleteUserToolStripMenuItem";
            this.deleteUserToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.deleteUserToolStripMenuItem.Text = "&Delete User";
            this.deleteUserToolStripMenuItem.Click += new System.EventHandler(this.deleteUserToolStripMenuItem_Click);
            // 
            // editUserToolStripMenuItem
            // 
            this.editUserToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.useredit_16;
            this.editUserToolStripMenuItem.Name = "editUserToolStripMenuItem";
            this.editUserToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.editUserToolStripMenuItem.Text = "Edit User";
            this.editUserToolStripMenuItem.Click += new System.EventHandler(this.editUserToolStripMenuItem_Click);
            // 
            // usersDataSet
            // 
            this.usersDataSet.DataSetName = "NewDataSet";
            this.usersDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.usersTable});
            // 
            // usersTable
            // 
            this.usersTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.firstName,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn2});
            this.usersTable.TableName = "UsersTable";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "UserObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // firstName
            // 
            this.firstName.Caption = "First Name";
            this.firstName.ColumnName = "firstName";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Last Name";
            this.dataColumn3.ColumnName = "lastName";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Access Level";
            this.dataColumn4.ColumnName = "accessLevel";
            this.dataColumn4.DataType = typeof(int);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Logon";
            this.dataColumn2.ColumnName = "logon";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.cbEnableSecurity);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(817, 39);
            this.ultraGroupBox1.TabIndex = 8;
            this.ultraGroupBox1.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // cbEnableSecurity
            // 
            this.cbEnableSecurity.BackColor = System.Drawing.Color.Transparent;
            this.cbEnableSecurity.Location = new System.Drawing.Point(7, 9);
            this.cbEnableSecurity.Name = "cbEnableSecurity";
            this.cbEnableSecurity.Size = new System.Drawing.Size(322, 24);
            this.cbEnableSecurity.TabIndex = 0;
            this.cbEnableSecurity.Text = "Enable Security within AuditWizard";
            this.cbEnableSecurity.UseVisualStyleBackColor = false;
            this.cbEnableSecurity.CheckedChanged += new System.EventHandler(this.cbEnableSecurity_CheckedChanged);
            // 
            // activityTabPage
            // 
            this.activityTabPage.Controls.Add(this.emailButton);
            this.activityTabPage.Controls.Add(this.advancedButton);
            this.activityTabPage.Controls.Add(this.smtpTextBox);
            this.activityTabPage.Controls.Add(this.label3);
            this.activityTabPage.Controls.Add(this.emailTextBox);
            this.activityTabPage.Controls.Add(this.label2);
            this.activityTabPage.Controls.Add(this.neverRadioButton);
            this.activityTabPage.Controls.Add(this.monthlyRadioButton);
            this.activityTabPage.Controls.Add(this.label1);
            this.activityTabPage.Controls.Add(this.weeklyRadioButton);
            this.activityTabPage.Controls.Add(this.dailyRadioButton);
            this.activityTabPage.Controls.Add(this.instructionLabel);
            this.activityTabPage.Controls.Add(this.footerPictureBox);
            this.activityTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.activityTabPage.Name = "activityTabPage";
            this.activityTabPage.Size = new System.Drawing.Size(823, 512);
            // 
            // emailButton
            // 
            this.emailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emailButton.BackColor = System.Drawing.Color.Transparent;
            this.emailButton.Location = new System.Drawing.Point(314, 175);
            this.emailButton.Name = "emailButton";
            this.emailButton.Size = new System.Drawing.Size(75, 23);
            this.emailButton.TabIndex = 29;
            this.emailButton.Text = "Send Email";
            this.emailButton.UseVisualStyleBackColor = false;
            // 
            // advancedButton
            // 
            this.advancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedButton.BackColor = System.Drawing.Color.Transparent;
            this.advancedButton.Location = new System.Drawing.Point(395, 175);
            this.advancedButton.Name = "advancedButton";
            this.advancedButton.Size = new System.Drawing.Size(75, 23);
            this.advancedButton.TabIndex = 28;
            this.advancedButton.Text = "Advanced";
            this.advancedButton.UseVisualStyleBackColor = false;
            // 
            // smtpTextBox
            // 
            this.smtpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.smtpTextBox.Location = new System.Drawing.Point(98, 149);
            this.smtpTextBox.Name = "smtpTextBox";
            this.smtpTextBox.Size = new System.Drawing.Size(372, 20);
            this.smtpTextBox.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "SMTP Host:";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.emailTextBox.Location = new System.Drawing.Point(98, 123);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(372, 20);
            this.emailTextBox.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Email Address:";
            // 
            // neverRadioButton
            // 
            this.neverRadioButton.AutoSize = true;
            this.neverRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.neverRadioButton.Location = new System.Drawing.Point(267, 79);
            this.neverRadioButton.Name = "neverRadioButton";
            this.neverRadioButton.Size = new System.Drawing.Size(54, 17);
            this.neverRadioButton.TabIndex = 23;
            this.neverRadioButton.Text = "Never";
            this.neverRadioButton.UseVisualStyleBackColor = false;
            // 
            // monthlyRadioButton
            // 
            this.monthlyRadioButton.AutoSize = true;
            this.monthlyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.monthlyRadioButton.Location = new System.Drawing.Point(204, 79);
            this.monthlyRadioButton.Name = "monthlyRadioButton";
            this.monthlyRadioButton.Size = new System.Drawing.Size(62, 17);
            this.monthlyRadioButton.TabIndex = 22;
            this.monthlyRadioButton.Text = "Monthly";
            this.monthlyRadioButton.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Frequency:";
            // 
            // weeklyRadioButton
            // 
            this.weeklyRadioButton.AutoSize = true;
            this.weeklyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.weeklyRadioButton.Location = new System.Drawing.Point(139, 79);
            this.weeklyRadioButton.Name = "weeklyRadioButton";
            this.weeklyRadioButton.Size = new System.Drawing.Size(61, 17);
            this.weeklyRadioButton.TabIndex = 20;
            this.weeklyRadioButton.Text = "Weekly";
            this.weeklyRadioButton.UseVisualStyleBackColor = false;
            // 
            // dailyRadioButton
            // 
            this.dailyRadioButton.AutoSize = true;
            this.dailyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.dailyRadioButton.Checked = true;
            this.dailyRadioButton.Location = new System.Drawing.Point(85, 79);
            this.dailyRadioButton.Name = "dailyRadioButton";
            this.dailyRadioButton.Size = new System.Drawing.Size(48, 17);
            this.dailyRadioButton.TabIndex = 19;
            this.dailyRadioButton.TabStop = true;
            this.dailyRadioButton.Text = "Daily";
            this.dailyRadioButton.UseVisualStyleBackColor = false;
            // 
            // instructionLabel
            // 
            this.instructionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instructionLabel.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.instructionLabel.Location = new System.Drawing.Point(16, 14);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(791, 69);
            this.instructionLabel.TabIndex = 17;
            this.instructionLabel.Text = resources.GetString("instructionLabel.Text");
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.footerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(508, 417);
            this.footerPictureBox.Name = "footerPictureBox";
            this.footerPictureBox.Size = new System.Drawing.Size(312, 92);
            this.footerPictureBox.TabIndex = 7;
            this.footerPictureBox.TabStop = false;
            // 
            // usersTabControl
            // 
            this.usersTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.usersTabControl.Controls.Add(this.ultraTabPageControl1);
            this.usersTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersTabControl.Location = new System.Drawing.Point(0, 80);
            this.usersTabControl.Name = "usersTabControl";
            this.usersTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.usersTabControl.Size = new System.Drawing.Size(817, 420);
            this.usersTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.usersTabControl.TabIndex = 3;
            this.usersTabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "User Accounts";
            this.usersTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            this.usersTabControl.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(817, 420);
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(817, 80);
            this.headerGroupBox.TabIndex = 7;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance9.BackColor = System.Drawing.Color.Transparent;
            appearance9.Image = global::Layton.AuditWizard.Administration.Properties.Resources.users_72;
            appearance9.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance9.TextHAlignAsString = "Center";
            appearance9.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance9;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 5);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(388, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "Users and Security";
            // 
            // SecurityTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.usersTabControl);
            this.Controls.Add(this.headerGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "SecurityTabView";
            this.Size = new System.Drawing.Size(817, 500);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridUsers)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.usersDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.activityTabPage.ResumeLayout(false);
            this.activityTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTabControl)).EndInit();
            this.usersTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTabControl.UltraTabControl usersTabControl;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl activityTabPage;
		private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridUsers;
		private System.Data.DataSet usersDataSet;
		private System.Data.DataTable usersTable;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn firstName;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn2;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem newUserToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteUserToolStripMenuItem;
		private System.Windows.Forms.PictureBox footerPictureBox;
		private System.Windows.Forms.Button emailButton;
		private System.Windows.Forms.Button advancedButton;
		private System.Windows.Forms.TextBox smtpTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox emailTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton neverRadioButton;
		private System.Windows.Forms.RadioButton monthlyRadioButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton weeklyRadioButton;
		private System.Windows.Forms.RadioButton dailyRadioButton;
		private Infragistics.Win.Misc.UltraLabel instructionLabel;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.CheckBox cbEnableSecurity;
		private System.Windows.Forms.ToolStripMenuItem editUserToolStripMenuItem;
    }
}
