using System;
using System.Runtime.CompilerServices;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private const int EofMarker = -1;
        private readonly HtmlTagToken currentTagToken = new HtmlTagToken();
        private readonly HtmlTagToken currentDoctypeToken = new HtmlTagToken();
        private readonly CharBuffer currentDataBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer currentCommentBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer temporaryBuffer = new CharBuffer(1024);
        private readonly CharBuffer appropriateTagName = new CharBuffer(100);
        private readonly BufferReader bufferReader;
        private readonly Action<string> parseError;
        private readonly bool decodeHtmlCharacters;
        private Action returnToState;
        private char additionalAllowedCharacter;

        internal HtmlStateMachine(BufferReader bufferReader, Action<string> parseError, bool decodeHtmlCharacters)
        {
            this.bufferReader = bufferReader;
            this.parseError = parseError;
            this.decodeHtmlCharacters = decodeHtmlCharacters;

            bogusCommentState = BogusCommentStateImplementation;
            commentEndBangState = CommentEndBangStateImplementation;
            commentEndDashState = CommentEndDashStateImplementation;
            commentEndState = CommentEndStateImplementation;
            commentStartDashState = CommentStartDashStateImplementation;
            commentStartState = CommentStartStateImplementation;
            commentState = CommentStateImplementation;
            cDataSectionState = CDataSectionStateImplementation;
            characterReferenceInDataState = CharacterReferenceInDataStateImplementation;
            dataState = DataStateImplementation;
            afterDoctypeNameState = AfterDoctypeNameStateImplementation;
            afterDoctypePublicIdentifierState = AfterDoctypePublicIdentifierStateImplementation;
            afterDoctypePublicKeywordState = AfterDoctypePublicKeywordStateImplementation;
            afterDoctypeSystemIdentifierState = AfterDoctypeSystemIdentifierStateImplementation;
            afterDoctypeSystemKeywordState = AfterDoctypeSystemKeywordStateImplementation;
            beforeDoctypeNameState = BeforeDoctypeNameStateImplementation;
            beforeDoctypePublicIdentifierState = BeforeDoctypePublicIdentifierStateImplementation;
            beforeDoctypeSystemIdentifierState = BeforeDoctypeSystemIdentifierStateImplementation;
            betweenDoctypePublicAndSystemIdentifiersState = BetweenDoctypePublicAndSystemIdentifiersStateImplementation;
            bogusDoctypeState = BogusDoctypeStateImplementation;
            doctypeNameState = DoctypeNameStateImplementation;
            doctypePublicIdentifierDoubleQuotedState = DoctypePublicIdentifierDoubleQuotedStateImplementation;
            doctypePublicIdentifierSingleQuotedState = DoctypePublicIdentifierSingleQuotedStateImplementation;
            doctypeState = DoctypeStateImplementation;
            doctypeSystemIdentifierDoubleQuotedState = DoctypeSystemIdentifierDoubleQuotedStateImplementation;
            doctypeSystemIdentifierSingleQuotedState = DoctypeSystemIdentifierSingleQuotedStateImplementation;
            plainTextState = PlainTextStateImplementation;
            rawTextEndTagNameState = RawTextEndTagNameStateImplementation;
            rawTextEndTagOpenState = RawTextEndTagOpenStateImplementation;
            rawTextLessThanSignState = RawTextLessThanSignStateImplementation;
            rawTextState = RawTextStateImplementation;
            characterReferenceInRcDataState = CharacterReferenceInRcDataStateImplementation;
            rcDataEndTagNameState = RcDataEndTagNameStateImplementation;
            rcDataEndTagOpenState = RcDataEndTagOpenStateImplementation;
            rcDataLessThanSignState = RcDataLessThanSignStateImplementation;
            rcDataState = RcDataStateImplementation;
            scriptDataDoubleEscapedDashDashState = ScriptDataDoubleEscapedDashDashStateImplementation;
            scriptDataDoubleEscapedDashState = ScriptDataDoubleEscapedDashStateImplementation;
            scriptDataDoubleEscapedLessThanSignState = ScriptDataDoubleEscapedLessThanSignStateImplementation;
            scriptDataDoubleEscapedState = ScriptDataDoubleEscapedStateImplementation;
            scriptDataDoubleEscapeEndState = ScriptDataDoubleEscapeEndStateImplementation;
            scriptDataDoubleEscapeStartState = ScriptDataDoubleEscapeStartStateImplementation;
            scriptDataEndTagNameState = ScriptDataEndTagNameStateImplementation;
            scriptDataEndTagOpenState = ScriptDataEndTagOpenStateImplementation;
            scriptDataEscapedDashDashState = ScriptDataEscapedDashDashStateImplementation;
            scriptDataEscapedDashState = ScriptDataEscapedDashStateImplementation;
            scriptDataEscapedEndTagNameState = ScriptDataEscapedEndTagNameStateImplementation;
            scriptDataEscapedEndTagOpenState = ScriptDataEscapedEndTagOpenStateImplementation;
            scriptDataEscapedLessThanSignState = ScriptDataEscapedLessThanSignStateImplementation;
            scriptDataEscapedState = ScriptDataEscapedStateImplementation;
            scriptDataEscapeStartDashState = ScriptDataEscapeStartDashStateImplementation;
            scriptDataEscapeStartState = ScriptDataEscapeStartStateImplementation;
            scriptDataLessThanSignState = ScriptDataLessThanSignStateImplementation;
            scriptDataState = ScriptDataStateImplementation;
            afterAttributeNameState = AfterAttributeNameStateImplementation;
            afterAttributeValueQuotedState = AfterAttributeValueQuotedStateImplementation;
            attributeNameState = AttributeNameStateImplementation;
            attributeValueDoubleQuotedState = AttributeValueDoubleQuotedStateImplementation;
            attributeValueSingleQuotedState = AttributeValueSingleQuotedStateImplementation;
            attributeValueUnquotedState = AttributeValueUnquotedStateImplementation;
            beforeAttributeNameState = BeforeAttributeNameStateImplementation;
            beforeAttributeValueState = BeforeAttributeValueStateImplementation;
            characterReferenceInAttributeValueState = CharacterReferenceInAttributeValueStateImplementation;
            endTagOpenState = EndTagOpenStateImplementation;
            markupDeclarationOpenState = MarkupDeclarationOpenStateImplementation;
            selfClosingStartTagState = SelfClosingStartTagStateImplementation;
            tagNameState = TagNameStateImplementation;
            tagOpenState = TagOpenStateImplementation;

            State = dataState;
        }

        internal Action State
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal bool Eof
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal HtmlTagToken EmitTagToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal CharBuffer EmitDataBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal HtmlTagToken EmitDoctypeToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal CharBuffer EmitCommentBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        internal void ParseError(string message)
        {
            parseError(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ResetEmit()
        {
            EmitTagToken = null;
            EmitDataBuffer = null;
            EmitDoctypeToken = null;
            EmitCommentBuffer = null;

            currentDataBuffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RememberLastStartTagName()
        {
            appropriateTagName.Clear();
            appropriateTagName.AddRange(EmitTagToken.Name);
        }
    }
}
