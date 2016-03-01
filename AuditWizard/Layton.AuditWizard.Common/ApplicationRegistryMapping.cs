using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// This class defines the mapping between a somgle application name and the registry
	/// key/value pairs which define the location of (any) serial number for that application
	/// </summary>
	public class ApplicationRegistryMapping : List<RegistryMapping>
	{
		private String _applicationName;

		public String ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public ApplicationRegistryMapping()
		{
		}

		/// <summary>
		/// Default constructor taking just the application name
		/// </summary>
		public ApplicationRegistryMapping(String applicationName)
		{
			_applicationName = applicationName;
		}


		/// <summary>
		/// Add mappings from a mappings string
		/// </summary>
		/// <param name="mappingString"></param>
		public void AddMapping(String mappingString)
		{
			// Each separate mapping is delimited by a semicolon
			String[] mappings = mappingString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); 

			// Iterate through the individual mappings
			foreach (String nextMapping in mappings)
			{
				// The key/value pair are separated by an ',' sign
				List<String> mappingParts = ParseDelimitedString(nextMapping, ',');							// 8.3.6
				//String[] mappingParts = nextMapping.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				// Skip any that don't conform
				if (mappingParts.Count != 2)
					continue;

				this.Add(new RegistryMapping(mappingParts[0] ,mappingParts[1]));
			}
		}


		/// <summary>
		/// 8.3.6
		/// Split a string into its delimited parts ensuring we handle cases where we have quoted strings containing the delimiter
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="delim"></param>
		/// <returns></returns>
		private List<string> ParseDelimitedString(string arguments, char delim = ',')
		{
			bool inQuotes = false;

			List<string> strings = new List<string>();

			StringBuilder sb = new StringBuilder();
			foreach (char c in arguments)
			{
				if (c == '\'' || c == '"')
				{
					inQuotes = !inQuotes;
				}
				
				else if (c == delim)
				{
					if (!inQuotes)
					{
						strings.Add(sb.Replace("'", string.Empty).Replace("\"", string.Empty).ToString());
						sb.Remove(0, sb.Length);
					}
					else
					{
						sb.Append(c);
					}
				}
				else 
				{
					sb.Append(c);
				}
			}
			strings.Add(sb.Replace("'", string.Empty).Replace("\"", string.Empty).ToString());


			return strings;
		}
	}


	/// <summary>
	/// This class encapsulates an instance of a registry mapping pair
	/// </summary>
	public class RegistryMapping
	{
		private string _registryKey;
		private string _valuename;

		public string RegistryKey
		{
			get { return _registryKey; }
			set { _registryKey = value; }
		}

		public string ValueName
		{
			get { return _valuename; }
			set { _valuename = value; }
		}

		public RegistryMapping()
		{
			_registryKey = "";
			_valuename = "";
		}

		public RegistryMapping (String keyname ,String valuename)
		{
			_registryKey = keyname;
			_valuename = valuename;
		}
	}		
}
