using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action commentState;

        /// <summary>
        /// 8.2.4.48 Comment state
        /// <br/>
        /// Consume the next input character:
        /// <br/>
        /// "-" (U+002D)
        /// Switch to the comment end dash state
        /// <br/>
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data.
        /// <br/>
        /// EOF
        /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
        /// <br/>
        /// Anything else
        /// Append the current input character to the comment token's data.
        /// </summary>
        private void CommentStateImplementation()
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '-':
                        State = commentEndDashState;
                        return;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                        currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                        return;

                    case EofMarker:
                        ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                        State = dataState;
                        EmitCommentBuffer = currentCommentBuffer;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        currentCommentBuffer.Add((char)currentInputCharacter);
                        return;
                }
            }
        }
    }
}
