using System;
using System.Diagnostics.CodeAnalysis;
using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
    private readonly Action rcDataState;

    /// <summary>
    /// 8.2.4.3 RCDATA state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// U+0026 AMPERSAND (&amp;)
    /// Switch to the character reference in RCDATA state.
    /// <br/>
    /// "&lt;" (U+003C)
    /// Switch to the RCDATA less-than sign state.
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
    private void RcDataStateImplementation()
    {
        while (true)
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '&' when decodeHtmlCharacters:
                    State = characterReferenceInRcDataState;
                    return;

                case '<':
                    State = rcDataLessThanSignState;
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