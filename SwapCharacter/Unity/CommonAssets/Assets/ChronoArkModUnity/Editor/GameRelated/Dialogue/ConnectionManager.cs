using UnityEngine;
using UnityEditor;
using Dialogical;

namespace ChronoArkMod.InUnity.Dialogue
{
    public static class ConnectionManager
    {
        public static  BaseNodePack m_selectedLeftNode;
        public static InstantiableNodePack m_selectedRightNode;
        public static int m_selectedOutput;

       

        public static void CreateConnection()
        {

            m_selectedLeftNode.CreateConnection(m_selectedOutput, m_selectedRightNode);
            
        }
        public static void CreateLinearConnection(BaseNodePack leftNode, InstantiableNodePack rightNode)
        {

            m_selectedLeftNode = leftNode;
            m_selectedRightNode = rightNode;
            m_selectedOutput = 0;
            CreateConnection();
            ClearConnectionSelection();
        }
        public static void ClearConnectionSelection()
        {
            m_selectedLeftNode = null;
            m_selectedRightNode = null;
        }
        public static void OnClickInput(InstantiableNodePack node)
        {
            m_selectedRightNode = node;
            if (m_selectedLeftNode != null)
            {
                CreateConnection();

                ClearConnectionSelection();
            }
        }

        public static void OnClickOutput(BaseNodePack node, int outputIndex)
        {
            m_selectedLeftNode = node;
            m_selectedOutput = outputIndex;
            if (m_selectedRightNode != null)
            {
                CreateConnection();


                ClearConnectionSelection(); 
            }
        }


        public static void DrawBezierToMouse()
        {
            Color colour = Color.white;

            if (m_selectedLeftNode != null)
            {
                
                Vector3 startPos = m_selectedLeftNode.m_outputPoints[m_selectedOutput].center;
                Vector3 endPos = DialogueEditorWindow.MousePosition;
                Vector3 startTangent = startPos + Vector3.right * 50;
                Vector3 endTangent = endPos + Vector3.left * 50;
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 5);
            }
            else if (m_selectedRightNode != null)
            {
                
                Vector3 startPos = DialogueEditorWindow.MousePosition;
                Vector3 endPos = m_selectedRightNode.m_inputPoint.center;
                Vector3 startTangent = startPos + Vector3.right * 50;
                Vector3 endTangent = endPos + Vector3.left * 50;
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 5);
            }
        }
    }
}