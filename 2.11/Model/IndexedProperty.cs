using System;

namespace Model
{
    public class IndexedProperty<TKey, TValue> : ILookup<TKey,TValue>
    {
        private readonly Func<TKey, TValue> _lookup;

        public IndexedProperty(Func<TKey, TValue> lookup)
        {
            _lookup = lookup;
        }

        public TValue this[TKey key]
        {
            get { return _lookup(key); }
        }
    }
}