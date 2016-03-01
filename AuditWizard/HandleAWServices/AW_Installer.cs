using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Runtime.InteropServices;


namespace HandleAWServices
{
    [RunInstaller(true)]
    public partial class AW_Installer : Installer
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);

        private static int GENERIC_WRITE = 0x40000000;

        public AW_Installer()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            StopAndDeleteService("AuditWizardService");
            StopAndDeleteService("AuditWizard Agent");
        }        

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            StopAndDeleteService("AuditWizardService");
            StopAndDeleteService("AuditWizard Agent");
        }

        private static void StopAndDeleteService(string aServiceName)
        {
            try
            {
                ServiceController sc = new ServiceController(aServiceName);

                if (sc != null)
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                        sc.Stop();

                    IntPtr scmHandle = OpenSCManager(System.Environment.MachineName, null, GENERIC_WRITE);
                    if (scmHandle.ToInt32() != 0)
                    {
                        int DELETE = 0x10000;
                        IntPtr serviceHandle = OpenService(scmHandle, aServiceName, DELETE);
                        if (serviceHandle.ToInt32() != 0)
                        {
                            int status = DeleteService(serviceHandle);

                            // Close the service handle
                            CloseServiceHandle(serviceHandle);
                        }
                        CloseServiceHandle(scmHandle);
                    }
                }
            }
            catch { }
        }
    }
}
