using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action afterDoctypePublicIdentifierState;

        /// <summary>
        /// 8.2.4.60 After DOCTYPE public identifier state
        ///
        /// Consume the next input character:
        /// 
        /// "tab" (U+0009)
        /// "LF" (U+000A)
        /// "FF" (U+000C)
        /// U+0020 SPACE
        /// Switch to the between DOCTYPE public and system identifiers state.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the data state. Emit the current DOCTYPE token.
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
        /// 
        /// "'" (U+0027)
        /// Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
        /// </summary>
        private void AfterDoctypePublicIdentifierStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                    State = betweenDoctypePublicAndSystemIdentifiersState;
                    return;

                case '>':
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case '"':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                    State = doctypeSystemIdentifierDoubleQuotedState;
                    return;

                case '\'':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Add();
                    currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                    State = doctypeSystemIdentifierSingleQuotedState;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = bogusDoctypeState;
                    return;
            }
        }
    }
}
