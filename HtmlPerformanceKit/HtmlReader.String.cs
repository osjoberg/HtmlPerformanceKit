using System;
using System.Collections.Generic;

namespace HtmlPerformanceKit
{
    public sealed partial class HtmlReader
    {
        /// <summary>
        /// Gets the last read tag name.
        /// </summary>
        /// <returns>Lowercased tag name.</returns>
        /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
        public string Name => tagToken == null ? throw new InvalidOperationException(NameTagTokenIsNullMessage) : tagToken.Name.ToString();

        /// <summary>
        /// Gets the last read text.
        /// </summary>
        /// <returns>Text if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise throws an exception.</returns>
        /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>.</exception>
        public string Text => textBuffer == null ? throw new InvalidOperationException(TextBufferIsNullMessage) : textBuffer.ToString();

        /// <summary>
        /// Gets the attribute value associated with the specified attribute name.
        /// </summary>
        /// <param name="name">The name of the attribute to get.</param>
        /// <param name="value">When this method returns, contains the value of the specified attribute name, if the attribute is found; otherwise <see cref="string.Empty"/>.</param>
        /// <returns><see langword="true"/> if an attribute exists with the specified name; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name">name</paramref> is null.</exception>
        /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
        public bool TryGetAttribute(string name, out string value)
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
                value = "";
                return false;
            }

            value = attributeValue.ToString();
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
        public string GetAttribute(string name)
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

            return attributeValue.ToString();
        }

        /// <summary>
        /// Gets the attribute value by attribute index.
        /// </summary>
        /// <param name="index">Index of attribute value to get.</param>
        /// <returns>The value of the attribute at the specified index.</returns>
        /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index">index</paramref> is out of range.</exception>
        public string GetAttribute(int index)
        {
            if (tagToken == null)
            {
                throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
            }

            if (index < 0 || index >= tagToken.Attributes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return tagToken.Attributes[index].Value.ToString();
        }

        /// <summary>
        /// Get attribute name by attribute index.
        /// </summary>
        /// <param name="index">Index of attribute name to get.</param>
        /// <returns>The name of the attribute at the specified index.</returns>
        /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index">index</paramref> is out of range.</exception>
        public string GetAttributeName(int index)
        {
            if (tagToken == null)
            {
                throw new InvalidOperationException(AttributeTagTokenIsNullMessage);
            }

            if (index < 0 || index >= tagToken.Attributes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return tagToken.Attributes[index].Name.ToString();
        }
    }
}
