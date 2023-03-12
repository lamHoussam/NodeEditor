
using Newtonsoft.Json.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditorFramework
{
    public class TestNode : Node
    {

        private int m_value;
        public static TestNode Create(Rect rect)
        {
            TestNode node = CreateInstance<TestNode>();

            node.name = "Camera Logic";
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
            Rect boxRect = new Rect(Position, Size);

            GUI.Box(boxRect, name, m_isSelected ? NodeEditor.Instance.m_SelectedNodeBase : NodeEditor.Instance.m_NodeBase);
            base.Draw();
            //Rect rect = new Rect(Position + Vector2.up * (Height - 5), Size);
            //GUI.Box(rect, "Settings", NodeEditor.Instance.DefaultNodeStyle);

        }
    }
}