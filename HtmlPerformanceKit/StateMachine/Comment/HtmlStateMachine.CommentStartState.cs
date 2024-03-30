using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action commentStartState;

        /// <summary>
        /// 8.2.4.46 Comment start state
        /// <br/>
        /// Consume the next input character:
        /// <br/>
        /// "-" (U+002D)
        /// Switch to the comment start dash state.
        /// <br/>
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// <br/>
        /// "&gt;" (U+003E)
        /// Parse error. Switch to the data state. Emit the comment token.
        /// <br/>
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// <br/>
        /// Anything else
        /// Append the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private void CommentStartStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = commentStartDashState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = commentState;
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = dataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Add((char)currentInputCharacter);
                    State = commentState;
                    return;
            }
        }
    }
}
