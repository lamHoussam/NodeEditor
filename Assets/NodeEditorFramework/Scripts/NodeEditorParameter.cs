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

        [SerializeField] private ParameterType m_Type;
        public ParameterType Type => m_Type;

        [SerializeField] private object m_Value;
        public object Value => m_Value;


        //public T GetValue<T>()
        //{
        //    if(typeof)
        //}


        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <returns></returns>
        /// /// <exception cref="Exception">If parameter is not boolean</exception>
        public bool GetBool()
        {
            if (Type == ParameterType.Bool)
                return (bool)Value;

            throw new Exception("Wrong type");
            //return Value;
        }

        /// <summary>
        /// Set boolean value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetBool(bool newValue)
        {
            if (Type == ParameterType.Bool)
                m_Value = newValue;

            throw new Exception("Wrong type");
        }


        /// <summary>
        /// Get integer value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetInt()
        {
            if (Type == ParameterType.Int)
                return (int)Value;

            throw new Exception("Wrong type");
        }

        /// <summary>
        /// Set integer value
        /// </summary>
        /// <param name="newValue"></param>
        /// <exception cref="Exception"></exception>
        public void SetInt(int newValue)
        {
            if (Type == ParameterType.Int)
                m_Value = newValue;

            throw new Exception("Wrong type");
        }

        /// <summary>
        /// Initialises parameter values 
        /// </summary>
        /// <param name="value">Parameter's value</param>
        /// <param name="name">Parameter's name</param>
        public void SetNodeEditorParameter(ParameterType type, bool value, string name)
        {
            m_Type = type;
            m_ParamName = name;
            m_Value = value;
        }


#if UNITY_EDITOR
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
        public void Display()
        {
            //GUILayout.BeginArea(NodeEditor.Instance.m_NodeBox);

            GUILayout.BeginHorizontal(GUI.skin.box);
            string newName = GUILayout.TextField(Name);
            if (CheckCanUseName(newName))
                m_ParamName = newName;

            GUILayout.FlexibleSpace();

            m_Value = GUILayout.Toggle((bool)m_Value, new GUIContent());
            GUILayout.EndHorizontal();

            //GUILayout.EndArea();
        }
#endif
    }
}