using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action scriptDataDoubleEscapedDashState;

    /// <summary>
    /// 8.2.4.30 Script data double escaped dash state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "-" (U+002D)
    /// Switch to the script data double escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
    /// <br/>
    /// "&lt;" (U+003C)
    /// Switch to the script data double escaped less-than sign state. Emit a U+003C LESS-THAN SIGN character token.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Switch to the script data double escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Switch to the script data double escaped state. Emit the current input character as a character token.
    /// </summary>
    private void ScriptDataDoubleEscapedDashStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '-':
                State = scriptDataDoubleEscapedDashDashState;
                currentDataBuffer.Add('-');
                return;

            case '<':
                State = scriptDataDoubleEscapedLessThanSignState;
                currentDataBuffer.Add('<');
                return;

            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                State = scriptDataDoubleEscapedState;
                currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                return;

            case EofMarker:
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                State = dataState;
                bufferReader.Reconsume(EofMarker);
                return;

            default:
                State = scriptDataDoubleEscapedState;
                currentDataBuffer.Add((char)currentInputCharacter);
                return;
        }
    }
}