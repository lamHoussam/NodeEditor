using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class ConnectionCondition 
    {
        [SerializeField] private NodeEditorParameter m_Parameter;
        [SerializeField] private object m_Value;

        public ConnectionCondition(NodeEditorParameter parameter, object value)
        {
            m_Parameter = parameter;
            m_Value = value;
        }

        public void Display()
        {
            if (m_Parameter == null)
                return;

            //GUILayout.BeginArea(rect, NodeEditor.Instance.m_NodeBox);
            //GUILayout.Label("Parameter");

            GUILayout.Label(m_Parameter.Name);

            //m_Type = (ParameterType)EditorGUILayout.EnumPopup(m_Type);

            switch (m_Parameter.Type)
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

            //GUILayout.EndArea();

        }

        public bool Evaluate() => m_Parameter == null || m_Value == m_Parameter.Value;
    }
}