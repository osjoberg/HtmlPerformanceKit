using System.Collections.Generic;
using System.IO;
using System.Reflection;

using BenchmarkDotNet.Attributes;

using HtmlAgilityPack;

namespace HtmlSpeedPack.Benchmark
{
    public class BenchmarkExtractTexts
    {
        private readonly Stream stream;

        public BenchmarkExtractTexts()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlSpeedPack.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
        }

        [Benchmark]
        public List<string> ThisLib()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = new HtmlReader(new StreamReader(stream));
            var texts = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.NodeType == HtmlNodeType.Text)
                {
                    texts.Add(htmlReader.Text);
                }
            }

            return texts;
        }

        [Benchmark(OperationsPerInvoke = 1)]
        public List<string> HtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var texts = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == global::HtmlAgilityPack.HtmlNodeType.Text)
                {
                    texts.Add(node.InnerText);
                }
            }

            return texts;
        }
    }
}