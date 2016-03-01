using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Layton.AuditWizard.Common
{
    public class AuditWizardSerialization
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool SerializeObjectToFile(AuditScannerDefinition aAuditScannerDefintion)
        {
            return SerializeObjectToFile(aAuditScannerDefintion, aAuditScannerDefintion.Filename);
        }

        public static bool SerializeObjectToFile(AuditScannerDefinition aAuditScannerDefintion, string aFileName)
        {
            TextWriter w = new StreamWriter(aFileName);

            try
            {
                XmlSerializer s = new XmlSerializer(typeof(AuditScannerDefinition));
                s.Serialize(w, aAuditScannerDefintion);
            }
            catch (Exception ex)
            {
                logger.Error("Error in SerializeObjectToFile() with file: " + aFileName, ex);
                MessageBox.Show("There was an error saving the configuration.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                w.Close();
            }

            return true;
        }

        /// <summary>
        /// Gets a AuditScannerDefinition object by filename
        /// </summary>
        /// <param name="filename">location of AuditScannerDefinition.xml</param>
        /// <returns>AuditScannerDefinition</returns>
        public static AuditScannerDefinition DeserializeObject(string filename)
        {
            // Create an instance of the XmlSerializer specifying type.
            XmlSerializer serializer = new XmlSerializer(typeof(AuditScannerDefinition));

            AuditScannerDefinition auditScannerDefinition = null;
            TextReader reader = null;

            try
            {
                reader = new StreamReader(filename, Encoding.Default);
                auditScannerDefinition = (AuditScannerDefinition)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                if (reader != null) 
                    reader.Close();
            }

            return auditScannerDefinition;
        }

        /// <summary>
        /// Gets the current AuditScanner.xml object
        /// </summary>
        /// <returns>AuditScannerDefinition</returns>
        public static AuditScannerDefinition DeserializeDefaultScannerObject()
        {
            string filename = Application.StartupPath + "\\Scanners\\default.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(AuditScannerDefinition));

            AuditScannerDefinition auditScannerDefinition = null;
            TextReader reader = null;

            try
            {
                reader = new StreamReader(filename, Encoding.Default);
                auditScannerDefinition = (AuditScannerDefinition)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                logger.Error("in DeserializeDefaultScannerObject, unable to load default.xml file, attempted location was : " + filename, ex);
            }
            finally
            {
                if (reader != null) 
                    reader.Close();
            }

            return auditScannerDefinition;
        }
    }
}
