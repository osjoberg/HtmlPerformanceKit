using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.53 Before DOCTYPE name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Ignore the character.
        /// 
        /// Uppercase ASCII letter
        /// Create a new DOCTYPE token. Set the token's name to the lowercase version of the current input character (add 0x0020 to the character's code point). Switch to the DOCTYPE name state.
        /// 
        /// U+0000 NULL
        /// Parse error. Create a new DOCTYPE token. Set the token's name to a U+FFFD REPLACEMENT CHARACTER character. Switch to the DOCTYPE name state.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Create a new DOCTYPE token. Set its force-quirks flag to on. Switch to the data state. Emit the token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Create a new DOCTYPE token. Set its force-quirks flag to on. Emit the token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Create a new DOCTYPE token. Set the token's name to the current input character. Switch to the DOCTYPE name state.
        /// </summary>
        private void BeforeDoctypeNameState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
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
                    currentDoctypeToken.Clear();
                    currentDoctypeToken.Name.Append((char)(currentInputCharacter + 0x20));
                    return;

                case HtmlChar.Null:
                    ParseError = ParseErrorMessage.UnexpectedNullCharacterInStream;
                    currentDoctypeToken.Clear();
                    currentDoctypeToken.Name.Append(HtmlChar.ReplacementCharacter);
                    State = DoctypeNameState;
                    return;

                case '>':
                    ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
                    currentDoctypeToken.Clear();
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    currentDoctypeToken.Clear();
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentDoctypeToken.Clear();
                    currentDoctypeToken.Name.Append((char)currentInputCharacter);
                    State = DoctypeNameState;
                    return;
            }
        }
    }
}
