using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
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
        private Action BuildScriptDataEscapedDashDashState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    buffers.CurrentDataBuffer.Add('-');
                    return;

                case '<':
                    State = ScriptDataEscapedLessThanSignState;
                    return;

                case '>':
                    State = ScriptDataState;
                    buffers.CurrentDataBuffer.Add('>');
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    buffers.CurrentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    State = DataState;
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    State = ScriptDataEscapedState;
                    return;
            }
        }
    }
}
