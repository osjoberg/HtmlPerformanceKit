using BenchmarkDotNet.Running;

namespace HtmlPerformanceKit.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(BenchmarkLibraries),
                typeof(BenchmarkHtmlPerformanceKit),
            });

            switcher.Run(args);
        }
    }
}
