using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Common
{
	public class InstalledPatch
	{
		#region Data
		private string _patchedProduct;
		private string _name;
		private string _servicePack;
		private string _description;
		private string _installedby;
		private string _installdate;
		#endregion Data

		#region Properties
		public string PatchedProduct
		{
			get { return _patchedProduct; }
			set { _patchedProduct = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string ServicePack
		{
			get { return _servicePack; }
			set { _servicePack = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string InstalledBy
		{
			get { return _installedby; }
			set { _installedby = value; }
		}

		public string InstallDate
		{
			get { return _installdate; }
			set { _installdate = value; }
		}
		#endregion Properties

		public InstalledPatch(string product, string name, string service_pack, string description, string installedby, string installdate)
		{
			_patchedProduct = product;
			_name = name;
			_servicePack = service_pack;
			_description = description;
			_installedby = installedby;
			_installdate = installdate;
		}
	}
}
