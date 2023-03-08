namespace GlobalLifeTimeCache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    public class SystemMemoryGlobalLifeTimeCache<TKey, TValue> : IGlobalLifeTimeCache<TKey, TValue>
    {
        private MemoryCache cache =  MemoryCache.Default;

        private DateTime lastRefresh = DateTime.MinValue;
        public string Name => this.GetType().Name;

        public long Count => this.cache.GetCount();

        public TimeSpan LifeTime { get; set; }

        public Func<IEnumerable<KeyValuePair<TKey, TValue>>> LoadingFunc { get; private set; }

        public SystemMemoryGlobalLifeTimeCache(TimeSpan lifeTime, Func<IEnumerable<KeyValuePair<TKey, TValue>>> loadingFunc)
        {
            this.LifeTime = lifeTime;
            this.LoadingFunc = loadingFunc;
        }

        ~SystemMemoryGlobalLifeTimeCache()
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
                var previousKeys = this.cache.AsEnumerable().Select(i => i.Key);
                var keysToRemove = previousKeys.Except(data.Select(x => x.Key.ToString()).ToList());
                foreach (var item in data)
                {
                    this.AddOrUpdate(item.Key, item.Value);
                }
                foreach (var oldKey in keysToRemove)
                {
                    this.RemoveInternal(oldKey);
                }

                this.lastRefresh = DateTime.Now;
            }
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
                this.cache.Add(new CacheItem(key.ToString(), value), new CacheItemPolicy());
            }
        }

        public TValue Get(TKey key)
        {
            this.AssureCacheIsUpToDate();

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
