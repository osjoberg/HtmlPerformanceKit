using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.50 Comment end state
        ///
        /// Consume the next input character:
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the comment token.
        /// 
        /// U+0000 NULL
        /// Parse error. Append two "-" (U+002D) characters and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// 
        /// "!" (U+0021)
        /// Parse error. Switch to the comment end bang state.
        /// 
        /// "-" (U+002D)
        /// Parse error. Append a "-" (U+002D) character to the comment token's data.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Append two "-" (U+002D) characters and the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private Action BuildCommentEndState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '>':
                    State = DataState;
                    EmitCommentBuffer = buffers.CurrentCommentBuffer;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                    State = CommentState;
                    return;

                case '!':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = CommentEndBangState;
                    return;

                case '-':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    buffers.CurrentCommentBuffer.Add('-');
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    EmitCommentBuffer = buffers.CurrentCommentBuffer;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add('-');
                    buffers.CurrentCommentBuffer.Add((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        };
    }
}
