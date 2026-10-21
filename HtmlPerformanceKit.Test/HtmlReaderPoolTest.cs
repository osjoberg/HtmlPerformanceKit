using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test;

[TestClass]
public class HtmlReaderPoolTest
{
    [TestMethod]
    public void ResetTest()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Test.en.wikipedia.org_wiki_List_of_Australian_treaties.html")!;

        var expected = ReadDocument(new HtmlReader(stream), int.MaxValue);

        var htmlReaderPool = new HtmlReaderPool();

        // Test with += 1 works, but takes over 2h to complete.
        for (var iteration = 1; iteration < 60000; iteration += 1000)
        {
            stream.Position = 0;
            var htmlReader = htmlReaderPool.Rent(stream);

            var actual = ReadDocument(htmlReader, iteration);

            htmlReaderPool.Return(htmlReader);

            CollectionAssert.AreEqual(expected.Take(actual.Length).ToArray(), actual);
        }
    }

    private static string[] ReadDocument(HtmlReader htmlReader, int iterations)
    {
        var result = new List<string>();

        void OnParseError(object? sender, HtmlParseErrorEventArgs args) => result.Add($"Error: {args.Message} at {args.LineNumber}:{args.LinePosition}");

        htmlReader.ParseError += OnParseError;

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var read = htmlReader.Read();
            result.Add(
            $"""
                Read = {read}, 
                LineNumber = {htmlReader.LineNumber}, 
                LinePosition = {htmlReader.LinePosition}, 
                TokenKind = {htmlReader.TokenKind.ToString()}, 
                AttrbuteCount = {Evaluate(() => htmlReader.AttributeCount)}, 
                Name = {Evaluate(() => htmlReader.Name)}, 
                SelfClosing = {Evaluate(() => htmlReader.SelfClosingElement)}, 
                Text = {Evaluate(() => htmlReader.Text)}, 
                Attributes = {(htmlReader.TokenKind == HtmlTokenKind.Tag || htmlReader.TokenKind == HtmlTokenKind.EndTag || htmlReader.TokenKind == HtmlTokenKind.Doctype ? string.Concat(Enumerable.Range(0, htmlReader.AttributeCount).Select(index => htmlReader.GetAttributeName(index) + ": " + htmlReader.GetAttribute(index))) : "")}");
            """);

            if (read == false)
            {
                break;
            }
        }

        htmlReader.ParseError -= OnParseError;

        return result.ToArray();
    }

    private static string Evaluate<T>(Func<T> func)
    {
        try
        {
            return func()!.ToString()!;
        }
        catch (Exception e)
        {
            return $"<{e.GetType().Name}>";
        }
    }
}