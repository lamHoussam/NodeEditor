using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorDebugger))]
public class EditorDebuggerEditor : Editor
{
    private EditorDebugger m_Target;
    private void OnEnable()
    {
        m_Target = target as EditorDebugger;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Crouch"))
            m_Target.Cnv.SetBool("crouch", !m_Target.Cnv.GetBool("crouch"));
        if (GUILayout.Button("Right SHoulder"))
            m_Target.Cnv.SetBool("rightShoulder", !m_Target.Cnv.GetBool("rightShoulder"));
        EditorGUILayout.EndHorizontal();
    }
}
