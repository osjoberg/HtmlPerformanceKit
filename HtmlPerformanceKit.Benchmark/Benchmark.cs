using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using AngleSharp.Dom;
using AngleSharp.Parser.Html;

using BenchmarkDotNet.Attributes;

using CsQuery;

using HtmlAgilityPack;

using NodeType = AngleSharp.Dom.NodeType;

namespace HtmlPerformanceKit.Benchmark
{
    public class Benchmark
    {
        private Stream stream;

        public Benchmark()
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
        public List<string> ExtractLinksHtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var links = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
                {
                    var hrefAttributeValue = node.Attributes["href"];
                    if (hrefAttributeValue != null)
                    {
                        links.Add(HttpUtility.HtmlDecode(hrefAttributeValue.Value));
                    }
                }
            }

            return links;
        }

        [Benchmark]
        public List<string> ExtractLinksAngleSharp()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlParser = new HtmlParser(new HtmlParserOptions { });
            var document = htmlParser.Parse(stream);


            var links = new List<string>();

            foreach (var node in document.All)
            {
                if (node.NodeType == NodeType.Element && node.LocalName == "a")
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

        [Benchmark]
        public List<string> ExtractLinksCsQuery()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
            stream.Seek(0, SeekOrigin.Begin);

            var dom = CQ.Create(stream);            

            var links = new List<string>();

            foreach (var element in dom.Document.QuerySelectorAll("*"))
            {
                if (element.NodeType == CsQuery.NodeType.ELEMENT_NODE && element.NodeName == "A")
                {
                    var hrefAttributeValue = element.Attributes["href"];
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

        [Benchmark]
        public List<string> ExtractTextsHtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var texts = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Text && node.InnerText != "" && node.InnerText != "</form>")
                {
                    texts.Add(HttpUtility.HtmlDecode(node.InnerText));
                }
            }

            return texts;
        }

        [Benchmark]
        public List<string> ExtractTextsAngleSharp()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlParser = new HtmlParser();
            var document = htmlParser.Parse(stream);

            var texts = new List<string>();

            foreach (var node in document.QuerySelectorAll("*"))
            {
                foreach (var childNode in node.ChildNodes.OfType<IText>())
                {
                    texts.Add(childNode.Text);
                }
            }

            return texts;
        }

        [Benchmark]
        public List<string> ExtractTextsCsQuery()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
            stream.Seek(0, SeekOrigin.Begin);

            var dom = CQ.Create(stream);

            var texts = new List<string>();

            foreach (var node in dom.Document.QuerySelectorAll("*"))
            {
                foreach (var childNode in node.ChildNodes.OfType<IText>())
                {
                    texts.Add(childNode.Text);
                }
            }

            return texts;
        }
    }
}