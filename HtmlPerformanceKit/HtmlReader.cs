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
        private readonly BufferReader bufferReader;
        private readonly HtmlStateMachine stateMachine;

        private CharBuffer textBuffer;
        private HtmlTagToken tagToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader" /> class.
        /// </summary>
        /// <param name="streamReader">StreamReader instance to read from.</param>
        public HtmlReader(StreamReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException(nameof(streamReader));
            }

            bufferReader = new BufferReader(streamReader);
            stateMachine = new HtmlStateMachine(bufferReader, message => OnParseError(this, new HtmlParseErrorEventArgs(message, bufferReader.LineNumber, bufferReader.LinePosition)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader" /> class.
        /// </summary>
        /// <param name="stream">Stream instance to read from.</param>
        public HtmlReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            bufferReader = new BufferReader(new StreamReader(stream));
            stateMachine = new HtmlStateMachine(bufferReader, message => OnParseError(this, new HtmlParseErrorEventArgs(message, bufferReader.LineNumber, bufferReader.LinePosition)));
        }

        /// <summary>
        /// Event that occurs whenever there is a parse error while parsing the Html.
        /// </summary>
        public event EventHandler<HtmlParseErrorEventArgs> ParseError;

        /// <summary>
        /// Gets last read node type.
        /// </summary>
        public HtmlNodeType NodeType { get; private set; }

        /// <summary>
        /// Gets if last read tag is a self closing element.
        /// <returns>True if last read node was <see cref="HtmlNodeType.Tag"/> or <see cref="HtmlNodeType.Doctype"/> and it was a self closing element, otherwise False.</returns>
        /// </summary>
        public bool SelfClosingElement => tagToken?.SelfClosing ?? false;

        /// <summary>
        /// Gets the last read tag name.
        /// <returns>Lowercased tag name if last read node was <see cref="HtmlNodeType.Tag"/> or <see cref="HtmlNodeType.Doctype"/>, otherwise Null.</returns>
        /// </summary>
        public string Name => tagToken?.Name.ToString();

        /// <summary>
        /// Gets the last read text.
        /// <returns>Text if last read node was <see cref="HtmlNodeType.Text"/> or <see cref="HtmlNodeType.Comment"/>, otherwise Null.</returns>
        /// </summary>
        public string Text => textBuffer?.ToString();

        /// <summary>
        /// Gets the last read attribute count.
        /// <returns>Number of attributes if last read node was <see cref="HtmlNodeType.Text"/> or <see cref="HtmlNodeType.Comment"/>, otherwise 0.</returns>
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
        /// Reads one more token from the stream.
        /// </summary>
        /// <returns>True if one node was read, False if end of stream is reached.</returns>
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
                    NodeType = stateMachine.EmitTagToken.EndTag ? HtmlNodeType.EndTag  : HtmlNodeType.Tag;

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
                    NodeType = HtmlNodeType.Text;
                    textBuffer = stateMachine.EmitDataBuffer;
                    return true;
                }

                if (stateMachine.EmitCommentBuffer != null)
                {
                    NodeType = HtmlNodeType.Comment;
                    textBuffer = stateMachine.EmitCommentBuffer;
                    return true;
                }

                if (stateMachine.EmitDoctypeToken != null)
                {
                    NodeType = HtmlNodeType.Doctype;
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
        /// <returns>Attribute value of first specified attribute name if last read node was <see cref="HtmlNodeType.Text"/> or <see cref="HtmlNodeType.Comment"/>, otherwise Null.</returns>
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
        /// <returns>Attribute value of specified index if last read node was <see cref="HtmlNodeType.Text"/> or <see cref="HtmlNodeType.Comment"/>, otherwise Null.</returns>
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
        /// <returns>Attribute name of specified index if last read node was <see cref="HtmlNodeType.Text"/> or <see cref="HtmlNodeType.Comment"/>, otherwise Null.</returns>
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