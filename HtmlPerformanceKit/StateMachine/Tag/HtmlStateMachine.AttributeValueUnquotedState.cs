using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.40 Attribute value (unquoted) state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before attribute name state.
        /// 
        /// U+0026 AMPERSAND (&amp;)
        /// Switch to the character reference in attribute value state, with the additional allowed character being "&gt;" (U+003E).
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current tag token.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// "'" (U+0027)
        /// "&lt;" (U+003C)
        /// "=" (U+003D)
        /// "`" (U+0060)
        /// Parse error. Treat it as per the "anything else" entry below.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current attribute's value.
        /// </summary>
        private void AttributeValueUnquotedState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\r':
                case '\n':
                case ' ':
                    State = BeforeAttributeNameState;
                    return;

                case '&':
                    State = CharacterReferenceInAttributeValueState;
                    additionalAllowedCharacter = '>';
                    returnToState = AttributeValueDoubleQuotedState;
                    return;

                case '>':
                    State = DataState;
                    EmitTagToken = currentTagToken;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Current.Value.Append(HtmlChar.ReplacementCharacter);
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
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Current.Value.Append((char)currentInputCharacter);
                    return;
            }
        }
    }
}
