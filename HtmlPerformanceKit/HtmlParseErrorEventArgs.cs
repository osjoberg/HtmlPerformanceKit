namespace HtmlPerformanceKit
{
    /// <summary>
    /// Error for when an error occurs while parsing Html.
    /// </summary>
    public class HtmlParseErrorEventArgs
    {
        internal HtmlParseErrorEventArgs(string message, int lineNumber, int linePosition)
        {
            Message = message;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        /// <summary>
        /// Gets message describing the parse error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets line number where the error occurred.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Gets line position where the error occurred.
        /// </summary>
        public int LinePosition { get; }
    }
}
