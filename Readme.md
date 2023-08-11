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
HtmlPerformanceKit is currently about 6x-7x faster than AngleSharp and HtmlAgilityPack in my benchmarks. 
This is probably because theese libraries construct a full DOM for the entire document in memory while HtmlPerformanceKit streams tokens as they are read and decoded. 
Theese libraries features a much more user-friendly API and are more battle-tested. If you are not concerned about performance, you should probably use one of those instead. 

HtmlPerformanceKit is currently about 0.3-5x faster than HtmlKit and HtmlParserSharp, which are the closest competitors both in performance and in features.

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

BenchmarkDotNet v0.13.7, Windows 10 (10.0.19045.3324/22H2/2022Update)
AMD Ryzen 7 PRO 4750U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 7.0.400
  [Host]     : .NET 6.0.21 (6.0.2123.36311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.21 (6.0.2123.36311), X64 RyuJIT AVX2


```
|                      Method |      Mean |    Error |   StdDev |
|---------------------------- |----------:|---------:|---------:|
|                ExtractLinks |  17.28 ms | 0.081 ms | 0.076 ms |
| ExtractLinksHtmlAgilityPack | 132.69 ms | 2.631 ms | 3.512 ms |
|      ExtractLinksAngleSharp | 115.23 ms | 2.297 ms | 4.640 ms |
| ExtractLinksHtmlParserSharp |  96.10 ms | 1.903 ms | 1.780 ms |
|         ExtractLinksHtmlKit |  22.53 ms | 0.177 ms | 0.165 ms |
|                ExtractTexts |  21.91 ms | 0.057 ms | 0.051 ms |
| ExtractTextsHtmlAgilityPack | 160.76 ms | 3.298 ms | 9.725 ms |
|      ExtractTextsAngleSharp | 130.07 ms | 2.557 ms | 3.499 ms |
| ExtractTextsHtmlParserSharp |  77.90 ms | 1.537 ms | 2.393 ms |
|         ExtractTextsHtmlKit |  27.61 ms | 0.366 ms | 0.343 ms |



