using System;
using System.Collections.Generic; 

namespace WPF.ViewModels.Generic
{
    public class ApplicationCache
    {
        Dictionary<string, CacheObject> CentralCache = new Dictionary<string, CacheObject>();

        public object Upsert(string key, object Value)
        {
            if (CentralCache.ContainsKey(key))
            {
                CentralCache.Remove(key);
            }
            
            CentralCache.Add(key, new CacheObject(){obj= Value, dt = DateTime.Now});
             
            return Value;
        }

        public object Upsert(string key, object Value, DateTime d)
        {
            if (CentralCache.ContainsKey(key))
            {
                CentralCache.Remove(key);
            }

            CentralCache.Add(key, new CacheObject() { obj = Value, dt = d});

            return Value;
        }

        public void Add(string key, object Value)
        {
            if (!CentralCache.ContainsKey(key))
            {
                CentralCache.Add(key,  new CacheObject(){obj= Value, dt = DateTime.Now});
            }
        }

        public void Remove(string key)
        {
            if (CentralCache.ContainsKey(key))
            {
                CentralCache.Remove(key);
            }
        }

        public CacheObject GetItem(string key)
        {
            if (CentralCache.ContainsKey(key))
            {
                return CentralCache[key];
            }
            return null;
        }
        static ApplicationCache cache = null;
        public static ApplicationCache CreateInstance()
        {
            if (cache == null)
            {
                return new ApplicationCache();
            }
            return cache;
        }
    }

    public class CacheObject
    {
        public object obj { get; set; }
        public DateTime dt { get; set; }
  

    }
}
