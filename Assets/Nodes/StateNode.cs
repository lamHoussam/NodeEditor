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

        //node.m_InConnection = CreateInstance<NodeConnectionPoint>();
        //node.m_OutConnection = CreateInstance<NodeConnectionPoint>();

        //node.m_InConnection.SetNodeConnection(node, NodeConnectionType.In);
        //node.m_OutConnection.SetNodeConnection(node, NodeConnectionType.Out);

        //NodeInput.Create(node, "myValue", typeof(int));

        node.Init();
        return node;
    }

    public override void Draw()
    {
        GUILayout.BeginArea(m_Rect, m_isEvaluationResult ? NodeEditor.Instance.m_EvaluatedNodeResult : NodeEditor.Instance.m_NodeBox);
        GUILayout.Label("Settings");

        m_Settings = (TestSettings)EditorGUILayout.ObjectField(m_Settings, typeof(TestSettings), false);
        GUILayout.EndArea();

        base.Draw();
    }

}
