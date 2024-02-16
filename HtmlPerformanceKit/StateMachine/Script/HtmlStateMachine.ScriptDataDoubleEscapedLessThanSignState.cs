using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action scriptDataDoubleEscapedLessThanSignState;

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
        private void ScriptDataDoubleEscapedLessThanSignStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    temporaryBuffer.Clear();
                    State = scriptDataDoubleEscapeEndState;
                    currentDataBuffer.Add('/');
                    return;

                default:
                    State = scriptDataDoubleEscapedState;
                    currentDataBuffer.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
