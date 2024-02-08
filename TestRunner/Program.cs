using HtmlPerformanceKit.Benchmark;

namespace TestRunner
{
    internal class Program
    {
        private static readonly BenchmarkHtmlPerformanceKit benchmark = new BenchmarkHtmlPerformanceKit();

        static void Main()
        {
            RunTimes(1);
            RunTimes(10);
            RunTimes(100);
            RunTimes(1000);
        }

        static void RunTimes(int times)
        {
            Console.WriteLine($"About to run {times} times. Press any key to start.");
            Console.ReadLine();

            for (var i = 0; i < times; i++)
            {
                benchmark.ExtractLinks();
                benchmark.ExtractLinksAsMemory();
                benchmark.ExtractTexts();
                benchmark.ExtractTextsAsMemory();
            }

            Console.WriteLine("Done");
            Console.WriteLine();
        }
    }
}
