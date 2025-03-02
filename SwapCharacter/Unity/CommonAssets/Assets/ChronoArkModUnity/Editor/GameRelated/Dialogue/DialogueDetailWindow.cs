using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue 
{
    public class DialogueDetailWindow : EditorWindow
    {
        public static DialogueDetailWindow _Instance;
        [MenuItem("ChronoArk/Dialugue/Details", false, -101)]
        public static void OpenWindow()
        {
            Instance.Show();
            Instance.Focus();
        }
        public static DialogueDetailWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GetWindow<DialogueDetailWindow>("Dialugue Details", true);
                    _Instance.Init(); 
                    _Instance.Show();
                    _Instance.Focus();
                }

                return _Instance;
            }

        }
        public void Init()
        {
            Instance.minSize = new Vector2(400, 400);

        }
        public static ScriptableObject _ActiveObject;
        public static ScriptableObject ActiveObject
        {
            get
            {
                return _ActiveObject;
            }
            set
            {
                if (_ActiveObject != value)
                {
                    _ActiveObject = value;
                    Instance.Repaint();
                }

            }
        }
        public void OnGUI()
        {
            if (ActiveObject == null) return;
            if(DialogueEditorWindow.Instance.Tree==null) return;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(ActiveObject,ActiveObject.GetType(),true);
            EditorGUI.EndDisabledGroup();
            
            switch (ActiveObject)
            {
                case ModDialogueStartNodePack modstart:
                    modstart.OnDetailGUI();
                    break;
                case InstantiableNodePack node:
                    node.OnDetailGUI();
                    break;
                case DialogueBaseOptionPack option:
                    option.OnDetailGUI();
                    break;
            }
        }
    }
}
