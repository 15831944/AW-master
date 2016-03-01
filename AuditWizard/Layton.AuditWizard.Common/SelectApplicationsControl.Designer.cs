namespace Layton.AuditWizard.Common
{
	partial class SelectApplicationsControl
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.applicationsTree = new Infragistics.Win.UltraWinTree.UltraTree();
            ((System.ComponentModel.ISupportInitialize)(this.applicationsTree)).BeginInit();
            this.SuspendLayout();
            // 
            // applicationsTree
            // 
            this.applicationsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applicationsTree.Location = new System.Drawing.Point(0, 0);
            this.applicationsTree.Name = "applicationsTree";
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
            this.applicationsTree.Override = _override1;
            this.applicationsTree.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.applicationsTree.Size = new System.Drawing.Size(302, 510);
            this.applicationsTree.TabIndex = 0;
            this.applicationsTree.BeforeCheck += new Infragistics.Win.UltraWinTree.BeforeCheckEventHandler(this.applicationsTree_BeforeCheck);
            this.applicationsTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.applicationsTree_BeforeExpand);
            this.applicationsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.applicationsTree_AfterCheck);
            // 
            // SelectApplicationsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.applicationsTree);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "SelectApplicationsControl";
            this.Size = new System.Drawing.Size(302, 510);
            ((System.ComponentModel.ISupportInitialize)(this.applicationsTree)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinTree.UltraTree applicationsTree;
	}
}
