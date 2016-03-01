namespace Layton.AuditWizard.Common
{
	partial class SelectAuditedDataFieldsControl
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
			this.fieldsTree = new Infragistics.Win.UltraWinTree.UltraTree();
			((System.ComponentModel.ISupportInitialize)(this.fieldsTree)).BeginInit();
			this.SuspendLayout();
			// 
			// fieldsTree
			// 
			this.fieldsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldsTree.HideSelection = false;
			this.fieldsTree.Location = new System.Drawing.Point(0, 0);
			this.fieldsTree.Name = "fieldsTree";
			_override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
			this.fieldsTree.Override = _override1;
			this.fieldsTree.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
			this.fieldsTree.Size = new System.Drawing.Size(357, 537);
			this.fieldsTree.TabIndex = 11;
			this.fieldsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.fieldsTree_AfterCheck);
			this.fieldsTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.fieldsTree_AfterSelect);
			this.fieldsTree.BeforeCheck += new Infragistics.Win.UltraWinTree.BeforeCheckEventHandler(this.fieldsTree_BeforeCheck);
			this.fieldsTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.networkTree_BeforeExpand);
			// 
			// SelectAuditedDataFieldsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.fieldsTree);
			this.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.Name = "SelectAuditedDataFieldsControl";
			this.Size = new System.Drawing.Size(357, 537);
			this.Load += new System.EventHandler(this.SelectAuditedDataFieldsControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.fieldsTree)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinTree.UltraTree fieldsTree;
	}
}
