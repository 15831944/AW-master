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

namespace Layton.AuditWizard.DataAccess
{
#region ApplicationSerial Class
	/// <summary>
	/// This class defines the implementation of a Serial Number for an application
	/// </summary>
	public class ApplicationSerial
	{
		#region Data Declarations
		private string _applicationName;
		private string _cdKey;
		private string _identifier;
		private bool _matched;
		private string _productId;
		private string _registryKey;
		private string _registryValue;
#endregion Data Declarations

		#region Constructors

		public ApplicationSerial()
		{
			this._applicationName = "";
			this._identifier = "";
			this._cdKey = "";
			this._productId = "";
			this._registryKey = "";
			this._registryValue = "";
			this._matched = false;
		}

		public ApplicationSerial(string name, string identifier, string productID, string cdKey)
		{
			this._applicationName = name;
			this._identifier = identifier;
			this._productId = productID;
			this._cdKey = cdKey;
			this._registryKey = "";
			this._registryValue = "";
			this._matched = false;
		}
		
		// Copy Constructor
		public ApplicationSerial(ApplicationSerial theSerial)
		{
			this.ApplicationName = theSerial.ApplicationName;
			this.Identifier = theSerial.Identifier;
			this.ProductId = theSerial.ProductId;
			this.CdKey = theSerial.CdKey;
			this.RegistryKey = theSerial.RegistryKey;
			this.RegistryValue = theSerial.RegistryValue;
			this.Matched = theSerial.Matched;
		}

#endregion Constructors

		/// <summary>
		/// Equality test - check to see if this serial number object matches another
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType().Equals(this.GetType()))
			{
				ApplicationSerial other = obj as ApplicationSerial;

				if ((object)other != null)
				{
					return other.ApplicationName == ApplicationName
						&& other.Identifier == Identifier
						&& other.ProductId == ProductId
						&& other.CdKey == CdKey
						&& other.RegistryKey == RegistryKey
						&& other.RegistryValue == RegistryValue;
				}
			}
			return base.Equals(obj);
		}

		#region Properties
		public string ApplicationName
		{
			get { return this._applicationName; }
			set { this._applicationName = value; }
		}

		public string CdKey
		{
			get { return this._cdKey; }
			set { this._cdKey = value; }
		}

		public string Identifier
		{
			get { return this._identifier; }
			set { this._identifier = value; }
		}

		public bool Matched
		{
			get { return this._matched; }
			set { this._matched = value; }
		}

		public string ProductId
		{
			get { return this._productId; }
			set { this._productId = value; }
		}

		public string RegistryKey
		{
			get { return this._registryKey; }
			set { _registryKey = (value.StartsWith("HKEY_LOCAL_MACHINE")) ? value.Remove(0, 0x13) : value; }
		}

		public string RegistryValue
		{
			get { return this._registryValue; }
			set { this._registryValue = value; }
		}
#endregion Properties
	}
#endregion ApplicationSerial Class


#region ApplicationSerials (List) Class
	/// <summary>
	/// This class implements a list of applicationserial objects
	/// </summary>
	public class ApplicationSerials : List<ApplicationSerial>
	{
		#region Local Data
		// Fields
		private int _currentDepth;
		private Dictionary<string, string> _guidMappings = new Dictionary<string, string>();
		private int _keysScanned;
		private int _maxDepth = 8;
		private int _valuesScanned;
		private static string APPLICATIONS_SECTION = "ApplicationSerials";
		private static string DIGITALPRODUCTID = "DigitalProductID";
		private static string DISPLAYNAME = "DisplayName";
		private static string PIDKEY = "PIDKEY";
		private static string[] SERIALNUMBER_NAMES = new string[] { "PRODUCTID", "REGISTRATION", "SERIAL", "REGISTEREDPID", "LICENSENUMBER", "PID", "LMKEY", "REGISTER NO.", "REGCODE", "KEYCODE", "CD-KEY" };
		private static string UNINSTALLKEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
#endregion Local Data

		#region Methods
		/// <summary>
		/// Determine if we have a Serial Number entry for the specified application and return it
		/// </summary>
		/// <param name="application">Name of the application to search for</param>
		/// <returns>The located object found, null otherwise</returns>
		public ApplicationSerial ContainsApplication(string application)
		{
			foreach (ApplicationSerial thisApplication in this)
			{
				if ((thisApplication.ApplicationName != "") && application.StartsWith(thisApplication.ApplicationName))
				{
					return thisApplication;
				}
			}
			return null;
		}


		/// <summary>
		/// Do we have an entry for the specified registry key if so return it
		/// </summary>
		/// <param name="registryKey"></param>
		/// <returns></returns>
		public ApplicationSerial ContainsRegistryKey(string registryKey)
		{
			string ourKey = registryKey.Replace(@"HKEY_LOCAL_MACHINE\", "");
			foreach (ApplicationSerial thisApplication in this)
			{
				if (thisApplication.RegistryKey == ourKey)
				{
					return thisApplication;
				}
			}
			return null;
		}


		/// <summary>
		/// Decode a Digital Product ID to its native format
		/// </summary>
		/// <param name="digitalProductId"></param>
		/// <returns></returns>
		public static string DecodeProductKey(byte[] digitalProductId)
		{
			char[] digits = new char[] { 
				'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 'T', 'V', 'W', 'X', 
				'Y', '2', '3', '4', '6', '7', '8', '9'
			 };
			char[] decodedChars = new char[0x1d];
			ArrayList hexPid = new ArrayList();
			for (int i = 0x34; i <= 0x43; i++)
			{
				hexPid.Add(digitalProductId[i]);
			}
			for (int i = 0x1c; i >= 0; i--)
			{
				if (((i + 1) % 6) == 0)
				{
					decodedChars[i] = '-';
				}
				else
				{
					int digitMapIndex = 0;
					for (int j = 14; j >= 0; j--)
					{
						int byteValue = (digitMapIndex << 8) | ((byte) hexPid[j]);
						hexPid[j] = (byte) (byteValue / 0x18);
						digitMapIndex = byteValue % 0x18;
						decodedChars[i] = digits[digitMapIndex];
					}
				}
			}
			return new string(decodedChars);
		}

		/// <summary>
		/// Detect serial numbers using WMI 
		/// </summary>
		/// <param name="remoteCredentials"></param>
		/// <returns></returns>
		protected int DetectWMI(String remoteHost)
		{
			return -1;
		}
#endregion Methods

#region Registry Local Functions
		/// <summary>
		/// Read the UNINSTALL key and recover any product GUID values along with the product names
		/// </summary>
		/// <param name="rootKey"></param>
		protected void FillGuidDictionary(RegistryKey rootKey)
		{
			RegistryKey uninstallKey = rootKey.OpenSubKey(UNINSTALLKEY);
			if (uninstallKey != null)
			{
				foreach (string subKeyName in uninstallKey.GetSubKeyNames())
				{
					try
					{
						string productGUID = "";
						if (subKeyName.StartsWith("{") && subKeyName.EndsWith("}"))
						{
							productGUID = subKeyName.Substring(1, subKeyName.Length - 2);
							RegistryKey applicationKey = uninstallKey.OpenSubKey(subKeyName);
							string displayName = ApplicationInstanceList.GetApplicationName(applicationKey, DISPLAYNAME);
							applicationKey.Close();
							if (displayName != "")
								this._guidMappings.Add(productGUID, displayName);
						}
					}
					catch (Exception)
					{
					}
				}
				uninstallKey.Close();
			}
		}



		/// <summary>
		/// Recursively search down the Software registry key looking for any possible product serial numbers
		/// and storing them for later inspection
		/// </summary>
		/// <param name="softwareKey"></param>
		protected void FindAllProductSerials(RegistryKey softwareKey)
		{
			this._keysScanned++;

			// Skip anything under Software\Classes as this does not contain serial numbers and is huge
			if (softwareKey.Name == @"HKEY_LOCAL_MACHINE\Software\Classes")
			{
				this._currentDepth--;
			}
			else
			{
				foreach (string valueName in softwareKey.GetValueNames())
				{
					this._valuesScanned++;
					if (this.IsSerialNumber(valueName))
					{
						string applicationName = "";
						softwareKey.GetValue(valueName, "");
						string digitalProductID = this.GetDigitalProductId(softwareKey);
						string productGUID = softwareKey.Name;
						int lastSegmentStart = productGUID.LastIndexOf(@"\");
						productGUID = productGUID.Substring(lastSegmentStart + 1);
						if (productGUID.StartsWith("{") && productGUID.EndsWith("}"))
						{
							productGUID = productGUID.Substring(1, productGUID.Length - 2);
							if (this._guidMappings.ContainsKey(productGUID))
							{
								applicationName = this._guidMappings[productGUID];
							}
						}
						else
						{
							productGUID = "";
						}
						ApplicationSerial newSerial = new ApplicationSerial();
						newSerial.ApplicationName = applicationName;
						newSerial.Identifier = productGUID;
						newSerial.ProductId = softwareKey.GetValue(valueName, "") as string;
						newSerial.CdKey = digitalProductID;
						newSerial.RegistryKey = softwareKey.Name;
						newSerial.RegistryValue = valueName;
						base.Add(newSerial);
					}
				}
				this._currentDepth++;
				if (this._currentDepth < this._maxDepth)
				{
					foreach (string subKeyName in softwareKey.GetSubKeyNames())
					{
						try
						{
							RegistryKey subKey = softwareKey.OpenSubKey(subKeyName);
							this.FindAllProductSerials(subKey);
							subKey.Close();
						}
						catch (Exception)
						{
						}
					}
				}
				this._currentDepth--;
			}
		}


		/// <summary>
		/// Recover and decode any Digital Product ID in the current registry key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected string GetDigitalProductId(RegistryKey key)
		{
			string digitalProductID = key.GetValue(PIDKEY, "") as string;
			if (digitalProductID == "")
			{
				object byteObject = key.GetValue(DIGITALPRODUCTID);
				if (byteObject != null)
				{
					byte[] byteKey = (byte[]) byteObject;
					if (byteKey[0] != 0)
					{
						digitalProductID = DecodeProductKey(byteKey);
					}
				}
				return digitalProductID;
			}

			if (digitalProductID.Length == 0x19)
			{
				digitalProductID = digitalProductID.Substring(0, 6) + "-" + digitalProductID.Substring(6, 6) + "-" + digitalProductID.Substring(12, 6) + "-" + digitalProductID.Substring(0x12);
			}
			return digitalProductID;
		}

		#endregion Registry Local Functions
		

		/// <summary>
		/// Determine if the specified value could possibly equate to a serial number 
		/// </summary>
		/// <param name="valueName"></param>
		/// <returns></returns>
		protected bool IsSerialNumber(string valueName)
		{
			foreach (string thisName in SERIALNUMBER_NAMES)
			{
				if (valueName.ToUpper().StartsWith(thisName))
				{
					return true;
				}
			}
			return false;
		}


#region Application Definitions File Functions

		/// <summary>
		/// Read the application serial numbers section of the configuration file
		/// </summary>
		/// <param name="rootKey"></param>
		protected void ReadSerials(RegistryKey rootKey)
		{
			try
			{
				ApplicationDefinitionsFile definitionsFile = new ApplicationDefinitionsFile();
				definitionsFile.SetSection(APPLICATIONS_SECTION);
				List<string> listApplications = new List<string>();
				definitionsFile.EnumerateKeys(APPLICATIONS_SECTION, listApplications);
				foreach (string thisKey in listApplications)
				{
					this.ProcessSerialsLine(rootKey, thisKey + "=" + definitionsFile.GetString(thisKey, ""));
				}
			}
			catch (Exception)
			{
			}
		}


		/// <summary>
		/// Process a line from the applications serial numbers section of the configuration file
		/// 
		/// The inout line will be formatted as follows:
		/// 
		///		[application name]=[registrykey],[registryvalue] {;[registrykey],[registryvalue]...}
		/// 
		/// This routine will parse this line splitting the application name off from the trailing registry
		/// key/value pairs.  
		/// 
		/// Each key/value pair is then split into the key and value.
		/// 
		/// Check to see if this registry key is already specified within our internal list
		/// 
		/// 
		/// </summary>
		/// <param name="rootKey"></param>
		/// <param name="inputLine"></param>
		protected void ProcessSerialsLine(RegistryKey rootKey, string inputLine)
		{
			inputLine = inputLine.Trim();
			if (inputLine.Length != 0)
			{
				// Split line into Application and Registry key/value pair(s) - delimiter here is '='
				string[] splitLine = inputLine.Split(new char[] { '=' });

				// We must have exactly two resultant strings, Application and Registry Pairs
				if (splitLine.Length == 2)
				{
					string applicationName = splitLine[0];

					// Was the application name specified?  If not then ignore this line as invalid
					if (applicationName != "")
					{
						// OK Split off the registry key/value pairs which are delimited by ';'
						string[] registryKeys = splitLine[1].Split(new char[] { ';' });
						ApplicationSerial foundSerial = null;

						// ...and iterate through the returned key/value pairs
						foreach (string registryKey in registryKeys)
						{
							// Split the key and value in this pair from each other, delimiter is ','
							string[] keyParts = registryKey.Split(new char[] { ',' });
							if (keyParts.Length == 2)
							{
								// Look in our list for an entry which specifies this registry key
								foundSerial = this.ContainsRegistryKey(keyParts[0]);
								if (foundSerial != null)
									break;
							}
						}

						// Did we find ANY of the registry keys specified for the application in our
						// existing list of registry key mappings?
						if (foundSerial != null)
						{
							// Yes - set the application name in the work ApplicationSerial object then
							foundSerial.ApplicationName = applicationName;
						}
						else
						{
							// No we have not found any existing instance of any of the registry keys
							// which were specified for the application - we therefore add an ApplicationSerial
							// object into our list for each key specified
							foreach (string registryKey in registryKeys)
							{
								// Split the pair into it's components
								string[] keyParts = registryKey.Split(new char[] { ',' });

								// If it's valid then we can process it further
								if (keyParts.Length == 2)
								{
									// Create the object
									ApplicationSerial newSerial = new ApplicationSerial();

									// Show matched and set the application name from that saved
									newSerial.Matched = true;
									newSerial.ApplicationName = applicationName;

									// Pick up the registry key name and value name
									string keyName = keyParts[0];
									string valueName = keyParts[1];
									string productID = "";
									string digitalProductID = "";
									try
									{
										// ...and try and read the specified key
										RegistryKey subKey = rootKey.OpenSubKey(keyName);
										if (subKey != null)
										{
											// Registry key was opened so read the value as the productID
											productID = subKey.GetValue(valueName, "").ToString();

											// ...then try and get any associated digital product iD
											digitalProductID = this.GetDigitalProductId(subKey);
											newSerial.ProductId = productID;
											newSerial.CdKey = digitalProductID;

											// Add this object to our list
											base.Add(newSerial);
											subKey.Close();
										}
									}
									catch (Exception)
									{
									}
								}
							}
						}
					}
				}
			}
		}

#endregion Application Definitions File Functions

#region Properties
		// Properties
		public int KeysScanned
		{
			get
			{
				return this._keysScanned;
			}
		}

		public int MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				this._maxDepth = value;
			}
		}

		public int ValuesScanned
		{
			get
			{
				return this._valuesScanned;
			}
		}
#endregion Properties
	}
#endregion ApplicationSerials (List) Class

}

