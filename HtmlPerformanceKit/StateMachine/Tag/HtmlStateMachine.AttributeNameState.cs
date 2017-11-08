using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.35 Attribute name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the after attribute name state.
        /// 
        /// "/" (U+002F)
        /// Switch to the self-closing start tag state.
        /// 
        /// "=" (U+003D)
        /// Switch to the before attribute value state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current tag token.
        /// 
        /// Uppercase ASCII letter
        /// Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current attribute's name.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's name.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// "'" (U+0027)
        /// "&lt;" (U+003C)
        /// Parse error. Treat it as per the "anything else" entry below.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current attribute's name.
        /// </summary>
        private void AttributeNameState()
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
                        State = AfterAttributeNameState;
                        return;

                    case '/':
                        State = SelfClosingStartTagState;
                        return;

                    case '=':
                        State = BeforeAttributeValueState;
                        return;

                    case '>':
                        State = DataState;
                        EmitTagToken = currentTagToken;
                        return;

                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        currentTagToken.Attributes.Current.Name.Add((char)(currentInputCharacter + 0x20));
                        break;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                        currentTagToken.Attributes.Current.Name.Add(HtmlChar.ReplacementCharacter);
                        break;

                    case '"':
                    case '\'':
                    case '<':
                        ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                        goto default;

                    case EofMarker:
                        ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                        State = DataState;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        currentTagToken.Attributes.Current.Name.Add((char)currentInputCharacter);
                        break;
                }
            }
        }
    }
}
