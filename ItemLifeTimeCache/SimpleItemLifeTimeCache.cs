namespace ItemLifeTimeCache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Timers;

    internal class InternalCacheItem<TValue>
    {
        internal DateTime TimeStamp { get; set; }

        internal TValue Content { get; set; }

        public InternalCacheItem(TValue value)
        {
            Content = value;
            TimeStamp = DateTime.Now;
        }
    }

    public class SimpleItemLifeTimeCache<TKey, TValue> : IItemLifeTimeCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, InternalCacheItem<TValue>> cache
            = new ConcurrentDictionary<TKey, InternalCacheItem<TValue>>();

        private Timer checkTimer;

        public string Name => this.GetType().Name;

        public long Count => this.cache.Count;

        public TimeSpan ItemLifeTime { get; set; }
        public TimeSpan CheckInterval { get; set; }

        public SimpleItemLifeTimeCache(
            TimeSpan itemLifeTime,
            TimeSpan checkInterval)
        {
            this.ItemLifeTime = itemLifeTime;
            this.CheckInterval = checkInterval;

            this.checkTimer = new Timer(checkInterval.TotalMilliseconds);
            this.checkTimer.Elapsed += OnCheckTimerElapsed;
            this.checkTimer.Start();
        }

        ~SimpleItemLifeTimeCache()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.checkTimer.Stop();
            this.checkTimer.Elapsed -= OnCheckTimerElapsed;            
            this.checkTimer.Dispose();
        }

        private void OnCheckTimerElapsed(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;
            List<TKey> keysToRemove = new List<TKey>();
            foreach (var item in this.cache)
            {
                if (Math.Abs((now - item.Value.TimeStamp).TotalMilliseconds) > this.ItemLifeTime.TotalMilliseconds)
                {
                    keysToRemove.Add(item.Key);
                }
            }

            foreach (var key in keysToRemove)
            { 
                this.Remove(key); 
            }
        }

        public bool ContainsKey(TKey key)
        {
            return this.cache.ContainsKey(key);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            var internalItem = new InternalCacheItem<TValue>(value);
            this.cache.AddOrUpdate(key, internalItem, (k, v) => internalItem);
        }

        public TValue Get(TKey key)
        {
            if (this.cache.TryGetValue(key, out var result))
            {
                result.TimeStamp = DateTime.Now;
                return result.Content;
            }

            return default(TValue);
        }

        public TValue Remove(TKey key)
        {
            _ = this.cache.TryRemove(key, out var oldValue);
            return oldValue == null ? default(TValue) : oldValue.Content;
        }

        public void Clear()
        {
            this.cache.Clear();
        }
    }
}
