using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Layton.AuditWizard.DataAccess
{
    public class AES
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string password = "1QaZxSw@3EdCvFr$";

        //Use a 32-byte key (for 256-bit encryption)
        private static byte[] keyBytes = { 105, 129, 186, 236, 10, 86, 161, 159, 37, 115, 227, 52, 243, 113, 136, 26, 
                                127, 191, 44, 241, 128, 195, 171, 75, 228, 5, 202, 210, 120, 77, 86, 6};

        //The IV for AES is 16 bytes, because the AES block size is 16.
        private static byte[] ivBytes = { 237, 212, 102, 125, 33, 255, 213, 70, 251, 94, 40, 213, 32, 214, 102, 55 };

        public static string EncryptFTPPassword(string clearText)
        {
            try
            {
                RijndaelManaged rman = new RijndaelManaged();
                rman.Mode = CipherMode.CBC;
                rman.Padding = PaddingMode.PKCS7;
                rman.KeySize = 256;

                ICryptoTransform encryptor = rman.CreateEncryptor(keyBytes, ivBytes);
                byte[] plainText = Encoding.UTF8.GetBytes(clearText);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(plainText, 0, plainText.Length));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return String.Empty;
            }
        }

        public static string DecryptFTPPassword(string cipherText)
        {
            try
            {
                RijndaelManaged rman = new RijndaelManaged();
                rman.Mode = CipherMode.CBC;
                rman.Padding = PaddingMode.PKCS7;
                rman.KeySize = 256;

                ICryptoTransform encryptor = rman.CreateDecryptor(keyBytes, ivBytes);
                byte[] plainText = Convert.FromBase64String(cipherText);
                return ASCIIEncoding.ASCII.GetString(encryptor.TransformFinalBlock(plainText, 0, plainText.Length));
            }
            catch (FormatException ex)
            {
                // if we hit a format exception, return the original string
                // this will handle users who were using an old (unencrypted password) version of the application
                logger.Error(ex.Message);
                return cipherText;
            }
            catch (Exception ex)
            {
                // do the same for all exceptions at this stage
                logger.Error(ex.Message);
                return cipherText;
            }
        }

        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // PasswordDeriveBytes is for getting Key and IV.
            // Using PasswordDeriveBytes object we are first getting 32 bytes for the Key (the default
            // Rijndael key length is 256bit = 32bytes) and then 16 bytes for the IV.
            // IV should always be the block size, which is by default 16 bytes (128 bit) for Rijndael.

            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }

        // Encrypt a byte array into a byte array using a key and an IV
        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();

            // Algorithm. Rijndael is available on all platforms.
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            //CryptoStream is for pumping our data.
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        // Decrypt a string into a string using a password
        // Uses Decrypt(byte[], byte[], byte[])
        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }
    }
}