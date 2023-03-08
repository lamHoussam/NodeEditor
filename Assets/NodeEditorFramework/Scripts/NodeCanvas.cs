using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeCanvas : ScriptableObject
    {
        private List<Node> m_Nodes;
        public int NodeCount => m_Nodes == null ? 0 : m_Nodes.Count;
        public Node GetNode(int ind) => m_Nodes[ind];

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

        }
    }
}