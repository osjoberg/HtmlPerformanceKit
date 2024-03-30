using System;

namespace HtmlPerformanceKit.Infrastructure;

/// <summary>
/// Implements a queue where it is also possible to insert items in the beginning of the queue.
/// </summary>
internal class QueueStack
{
    private int firstIndex;
    private int lastIndex;
    private int[] items;
    private char[] copyBuffer;

    internal QueueStack(int capacity)
    {
        Capacity = capacity;
        items = new int[capacity];
        copyBuffer = new char[capacity];
    }

    internal int Count { get; private set; }

    internal int Capacity { get; private set; }

    internal void Push(int item)
    {
        if (firstIndex == 0)
        {
            firstIndex = Capacity - 1;
        }
        else
        {
            firstIndex--;
        }

        if (firstIndex == lastIndex)
        {
            Resize();
        }

        items[firstIndex] = item;
        Count++;
    }

    internal void Enqueue(int item)
    {
        items[lastIndex] = item;
        Count++;

        if (lastIndex == Capacity - 1)
        {
            lastIndex = 0;
        }
        else
        {
            lastIndex++;
        }

        if (firstIndex == lastIndex)
        {
            Resize();
        }
    }

    internal int Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Collection is empty.");
        }

        return items[firstIndex];
    }

    internal int Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Collection is empty.");
        }

        var item = items[firstIndex];

        if (firstIndex == Capacity - 1)
        {
            firstIndex = 0;
        }
        else
        {
            firstIndex++;
        }

        Count--;

        if (Count == 0)
        {
            firstIndex = 0;
            lastIndex = 0;
        }

        return item;
    }

    internal ReadOnlyMemory<char> AsMemory()
    {
        if (firstIndex < lastIndex)
        {
            var copyBufferIndex = 0;
            for (var index = firstIndex; index < lastIndex; index++, copyBufferIndex++)
            {
                var read = items[index];
                if (read == -1)
                {
                    break;
                }

                copyBuffer[copyBufferIndex] = (char)read;
            }

            return new ReadOnlyMemory<char>(copyBuffer, 0, copyBufferIndex);
        }

        var copyBufferIndex2 = 0;
        for (var index = firstIndex; index < Capacity; index++, copyBufferIndex2++)
        {
            var read = items[index];
            if (read == -1)
            {
                return new ReadOnlyMemory<char>(copyBuffer, 0, copyBufferIndex2);
            }

            copyBuffer[copyBufferIndex2] = (char)read;
        }

        for (var index = 0; index < firstIndex; index++, copyBufferIndex2++)
        {
            var read = items[index];
            if (read == -1)
            {
                break;
            }

            copyBuffer[copyBufferIndex2] = (char)read;
        }

        return new ReadOnlyMemory<char>(copyBuffer, 0, copyBufferIndex2);
    }

    private void Resize()
    {
        var newCapacity = Capacity * 2;
        var newItems = new int[newCapacity];

        if (firstIndex < lastIndex)
        {
            Array.Copy(items, newItems, items.Length);
        }
        else
        {
            Array.Copy(items, firstIndex, newItems, 0, Capacity - firstIndex);
            Array.Copy(items, 0, newItems, Capacity - firstIndex, lastIndex);
            firstIndex = 0;
            lastIndex = Count;
        }

        Capacity = newCapacity;
        copyBuffer = new char[newCapacity];
        items = newItems;
    }
}