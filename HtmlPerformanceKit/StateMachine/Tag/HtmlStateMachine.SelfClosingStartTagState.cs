using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action selfClosingStartTagState;

    /// <summary>
    /// 8.2.4.43 Self-closing start tag state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "&gt;" (U+003E)
    /// Set the self-closing flag of the current tag token. Switch to the data state. Emit the current tag token.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Parse error. Switch to the before attribute name state. Reconsume the character.
    /// </summary>
    private void SelfClosingStartTagStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '>':
                currentTagToken.SelfClosing = true;
                State = dataState;
                EmitTagToken = currentTagToken;
                break;

            case EofMarker:
                ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                State = dataState;
                bufferReader.Reconsume(EofMarker);
                break;

            default:
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = beforeAttributeNameState;
                bufferReader.Reconsume(currentInputCharacter);
                break;
        }
    }
}