using DG.Tweening.Plugins.Options;
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

    public class DialogueStartNodePack : BaseNodePack
    {
        private DialogueStartNode Value => Value<DialogueStartNode>();

        public override void Init(Vector2 position, DialogueTreePack parentTree)
        {
            base.Init(position, parentTree);    
            Name = "Start";

            nodeRect.size = new Vector2(50, 50);
            CreateOption(true);
        }
        public override void CreateOption(bool first = false)
        {                                                                 
            DialogueBaseOptionPack option = DialoguePack<DialogueBaseOptionPack, DialogueBaseOption>();
            options.Add(option);
            Value.options.Add(option.Value<DialogueBaseOption>());
            AddOutputPoint();
            option.Init(this);
            if (first)
            {
                CreateConnection(0, ParentTree.CreateModStartNode(DialogueEditorWindow.Instance.position.size/3+new Vector2(100,0)));
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

}
 