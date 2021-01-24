using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DataQuery.Net.Extensions
{

    internal static class String
    {
        /// <summary>
        /// Replace the first occurence in a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        internal static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Create a unique Guid from string
        /// </summary>
        /// <param name="input">String à partir de laquelle générer un GUID</param>
        /// <returns>GUID réalisé à partir de MD5</returns>
        internal static Guid ToGuid(this string input)
        {
            // http://stackoverflow.com/questions/2190890/how-can-i-generate-guid-for-a-string-values
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }

    }
}
