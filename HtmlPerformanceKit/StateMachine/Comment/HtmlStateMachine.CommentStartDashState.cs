using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    /// <summary>
    /// 8.2.4.47 Comment start dash state
    /// 
    /// Consume the next input character:
    /// 
    /// "-" (U+002D)
    /// Switch to the comment end state
    /// 
    /// U+0000 NULL
    /// Parse error.Append a "-" (U+002D) character and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
    /// 
    /// ">" (U+003E)
    /// Parse error.Switch to the data state. Emit the comment token.
    /// 
    /// EOF
    /// Parse error.Switch to the data state. Emit the comment token. Reconsume the EOF character.
    /// 
    /// Anything else
    /// Append a "-" (U+002D) character and the current input character to the comment token's data. Switch to the comment state.    
    /// </summary>
    internal partial class HtmlStateMachine
    {
        private void CommentStartDashState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = CommentStartDashState;
                    return;

                case HtmlChar.Null:
                    ParseError = ParseErrorMessage.UnexpectedNullCharacterInStream;
                    currentCommentBuffer.Append('-');
                    currentCommentBuffer.Append(HtmlChar.ReplacementCharacter);
                    State = CommentState;
                    return;

                case '>':
                    ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Append('-');
                    currentCommentBuffer.Append((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        }
    }
}
