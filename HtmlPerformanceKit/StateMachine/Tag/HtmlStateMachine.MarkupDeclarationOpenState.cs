using System;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine;

internal partial class HtmlStateMachine
{
    private const string CommentMarker = "--";
    private const string DoctypeMarker = "doctype";
    private const string CDataMarker = "[CDATA[";

    private readonly Action markupDeclarationOpenState;

    /// <summary>
    /// 8.2.4.45 Markup declaration open state
    /// <br/>
    /// If the next two characters are both "-" (U+002D) characters, consume those two characters, create a comment token whose data is the empty string, and switch to the comment start state.
    /// <br/>
    /// Otherwise, if the next seven characters are an ASCII case-insensitive match for the word "DOCTYPE", then consume those characters and switch to the DOCTYPE state.
    /// <br/>
    /// Otherwise, if there is an adjusted current node and it is not an element in the HTML namespace and the next seven characters are a case-sensitive match for the string "[CDATA[" (the five uppercase letters "CDATA" with a U+005B LEFT SQUARE BRACKET character before and after), then consume those characters and switch to the CDATA section state.
    /// <br/>
    /// Otherwise, this is a parse error.Switch to the bogus comment state. The next character that is consumed, if any, is the first character that will be in the comment.
    /// </summary>
    private void MarkupDeclarationOpenStateImplementation()
    {
        var currentInputCharacter = bufferReader.Peek();

        switch (currentInputCharacter)
        {
            case '-':
                var comment = bufferReader.Peek(CommentMarker.Length);
                if (comment.Span[1] != CommentMarker[1])
                {
                    goto default;
                }

                bufferReader.Consume(CommentMarker.Length);
                currentCommentBuffer.Clear();
                State = commentStartState;
                return;

            case 'd':
            case 'D':
                if (bufferReader.Peek(DoctypeMarker.Length).Span.Equals(DoctypeMarker.AsSpan(), StringComparison.OrdinalIgnoreCase) == false)
                {
                    goto default;
                }

                bufferReader.Consume(DoctypeMarker.Length);
                State = doctypeState;
                return;

            case '[':
                if (bufferReader.Peek(CDataMarker.Length).Span.Equals(CDataMarker.AsSpan(), StringComparison.Ordinal) == false)
                {
                    goto default;
                }

                bufferReader.Consume(CDataMarker.Length);
                State = cDataSectionState;
                return;

            default:
                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                State = bogusCommentState;
                return;
        }
    }
}