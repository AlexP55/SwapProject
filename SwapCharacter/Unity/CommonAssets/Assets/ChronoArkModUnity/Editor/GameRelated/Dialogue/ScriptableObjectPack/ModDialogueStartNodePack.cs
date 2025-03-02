using ChronoArkMod.InUnity;

using DG.Tweening.Plugins.Options;
using Dialogical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue
{
    public class ModDialogueStartNodePack : DialogueNodePack
    {
        private ModDialogueStartNode Value => Value<ModDialogueStartNode>();



        public override void Init(Vector2 position, DialogueTreePack parentTree)
        {
            base.Init(position, parentTree);
            Name = "ModStart";
            nodeRect.width = 170;
            Value.Text = new List<string>() { "" };
           
            var modstartevent = MakeEvent<ModDialogueStartEvent>();
            parentTree.ModEvent = modstartevent;
            modstartevent.name = "ModStartEvent";
            Value.Events1 = new List<DialogueEvent> { modstartevent };
            CreateOption(true);
            modstartevent.ModID = ModProjectSetting.Setting.ModID;
            var importer = AssetImporter.GetAtPath("Assets/ModAssets");
            modstartevent.AssetBundlePath =  importer.assetBundleName;

        }
        public ModDialogueStartEvent ModEvent
        {
            get
            {
                return Value.Events1[0] as ModDialogueStartEvent;
            }
        }
        public override void DrawNodeWindow(int id)
        {
            EditorGUILayout.LabelField("Mod ID");
            EditorGUILayout.LabelField(ModEvent.ModID, new GUIStyle(GUI.skin.textArea) { clipping = TextClipping.Clip });
            EditorGUILayout.LabelField("AssetBundle Path");

            EditorGUILayout.LabelField(ModEvent.AssetBundlePath, new GUIStyle(GUI.skin.textArea) { clipping = TextClipping.Clip });
            if (Event.current.type == EventType.Repaint)
            {

                Rect lastRect = GUILayoutUtility.GetLastRect();
                nodeRect.height = lastRect.y + lastRect.height + 4;
            }
            ProcessEvents(Event.current);
            GUI.DragWindow();

        }
        public override void OnDetailGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mod ID", GUILayout.Width(100));
            ModEvent.ModID = EditorGUILayout.TextField(ModEvent.ModID);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("AssetBundle Path");
            ModEvent.AssetBundlePath = EditorGUILayout.TextField(ModEvent.AssetBundlePath);
        }
        public override void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        DialogueDetailWindow.ActiveObject = this;
                    }


                    break;

            }
        }
    }
}
