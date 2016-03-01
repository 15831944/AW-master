using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Management;
using System.Windows.Forms;
//
using Microsoft.Win32;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// The OSInstance class is a form of ApplicationInstance specifically optimized to work
	/// with Operating System instances as these have less data but only ever a single instance
	/// per computer.
	/// </summary>
	public class OSInstance : ApplicationInstance
	{
		#region Data
		
        const uint LOCAL_MACHINE = 0x80000002;

		// Enum for attributes - this should match up with the names below
		public enum eAttributes { family, fullname, serial, cdkey };
		public static readonly ICollection<string> _listAttributes = new List<string>(new string[] 
		{ 
			 "Family"
			,"Version"
			,"Serial Number"
			,"CD Key"
		});

		#endregion
		
		#region Properties
		
		
		#endregion Properties
		
		#region Constructor

		public OSInstance()
			: base()
		{
			this._publisher = DataStrings.MICROSOFT;
			this._instanceid = 0;
			this._name = "<not detected>";
			this._version = "<not detected>";
			this._serial = null;
		}

		public OSInstance(DataRow dataRow)
			: base()
		{
			try
			{
				this.InstanceID = (int)dataRow["_INSTANCEID"];
				this.Name = (string)dataRow["_NAME"];
				this.Version = (string)dataRow["_VERSION"];
				this.InstalledOnComputerID = (int)dataRow["_ASSETID"];
				this.InstalledOnComputer = (string)dataRow["ASSETNAME"];
				this.InstalledOnComputerIcon = (string)dataRow["ASSETICON"];
				string productID = (string)dataRow["_PRODUCTID"];
				string cdKey = (string)dataRow["_CDKEY"];
				this.Serial = new ApplicationSerial("", "", productID, cdKey);
				this._publisher = DataStrings.MICROSOFT;
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating an OSINSTANCE Object, please check database schema.  The message was " + ex.Message);
			}
		}
		
		#endregion Constructor
		
		#region Methods
		
		/// <summary>
		/// This is the main detection function called to recover the OS
		/// </summary>
		/// <param name="remoteCredentials"></param>
		/// <returns></returns>
		public int Detect(RemoteCredentials remoteCredentials)
		{
			Layton.Common.Controls.Impersonator impersonator = null;
			String remoteHost = remoteCredentials.RemoteHost;

			try
			{
				// We may want to impersonate a different user so that we can audit remote computers - if so
				// start the impersonation here
				if (remoteCredentials.Username != null && remoteCredentials.Username != "")
					impersonator = new Impersonator(remoteCredentials.Username, remoteCredentials.Domain ,remoteCredentials.Password);

				// Pickup and format the remote host name for WMI
				if (remoteCredentials.IsLocalComputer())
					remoteHost = @"\\localhost";
				else
					remoteHost = @"\\" + remoteCredentials.RemoteHost;

				//Connection credentials to the remote computer - not needed if the logged in account has access
				ConnectionOptions oConn = null;

				// Construct the path to the WMI node we are interested in
				String path = remoteHost + @"\root\cimv2";
				ManagementScope scope;
				if (oConn == null)
					scope = new ManagementScope(path);
				else
					scope = new ManagementScope(path ,oConn);

				// ...and connect
				scope.Connect();

				// Query the Operating System
				ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
				ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
		        ManagementObjectCollection queryCollection = searcher.Get();
				foreach (ManagementObject managementObject in queryCollection)
				{
					_name = managementObject["Caption"].ToString();
					_name = RationalizeOS(_name);
					_serial = new ApplicationSerial();
					_serial.ProductId = managementObject["SerialNumber"].ToString();
					managementObject.Dispose();
					break;
				}

				// The above WMI call works as far as it goes however it cannot recover the CD Key
				// for this we will need to use registry access - we may as well use WMI for this as
				// well as if the above fails we stuck anyway
				DetectOSCdKey(remoteHost);
			}
			catch (Exception)
			{
                return -1;
			}

			finally
			{
				if (impersonator != null)
					impersonator.Dispose();
			}

			return 0;
		}


        /// <summary>
        /// Use WMI to read a remote registry key and pull back the OS CD KEY which we can 
        /// then translate
        /// </summary>
        /// <param name="remoteCredentials"></param>
        /// <returns></returns>
        protected int DetectOSCdKey(String remoteHost)
        {
            try
            {
                // Construct the path to the WMI node we are interested in
                String path = remoteHost + @"\root\default";
                ManagementScope myScope;
				myScope = new ManagementScope(path);
                ManagementPath mypath = new ManagementPath("StdRegProv");
                ManagementClass mc = new ManagementClass(myScope, mypath, null);
                ManagementBaseObject inParams = mc.GetMethodParameters("GetBinaryValue");

                // We will always be looking at HKEY_LOCAL_MACHINE
                inParams["hDefKey"] = LOCAL_MACHINE;
                inParams["sSubKeyName"] = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                inParams["sValueName"] = "DigitalProductID";

                // Get the binary value for the CD Key
                ManagementBaseObject outParams = mc.InvokeMethod("GetBinaryValue", inParams, null);

                // Did we get it?
                String cdKey = "";
                if (Convert.ToUInt32(outParams["ReturnValue"]) == 0)
                {
                    byte[] binaryCdKey = outParams["uValue"] as byte[];
                    cdKey = Utility.DecodeDigitalProductKey(binaryCdKey);

                    // ...finally store it
                    this._serial.CdKey = cdKey;
                }

            }
            catch (Exception)
            {
            }

			return 0;
        }


		/// <summary>
		/// Remove unwanted characters from the OS name
		/// </summary>
		/// <param name="osName"></param>
		protected String RationalizeOS(String osName)
		{
			String rationalizedName = osName;
			List<String> discardStrings = new List<String>();
			discardStrings.Add("(R)");
			discardStrings.Add("(r)");
			discardStrings.Add("(TM)");
			discardStrings.Add("(tm)");

			foreach (String discardString in discardStrings)
			{
				rationalizedName = rationalizedName.Replace(discardString, "");
			}

			return rationalizedName;
		}


		/// <summary>
		/// Return the current value of the specified attribute for this OS Instance
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public string GetAttributeValue(string attribute)
		{
			List<string> listAttributes = (List<string>)_listAttributes;

			if (attribute == listAttributes[(int)eAttributes.family])
				return _name;

			else if (attribute == listAttributes[(int)eAttributes.fullname])
				return _version;

			else if (attribute == listAttributes[(int)eAttributes.cdkey])
				return this._serial.CdKey;

			else if (attribute == listAttributes[(int)eAttributes.serial])
				return this._serial.ProductId;

			else
				return "";
		}

		public static string GetAttributeName(eAttributes attribute)
		{
			List<string> listAttributes = (List<string>)_listAttributes;
			return listAttributes[(int)attribute];
		}

		#endregion Methods

	}
}

 
