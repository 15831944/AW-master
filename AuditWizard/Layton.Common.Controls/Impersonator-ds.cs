using System;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Management;

namespace Layton.ControlLibrary
{
    /// <summary>
    /// Summary description for Impersonator.
    /// </summary>
    public class Impersonator
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        // constants used by LogonUser() method
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        private WindowsImpersonationContext wic = null;
        private bool isImpersonating = false;
        private static Impersonator impersonator = new Impersonator();

        private string impersonateUsername;
        private string impersonatePassword;
        private string impersonateDomain;

        private Impersonator()
        {
        }

        public static Impersonator Instance
        {
            get { return impersonator; }
        }

        public void Impersonate(string username, string password)
        {
            isImpersonating = true;
            impersonateUsername = username;
            impersonatePassword = password;
        }

        public string ImpersonationUsername
        {
            get
            {
                if (isImpersonating)
                {
                    return impersonateUsername;
                }
                throw new Exception("Invalid attempt to retieve Active Login details.");
            }
        }

        public string ImpersonationPassword
        {
            get
            {
                if (isImpersonating)
                {
                    return impersonatePassword;
                }
                throw new Exception("Invalid attempt to retieve Active Login details.");
            }
        }

        public bool DoImpersonate
        {
            get { return isImpersonating; }
            set { isImpersonating = false; }
        }

        public string ActiveLoginUser
        {
            get { return WindowsIdentity.GetCurrent().Name; }
        }
    }
}
