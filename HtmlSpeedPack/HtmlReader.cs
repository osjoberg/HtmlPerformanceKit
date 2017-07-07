using System.IO;
using HtmlSpeedPack.StateMachine;

namespace HtmlSpeedPack
{
    public class HtmlReader
    {
        private readonly HtmlStateMachine stateMachine;

        public HtmlReader(StreamReader textReader)
        {
            stateMachine = new HtmlStateMachine(textReader);
        }

        public HtmlNodeType NodeType { get; private set; }

        public bool SelfClosingElement => stateMachine.EmitTagToken?.SelfClosing ?? false;

        public string Name => stateMachine.EmitTagToken?.Name.ToString();

        public string Text => stateMachine.EmitDataBuffer.ToString();

        public string Comment => stateMachine.EmitCommentBuffer.ToString();

        public bool Read()
        {
            stateMachine.ResetEmit();

            while (true)
            {
                stateMachine.State();

                if (stateMachine.EmitTagToken != null)
                {
                    NodeType = HtmlNodeType.Tag;
                    return true;
                }

                if (stateMachine.EmitDataBuffer != null)
                {
                    NodeType = HtmlNodeType.Text;
                    return true;
                }

                if (stateMachine.EmitCommentBuffer != null)
                {
                    NodeType = HtmlNodeType.Comment;
                    return true;
                }

                if (stateMachine.EmitDoctypeToken != null)
                {
                    NodeType = HtmlNodeType.Doctype;
                    return true;
                }

                if (stateMachine.Eof)
                {
                    return false;
                }
            }
        }

        public string GetAttribute(string name)
        {
            return stateMachine.EmitTagToken?.Attributes[name]?.ToString();
        }
    }
}
