using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{

#region AlertNotification class	

	public class AlertNotification
	{
	
#region data
		private string _alertName;
		private string _type;
		private string _key;
		private string _category;
		private string _oldValue;
		private string _newValue;
#endregion data
	
#region Properties

		/// <summary>recover the name of the alert being notified</summary>
		public string AlertName
		{ 
			get { return _alertName; }
			set { _alertName = value; }
		}

		/// <summary>recover the name of the alert being notified</summary>
		public string Type
		{ 
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>this is the Category of the field being alerted</summary>
		public string Category
		{ 
			get { return _category; }
			set { _category = value; }
		}

		/// <summary>this is the key for the field being alerted</summary>
		public string Key
		{ 
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>This is the orginal value of the field being alerted</summary>
		public string OldValue
		{ 
			get { return _oldValue; }
			set { _oldValue = value; }
		}

		/// <summary>This is the new value of the field being alerted</summary>
		public string NewValue
		{ 
			get { return _newValue; }
			set { _newValue = value; }
		}
		
#endregion Properties

#region Constructor
		public AlertNotification ()
		{
			_alertName = "";
			_type = "";
			_category = "";
			_key = "";
			_oldValue = "";
			_newValue = "";
		}	
		
		public AlertNotification (string alertName ,string type ,string category ,string key ,string oldValue, string newValue)
		{
			_alertName = alertName;
			_type = type;
			_category = category;
			_key = key;
			_oldValue = oldValue;
			_newValue = newValue;
		}

#endregion Constructor

	}
	
#endregion AlertNotification class	
	
#region AlertNotificationFile class	
	
    /// <summary>
    /// This class encapsulates the AuditWizard Alert Notification File
	/// This data file acts as the interface between the AuditWizard scanner and the AuditWizard User
	/// interface and communicates alerts which have been triggered back to the UI
    /// </summary>
    public class AlertNotificationFile
    {

#region Data
		public static string _fileExtension = ".anf";
		private string _assetName;
		private List<AlertNotification> _listAlertNotifications = new List<AlertNotification>();
		
#endregion Data

#region Properties

		/// <summary>Name of the asset for which this notification has been generated</summary>
		public string AssetName
		{
			get { return _assetName; }
			set { _assetName = value; }
		}

		public List<AlertNotification> AlertNotifications
		{
			get { return _listAlertNotifications; }
		}

#endregion Properties

#region XMLStrings
        // String storage for sections and values in the XML alert notification File
		private const string S_ALERTNOTIFICATION_FILE = "AlertNotificationFile";
		//
		private const string V_ASSET_NAME		= "AssetName";
		//
		private const string S_ALERT			= "Alert";
		private const string V_ALERT_NAME		= "Name";
		private const string V_ALERT_TYPE		= "Type";
		private const string V_ALERT_CATEGORY	= "Category";
		private const string V_ALERT_KEY		= "Key";
		private const string V_ALERT_OLDVALUE	= "OldValue"; 
		private const string V_ALERT_NEWVALUE	= "NewValue";
		
#endregion

#region Constructor
#endregion constructor

#region READER Functions

		/// <summary>
		/// Read the Audit Data File into our internal buffer
		/// </summary>
		/// <returns></returns>
		public bool Read(string fileName)
		{
			XmlTextReader textReader;
			XmlSimpleElement xmlSimpleElement = new XmlSimpleElement("junk");
			XmlParser xmlParser;

			// First of all parse the file
			try 
			{
				textReader = new XmlTextReader(fileName);
				xmlParser = new XmlParser();
				xmlSimpleElement = xmlParser.Parse(textReader);
				textReader.Close();
            }

			catch (Exception)
			{
				return false;
			}

			// Now iterate through the data recovered 
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				ProcessElementRead(childElement);
			}

			return true;
		}


		/// <summary>
		/// Read an audit data file (from an existing XML stream in an XmlTextReader)
		/// </summary>
		/// <param name="xmlDocument"></param>
		/// <returns></returns>
		public bool Read(XmlTextReader textReader)
		{
			XmlSimpleElement xmlSimpleElement = new XmlSimpleElement("junk");
			XmlParser xmlParser;

			// First of all parse the file
			try 
			{
				xmlParser = new XmlParser();
				xmlSimpleElement = xmlParser.Parse(textReader);
				textReader.Close();
            }

			catch (Exception)
			{
				return false;
			}

			// If we can't find the 'Alert Notification' element then this is NOT a valid file
			if (xmlSimpleElement.TagName != S_ALERTNOTIFICATION_FILE)
				return false;

			// Now iterate through the data recovered 
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				ProcessElementRead(childElement);
			}

			return true;
		}


		/// <summary>
		/// Called as we parse a top level element from the Alert Notification file
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessElementRead(XmlSimpleElement xmlSimpleElement)
		{
			string elementName = xmlSimpleElement.TagName;

			// OK what sort of element is it?
			switch (elementName)
			{
				case V_ASSET_NAME:
					_assetName = xmlSimpleElement.Text;
					break;

				case S_ALERT:
					ProcessAlertRead(xmlSimpleElement);
					break;
					
				default:
					break;
			}
			return;
		}



		/// <summary>
		/// We have parsed the 'Alert' element so now parse the alert notification itself
		/// </summary>
		/// <param name="xmlElement"></param>
		protected void ProcessAlertRead(XmlSimpleElement xmlSimpleElement)
		{
			// Create an alert notification object
			AlertNotification alertNotification = new AlertNotification();
			
			// ...then loop through the fields in this section
			foreach (XmlSimpleElement childElement in xmlSimpleElement.ChildElements)
			{
				switch (childElement.TagName)
				{
					case V_ALERT_NAME:
						alertNotification.AlertName = childElement.Text;
						break;

					case V_ALERT_TYPE:
						alertNotification.Type = childElement.Text;
						break;

					case V_ALERT_CATEGORY:
						alertNotification.Category = childElement.Text;
						break;

					case V_ALERT_KEY:
						alertNotification.Key = childElement.Text;
						break;

					case V_ALERT_OLDVALUE:
						alertNotification.OldValue = childElement.Text;
						break;

					case V_ALERT_NEWVALUE:
						alertNotification.NewValue = childElement.Text;
						break;

					default:
						break;
				}
				
			}

			// Add the Alert Notification to our internal list
			_listAlertNotifications.Add(alertNotification);
		}

#endregion Reader Functions

	}
#endregion AlertNotificationFile class

#region AlertNotificationFileList

	public class AlertNotificationFileList
	{
		#region Data

		private List<AlertNotificationFile> _listAlertNotificationFiles = new List<AlertNotificationFile>();
		
		#endregion Data

		#region Properties
		
		public List<AlertNotificationFile> AlertNotificationFiles
		{
			get { return _listAlertNotificationFiles; }
		}
		
		#endregion Properties

		#region Methods
		
		/// <summary>
		/// Called to load all available alert notification files from the specified folder and 
		/// then delete the physical file
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public int Populate (string folder)
		{
			LogFile ourLog = LogFile.Instance;

			try
			{
				DirectoryInfo di = new DirectoryInfo(folder);
				string fileSpec = "*" + AlertNotificationFile._fileExtension;
				FileInfo[] rgFiles = di.GetFiles(fileSpec);

				foreach (FileInfo fi in rgFiles)
				{
					// Try and read the file as an audit data file, if we fail skip this file
					AlertNotificationFile thisFile = new AlertNotificationFile();
					string fullPath = Path.Combine(folder, fi.Name);
					if (!thisFile.Read(fullPath))
					{
						ourLog.Write("failed to read ANF file, skipping", true);
						continue;
					}
					
					_listAlertNotificationFiles.Add(thisFile);
					
					// Delete the notification file
					fi.Delete();
				}
			}

			// Any exceptions here probably indicate a locked file so we shall just ignore and 
			// have another go later
			catch (Exception ex)
			{
				ourLog.Write("Exception processing ANF Files, the error was " + ex.Message, true);
			}
			
			return _listAlertNotificationFiles.Count;
		}
		
		#endregion Methods
	}

#endregion AlertNotificationFileList
}

