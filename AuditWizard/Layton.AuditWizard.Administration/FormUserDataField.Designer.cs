namespace Layton.AuditWizard.Administration
{
	partial class FormUserDataField
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.tbFieldName = new System.Windows.Forms.TextBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.cbFieldType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblCase = new Infragistics.Win.Misc.UltraLabel();
            this.InputCase = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblMinimumValue = new Infragistics.Win.Misc.UltraLabel();
            this.lblMaximumValue = new Infragistics.Win.Misc.UltraLabel();
            this.Picklist = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblEnvironmentVariable = new Infragistics.Win.Misc.UltraLabel();
            this.tbEnvironmentVariableName = new System.Windows.Forms.TextBox();
            this.lblRegistryKey = new Infragistics.Win.Misc.UltraLabel();
            this.tbRegistryKey = new System.Windows.Forms.TextBox();
            this.lblRegistryValue = new Infragistics.Win.Misc.UltraLabel();
            this.tbRegistryValue = new System.Windows.Forms.TextBox();
            this.lblPicklist = new Infragistics.Win.Misc.UltraLabel();
            this.cbCategories = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbMandatory = new System.Windows.Forms.CheckBox();
            this.pnlEnvVar = new System.Windows.Forms.Panel();
            this.bnEnvVarBrowse = new System.Windows.Forms.Button();
            this.pnlText = new System.Windows.Forms.Panel();
            this.pnlRegistry = new System.Windows.Forms.Panel();
            this.bnRegistryBrowse = new System.Windows.Forms.Button();
            this.pnlNumeric = new System.Windows.Forms.Panel();
            this.nupMax = new System.Windows.Forms.NumericUpDown();
            this.nupMin = new System.Windows.Forms.NumericUpDown();
            this.cbInteractiveInclude = new System.Windows.Forms.CheckBox();
            this.pnlPicklist = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputCase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Picklist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCategories)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pnlEnvVar.SuspendLayout();
            this.pnlText.SuspendLayout();
            this.pnlRegistry.SuspendLayout();
            this.pnlNumeric.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupMin)).BeginInit();
            this.pnlPicklist.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance2;
            this.ultraLabel1.Location = new System.Drawing.Point(15, 26);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(107, 18);
            this.ultraLabel1.TabIndex = 30;
            this.ultraLabel1.Text = "Category Name:";
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(292, 317);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 28;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(387, 317);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 29;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // ultraLabel4
            // 
            appearance8.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel4.Appearance = appearance8;
            this.ultraLabel4.Location = new System.Drawing.Point(15, 63);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(77, 18);
            this.ultraLabel4.TabIndex = 0;
            this.ultraLabel4.Text = "Field Name:";
            // 
            // tbFieldName
            // 
            this.tbFieldName.Location = new System.Drawing.Point(128, 60);
            this.tbFieldName.Name = "tbFieldName";
            this.tbFieldName.Size = new System.Drawing.Size(308, 21);
            this.tbFieldName.TabIndex = 1;
            // 
            // ultraLabel5
            // 
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel5.Appearance = appearance9;
            this.ultraLabel5.Location = new System.Drawing.Point(15, 100);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(77, 17);
            this.ultraLabel5.TabIndex = 2;
            this.ultraLabel5.Text = "Field Type:";
            // 
            // cbFieldType
            // 
            this.cbFieldType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbFieldType.Location = new System.Drawing.Point(128, 96);
            this.cbFieldType.Name = "cbFieldType";
            this.cbFieldType.Size = new System.Drawing.Size(214, 22);
            this.cbFieldType.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.cbFieldType.TabIndex = 2;
            this.cbFieldType.SelectionChanged += new System.EventHandler(this.FieldType_SelectionChanged);
            // 
            // lblCase
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.lblCase.Appearance = appearance5;
            this.lblCase.Location = new System.Drawing.Point(2, 8);
            this.lblCase.Name = "lblCase";
            this.lblCase.Size = new System.Drawing.Size(78, 17);
            this.lblCase.TabIndex = 13;
            this.lblCase.Text = "Input Case:";
            // 
            // InputCase
            // 
            this.InputCase.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.InputCase.Location = new System.Drawing.Point(114, 3);
            this.InputCase.Name = "InputCase";
            this.InputCase.Size = new System.Drawing.Size(152, 22);
            this.InputCase.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.InputCase.TabIndex = 14;
            // 
            // lblMinimumValue
            // 
            appearance7.BackColor = System.Drawing.Color.Transparent;
            this.lblMinimumValue.Appearance = appearance7;
            this.lblMinimumValue.Location = new System.Drawing.Point(3, 5);
            this.lblMinimumValue.Name = "lblMinimumValue";
            this.lblMinimumValue.Size = new System.Drawing.Size(102, 19);
            this.lblMinimumValue.TabIndex = 15;
            this.lblMinimumValue.Text = "Min Value:";
            // 
            // lblMaximumValue
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.lblMaximumValue.Appearance = appearance4;
            this.lblMaximumValue.Location = new System.Drawing.Point(192, 5);
            this.lblMaximumValue.Name = "lblMaximumValue";
            this.lblMaximumValue.Size = new System.Drawing.Size(70, 19);
            this.lblMaximumValue.TabIndex = 17;
            this.lblMaximumValue.Text = "Max value:";
            // 
            // Picklist
            // 
            this.Picklist.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.Picklist.Location = new System.Drawing.Point(113, 3);
            this.Picklist.Name = "Picklist";
            this.Picklist.Size = new System.Drawing.Size(227, 22);
            this.Picklist.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.Picklist.TabIndex = 20;
            // 
            // lblEnvironmentVariable
            // 
            appearance14.BackColor = System.Drawing.Color.Transparent;
            this.lblEnvironmentVariable.Appearance = appearance14;
            this.lblEnvironmentVariable.Location = new System.Drawing.Point(3, 6);
            this.lblEnvironmentVariable.Name = "lblEnvironmentVariable";
            this.lblEnvironmentVariable.Size = new System.Drawing.Size(50, 18);
            this.lblEnvironmentVariable.TabIndex = 21;
            this.lblEnvironmentVariable.Text = "Value:";
            // 
            // tbEnvironmentVariableName
            // 
            this.tbEnvironmentVariableName.Location = new System.Drawing.Point(114, 3);
            this.tbEnvironmentVariableName.Name = "tbEnvironmentVariableName";
            this.tbEnvironmentVariableName.Size = new System.Drawing.Size(279, 21);
            this.tbEnvironmentVariableName.TabIndex = 22;
            // 
            // lblRegistryKey
            // 
            appearance12.BackColor = System.Drawing.Color.Transparent;
            this.lblRegistryKey.Appearance = appearance12;
            this.lblRegistryKey.Location = new System.Drawing.Point(3, 5);
            this.lblRegistryKey.Name = "lblRegistryKey";
            this.lblRegistryKey.Size = new System.Drawing.Size(92, 20);
            this.lblRegistryKey.TabIndex = 23;
            this.lblRegistryKey.Text = "Registry Key:";
            // 
            // tbRegistryKey
            // 
            this.tbRegistryKey.Location = new System.Drawing.Point(113, 2);
            this.tbRegistryKey.Name = "tbRegistryKey";
            this.tbRegistryKey.Size = new System.Drawing.Size(280, 21);
            this.tbRegistryKey.TabIndex = 24;
            // 
            // lblRegistryValue
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.lblRegistryValue.Appearance = appearance3;
            this.lblRegistryValue.Location = new System.Drawing.Point(3, 31);
            this.lblRegistryValue.Name = "lblRegistryValue";
            this.lblRegistryValue.Size = new System.Drawing.Size(92, 18);
            this.lblRegistryValue.TabIndex = 26;
            this.lblRegistryValue.Text = "Registry Value:";
            // 
            // tbRegistryValue
            // 
            this.tbRegistryValue.Location = new System.Drawing.Point(113, 28);
            this.tbRegistryValue.Name = "tbRegistryValue";
            this.tbRegistryValue.Size = new System.Drawing.Size(280, 21);
            this.tbRegistryValue.TabIndex = 27;
            // 
            // lblPicklist
            // 
            appearance11.BackColor = System.Drawing.Color.Transparent;
            this.lblPicklist.Appearance = appearance11;
            this.lblPicklist.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lblPicklist.Location = new System.Drawing.Point(4, 7);
            this.lblPicklist.Name = "lblPicklist";
            this.lblPicklist.Size = new System.Drawing.Size(56, 19);
            this.lblPicklist.TabIndex = 19;
            this.lblPicklist.Text = "PickList:";
            // 
            // cbCategories
            // 
            this.cbCategories.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbCategories.Location = new System.Drawing.Point(128, 24);
            this.cbCategories.Name = "cbCategories";
            this.cbCategories.Size = new System.Drawing.Size(309, 22);
            this.cbCategories.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlNumeric);
            this.groupBox1.Controls.Add(this.cbMandatory);
            this.groupBox1.Controls.Add(this.pnlEnvVar);
            this.groupBox1.Controls.Add(this.pnlText);
            this.groupBox1.Controls.Add(this.pnlRegistry);
            this.groupBox1.Controls.Add(this.cbInteractiveInclude);
            this.groupBox1.Controls.Add(this.ultraLabel1);
            this.groupBox1.Controls.Add(this.cbCategories);
            this.groupBox1.Controls.Add(this.pnlPicklist);
            this.groupBox1.Controls.Add(this.ultraLabel4);
            this.groupBox1.Controls.Add(this.tbFieldName);
            this.groupBox1.Controls.Add(this.ultraLabel5);
            this.groupBox1.Controls.Add(this.cbFieldType);
            this.groupBox1.Location = new System.Drawing.Point(16, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 250);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            // 
            // cbMandatory
            // 
            this.cbMandatory.AutoSize = true;
            this.cbMandatory.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbMandatory.Enabled = false;
            this.cbMandatory.Location = new System.Drawing.Point(201, 137);
            this.cbMandatory.Name = "cbMandatory";
            this.cbMandatory.Size = new System.Drawing.Size(86, 17);
            this.cbMandatory.TabIndex = 4;
            this.cbMandatory.Text = "Mandatory";
            this.cbMandatory.UseVisualStyleBackColor = true;
            // 
            // pnlEnvVar
            // 
            this.pnlEnvVar.Controls.Add(this.bnEnvVarBrowse);
            this.pnlEnvVar.Controls.Add(this.tbEnvironmentVariableName);
            this.pnlEnvVar.Controls.Add(this.lblEnvironmentVariable);
            this.pnlEnvVar.Location = new System.Drawing.Point(15, 174);
            this.pnlEnvVar.Name = "pnlEnvVar";
            this.pnlEnvVar.Size = new System.Drawing.Size(437, 28);
            this.pnlEnvVar.TabIndex = 34;
            // 
            // bnEnvVarBrowse
            // 
            this.bnEnvVarBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bnEnvVarBrowse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnEnvVarBrowse.Image = global::Layton.AuditWizard.Administration.Properties.Resources.add16;
            this.bnEnvVarBrowse.Location = new System.Drawing.Point(399, 3);
            this.bnEnvVarBrowse.MaximumSize = new System.Drawing.Size(20, 20);
            this.bnEnvVarBrowse.MinimumSize = new System.Drawing.Size(20, 20);
            this.bnEnvVarBrowse.Name = "bnEnvVarBrowse";
            this.bnEnvVarBrowse.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnEnvVarBrowse.Size = new System.Drawing.Size(20, 20);
            this.bnEnvVarBrowse.TabIndex = 35;
            this.toolTip1.SetToolTip(this.bnEnvVarBrowse, "Browse local environment variables");
            this.bnEnvVarBrowse.UseVisualStyleBackColor = true;
            this.bnEnvVarBrowse.Click += new System.EventHandler(this.bnEnvVarBrowse_Click);
            // 
            // pnlText
            // 
            this.pnlText.Controls.Add(this.lblCase);
            this.pnlText.Controls.Add(this.InputCase);
            this.pnlText.Location = new System.Drawing.Point(15, 174);
            this.pnlText.Name = "pnlText";
            this.pnlText.Size = new System.Drawing.Size(423, 28);
            this.pnlText.TabIndex = 36;
            // 
            // pnlRegistry
            // 
            this.pnlRegistry.Controls.Add(this.bnRegistryBrowse);
            this.pnlRegistry.Controls.Add(this.lblRegistryKey);
            this.pnlRegistry.Controls.Add(this.tbRegistryKey);
            this.pnlRegistry.Controls.Add(this.tbRegistryValue);
            this.pnlRegistry.Controls.Add(this.lblRegistryValue);
            this.pnlRegistry.Location = new System.Drawing.Point(15, 174);
            this.pnlRegistry.Name = "pnlRegistry";
            this.pnlRegistry.Size = new System.Drawing.Size(422, 53);
            this.pnlRegistry.TabIndex = 33;
            // 
            // bnRegistryBrowse
            // 
            this.bnRegistryBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bnRegistryBrowse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnRegistryBrowse.Image = global::Layton.AuditWizard.Administration.Properties.Resources.add16;
            this.bnRegistryBrowse.Location = new System.Drawing.Point(399, 14);
            this.bnRegistryBrowse.MaximumSize = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.MinimumSize = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.Name = "bnRegistryBrowse";
            this.bnRegistryBrowse.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bnRegistryBrowse.Size = new System.Drawing.Size(20, 20);
            this.bnRegistryBrowse.TabIndex = 34;
            this.toolTip1.SetToolTip(this.bnRegistryBrowse, "Browse local registry");
            this.bnRegistryBrowse.UseVisualStyleBackColor = true;
            this.bnRegistryBrowse.Click += new System.EventHandler(this.bnRegistryBrowse_Click);
            // 
            // pnlNumeric
            // 
            this.pnlNumeric.Controls.Add(this.nupMax);
            this.pnlNumeric.Controls.Add(this.nupMin);
            this.pnlNumeric.Controls.Add(this.lblMaximumValue);
            this.pnlNumeric.Controls.Add(this.lblMinimumValue);
            this.pnlNumeric.Location = new System.Drawing.Point(15, 174);
            this.pnlNumeric.Name = "pnlNumeric";
            this.pnlNumeric.Size = new System.Drawing.Size(396, 29);
            this.pnlNumeric.TabIndex = 37;
            // 
            // nupMax
            // 
            this.nupMax.Location = new System.Drawing.Point(268, 3);
            this.nupMax.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nupMax.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nupMax.Name = "nupMax";
            this.nupMax.Size = new System.Drawing.Size(100, 21);
            this.nupMax.TabIndex = 19;
            // 
            // nupMin
            // 
            this.nupMin.Location = new System.Drawing.Point(78, 3);
            this.nupMin.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nupMin.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nupMin.Name = "nupMin";
            this.nupMin.Size = new System.Drawing.Size(100, 21);
            this.nupMin.TabIndex = 18;
            // 
            // cbInteractiveInclude
            // 
            this.cbInteractiveInclude.AutoSize = true;
            this.cbInteractiveInclude.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbInteractiveInclude.Location = new System.Drawing.Point(15, 137);
            this.cbInteractiveInclude.Name = "cbInteractiveInclude";
            this.cbInteractiveInclude.Size = new System.Drawing.Size(178, 17);
            this.cbInteractiveInclude.TabIndex = 3;
            this.cbInteractiveInclude.Text = "Include in interactive audit";
            this.cbInteractiveInclude.UseVisualStyleBackColor = true;
            this.cbInteractiveInclude.CheckedChanged += new System.EventHandler(this.cbInteractiveInclude_CheckedChanged);
            // 
            // pnlPicklist
            // 
            this.pnlPicklist.Controls.Add(this.Picklist);
            this.pnlPicklist.Controls.Add(this.lblPicklist);
            this.pnlPicklist.Location = new System.Drawing.Point(15, 174);
            this.pnlPicklist.Name = "pnlPicklist";
            this.pnlPicklist.Size = new System.Drawing.Size(396, 28);
            this.pnlPicklist.TabIndex = 35;
            // 
            // toolTip1
            // 
            this.toolTip1.Tag = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Enter details for user-defined data:";
            // 
            // FormUserDataField
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(491, 353);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUserDataField";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User-defined data";
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputCase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Picklist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCategories)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlEnvVar.ResumeLayout(false);
            this.pnlEnvVar.PerformLayout();
            this.pnlText.ResumeLayout(false);
            this.pnlText.PerformLayout();
            this.pnlRegistry.ResumeLayout(false);
            this.pnlRegistry.PerformLayout();
            this.pnlNumeric.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nupMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupMin)).EndInit();
            this.pnlPicklist.ResumeLayout(false);
            this.pnlPicklist.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.TextBox tbFieldName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbFieldType;
		private Infragistics.Win.Misc.UltraLabel lblCase;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor InputCase;
		private Infragistics.Win.Misc.UltraLabel lblMinimumValue;
        private Infragistics.Win.Misc.UltraLabel lblMaximumValue;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor Picklist;
		private Infragistics.Win.Misc.UltraLabel lblEnvironmentVariable;
		private System.Windows.Forms.TextBox tbEnvironmentVariableName;
		private Infragistics.Win.Misc.UltraLabel lblRegistryKey;
		private System.Windows.Forms.TextBox tbRegistryKey;
		private Infragistics.Win.Misc.UltraLabel lblRegistryValue;
		private System.Windows.Forms.TextBox tbRegistryValue;
        private Infragistics.Win.Misc.UltraLabel lblPicklist;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbCategories;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlRegistry;
        private System.Windows.Forms.Panel pnlEnvVar;
        private System.Windows.Forms.Panel pnlPicklist;
        private System.Windows.Forms.Panel pnlText;
        private System.Windows.Forms.Panel pnlNumeric;
        private System.Windows.Forms.CheckBox cbInteractiveInclude;
        private System.Windows.Forms.CheckBox cbMandatory;
        private System.Windows.Forms.NumericUpDown nupMax;
        private System.Windows.Forms.NumericUpDown nupMin;
        private System.Windows.Forms.Button bnRegistryBrowse;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bnEnvVarBrowse;
        private System.Windows.Forms.Label label1;
	}
}