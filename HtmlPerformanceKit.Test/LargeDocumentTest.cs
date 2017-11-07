using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class LargeDocumentTest
    {
        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreaties()
        {
            var benchmark = new Benchmark.Benchmark();
            var links = benchmark.ExtractLinks();
            var linksHtmlAgilitypack = benchmark.ExtractLinksHtmlAgilityPack();
            var linksAngleSharp = benchmark.ExtractLinksAngleSharp();
            var linksCsQuery = benchmark.ExtractLinksCsQuery();
            var linksHtmlParserSharp = benchmark.ExtractLinksHtmlParserSharp();
            var linksHtmlKit = benchmark.ExtractLinksHtmlKit();

            CollectionAssert.AreEqual(links, linksHtmlAgilitypack);
            CollectionAssert.AreEqual(links, linksAngleSharp);
            CollectionAssert.AreEqual(links, linksCsQuery);
            CollectionAssert.AreEqual(links, linksHtmlParserSharp);
            CollectionAssert.AreEqual(links, linksHtmlKit);
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreaties()
        {
            var benchmark = new Benchmark.Benchmark();

            var texts = benchmark.ExtractTexts();
            var textsHtmlAgilitypack = benchmark.ExtractTextsHtmlAgilityPack();
            var textsAngleSharp = benchmark.ExtractTextsAngleSharp();
            var textsCsQuery = benchmark.ExtractTextsCsQuery();
            var textsHtmlParserSharp = benchmark.ExtractTextsHtmlParserSharp();
            var textsHtmlKit = benchmark.ExtractTextsHtmlKit();

            CollectionAssert.AreEqual(texts, textsHtmlAgilitypack);
            //CollectionAssert.AreEqual(texts, textsAngleSharp);
            //CollectionAssert.AreEqual(texts, textsCsQuery);
            //CollectionAssert.AreEqual(texts, textsHtmlParserSharp);
            //CollectionAssert.AreEqual(texts, textsHtmlKit);
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            var benchmark = new Benchmark.Benchmark();
            var links = benchmark.ExtractLinks();
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            var benchmark = new Benchmark.Benchmark();
            var texts = benchmark.ExtractLinks();
        }
    }
}