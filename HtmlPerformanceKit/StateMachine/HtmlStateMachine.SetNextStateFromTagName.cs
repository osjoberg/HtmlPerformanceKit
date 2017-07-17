namespace HtmlPerformanceKit.StateMachine
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
                    if (name.Equals("xmp"))
                    {
                        State = RawTextState;
                        return;
                    }

                    goto default;

                case 5:
                    if (name.Equals("title"))
                    {
                        State = RcDataState;
                        return;
                    }

                    if (name.Equals("style"))
                    {
                        State = RawTextState;
                        return;
                    }

                    goto default;

                case 6:
                    if (name.Equals("iframe"))
                    {
                        State = RawTextState;
                        return;
                    }

                    if (name.Equals("script"))
                    {
                        State = ScriptDataState;
                        return;
                    }

                    goto default;

                case 7:
                    if (name.Equals("noembed"))
                    {
                        State = RawTextState;
                        return;
                    }

                    goto default;

                case 8:
                    if (name.Equals("textarea"))
                    {
                        State = RcDataState;
                        return;
                    }

                    if (name.Equals("noframes"))
                    {
                        State = RawTextState;
                        return;
                    }

                    // noscript is not handled according to spec, always allowing noscript tags to be tokenized in case someone wants to tokenize noscript.
                    goto default;
                case 9:

                    if (name.Equals("plaintext"))
                    {
                        State = PlainTextState;
                        return;
                    }

                    goto default;

                default:
                    State = DataState;
                    return;
            }
        }
    }
}
