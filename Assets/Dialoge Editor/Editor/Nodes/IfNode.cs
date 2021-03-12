using UnityEditor.Experimental.GraphView;

namespace Dialoge_Editor.Editor.Nodes
{
    public class IfNode : BaseNode
    {
        public string Function;
        //добавление класса, описывающий ноду. 

        public IfNode()
        {
            EntryPoint = false;
        }
    }
}