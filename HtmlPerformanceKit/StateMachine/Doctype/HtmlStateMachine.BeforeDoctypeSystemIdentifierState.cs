using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private readonly Action beforeDoctypeSystemIdentifierState;

    /// <summary>
    /// 8.2.4.63 Before DOCTYPE system identifier state
    /// <br/>
    /// Consume the next input character:
    /// <br/>
    /// "tab" (U+0009)
    /// "LF" (U+000A)
    /// "FF" (U+000C)
    /// U+0020 SPACE
    /// Ignore the character.
    /// <br/>
    /// U+0022 QUOTATION MARK (")
    /// Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
    /// <br/>
    /// "'" (U+0027)
    /// Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
    /// <br/>
    /// "&gt;" (U+003E)
    /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
    /// <br/>
    /// EOF
    /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
    /// <br/>
    /// Anything else
    /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
    /// </summary>
    private void BeforeDoctypeSystemIdentifierStateImplementation()
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
                currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                State = doctypeSystemIdentifierDoubleQuotedState;
                return;

            case '\'':
                currentDoctypeToken.Attributes.Add();
                currentDoctypeToken.Attributes.Current.Name.AddRange("system");
                State = doctypeSystemIdentifierSingleQuotedState;
                return;

            case '>':
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
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
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = bogusDoctypeState;
                return;
        }
    }
}