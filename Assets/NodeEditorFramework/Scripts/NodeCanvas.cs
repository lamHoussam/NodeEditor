using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeCanvas : ScriptableObject
    {
        private List<Node> m_Nodes;
        public EntryNode Entry => NodeCount == 0 ? null : (EntryNode)m_Nodes[0];
        public int NodeCount => m_Nodes == null ? 0 : m_Nodes.Count;
        public Node GetNode(int ind) => m_Nodes[ind];

        public List<NodeConnection> m_NodesConnections;
        public int NodeConnectionsCount => m_NodesConnections == null ? 0 : m_NodesConnections.Count;
        public NodeConnection GetNodeConnection(int ind) => m_NodesConnections[ind];


        // TODO: Optimise to use only Hashtable

        private Hashtable m_Parameters;
        public int ParametersCount => m_Parameters == null ? 0 : m_Parameters.Count;
        public NodeEditorParameter GetParameter(string name) => (NodeEditorParameter)m_Parameters[name];
        public NodeEditorParameter GetParameter(int ind) => (NodeEditorParameter)m_Parameters[GetParameterName(ind)];
        public string GetParameterName(int ind) => m_ParameterNames[ind];
        private List<string> m_ParameterNames;
        //public NodeEditorParameter GetParameter(int ind) => m_Parameters[m_Parameters.Keys[ind]];
        public NodeEditorParameter GetFirst() => ParametersCount == 0 ? null : (NodeEditorParameter)m_Parameters[m_ParameterNames[0]];

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

                //cnxsToRemove = null;
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


        // TODO: Make more efficient (try to call once until name is fully changed);
        public void ChangeParametersName(string oldName, string newName)
        {
            if (!m_Parameters.ContainsKey(oldName))
                return;

            NodeEditorParameter param = (NodeEditorParameter)m_Parameters[oldName];
            m_Parameters.Remove(oldName);
            m_Parameters.Add(newName, param);

            m_ParameterNames.Remove(oldName);
            m_ParameterNames.Add(newName);
        }

        public void AddParameter(NodeEditorParameter parameter)
        {
            m_Parameters ??= new Hashtable();
            m_ParameterNames ??= new List<string>();

            if (m_Parameters.ContainsKey(parameter.Name))
            {
                Debug.LogError("Already have parameter with name : " + parameter.Name);
                return;
            }

            m_Parameters.Add(parameter.Name, parameter);
            m_ParameterNames.Add(parameter.Name);

            //m_Parameters.get
        }

        public bool ContainsParameter(string name) => m_Parameters.ContainsKey(name);

        public void DisplayParameters(Rect rect)
        {
            GUILayout.Label(new GUIContent("Parameters"), NodeEditor.Instance.m_NodeLabelBold);
            if (m_Parameters == null)
                return;

            for (int i = 0; i < m_Parameters.Count; i++)
            {
                NodeEditorParameter param = (NodeEditorParameter)m_Parameters[m_ParameterNames[i]];
                param.Display(rect);
                rect.position += Vector2.up * 100;
            }
        }

        public T GetValue<T>(string name) => ((NodeEditorParameter)m_Parameters[name]).GetValue<T>();
        public bool GetBool(string name) => ((NodeEditorParameter)m_Parameters[name]).GetBool();
        public int GetInt(string name) => ((NodeEditorParameter)m_Parameters[name]).GetInt();


        public void SetValue<T>(string name, T value) => ((NodeEditorParameter)m_Parameters[name]).SetValue<T>(value);
        public void SetBool(string name, bool value) => ((NodeEditorParameter)m_Parameters[name]).SetBool(value);
        public void SetInt(string name, int value) => ((NodeEditorParameter)m_Parameters[name]).SetInt(value);
    }
}