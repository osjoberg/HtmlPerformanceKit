using System;

using HtmlSpeedPack.Infrastructure;

namespace HtmlSpeedPack.StateMachine
{
    internal partial class HtmlStateMachine
    {
        private const string CommentMarker = "--";
        private const string DoctypeMarker = "doctype";
        private const string CDataMarker = "[CDATA[";

        /// <summary>
        /// 8.2.4.45 Markup declaration open state
        /// 
        /// If the next two characters are both "-" (U+002D) characters, consume those two characters, create a comment token whose data is the empty string, and switch to the comment start state.
        /// 
        /// Otherwise, if the next seven characters are an ASCII case-insensitive match for the word "DOCTYPE", then consume those characters and switch to the DOCTYPE state.
        /// 
        /// Otherwise, if there is an adjusted current node and it is not an element in the HTML namespace and the next seven characters are a case-sensitive match for the string "[CDATA[" (the five uppercase letters "CDATA" with a U+005B LEFT SQUARE BRACKET character before and after), then consume those characters and switch to the CDATA section state.
        /// 
        /// Otherwise, this is a parse error.Switch to the bogus comment state. The next character that is consumed, if any, is the first character that will be in the comment.
        /// </summary>
        private void MarkupDeclarationOpenState()
        {
            var currentInputCharacter = bufferReader.Peek();

            switch (currentInputCharacter)
            {
                case '-':
                    var comment = bufferReader.Peek(CommentMarker.Length);
                    if (comment[1] != CommentMarker[1])
                    {
                        break;
                    }

                    bufferReader.Consume(CommentMarker.Length);
                    currentCommentBuffer.Clear();
                    State = CommentStartState;
                    return;

                case 'd':
                case 'D':
                    if (bufferReader.Peek(DoctypeMarker.Length).Equals(DoctypeMarker, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        break;
                    }

                    bufferReader.Consume(DoctypeMarker.Length);
                    State = DoctypeState;
                    return;

                case '[':
                    if (bufferReader.Peek(CDataMarker.Length) != CDataMarker)
                    {
                        break;
                    }

                    bufferReader.Consume(CDataMarker.Length);
                    State = CDataSectionState;

                    return;
            }

            ParseError = ParseErrorMessage.UnexpectedCharacterInStream;
            State = BogusCommentState;
        }
    }
}
