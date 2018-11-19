using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public static class UrlConverter
    {
        public static string FromAbsolutePath2RelativeUrl(string path )
        {
            string url = "";
            path = path.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
            url = path.Replace("\\", "/");
            //System.Web.HttpContext.Current.Request.Url
            return url;
        }
    }
}
