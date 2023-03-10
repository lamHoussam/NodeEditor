using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeConnection : ScriptableObject
    {
        private NodeConnectionPoint m_InPoint;
        private NodeConnectionPoint m_OutPoint;

        public NodeConnectionPoint InPoint => m_InPoint;
        public NodeConnectionPoint OutPoint => m_OutPoint;

        private List<ConnectionCondition> m_Conditions;

        public void SetNodeConnectionPoints(NodeConnectionPoint inPoint, NodeConnectionPoint outPoint)
        {
            m_InPoint = inPoint;
            m_OutPoint = outPoint;
        }

        public void Draw()
        {
            Handles.DrawBezier(
                m_InPoint.Center,
                m_OutPoint.Center,
                m_InPoint.Center + Vector2.left * 50f,
                m_OutPoint.Center - Vector2.left * 50f,
                EvaluateConditions() ? new Color(0, .5f, .8f) : Color.white,
                null,
                2f
            );

            if (Handles.Button((m_InPoint.Center + m_OutPoint.Center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                NodeEditor.Instance.OnClickNodeConnection(this);

        }


        public void DisplayConditions()
        {
            GUILayout.Label(new GUIContent("Condition"), NodeEditor.Instance.m_NodeLabelBold);
            if (GUILayout.Button("New Condition"))
                NodeEditor.Instance.OnClickAddCondition(this);


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