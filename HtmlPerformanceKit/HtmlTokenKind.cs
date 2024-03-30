namespace HtmlPerformanceKit;

/// <summary>
/// Identifies what kind of token that was read from stream.
/// </summary>
public enum HtmlTokenKind
{
    /// <summary>
    /// Nothing has been read from stream yet.
    /// </summary>
    None = 0,

    /// <summary>
    /// Doctype was read from stream. Use HtmlReader.Name to get tag name or HtmlReader.GetAttribute to get attribute values.
    /// </summary>
    Doctype = 1,

    /// <summary>
    /// Tag was read from stream. Use HtmlReader.Name to get tag name or HtmlReader.GetAttribute to get attribute values.
    /// </summary>
    Tag = 2,

    /// <summary>
    /// End tag was read from stream. Use HtmlReader.Name to get element name.
    /// </summary>
    EndTag = 3,

    /// <summary>
    /// Comment tag was read from stream. Use HtmlReader.Text to get comment text.
    /// </summary>
    Comment = 4,

    /// <summary>
    /// Text was read from stream. Use HtmlReader.Text to get text.
    /// </summary>
    Text = 5
}