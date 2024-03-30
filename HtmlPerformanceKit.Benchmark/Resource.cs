using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlPerformanceKit.Benchmark;

internal class Resource
{
    internal static Stream GetResourceStream(int bodyCount = 1)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var inputStream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html") !;
        if (bodyCount == 1)
        {
            return inputStream;
        }

        var inputHtml = new StreamReader(inputStream).ReadToEnd();

        var bodyStart = Regex.Match(inputHtml, "<body[^>]+>");
        var bodyEnd = Regex.Match(inputHtml, "</body>");

        var beforeBody = inputHtml.Substring(bodyStart.Index + bodyStart.Length);
        var body = inputHtml.Substring(bodyStart.Index + bodyStart.Length, bodyEnd.Index - bodyStart.Index - bodyStart.Length);
        var afterBody = inputHtml.Substring(bodyEnd.Index);

        var stream = new MemoryStream();
        stream.Write(Encoding.UTF8.GetBytes(beforeBody), 0, beforeBody.Length);
        for (var repeat = 0; repeat < bodyCount; repeat++)
        {
            stream.Write(Encoding.UTF8.GetBytes(body), 0, body.Length);
        }

        stream.Write(Encoding.UTF8.GetBytes(afterBody));

        stream.Position = 0;
        return stream;
    }
}