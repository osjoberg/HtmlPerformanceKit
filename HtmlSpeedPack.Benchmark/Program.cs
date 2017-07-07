using BenchmarkDotNet.Running;

namespace HtmlSpeedPack.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkExtractLinks>();
            BenchmarkRunner.Run<BenchmarkExtractTexts>();
        }
    }
}
