using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.8 Tag open state
        ///
        /// Consume the next input character:
        /// 
        /// "!" (U+0021)
        /// Switch to the markup declaration open state.
        /// 
        /// "/" (U+002F)
        /// Switch to the end tag open state.
        /// 
        /// Uppercase ASCII letter
        /// Create a new start tag token, set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point), then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
        /// 
        /// Lowercase ASCII letter
        /// Create a new start tag token, set its tag name to the current input character, then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
        /// 
        /// "?" (U+003F)
        /// Parse error. Switch to the bogus comment state.
        /// 
        /// Anything else
        /// Parse error. Switch to the data state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
        /// </summary>
        private void TagOpenState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '!':
                    State = MarkupDeclarationOpenState;
                    return;

                case '/':
                    State = EndTagOpenState;
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
                    currentTagToken.Clear();
                    currentTagToken.Name.Add((char)(currentInputCharacter + 0x20));
                    State = TagNameState;
                    return;

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    currentTagToken.Clear();
                    currentTagToken.Name.Add((char)currentInputCharacter);
                    State = TagNameState;
                    return;

                case '?':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = BogusCommentState;
                    return;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    currentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
