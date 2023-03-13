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
            node.m_InitialRect = rect;

            node.Init();
            return node;
        }



        public override void Draw(float scale = 1)
        {
            base.Draw(scale);

            GUILayout.BeginArea(new Rect(m_Rect.position, m_Rect.size * scale), NodeEditor.Instance.m_NodeBox);
            GUILayout.Label("Entry");
            GUILayout.EndArea();

        }
    }
}