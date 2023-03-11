using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class ParametersPopup : PopupWindowContent
    {

        private List<string> m_Parameters;

        // TODO: Implement a copy function instead
        public override void OnOpen()
        {
            m_Parameters ??= new List<string>();

            for(int i = 0; i < NodeEditor.Instance.LoadedNodeCanvas.ParametersCount; i++)
                m_Parameters.Add(NodeEditor.Instance.LoadedNodeCanvas.GetParameterName(i));

            Debug.Log("Popup opened: " + this);
        }
        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Parameters", EditorStyles.boldLabel);
            for(int i = 0; i < m_Parameters.Count; i++)
            {
                EditorGUILayout.Toggle(NodeEditor.Instance.LoadedNodeCanvas.GetParameter(i).GetBool());
            }

            //toggle1 = EditorGUILayout.Toggle("Toggle 1", toggle1);
            //toggle2 = EditorGUILayout.Toggle("Toggle 2", toggle2);
            //toggle3 = EditorGUILayout.Toggle("Toggle 3", toggle3);

        }


    }
}