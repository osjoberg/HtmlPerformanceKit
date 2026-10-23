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
HtmlPerformanceKit can currently decode HTML documents in faster than 500 MB/s on a normal laptop CPU which is currently about 12x-40x faster than AngleSharp and HtmlAgilityPack in my benchmarks. 
This is probably because theese libraries construct a full DOM for the entire document in memory while HtmlPerformanceKit streams tokens as they are read and decoded. 
Theese libraries features a much more user-friendly API and are more battle-tested. If you are not concerned about performance, you should probably use one of those instead. 

HtmlPerformanceKit is currently about 1.6x faster than HtmlKit and 8x faster than HtmlParserSharp while using 5x less memory, which are the closest competitors both in performance and in features.

With the `AsMemory` suffixed properties and methods you can process large documents while only using a few kilobytes of memory for internal buffers further improving performance. If many HTML documents are to be parsed in one go, you can use `HtmlReaderPool` to reduce allocations even further by reusing internal buffers.

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
Benchmarks are extracting links and texts from a large Wikipedia article, List of Australian treaties, https://en.wikipedia.org/wiki/List_of_Australian_treaties (1.75MB). The HTML body is duplicated 30 times to get longer and more consistent benchmarks.

``` ini

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9711/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  Job-CNUJVU : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

```

| Method                                 | Mean       | Median     | Allocated     |
|--------------------------------------- |-----------:|-----------:|--------------:|
| ExtractLinksHtmlPerformanceKit         |   141.1 ms |   139.5 ms |   37525.42 KB |
| ExtractLinksHtmlPerformanceKitAsMemory |   101.2 ms |   101.1 ms |      69.43 KB |
| ExtractLinksHtmlAgilityPack            | 4,219.7 ms | 4,172.2 ms | 1258305.61 KB |
| ExtractLinksAngleSharp                 | 1,933.5 ms | 1,925.8 ms |  740862.41 KB |
| ExtractLinksHtmlParserSharp            | 1,691.4 ms | 1,693.8 ms |  573443.72 KB |
| ExtractLinksHtmlKit                    |   247.2 ms |   248.2 ms |  436337.41 KB |
| ExtractTextsHtmlPerformanceKit         |   212.4 ms |   207.5 ms |   71097.55 KB |
| ExtractTextsHtmlPerformanceKitAsMemory |   103.0 ms |   103.3 ms |      69.43 KB |
| ExtractTextsHtmlAgilityPack            | 3,597.3 ms | 3,535.8 ms | 2029971.43 KB |
| ExtractTextsAngleSharp                 | 2,523.3 ms | 2,538.5 ms |   882017.4 KB |
| ExtractTextsHtmlParserSharp            | 1,733.9 ms | 1,725.2 ms |  623894.48 KB |
| ExtractTextsHtmlKit                    |   342.5 ms |   340.0 ms |  418494.22 KB |
