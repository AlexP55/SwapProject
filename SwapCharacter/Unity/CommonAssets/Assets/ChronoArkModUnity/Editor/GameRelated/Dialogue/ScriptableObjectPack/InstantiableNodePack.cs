using ChronoArkMod.InUnity.Dialogue;
using Dialogical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue
{
    public class InstantiableNodePack : BaseNodePack
    {
        public override void Init(Vector2 position, DialogueTreePack parentTree)
        {
            base.Init(position, parentTree);
            nodeRect.width = 100;
            nodeRect.height = 50;
        }
        private InstantiableNode Value => Value<InstantiableNode>();

        public override void DrawInputPoint()
        {
            base.DrawInputPoint();
            float xOffset = nodeRect.x - m_inputPoint.width;
            m_inputPoint.x = xOffset;

            float yOffset = nodeRect.height * 0.5f;
            m_inputPoint.y = nodeRect.y + yOffset - m_inputPoint.height * 0.5f;
            if (GUI.Button(m_inputPoint, "", DialogueEditorUIInfo.Instance.m_inputStyle))
            {
                ConnectionManager.OnClickInput(this);
            }
        }
        public override void DrawNodeWindow(int id)
        {

            EditorGUILayout.LabelField("Options");
            for(int i = 0;i<options.Count;i++)
            {
                var option = options[i];    
                option.rect = GUILayoutUtility.GetRect(nodeRect.width, 30);


                option.DrawNodeWindow();


            }
            Rect buttonRect = GUILayoutUtility.GetRect(28, 16);
            if (GUI.Button(buttonRect, "Add Option"))
                CreateOption();
            if (Event.current.type == EventType.Repaint)
            {
                
                Rect lastRect = GUILayoutUtility.GetLastRect();
                nodeRect.height = lastRect.y + lastRect.height + 4;
            }

            ProcessEvents(Event.current);
            if (GUI.Button(new Rect(nodeRect.width - 20, 0, 20, 15), "X"))
            {
                ParentTree.TryDeleteNode(this);
            }
                
            base.DrawNodeWindow(id);
        }
        public virtual void ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:

                    if(e.button == 0)
                    {
                        if(ConnectionManager.m_selectedLeftNode != null)
                        {
                            ConnectionManager.OnClickInput(this);
                            Event.current.Use();
                        }
                        bool isOption = false;
                        for (int i = 0; i < options.Count; i++)
                        {
                            var option = options[i];
                            if (option.rect.Contains(e.mousePosition))
                            {
                                DialogueDetailWindow.ActiveObject = option;
                                isOption = true;
                            }
                        }
                        if(!isOption)
                            DialogueDetailWindow.ActiveObject = this;
                        if (e.clickCount == 2)
                        {
                            DialogueDetailWindow.OpenWindow();
                        }
                    }
                    if (e.button == 1)
                    {
                        bool isOption = false;
                        for (int i = 0; i < options.Count; i++)
                        {
                            var option = options[i];
                            if (option.rect.Contains(e.mousePosition))
                            {
                                option.ProcessContextMenu();
                                isOption = true;
                                
                            }
                        }
                        if (!isOption)
                            ProcessContextMenu(); 
                        e.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete)
                        ParentTree.TryDeleteNode(this);

                    break;
            }
        }
        public virtual void OnDestory()
        {
            
            foreach(var Option in options)
            {
                
                Option.OnDestory();
            }
            foreach (var Option in options)
            {
                DestroyImmediate(Option, true);
            }
            DestroyImmediate(Value, true);
        }
        public virtual void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent("Remove Node"), false, () => ParentTree.TryDeleteNode(this));
           
            if (options.Count == 1)
            {
                genericMenu.AddItem(new GUIContent("Next Node"), false, () =>
                {
                    ConnectionManager.OnClickOutput(this, 0);
                    
                    var New =  ParentTree.CreateNode(nodeRect.position + new Vector2(nodeRect.width + 30, 0));
                    ConnectionManager.OnClickInput(New);

                });
                genericMenu.AddItem(new GUIContent("Connection Start"), false, () => ConnectionManager.OnClickOutput(this,0));
            }
            else
            {
                for(int i = 0; i < options.Count; i++)
                {
                    int j = i;
                    genericMenu.AddItem(new GUIContent("Connection Start/Option "+i), false, () => ConnectionManager.OnClickOutput(this, j));
                }
            }
            
            genericMenu.ShowAsContext(); 
        }

        public virtual void OnDetailGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Title",GUILayout.Width(30));
            Name = EditorGUILayout.TextField(Name);
            GUILayout.EndHorizontal();
        }
        
    }
}
