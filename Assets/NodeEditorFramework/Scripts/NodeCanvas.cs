using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeCanvas : ScriptableObject
    {
        private List<Node> m_Nodes;
        public int NodeCount => m_Nodes == null ? 0 : m_Nodes.Count;
        public Node GetNode(int ind) => m_Nodes[ind];

        public List<NodeConnection> m_NodesConnections;
        public int NodeConnectionsCount => m_NodesConnections == null ? 0 : m_NodesConnections.Count;
        public NodeConnection GetNodeConnection(int ind) => m_NodesConnections[ind];

        public void AddNode(Node node)
        {
            m_Nodes ??= new List<Node>();
            m_Nodes.Add(node);
        }
        public void RemoveNode(Node node) => m_Nodes?.Remove(node);

        public void ProcessNodeEvents(Event e)
        {
            if (m_Nodes == null)
                return;

            for (int i = m_Nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = m_Nodes[i].ProcessEvents(e);

                if (guiChanged)
                    GUI.changed = true;
            }

            //for(int i = m_NodesConnections.Count - 1; i >= 0; i--)
            //{

            //}
        }

        public void AddNodeConnection(NodeConnection nodeConnection)
        {
            m_NodesConnections ??= new List<NodeConnection>();
            m_NodesConnections.Add(nodeConnection);
        }

        public void RemoveNodeConnection(NodeConnection nodeConnection) => m_NodesConnections?.Remove(nodeConnection);

    }
}