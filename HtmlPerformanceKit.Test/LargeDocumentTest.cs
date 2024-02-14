using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HtmlPerformanceKit.Benchmark;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class LargeDocumentTest : VerifyBase
    {
        [TestMethod]
        public Task WikipediaListOfAustralianTreatiesApiTest()
        {
            var output = new List<object>();

            var executingAssembly = Assembly.GetExecutingAssembly();

            var inputStream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Test.en.wikipedia.org_wiki_List_of_Australian_treaties.html")!;

            var htmlReader = new HtmlReader(inputStream);
            htmlReader.ParseError += (_, args) => output.Add(args);

            for (; ; )
            {
                var read = htmlReader.Read();

                output.Add(new
                {
                    Read = read,
                    htmlReader.AttributeCount,
                    htmlReader.LineNumber,
                    htmlReader.LinePosition,
                    htmlReader.Name,
                    htmlReader.SelfClosingElement,
                    htmlReader.Text,
                    TokenKind = htmlReader.TokenKind.ToString(),
                    Attributes = Enumerable.Range(0, htmlReader.AttributeCount).ToDictionary(index => htmlReader.GetAttributeName(index), index => htmlReader.GetAttribute(index)),
                });

                if (read == false)
                {
                    break;
                }
            }

            return Verify(output);
        }


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
            var texts = benchmark.ExtractTexts();
        }
    }
}