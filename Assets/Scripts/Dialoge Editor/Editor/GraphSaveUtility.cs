using System.Collections.Generic;
using System.Linq;
using Dialoge_Editor.Editor.Graph;
using Dialoge_Editor.Editor.Nodes;
using Dialoge_Editor.Runtime;
using Subtegral.DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

namespace Dialoge_Editor.Editor
{
    public class GraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<BaseNode> Nodes => _graphView.nodes.ToList().Cast<BaseNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private StoryGraphView _graphView;

        public static GraphSaveUtility GetInstance(StoryGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(dialogueContainerObject)) return;
            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            var loadedAsset =
                AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
            {
                if (loadedAsset is DialogueContainer container)
                {
                    container.NodeLinks = dialogueContainerObject.NodeLinks;
                    container.IfDialogueNodeData = dialogueContainerObject.IfDialogueNodeData; //тут приравнивание нового поля в контейнере
                    container.CheckQuestNodeData = dialogueContainerObject.CheckQuestNodeData;
                    container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                    container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                    container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                    EditorUtility.SetDirty(container);
                }
            }

            AssetDatabase.SaveAssets();
        }

        //тут добавление в список в контейнере 
        private void SaveDialogeNode(DialogueContainer dialogueContainerObject)
        {
            var dialogeNodes = Nodes.OfType<DialogueNode>().ToList();
            foreach (var node in dialogeNodes)
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = node.Guid,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }
        }
        private void SaveCheckQuestNode(DialogueContainer dialogueContainerObject)
        {
            var dialogeNodes = Nodes.OfType<CheckQuestNode>().ToList();
            foreach (var node in dialogeNodes.Where(node => !node.EntryPoint))
            {
                dialogueContainerObject.CheckQuestNodeData.Add(new CheckQuestNodeData
                {
                    NodeGUID = node.Guid,
                    FunctionName = node.Function,
                    Position = node.GetPosition().position,
                    Quest = node.QuestSO
                });
            }
        }
        
        private void SaveGiveQuestNode(DialogueContainer dialogueContainerObject)
        {
            var dialogeNodes = Nodes.OfType<GetQuestNode>().ToList();
            foreach (var node in dialogeNodes.Where(node => !node.EntryPoint))
            {
                dialogueContainerObject.IfDialogueNodeData.Add(new GiveQuestNodeData
                {
                    NodeGUID = node.Guid,
                    FunctionName = node.Function,
                    Position = node.GetPosition().position,
                    Quest = node.QuestSO
                });
            }
        }
        
        private bool SaveNodes(DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                if ((connectedSockets[i].input.node is BaseNode inputNode) &&
                    (connectedSockets[i].output.node is BaseNode outputNode))
                    dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGUID = outputNode.Guid,
                        PortName = connectedSockets[i].output.portName,
                        TargetNodeGUID = inputNode.Guid
                    });
            }
            
            SaveDialogeNode(dialogueContainerObject);
            SaveGiveQuestNode(dialogueContainerObject);
            SaveCheckQuestNode(dialogueContainerObject);
            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>()
                    .Select(x => x.Guid)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            _dialogueContainer = Resources.Load<DialogueContainer>(fileName);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).Guid = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes.Where(perNode => !perNode.EntryPoint))
            {
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>

        private void GenerateDialogeNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData.Where(x => !x.DialogueText.Equals("ENTRYPOINT")))
            {
                var tempNode = _graphView.CreateNode(perNode.DialogueText, Vector2.zero);
                tempNode.Guid = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }
        
        //тут добавление в graphview
        private void GenerateGiveQuestNodes()
        {
            foreach (var perNode in _dialogueContainer.IfDialogueNodeData.Where(x => !x.FunctionName.Equals("ENTRYPOINT")))
            {
                var tempNode = _graphView.CreateGetQuestNode(perNode.FunctionName, perNode.Quest, Vector2.zero);
                tempNode.Guid = perNode.NodeGUID;
                _graphView.AddElement(tempNode);
                var nodePort = _dialogueContainer.NodeLinks.Find(x => x.BaseNodeGUID == perNode.NodeGUID);
                var tempporty = tempNode.outputContainer.Q<Port>().portName;
                nodePort.PortName = tempporty;

            }
        }
        private void GenerateCheckQuestNodes()
        {
            foreach (var perNode in _dialogueContainer.CheckQuestNodeData.Where(x => !x.FunctionName.Equals("ENTRYPOINT")))
            {
                var tempNode = _graphView.CreateCheckQuestNode(perNode.FunctionName, perNode.Quest, Vector2.zero);
                tempNode.Guid = perNode.NodeGUID;
                _graphView.AddElement(tempNode);
            }
        }
        
        
        
        private void GenerateNodes()
        {
            GenerateDialogeNodes();
            GenerateGiveQuestNodes();
            GenerateCheckQuestNodes();
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].Guid).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.Guid == targetNodeGuid);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    //тут задание позиции ноды
                    switch (targetNode)
                    {
                        case DialogueNode _:
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.DialogueNodeData.First(x => x.NodeGUID == targetNodeGuid).Position,
                                _graphView.DefaultNodeSize));
                            break;
                        case GetQuestNode _:
                            targetNode.SetPosition(new Rect(
                                _dialogueContainer.IfDialogueNodeData.First(x => x.NodeGUID == targetNodeGuid).Position,
                                _graphView.DefaultNodeSize));
                            break;
                        case CheckQuestNode _:
                            targetNode.SetPosition((new Rect(
                                _dialogueContainer.CheckQuestNodeData.First(x => x.NodeGUID == targetNodeGuid).Position,
                                _graphView.DefaultNodeSize)));
                            break;
                    }
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _dialogueContainer.CommentBlockData)
            {
                var block = _graphView.CreateCommentBlock(
                    new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                    commentBlockData);
                block.AddElements(Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.Guid)));
            }
        }
    }
}