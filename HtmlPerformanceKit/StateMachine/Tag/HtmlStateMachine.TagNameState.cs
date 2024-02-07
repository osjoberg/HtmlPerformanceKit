using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.10 Tag name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before attribute name state.
        /// 
        /// "/" (U+002F)
        /// Switch to the self-closing start tag state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current tag token.
        /// 
        /// Uppercase ASCII letter
        /// Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current tag token's tag name.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current tag token's tag name.
        /// </summary>
        private Action BuildTagNameState() => () =>
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
                        State = BeforeAttributeNameState;
                        return;

                    case '/':
                        State = SelfClosingStartTagState;
                        return;

                    case '>':
                        State = DataState;
                        EmitTagToken = buffers.CurrentTagToken;
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
                        buffers.CurrentTagToken.Name.Add((char)(currentInputCharacter + 0x20));
                        break;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                        buffers.CurrentTagToken.Name.Add(HtmlChar.ReplacementCharacter);
                        break;

                    case EofMarker:
                        ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                        State = DataState;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        buffers.CurrentTagToken.Name.Add((char)currentInputCharacter);
                        break;
                }
            }
        }
    }
}
