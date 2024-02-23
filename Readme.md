# HtmlPerformanceKit
HtmlPerformanceKit is a fast HTML parser whith a similar API as XmlReader. HtmlPerformanceKit is implemented according to the HTML5 tokenization specification:
https://www.w3.org/TR/html5/syntax.html#tokenization

## Install via NuGet
To install HtmlPerformanceKit, run the following command in the Package Manager Console:

```cmd
PM> Install-Package HtmlPerformanceKit
```
You can also view the package page on [Nuget](https://www.nuget.org/packages/HtmlPerformanceKit/).

## Performance
HtmlPerformanceKit is currently about 6x-15x faster than AngleSharp and HtmlAgilityPack in my benchmarks. 
This is probably because theese libraries construct a full DOM for the entire document in memory while HtmlPerformanceKit streams tokens as they are read and decoded. 
Theese libraries features a much more user-friendly API and are more battle-tested. If you are not concerned about performance, you should probably use one of those instead. 

HtmlPerformanceKit is currently about 0.5x faster than HtmlKit and 5x faster than HtmlParserSharp while using 5x less memory, which are the closest competitors both in performance and in features.

With the `AsMemory` suffixed properties and methods you can process large documents while only using a few kilobytes of memory for internal buffers.

## Example usage
```csharp
public IEnumerable<string> ExtractLinks(string filename)
{
    using (var htmlReader = new HtmlReader(File.OpenRead(filename)))
    {
        while (htmlReader.Read())
        {
            if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
            {                
                if (htmlReader.GetAttribute("href", out var hrefAttributeValue))
                {
                    yield return hrefAttributeValue;
                }
            }
        }
    }
}
```

## Benchmarks
Benchmarks are extracting links and texts from a large Wikipedia article, List of Australian treaties, https://en.wikipedia.org/wiki/List_of_Australian_treaties (1.7MB). The HTML body is duplicated 10 times to get longer and more consistent benchmarks.

``` ini

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
AMD Ryzen 7 PRO 4750U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-DPXRAR : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2

```

| Method                                 | Mean       | Error    | StdDev   | Median     | Allocated    |
|--------------------------------------- |-----------:|---------:|---------:|-----------:|-------------:|
| ExtractLinksHtmlPerformanceKit         |   130.7 ms |  0.23 ms |  0.19 ms |   130.6 ms |  13353.07 KB |
| ExtractLinksHtmlPerformanceKitAsMemory |   105.3 ms |  0.14 ms |  0.12 ms |   105.3 ms |     62.23 KB |
| ExtractLinksHtmlAgilityPack            | 1,848.7 ms | 36.54 ms | 80.22 ms | 1,895.6 ms | 576739.72 KB |
| ExtractLinksAngleSharp                 |   959.9 ms | 19.11 ms | 37.27 ms |   984.2 ms | 294108.93 KB |
| ExtractLinksHtmlParserSharp            |   713.2 ms | 14.13 ms | 22.81 ms |   707.2 ms | 222542.52 KB |
| ExtractLinksHtmlKit                    |   184.3 ms |  1.39 ms |  1.30 ms |   184.5 ms | 154834.69 KB |
| ExtractTextsHtmlPerformanceKit         |   150.4 ms |  0.74 ms |  0.66 ms |   150.2 ms |   25265.7 KB |
| ExtractTextsHtmlPerformanceKitAsMemory |   112.7 ms |  1.36 ms |  1.27 ms |   112.2 ms |     62.23 KB |
| ExtractTextsHtmlAgilityPack            | 1,770.6 ms |  8.56 ms |  8.01 ms | 1,770.1 ms |  850558.3 KB |
| ExtractTextsAngleSharp                 | 1,201.6 ms | 23.77 ms | 30.06 ms | 1,216.6 ms | 344377.57 KB |
| ExtractTextsHtmlParserSharp            |   721.4 ms |  2.63 ms |  2.20 ms |   721.2 ms | 240448.28 KB |
| ExtractTextsHtmlKit                    |   232.7 ms |  2.12 ms |  1.77 ms |   233.1 ms | 148503.51 KB |
