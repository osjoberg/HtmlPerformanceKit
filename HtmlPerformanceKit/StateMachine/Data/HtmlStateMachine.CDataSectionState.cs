using System;

namespace HtmlPerformanceKit.StateMachine
{
    /// <summary>
    /// 8.2.4.68 CDATA section state
    /// 
    /// Switch to the data state.
    /// 
    /// Consume every character up to the next occurrence of the three character sequence U+005D RIGHT SQUARE BRACKET U+005D RIGHT SQUARE BRACKET U+003E GREATER-THAN SIGN (]]>), or the end of the file(EOF), whichever comes first.Emit a series of character tokens consisting of all the characters consumed except the matching three character sequence at the end(if one was found before the end of the file).
    /// 
    /// If the end of the file was reached, reconsume the EOF character.    /// </summary>
    internal partial class HtmlStateMachine
    {
        private Action BuildCDataSectionState() => () =>
        {
            State = DataState;

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
                            EmitDataBuffer = buffers.CurrentDataBuffer;
                        }
                        else
                        {
                            bufferReader.Reconsume(nextInputCharacter);
                        }

                        break;

                    case EofMarker:
                        EmitDataBuffer = buffers.CurrentDataBuffer;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    default:
                        buffers.CurrentDataBuffer.Add((char)currentInputCharacter);
                        continue;
                }
            }
        };
    }
}
