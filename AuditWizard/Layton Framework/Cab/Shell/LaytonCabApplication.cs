using System;
using System.Windows.Forms;
using Infragistics.Practices.CompositeUI.WinForms;
using System.Threading;
using Microsoft.Practices.CompositeUI;

namespace AuditWizardv8
{
    public class LaytonCabApplication : IGFormShellApplication<LaytonCabShellWorkItem, LaytonFormShell>
    {
        private Thread splashThread;
        private static SplashForm splashForm;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;

        [STAThread]
        static void Main()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                LaytonCabApplication app = new LaytonCabApplication();
                app.Run();
            }
            catch (Exception e)
            {
                if (e.Message.ToLower() != "object is currently in use elsewhere.")
                {
                    logger.Error("Exception occurred in Main().", e);

                    MessageBox.Show("An error has occurred in AuditWizard and the application needs to close." + Environment.NewLine + Environment.NewLine +
                        "Please check the log file for further information and contact technical support if the problem persists."
                        ,"AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    Application.Exit();
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            logger.Error("Exception occurred in Main().", (Exception)args.ExceptionObject);

            MessageBox.Show("An error has occurred in AuditWizard and the application needs to close." + Environment.NewLine + Environment.NewLine +
                "Please check the log file for further information and contact technical support if the problem persists."
                , "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.Exit();
        }

        /// <summary>
        /// Overrides the default implementation to never use the ink provider capability
        /// </summary>
        protected override Infragistics.Win.ShowInkButton ProvideInkSupport
        {
            get
            {
                return Infragistics.Win.ShowInkButton.Never;
            }
        }

        /// <summary>
        /// Invoked before the Shell is created. Perform any appropriate initialisation here.
        /// </summary>
        protected override void BeforeShellCreated()
        {
            // Start a thread to service the splash UI exclusively. Important that the splash instance and
            // its SplashImage properties are set before starting its UI thread since an unfortunate timeslice
            // may cause a reference to these before they are instantiated. It appears safe to set this up from the
            // main thread even though the form is serviced (i.e. ShowDialog called) from the splash's UI thread.
            splashForm = new SplashForm();
            splashThread = new Thread(new ThreadStart(this.SplashThreadMain));
            splashThread.Priority = ThreadPriority.AboveNormal;
            splashThread.Start();
            
            // Update the SplashScreen with each of the WorkItems that are loaded
            base.RootWorkItem.WorkItems.Added += delegate(object sender, Microsoft.Practices.CompositeUI.Utility.DataEventArgs<WorkItem> e)
            {
                 splashForm.EnqueueStatusText("Loading " + e.Data.GetType().Assembly.GetName().Name + "...");
            };

            base.BeforeShellCreated();
        }

        private void SplashThreadMain()
        {
            splashForm.ShowDialog();
        }

        protected override void Start()
        {
            splashForm.AllowToClose();
            splashForm.SplashClosedWaitHandle.WaitOne();
            base.Shell.BringToFront();
            base.Start();
        }

        protected override void AfterShellCreated()
        {
            base.AfterShellCreated();

            // See: http://devcenter.infragistics.com/Support/KnowledgeBaseArticle.Aspx?ArticleID=10042

            // Register toobarManager's Ribbon
            RootWorkItem.UIExtensionSites.RegisterSite("ribbon", this.Shell.ToolbarsWorkspace.Ribbon);

            // Register the toobarManager's RibbonTabs collection
            RootWorkItem.UIExtensionSites.RegisterSite("ribbonTabs", Shell.ToolbarsWorkspace.Ribbon.Tabs);

            this.RootWorkItem.Show(Shell);
            Shell.Shown += new EventHandler(Shell_Shown);
        }

        void Shell_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.initWorkItem != null)
            {
                WorkItem workItem = RootWorkItem.WorkItems[Properties.Settings.Default.initWorkItem];
                if (workItem != null)
                {
                    ((Layton.Cab.Interface.LaytonWorkItem)workItem).Controller.ActivateWorkItem();
                }
            }
        }
    }
}
