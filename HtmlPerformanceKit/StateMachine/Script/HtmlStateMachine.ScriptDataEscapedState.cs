using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action ScriptDataEscapedState;

        /// <summary>
        /// 8.2.4.22 Script data escaped state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Switch to the script data escaped dash state. Emit a U+002D HYPHEN-MINUS character token.
        /// 
        /// "&lt;" (U+003C)
        /// Switch to the script data escaped less-than sign state.
        /// 
        /// U+0000 NULL
        /// Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// 
        /// EOF
        /// Switch to the data state. Parse error. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Emit the current input character as a character token.
        /// </summary>
        private void ScriptDataEscapedStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = ScriptDataEscapedDashState;
                    currentDataBuffer.Add('-');
                    return;

                case '<':
                    State = ScriptDataEscapedLessThanSignState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    State = DataState;
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentDataBuffer.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
