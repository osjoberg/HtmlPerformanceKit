namespace HtmlSpeedPack.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.2 Character reference in data state
        /// 
        /// Switch to the data state.
        /// 
        /// Attempt to consume a character reference, with no additional allowed character.
        /// 
        /// If nothing is returned, emit a U+0026 AMPERSAND character (&) token.
        /// 
        /// Otherwise, emit the character tokens that were returned.
        /// </summary>
        private void CharacterReferenceInDataState()
        {
            additionalAllowedCharacter = '\t'; // Works as no additional character since '\t' is always allowed.

            var characters = ConsumeCharacterReference();
            if (characters == null)
            {
                currentDataBuffer.Append('&');
            }
            else
            {
                currentDataBuffer.Append(characters, characters.Length);
            }

            State = DataState;
        }
    }
}
