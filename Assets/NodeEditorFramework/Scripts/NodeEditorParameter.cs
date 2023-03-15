using System;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [Serializable]
    public enum ParameterType
    {
        Bool = 0,
        Int = 1,
    }

    [System.Serializable]
    public class NodeEditorParameter : ScriptableObject
    {
        [SerializeField] private string m_ParamName;
        public string Name => m_ParamName;

        [SerializeField] private bool m_Value;
        public bool Value => m_Value;


        public bool GetBool()
        {
            return Value;
        }
        public void SetBool(bool newValue)
        {
            m_Value = newValue;
        }


        public void SetNodeEditorParameter(bool value, string name)
        {
            m_ParamName = name;
            m_Value = value;
        }


        // TODO: Add Regex
        public bool CheckCanUseName(string newName)
        {
            return !NodeEditor.Instance.LoadedNodeCanvas.ContainsParameter(newName);
        }



        public void Display(Rect rect)
        {
            GUILayout.BeginArea(rect, NodeEditor.Instance.m_NodeBox);
            string newName = GUILayout.TextField(Name);
            if (CheckCanUseName(newName))
                m_ParamName = newName;


             m_Value = EditorGUILayout.Toggle((bool)m_Value);

            GUILayout.EndArea();
        }
    }
}