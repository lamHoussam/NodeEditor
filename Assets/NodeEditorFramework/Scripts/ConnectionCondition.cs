using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class ConnectionCondition : ScriptableObject
    {
        [SerializeField] private NodeEditorParameter m_Parameter;
        [SerializeField] private NodeEditorParameterValue m_Value;

        /// <summary>
        /// Set condition's parameter and value to check with
        /// </summary>
        /// <param name="parameter">condition's parameter</param>
        /// <param name="value">value to check</param>
        public void SetConnectionCondition(NodeEditorParameter parameter, object value)
        {
            m_Parameter = parameter;
            if(parameter.Type == ParameterType.Bool)
                m_Value.BoolValue = (bool)value;
            if(parameter.Type == ParameterType.Int)
                m_Value.IntValue = (int)value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Display condition to node editor
        /// </summary>
        public void Display()
        {
            if (m_Parameter == null)
                return;

            NodeCanvas cnv = NodeEditor.Instance.LoadedNodeCanvas;
            int paramCount = cnv.ParametersCount;

            string[] choices = new string[paramCount];
            int currentIndx = 0;

            for (int i = 0; i < paramCount; i++)
            {
                choices[i] = cnv.GetParameter(i).Name;
                if (choices[i] == m_Parameter.Name)
                    currentIndx = i;
            }


            bool verified = Evaluate();

            Color oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = verified ? NodeEditor.Instance.m_trueColor : NodeEditor.Instance.m_falseColor;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();

            int chosenParamNameIndx = EditorGUILayout.Popup(currentIndx, choices);

            m_Parameter = cnv.GetParameter(chosenParamNameIndx);

            switch (m_Parameter.Type)
            {
                case ParameterType.Bool:
                    m_Value.BoolValue = EditorGUILayout.Toggle(m_Value.BoolValue);
                    break;
                case ParameterType.Int:
                    m_Value.IntValue = EditorGUILayout.IntField(m_Value.IntValue);
                    break;
                default:
                    break;
            }

            //GUILayout.Label(Evaluate().ToString());

            GUILayout.EndHorizontal();
            if (GUILayout.Button("Remove Condition"))
            {
                NodeEditor.Instance.OnClickRemoveCondition(this);
            }

            GUILayout.EndVertical();

            GUI.backgroundColor = oldBackgroundColor;
        }
#endif

        /// <summary>
        /// Evaluate condition
        /// </summary>
        /// <returns>Equality between parameter's value and condition's value</returns>
        public bool Evaluate()
        {
            if (m_Parameter == null)
                return true;

            if (m_Parameter.Type == ParameterType.Bool)
                return m_Value.BoolValue == m_Parameter.Value.BoolValue;
            else
                return m_Value.IntValue == m_Parameter.Value.IntValue;
        }
    }
}