namespace Layton.AuditWizard.Network
{
	partial class FormOperationsLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOperationsLog));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OperationsTable", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DataObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("queuedat");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("assetname");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("operation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("completedat");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("messages");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            this.operationsGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.operationsDataSet = new System.Data.DataSet();
            this.operationsTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.bnServiceLog = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAlwaysShowLog = new System.Windows.Forms.CheckBox();
            this.dtpStartDateTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.operationsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpStartDateTime)).BeginInit();
            this.operationsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("footerPictureBox.Image")));
            this.footerPictureBox.Location = new System.Drawing.Point(658, 467);
            // 
            // operationsGridView
            // 
            this.operationsGridView.AllowDrop = true;
            this.operationsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.operationsGridView.DataSource = this.operationsDataSet;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.operationsGridView.DisplayLayout.AddNewBox.Appearance = appearance3;
            this.operationsGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance4.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance4.ImageBackground")));
            appearance4.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance4.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance4.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.operationsGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance4;
            this.operationsGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.operationsGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance5.BackColor = System.Drawing.Color.White;
            this.operationsGridView.DisplayLayout.Appearance = appearance5;
            this.operationsGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Format = "G";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateWithoutDropDown;
            ultraGridColumn2.Width = 169;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 189;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 169;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 169;
            ultraGridColumn6.Format = "G";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Nothing;
            ultraGridColumn6.NullText = "-";
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateWithoutDropDown;
            ultraGridColumn6.Width = 195;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 169;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance1.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance1;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            ultraGridBand1.Override.RowAlternateAppearance = appearance2;
            this.operationsGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.operationsGridView.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance7.FontData.Name = "Trebuchet MS";
            appearance7.FontData.SizeInPoints = 9F;
            appearance7.ForeColor = System.Drawing.Color.White;
            appearance7.TextHAlignAsString = "Right";
            this.operationsGridView.DisplayLayout.CaptionAppearance = appearance7;
            this.operationsGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.operationsGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("operationsGridView.DisplayLayout.FixedHeaderOffImage")));
            this.operationsGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("operationsGridView.DisplayLayout.FixedHeaderOnImage")));
            this.operationsGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("operationsGridView.DisplayLayout.FixedRowOffImage")));
            this.operationsGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("operationsGridView.DisplayLayout.FixedRowOnImage")));
            appearance8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance8.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance8.FontData.BoldAsString = "True";
            appearance8.FontData.Name = "Verdana";
            appearance8.FontData.SizeInPoints = 10F;
            appearance8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.operationsGridView.DisplayLayout.GroupByBox.Appearance = appearance8;
            this.operationsGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.operationsGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance11.BackColor = System.Drawing.Color.Transparent;
            appearance11.FontData.Name = "Verdana";
            this.operationsGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance11;
            this.operationsGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.operationsGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.operationsGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.operationsGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance12.BackColor = System.Drawing.Color.Transparent;
            this.operationsGridView.DisplayLayout.Override.CardAreaAppearance = appearance12;
            appearance33.BorderColor = System.Drawing.Color.Transparent;
            appearance33.FontData.Name = "Verdana";
            this.operationsGridView.DisplayLayout.Override.CellAppearance = appearance33;
            appearance34.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance34.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance34.ImageBackground")));
            appearance34.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance34.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.operationsGridView.DisplayLayout.Override.CellButtonAppearance = appearance34;
            this.operationsGridView.DisplayLayout.Override.CellPadding = 4;
            appearance35.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.operationsGridView.DisplayLayout.Override.FilterCellAppearance = appearance35;
            appearance36.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance36.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance36.ImageBackground")));
            appearance36.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.operationsGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance36;
            appearance37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance37.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.operationsGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance37;
            this.operationsGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance38.BackColor2 = System.Drawing.Color.White;
            appearance38.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance38.FontData.BoldAsString = "True";
            appearance38.FontData.Name = "Verdana";
            appearance38.FontData.SizeInPoints = 8F;
            appearance38.ForeColor = System.Drawing.Color.DimGray;
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance38.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance38.TextHAlignAsString = "Center";
            this.operationsGridView.DisplayLayout.Override.HeaderAppearance = appearance38;
            appearance39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.operationsGridView.DisplayLayout.Override.RowAlternateAppearance = appearance39;
            appearance40.BorderColor = System.Drawing.Color.LightGray;
            this.operationsGridView.DisplayLayout.Override.RowAppearance = appearance40;
            appearance41.BackColor = System.Drawing.Color.White;
            this.operationsGridView.DisplayLayout.Override.RowSelectorAppearance = appearance41;
            this.operationsGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance42.BorderColor = System.Drawing.Color.Transparent;
            appearance42.ForeColor = System.Drawing.Color.Black;
            this.operationsGridView.DisplayLayout.Override.SelectedCellAppearance = appearance42;
            appearance43.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance43.BorderColor = System.Drawing.Color.Transparent;
            appearance43.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance43.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance43.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.operationsGridView.DisplayLayout.Override.SelectedRowAppearance = appearance43;
            appearance44.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance44.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance44;
            appearance45.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance45.ImageBackground")));
            appearance45.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance45;
            appearance46.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance46.ImageBackground")));
            appearance46.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance46.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance46;
            appearance47.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance47.ImageBackground")));
            appearance47.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance47;
            appearance48.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance48.ImageBackground")));
            appearance48.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance48.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance48;
            this.operationsGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.operationsGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.operationsGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.operationsGridView.Location = new System.Drawing.Point(9, 106);
            this.operationsGridView.Name = "operationsGridView";
            this.operationsGridView.Size = new System.Drawing.Size(1081, 315);
            this.operationsGridView.TabIndex = 10;
            this.operationsGridView.Text = "Grid Caption Area";
            this.operationsGridView.UseAppStyling = false;
            this.operationsGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // operationsDataSet
            // 
            this.operationsDataSet.DataSetName = "NewDataSet";
            this.operationsDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.operationsTable});
            // 
            // operationsTable
            // 
            this.operationsTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn4,
            this.dataColumn3,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7});
            this.operationsTable.TableName = "OperationsTable";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "DataObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Queued At";
            this.dataColumn2.ColumnName = "queuedat";
            this.dataColumn2.DataType = typeof(System.DateTime);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Asset Name";
            this.dataColumn4.ColumnName = "assetname";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Operation";
            this.dataColumn3.ColumnName = "operation";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Status";
            this.dataColumn5.ColumnName = "status";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Completed At";
            this.dataColumn6.ColumnName = "completedat";
            this.dataColumn6.DataType = typeof(System.DateTime);
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Messages";
            this.dataColumn7.ColumnName = "messages";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1077, 41);
            this.label1.TabIndex = 11;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(899, 437);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 23;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(999, 437);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 24;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 5000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // bnServiceLog
            // 
            this.bnServiceLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnServiceLog.Location = new System.Drawing.Point(765, 437);
            this.bnServiceLog.Name = "bnServiceLog";
            this.bnServiceLog.Size = new System.Drawing.Size(127, 24);
            this.bnServiceLog.TabIndex = 26;
            this.bnServiceLog.Text = "&View Service Log";
            this.bnServiceLog.UseVisualStyleBackColor = true;
            this.bnServiceLog.Click += new System.EventHandler(this.bnServiceLog_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(14, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Start Date\\Time:";
            // 
            // cbAlwaysShowLog
            // 
            this.cbAlwaysShowLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbAlwaysShowLog.AutoSize = true;
            this.cbAlwaysShowLog.Checked = global::Layton.AuditWizard.Network.Properties.Settings.Default.AlwaysShowOperationsLog;
            this.cbAlwaysShowLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAlwaysShowLog.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Layton.AuditWizard.Network.Properties.Settings.Default, "AlwaysShowOperationsLog", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbAlwaysShowLog.Location = new System.Drawing.Point(9, 442);
            this.cbAlwaysShowLog.Name = "cbAlwaysShowLog";
            this.cbAlwaysShowLog.Size = new System.Drawing.Size(319, 17);
            this.cbAlwaysShowLog.TabIndex = 29;
            this.cbAlwaysShowLog.Text = "Always show this log after an AuditAgent Operation";
            this.cbAlwaysShowLog.UseVisualStyleBackColor = true;
            // 
            // dtpStartDateTime
            // 
            this.dtpStartDateTime.Location = new System.Drawing.Point(133, 65);
            this.dtpStartDateTime.MaskInput = "{date} {time}";
            this.dtpStartDateTime.Name = "dtpStartDateTime";
            this.dtpStartDateTime.Size = new System.Drawing.Size(162, 22);
            this.dtpStartDateTime.TabIndex = 30;
            this.dtpStartDateTime.ValueChanged += new System.EventHandler(this.dtpStartDateTime_ValueChanged);
            // 
            // operationsMenuStrip
            // 
            this.operationsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.operationsMenuStrip.Name = "OSMenuStrip";
            this.operationsMenuStrip.Size = new System.Drawing.Size(169, 26);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.excel_16;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.pdf;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.xps_16;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            // 
            // bnDelete
            // 
            this.bnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDelete.Location = new System.Drawing.Point(631, 437);
            this.bnDelete.Name = "bnDelete";
            this.bnDelete.Size = new System.Drawing.Size(127, 24);
            this.bnDelete.TabIndex = 31;
            this.bnDelete.Text = "&Delete Entries";
            this.bnDelete.UseVisualStyleBackColor = true;
            this.bnDelete.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // FormOperationsLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(1105, 587);
            this.Controls.Add(this.bnDelete);
            this.Controls.Add(this.dtpStartDateTime);
            this.Controls.Add(this.cbAlwaysShowLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bnServiceLog);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.operationsGridView);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "FormOperationsLog";
            this.Text = "AuditWizard Operations Log";
            this.Load += new System.EventHandler(this.FormOperationsLog_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.operationsGridView, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnServiceLog, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cbAlwaysShowLog, 0);
            this.Controls.SetChildIndex(this.dtpStartDateTime, 0);
            this.Controls.SetChildIndex(this.bnDelete, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.operationsTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpStartDateTime)).EndInit();
            this.operationsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinGrid.UltraGrid operationsGridView;
		private System.Data.DataSet operationsDataSet;
		private System.Data.DataTable operationsTable;
		private System.Data.DataColumn dataColumn1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Timer timerRefresh;
		private System.Windows.Forms.Button bnServiceLog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox cbAlwaysShowLog;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtpStartDateTime;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataColumn dataColumn7;
		private System.Windows.Forms.ContextMenuStrip operationsMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
		private System.Windows.Forms.Button bnDelete;
	}
}
