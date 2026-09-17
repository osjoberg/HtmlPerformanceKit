using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test;

[TestClass]
public class ResetTest
{
    private static readonly string[] Documents =
    {
        "<!DOCTYPE html><html lang=\"en\"><head><title>A &amp; B</title></head><body><p class='x' id=y>Hello &lt;World&gt;</p><br/></body></html>",
        "<div>\n  <!-- comment -->\n  <a href=\"https://example.com?a=1&amp;b=2\">Link</a>\n</div>",
        "<script>if (a < b && c > d) { document.write('</div>'); }</script><style>p > a { color: red; }</style>",
        "<textarea>Some &quot;text&quot;</textarea><p>Text with &#x41;&#66; references</p>",
        "<div unclosed=\"value",
        "<p>\r\nLine 2\r\nLine 3</p>",
    };

    [TestMethod]
    public void ResetAfterCompleteDocumentReadsLikeNewReader()
    {
        var parseErrors = new List<string>();
        var reader = CreateReader(string.Empty, parseErrors);

        foreach (var document in Documents)
        {
            parseErrors.Clear();
            reader.Reset(new StringReader(document));

            var actual = ReadTokens(reader, parseErrors);

            CollectionAssert.AreEqual(ReadWithNewReader(document), actual, document);
        }
    }

    [TestMethod]
    public void ResetInTheMiddleOfDocumentReadsLikeNewReader()
    {
        var parseErrors = new List<string>();

        foreach (var abandoned in Documents)
        {
            foreach (var document in Documents)
            {
                // Stop in different states, e.g. inside a raw text element or with an unfinished attribute.
                for (var tokensToRead = 0; tokensToRead < 4; tokensToRead++)
                {
                    var reader = CreateReader(abandoned, parseErrors);

                    for (var i = 0; i < tokensToRead && reader.Read(); i++)
                    {
                    }

                    parseErrors.Clear();
                    reader.Reset(new StringReader(document));

                    var actual = ReadTokens(reader, parseErrors);

                    CollectionAssert.AreEqual(ReadWithNewReader(document), actual, $"{abandoned} -> {document} after {tokensToRead} tokens");
                }
            }
        }
    }

    [TestMethod]
    public void ResetAfterLargeDocumentReadsLikeNewReader()
    {
        var html = ReadLargeDocument();

        var parseErrors = new List<string>();
        var reader = CreateReader(html, parseErrors);

        ReadTokens(reader, parseErrors);

        foreach (var document in Documents)
        {
            parseErrors.Clear();
            reader.Reset(new StringReader(document));

            CollectionAssert.AreEqual(ReadWithNewReader(document), ReadTokens(reader, parseErrors), document);
        }

        parseErrors.Clear();
        reader.Reset(new StringReader(html));

        CollectionAssert.AreEqual(ReadWithNewReader(html), ReadTokens(reader, parseErrors));
    }

    [TestMethod]
    public void ResetWithStream()
    {
        const string document = "<p class=\"a\">Text</p>";

        var parseErrors = new List<string>();
        var reader = CreateReader("<div>", parseErrors);

        reader.Read();
        reader.Reset(new MemoryStream(Encoding.UTF8.GetBytes(document)));

        CollectionAssert.AreEqual(ReadWithNewReader(document), ReadTokens(reader, parseErrors));
    }

    [TestMethod]
    public void ResetClearsCurrentToken()
    {
        var reader = new HtmlReader(new StringReader("<p>"));

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);

        reader.Reset(new StringReader("<p>"));

        Assert.AreEqual(HtmlTokenKind.None, reader.TokenKind);
        Assert.AreEqual(0, reader.LineNumber);
        Assert.AreEqual(0, reader.LinePosition);
        Assert.ThrowsException<InvalidOperationException>(() => reader.Name);
    }

    [TestMethod]
    public void ResetWithNullThrows()
    {
        var reader = new HtmlReader(new StringReader(string.Empty));

        Assert.ThrowsException<ArgumentNullException>(() => reader.Reset((TextReader)null!));
        Assert.ThrowsException<ArgumentNullException>(() => reader.Reset((Stream)null!));
    }

    private static HtmlReader CreateReader(string html, List<string> parseErrors)
    {
        var reader = new HtmlReader(new StringReader(html));
        reader.ParseError += (_, args) => parseErrors.Add($"{args.Message} {args.LineNumber}:{args.LinePosition}");
        return reader;
    }

    private static List<string> ReadWithNewReader(string html)
    {
        var parseErrors = new List<string>();

        return ReadTokens(CreateReader(html, parseErrors), parseErrors);
    }

    private static List<string> ReadTokens(HtmlReader reader, List<string> parseErrors)
    {
        var tokens = new List<string>();

        while (reader.Read())
        {
            var token = new StringBuilder();
            token.Append($"{reader.TokenKind} {reader.LineNumber}:{reader.LinePosition}");

            switch (reader.TokenKind)
            {
                case HtmlTokenKind.Tag:
                case HtmlTokenKind.EndTag:
                case HtmlTokenKind.Doctype:
                    token.Append($" {reader.Name} self-closing={reader.SelfClosingElement}");

                    for (var i = 0; i < reader.AttributeCount; i++)
                    {
                        token.Append($" {reader.GetAttributeName(i)}=\"{reader.GetAttribute(i)}\"");
                    }

                    break;
                case HtmlTokenKind.Text:
                case HtmlTokenKind.Comment:
                    token.Append($" {reader.Text}");
                    break;
            }

            tokens.Add(token.ToString());
        }

        tokens.AddRange(parseErrors);
        return tokens;
    }

    private static string ReadLargeDocument()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HtmlPerformanceKit.Test.en.wikipedia.org_wiki_List_of_Australian_treaties.html") !;
        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}