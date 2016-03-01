namespace Layton.AuditWizard.Common
{
    partial class FormEnvVarBrowser
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
            this.lvEnvVar = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.bnOk = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvEnvVar
            // 
            this.lvEnvVar.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvEnvVar.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvEnvVar.Location = new System.Drawing.Point(24, 58);
            this.lvEnvVar.MultiSelect = false;
            this.lvEnvVar.Name = "lvEnvVar";
            this.lvEnvVar.Size = new System.Drawing.Size(436, 263);
            this.lvEnvVar.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEnvVar.TabIndex = 0;
            this.lvEnvVar.UseCompatibleStateImageBehavior = false;
            this.lvEnvVar.View = System.Windows.Forms.View.Details;
            this.lvEnvVar.SelectedIndexChanged += new System.EventHandler(this.lvEnvVar_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Variable";
            this.columnHeader1.Width = 380;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select an environment variable from the list:";
            // 
            // bnOk
            // 
            this.bnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOk.Enabled = false;
            this.bnOk.Location = new System.Drawing.Point(315, 354);
            this.bnOk.Name = "bnOk";
            this.bnOk.Size = new System.Drawing.Size(75, 23);
            this.bnOk.TabIndex = 2;
            this.bnOk.Text = "Select";
            this.bnOk.UseVisualStyleBackColor = true;
            this.bnOk.Click += new System.EventHandler(this.bnOk_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(396, 354);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 3;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // FormEnvVarBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 389);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvEnvVar);
            this.Name = "FormEnvVarBrowser";
            this.Text = "Environment Variable Browser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvEnvVar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnOk;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}