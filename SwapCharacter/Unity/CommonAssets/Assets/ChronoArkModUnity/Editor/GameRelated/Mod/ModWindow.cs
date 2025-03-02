using ChronoArkMod.InUnity.Dialogue;
using ChronoArkMod.ModData;
using ChronoArkMod.ModEditor;
using Dialogical;
using I2.Loc;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace ChronoArkMod.InUnity
{
    public class ModWindow : EditorWindow
    {
        public static ModWindow _Instance;
        [MenuItem("ChronoArk/Mod Window", false, -101)]
        public static void OpenEditorWindow()
        {
            _ = Instance;
        }
        public static ModWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GetWindow<ModWindow>("Mod Window", true);
                    _Instance.Init();
                }
                _Instance.Show();
                _Instance.Focus();
                return _Instance;
            }

        }
        public void Init()
        {
            Instance.minSize = new Vector2(200, 0);

        }
        public static ModInfo Info => ModProjectSetting.Setting.Info;
        public void OnGUI()
        {
            List<string> ids = new List<string>() { "NULL" };
            ids.AddRange(ModProjectSetting.ModIDs);
            int index = ids.IndexOf(ModProjectSetting.Setting.ModID);
            if (index == -1) index = 0;
            GUILayout.Label("Mod ID");
            index = EditorGUILayout.Popup(index, ids.ToArray());
            ids[0] = "";
            if(ModProjectSetting.Setting.ModID != ids[index])
            {
                ModProjectSetting.Setting.ModID = ids[index];
                if (Info != null)
                {
                    var importer =  AssetImporter.GetAtPath("Assets/ModAssets");
                    importer.assetBundleName = ModProjectSetting.Setting.ModID+"UnityAssetBundle";
                    
                }
                ModProjectSetting.Setting.RenameAssembly();
                EditorUtility.SetDirty(ModProjectSetting.Setting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.Label("");
            if (Info != null)
            {
                if(GUILayout.Button("Build AssetBundle"))
                {
                    BuildAsset();
                }
                if (GUILayout.Button("Copy Assembly"))
                {
                    ModProjectSetting.CopyModAssembly();
                }
                GUILayout.Label($"Current Language:{LocalizationManager.CurrentLanguage}");
                if(GUILayout.Button("Export LangDialogueDB.csv"))
                {
                    ExportDialogueData();
                }
            }
            if (Event.current.type == EventType.Repaint)
            {

                Rect lastRect = GUILayoutUtility.GetLastRect();
                minSize = new Vector2(minSize.x, lastRect.y + lastRect.height + 4);
            }

        }
        public void BuildAsset()
        {
            string outputfolder = Path.Combine(Info.DirectoryName, "Assets");
            if (!Directory.Exists(outputfolder))
            {
                Directory.CreateDirectory(outputfolder);
            }
            AssetDatabase.Refresh();


            BuildPipeline.BuildAssetBundles(outputfolder, BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);

        }
        static public void ExportTree(LanguageSourceData MasterFile, string TreeName, string Title, string Text, string Desc = null)
        {
            string Key = LocalizeManager.DialogueKey(TreeName, Title);
            TermData MainTermData = MasterFile.GetTermData(Key);

            int index =  MasterFile.GetLanguageIndex(LocalizationManager.CurrentLanguage);
            if (MainTermData != null)
            {
                if (MainTermData.Languages[index] != Text)
                {

                    MainTermData.Languages[index] = Text;
                }
            }
            else
            {
                MainTermData = MasterFile.AddTerm(Key, eTermType.Text);
                MainTermData.Languages[index] = Text;
            }
            if (!string.IsNullOrEmpty(Desc))
            {
                MainTermData.Description = Desc;
            }

        }
        
        static void ExportDialogueData()
        {
            
            var treeInstanceToFileName = new Dictionary<DialogueTree, string>();
            var allTrees = new List<DialogueTree>();
            string[] treeAssetGUIDs = AssetDatabase.FindAssets("t:DialogueTree", new string[] { DialogueEditorWindow.DialogueFolderPath });
            foreach (var treeAssetGUID in treeAssetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(treeAssetGUID);
                var tree = AssetDatabase.LoadAssetAtPath<DialogueTree>(assetPath);
                allTrees.Add(tree);
                treeInstanceToFileName[tree] = Path.GetFileNameWithoutExtension(assetPath);
            }

            LanguageSourceData MasterFile = ModProjectSetting.DialogueDB;



            foreach (var tree in allTrees)
            {
                foreach (var i in tree.nodes)
                {

                    if (i is DialogueNode)
                    {
                        DialogueNode Node = i as DialogueNode;



                        if (!string.IsNullOrEmpty(Node.Text[0]))
                        {
                            if (Node.NameIndex != 0)
                            {
                                ExportTree(MasterFile, tree.name, Node.title, Node.Text[0],DialogueNodePack.LocalizationIndexToName(Node.NameIndex));
                            }
                            else
                                ExportTree(MasterFile, tree.name, Node.title, Node.Text[0]);
                        }

                        if (Node.options.Count != 0)
                        {
                            for (int num = 0; num < Node.options.Count; num++)
                            {
                                if (Node.options[num] is DialogueNodeOption)
                                {
                                    DialogueNodeOption NodeOption = (Node.options[num] as DialogueNodeOption);

                                    if (!string.IsNullOrEmpty(NodeOption.Text[0]) && NodeOption.Text[0] != "Continue")
                                    {
                                        ExportTree(MasterFile, tree.name, Node.title + "_Option" + num, NodeOption.Text[0]);
                                    }

                                }
                            }
                        }
                    }
                }
                
            }

            if(ModProjectSetting.Setting.Info!=null)
            {
                Directory.CreateDirectory(Path.Combine(ModProjectSetting.Setting.Info.DirectoryName, "Localization"));
                string path = Path.Combine(ModProjectSetting.Setting.Info.DirectoryName, "Localization", "LangDialogueDB.csv");
                string contents = MasterFile.Export_CSV("", ',', true);
                File.WriteAllText(path, contents);
            }



        }


    }

}

