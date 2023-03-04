namespace Cache
{
    /// <summary>
    /// Interface for key-value caches
    /// Generic, Thread safe, basic functionalities
    /// </summary>
    /// <typeparam name="TKey">Type of keys</typeparam>
    /// <typeparam name="TValue">Type of values</typeparam>
    public interface IEasyCache<TKey, TValue>
    {
        string Name { get; }
        long Count { get; }
        void AddOrUpdate(TKey key, TValue value);
        TValue Get(TKey key);
        bool ContainsKey(TKey key);
        TValue Remove(TKey key);
        void Clear();
    }
}