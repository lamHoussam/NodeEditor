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
                Color.white,
                null,
                2f
            );

            //if (Handles.Button((m_InPoint.Center + m_OutPoint.Center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            //    NodeEditor.Instance.OnClickRemoveConnection(this);

        }

        public void ProcessEevents(Event e)
        {

        }
    }
}