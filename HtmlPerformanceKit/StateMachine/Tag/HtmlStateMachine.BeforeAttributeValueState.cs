using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action beforeAttributeValueState;

    /// <summary>
    /// 8.2.4.37 Before attribute value state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "tab" (U+0009)
    /// "LF" (U+000A)
    /// "FF" (U+000C)
    /// U+0020 SPACE
    /// Ignore the character.
    /// <br/>
    /// U+0022 QUOTATION MARK (")
    /// Switch to the attribute value (double-quoted) state.
    /// <br/>
    /// U+0026 AMPERSAND (&amp;)
    /// Switch to the attribute value (unquoted) state. Reconsume the current input character.
    /// <br/>
    /// "'" (U+0027)
    /// Switch to the attribute value (single-quoted) state.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value. Switch to the attribute value (unquoted) state.
    /// <br/>
    /// "&gt;" (U+003E)
    /// Parse error. Switch to the data state. Emit the current tag token.
    /// <br/>
    /// "&lt;" (U+003C)
    /// "=" (U+003D)
    /// "`" (U+0060)
    /// Parse error. Treat it as per the "anything else" entry below.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Append the current input character to the current attribute's value. Switch to the attribute value (unquoted) state.
    /// </summary>
    private void BeforeAttributeValueStateImplementation()
    {
        while (true)
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    return;

                case '"':
                    State = attributeValueDoubleQuotedState;
                    return;

                case '&':
                    State = attributeValueUnquotedState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;

                case '\'':
                    State = attributeValueSingleQuotedState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    State = dataState;
                    EmitTagToken = currentTagToken;
                    return;

                case '<':
                case '=':
                case '`':
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    goto default;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                    State = attributeValueUnquotedState;
                    return;
            }
        }
    }
}