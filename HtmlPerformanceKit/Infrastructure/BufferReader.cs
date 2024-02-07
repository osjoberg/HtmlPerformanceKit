using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace HtmlPerformanceKit.Infrastructure
{
    internal class BufferReader
    {
        private readonly char[] PeekBuffer = new char[40];
        private readonly Queue<int> peekBuffer = new Queue<int>();
        private readonly Queue<int> reconsumeBuffer = new Queue<int>();
        private TextReader textReader;

        internal int LineNumber { get; private set; }

        internal int LinePosition { get; private set; }

        public void Clear()
        {
            peekBuffer.Clear();
            reconsumeBuffer.Clear();
        }

        public void Close()
        {
            textReader.Dispose();
        }

        public void Init(TextReader reader)
        {
            textReader = reader;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlyMemory<char> Peek(int length)
        {
            int i = 0;

            for (; i < length; i++)
            {
                if (!peekBuffer.TryDequeue(out var peekResult))
                {
                    break;
                }

                PeekBuffer[i] = (char)peekResult;
            }

            // Put them back to the peek buffer.
            for (var j = 0; j < i; j++)
            {
                peekBuffer.Enqueue(PeekBuffer[j]);
            }

            for (; i < length; i++)
            {
                var consumed = textReader.Read();

                if (consumed < 0)
                {
                    break;
                }

                peekBuffer.Enqueue(consumed);

                PeekBuffer[i] = (char)consumed;
            }

            return new ReadOnlyMemory<char>(PeekBuffer, 0, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Consume(int length)
        {
            for (var i = 0; i < length; i++)
            {
                if (Consume() < 0)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reconsume(int data)
        {
            reconsumeBuffer.Enqueue(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Peek()
        {
            if (peekBuffer.TryPeek(out var peekResult))
            {
                return peekResult;
            }

            var consumed = textReader.Read();

            if (consumed >= 0)
            {
                peekBuffer.Enqueue(consumed);
            }

            return consumed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Consume()
        {
            if (reconsumeBuffer.TryDequeue(out var reconsume))
            {
                return reconsume;
            }

            if (peekBuffer.TryDequeue(out var peekResult))
            {
                return peekResult;
            }

            if (LineNumber == 0)
            {
                LineNumber = 1;
                LinePosition = 1;
            }

            var result = textReader.Read();
            if (result == '\n')
            {
                LineNumber++;
                LinePosition = 1;
            }
            else if (result >= 0)
            {
                LinePosition++;
            }

            return result;
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
    }
}
