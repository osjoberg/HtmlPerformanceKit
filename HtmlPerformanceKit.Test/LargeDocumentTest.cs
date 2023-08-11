using HtmlPerformanceKit.Benchmark;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class LargeDocumentTest
    {
        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreaties()
        {
            var benchmark = new BenchmarkLibraries();
            var links = benchmark.ExtractLinks();
            var linksHtmlAgilityPack = benchmark.ExtractLinksHtmlAgilityPack();
            var linksAngleSharp = benchmark.ExtractLinksAngleSharp();
            var linksHtmlParserSharp = benchmark.ExtractLinksHtmlParserSharp();
            var linksHtmlKit = benchmark.ExtractLinksHtmlKit();

            CollectionAssert.AreEqual(links, linksHtmlAgilityPack);
            CollectionAssert.AreEqual(links, linksAngleSharp);
            CollectionAssert.AreEqual(links, linksHtmlParserSharp);
            CollectionAssert.AreEqual(links, linksHtmlKit);
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreaties()
        {
            var benchmark = new BenchmarkLibraries();

            var texts = benchmark.ExtractTexts();
            var textsHtmlAgilityPack = benchmark.ExtractTextsHtmlAgilityPack();
            var textsAngleSharp = benchmark.ExtractTextsAngleSharp();
            var textsHtmlParserSharp = benchmark.ExtractTextsHtmlParserSharp();
            var textsHtmlKit = benchmark.ExtractTextsHtmlKit();

            CollectionAssert.AreEqual(texts, textsHtmlAgilityPack);
            //CollectionAssert.AreEqual(texts, textsAngleSharp);
            //CollectionAssert.AreEqual(texts, textsHtmlParserSharp);
            //CollectionAssert.AreEqual(texts, textsHtmlKit);
        }

        [TestMethod]
        public void ExtractLinksFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            var benchmark = new BenchmarkLibraries();
            var links = benchmark.ExtractLinks();
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            var benchmark = new BenchmarkLibraries();
            var texts = benchmark.ExtractLinks();
        }
    }
}