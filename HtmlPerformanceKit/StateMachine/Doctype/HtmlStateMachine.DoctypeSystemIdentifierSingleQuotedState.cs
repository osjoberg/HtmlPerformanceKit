using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action doctypeSystemIdentifierSingleQuotedState;

        /// <summary>
        /// 8.2.4.65 DOCTYPE system identifier (single-quoted) state
        ///
        /// Consume the next input character:
        /// 
        /// "'" (U+0027)
        /// Switch to the after DOCTYPE system identifier state.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's system identifier.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current DOCTYPE token's system identifier.
        /// </summary>
        private void DoctypeSystemIdentifierSingleQuotedStateImplementation()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '"':
                    State = afterDoctypeSystemIdentifierState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    currentDoctypeToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    currentDoctypeToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
