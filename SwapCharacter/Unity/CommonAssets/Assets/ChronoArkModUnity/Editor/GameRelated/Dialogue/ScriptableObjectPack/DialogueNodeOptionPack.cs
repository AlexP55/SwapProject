
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
    public class DialogueNodeOptionPack : DialogueBaseOptionPack
    {
        private DialogueNodeOption Value => Value<DialogueNodeOption>();


        public override void Init(BaseNodePack parent)
        {
            base.Init(parent);
            Value.Text.Add("");
        }
        public override void DrawNodeWindow()
        {
            base.DrawNodeWindow();
            var rect_Remove = new Rect(rect.x + 20, rect.y + 7.5f, 15, rect.height - 15);
            if (rect_Remove.Contains(Event.current.mousePosition))
            {

                GUI.color = new Color(0.7f, 0.7f, 0.7f);
            }
            if (GUI.Button(rect_Remove, "-"))
            {
                Parent.TryDeleteOption(this);
            }
            GUI.color = Color.white;
            var rect_Content = new Rect(rect.x + 40, rect.y + 7.5f, rect.width - 50, rect.height - 15);
            EditorGUI.LabelField(rect_Content, Value.Text[0], new GUIStyle(GUI.skin.textArea));
        }
        public override void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu(); 


            genericMenu.AddItem(new GUIContent("Remove Option"), false, () => Parent.TryDeleteOption(this));
            genericMenu.AddItem(new GUIContent("Connection Start"), false, () => ConnectionManager.OnClickOutput(Parent,Index));
            genericMenu.ShowAsContext();
        }
        public override void OnDetailGUI()
        {
            base.OnDetailGUI();
            EditorGUILayout.LabelField("Option Text");
            if (Value.Text.Count == 0) Value.Text = new List<string>() { "" };
            Value.Text[0] = EditorGUILayout.TextArea(Value.Text[0]);
            Value.preCondition = EditorGUILayout.TextField("Pre-Condition",Value.preCondition);
            Value.postEvent = EditorGUILayout.TextField("Post-Event", Value.postEvent);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Shown Once Per Conversation");
            Value.isShownOncePerConversation = EditorGUILayout.Toggle(Value.isShownOncePerConversation);
            GUILayout.EndHorizontal();
        }
    }

}
