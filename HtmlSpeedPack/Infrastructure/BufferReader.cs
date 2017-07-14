using System;
using System.Collections.Generic;
using System.IO;

namespace HtmlSpeedPack.Infrastructure
{
    internal class BufferReader
    {
        private readonly StreamReader streamReader;
        private readonly Queue<int> peekBuffer = new Queue<int>(8);

        public BufferReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        public string Peek(int length)
        {
            var outputLength = Math.Min(peekBuffer.Count, length);

            while (peekBuffer.Count < length)
            {
                var currentInputCharacter = streamReader.Read();
                peekBuffer.Enqueue(currentInputCharacter);
                outputLength++;

                if (currentInputCharacter == -1)
                {
                    break;
                }
            }

            var intBuffer = peekBuffer.ToArray();
            var charBuffer = new char[outputLength];
            for (var index = 0; index < outputLength; index++)
            {
                if (intBuffer[index] == -1)
                {
                    break;
                }

                charBuffer[index] = (char)intBuffer[index];
            }

            return new string(charBuffer, 0, outputLength);
        }

        public void Consume(int length)
        {
            while (peekBuffer.Count > 0 && length > 0)
            {
                peekBuffer.Dequeue();
                length--;
            }

            streamReader.ReadBlock(new char[length], 0, length);
        }

        public int Consume()
        {
            return peekBuffer.Count == 0 ? streamReader.Read() : peekBuffer.Dequeue();
        }

        public int ConsumeDigits()
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

        public int ConsumeHexDigits()
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

        public void Reconsume(int data)
        {
            peekBuffer.Enqueue(data);
        }

        public int Peek()
        {
            return peekBuffer.Count == 0 ? streamReader.Peek() : peekBuffer.Peek();
        }
    }
}
