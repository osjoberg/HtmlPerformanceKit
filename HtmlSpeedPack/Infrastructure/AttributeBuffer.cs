namespace HtmlSpeedPack.Infrastructure
{
    internal class AttributeBuffer
    {
        public AttributeBuffer()
        {
            Name = new CharBuffer(32);
            Value = new CharBuffer(1024);
        }

        internal CharBuffer Name { get; }

        internal CharBuffer Value { get; }

        public void Clear()
        {
            Name.Clear();
            Value.Clear();
        }
    }
}
