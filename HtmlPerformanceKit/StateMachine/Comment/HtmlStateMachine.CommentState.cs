using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.48 Comment state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the comment end dash state
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the comment token's data.
        /// </summary>
        private void CommentState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = CommentEndDashState;
                    return;

                case HtmlChar.Null:
                    ParseError = ParseErrorMessage.UnexpectedNullCharacterInStream;
                    currentCommentBuffer.Append(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Append((char)currentInputCharacter);
                    return;
            }
        }
    }
}
