using System;
using System.Collections.Generic;

namespace HtmlPerformanceKit.Infrastructure;

internal class ReadOnlyMemoryComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    internal static readonly ReadOnlyMemoryComparer Instance = new ReadOnlyMemoryComparer();

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return x.Span.Equals(y.Span, StringComparison.Ordinal);
    }

    public int GetHashCode(ReadOnlyMemory<char> obj)
    {
        var span = obj.Span;

        unchecked
        {
            var hash = 19;

            for (var i = 0; i < span.Length; i++)
            {
                hash = (hash * 31) + span[i].GetHashCode();
            }

            return hash;
        }
    }
}