using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.25 Script data escaped less-than sign state
        ///
        /// Consume the next input character:
        /// 
        /// "/" (U+002F)
        /// Set the temporary buffer to the empty string. Switch to the script data escaped end tag open state.
        /// 
        /// Uppercase ASCII letter
        /// Set the temporary buffer to the empty string. Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the temporary buffer. Switch to the script data double escape start state. Emit a U+003C LESS-THAN SIGN character token and the current input character as a character token.
        /// 
        /// Lowercase ASCII letter
        /// Set the temporary buffer to the empty string. Append the current input character to the temporary buffer. Switch to the script data double escape start state. Emit a U+003C LESS-THAN SIGN character token and the current input character as a character token.
        /// 
        /// Anything else
        /// Switch to the script data escaped state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
        /// </summary>
        private Action BuildScriptDataEscapedLessThanSignState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    temporaryBuffer.Clear();
                    State = ScriptDataEscapedEndTagOpenState;
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
                    temporaryBuffer.Clear();
                    temporaryBuffer.Add((char)(currentInputCharacter + 0x20));
                    State = ScriptDataDoubleEscapeStartState;
                    currentDataBuffer.Add('<');
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
                    temporaryBuffer.Clear();
                    temporaryBuffer.Add((char)currentInputCharacter);
                    State = ScriptDataDoubleEscapeStartState;
                    currentDataBuffer.Add('<');
                    currentDataBuffer.Add((char)currentInputCharacter);
                    return;

                default:
                    State = ScriptDataEscapedState;
                    currentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        };
    }
}
