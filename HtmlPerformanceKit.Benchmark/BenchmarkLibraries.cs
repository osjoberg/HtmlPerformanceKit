using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using HtmlAgilityPack;

using HtmlKit;

using HtmlParserSharp;

using NodeType = AngleSharp.Dom.NodeType;

namespace HtmlPerformanceKit.Benchmark;

[MemoryDiagnoser]
public class BenchmarkLibraries
{
    private readonly Stream stream;
    private readonly StreamReader streamReader;
    private readonly List<string> result = new List<string>(100_000);
    private readonly StringBuilder memoryResult = new StringBuilder(2_000_000);

    public BenchmarkLibraries()
    {
        stream = Resource.GetResourceStream(10);
        streamReader = new StreamReader(stream);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        stream.Seek(0, SeekOrigin.Begin);
        streamReader.DiscardBufferedData();
        result.Clear();
        memoryResult.Clear();
    }

    [Benchmark]
    public List<string> ExtractLinksHtmlPerformanceKit()
    {
        var htmlReader = new HtmlReader(streamReader);

        while (htmlReader.Read())
        {
            if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
            {
                if (htmlReader.TryGetAttribute("href", out var hrefAttributeValue))
                {
                    result.Add(hrefAttributeValue);
                }
            }
        }

        return result;
    }

    [Benchmark]
    public StringBuilder ExtractLinksHtmlPerformanceKitAsMemory()
    {
        var htmlReader = new HtmlReader(streamReader);

        while (htmlReader.Read())
        {
            if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.NameAsMemory.Equals("a".AsMemory()))
            {
                if (htmlReader.TryGetAttributeAsMemory("href", out var hrefAttributeValue))
                {
                    memoryResult.Append(hrefAttributeValue);
                }
            }
        }

        return memoryResult;
    }

    [Benchmark]
    public List<string> ExtractLinksHtmlAgilityPack()
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.Load(stream);

        foreach (var node in htmlDocument.DocumentNode.Descendants())
        {
            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
            {
                var hrefAttributeValue = node.Attributes["href"];
                if (hrefAttributeValue != null)
                {
                    result.Add(HttpUtility.HtmlDecode(hrefAttributeValue.Value));
                }
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractLinksAngleSharp()
    {
        var htmlParser = new HtmlParser();
        var document = htmlParser.ParseDocument(stream);

        foreach (var node in document.All)
        {
            if (node.NodeType == NodeType.Element && node.LocalName == "a")
            {
                var hrefAttributeValue = node.Attributes["href"];
                if (hrefAttributeValue != null)
                {
                    result.Add(hrefAttributeValue.Value);
                }
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractLinksHtmlParserSharp()
    {
        var simpleHtmlParser = new SimpleHtmlParser();
        var document = simpleHtmlParser.Parse(streamReader);
        var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            if (reader.Name != "a")
            {
                continue;
            }

            var hrefAttributeValue = reader.GetAttribute("href");
            if (hrefAttributeValue == null)
            {
                continue;
            }

            result.Add(hrefAttributeValue);
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractLinksHtmlKit()
    {
        var htmlTokenizer = new HtmlKit.HtmlTokenizer(streamReader);

        while (htmlTokenizer.ReadNextToken(out var token))
        {
            if (token.Kind != HtmlKit.HtmlTokenKind.Tag)
            {
                continue;
            }

            var dataToken = (HtmlTagToken)token;
            if (dataToken.Name != "a")
            {
                continue;
            }

            foreach (var attribute in dataToken.Attributes)
            {
                if (attribute.Name != "href")
                {
                    continue;
                }

                result.Add(attribute.Value);
                break;
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractTextsHtmlPerformanceKit()
    {
        var htmlReader = new HtmlReader(streamReader);

        while (htmlReader.Read())
        {
            if (htmlReader.TokenKind == HtmlTokenKind.Text)
            {
                result.Add(htmlReader.Text);
            }
        }

        return result;
    }

    [Benchmark]
    public StringBuilder ExtractTextsHtmlPerformanceKitAsMemory()
    {
        var htmlReader = new HtmlReader(streamReader);

        while (htmlReader.Read())
        {
            if (htmlReader.TokenKind == HtmlTokenKind.Text)
            {
                memoryResult.Append(htmlReader.TextAsMemory);
            }
        }

        return memoryResult;
    }

    [Benchmark]
    public List<string> ExtractTextsHtmlAgilityPack()
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.Load(stream);

        foreach (var node in htmlDocument.DocumentNode.Descendants())
        {
            if (node.NodeType == HtmlNodeType.Text && node.InnerText != "" && node.InnerText != "</form>")
            {
                result.Add(HttpUtility.HtmlDecode(node.InnerText));
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractTextsAngleSharp()
    {
        var htmlParser = new HtmlParser();
        var document = htmlParser.ParseDocument(stream);

        foreach (var node in document.QuerySelectorAll("*"))
        {
            foreach (var childNode in node.ChildNodes.OfType<IText>())
            {
                result.Add(childNode.Text);
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractTextsHtmlParserSharp()
    {
        var simpleHtmlParser = new SimpleHtmlParser();
        var document = simpleHtmlParser.Parse(streamReader);
        var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.Whitespace)
            {
                continue;
            }

            var value = reader.Value;
            if (value == "")
            {
                continue;
            }

            result.Add(reader.Value);
        }

        return result;
    }

    [Benchmark]
    public List<string> ExtractTextsHtmlKit()
    {
        var htmlTokenizer = new HtmlKit.HtmlTokenizer(streamReader);

        while (htmlTokenizer.ReadNextToken(out var token))
        {
            if (token.Kind != HtmlKit.HtmlTokenKind.Data && token.Kind != HtmlKit.HtmlTokenKind.ScriptData && token.Kind != HtmlKit.HtmlTokenKind.CData)
            {
                continue;
            }

            var dataToken = (HtmlDataToken)token;
            result.Add(dataToken.Data);
        }

        return result;
    }
}