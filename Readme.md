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

HtmlPerformanceKit is currently about 3x faster than HtmlParserSharp, which is the closest competitor both in performance and in features.

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
                      Method |        Mean |    StdDev |      Median |
---------------------------- |------------ |---------- |------------ |
                ExtractLinks |  25.2016 ms | 0.2778 ms |  25.1799 ms |
 ExtractLinksHtmlAgilityPack | 190.5365 ms | 4.4316 ms | 189.4746 ms |
      ExtractLinksAngleSharp | 177.6399 ms | 7.2088 ms | 176.1924 ms |
         ExtractLinksCsQuery | 312.1999 ms | 4.1638 ms | 310.6307 ms |
 ExtractLinksHtmlParserSharp |  84.3848 ms | 2.6910 ms |  83.4875 ms |
                ExtractTexts |  29.5441 ms | 0.2491 ms |  29.4681 ms |
 ExtractTextsHtmlAgilityPack | 212.3233 ms | 7.4466 ms | 211.2080 ms |
      ExtractTextsAngleSharp | 206.5462 ms | 5.1078 ms | 206.5650 ms |
         ExtractTextsCsQuery | 318.1908 ms | 3.7805 ms | 317.8808 ms |
 ExtractTextsHtmlParserSharp |  88.0808 ms | 0.8588 ms |  87.9277 ms |


