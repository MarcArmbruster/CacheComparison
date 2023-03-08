// See https://aka.ms/new-console-template for more information
using x_FileObserverCache;

Console.WriteLine("File Observer with System.Memory.Caching");

// expecting a text file C:\Test\Marc.txt exists
FileSystemMemoryCache instance = new FileSystemMemoryCache();

foreach (var item in Enumerable.Range(0,10))
{
    Thread.Sleep(5000);
    Console.WriteLine(instance.Get("myFile"));

    Thread.Sleep(100);

    //string content = "".PadRight(item, 'x');
    //System.IO.File.WriteAllText(@"C:\Test\Marc.txt", content);    
}

Console.WriteLine("Done");
Console.ReadLine();