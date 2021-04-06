using UnityEngine;

namespace Scripts
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        public string QuestText;
        public int ExpValue;
        public Iteam condition;
        public bool IsDone;
        public string doneText;
        
        public override bool Equals(object other)
        {
            var quest = (Quest) other;
            if (quest != null)
                return quest.QuestText.Equals(QuestText)
                       && quest.condition.Equals(condition)
                       && quest.ExpValue.Equals(ExpValue);
            else
            {
                return false;
            }
        }

        protected bool Equals(Quest other)
        {
            return base.Equals(other) && QuestText == other.QuestText && ExpValue == other.ExpValue && Equals(condition, other.condition);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (QuestText != null ? QuestText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ExpValue;
                hashCode = (hashCode * 397) ^ (condition != null ? condition.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Complete()
        {
            var player = condition.playerInv.gameObject;
            condition.CurInventory.RemoveIteam(condition);
            player.GetComponent<Atributes>().atributeUICanvas.transform.GetChild(0).GetChild(1)
                .GetComponent<SkillPoints>().ChangeSkillPointsValue(ExpValue);
            player.GetComponent<QuestsList>().RemoveQuest(this);
        }
    }
}