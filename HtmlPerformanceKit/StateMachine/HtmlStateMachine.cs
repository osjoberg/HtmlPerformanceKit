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

            BogusCommentState = BogusCommentStateImplementation;
            CommentEndBangState = CommentEndBangStateImplementation;
            CommentEndDashState = CommentEndDashStateImplementation;
            CommentEndState = CommentEndStateImplementation;
            CommentStartDashState = CommentStartDashStateImplementation;
            CommentStartState = CommentStartStateImplementation;
            CommentState = CommentStateImplementation;
            CDataSectionState = CDataSectionStateImplementation;
            CharacterReferenceInDataState = CharacterReferenceInDataStateImplementation;
            DataState = DataStateImplementation;
            AfterDoctypeNameState = AfterDoctypeNameStateImplementation;
            AfterDoctypePublicIdentifierState = AfterDoctypePublicIdentifierStateImplementation;
            AfterDoctypePublicKeywordState = AfterDoctypePublicKeywordStateImplementation;
            AfterDoctypeSystemIdentifierState = AfterDoctypeSystemIdentifierStateImplementation;
            AfterDoctypeSystemKeywordState = AfterDoctypeSystemKeywordStateImplementation;
            BeforeDoctypeNameState = BeforeDoctypeNameStateImplementation;
            BeforeDoctypePublicIdentifierState = BeforeDoctypePublicIdentifierStateImplementation;
            BeforeDoctypeSystemIdentifierState = BeforeDoctypeSystemIdentifierStateImplementation;
            BetweenDoctypePublicAndSystemIdentifiersState = BetweenDoctypePublicAndSystemIdentifiersStateImplementation;
            BogusDoctypeState = BogusDoctypeStateImplementation;
            DoctypeNameState = DoctypeNameStateImplementation;
            DoctypePublicIdentifierDoubleQuotedState = DoctypePublicIdentifierDoubleQuotedStateImplementation;
            DoctypePublicIdentifierSingleQuotedState = DoctypePublicIdentifierSingleQuotedStateImplementation;
            DoctypeState = DoctypeStateImplementation;
            DoctypeSystemIdentifierDoubleQuotedState = DoctypeSystemIdentifierDoubleQuotedStateImplementation;
            DoctypeSystemIdentifierSingleQuotedState = DoctypeSystemIdentifierSingleQuotedStateImplementation;
            PlainTextState = PlainTextStateImplementation;
            RawTextEndTagNameState = RawTextEndTagNameStateImplementation;
            RawTextEndTagOpenState = RawTextEndTagOpenStateImplementation;
            RawTextLessThanSignState = RawTextLessThanSignStateImplementation;
            RawTextState = RawTextStateImplementation;
            CharacterReferenceInRcDataState = CharacterReferenceInRcDataStateImplementation;
            RcDataEndTagNameState = RcDataEndTagNameStateImplementation;
            RcDataEndTagOpenState = RcDataEndTagOpenStateImplementation;
            RcDataLessThanSignState = RcDataLessThanSignStateImplementation;
            RcDataState = RcDataStateImplementation;
            ScriptDataDoubleEscapedDashDashState = ScriptDataDoubleEscapedDashDashStateImplementation;
            ScriptDataDoubleEscapedDashState = ScriptDataDoubleEscapedDashStateImplementation;
            ScriptDataDoubleEscapedLessThanSignState = ScriptDataDoubleEscapedLessThanSignStateImplementation;
            ScriptDataDoubleEscapedState = ScriptDataDoubleEscapedStateImplementation;
            ScriptDataDoubleEscapeEndState = ScriptDataDoubleEscapeEndStateImplementation;
            ScriptDataDoubleEscapeStartState = ScriptDataDoubleEscapeStartStateImplementation;
            ScriptDataEndTagNameState = ScriptDataEndTagNameStateImplementation;
            ScriptDataEndTagOpenState = ScriptDataEndTagOpenStateImplementation;
            ScriptDataEscapedDashDashState = ScriptDataEscapedDashDashStateImplementation;
            ScriptDataEscapedDashState = ScriptDataEscapedDashStateImplementation;
            ScriptDataEscapedEndTagNameState = ScriptDataEscapedEndTagNameStateImplementation;
            ScriptDataEscapedEndTagOpenState = ScriptDataEscapedEndTagOpenStateImplementation;
            ScriptDataEscapedLessThanSignState = ScriptDataEscapedLessThanSignStateImplementation;
            ScriptDataEscapedState = ScriptDataEscapedStateImplementation;
            ScriptDataEscapeStartDashState = ScriptDataEscapeStartDashStateImplementation;
            ScriptDataEscapeStartState = ScriptDataEscapeStartStateImplementation;
            ScriptDataLessThanSignState = ScriptDataLessThanSignStateImplementation;
            ScriptDataState = ScriptDataStateImplementation;
            AfterAttributeNameState = AfterAttributeNameStateImplementation;
            AfterAttributeValueQuotedState = AfterAttributeValueQuotedStateImplementation;
            AttributeNameState = AttributeNameStateImplementation;
            AttributeValueDoubleQuotedState = AttributeValueDoubleQuotedStateImplementation;
            AttributeValueSingleQuotedState = AttributeValueSingleQuotedStateImplementation;
            AttributeValueUnquotedState = AttributeValueUnquotedStateImplementation;
            BeforeAttributeNameState = BeforeAttributeNameStateImplementation;
            BeforeAttributeValueState = BeforeAttributeValueStateImplementation;
            CharacterReferenceInAttributeValueState = CharacterReferenceInAttributeValueStateImplementation;
            EndTagOpenState = EndTagOpenStateImplementation;
            MarkupDeclarationOpenState = MarkupDeclarationOpenStateImplementation;
            SelfClosingStartTagState = SelfClosingStartTagStateImplementation;
            TagNameState = TagNameStateImplementation;
            TagOpenState = TagOpenStateImplementation;

            State = DataState;
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
