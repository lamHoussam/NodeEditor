using System;
using System.Runtime.InteropServices.WindowsRuntime;
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

        //[SerializeField] private ParameterType m_Type;
        //public ParameterType Type => m_Type;

        [SerializeField] private bool m_Value;
        public bool Value => m_Value;

        //public T GetValue<T>()
        //{
        //    if (typeof(T) == m_Value.GetType())
        //        return (T)m_Value;

        //    Debug.LogError("Wrong type");
        //    throw new InvalidCastException("Wrong type");
        //}

        public bool GetBool()
        {
            return Value;
            //if (m_Type == ParameterType.Bool)
            //    return (bool)m_Value;

            //Debug.LogError("Wrong type");
            //throw new InvalidCastException("Wrong type");
        }

        //public int GetInt()
        //{
        //    if (m_Type == ParameterType.Int)
        //        return (int)m_Value;

        //    Debug.LogError("Wrong type");
        //    throw new InvalidCastException("Wrong type");
        //}

        //public void SetValue<T>(T newValue)
        //{
        //    if (typeof(T) == m_Value.GetType())
        //        m_Value = newValue;

        //    Debug.LogError("Wrong type");
        //    throw new InvalidCastException("Wrong type");
        //}

        public void SetBool(bool newValue)
        {
            m_Value = newValue;
            //if (m_Type == ParameterType.Bool)
            //{
            //    m_Value = newValue;
            //    return;
            //}

            //Debug.LogError("Wrong type");
            //throw new InvalidCastException("Wrong type");
        }

        //public void SetInt(int newValue)
        //{
        //    if (m_Type == ParameterType.Int)
        //    {
        //        m_Value = newValue;
        //        return;
        //    }

        //    Debug.LogError("Wrong type");
        //    throw new InvalidCastException("Wrong type");
        //}

        public void SetNodeEditorParameter(bool value, string name)
        {
            m_ParamName = name;
            //m_Type = type;
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
                NodeEditor.Instance.LoadedNodeCanvas.ChangeParametersName(m_ParamName, newName);
                m_ParamName = newName;
            }

            //m_Type = (ParameterType)EditorGUILayout.EnumPopup(m_Type);
             m_Value = EditorGUILayout.Toggle((bool)m_Value);

            //switch (m_Type)
            //{
            //    case ParameterType.Bool:
            //        try
            //        {

            //        }
            //        catch (System.Exception)
            //        {
            //            m_Value = false;
            //        }

            //        break;
            //    //case ParameterType.Int:
            //    //    try
            //    //    {
            //    //        m_Value = EditorGUILayout.IntField((int)m_Value);
            //    //    }
            //    //    catch (System.Exception)
            //    //    {
            //    //        m_Value = 0;
            //    //    }


            //        //break;
            //    default:
            //        break;
            //}

            GUILayout.EndArea();
        }
    }
}