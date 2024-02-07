using System;

namespace HtmlPerformanceKit.StateMachine
{
    /// <summary>
    /// 8.2.4.4 Character reference in RCDATA state
    ///
    /// Switch to the RCDATA state.
    /// 
    /// Attempt to consume a character reference, with no additional allowed character.
    /// 
    /// If nothing is returned, emit a U+0026 AMPERSAND character (&amp;) token.
    /// 
    /// Otherwise, emit the character tokens that were returned.
    /// </summary>
    internal partial class HtmlStateMachine
    {
        private Action BuildCharacterReferenceInRcDataState() => () =>
        {
            State = RcDataState;
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
        };
    }
}
