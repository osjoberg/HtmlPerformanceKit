using System;
using System.IO;

using HtmlPerformanceKit.Infrastructure;
using HtmlPerformanceKit.StateMachine;

namespace HtmlPerformanceKit
{
    /// <summary>
    /// HtmlReader is a streaming reader for HTML documents.
    /// </summary>
    public sealed class HtmlReader : IDisposable
    {
        private static readonly HtmlReaderOptions DefaultOptions = new HtmlReaderOptions();
        private readonly BufferReader bufferReader;
        private readonly HtmlStateMachine stateMachine;

        private CharBuffer textBuffer;
        private HtmlTagToken tagToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader" /> class.
        /// </summary>
        /// <param name="streamReader">StreamReader instance to read from.</param>
        /// <param name="options">The optional options.</param>
        public HtmlReader(StreamReader streamReader, HtmlReaderOptions options = null)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException(nameof(streamReader));
            }

            if (options == null)
            {
                options = DefaultOptions;
            }

            bufferReader = new BufferReader(streamReader);
            stateMachine = new HtmlStateMachine(bufferReader, ParseErrorFromMessage, options.SkipCharacterReferenceDecoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader" /> class.
        /// </summary>
        /// <param name="stream">Stream instance to read from.</param>
        /// <param name="options">The optional options.</param>
        public HtmlReader(Stream stream, HtmlReaderOptions options = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (options == null)
            {
                options = DefaultOptions;
            }

            bufferReader = new BufferReader(new StreamReader(stream));
            stateMachine = new HtmlStateMachine(bufferReader, ParseErrorFromMessage, options.SkipCharacterReferenceDecoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader" /> class.
        /// </summary>
        /// <param name="textReader">Stream instance to read from.</param>
        /// <param name="options">The optional options.</param>
        public HtmlReader(TextReader textReader, HtmlReaderOptions options = null)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }

            if (options == null)
            {
                options = DefaultOptions;
            }

            bufferReader = new BufferReader(textReader);
            stateMachine = new HtmlStateMachine(bufferReader, ParseErrorFromMessage, options.SkipCharacterReferenceDecoding);
        }

        private void ParseErrorFromMessage(string message)
        {
            OnParseError(this, new HtmlParseErrorEventArgs(message, bufferReader.LineNumber, bufferReader.LinePosition));
        }

        /// <summary>
        /// Event that occurs whenever there is a parse error while parsing the Html.
        /// </summary>
        public event EventHandler<HtmlParseErrorEventArgs> ParseError;

        /// <summary>
        /// Gets last read token kind.
        /// </summary>
        public HtmlTokenKind TokenKind { get; private set; }

        /// <summary>
        /// Gets if last read tag is a self closing element.
        /// <returns>True if last read token kind was <see cref="HtmlTokenKind.Tag"/> or <see cref="HtmlTokenKind.Doctype"/> and it was a self closing element, otherwise False.</returns>
        /// </summary>
        public bool SelfClosingElement => tagToken?.SelfClosing ?? false;

        /// <summary>
        /// Gets the last read tag name.
        /// <returns>Lowercased tag name if last read token kind was <see cref="HtmlTokenKind.Tag"/> or <see cref="HtmlTokenKind.Doctype"/>, otherwise Null.</returns>
        /// </summary>
        public string Name => tagToken?.Name.ToString();

        /// <summary>
        /// Gets the last read text.
        /// <returns>Text if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise Null.</returns>
        /// </summary>
        public string Text => textBuffer?.ToString();

        /// <summary>
        /// Gets the last read attribute count.
        /// <returns>Number of attributes if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise 0.</returns>
        /// </summary>
        public int AttributeCount => tagToken?.Attributes.Count ?? 0;

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int LineNumber => bufferReader.LineNumber;

        /// <summary>
        /// Gets the current line position.
        /// </summary>
        public int LinePosition => bufferReader.LinePosition;

        /// <summary>
        /// Read one token from the stream.
        /// </summary>
        /// <returns>True if a token was read, False if end of stream is reached.</returns>
        public bool Read()
        {
            textBuffer = null;
            tagToken = null;

            stateMachine.ResetEmit();

            while (true)
            {
                stateMachine.State();

                if (stateMachine.EmitTagToken != null)
                {
                    TokenKind = stateMachine.EmitTagToken.EndTag ? HtmlTokenKind.EndTag : HtmlTokenKind.Tag;

                    stateMachine.SetNextStateFromTagName();

                    if (stateMachine.EmitTagToken.EndTag == false)
                    {
                        stateMachine.RememberLastStartTagName();
                    }

                    tagToken = stateMachine.EmitTagToken;
                    return true;
                }

                if (stateMachine.EmitDataBuffer != null)
                {
                    TokenKind = HtmlTokenKind.Text;
                    textBuffer = stateMachine.EmitDataBuffer;
                    return true;
                }

                if (stateMachine.EmitCommentBuffer != null)
                {
                    TokenKind = HtmlTokenKind.Comment;
                    textBuffer = stateMachine.EmitCommentBuffer;
                    return true;
                }

                if (stateMachine.EmitDoctypeToken != null)
                {
                    TokenKind = HtmlTokenKind.Doctype;
                    tagToken = stateMachine.EmitDoctypeToken;
                    return true;
                }

                if (stateMachine.Eof)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get attribute value by attribute name.
        /// </summary>
        /// <param name="name">Name of attribute value to get.</param>
        /// <returns>Attribute value of first specified attribute name if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise Null.</returns>
        public string GetAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Invalid attribute name, \"\".", nameof(name));
            }

            return tagToken?.Attributes[name]?.ToString();
        }

        /// <summary>
        /// Get attribute value by attribute index.
        /// </summary>
        /// <param name="index">Index of attribute value to get.</param>
        /// <returns>Attribute value of specified index if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise Null.</returns>
        public string GetAttribute(int index)
        {
            if (index < 0 || index >= AttributeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return tagToken?.Attributes[index]?.Value.ToString();
        }

        /// <summary>
        /// Get attribute name by attribute index.
        /// </summary>
        /// <param name="index">Index of attribute name to get.</param>
        /// <returns>Attribute name of specified index if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise Null.</returns>
        public string GetAttributeName(int index)
        {
            if (index < 0 || index >= AttributeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return tagToken?.Attributes[index]?.Name.ToString();
        }

        /// <summary>
        /// Disposes the instance and it's associated StreamReader.
        /// </summary>
        public void Dispose()
        {
            bufferReader.Dispose();
        }

        private void OnParseError(object sender, HtmlParseErrorEventArgs args)
        {
            ParseError?.Invoke(sender, args);
        }
    }
}