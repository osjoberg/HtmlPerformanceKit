using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.49 Comment end dash state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the comment end state
        /// 
        /// U+0000 NULL
        /// Parse error. Append a "-" (U+002D) character and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append a "-" (U+002D) character and the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private Action BuildCommentEndDashState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = CommentEndState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = CommentState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    EmitCommentBuffer = buffers.CurrentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        };
    }
}
