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
HtmlPerformanceKit is currently about 5-9x faster than HtmlAgilityPack, AngleSharp and CsQuery in my benchmarks. 
This is probably because theese libraries construct a full DOM for the entire document in memory while HtmlPerformanceKit is the tokens as they are read. 
HtmlAgilityPack has a much more user-friendly API and is more battle-tested. If you are not concerned about performance, you should probably use one of theese libraries instead. 

HtmlPerformanceKit is currentlly about 3x faster than HtmlParserSharp, which is the closest competitor both in performance and in features.

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
Frequency=2742188 Hz, Resolution=364.6723 ns, Timer=TSC
Host Runtime=Clr 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.7.2101.1
Job Runtime(s):
	Clr 4.0.30319.42000, Arch=32-bit RELEASE


```
|                      Method |        Mean |    StdErr |     StdDev |      Median |
|---------------------------- |------------ |---------- |----------- |------------ |
| ExtractLinks |  30.5040 ms | 0.3022 ms |  2.0936 ms |  29.8002 ms |
| ExtractLinksHtmlAgilityPack | 257.5180 ms | 2.5666 ms | 22.0788 ms | 250.6009 ms |
| ExtractLinksAngleSharp | 199.8211 ms | 1.9656 ms | 14.5770 ms | 194.6425 ms |
| ExtractLinksCsQuery | 346.4787 ms | 3.4372 ms | 15.3718 ms | 342.9341 ms |
| ExtractLinksHtmlParserSharp |  92.4987 ms | 1.1794 ms |  5.5320 ms |  89.6711 ms |
| ExtractTexts |  32.6740 ms | 0.0918 ms |  0.3554 ms |  32.6529 ms |
| ExtractTextsHtmlAgilityPack | 266.3874 ms | 2.6395 ms | 19.0337 ms | 260.1867 ms |
| ExtractTextsAngleSharp | 219.3714 ms | 1.4480 ms |  5.6079 ms | 218.1780 ms |
| ExtractTextsCsQuery | 344.4637 ms | 3.3692 ms | 14.2943 ms | 336.0718 ms |
| ExtractTextsHtmlParserSharp |  94.9927 ms | 0.3013 ms |  1.1669 ms |  95.0740 ms |

