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

        private Vector2 m_scrollPosition;

        public NodeEditorParameter GetParameter(string name)
        {
            for(int i = 0; i < m_Parameters.Count; i++)
                if (m_Parameters[i].Name.Equals(name))
                    return m_Parameters[i];

            return null;
        }
        public NodeEditorParameter GetParameter(int ind)
        {
            return ind >= ParametersCount ? null : m_Parameters[ind];
            //object obj = m_Parameters[ind];
            //return (NodeEditorParameter)obj;
        }

        /// <summary>
        /// Get boolean value of parameter with name param
        /// </summary>
        /// <param name="param">Variable's name</param>
        /// <returns></returns>
        public bool GetBool(string param) => (bool)GetParameter(param).Value.BoolValue;

        /// <summary>
        /// Set boolean value of parameter with name param
        /// </summary>
        /// <param name="param">Parameter to change</param>
        /// <param name="value">New Value</param>
        public void SetBool(string param, bool value) => GetParameter(param).SetBool(value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>First parameter null if no parameters</returns>
        public NodeEditorParameter GetFirst() => ParametersCount == 0 ? null : m_Parameters[0];

        /// <summary>
        /// Checks if parameter with name exists already
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>if exists</returns>
        public bool ContainsParameter(string name)
        {
            for(int i = 0; i < m_Parameters.Count; i++)
                if (m_Parameters[i].Name.Equals(name))
                    return true;

            return false;
        }
        

        /// <summary>
        /// Evaluate conditions starting from Entry
        /// </summary>
        /// <returns>Last node such that path from Entry to node evaluates to true</returns>
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

        /// <summary>
        /// Evaluate conditions starting from Entry to first node of type T
        /// </summary>
        /// <typeparam name="T">node's type</typeparam>
        /// <returns>Last node such that path from Entry to node evaluates to true and node is of type T</returns>
        public T Evaluate<T>() where T : Node
        {
            Node node = Entry;
            T next = node.GetNextNode<T>();

            while (next != null)
            {
                node = next;
                next = next.GetNextNode<T>();
            }

            return node == Entry ? default : (T)node;
        }


#if UNITY_EDITOR
        /// <summary>
        /// Add new Node
        /// </summary>
        /// <param name="node">node to add</param>
        public void AddNode(Node node)
        {
            m_Nodes ??= new List<Node>();
            m_Nodes.Add(node);
        }

        /// <summary>
        /// Remove Node
        /// </summary>
        /// <param name="node">node to remove</param>
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

        /// <summary>
        /// Add node connection
        /// </summary>
        /// <param name="nodeConnection">Node connectin to add</param>
        public void AddNodeConnection(NodeConnection nodeConnection)
        {
            m_NodesConnections ??= new List<NodeConnection>();
            m_NodesConnections.Add(nodeConnection);
        }

        /// <summary>
        /// Remove node connection
        /// </summary>
        /// <param name="nodeConnection">Node connection to remove</param>
        public void RemoveNodeConnection(NodeConnection nodeConnection) => m_NodesConnections?.Remove(nodeConnection);
        /// <summary>
        /// Add new parameter
        /// </summary>
        /// <param name="parameter">Parameter to add</param>
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

        public void RemoveParameter(NodeEditorParameter param)
        {
            if (m_Parameters != null)
                m_Parameters.Remove(param);
        }

        public void RemoveParameter(string name) => RemoveParameter(GetParameter(name));
        public void RemoveParameter(int ind) => RemoveParameter(GetParameter(ind));

        /// <summary>
        /// Display to Node editor
        /// </summary>
        /// <param name="rect"></param>
        public void DisplayParameters()
        {

            GUILayout.Label(new GUIContent("Parameters"), NodeEditor.Instance.m_NodeLabelBold);
            if (m_Parameters == null)
                return;

            GUILayout.BeginVertical(GUI.skin.box);
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, true);

            for (int i = 0; i < m_Parameters.Count; i++)
            {
                NodeEditorParameter param = GetParameter(i);
                param.Display();
                //rect.position += Vector2.up * 100;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
#endif
    }
}