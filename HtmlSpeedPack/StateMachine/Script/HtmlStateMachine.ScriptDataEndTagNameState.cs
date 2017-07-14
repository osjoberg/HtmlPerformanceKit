namespace HtmlSpeedPack.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.19 Script data end tag name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// If the current end tag token is an appropriate end tag token, then switch to the before attribute name state. Otherwise, treat it as per the "anything else" entry below.
        ///  
        /// "/" (U+002F)
        /// If the current end tag token is an appropriate end tag token, then switch to the self-closing start tag state. Otherwise, treat it as per the "anything else" entry below.
        ///  
        /// "&gt;" (U+003E)
        /// If the current end tag token is an appropriate end tag token, then switch to the data state and emit the current tag token. Otherwise, treat it as per the "anything else" entry below.
        ///  
        /// Uppercase ASCII letter
        /// Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name. Append the current input character to the temporary buffer.
        ///  
        /// Lowercase ASCII letter
        /// Append the current input character to the current tag token's tag name. Append the current input character to the temporary buffer.
        ///  
        /// Anything else
        /// Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token, a U+002F SOLIDUS character token, and a character token for each of the characters in the temporary buffer (in the order they were added to the buffer). Reconsume the current input character.
        /// </summary>
        private void ScriptDataEndTagNameState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    if (temporaryBuffer.Equals(appropriateTagName.ToString()))
                    {
                        State = BeforeAttributeNameState;
                        return;                        
                    }

                    goto default;

                case '/':
                    if (temporaryBuffer.Equals(appropriateTagName.ToString()))
                    {
                        State = SelfClosingStartTagState;
                        return;
                    }

                    goto default;

                case '>':
                    if (temporaryBuffer.Equals(appropriateTagName.ToString()))
                    {
                        if (currentDataBuffer.Length > 0)
                        {
                            EmitDataBuffer = currentDataBuffer;
                            bufferReader.Reconsume('>');
                            return;
                        }

                        State = DataState;
                        EmitTagToken = currentTagToken;
                        return;
                    }

                    goto default;

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
                    currentTagToken.Name.Append((char)(currentInputCharacter + 0x20));
                    temporaryBuffer.Append((char)currentInputCharacter);
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
                    currentTagToken.Name.Append((char)currentInputCharacter);
                    temporaryBuffer.Append((char)currentInputCharacter);
                    return;

                default:
                    State = ScriptDataState;
                    currentDataBuffer.Append('<');
                    currentDataBuffer.Append('/');
                    currentDataBuffer.Append(temporaryBuffer.ToString());
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
