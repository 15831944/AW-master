namespace Layton.AuditWizard.Reports
{
    partial class FormViewSchedules
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewSchedules));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bnDeleteAllSchedules = new System.Windows.Forms.Button();
            this.ultraGridReportSchedules = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bnDeleteSchedule = new System.Windows.Forms.Button();
            this.bnEditSchedule = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bnCreateSchedule = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbLocations = new System.Windows.Forms.TextBox();
            this.bnLocationFilter = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxReportName = new System.Windows.Forms.ComboBox();
            this.cmbPublishers = new System.Windows.Forms.ComboBox();
            this.comboBoxReportCategory = new System.Windows.Forms.ComboBox();
            this.lblSelectPublisher = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bnClose = new System.Windows.Forms.Button();
            this.lblServiceRunning = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridReportSchedules)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.bnDeleteAllSchedules);
            this.groupBox1.Controls.Add(this.ultraGridReportSchedules);
            this.groupBox1.Controls.Add(this.bnDeleteSchedule);
            this.groupBox1.Controls.Add(this.bnEditSchedule);
            this.groupBox1.Location = new System.Drawing.Point(13, 193);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(976, 457);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View, edit and delete existing report schedules";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(393, 434);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Expired";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Orange;
            this.label5.Location = new System.Drawing.Point(339, 434);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Future";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(284, 434);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Active";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 434);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Key:";
            // 
            // bnDeleteAllSchedules
            // 
            this.bnDeleteAllSchedules.Location = new System.Drawing.Point(10, 430);
            this.bnDeleteAllSchedules.Name = "bnDeleteAllSchedules";
            this.bnDeleteAllSchedules.Size = new System.Drawing.Size(179, 21);
            this.bnDeleteAllSchedules.TabIndex = 8;
            this.bnDeleteAllSchedules.Text = "Delete All Expired Schedules";
            this.bnDeleteAllSchedules.UseVisualStyleBackColor = true;
            this.bnDeleteAllSchedules.Click += new System.EventHandler(this.bnDeleteAllSchedules_Click);
            // 
            // ultraGridReportSchedules
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.ultraGridReportSchedules.DisplayLayout.AddNewBox.Appearance = appearance1;
            this.ultraGridReportSchedules.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance2.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance2.ImageBackground")));
            appearance2.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance2.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance2.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridReportSchedules.DisplayLayout.AddNewBox.ButtonAppearance = appearance2;
            this.ultraGridReportSchedules.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.ultraGridReportSchedules.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance3.BackColor = System.Drawing.Color.White;
            this.ultraGridReportSchedules.DisplayLayout.Appearance = appearance3;
            this.ultraGridReportSchedules.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.ultraGridReportSchedules.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance4.FontData.Name = "Trebuchet MS";
            appearance4.FontData.SizeInPoints = 9F;
            appearance4.ForeColor = System.Drawing.Color.White;
            appearance4.TextHAlignAsString = "Right";
            this.ultraGridReportSchedules.DisplayLayout.CaptionAppearance = appearance4;
            this.ultraGridReportSchedules.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridReportSchedules.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("ultraGridReportSchedules.DisplayLayout.FixedHeaderOffImage")));
            this.ultraGridReportSchedules.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("ultraGridReportSchedules.DisplayLayout.FixedHeaderOnImage")));
            this.ultraGridReportSchedules.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("ultraGridReportSchedules.DisplayLayout.FixedRowOffImage")));
            this.ultraGridReportSchedules.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("ultraGridReportSchedules.DisplayLayout.FixedRowOnImage")));
            appearance5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance5.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance5.FontData.BoldAsString = "True";
            appearance5.FontData.Name = "Verdana";
            appearance5.FontData.SizeInPoints = 10F;
            appearance5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.ultraGridReportSchedules.DisplayLayout.GroupByBox.Appearance = appearance5;
            this.ultraGridReportSchedules.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridReportSchedules.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance6.BackColor = System.Drawing.Color.Transparent;
            appearance6.FontData.Name = "Verdana";
            this.ultraGridReportSchedules.DisplayLayout.GroupByBox.PromptAppearance = appearance6;
            this.ultraGridReportSchedules.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ultraGridReportSchedules.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridReportSchedules.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridReportSchedules.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance7.BackColor = System.Drawing.Color.Transparent;
            this.ultraGridReportSchedules.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Transparent;
            appearance8.FontData.Name = "Verdana";
            this.ultraGridReportSchedules.DisplayLayout.Override.CellAppearance = appearance8;
            appearance9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance9.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance9.ImageBackground")));
            appearance9.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance9.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridReportSchedules.DisplayLayout.Override.CellButtonAppearance = appearance9;
            this.ultraGridReportSchedules.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ultraGridReportSchedules.DisplayLayout.Override.CellPadding = 4;
            appearance10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.ultraGridReportSchedules.DisplayLayout.Override.FilterCellAppearance = appearance10;
            appearance11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance11.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance11.ImageBackground")));
            appearance11.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.ultraGridReportSchedules.DisplayLayout.Override.FilterClearButtonAppearance = appearance11;
            appearance12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance12.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.ultraGridReportSchedules.DisplayLayout.Override.FilterRowPromptAppearance = appearance12;
            this.ultraGridReportSchedules.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance13.BackColor2 = System.Drawing.Color.White;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance13.FontData.BoldAsString = "True";
            appearance13.FontData.Name = "Verdana";
            appearance13.FontData.SizeInPoints = 8F;
            appearance13.ForeColor = System.Drawing.Color.DimGray;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance13.TextHAlignAsString = "Center";
            this.ultraGridReportSchedules.DisplayLayout.Override.HeaderAppearance = appearance13;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.ultraGridReportSchedules.DisplayLayout.Override.RowAlternateAppearance = appearance14;
            appearance15.BorderColor = System.Drawing.Color.LightGray;
            this.ultraGridReportSchedules.DisplayLayout.Override.RowAppearance = appearance15;
            appearance16.BackColor = System.Drawing.Color.White;
            this.ultraGridReportSchedules.DisplayLayout.Override.RowSelectorAppearance = appearance16;
            this.ultraGridReportSchedules.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance17.BorderColor = System.Drawing.Color.Transparent;
            appearance17.ForeColor = System.Drawing.Color.Black;
            this.ultraGridReportSchedules.DisplayLayout.Override.SelectedCellAppearance = appearance17;
            appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance18.BorderColor = System.Drawing.Color.Transparent;
            appearance18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance18.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance18.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridReportSchedules.DisplayLayout.Override.SelectedRowAppearance = appearance18;
            appearance19.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance19.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance19;
            appearance20.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance20.ImageBackground")));
            appearance20.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance20;
            appearance21.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance21.ImageBackground")));
            appearance21.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance21.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance21;
            appearance22.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance22.ImageBackground")));
            appearance22.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance22;
            appearance23.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance23.ImageBackground")));
            appearance23.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance23.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance23;
            this.ultraGridReportSchedules.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.ultraGridReportSchedules.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridReportSchedules.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraGridReportSchedules.Location = new System.Drawing.Point(10, 34);
            this.ultraGridReportSchedules.Name = "ultraGridReportSchedules";
            this.ultraGridReportSchedules.Size = new System.Drawing.Size(949, 390);
            this.ultraGridReportSchedules.TabIndex = 7;
            this.ultraGridReportSchedules.Text = "Grid Caption Area";
            this.ultraGridReportSchedules.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridReportSchedules.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ultraGridReportSchedules_AfterSelectChange);
            this.ultraGridReportSchedules.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ultraGridReportSchedules_InitializeRow);
            this.ultraGridReportSchedules.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.ultraGridReportSchedules_DoubleClickRow);
            // 
            // bnDeleteSchedule
            // 
            this.bnDeleteSchedule.Enabled = false;
            this.bnDeleteSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnDeleteSchedule.Location = new System.Drawing.Point(872, 430);
            this.bnDeleteSchedule.Name = "bnDeleteSchedule";
            this.bnDeleteSchedule.Size = new System.Drawing.Size(87, 21);
            this.bnDeleteSchedule.TabIndex = 6;
            this.bnDeleteSchedule.Text = "Delete";
            this.bnDeleteSchedule.UseVisualStyleBackColor = true;
            this.bnDeleteSchedule.Click += new System.EventHandler(this.bnDeleteSchedule_Click);
            // 
            // bnEditSchedule
            // 
            this.bnEditSchedule.Enabled = false;
            this.bnEditSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnEditSchedule.Location = new System.Drawing.Point(779, 430);
            this.bnEditSchedule.Name = "bnEditSchedule";
            this.bnEditSchedule.Size = new System.Drawing.Size(87, 21);
            this.bnEditSchedule.TabIndex = 5;
            this.bnEditSchedule.Text = "Edit";
            this.bnEditSchedule.UseVisualStyleBackColor = true;
            this.bnEditSchedule.Click += new System.EventHandler(this.bnEditSchedule_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bnCreateSchedule);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(13, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(976, 163);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create a new schedule";
            // 
            // bnCreateSchedule
            // 
            this.bnCreateSchedule.Enabled = false;
            this.bnCreateSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnCreateSchedule.Location = new System.Drawing.Point(833, 119);
            this.bnCreateSchedule.Name = "bnCreateSchedule";
            this.bnCreateSchedule.Size = new System.Drawing.Size(122, 21);
            this.bnCreateSchedule.TabIndex = 8;
            this.bnCreateSchedule.Text = "Create Schedule";
            this.bnCreateSchedule.UseVisualStyleBackColor = true;
            this.bnCreateSchedule.Click += new System.EventHandler(this.bnCreateSchedule_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbLocations);
            this.groupBox4.Controls.Add(this.bnLocationFilter);
            this.groupBox4.Location = new System.Drawing.Point(505, 26);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(287, 127);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select location filter";
            // 
            // tbLocations
            // 
            this.tbLocations.BackColor = System.Drawing.Color.White;
            this.tbLocations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbLocations.Location = new System.Drawing.Point(15, 37);
            this.tbLocations.Multiline = true;
            this.tbLocations.Name = "tbLocations";
            this.tbLocations.ReadOnly = true;
            this.tbLocations.Size = new System.Drawing.Size(256, 42);
            this.tbLocations.TabIndex = 14;
            // 
            // bnLocationFilter
            // 
            this.bnLocationFilter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnLocationFilter.Location = new System.Drawing.Point(82, 92);
            this.bnLocationFilter.Name = "bnLocationFilter";
            this.bnLocationFilter.Size = new System.Drawing.Size(122, 21);
            this.bnLocationFilter.TabIndex = 13;
            this.bnLocationFilter.Text = "Select Locations";
            this.bnLocationFilter.UseVisualStyleBackColor = true;
            this.bnLocationFilter.Click += new System.EventHandler(this.bnLocationFilter_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxReportName);
            this.groupBox3.Controls.Add(this.cmbPublishers);
            this.groupBox3.Controls.Add(this.comboBoxReportCategory);
            this.groupBox3.Controls.Add(this.lblSelectPublisher);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(20, 26);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(463, 125);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select report";
            // 
            // comboBoxReportName
            // 
            this.comboBoxReportName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReportName.FormattingEnabled = true;
            this.comboBoxReportName.Location = new System.Drawing.Point(133, 58);
            this.comboBoxReportName.Name = "comboBoxReportName";
            this.comboBoxReportName.Size = new System.Drawing.Size(304, 21);
            this.comboBoxReportName.TabIndex = 1;
            this.comboBoxReportName.SelectedIndexChanged += new System.EventHandler(this.comboBoxReportName_SelectedIndexChanged);
            // 
            // cmbPublishers
            // 
            this.cmbPublishers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPublishers.FormattingEnabled = true;
            this.cmbPublishers.Location = new System.Drawing.Point(133, 92);
            this.cmbPublishers.Name = "cmbPublishers";
            this.cmbPublishers.Size = new System.Drawing.Size(304, 21);
            this.cmbPublishers.TabIndex = 10;
            // 
            // comboBoxReportCategory
            // 
            this.comboBoxReportCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReportCategory.FormattingEnabled = true;
            this.comboBoxReportCategory.Items.AddRange(new object[] {
            "Hardware",
            "Software",
            "Asset Management",
            "Audit Status",
            "Compliance",
            "Custom",
            "User-Defined SQL"});
            this.comboBoxReportCategory.Location = new System.Drawing.Point(133, 24);
            this.comboBoxReportCategory.Name = "comboBoxReportCategory";
            this.comboBoxReportCategory.Size = new System.Drawing.Size(191, 21);
            this.comboBoxReportCategory.TabIndex = 0;
            this.comboBoxReportCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxReportCategory_SelectedIndexChanged);
            // 
            // lblSelectPublisher
            // 
            this.lblSelectPublisher.AutoSize = true;
            this.lblSelectPublisher.Location = new System.Drawing.Point(25, 95);
            this.lblSelectPublisher.Name = "lblSelectPublisher";
            this.lblSelectPublisher.Size = new System.Drawing.Size(59, 13);
            this.lblSelectPublisher.TabIndex = 9;
            this.lblSelectPublisher.Text = "Publisher";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Report Category";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Report Name";
            // 
            // bnClose
            // 
            this.bnClose.Location = new System.Drawing.Point(914, 660);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 7;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // lblServiceRunning
            // 
            this.lblServiceRunning.AutoSize = true;
            this.lblServiceRunning.ForeColor = System.Drawing.Color.Red;
            this.lblServiceRunning.Location = new System.Drawing.Point(20, 665);
            this.lblServiceRunning.Name = "lblServiceRunning";
            this.lblServiceRunning.Size = new System.Drawing.Size(735, 13);
            this.lblServiceRunning.TabIndex = 8;
            this.lblServiceRunning.Text = "*The AuditWizard Service needs to be running for any scheduled reports to be proc" +
                "essed - currently the service is not running.";
            this.lblServiceRunning.Visible = false;
            this.lblServiceRunning.Click += new System.EventHandler(this.lblServiceRunning_Click);
            // 
            // FormViewSchedules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1001, 695);
            this.Controls.Add(this.lblServiceRunning);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormViewSchedules";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View/Edit Report Schedules";
            this.Load += new System.EventHandler(this.FormViewSchedules_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridReportSchedules)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridReportSchedules;
        private System.Windows.Forms.Button bnDeleteSchedule;
        private System.Windows.Forms.Button bnEditSchedule;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxReportName;
        private System.Windows.Forms.ComboBox comboBoxReportCategory;
        private System.Windows.Forms.Button bnCreateSchedule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.ComboBox cmbPublishers;
        private System.Windows.Forms.Label lblSelectPublisher;
        private System.Windows.Forms.Label lblServiceRunning;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bnLocationFilter;
        private System.Windows.Forms.TextBox tbLocations;
        private System.Windows.Forms.Button bnDeleteAllSchedules;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;

    }
}