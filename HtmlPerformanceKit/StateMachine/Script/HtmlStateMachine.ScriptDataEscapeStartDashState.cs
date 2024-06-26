using System;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action scriptDataEscapeStartDashState;

    /// <summary>
    /// 8.2.4.21 Script data escape start dash state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "-" (U+002D)
    /// Switch to the script data escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
    /// <br/>
    /// Anything else
    /// Switch to the script data state. Reconsume the current input character.
    /// </summary>
    private void ScriptDataEscapeStartDashStateImplementation()
    {
        var currentInputCharacter = bufferReader.Consume();

        switch (currentInputCharacter)
        {
            case '-':
                State = scriptDataEscapedDashState;
                currentDataBuffer.Add('-');
                return;

            default:
                State = scriptDataState;
                bufferReader.Reconsume(currentInputCharacter);
                return;
        }
    }
}