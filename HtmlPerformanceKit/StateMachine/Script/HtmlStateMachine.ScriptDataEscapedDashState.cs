using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action scriptDataEscapedDashState;

        /// <summary>
        /// 8.2.4.23 Script data escaped dash state
        /// <br/>
        /// Consume the next input character:
        /// <br/>
        /// "-" (U+002D)
        /// Switch to the script data escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
        /// <br/>
        /// "&lt;" (U+003C)
        /// Switch to the script data escaped less-than sign state.
        /// <br/>
        /// U+0000 NULL
        /// Parse error. Switch to the script data escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// <br/>
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// <br/>
        /// Anything else
        /// Switch to the script data escaped state. Emit the current input character as a character token.
        /// </summary>
        private void ScriptDataEscapedDashStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    State = scriptDataEscapedDashDashState;
                    currentDataBuffer.Add('-');
                    return;

                case '<':
                    State = scriptDataEscapedLessThanSignState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    State = scriptDataEscapedState;
                    currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    State = scriptDataEscapedState;
                    currentDataBuffer.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
