using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtmlPerformanceKit.Test
{
    internal class HtmlReaderFactory
    {
        public static HtmlReader FromString(string html, List<HtmlParseErrorEventArgs> parseErrors, HtmlReaderOptions options = null)
        {
            var reader = new HtmlReader(new MemoryStream(Encoding.UTF8.GetBytes(html)), options);
            reader.ParseError += (sender, args) => parseErrors.Add(args);
            return reader;
        }

        public static HtmlReader FromStream(Stream stream, List<HtmlParseErrorEventArgs> parseErrors, HtmlReaderOptions options = null)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new HtmlReader(stream, options);
            reader.ParseError += (sender, args) => parseErrors.Add(args);
            return reader;
        }
    }
}
