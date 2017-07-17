# HtmlPerformanceKit
HtmlPerformanceKit is a fast HTML parser whith a similar API as XmlReader. HtmlPerformanceKit tries to follow the HTML 5 tokenization specification:
https://www.w3.org/TR/html5/syntax.html#tokenization

## Install via NuGet
To install HtmlPerformanceKit, run the following command in the Package Manager Console:

```cmd
PM> Install-Package HtmlPerformanceKit
```

## Performance
HtmlPerformanceKit is currently about 7x faster than HtmlAgilityPack in my benchmarks. This is probably because HtmlPerformanceKit is streaming the HTML document tokens as they are read while HtmlAgilityPack constructs a DOM for the entire document in memory. HtmlAgilityPack has a much more user-friendly API and is more battle-tested. If you are not concerned about performance, you should probably use HtmlAgilityPack instead.

## Example usage
```
	public IEnumerable<string> ExtractLinks()
	{
        using (var htmlReader = new HtmlReader(File.OpenRead("test.html")))
		{
			while (htmlReader.Read())
			{
				if (htmlReader.NodeType == HtmlNodeType.Tag && htmlReader.Name == "a")
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
JitModules=clrjit-v4.6.1648.0
Job Runtime(s):
	Clr 4.0.30319.42000, Arch=32-bit RELEASE
```
|                      Method |        Mean |    StdDev |      Median |
|---------------------------- |------------ |---------- |------------ |
|                ExtractLinks |  29.2549 ms | 0.7443 ms |  29.2327 ms |
| ExtractLinksHtmlAgilityPack | 249.1747 ms | 6.6903 ms | 249.3893 ms |
|                ExtractTexts |  35.2610 ms | 1.0202 ms |  35.5725 ms |
| ExtractTextsHtmlAgilityPack | 261.6217 ms | 7.8588 ms | 258.0977 ms |
