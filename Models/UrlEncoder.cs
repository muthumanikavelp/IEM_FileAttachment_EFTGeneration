using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Upload.Models
{
    public class UrlEncoder
    {

        public string URLDecode(string decode)
        {
            if (decode == null) return null;
            if (decode.StartsWith("="))
            {
                return FromBase64(decode.TrimStart('='));
            }
            else
            {
                return HttpUtility.UrlDecode(decode);
            }
        }

        public string UrlEncode(string encode)
        {
            if (encode == null) return null;
            string encoded = HttpUtility.UrlEncode(encode);
            if (encoded.Replace("%20", "") == encode.Replace(" ", ""))
            {
                return encoded;
            }
            else
            {
                return "=" + ToBase64(encode);
            }
        }

        public string ToBase64(string encode)
        {
            Byte[] btByteArray = null;
            UTF8Encoding encoding = new UTF8Encoding();
            btByteArray = encoding.GetBytes(encode);
            string sResult = System.Convert.ToBase64String(btByteArray, 0, btByteArray.Length);
            sResult = sResult.Replace("+", "-").Replace("/", "_");
            return sResult;
        }

        public string FromBase64(string decode)
        {
            decode = decode.Replace("-", "+").Replace("_", "/");
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(Convert.FromBase64String(decode));
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}