using System;
using System.Runtime.CompilerServices;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine : IDisposable
    {
        private const int EofMarker = -1;
        private readonly Buffers buffers = BuffersPool.Get();
        private readonly BufferReader bufferReader;
        private readonly Action<string> parseError;
        private readonly bool skipDecodingCharacterReferences;
        private Action returnToState;
        private char additionalAllowedCharacter;

        internal HtmlStateMachine(BufferReader bufferReader, Action<string> parseError, bool skipDecodingCharacterReferences)
        {
            this.bufferReader = bufferReader;
            this.parseError = parseError;
            this.skipDecodingCharacterReferences = skipDecodingCharacterReferences;
            State = DataState;
        }

        public void Dispose()
        {
            BuffersPool.Return(buffers);
        }

        internal Action State
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal bool Eof
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal HtmlTagToken EmitTagToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal CharBuffer EmitDataBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal HtmlTagToken EmitDoctypeToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal CharBuffer EmitCommentBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal void ParseError(string message)
        {
            parseError(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ResetEmit()
        {
            EmitTagToken = null;
            EmitDataBuffer = null;
            EmitDoctypeToken = null;
            EmitCommentBuffer = null;

            buffers.CurrentDataBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RememberLastStartTagName()
        {
            buffers.AppropriateTagName.Clear();
            buffers.AppropriateTagName.AddRange(EmitTagToken.Name);
        }
    }
}
