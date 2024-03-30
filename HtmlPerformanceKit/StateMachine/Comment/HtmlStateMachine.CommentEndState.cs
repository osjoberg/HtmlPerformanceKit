using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action commentEndState;

    /// <summary>
    /// 8.2.4.50 Comment end state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "&gt;" (U+003E)
    /// Switch to the data state. Emit the comment token.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Append two "-" (U+002D) characters and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
    /// <br/>
    /// "!" (U+0021)
    /// Parse error. Switch to the comment end bang state.
    /// <br/>
    /// "-" (U+002D)
    /// Parse error. Append a "-" (U+002D) character to the comment token's data.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Parse error. Append two "-" (U+002D) characters and the current input character to the comment token's data. Switch to the comment state.
    /// </summary>
    private void CommentEndStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '>':
                State = dataState;
                EmitCommentBuffer = currentCommentBuffer;
                return;

            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add(HtmlChar.ReplacementCharacter);
                State = commentState;
                return;

            case '!':
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = commentEndBangState;
                return;

            case '-':
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                currentCommentBuffer.Add('-');
                return;

            case EofMarker:
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                State = dataState;
                EmitCommentBuffer = currentCommentBuffer;
                bufferReader.Reconsume(EofMarker);
                return;

            default:
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add('-');
                currentCommentBuffer.Add((char)currentInputCharacter);
                State = commentState;
                return;
        }
    }
}