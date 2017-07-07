using System.Collections.Generic;
using System.IO;
using System.Reflection;

using HtmlAgilityPack;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlSpeedPack.Test
{
    [TestClass]
    public class LargeDocumentTest
    {
        private readonly Stream stream;

        public LargeDocumentTest()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlSpeedPack.Test.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreaties()
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

            Assert.AreEqual(7376, links.Count);
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreatiesHtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var links = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlAgilityPack.HtmlNodeType.Element && node.Name == "a")
                {
                    var hrefAttributeValue = node.Attributes["href"];
                    if (hrefAttributeValue != null)
                    {
                        links.Add(hrefAttributeValue.Value);
                    }
                }
            }

            Assert.AreEqual(7376, links.Count);
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreaties()
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

            Assert.AreEqual(18618, texts.Count);
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreatiesHtmlAgilityPack()
        {
            stream.Seek(0, SeekOrigin.Begin);

            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            var texts = new List<string>();

            foreach (var node in htmlDocument.DocumentNode.Descendants())
            {
                if (node.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                {
                    texts.Add(node.InnerText);
                }
            }

            Assert.AreEqual(18620, texts.Count);
        }
    }
}