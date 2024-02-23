using System;
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

            for (;;)
            {
                var read = htmlReader.Read();

                output.Add(new
                {
                    Read = read,
                    htmlReader.LineNumber,
                    htmlReader.LinePosition,
                    TokenKind = htmlReader.TokenKind.ToString(),
                    AttributeCount = Evaluate(() => htmlReader.AttributeCount),
                    Name = Evaluate(() => htmlReader.Name),
                    SelfClosingElement = Evaluate(() => htmlReader.SelfClosingElement),
                    Text = Evaluate(() => htmlReader.Text),
                    Attributes = htmlReader.TokenKind == HtmlTokenKind.Tag || htmlReader.TokenKind == HtmlTokenKind.EndTag || htmlReader.TokenKind == HtmlTokenKind.Doctype ? Enumerable.Range(0, htmlReader.AttributeCount).ToDictionary(index => htmlReader.GetAttributeName(index), index => htmlReader.GetAttribute(index)) : null,
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
            benchmark.IterationSetup();
            var links = benchmark.ExtractLinksHtmlPerformanceKit();
            benchmark.IterationSetup();
            var linksHtmlAgilityPack = benchmark.ExtractLinksHtmlAgilityPack();
            benchmark.IterationSetup();
            var linksAngleSharp = benchmark.ExtractLinksAngleSharp();
            benchmark.IterationSetup();
            var linksHtmlParserSharp = benchmark.ExtractLinksHtmlParserSharp();
            benchmark.IterationSetup();
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
            benchmark.IterationSetup();
            var texts = benchmark.ExtractTextsHtmlPerformanceKit();
            benchmark.IterationSetup();
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
            var links = benchmark.ExtractLinksHtmlPerformanceKit();
        }

        [TestMethod]
        public void ExtractTextFromWikipediaListOfAustralianTreatiesHtmlPerformanceKit()
        {
            var benchmark = new BenchmarkLibraries();
            var texts = benchmark.ExtractTextsHtmlPerformanceKit();
        }

        private static string Evaluate<T>(Func<T> func)
        {
            try
            {
                return func().ToString();
            }
            catch (Exception e)
            {
                return $"<{e.GetType().Name}>";
            }
        }
    }
}