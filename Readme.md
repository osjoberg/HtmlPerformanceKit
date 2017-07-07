# HtmlSpeedPack
HtmlSpeedPack is a fast HTML parser whith a similar API as XmlReader. HtmlSpeedPack tries to follow the HTML 5 tokenization specification:
https://www.w3.org/TR/html5/syntax.html#tokenization

## Install via NuGet
To install HtmlSpeedPack, run the following command in the Package Manager Console:

```cmd
PM> Install-Package HtmlSpeedPack
```

## Performance
-----------
HtmlSpeedPack It is currently about 7x faster than HtmlAgilityPack in my benchmarks. This is probably because HtmlSpeedPack is streaming the HTML document tokens as they are read while HtmlAgilityPack constructs a DOM for the entire document in memory. However HtmlAgilityPack has a much more user-friendly API and is more battle-tested. If you are not concerned about performance, you should probably use HtmlAgilityPack instead.

Please note that all states are not implemented yet, so it is not sutiable for production yet.

## Implemented states
------------------
```
[X] 8.2.4.1 Data state
[X] 8.2.4.2 Character reference in data state
[ ] 8.2.4.3 RCDATA state
[ ] 8.2.4.4 Character reference in RCDATA state
[ ] 8.2.4.5 RAWTEXT state
[ ] 8.2.4.6 Script data state
[ ] 8.2.4.7 PLAINTEXT state
[X] 8.2.4.8 Tag open state
[X] 8.2.4.9 End tag open state
[X] 8.2.4.10 Tag name state
[ ] 8.2.4.11 RCDATA less-than sign state
[ ] 8.2.4.12 RCDATA end tag open state
[ ] 8.2.4.13 RCDATA end tag name state
[ ] 8.2.4.14 RAWTEXT less-than sign state
[ ] 8.2.4.15 RAWTEXT end tag open state
[ ] 8.2.4.16 RAWTEXT end tag name state
[ ] 8.2.4.17 Script data less-than sign state
[ ] 8.2.4.18 Script data end tag open state
[ ] 8.2.4.19 Script data end tag name state
[ ] 8.2.4.20 Script data escape start state
[ ] 8.2.4.21 Script data escape start dash state
[ ] 8.2.4.22 Script data escaped state
[ ] 8.2.4.23 Script data escaped dash state
[ ] 8.2.4.24 Script data escaped dash dash state
[ ] 8.2.4.25 Script data escaped less-than sign state
[ ] 8.2.4.26 Script data escaped end tag open state
[ ] 8.2.4.27 Script data escaped end tag name state
[ ] 8.2.4.28 Script data double escape start state
[ ] 8.2.4.29 Script data double escaped state
[ ] 8.2.4.30 Script data double escaped dash state
[ ] 8.2.4.31 Script data double escaped dash dash state
[ ] 8.2.4.32 Script data double escaped less-than sign state
[ ] 8.2.4.33 Script data double escape end state
[X] 8.2.4.34 Before attribute name state
[X] 8.2.4.35 Attribute name state
[X] 8.2.4.36 After attribute name state
[X] 8.2.4.37 Before attribute value state
[X] 8.2.4.38 Attribute value(double-quoted) state
[X] 8.2.4.39 Attribute value(single-quoted) state
[X] 8.2.4.40 Attribute value(unquoted) state
[X] 8.2.4.41 Character reference in attribute value state
[X] 8.2.4.42 After attribute value(quoted) state
[X] 8.2.4.43 Self-closing start tag state
[X] 8.2.4.44 Bogus comment state
[X] 8.2.4.45 Markup declaration open state
[X] 8.2.4.46 Comment start state
[X] 8.2.4.47 Comment start dash state
[X] 8.2.4.48 Comment state
[X] 8.2.4.49 Comment end dash state
[X] 8.2.4.50 Comment end state
[X] 8.2.4.51 Comment end bang state
[X] 8.2.4.52 DOCTYPE state
[X] 8.2.4.53 Before DOCTYPE name state
[X] 8.2.4.54 DOCTYPE name state
[X] 8.2.4.55 After DOCTYPE name state
[X] 8.2.4.56 After DOCTYPE public keyword state
[X] 8.2.4.57 Before DOCTYPE public identifier state
[X] 8.2.4.58 DOCTYPE public identifier(double-quoted) state
[X] 8.2.4.59 DOCTYPE public identifier(single-quoted) state
[X] 8.2.4.60 After DOCTYPE public identifier state
[X] 8.2.4.61 Between DOCTYPE public and system identifiers state
[X] 8.2.4.62 After DOCTYPE system keyword state
[X] 8.2.4.63 Before DOCTYPE system identifier state
[X] 8.2.4.64 DOCTYPE system identifier(double-quoted) state
[X] 8.2.4.65 DOCTYPE system identifier(single-quoted) state
[X] 8.2.4.66 After DOCTYPE system identifier state
[X] 8.2.4.67 Bogus DOCTYPE state
[X] 8.2.4.68 CDATA section state
[/] 8.2.4.69 Tokenizing character references (Using HttpUtility.HtmlDecode when a character reference is found)
```