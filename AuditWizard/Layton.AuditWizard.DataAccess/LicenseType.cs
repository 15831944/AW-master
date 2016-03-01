using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Layton.AuditWizard.DataAccess
{
	public class LicenseType
	{
		private int _licenseID;
		private string _name;
		private bool _percomputer;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public int LicenseTypeID
		{
			get { return _licenseID; }
			set { _licenseID = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool PerComputer
		{
			get { return _percomputer; }
			set { _percomputer = value; }
		}

		public LicenseType()
		{
			_licenseID = 0;
			_name = "";
			_percomputer = false;
		}

		public LicenseType(DataRow dataRow)
		{
			try
			{
				this._licenseID = (int)dataRow["_LICENSETYPEID"];
				this.Name = (string) dataRow["_NAME"];
				this._percomputer = (bool)dataRow["_COUNTED"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating a LICENSETYPE Object, please check database schema.  The message was " + ex.Message);
			}
		}
	}
}
