using System;

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

        internal CurrentState State { get; private set; }

        internal bool Eof { get; private set; }

        internal HtmlTagToken EmitTagToken { get; private set; }

        internal CharBuffer EmitDataBuffer { get; private set; }

        internal HtmlTagToken EmitDoctypeToken { get; private set; }

        internal CharBuffer EmitCommentBuffer { get; private set; }

        internal void ParseError(string message)
        {
            parseError(message);
        }

        internal void ResetEmit()
        {
            EmitTagToken = null;
            EmitDataBuffer = null;
            EmitDoctypeToken = null;
            EmitCommentBuffer = null;

            currentDataBuffer.Clear();
        }

        internal void RememberLastStartTagName()
        {
            appropriateTagName.Clear();
            appropriateTagName.Append(EmitTagToken.Name.Buffer, EmitTagToken.Name.Length);
        }
    }
}
