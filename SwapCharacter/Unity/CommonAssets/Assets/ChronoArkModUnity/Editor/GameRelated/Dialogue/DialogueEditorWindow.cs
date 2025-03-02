using Dialogical;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
namespace ChronoArkMod.InUnity.Dialogue
{
    public class DialogueEditorWindow : EditorWindow
    {
        public static DialogueEditorWindow _Instance;
        [MenuItem("ChronoArk/Dialugue/Editor Window", false, -102)]
        public static void OpenEditorWindow()
        {
            Instance.Show();
            Instance.Focus();
            DialogueDetailWindow.OpenWindow();
        }
        public static DialogueEditorWindow Instance
        {
            get
            {
                if( _Instance == null)
                {
                    _Instance = GetWindow<DialogueEditorWindow>("Dialogue Editor", true);
                    _Instance.Init();
                    _Instance.Show();
                    _Instance.Focus();
                }

                return _Instance;
            }

        }
        public static Vector2 MousePosition;
        public void Init()
        {
            Instance.minSize = new Vector2(800, 600);

        }


        public DialogueTreePack Tree;
        public void OnGUI()
        {
            GUIHead();
            DrawGrid(20, Color.grey, 0.2f);
            DrawGrid(100, Color.grey, 0.4f);
            if (Tree == null) return;


            
            BeginWindows();
            MousePosition = Event.current.mousePosition;
            ConnectionManager.DrawBezierToMouse();
            for(int i = 0; i < Tree.nodes.Count; i++)
            {
                Tree.nodes[i].Draw();
            }

            EndWindows();


            ProcessEvents();
            EditorUtility.SetDirty(Tree);
            Repaint();
        }
        public void GUIHead()
        {
            EditorGUILayout.BeginHorizontal();
            Tree = EditorGUILayout.ObjectField(Tree, typeof(DialogueTreePack), false,GUILayout.Width(200)) as DialogueTreePack;
            EditorGUILayout.LabelField("Dialogue Tree Name",new GUIStyle() { fontStyle=FontStyle.Bold}, GUILayout.Width(130));
            
            if (Tree == null)
            {
                
                DialogueNameToCreate = EditorGUILayout.TextField(DialogueNameToCreate, GUILayout.Width(200));
                if (GUILayout.Button("Create New Dialogue", GUILayout.Width(200)))
                {
                    CreateNew();
                }
            }
            else
            {
                DialogueNameToCreate = "";
                
                EditorGUILayout.LabelField(Tree.Name, GUILayout.Width(200));
                if (GUILayout.Button("Locate Start Node", GUILayout.Width(200)))
                {
                    LocateStartNode();
                }
            }
            GUILayout.EndHorizontal();
            
        }
        public string DialogueNameToCreate  = "";
        public string DialogueFileName=> Path.GetFileName(AssetDatabase.GetAssetPath(Tree));

        public static string DialoguePackFolderPath = "Assets/ChronoArkModUnity/ModDialogues";
        public static string DialogueFolderPath = "Assets/ModAssets/ModDialogues";

        public void CreateNew()
        {
            if (string.IsNullOrEmpty(DialogueNameToCreate)) return;
            Directory.CreateDirectory(DialoguePackFolderPath);
            Directory.CreateDirectory(DialogueFolderPath);
            string FileName = DialogueNameToCreate + ".asset";
            string pathpack = Path.Combine(DialoguePackFolderPath, FileName);
            string path = Path.Combine(DialogueFolderPath, FileName);
            if (File.Exists(path)||File.Exists(pathpack))
            {
                var still = EditorUtility.DisplayDialog(
              title: "Tree Exists",
            message: path+"\n"+pathpack,
                 ok: "Delete&Create",
                 cancel:"Cancel"
                 );
                if (still)
                {
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.DeleteAsset(pathpack);
                }
                else
                {
                    return;
                }
            }

            Tree = ScriptableObjectPack.DialoguePack<DialogueTreePack, DialogueTree>(FileName);
            Tree.Name = DialogueNameToCreate;
            DialogueNameToCreate = "";


            Tree.CreateStartNode(position.size/3);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        private Vector2 m_offset;
        private Vector2 m_drag;
        private void ProcessEvents()
        {
            Event current = Event.current; 
            m_drag = Vector2.zero;

            switch (current.type)
            {
                case EventType.MouseDown: 

                    if (current.button == (int)MouseButton.LeftMouse)
                    {
                        ConnectionManager.ClearConnectionSelection();
                        GUI.FocusControl(null); 
                    }

                    if (current.button == (int)MouseButton.RightMouse)
                        ProcessContextMenu();

                    break;

                case EventType.MouseDrag:
                    DragNodes(current.delta);




                    break;
            }
        }
        void DragNodes(Vector2 delta)
        {
            List<BaseNodePack> nodes = Tree.nodes;
            m_drag = delta;

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Drag(delta);

        }
        public void LocateStartNode()
        {
            if (Tree != null&& Tree.nodes.Count > 0)
            {
                DragNodes(position.size/3-Tree.nodes[0].nodeRect.position);
            }
        }
        private void DrawGrid(float spacing, Color colour, float transparency)
        {
            int widthDivs = Mathf.CeilToInt(Screen.width / spacing);
            int heightDivs = Mathf.CeilToInt(Screen.height / spacing);


            Handles.BeginGUI();
            Handles.color = new Color(colour.r, colour.g, colour.b, transparency);

            m_offset += m_drag * 0.5f; 
            Vector3 newOffset = new Vector3(m_offset.x % spacing, m_offset.y % spacing, 0);


            for (int i = 0; i <= widthDivs; i++)
                Handles.DrawLine(new Vector3(spacing * i, -spacing, 0) + newOffset,
                                 new Vector3(spacing * i, Screen.height + spacing, 0f) + newOffset);


            for (int j = 0; j <= heightDivs; j++)
                Handles.DrawLine(new Vector3(-spacing, spacing * j, 0) + newOffset,
                                 new Vector3(Screen.width + spacing, spacing * j, 0f) + newOffset);


            Handles.color = Color.white;
            Handles.EndGUI();
        }
        void ProcessContextMenu()
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Create/Dialogue Node"), false, () => Tree.CreateNode(MousePosition));
            contextMenu.ShowAsContext();
        }
    }
}

