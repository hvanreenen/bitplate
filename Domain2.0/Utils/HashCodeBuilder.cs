using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BitPlate.Domain.Utils
{
    public static class HashCodeBuilder
    {
        public static string GetSHA1HashCode(string value){
            SHA1 hash = SHA1.Create();  
           System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();  
           byte[] combined = encoder.GetBytes(value);  
           hash.ComputeHash(combined);
           
            //maak er hex-string van
            string output = "";
           for (int i = 0; i < 20; i++)
           {
               string tmp = hash.Hash[i].ToString("X2");
               
               if (tmp.Length == 1)
               {
                   tmp = "0" + tmp;
               }
               output += tmp;
           }

           return output;
          
        }
    }
}
