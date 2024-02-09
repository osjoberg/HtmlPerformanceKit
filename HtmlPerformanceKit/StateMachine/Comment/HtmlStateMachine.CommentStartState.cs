using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action CommentStartState;

        /// <summary>
        /// 8.2.4.46 Comment start state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the comment start dash state.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Switch to the data state. Emit the comment token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private void CommentStartStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = CommentStartDashState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = CommentState;
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentCommentBuffer.Add((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        }
    }
}
