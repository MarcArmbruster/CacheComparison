namespace ItemLifeTimeCache
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;

    public class SystemMemoryItemLifeTimeCache<TKey, TValue> : IItemLifeTimeCache<TKey, TValue>
    {
        private MemoryCache cache = MemoryCache.Default;

        public string Name => this.GetType().Name;

        public long Count => this.cache.GetCount();

        public TimeSpan ItemLifeTime { get; set; }
        public TimeSpan CheckInterval { get; set; }

        public SystemMemoryItemLifeTimeCache(TimeSpan itemLifeTime, TimeSpan checkInterval)
        {
            this.ItemLifeTime = itemLifeTime;
            //this.cache.PollingInterval.Subtract(this.cache.PollingInterval);
            //this.cache.PollingInterval.Add(checkInterval);
        }

        ~SystemMemoryItemLifeTimeCache()
        {
            this.Dispose();
        }

        public void Dispose()
        {
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.Contains(key.ToString());
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (this.cache.Contains(key.ToString()))
            {
                this.cache[key.ToString()] = value;
            }
            else
            {
                var policy = new CacheItemPolicy();
                policy.SlidingExpiration = this.ItemLifeTime;
                
                this.cache.Add(
                    new CacheItem(key.ToString(), value), 
                    policy);
            }
        }

        public TValue Get(TKey key)
        {
            return (TValue)this.cache[key.ToString()];
        }

        public void RemoveInternal(string keyAsString)
        {
            _ = this.cache.Remove(keyAsString);
        }

        public TValue Remove(TKey key)
        {
            return (TValue)this.cache.Remove(key.ToString());
        }

        public void Clear()
        {
            foreach (var keyValuePair in this.cache.AsEnumerable())
            {
                _ = this.cache.Remove(keyValuePair.Key);
            }
        }
    }
}
