using BitPlate.Domain;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;

namespace BitSite._bitPlate._bitSystem
{
    /* Expirimenteel */
    public static class BitCaching
    {
        public static bool CachedPage { get; set; }

        public static T FromCache<T>(string key)
        {
            return WebCache.Get(key);
        }

        public static void ToCache<T>(this T self, string key, int minutesToCache = 60, bool slidingExpiration = true)
        {
            WebCache.Set(key, self, minutesToCache, slidingExpiration);
        }

        public static void RemoveItemFromCache(string key)
        {
            WebCache.Remove(key);
        }

        //public static void ClearCache()
        //{
        //    BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Site='" + SessionObject.CurrentSite.ID.ToString() + "'");
        //    foreach (CmsPage page in pages)
        //    {
        //        WebCache.Remove(page.ID.ToString());
        //    }
        //}
    }
}