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


            GUILayout.Label(m_Parameter.Name);
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

            GUILayout.Label(Evaluate().ToString());
            //GUILayout.EndArea();

        }

        public bool Evaluate()
        {
            if (m_Parameter == null)
                return true;
            switch (m_Parameter.Type)
            {
                case ParameterType.Bool:
                    return ((bool)m_Value) == ((bool)m_Parameter.Value);
                case ParameterType.Int:
                    return ((int)m_Value) == ((int)m_Parameter.Value);
                default:
                    break;
            }
            return true;
        }
    }
}