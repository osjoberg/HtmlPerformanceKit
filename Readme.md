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
                var hrefAttributeValue = htmlReader.GetAttribute("href");
                if (hrefAttributeValue != null)
                {
                    yield return hrefAttributeValue;
                }
            }
        }
    }
}
```

## Benchmarks
Benchmarks are extracting links and texts from a large Wikipedia article, List of Australian treaties, https://en.wikipedia.org/wiki/List_of_Australian_treaties (1.7MB)

``` ini

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4046/22H2/2022Update)
AMD Ryzen 7 PRO 4750U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  Job-USNJHW : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2

```

| Method                      | Mean      | Error    | StdDev   | Median    | Allocated |
|---------------------------- |----------:|---------:|---------:|----------:|----------:|
| ExtractLinks                |  10.99 ms | 0.220 ms | 0.413 ms |  10.76 ms |   1.37 MB |
| ExtractLinksHtmlKit         |  15.10 ms | 0.293 ms | 0.371 ms |  14.97 ms |  13.95 MB |
| ExtractLinksHtmlParserSharp |  50.12 ms | 1.002 ms | 2.024 ms |  49.97 ms |  19.01 MB |
| ExtractLinksAngleSharp      |  64.21 ms | 1.358 ms | 4.003 ms |  63.22 ms |  20.63 MB |
| ExtractLinksHtmlAgilityPack | 135.77 ms | 2.715 ms | 6.292 ms | 134.53 ms |  52.66 MB |
| ExtractTexts                |  11.22 ms | 0.224 ms | 0.658 ms |  10.92 ms |   2.82 MB |
| ExtractTextsHtmlKit         |  17.18 ms | 0.343 ms | 0.574 ms |  17.25 ms |  13.76 MB |
| ExtractTextsHtmlParserSharp |  54.04 ms | 0.879 ms | 0.734 ms |  53.74 ms |  20.99 MB |
| ExtractTextsAngleSharp      |  86.79 ms | 1.696 ms | 2.019 ms |  86.61 ms |  25.73 MB |
| ExtractTextsHtmlAgilityPack | 154.84 ms | 3.068 ms | 5.293 ms | 155.50 ms |  77.48 MB |
