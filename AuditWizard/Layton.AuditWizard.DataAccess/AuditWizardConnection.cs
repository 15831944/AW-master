using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	public class AuditWizardConnection
	{
		// Fields
		private AuditWizardConfiguration _configuration = new AuditWizardConfiguration();
		private string connStr;
		private static readonly AuditWizardConnection dbConn = new AuditWizardConnection();
		private string serverName;
		private string serverPassword = "AuditWizard1";
		private string serverUserName = "AuditWizardUser1";
		private bool useTrustedConnection = true;

		// Methods
		private AuditWizardConnection()
		{
			this.ServerName = this._configuration.DatabaseServer;
			this.UseTrustedConnection = this._configuration.TrustedConnection;
		}

		private void UpdateConnectionString()
		{
			if (this.useTrustedConnection)
			{
				this.connStr = "Server=" + this.serverName + ";Database=AuditWizard;Trusted_Connection=Yes;";
			}
			else
			{
				this.connStr = "Server=" + this.serverName + ";Database=AuditWizard;User Id=" + this.serverUserName + ";password=" + this.serverPassword;
			}
		}

		// Properties
		public string ConnectionString
		{
			get { return this.connStr; }
			set { this.connStr = value; }
		}

		public static AuditWizardConnection Instance
		{
			get { return dbConn; }
		}

		public string ServerName
		{
			get { return this.serverName; }
			set { this.serverName = value; this.UpdateConnectionString(); }
		}

		public string ServerPassword
		{
			get { return this.serverPassword; }
			set { this.serverPassword = value; this.UpdateConnectionString(); }
		}

		public string ServerUserName
		{
			get { return this.serverUserName; }
			set { this.serverUserName = value; this.UpdateConnectionString(); }
		}

		public bool UseTrustedConnection
		{
			get { return this.useTrustedConnection; }
			set { this.useTrustedConnection = value; this.UpdateConnectionString(); }
		}
	}
}

