using System;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;

namespace Layton.NetworkDiscovery
{
    public class DiscoveryConfigurationManager
    {
        private static KeyValueConfigurationCollection OpenIpRangeSettings(Configuration config)
        {
            // Get the IP Ranges Section
            AppSettingsSection ipRangeSettings = (AppSettingsSection)config.GetSection(XmlSettings.tcpipSettings + "/" + XmlSettings.ipAddressRanges);

            if (ipRangeSettings == null)
            {
                if (config.SectionGroups[XmlSettings.tcpipSettings] == null)
                {
                    config.SectionGroups.Add(XmlSettings.tcpipSettings, new ConfigurationSectionGroup());
                    config.SectionGroups[XmlSettings.tcpipSettings].Sections.Add(XmlSettings.ipAddressRanges, new AppSettingsSection());
                }
                else
                {
                    config.SectionGroups[XmlSettings.tcpipSettings].Sections.Add(XmlSettings.ipAddressRanges, new AppSettingsSection());
                }
            }
            ipRangeSettings = (AppSettingsSection)config.GetSection(XmlSettings.tcpipSettings + "/" + XmlSettings.ipAddressRanges);
            return ipRangeSettings.Settings;
        }

        public static NameValueCollection GetIpRangeSettings()
        {
            // Open the application configuration file and get the IP Ranges Section
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection ipRangeSettings = OpenIpRangeSettings(config);

            NameValueCollection ipRanges = new NameValueCollection();
            foreach (string settingsKey in ipRangeSettings.AllKeys)
            {
                if (ipRanges[settingsKey] == null)
                {
                    ipRanges.Add(settingsKey, ipRangeSettings[settingsKey].Value);
                }
            }
            return ipRanges;
        }

        public static void SaveIpRangeSettings(NameValueCollection ipRanges)
        {
            // Open the application configuration file and get the IP Ranges Section
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection ipRangeSettings = OpenIpRangeSettings(config);

            // Clear out the existing ranges to fill with the new ones
            ipRangeSettings.Clear();

            // Add the IP ranges and save the config file
            foreach (string key in ipRanges.AllKeys)
            {
                ipRangeSettings.Add(key, ipRanges[key]);
            }
            config.Save();
        }
    }
}
