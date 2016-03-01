using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Layton.Common.Controls.GeoIP
{
	public class GeoIpRegions
	{
		Dictionary<string, Dictionary<string, string>> _countryDictionary = new Dictionary<string, Dictionary<string, string>>();

		public GeoIpRegions(string regionDefinitions)
		{
		}


		public void Initialize (string regionDefinitionFile)
		{
			// Open the geoipregionvars.txt file
			try
			{
				using (StreamReader sr = new StreamReader(regionDefinitionFile))
				{
					String line="";
					String country="";
					String regioncode="";
					String regionname="";

					Dictionary<string, string> thisRegionDictionary = null;
					while ((line = sr.ReadLine()) != null)
					{

						// If the line contains '=> array' then this is a Country Code Line so we strip
						// out and save the country code .  We must also save the region dictionary if we have
						// created one and throw away this line
						if (line.Contains("=> array"))
						{
							// OK end of this region so we must now add the region to the Dictionary of countries
							if (thisRegionDictionary != null)
								_countryDictionary.Add(country, thisRegionDictionary);

							// Create a new region dictionary object for the next region
							country = line.Substring(1, 2);
							thisRegionDictionary = new Dictionary<string, string>();
							continue;
						}

						// OK if the line starts with a quote then we can safely assume it is a region
						if (line[0] != '\"')
							continue;

						// The region code is in the form "RG" at the start of the line so get it
						regioncode = line.Substring(1, 2);

						// Now find the region name which is deleimited by quotes
						int regionNameStart = line.IndexOf("\"", 4);
						if (regionNameStart == -1)
							continue;
						int regionNameEnd = line.IndexOf("\"", regionNameStart + 1);
						if (regionNameEnd == -1)
							continue;

						// Strip out the region name
						regionname = line.Substring(regionNameStart + 1, regionNameEnd - regionNameStart - 1);

						// ...and add as a dictionary entry
						thisRegionDictionary.Add(regioncode, regionname);
					}
				}
			}

			catch (Exception)
			{
			}
		}


		/// <summary>
		/// Perform a region lookup 
		/// </summary>
		/// <param name="countryCode"></param>
		/// <param name="regionCode"></param>
		/// <returns></returns>
		public string LookupRegion(string countryCode, string regionCode)
		{
			string regionName = "unknown";

			// OK try and find an entry in the Country dictionary for this code
			Dictionary<string, string> regionDictionary = _countryDictionary[countryCode];
			if (regionDictionary != null)
				regionName = regionDictionary[regionCode];
			return regionName;
		}
	}
}
