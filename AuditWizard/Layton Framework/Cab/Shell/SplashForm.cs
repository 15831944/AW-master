using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AuditWizardv8
{
    /// <summary>
    /// Base class for splash screens featuring an image placeholder, a marquee progress bar,
    /// and a background threaded status text queuing facility that supports minimum display
    /// timespans for text.
    /// </summary>
    public partial class SplashForm : Form
    {
        #region Private Data
        /// <summary>
        /// Synchronisation primitive for thread safe access to splash status text item queue.
        /// </summary>
        private object synchronisationObject = new Object();

        /// <summary>
        /// Event which is set when all queued status text has been processed and the form is closing.
        /// Allows the application main form to wait until the splash is ready before display itself.
        /// </summary>
        private ManualResetEvent splashClosedEvent = new ManualResetEvent(false);

        /// <summary>
        /// Semaphore that allows the background worker to wait for splash items to be enqueued.
        /// Initially zero, with the potential to grow up to Int32.MaxValue.
        /// </summary>
        private Semaphore splashItemsSemaphore = new Semaphore(0, Int32.MaxValue);

        /// <summary>
        /// Boolean flag that indicates that the background worker should now exit when all status text items
        /// have been displayed.
        /// </summary>
        private bool allowToClose;

        /// <summary>
        /// Strongly typed queue of <see cref="SplashStatusTextItem"/>'s.
        /// </summary>
        private Queue<SplashStatusTextItem> splashTextQueue = new Queue<SplashStatusTextItem>();
        #endregion

        #region Public Properties

        /// <summary>
        /// Public accessor which returns the <see cref="ProgressBar"/>, which is configured for marquee
        /// mode display by default.
        /// </summary>
        public ProgressBar ProgressBar
        {
            get { return this.progressBar; }
        }

        /// <summary>
        /// Wait handle which is set when all queued status text has been processed and the form is closing.
        /// Allows the application main form to wait until the splash is ready before display itself.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitHandle SplashClosedWaitHandle
        {
            get { return this.splashClosedEvent; }
        }
        #endregion

        #region Constructors/Destructor
        public SplashForm()
        {
            InitializeComponent();
            
            try
            {
                Bitmap bgImage = new Bitmap(Properties.Settings.Default.appSplashScreen);
                this.BackgroundImage = bgImage;
                this.Size = bgImage.Size;
            }
            catch
            {
                // TO DO:  use a generic Layton image
            }
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        #endregion

        #region Public Methods

        #region AllowToClose
        /// <summary>
        /// Indicates to the background worker should now exit when all status text items
        /// have been displayed. The <see cref="Enqueue"/> methods cannot be called after
        /// calling this method.
        /// </summary>
        public void AllowToClose()
        {
            this.allowToClose = true;

            // Perform a release on the semaphore in case the background worker is currently sleeping.
            this.splashItemsSemaphore.Release();
        }
        #endregion

        #region EnqueueStatusText
        /// <summary>
        /// Enqueues the supplied text for display in the status area of the splash screen.
        /// No minimum time is specified for the display of this text item when this overload is called.
        /// </summary>
        /// <param name="text">Represents a <see cref="System.String"/> containing the text.</param>
        public void EnqueueStatusText(string text)
        {
            // If allow to close is set then we must not enqueue - return immediately.
            if (this.allowToClose)
                return;

            // Make sure that the background worker is not concurrently accessing the queue whilst we're working with it.
            lock (this.synchronisationObject)
            {
                // Enqueue the new status text item.
                this.splashTextQueue.Enqueue(new SplashStatusTextItem(text));

                // And perform a single release on the semaphore. If the background worker was sleeping on the
                // semaphore it will now wake up.
                this.splashItemsSemaphore.Release();
            }
        }

        /// <summary>
        /// Enqueues the supplied text for display in the status area of the splash screen.
        /// No minimum time is specified for the display of this text item when this overload is called.
        /// </summary>
        /// <param name="text">Represents a <see cref="System.String"/> containing the text.</param>
        /// <param name="minimumTimeToDisplayInMilliseconds">The minimum time to display the status text in milliseconds.</param>
        public void EnqueueStatusText(string text, int minimumTimeToDisplayInMilliseconds)
        {
            // If allow to close is set then we must not enqueue - return immediately.
            if (this.allowToClose)
                return;

            // Make sure that the background worker is not concurrently accessing the queue whilst we're working with it.
            lock (this.synchronisationObject)
            {
                // Enqueue the new status text item.
                this.splashTextQueue.Enqueue(new SplashStatusTextItem(text, new TimeSpan(0, 0, 0, 0, minimumTimeToDisplayInMilliseconds)));

                // And perform a single release on the semaphore.
                // If the background worker was sleeping on the semaphore it will now wake up.
                this.splashItemsSemaphore.Release();
            }
        }
        #endregion

        #endregion

        #region Protected Methods
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Start the background worker to process status text items.
            if (!this.DesignMode)
                this.backgroundWorker.RunWorkerAsync();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Set the splash closed wait handle so that the shell form can now go visible, if it was waiting.
            this.splashClosedEvent.Set();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The <see cref="BackgroundWorker.DoWork"/> event handler here waits on a semaphore if there
        /// are not status text items yet to process. If the semaphore has a non-zero value and we haven't
        /// been asked to close yet and there are pending status text items to be displayed, then we safely
        /// dequeue one and use the <see cref="BackgroundWorker.ReportProgress"/> event to notify the UI
        /// thread for this instance of the status text to display. We then wait for the minimum display
        /// timespan specified for the status text item before iterating.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // Wait on the semaphore, which will return if/as soon as there are status text items in
                // the queue or we've been allowed to close, otherwise will block indefinitely (no timeout).
                splashItemsSemaphore.WaitOne();

                SplashStatusTextItem statusTextItem = null;

                // Safely attempt to retrieve an item from the queue.
                lock (this.synchronisationObject)
                {
                    // If we're now allowed to close and there's nothing left in the queue then it's time to break.
                    if (this.allowToClose && this.splashTextQueue.Count == 0)
                        break;

                    statusTextItem = this.splashTextQueue.Dequeue();
                }

                // Use the report progress facility to communicate to this control's UI thread with the new status
                // text item to display.
                this.backgroundWorker.ReportProgress(0, statusTextItem);

                // If there's a non-zero timespan to keep the text displayed then sleep for that duration before continuing.
                if (statusTextItem.MinimumTimeSpanDisplayed > TimeSpan.Zero)
                    Thread.Sleep(statusTextItem.MinimumTimeSpanDisplayed);
            }
        }

        /// <summary>
        /// <see cref="BackgroundWorker.ProgressChanged"/> event handler which updates the status text label
        /// with the supplied status text as provided from the background worker thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SplashStatusTextItem statusTextItem = (SplashStatusTextItem)e.UserState;
            this.labelStatus.Text = statusTextItem.Text;
        }

        /// <summary>
        /// <see cref="BackgroundWorker.RunWorkerCompleted"/> event handler which is invoked when the 
        /// background worker completes, it's time to close ourself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // When the background worker completes, it's time to close ourself.
            this.Close();
        }

        private void SplashForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
        #endregion

        #region Methods for making Form Translucent

        // Sets the current bitmap
        public void SelectBitmap(Bitmap bitmap)
        {
            // Does this bitmap contain an alpha channel?
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            // Get device contexts
            IntPtr screenDc = APIHelp.GetDC(IntPtr.Zero);
            IntPtr memDc = APIHelp.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;

            try
            {
                // Get handle to the new bitmap and select it into the current device context
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = APIHelp.SelectObject(memDc, hBitmap);

                // Set parameters for layered window update
                APIHelp.Size newSize = new APIHelp.Size(bitmap.Width, bitmap.Height);	// Size window to match bitmap
                APIHelp.Point sourceLocation = new APIHelp.Point(0, 0);
                APIHelp.Point newLocation = new APIHelp.Point(this.Left, this.Top);		// Same as this window
                APIHelp.BLENDFUNCTION blend = new APIHelp.BLENDFUNCTION();
                blend.BlendOp = APIHelp.AC_SRC_OVER;						// Only works with a 32bpp bitmap
                blend.BlendFlags = 0;											// Always 0
                blend.SourceConstantAlpha = 255;										// Set to 255 for per-pixel alpha values
                blend.AlphaFormat = APIHelp.AC_SRC_ALPHA;						// Only works when the bitmap contains an alpha channel

                // Update the window
                APIHelp.UpdateLayeredWindow(Handle, screenDc, ref newLocation, ref newSize,
                    memDc, ref sourceLocation, 0, ref blend, APIHelp.ULW_ALPHA);
            }
            finally
            {
                // Release device context
                APIHelp.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    APIHelp.SelectObject(memDc, hOldBitmap);
                    APIHelp.DeleteObject(hBitmap);										// Remove bitmap resources
                }
                APIHelp.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Add the layered extended style (WS_EX_LAYERED) to this window
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= APIHelp.WS_EX_LAYERED;
                return createParams;
            }
        }

        #endregion

        #region Win32 API Helpers

        #region Win32 Interop for dragging form

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        #endregion

        // Class to assist with Win32 API calls
        class APIHelp
        {
            // Required constants
            public const Int32 WS_EX_LAYERED = 0x80000;
            public const Int32 HTCAPTION = 0x02;
            public const Int32 WM_NCHITTEST = 0x84;
            public const Int32 ULW_ALPHA = 0x02;
            public const byte AC_SRC_OVER = 0x00;
            public const byte AC_SRC_ALPHA = 0x01;

            public enum Bool
            {
                False = 0,
                True = 1
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public Int32 x;
                public Int32 y;

                public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Size
            {
                public Int32 cx;
                public Int32 cy;

                public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct ARGB
            {
                public byte Blue;
                public byte Green;
                public byte Red;
                public byte Alpha;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct BLENDFUNCTION
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;
            }

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = true)]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool DeleteDC(IntPtr hdc);

            [DllImport("gdi32.dll", ExactSpelling = true)]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool DeleteObject(IntPtr hObject);
        }

        #endregion

    }

    #region SplashStatusTextItem
    /// <summary>
    /// This class encapsulates a single request to display an item of text in the splash status text area.
    /// </summary>
    public class SplashStatusTextItem
    {
        public readonly TimeSpan MinimumTimeSpanDisplayed;
        public readonly string Text;

        /// <summary>
        /// SplashStatusTextItem class constructor requiring the status text to be displayed.
        /// </summary>
        /// <param name="text">Represents a <see cref="System.String"/> containing the text.</param>
        public SplashStatusTextItem(string text)
            : this(text, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// SplashStatusTextItem class constructor requiring the status text to be displayed and minimum time for display.
        /// </summary>
        /// <param name="text">Represents a <see cref="System.String"/> containing the text.</param>
        /// <param name="minimumTimeSpanDisplayed">Represents the minimum time span to display the status text.</param>
        public SplashStatusTextItem(string text, TimeSpan minimumTimeSpanDisplayed)
        {
            this.Text = text;
            this.MinimumTimeSpanDisplayed = minimumTimeSpanDisplayed;
        }
    }
    #endregion
}