using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Layton.Cab.Interface
{
    #region Deisgner Support for Abstract UserControl

    internal class ConcreteCompleoView : CompleoView
    {

    }

    internal class ConcreteClassProvider : TypeDescriptionProvider
    {
        public ConcreteClassProvider() : base(TypeDescriptor.GetProvider(typeof(CompleoView))) {}

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(CompleoView))
            {
                return typeof(ConcreteCompleoView);
            }
            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(CompleoView))
            {
                objectType = typeof(ConcreteCompleoView);
            }
            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }

    #endregion

    [TypeDescriptionProvider(typeof(ConcreteClassProvider))]
    public abstract class CompleoView : System.Windows.Forms.UserControl
    {
        private bool isMaximized = false;
        private Infragistics.Win.Misc.UltraButton maximizeRestoreButton;
        private Form compleoForm;
        private Control parentControl;
        private Timer animateTimer;

        public CompleoView()
        {
            // initialize the view to show the maximize button
            InitializeComponent();
            
            // Initialize the containing form
            compleoForm = new Form();
            compleoForm.FormBorderStyle = FormBorderStyle.None;
            compleoForm.StartPosition = FormStartPosition.Manual;
            compleoForm.ShowInTaskbar = false;

            // Initialize the timer
            animateTimer = new Timer();
            animateTimer.Interval = 5;
            animateTimer.Enabled = true;
            animateTimer.Tick += new EventHandler(animateTimer_Tick);

            // make sure the button is brought to the top of the inheriting control
            maximizeRestoreButton.BringToFront();
        }

        public bool IsMaximized
        {
            get { return isMaximized; }
        }

        void CompleoView_SizeChanged(object sender, EventArgs e)
        {
            maximizeRestoreButton.Location = new Point(this.Width - maximizeRestoreButton.Width - 10, 10);
        }

        void maximizeRestoreButton_Click(object sender, EventArgs e)
        {
            if (isMaximized)
            {
                maximizeRestoreButton.Appearance.Image = Properties.Resources.expand_view_16;
                compleoForm.Hide();
                parentControl.Controls.Add(this);
                this.ParentForm.Show();
                isMaximized = false;
            }
            else
            {
                maximizeRestoreButton.Appearance.Image = Properties.Resources.collapse_view_16;
                parentControl = this.Parent;
                compleoForm.Size = new Size(this.ParentForm.Width - 48, this.ParentForm.Height - 48);
                compleoForm.Location = new Point(this.ParentForm.Location.X + (this.ParentForm.Width - compleoForm.Width) / 2, this.ParentForm.Location.Y + (this.ParentForm.Height - compleoForm.Height) / 2);
                compleoForm.Controls.Add(this);
                compleoForm.Opacity = .20;
                animateTimer.Start();
                compleoForm.Show();
                isMaximized = true;
            }
        }

        void animateTimer_Tick(object sender, EventArgs e)
        {
            if (compleoForm.Opacity < .95)
            {
                double opacityStep = .05;
                compleoForm.Opacity += opacityStep;
            }
            else
            {
                animateTimer.Stop();
            }
        }

        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.maximizeRestoreButton = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // maximizeRestoreButton
            // 
            this.maximizeRestoreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::Layton.Cab.Interface.Properties.Resources.expand_view_16;
            this.maximizeRestoreButton.Appearance = appearance1;
            this.maximizeRestoreButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.maximizeRestoreButton.Location = new System.Drawing.Point(10, 10);
            this.maximizeRestoreButton.Name = "maximizeRestoreButton";
            this.maximizeRestoreButton.Size = new System.Drawing.Size(28, 28);
            this.maximizeRestoreButton.TabIndex = 0;
            this.maximizeRestoreButton.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this.maximizeRestoreButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.maximizeRestoreButton.Click += new EventHandler(maximizeRestoreButton_Click);
            // 
            // ConcreteCompleoView
            // 
            this.Controls.Add(this.maximizeRestoreButton);
            this.Name = "CompleoView";
            this.SizeChanged += new EventHandler(CompleoView_SizeChanged);
            this.ResumeLayout(false);
        }
    }
}