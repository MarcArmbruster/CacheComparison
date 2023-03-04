namespace GlobalLifeTimeCache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public class MicrosoftExtensionsGlobalLifeTimeCache<TKey, TValue> : IGlobalLifeTimeCache<TKey, TValue>
    {
        private IMemoryCache cache;

        private DateTime lastRefresh = DateTime.MinValue;
        public string Name => this.GetType().Name;

        public long Count => ((MemoryCache)this.cache).Count;

        public TimeSpan LifeTime { get; set; }

        public Func<IEnumerable<KeyValuePair<TKey, TValue>>> LoadingFunc { get; private set; }

        public MicrosoftExtensionsGlobalLifeTimeCache(TimeSpan lifeTime, Func<IEnumerable<KeyValuePair<TKey, TValue>>> loadingFunc)
        {
            this.LifeTime = lifeTime;
            this.LoadingFunc = loadingFunc;

            MemoryCacheOptions options = new MemoryCacheOptions();
            IOptions<MemoryCacheOptions> cacheOptions = options;
            this.cache = new MemoryCache(cacheOptions);
        }

        ~MicrosoftExtensionsGlobalLifeTimeCache()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.LoadingFunc = null;
        }

        private void AssureCacheIsUpToDate()
        {
            if (Math.Abs((DateTime.Now - this.lastRefresh).TotalSeconds) > LifeTime.TotalSeconds)
            {
                Console.WriteLine($"Call refresh");
                var data = this.LoadingFunc.Invoke();
                this.Clear();
                foreach (var item in data)
                {
                    this.cache.Set(item.Key, item.Value);
                }
                
                this.lastRefresh = DateTime.Now;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.TryGetValue(key, out _);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            this.cache.Set(key, value);
        }

        public TValue Get(TKey key)
        {
            this.AssureCacheIsUpToDate();
            return this.cache.Get<TValue>(key);
        }

        public TValue Remove(TKey key)
        {
            TValue oldValue = default;
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
