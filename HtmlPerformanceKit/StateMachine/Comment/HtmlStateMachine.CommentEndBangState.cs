using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action commentEndBangState;

    /// <summary>
    /// 8.2.4.51 Comment end bang state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "-" (U+002D)
    /// Append two "-" (U+002D) characters and a "!" (U+0021) character to the comment token's data. Switch to the comment end dash state.
    /// <br/>
    /// ">" (U+003E)
    /// Switch to the data state. Emit the comment token.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Append two "-" (U+002D) characters, a "!" (U+0021) character, and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
    /// <br/>
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
                State = commentEndDashState;
                return;

            case '>':
                State = dataState;
                EmitCommentBuffer = currentCommentBuffer;
                return;

            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add('!');
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
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add('!');
                currentCommentBuffer.Add((char)currentInputCharacter);
                State = commentState;
                return;
        }
    }
}