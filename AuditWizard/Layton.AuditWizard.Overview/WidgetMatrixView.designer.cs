namespace Layton.AuditWizard.Overview
{
    partial class WidgetMatrixView
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
            this.components = new System.ComponentModel.Container();
            this.layoutManager = new Infragistics.Win.Misc.UltraGridBagLayoutManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutManager)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutManager
            // 
            this.layoutManager.ContainerControl = this;
            this.layoutManager.ExpandToFitHeight = true;
            this.layoutManager.ExpandToFitWidth = true;
            // 
            // WidgetMatrixView
            // 
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Name = "WidgetMatrixView";
            this.Size = new System.Drawing.Size(507, 373);
            ((System.ComponentModel.ISupportInitialize)(this.layoutManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGridBagLayoutManager layoutManager;













    }
}
