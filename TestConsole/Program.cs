// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cache;
using TestConsole;

int size = 100000;

List<IEasyCache<int, TestItem>> caches = new List<IEasyCache<int, TestItem>>
{ 
    new MicrosoftExtensionsCache<int, TestItem>(),
    new SystemMemoryCache<int, TestItem>(),
    new SimpleCache<int, TestItem>()
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
            .Invoke("Add and update",
                () =>
                {
                    for (int i = 0; i < size; i++)
                    {
                        cache.Set(i, new TestItem(i, i.ToString(), i.ToString()));
                        cache.Set(i, new TestItem(i, i.ToString(), i.ToString()));
                    }
                })
            .Invoke("Contains Key",
                ()=>
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
                for (int i = 0; i < size; i++)
                {
                    cache.Set(i, new TestItem(i, i.ToString(), i.ToString()));
                }
            })
            .Invoke("Clear", () => cache.Clear())
            .Stop ();

    Console.WriteLine("---------------------------------------------");
}

foreach (var cache in caches)
{
    Console.WriteLine($"Size Check: {cache.Name}");
    var memoryBefore = GC.GetTotalMemory(true);
    for (int i = 0; i < size; i++)
    {
        cache.Set(i, new TestItem(i, i.ToString(), i.ToString()));
    }
    var memoryAfter = GC.GetTotalMemory(true);
    Console.WriteLine($"Size after loading: {(int)((memoryAfter - memoryBefore) / 1024)} kB");
    Console.WriteLine("---------------------------------------------");
}
    
Console.ReadLine();
