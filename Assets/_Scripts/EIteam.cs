using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    [CustomEditor(typeof(Iteam), true)]
    public class EIteam : Editor
    {
        private Iteam _iteam;
        
        private List<SerializedProperty> _attributes;
        
        public void OnEnable()
        {
            _iteam = (Iteam) target;
            _attributes = new List<SerializedProperty>();
            switch (_iteam)
            {
                case Sword _:
                    _attributes.Add(serializedObject.FindProperty("damage"));
                    break;
                case Poition _:
                    _attributes.Add(serializedObject.FindProperty("effectCount"));
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _iteam.name = EditorGUILayout.TextField("Название предмета", _iteam.name);
            _iteam.description = EditorGUILayout.TextField("Описание", _iteam.description);
            _iteam.prefab = (GameObject) EditorGUILayout.ObjectField("Prefab", _iteam.prefab, typeof(GameObject));
            _iteam.Icon = EditorGUILayout.ObjectField("Icon", _iteam.Icon, typeof(Sprite)) as Sprite;
            
            foreach (var attribute in _attributes)
            {
                EditorGUILayout.PropertyField(attribute);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}