using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class EntryNode : Node
    {
        public static EntryNode Create(Rect rect)
        {
            EntryNode node = CreateInstance<EntryNode>();

            node.m_Rect = rect;

            node.Init();
            return node;
        }



        public override void Draw()
        {
            GUILayout.BeginArea(m_Rect, NodeEditor.Instance.m_NodeBox);
            GUILayout.Label("Entry");
            GUILayout.EndArea();

            base.Draw();
        }
    }
}