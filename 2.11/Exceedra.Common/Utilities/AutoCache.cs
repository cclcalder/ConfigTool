using AutoMapper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace Exceedra.Common
{
    public class AutoCache<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly Func<TValue, TKey> _keySelector;
        private readonly ConcurrentDictionary<TKey, TValue> _cache = new ConcurrentDictionary<TKey, TValue>();
        private readonly List<TValue> _list = new List<TValue>(); 

        static AutoCache()
        {
            Mapper.CreateMap<TValue, TValue>();
        }

        public AutoCache(Func<TValue, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public TValue Cache(TValue value)
        {
            value = _cache.AddOrMap(_keySelector(value), value);
            if (!_list.Contains(value))
            {
                _list.Add(value);
            }
            return value;
        }

        public IEnumerable<TValue> CacheRange(IEnumerable<TValue> source)
        {
            if (source == null)
            {
                _cache.Clear();
                _list.Clear();
                return _list;
            }

            var retained = source.ToList();
            var remove = _cache.Keys.Where(key => !retained.Any(v => Equals(_keySelector(v), key)))
                .Select(key => key).ToList();
            foreach (var key in remove)
            {
                TValue value;
                if (_cache.TryRemove(key, out value))
                {
                    _list.Remove(value);
                }
            }
            // Force materialisation for caching...
            return retained.Select(Cache).ToList();
        }

        public IEnumerable<TValue> CacheRange(TValue[] source)
        {
            return source.Select(Cache);
        }
        
        public int Count
        {
            get { return _list.Count; }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _list.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _cache.Clear();
            _list.Clear();
        }

        private readonly IDictionary<string,string> _bag = new Dictionary<string, string>(StringComparer.CurrentCulture);
 
        public void SetExtraValue(string key, string value)
        {
            _bag[key] = value;
        }

        public string GetExtraValue(string key)
        {
            string value;
            _bag.TryGetValue(key, out value);
            return value;
        }
    }
}