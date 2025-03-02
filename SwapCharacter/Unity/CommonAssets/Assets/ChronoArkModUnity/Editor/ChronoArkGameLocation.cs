using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace ChronoArkMod.InUnity
{

    using static ChronoArkGameLocation.Constants;

    [ExecuteAlways]
    public class ChronoArkGameLocation : ScriptableObject
    {
        
        public static class Constants
        {

            public const string GameLocationAssetPath = "Assets/ChronoArkModUnity/ChronoArkGameLocation.asset";
            public const string PackageFilePath = "Packages/ChronoArk";
            public const string PackagePluginsPath = "Packages/ChronoArk/Plugins";
            public const string PackageJsonPath = "Packages/ChronoArk/package.json";
            public const string TagFilePath = "ProjectSettings/TagManager.asset";
            public const string ScriptAssembliesPath = "Library/ScriptAssemblies";
            public const string SelfScriptPath = "Assets/ChronoArkModUnity/Scripts/Editor/ChronoArkGameLocation.cs";

            public const string PackageJson = @"{""name"":""chronoark"",""displayName"": ""Chrono Ark"",""version"":""0.0.0""}";
            public const string TagFile =
    @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!78 &1
TagManager:
  serializedVersion: 2
  tags:
  - BattleEnemy
  - BattleAlly
  - Particle
  - ParticleRender
  - CameraOcllusion
  - Tile
  - MonsterC
  - FieldAllyWindow
  - ItemSlot
  - Back_After
  - EventObject
  - MainUICanvas
  - BattleStop
  - 3DUICanvas
  - Tutorial
  - FieldMap
  - eventsystem
  - SkillButton
  - Isotile
  - PlayerC
  - EffectView
  - UICamera
  - SelectButton
  - MainCharEquipInven
  - DestroyMain
  - SkillWindow
  - Pictrue
  - Picture
  - ObjectLight
  layers:
  - Default
  - TransparentFX
  - Ignore Raycast
  - 
  - Water
  - UI
  - 
  - 
  - Lock
  - BackGround
  - Tile
  - Light
  - BlockLight
  - FreeLight
  - BlockTile
  - DefaultTile
  - ShadowObject
  - BattleObject
  - FieldLock
  - Lucy
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  m_SortingLayers:
  - name: Default
    uniqueID: 0
    locked: 0
  - name: Floor
    uniqueID: 2292566043
    locked: 0
  - name: Block
    uniqueID: 3111327743
    locked: 0
  - name: Shadow
    uniqueID: 2053370039
    locked: 0
  - name: Object
    uniqueID: 3843044123
    locked: 0
  - name: Fog
    uniqueID: 2679160573
    locked: 0
  - name: UI
    uniqueID: 3499016583
    locked: 0
  - name: Fade
    uniqueID: 1128343013
    locked: 0
";
            public const string AssemblyMetaDataTemplate = "fileFormatVersion: 2\r\nguid: {0}\r\nPluginImporter:\r\n  serializedVersion: 2\r\n  isExplicitlyReferenced: 1";

            public const string ChronoArkScriptingDefine = "CHRONOARKLOADED";
            
        }

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.wantsToQuit -= QuitSave;
            EditorApplication.wantsToQuit += QuitSave;
            CompilationPipeline.assemblyCompilationFinished -= CopyAssemblyCSharp;
            CompilationPipeline.assemblyCompilationFinished += CopyAssemblyCSharp;
            EditorApplication.update -= CheckScriptingDefine;
            EditorApplication.update += CheckScriptingDefine;

        }
        public static bool QuitSave()
        {
            EditorUtility.SetDirty(GameLocation);
            AssetDatabase.SaveAssets();
            return true;
        }
       
        public static void CopyAssemblyCSharp(string somevalue = null, CompilerMessage[] message = null)
        {
            string path = Path.Combine(PackageFilePath, "Assembly-CSharp.dll");
            string path2 = Path.Combine(PackageFilePath, "Assembly-CSharp-firstpass.dll");
            bool GameLoaded = File.Exists(path) && File.Exists(path2);
            if (GameLoaded)
            {
                string pathU = Path.Combine(ScriptAssembliesPath, "Assembly-CSharp.dll");
                string path2U = Path.Combine(ScriptAssembliesPath, "Assembly-CSharp-firstpass.dll");
                File.Copy(path, pathU, true);
                File.Copy(path2, path2U, true);
                File.SetLastWriteTime(pathU, DateTime.Now);
                File.SetLastWriteTime(path2U, DateTime.Now);

            }



            

            
        }

        public static void CheckScriptingDefine()
        {
            SetScriptingDefine(File.Exists(PackageJsonPath));
        }



        public static ChronoArkGameLocation GameLocation
        {
            get
            {
                Directory.CreateDirectory(Path.GetDirectoryName(GameLocationAssetPath));
                var location = AssetDatabase.LoadAssetAtPath<ChronoArkGameLocation>(GameLocationAssetPath);
                if (location == null)
                {
                    location = ScriptableObject.CreateInstance<ChronoArkGameLocation>();
                    location.GamePath = "";
                    location.GameDataPath = "";
                    AssetDatabase.CreateAsset(location, GameLocationAssetPath);
                    AssetDatabase.SaveAssets();
                }
                return location;
            }
        }
        public string GamePath;
        public string GameDataPath;
        public string ManagedFolder => Path.Combine(GameDataPath, "Managed");
        public string PluginsFolder => Path.Combine(GameDataPath, "Plugins");

        [MenuItem("ChronoArk/Load ChronoArk and Restart")]
        private static void TryLoadingGame()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (string.IsNullOrEmpty(GameLocation.GameDataPath))
            {
                if (LocateGame())
                {
                    LoadGame();
                }
            }
            else
            {
                LoadGame();
            }

        }
        [MenuItem("ChronoArk/Reset Game Location")]
        public static bool LocateGame()
        {


            var path = string.IsNullOrEmpty(GameLocation.GamePath) ? Directory.GetCurrentDirectory() : GameLocation.GamePath;
            path = EditorUtility.OpenFilePanel("Choose ChronoArk Executable", path, "exe");
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            else
            {
                GameLocation.GamePath = Path.GetDirectoryName(path);
                GameLocation.GameDataPath = Path.Combine(GameLocation.GamePath, "..", "..", "ChronoArk_Data");
                if (!Directory.Exists(GameLocation.GameDataPath))
                {
                    GameLocation.GameDataPath = Path.Combine(GameLocation.GamePath, "ChronoArk_Data");
                }
                EditorUtility.SetDirty(GameLocation);
                AssetDatabase.SaveAssets();
                return true;
            }

            
        }
        public static void LoadGame()
        {


            if (!Application.unityVersion.StartsWith("2018.4.32"))
            {
                throw new System.Exception($"Unity Editor version: [{Application.unityVersion}]. ChronoArk Unity Player version: [2018.4.32f1].");
            }
            Directory.CreateDirectory(PackageFilePath);
            Directory.CreateDirectory(PackagePluginsPath);
            var PossibleConflictingAssemblies = GetPossibleConflictingAssemblies();
            AssetDatabase.StartAssetEditing();
            EditorApplication.LockReloadAssemblies();
            try
            {
                ImportFilteredAssemblies(PackageFilePath, GameLocation.ManagedFolder,PossibleConflictingAssemblies);
                ImportFilteredAssemblies(PackagePluginsPath, GameLocation.PluginsFolder, PossibleConflictingAssemblies);
            }
            catch (Exception ex) { Debug.LogException(ex); }
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.StopAssetEditing();
            File.WriteAllText(TagFilePath, TagFile);
            EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            File.WriteAllText(PackageJsonPath, PackageJson);
            AssetDatabase.Refresh();
            EditorApplication.OpenProject(Directory.GetCurrentDirectory());

        }

        public static List<string> GetPossibleConflictingAssemblies()
        {
            List<string> LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => !asm.IsDynamic)
                .Select(asm => asm.Location ?? "")
                    .Select(Path.GetFileName).ToList();
            var LoadedGameAsseblies = new List<string>()
            {
                "Assembly-CSharp.dll",
                "Assembly-CSharp-firstpass.dll",
            };
            LoadedGameAsseblies.AddRange(Directory.GetFiles(PackageFilePath).Select(Path.GetFileName));
            LoadedGameAsseblies.AddRange(Directory.GetFiles(PackagePluginsPath).Select(Path.GetFileName));
            LoadedAssemblies.RemoveAll(LoadedGameAsseblies.Contains);
            return LoadedAssemblies;
        }
        public static void ImportFilteredAssemblies(string destinationFolder, string sourceFolder,List<string> PossibleConflictingAssemblies)
        {
            if (!Directory.Exists(sourceFolder)) return;
            foreach (var assemblyPath in Directory.GetFiles(sourceFolder, "*.dll"))
            {

                string assemblyFileName = Path.GetFileName(assemblyPath);
                if (PossibleConflictingAssemblies.Contains(assemblyFileName))continue;


                var destinationPath = Path.Combine(destinationFolder, assemblyFileName);
                File.Copy(assemblyPath,destinationPath, true);



                var metaDataPath = Path.Combine(destinationFolder, $"{assemblyFileName}.meta");
                MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(assemblyFileName));
                string guid = new Guid(hash).ToString().Replace("-", "");
                File.WriteAllText(metaDataPath,string.Format(AssemblyMetaDataTemplate,guid));
            }
        }



        [MenuItem("ChronoArk/Fix Assembly",priority = 100)]
        public static void FixAssembly()
        {
            Type type = typeof(CompilationPipeline).Assembly.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
            type.GetMethod("DirtyAllScripts").Invoke(null, null);
            type.GetMethod("PollCompilation").Invoke(null, null);

 
        }

        public static void SetScriptingDefine(bool SetDefine)
        {
            
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (symbols.Contains(ChronoArkScriptingDefine))
            {
                if (!SetDefine)
                {
                    symbols = symbols.Replace(ChronoArkScriptingDefine + ";", "");
                    symbols = symbols.Replace(ChronoArkScriptingDefine, "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
                }

            }
            else
            {
                if (SetDefine)
                {
                    if (symbols.Length == 0)
                        symbols = ChronoArkScriptingDefine;
                    else if (symbols.EndsWith(";"))
                        symbols += ChronoArkScriptingDefine;
                    else
                        symbols += ";" + ChronoArkScriptingDefine;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
                }
            }

        }
    }
   

}
