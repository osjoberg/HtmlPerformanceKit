using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.61 Between DOCTYPE public and system identifiers state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Ignore the character.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current DOCTYPE token.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
        /// 
        /// "'" (U+0027)
        /// Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
        /// </summary>
        private void BetweenDoctypePublicAndSystemIdentifiersState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    return;

                case '>':
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case '"':
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.Append("system");
                    State = DoctypeSystemIdentifierDoubleQuotedState;
                    return;

                case '\'':
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.Append("system");
                    State = DoctypeSystemIdentifierSingleQuotedState;
                    return;

                case EofMarker:
                    ParseError = ParseErrorMessage.UnexpectedEndOfFile;
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
                    State = BogusDoctypeState;
                    return;
            }
        }
    }
}
