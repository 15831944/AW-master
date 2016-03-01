namespace Layton.AuditWizard.Common
{
	partial class FormFilterPublishers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFilterPublishers));
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            this.lbAvailablePublishers = new System.Windows.Forms.ListBox();
            this.lbFilteredPublishers = new System.Windows.Forms.ListBox();
            this.bnRemove = new System.Windows.Forms.Button();
            this.bnAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bnNewPublisher = new Infragistics.Win.Misc.UltraButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbAvailablePublishers
            // 
            this.lbAvailablePublishers.FormattingEnabled = true;
            this.lbAvailablePublishers.Location = new System.Drawing.Point(27, 47);
            this.lbAvailablePublishers.Name = "lbAvailablePublishers";
            this.lbAvailablePublishers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAvailablePublishers.Size = new System.Drawing.Size(315, 277);
            this.lbAvailablePublishers.Sorted = true;
            this.lbAvailablePublishers.TabIndex = 0;
            this.lbAvailablePublishers.SelectedIndexChanged += new System.EventHandler(this.lbAvailablePublishers_SelectedIndexChanged);
            this.lbAvailablePublishers.DoubleClick += new System.EventHandler(this.lbAvailablePublishers_DoubleClick);
            // 
            // lbFilteredPublishers
            // 
            this.lbFilteredPublishers.FormattingEnabled = true;
            this.lbFilteredPublishers.Location = new System.Drawing.Point(383, 47);
            this.lbFilteredPublishers.Name = "lbFilteredPublishers";
            this.lbFilteredPublishers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFilteredPublishers.Size = new System.Drawing.Size(315, 277);
            this.lbFilteredPublishers.Sorted = true;
            this.lbFilteredPublishers.TabIndex = 1;
            this.lbFilteredPublishers.SelectedIndexChanged += new System.EventHandler(this.lbFilteredPublishers_SelectedIndexChanged);
            this.lbFilteredPublishers.DoubleClick += new System.EventHandler(this.lbFilteredPublishers_DoubleClick);
            // 
            // bnRemove
            // 
            this.bnRemove.Enabled = false;
            this.bnRemove.Image = ((System.Drawing.Image)(resources.GetObject("bnRemove.Image")));
            this.bnRemove.Location = new System.Drawing.Point(348, 148);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(29, 23);
            this.bnRemove.TabIndex = 2;
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.bnRemove_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.Enabled = false;
            this.bnAdd.Image = ((System.Drawing.Image)(resources.GetObject("bnAdd.Image")));
            this.bnAdd.Location = new System.Drawing.Point(348, 177);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(29, 23);
            this.bnAdd.TabIndex = 6;
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(124, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Available Publishers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(485, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Filtered Publishers";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(652, 445);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 9;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(559, 445);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 10;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(10, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(729, 32);
            this.label3.TabIndex = 11;
            this.label3.Text = "The Publisher Filter allows you to select which publishers are displayed within A" +
                "uditWizard. If no publishers are selected, all publishers will be displayed.";
            // 
            // bnNewPublisher
            // 
            appearance11.ForeColor = System.Drawing.SystemColors.ControlText;
            appearance11.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.bnNewPublisher.Appearance = appearance11;
            this.bnNewPublisher.Location = new System.Drawing.Point(27, 330);
            this.bnNewPublisher.Name = "bnNewPublisher";
            this.bnNewPublisher.Size = new System.Drawing.Size(87, 23);
            this.bnNewPublisher.TabIndex = 28;
            this.bnNewPublisher.Text = "&New";
            this.bnNewPublisher.Click += new System.EventHandler(this.bnNewPublisher_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbFilteredPublishers);
            this.groupBox1.Controls.Add(this.bnNewPublisher);
            this.groupBox1.Controls.Add(this.lbAvailablePublishers);
            this.groupBox1.Controls.Add(this.bnRemove);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnAdd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(724, 365);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select publishers";
            // 
            // FormFilterPublishers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 480);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFilterPublishers";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Publisher Filter";
            this.Load += new System.EventHandler(this.FormFilterPublishers_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lbAvailablePublishers;
		private System.Windows.Forms.ListBox lbFilteredPublishers;
		private System.Windows.Forms.Button bnRemove;
		private System.Windows.Forms.Button bnAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Label label3;
		private Infragistics.Win.Misc.UltraButton bnNewPublisher;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}