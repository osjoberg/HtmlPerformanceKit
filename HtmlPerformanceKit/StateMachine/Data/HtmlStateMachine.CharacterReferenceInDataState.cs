using System;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action characterReferenceInDataState;

    /// <summary>
    /// 8.2.4.2 Character reference in data state
    /// <br/>
    /// Switch to the data state.
    /// <br/>
    /// Attempt to consume a character reference, with no additional allowed character.
    /// <br/>
    /// If nothing is returned, emit a U+0026 AMPERSAND character (&amp;) token.
    /// <br/>
    /// Otherwise, emit the character tokens that were returned.
    /// </summary>
    private void CharacterReferenceInDataStateImplementation()
    {
        additionalAllowedCharacter = '\t'; // Works as no additional character since '\t' is always allowed.

        var characters = ConsumeCharacterReference();
        if (characters.Length == 0)
        {
            currentDataBuffer.Add('&');
        }
        else
        {
            currentDataBuffer.AddRange(characters);
        }

        State = dataState;
    }
}