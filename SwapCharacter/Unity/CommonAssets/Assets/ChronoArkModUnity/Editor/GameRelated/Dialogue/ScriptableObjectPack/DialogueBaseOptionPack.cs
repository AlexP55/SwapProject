
using Dialogical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ChronoArkMod.InUnity.Dialogue
{
    public class DialogueBaseOptionPack : ScriptableObjectPack<DialogueBaseOption>
    {
        private DialogueBaseOption Value => Value<DialogueBaseOption>();
        [SerializeField]
        protected BaseNodePack Parent;

        public InstantiableNodePack target;
        public virtual void Init(BaseNodePack parent)
        {
            
            Parent = parent;
            SetIndex();

        }
        public virtual void OnDestory()
        {
            DestroyImmediate(Value,true);
        }
        public int Index;
        public void SetIndex()
        {
             Index= Parent.options.IndexOf(this);
             Name = Parent.name+ "_Option"+Index;
        }
        public Rect rect;
        public virtual void DrawNodeWindow()
        {
            GUI.color = new Color(0.6f, 0.6f, 0.6f);
            if (rect.Contains(Event.current.mousePosition))
            {

                GUI.color = new Color(0.9f, 0.9f, 0.9f);
            }
            GUI.DrawTexture(new Rect(rect.x,rect.y+3,rect.width,rect.height-6), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);

            GUI.color = Color.white;

            var rect_Index = new Rect(rect.x+3, rect.y + 7.5f, 12, rect.height - 15);
            EditorGUI.LabelField(rect_Index, Index.ToString());

        }
        public virtual void ProcessContextMenu()
        {

        }
        public virtual void OnDetailGUI()
        {

        }
        
    }

}
