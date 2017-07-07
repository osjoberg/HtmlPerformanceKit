namespace HtmlSpeedPack.StateMachine
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
    internal partial class HtmlStateMachine
    {
        private void CharacterReferenceInDataState()
        {
            currentDataBuffer.Append('&');
            currentDataBuffer.MayHaveCharacterReference();
            State = DataState;
        }
    }
}
