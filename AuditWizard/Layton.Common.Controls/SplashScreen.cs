#region Using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Layton.Common.Controls
{
	/// <summary>
	///		<para>
	///			Represents a splash screen.
	///		</para>
	/// </summary>
	/// <remarks>
	///		<para>
	///			There are two main properties used to customise the <see cref="SplashScreen"/>; <see cref="MinimumDuration"/> and <see cref="Control.BackgroundImage"/>. <see cref="MinimumDuration"/> controls 
	///			the duration a splash screen remains visible and <see cref="Control.BackgroundImage"/> sets the background image to display on the form.
	///		</para>
	///		<para>
	///			You can draw version and copyright information onto the <see cref="SplashScreen"/> by attaching to the <see cref="Control.Paint"/> event.
	///		</para>
	/// </remarks>
	public class SplashScreen : DropShadowForm
	{
		private TimeSpan _MinimumDuration;
		private bool _MinimumDurationCompleted;
		private bool _WaitingForTimer;
		private Timer _Timer;

		public SplashScreen()
		{
			InitializeComponent();
		}

		protected override void OnBackgroundImageChanged(EventArgs e)
		{
			base.OnBackgroundImageChanged(e);

			if (BackgroundImage != null)
			{
				ClientSize = BackgroundImage.Size;
			}
			else
			{
				ClientSize = Size.Empty;
			}
		}

		public TimeSpan MinimumDuration
		{
			get { return _MinimumDuration; }
			set 
			{
				if (value < TimeSpan.Zero)
					throw new ArgumentOutOfRangeException("value", value, "value must be greater than or equal to TimeSpan.Zero");

				_MinimumDuration = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (_Timer != null)
					{
						_Timer.Dispose();
						_Timer.Tick -= new EventHandler(OnTimerTick);
						_Timer = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && _MinimumDuration.TotalMilliseconds > 0)
			{
				_MinimumDurationCompleted = false;
				_WaitingForTimer = false;
				
				_Timer = new Timer();
				_Timer.Tick += new EventHandler(OnTimerTick);
				_Timer.Interval = (int)_MinimumDuration.TotalMilliseconds;
				_Timer.Start();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_MinimumDuration > TimeSpan.Zero && !_MinimumDurationCompleted)
			{	// Waiting for the timer to finish
				_WaitingForTimer = true;
				e.Cancel = true;
			}
			else
			{				
				base.OnClosing(e);
			}
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			_Timer.Stop();
			_MinimumDurationCompleted = true;
			
			if (_WaitingForTimer)
			{
				Close();
			}
		}

		#region InitializeComponent()
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // SplashScreen
            // 
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.ControlBox = false;
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

		}
		#endregion
	}
}
