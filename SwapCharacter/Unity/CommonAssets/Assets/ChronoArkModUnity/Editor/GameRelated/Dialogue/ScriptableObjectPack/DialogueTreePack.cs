using Dialogical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue
{

    public class DialogueTreePack : ScriptableObjectPack<DialogueTree>
    {
       
        private DialogueTree Value => Value<DialogueTree>();

        public ModDialogueStartEvent ModEvent;

        public List<BaseNodePack> nodes = new List<BaseNodePack>();
        public void CreateStartNode(Vector2 where)
        {
            var startNode = DialoguePack<DialogueStartNodePack, DialogueStartNode>();
            nodes.Add(startNode);
            Value.nodes.Add(startNode.Value<DialogueStartNode>());
            startNode.Init(where, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public ModDialogueStartNodePack CreateModStartNode(Vector2 where)
        {
            var startNode = DialoguePack<ModDialogueStartNodePack, ModDialogueStartNode>();
            nodes.Add(startNode);
            Value.nodes.Add(startNode.Value<ModDialogueStartNode>());
            startNode.Init(where, this);
            return startNode;
        }
        public DialogueNodePack CreateNode(Vector2 where)
        {
            var Node = DialoguePack<DialogueNodePack, DialogueNode>();
            nodes.Add(Node);
            Value.nodes.Add(Node.Value<DialogueNode>());
            Node.Init(string.Format("{0:000}", (nodes.Count - 1)),where, this);

           

            if (nodes.Count == 3)
            {
                nodes[1].options[0].target = Node;
                
            }
            DialogueDetailWindow.ActiveObject = Node;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return Node;
        }
        public void TryDeleteNode(InstantiableNodePack node)
        {
            var confirm = EditorUtility.DisplayDialog(
              title: "Delete Node?",
            message: node.Name,
                 ok: "Delete",
                 cancel: "Cancel"
                 ); ;
            if (confirm)
            {
                DeleteNode(node);
            }
        }
        public void DeleteNode(InstantiableNodePack node)
        {
            nodes.Remove(node);
            Value.nodes.Remove(node.m_Value);
            ModEvent.StandingImagePairs.Remove(node.Value<DialogueNode>());
            node.OnDestory();
            DestroyImmediate(node,true);

        }

    }

}
