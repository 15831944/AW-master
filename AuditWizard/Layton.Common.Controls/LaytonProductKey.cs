using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Layton.Common.Controls
{
    public class LaytonProductKey
    {
        #region Constants
        const uint AWL_EVAL = 0x00010000;
        const uint AWL_SPARE = 0x00020000;
        const uint MASK_ALL_PRODUCTS = 0x00FF;
        const uint MASK_ALL_OPTIONS = 0xFF00;
        const uint MASK_ALL_FLAGS = (MASK_ALL_PRODUCTS | MASK_ALL_OPTIONS | AWL_EVAL | AWL_SPARE);
        const uint MASK_CRC = 0xFFFC0000;
        const uint XOR_KEY_1 = 0xA7FFFBC6;
        uint XOR_KEY_2 = 0x12BBA84F;
        const uint XOR_KEY_3 = 0xC4F339D9;
        const uint XOR_KEY_4 = 0x9B67DDA8;
        const uint XOR_KEY_5 = 0x8C98AC75;
        #endregion

        private string key;
        private int productID;
        private string code1;
        private string companyName;
        private int companyID;
        private DateTime installDate;
        private bool isTrial = true;
        private bool isTrialExpired = true;
        private int trialDayLength = 14;
        private int trialDaysRemaining = 0;
        private int assetCount = 0;

        public LaytonProductKey(string key, int productID, string companyName, int companyID, string code1)
        {
            this.key = key;
            this.productID = productID;
            this.companyName = companyName;
            this.companyID = companyID;
            this.code1 = code1;
            Initialize();
        }

        public string ProductKey
        {
            get { return key; }
        }

        public string Code1
        {
            get { return code1; }
        }

        public string CompanyName
        {
            get { return companyName; }
        }

        public int CompanyID
        {
            get { return companyID; }
        }

        public bool IsTrial
        {
            get { return isTrial; }
        }

        public bool IsTrialExpired
        {
            get { return isTrialExpired; }
        }

        public int TrialDaysRemaining
        {
            get { return trialDaysRemaining; }
        }

        public int ProductID
        {
            get { return productID; }
        }

        public int AssetCount
        {
            get { return assetCount; }
        }
        
        private void Initialize()
        {
            // first check if the user has a valid license key for the specified Product ID
            if (IsValidKey())
            {
                isTrial = false;
            }
            else
            {
                // user does not have a valid license key - check if they are under the trial period
                try
                {
                    installDate = CovertCodeToDateTime(code1);
                    TimeSpan trialSpan = DateTime.Today.Date - installDate;
                    isTrialExpired = trialSpan.Days > trialDayLength;
                    if (!isTrialExpired)
                    {
                        trialDaysRemaining = trialDayLength - trialSpan.Days;
                    }
                }
                catch
                {
                    isTrialExpired = true;
                }
            }
        }

        private DateTime CovertCodeToDateTime(string code)
        {
            long longVal = Convert.ToInt64(code);
            string hexVal = longVal.ToString("X");
            int year = int.Parse(hexVal.Substring(0, 3), System.Globalization.NumberStyles.HexNumber);
            int month = int.Parse(hexVal.Substring(3, 1), System.Globalization.NumberStyles.HexNumber);
            int day = int.Parse(hexVal.Substring(4), System.Globalization.NumberStyles.HexNumber);
            return new DateTime(year, month, day);
        }

        public static string GenerateCode2()
        {
            string hexDate = DateTime.Today.Year.ToString("X") + DateTime.Today.Month.ToString("X") + DateTime.Today.Day.ToString("X");
            int intDate = int.Parse(hexDate, System.Globalization.NumberStyles.HexNumber);
            return intDate.ToString();
        }

        #region private Helper functions

        private ulong Base32ToInt(string originalString)
        {
            ulong result = 0;
            for (int n = 0; n < originalString.Length; n++)
            {
                // make room with what we already have
                result <<= 5;
                int ascii = CharToInt32(originalString[n]);
                result += (ulong)ascii;
            }
            return result;
        }

        private int CharToInt32(char ch)
        {
            int asciiO = (int)'O';
            int asciiI = (int)'I';
            int asciiA = (int)'A';

            int result = (int)ch;
            if (result > asciiO)
                result--;
            if (result > asciiI)
                result--;
            if (result >= asciiA)
                result -= 7;
            return result - 50;
        }

        private ulong CountBits(ulong value)
        {
            // Calculates the number of set bits
            ulong result = 0;
            for (int n = 0; n < 32; n++)
            {
                result += (value & 0x01);
                value >>= 1;
            }
            return result;
        }

        private ulong CalculateCRC(ulong flags, ulong count, ulong date, ulong compId)
        {
            // we want a 14 bit CRC value to fill the available space
            // start with a simple sum of all the 1 bits in the data words
            ulong result = CountBits(flags & MASK_ALL_FLAGS)
                + CountBits(count)
                + CountBits(date)
                + CountBits(compId);
            // That has a value of 0 < val < 146 so occupies 8 bits
            // Fill the remaining 6 by repeating the low 2 bits of the count, flags & Company ID
            result <<= 2;
            result |= (count & 0x03);
            result <<= 2;
            result |= (flags & 0x03);
            result <<= 2;
            result |= (compId & 0x03);

            return result;
        }

        /// <summary>
        /// Validates the specified provided key.  Also sets the asset count.
        /// </summary>
        /// <returns>true, if a valid key</returns>
        private bool IsValidKey()
        {
            try
            {
                assetCount = 0;
                // string must be 5 x 5 chars long with 4 separators
                if (key.Length != 29)
                    return false;

                // remove the separators
                string tempKey = key.Replace("-", "");

                // break up the key into its parts
                string flagsString = tempKey.Substring(0, 7);
                string countString = tempKey.Substring(7, 4);
                string dateString = tempKey.Substring(11, 7);
                string companyString = tempKey.Substring(18, 7);

                // decode the key values
                ulong flags = Base32ToInt(flagsString);
                ushort count = (ushort)Base32ToInt(countString);
                ulong date = Base32ToInt(dateString);
                ulong compId = Base32ToInt(companyString);

                // un-encrypt the decoded values
                flags ^= XOR_KEY_1;
                count ^= (ushort)XOR_KEY_2;
                date ^= XOR_KEY_3;
                compId ^= XOR_KEY_5;

                // caluculate the CRC
                ulong calcCRC = CalculateCRC(flags, count, date, compId);
                ulong crc = flags >> 18;
                if (calcCRC != crc)
                {
                    return false;
                }

                // check that the company ID matches
                if ((int)compId != companyID)
                {
                    return false;
                }

                // check if the key is for the specified Product ID
                if ((uint)productID != (flags & (uint)productID))
                {
                    return false;
                }

                // set the asset count
                if (count == 0)
                {
                    // essentualy unlimited
                    count = 10000;
                }
                assetCount = count;
            }
            catch
            {
                // error validating license...assume an invalid key
                return false;
            }
            return true;
        }

        #endregion
    }
}
