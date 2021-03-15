using System.Collections.Generic;
using Dialoge_Editor.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialoge_Editor.Editor.Graph
{
    public class NodeSearchWindow : ScriptableObject,ISearchWindowProvider
    {
        private EditorWindow _window;
        private StoryGraphView _graphView;

        private Texture2D _indentationIcon;
        
        public void Configure(EditorWindow window,StoryGraphView graphView)
        {
            _window = window;
            _graphView = graphView;
            
            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    level = 2, userData = new DialogueNode()
                },
                //добавление элемента в searchWindow
                new SearchTreeEntry(new GUIContent("If Node", _indentationIcon))
                {
                    level = 2, userData = new IfNode()
                },
                new SearchTreeEntry(new GUIContent("Comment Block",_indentationIcon))
                {
                    level = 1,
                    userData = new Group()
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (searchTreeEntry.userData)
            {
                //вызов метода из search window
                case IfNode ifNode:
                    _graphView.CreateNewIfNode("GivePhrase",null, graphMousePosition);
                    return true;
                case DialogueNode dialogueNode:
                    _graphView.CreateNewDialogueNode("Dialogue Node",graphMousePosition);
                    return true;
                case Group group:
                    var rect = new Rect(graphMousePosition, _graphView.DefaultCommentBlockSize);
                     _graphView.CreateCommentBlock(rect);
                    return true;
            }
            return false;
        }
    }
}