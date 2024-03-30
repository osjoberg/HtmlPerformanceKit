using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action plainTextState;

    /// <summary>
    /// 8.2.4.7 PLAINTEXT state
    /// <br/>
    /// Consume the next input character:
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
    private void PlainTextStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                break;

            case EofMarker:
                State = dataState;
                EmitDataBuffer = currentDataBuffer;
                bufferReader.Reconsume(EofMarker);
                break;

            default:
                currentDataBuffer.Add((char)currentInputCharacter);
                break;
        }
    }
}