using System;
using System.Runtime.CompilerServices;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private const int EofMarker = -1;
        private readonly HtmlTagToken currentTagToken = new HtmlTagToken();
        private readonly HtmlTagToken currentDoctypeToken = new HtmlTagToken();
        private readonly CharBuffer currentDataBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer currentCommentBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer temporaryBuffer = new CharBuffer(1024);
        private readonly CharBuffer appropriateTagName = new CharBuffer(100);
        private readonly BufferReader bufferReader;
        private readonly Action<string> parseError;
        private CurrentState returnToState;
        private char additionalAllowedCharacter;

        internal HtmlStateMachine(BufferReader bufferReader, Action<string> parseError)
        {
            this.bufferReader = bufferReader;
            this.parseError = parseError;
            State = DataState;
        }

        internal delegate void CurrentState();

        internal CurrentState State
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

            currentDataBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RememberLastStartTagName()
        {
            appropriateTagName.Clear();
            appropriateTagName.AddRange(EmitTagToken.Name);
        }
    }
}
