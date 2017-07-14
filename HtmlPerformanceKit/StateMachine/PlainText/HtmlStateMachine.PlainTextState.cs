using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.7 PLAINTEXT state
        ///
        /// Consume the next input character:
        /// 
        /// U+0000 NULL
        /// Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// 
        /// EOF
        /// Emit an end-of-file token.
        /// 
        /// Anything else
        /// Emit the current input character as a character token.
        /// </summary>
        private void PlainTextState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case HtmlChar.Null:
                    ParseError = ParseErrorMessage.UnexpectedNullCharacterInStream;
					currentDataBuffer.Append(HtmlChar.ReplacementCharacter);
					break;

                case EofMarker:
                    State = DataState;
                    EmitDataBuffer = currentDataBuffer;
                    bufferReader.Reconsume(EofMarker);
                    break;

                default:
					currentDataBuffer.Append((char)currentInputCharacter);
                    break;
            }
        }
    }
}
