using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action doctypeNameState;

        /// <summary>
        /// 8.2.4.54 DOCTYPE name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the after DOCTYPE name state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current DOCTYPE token.
        /// 
        /// Uppercase ASCII letter
        /// Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current DOCTYPE token's name.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's name.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current DOCTYPE token's name.
        /// </summary>
        private void DoctypeNameStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = afterDoctypeNameState;
                    return;

                case '>':
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
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
                    currentDoctypeToken.Name.Add((char)(currentInputCharacter + 0x20));
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentDoctypeToken.Name.Add(HtmlChar.ReplacementCharacter);
                    State = doctypeNameState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentDoctypeToken.Name.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
