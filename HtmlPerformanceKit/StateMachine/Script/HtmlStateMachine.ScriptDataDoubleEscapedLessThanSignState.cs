namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.32 Script data double escaped less-than sign state
        ///
        /// Consume the next input character:
        /// 
        /// "/" (U+002F)
        /// Set the temporary buffer to the empty string. Switch to the script data double escape end state. Emit a U+002F SOLIDUS character token.
        /// 
        /// Anything else
        /// Switch to the script data double escaped state. Reconsume the current input character.
        /// </summary>
        private Action BuildScriptDataDoubleEscapedLessThanSignState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    buffers.TemporaryBuffer.Clear();
                    State = ScriptDataDoubleEscapeEndState;
                    buffers.CurrentDataBuffer.Add('/');
                    return;

                default:
                    State = ScriptDataDoubleEscapedState;
                    buffers.CurrentDataBuffer.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
