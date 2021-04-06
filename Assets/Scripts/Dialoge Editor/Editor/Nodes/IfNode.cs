using UnityEngine;

namespace Dialoge_Editor.Editor.Nodes
{
    public class IfNode : BaseNode
    {
        public string FunctionName;

        public IfNode()
        {
            EntryPoint = false;
        }
    }
}