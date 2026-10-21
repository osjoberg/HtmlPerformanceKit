using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private static readonly char[] AttributeValueDoubleQuotedStateChars = ['"', '&', '<', HtmlChar.Null];

    private readonly Action attributeValueDoubleQuotedState;

    /// <summary>
    /// 8.2.4.38 Attribute value (double-quoted) state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// U+0022 QUOTATION MARK (")
    /// Switch to the after attribute value (quoted) state.
    /// <br/>
    /// U+0026 AMPERSAND (&amp;)
    /// Switch to the character reference in attribute value state, with the additional allowed character being U+0022 QUOTATION MARK (").
    /// <br/>
    /// U+0000 NULL
    /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Append the current input character to the current attribute's value.
    /// </summary>
    private void AttributeValueDoubleQuotedStateImplementation()
    {
        while (true)
        {
            var currentInputCharacter = bufferReader.ConsumeUntil(AttributeValueDoubleQuotedStateChars, out var consumed);

            switch (currentInputCharacter)
            {
                case '"':
                    currentTagToken.Attributes.Current.Value.AddRange(consumed.Slice(0, consumed.Length - 1));
                    State = afterAttributeValueQuotedState;
                    return;

                case '&' when decodeHtmlCharacters:
                    currentTagToken.Attributes.Current.Value.AddRange(consumed.Slice(0, consumed.Length - 1));
                    State = characterReferenceInAttributeValueState;
                    additionalAllowedCharacter = '"';
                    returnToState = attributeValueDoubleQuotedState;
                    return;

                case HtmlChar.Null:
                    currentTagToken.Attributes.Current.Value.AddRange(consumed.Slice(0, consumed.Length - 1));
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    break;

                case EofMarker:
                    currentTagToken.Attributes.Current.Value.AddRange(consumed);
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Current.Value.AddRange(consumed);
                    break;
            }
        }
    }
}