using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action bogusDoctypeState;

        /// <summary>
        /// 8.2.4.67 Bogus DOCTYPE state
        ///
        /// Consume the next input character:
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the DOCTYPE token.
        /// 
        /// EOF
        /// Switch to the data state. Emit the DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Ignore the character.
        /// </summary>
        private void BogusDoctypeStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '>':
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    return;
            }
        }
    }
}
