using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
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
        private Action BuildBogusDoctypeState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '>':
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    return;
            }
        };
    }
}
