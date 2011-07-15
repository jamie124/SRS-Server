using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SRS_Server.Net
{
    public class Encryption
    {
        // Encryption functions have been copied and variables renamed from
        // http://www.codeproject.com/KB/security/DotNetCrypto.aspx
        private byte[] Encrypt(byte[] prData, byte[] prKey, byte[] prIV)
        {
            // Create a MemoryStream to accept the encrypted bytes 

            MemoryStream iStream = new MemoryStream();

            Rijndael iRijndael = Rijndael.Create();

            iRijndael.Key = prKey;
            iRijndael.IV = prIV;

            CryptoStream iCryptoStream = new CryptoStream(iStream,
               iRijndael.CreateEncryptor(), CryptoStreamMode.Write);


            iCryptoStream.Write(prData, 0, prData.Length);
            iCryptoStream.Close();

            byte[] iEncryptedData = iStream.ToArray();

            return iEncryptedData;
        }

        public string Encrypt(string prStringToEncrypt, string prPassword)
        {

            byte[] iStringBytes = System.Text.Encoding.Unicode.GetBytes(prStringToEncrypt);

            PasswordDeriveBytes iPDB = new PasswordDeriveBytes(prPassword,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            byte[] iEncryptedData = Encrypt(iStringBytes,
                     iPDB.GetBytes(32), iPDB.GetBytes(16));

            return Convert.ToBase64String(iEncryptedData);
        }

        public  byte[] Decrypt(byte[] prData, byte[] prKey, byte[] prIV)
        {

            MemoryStream iStream = new MemoryStream();

            Rijndael iRijndael = Rijndael.Create();

            iRijndael.Key = prKey;
            iRijndael.IV = prIV;

            CryptoStream iCryptoStream = new CryptoStream(iStream,
                iRijndael.CreateDecryptor(), CryptoStreamMode.Write);

            iCryptoStream.Write(prData, 0, prData.Length);
            iCryptoStream.Close();

            byte[] iDecryptedData = iStream.ToArray();

            return iDecryptedData;
        }

        public string Decrypt(string prStringToEncrypt, string prPassword)
        {
            byte[] iEncryptedBytes = Convert.FromBase64String(prStringToEncrypt);

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(prPassword,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 
            0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            byte[] iDecryptedData = Decrypt(iEncryptedBytes,
                pdb.GetBytes(32), pdb.GetBytes(16));

            return System.Text.Encoding.Unicode.GetString(iDecryptedData);
        }
    }
}
