namespace Cache
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Uses Microsoft.Extensions.Caching.Memory package.
    /// Internally it is based in ConcurrentDictionary<T,K>.
    /// </summary>
    public class MicrosoftExtensionsCache<TKey, TValue> : IEasyCache<TKey, TValue>
    {
        private IMemoryCache cache;

        public MicrosoftExtensionsCache()
        {
            MemoryCacheOptions options = new MemoryCacheOptions();           
            IOptions<MemoryCacheOptions> cacheOptions = options;
            this.cache = new MemoryCache(cacheOptions);
        }

        public string Name => this.GetType().Name;

        public long Count => ((MemoryCache)this.cache).Count;

        public bool ContainsKey(TKey key)
        {
            return this.cache.TryGetValue(key, out _);
        }

        public TValue Get(TKey key)
        {
            return this.cache.Get<TValue>(key);
        }

        public void Set(TKey key, TValue value)
        {
            this.cache.Set(key, value);
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