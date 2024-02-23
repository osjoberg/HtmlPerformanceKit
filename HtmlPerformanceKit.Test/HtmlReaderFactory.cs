using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtmlPerformanceKit.Test
{
    internal class HtmlReaderFactory
    {
        public static HtmlReader FromString(string html, List<HtmlParseErrorEventArgs> parseErrors, HtmlReaderOptions? options = null)
        {
            var reader = new HtmlReader(new MemoryStream(Encoding.UTF8.GetBytes(html)), options);
            reader.ParseError += (_, args) => parseErrors.Add(args);
            return reader;
        }
    }
}
