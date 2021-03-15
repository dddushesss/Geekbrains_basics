using UnityEngine;

namespace Scripts
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        public string QuestText;
        public int ExpValue;
        
    }
}