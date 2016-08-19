using System.Collections.Concurrent;
using AutoMapper;

namespace Exceedra.Common
{
    static class ConcurrentDictionaryMapper
    {
        public static TValue AddOrMap<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue addValue)
        {
            return dict.AddOrUpdate(key, addValue, (k, v) => Mapper.Map(addValue, v));
        }
    }
}