using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

using System.Data.SqlServerCe;

namespace Layton.AuditWizard.DataAccess
{
    public class Utility
    {
        private static SqlCeConnection _conn = null;
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Gets the IP address of an entry
        public static string GetIpAddress(string strName)
        {
            string strIpAddress = "";
            try
            {
                IPHostEntry hostInfo = Dns.GetHostByName(strName);
                if (hostInfo.AddressList.Length != 0)
                    strIpAddress = hostInfo.AddressList[0].ToString();
            }
            catch (SocketException)
            {
            }
            return strIpAddress;
        }

        //
        //    ListToString
        //    ============
        //
        //    Convert a dynalist of strings to a semi-colon seperated string
        //
        public static string ListToString(List<string> list, char chSep)
        {
            string result = "";
            foreach (string thisString in list)
            {
                if (result != "")
                    result += chSep;
                result += thisString;
            }
            return result;
        }

        /// <summary>
        /// Construct a List of strings from the delimited string passed in
        /// </summary>
        /// <param name="source">Delimited string</param>
        /// <param name="separator">Single character separator</param>
        /// <returns></returns>
        public static List<string> ListFromString(string source, char separator, bool removeBlankEntries)
        {
            List<string> outputList = new List<string>();

            // Set options as required
            StringSplitOptions options = StringSplitOptions.None;
            if (removeBlankEntries)
                options = StringSplitOptions.RemoveEmptyEntries;

            string[] tokens = source.Split(new char[] { separator }, options);
            outputList.AddRange(tokens);
            return outputList;
        }


        //Function to count no.of occurences of Substring in Main string
        public static int CharCount(String strSource, String strToCount)
        {
            int iCount = 0;
            int iPos = strSource.IndexOf(strToCount);
            while (iPos != -1)
            {
                iCount++;
                strSource = strSource.Substring(iPos + 1);
                iPos = strSource.IndexOf(strToCount);
            }
            return iCount;
        }

        /// <summary>
        /// Count the number of occurences of a string in another string ignoring case
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strToCount"></param>
        /// <param name="IgnoreCase"></param>
        /// <returns></returns>
        public static int CharCount(String strSource, String strToCount, bool IgnoreCase)
        {
            if (IgnoreCase)
            {
                return CharCount(strSource.ToLower(), strToCount.ToLower());
            }
            else
            {
                return CharCount(strSource, strToCount);
            }
        }

        //Useful Function can be used whitespace stripping programs
        //Function Trim the string to contain Single between words
        public static String ToSingleSpace(String strParam)
        {
            int iPosition = strParam.IndexOf("  ");
            if (iPosition == -1)
            {
                return strParam;
            }
            else
            {
                return ToSingleSpace(strParam.Substring(0, iPosition) + strParam.Substring(iPosition + 1));
            }
        }


        /// <summary>
        /// Copy the specified source file to the destination if it is newer
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool CopyFileIfNewer(string sourceFile, string destinationFile)
        {
            try
            {
                // Cannot copy if the source file does not exist
                if (!File.Exists(sourceFile))
                    return false;

                // If the destination file exists then get the last written time of both files
                if (File.Exists(destinationFile))
                {
                    DateTime sourceFileModified = File.GetLastWriteTime(sourceFile);
                    DateTime destinationFileModified = File.GetLastWriteTime(destinationFile);

                    // Do not copy if the source is not newer than the destination
                    if (sourceFileModified <= destinationFileModified)
                        return true;
                }

                File.Copy(sourceFile, destinationFile, true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static void DisplayErrorMessage(string errorMsgText)
        {
            // JML TODO temp removing this as creates error when called via a service
            //MessageBox.Show(errorMsgText, "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void DisplayApplicationErrorMessage(string errorMsgText)
        {
            MessageBox.Show(errorMsgText, "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static NameValueCollection GetComputerIpRanges()
        {
            NameValueCollection ipRanges = new NameValueCollection();
            LayIpAddressDAO layIpAddressDao = new LayIpAddressDAO();

            // if there are no rows at all, use the default settings - this is likely on first time use
            if (layIpAddressDao.SelectAllTcpIp().Rows.Count == 0)
            {
                string ipAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                string initialIpAddress = ipAddress.Substring(0, ipAddress.LastIndexOf('.'));

                ipRanges = new NameValueCollection { { initialIpAddress + ".1", initialIpAddress + ".254" } };
                layIpAddressDao.Add(new IPAddressRange(initialIpAddress + ".1", initialIpAddress + ".254", true,IPAddressRange.IPType.Tcpip));
                return ipRanges;
            }

            DataTable dt = layIpAddressDao.SelectAllActiveTcpIp();

            foreach (DataRow row in dt.Rows)
            {
                ipRanges.Add(row[0].ToString(), row[1].ToString());
            }

            return ipRanges;
        }

        public static NameValueCollection GetSNMPIpRanges()
        {
            NameValueCollection ipRanges = new NameValueCollection();
            LayIpAddressDAO layIpAddressDao = new LayIpAddressDAO();

            // if there are no rows at all, use the default settings - this is likely on first time use
            if (layIpAddressDao.SelectAllSnmp().Rows.Count == 0)
            {
                string ipAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                string initialIpAddress = ipAddress.Substring(0, ipAddress.LastIndexOf('.'));

                ipRanges = new NameValueCollection { { initialIpAddress + ".1", initialIpAddress + ".254" } };
                layIpAddressDao.Add(new IPAddressRange(initialIpAddress + ".1", initialIpAddress + ".254", true, IPAddressRange.IPType.Snmp));
                return ipRanges;
            }

            foreach (DataRow row in layIpAddressDao.SelectAllActiveSnmp().Rows)
            {
                ipRanges.Add(row[0].ToString(), row[1].ToString());
            }

            return ipRanges;
        }
    }
}
