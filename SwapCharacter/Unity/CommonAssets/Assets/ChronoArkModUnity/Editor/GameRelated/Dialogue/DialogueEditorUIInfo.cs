using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue
{
    public class DialogueEditorUIInfo 
    {
        static DialogueEditorUIInfo _Instance;
        public static DialogueEditorUIInfo Instance
        {
            get
            {
                if( _Instance == null)
                {
                    _Instance = Create();
                }
                return _Instance;
            }
        }
        public static DialogueEditorUIInfo Create()
        {
            var info = new DialogueEditorUIInfo();
            info.Load();
            return info;
        }
        public void Load()
        {
            m_inputStyle = new GUIStyle();
            m_inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            m_inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            m_inputStyle.border = new RectOffset(4, 4, 12, 12);

            m_outputStyle = new GUIStyle();
            m_outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            m_outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            m_outputStyle.border = new RectOffset(4, 4, 12, 12);
        }

        public GUIStyle m_inputStyle;
        public GUIStyle m_outputStyle;
    }
}
