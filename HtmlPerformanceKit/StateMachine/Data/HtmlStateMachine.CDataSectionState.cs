using System;
using System.Diagnostics.CodeAnalysis;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
        private readonly Action cDataSectionState;

        /// <summary>
        /// 8.2.4.68 CDATA section state
        /// <br/>
        /// Switch to the data state.
        /// <br/>
        /// Consume every character up to the next occurrence of the three character sequence U+005D RIGHT SQUARE BRACKET U+005D RIGHT SQUARE BRACKET U+003E GREATER-THAN SIGN (&#x5d;&#x5d;>), or the end of the file(EOF), whichever comes first.Emit a series of character tokens consisting of all the characters consumed except the matching three character sequence at the end(if one was found before the end of the file).
        /// <br/>
        /// If the end of the file was reached, reconsume the EOF character.
        /// </summary>
        private void CDataSectionStateImplementation()
        {
            State = dataState;

            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();
                switch (currentInputCharacter)
                {
                    case ']':
                        var nextInputCharacter = bufferReader.Consume();
                        if (nextInputCharacter == ']' && bufferReader.Peek() == '>')
                        {
                            bufferReader.Consume();
                            EmitDataBuffer = currentDataBuffer;
                        }
                        else
                        {
                            bufferReader.Reconsume(nextInputCharacter);
                        }

                        break;

                    case EofMarker:
                        EmitDataBuffer = currentDataBuffer;
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
