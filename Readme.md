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
HtmlPerformanceKit is currently about 6-11x faster than AngleSharp, HtmlAgilityPack and CsQuery in my benchmarks. 
This is probably because theese libraries construct a full DOM for the entire document in memory while HtmlPerformanceKit streams tokens as they are read and decoded. 
Theese libraries features a much more user-friendly API and are more battle-tested. If you are not concerned about performance, you should probably use one of those instead. 

HtmlPerformanceKit is currently about 0.5-3x faster than HtmlKit and HtmlParserSharp, which are the closest competitors both in performance and in features.

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

BenchmarkDotNet=v0.10.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-6600U CPU 2.60GHz, ProcessorCount=4
Frequency=2742190 Hz, Resolution=364.6720 ns, Timer=TSC
Host Runtime=Clr 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.7.2110.0
Job Runtime(s):
	Clr 4.0.30319.42000, Arch=32-bit RELEASE


```
Method |        Mean |    StdErr |    StdDev |      Median |
---------------------------- |------------ |---------- |---------- |------------ |
ExtractLinks |  25.5086 ms | 0.2582 ms | 1.4140 ms |  24.8393 ms |
ExtractLinksHtmlAgilityPack | 194.9712 ms | 1.1450 ms | 4.1285 ms | 192.5482 ms |
ExtractLinksAngleSharp | 174.4118 ms | 1.1617 ms | 4.3465 ms | 176.0270 ms |
ExtractLinksCsQuery | 314.7059 ms | 0.7754 ms | 2.9013 ms | 313.8183 ms |
ExtractLinksHtmlParserSharp |  85.0704 ms | 0.7126 ms | 2.5693 ms |  84.5753 ms |
ExtractLinksHtmlKit |  41.9969 ms | 0.1814 ms | 0.7024 ms |  41.8550 ms |
ExtractTexts |  30.1057 ms | 0.1060 ms | 0.3966 ms |  30.0275 ms |
ExtractTextsHtmlAgilityPack | 208.2669 ms | 0.2946 ms | 1.1022 ms | 208.1993 ms |
ExtractTextsAngleSharp | 204.7175 ms | 1.2844 ms | 4.8058 ms | 203.9816 ms |
ExtractTextsCsQuery | 313.0944 ms | 0.6130 ms | 2.2936 ms | 312.8229 ms |
ExtractTextsHtmlParserSharp |  88.8068 ms | 0.2684 ms | 1.0396 ms |  88.3509 ms |
ExtractTextsHtmlKit |  44.6560 ms | 0.0978 ms | 0.3527 ms |  44.6393 ms |



