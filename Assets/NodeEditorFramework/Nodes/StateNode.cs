using UnityEngine;
using NodeEditorFramework;
using UnityEditor;

public class StateNode : Node
{
    [SerializeField] private TestSettings m_Settings;
    public TestSettings Settings => m_Settings;

    public static StateNode Create(Rect rect)
    {
        StateNode node = CreateInstance<StateNode>();

        node.m_Rect = rect;

        node.Init();
        return node;
    }

    public override void Draw()
    {
        GUILayout.BeginArea(m_Rect, m_isEvaluationResult ? NodeEditor.Instance.m_EvaluatedNodeResult : NodeEditor.Instance.m_NodeBox);

        GUILayout.Label(m_Settings ? m_Settings.Title : "Settings");
        m_Settings = (TestSettings)EditorGUILayout.ObjectField(m_Settings, typeof(TestSettings), false);
        GUILayout.EndArea();

        base.Draw();
    }

}
