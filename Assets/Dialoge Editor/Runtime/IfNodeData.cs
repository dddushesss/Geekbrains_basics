using System;
using UnityEngine;

namespace Dialoge_Editor.Runtime
{
    [Serializable]
    public class IfNodeData
    {
        //добавление класса для контейнера
        public string NodeGUID;
        public string FunctionName;
        public Vector2 Position;
    }
}