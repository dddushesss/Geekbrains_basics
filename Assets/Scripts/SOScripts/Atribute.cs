using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Atribute : ScriptableObject
{
   public Atributes CharacterAtributes;
   public int value;
   public int MaxValue;
   
   public virtual int GetAtributeValue()
   {
      return value;
   }

   public override string ToString()
   {
      return "Атрибут";
   }

   public abstract string GetDesctiption();

   public string GetAtributeName()
   {
      return this.ToString();
   }

   public virtual void ChangeAtributeValue(int count)
   {
      value += count;
   }

   public virtual int ThrowCube()
   {
      return value + Random.Range(1, 6);
   }
}
