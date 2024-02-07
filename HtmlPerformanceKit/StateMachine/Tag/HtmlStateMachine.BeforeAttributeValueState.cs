using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.37 Before attribute value state
        ///
        ///Consume the next input character:
        ///
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Ignore the character.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Switch to the attribute value (double-quoted) state.
        /// 
        /// U+0026 AMPERSAND (&amp;)
        /// Switch to the attribute value (unquoted) state. Reconsume the current input character.
        /// 
        /// "'" (U+0027)
        /// Switch to the attribute value (single-quoted) state.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value. Switch to the attribute value (unquoted) state.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Switch to the data state. Emit the current tag token.
        /// 
        /// "&lt;" (U+003C)
        /// "=" (U+003D)
        /// "`" (U+0060)
        /// Parse error. Treat it as per the "anything else" entry below.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current attribute's value. Switch to the attribute value (unquoted) state.
        /// </summary>
        private Action BuildBeforeAttributeValueState() => () =>
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
                    State = AttributeValueDoubleQuotedState;
                    return;

                case '&':
                    State = AttributeValueUnquotedState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;

                case '\'':
                    State = AttributeValueSingleQuotedState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    State = DataState;
                    EmitTagToken = currentTagToken;
                    return;

                case '<':
                case '=':
                case '`':
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    goto default;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                    State = AttributeValueUnquotedState;
                    return;
            }
        };
    }
}
