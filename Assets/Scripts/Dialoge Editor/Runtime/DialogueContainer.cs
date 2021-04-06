using System;
using System.Collections.Generic;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine;

namespace Dialoge_Editor.Runtime
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<GiveQuestNodeData> IfDialogueNodeData = new List<GiveQuestNodeData>();
        public List<CheckQuestNodeData> CheckQuestNodeData = new List<CheckQuestNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
    }
}