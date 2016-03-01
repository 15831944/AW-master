namespace Layton.AuditWizard.Common
{
	partial class SelectLocationsControl
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
			this.locationsTree = new Infragistics.Win.UltraWinTree.UltraTree();
			((System.ComponentModel.ISupportInitialize)(this.locationsTree)).BeginInit();
			this.SuspendLayout();
			// 
			// locationsTree
			// 
			this.locationsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.locationsTree.Location = new System.Drawing.Point(0, 0);
			this.locationsTree.Name = "locationsTree";
			_override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
			_override1.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.CheckOnExpand;
			_override1.Sort = Infragistics.Win.UltraWinTree.SortType.None;
			this.locationsTree.Override = _override1;
			this.locationsTree.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
			this.locationsTree.Size = new System.Drawing.Size(302, 510);
			this.locationsTree.TabIndex = 0;
			this.locationsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.locationsTree_AfterCheck);
			this.locationsTree.BeforeCheck += new Infragistics.Win.UltraWinTree.BeforeCheckEventHandler(this.locationsTree_BeforeCheck);
			this.locationsTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.networkTree_BeforeExpand);
			// 
			// SelectLocationsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.locationsTree);
			this.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.Name = "SelectLocationsControl";
			this.Size = new System.Drawing.Size(302, 510);
			this.Load += new System.EventHandler(this.SelectLocationsControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.locationsTree)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinTree.UltraTree locationsTree;
	}
}
