using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action afterAttributeValueQuotedState;

    /// <summary>
    /// 8.2.4.42 After attribute value (quoted) state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "tab" (U+0009)
    /// "LF" (U+000A)
    /// "FF" (U+000C)
    /// U+0020 SPACE
    /// Switch to the before attribute name state.
    /// <br/>
    /// "/" (U+002F)
    /// Switch to the self-closing start tag state.
    /// <br/>
    /// "&gt;" (U+003E)
    /// Switch to the data state. Emit the current tag token.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Parse error. Switch to the before attribute name state. Reconsume the character.
    /// </summary>
    private void AfterAttributeValueQuotedStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '\t':
            case '\n':
            case '\r':
            case ' ':
                State = beforeAttributeNameState;
                return;

            case '/':
                State = selfClosingStartTagState;
                return;

            case '>':
                State = dataState;
                EmitTagToken = currentTagToken;
                return;

            case EofMarker:
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                State = dataState;
                bufferReader.Reconsume(EofMarker);
                return;

            default:
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = beforeAttributeNameState;
                bufferReader.Reconsume(currentInputCharacter);
                return;
        }
    }
}