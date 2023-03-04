namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using GlobalLifeTimeCache;

    internal static class AnalyzerGlobalLifeTimeCaches
    {
        internal static void Run()
        {
            int size = 100000;
            var reloadFunc = () =>
            {
                return
                    Enumerable.Range(0, size)
                              .Select(x => new KeyValuePair<int, TestItem>(x, new TestItem(x, x.ToString(), x.ToString())));
            };

            List<IGlobalLifeTimeCache<int, TestItem>> caches = new List<IGlobalLifeTimeCache<int, TestItem>>
            {
                new MicrosoftExtensionsGlobalLifeTimeCache<int, TestItem>(TimeSpan.FromSeconds(2), reloadFunc),
                new SystemMemoryGlobalLifeTimeCache<int, TestItem>(TimeSpan.FromSeconds(2), reloadFunc),
                new SimpleGlobalLifeTimeCache<int, TestItem>(TimeSpan.FromSeconds(2), reloadFunc)
            };

            Console.WriteLine("---------------------------------------------");
            foreach (var cache in caches)
            {
                Console.WriteLine($"Cache type: {cache.Name}");
                Console.WriteLine($"Test data amount: {size}");

                var measurementResult =
                    Measurement
                        .Create(Console.WriteLine)
                        .Start()
                        .Invoke("Trigger first load", () => cache.Get(1))
                        .Invoke("Contains Key",
                            () =>
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    _ = cache.ContainsKey(i);
                                }
                            })
                        .Invoke("Get",
                            () =>
                            {
                                for (int i = 0; i < size; i++)
                                {
                                    var cachedItem = cache.Get(i);
                                }
                            })
                        .Invoke("Remove",
                            () => {
                                for (int i = 0; i < size; i++)
                                {
                                    cache.Remove(i);
                                }
                            })
                        .Invoke("Reload",
                        () =>
                        {
                            var milliSecondsToWait = (cache.LifeTime.Seconds + 2) * 1000;
                            Console.WriteLine($"Wait until cache is outdated ({milliSecondsToWait}ms)");
                            Thread.Sleep(milliSecondsToWait);
                            cache.Get(1);
                        })
                        .Invoke("Clear", () => cache.Clear())
                        .Stop();

                Console.WriteLine("---------------------------------------------");
            }

            foreach (var cache in caches)
            {
                Console.WriteLine($"Size Check: {cache.Name}");
                var memoryBefore = GC.GetTotalMemory(true);
                for (int i = 0; i < size; i++)
                {
                    cache.AddOrUpdate(i, new TestItem(i, i.ToString(), i.ToString()));
                }
                var memoryAfter = GC.GetTotalMemory(true);
                Console.WriteLine($"Size after loading: {(int)((memoryAfter - memoryBefore) / 1024)} kB");
                Console.WriteLine("---------------------------------------------");
            }
        }
    }
}
