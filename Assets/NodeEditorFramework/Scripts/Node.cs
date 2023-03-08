using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace NodeEditorFramework
{
    public abstract class Node : ScriptableObject
    {
        protected Rect m_Rect = new Rect();
        public Vector2 Position => m_Rect.position;
        public Vector2 Size => m_Rect.size;
        public float Width => m_Rect.width;
        public float Height => m_Rect.height;

        private List<NodeConnection> m_InConnections = new List<NodeConnection>();
        public NodeConnection GetInConnection(int ind) => m_InConnections[ind];
        public int InConnectionsCount => m_InConnections.Count; 

        private List<NodeConnection> m_OutConnections = new List<NodeConnection>();
        public NodeConnection GetOutConnection(int ind) => m_OutConnections[ind];
        public int OutConnectionsCount => m_OutConnections.Count;

        public virtual void Draw()
        {

        }

        public virtual void OnRemove()
        {
        }

        protected void Init()
        {
            NodeEditor.Instance.LoadedNodeCanvas.AddNode(this);
            if (!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(NodeEditor.Instance.LoadedNodeCanvas)))
            {
                AssetDatabase.AddObjectToAsset(this, NodeEditor.Instance.LoadedNodeCanvas);
                for (int inCnt = 0; inCnt < m_InConnections.Count; inCnt++)
                    AssetDatabase.AddObjectToAsset(m_InConnections[inCnt], this);
                for (int outCnt = 0; outCnt < m_OutConnections.Count; outCnt++)
                    AssetDatabase.AddObjectToAsset(m_OutConnections[outCnt], this);

                //AssetDatabase.ImportAsset(Node_Editor.editor.openedCanvasPath);
                AssetDatabase.Refresh();
            }
        }

    }
}