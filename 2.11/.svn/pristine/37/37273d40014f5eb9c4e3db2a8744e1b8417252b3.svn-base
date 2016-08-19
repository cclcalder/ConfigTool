using System;
using System.Collections.Generic;

namespace Model.DataAccess.Generic
{
    public static class XmlCache
    {
        static Dictionary<string, CacheObject> CentralCache = new Dictionary<string, CacheObject>();

        public static object Upsert(string key, object Value)
        {
            CentralCache[key] = new CacheObject() { obj = Value, dt = DateTime.Now };

            return Value;
        }

        public static CacheObject GetItem(string key)
        {
            if (CentralCache.ContainsKey(key))
            {
                return CentralCache[key];
            }
            return null;
        }

        public static bool Contains(string key)
        {
            return CentralCache.ContainsKey(key);
        }

        public static void Clear()
        {
            CentralCache = new Dictionary<string, CacheObject>();
        }
    }

    public class CacheObject
    {
        public object obj { get; set; }
        public DateTime dt { get; set; }
  

    }
}
