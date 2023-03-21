using System.Text.RegularExpressions;
using System;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public enum ParameterType
    {
        Bool = 0,
        Int = 1,
    }

    [System.Serializable]
    public struct NodeEditorParameterValue
    {
        [SerializeField] private bool m_boolValue;
        public bool BoolValue
        {
            get { return m_boolValue; }
            set { m_boolValue = value; }
        }

        [SerializeField] private int m_intValue;
        public int IntValue
        {
            get { return m_intValue; }
            set { m_intValue = value; }
        }
    }

    [System.Serializable]
    public class NodeEditorParameter
    {
        [SerializeField] private string m_ParamName;
        [SerializeField] private ParameterType m_Type;
        [SerializeField] private NodeEditorParameterValue m_Value;

        

        #region Properties API
        public string Name => m_ParamName;
        public ParameterType Type => m_Type;
        public NodeEditorParameterValue Value => m_Value;
        #endregion
    
        //public T GetValue<T>()
        //{
        //    if(typeof)
        //}


        #region API

        public NodeEditorParameter()
        {
            m_Type = ParameterType.Bool;
            m_Value = new NodeEditorParameterValue();
            m_ParamName = "Parameter";
        }

        public NodeEditorParameter(ParameterType type, object value, string name)
        {
            m_Type = type;
            m_ParamName = name;

            if (m_Type == ParameterType.Bool)
                m_Value.BoolValue = (bool)value;
            else
                m_Value.IntValue = (int)value;
        }

        public NodeEditorParameter(int type, object value, string name)
        {
            m_Type = (ParameterType)type;
            m_ParamName = name;

            if (m_Type == ParameterType.Bool)
                m_Value.BoolValue = (bool)value;
            else
                m_Value.IntValue = (int)value;
        }


        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <returns></returns>
        /// /// <exception cref="Exception">If parameter is not boolean</exception>
        public bool GetBool()
        {
            if (Type == ParameterType.Bool)
                return m_Value.BoolValue;

            throw new System.Exception("Wrong type");
            //return Value;
        }

        /// <summary>
        /// Set boolean value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetBool(bool newValue)
        {
            if (Type == ParameterType.Bool)
            {
                m_Value.BoolValue = newValue;
                return;
            }

            throw new System.Exception("Wrong type");
        }


        /// <summary>
        /// Get integer value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetInt()
        {
            if (Type == ParameterType.Int)
                return Value.IntValue;

            throw new System.Exception("Wrong type");
        }

        /// <summary>
        /// Set integer value
        /// </summary>
        /// <param name="newValue"></param>
        /// <exception cref="Exception"></exception>
        public void SetInt(int newValue)
        {
            if (Type == ParameterType.Int)
            {
                m_Value.IntValue = newValue;
                return;
            }

            throw new System.Exception("Wrong type");
        }

        #endregion

#if UNITY_EDITOR

        #region Editor API
        /// <summary>
        /// Check if newName can be used for parameter
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool CheckCanUseName(string newName)
        {
            if (NodeEditor.Instance.LoadedNodeCanvas.ContainsParameter(newName))
                return false;

            Regex regex = new Regex(@"^[a-zA-Z_]\w*$"); // regex pattern for valid parameter name
            return regex.IsMatch(newName);
        }



        /// <summary>
        /// Display parameter to Node editor window
        /// </summary>
        /// <param name="rect"></param>
        public void Display()
        {
            //GUILayout.BeginArea(NodeEditor.Instance.m_NodeBox);

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();
            string newName = GUILayout.TextField(Name);
            if (CheckCanUseName(newName))
            {
                NodeEditor.Instance.LoadedNodeCanvas.ChangeParameterName(m_ParamName, newName);
                m_ParamName = newName;
            }

            GUILayout.FlexibleSpace();

            string[] choices = System.Enum.GetNames(typeof(ParameterType));
            int currentIndx = (int)Type;

            int choseTypeInd = EditorGUILayout.Popup(currentIndx, choices);

            m_Type = (ParameterType)choseTypeInd;

            switch (Type)
            {
                case ParameterType.Bool:
                    m_Value.BoolValue = GUILayout.Toggle(m_Value.BoolValue, new GUIContent());

                    break;
                case ParameterType.Int:
                    m_Value.IntValue = EditorGUILayout.IntField(m_Value.IntValue);
                    break;
                default:
                    break;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove Parameter"))
                NodeEditor.Instance.OnClickRemoveParameter(this);

            GUILayout.EndVertical();
        }

        public void OnRemove()
        {

        }

        #endregion
#endif
    }
}