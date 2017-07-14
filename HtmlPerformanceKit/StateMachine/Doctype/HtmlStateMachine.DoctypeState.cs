using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.52 DOCTYPE state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before DOCTYPE name state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Create a new DOCTYPE token. Set its force-quirks flag to on. Emit the token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Switch to the before DOCTYPE name state. Reconsume the character.
        /// </summary>
        private void DoctypeState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = BeforeDoctypeNameState;
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
                    State = BeforeDoctypeNameState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
