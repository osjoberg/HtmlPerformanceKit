using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action attributeValueUnquotedState;

    /// <summary>
    /// 8.2.4.40 Attribute value (unquoted) state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "tab" (U+0009)
    /// "LF" (U+000A)
    /// "FF" (U+000C)
    /// U+0020 SPACE
    /// Switch to the before attribute name state.
    /// <br/>
    /// U+0026 AMPERSAND (&amp;)
    /// Switch to the character reference in attribute value state, with the additional allowed character being "&gt;" (U+003E).
    /// <br/>
    /// "&gt;" (U+003E)
    /// Switch to the data state. Emit the current tag token.
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
    /// <br/>
    /// U+0022 QUOTATION MARK (")
    /// "'" (U+0027)
    /// "&lt;" (U+003C)
    /// "=" (U+003D)
    /// "`" (U+0060)
    /// Parse error. Treat it as per the "anything else" entry below.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Append the current input character to the current attribute's value.
    /// </summary>
    private void AttributeValueUnquotedStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '\t':
            case '\r':
            case '\n':
            case ' ':
                State = beforeAttributeNameState;
                return;

            case '&' when decodeHtmlCharacters:
                State = characterReferenceInAttributeValueState;
                additionalAllowedCharacter = '>';
                returnToState = attributeValueDoubleQuotedState;
                return;

            case '>':
                State = dataState;
                EmitTagToken = currentTagToken;
                return;

            case HtmlChar.Null:
                ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                return;

            case '"':
            case '\'':
            case '<':
            case '=':
            case '`':
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                goto default;

            case EofMarker:
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                State = dataState;
                bufferReader.Reconsume(EofMarker);
                return;

            default:
                currentTagToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                return;
        }
    }
}