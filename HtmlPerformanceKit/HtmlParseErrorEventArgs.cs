using System;

namespace HtmlPerformanceKit
{
    /// <summary>
    /// Error for when an error occurs while parsing Html.
    /// </summary>
    public class HtmlParseErrorEventArgs : EventArgs
    {
        internal HtmlParseErrorEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
