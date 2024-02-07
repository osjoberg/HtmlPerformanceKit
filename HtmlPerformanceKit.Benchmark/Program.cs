using BenchmarkDotNet.Running;
using System;

namespace HtmlPerformanceKit.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var x = new BenchmarkHtmlPerformanceKit();
            // Console.WriteLine("Press to continue");
            // Console.ReadLine();
            RunOnce(x);
            Console.WriteLine("Press to continue");
            Console.ReadLine();
            RunMore(x);
            Console.WriteLine("Press to continue");
            Console.ReadLine();

            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(BenchmarkLibraries),
                typeof(BenchmarkHtmlPerformanceKit),
            });

            switcher.Run(args);
        }

        private static void RunOnce(BenchmarkHtmlPerformanceKit x)
        {
            for (var i = 0; i < 1; i++)
            {
                Console.WriteLine(x.ExtractTexts().Count);
                Console.WriteLine(x.ExtractTextsAsMemory().Count);
                Console.WriteLine(x.ExtractLinks().Count);
                Console.WriteLine(x.ExtractLinksAsMemory().Count);
            }
        }

        private static void RunMore(BenchmarkHtmlPerformanceKit x)
        {
            for (var i = 0; i < 10; i++)
            {
                x.ExtractTexts();
                x.ExtractTextsAsMemory();
                x.ExtractLinks();
                x.ExtractLinksAsMemory();
            }
        }
    }
}
