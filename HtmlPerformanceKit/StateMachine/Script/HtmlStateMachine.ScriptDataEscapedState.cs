using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action scriptDataEscapedState;

    /// <summary>
    /// 8.2.4.22 Script data escaped state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "-" (U+002D)
    /// Switch to the script data escaped dash state. Emit a U+002D HYPHEN-MINUS character token.
    /// <br/>
    /// "&lt;" (U+003C)
    /// Switch to the script data escaped less-than sign state.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
    /// <br/>
    /// EOF
    /// Switch to the data state. Parse error. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Emit the current input character as a character token.
    /// </summary>
    private void ScriptDataEscapedStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '-':
                State = scriptDataEscapedDashState;
                currentDataBuffer.Add('-');
                return;

            case '<':
                State = scriptDataEscapedLessThanSignState;
                return;

            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                return;

            case EofMarker:
                State = dataState;
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                bufferReader.Reconsume(EofMarker);
                return;

            default:
                currentDataBuffer.Add((char)currentInputCharacter);
                return;
        }
    }
}