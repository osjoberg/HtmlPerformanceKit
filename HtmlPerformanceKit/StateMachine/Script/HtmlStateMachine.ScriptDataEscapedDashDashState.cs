using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action scriptDataEscapedDashDashState;

        /// <summary>
        /// 8.2.4.24 Script data escaped dash dash state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Emit a U+002D HYPHEN-MINUS character token.
        /// 
        /// "&lt;" (U+003C)
        /// Switch to the script data escaped less-than sign state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the script data state. Emit a U+003E GREATER-THAN SIGN character token.
        /// 
        /// U+0000 NULL
        /// Parse error. Switch to the script data escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Switch to the script data escaped state. Emit the current input character as a character token.
        /// </summary>
        private void ScriptDataEscapedDashDashStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    currentDataBuffer.Add('-');
                    return;

                case '<':
                    State = scriptDataEscapedLessThanSignState;
                    return;

                case '>':
                    State = scriptDataState;
                    currentDataBuffer.Add('>');
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    State = dataState;
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    State = scriptDataEscapedState;
                    return;
            }
        }
    }
}
