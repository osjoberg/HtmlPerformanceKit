using System;
using System.IO;

using HtmlPerformanceKit.Infrastructure;
using HtmlPerformanceKit.StateMachine;

namespace HtmlPerformanceKit;

/// <summary>
/// HtmlReader is a streaming reader for HTML documents.
/// </summary>
public sealed partial class HtmlReader
{
    private const string AttributeTagTokenIsNullMessage = "Attributes can only be accessed when TokenKind is HtmlTokenKind.Tag, HtmlTokenKind.EndTag or HtmlTokenKind.Doctype.";
    private const string SelfClosingElementTagTokenIsNullMessage = "SelfClosingElement property can only be accessed when TokenKind is HtmlTokenKind.Tag, HtmlTokenKind.EndTag or HtmlTokenKind.Doctype.";
    private const string AttributeCountElementTagTokenIsNullMessage = "AttributeCount property can only be accessed when TokenKind is HtmlTokenKind.Tag, HtmlTokenKind.EndTag or HtmlTokenKind.Doctype.";
    private const string NameTagTokenIsNullMessage = "Name property can only be accessed when TokenKind is HtmlTokenKind.Tag, HtmlTokenKind.EndTag or HtmlTokenKind.Doctype.";
    private const string TextBufferIsNullMessage = "Text property can only be accessed when TokenKind is HtmlTokenKind.Text or HtmlTokenKind.Comment.";
    private const string AttributeNotFoundMessage = "An attribute with the given name does not exist.";

    private readonly HtmlStateMachine stateMachine;
    private CharBuffer? textBuffer;
    private HtmlTagToken? tagToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReader" /> class.
    /// </summary>
    /// <param name="stream">Stream instance to read from.</param>
    /// <param name="options">Options to configure <see cref="HtmlReader" /> behaviour.</param>
    public HtmlReader(Stream stream, HtmlReaderOptions? options = null)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        options ??= HtmlReaderOptions.Default;
        stateMachine = new HtmlStateMachine(OnParserError, options.DecodeHtmlCharacters);
        Initialize(new StreamReader(stream));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReader" /> class.
    /// </summary>
    /// <param name="textReader">Stream instance to read from.</param>
    /// <param name="options">Options to configure <see cref="HtmlReader" /> behaviour.</param>
    public HtmlReader(TextReader textReader, HtmlReaderOptions? options = null)
    {
        if (textReader == null)
        {
            throw new ArgumentNullException(nameof(textReader));
        }

        options ??= HtmlReaderOptions.Default;
        stateMachine = new HtmlStateMachine(OnParserError, options.DecodeHtmlCharacters);
        Initialize(textReader);
    }

    /// <summary>
    /// Event that occurs whenever there is a parse error while parsing the Html.
    /// </summary>
    public event EventHandler<HtmlParseErrorEventArgs>? ParseError;

    /// <summary>
    /// Gets last read token kind.
    /// </summary>
    public HtmlTokenKind TokenKind { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the last read tag is a self-closing element.
    /// <returns>True if last read token kind was a self-closing element, otherwise False.</returns>
    /// </summary>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    public bool SelfClosingElement => tagToken == null ? throw new InvalidOperationException(SelfClosingElementTagTokenIsNullMessage) : tagToken.SelfClosing;

    /// <summary>
    /// Gets the last read attribute count.
    /// <returns>Number of attributes if last read token kind was <see cref="HtmlTokenKind.Text"/> or <see cref="HtmlTokenKind.Comment"/>, otherwise 0.</returns>
    /// </summary>
    /// <exception cref="InvalidOperationException">The last read <see cref="TokenKind"/> was not <see cref="HtmlTokenKind.Tag"/>, <see cref="HtmlTokenKind.EndTag"/> or <see cref="HtmlTokenKind.Doctype"/>.</exception>
    public int AttributeCount => tagToken == null ? throw new InvalidOperationException(AttributeCountElementTagTokenIsNullMessage) : tagToken.Attributes.Count;

    /// <summary>
    /// Gets the current line number.
    /// </summary>
    public int LineNumber => stateMachine.LineNumber;

    /// <summary>
    /// Gets the current line position.
    /// </summary>
    public int LinePosition => stateMachine.LinePosition;

    /// <summary>
    /// Read one token from the stream.
    /// </summary>
    /// <returns><see langword="true"/> if a token was read, <see langword="false"/> if end of stream is reached.</returns>
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

                stateMachine.SetNextStateFromTagName(stateMachine.EmitTagToken);

                if (stateMachine.EmitTagToken.EndTag == false)
                {
                    stateMachine.RememberLastStartTagName(stateMachine.EmitTagToken);
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

    private void OnParserError(string message)
    {
        ParseError?.Invoke(this, new HtmlParseErrorEventArgs(message, stateMachine.LineNumber, stateMachine.LinePosition));
    }
}