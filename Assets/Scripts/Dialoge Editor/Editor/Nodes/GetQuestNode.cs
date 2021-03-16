using UnityEngine;

namespace Dialoge_Editor.Editor.Nodes
{
    public class GetQuestNode : BaseNode
    {
        public string Function;

        public ScriptableObject QuestSO;
        //добавление класса, описывающий ноду. 

        public GetQuestNode()
        {
            EntryPoint = false;
        }
    }
}