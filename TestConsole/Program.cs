// See https://aka.ms/new-console-template for more information
using TestConsole;

Console.WriteLine("================================");
Console.WriteLine("AnalyzerSimpleCaches");
Console.WriteLine("================================");
AnalyzerSimpleCaches.Run();

Console.WriteLine("================================");
Console.WriteLine("AnalyzerGlobalLifeTimeCaches");
Console.WriteLine("================================");
AnalyzerGlobalLifeTimeCaches.Run();
    
Console.ReadLine();
