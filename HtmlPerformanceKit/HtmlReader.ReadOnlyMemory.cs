using System;
using System.Collections.Generic;

namespace HtmlPerformanceKit;

public sealed partial class HtmlReader
{
    /// <summary>
    /// Gets the last read tag name.
    /// </summary>
    /// <returns>Lowercased tag name.</returns>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    public ReadOnlyMemory<char> NameAsMemory => tagToken == null ? throw new InvalidOperationException(NameTagTokenIsNullMessage) : tagToken.Name.AsMemory();

    /// <summary>
    /// Gets the last read text.
    /// </summary>
    /// <returns>Text if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise throws an exception.</returns>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>.</exception>
    public ReadOnlyMemory<char> TextAsMemory => textBuffer == null ? throw new InvalidOperationException(TextBufferIsNullMessage) : textBuffer.AsMemory();

    /// <summary>
    /// Gets the attribute value associated with the specified attribute name.
    /// </summary>
    /// <param name="name">The name of the attribute to get.</param>
    /// <param name="value">When this method returns, contains the value of the specified attribute name, if the attribute is found; otherwise empty.</param>
    /// <returns><see langword="true"/> if an attribute exists with the specified name; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name">name</paramref> is null.</exception>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    public bool TryGetAttributeAsMemory(string name, out ReadOnlyMemory<char> value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (tagToken == null)
        {
            throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
        }

        var attributeValue = tagToken.Attributes[name];
        if (attributeValue == null)
        {
            value = ReadOnlyMemory<char>.Empty;
            return false;
        }

        value = attributeValue.AsMemory();
        return true;
    }

    /// <summary>
    /// Gets the attribute value associated with the specified attribute name.
    /// </summary>
    /// <param name="name">The name of the attribute to get.</param>
    /// <returns>The value of the specified attribute name, if the attribute exists.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name">name</paramref> is null.</exception>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    /// <exception cref="KeyNotFoundException">The attribute <paramref name="name">name</paramref> does not exist.</exception>
    public ReadOnlyMemory<char> GetAttributeAsMemory(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (tagToken == null)
        {
            throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
        }

        var attributeValue = tagToken.Attributes[name];
        if (attributeValue == null)
        {
            throw new KeyNotFoundException(AttributeNotFoundMessage);
        }

        return attributeValue.AsMemory();
    }

    /// <summary>
    /// Gets the attribute value by attribute index.
    /// </summary>
    /// <param name="index">Index of attribute value to get.</param>
    /// <returns>The value of the attribute at the specified index.</returns>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index">index</paramref> is out of range.</exception>
    public ReadOnlyMemory<char> GetAttributeAsMemory(int index)
    {
        if (tagToken == null)
        {
            throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
        }

        if (index < 0 || index >= tagToken.Attributes.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return tagToken.Attributes[index].Value.AsMemory();
    }

    /// <summary>
    /// Get attribute name by attribute index.
    /// </summary>
    /// <param name="index">Index of attribute name to get.</param>
    /// <returns>The name of the attribute at the specified index.</returns>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index">index</paramref> is out of range.</exception>
    public ReadOnlyMemory<char> GetAttributeNameAsMemory(int index)
    {
        if (tagToken == null)
        {
            throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
        }

        if (index < 0 || index >= tagToken.Attributes.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return tagToken.Attributes[index].Name.AsMemory();
    }
}