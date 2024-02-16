using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action rcDataState;

        /// <summary>
        /// 8.2.4.3 RCDATA state
        ///
        /// Consume the next input character:
        /// 
        /// U+0026 AMPERSAND (&amp;)
        /// Switch to the character reference in RCDATA state.
        /// 
        /// "&lt;" (U+003C)
        /// Switch to the RCDATA less-than sign state.
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
        private void RcDataStateImplementation()
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '&' when decodeHtmlCharacters:
                        State = characterReferenceInRcDataState;
                        return;

                    case '<':
                        State = rcDataLessThanSignState;
                        return;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        currentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                        continue;

                    case EofMarker:
                        State = dataState;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        currentDataBuffer.Add((char)currentInputCharacter);
                        continue;
                }
            }
        }
    }
}
