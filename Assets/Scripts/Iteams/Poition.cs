using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "Poition", menuName = "Iteams/Poition")]
    public class Poition : Iteam
    {
        [SerializeField]
        private int effectCount;
        
        
        public override void Usage()
        {
            playerInv.gameObject.GetComponent<Atributes>().ChangeHealth(effectCount);
        }
    }
}