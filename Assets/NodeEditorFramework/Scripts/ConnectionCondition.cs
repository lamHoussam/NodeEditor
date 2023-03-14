using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class ConnectionCondition : ScriptableObject
    {
        [SerializeField] private NodeEditorParameter m_Parameter;
        [SerializeField] private bool m_Value;

        public void SetConnectionCondition(NodeEditorParameter parameter, bool value)
        {
            m_Parameter = parameter;
            m_Value = value;
        }

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

            int chosenParamNameIndx = EditorGUILayout.Popup(currentIndx, choices);

            m_Parameter = cnv.GetParameter(chosenParamNameIndx);

                        m_Value = EditorGUILayout.Toggle((bool)m_Value);
            //switch (m_Parameter.Type)
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

            GUILayout.Label(Evaluate().ToString());

            if (GUILayout.Button("Remove Condition"))
            {
                NodeEditor.Instance.OnClickRemoveCondition(this);
            }
        }

        public bool Evaluate()
        {
            if (m_Parameter == null)
                return true;

            return ((bool)m_Value) == ((bool)m_Parameter.Value);
            //switch (m_Parameter.Type)
            //{
            //    case ParameterType.Bool:
            //    //case ParameterType.Int:
            //    //    return ((int)m_Value) == ((int)m_Parameter.Value);
            //    default:
            //        break;
            //}
            //return true;
        }
    }
}