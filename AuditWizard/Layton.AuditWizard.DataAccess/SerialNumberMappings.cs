using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
    #region SerialNumberMapping Class

    public class SerialNumberMapping
    {
        #region Data
			
        private string _name;
        private List<SerialNumberMappingsRegistryKey> _registryKeys = new List<SerialNumberMappingsRegistryKey>();

		#endregion Data
	
		#region Properties
	
		public string ApplicationName
		{
			get { return _name; }
			set { _name = value; }
		}

        public List<SerialNumberMappingsRegistryKey> RegistryKeysList
        {
            get { return _registryKeys; }
            set { _registryKeys = value; }
        }

		#endregion Properties
			
		#region Constructor

        public SerialNumberMapping()
		{
		}

		#endregion Constructor
    }

    #endregion

    #region SerialNumberMappingsRegistryKey Class

    public class SerialNumberMappingsRegistryKey
    {
        #region Data

        private string _registryKeyName;
        private string _registryKeyValue;

		#endregion Data
	
		#region Properties
	
		/// <summary>Name of the Alert Definition</summary>
		public string RegistryKeyName
		{
			get { return _registryKeyName; }
			set { _registryKeyName = value; }
		}

        /// <summary>Alert defintion description</summary>
        public string RegistryKeyValue
        {
            get { return _registryKeyValue; }
            set { _registryKeyValue = value; }
        }

		#endregion Properties
			
		#region Constructor

        public SerialNumberMappingsRegistryKey()
		{
		}

		#endregion Constructor
    }

    #endregion
}
