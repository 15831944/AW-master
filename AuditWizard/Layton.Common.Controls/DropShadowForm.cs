#region Using directives

using System;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

#endregion

namespace Layton.Common.Controls
{
	public class DropShadowForm : Form
	{
		private const int CS_DROPSHADOW = 0x00020000;

		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // DropShadowForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "DropShadowForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

		}

		public DropShadowForm()
		{
		}

		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
			get
			{
				CreateParams parameters = base.CreateParams;

				if (DropShadowSupported)
				{
					parameters.ClassStyle = (parameters.ClassStyle | CS_DROPSHADOW);
				}

				return parameters;
			}
		}

		public static bool DropShadowSupported
		{
			get { return IsWindowsXPOrAbove; }
		}

		public static bool IsWindowsXPOrAbove
		{
			get
			{
				OperatingSystem system = Environment.OSVersion;
				bool runningNT = system.Platform == PlatformID.Win32NT;

				return runningNT && system.Version.CompareTo(new Version(5, 1, 0, 0)) >= 0;
			}
		}
	}
}
