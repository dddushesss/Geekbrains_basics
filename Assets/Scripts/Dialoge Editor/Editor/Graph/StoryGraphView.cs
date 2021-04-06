﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dialoge_Editor.Editor.Nodes;
using Subtegral.DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialoge_Editor.Editor.Graph
{
    public class StoryGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;
        private ResourcesLoader _resourcesLoader;

        public StoryGraphView(StoryGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());

            AddSearchWindow(editorWindow);
        }

        public void OpenLoader(StoryGraph editorWindow)
        {
            AddResoursecLoaderWindow(editorWindow);
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        private void AddResoursecLoaderWindow(StoryGraph editorWindow)
        {
            _resourcesLoader = ScriptableObject.CreateInstance<ResourcesLoader>();
            _resourcesLoader.Configure(editorWindow, this);
            SearchWindow.Open(new SearchWindowContext(new Vector2(900, 500)), _resourcesLoader);
        }

        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            if (commentBlockData == null)
                commentBlockData = new CommentBlockData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField {text = localPropertyName, typeText = "string"};
            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(string nodeName, Vector2 position)
        {
            AddElement(CreateNode(nodeName, position));
        }

        //функция для добавление ноды на graphView и описание интерфейса ноды
        public void CreateNewGetQuestNode(string functionName, ScriptableObject Quest, Vector2 position)
        {
            var GetQuestNode = CreateGetQuestNode(functionName, Quest, position);
            CreatePortWhithTextField(GetQuestNode);
            CreatePortWhithTextField(GetQuestNode);
            AddElement(GetQuestNode);
        }

        public void CreateNewCheckQuestNode(string haveQuest, ScriptableObject Quest, Vector2 position)
        {
            var CheckQuestNode = CreateCheckQuestNode(haveQuest, Quest, position);
            CreatePortWhithTextField(CheckQuestNode);
            CreatePortWhithTextField(CheckQuestNode);
            AddElement(CheckQuestNode);
        }

        public GetQuestNode CreateGetQuestNode(string functionName, ScriptableObject Quest,
            Vector2 position)
        {
            var tempDialogueNode = new GetQuestNode()
            {
                QuestSO = Quest,
                title = "Quest node",
                Function = functionName,
                Guid = Guid.NewGuid().ToString()
            };
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogeNode"));
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);

            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            var textField = new TextField("") {multiline = true};
            textField.RegisterValueChangedCallback(evt => { tempDialogueNode.Function = evt.newValue; });
            textField.SetValueWithoutNotify(tempDialogueNode.Function);
            tempDialogueNode.mainContainer.Add(textField);


            var gameobjectField = new ObjectField()
            {
                objectType = typeof(ScriptableObject)
            };

            gameobjectField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.QuestSO = evt.newValue as ScriptableObject;
            });
            gameobjectField.SetValueWithoutNotify(Quest);
            tempDialogueNode.contentContainer.Add(gameobjectField);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            return tempDialogueNode;
        }

        public void CreatePortWhithTextField(Node tempNode, string overrideText = "")
        {
            var questPort = GetPortInstance(tempNode, Direction.Output);
            questPort.portName = overrideText;
            var portLabel = questPort.contentContainer.Q<Label>("type");
            questPort.contentContainer.Remove(portLabel);
            questPort.contentContainer.Add(new Label(" "));
            var questPortTextField = new TextField()
            {
                name = string.Empty,
                value = questPort.portName
            };
            questPortTextField.RegisterValueChangedCallback(evt => questPort.portName = evt.newValue);
            questPort.contentContainer.Add(questPortTextField);
            tempNode.outputContainer.Add(questPort);
            tempNode.RefreshPorts();
            tempNode.RefreshExpandedState();
        }

        public CheckQuestNode CreateCheckQuestNode(string hasQuest, ScriptableObject Quest, Vector2 position)
        {
            var tempDialogueNode = new CheckQuestNode()
            {
                QuestSO = Quest,
                title = "Quest node",
                Guid = Guid.NewGuid().ToString()
            };
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogeNode"));
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);
            
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize));


            var gameobjectField = new ObjectField()
            {
                objectType = typeof(ScriptableObject)
            };

            gameobjectField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.QuestSO = evt.newValue as ScriptableObject;
            });

            gameobjectField.SetValueWithoutNotify(Quest);
            tempDialogueNode.contentContainer.Add(gameobjectField);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            return tempDialogueNode;
        }

        public DialogueNode CreateNode(string nodeName, Vector2 position)
        {
            var tempDialogueNode = new DialogueNode()
            {
                title = "Dialoge Node",
                DialogueText = nodeName,
                Guid = Guid.NewGuid().ToString()
            };
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogeNode"));
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            var textField = new TextField("") {multiline = true};
            textField.RegisterValueChangedCallback(evt => { tempDialogueNode.DialogueText = evt.newValue; });
            textField.SetValueWithoutNotify(tempDialogueNode.DialogueText);

            tempDialogueNode.mainContainer.Add(textField);
            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);
            return tempDialogueNode;
        }


        public IfNode CreateIfNode(string functionName, Vector2 position)
        {
            var tempDialogueNode = new IfNode()
            {
                title = "If Node",
                FunctionName = functionName,
                Guid = Guid.NewGuid().ToString()
            };
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("DialogeNode"));
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            var textField = new TextField("") {multiline = true};
            textField.RegisterValueChangedCallback(evt => { tempDialogueNode.FunctionName = evt.newValue; });
            textField.SetValueWithoutNotify(tempDialogueNode.FunctionName);

            tempDialogueNode.mainContainer.Add(textField);
            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);
            return tempDialogueNode;
        }


        public void AddChoicePort(Node nodeCache, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;


            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            var enumerable = targetEdge as Edge[] ?? targetEdge.ToArray();
            if (enumerable.Any())
            {
                var edge = enumerable.First();
                edge.input.Disconnect(edge);
                RemoveElement(enumerable.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(Node node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                title = "START",
                Guid = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true
            };

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }
    }
}