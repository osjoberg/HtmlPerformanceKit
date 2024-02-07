using HtmlPerformanceKit.Infrastructure;
using Microsoft.Extensions.ObjectPool;

namespace HtmlPerformanceKit.StateMachine
{
    partial class HtmlStateMachine
    {
        private static readonly ObjectPool<HtmlStateMachine> Pool = new DefaultObjectPool<HtmlStateMachine>(
            new StateMachinePolicy());

        private class StateMachinePolicy : IPooledObjectPolicy<HtmlStateMachine>
        {
            public HtmlStateMachine Create()
            {
                return new HtmlStateMachine();
            }

            public bool Return(HtmlStateMachine obj)
            {
                const int MaxSize = 50 * 1024;

                int size =
                    obj.appropriateTagName.Capacity +
                    obj.currentCommentBuffer.Capacity +
                    obj.currentDataBuffer.Capacity +
                    obj.currentDoctypeToken.Name.Capacity +
                    obj.currentDoctypeToken.Attributes.Capacity +
                    obj.currentTagToken.Name.Capacity +
                    obj.currentTagToken.Attributes.Capacity +
                    obj.temporaryBuffer.Capacity;

                if (size > MaxSize)
                {
                    return false;
                }

                obj.additionalAllowedCharacter = default;
                obj.returnToState = null;
                obj.appropriateTagName.Clear();
                obj.currentCommentBuffer.Clear();
                obj.currentDoctypeToken.Clear();
                obj.currentTagToken.Clear();
                obj.temporaryBuffer.Clear();
                obj.ResetEmit();
                obj.Eof = false;

                return true;
            }
        }

        private sealed class Buffers
        {
        }
    }
}
