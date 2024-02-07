using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.38 Attribute value (double-quoted) state
        ///
        /// Consume the next input character:
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Switch to the after attribute value (quoted) state.
        /// 
        /// U+0026 AMPERSAND (&amp;)
        /// Switch to the character reference in attribute value state, with the additional allowed character being U+0022 QUOTATION MARK (").
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
        private Action BuildAttributeValueDoubleQuotedState() => () =>
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '"':
                        State = AfterAttributeValueQuotedState;
                        return;

                    case '&' when !skipDecodingCharacterReferences:
                        State = CharacterReferenceInAttributeValueState;
                        additionalAllowedCharacter = '"';
                        returnToState = AttributeValueDoubleQuotedState;
                        return;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                        currentTagToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                        break;

                    case EofMarker:
                        ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                        State = DataState;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        currentTagToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                        break;
                }
            }
        };
    }
}
