using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.42 After attribute value (quoted) state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before attribute name state.
        /// 
        /// "/" (U+002F)
        /// Switch to the self-closing start tag state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current tag token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Switch to the before attribute name state. Reconsume the character.
        /// </summary>
        private void AfterAttributeValueQuotedState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = BeforeAttributeNameState;
                    return;

                case '/':
                    State = SelfClosingStartTagState;               
                    return;

                case '>':
                    State = DataState;
                    EmitTagToken = currentTagToken;
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
                    State = BeforeAttributeNameState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
