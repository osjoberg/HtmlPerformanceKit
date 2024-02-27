using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace HtmlPerformanceKit.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkHtmlPerformanceKit
    {
        private readonly Stream stream;
        private readonly StreamReader streamReader;
        private readonly List<string> result = new List<string>(1_000_000);
        private readonly StringBuilder memoryResult = new StringBuilder(20_000_000);

        public BenchmarkHtmlPerformanceKit()
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
        public List<string> ExtractLinks()
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
        public StringBuilder ExtractLinksAsMemory()
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
        public List<string> ExtractTexts()
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
        public StringBuilder ExtractTextsAsMemory()
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
    }
}
