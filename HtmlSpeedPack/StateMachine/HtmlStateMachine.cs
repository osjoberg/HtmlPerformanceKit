using System.IO;

using HtmlSpeedPack.Infrastructure;

namespace HtmlSpeedPack.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private const int EofMarker = -1;

        private readonly HtmlTagToken currentTagToken = new HtmlTagToken();

        private readonly HtmlTagToken currentDoctypeToken = new HtmlTagToken();

        private readonly CharBuffer currentDataBuffer = new CharBuffer(1024 * 10);

        private readonly CharBuffer currentCommentBuffer = new CharBuffer(1024 * 10);

        private readonly CharBuffer temporaryBuffer = new CharBuffer(1024);


        private readonly BufferReader bufferReader;

        private CurrentState returnToState;

        private char additionalAllowedCharacter;

        public HtmlStateMachine(StreamReader streamReader)
        {
            this.bufferReader = new BufferReader(streamReader);
            State = DataState;
        }

        public delegate void CurrentState();

        public string ParseError { get; private set; }

        public CurrentState State { get; private set; }




        public bool Eof { get; private set; }

        public HtmlTagToken EmitTagToken { get; private set; }

        public CharBuffer EmitDataBuffer { get; private set; }

        public HtmlTagToken EmitDoctypeToken { get; private set; }

        public CharBuffer EmitCommentBuffer { get; private set; }

        internal void ResetEmit()
        {
            EmitTagToken = null;
            EmitDataBuffer = null;
            EmitDoctypeToken = null;
            EmitCommentBuffer = null;

            currentDataBuffer.Clear();

            ParseError = null;
        }
    }
}
