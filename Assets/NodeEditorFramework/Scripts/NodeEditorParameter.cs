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
        [SerializeField] private string m_Name;
        public string Name => m_Name;

        [SerializeField] private ParameterType m_Type;
        public ParameterType Type => m_Type;

        [SerializeField] private object m_Value;
        public object Value => m_Value;

        public NodeEditorParameter(ParameterType type, object value, string name)
        {
            m_Name = name;
            m_Type = type;
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
            //GUILayout.Label(Name);
            if (CheckCanUseName(newName))
            {
                NodeEditor.Instance.LoadedNodeCanvas.ChangeParametersName(m_Name, newName);
                m_Name = newName;
            }

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