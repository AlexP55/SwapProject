using ChronoArkMod.ModData;
using DG.Tweening.Plugins.Options;
using Dialogical;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Profiling;
using UnityEngine.UI;
using static ChronoArkMod.ModData.ModAssetInfo;

namespace ChronoArkMod.InUnity.Dialogue
{

    public class DialogueNodePack : InstantiableNodePack
    {
        private DialogueNode Value => Value<DialogueNode>();
        [SerializeField]
        public string m_CharacterName;
        public string CharacterName
        {
            get
            {
                if(Value.NameIndex == 0)
                {
                    return m_CharacterName;
                }
                else
                {
                    return LocalizationIndexToName(Value.NameIndex);
                }
            }
            set
            {
                m_CharacterName = value;
                SetText();
            }
        }
        

        [SerializeField]
        public string m_Text;
        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
                SetText();
            }
        }
        public void SetText()
        {
            if (Value.Text.Count == 0) Value.Text = new List<string>() { "" };
            if (Value.NameIndex == 0&&!string.IsNullOrEmpty(m_CharacterName))
            {
                Value.Text[0] = $"*{m_CharacterName}\n{m_Text}";
            }
            else
            {
                Value.Text[0] = m_Text;
            }
            
        }
        private Sprite m_Standing;
        public Sprite Standing
        {
            get
            {
                if(m_Standing == null&&!string.IsNullOrEmpty(StandingPath))
                {
                    if (!UseChronoArkImage)
                    {
                        m_Standing = AssetDatabase.LoadAssetAtPath<Sprite>(StandingPath);
                    }
                    else
                    {
                        m_Standing = Addressables.LoadAssetAsync<Sprite>(StandingPath).WaitForCompletion();
                    }
                    
                }
                return m_Standing;
            }
            set
            {
                m_Standing = value;
                if (!UseChronoArkImage)
                {
                   
                    StandingPath = AssetDatabase.GetAssetPath(value);
                }
            }
        }
        public bool m_UseChronoArkFace;
        public bool UseChronoArkFace
        {
            get
            {
                return (m_UseChronoArkFace);
            }
            set
            {
                if (m_UseChronoArkFace != value)
                {
                    m_UseChronoArkFace = value;
                    if (m_UseChronoArkFace)
                    {
                        ParentTree.ModEvent.FaceImagePairs[Value] = "";
                        Value.FaceChip = null;
                        _FaceChip = null;
                    }
                    else
                    {
                        ParentTree.ModEvent.FaceImagePairs.Remove(Value);
                        Value.FaceChip = null;
                        

                    }
                }



            }
        }
        public string ChronoArkFacePath
        {
            get
            {
                if (m_UseChronoArkFace)
                {
                    
                    if(ParentTree.ModEvent.FaceImagePairs.TryGetValue(Value, out var facepath))
                    {
                        return facepath;
                    }
                    else
                    {
                        ParentTree.ModEvent.FaceImagePairs[Value] = "";
                        return "";
                    }
                }
                else
                {
                    ParentTree.ModEvent.FaceImagePairs.Remove(Value);
                    return "";
                }

            }
            set
            {
                if (m_UseChronoArkFace)
                {
                    ParentTree.ModEvent.FaceImagePairs[Value] = value;
                    Value.FaceChip = null;
                    _FaceChip = null;
                }
                else
                {
                    ParentTree.ModEvent.FaceImagePairs.Remove(Value);
                    
                }


            }
        }
        Texture _FaceChip;
        public Texture FaceChip
        {
            get
            {
                if(_FaceChip == null)
                {
                    if (!string.IsNullOrEmpty(ChronoArkFacePath))
                    {
                        var async = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Texture>(ChronoArkFacePath);
                        ChronoArkAssetLoader.AllHandle.Add(async);
                        _FaceChip = async.WaitForCompletion();
                    }

                }
                
                return _FaceChip;
            }
        }
        public bool m_UseChronoArkImage;
        public bool UseChronoArkImage
        {
            get
            {
                return (m_UseChronoArkImage);
            }
            set
            {
                if (m_UseChronoArkImage != value)
                {
                    m_UseChronoArkImage = value;
                    if (m_UseChronoArkImage)
                    {
                        ParentTree.ModEvent.StandingImagePairs.Remove(Value);
                        Value.Standing_Path = "";
                        m_Standing = null;

                    }
                    else
                    {
                        ParentTree.ModEvent.StandingImagePairs[Value] = "";
                        Value.Standing_Path = "";
                        m_Standing = null;
                    }
                }
               
                
               
            }
        }
        public string StandingPath
        {
            get
            {
                if (m_UseChronoArkImage)
                {
                    return Value.Standing_Path;
                }
                else
                {
                    if (ParentTree.ModEvent.StandingImagePairs.ContainsKey(Value))
                    {
                        return ParentTree.ModEvent.StandingImagePairs[Value];
                    }
                    return "";
                }
 
            }
            set
            {
                if (m_UseChronoArkImage)
                {
                    ParentTree.ModEvent.StandingImagePairs.Remove(Value);
                    Value.Standing_Path = value;
                }
                else
                {
                    ParentTree.ModEvent.StandingImagePairs[Value] = value;
                    Value.Standing_Path = "";
                }
               

            }
        }
        public void Init(string Title,Vector2 position, DialogueTreePack parentTree)
        {
            base.Init(position, parentTree);
            Name = Title;
            nodeRect.width = 170;
            CreateOption(true);
            
        }
        public override void CreateOption(bool first = false)
        {
            var option = DialoguePack<DialogueNodeOptionPack, DialogueNodeOption>();
            options.Add(option);
            Value.options.Add(option.Value<DialogueBaseOption>());
            AddOutputPoint();
            option.Init(this);
            option.SetIndex();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if(first)
            {
                (options[0] as DialogueNodeOptionPack).Value<DialogueNodeOption>().Text[0] = "Continue";
            }
            else
            {
                DialogueDetailWindow.ActiveObject = option;
            }
            
                                                                                      
        }
        public override void DrawNodeWindow(int id)
        {
            EditorGUILayout.LabelField("Character Name");
            EditorGUILayout.LabelField(CharacterName, new GUIStyle(GUI.skin.textArea) { clipping = TextClipping.Clip });
            EditorGUILayout.LabelField("Dialogue Text");

            EditorGUILayout.LabelField(Text, new GUIStyle(GUI.skin.textArea) { clipping = TextClipping.Clip });
            if(Value.Events1.Count > 0)
            {
                EditorGUILayout.LabelField("Dialogue Event: " + Value.Events1.Count);
            }
            

            base.DrawNodeWindow(id);

        }

        public static GameObject ShowFace;
        public override void OnDetailGUI()
        {
            base.OnDetailGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Character Name", GUILayout.Width(100));
            
            List<string> list = new List<string>() { "???" };
            for (int i = 1; i < 27; i++)
            {
                list.Add(LocalizationIndexToName(i));
            }
            Value.NameIndex = EditorGUILayout.Popup(Value.NameIndex, list.ToArray());
            
            if(Value.NameIndex == 0)
            {
                CharacterName = EditorGUILayout.TextField(CharacterName);
            }
            else
            {
                CharacterName = "";
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Dialogue Text");
            Text=EditorGUILayout.TextArea(Text);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use the Previous Node's Image", GUILayout.Width(200));
            Value.StandingBefore = EditorGUILayout.Toggle(Value.StandingBefore);
            GUILayout.EndHorizontal();
            if(!Value.StandingBefore)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Use ChronoArk Image", GUILayout.Width(200));
                UseChronoArkImage = EditorGUILayout.Toggle(UseChronoArkImage);
                GUILayout.EndHorizontal();
               
                if (UseChronoArkImage)
                {
                    EditorGUILayout.LabelField("Character Image Path (ChronoArk)");
                    StandingPath = EditorGUILayout.TextField(StandingPath);

                }
                else
                {
                    EditorGUILayout.LabelField("Character Image");
                    Standing = (Sprite)EditorGUILayout.ObjectField(Standing, typeof(Sprite), false, GUILayout.Width(90), GUILayout.Height(90));
                }


            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use ChronoArk Face Chip", GUILayout.Width(200));
            UseChronoArkFace = EditorGUILayout.Toggle(UseChronoArkFace);
            GUILayout.EndHorizontal();
            if (!EditorApplication.isPlaying)
            {
                if(ShowFace!=null)
                DestroyImmediate(ShowFace);
                ShowFace = null;
            }
            if (UseChronoArkFace)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Face Chip Path (ChronoArk)");
                List<string> path = new List<string>() { "" };
                for (int i = 0; i < 10; i++) path.Add($"F_Lucy_{i}.png");
                int index = path.Select(s => "Assets/DialogueScene/image/Standing/Face/" + s).ToList().IndexOf(ChronoArkFacePath);
                if (index < 0) index = 0;
                index = EditorGUILayout.Popup(index, path.ToArray());
                if (index > 0)
                {
                    ChronoArkFacePath = "Assets/DialogueScene/image/Standing/Face/" + path[index];
                }
                GUILayout.EndHorizontal();
                ChronoArkFacePath = EditorGUILayout.TextField(ChronoArkFacePath);
                if(GUILayout.Button("Enter Play Mode to Show in Scene"))
                {
                    EditorApplication.isPlaying = true;

                    var i = ShowFace?.GetComponent<Image>();
                    if (i != null) i.sprite = null;
                    if (EditorApplication.isPlaying)
                    {

                        if (ShowFace == null)
                        {
                            ShowFace = new GameObject("Face Chip");
                            ShowFace.AddComponent<Canvas>();
                        }
                        var img = ShowFace.GetComponent<Image>();
                        if (img == null)
                        {
                            img = ShowFace.AddComponent<Image>();
                        }
                        var Async = Addressables.LoadAssetAsync<Sprite>(ChronoArkFacePath);
                        ChronoArkAssetLoader.AllHandle.Add(Async);
                        img.sprite = Async.WaitForCompletion();
                        img.SetNativeSize();
                        img.useSpriteMesh = true;

                    }
                }



            }
            else
            {
                EditorGUILayout.LabelField("Face Chip");
                Value.FaceChip = (Sprite)EditorGUILayout.ObjectField(Value.FaceChip, typeof(Sprite), false, GUILayout.Width(90), GUILayout.Height(90));
            }
            Value.localEvent =  EditorGUILayout.TextField("Local Event",Value.localEvent);

            if (Event.current.type == EventType.Repaint)
                reorderableList.list = Value.Events1;
            reorderableList.DoLayoutList();
            if(NowEvent != null)
            {
                
               Editor.CreateEditor(NowEvent).OnInspectorGUI();
            }
        }
        ReorderableList _reorderableList;
        public ReorderableList reorderableList
        {
            get
            {
                if(_reorderableList == null)
                {
                    var reorderable = new ReorderableList(
                        elements: Value.Events1,
                        elementType: typeof(DialogueEvent),
                        draggable: true,
                        displayHeader: true,
                        displayAddButton: true,
                        displayRemoveButton: true
                        );
                    reorderable.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        DialogueEvent element = Value.Events1[index];
                        element.name = Name+'_'+element.GetType().Name;
                        EditorGUI.LabelField(rect, element.GetType().Name);
                    };

                    reorderable.onRemoveCallback = (lst) =>
                    {
                        DialogueEvent ev = Value.Events1[lst.index];
                        Value.Events1.Remove(ev);
                        DestroyImmediate(ev, true);
                    };

                    reorderable.onAddCallback = (lst) =>
                    {
                        DialogueEvent evt = MakeEvent(DialogueEventTypes[DialogueEventTypesIndex]) as DialogueEvent;
                        Value.Events1.Add(evt);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    };

                    reorderable.onMouseUpCallback = (lst) =>
                    {
                        NowEvent = Value.Events1[lst.index];
                    };
                    reorderable.drawHeaderCallback = (rect) =>
                         EditorGUI.LabelField(rect, "Dialogue Events");


                    reorderable.drawFooterCallback = (rect) =>
                    {
                        DialogueEventTypesIndex = EditorGUI.Popup(new Rect(0, rect.y, rect.xMax - 60f, rect.height), DialogueEventTypesIndex, DialogueEventTypes.Select(t => t.Name).ToArray());
                        ReorderableList.defaultBehaviours.DrawFooter(rect, reorderable);


                    };
                    _reorderableList = reorderable;
                }
                return _reorderableList;
            }
        }
        public static int DialogueEventTypesIndex;
        static Type[] _DialogueEventTypes;
        public static DialogueEvent NowEvent;
        public static Type[] DialogueEventTypes
        {
            get
            {
                if(_DialogueEventTypes == null)
                {
                    _DialogueEventTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => !t.IsAbstract && typeof(DialogueEvent).IsAssignableFrom(t)&&t.Name!=nameof(DialogueEvent)
                        ).ToArray();


                }
                return _DialogueEventTypes;
            }
        }
        [InitializeOnLoadMethod]
        public static void RefreshDialogueEventTypes_Set()
        {
            CompilationPipeline.assemblyCompilationFinished -= RefreshDialogueEventTypes;
            CompilationPipeline.assemblyCompilationFinished += RefreshDialogueEventTypes;
        }

        public static void RefreshDialogueEventTypes(string s, CompilerMessage[] ms)
        {
            _DialogueEventTypes = null;
        }
        public static string LocalizationIndexToName(int index)
        {
            switch(index)
            {
                case 1:
                    return ScriptLocalization.StoryNames.Azar;
                case 2:
                    return ScriptLocalization.StoryNames.Lian;
                case 3:
                    return ScriptLocalization.StoryNames.Joey;
                case 4:
                    return ScriptLocalization.StoryNames.Clyne;
                case 6:
                    return ScriptLocalization.StoryNames.Lucy;
                case 7:
                    return ScriptLocalization.StoryNames.Haru;
                case 8:
                    return ScriptLocalization.StoryNames.Ilya;
                case 9:
                    return ScriptLocalization.StoryNames.Merlant;
                case 10:
                    return ScriptLocalization.StoryNames.Silverstein;
                case 11:
                    return ScriptLocalization.StoryNames.Annie;
                case 12:
                    return ScriptLocalization.StoryNames.Leryn;
                case 13:
                    return ScriptLocalization.StoryNames.Phoenix;
                case 14:
                    return ScriptLocalization.StoryNames.Narhan;
                case 15:
                    return ScriptLocalization.StoryNames.Sizz;
                case 16:
                    return ScriptLocalization.StoryNames.Bebe;
                case 17:
                    return ScriptLocalization.StoryNames.Hein;
                case 18:
                    return ScriptLocalization.StoryNames.Trisha;
                case 19:
                    return ScriptLocalization.StoryNames.Pressel;
                case 20:
                    return ScriptLocalization.StoryNames.King;
                case 21:
                    return ScriptLocalization.StoryNames.Chiyo;
                case 22:
                    return ScriptLocalization.StoryNames.Charon;
                case 23:
                    return ScriptLocalization.StoryNames.Miss;
                case 24:
                    return ScriptLocalization.StoryNames.Iron;
                case 25:
                    return ScriptLocalization.StoryNames.ProgramMaster;
                case 26:
                    return ScriptLocalization.StoryNames.Momori;
                default:
                    return "";
            }
        }
    }
}
 