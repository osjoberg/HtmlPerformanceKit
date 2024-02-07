namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.17 Script data less-than sign state
        ///
        /// Consume the next input character:
        /// 
        /// "/" (U+002F)
        /// Set the temporary buffer to the empty string. Switch to the script data end tag open state.
        /// 
        /// "!" (U+0021)
        /// Switch to the script data escape start state. Emit a U+003C LESS-THAN SIGN character token and a U+0021 EXCLAMATION MARK character token.
        /// 
        /// Anything else
        /// Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
        /// </summary>
        private Action BuildScriptDataLessThanSignState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    buffers.TemporaryBuffer.Clear();
                    State = ScriptDataEndTagOpenState;
                    return;

                case '!':
                    State = ScriptDataEscapeStartState;
                    buffers.CurrentDataBuffer.Add('<');
                    buffers.CurrentDataBuffer.Add('!');
                    return;

                default:
                    State = ScriptDataState;
                    buffers.CurrentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
