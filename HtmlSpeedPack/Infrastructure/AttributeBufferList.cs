using System.Collections.Generic;

namespace HtmlSpeedPack.Infrastructure
{
    internal class AttributeBufferList
    {
        private readonly List<AttributeBuffer> attributes = new List<AttributeBuffer>();

        public int Count { get; private set; }

        public AttributeBuffer Current { get; private set; }

        public CharBuffer this[string name] => GetItem(name)?.Value;
        
        public AttributeBuffer this[int index] => index < Count ? attributes[index] : null;

        public void Add()
        {
            if (attributes.Count > Count)
            {
                var existingAttributeBuffer = attributes[Count];
                existingAttributeBuffer.Clear();
                Count++;

                Current = existingAttributeBuffer;
            }
            else
            {
                var newAttributeBuffer = new AttributeBuffer();
                attributes.Add(newAttributeBuffer);
                Count++;

                Current = newAttributeBuffer;
            }
        }

        public void Clear()
        {
            Count = 0;
            Current = null;
        }

        public bool Contains(string name)
        {
            return GetItem(name) != null;
        }

        private AttributeBuffer GetItem(string name)
        {
            for (var index = 0; index < Count; index++)
            {
                if (attributes[index].Name.Equals(name))
                {
                    return attributes[index];
                }
            }

            return null;
        }        
    }
}
