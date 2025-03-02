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

    public class BaseNodePack : ScriptableObjectPack<BaseNode>
    {
        private BaseNode  Value => Value<BaseNode>();
        public DialogueTreePack ParentTree;


        public Rect m_inputPoint = new Rect(0, 0, 10, 20);
        public List<Rect> m_outputPoints = new List<Rect>();
        public Rect nodeRect;
        [SerializeField]
        public List<DialogueBaseOptionPack> options = new List<DialogueBaseOptionPack>();
        public virtual void Init(Vector2 position, DialogueTreePack parentTree)
        {
            ParentTree = parentTree;
            nodeRect = new Rect(position.x,position.y, 50, 100);

        }
        public virtual void CreateOption(bool first = false)
        {

        }
        public void AddOutputPoint()
        {
            m_outputPoints.Add(new Rect(0, 0, 10, 20));
        }

        public void Draw()
        {
            DrawInputPoint();
            DrawOutputPoints();
            DrawBeziers();
            nodeRect = GUI.Window(ParentTree.nodes.IndexOf(this), nodeRect, DrawNodeWindow,Value.title);

        }
        public virtual void DrawNodeWindow(int id)
        {

            GUI.DragWindow();
        }
        public virtual void DrawInputPoint()
        {


        }
        void DrawOutputPoints()
        {
            if (m_outputPoints.Count == 1)
            {
                Rect rectangle = m_outputPoints[0];



                rectangle.x = nodeRect.x + nodeRect.width;

                rectangle.y = nodeRect.y + nodeRect.height*0.5f - rectangle.height * 0.5f;

                m_outputPoints[0] = rectangle;

                if (GUI.Button(m_outputPoints[0], "", DialogueEditorUIInfo.Instance.m_outputStyle))
                {
                    ConnectionManager.OnClickOutput(this, 0);
                }
            }
            else
            {
                for (int i = 0; i < m_outputPoints.Count; i++)
                {
                    Rect optionRect = options[i].rect;
                    if (optionRect.y > 0)
                    {
                        Rect rectangle = m_outputPoints[i];


                        rectangle.x = nodeRect.x + nodeRect.width;

                        rectangle.y = nodeRect.y + optionRect.y + optionRect.height/2-rectangle.height/2;

                        m_outputPoints[i] = rectangle;
                    }

                     
                    if (GUI.Button(m_outputPoints[i], "", DialogueEditorUIInfo.Instance.m_outputStyle))
                    {
                        ConnectionManager.OnClickOutput(this, i);
                    }
                }
            }

            foreach(var Option in options)
            {
                Option.SetIndex();
            }
        }
        void DrawBeziers()
        {
            Color colour = Color.white;

            for (int i = 0; i < options.Count; i++)
            {

                InstantiableNodePack outputNode = options[i].target;
                CreateConnection(i, outputNode);
                if (outputNode != null )
                {
                    Vector3 startPos = m_outputPoints[i].center;
                    Vector3 endPos = outputNode.m_inputPoint.center;
                    Vector3 startTangent = startPos + Vector3.right * 50;
                    Vector3 endTangent = endPos + Vector3.left * 50;

                    Handles.color = colour;
                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Handles.color, null, 5);

                    if (Handles.Button((startPos + endPos) * 0.5f, Quaternion.identity, 8, 8, Handles.RectangleHandleCap))
                        CreateConnection(i, null);
                }

            }
        }
        public void Drag(Vector2 translation)
        {
            nodeRect.position += translation;
        }
        public void CreateConnection(int index,InstantiableNodePack target)
        {
            options[index].target = target;
            Value.options[index].target = target?.Value<InstantiableNode>();
        }
        public override string Name
        {
            get
            {
                Value.title = base.Name;
                return base.Name;
            }
            set
            {
                base.Name = value;
                Value.title = value;
            }
        }

        public void TryDeleteOption(DialogueBaseOptionPack option)
        {
            var confirm = EditorUtility.DisplayDialog(
              title: "Delete Node?",
            message: option.Name,
                 ok: "Delete",
                 cancel: "Cancel"
                 ); ;
            if (confirm)
            {
                DeleteOption(option);
            }
        }
        public void DeleteOption(DialogueBaseOptionPack option)
        {

            options.Remove(option);
            Value.options.Remove(option.m_Value);
            option.OnDestory();
            DestroyImmediate(option,true);
            m_outputPoints.RemoveAt(0);

            if (options.Count == 0)
            {
                CreateOption(true);
            }
            
        }
    }
}
