using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action ScriptDataEscapeStartDashState;

        /// <summary>
        /// 8.2.4.21 Script data escape start dash state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the script data escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
        /// 
        /// Anything else
        /// Switch to the script data state. Reconsume the current input character.
        /// </summary>
        private void ScriptDataEscapeStartDashStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = ScriptDataEscapedDashState;
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
