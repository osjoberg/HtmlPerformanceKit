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
            while (peekBuffer.Count > 0)
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
