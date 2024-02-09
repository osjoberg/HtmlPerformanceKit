using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action AfterDoctypePublicKeywordState;

        public const string Public = "PUBLIC";
        public const string System = "SYSTEM";

        /// <summary>
        /// 8.2.4.56 After DOCTYPE public keyword state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before DOCTYPE public identifier state.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Parse error. Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (double-quoted) state.
        /// 
        /// "'" (U+0027)
        /// Parse error. Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (single-quoted) state.
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
        private void AfterDoctypePublicKeywordStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = BeforeDoctypePublicIdentifierState;
                    return;

                case '"':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("public");
                    State = DoctypePublicIdentifierDoubleQuotedState;
                    return;

                case '\'':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("public");
                    State = DoctypePublicIdentifierSingleQuotedState;
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    break;

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
        }
    }
}
