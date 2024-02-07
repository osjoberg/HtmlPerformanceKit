namespace HtmlPerformanceKit
{
    /// <summary>
    /// Options for the HTML reader.
    /// </summary>
    public sealed class HtmlReaderOptions
    {       
        /// <summary>
        /// True, to skip decoding of character references.
        /// </summary>
        public bool SkipCharacterReferenceDecoding { get; set; }

        /// <summary>
        /// True to keep the stream open.
        /// </summary>
        public bool KeepOpen { get; set; }
    }
}
