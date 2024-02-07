using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.5 RAWTEXT state
        ///
        /// Consume the next input character:
        /// 
        /// "&lt;" (U+003C)
        /// Switch to the RAWTEXT less-than sign state.
        /// 
        /// U+0000 NULL
        /// Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// 
        /// EOF
        /// Emit an end-of-file token.
        /// 
        /// Anything else
        /// Emit the current input character as a character token.
        /// </summary>
        private Action BuildRawTextState() => () =>
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '<':
                        State = RawTextLessThanSignState;
                        return;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        buffers.CurrentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                        continue;

                    case EofMarker:
                        State = DataState;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        buffers.CurrentDataBuffer.Add((char)currentInputCharacter);
                        continue;
                }
            }
        }
    }
}
