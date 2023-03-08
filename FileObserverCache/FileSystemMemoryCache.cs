namespace x_FileObserverCache
{
    using System.Collections.Concurrent;
    using System.Runtime.Caching;

    /// <summary>
    /// MemoryCache derives from ObjectCache
    /// ObjectCache uses internally a IEnumerable<KeyValuePair<string, object>> as storage
    /// and therefore is NOT a generic solution.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FileSystemMemoryCache
    {
        private MemoryCache innerFileCache = MemoryCache.Default;

        public ConcurrentDictionary<string, string> FilesToMonitor { get; }
            = new ConcurrentDictionary<string, string>();            

        public FileSystemMemoryCache()
        {
            // register test data file
            this.FilesToMonitor.AddOrUpdate("myFile", @"C:\Test\Marc.txt", (k,v) => v);

            foreach (var key in this.FilesToMonitor.Keys)
            {
                this.LoadFileToCache(key);
            }
        }

        private CacheItemPolicy CreatePolicy(string filePath)
        {
            var filePaths = new List<string> { filePath };
            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromDays(1);
            var fileChangeMonitor = new HostFileChangeMonitor(filePaths);
            policy.ChangeMonitors.Add(fileChangeMonitor);
            return policy;
        }

        private void LoadFileToCache(string key)
        {
            if (this.FilesToMonitor.TryGetValue(key, out string? filePath))
            {
                var fileContent = File.ReadAllText(filePath);
                var cacheItem = new CacheItem(key, fileContent);
                var policy = CreatePolicy(filePath);

                this.innerFileCache.Set(key, cacheItem, policy);
            }
        }

        public string Get(string key)
        {
            var item = (CacheItem)this.innerFileCache[key];
            if (item == null || item.Value == null)
            {
                this.LoadFileToCache(key);
                item = (CacheItem)this.innerFileCache[key];
            }
            
            return item?.Value?.ToString() ?? "not found";
        }
    }
}
