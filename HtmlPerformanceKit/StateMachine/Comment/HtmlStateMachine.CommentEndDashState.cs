using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action commentEndDashState;

        /// <summary>
        /// 8.2.4.49 Comment end dash state
        /// <br/>
        /// Consume the next input character:
        /// <br/>
        /// "-" (U+002D)
        /// Switch to the comment end state
        /// <br/>
        /// U+0000 NULL
        /// Parse error. Append a "-" (U+002D) character and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// <br/>
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// <br/>
        /// Anything else
        /// Append a "-" (U+002D) character and the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private void CommentEndDashStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = commentEndState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = commentState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add((char)currentInputCharacter);
                    State = commentState;
                    return;
            }
        }
    }
}
