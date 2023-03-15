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


        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <returns></returns>
        public bool GetBool()
        {
            return Value;
        }

        /// <summary>
        /// Set boolean value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetBool(bool newValue)
        {
            m_Value = newValue;
        }


        /// <summary>
        /// Initialises parameter values 
        /// </summary>
        /// <param name="value">Parameter's value</param>
        /// <param name="name">Parameter's name</param>
        public void SetNodeEditorParameter(bool value, string name)
        {
            m_ParamName = name;
            m_Value = value;
        }


        // TODO: Add Regex

        /// <summary>
        /// Check if newName can be used for parameter
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool CheckCanUseName(string newName)
        {
            return !NodeEditor.Instance.LoadedNodeCanvas.ContainsParameter(newName);
        }



        /// <summary>
        /// Display parameter to Node editor window
        /// </summary>
        /// <param name="rect"></param>
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