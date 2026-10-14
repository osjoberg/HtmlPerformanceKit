using BenchmarkDotNet.Running;

namespace HtmlPerformanceKit.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "profile")
        {
            var benchmark = new BenchmarkHtmlPerformanceKit();
            benchmark.IterationSetup();
            benchmark.ExtractTexts();

            return;
        }

        var switcher = new BenchmarkSwitcher(
        [
            typeof(BenchmarkLibraries),
            typeof(BenchmarkHtmlPerformanceKit)
        ]);

        switcher.Run(args);
    }
}