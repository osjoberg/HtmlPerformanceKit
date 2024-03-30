using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test;

[TestClass]
public class TagTest
{
    private readonly List<HtmlParseErrorEventArgs> parseErrors = new List<HtmlParseErrorEventArgs>();

    [TestMethod]
    public void Tag()
    {
        var reader = HtmlReaderFactory.FromString("<br/>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("br", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagData()
    {
        var reader = HtmlReaderFactory.FromString("<br/>a", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("br", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("a", reader.Text);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void DataTag()
    {
        var reader = HtmlReaderFactory.FromString("a<br/>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("a", reader.Text);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("br", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void DataTagDataTag()
    {
        var reader = HtmlReaderFactory.FromString("a<br/>b<p/>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("a", reader.Text);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("br", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("b", reader.Text);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("p", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagDataTagData()
    {
        var reader = HtmlReaderFactory.FromString("<br/>a<p/>b", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("br", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("a", reader.Text);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("p", reader.Name);
        Assert.IsTrue(reader.SelfClosingElement);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
        Assert.AreEqual("b", reader.Text);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagDoubleQuotedAttributeValue()
    {
        var reader = HtmlReaderFactory.FromString("<a href=\"javascript:;\">", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
        Assert.IsFalse(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagSingleQuotedAttributeValue()
    {
        var reader = HtmlReaderFactory.FromString("<a href='javascript:;'>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
        Assert.IsFalse(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagUnquotedAttributeValue()
    {
        var reader = HtmlReaderFactory.FromString("<a href=javascript:;>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
        Assert.IsFalse(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagNoAttributeValue()
    {
        var reader = HtmlReaderFactory.FromString("<a href>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.AreEqual("", reader.GetAttribute("href"));
        Assert.IsFalse(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void TagMissingAttribute()
    {
        var reader = HtmlReaderFactory.FromString("<a>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("a", reader.Name);
        Assert.ThrowsException<KeyNotFoundException>(() => reader.GetAttribute("href"));
        Assert.IsFalse(reader.SelfClosingElement);

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueDecimalCharacterReference()
    {
        var reader = HtmlReaderFactory.FromString("<a title=\"&#65;\">", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("A", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueDecimalCharacterReferenceWhenDecodingSkipped()
    {
        var options = new HtmlReaderOptions
        {
            DecodeHtmlCharacters = false
        };

        var reader = HtmlReaderFactory.FromString("<a title=\"&#65;\">", parseErrors, options);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("&#65;", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueHexCharacterReference()
    {
        var reader = HtmlReaderFactory.FromString("<a title=\"&#x41;\">", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("A", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueHexCharacterReferenceWhenDecodingSkipped()
    {
        var options = new HtmlReaderOptions
        {
            DecodeHtmlCharacters = false
        };

        var reader = HtmlReaderFactory.FromString("<a title=\"&#x41;\">", parseErrors, options);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("&#x41;", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueNamedCharacterReference()
    {
        var reader = HtmlReaderFactory.FromString("<a title=\"&lt;\">", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("<", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueNamedCharacterReferenceWhenDecodingSkipped()
    {
        var options = new HtmlReaderOptions
        {
            DecodeHtmlCharacters = false
        };

        var reader = HtmlReaderFactory.FromString("<a title=\"&lt;\">", parseErrors, options);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("&lt;", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueNamedCharacterReferenceSingleQuoted()
    {
        var reader = HtmlReaderFactory.FromString("<a title='&lt;'>", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("<", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void AttributeValueNamedCharacterReferenceSingleQuotedWhenDecodingSkipped()
    {
        var options = new HtmlReaderOptions
        {
            DecodeHtmlCharacters = false
        };

        var reader = HtmlReaderFactory.FromString("<a title='&lt;'>", parseErrors, options);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual("&lt;", reader.GetAttribute("title"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }

    [TestMethod]
    public void GetAttributeReturnsFirstAttributeValue()
    {
        var reader = HtmlReaderFactory.FromString("<img src=\"a\" src=\"b\" />", parseErrors);

        Assert.IsTrue(reader.Read());
        Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
        Assert.AreEqual("img", reader.Name);

        Assert.AreEqual(2, reader.AttributeCount);
        Assert.AreEqual("a", reader.GetAttribute("src"));

        Assert.IsFalse(reader.Read());
        Assert.AreEqual(0, parseErrors.Count);
    }
}