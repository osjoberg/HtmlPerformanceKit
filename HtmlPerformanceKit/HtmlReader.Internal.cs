using System;
using System.IO;
using System.Linq;

namespace HtmlPerformanceKit;

public partial class HtmlReader
{
    internal void Initialize(TextReader textReader)
    {
        textBuffer = null;
        tagToken = null;
        TokenKind = HtmlTokenKind.None;

        stateMachine.Initialize(textReader);
    }

    internal void Reset()
    {
        stateMachine.Reset();

        if (ParseError == null)
        {
            return;
        }

        var eventHandlers = ParseError.GetInvocationList().Cast<EventHandler<HtmlParseErrorEventArgs>>();
        foreach (var eventHandler in eventHandlers)
        {
            ParseError -= eventHandler;
        }
    }
}