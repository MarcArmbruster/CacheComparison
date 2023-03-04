namespace GlobalLifeTimeCache
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Interface for key-value caches
    /// Generic, Thread safe, overall life time
    /// </summary>
    /// <typeparam name="TKey">Type of keys</typeparam>
    /// <typeparam name="TValue">Type of values</typeparam>
    public interface IGlobalLifeTimeCache<TKey, TValue> : IDisposable
    {
        TimeSpan LifeTime { get; }
        
        Func<IEnumerable<KeyValuePair<TKey, TValue>>> LoadingFunc { get; }
        
        string Name { get; }
        long Count { get; }
        void AddOrUpdate(TKey key, TValue value);
        TValue Get(TKey key);
        bool ContainsKey(TKey key);
        TValue Remove(TKey key);
        void Clear();
    }
}
