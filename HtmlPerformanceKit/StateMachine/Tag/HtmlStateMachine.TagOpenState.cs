using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action tagOpenState;

    /// <summary>
    /// 8.2.4.8 Tag open state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "!" (U+0021)
    /// Switch to the markup declaration open state.
    /// <br/>
    /// "/" (U+002F)
    /// Switch to the end tag open state.
    /// <br/>
    /// Uppercase ASCII letter
    /// Create a new start tag token, set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point), then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
    /// <br/>
    /// Lowercase ASCII letter
    /// Create a new start tag token, set its tag name to the current input character, then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
    /// <br/>
    /// "?" (U+003F)
    /// Parse error. Switch to the bogus comment state.
    /// <br/>
    /// Anything else
    /// Parse error. Switch to the data state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
    /// </summary>
    private void TagOpenStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '!':
                State = markupDeclarationOpenState;
                return;

            case '/':
                State = endTagOpenState;
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
                State = tagNameState;
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
                State = tagNameState;
                return;

            case '?':
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = bogusCommentState;
                return;

            default:
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = dataState;
                currentDataBuffer.Add('<');
                bufferReader.Reconsume(currentInputCharacter);
                return;
        }
    }
}