using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action ScriptDataLessThanSignState;

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
        private void ScriptDataLessThanSignStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '/':
                    temporaryBuffer.Clear();
                    State = ScriptDataEndTagOpenState;
                    return;

                case '!':
                    State = ScriptDataEscapeStartState;
                    currentDataBuffer.Add('<');
                    currentDataBuffer.Add('!');
                    return;

                default:
                    State = ScriptDataState;
                    currentDataBuffer.Add('<');
                    bufferReader.Reconsume(currentInputCharacter);
                    return;
            }
        }
    }
}
