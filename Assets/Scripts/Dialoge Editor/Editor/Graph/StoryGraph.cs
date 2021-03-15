using System;
using System.IO;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Dialoge_Editor.Runtime;
using Button = UnityEngine.UIElements.Button;

namespace Dialoge_Editor.Editor.Graph
{
    public class StoryGraph : EditorWindow
    {
        public string fileName = "New Narrative";
        private Toolbar _toolbar;
        public TextField FileNameTextField;
        private StoryGraphView _graphView;
        private DialogueContainer _dialogueContainer;


        [MenuItem("Game Data/Dialoge Editor")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<StoryGraph>();
            window.titleContent = new GUIContent("Narrative Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new StoryGraphView(this)
            {
                name = "Narrative Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            _toolbar = new Toolbar();
            FileNameTextField = new TextField("File Name:");
            FileNameTextField.SetValueWithoutNotify(fileName);
            FileNameTextField.MarkDirtyRepaint();
            FileNameTextField.RegisterValueChangedCallback(evt => { fileName = evt.newValue; });
            _toolbar.Add(FileNameTextField);
            _toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});
            _toolbar.Add(new Button(() => _graphView.OpenLoader(this)) {text = "Load dialoge"});
            rootVisualElement.Add(_toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save)
                {
                    saveUtility.SaveGraph(fileName);
                }
                else
                {
                    saveUtility.LoadNarrative(fileName);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }


        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            GenerateBlackBoard();
            try
            {
                var sr = new StreamReader("Assets/Resources/LastOpenedDialoge.txt");
                fileName = sr.ReadLine();
                FileNameTextField.value = fileName;
                sr.Close();
            }
            catch(Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }

            RequestDataOperation(false);
        }


        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap {anchored = true};
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance());
            };
            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }

        private void OnDisable()
        {
            RequestDataOperation(true);
            try
            {
                var sw = new StreamWriter("Assets/Resources/LastOpenedDialoge.txt");
                sw.WriteLine(fileName);
                sw.Close();
            }
            catch(Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }
            rootVisualElement.Remove(_graphView);
        }
    }
}