using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action scriptDataState;

    /// <summary>
    /// 8.2.4.6 Script data state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "&lt;" (U+003C)
    /// Switch to the script data less-than sign state.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
    /// <br/>
    /// EOF
    /// Emit an end-of-file token.
    /// <br/>
    /// Anything else
    /// Emit the current input character as a character token.
    /// </summary>
    private void ScriptDataStateImplementation()
    {
        while (true)
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '<':
                    State = scriptDataLessThanSignState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    continue;

                case EofMarker:
                    State = dataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentDataBuffer.Add((char)currentInputCharacter);
                    continue;
            }
        }
    }
}