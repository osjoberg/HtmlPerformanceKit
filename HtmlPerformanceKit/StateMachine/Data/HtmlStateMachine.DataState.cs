using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action dataState;

        /// <summary>
        /// 8.2.4.1 Data state
        /// <br/>
        /// Consume the next input character:
        /// <br/>
        /// U+0026 AMPERSAND(&amp;)
        /// Switch to the character reference in data state.
        /// <br/>
        /// "&lt;" (U+003C)
        /// Switch to the tag open state.
        /// <br/>
        /// U+0000 NULL
        /// Parse error.Emit the current input character as a character token.
        /// <br/>
        /// EOF
        /// Emit an end-of-file token.
        /// <br/>
        /// Anything else
        /// Emit the current input character as a character token.
        /// </summary>
        private void DataStateImplementation()
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '&' when decodeHtmlCharacters:
                        State = characterReferenceInDataState;
                        return;

                    case '<':
                        State = tagOpenState;
                        if (currentDataBuffer.Length > 0)
                        {
                            EmitDataBuffer = currentDataBuffer;
                        }

                        return;

                    case HtmlChar.Null:
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        currentDataBuffer.Add(HtmlChar.Null);
                        break;

                    case EofMarker:
                        if (currentDataBuffer.Length > 0)
                        {
                            EmitDataBuffer = currentDataBuffer;
                        }

                        Eof = true;
                        return;

                    default:
                        currentDataBuffer.Add((char)currentInputCharacter);
                        break;
                }
            }
        }
    }
}
