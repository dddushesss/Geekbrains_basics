using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "Sword", menuName = "Iteams/Sword")]
    public class Sword : Iteam
    {
        public int damage;
        
        
        public override void Usage()
        {
            throw new System.NotImplementedException();
        }
    }
    
}