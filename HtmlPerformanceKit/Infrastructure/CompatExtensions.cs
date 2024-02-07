#if !NET8_0_OR_GREATER
using System.Collections.Generic;

namespace HtmlPerformanceKit.Infrastructure
{
    internal static class CompatExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> queue, out T result)
        {
            if (queue.Count == 0)
            {
                result = default(T);    
                return false;
            }
            else
            { 
                result = queue.Dequeue();
                return true;
            }
        }

        public static bool TryPeek<T>(this Queue<T> queue, out T result)
        {
            if (queue.Count == 0)
            {
                result = default(T);
                return false;
            }
            else
            {
                result = queue.Peek();
                return true;
            }
        }
    }
}
#endif