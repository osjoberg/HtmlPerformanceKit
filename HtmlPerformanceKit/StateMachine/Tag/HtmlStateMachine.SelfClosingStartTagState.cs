using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.43 Self-closing start tag state
        ///
        /// Consume the next input character:
        /// 
        /// "&gt;" (U+003E)
        /// Set the self-closing flag of the current tag token. Switch to the data state. Emit the current tag token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Switch to the before attribute name state. Reconsume the character.
        /// </summary>
        private Action BuildSelfClosingStartTagState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '>':
                    currentTagToken.SelfClosing = true;
                    State = DataState;
                    EmitTagToken = currentTagToken;
                    break;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    break;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = BeforeAttributeNameState;
                    bufferReader.Reconsume(currentInputCharacter);
                    break;
            }
        };
    }
}
