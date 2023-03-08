namespace ItemLifeTimeCache
{
    using System;
    using System.Timers;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public class MicrosoftExtensionsItemLifeTimeCache<TKey, TValue> : IItemLifeTimeCache<TKey, TValue>
    {
        private IMemoryCache cache;

        public string Name => this.GetType().Name;

        public long Count => ((MemoryCache)this.cache).Count;

        public TimeSpan ItemLifeTime { get; set; }
        public TimeSpan CheckInterval { get; set; }

        public MicrosoftExtensionsItemLifeTimeCache(
            TimeSpan itemLifeTime, 
            TimeSpan checkInterval)
        {
            this.ItemLifeTime = itemLifeTime;
            this.CheckInterval = checkInterval;

            MemoryCacheOptions options = new MemoryCacheOptions();
            options.ExpirationScanFrequency = checkInterval;
            this.cache = new MemoryCache(options);
        }

        ~MicrosoftExtensionsItemLifeTimeCache()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.TryGetValue(key, out _);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(CheckInterval)
                .SetAbsoluteExpiration(ItemLifeTime);

            this.cache.Set(key, value, cacheEntryOptions);
        }

        public TValue Get(TKey key)
        {
            return this.cache.Get<TValue>(key);
        }

        public TValue Remove(TKey key)
        {
            TValue oldValue;
            this.cache.TryGetValue(key, out oldValue);
            this.cache.Remove(key);
            return oldValue;
        }

        public void Clear()
        {
            ((MemoryCache)this.cache).Clear();
        }
    }
}
