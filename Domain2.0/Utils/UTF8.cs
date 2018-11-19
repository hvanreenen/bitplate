using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public static class UTF8
    {
        public static byte[] Encode(string str)
        {
            UTF8Encoding Utf8 = new UTF8Encoding();
            return Utf8.GetBytes(str);
        }

        public static string Decode(byte[] buffer)
        {
            UTF8Encoding Utf8 = new UTF8Encoding();
            return Utf8.GetString(buffer);
        }
    }
}
