using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action afterAttributeNameState;

        /// <summary>
        /// 8.2.4.36 After attribute name state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Ignore the character.
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
        /// Start a new attribute in the current tag token. Set that attribute's name to the lowercase version of the current input character (add 0x0020 to the character's code point), and its value to the empty string. Switch to the attribute name state.
        /// 
        /// U+0000 NULL
        /// Parse error. Start a new attribute in the current tag token. Set that attribute's name to a U+FFFD REPLACEMENT CHARACTER character, and its value to the empty string. Switch to the attribute name state.
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
        /// Start a new attribute in the current tag token. Set that attribute's name to the current input character, and its value to the empty string. Switch to the attribute name state.
        /// </summary>
        private void AfterAttributeNameStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    return;

                case '/':
                    State = selfClosingStartTagState;               
                    return;

                case '=':
                    State = beforeAttributeValueState;
                    return;

                case '>':
                    State = dataState;
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
                    currentTagToken.Attributes.Add();
                    currentTagToken.Attributes.Current.Name.Add((char)(currentInputCharacter + 0x20));
                    State = attributeNameState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentTagToken.Attributes.Add();
                    currentTagToken.Attributes.Current.Name.Add(HtmlChar.ReplacementCharacter);
                    State = attributeNameState;
                    return;

                case '"':
                case '\'':
                case '<':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    goto default;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentTagToken.Attributes.Add();
                    currentTagToken.Attributes.Current.Name.Add((char)currentInputCharacter);
                    State = attributeNameState;
                    return;
            }
        }
    }
}
