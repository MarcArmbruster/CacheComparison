namespace Cache
{
    using System;
    using System.Collections.Concurrent;

    public class SimpleCache<TKey, TValue> : IEasyCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> cache = new ConcurrentDictionary<TKey, TValue>();

        public string Name => this.GetType().Name;

        public long Count => this.cache.Count;

        public bool ContainsKey(TKey key)
        {
            return this.cache.ContainsKey(key);
        }

        public TValue Get(TKey key)
        {
            if (this.cache.TryGetValue(key, out var result))
            {
                return result;
            }

            throw new InvalidOperationException("Missing key");
        }

        public TValue Remove(TKey key)
        {
            _ = this.cache.TryRemove(key, out var oldValue);
            return oldValue;
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            this.cache.AddOrUpdate(key, value, (k,v) => value);
        }

        public void Clear()
        {
            this.cache.Clear();
        }
    }
}
