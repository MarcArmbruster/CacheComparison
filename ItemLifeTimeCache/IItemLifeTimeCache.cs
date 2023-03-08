namespace ItemLifeTimeCache
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Interface for key-value caches
    /// Generic, Thread safe, overall life time
    /// </summary>
    /// <typeparam name="TKey">Type of keys</typeparam>
    /// <typeparam name="TValue">Type of values</typeparam>
    public interface IItemLifeTimeCache<TKey, TValue> : IDisposable
    {
        TimeSpan ItemLifeTime { get; }
        TimeSpan CheckInterval { get; }
        string Name { get; }
        long Count { get; }
        void AddOrUpdate(TKey key, TValue value);
        TValue Get(TKey key);
        bool ContainsKey(TKey key);
        TValue Remove(TKey key);
        void Clear();
    }
}
