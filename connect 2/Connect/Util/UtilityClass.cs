using Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Connect.Util
{
    static public class ConnectUtils
    {
        public static String GetMD5Hash(String TextToHash)
        {
            if ((TextToHash == null) || (TextToHash.Length == 0))
            {
                return String.Empty;
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] textToHash = Encoding.Default.GetBytes(TextToHash);
            byte[] result = md5.ComputeHash(textToHash);

            return System.BitConverter.ToString(result).Replace("-", string.Empty);
        }
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }
    }
}