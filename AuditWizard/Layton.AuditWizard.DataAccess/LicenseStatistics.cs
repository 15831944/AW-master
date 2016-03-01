using System;
using System.Data;

namespace Layton.AuditWizard.DataAccess
{
	/// <summary>
	/// This object encapsulates statistical information recovered relating to Licenses
	/// </summary>
	public class LicenseStatistics
	{
		// Fields
		private int _totalDeclared;
		private int _compliantApplications;
		private int _nonCompliantApplications;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// Methods
		public LicenseStatistics(DataRow dataRow)
		{
			try
			{
				this._totalDeclared = (int)dataRow["totaldeclared"];
				this._compliantApplications = (int)dataRow["compliantapplications"];
				this._nonCompliantApplications = (int)dataRow["noncompliantapplications"];
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an LICENSESTATISTICS Object, please check database schema.  The message was " + ex.Message);
			}
		}

		// Properties
	}



}

