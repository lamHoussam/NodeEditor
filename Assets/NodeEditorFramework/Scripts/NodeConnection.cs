using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
    public enum NodeConnectionType { In, Out }

    public class NodeConnection : ScriptableObject
    {
        private Node m_Node;
        private Rect m_Rect = new Rect();

        private NodeConnectionType m_ConnectionType;
        public NodeConnectionType ConnectionType => m_ConnectionType;

        public void SetRect(Rect labelRect)
        {
            m_Rect = new Rect(m_Node.Position.x,
                             m_Node.Position.y + labelRect.y,
                             labelRect.width + labelRect.x,
                             labelRect.height);
        }

    }
}