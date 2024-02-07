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
    }
}
