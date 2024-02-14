using System;
using System.Runtime.CompilerServices;

namespace HtmlPerformanceKit.Infrastructure
{
    internal class CharBuffer
    {
        private char[] buffer;

        internal CharBuffer(int initialSize)
        {
            buffer = new char[initialSize];
        }

        internal int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return new string(buffer, 0, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear()
        {
            Length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(char @char)
        {
            if (Length == buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            buffer[Length++] = @char;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddRange(string @string)
        {
            while (Length + @string.Length > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            @string.CopyTo(0, buffer, Length, @string.Length);
            Length += @string.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddRange(char[] array)
        {
            while (Length + array.Length > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            Array.Copy(array, 0, buffer, Length, array.Length);
            Length += array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddRange(CharBuffer charBuffer)
        {
            while (Length + charBuffer.Length > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            Array.Copy(charBuffer.buffer, 0, buffer, Length, charBuffer.Length);
            Length += charBuffer.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Equals(CharBuffer charBuffer)
        {
            if (Length != charBuffer.Length)
            {
                return false;
            }

            for (var index = 0; index < Length; index++)
            {
                if (charBuffer.buffer[index] != buffer[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
