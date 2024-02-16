using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action scriptDataEscapedEndTagOpenState;

        /// <summary>
        /// 8.2.4.18 Script data end tag open state
        ///
        /// Consume the next input character:
        /// 
        /// Uppercase ASCII letter
        /// Create a new end tag token, and set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point). Append the current input character to the temporary buffer. Finally, switch to the script data end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
        /// 
        /// Lowercase ASCII letter
        /// Create a new end tag token, and set its tag name to the current input character. Append the current input character to the temporary buffer. Finally, switch to the script data end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
        /// 
        /// Anything else
        /// Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the current input character.
        /// </summary>
        private void ScriptDataEscapedEndTagOpenStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
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
                    currentTagToken.EndTag = true;
                    currentTagToken.Name.Add((char)(currentInputCharacter + 0x20));
                    temporaryBuffer.Add((char)currentInputCharacter);
                    State = scriptDataEscapedEndTagNameState;
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
                    currentTagToken.EndTag = true;
                    currentTagToken.Name.Add((char)currentInputCharacter);
                    temporaryBuffer.Add((char)currentInputCharacter);
                    State = scriptDataEscapedEndTagNameState;
                    return;

                default:
                    State = scriptDataState;
                    currentDataBuffer.Add('<');
                    currentDataBuffer.Add('/');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
