using System;

namespace HtmlSpeedPack.Infrastructure
{
    internal class CharBuffer
    {
        private char[] buffer;

        internal CharBuffer(int initialSize)
        {
            buffer = new char[initialSize];
        }

        internal char[] Buffer => buffer;

        internal int Length { get; private set; }

        public override string ToString()
        {
            return new string(buffer, 0, Length);
        }

        internal void Clear()
        {
            Length = 0;
        }

        internal void Append(char @char)
        {
            if (Length == buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            buffer[Length++] = @char;
        }

        internal void Append(string @string)
        {
            Append(@string.ToCharArray(), @string.Length);
        }

        internal void Append(char[] array, int length)
        {
            while (Length + length > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            Array.Copy(array, 0, buffer, Length, length);
            Length += length;
        }

        internal bool Equals(string @string)
        {
            if (Length != @string.Length)
            {
                return false;
            }

            for (var index = 0; index < Length; index++)
            {
                if (@string[index] != buffer[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
