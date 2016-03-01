namespace Layton.AuditWizard.Reports
{
    partial class FormSQLReport
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
            this.textBoxSQLString = new System.Windows.Forms.TextBox();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.bnSave = new System.Windows.Forms.Button();
            this.gb4 = new System.Windows.Forms.GroupBox();
            this.lbReportName = new System.Windows.Forms.Label();
            this.tbReportName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gb4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSQLString
            // 
            this.textBoxSQLString.Location = new System.Drawing.Point(19, 24);
            this.textBoxSQLString.Multiline = true;
            this.textBoxSQLString.Name = "textBoxSQLString";
            this.textBoxSQLString.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSQLString.Size = new System.Drawing.Size(738, 350);
            this.textBoxSQLString.TabIndex = 0;
            this.textBoxSQLString.TextChanged += new System.EventHandler(this.textBoxSQLString_TextChanged);
            this.textBoxSQLString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSQLString_KeyDown);
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(690, 30);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(70, 23);
            this.bnOK.TabIndex = 1;
            this.bnOK.Text = "Run";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnClose
            // 
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(719, 519);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(70, 23);
            this.bnClose.TabIndex = 2;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // bnSave
            // 
            this.bnSave.Enabled = false;
            this.bnSave.Location = new System.Drawing.Point(614, 30);
            this.bnSave.Name = "bnSave";
            this.bnSave.Size = new System.Drawing.Size(70, 23);
            this.bnSave.TabIndex = 4;
            this.bnSave.Text = "Save";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(this.bnSave_Click);
            // 
            // gb4
            // 
            this.gb4.Controls.Add(this.lbReportName);
            this.gb4.Controls.Add(this.bnSave);
            this.gb4.Controls.Add(this.tbReportName);
            this.gb4.Controls.Add(this.bnOK);
            this.gb4.Location = new System.Drawing.Point(11, 435);
            this.gb4.Name = "gb4";
            this.gb4.Size = new System.Drawing.Size(778, 68);
            this.gb4.TabIndex = 69;
            this.gb4.TabStop = false;
            this.gb4.Text = "Save and/or run report";
            // 
            // lbReportName
            // 
            this.lbReportName.AutoSize = true;
            this.lbReportName.Location = new System.Drawing.Point(10, 35);
            this.lbReportName.Name = "lbReportName";
            this.lbReportName.Size = new System.Drawing.Size(86, 13);
            this.lbReportName.TabIndex = 60;
            this.lbReportName.Text = "Report name:";
            // 
            // tbReportName
            // 
            this.tbReportName.Location = new System.Drawing.Point(102, 31);
            this.tbReportName.Name = "tbReportName";
            this.tbReportName.Size = new System.Drawing.Size(487, 21);
            this.tbReportName.TabIndex = 8;
            this.tbReportName.TextChanged += new System.EventHandler(this.tbReportName_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxSQLString);
            this.groupBox1.Location = new System.Drawing.Point(12, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(777, 399);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enter a SQL string to run against the database";
            // 
            // FormSQLReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 552);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb4);
            this.Controls.Add(this.bnClose);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSQLReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Report";
            this.gb4.ResumeLayout(false);
            this.gb4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSQLString;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Button bnSave;
        private System.Windows.Forms.GroupBox gb4;
        private System.Windows.Forms.Label lbReportName;
        private System.Windows.Forms.TextBox tbReportName;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}