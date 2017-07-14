namespace HtmlPerformanceKit.StateMachine
{
    /// <summary>
    /// 8.2.4.41 Character reference in attribute value state
    /// 
    /// Attempt to consume a character reference.
    /// 
    /// If nothing is returned, append a U+0026 AMPERSAND character(&) to the current attribute's value.
    /// 
    /// Otherwise, append the returned character tokens to the current attribute's value.
    /// 
    /// Finally, switch back to the attribute value state that switched into this state.    
    /// </summary>
    internal partial class HtmlStateMachine
    {
        private void CharacterReferenceInAttributeValueState()
        {
            var characters = ConsumeCharacterReference();
            if (characters == null)
            {
                currentTagToken.Attributes.Current.Value.Append('&');
            }
            else
            {
                currentTagToken.Attributes.Current.Value.Append(characters, characters.Length);
            }

            State = returnToState;
        }
    }
}
