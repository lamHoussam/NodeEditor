using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public abstract class Node : ScriptableObject
    {
        protected Rect m_Rect = new Rect();
        public Vector2 Position => m_Rect.position;
        public Vector2 Size => m_Rect.size;
        public float Width => m_Rect.width;
        public float Height => m_Rect.height;
        public Vector2 Center => m_Rect.center;

        private List<NodeConnection> m_Connections;

        //protected NodeConnectionPoint m_InConnection;
        //protected NodeConnectionPoint m_OutConnection;

        //public NodeConnectionPoint InConnection => m_InConnection;
        //public NodeConnectionPoint OutConnection => m_OutConnection;

        //private List<NodeConnection> m_InConnections = new List<NodeConnection>();
        //public NodeConnection GetInConnection(int ind) => m_InConnections[ind];
        //public int InConnectionsCount => m_InConnections.Count; 

        //private List<NodeConnection> m_OutConnections = new List<NodeConnection>();
        //public NodeConnection GetOutConnection(int ind) => m_OutConnections[ind];
        //public int OutConnectionsCount => m_OutConnections.Count;

        protected bool m_isSelected;
        protected bool m_isDragged;
        protected bool m_isEvaluationResult;

        public virtual void Draw()
        {
            //m_InConnection.Draw();
            //m_OutConnection.Draw();
        }

        public virtual void OnRemove()
        {
        }

        public virtual void OnDrag(Vector2 delta)
        {
            m_Rect.position += delta;
        }

        public void Select()
        {
            m_isSelected = true;
        }

        public void Deselect()
        {
            m_isSelected = false;
        }

        public bool ProcessEvents(Event e)
        {

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (m_Rect.Contains(e.mousePosition))
                        {
                            m_isDragged = true;
                            GUI.changed = true;

                            m_isSelected = true;
                            NodeEditor.Instance.CreateConnection(this);
                        }
                        else
                        {
                            GUI.changed = true;
                            m_isSelected = false;
                        }
                    }

                    if (e.button == 1 && m_isSelected && m_Rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }

                    break;

                case EventType.MouseUp:
                    m_isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && m_isDragged)
                    {
                        OnDrag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }


        public void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Make Connection"), false, () => OnClickMakeConnection());
            genericMenu.ShowAsContext();

        }

        public void OnClickMakeConnection()
        {
            NodeEditor.Instance.OnClickFirstNodeForConnection(this);
        }

        public void AddConnection(NodeConnection connection)
        {
            m_Connections ??= new List<NodeConnection>();
            m_Connections.Add(connection);
        }

        protected void Init()
        {
            NodeEditor.Instance.LoadedNodeCanvas.AddNode(this);
            if (!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(NodeEditor.Instance.LoadedNodeCanvas)))
            {
                AssetDatabase.AddObjectToAsset(this, NodeEditor.Instance.LoadedNodeCanvas);

                for (int i = 0; i < m_Connections.Count; i++)
                    AssetDatabase.AddObjectToAsset(m_Connections[i], this);

                //AssetDatabase.AddObjectToAsset(m_InConnection, this);
                //AssetDatabase.AddObjectToAsset(m_OutConnection, this);

                //AssetDatabase.ImportAsset(Node_Editor.editor.openedCanvasPath);
                AssetDatabase.Refresh();
            }
        }

        public Node GetNextNode()
        {
            //NodeConnection cnx = m_OutConnection.GetFirstTrueConnection();
            if (m_Connections == null)
                return null;

            for(int i = 0; i < m_Connections.Count; i++)
                if (m_Connections[i].EvaluateConditions())
                    return m_Connections[i].To;

            return null;
        }

        public void SetEvaluationResult(bool evaluationResult = true)
        {
            m_isEvaluationResult = evaluationResult;
        }
    }
}