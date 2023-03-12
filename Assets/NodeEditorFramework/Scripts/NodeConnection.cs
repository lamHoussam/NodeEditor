using System.Collections.Generic;
//using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeConnection : ScriptableObject
    {
        //private NodeConnectionPoint m_InPoint;
        //private NodeConnectionPoint m_OutPoint;

        //public NodeConnectionPoint InPoint => m_InPoint;
        //public NodeConnectionPoint OutPoint => m_OutPoint;

        private Node m_From, m_To;
        public Node From => m_From;
        public Node To => m_To;

        private List<ConnectionCondition> m_Conditions;

        public void SetNodeConnectionPoints(Node from, Node to)
        {
            //m_InPoint = inPoint;
            //m_OutPoint = outPoint;

            //m_InPoint.AddConnection(this);
            //m_OutPoint.AddConnection(this);

            m_From = from;
            m_To = to;
        }

        public void Draw()
        {
            //Handles.Draw
            //Handles.color = Color.blue;
            Handles.DrawBezier(
                m_From.Center,
                m_To.Center,
                m_From.Center + Vector2.left * 50f,
                m_To.Center - Vector2.left * 50f,
                EvaluateConditions() ? new Color(0, .5f, .8f) : Color.white,
                null,
                2f
            );

            if (Handles.Button(m_To.Center, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                Debug.Log("Houssam");

            //Handles.ArrowHandleCap(0, m_To.Center, Quaternion.identity, 8, EventType.Repaint);
            if (Handles.Button((m_From.Center + m_To.Center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                NodeEditor.Instance.OnClickNodeConnection(this);

        }


        public void DisplayConditions()
        {
            GUILayout.Label(new GUIContent("Condition"), NodeEditor.Instance.m_NodeLabelBold);
            if (GUILayout.Button("New Condition"))
                NodeEditor.Instance.OnClickAddCondition(this);

            //if (GUILayout.Button("Parameters Popup", GUILayout.Width(200)))
            //{
            //    PopupWindow.Show(GUILayoutUtility.GetLastRect(), new ParametersPopup());
            //}


            if (m_Conditions != null)
                for (int i = 0; i < m_Conditions.Count; i++)
                    m_Conditions[i].Display();
        }

        public void AddCondition(ConnectionCondition cndition)
        {
            m_Conditions ??= new List<ConnectionCondition>();
            m_Conditions.Add(cndition);

            //GUI.changed = true;
        }

        public bool EvaluateConditions()
        {
            if (m_Conditions == null)
                return false;

            for(int i = 0; i < m_Conditions.Count; i++)
                if (!m_Conditions[i].Evaluate()) 
                    return false;

            return true;
        }
    }
}