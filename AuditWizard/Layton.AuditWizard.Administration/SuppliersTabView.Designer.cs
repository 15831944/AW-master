namespace Layton.AuditWizard.Administration
{
	partial class SuppliersTabView
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
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("SuppliersTable", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SupplierObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("addressline1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("addressline2");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("city");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("state");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("zip");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("contactname");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("telephone");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("email");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("fax");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuppliersTabView));
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(20951407);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGridSuppliers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSupplierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suppliersDataSet = new System.Data.DataSet();
            this.suppliersTable = new System.Data.DataTable();
            this.firstName = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.dataColumn9 = new System.Data.DataColumn();
            this.dataColumn10 = new System.Data.DataColumn();
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
            this.generalTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bnEditSupplier = new System.Windows.Forms.Button();
            this.bnNewSupplier = new System.Windows.Forms.Button();
            this.bnDeleteSupplier = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dataColumn1 = new System.Data.DataColumn();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridSuppliers)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.suppliersDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppliersTable)).BeginInit();
            this.activityTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.generalTabControl)).BeginInit();
            this.generalTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.splitContainer1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(817, 420);
            // 
            // ultraGridSuppliers
            // 
            this.ultraGridSuppliers.ContextMenuStrip = this.contextMenuStrip1;
            this.ultraGridSuppliers.DataSource = this.suppliersDataSet;
            appearance10.BackColor = System.Drawing.Color.Transparent;
            appearance10.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGridSuppliers.DisplayLayout.Appearance = appearance10;
            ultraGridBand1.CardSettings.CaptionField = "name";
            ultraGridBand1.CardSettings.LabelWidth = 100;
            ultraGridBand1.CardSettings.Style = Infragistics.Win.UltraWinGrid.CardStyle.StandardLabels;
            ultraGridBand1.CardSettings.Width = 250;
            ultraGridBand1.CardView = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.Header.Caption = "ContactName";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn9.Header.Caption = "Email";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            ultraGridBand1.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            ultraGridBand1.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            appearance4.FontData.BoldAsString = "True";
            appearance4.FontData.SizeInPoints = 10F;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            appearance4.TextHAlignAsString = "Left";
            ultraGridBand1.Override.CardCaptionAppearance = appearance4;
            ultraGridBand1.Override.CardSpacing = 5;
            ultraGridBand1.Override.CellSpacing = 0;
            appearance5.TextHAlignAsString = "Right";
            ultraGridBand1.Override.HeaderAppearance = appearance5;
            this.ultraGridSuppliers.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGridSuppliers.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance15.TextVAlignAsString = "Middle";
            this.ultraGridSuppliers.DisplayLayout.CaptionAppearance = appearance15;
            this.ultraGridSuppliers.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance16.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance16.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridSuppliers.DisplayLayout.GroupByBox.Appearance = appearance16;
            appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridSuppliers.DisplayLayout.GroupByBox.BandLabelAppearance = appearance17;
            this.ultraGridSuppliers.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance18.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance18.BackColor2 = System.Drawing.SystemColors.Control;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridSuppliers.DisplayLayout.GroupByBox.PromptAppearance = appearance18;
            this.ultraGridSuppliers.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGridSuppliers.DisplayLayout.MaxRowScrollRegions = 1;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraGridSuppliers.DisplayLayout.Override.ActiveCellAppearance = appearance19;
            appearance20.BackColor = System.Drawing.SystemColors.Highlight;
            appearance20.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ultraGridSuppliers.DisplayLayout.Override.ActiveRowAppearance = appearance20;
            this.ultraGridSuppliers.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ultraGridSuppliers.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGridSuppliers.DisplayLayout.Override.CardAreaAppearance = appearance21;
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGridSuppliers.DisplayLayout.Override.CellAppearance = appearance22;
            this.ultraGridSuppliers.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ultraGridSuppliers.DisplayLayout.Override.CellPadding = 0;
            appearance23.BackColor = System.Drawing.SystemColors.Control;
            appearance23.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance23.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance23.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridSuppliers.DisplayLayout.Override.GroupByRowAppearance = appearance23;
            appearance24.TextHAlignAsString = "Left";
            this.ultraGridSuppliers.DisplayLayout.Override.HeaderAppearance = appearance24;
            this.ultraGridSuppliers.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGridSuppliers.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            this.ultraGridSuppliers.DisplayLayout.Override.RowAppearance = appearance25;
            this.ultraGridSuppliers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGridSuppliers.DisplayLayout.Override.SelectedCellAppearance = appearance26;
            this.ultraGridSuppliers.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance27.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGridSuppliers.DisplayLayout.Override.TemplateAddRowAppearance = appearance27;
            this.ultraGridSuppliers.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridSuppliers.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
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
            this.ultraGridSuppliers.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.ultraGridSuppliers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridSuppliers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGridSuppliers.Location = new System.Drawing.Point(0, 0);
            this.ultraGridSuppliers.Name = "ultraGridSuppliers";
            this.ultraGridSuppliers.Size = new System.Drawing.Size(817, 391);
            this.ultraGridSuppliers.TabIndex = 0;
            this.ultraGridSuppliers.Text = "ultraGrid1";
            this.ultraGridSuppliers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ultraGridSuppliers_MouseDown);
            this.ultraGridSuppliers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridSuppliers_InitializeLayout);
            this.ultraGridSuppliers.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ultraGridSuppliers_AfterSelectChange);
            this.ultraGridSuppliers.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultraGridSuppliers_BeforeRowsDeleted);
            this.ultraGridSuppliers.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridSuppliers_AfterCellUpdate);
            this.ultraGridSuppliers.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.ultraGridSuppliers_DoubleClickRow);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newUserToolStripMenuItem,
            this.deleteUserToolStripMenuItem,
            this.editSupplierToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(158, 70);
            // 
            // newUserToolStripMenuItem
            // 
            this.newUserToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.useradd_16;
            this.newUserToolStripMenuItem.Name = "newUserToolStripMenuItem";
            this.newUserToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.newUserToolStripMenuItem.Text = "&New Supplier";
            this.newUserToolStripMenuItem.Click += new System.EventHandler(this.newSupplierToolStripMenuItem_Click);
            // 
            // deleteUserToolStripMenuItem
            // 
            this.deleteUserToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.userdelete_16;
            this.deleteUserToolStripMenuItem.Name = "deleteUserToolStripMenuItem";
            this.deleteUserToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.deleteUserToolStripMenuItem.Text = "&Delete Supplier";
            this.deleteUserToolStripMenuItem.Click += new System.EventHandler(this.deleteSupplierToolStripMenuItem_Click);
            // 
            // editSupplierToolStripMenuItem
            // 
            this.editSupplierToolStripMenuItem.Image = global::Layton.AuditWizard.Administration.Properties.Resources.useredit_16;
            this.editSupplierToolStripMenuItem.Name = "editSupplierToolStripMenuItem";
            this.editSupplierToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.editSupplierToolStripMenuItem.Text = "&Edit Supplier";
            this.editSupplierToolStripMenuItem.Click += new System.EventHandler(this.editSupplierToolStripMenuItem_Click);
            // 
            // suppliersDataSet
            // 
            this.suppliersDataSet.DataSetName = "NewDataSet";
            this.suppliersDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.suppliersTable});
            // 
            // suppliersTable
            // 
            this.suppliersTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.firstName,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn2,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8,
            this.dataColumn9,
            this.dataColumn10});
            this.suppliersTable.TableName = "SuppliersTable";
            // 
            // firstName
            // 
            this.firstName.Caption = "Address Line 1";
            this.firstName.ColumnName = "addressline1";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Address Line 2";
            this.dataColumn3.ColumnName = "addressline2";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "City";
            this.dataColumn4.ColumnName = "city";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "State";
            this.dataColumn5.ColumnName = "state";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Zip";
            this.dataColumn2.ColumnName = "zip";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "contactname";
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Telephone";
            this.dataColumn7.ColumnName = "telephone";
            // 
            // dataColumn8
            // 
            this.dataColumn8.ColumnName = "email";
            // 
            // dataColumn9
            // 
            this.dataColumn9.Caption = "Name";
            this.dataColumn9.ColumnName = "name";
            // 
            // dataColumn10
            // 
            this.dataColumn10.Caption = "Fax";
            this.dataColumn10.ColumnName = "fax";
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
            this.activityTabPage.Size = new System.Drawing.Size(825, 535);
            // 
            // emailButton
            // 
            this.emailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emailButton.BackColor = System.Drawing.Color.Transparent;
            this.emailButton.Location = new System.Drawing.Point(316, 175);
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
            this.advancedButton.Location = new System.Drawing.Point(397, 175);
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
            this.smtpTextBox.Size = new System.Drawing.Size(374, 20);
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
            this.emailTextBox.Size = new System.Drawing.Size(374, 20);
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
            this.instructionLabel.Size = new System.Drawing.Size(793, 69);
            this.instructionLabel.TabIndex = 17;
            this.instructionLabel.Text = resources.GetString("instructionLabel.Text");
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.footerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(510, 440);
            this.footerPictureBox.Name = "footerPictureBox";
            this.footerPictureBox.Size = new System.Drawing.Size(312, 92);
            this.footerPictureBox.TabIndex = 7;
            this.footerPictureBox.TabStop = false;
            // 
            // generalTabControl
            // 
            this.generalTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.generalTabControl.Controls.Add(this.ultraTabPageControl1);
            this.generalTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalTabControl.Location = new System.Drawing.Point(0, 80);
            this.generalTabControl.Name = "generalTabControl";
            this.generalTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.generalTabControl.Size = new System.Drawing.Size(817, 420);
            this.generalTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.generalTabControl.TabIndex = 3;
            this.generalTabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "User Accounts";
            this.generalTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
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
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.supplier_72;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance1;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 5);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(177, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "Suppliers";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ultraGridSuppliers);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.bnEditSupplier);
            this.splitContainer1.Panel2.Controls.Add(this.bnNewSupplier);
            this.splitContainer1.Panel2.Controls.Add(this.bnDeleteSupplier);
            this.splitContainer1.Size = new System.Drawing.Size(817, 420);
            this.splitContainer1.SplitterDistance = 391;
            this.splitContainer1.TabIndex = 1;
            // 
            // bnEditSupplier
            // 
            this.bnEditSupplier.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnEditSupplier.Enabled = false;
            this.bnEditSupplier.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Edit;
            this.bnEditSupplier.Location = new System.Drawing.Point(54, 2);
            this.bnEditSupplier.MaximumSize = new System.Drawing.Size(24, 24);
            this.bnEditSupplier.MinimumSize = new System.Drawing.Size(24, 24);
            this.bnEditSupplier.Name = "bnEditSupplier";
            this.bnEditSupplier.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnEditSupplier.Size = new System.Drawing.Size(24, 24);
            this.bnEditSupplier.TabIndex = 10;
            this.toolTip1.SetToolTip(this.bnEditSupplier, "Edit supplier");
            this.bnEditSupplier.UseVisualStyleBackColor = true;
            this.bnEditSupplier.Click += new System.EventHandler(this.bnEditSupplier_Click);
            // 
            // bnNewSupplier
            // 
            this.bnNewSupplier.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnNewSupplier.Image = global::Layton.AuditWizard.Administration.Properties.Resources.add16;
            this.bnNewSupplier.Location = new System.Drawing.Point(4, 2);
            this.bnNewSupplier.MaximumSize = new System.Drawing.Size(24, 24);
            this.bnNewSupplier.MinimumSize = new System.Drawing.Size(24, 24);
            this.bnNewSupplier.Name = "bnNewSupplier";
            this.bnNewSupplier.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnNewSupplier.Size = new System.Drawing.Size(24, 24);
            this.bnNewSupplier.TabIndex = 8;
            this.toolTip1.SetToolTip(this.bnNewSupplier, "Add new supplier");
            this.bnNewSupplier.UseVisualStyleBackColor = true;
            this.bnNewSupplier.Click += new System.EventHandler(this.bnNewSupplier_Click);
            // 
            // bnDeleteSupplier
            // 
            this.bnDeleteSupplier.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnDeleteSupplier.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Delete;
            this.bnDeleteSupplier.Location = new System.Drawing.Point(29, 2);
            this.bnDeleteSupplier.MaximumSize = new System.Drawing.Size(24, 24);
            this.bnDeleteSupplier.MinimumSize = new System.Drawing.Size(24, 24);
            this.bnDeleteSupplier.Name = "bnDeleteSupplier";
            this.bnDeleteSupplier.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnDeleteSupplier.Size = new System.Drawing.Size(24, 24);
            this.bnDeleteSupplier.TabIndex = 9;
            this.toolTip1.SetToolTip(this.bnDeleteSupplier, "Delete supplier");
            this.bnDeleteSupplier.UseVisualStyleBackColor = true;
            this.bnDeleteSupplier.Click += new System.EventHandler(this.bnDeleteSupplier_Click);
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "SupplierObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // SuppliersTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.generalTabControl);
            this.Controls.Add(this.headerGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "SuppliersTabView";
            this.Size = new System.Drawing.Size(817, 500);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridSuppliers)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.suppliersDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppliersTable)).EndInit();
            this.activityTabPage.ResumeLayout(false);
            this.activityTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.generalTabControl)).EndInit();
            this.generalTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTabControl.UltraTabControl generalTabControl;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl activityTabPage;
		private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridSuppliers;
		private System.Data.DataSet suppliersDataSet;
		private System.Data.DataTable suppliersTable;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn firstName;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn5;
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
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataColumn dataColumn7;
		private System.Data.DataColumn dataColumn8;
		private System.Data.DataColumn dataColumn9;
		private System.Data.DataColumn dataColumn10;
		private System.Windows.Forms.ToolStripMenuItem editSupplierToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bnEditSupplier;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bnNewSupplier;
        private System.Windows.Forms.Button bnDeleteSupplier;
    }
}
