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
namespace Layton.AuditWizard.DataAccess
{
	/// <summary>
	/// The OSInstance class is a form of ApplicationInstance specifically optimized to work
	/// with Operating System instances as these have less data but only ever a single instance
	/// per computer.
	/// </summary>
	public class OSInstance : ApplicationInstance
	{
		#region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
        const uint LOCAL_MACHINE = 0x80000002;

		// Enum for attributes - this should match up with the names below
		public enum eAttributes { family, fullname, serial, cdkey};
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
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an OSINSTANCE Object, please check database schema.  The message was " + ex.Message);
			}
		}
		
		#endregion Constructor
		
		#region Methods 
	

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

 
