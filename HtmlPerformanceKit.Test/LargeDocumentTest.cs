using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

using HtmlAgilityPack;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class LargeDocumentTest
    {
        private readonly List<HtmlParseErrorEventArgs> parseErrors = new List<HtmlParseErrorEventArgs>();
        private readonly Stream stream;

        public LargeDocumentTest()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Test.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreaties()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var htmlAgilityPackLinks = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
                {
                    var hrefAttributeValue = node.Attributes["href"];
                    if (hrefAttributeValue != null)
                    {
                        htmlAgilityPackLinks.Add(HttpUtility.HtmlDecode(hrefAttributeValue.Value));
                    }
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = HtmlReaderFactory.FromStream(stream, parseErrors);
            var htmlPerformanceKitLinks = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
                {
                    var hrefAttributeValue = htmlReader.GetAttribute("href");
                    if (hrefAttributeValue != null)
                    {
                        htmlPerformanceKitLinks.Add(hrefAttributeValue);
                    }
                }
            }

            CollectionAssert.AreEqual(htmlAgilityPackLinks, htmlPerformanceKitLinks);
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreaties()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var htmlAgilityPackTexts = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Text && node.InnerText != "</form>" && node.InnerText != "")
                {
                    htmlAgilityPackTexts.Add(HttpUtility.HtmlDecode(node.InnerText));
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = HtmlReaderFactory.FromStream(stream, parseErrors);
            var htmlPerformanceKitTexts = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Text)
                {
                    htmlPerformanceKitTexts.Add(htmlReader.Text);
                }
            }

            CollectionAssert.AreEqual(htmlAgilityPackTexts, htmlPerformanceKitTexts);
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = HtmlReaderFactory.FromStream(stream, parseErrors);
            var htmlPerformanceKitLinks = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
                {
                    var hrefAttributeValue = htmlReader.GetAttribute("href");
                    if (hrefAttributeValue != null)
                    {
                        htmlPerformanceKitLinks.Add(hrefAttributeValue);
                    }
                }
            }
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlReader = HtmlReaderFactory.FromStream(stream, parseErrors);
            var htmlPerformanceKitTexts = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Text)
                {
                    htmlPerformanceKitTexts.Add(htmlReader.Text);
                }
            }
        }
    }
}