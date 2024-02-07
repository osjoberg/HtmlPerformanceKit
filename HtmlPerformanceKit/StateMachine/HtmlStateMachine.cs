using System;
using System.IO;
using System.Runtime.CompilerServices;

using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine : IDisposable
    {
        private const int EofMarker = -1;
        private readonly BufferReader bufferReader = new BufferReader();
        private readonly HtmlTagToken currentDoctypeToken = new HtmlTagToken();
        private readonly HtmlTagToken currentTagToken = new HtmlTagToken();
        private readonly CharBuffer appropriateTagName = new CharBuffer(100);
        private readonly CharBuffer currentCommentBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer currentDataBuffer = new CharBuffer(1024 * 10);
        private readonly CharBuffer temporaryBuffer = new CharBuffer(1024);
        private readonly Action AfterAttributeNameState;
        private readonly Action AfterAttributeValueQuotedState;
        private readonly Action AfterDoctypeNameState;
        private readonly Action AfterDoctypePublicIdentifierState;
        private readonly Action AfterDoctypePublicKeywordState;
        private readonly Action AfterDoctypeSystemIdentifierState;
        private readonly Action AfterDoctypeSystemKeywordState;
        private readonly Action AttributeNameState;
        private readonly Action AttributeValueDoubleQuotedState;
        private readonly Action AttributeValueSingleQuotedState;
        private readonly Action AttributeValueUnquotedState;
        private readonly Action BeforeAttributeNameState;
        private readonly Action BeforeAttributeValueState;
        private readonly Action BeforeDoctypeNameState;
        private readonly Action BeforeDoctypePublicIdentifierState;
        private readonly Action BeforeDoctypeSystemIdentifierState;
        private readonly Action BetweenDoctypePublicAndSystemIdentifiersState;
        private readonly Action BogusCommentState;
        private readonly Action BogusDoctypeState;
        private readonly Action CDataSectionState;
        private readonly Action CharacterReferenceInAttributeValueState;
        private readonly Action CharacterReferenceInDataState;
        private readonly Action CharacterReferenceInRcDataState;
        private readonly Action CommentEndBangState;
        private readonly Action CommentEndDashState;
        private readonly Action CommentEndState;
        private readonly Action CommentStartDashState;
        private readonly Action CommentStartState;
        private readonly Action CommentState;
        private readonly Action DataState;
        private readonly Action DoctypeNameState;
        private readonly Action DoctypePublicIdentifierDoubleQuotedState;
        private readonly Action DoctypePublicIdentifierSingleQuotedState;
        private readonly Action DoctypeState;
        private readonly Action DoctypeSystemIdentifierDoubleQuotedState;
        private readonly Action DoctypeSystemIdentifierSingleQuotedState;
        private readonly Action EndTagOpenState;
        private readonly Action MarkupDeclarationOpenState;
        private readonly Action PlainTextState;
        private readonly Action RawTextEndTagNameState;
        private readonly Action RawTextEndTagOpenState;
        private readonly Action RawTextLessThanSignState;
        private readonly Action RawTextState;
        private readonly Action RcDataEndTagNameState;
        private readonly Action RcDataEndTagOpenState;
        private readonly Action RcDataLessThanSignState;
        private readonly Action RcDataState;
        private readonly Action ScriptDataDoubleEscapedDashDashState;
        private readonly Action ScriptDataDoubleEscapedDashState;
        private readonly Action ScriptDataDoubleEscapedLessThanSignState;
        private readonly Action ScriptDataDoubleEscapedState;
        private readonly Action ScriptDataDoubleEscapeEndState;
        private readonly Action ScriptDataDoubleEscapeStartState;
        private readonly Action ScriptDataEndTagNameState;
        private readonly Action ScriptDataEndTagOpenState;
        private readonly Action ScriptDataEscapedDashDashState;
        private readonly Action ScriptDataEscapedDashState;
        private readonly Action ScriptDataEscapedEndTagNameState;
        private readonly Action ScriptDataEscapedEndTagOpenState;
        private readonly Action ScriptDataEscapedLessThanSignState;
        private readonly Action ScriptDataEscapedState;
        private readonly Action ScriptDataEscapeStartDashState;
        private readonly Action ScriptDataEscapeStartState;
        private readonly Action ScriptDataLessThanSignState;
        private readonly Action ScriptDataState;
        private readonly Action SelfClosingStartTagState;
        private readonly Action TagNameState;
        private readonly Action TagOpenState;
        private Action<string> parseError;
        private Action returnToState;
        private bool skipDecodingCharacterReferences;
        private char additionalAllowedCharacter;

        private HtmlStateMachine()
        {
            AfterAttributeNameState = BuildAfterAttributeNameState();
            AfterAttributeValueQuotedState = BuildAfterAttributeValueQuotedState();
            AfterDoctypeNameState = BuildAfterDoctypeNameState();
            AfterDoctypePublicIdentifierState = BuildAfterDoctypePublicIdentifierState();
            AfterDoctypePublicKeywordState = BuildAfterDoctypePublicKeywordState();
            AfterDoctypeSystemIdentifierState = BuildAfterDoctypeSystemIdentifierState();
            AfterDoctypeSystemKeywordState = BuildAfterDoctypeSystemKeywordState();
            AttributeNameState = BuildAttributeNameState();
            AttributeValueDoubleQuotedState = BuildAttributeValueDoubleQuotedState();
            AttributeValueSingleQuotedState = BuildAttributeValueSingleQuotedState();
            AttributeValueUnquotedState = BuildAttributeValueUnquotedState();
            BeforeAttributeNameState = BuildBeforeAttributeNameState();
            BeforeAttributeValueState = BuildBeforeAttributeValueState();
            BeforeDoctypeNameState = BuildBeforeDoctypeNameState();
            BeforeDoctypePublicIdentifierState = BuildBeforeDoctypePublicIdentifierState();
            BeforeDoctypeSystemIdentifierState = BuildBeforeDoctypeSystemIdentifierState();
            BetweenDoctypePublicAndSystemIdentifiersState = BuildBetweenDoctypePublicAndSystemIdentifiersState();
            BogusCommentState = BuildBogusCommentState();
            BogusDoctypeState = BuildBogusDoctypeState();
            CDataSectionState = BuildCDataSectionState();
            CharacterReferenceInAttributeValueState = BuildCharacterReferenceInAttributeValueState();
            CharacterReferenceInDataState = BuildCharacterReferenceInDataState();
            CharacterReferenceInRcDataState = BuildCharacterReferenceInRcDataState();
            CommentEndBangState = BuildCommentEndBangState();
            CommentEndDashState = BuildCommentEndDashState();
            CommentEndState = BuildCommentEndState();
            CommentStartDashState = BuildCommentStartDashState();
            CommentStartState = BuildCommentStartState();
            CommentState = BuildCommentState();
            DataState = BuildDataState();
            DoctypeNameState = BuildDoctypeNameState();
            DoctypePublicIdentifierDoubleQuotedState = BuildDoctypePublicIdentifierDoubleQuotedState();
            DoctypePublicIdentifierSingleQuotedState = BuildDoctypePublicIdentifierSingleQuotedState();
            DoctypeState = BuildDoctypeState();
            DoctypeSystemIdentifierDoubleQuotedState = BuildDoctypeSystemIdentifierDoubleQuotedState();
            DoctypeSystemIdentifierSingleQuotedState = BuildDoctypeSystemIdentifierSingleQuotedState();
            EndTagOpenState = BuildEndTagOpenState();
            MarkupDeclarationOpenState = BuildMarkupDeclarationOpenState();
            PlainTextState = BuildPlainTextState();
            RawTextEndTagNameState = BuildRawTextEndTagNameState();
            RawTextEndTagOpenState = BuildRawTextEndTagOpenState();
            RawTextLessThanSignState = BuildRawTextLessThanSignState();
            RawTextState = BuildRawTextState();
            RcDataEndTagNameState = BuildRcDataEndTagNameState();
            RcDataEndTagOpenState = BuildRcDataEndTagOpenState();
            RcDataLessThanSignState = BuildRcDataLessThanSignState();
            RcDataState = BuildRcDataState();
            ScriptDataDoubleEscapedDashDashState = BuildScriptDataDoubleEscapedDashDashState();
            ScriptDataDoubleEscapedDashState = BuildScriptDataDoubleEscapedDashState();
            ScriptDataDoubleEscapedLessThanSignState = BuildScriptDataDoubleEscapedLessThanSignState();
            ScriptDataDoubleEscapedState = BuildScriptDataDoubleEscapedState();
            ScriptDataDoubleEscapeEndState = BuildScriptDataDoubleEscapeEndState();
            ScriptDataDoubleEscapeStartState = BuildScriptDataDoubleEscapeStartState();
            ScriptDataEndTagNameState = BuildScriptDataEndTagNameState();
            ScriptDataEndTagOpenState = BuildScriptDataEndTagOpenState();
            ScriptDataEscapedDashDashState = BuildScriptDataEscapedDashDashState();
            ScriptDataEscapedDashState = BuildScriptDataEscapedDashState();
            ScriptDataEscapedEndTagNameState = BuildScriptDataEscapedEndTagNameState();
            ScriptDataEscapedEndTagOpenState = BuildScriptDataEscapedEndTagOpenState();
            ScriptDataEscapedLessThanSignState = BuildScriptDataEscapedLessThanSignState();
            ScriptDataEscapedState = BuildScriptDataEscapedState();
            ScriptDataEscapeStartDashState = BuildScriptDataEscapeStartDashState();
            ScriptDataEscapeStartState = BuildScriptDataEscapeStartState();
            ScriptDataLessThanSignState = BuildScriptDataLessThanSignState();
            ScriptDataState = BuildScriptDataState();
            SelfClosingStartTagState = BuildSelfClosingStartTagState();
            TagNameState = BuildTagNameState();
            TagOpenState = BuildTagOpenState();
            State = DataState;
        }

        public static HtmlStateMachine Create(TextReader streamReader, Action<string> parseError, bool skipDecodingCharacterReferences)
        {
            var instance = Pool.Get();
            instance.Prepare(streamReader, parseError, skipDecodingCharacterReferences);
            return instance;
        }

        public void Prepare(TextReader streamReader, Action<string> parseError, bool skipDecodingCharacterReferences)
        {
            bufferReader.Init(streamReader);

            this.parseError = parseError;
            this.skipDecodingCharacterReferences = skipDecodingCharacterReferences;
        }

        public void Dispose()
        {
            Pool.Return(this);
        }

        internal BufferReader BufferReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => bufferReader;
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
