namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ItemLifeTimeCache;

    internal static class AnalyzerItemLifeTimeCaches
    {
        internal static void Run()
        {
            int size = 100000;
            var loadFunc = (IItemLifeTimeCache<int, TestItem> cache) =>
            {
                foreach (var item in
                    Enumerable.Range(0, size)
                              .Select(x => new KeyValuePair<int, TestItem>(x, new TestItem(x, x.ToString(), x.ToString()))))
                {
                    cache.AddOrUpdate(item.Key, item.Value);
                }
            };

            TimeSpan itemLifeTime = TimeSpan.FromSeconds(1);
            TimeSpan checkLifeTime = TimeSpan.FromSeconds(3);
            List<IItemLifeTimeCache<int, TestItem>> caches = new List<IItemLifeTimeCache<int, TestItem>>
            {
                new MicrosoftExtensionsItemLifeTimeCache<int, TestItem>(itemLifeTime, checkLifeTime),
                new SystemMemoryItemLifeTimeCache<int, TestItem>(itemLifeTime, checkLifeTime),
                new SimpleItemLifeTimeCache<int, TestItem>(itemLifeTime, checkLifeTime)
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
                        .Invoke("First load", () => loadFunc(cache))
                        .Invoke("Find Item1", () => Console.WriteLine(cache.Get(1) != default(TestItem)))
                        .Invoke("Wait 5000ms", () => System.Threading.Thread.Sleep(5000))
                        .Invoke("Count", () => Console.WriteLine(cache.Count))
                        .Invoke("Find Item1", () => Console.WriteLine(cache.Get(1) != default(TestItem)))
                        .Stop();

                Console.WriteLine("---------------------------------------------");
            }

            foreach (var cache in caches)
            {
                cache.Clear();
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