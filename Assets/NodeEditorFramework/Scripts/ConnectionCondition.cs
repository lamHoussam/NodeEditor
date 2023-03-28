using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class ConnectionCondition : ScriptableObject
    {
        [SerializeField] private string m_ParameterName;
        [SerializeField] private NodeEditorParameterValue m_Value;

        private NodeCanvas m_NodeCanvas;

        #region API
        /// <summary>
        /// Set condition's parameter and value to check with
        /// </summary>
        /// <param name="parameter">condition's parameter</param>
        /// <param name="value">value to check</param>
        public void SetConnectionCondition(NodeEditorParameter parameter, object value, NodeCanvas nodeCnv)
        {
            m_ParameterName = parameter.Name;
            if(parameter.Type == ParameterType.Bool)
                m_Value.BoolValue = (bool)value;
            if(parameter.Type == ParameterType.Int)
                m_Value.IntValue = (int)value;

            m_NodeCanvas = nodeCnv; 
        }

#if UNITY_EDITOR
        /// <summary>
        /// Display condition to node editor
        /// </summary>
        public void Display()
        {
            if (m_ParameterName == null)
                return;

            NodeCanvas cnv = NodeEditor.Instance.LoadedNodeCanvas;
            int paramCount = cnv.ParametersCount;

            string[] choices = new string[paramCount];
            int currentIndx = -1;

            for (int i = 0; i < paramCount; i++)
            {
                choices[i] = cnv.GetParameter(i).Name;
                if (choices[i] == m_ParameterName)
                    currentIndx = i;
            }

            if(currentIndx == -1)
            {
                NodeEditor.Instance.OnClickRemoveCondition(this);
                GUI.changed = true;

                return;
            }


            bool verified = Evaluate();

            Color oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = verified ? NodeEditor.Instance.m_trueColor : NodeEditor.Instance.m_falseColor;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();

            int chosenParamNameIndx = EditorGUILayout.Popup(currentIndx, choices);

            NodeEditorParameter param = cnv.GetParameter(chosenParamNameIndx);

            if (param == null)
            {
                NodeEditor.Instance.OnClickRemoveCondition(this);

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUI.changed = true;
                return;
            }
            m_ParameterName = param.Name;

            switch (param.Type)
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
            if (m_ParameterName == null)
                return true;

            NodeEditorParameter param = m_NodeCanvas.GetParameter(m_ParameterName);
            if (param.Type == ParameterType.Bool)
                return m_Value.BoolValue == param.Value.BoolValue;
            else
                return m_Value.IntValue == param.Value.IntValue;
        }

        #endregion
    }
}