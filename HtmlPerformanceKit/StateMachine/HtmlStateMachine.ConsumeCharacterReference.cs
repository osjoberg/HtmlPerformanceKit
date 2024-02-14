using System;
using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private static readonly char[] Nothing = { };

        internal char[] ConsumeCharacterReference()
        {
            var currentInputCharacter = bufferReader.Peek();

            switch (currentInputCharacter)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                case '<':
                case '&':
                case EofMarker:
                    return Nothing;

                case '#':
                    bufferReader.Consume();

                    var afterNumberSignCharacter = bufferReader.Peek();
                    int codepoint;
                    switch (afterNumberSignCharacter)
                    {
                        case 'x':
                        case 'X':
                            bufferReader.Consume();
                            codepoint = bufferReader.ConsumeHexDigits();
                            if (codepoint == int.MinValue)
                            {
                                bufferReader.Reconsume('#');
                                bufferReader.Reconsume(afterNumberSignCharacter);
                                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                                return Nothing;
                            }

                            break;

                        default:
                            codepoint = bufferReader.ConsumeDigits();
                            if (codepoint == int.MinValue)
                            {
                                bufferReader.Reconsume('#');
                                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                                return Nothing;
                            }

                            break;
                    }

                    if (bufferReader.Consume() != ';')
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    }

                    var replacementCharacter = HtmlChar.GetReplacementCharacterReference(codepoint);
                    if (replacementCharacter.HasValue)
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        return new[] { replacementCharacter.Value };
                    }

                    if (HtmlChar.IsCharacterReferenceReplacementToken(codepoint))
                    {
                        return new[] { HtmlChar.ReplacementCharacter };
                    }

                    if (HtmlChar.IsCharacterReferenceParseError(codepoint))
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        return Nothing;
                    }

                    return char.ConvertFromUtf32(codepoint).ToCharArray();

                default:
                    if (currentInputCharacter == additionalAllowedCharacter)
                    {
                        return Nothing;
                    }

                    var characterReferenceBuffer = bufferReader.Peek(35);
                    var startIndex = characterReferenceBuffer.Span.IndexOf(';');
                    startIndex = startIndex == -1 ? characterReferenceBuffer.Length : startIndex + 1;

                    for (var i = startIndex; i > 0; i--)
                    {
                        var characterReferenceAttempt = characterReferenceBuffer.Slice(0, i);
                        var characterReferenceResult = HtmlChar.GetCharactersByCharacterReference(characterReferenceAttempt);
                        if (characterReferenceResult == null)
                        {
                            continue;
                        }

                        if ((additionalAllowedCharacter == '"' || additionalAllowedCharacter == '\'')
                            && characterReferenceBuffer.Span[i - 1] != ';' && characterReferenceBuffer.Length > i
                            && (characterReferenceBuffer.Span[i] == '='
                                || (characterReferenceBuffer.Span[i] >= 'A' && characterReferenceBuffer.Span[i] <= 'Z')
                                || (characterReferenceBuffer.Span[i] >= 'a' && characterReferenceBuffer.Span[i] <= 'z')
                                || (characterReferenceBuffer.Span[i] >= '0' && characterReferenceBuffer.Span[i] <= '9')))
                        {
                            if (characterReferenceBuffer.Span[i] == '=')
                            {
                                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                            }

                            return Nothing;
                        }

                        bufferReader.Consume(characterReferenceAttempt.Length);
                        if (characterReferenceBuffer.Span[i - 1] != ';')
                        {
                            ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        }

                        return characterReferenceResult;
                    }

                    for (var i = 0; i < characterReferenceBuffer.Length; i++)
                    {
                        if ((characterReferenceBuffer.Span[i] >= 'A' && characterReferenceBuffer.Span[i] <= 'Z')
                            || (characterReferenceBuffer.Span[i] >= 'a' && characterReferenceBuffer.Span[i] <= 'z')
                            || (characterReferenceBuffer.Span[i] >= '0' && characterReferenceBuffer.Span[i] <= '9'))
                        {
                            continue;
                        }

                        if (characterReferenceBuffer.Span[i] != ';')
                        {
                            return Nothing;
                        }
                            
                        if (i > 0)
                        {
                            ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        }

                        return Nothing;
                    }

                    return Nothing;
            }
        }
    }
}
