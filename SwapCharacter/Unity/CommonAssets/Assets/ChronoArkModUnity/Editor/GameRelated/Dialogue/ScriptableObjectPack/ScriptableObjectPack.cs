using Dialogical;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace ChronoArkMod.InUnity.Dialogue
{
    public abstract class ScriptableObjectPack : ScriptableObject
    {
        public abstract void CreateAt<T2>(string path) where T2 : ScriptableObject;
        public static TPack DialoguePack<TPack, T2>() where TPack : ScriptableObjectPack where T2 : ScriptableObject
        {
            return DialoguePack<TPack, T2>(DialogueEditorWindow.Instance.DialogueFileName);

        }
        public static TPack DialoguePack<TPack, T2>(string filename) where TPack : ScriptableObjectPack where T2 : ScriptableObject
        {
            TPack asset = AssetMisc.CreateInstanceAt<TPack>(Path.Combine(DialogueEditorWindow.DialoguePackFolderPath, filename));
            asset.CreateAt<T2>(Path.Combine(DialogueEditorWindow.DialogueFolderPath, filename));
            return asset;

        }
        public static ScriptableObject MakeEvent(Type type)
        {
            return AssetMisc.CreateInstanceAt(Path.Combine(DialogueEditorWindow.DialogueFolderPath, DialogueEditorWindow.Instance.DialogueFileName),type);

        }
        public static T MakeEvent<T>() where T : DialogueEvent
        {
            return MakeEvent<T>(DialogueEditorWindow.Instance.DialogueFileName);

        }
        public static T MakeEvent<T>(string filename) where T:DialogueEvent
        {
            T asset = AssetMisc.CreateInstanceAt<T>(Path.Combine(DialogueEditorWindow.DialogueFolderPath, filename));
            return asset;

        }
    }
    public abstract class ScriptableObjectPack<T>: ScriptableObjectPack where T : ScriptableObject
    {
        [SerializeField]
        public T m_Value;
        public T2 Value<T2>() where T2:T
        {
            return m_Value as T2;

        }
        public override void CreateAt<T2>(string path) 
        {

            var Temp = AssetMisc.CreateInstanceAt<T2>(path);
            Temp.name = name;
            m_Value = Temp as T;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }


        public virtual string Name
        {
            get
            {
                m_Value.name = name;
                return m_Value.name;
            }
            set
            {
                name = value;
                m_Value.name = value;

            }
        }
        
    }
}
