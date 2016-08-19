namespace Model
{
    public interface ILookup<in TKey, out TValue>
    {
        TValue this[TKey key] { get; }
    }
}