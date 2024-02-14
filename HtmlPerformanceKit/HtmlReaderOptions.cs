namespace HtmlPerformanceKit
{
    /// <summary>
    /// Options for the HTML reader.
    /// </summary>
    public sealed class HtmlReaderOptions
    {
        internal static readonly HtmlReaderOptions Default = new HtmlReaderOptions();

        /// <summary>
        /// Decode HTML character references during parsing. The default value is <see langword="true" />.
        /// </summary>
        public bool DecodeHtmlCharacters { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the underlying stream or <see cref="T:System.IO.TextReader" /> should be closed when the reader is disposed.</summary>
        /// <returns>
        /// <see langword="true" /> to close the underlying stream or <see cref="T:System.IO.TextReader" /> when the reader is closed; otherwise <see langword="false" />. The default is <see langword="true" />.</returns>
        public bool CloseInput { get; set; } = true;
    }
}
