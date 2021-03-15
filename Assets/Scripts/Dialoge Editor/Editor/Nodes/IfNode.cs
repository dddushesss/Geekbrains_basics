using UnityEngine;

namespace Dialoge_Editor.Editor.Nodes
{
    public class IfNode : BaseNode
    {
        public string Function;

        public ScriptableObject QuestSO;
        //добавление класса, описывающий ноду. 

        public IfNode()
        {
            EntryPoint = false;
        }
    }
}