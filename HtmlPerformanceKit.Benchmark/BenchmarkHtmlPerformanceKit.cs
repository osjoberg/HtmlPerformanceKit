using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace HtmlPerformanceKit.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkHtmlPerformanceKit
    {
        private readonly Stream stream;
        private readonly StreamReader streamReader;
        private readonly HtmlReaderOptions options = new HtmlReaderOptions { CloseInput = false };

        public BenchmarkHtmlPerformanceKit()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
            streamReader = new StreamReader(stream);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            stream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
        }

        [Benchmark]
        public List<string> ExtractLinks()
        {
            using var htmlReader = new HtmlReader(new StreamReader(stream), options);
            var links = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
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

        [Benchmark]
        public List<string> ExtractTexts()
        {
            using var htmlReader = new HtmlReader(new StreamReader(stream), options);
            var texts = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Text)
                {
                    texts.Add(htmlReader.Text);
                }
            }

            return texts;
        }
    }
}
