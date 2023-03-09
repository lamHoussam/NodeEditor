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

        public void SetNodeConnection(Node node, NodeConnectionType type)
        {
            m_Node = node;
            m_Rect = new Rect(0, 0, 10f, 20f);

            m_ConnectionType = type;

            //m_Rect = new Rect(m_Node.Position.x,
            //                 m_Node.Position.y + labelRect.y,
            //                 labelRect.width + labelRect.x,
            //                 labelRect.height);
        }

        public void Draw()
        {
            m_Rect.y = m_Node.Position.y + (m_Node.Height * 0.5f) - m_Rect.height * 0.5f;

            switch (m_ConnectionType)
            {
                case NodeConnectionType.In:
                    m_Rect.x = m_Node.Position.x - m_Rect.width + 8f;
                    break;

                case NodeConnectionType.Out:
                    m_Rect.x = m_Node.Position.x + m_Node.Width - 8f;
                    break;
            }

            if (GUI.Button(m_Rect, "", /*m_ConnectionType == NodeConnectionType.In ? NodeEditor.Instance.InStyle :*/NodeEditor.Instance.m_KnobStyle))
            {
                //if (m_ConnectionType == NodeConnectionType.Out)
                //    NodeEditor.Instance.OnClickOutPoint(this);
                //else
                //    NodeEditor.Instance.OnClickInPoint(this);
            }
        }

        public void Drag(Vector2 delta)
        {
            m_Rect.position += delta;
        }


    }
}