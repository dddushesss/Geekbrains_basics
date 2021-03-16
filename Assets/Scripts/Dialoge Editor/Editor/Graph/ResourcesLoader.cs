using System.Collections.Generic;
using System.IO;
using Dialoge_Editor.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dialoge_Editor.Editor.Graph
{
    public class ResourcesLoader : ScriptableObject, ISearchWindowProvider
    {
        private StoryGraph _window;
        private StoryGraphView _graphView;
        private Texture2D _indentationIcon;

        public void Configure(StoryGraph window, StoryGraphView graphView)
        {
            _window = window;
            _graphView = graphView;

            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var fileNames = Directory.GetFiles("Assets\\Resources");

            var containers = new List<string>();
            foreach (var filePath in fileNames)
            {
                var fileName = filePath.Replace(".asset", "");
                var res = Resources.Load<DialogueContainer>(fileName.Replace("Assets\\Resources\\", ""));
                if (res != null)
                {
                    containers.Add(fileName.Replace("Assets\\Resources\\", ""));
                }
            }
            var tree = new List<SearchTreeEntry>
            {
                
                new SearchTreeGroupEntry(new GUIContent("Load"))
            };
            foreach (var container in containers)
            {
                var treeSearchEntry = new SearchTreeEntry(new GUIContent(container)) {level = 1, userData = container};
                tree.Add(treeSearchEntry);
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            _window.fileName = searchTreeEntry.userData.ToString();
            _window.FileNameTextField.value = searchTreeEntry.userData.ToString();
            saveUtility.LoadNarrative(searchTreeEntry.userData.ToString());
            return true;
        }
    }
}