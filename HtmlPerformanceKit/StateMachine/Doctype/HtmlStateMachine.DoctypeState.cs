using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action doctypeState;

        /// <summary>
        /// 8.2.4.52 DOCTYPE state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before DOCTYPE name state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Create a new DOCTYPE token. Set its force-quirks flag to on. Emit the token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Switch to the before DOCTYPE name state. Reconsume the character.
        /// </summary>
        private void DoctypeStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = beforeDoctypeNameState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = beforeDoctypeNameState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
