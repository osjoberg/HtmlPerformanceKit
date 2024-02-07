namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
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
        private Action BuildRcDataLessThanSignState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    buffers.TemporaryBuffer.Clear();
                    State = RcDataEndTagOpenState;
                    return;

                default:
                    State = RcDataState;
                    buffers.CurrentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
