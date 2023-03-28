using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class SerializableHashtable
    {
        public string[] keys;
        public NodeEditorParameter[] values;

        public Hashtable ToHashtable()
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < keys.Length; i++)
            {
                ht.Add(keys[i], values[i]);
            }
            return ht;
        }

        public static SerializableHashtable FromHashtable(Hashtable ht)
        {
            SerializableHashtable sht = new SerializableHashtable();
            sht.keys = new string[ht.Keys.Count];
            sht.values = new NodeEditorParameter[ht.Values.Count];
            int i = 0;
            foreach (var key in ht.Keys)
            {
                sht.keys[i] = key.ToString();
                sht.values[i] = (NodeEditorParameter)ht[key];
                i++;
            }
            return sht;
        }
    }

    public class NodeCanvas : ScriptableObject
    {
        [SerializeField] private List<Node> m_Nodes;

        [SerializeField] private List<NodeConnection> m_NodesConnections;
        private Vector2 m_scrollPosition;

        // TODO: Optimise to use only Hashtable
        [SerializeField] private Hashtable m_Parameters;
        private List<string> m_ParametersNames;

        private Node m_LastEvaluatedNode;
        public Node LastEvaluatedNode => m_LastEvaluatedNode == default(Node) ? Entry : m_LastEvaluatedNode;

        private Hashtable m_NodesDiscoveredHashtable;

        #region Properties API
        public EntryNode Entry => NodeCount == 0 ? default : (EntryNode)m_Nodes[0];
        public int NodeCount => m_Nodes == null ? 0 : m_Nodes.Count;
        public int NodeConnectionsCount => m_NodesConnections == null ? 0 : m_NodesConnections.Count;
        public int ParametersCount => m_Parameters == null ? 0 : m_Parameters.Count;
        #endregion

        #region API
        public Node GetNode(int ind) => m_Nodes[ind];
        public NodeConnection GetNodeConnection(int ind) => m_NodesConnections[ind];

        public NodeEditorParameter GetParameter(string name) => (NodeEditorParameter)m_Parameters[name];
        public NodeEditorParameter GetParameter(int ind) => (NodeEditorParameter)(m_Parameters[m_ParametersNames[ind]]);
        public NodeEditorParameter GetFirstOrNull() => ParametersCount == 0 ? null : GetParameter(0);

        /// <summary>
        /// Get boolean value of parameter with name param
        /// </summary>
        /// <param name="param">Variable's name</param>
        /// <returns></returns>
        public bool GetBool(string param) => (bool)GetParameter(param)?.Value.BoolValue;

        /// <summary>
        /// Set boolean value of parameter with name param
        /// </summary>
        /// <param name="param">Parameter to change</param>
        /// <param name="value">New Value</param>
        public void SetBool(string param, bool value) => GetParameter(param)?.SetBool(value);

        /// <summary>
        /// Checks if parameter with name exists already
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>if exists</returns>
        public bool ContainsParameter(string name) => m_Parameters.ContainsKey(name);


        public Node EvaluateFrom(Node startingNode)
        {
            Node node = startingNode;
            Node next = node.GetNextNode();
            while (next != null)
            {
                node = next;
                next = next.GetNextNode();
            }

            m_LastEvaluatedNode = node;
            return node;
        }


        /// <summary>
        /// Evaluate conditions starting from Entry
        /// </summary>
        /// <returns>Last node such that path from Entry to node evaluates to true</returns>
        public Node Evaluate() => EvaluateFrom(Entry);


        /// <summary>
        /// Evaluate conditions starting from Last evaluated node (default Entry)
        /// </summary>
        /// <returns>Last node such that path from Last evaluated node to result node evaluates to true</returns>
        public Node EvaluateFromLastEvaluatedNode() => EvaluateFrom(LastEvaluatedNode);

        private T FindNode<T>(Node node) where T : Node
        {
            m_NodesDiscoveredHashtable[node.GetInstanceID()] = true;
            //Node next = node.GetNextNode();

            Node next = default;

            for(int i = 0; i < node.ConnectionsCount; i++)
            {
                NodeConnection cnx = node.GetConnection(i);
                //Debug.LogWarning("Node : " + cnx.To);
                bool discovered = (bool)m_NodesDiscoveredHashtable[cnx.To.GetInstanceID()];
                //Debug.LogWarning("Disc : " + discovered);
                if (cnx.EvaluateConditions() && !discovered)
                    next = cnx.To;
            }

            //// Only consider paths from Entry
            if (next == default(Node))
            {
                for(int i = 0; i < Entry.ConnectionsCount; i++)
                {
                    if (!Entry.GetConnection(i).EvaluateConditions())
                        continue;
                    Node currentNode = Entry.GetConnection(i).To;
                    if (currentNode)
                    {
                        bool discovered = (bool)m_NodesDiscoveredHashtable[currentNode.GetInstanceID()];
                        m_NodesDiscoveredHashtable[currentNode.GetInstanceID()] = true;
                        if (!discovered)
                        {
                            next = currentNode;
                            break;
                        }
                    }

                }
            }

            if (next == default(Node))
                return default(T);

            T prevRes = default(T);
            if (next.GetType() == typeof(T))
                prevRes = next as T;

            T nextUndiscovered = FindNode<T>(next);
            return nextUndiscovered == default(T) ? prevRes : nextUndiscovered;
        }


        /// <summary>
        /// Evaluate conditions starting from node to first node of type T
        /// </summary>
        /// <typeparam name="T">node's type</typeparam>
        /// <returns>Last node such that path from node to result node evaluates to true and node is of type T</returns>
        public T EvaluateFrom<T>(Node node) where T : Node
        {
            m_NodesDiscoveredHashtable?.Clear();
            m_NodesDiscoveredHashtable = new Hashtable();

            for (int i = 0; i < NodeCount; i++)
                m_NodesDiscoveredHashtable.Add(m_Nodes[i].GetInstanceID(), false);

            T result = FindNode<T>(node);
            m_LastEvaluatedNode = result;

            return result;
        }


        /// <summary>
        /// Evaluate conditions starting from Last evaluated node (if none then Enrty) to first node of type T
        /// </summary>
        /// <typeparam name="T">node's type</typeparam>
        /// <returns>Last node such that path from Last evaluated node (if none then Enrty) to node evaluates to true and node is of type T</returns>
        public T EvaluateFromLastEvaluatedNode<T>() where T : Node => EvaluateFrom<T>(LastEvaluatedNode);


        /// <summary>
        /// Evaluate conditions starting from Entry to first node of type T
        /// </summary>
        /// <typeparam name="T">node's type</typeparam>
        /// <returns>Last node such that path from Entry to node evaluates to true and node is of type T</returns>
        public T Evaluate<T>() where T : Node => EvaluateFrom<T>(Entry);

        public void SaveCanvasParameterState()
        {
            string saveFileName = name + "_params.json";

            
            string json = JsonUtility.ToJson(SerializableHashtable.FromHashtable(m_Parameters));
            string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.LogWarning(json);
            File.WriteAllText(filePath, json);
        }

        public void LoadCanvasParameterState()
        {
            string saveFileName = name + "_params.json";

            string filePath = Path.Combine(Application.persistentDataPath, saveFileName);


            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                m_Parameters = JsonUtility.FromJson<SerializableHashtable>(json).ToHashtable();
                //m_ParametersNames = (List<string>)m_Parameters.Keys;
                m_ParametersNames = new List<string>();

                foreach (string parName in m_Parameters.Keys)
                    m_ParametersNames.Add(parName);

            }
            else
            {
                Debug.Log("Save file not found.");
            }
        }

        public void DeleteCanvasParameterState()
        {
            string saveFileName = name + "_params.json";
            string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

            if (File.Exists(filePath)) 
                File.Delete(filePath);
        }

        public void ChangeParameterName(string oldName, string newName)
        {
            if (!m_Parameters.ContainsKey(oldName))
                return;

            NodeEditorParameter param = (NodeEditorParameter)m_Parameters[oldName];
            m_Parameters.Remove(oldName);
            m_Parameters.Add(newName, param);

            m_ParametersNames.Remove(oldName);
            m_ParametersNames.Add(newName);
        }

        #endregion

#if UNITY_EDITOR
        #region Editor API
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
            m_Parameters ??= new Hashtable();
            m_ParametersNames ??= new List<string>();

            if (ContainsParameter(parameter.Name))
            {
                Debug.LogError("Already have parameter with name : " + parameter.Name);
                return;
            }

            m_Parameters.Add(parameter.Name, parameter);
            m_ParametersNames.Add(parameter.Name);
        }

        public void RemoveParameter(NodeEditorParameter param)
        {
            m_Parameters?.Remove(param);
            m_ParametersNames?.Remove(param.Name);
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

        #endregion
#endif
    }
}