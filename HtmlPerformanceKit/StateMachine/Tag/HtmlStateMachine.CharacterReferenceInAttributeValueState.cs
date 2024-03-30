using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action characterReferenceInAttributeValueState;

        /// <summary>
        /// 8.2.4.41 Character reference in attribute value state
        /// <br/>
        /// Attempt to consume a character reference.
        /// <br/>
        /// If nothing is returned, append a U+0026 AMPERSAND character(&amp;) to the current attribute's value.
        /// <br/>
        /// Otherwise, append the returned character tokens to the current attribute's value.
        /// <br/>
        /// Finally, switch back to the attribute value state that switched into this state.
        /// </summary>
        private void CharacterReferenceInAttributeValueStateImplementation()
        {
            var characters = ConsumeCharacterReference();
            if (characters.Length == 0)
            {
                currentTagToken.Attributes.Current.Value.Add('&');
            }
            else
            {
                currentTagToken.Attributes.Current.Value.AddRange(characters);
            }

            State = returnToState;
        }
    }
}
