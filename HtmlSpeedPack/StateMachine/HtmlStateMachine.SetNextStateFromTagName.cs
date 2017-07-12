namespace HtmlSpeedPack.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        ///  Set the state of the HTML parser's tokenization stage as follows:
        /// 
        /// If it is a title or textarea element
        /// Switch the tokenizer to the RCDATA state.
        /// 
        /// If it is a style, xmp, iframe, noembed, or noframes element
        /// Switch the tokenizer to the RAWTEXT state.
        /// 
        /// If it is a script element
        /// Switch the tokenizer to the script data state.
        /// 
        /// If it is a noscript element
        /// If the scripting flag is enabled, switch the tokenizer to the RAWTEXT state.Otherwise, leave the tokenizer in the data state.
        ///
        /// If it is a plaintext element
        /// Switch the tokenizer to the PLAINTEXT state.
        /// 
        /// Otherwise
        /// Leave the tokenizer in the data state.
        /// </summary>
        internal void SetNextStateFromTagName()
        {
            if (EmitTagToken.EndTag)
            {
                return;
            }
                
            var name = EmitTagToken.Name;
            var buffer = name.Buffer;

            switch (name.Length)
            {
                case 3:
                    if (buffer[0] == 'x' && buffer[1] == 'm' && buffer[2] == 'p')
                    {
                        State = RawTextState;
                        return;
                    }

                    break;

                case 5:
                    if (buffer[0] == 't' && buffer[1] == 'i' && buffer[2] == 't' && buffer[3] == 'l' && buffer[4] == 'e')
                    {
                        State = RcDataState;
                        return;
                    }

                    if (buffer[0] == 's' && buffer[1] == 't' && buffer[2] == 'y' && buffer[3] == 'l' && buffer[4] == 'e')
                    {
                        State = RawTextState;
                        return;
                    }

                    break;

                case 6:
                    if (buffer[0] == 'i' && buffer[1] == 'f' && buffer[2] == 'r' && buffer[3] == 'a' && buffer[4] == 'm' && buffer[5] == 'e')
                    {
                        State = RawTextState;
                        return;
                    }

                    if (buffer[0] == 's' && buffer[1] == 'c' && buffer[2] == 'r' && buffer[3] == 'i' && buffer[4] == 'p' && buffer[5] == 't')
                    {
                        // TODO: State = ScriptDataState;
                        return;
                    }

                    break;

                case 7:
                    if (buffer[0] == 'n' && buffer[1] == 'o' && buffer[2] == 'e' && buffer[3] == 'm' && buffer[4] == 'b' && buffer[5] == 'e' && buffer[5] == 'd')
                    {
                        State = RawTextState;
                        return;
                    }

                    break;

                case 8:
                    if (buffer[0] == 't' && buffer[1] == 'e' && buffer[2] == 'x' && buffer[3] == 't' && buffer[4] == 'a' && buffer[5] == 'r' && buffer[6] == 'e' && buffer[7] == 'a')
                    {
                        State = RcDataState;
                        return;
                    }

                    if (buffer[0] == 'n' && buffer[1] == 'o' && buffer[2] == 'f' && buffer[3] == 'r' && buffer[4] == 'a' && buffer[5] == 'm' && buffer[6] == 'e' && buffer[7] == 's')
                    {
                        State = RawTextState;
                        return;
                    }

                    // noscript is not handled according to spec, always allowing noscript tags to be tokenized in case someone wants to tokenize noscript.
                    break;
                case 9:

                    if (buffer[0] == 'p' && buffer[1] == 'l' && buffer[2] == 'a' && buffer[3] == 'i' && buffer[4] == 'n' && buffer[5] == 't' && buffer[6] == 'e' && buffer[7] == 'x' && buffer[8] == 't')
                    {
                        State = PlainTextState;
                        return;
                    }

                    break;
            }

            State = DataState;
        }
    }
}
