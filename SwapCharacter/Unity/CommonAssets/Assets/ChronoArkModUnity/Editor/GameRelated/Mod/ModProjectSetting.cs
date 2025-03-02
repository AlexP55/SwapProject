using ChronoArkMod;
using ChronoArkMod.InUnity;
using ChronoArkMod.ModData;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
namespace ChronoArkMod.InUnity
{
    [ExecuteAlways]
    public class ModProjectSetting:ScriptableObject
    {
        public const string ModProjectSettingPath = "Assets/ChronoArkModUnity/ModProjectSetting.asset";
        
        public static ModProjectSetting Setting
        {
            get
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ModProjectSettingPath));
                var Setting = AssetDatabase.LoadAssetAtPath<ModProjectSetting>(ModProjectSettingPath);
                if (Setting == null)
                {
                    Setting = ScriptableObject.CreateInstance<ModProjectSetting>();
                    Setting.ModID = "";
                    
                    AssetDatabase.CreateAsset(Setting, ModProjectSettingPath);
                    AssetDatabase.SaveAssets();
                }
                Setting.RenameAssembly();
                if (!Inited) Init();
                return Setting;
            }
        }

        private static string CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path.PathFix();
        }
        internal static Dictionary<string, ModInfo> Mods = new Dictionary<string, ModInfo>();
        public static List<string> ModIDs
        {
            get
            {
                if (!Inited) Init();
                return Mods.Keys.ToList();
            }
        }
        static bool Inited;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            if(Inited) return;
            Mods.Clear();
            string[] directories = Directory.GetDirectories(CheckDirectory(ChronoArkGameLocation.GameLocation.GameDataPath + "/StreamingAssets/Mod").PathFix());
            string[] directories2 = Directory.GetDirectories(CheckDirectory(string.Concat(Directory.GetParent(ChronoArkGameLocation.GameLocation.GameDataPath), "/Mod")).PathFix());
            List<string> list = new List<string>();
            list.AddRange(directories);
            list.AddRange(directories2);
            foreach (string directory in list)
            {
                ModInfo modInfo = ModManager.ReadModInfoFromDirectory(directory);
                if (modInfo != null)
                {
                    Mods[modInfo.id] = modInfo;
                } 
            }
            Directory.CreateDirectory("Assets/ModAssets");
            
            
            Inited = true;
        }
        public string ModID;
        public ModInfo Info => Mods.ContainsKey(ModID) ? Mods[ModID] : null;


         static LanguageSourceData _DialogueDB;
        public static LanguageSourceData DialogueDB
        {
            get
            {
                if(_DialogueDB == null)
                {

                    if (Setting.Info != null)
                    {
                        var Source = new LanguageSourceData();
                        string path = Path.Combine(Setting.Info.DirectoryName, "Localization", "LangDialogueDB.csv");
                        if (File.Exists(path))
                        {
                            try
                            {
                                string csvstring = LocalizationReader.ReadCSVfile(path, Encoding.UTF8);
                                Source.Import_CSV(string.Empty, csvstring, eSpreadsheetUpdateMode.Replace, ',');
                                _DialogueDB = Source;
                            }
                            catch (Exception exception)
                            {
                                Debug.LogException(exception);

                            }
                        }
                        try
                        {
                            string csvstring2 = "Key,Type,Desc,Korean,English,Japanese,Chinese,Chinese-TW [zh-tw]\n";
                            Source.Import_CSV(string.Empty, csvstring2, eSpreadsheetUpdateMode.Replace, ',');
                        }
                        catch (Exception exception2)
                        {
                            Debug.LogException(exception2);
                        }
                        _DialogueDB = Source;
                    }
                }

                return _DialogueDB;
            }
        }
        public static void LoadDialogueLocalization()
        {
            if (!Inited && !string.IsNullOrEmpty(ChronoArkGameLocation.GameLocation.GameDataPath))
            {
                Inited = true;
                var Source = new LanguageSourceData();
                string path = Path.Combine(ChronoArkGameLocation.GameLocation.GameDataPath, "StreamingAssets", "LangSystemDB.csv");
                string csvstring = LocalizationReader.ReadCSVfile(path, Encoding.UTF8);
                Source.Import_CSV(string.Empty, csvstring, eSpreadsheetUpdateMode.Replace, ',');
                LocalizationManager.InitializeIfNeeded();
                LocalizationManager.Sources.Add(Source);
                for (int i = 0; i < Source.mLanguages.Count<LanguageData>(); i++)
                {
                    Source.mLanguages[i].SetLoaded(true);
                }
                if (Source.mDictionary.Count == 0)
                {
                    Source.UpdateDictionary(true);
                }
                Source.UpdateAssetDictionary();
                LocalizationManager.LocalizeAll(true);
            }
        }
        public static void CopyModAssembly()
        {
            if (Setting.Info != null)
            {
                string DLLName = "ChronoArkMod." + ConvertToValidFileName(Setting.ModID) + ".ModAssembly.dll";
                File.Copy(Path.Combine(ChronoArkGameLocation.Constants.ScriptAssembliesPath, DLLName), Path.Combine(Setting.Info.DirectoryName, "Assemblies",DLLName),true);
            }
        }
        public void RenameAssembly()
        {
            string path = "Assets/ChronoArkModUnity/Editor/GameRelated/ChronoArkMod.GameRelated.UnityEditor.asmdef";
            string input = File.ReadAllText(path);
            string pattern = "ChronoArkMod(.*?)ModAssembly";
            string replacement;
            if (string.IsNullOrEmpty(ModID))
            {
                replacement = "ChronoArkMod.ModAssembly";
            }
            else
            {
                replacement = "ChronoArkMod." + ConvertToValidFileName(ModID) + ".ModAssembly";
            }
            Regex regex = new Regex(pattern);
            string result = regex.Replace(input, replacement);
            File.WriteAllText(path, result);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            path = "Assets/Scripts/ChronoArkMod.ModAssembly.asmdef";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, AssemblyDefTemplate);
            }
            input = File.ReadAllText(path);
            result = regex.Replace(input, replacement);
            File.WriteAllText(path, result);
        }
        public static string ConvertToValidFileName(string input)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            string validFileName = r.Replace(input, "");
            validFileName = validFileName.Replace(" ", "_");

            return validFileName;
        }
        public const string AssemblyDefTemplate =
@"{
    ""name"": ""ChronoArkMod.ModAssembly"",
    ""references"": [
        ""ChronoArkMod.UnityEditor""
    ],
    ""optionalUnityReferences"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": true,
    ""precompiledReferences"": [
        ""Assembly-CSharp.dll"",
        ""Assembly-CSharp-firstpass.dll"",
        ""Unity.Addressables.dll"",
        ""Unity.ResourceManager.dll"",
        ""Newtonsoft.Json.dll""
    ],
    ""autoReferenced"": false,
    ""defineConstraints"": [
        ""CHRONOARKLOADED""
    ]
}";
    }
}

