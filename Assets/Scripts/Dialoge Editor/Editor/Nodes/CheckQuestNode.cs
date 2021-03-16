using UnityEngine;

namespace Dialoge_Editor.Editor.Nodes
{
    public class CheckQuestNode : BaseNode
    {
        public string Function;

        public ScriptableObject QuestSO;
        //добавление класса, описывающий ноду. 

        public CheckQuestNode()
        {
            EntryPoint = false;
        }
    }
}