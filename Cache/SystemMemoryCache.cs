namespace Cache
{
    using System.Linq;
    using System.Runtime.Caching;

    /// <summary>
    /// MemoryCache derives from ObjectCache
    /// ObjectCache uses internally a IEnumerable<KeyValuePair<string, object>> as storage
    /// and therefore is NOT a generic solution.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SystemMemoryCache<TKey, TValue> : IEasyCache<TKey, TValue>
    {
        private MemoryCache cache = MemoryCache.Default;
        
        public string Name => this.cache.Name;

        public long Count => this.cache.GetCount();

        public void Clear()
        {
            foreach (var keyValuePair in this.cache.AsEnumerable())
            {
                _ = this.cache.Remove(keyValuePair.Key);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.Contains(key.ToString());
        }

        public TValue Get(TKey key)
        {
            return (TValue)this.cache[key.ToString()];
        }

        public TValue Remove(TKey key)
        {
            return (TValue)this.cache.Remove(key.ToString());
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            this.cache.Add(new CacheItem(key.ToString(), value), new CacheItemPolicy());
        }
    }
}
