namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.20 Script data escape start state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the script data escape start dash state. Emit a U+002D HYPHEN-MINUS character token.
        /// 
        /// Anything else
        /// Switch to the script data state. Reconsume the current input character.
        /// </summary>
        private void ScriptDataEscapeStartState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = ScriptDataEscapeStartDashState;
                    currentDataBuffer.Add('-');
                    return;

                default:
                    State = ScriptDataState;
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
