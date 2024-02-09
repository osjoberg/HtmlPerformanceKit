using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action AfterDoctypeSystemKeywordState;

        /// <summary>
        /// 8.2.4.62 After DOCTYPE system keyword state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the before DOCTYPE system identifier state.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
        /// 
        /// "'" (U+0027)
        /// Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
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
        private void AfterDoctypeSystemKeywordStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = BeforeDoctypeSystemIdentifierState;
                    return;

                case '"':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                    State = DoctypeSystemIdentifierDoubleQuotedState;
                    return;

                case '\'':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                    State = DoctypeSystemIdentifierSingleQuotedState;
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