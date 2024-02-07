using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    /// <summary>
    /// 8.2.4.1 Data state
    /// 
    /// Consume the next input character:
    /// 
    /// U+0026 AMPERSAND(&)
    /// Switch to the character reference in data state.
    /// 
    /// "&lt;" (U+003C)
    /// Switch to the tag open state.
    /// 
    /// U+0000 NULL
    /// Parse error.Emit the current input character as a character token.
    /// 
    /// EOF
    /// Emit an end-of-file token.
    /// 
    /// Anything else
    /// Emit the current input character as a character token.
    ///     
    /// 
    /// /// </summary>
    internal partial class HtmlStateMachine
    {
        private Action BuildDataState() => () =>
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '&' when decodeHtmlCharacters:
                        State = CharacterReferenceInDataState;
                        return;

                    case '<':
                        State = TagOpenState;
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
        };
    }
}
