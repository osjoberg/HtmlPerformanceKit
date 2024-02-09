using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action CommentEndBangState;

        /// <summary>
        /// 8.2.4.51 Comment end bang state
        /// 
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Append two "-" (U+002D) characters and a "!" (U+0021) character to the comment token's data. Switch to the comment end dash state.
        /// 
        /// ">" (U+003E)
        /// Switch to the data state. Emit the comment token.
        /// 
        /// U+0000 NULL
        /// Parse error. Append two "-" (U+002D) characters, a "!" (U+0021) character, and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append two "-" (U+002D) characters, a "!" (U+0021) character, and the current input character to the comment token's data. Switch to the comment state.
        /// </summary>
        private void CommentEndBangStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add('!');
                    State = CommentEndDashState;
                    return;

                case '>':
                    State = DataState;
                    EmitCommentBuffer = currentCommentBuffer;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add('!');
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
                    currentCommentBuffer.Add('-');
                    currentCommentBuffer.Add('!');
                    currentCommentBuffer.Add((char)currentInputCharacter);
                    State = CommentState;
                    return;
            }
        }
    }
}
