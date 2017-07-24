﻿using HtmlPerformanceKit.Infrastructure;

namespace HtmlPerformanceKit.StateMachine
{

    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.44 Bogus comment state
        /// 
        /// Consume every character up to and including the first ">" (U+003E) character or the end of the file(EOF), whichever comes first. Emit a comment token whose data is the concatenation of all the characters starting from and including the character that caused the state machine to switch into the bogus comment state, up to and including the character immediately before the last consumed character (i.e.up to the character just before the U+003E or EOF character), but with any U+0000 NULL characters replaced by U+FFFD REPLACEMENT CHARACTER characters. (If the comment was started by the end of the file (EOF), the token is empty.Similarly, the token is empty if it was generated by the string "&lt;!>".)
        /// 
        /// Switch to the data state.
        /// 
        /// If the end of the file was reached, reconsume the EOF character.
        /// </summary>
        private void BogusCommentState()
        {
            while (true)
            {
                var currentInputCharacter = bufferReader.Consume();

                switch (currentInputCharacter)
                {
                    case '>':
                        EmitCommentBuffer = currentCommentBuffer;
                        State = DataState;
                        return;
                        
                    case EofMarker:
                        State = DataState;
                        EmitCommentBuffer = currentCommentBuffer;
                        bufferReader.Reconsume(EofMarker);
                        return;

                    case HtmlChar.Null:
                        currentCommentBuffer.Append(HtmlChar.ReplacementCharacter);
                        continue;

                    default:
                        currentCommentBuffer.Append((char)currentInputCharacter);
                        continue;
                }
            }
        }
    }
}