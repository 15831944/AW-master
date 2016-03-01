using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DBUtility
{

    public class AppConfigFileHandler
    {
        #region Member variables
        private string m_strConfigFileName;
        private KeyValueConfigurationCollection m_objAppSettingsSection;
      
        #endregion

        #region Properties

        public KeyValueConfigurationCollection AppSettings
        {
            get
            {
                //read AppSetting section from configuration settings
                m_objAppSettingsSection = Read();
                return m_objAppSettingsSection;
            }
            set { m_objAppSettingsSection = value; }
        }
        #endregion

        #region Constructor
        public AppConfigFileHandler(string strConfigFile)
        {
            m_strConfigFileName = strConfigFile;
            m_objAppSettingsSection = new KeyValueConfigurationCollection();
        } 
        #endregion


        #region Private Methods
        private KeyValueConfigurationCollection Read()
        {
            try
            {
                //opening configuration file
                ExeConfigurationFileMap ConfigurationFile = new ExeConfigurationFileMap();
                ConfigurationFile.ExeConfigFilename = m_strConfigFileName;

                // Reading the contents of config file
                Configuration configSettings = ConfigurationManager.OpenMappedExeConfiguration(ConfigurationFile, ConfigurationUserLevel.None);
                return configSettings.AppSettings.Settings;
            }
            catch (Exception Ex)
            {
                throw new Exception("An error occured while reading config file.Details : " + Ex.Message, Ex.InnerException);
            }
           
        }
        #endregion
    }
}
