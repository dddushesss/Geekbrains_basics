using UnityEditor.Experimental.GraphView;

namespace Dialoge_Editor.Editor.Nodes
{
    public class DialogueNode : BaseNode
    {
        public string DialogueText;

        public DialogueNode()
        {
            EntryPoint = false;
        }
    }
}