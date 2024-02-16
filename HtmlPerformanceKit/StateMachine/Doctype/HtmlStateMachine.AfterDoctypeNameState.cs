using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private readonly Action afterDoctypeNameState;

        /// <summary>
        /// 8.2.4.55 After DOCTYPE name state
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
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// If the six characters starting from the current input character are an ASCII case-insensitive match for the word "PUBLIC", then consume those characters and switch to the after DOCTYPE public keyword state.
        /// 
        /// Otherwise, if the six characters starting from the current input character are an ASCII case-insensitive match for the word "SYSTEM", then consume those characters and switch to the after DOCTYPE system keyword state.
        /// 
        /// Otherwise, this is a parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
        /// </summary>
        private void AfterDoctypeNameStateImplementation()
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
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = dataState;
                    EmitDoctypeToken = currentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    bufferReader.Reconsume(currentInputCharacter);
                    var peek = bufferReader.Peek(6);

                    if (Public.AsSpan().Equals(peek.Span, StringComparison.OrdinalIgnoreCase))
                    {
                        bufferReader.Consume(Public.Length);
                        State = afterDoctypePublicKeywordState;
                        return;
                    }

                    if (System.AsSpan().Equals(peek.Span, StringComparison.OrdinalIgnoreCase))
                    {
                        bufferReader.Consume(System.Length);
                        State = afterDoctypeSystemKeywordState;
                        return;
                    }

                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = bogusDoctypeState;
                    return;
            }
        }
    }
}
