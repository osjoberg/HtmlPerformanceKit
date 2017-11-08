using System.Collections.Generic;
using System.IO;
using System.Reflection;

using BenchmarkDotNet.Attributes;

namespace HtmlPerformanceKit.Benchmark
{
    public class BenchmarkHtmlPerformanceKit
    {
        private readonly Stream stream;

        public BenchmarkHtmlPerformanceKit()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
        }

        [Benchmark]
        public List<string> ExtractLinks()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = new HtmlReader(new StreamReader(stream));
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
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = new HtmlReader(new StreamReader(stream));
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
