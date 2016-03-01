using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace Layton.AuditWizard.DataAccess
{
	public class IniItem
	{
		#region Data
		private string _key;
		private string _value;
		#endregion Data

		#region Accessors
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}
		#endregion Accessors

		#region Constructor

		public IniItem(String key, String value)
		{
			_key = key;
			_value = value;
		}

		#endregion Constructor

		public int Write(StreamWriter writer)
		{
			String buffer = String.Format("{0}={1}", _key, _value);
			try
			{
				writer.WriteLine(buffer);
			}
			catch (Exception)
			{
				return -1;
			}
			return 0;
		}
	}


	public class IniSection : List<IniItem>
	{
		#region Data
		private string _name;
		#endregion Data

#region Accessors
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

#endregion Accessors

#region Constructor

		public IniSection(String name)
		{
			_name = name;
		}

		#endregion Constructor

		public String GetKey(String key, String defaultValue)
		{
			int keyIndex = FindKey(key);
			if (keyIndex == -1)
			{
				return defaultValue;
			}
			else
			{
				return ((IniItem)this[keyIndex]).Value;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetKey(String key, String value)
		{
			// strip out illegal chars
			String validKey = key.Replace("[", "");
			validKey = validKey.Replace("]", "");
			//
			String validValue = value.Replace("[", "");
			validValue = validValue.Replace("]", "");

			// Find the IniItem in our list
			int keyIndex = FindKey(validKey);
			if (keyIndex == -1)
			{
				this.Add(new IniItem(validKey, validValue));
			}
			else
			{
				IniItem thisItem = this[keyIndex];
				thisItem.Value = value;
			}
		}


		/// <summary>
		/// Return the index of any item with the specified key
		/// </summary>
		/// <param name="key">key to find</param>
		/// <returns>Index of key or -1 if not found</returns>
		public int FindKey(String key)
		{
			for (int index = 0; index < this.Count; index++)
			{
				if (this[index].Key == key)
					return index;
			}
			return -1;
		}


		/// <summary>
		/// Write this section to disk including all items within the section
		/// </summary>
		/// <param name="writer">StreamWriter object</param>
		/// <returns>0 if success, -1 otherwise</returns>
		public int Write(StreamWriter writer)
		{
			int status = 0;
			String buffer = "[" + Name + "]";

			try
			{
				writer.WriteLine(buffer);
			}
			catch (Exception)
			{
				return -1;
			}

			// write the keys
			foreach (IniItem thisIniItem in this)
			{
				status = thisIniItem.Write(writer);
			}

			return status;
		}


		/// <summary>
		/// Return a list of the key (names) currently defined
		/// </summary>
		/// <param name="listKeys">Return list of keys defined</param>
		/// <returns></returns>
		public int EnumerateKeys(List<String> listKeys)
		{
			// Ensure that the list is empty before we proceed
			listKeys.Clear();

			// Now iterate through our keys
			foreach (IniItem thisIniItem in this)
			{
				listKeys.Add(thisIniItem.Key);
			}
			return listKeys.Count;
		}


		/// <summary>
		/// Remove (any) key with the specified key name from our list
		/// </summary>
		/// <param name="removeKey"></param>
		public void RemoveKey(String removeKey)
		{
			int keyIndex = FindKey(removeKey);
			if (keyIndex != -1)
				RemoveAt(keyIndex);
		}
	}


	public class IniFile : List<IniSection>
	{
#region Data Definitions

		private String _fileName;				// The INI file name
		private int _selectedSection;		// Currently selected section
		private int _changeCount;			// number of changes since last read or write
		private int _flags;					// see constructor

		// Flags for INI file operations - may be combined
		public static int IF_NOREAD	= 0x01;		// inhibit from reading any existing file
		public static int IF_NOWRITE	= 0x02;		// inhibit from writing changes

#endregion Data Definitions

#region Data Accessors
		public String FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		public int ChangeCount
		{
			get { return _changeCount; }
		}

#endregion Data Accessors

#region Constructor

		public IniFile() : this(null ,0)
		{
		}

		public IniFile (String name ,int flags)
		{
			_flags = flags;
			
			// If we have been passed the base file name then use it otherwise default from the 
			// name of the currently executing product
			if (name != null)
			{
				_fileName = Path.Combine(Application.StartupPath, name);
			}
			else
			{
				String baseName = Path.Combine(Application.StartupPath, Application.ProductName);
				_fileName = Path.Combine(baseName, ".ini");
			}
		
			// error reading is acceptable - file may not exist yet...
			if ((_flags & IF_NOREAD) == 0)
				Read(null);
		}

#endregion Constructor

#region GET VALUE functions

		/// <summary>
		/// Return a string value for an entry within the specified section
		/// </summary>
		/// <param name="section">Name of the parent section</param>
		/// <param name="key">Key to return value of</param>
		/// <param name="defaultValue">Default value to apply if key not found</param>
		/// <returns></returns>
		public String GetString (String section ,String key ,String defaultValue)
		{
			SetSection(section);
			return GetString(key ,defaultValue).Trim();
		}


		/// <summary>
		/// Return the value of the specified key (in the current section)
		/// </summary>
		/// <param name="key">Key to return value of</param>
		/// <param name="defaultValue">Default value to apply if key not found</param>
		/// <returns></returns>
		public String GetString (String key ,String defaultValue)
		{
			String result = defaultValue;
			if (_selectedSection != -1)
				result = this[_selectedSection].GetKey(key ,defaultValue).Trim();
			return result;
		}


		/// <summary>
		/// Return an Integer value for an entry within the specified section
		/// </summary>
		/// <param name="section">Name of the parent section</param>
		/// <param name="key">Key to return value of</param>
		/// <param name="defaultValue">Default value to apply if key not found</param>
		/// <returns></returns>
		public int GetInteger (String section, String key, int defaultValue)
		{
			SetSection(section);
			return GetInteger(key, defaultValue);
		}


		/// <summary>
		/// Return the value of the specified key (in the current section) as an INTEGER
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public int GetInteger (String key, int defaultValue)
		{
			String defaultString = defaultValue.ToString();
			String result = GetString(key ,defaultString);
			return Convert.ToInt32(result);
		}


		/// <summary>
		/// Return a boolean value for an entry within the specified section
		/// </summary>
		/// <param name="section">Name of the parent section</param>
		/// <param name="key">Key to return value of</param>
		/// <param name="defaultValue">Default value to apply if key not found</param>
		/// <returns></returns>
		public bool GetBoolean (String section, String key, bool defaultValue)
		{
			SetSection(section);
			return GetBoolean(key, defaultValue);
		}


		/// <summary>
		/// Return the value of the specified key (in the current section) as an BOOL
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public bool GetBoolean (String key ,bool defaultValue)
		{
			String defaultBoolean = (defaultValue) ? "Y" : "N";
			String result = GetString(key ,defaultBoolean);
			bool returnValue = (result[0] == 'Y' || result[0] == 'y' || result[0] == 't' || result[0] == '1');
			return returnValue;
		}


		/// <summary>
		/// Return the value of the specified key (in the current section) as a DateTime
		/// Note that the date/time is stored in "yyyy-mm-dd hh:mm:ss" format
		/// </summary>
		/// <param name="?"></param>
		/// <param name="defaultValue"></param>
		/// <param name="timeOnly"></param>
		/// <returns></returns>
		public DateTime GetTime(String section, String key, DateTime defaultValue, bool timeOnly)
		{
			SetSection(section);
			return GetTime(key, defaultValue, timeOnly);
		}


		/// <summary>
		/// Return the value of the specified key (in the current section) as a DateTime
		/// Note that the date/time is stored in "yyyy-mm-dd hh:mm:ss" format
		/// </summary>
		/// <param name="?"></param>
		/// <param name="defaultValue"></param>
		/// <param name="timeOnly"></param>
		/// <returns></returns>
		public DateTime GetTime (String key ,DateTime defaultValue ,bool timeOnly)
		{
			DateTime result = defaultValue;
			DateTime now = DateTime.Now;

			String buffer = GetString(key ,"");
			if (buffer.Length != 0)
			{
				int year = 0, month=0, day=0 ,hour=0 ,minute=0 ,second=0;
				int nextIndex = 0;

				if (!timeOnly)
				{
					year = Convert.ToInt32(buffer.Substring(nextIndex ,4));
					nextIndex += 5;
					month = Convert.ToInt32(buffer.Substring(nextIndex ,2));
					nextIndex += 3;
					day = Convert.ToInt32(buffer.Substring(nextIndex ,2));
					nextIndex += 3;
				}
				else
				{
					year = now.Year;
					month = now.Month;
					day = now.Day;
				}

				hour = Convert.ToInt32(buffer.Substring(nextIndex ,2));
				nextIndex += 3;
				minute = Convert.ToInt32(buffer.Substring(nextIndex ,2));
				nextIndex += 3;
				second = Convert.ToInt32(buffer.Substring(nextIndex ,2));

				// Construct the DateTime to return from the above components
				result = new DateTime(year ,month ,day ,hour ,minute ,second);
			}

			return result;
		}

#endregion GET VALUE functions

#region SET VALUE functions

		/// <summary>
		/// Store a String value under the specified key in the current section
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetString (String key ,String value ,bool flushNow)
		{
			if (_selectedSection == -1)
				return;

			this[_selectedSection].SetKey(key ,value);
			_changeCount++;
			if (flushNow)
				Write(false);
		}


		/// <summary>
		/// Store a String value under the specified key in the specified section
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetString (String section ,String key ,String value ,bool flushNow)
		{
			SetSection(section);
			SetString(key ,value ,flushNow);
		}
	

		/// <summary>
		/// Store a Integer value in the specified section
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetInteger (String key ,int value ,bool flushNow)
		{
			String stringValue = value.ToString();
			SetString(key, stringValue, flushNow);
		}


		/// <summary>
		/// Store an Integer value under the specified key in the specified section
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetInteger(String section, String key, int value, bool flushNow)
		{
			SetSection(section);
			SetInteger(key, value, flushNow);
		}


		/// <summary>
		/// Store a boolean value in the specified section
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetBoolean(String key, bool value, bool flushNow)
		{
			String boolValue = (value) ? "Y" : "N";
			SetString(key, boolValue, flushNow);
		}


		/// <summary>
		/// Store a Boolean value under the specified key in the specified section
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetBoolean(String section, String key, bool value, bool flushNow)
		{
			SetSection(section);
			SetBoolean(key, value, flushNow);
		}


		/// <summary>
		/// Store a DateTime value in the specified section
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetTime(String key, DateTime value, bool flushNow)
		{
			SetString(key, value.ToString(), flushNow);
		}


		/// <summary>
		/// Store a DateTime value under the specified key in the specified section
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="flushNow"></param>
		public void SetTime (String section, String key, DateTime value, bool flushNow)
		{
			SetSection(section);
			SetTime(key, value, flushNow);
		}

#endregion SET VALUE functions

#region File functions

		public int Read (String fileName)
		{
			// Ensure that the file is not just open for read access
			if ((_flags & IF_NOREAD) != 0)
				return -1;

			// update filename if one was specified
			if (fileName != null)
				_fileName = fileName;

			// try and open the file - catch exceptions in case this fails
			StreamReader reader;
			try
			{
				reader = File.OpenText(_fileName);
			}
			catch (Exception)
			{
				// Failed to read the file - flag that we have changes
				_changeCount = 1;
				return -1;
			}

			// Now try and read the file
			String lineRead;
			lineRead = reader.ReadLine();
		    while (lineRead != null)
		    {
				// Process the line read from the file				
				lineRead.Trim();
				
				// Is there anything left to process
				if (lineRead.Length != 0)
				{
					// is it the start of a section ?
					if (lineRead[0] == '[')
					{
						// extract the name
						int n = lineRead.LastIndexOf(']');
						String sectionName = lineRead.Substring(1, (n - 1));
						_selectedSection = AddSection(sectionName);
					}

					else
					{
						// assume all others are keys - split the string at the equals sign
						int delimiter = lineRead.IndexOf('=');
						if (delimiter != -1)
						{
							String key = lineRead.Substring(0, delimiter);
							String value = lineRead.Substring(delimiter + 1);
							SetString (key, value ,false);
						}
					}
				}
				
				// Read the next line and loop back to process it
				lineRead = reader.ReadLine();
			}
			reader.Close();
			_changeCount = 0;
			return 0;
		}

		
		
		/// <summary>
		/// Write the ini file to disk if it has changed
		/// </summary>
		/// <param name="forceWrite"></param>
		/// <returns></returns>
		public int Write (bool forceWrite)
		{
			int errorCode = 0;

			// Ensure that the file is not just open for read access
			if ((_flags & IF_NOWRITE) != 0)
				return -1;

			// We only write if we have changes or have specified force write
			if ((_changeCount == 0) && (!forceWrite))
				return 0;

			// try and create a file to write to
			try
			{
			    StreamWriter writer;
				writer = File.CreateText(_fileName);

				// run through the ini file sections
				for (int sectionIndex = 0; ((errorCode == 0) && (sectionIndex < this.Count)) ; sectionIndex++)
				{
					// if not first one then write a blank line
					if (sectionIndex != 0)
						writer.WriteLine();

					// ...then write each section
					errorCode = this[sectionIndex].Write(writer);
				}

				// Close the file
				writer.Flush();
				writer.Close();

				// reset change count (cheat the const operator)
				_changeCount = 0;
			}
			catch (Exception)
			{
				errorCode = -1;
			}
			return errorCode;
		}

#endregion File Handling Functions


#region Section Handling Functions

		/// <summary>
		/// Remove a complete section from the IniFile
		/// </summary>
		/// <param name="section">Name of the section to remove</param>
		/// <param name="flushNow">True if we are to flush to disk after removal</param>
		public void RemoveSection (String section ,bool flushNow)
		{
			int sectionIndex = FindSection(section);
			if (sectionIndex != -1)
			{
				this.RemoveAt(sectionIndex);
				_changeCount++;
			}
		
			if (flushNow)
				this.Write(false);
		}



		/// <summary>
		/// Remove ALL sections from the file optionally flushing to disk
		/// </summary>
		/// <param name="flushNow"></param>
		public void RemoveAllSections (bool flushNow)
		{
			this.Clear();
			_changeCount++;
			_selectedSection = -1;
			if (flushNow)
				Write(false);
		}


		
		/// <summary>
		/// Set current section to the title of the specified window
		/// </summary>
		/// <param name="thisForm"></param>
		public void SetSection (Form thisForm)	
		{
			String formTitle = thisForm.Text;
			SetSection(formTitle);
		}


		/// <summary>
		/// Set the selection section to the specified named section
		/// </summary>
		/// <param name="section"></param>
		public void SetSection (String section)
		{
			_selectedSection = AddSection(section);
		}



		/// <summary>
		/// Return a list of the key (names) within the specified section
		/// </summary>
		/// <param name="section"></param>
		/// <param name="listKeys"></param>
		/// <returns></returns>
		public int EnumerateKeys (String section ,List<String> listKeys)
		{

			listKeys.Clear();
			int sectionIndex = FindSection(section);
			if (sectionIndex != -1)
				return this[sectionIndex].EnumerateKeys(listKeys);
			else
				return 0;
		}


		/// <summary>
		/// Remove the named key from the current section
		/// </summary>
		/// <param name="key"></param>
		/// <param name="flushNow"></param>
		public void RemoveKey (String key, bool flushNow)
		{
			if (_selectedSection != -1)
			{
				this[_selectedSection].RemoveKey(key);
				_changeCount++;
			}

			if (flushNow)
				Write(false);
		}


		/// <summary>
		/// Add a new section with the specified name if one does not already exist
		/// </summary>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public int AddSection (String sectionName)
		{
			// see if it already exists
			int sectionIndex = FindSection(sectionName);
			if (sectionIndex == -1)
			{
				// This section does not exist so add a new one with this name
				Add(new IniSection(sectionName));
				sectionIndex = this.Count - 1;
			}
			return sectionIndex;
		}


		/// <summary>
		/// Find an existing section with the specified name
		/// </summary>
		/// <param name="sectionName">Name of the section to find</param>
		/// <returns>Index of section if found, 0 otherwise</returns>
		public int FindSection (String sectionName)
		{
			for (int sectionIndex = 0; sectionIndex<this.Count; sectionIndex++)
			{
				if (this[sectionIndex].Name == sectionName)
					return sectionIndex;
			}
			return -1;
		}

#endregion Section Handling Functions

	}
}