using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action AttributeValueSingleQuotedState;

        /// <summary>
        /// 8.2.4.39 Attribute value (single-quoted) state
        ///
        /// Consume the next input character:
        /// 
        /// "'" (U+0027)
        /// Switch to the after attribute value (quoted) state.
        /// 
        /// U+0026 AMPERSAND (&amp;)
        /// Switch to the character reference in attribute value state, with the additional allowed character being "'" (U+0027).
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current attribute's value.
        /// </summary>
        private void AttributeValueSingleQuotedStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\'':
                    State = AfterAttributeValueQuotedState;
                    return;

                case '&' when decodeHtmlCharacters:
                    State = CharacterReferenceInAttributeValueState;
                    additionalAllowedCharacter = '\'';
                    returnToState = AttributeValueSingleQuotedState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
