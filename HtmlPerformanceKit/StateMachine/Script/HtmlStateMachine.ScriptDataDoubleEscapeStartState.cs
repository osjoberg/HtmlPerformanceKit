using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action ScriptDataDoubleEscapeStartState;

        /// <summary>
        /// 8.2.4.28 Script data double escape start state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// "/" (U+002F)
        /// "&gt;" (U+003E)
        /// If the temporary buffer is the string "script", then switch to the script data double escaped state. Otherwise, switch to the script data escaped state. Emit the current input character as a character token.
        /// 
        /// Uppercase ASCII letter
        /// Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the temporary buffer. Emit the current input character as a character token.
        /// 
        /// Lowercase ASCII letter
        /// Append the current input character to the temporary buffer. Emit the current input character as a character token.
        /// 
        /// Anything else
        /// Switch to the script data escaped state. Reconsume the current input character.
        /// </summary>
        private void ScriptDataDoubleEscapeStartStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                case '/':
                case '>':
                    if (temporaryBuffer.Equals("script"))
                    {
                        State = ScriptDataDoubleEscapedState;
                        EmitTagToken = currentTagToken;
                        return;
                    }

                    currentDataBuffer.Add((char)currentInputCharacter);
                    State = ScriptDataEscapedState;
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
                    temporaryBuffer.Add((char)(currentInputCharacter + 0x20));
                    currentDataBuffer.Add((char)currentInputCharacter);
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
                    temporaryBuffer.Add((char)currentInputCharacter);
                    currentDataBuffer.Add((char)currentInputCharacter);
                    return;

                default:
                    State = ScriptDataEscapedState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
