namespace HtmlPerformanceKit.StateMachine
{

    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.14 RAWTEXT less-than sign state
        ///
        /// Consume the next input character:
        /// 
        /// "/" (U+002F)
        /// Set the temporary buffer to the empty string. Switch to the RAWTEXT end tag open state.
        /// 
        /// Anything else
        /// Switch to the RAWTEXT state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
        /// </summary>
        private Action BuildRawTextLessThanSignState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    buffers.TemporaryBuffer.Clear();
                    State = RawTextEndTagOpenState;
                    return;

                default:
                    State = RawTextState;
                    buffers.CurrentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
