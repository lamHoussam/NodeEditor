using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public enum ParameterType
    {
        Bool,
        Int,
    }

    [System.Serializable]
    public class NodeEditorParameter 
    {
        [SerializeField] private ParameterType m_Type;
        public ParameterType Type => m_Type;

        [SerializeField] private object m_Value;
        public object Value => m_Value;

        public NodeEditorParameter(ParameterType type, object value)
        {
            m_Type = type;
            m_Value = value;
        }

        public void Display(Rect rect)
        {
            GUILayout.BeginArea(rect, NodeEditor.Instance.m_NodeBox);
            GUILayout.Label("Parameter");

            m_Type = (ParameterType)EditorGUILayout.EnumPopup(m_Type);

            switch (m_Type)
            {
                case ParameterType.Bool:
                    try
                    {
                        m_Value = EditorGUILayout.Toggle((bool)m_Value);

                    }
                    catch (System.Exception)
                    {
                        m_Value = false;
                    }

                    break;
                case ParameterType.Int:
                    try
                    {
                        m_Value = EditorGUILayout.IntField((int)m_Value);
                    }
                    catch (System.Exception)
                    {
                        m_Value = 0;
                    }


                    break;
                default:
                    break;
            }

            GUILayout.EndArea();
        }
    }
}