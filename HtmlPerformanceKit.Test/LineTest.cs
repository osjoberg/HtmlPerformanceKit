using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test;

[TestClass]
public class LineTest
{
    private readonly List<HtmlParseErrorEventArgs> parseErrors = new();

    [TestMethod]
    public void TagBeginning()
    {
        var reader = HtmlReaderFactory.FromString("<a>a", parseErrors);

        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(3, reader.LinePosition);
    }

    [TestMethod]
    public void EndTagBeginning()
    {
        var reader = HtmlReaderFactory.FromString("</a>a", parseErrors);

        reader.Read();
        Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(4, reader.LinePosition);
    }

    [TestMethod]
    public void TextBeginning()
    {
        var reader = HtmlReaderFactory.FromString("&amp;<a>", parseErrors);

        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("&", reader.Text);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(6, reader.LinePosition);
    }

    [TestMethod]
    public void TagMiddle()
    {
        var reader = HtmlReaderFactory.FromString("a<a>a", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(4, reader.LinePosition);
    }

    [TestMethod]
    public void EndTagMiddle()
    {
        var reader = HtmlReaderFactory.FromString("a</a>a", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(5, reader.LinePosition);
    }

    [TestMethod]
    public void TextMiddle()
    {
        var reader = HtmlReaderFactory.FromString("<a>&amp;</a>", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("&", reader.Text);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(9, reader.LinePosition);
    }

    [TestMethod]
    public void TagEnd()
    {
        var reader = HtmlReaderFactory.FromString("a<a>", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(4, reader.LinePosition);
    }

    [TestMethod]
    public void EndTagEnd()
    {
        var reader = HtmlReaderFactory.FromString("a</a>", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(5, reader.LinePosition);
    }

    [TestMethod]
    public void TextEnd()
    {
        var reader = HtmlReaderFactory.FromString("<a>&amp;", parseErrors);

        reader.Read();
        reader.Read();
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("&", reader.Text);
        Assert.AreEqual(1, reader.LineNumber);
        Assert.AreEqual(8, reader.LinePosition);
    }
}