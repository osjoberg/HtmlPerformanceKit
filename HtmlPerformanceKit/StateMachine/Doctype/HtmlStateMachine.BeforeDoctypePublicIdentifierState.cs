using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.57 Before DOCTYPE public identifier state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Ignore the character.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (double-quoted) state.
        /// 
        /// "'" (U+0027)
        /// Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (single-quoted) state.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
        /// </summary>
        private Action BuildBeforeDoctypePublicIdentifierState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    return;

                case '"':
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("public");
                    State = DoctypePublicIdentifierDoubleQuotedState;
                    return;

                case '\'':
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("public");
                    State = DoctypePublicIdentifierSingleQuotedState;
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = BogusDoctypeState;
                    return;
            }
        };
    }
}
