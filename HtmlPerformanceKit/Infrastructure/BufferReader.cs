using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace HtmlPerformanceKit.Infrastructure;

internal class BufferReader
{
    private const int ReconsumeBufferSize = 10;
    private const int ReadBufferSize = 4096;
    private const int BufferSize = ReconsumeBufferSize + ReadBufferSize;

    private readonly TextReader textReader;
    private readonly char[] buffer = new char[BufferSize];

    private int startIndex;
    private int endIndex;
    private int reconsumeCount;

    internal BufferReader(TextReader textReader)
    {
        this.textReader = textReader;
    }

    internal int LineNumber { get; private set; }

    internal int LinePosition { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Peek()
    {
        if (startIndex == endIndex)
        {
            startIndex = ReconsumeBufferSize;
            var read = textReader.Read(buffer, ReconsumeBufferSize, ReadBufferSize);
            endIndex = ReconsumeBufferSize + read;

            if (read == 0)
            {
                return -1;
            }
        }

        return buffer[startIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlyMemory<char> Peek(int length)
    {
        if (startIndex + length <= endIndex)
        {
            return buffer.AsMemory(startIndex, length);
        }

        var saveLength = endIndex - startIndex;
        Array.Copy(buffer, startIndex, buffer, ReconsumeBufferSize, saveLength);
        startIndex = ReconsumeBufferSize;
        var read = textReader.Read(buffer, startIndex + saveLength, BufferSize - startIndex - saveLength);
        endIndex = startIndex + saveLength + read;

        return buffer.AsMemory(startIndex, Math.Min(length, saveLength + read));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Consume()
    {
        if (startIndex == endIndex)
        {
            startIndex = ReconsumeBufferSize;
            var read = textReader.Read(buffer, ReconsumeBufferSize, ReadBufferSize);
            endIndex = ReconsumeBufferSize + read;

            if (read == 0)
            {
                return -1;
            }
        }

        var result = buffer[startIndex];
        startIndex++;

        if (LineNumber == 0)
        {
            LineNumber = 1;
        }

        if (reconsumeCount > 0)
        {
            reconsumeCount--;
            return result;
        }

        if (result == '\n')
        {
            LineNumber++;
            LinePosition = 0;
        }
        else
        {
            LinePosition++;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Consume(int length)
    {
        var positonSpan = buffer.AsSpan(startIndex, length);

        for (var newLineIndex = positonSpan.IndexOf('\n'); newLineIndex != -1; newLineIndex = positonSpan.IndexOf('\n'))
        {
            LineNumber++;
            LinePosition = 0;
            positonSpan = positonSpan.Slice(newLineIndex + 1);
        }

        LinePosition += positonSpan.Length;

        startIndex += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ConsumeUntil(char[] characters, out Span<char> consumed)
    {
        if (startIndex == endIndex)
        {
            startIndex = ReconsumeBufferSize;
            var read = textReader.Read(buffer, ReconsumeBufferSize, ReadBufferSize);
            endIndex = ReconsumeBufferSize + read;

            if (read == 0)
            {
                consumed = Span<char>.Empty;
                return -1;
            }
        }

        consumed = buffer.AsSpan(startIndex, endIndex - startIndex);
        var foundIndex = consumed.IndexOfAny(characters);
        if (foundIndex != -1)
        {
            consumed = consumed.Slice(0, foundIndex + 1);
        }

        startIndex += consumed.Length;

        if (LineNumber == 0)
        {
            LineNumber = 1;
            LinePosition = 0;
        }

        var positonSpan = consumed.Slice(reconsumeCount);

        for (var newLineIndex = positonSpan.IndexOf('\n'); newLineIndex != -1; newLineIndex = positonSpan.IndexOf('\n'))
        {
            LineNumber++;
            LinePosition = 0;
            positonSpan = positonSpan.Slice(newLineIndex + 1);
        }

        LinePosition += positonSpan.Length;

        reconsumeCount -= Math.Min(reconsumeCount, consumed.Length);

        return consumed[^1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ConsumeDigits()
    {
        long number = 0;
        var digitConsumed = false;

        while (true)
        {
            var currentInputChar = Peek();
            switch (currentInputChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    var thisNumber = Consume() - '0';

                    if (number < int.MaxValue)
                    {
                        digitConsumed = true;
                        number *= 10;
                        number += thisNumber;
                    }

                    break;

                default:
                    if (digitConsumed == false)
                    {
                        return int.MinValue;
                    }

                    if (number > int.MaxValue)
                    {
                        return int.MaxValue;
                    }

                    return (int)number;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int ConsumeHexDigits()
    {
        long number = 0;
        var digitConsumed = false;

        while (true)
        {
            var currentInputChar = Peek();
            var thisNumber = -1;
            switch (currentInputChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    thisNumber = Consume() - '0';
                    goto default;

                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    thisNumber = Consume() - 'A' + 10;
                    goto default;

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    thisNumber = Consume() - 'a' + 10;
                    goto default;

                default:
                    if (thisNumber != -1 && number < int.MaxValue)
                    {
                        digitConsumed = true;
                        number *= 16;
                        number += thisNumber;
                        continue;
                    }

                    if (digitConsumed == false)
                    {
                        return int.MinValue;
                    }

                    if (number > int.MaxValue)
                    {
                        return int.MaxValue;
                    }

                    return (int)number;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reconsume(int data)
    {
        startIndex--;

        if (data == -1)
        {
            endIndex = startIndex;
        }
        else
        {
            buffer[startIndex] = (char)data;
            reconsumeCount++;
        }
    }
}