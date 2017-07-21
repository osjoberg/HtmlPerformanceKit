using BenchmarkDotNet.Running;

namespace HtmlPerformanceKit.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}
