using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeCanvas : ScriptableObject
    {
        [SerializeField] private List<Node> m_Nodes;
        public EntryNode Entry => NodeCount == 0 ? null : (EntryNode)m_Nodes[0];
        public int NodeCount => m_Nodes == null ? 0 : m_Nodes.Count;
        public Node GetNode(int ind) => m_Nodes[ind];

        [SerializeField] private List<NodeConnection> m_NodesConnections;
        public int NodeConnectionsCount => m_NodesConnections == null ? 0 : m_NodesConnections.Count;
        public NodeConnection GetNodeConnection(int ind) => m_NodesConnections[ind];


        // TODO: Optimise to use only Hashtable
        [SerializeField] private List<NodeEditorParameter> m_Parameters;
        public int ParametersCount => m_Parameters == null ? 0 : m_Parameters.Count;
        public NodeEditorParameter GetParameter(string name)
        {
            for(int i = 0; i < m_Parameters.Count; i++)
                if (m_Parameters[i].Name.Equals(name))
                    return m_Parameters[i];

            return null;
        }
        public NodeEditorParameter GetParameter(int ind)
        {
            return m_Parameters[ind];
            //object obj = m_Parameters[ind];
            //return (NodeEditorParameter)obj;
        }

        public bool GetBool(string param) => GetParameter(param).Value;
        public void SetBool(string param, bool value) => GetParameter(param).SetBool(value);

        public NodeEditorParameter GetFirst() => ParametersCount == 0 ? null : m_Parameters[0];
        
        public void AddNode(Node node)
        {
            m_Nodes ??= new List<Node>();
            m_Nodes.Add(node);
        }
        public void RemoveNode(Node node)
        {
            if(m_NodesConnections != null)
            {
                int initCount = m_NodesConnections.Count;

                List<NodeConnection> cnxsToRemove = new List<NodeConnection>();

                for(int i = 0; i < initCount; i++)
                {
                    if (m_NodesConnections[i].From == node || m_NodesConnections[i].To == node)
                    {
                        Debug.Log(m_NodesConnections[i]);
                        cnxsToRemove.Add(m_NodesConnections[i]);
                    }
                }

                for(int i = 0; i < cnxsToRemove.Count; i++)
                    m_NodesConnections.Remove(cnxsToRemove[i]);
            }


            m_Nodes?.Remove(node);
        }

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

        public void AddNodeConnection(NodeConnection nodeConnection)
        {
            m_NodesConnections ??= new List<NodeConnection>();
            m_NodesConnections.Add(nodeConnection);
        }

        public void RemoveNodeConnection(NodeConnection nodeConnection) => m_NodesConnections?.Remove(nodeConnection);

        public void RemoveNodeConnections(ICollection<NodeConnection> connections)
        {

        }

        public Node Evaluate()
        {

            Node node = Entry;
            Node next = node.GetNextNode();
            while (next != null)
            {
                node = next;
                next = next.GetNextNode();
            }

            return node;
        }

        public void AddParameter(NodeEditorParameter parameter)
        {
            m_Parameters ??= new List<NodeEditorParameter>();

            if (ContainsParameter(parameter.Name))
            {
                Debug.LogError("Already have parameter with name : " + parameter.Name);
                return;
            }

            m_Parameters.Add(parameter);

            if (!System.String.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                AssetDatabase.AddObjectToAsset(parameter, this);
                AssetDatabase.Refresh();
            }
        }

        public bool ContainsParameter(string name)
        {
            for(int i = 0; i < m_Parameters.Count; i++)
                if (m_Parameters[i].Name.Equals(name))
                    return true;

            return false;
        }
        
        public void DisplayParameters(Rect rect)
        {
            GUILayout.Label(new GUIContent("Parameters"), NodeEditor.Instance.m_NodeLabelBold);
            if (m_Parameters == null)
                return;

            for (int i = 0; i < m_Parameters.Count; i++)
            {
                NodeEditorParameter param = (NodeEditorParameter)GetParameter(i);
                param.Display(rect);
                rect.position += Vector2.up * 100;
            }
        }
    }
}