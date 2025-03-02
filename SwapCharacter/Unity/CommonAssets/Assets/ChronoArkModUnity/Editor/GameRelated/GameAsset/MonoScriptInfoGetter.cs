
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChronoArkMod.InUnity
{
    public class MonoScriptInfoGetter : ScriptableObject
    {
        public const string MonoScriptInfoGetterPath = "Assets/ChronoArkModUnity/MonoScriptInfoGetter.asset";
        public List<string> ImportedScriptStrings = new List<string>();
        public List<MonoScript> ImportedScripts = new List<MonoScript>();
        static bool IsDerivedFrom(Type derivedType, Type baseType)
        {
            if (derivedType == baseType)
                return true;

            if (derivedType.BaseType == null)
                return false;

            if (derivedType.BaseType == baseType)
                return true;

            return IsDerivedFrom(derivedType.BaseType, baseType);
        }
        public struct ImportedScriptInfo
        {
            public string ClassName;
            public string AssemblyName;
            public string Namespace;
            public string ID;
            public ImportedScriptInfo(MonoScript script,string id)
            {
                ID = id;
                var t =  script.GetClass();
                AssemblyName = t.Assembly.GetName().Name;
                Namespace = t.Namespace??"";
                ClassName = t.Name;
            }
        }
        [MenuItem("ChronoArk/Game Asset/Export Script Infomation")]
        public static MonoScriptInfoGetter MonoScriptInfoGet() 
        {
            
            var getter = ScriptableObject.CreateInstance<MonoScriptInfoGetter>();
            GameObject gameObject_tempalign = new GameObject("Align");
            
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => IsDerivedFrom(t,typeof(Component)) && !t.IsAbstract))
            {
                try
                {
                    GameObject gameObject_temp = new GameObject(type.Name);
                    var component = gameObject_temp.AddComponent(type);
                    gameObject_temp.transform.parent = gameObject_tempalign.transform;
                    var mono = typeof(MonoScript).GetMethod("FromScriptedObject", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { component }) as MonoScript;
                    if (mono != null)
                    {
                        getter.ImportedScripts.Add(mono);
                    }
                    DestroyImmediate(gameObject_temp);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log(type.Name);
                }
            }
            DestroyImmediate(gameObject_tempalign);
            getter.ImportedScripts.Sort((a,b)=>string.Compare(a.GetClass().Name,b.GetClass().Name)); 
            AssetDatabase.CreateAsset(getter, MonoScriptInfoGetterPath);
            string file =  File.ReadAllText(MonoScriptInfoGetterPath);
            file = file.Substring(file.IndexOf("ImportedScripts"));
            getter.ImportedScriptStrings = file.Split('\n').Select(s=>s.Trim()).Where(s=>s.StartsWith("- ")).Select(s=>s.Replace("- ","")).ToList();
            List<ImportedScriptInfo> list = new List<ImportedScriptInfo>();
            for (int i = 0; i < getter.ImportedScripts.Count;i++)
            {
                list.Add(new ImportedScriptInfo(getter.ImportedScripts[i], getter.ImportedScriptStrings[i]));
            }
            File.WriteAllText("Assets/ChronoArkModUnity/MonoScriptInfo.json", JsonConvert.SerializeObject(list));
            return getter;

        }

    }
}
