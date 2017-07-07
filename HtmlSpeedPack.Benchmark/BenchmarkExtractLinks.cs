using System.Collections.Generic;
using System.IO;
using System.Reflection;

using BenchmarkDotNet.Attributes;

using HtmlAgilityPack;

namespace HtmlSpeedPack.Benchmark
{
    public class BenchmarkExtractLinks
    {
        private readonly Stream stream;

        public BenchmarkExtractLinks()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlSpeedPack.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
        }

        [Benchmark]
        public List<string> ThisLib()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = new HtmlReader(new StreamReader(stream));
            var links = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.NodeType == HtmlNodeType.Tag && htmlReader.Name == "a")
                {
                    var hrefAttributeValue = htmlReader.GetAttribute("href");
                    if (hrefAttributeValue != null)
                    {
                        links.Add(hrefAttributeValue);
                    }
                }
            }

            return links;
        }

        [Benchmark(OperationsPerInvoke = 1)]
        public List<string> HtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var links = new List<string>();

            foreach (HtmlNode node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == global::HtmlAgilityPack.HtmlNodeType.Element && node.Name == "a")
                {
                    var hrefAttributeValue = node.Attributes["href"];
                    if (hrefAttributeValue != null)
                    {
                        links.Add(hrefAttributeValue.Value);
                    }
                }
            }

            return links;
        }

    }
}