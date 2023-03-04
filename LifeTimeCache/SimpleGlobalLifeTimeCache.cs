namespace GlobalLifeTimeCache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class SimpleGlobalLifeTimeCache<TKey, TValue> : IGlobalLifeTimeCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> cache 
            = new ConcurrentDictionary<TKey, TValue>();

        private DateTime lastRefresh = DateTime.MinValue;
        public string Name => this.GetType().Name;

        public long Count => this.cache.Count;

        public TimeSpan LifeTime { get; set; }

        public Func<IEnumerable<KeyValuePair<TKey,TValue>>> LoadingFunc { get; private set; }

        public SimpleGlobalLifeTimeCache(TimeSpan lifeTime, Func<IEnumerable<KeyValuePair<TKey, TValue>>> loadingFunc)
        {
            this.LifeTime = lifeTime;
            this.LoadingFunc = loadingFunc;
        }

        ~SimpleGlobalLifeTimeCache()
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
                var previousKeys = this.cache.Keys;
                var keysToRemove = previousKeys.Except(data.Select(x => x.Key).ToList());
                foreach (var item in data)
                {
                    this.cache.AddOrUpdate(item.Key, item.Value, (k, v) => item.Value);
                }
                foreach (var oldKey in keysToRemove)
                {
                    _ = this.cache.TryRemove(oldKey, out _);
                }

                this.lastRefresh = DateTime.Now;                
            }            
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.ContainsKey(key);
        }

        public void Set(TKey key, TValue value)
        {
            this.cache.AddOrUpdate(key, value, (k, v) => value);
        }

        public TValue Get(TKey key)
        {   
            this.AssureCacheIsUpToDate();
            if (this.cache.TryGetValue(key, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Missing key: {key}"); 
        }

        public TValue Remove(TKey key)
        {
            _ = this.cache.TryRemove(key, out var oldValue);
            return oldValue;
        }

        public void Clear()
        {
            this.cache.Clear();
        }
    }

}
