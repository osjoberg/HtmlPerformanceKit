using System;
using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        internal HtmlChar ConsumeCharacterReference()
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
                    return HtmlChar.Nothing;

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
                                return HtmlChar.Nothing;
                            }

                            break;

                        default:
                            codepoint = bufferReader.ConsumeDigits();
                            if (codepoint == int.MinValue)
                            {
                                bufferReader.Reconsume('#');
                                ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                                return HtmlChar.Nothing;
                            }

                            break;
                    }

                    if (bufferReader.Consume() != ';')
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    }

                    var replacementCharacter = HtmlChar.GetReplacementCharacterReference(codepoint);
                    if (replacementCharacter.IsNothing == false)
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        return replacementCharacter;
                    }

                    if (HtmlChar.IsCharacterReferenceReplacementToken(codepoint))
                    {
                        return HtmlChar.ReplacementCharacterHtmlChar;
                    }

                    if (HtmlChar.IsCharacterReferenceParseError(codepoint))
                    {
                        ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        return HtmlChar.Nothing;
                    }

                    return HtmlChar.ConvertFromUtf32(codepoint);

                default:
                    if (currentInputCharacter == additionalAllowedCharacter)
                    {
                        return HtmlChar.Nothing;
                    }

                    var characterReferenceBuffer = bufferReader.Peek(35);
                    var startIndex = characterReferenceBuffer.Span.IndexOf(';');
                    startIndex = startIndex == -1 ? characterReferenceBuffer.Length : startIndex + 1;

                    for (var i = startIndex; i > 0; i--)
                    {
                        var characterReferenceAttempt = characterReferenceBuffer.Slice(0, i);
                        var characterReferenceResult = HtmlChar.GetCharactersByCharacterReference(characterReferenceAttempt);
                        if (characterReferenceResult.IsNothing)
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

                            return HtmlChar.Nothing;
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
                            return HtmlChar.Nothing;
                        }
                            
                        if (i > 0)
                        {
                            ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                        }

                        return HtmlChar.Nothing;
                    }

                    return HtmlChar.Nothing;
            }
        }
    }
}
