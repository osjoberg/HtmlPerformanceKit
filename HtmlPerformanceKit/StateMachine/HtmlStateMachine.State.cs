using HtmlPerformanceKit.Infrastructure;
using Microsoft.Extensions.ObjectPool;

namespace HtmlPerformanceKit.StateMachine
{
    partial class HtmlStateMachine
    {
        private static readonly ObjectPool<Buffers> BuffersPool = new DefaultObjectPool<Buffers>(
            new BuffersPoolPolicy());

        private class BuffersPoolPolicy : IPooledObjectPolicy<Buffers>
        {
            public Buffers Create()
            {
                return new Buffers();
            }

            public bool Return(Buffers obj)
            {
                const int MaxSize = 500 * 1024;

                int size =
                    obj.AppropriateTagName.Capacity +
                    obj.CurrentCommentBuffer.Capacity +
                    obj.CurrentDataBuffer.Capacity +
                    obj.CurrentDoctypeToken.Name.Capacity +
                    obj.CurrentDoctypeToken.Attributes.Capacity +
                    obj.CurrentTagToken.Name.Capacity +
                    obj.CurrentTagToken.Attributes.Capacity +
                    obj.TemporaryBuffer.Capacity;

                if (size > MaxSize)
                {
                    return false;
                }

                obj.AppropriateTagName.Clear();
                obj.CurrentCommentBuffer.Clear();
                obj.CurrentDataBuffer.Clear();
                obj.CurrentDoctypeToken.Clear();
                obj.CurrentTagToken.Clear();
                obj.TemporaryBuffer.Clear();

                return true;
            }
        }

        private sealed class Buffers
        {
            public readonly HtmlTagToken CurrentTagToken = new HtmlTagToken();
            public readonly HtmlTagToken CurrentDoctypeToken = new HtmlTagToken();
            public readonly CharBuffer CurrentDataBuffer = new CharBuffer(1024 * 10);
            public readonly CharBuffer CurrentCommentBuffer = new CharBuffer(1024 * 10);
            public readonly CharBuffer TemporaryBuffer = new CharBuffer(1024);
            public readonly CharBuffer AppropriateTagName = new CharBuffer(100);
        }
    }
}
