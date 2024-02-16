using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using HtmlAgilityPack;

using HtmlKit;

using HtmlParserSharp;

using NodeType = AngleSharp.Dom.NodeType;

namespace HtmlPerformanceKit.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkLibraries
    {
        private readonly Stream stream;
        private readonly StreamReader streamReader;

        public BenchmarkLibraries()
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
            var htmlReader = new HtmlReader(streamReader);
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
            var htmlParser = new HtmlParser();
            var document = htmlParser.ParseDocument(stream);

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
        public List<string> ExtractLinksHtmlParserSharp()
        {
            var links = new List<string>();

            var simpleHtmlparser = new SimpleHtmlParser();
            var document = simpleHtmlparser.Parse(new StreamReader(stream));
            var memoryStream = new MemoryStream();
            document.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.Name != "a")
                {
                    continue;
                }

                var hrefAttributeValue = reader.GetAttribute("href");
                if (hrefAttributeValue == null)
                {
                    continue;
                }

                links.Add(hrefAttributeValue);
            }

            return links;
        }

        [Benchmark]
        public List<string> ExtractLinksHtmlKit()
        {
            var htmlTokenizer = new HtmlKit.HtmlTokenizer(new StreamReader(stream));

            var links = new List<string>();

            while (htmlTokenizer.ReadNextToken(out var token))
            {
                if (token.Kind != HtmlKit.HtmlTokenKind.Tag)
                {
                    continue;
                }

                var dataToken = (HtmlTagToken)token;
                if (dataToken.Name != "a")
                {
                    continue;
                }

                foreach (var attribute in dataToken.Attributes)
                {
                    if (attribute.Name != "href")
                    {
                        continue;
                    }

                    links.Add(attribute.Value);
                    break;
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
            var htmlParser = new HtmlParser();
            var document = htmlParser.ParseDocument(stream);

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
        public List<string> ExtractTextsHtmlParserSharp()
        {
            var simpleHtmlparser = new SimpleHtmlParser();
            var document = simpleHtmlparser.Parse(new StreamReader(stream));
            var memoryStream = new MemoryStream();
            document.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var texts = new List<string>();

            var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.Whitespace)
                {
                    continue;
                }

                var value = reader.Value;
                if (value == "")
                {
                    continue;
                }

                texts.Add(reader.Value);
            }

            return texts;
        }

        [Benchmark]
        public List<string> ExtractTextsHtmlKit()
        {
            var htmlTokenizer = new HtmlKit.HtmlTokenizer(new StreamReader(stream));

            var texts = new List<string>();

            while (htmlTokenizer.ReadNextToken(out var token))
            {
                if (token.Kind != HtmlKit.HtmlTokenKind.Data && token.Kind != HtmlKit.HtmlTokenKind.ScriptData && token.Kind != HtmlKit.HtmlTokenKind.CData)
                {
                    continue;
                }

                var dataToken = (HtmlDataToken)token;
                texts.Add(dataToken.Data);
            }

            return texts;
        }
    }
}