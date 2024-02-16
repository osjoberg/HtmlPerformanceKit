using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action rcDataLessThanSignState;

        /// <summary>
        /// 8.2.4.11 RCDATA less-than sign state
        ///
        /// Consume the next input character:
        /// 
        /// "/" (U+002F)
        /// Set the temporary buffer to the empty string. Switch to the RCDATA end tag open state.
        /// 
        /// Anything else
        /// Switch to the RCDATA state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
        /// </summary>
        private void RcDataLessThanSignStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    temporaryBuffer.Clear();
                    State = rcDataEndTagOpenState;
                    return;

                default:
                    State = rcDataState;
                    currentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
