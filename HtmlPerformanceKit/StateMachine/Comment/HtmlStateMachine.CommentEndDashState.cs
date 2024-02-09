using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action CommentEndDashState;

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
        private void CommentEndDashStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = CommentEndState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = CommentState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        }
    }
}
