namespace HtmlPerformanceKit.Infrastructure;

internal class HtmlTagToken
{
    public HtmlTagToken()
    {
        Name = new CharBuffer(32);
        Attributes = new AttributeBufferList();
    }

    internal bool SelfClosing { get; set; }

    internal CharBuffer Name { get; set; }

    internal AttributeBufferList Attributes { get; set; }

    internal bool EndTag { get; set; }

    internal void Clear()
    {
        SelfClosing = false;
        EndTag = false;
        Name.Clear();
        Attributes.Clear();
    }
}