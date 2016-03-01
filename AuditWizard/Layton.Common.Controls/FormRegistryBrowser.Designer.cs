namespace Layton.Common.Controls
{
    partial class FormRegistryBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegistryBrowser));
            this.tvRegistry = new System.Windows.Forms.TreeView();
            this.imageListRegistry = new System.Windows.Forms.ImageList(this.components);
            this.lvItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tvRegistry
            // 
            this.tvRegistry.ImageIndex = 0;
            this.tvRegistry.ImageList = this.imageListRegistry;
            this.tvRegistry.Location = new System.Drawing.Point(23, 50);
            this.tvRegistry.Name = "tvRegistry";
            this.tvRegistry.SelectedImageIndex = 1;
            this.tvRegistry.Size = new System.Drawing.Size(343, 359);
            this.tvRegistry.TabIndex = 0;
            this.tvRegistry.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvRegistry_BeforeExpand);
            this.tvRegistry.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvRegistry_AfterSelect);
            // 
            // imageListRegistry
            // 
            this.imageListRegistry.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListRegistry.ImageStream")));
            this.imageListRegistry.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListRegistry.Images.SetKeyName(0, "open_folder_full.png");
            this.imageListRegistry.Images.SetKeyName(1, "open_folder_accept.png");
            // 
            // lvItems
            // 
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvItems.Location = new System.Drawing.Point(385, 50);
            this.lvItems.MultiSelect = false;
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(343, 359);
            this.lvItems.TabIndex = 1;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            this.lvItems.SelectedIndexChanged += new System.EventHandler(this.lvItems_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Value";
            this.columnHeader1.Width = 339;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(572, 427);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Select";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(653, 427);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select a registry key and value:";
            // 
            // FormRegistryBrowser
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(761, 466);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lvItems);
            this.Controls.Add(this.tvRegistry);
            this.Name = "FormRegistryBrowser";
            this.Text = "Registry Browser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvRegistry;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageListRegistry;
    }
}