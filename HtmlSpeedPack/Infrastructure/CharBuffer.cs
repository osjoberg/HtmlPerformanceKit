using System;
using System.Web;

namespace HtmlSpeedPack.Infrastructure
{
    internal class CharBuffer
    {
        private bool htmlDecode;
        private char[] buffer;

        public CharBuffer(int initialSize)
        {
            buffer = new char[initialSize];
        }

        public int Length { get; private set; }

        public void Clear()
        {
            Length = 0;
            htmlDecode = false;
        }

        public void Append(char @char)
        {
            if (Length == buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            buffer[Length++] = @char;
        }

        public void MayHaveCharacterReference()
        {
            htmlDecode = true;
        }

        internal void Append(string @string)
        {
            while (Length + @string.Length > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }
            
            Array.Copy(@string.ToCharArray(), 0, buffer, Length, @string.Length);
        }

        public bool Equals(string @string)
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

        public override string ToString()
        {           
            return htmlDecode == false ? new string(buffer, 0, Length) : HttpUtility.HtmlDecode(new string(buffer, 0, Length));
        }
    }
}
